using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        private DialogViewModel _dialog;

        public DialogViewModel Dialog
        {
            get { return _dialog; }
            set { _dialog = value; NotifyChanged(); }
        }

    }
}
