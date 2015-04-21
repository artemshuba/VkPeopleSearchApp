using System;
using System.Globalization;
using Windows.UI.Xaml.Data;
using VkPeopleSearchApp.Helpers;

namespace VkPeopleSearchApp.Converters
{
    public class SearchResultsStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var count = (int) value;

            return string.Format("{0} {1}", count.ToString("N0", new CultureInfo("ru-RU")),
                StringHelper.LocNum(count, "человек", "человека", "человек"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
