using System.Globalization;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Synthesis;
using System.Configuration;

namespace CarRepair.TTS
{
    public class SpeechSynth : IDisposable
    {
        SpeechSynthesizer tts;

        public SpeechSynth()
        {
            tts = new SpeechSynthesizer();
            tts.SetOutputToDefaultAudioDevice();
        }

        public void Speak(string text)
        {
            PromptBuilder pb = new PromptBuilder(CultureInfo.GetCultureInfo(ConfigurationManager.AppSettings["defaultCulture"]));
            pb.AppendText(text);
            tts.Speak(pb);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    tts.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
