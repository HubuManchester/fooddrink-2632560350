using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// 地点标记
/// </summary>
[Table("PlaceMarks")]
public partial class PlaceMark : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [Column("Id")]
    public int Id { get; set; }

    [Column("Name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 类型：美食街/老字号/夜市/商圈/景点周边
    /// </summary>
    [Column("Category")]
    public string Category { get; set; } = string.Empty;

    [Column("Address")]
    public string? Address { get; set; }

    [Column("Latitude")]
    public double Latitude { get; set; }

    [Column("Longitude")]
    public double Longitude { get; set; }

    /// <summary>
    /// 计算属性：格式 "纬度 xx.xxxxxx 经度 xx.xxxxxx"
    /// </summary>
    [Ignore]
    public string CoordinatesText => $"纬度 {Latitude:F6} 经度 {Longitude:F6}";

    [Column("Region")]
    public string? Region { get; set; }

    [Column("Feature")]
    public string? Feature { get; set; }

    /// <summary>
    /// 推荐指数（1-5）
    /// </summary>
    [Column("StarRating")]
    public int StarRating { get; set; } = 3;

    /// <summary>
    /// 访问日期，格式 yyyy-MM-dd
    /// </summary>
    [Column("VisitDate")]
    public string? VisitDate { get; set; }

    /// <summary>
    /// 创建时间，字符串存储（SQLite兼容）
    /// </summary>
    [Column("CreatedAt")]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
