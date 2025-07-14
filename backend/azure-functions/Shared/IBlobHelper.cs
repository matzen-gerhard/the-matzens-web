namespace azure_functions.Shared
{
    public interface IBlobHelper
    {
        Task<Uri> GetDownloadUriAsync(string blobName);
    }
}