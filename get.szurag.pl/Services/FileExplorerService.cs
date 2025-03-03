using get.szurag.pl.Data;
using get.szurag.pl.Models;

namespace get.szurag.pl.Services;

public class FileExplorerService(ApplicationDbContext context)
{
    // private readonly string _rootPath = Path.GetFullPath(rootPath);
    private const string RootPath = "public";

    public (bool isFile, string fullPath, string? path) GetSafePath(string? path)
    {
        var fullPath = string.IsNullOrEmpty(path) ? RootPath : Path.GetFullPath(Path.Combine(RootPath, path));

        return (System.IO.File.Exists(fullPath), fullPath, path);
    }

    public List<string?> GetDirectories(string fullPath)
    {
        if (!Directory.Exists(fullPath)) return [];

        var directories = Directory.GetDirectories(fullPath)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();

        var hiddenItems = context.ProtectedItems
            .Where(x => x.IsFolder && x.Path.StartsWith(fullPath) && !x.IsVisible)
            .Select(x => Path.GetFileName(x.Path))
            .ToHashSet();

        return directories
            .Where(name => !hiddenItems.Contains(name))
            .ToList();
    }

    public List<string?> GetFiles(string fullPath)
    {
        if (!Directory.Exists(fullPath)) return [];

        var files = Directory.GetFiles(fullPath)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrEmpty(name) && !name.Equals(".DS_Store", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var hiddenItems = context.ProtectedItems
            .Where(x => !x.IsFolder && x.Path.StartsWith(fullPath) && !x.IsVisible)
            .Select(x => Path.GetFileName(x.Path))
            .ToHashSet();

        return files
            .Where(name => !hiddenItems.Contains(name))
            .ToList();
    }
}