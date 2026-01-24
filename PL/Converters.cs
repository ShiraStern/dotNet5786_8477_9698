using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL;
////האם שדה יהיה ReadOnly
// האם שדה יוצג או יוסתר
//בהתאם לטקסט “Update” או “Add”.
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
//public class ConvertUpdate : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        string? text = value?.ToString();
//        return string.Equals(text, "Update", StringComparison.OrdinalIgnoreCase)
//            ? Visibility.Visible
//            : Visibility.Collapsed;
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        throw new NotImplementedException();
//    }

//}


public class TimeSpanToDaysHoursConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan ts)
            return $"{ts.Days} days, {ts.Hours} hours";

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}

