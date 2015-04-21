using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using VkLib.Core.Users;
using VkLib.Core.Users.Types;
using VkPeopleSearchApp.Model;
using VkPeopleSearchApp.ViewModel.Messages;

namespace VkPeopleSearchApp.ViewModel
{
    public class SearchParamsViewModel : ViewModelBase
    {
        private SearchParams _searchParams;

        private VkCountry _selectedCountry;

        private List<VkCity> _cities;
        private VkCity _selectedCity;

        private bool _isAnyGender, _isMale, _isFemale;

        private bool _isAnyAge;
        private double _minAge, _maxAge;

        private readonly List<VkUserStatus> _statuses = Enum.GetValues(typeof(VkUserStatus))
            .Cast<VkUserStatus>().ToList();

        private VkUserStatus? _selectedStatus;

        #region Commands

        public RelayCommand SaveCommand { get; private set; }

        public RelayCommand CancelCommand { get; private set; }

        public RelayCommand SelectCountryCommand { get; private set; }

        public RelayCommand SelectCityCommand { get; private set; }

        #endregion

        public VkCountry SelectedCountry
        {
            get { return _selectedCountry; }
            set { Set(ref _selectedCountry, value); }
        }

        public VkCity SelectedCity
        {
            get { return _selectedCity; }
            set { Set(ref _selectedCity, value); }
        }

        public bool IsAnyGender
        {
            get { return _isAnyGender; }
            set { Set(ref _isAnyGender, value); }
        }

        public bool IsMale
        {
            get { return _isMale; }
            set { Set(ref _isMale, value); }
        }

        public bool IsFemale
        {
            get { return _isFemale; }
            set
            {
                if (Set(ref _isFemale, value))
                {
                    //force to call converter
                    var s = SelectedStatus;
                    SelectedStatus = null;
                    SelectedStatus = s;
                }
            }
        }

        public bool IsAnyAge
        {
            get { return _isAnyAge; }
            set { Set(ref _isAnyAge, value); }
        }

        public double MinAge
        {
            get { return _minAge; }
            set { Set(ref _minAge, value); }
        }

        public double MaxAge
        {
            get { return _maxAge; }
            set { Set(ref _maxAge, value); }
        }

        public List<VkUserStatus> Statuses
        {
            get { return _statuses; }
        }

        public VkUserStatus? SelectedStatus
        {
            get { return _selectedStatus; }
            set { Set(ref _selectedStatus, value); }
        }

        public SearchParamsViewModel(SearchParams searchParams)
        {
            _searchParams = searchParams;
            _searchParams.NeedReload = false;

            InitializeCommands();

            LoadParams();
        }

        private void InitializeCommands()
        {
            SaveCommand = new RelayCommand(() =>
            {
                SaveParams();

                MessengerInstance.Send(new GoBackMessage());
            });

            CancelCommand = new RelayCommand(() =>
            {
                MessengerInstance.Send(new GoBackMessage());
            });

            SelectCountryCommand = new RelayCommand(() =>
            {
                MessengerInstance.Send(new NavigateToPageMessage()
                {
                    Page = "/SelectCountryView",
                    Parameters = new Dictionary<string, object>()
                       {
                           {"params", _searchParams}
                       }
                });
            });

            SelectCityCommand = new RelayCommand(() =>
            {
                MessengerInstance.Send(new NavigateToPageMessage()
                {
                    Page = "/SelectCityView",
                    Parameters = new Dictionary<string, object>()
                       {
                           {"params", _searchParams}
                       }
                });
            });
        }

        private void LoadParams()
        {
            SelectedCountry = _searchParams.Country;
            SelectedCity = _searchParams.City;

            switch (_searchParams.Sex)
            {
                case VkUserSex.Any:
                    IsAnyGender = true;
                    break;

                case VkUserSex.Male:
                    IsMale = true;
                    break;

                case VkUserSex.Female:
                    IsFemale = true;
                    break;
            }

            if (_searchParams.AgeMin == _searchParams.AgeMax && _searchParams.AgeMin == 0)
            {
                IsAnyAge = true;
            }
            else
            {
                MinAge = _searchParams.AgeMin;
                MaxAge = _searchParams.AgeMax;
            }

            SelectedStatus = _searchParams.Status;
        }

        private void SaveParams()
        {
            _searchParams.Country = SelectedCountry;
            _searchParams.City = SelectedCity;
            _searchParams.Sex = IsMale ? VkUserSex.Male : IsFemale ? VkUserSex.Female : VkUserSex.Any;

            if (IsAnyAge)
            {
                _searchParams.AgeMin = 0;
                _searchParams.AgeMax = 0;
            }
            else
            {
                _searchParams.AgeMin = (int)Math.Round(MinAge);
                _searchParams.AgeMax = (int)Math.Round(MaxAge);
            }

            _searchParams.Status = SelectedStatus;

            _searchParams.NeedReload = true;
        }
    }
}
