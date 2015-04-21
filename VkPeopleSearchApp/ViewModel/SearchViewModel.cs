using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using VkLib.Core.Auth;
using VkLib.Core.Users;
using VkLib.Error;
using VkPeopleSearchApp.Common;
using VkPeopleSearchApp.Domain;
using VkPeopleSearchApp.Extensions;
using VkPeopleSearchApp.Helpers;
using VkPeopleSearchApp.Model;
using VkPeopleSearchApp.Services;
using VkPeopleSearchApp.ViewModel.Messages;

namespace VkPeopleSearchApp.ViewModel
{
    public class SearchViewModel : ViewModelBase
    {
        private const string _userFields = "photo_50,verified,universities,city,country,is_friend";

        private SearchParams _searchParams = new SearchParams();

        private string _query;
        private IncrementalLoadingCollection<VkProfile> _users;
        private int _totalCount;

        #region Commands

        public RelayCommand SearchCommand { get; private set; }

        public RelayCommand GoToParamsCommand { get; private set; }

        public RelayCommand ClearParamsCommand { get; private set; }

        #endregion

        public string Query
        {
            get { return _query; }
            set { Set(ref _query, value); }
        }

        public IncrementalLoadingCollection<VkProfile> Users
        {
            get { return _users; }
            private set { Set(ref _users, value); }
        }

        public int TotalCount
        {
            get { return _totalCount; }
            private set { Set(ref _totalCount, value); }
        }

        public string SearchParamsString
        {
            get { return _searchParams != null ? _searchParams.ToString() : null; }
        }

        public SearchViewModel()
        {
            RegisterTasks("search", "searchMore");

            InitializeCommands();
        }

        public override void Activate(NavigationMode navigationMode)
        {
            RaisePropertyChanged("SearchParamsString");

            if (_searchParams.NeedReload)
            {
                Search();
            }
        }

        private void InitializeCommands()
        {
            SearchCommand = new RelayCommand(Search);

            GoToParamsCommand = new RelayCommand(() =>
            {
                MessengerInstance.Send(new NavigateToPageMessage()
                {
                    Page = "/SearchParamsView",
                    Parameters = new Dictionary<string, object>()
                    {
                        {"params", _searchParams}
                    }
                });
            });

            ClearParamsCommand = new RelayCommand(() =>
            {
                _searchParams = new SearchParams();
                RaisePropertyChanged("SearchParamsString");

                Search();
            });
        }

        private async void Search()
        {
            if (string.IsNullOrWhiteSpace(Query))
                return;

            TaskStarted("search");

            try
            {
                var response = await ServiceLocator.Vkontakte.Users.Search(Query, fields: _userFields,
                    country: _searchParams.Country != null ? _searchParams.Country.Id : 0,
                    city: _searchParams.City != null ? _searchParams.City.Id : 0,
                    sex: _searchParams.Sex,
                    ageFrom: _searchParams.AgeMin, ageTo: _searchParams.AgeMax,
                    status: _searchParams.Status);
                TotalCount = response.TotalCount;
                if (!response.Items.IsNullOrEmpty())
                {
                    var users = new IncrementalLoadingCollection<VkProfile>(response.Items);
                    users.OnMoreItemsRequested = LoadMoreUsers;
                    users.HasMoreItemsRequested = () =>
                    {
                        return TotalCount > Users.Count;
                    };
                    Users = users;
                }
                else
                    TaskError("search", "Ничего не найдено");
            }
            catch (VkInvalidTokenException ex)
            {
                Debug.WriteLine("Invalid token, refreshing");

                AuthHelper.AuthAsync().ContinueWith(t =>
                {
                    RefreshLogin(t.Result);
                    DispatcherHelper.RunAsync(Search);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                TaskError("search", "Не удалось выполнить поиск");
            }

            TaskFinished("search");
        }

        private async Task<List<VkProfile>> LoadMoreUsers(CancellationToken token, uint count)
        {
            TaskStarted("searchMore");

            try
            {
                //BUG баг в api ВК, если искать по запросу "Павел", ВК неправильно обрабатывает offset (например offset=18 и offset=19 первым возвращает одного и того же пользователя)
                //Пример запроса https://api.vk.com/method/users.search?q=Pavel&sort=0&fields=photo_50%2Cverified%2Cuniversities%2Ccity%2Ccountry%2Cis_friend&sex=0&count=21&offset=18&access_token=b1de0459d6af529f96fc8e1ec0ac0f4ce98fce672b05ce08aa06c13b615f74ef62e95118d60111f0ae6f9&v=5.29
                //https://api.vk.com/method/users.search?q=Pavel&sort=0&fields=photo_50%2Cverified%2Cuniversities%2Ccity%2Ccountry%2Cis_friend&sex=0&count=21&offset=19&access_token=b1de0459d6af529f96fc8e1ec0ac0f4ce98fce672b05ce08aa06c13b615f74ef62e95118d60111f0ae6f9&v=5.29
                //BUG а так (count=1 & offset=18) вообще ничего не возвращает https://api.vk.com/method/users.search?q=Pavel&sort=0&fields=photo_50%2Cverified%2Cuniversities%2Ccity%2Ccountry%2Cis_friend&sex=0&count=1&offset=18&access_token=b1de0459d6af529f96fc8e1ec0ac0f4ce98fce672b05ce08aa06c13b615f74ef62e95118d60111f0ae6f9&v=5.29
                //т.е. 19-го пользователя как будто нет

                var response =
                    await
                        ServiceLocator.Vkontakte.Users.Search(Query, fields: _userFields, count: (int)count,
                            offset: Users.Count,
                            country: _searchParams.Country != null ? _searchParams.Country.Id : 0,
                            city: _searchParams.City != null ? _searchParams.City.Id : 0,
                            sex: _searchParams.Sex,
                            ageFrom: _searchParams.AgeMin, ageTo: _searchParams.AgeMax,
                            status: _searchParams.Status);
                if (!response.Items.IsNullOrEmpty())
                {
                    return response.Items;
                }
            }
            catch (VkInvalidTokenException ex)
            {
                Debug.WriteLine("Invalid token, refreshing");

                AuthHelper.AuthAsync().ContinueWith(t =>
                {
                    RefreshLogin(t.Result);
                    DispatcherHelper.RunAsync(Search);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                //If there is an error on loading, set TotalCount to loaded users count to avoid recursive loading
                TotalCount = Users != null ? Users.Count : 0;

                TaskError("searchMore", "Не удалось загрузить результаты поиска");
            }
            finally
            {
                TaskFinished("searchMore");
            }

            return null;
        }

        private void RefreshLogin(OAuthResult result)
        {
            if (result.IsSuccess)
            {
                ServiceLocator.Vkontakte.AccessToken = result.AccessToken;
                Settings.Instance.AccessToken = result.AccessToken;
                Settings.Instance.Save();
            }
            else
            {
                //if we failed refreshing token, send user to login page
                MessengerInstance.Send(new NavigateToPageMessage()
                {
                    Page = "/LoginView",
                    ClearHistory = true
                });
            }
        }
    }
}
