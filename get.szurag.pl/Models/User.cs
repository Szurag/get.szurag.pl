using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OtpNet;

namespace get.szurag.pl.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    [Column(TypeName = "VARCHAR(255)")]
    public required string Name { get; set; }

    [Required]
    [MaxLength(36)]
    [Column(TypeName = "VARCHAR(36)")]
    public string SecretKey { get; set; } = GenerateSecretKey();

    private static string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }
}