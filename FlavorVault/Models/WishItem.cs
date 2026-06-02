using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// 心愿条目
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
    /// 优先级：尽快/有空就去/随缘
    /// </summary>
    [Column("Priority")]
    public string Priority { get; set; } = "有空就去";

    [Column("SourceReason")]
    public string? SourceReason { get; set; }

    [Column("IsCompleted")]
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// 关联的FoodEntry ID（体验后关联）
    /// </summary>
    [Column("LinkedEntryId")]
    public int? LinkedEntryId { get; set; }

    /// <summary>
    /// 创建时间，字符串存储（SQLite兼容）
    /// </summary>
    [Column("CreatedAt")]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
