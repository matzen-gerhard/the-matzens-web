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

    public async Task<List<StoryInfo>> GetStoriesAsync()
    {
        var containerClient = await _lazyContainerClient.Value;
        var storyMap = new Dictionary<string, StoryInfo>();

        // Blob names are of the form:
        // stories/SpiritNight/SpiritNight.htm
        // stories/SpiritNight/SpiritNight.jpg
        // stories/SpiritNight/Chapters/MovingDay.html
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: "stories/"))
        {
            var parts = blobItem.Name.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) continue; // Should not happen because of filter on "stories/".

            var storyFolder = parts[1]; // e.g., stories/SpiritNight/SpiritNight.htm -> "SpiritNight"
            if (!storyMap.TryGetValue(storyFolder, out var storyInfo))
            {
                storyInfo = new StoryInfo();
                storyMap[storyFolder] = storyInfo;
            }

            if (parts.Length == 3) // Items in story folder
            {
                if (blobItem.Name.EndsWith(".htm", StringComparison.OrdinalIgnoreCase))
                {
                    storyInfo.Html = await GetHtmlBlobAsync(blobItem.Name);
                }
                else if (blobItem.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    var (uri, props) = await GetMediaBlobAsync(blobItem.Name);
                    storyInfo.CoverImage = uri;
                    if (props.Metadata.TryGetValue("Title", out var title))
                    {
                        storyInfo.Title = title;
                    }

                    if (props.Metadata.TryGetValue("Order", out var orderString) &&
                        int.TryParse(orderString, out var order))
                    {
                        storyInfo.Order = order;
                    }
                }
            }
            else if (parts.Length > 3 && parts[2].Equals("Chapters", StringComparison.OrdinalIgnoreCase))
            {
                // e.g., stories/SpiritNight/Chapters/Chapter1.htm
                if (blobItem.Name.EndsWith(".htm", StringComparison.OrdinalIgnoreCase) || 
                    blobItem.Name.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                {
                    var chapterTitle = Path.GetFileNameWithoutExtension(blobItem.Name);
                    var blobClient = containerClient.GetBlobClient(blobItem.Name);
                    storyInfo.Chapters.Add(new ChapterInfo
                    {
                        Title = chapterTitle,
                        HtmlUri = blobClient.Uri,
                    });
                }
            }

            storyInfo.Order ??= int.MaxValue;
            storyInfo.Title ??= storyFolder;
        }

        return [.. storyMap.Values
            .Where(s => s.CoverImage != null && !string.IsNullOrEmpty(s.Html))
            .OrderBy(s => s.Title)];

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
