using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
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

    public async Task<Uri> GetDownloadUriAsync(string blobName)
    {
        var containerClient = await _lazyContainerClient.Value;
        var blobClient = containerClient.GetBlobClient(blobName);
        return blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(TokenTimeoutMinutes));
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
                filmInfo.Html = blobItem.Name;
            }
            else
            {
                filmInfo.Media = blobItem.Name; // assume any non-htm is media
            }

            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            var props = await blobClient.GetPropertiesAsync();
            if (props.Value.Metadata.TryGetValue("Title", out var title))
            {
                filmInfo.Title = title;
            }

            filmInfo.Title ??= nameWithoutExt;
        }

        return [.. filmMap.Values.Where(f => f.Media != null)];
    }

    public async Task<bool> BlobExistsAsync(string blobName)
    {
        var containerClient = await _lazyContainerClient.Value;
        var blobClient = containerClient.GetBlobClient(blobName);
        return await blobClient.ExistsAsync();
    }

    public async Task<FilmDetail> GetFilmDetailAsync(string mediaBlob)
    {
        ArgumentNullException.ThrowIfNull(mediaBlob);

        var containerClient = await _lazyContainerClient.Value;

        var baseName = Path.GetFileNameWithoutExtension(mediaBlob); // e.g., movie1

        var htmlBlob = $"films/{baseName}.htm";

        // Generate SAS for media
        var mediaUrl = await GetDownloadUriAsync(mediaBlob);

        // Check for HTML file
        Uri? htmlUrl = null;
        if (await BlobExistsAsync(htmlBlob))
        {
            htmlUrl = await GetDownloadUriAsync(htmlBlob);
        }

        return new FilmDetail
        {
            Title = baseName,
            MediaUrl = mediaUrl,
            HtmlUrl = htmlUrl
        };
    }
}
