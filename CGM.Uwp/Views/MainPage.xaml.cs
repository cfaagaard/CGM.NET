using CGM.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace CGM.Uwp.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel
        {
            get { return DataContext as MainViewModel; }
        }

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
