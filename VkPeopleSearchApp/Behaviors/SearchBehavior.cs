using System;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;

namespace VkPeopleSearchApp.Behaviors
{
    public class SearchBehavior : DependencyObject, IBehavior
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(SearchBehavior), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public DependencyObject AssociatedObject { get; private set; }

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;

            ((TextBox)AssociatedObject).KeyDown += OnKeyDown;
        }

        public void Detach()
        {
            ((TextBox)AssociatedObject).KeyDown -= OnKeyDown;

            AssociatedObject = null;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            var textBox = (TextBox)AssociatedObject;
            if (!string.IsNullOrWhiteSpace(textBox.Text) && e.Key == VirtualKey.Enter)
            {
                Command.Execute(null);

                //hide keyboard
                FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
            }
        }
    }
}
