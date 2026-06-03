using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// 用户偏好 KV 存储
/// 关键键值：theme(Light/Dark/System)、fontSize(Small/Medium/Large/ExtraLarge)、
/// userName、first_run(completed)、seed_version
/// </summary>
[Table("UserPreferences")]
public partial class UserPreference : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [Column("Id")]
    public int Id { get; set; }

    /// <summary>
    /// 偏好键（唯一约束）
    /// </summary>
    [Unique]
    [Column("Key")]
    public string Key { get; set; } = string.Empty;

    [Column("Value")]
    public string Value { get; set; } = string.Empty;
}
