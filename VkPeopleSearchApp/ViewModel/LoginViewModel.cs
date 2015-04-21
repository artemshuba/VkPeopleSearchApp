using GalaSoft.MvvmLight.Command;
using VkPeopleSearchApp.Domain;
using VkPeopleSearchApp.Helpers;
using VkPeopleSearchApp.Services;
using VkPeopleSearchApp.ViewModel.Messages;

namespace VkPeopleSearchApp.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        #region Commands

        public RelayCommand LoginCommand { get; private set; }

        #endregion

        public LoginViewModel()
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private async void Login()
        {
            var result = await AuthHelper.AuthAsync();
            if (result != null && result.IsSuccess)
            {
                //success auth, go to main view

                ServiceLocator.Vkontakte.AccessToken = result.AccessToken;
                Settings.Instance.AccessToken = result.AccessToken;
                Settings.Instance.Save();

                MessengerInstance.Send(new NavigateToPageMessage() { Page = "/MainView", ClearHistory = true });
            }
        }
    }
}
