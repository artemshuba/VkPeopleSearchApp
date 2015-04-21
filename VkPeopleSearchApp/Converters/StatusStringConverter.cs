using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using VkLib.Core.Users;
using VkPeopleSearchApp.Helpers;

namespace VkPeopleSearchApp.Converters
{
    public class StatusStringConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty IsFemaleProperty = DependencyProperty.Register(
            "IsFemale", typeof (bool), typeof (StatusStringConverter), new PropertyMetadata(default(bool)));

        public bool IsFemale
        {
            get { return (bool) GetValue(IsFemaleProperty); }
            set { SetValue(IsFemaleProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var status = (VkUserStatus)value;

            return StringHelper.GetStatusString(status, IsFemale ? VkUserSex.Female : VkUserSex.Any);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
