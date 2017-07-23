using CGM.Communication.MiniMed.Responses;
using CGM.Uwp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CGM.Uwp.Views
{


    public sealed partial class StatusDetailControl : UserControl
    {



        public PumpStatusMessage MasterMenuItem
        {
            get { return GetValue(MasterMenuItemProperty) as PumpStatusMessage; }
            set
            {
                SetValue(MasterMenuItemProperty, value);

            }
        }

        public static DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem", typeof(PumpStatusMessage), typeof(StatusDetailControl), new PropertyMetadata(null));

        public StatusDetailControl()
        {
            InitializeComponent();
        }


    }
}
