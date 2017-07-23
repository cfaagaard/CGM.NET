using CGM.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CGM.Communication.MiniMed.Responses;

namespace CGM.Uwp.Views
{
    public sealed partial class StatusPage : Page
    {
        private StatusViewModel ViewModel
        {
            get { return DataContext as StatusViewModel; }
        }

        public StatusPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.LoadDataAsync(WindowStates.CurrentState);
        }
    }
}
