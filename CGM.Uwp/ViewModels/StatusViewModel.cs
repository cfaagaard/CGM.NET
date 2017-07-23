using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using CGM.Uwp.Models;
using CGM.Uwp.Services;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CGM.Communication.MiniMed.Responses;
using GalaSoft.MvvmLight.Messaging;
using CGM.Uwp.Helpers;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Reflection;
using CGM.Communication.Common.Serialize;

namespace CGM.Uwp.ViewModels
{
    public class PropertyValue
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }
    }
    public class StatusViewModel : ViewModelBase
    {
        public NavigationServiceEx NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationServiceEx>();
            }
        }

        public FixedSizeObservableCollection<PumpStatusMessage> PumpStatusMessages { get { return ((App)App.Current).Data.PumpStatusMessages; } }

        const string NarrowStateName = "NarrowState";
        const string WideStateName = "WideState";

        private VisualState _currentState;

        private PumpStatusMessage _selected;
        public PumpStatusMessage Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value);
                Set();
            }
        }




        public ICommand ItemClickCommand { get; private set; }
        public ICommand StateChangedCommand { get; private set; }


        public StatusViewModel()
        {
            Messenger.Default.Register<SerializerSession>(this,(session) => UpdatedSession(session));

            this.PumpStatusMessages.CollectionChanged += PumpStatusMessages_CollectionChanged;
            ItemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
            StateChangedCommand = new RelayCommand<VisualStateChangedEventArgs>(OnStateChanged);
        }

        private void UpdatedSession(SerializerSession session)
        {

            
        }

        private async void PumpStatusMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (e.NewItems!=null && e.NewItems.Count>0)
                    {
                        Selected = (PumpStatusMessage)e.NewItems[0];
                    }
                    else
                    {
                        Selected = null;
                    }
                    
                });
        }

//        private async void NewStatus(PumpStatusMessage entry)
//        {
//            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
//() =>
//{
//    Selected = entry;
//});
//            //}
          

//        }
        private ObservableCollection<PropertyValue> _values;

        public ObservableCollection<PropertyValue> Values
        {
            get { return _values; }
            set
            {
               Set(ref _values, value);
            }
        }
        private void Set()
        {
            if (Selected != null)
            {

                var props = typeof(PumpStatusMessage).GetProperties();
                this.Values = new ObservableCollection<PropertyValue>();
                foreach (var prop in props.OrderBy(e=>e.Name))
                {
                    if (!prop.Name.StartsWith("unknown",StringComparison.CurrentCultureIgnoreCase))
                    {
                        var value = prop.GetValue(Selected);
                        Values.Add(new PropertyValue() { PropertyName = prop.Name, Value = value });
                    }
              
                }
            }

        }
        public async Task LoadDataAsync(VisualState currentState)
        {
            _currentState = currentState;
            //PumpStatusMessages.Clear();

            //var data = await SampleDataService.GetSampleModelDataAsync();

            //foreach (var item in data)
            //{
            //    PumpStatusMessages.Add(item);
            //}
            if (PumpStatusMessages.Count>0)
            {
                Selected = PumpStatusMessages.First();
            }
    
        }

        private void OnStateChanged(VisualStateChangedEventArgs args)
        {
            _currentState = args.NewState;
        }

        private void OnItemClick(ItemClickEventArgs args)
        {
            PumpStatusMessage item = args?.ClickedItem as PumpStatusMessage;
            if (item != null)
            {
                if (_currentState.Name == NarrowStateName)
                {
                    NavigationService.Navigate(typeof(StatusDetailViewModel).FullName, item);
                }
                else
                {
                    Selected = item;
                }
            }
        }


    }
}
