using BatchProcess.Dtos;
using BatchProcess.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BatchProcess.Controllers;


/// <summary>
/// Controller for handling batch processing operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class BatchController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchController"/> class.
    /// </summary>
    /// <param name="customerRepository">The customer repository to use for data operations.</param>
    public BatchController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <summary>
    /// Uploads a batch of customer records.
    /// </summary>
    /// <param name="request">The bulk upload request containing customer data.</param>
    /// <returns>An IActionResult indicating the result of the bulk upload operation.</returns>
    /// <response code="200">Returns the success message and records processed.</response>
    /// <response code="400">If the request is invalid or has no customer data.</response>
    [HttpPost("bulk-upload")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkUpload([FromBody] BulkUploadRequest request)
    {
        if (request == null || request.Customers == null || !request.Customers.Any())
        {
            return BadRequest("Invalid request data.");
        }

        // Process the records and save to the database
        var results = await _customerRepository.BulkUploadAsync(request.Customers);

        return Ok(new { Message = "Bulk upload completed.", RecordsProcessed = results });
    }
}