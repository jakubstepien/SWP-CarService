using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.ViewModels
{
    class OneOfAnswersViewModel : AnswersViewModel
    {
        public OneOfAnswersViewModel()
        {
            Answers = new ObservableCollection<string>();
        }

        private ObservableCollection<string> _answers;

        public ObservableCollection<string> Answers
        {
            get { return _answers; }
            set { _answers = value; NotifyChanged(); }
        }
    }
}
