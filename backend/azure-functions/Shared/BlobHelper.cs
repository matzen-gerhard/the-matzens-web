using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using azure_functions.Shared.Models;
using Microsoft.Extensions.Logging;

namespace azure_functions.Shared;

public class BlobHelper : IBlobHelper
{
    private readonly int TokenTimeoutMinutes = 15;
    private readonly Lazy<Task<BlobContainerClient>> _lazyContainerClient;
    private readonly ILogger<BlobHelper> _logger;

    public BlobHelper(IKeyVaultSettingsProvider provider, ILogger<BlobHelper> logger)
    {
        _logger = logger;
        _lazyContainerClient = new Lazy<Task<BlobContainerClient>>(async () =>
        {
            var storageAccountName = await provider.GetStorageAccountNameAsync();
            var storageAccountKey = await provider.GetStorageKeyAsync();
            var containerName = await provider.GetContainerNameAsync();

            var sharedKeyCredential = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);
            var serviceUri = new Uri($"https://{storageAccountName}.blob.core.windows.net");
            var blobServiceClient = new BlobServiceClient(serviceUri, sharedKeyCredential);
            return blobServiceClient.GetBlobContainerClient(containerName);
        });
    }

    public async Task<List<FilmInfo>> GetFilmsAsync()
    {
        var containerClient = await _lazyContainerClient.Value;
        var filmMap = new Dictionary<string, FilmInfo>();


        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: "films/"))
        {
            var nameWithoutExt = Path.GetFileNameWithoutExtension(blobItem.Name);

            if (!filmMap.TryGetValue(nameWithoutExt, out FilmInfo? filmInfo))
            {
                filmInfo = new FilmInfo();
                filmMap[nameWithoutExt] = filmInfo;
            }

            if (blobItem.Name.EndsWith(".htm", StringComparison.OrdinalIgnoreCase))
            {
                var html = await GetHtmlBlobAsync(blobItem.Name);
                filmInfo.Html = html;
            }
            else
            {
                // Assume any non-htm is media
                var (uri, props) = await GetMediaBlobAsync(blobItem.Name);
                filmInfo.Media = uri;
                if (props.Metadata.TryGetValue("Title", out var title))
                {
                    filmInfo.Title = title;
                }

                if (props.Metadata.TryGetValue("Order", out var orderString) &&
                    int.TryParse(orderString, out var order))
                {
                    filmInfo.Order = order;
                }
            }

            filmInfo.Order ??= int.MaxValue;
            filmInfo.Title ??= nameWithoutExt;
        }

        return [.. filmMap.Values
                   .Where(f => f.Media != null)
                   .OrderBy(f => f.Order)];

        // Local function
        async Task<(Uri Uri, BlobProperties Properties)> GetMediaBlobAsync(string blobName)
        {
            var client = GetBlobClient(blobName);
            var props = await client.GetPropertiesAsync();
            return (client.Uri, props.Value);
        }

        async Task<string> GetHtmlBlobAsync(string blobName)
        {
            var client = GetBlobClient(blobName);
            var response = await client.DownloadContentAsync();
            return response.Value.Content.ToString();
        }

        BlobClient GetBlobClient(string blobName)
        {
            return containerClient.GetBlobClient(blobName);
        }
    }
}
