using azure_functions.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace azure_functions.Functions;

public class GetFilm
{
    private readonly IBlobHelper _blobHelper;
    private readonly ILogger<GetFilm> _logger;

    public GetFilm(IBlobHelper blobHelper, ILogger<GetFilm> logger)
    {
        ArgumentNullException.ThrowIfNull(blobHelper);
        ArgumentNullException.ThrowIfNull(logger);

        _blobHelper = blobHelper;
        _logger = logger;
    }

    [Function("film")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "film/{blobName}")] 
                              HttpRequest req, string blobName)
    {
        // blobName will be like "films/movie1.mp4"
        if (string.IsNullOrEmpty(blobName))
        {
            return new BadRequestObjectResult("Blob name is required");
        }

        var decodedBlobName = Uri.UnescapeDataString(blobName);
        var blobExists = await _blobHelper.BlobExistsAsync(decodedBlobName);
        if (!blobExists)
        {
            return new NotFoundObjectResult($"Blob '{decodedBlobName}' not found.");
        }

        var filmDetail = await _blobHelper.GetFilmDetailAsync(decodedBlobName);
        return new OkObjectResult(filmDetail);
    }
}