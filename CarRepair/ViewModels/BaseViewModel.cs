using System.Runtime.CompilerServices;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.ViewModels
{
    class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyChanged([CallerMemberName] string callerMember = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMember));
        }
    }
}
