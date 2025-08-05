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
        var films = await _blobHelper.GetFilmsAsync();
        var stories = await _blobHelper.GetStoriesAsync();

        var content = new
        {
            films,
            stories,
        };

        return new OkObjectResult(content);
    }
}