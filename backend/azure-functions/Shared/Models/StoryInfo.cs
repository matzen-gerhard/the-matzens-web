namespace azure_functions.Shared.Models
{
    public class StoryInfo
    {
        public string? Title { get; set; }
        public int? Order { get; set; }
        public Uri? CoverImage { get; set; }
        public string? Html { get; set; } // HTML content (not URI)
        public List<ChapterInfo> Chapters { get; set; } = [];
    }
}
