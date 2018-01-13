using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.ASR.Events
{
    public class AnswerSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Flaga czy udało się rozpoznać odpowiedź
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Nazwa pola do którego odnosi się odpowiedź
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Wybrana odpowiedz
        /// </summary>
        public string SelectedAnswer { get; set; }

    }
}
