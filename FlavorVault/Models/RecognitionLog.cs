using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// 识别记录
/// </summary>
[Table("RecognitionLogs")]
public partial class RecognitionLog : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [Column("Id")]
    public int Id { get; set; }

    [Column("ImagePath")]
    public string? ImagePath { get; set; }

    [Column("RecognizedLabel")]
    public string RecognizedLabel { get; set; } = string.Empty;

    [Column("Confidence")]
    public float Confidence { get; set; }

    [Column("MappedName")]
    public string? MappedName { get; set; }

    [Column("WasSaved")]
    public bool WasSaved { get; set; } = false;

    /// <summary>
    /// 创建时间，字符串存储（SQLite兼容）
    /// </summary>
    [Column("CreatedAt")]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
