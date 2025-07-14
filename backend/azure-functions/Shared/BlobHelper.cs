using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace azure_functions.Shared;

public class BlobHelper : IBlobHelper
{
    private readonly int TokenTimeoutMinutes = 15;
    private readonly Lazy<Task<BlobContainerClient>> _lazyContainerClient;

    public BlobHelper(IKeyVaultSettingsProvider provider)
    {
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
}
