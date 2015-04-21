using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using VkLib.Core.Users;
using VkPeopleSearchApp.Extensions;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace VkPeopleSearchApp.Controls
{
    public sealed partial class PeopleItemControl : UserControl
    {
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register(
            "User", typeof(VkProfile), typeof(PeopleItemControl), new PropertyMetadata(default(VkProfile), UserPropertyChanged));

        /// <summary>
        /// User
        /// </summary>
        public VkProfile User
        {
            get { return (VkProfile)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        private static readonly DependencyProperty SecondLineProperty = DependencyProperty.Register(
            "SecondLine", typeof(string), typeof(PeopleItemControl), new PropertyMetadata(default(string)));

        /// <summary>
        /// Second line string (university or city info)
        /// </summary>
        private string SecondLine
        {
            get { return (string)GetValue(SecondLineProperty); }
            set { SetValue(SecondLineProperty, value); }
        }

        public PeopleItemControl()
        {
            this.InitializeComponent();
        }

        private static void UserPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (PeopleItemControl)d;
            if (e.NewValue != null)
            {
                var user = (VkProfile)e.NewValue;

                //build second line string (university or city)
                string secondLine = String.Empty;
                if (!user.Universities.IsNullOrEmpty())
                {
                    var university = user.Universities.First();
                    secondLine += university.Name;
                    if (university.Graduation != 0)
                        secondLine += " ‘" + university.Graduation.ToString().Substring(2, 2); //will not properly work after 9999 year, but I don't think this code will still be here
                }
                else
                {
                    if (user.City != null)
                        secondLine += user.City.Title;

                    if (user.Country != null)
                        secondLine += (!string.IsNullOrEmpty(secondLine) ? ", " : "") + user.Country.Title;
                }

                c.SecondLine = secondLine;

                //setup verified icon
                c.VerifiedIcon.Visibility = user.IsVerified ? Visibility.Visible : Visibility.Collapsed;

                //setup add friend icon
                if (user.IsFriend)
                {
                    c.FriendedIcon.Visibility = Visibility.Visible;
                    c.AddFriendButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    c.FriendedIcon.Visibility = Visibility.Collapsed;
                    c.AddFriendButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void NameTextBlock_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Place icon at the right of the NameTextBlock after it's measured to avoid icon clipping on long names 
            VerifiedIcon.Margin = new Thickness(16 + NameTextBlock.ActualWidth, (NameTextBlock.ActualHeight - VerifiedIcon.ActualHeight) / 2, VerifiedIcon.Margin.Right, VerifiedIcon.Margin.Bottom);
        }
    }
}
