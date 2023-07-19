using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace Timer.WPF.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is null || !bool.TryParse($"{value}", out bool result) || !result)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is null || (Visibility)value == Visibility.Collapsed || (Visibility)value == Visibility.Hidden)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

    }
}