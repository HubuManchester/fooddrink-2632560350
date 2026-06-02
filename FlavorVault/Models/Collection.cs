using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// Collection
/// </summary>
[Table("Collections")]
public partial class Collection : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [Column("Id")]
    public int Id { get; set; }

    [Column("Name")]
    public string Name { get; set; } = string.Empty;

    [Column("Theme")]
    public string? Theme { get; set; }

    [Column("Description")]
    public string? Description { get; set; }

    [Column("CoverImagePath")]
    public string? CoverImagePath { get; set; }

    /// <summary>
    /// Color tag: Red/Orange/Yellow/Green/Blue/Purple
    /// </summary>
    [Column("ColorTag")]
    public string ColorTag { get; set; } = "Orange";

    [Column("SortOrder")]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Creation time, stored as string (SQLite compatible)
    /// </summary>
    [Column("CreatedAt")]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    /// <summary>
    /// Update time, stored as string (SQLite compatible)
    /// </summary>
    [Column("UpdatedAt")]
    public string UpdatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
