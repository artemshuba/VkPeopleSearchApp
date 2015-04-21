using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using VkLib.Core.Users.Types;
using VkPeopleSearchApp.Extensions;
using VkPeopleSearchApp.Helpers;
using VkPeopleSearchApp.Model;
using VkPeopleSearchApp.Services;
using VkPeopleSearchApp.ViewModel.Messages;

namespace VkPeopleSearchApp.ViewModel
{
    public class SelectCityViewModel : ViewModelBase
    {
        private List<VkCity> _nearCities;
        private List<VkCity> _countries;

        private SearchParams _searchParams;
        private string _query;

        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        #region Commands

        public RelayCommand<VkCity> SelectCityCommand { get; private set; }

        public RelayCommand ResetCommand { get; private set; }

        #endregion

        public List<VkCity> Cities
        {
            get { return _countries; }
            private set { Set(ref _countries, value); }
        }

        public string Query
        {
            get { return _query; }
            set
            {
                if (Set(ref _query, value))
                {
                    _cancellationToken.Cancel();
                    _cancellationToken = new CancellationTokenSource();

                    Search(_cancellationToken.Token);
                }
            }
        }

        public SelectCityViewModel(SearchParams searchParams)
        {
            _searchParams = searchParams;

            RegisterTasks("cities");

            InitializeCommands();

            LoadCities();
        }

        private void InitializeCommands()
        {
            SelectCityCommand = new RelayCommand<VkCity>(city =>
            {
                _searchParams.City = city;

                MessengerInstance.Send(new GoBackMessage());
            });

            ResetCommand = new RelayCommand(() =>
            {
                _searchParams.City = null;

                MessengerInstance.Send(new GoBackMessage());
            });
        }

        private async Task LoadCities()
        {
            TaskStarted("cities");

            try
            {
                //load near cities
                var response = await ServiceLocator.Vkontakte.Database.GetCities(_searchParams.Country.Id);
                if (!response.Items.IsNullOrEmpty())
                {
                    _nearCities = response.Items;
                    Cities = _nearCities;
                }
                else
                {
                    TaskError("cities", "Не удалось загрузить список городов");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                TaskError("cities", "Не удалось загрузить список городов");
            }

            TaskFinished("cities");
        }

        private async void Search(CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                Cities = _nearCities;
                return;
            }

            TaskStarted("cities");

            try
            {
                //load near cities
                var response = await ServiceLocator.Vkontakte.Database.GetCities(_searchParams.Country.Id, query: Query);

                if (token.IsCancellationRequested)
                    return;

                if (!response.Items.IsNullOrEmpty())
                {
                    Cities = response.Items;
                }
                else
                {
                    TaskError("cities", "Не удалось загрузить список городов");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                TaskError("cities", "Не удалось загрузить список городов");
            }
            finally
            {
                TaskFinished("cities");
            }
        }
    }
}
