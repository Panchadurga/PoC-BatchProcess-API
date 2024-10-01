using BatchProcess.Models;

namespace BatchProcess.Repository;

public interface ICustomerRepository
{
    Task<Customer?> GetByEmailAsync(string email);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task<int> SaveChangesAsync();
    Task<List<Customer>> BulkUploadAsync(List<Customer> customers);
}
