using CGM.Communication.MiniMed.Responses;
using CGM.Uwp.Models;
using CGM.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CGM.Uwp.Views
{
    public sealed partial class StatusDetailPage : Page
    {
        private StatusDetailViewModel ViewModel
        {
            get { return DataContext as StatusDetailViewModel; }
        }

        public StatusDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Item = e.Parameter as PumpStatusMessage;
        }
    }
}
