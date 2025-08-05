namespace azure_functions.Shared.Models;

public class ChapterInfo
{
    public required string Title { get; set; }
    public required Uri HtmlUri { get; set; } // Chapter HTML blob URI (not content)
    public required int Order { get; set; }
}
