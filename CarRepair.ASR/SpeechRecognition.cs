using CarRepair.ASR.Events;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRepair.ASR.Models;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;
using System.Configuration;

namespace CarRepair.ASR
{
    public class SpeechRecognition : IDisposable
    {
        CultureInfo culture;
        AnswerModel currentAnswer;
        SpeechRecognitionEngine sre;

        #region Ctor
        public SpeechRecognition(string culture = null)
        {
            if (String.IsNullOrEmpty(culture))
            {
                culture = ConfigurationManager.AppSettings["defaultCulture"];
            }
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
            sre.RecognizeCompleted += HandleSpeechCompleted;
            sre.SpeechRecognitionRejected += (s,e)=>
            {
                Console.WriteLine("Rejected");
            };
            sre.SpeechRecognized += (s, e) =>
            {
                Console.WriteLine("SpeechRecognized: " + e.Result?.Text);
            };
        }

        #endregion

        public event EventHandler<AnswerSelectedEventArgs> SpeechRecognized;

        public event EventHandler SpeechNotRecognized;

        private void HandleSpeechCompleted(object sender, RecognizeCompletedEventArgs eventArgs)
        {
            if(eventArgs.Result is null)
            {
                SpeechNotRecognized?.Invoke(this, new EventArgs());
            }
            else
            {
                var answer = new AnswerSelectedEventArgs
                {
                    Result = true,
                    FieldName = currentAnswer.FieldName,
                    SelectedAnswer = GetProperResult(eventArgs),
                };
                SpeechRecognized?.Invoke(this, answer);
            }
        }

        private static string GetProperResult(RecognizeCompletedEventArgs eventArgs)
        {
            var sementicResult = eventArgs?.Result?.Semantics?.Value?.ToString();
            //jak jest bardziej skomplikowana gramatyka z tag np. number
            if (sementicResult != null)
                return sementicResult;
            //jak jest garbage
            var multipleWords = eventArgs?.Result?.Words?.Any();
            if (multipleWords.HasValue && multipleWords.Value)
            {
                var multiwordValue = string.Join(" ", eventArgs.Result.Words.Where(w => w.LexicalForm != "..." && !string.IsNullOrEmpty(w.Pronunciation)).Select(s => s.Text));
                return multiwordValue;
            }

            return eventArgs.Result.Text;
        }

        public bool StartRecognition(AnswerModel answerModel, string xmlGrammar, bool multipleRecognitions = false)
        {
            currentAnswer = answerModel;
            if (currentAnswer == null)
                return false;
            sre.UnloadAllGrammars();
            sre.LoadGrammar(new Grammar(DeserializeSrgs(xmlGrammar)));

            if (multipleRecognitions)
                sre.RecognizeAsync(RecognizeMode.Multiple);
            else
                sre.RecognizeAsync();
            return true;
        }

        public SrgsDocument DeserializeSrgs(string xmlGrammar)
        {
            using(var xmlReader = System.Xml.XmlReader.Create(new System.IO.StringReader(xmlGrammar)))
            {
                return new SrgsDocument(xmlReader);
            }
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
