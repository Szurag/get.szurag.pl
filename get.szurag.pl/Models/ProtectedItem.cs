using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace get.szurag.pl.Models;

public class ProtectedItem
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    [Column(TypeName = "VARCHAR(255)")]
    public required string Path { get; set; }
    
    public required bool IsVisible { get; init; }
    
    public required bool IsFolder { get; init; }
    
    [MaxLength(255)]
    [Column(TypeName = "VARCHAR(255)")]
    public string? PasswordHash { get; set; }
    
}