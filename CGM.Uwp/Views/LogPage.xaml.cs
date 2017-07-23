using CGM.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace CGM.Uwp.Views
{
    public sealed partial class LogPage : Page
    {
        private LogViewModel ViewModel
        {
            get { return DataContext as LogViewModel; }
        }

        public LogPage()
        {
            InitializeComponent();
        }
    }
}
