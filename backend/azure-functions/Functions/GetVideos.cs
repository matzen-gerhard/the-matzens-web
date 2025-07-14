using azure_functions.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace azure_functions.Functions;

public class GetVideos
{
    private readonly IBlobHelper _blobHelper;
    private readonly ILogger<GetVideos> _logger;

    public GetVideos(IBlobHelper blobHelper, ILogger<GetVideos> logger)
    {
        ArgumentNullException.ThrowIfNull(blobHelper);
        ArgumentNullException.ThrowIfNull(logger);
        _blobHelper = blobHelper;
        _logger = logger;
    }

    [Function("videos")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var blobName = "some-blob-name";
        var downloadUri = await _blobHelper.GetDownloadUriAsync(blobName);
        var data = new VideoMetadata
        {
            Title = "Griffin hanging 10!",
            BlobName = blobName,
            DownloadUri = downloadUri,
        };

        List<VideoMetadata> list = [data];
        return new OkObjectResult(list);
    }
}