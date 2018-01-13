using CarRepair.ASR.Extensions;
using CarRepair.ASR.Events;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRepair.ASR.Models;
using System.Speech.Recognition;

namespace CarRepair.ASR
{
    public class SpeechRecognition : IDisposable
    {
        CultureInfo culture;
        AnswerModel answers;
        SpeechRecognitionEngine sre;

        #region Ctor
        public SpeechRecognition(string culture = null)
        {
            if (String.IsNullOrEmpty(culture))
                culture = "en-GB";
            this.culture = CultureInfo.GetCultureInfo(culture);
            Init();
        }

        public SpeechRecognition(CultureInfo culture)
        {
            this.culture = culture;
            sre = new SpeechRecognitionEngine(this.culture);
            Init();
        }

        private void Init()
        {
            var x = SpeechRecognitionEngine.InstalledRecognizers();
            sre = new SpeechRecognitionEngine(this.culture);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += HandleSpeechRecognized;
            sre.SpeechRecognitionRejected += HandleNotRecognized;

#if DEBUG
            sre.SpeechRecognized += (s, e) =>
            {
                Console.WriteLine("SpeechRecognized: " + e.Result?.Text);
            };
#endif
        }

        private void HandleNotRecognized(object sender, SpeechRecognitionRejectedEventArgs e)
        {
#if DEBUG
            Console.WriteLine("Rejected");
#endif
            SpeechNotRecognized?.Invoke(this, new EventArgs());
        }

        #endregion

        public event EventHandler<AnswerSelectedEventArgs> SpeechRecognized;
        public event EventHandler SpeechNotRecognized;

        private void HandleSpeechRecognized(object sender, SpeechRecognizedEventArgs eventArgs)
        {
            var answer = new AnswerSelectedEventArgs
            {
                Result = true,
                FieldName = answers.FieldName,
                SelectedAnswer = eventArgs.Result.Text,
            };
            SpeechRecognized?.Invoke(this, answer);
        }

        public bool StartRecognition(AnswerModel answerModel, bool multipleRecognitions = false)
        {
            answers = answerModel;
            if (answers == null)
                return false;
            sre.UnloadAllGrammars();
            sre.LoadGrammar(answers.ToGrammar(culture));

            if (multipleRecognitions)
                sre.RecognizeAsync(RecognizeMode.Multiple);
            else
                sre.RecognizeAsync();
            return true;
        }

        public void StopRecognition()
        {
            sre.RecognizeAsyncStop();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    sre.SetInputToNull();
                    sre.Dispose();
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
