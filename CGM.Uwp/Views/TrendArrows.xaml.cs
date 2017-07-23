using CGM.Communication.MiniMed.Model;
using CGM.Communication.MiniMed.Responses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CGM.Uwp.Views
{
    public class ArrowModel
    {
        public int Angle { get; set; }

        public override string ToString()
        {
            return this.Angle.ToString();
        }
    }
    public sealed partial class TrendArrows : UserControl, INotifyPropertyChanged
    {

        public PumpStatusMessage StatusMessage
        {
            get { return GetValue(StatusMessageProperty) as PumpStatusMessage; }
            set
            {
                SetValue(StatusMessageProperty, value);
                SetArrows();
            }
        }

        private bool _unknownTrend;

        public bool UnknownTrend
        {
            get { return _unknownTrend; }
            set { _unknownTrend = value; }
        }

        private bool _showAlert;

        public bool ShowAlert
        {
            get { return _showAlert; }
            set { _showAlert = value;
                OnPropertyChanged("ShowAlert");
            }
        }


        public static DependencyProperty StatusMessageProperty = DependencyProperty.Register("StatusMessage", typeof(PumpStatusMessage), typeof(TrendArrows), new PropertyMetadata(null));

         public ObservableCollection<ArrowModel> Arrows { get; set; } = new ObservableCollection<ArrowModel>();

        public TrendArrows()
        {
            this.InitializeComponent();
            ArrowImages.DataContext = this;
        }

        private void SetArrows()
        {
            if (this.StatusMessage!=null)
            {

            
            this.Arrows.Clear();
            UnknownTrend = false;
            switch (this.StatusMessage.CgmTrendName)
            {
                case SgvTrend.None:
                    UnknownTrend = true;
                    break;
                case SgvTrend.DoubleUp:
                    this.Arrows.Add(new ArrowModel() { Angle = -90 });
                    this.Arrows.Add(new ArrowModel() { Angle = -90 });
                    break;
                case SgvTrend.SingleUp:
                    this.Arrows.Add(new ArrowModel() { Angle = -90 });
                    break;
                case SgvTrend.FortyFiveUp:
                    this.Arrows.Add(new ArrowModel() { Angle = -45 });
                    break;
                case SgvTrend.Flat:
                    this.Arrows.Add(new ArrowModel() { Angle = 0 });
                    break;
                case SgvTrend.FortyFiveDown:
                    this.Arrows.Add(new ArrowModel() { Angle = 45 });
                    break;
                case SgvTrend.SingleDown:
                    this.Arrows.Add(new ArrowModel() { Angle = 90 });
                    break;
                case SgvTrend.DoubleDown:
                    this.Arrows.Add(new ArrowModel() { Angle = 90 });
                    this.Arrows.Add(new ArrowModel() { Angle = 90 });
                    break;
                case SgvTrend.NotComputable:
                    UnknownTrend = true;
                    break;
                case SgvTrend.RateOutOfRange:
                    UnknownTrend = true;
                    break;
                case SgvTrend.NotSet:
                    UnknownTrend = true;
                    break;
                default:
                    UnknownTrend = true;
                    break;
                }
                this.ShowAlert = StatusMessage.Alert != 0;

            }
        }


        void OnPropertyChanged(String prop)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
