using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DevFlow_Assistant.Shared.Converters;

public class CountToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var count = value switch
        {
            int intValue => intValue,
            null => 0,
            _ => 1
        };

        var isVisible = count > 0;
        if (Invert)
        {
            isVisible = !isVisible;
        }

        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
