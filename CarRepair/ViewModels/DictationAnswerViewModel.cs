﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.ViewModels
{
    class DictationAnswerViewModel : AnswersViewModel
    {
        private string _label;

        public string Label
        {
            get { return _label; }
            set { _label = value; NotifyChanged(); }
        }

    }
}
