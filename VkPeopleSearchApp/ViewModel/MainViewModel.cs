using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Command;
using VkPeopleSearchApp.ViewModel.Messages;

namespace VkPeopleSearchApp.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Commands

        /// <summary>
        /// Go to page command
        /// </summary>
        public RelayCommand<string> GoToPageCommand { get; private set; }

        #endregion

        public MainViewModel()
        {
            InitializeMessageInterception();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            GoToPageCommand = new RelayCommand<string>(page => NavigateToPage(page));
        }

        private void InitializeMessageInterception()
        {
            MessengerInstance.Register<NavigateToPageMessage>(this, message =>
            {
                NavigateToPage(message.Page, message.Parameters);
            });

            MessengerInstance.Register<GoBackMessage>(this, GoBack);
        }

        private void NavigateToPage(string page, Dictionary<string, object> parameters = null)
        {
            var rootNamespace = Application.Current.GetType().Namespace;
            Type type;
            if (page.StartsWith("/MainView"))
                type = Type.GetType(rootNamespace + ".MainPage", false);
            else
                type = Type.GetType(rootNamespace + ".View." + page.Substring(1), false);
            if (type == null)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                return;
            }

            if (typeof(Page).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                var frame = (Frame)Window.Current.Content;
                frame.Navigate(type, parameters);
            }
            else throw new Exception("Unable to navigate to page " + page);
        }

        private void GoBack(GoBackMessage message)
        {
            var frame = (Frame)Window.Current.Content;
            frame.GoBack();
        }
    }
}
