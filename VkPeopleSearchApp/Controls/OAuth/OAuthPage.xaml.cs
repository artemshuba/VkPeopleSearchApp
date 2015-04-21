using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace VkPeopleSearchApp.Controls.OAuth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OAuthPage : Page
    {
        private AutoResetEvent _handle;
        private string _url;
        private string _callbackUrl;
        private Uri _currentUri;

        public Uri CurrentUri
        {
            get { return _currentUri; }
        }

        public OAuthPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;

            var p = (Dictionary<string, object>)e.Parameter;
            _url = (string)p["url"];
            _callbackUrl = (string)p["callbackUrl"];
            _handle = (AutoResetEvent)p["handle"];
            Start();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtonsOnBackPressed;

            base.OnNavigatedFrom(e);
        }

        private void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs e)
        {
            if (WebView.CanGoBack)
                WebView.GoBack();
            else
                _handle.Set();

            e.Handled = true;
        }

        private void Start()
        {
            WebView.Navigate(new Uri(_url));
        }

        private void WebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            Debug.WriteLine("OAuthPage: Navigating to " + args.Uri.OriginalString);

            _currentUri = args.Uri;

            if (args.Uri.OriginalString.StartsWith(_callbackUrl))
            {
                args.Cancel = true;
                _handle.Set();
            }
            else
                StartLoading();
        }

        private void WebView_OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            FinishLoading();
        }

        private void StartLoading()
        {
            WebView.Visibility = Visibility.Collapsed;
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.IsIndeterminate = true;
        }

        private void FinishLoading()
        {
            WebView.Visibility = Visibility.Visible;
            ProgressBar.Visibility = Visibility.Collapsed;
            ProgressBar.IsIndeterminate = false;
        }
    }
}
