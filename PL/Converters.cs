using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL
{
    // מחזיר true במצב Update ו-false במצב Add – לשימוש עם IsReadOnly
    public class ConvertUpdateToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? text = value?.ToString();
            return string.Equals(text, "Update", StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // מחזיר Visible במצב Update ו-Collapsed במצב Add – לשימוש עם Visibility
    public class ConvertUpdateToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? text = value?.ToString();
            return string.Equals(text, "Update", StringComparison.OrdinalIgnoreCase)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
