using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VkPeopleSearchApp.Helpers
{
    /// <summary>
    /// Вспомогательный класс для индикации каких-либо операций (например загрузка данных)
    /// </summary>
    public class LongRunningTask : INotifyPropertyChanged
    {
        private bool _isWorking;
        private string _error;

        /// <summary>
        /// True - операция выполняется
        /// </summary>
        public bool IsWorking
        {
            get { return _isWorking; }
            set
            {
                if (_isWorking == value)
                    return;

                _isWorking = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текст ошибки
        /// </summary>
        public string Error
        {
            get { return _error; }
            set
            {
                if (_error == value)
                    return;

                _error = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
