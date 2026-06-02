using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// User preference KV store
/// Key values: theme(Light/Dark/System), fontSize(Small/Medium/Large/ExtraLarge),
/// userName, first_run(completed), seed_version
/// </summary>
[Table("UserPreferences")]
public partial class UserPreference : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [Column("Id")]
    public int Id { get; set; }

    /// <summary>
    /// Preference key (unique constraint)
    /// </summary>
    [Unique]
    [Column("Key")]
    public string Key { get; set; } = string.Empty;

    [Column("Value")]
    public string Value { get; set; } = string.Empty;
}
