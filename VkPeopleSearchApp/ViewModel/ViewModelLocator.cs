using Windows.UI.Xaml;

namespace VkPeopleSearchApp.ViewModel
{
    public class ViewModelLocator
    {
        private static MainViewModel _main;

        public static MainViewModel Main
        {
            get
            {
                return _main;
            }
        }

        public double RootWidth
        {
            get { return Window.Current.Bounds.Width; }
        }

        public ViewModelLocator()
        {
            _main = new MainViewModel();
        }
    }
}
