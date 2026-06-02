using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// Wish list item
/// </summary>
[Table("WishItems")]
public partial class WishItem : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [Column("Id")]
    public int Id { get; set; }

    [Column("FoodName")]
    public string FoodName { get; set; } = string.Empty;

    [Column("Region")]
    public string? Region { get; set; }

    /// <summary>
    /// Priority: Urgent/WhenFree/Whenever
    /// </summary>
    [Column("Priority")]
    public string Priority { get; set; } = "WhenFree";

    [Column("SourceReason")]
    public string? SourceReason { get; set; }

    [Column("IsCompleted")]
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Linked FoodEntry ID (linked after trying)
    /// </summary>
    [Column("LinkedEntryId")]
    public int? LinkedEntryId { get; set; }

    /// <summary>
    /// Creation time, stored as string (SQLite compatible)
    /// </summary>
    [Column("CreatedAt")]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
