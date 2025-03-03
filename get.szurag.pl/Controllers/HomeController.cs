using System.Reflection;
using get.szurag.pl.Data;
using get.szurag.pl.Models;
using get.szurag.pl.Services;
using Microsoft.AspNetCore.Mvc;

namespace get.szurag.pl.Controllers;

public class HomeController(FileExplorerService fileExplorerService, ApplicationDbContext context) : Controller
{
    [Route("{**path}")]
    public IActionResult Index(string? path)
    {
        if (path!.StartsWith("api/"))
        {
            return NotFound();
        }
        
        var safePath = fileExplorerService.GetSafePath(path);
        var (isFile, fullPath, fileRelativePath) = safePath;

        var protectedItem = GetProtectedItem(fileRelativePath);

        if (HttpContext.Request.Method == "POST")
        {
            var password = HttpContext.Request.Form["password"];
            if (protectedItem is { PasswordHash: not null } && BCrypt.Net.BCrypt.Verify(password, protectedItem.PasswordHash))
            {
                HttpContext.Session.SetInt32("ProtectedItemId." + protectedItem.Id, 1);
            }
        }

        if (protectedItem is { PasswordHash: not null } && HttpContext.Session.GetInt32("ProtectedItemId." + protectedItem.Id) == null)
        {
            return View("Protected");
        }

        if (isFile)
        {
            var mimeType = MimeMapping.MimeUtility.GetMimeMapping(fullPath);
            return PhysicalFile(fullPath, mimeType);
        }

        if (!Directory.Exists(fullPath))
        {
            return NotFound("Nie znaleziono.");
        }

        HttpContext.Session.SetString("CurrentPath", fullPath);

        return View(new FileExplorerViewModel()
        {
            CurrentPath = string.IsNullOrEmpty(path) ? "/" : path,
            Directories = fileExplorerService.GetDirectories(fullPath),
            Files = fileExplorerService.GetFiles(fullPath)
        });
    }

    private ProtectedItem? GetProtectedItem(string? fileRelativePath)
    {
        if (string.IsNullOrEmpty(fileRelativePath))
            return null;

        var directories = fileRelativePath.Split('/');

        for (var i = directories.Length; i > 0; i--)
        {
            var partialPath = string.Join("/", directories.Take(i));
            var item = context.ProtectedItems.FirstOrDefault(x => x.Path == partialPath);

            if (item is { PasswordHash: not null })
            {
                return item;
            }
        }

        return null;
    }
}
