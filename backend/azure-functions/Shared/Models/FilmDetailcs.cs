namespace azure_functions.Shared.Models
{
    public class FilmDetail
    {
        public required string Title { get; set; }
        public required Uri MediaUrl { get; set; }
        public required Uri? HtmlUrl { get; set; }
    }

}
