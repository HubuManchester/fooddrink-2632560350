using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

[Table("Users")]
public partial class User : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [Column("Id")]
    public int Id { get; set; }

    [Column("Username"), Unique, NotNull, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Column("PasswordHash"), NotNull]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("DisplayName"), MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [Column("Avatar"), MaxLength(500)]
    public string? Avatar { get; set; }

    [Column("CreatedAt")]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    [Column("LastLoginAt")]
    public string? LastLoginAt { get; set; }
}
