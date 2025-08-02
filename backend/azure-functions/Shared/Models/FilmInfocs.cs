namespace azure_functions.Shared.Models
{
    public class FilmInfo
    {
        public string? Title { get; set; }
        public string? Media { get; set; } // Blob name for media file
        public string? Html { get; set; }  // Blob name for HTML file (optional)
    }
}
