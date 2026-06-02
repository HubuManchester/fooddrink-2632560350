using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// Priority to color (Urgent=#F44336 / WhenFree=#FFC107 / Whenever=#4CAF50)
/// </summary>
public class PriorityToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var priority = value?.ToString() ?? string.Empty;

        return priority switch
        {
            "Urgent" => Color.FromArgb("#F44336"),
            "WhenFree" => Color.FromArgb("#FFC107"),
            "Whenever" => Color.FromArgb("#4CAF50"),
            _ => Color.FromArgb("#FFC107")
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "WhenFree";
    }
}
