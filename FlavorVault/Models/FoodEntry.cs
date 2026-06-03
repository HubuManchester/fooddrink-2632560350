using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// 食物图鉴条目
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
    public string Rarity { get; set; } = "日常";

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
    /// 主要食材，JSON数组字符串，如 ["豆腐","牛肉","花椒"]
    /// </summary>
    [Column("Ingredients")]
    public string? Ingredients { get; set; }

    [Column("PriceRange")]
    public string PriceRange { get; set; } = "10~30";

    [Column("CollectStatus")]
    public string CollectStatus { get; set; } = "已收藏";

    [Column("Latitude")]
    public double? Latitude { get; set; }

    [Column("Longitude")]
    public double? Longitude { get; set; }

    [Column("LocationName")]
    public string? LocationName { get; set; }

    /// <summary>
    /// 发现日期，格式 yyyy-MM-dd
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
    /// 创建时间，字符串存储（SQLite兼容）
    /// </summary>
    [Column("CreatedAt")]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    /// <summary>
    /// 更新时间，字符串存储（SQLite兼容）
    /// </summary>
    [Column("UpdatedAt")]
    public string UpdatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
