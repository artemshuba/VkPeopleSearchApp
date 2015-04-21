using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using VkLib.Core.Auth;
using VkPeopleSearchApp.Controls.OAuth;
using VkPeopleSearchApp.Services;

namespace VkPeopleSearchApp.Helpers
{
    /// <summary>
    /// Helper class for OAuth
    /// </summary>
    public class AuthHelper
    {
        private static AutoResetEvent _handle;

        public static async Task<OAuthResult> AuthAsync()
        {
            Frame f = Window.Current.Content as Frame;
            if (f != null)
            {
                if (!f.Dispatcher.HasThreadAccess)
                    throw new InvalidOperationException("Must be called on UI thread.");

                _handle = new AutoResetEvent(false);
                f.Navigate(typeof(OAuthPage), new Dictionary<string, object>()
                {
                    {"url", ServiceLocator.Vkontakte.OAuth.GetAuthUrl()},
                    {"callbackUrl", ServiceLocator.Vkontakte.OAuth.GetAuthRedirectUrl()},
                    {"handle", _handle}
                });

                await Task.Run(() => WaitResponse());
                var page = (OAuthPage)f.Content;

                var result = ServiceLocator.Vkontakte.OAuth.ProcessAuth(page.CurrentUri);

                f.GoBack();

                return result;
            }

            return null;
        }

        private static void WaitResponse()
        {
            _handle.WaitOne();
        }
    }
}
