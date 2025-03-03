namespace get.szurag.pl.Models;

public class FileExplorerViewModel
{
    public string? CurrentPath { get; set; } = null;
    public List<string?> Directories { get; set; } = [];
    public List<string?> Files { get; set; } = [];
}