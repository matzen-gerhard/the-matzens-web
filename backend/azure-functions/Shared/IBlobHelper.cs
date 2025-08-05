using azure_functions.Shared.Models;

namespace azure_functions.Shared
{
    public interface IBlobHelper
    {
        Task<List<FilmInfo>> GetFilmsAsync();

        Task<List<StoryInfo>> GetStoriesAsync();
    }
}