using BatchProcess.Models;

namespace BatchProcess.Dtos;

public class BulkUploadRequest
{
    public List<Customer>? Customers { get; set; }
}

