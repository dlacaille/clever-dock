using System.Globalization;
using System.Windows.Data;

namespace CleverDock.Converters;

public class AdditionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var number = double.Parse(parameter.ToString());
        return double.Parse(value.ToString()) + number;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var number = double.Parse(parameter.ToString());
        return double.Parse(value.ToString()) - number;
    }
}