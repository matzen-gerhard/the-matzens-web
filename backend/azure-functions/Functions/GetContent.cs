using azure_functions.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace azure_functions.Functions;

public class GetContent
{
    private readonly IBlobHelper _blobHelper;
    private readonly ILogger<GetContent> _logger;

    public GetContent(IBlobHelper blobHelper, ILogger<GetContent> logger)
    {
        ArgumentNullException.ThrowIfNull(blobHelper);
        ArgumentNullException.ThrowIfNull(logger);

        _blobHelper = blobHelper;
        _logger = logger;
    }

    [Function("content")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var films = await _blobHelper.GetFilmsAsync(); // Add a method to return the container
 
        var content = new
        {
            films,
            stories = new List<string> { "Story1", "Story2" } // Placeholder for now
        };

        return new OkObjectResult(content);
    }
}