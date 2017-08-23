using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGM.Uwp.ViewModels
{
    public class ContextMenuViewModel : ViewModelBase
    {
        private Action _action;

        public Action Action
        {
            get { return _action; }
            set { Set(ref _action, value); }
        }
        private string  _menuText;

        public string MenuText
        {
            get { return _menuText; }
            set { Set(ref _menuText, value); }
        }
    }
}
