using System.ComponentModel.DataAnnotations;
using MarketLinker.Domain.Entities.Marketplace.Auth;

namespace MarketLinker.Domain.Entities.User;

public class User
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(60)]
    [MaxLength(100)]
    public string PasswordHash { get; private set; } = null!;
    
    public ICollection<MarketplaceAuthBase> MarketplaceAuths { get; set; } = new List<MarketplaceAuthBase>();
    
    public void SetPassword(string plainTextPassword)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
    }
    
    public bool CheckPassword(string plainTextPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainTextPassword, PasswordHash);
    }

}