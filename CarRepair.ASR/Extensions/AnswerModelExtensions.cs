using System.IO;
using System.Globalization;
using CarRepair.ASR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;

namespace CarRepair.ASR.Extensions
{
    internal static class AnswerModelExtensions
    {
        public static Grammar ToGrammar(this AnswerModel answerModel, CultureInfo culture)
        {
            var options = answerModel.Options;
            Choices words = new Choices();
            GrammarBuilder gramBuild = new GrammarBuilder
            {
                Culture = culture
            };
            if(options != null && options.Length > 0)
            {
                foreach (var option in options)
                {
                    words.Add(option);
                }
                gramBuild.Append(words);
            }
            else
            {
                gramBuild.Append("start");
                gramBuild.AppendDictation("spelling");
                gramBuild.Append("koniec");
            }
            Grammar gramSRE = new Grammar(gramBuild);
            gramSRE.Name = answerModel.FieldName;
            return gramSRE;
        }

        //Przykład wild card
        //private static Grammar CreatePasswordGrammar(CultureInfo culture)
        //{
        //    GrammarBuilder wildcardBuilder = new GrammarBuilder();
        //    wildcardBuilder.AppendWildcard();
        //    SemanticResultKey passwordKey =
        //      new SemanticResultKey("Password", wildcardBuilder);

        //    GrammarBuilder passwordBuilder =
        //      new GrammarBuilder("hasło to ");
        //    passwordBuilder.Append(passwordKey);
        //    passwordBuilder.Culture = culture;
        //    Grammar passwordGrammar = new Grammar(passwordBuilder);
        //    passwordGrammar.Name = "Password input";

        //    passwordGrammar.SpeechRecognized +=
        //      new EventHandler<SpeechRecognizedEventArgs>(
        //        PasswordInputHandler);

        //    return passwordGrammar;
        //}

        //// Handle the SpeechRecognized event for the password grammar.
        //private static void PasswordInputHandler(object sender, SpeechRecognizedEventArgs e)
        //{
        //    if (e.Result == null) return;

        //    RecognitionResult result = e.Result;
        //    SemanticValue semantics = e.Result.Semantics;

        //    if (semantics.ContainsKey("Password"))
        //    {
        //        using(var stream = new FileStream("test.wav", FileMode.CreateNew))
        //        {
        //            result.Audio.WriteToWaveStream(stream);
        //        }
                
        //    }
        //}

    }
}
