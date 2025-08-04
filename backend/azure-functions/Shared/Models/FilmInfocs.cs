namespace azure_functions.Shared.Models
{
    public class FilmInfo
    {
        public string? Title { get; set; }
        public int? Order { get; set; }
        public Uri? Media { get; set; } 
        public string? Html { get; set; }  
    }
}
