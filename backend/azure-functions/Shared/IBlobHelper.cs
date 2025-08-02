using azure_functions.Shared.Models;

namespace azure_functions.Shared
{
    public interface IBlobHelper
    {
        Task<bool> BlobExistsAsync(string blobName);

        Task<Uri> GetDownloadUriAsync(string blobName);

        Task<List<FilmInfo>> GetFilmsAsync();

        Task<FilmDetail> GetFilmDetailAsync(string mediaBlobName);
    }
}