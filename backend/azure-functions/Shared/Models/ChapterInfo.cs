namespace azure_functions.Shared.Models;

public class ChapterInfo
{
    public string? Title { get; set; }
    public Uri? HtmlUri { get; set; } // Chapter HTML blob URI (not content)
}
