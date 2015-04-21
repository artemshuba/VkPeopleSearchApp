using System;
using Windows.UI.Xaml.Data;

namespace VkPeopleSearchApp.Converters
{
    public class InvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return false;
        }
    }
}
