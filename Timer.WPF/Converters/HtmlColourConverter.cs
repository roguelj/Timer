using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Timer.WPF.Converters
{
    internal class HtmlColourConverter : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color colour;
            if (value == null)
            {
                colour = Color.FromRgb(255, 255, 255);
            }
            else
            {
                colour = (Color)ColorConverter.ConvertFromString($"{value}");
            }
            return new SolidColorBrush(colour);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
