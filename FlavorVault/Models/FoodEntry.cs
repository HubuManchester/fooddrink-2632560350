using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// Food catalog entry
/// </summary>
[Table("FoodEntries")]
public partial class FoodEntry : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [Column("Id")]
    public int Id { get; set; }

    [Column("CatalogNumber")]
    public string CatalogNumber { get; set; } = string.Empty;

    [Column("Name")]
    public string Name { get; set; } = string.Empty;

    [Column("Region")]
    public string Region { get; set; } = string.Empty;

    [Column("Rarity")]
    public string Rarity { get; set; } = "Common";

    [Column("ImagePath")]
    public string? ImagePath { get; set; }

    [Column("StarRating")]
    public int StarRating { get; set; } = 3;

    [Column("PrimaryTaste")]
    public string? PrimaryTaste { get; set; }

    [Column("AromaTag")]
    public string? AromaTag { get; set; }

    [Column("TextureTag")]
    public string? TextureTag { get; set; }

    [Column("Description")]
    public string? Description { get; set; }

    /// <summary>
    /// Main ingredients, stored as JSON array string, e.g. ["Tofu","Beef","SichuanPeppercorn"]
    /// </summary>
    [Column("Ingredients")]
    public string? Ingredients { get; set; }

    [Column("PriceRange")]
    public string PriceRange { get; set; } = "10~30";

    [Column("CollectStatus")]
    public string CollectStatus { get; set; } = "Collected";

    [Column("Latitude")]
    public double? Latitude { get; set; }

    [Column("Longitude")]
    public double? Longitude { get; set; }

    [Column("LocationName")]
    public string? LocationName { get; set; }

    /// <summary>
    /// Discovery date, format yyyy-MM-dd
    /// </summary>
    [Column("DiscoverDate")]
    public string? DiscoverDate { get; set; }

    [Column("NoteText")]
    public string? NoteText { get; set; }

    [Column("VoiceNoteText")]
    public string? VoiceNoteText { get; set; }

    [Column("CollectionName")]
    public string? CollectionName { get; set; }

    [Column("IsShowcase")]
    public bool IsShowcase { get; set; } = false;

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
