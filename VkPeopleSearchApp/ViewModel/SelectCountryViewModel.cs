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
    public class SelectCountryViewModel : ViewModelBase
    {
        private List<VkCountry> _nearCountries;
        private List<VkCountry> _allCountries;
        private List<VkCountry> _countries;

        private SearchParams _searchParams;
        private string _query;

        private ManualResetEventAsync _loadDataEvent = new ManualResetEventAsync();
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        #region Commands

        public RelayCommand<VkCountry> SelectCountryCommand { get; private set; }

        public RelayCommand ResetCommand { get; private set; }

        #endregion

        public List<VkCountry> Countries
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

        public SelectCountryViewModel(SearchParams searchParams)
        {
            _searchParams = searchParams;

            RegisterTasks("countries");

            InitializeCommands();

            LoadCountries();
        }

        private void InitializeCommands()
        {
            SelectCountryCommand = new RelayCommand<VkCountry>(country =>
            {
                _searchParams.Country = country;

                MessengerInstance.Send(new GoBackMessage());
            });

            ResetCommand = new RelayCommand(() =>
            {
                _searchParams.Country = null;
                _searchParams.City = null;

                MessengerInstance.Send(new GoBackMessage());
            });
        }

        private async Task LoadCountries()
        {
            TaskStarted("countries");

            try
            {
                //load near countries
                var response = await ServiceLocator.Vkontakte.Database.GetCountries();
                if (!response.Items.IsNullOrEmpty())
                {
                    _nearCountries = response.Items;
                    Countries = _nearCountries;
                }
                else
                {
                    TaskError("countries", "Не удалось загрузить список стран");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                TaskError("countries", "Не удалось загрузить список стран");
            }

            TaskFinished("countries");

            try
            {
                //load all countries
                var response = await ServiceLocator.Vkontakte.Database.GetCountries(needAll: true, count: 1000);
                if (!response.Items.IsNullOrEmpty())
                {
                    _allCountries = response.Items;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                _loadDataEvent.Set();
            }
        }

        private async void Search(CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                Countries = _nearCountries;
                return;
            }

            TaskStarted("countries");

            await _loadDataEvent.WaitAsync();

            TaskFinished("countries");

            if (token.IsCancellationRequested)
                return;

            if (_allCountries.IsNullOrEmpty())
            {
                TaskError("countries", "Не удалось загрузить список стран");
                return;
            }

            var foundCountries =
                _allCountries.Where(c => c.Title.ToLower().Contains(Query.ToLower()))
                    .OrderByDescending(c => c.Title.ToLower().StartsWith(Query.ToLower()))
                    .ToList();

            Countries = foundCountries;
        }
    }
}
