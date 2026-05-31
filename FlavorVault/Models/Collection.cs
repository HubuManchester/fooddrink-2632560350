using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// 收藏集
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
    /// 标签色：红/橙/黄/绿/蓝/紫
    /// </summary>
    [Column("ColorTag")]
    public string ColorTag { get; set; } = "橙";

    [Column("SortOrder")]
    public int SortOrder { get; set; } = 0;

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
