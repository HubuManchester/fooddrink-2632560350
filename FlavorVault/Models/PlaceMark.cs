using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// Place mark
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
    /// Category: Food Street/Heritage/Night Market/Shopping District/Tourist Area
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
    /// Computed property: format "Lat xx.xxxxxx Lon xx.xxxxxx"
    /// </summary>
    [Ignore]
    public string CoordinatesText => $"Lat {Latitude:F6} Lon {Longitude:F6}";

    [Column("Region")]
    public string? Region { get; set; }

    [Column("Feature")]
    public string? Feature { get; set; }

    /// <summary>
    /// Recommendation rating (1-5)
    /// </summary>
    [Column("StarRating")]
    public int StarRating { get; set; } = 3;

    /// <summary>
    /// Visit date, format yyyy-MM-dd
    /// </summary>
    [Column("VisitDate")]
    public string? VisitDate { get; set; }

    /// <summary>
    /// Creation time, stored as string (SQLite compatible)
    /// </summary>
    [Column("CreatedAt")]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
