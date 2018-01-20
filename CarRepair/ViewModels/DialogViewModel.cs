using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.ViewModels
{
    class DialogViewModel : BaseViewModel
    {
        public string FieldName { get; set; }

        private string _question;
        public string Question
        {
            get { return _question; }
            set { _question = value; NotifyChanged(); }
        }

        private AnswersViewModel _answers;
        public AnswersViewModel Answers
        {
            get { return _answers; }
            set { _answers = value; NotifyChanged(); }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; NotifyChanged(); }
        }

        public string XMLGrammar { get; set; }
    }
}
