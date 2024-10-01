using BatchProcess.Data;
using BatchProcess.Models;
using Microsoft.EntityFrameworkCore;

namespace BatchProcess.Repository;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customer.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customer.AddAsync(customer);
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customer.Update(customer);
        await SaveChangesAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<List<Customer>> BulkUploadAsync(List<Customer> customers)
    {
        // Remove duplicates based on Email
        var distinctCustomers = customers
            .GroupBy(c => c.Email)
            .Select(g => g.First())
            .ToList();

        var processedCustomers = new List<Customer>();
        var newCustomers = new List<Customer>();

        // Get existing customers in one go
        var existingEmails = distinctCustomers.Select(c => c.Email).ToList();
        var existingCustomers = await _context.Customer
            .Where(c => existingEmails.Contains(c.Email))
            .ToListAsync();

        var existingEmailSet = existingCustomers.Select(c => c.Email).ToHashSet();

        foreach (var customer in distinctCustomers)
        {
            if (existingEmailSet.Contains(customer.Email))
            {
                var existingCustomer = existingCustomers.First(c => c.Email == customer.Email);
                // Update the existing customer details
                existingCustomer.CustomerName = customer.CustomerName;
                existingCustomer.Address = customer.Address;
                // Update other fields as necessary
                processedCustomers.Add(existingCustomer);
            }
            else
            {
                newCustomers.Add(customer); // Collect new customers for bulk insertion
            }
        }

        // Add new customers in bulk
        if (newCustomers.Any())
        {
            await _context.Customer.AddRangeAsync(newCustomers);
        }

        // Save all changes at once
        await SaveChangesAsync();

        return processedCustomers; // Return the list of processed customers

    }
}

