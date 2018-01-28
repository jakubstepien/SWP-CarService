using System.Threading;
using CarRepair.ViewModels;
using CarRepair.ASR.Models;
using CarRepair.ASR;
using System.Runtime.InteropServices.ComTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarRepair.Engine;
using CarRepair.Engine.Events;

namespace CarRepair
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AppEngine engine;
        MainWindowViewModel viewModel;
        TTS.SpeechSynth tts;
        SpeechRecognition asr;

        public MainWindow()
        {
            viewModel = new MainWindowViewModel();
            DataContext = viewModel;

            InitializeComponent();

            engine = new AppEngine();
            tts = new TTS.SpeechSynth();
            asr = new SpeechRecognition();
            asr.SpeechRecognized += SpeechRecognized;
            asr.SpeechNotRecognized += SpeechNotRecognized;
            Loaded += StartEngine;
        }

        private async void SpeechNotRecognized(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                tts.Speak(viewModel.Dialog.ErrorMessage ?? "Not recognized");
                StartRecognitionOfCurrentQuestion();
            });
        }

        private void SpeechRecognized(object sender, ASR.Events.AnswerSelectedEventArgs e)
        {
            var dbHandler = new DbDataHandler();
            var result = dbHandler.Handle(engine.AnswersCopy, new KeyValuePair<string, string>(e.FieldName, e.SelectedAnswer));
            if (result.Success)
            {
                foreach (var newValue in result.ValuesToAdd)
                {
                    engine.AddAnswer(newValue.Key, newValue.Value);
                }
                SetNewQuestion(e);
                currentVarrialbes.Varriables.Items.Clear();
                var answers = engine.AnswersCopy.ToArray();
                foreach (var field in answers)
                {
                    currentVarrialbes.Varriables.Items.Add(new { Name = field.Key, Value = field.Value });
                }
                CollectionViewSource.GetDefaultView(currentVarrialbes.Varriables.Items).Refresh();
            }
            else
            {
                SpeechNotRecognized(this, new EventArgs());
            }
        }

        private void StartEngine(object sender, RoutedEventArgs e)
        {
            engine.Init();
            viewModel.DialogRunning = true;
            SetNewQuestion();
        }

        private Task SetNewQuestion(ASR.Events.AnswerSelectedEventArgs previousAnswer = null)
        {
            return Task.Run(() =>
            {
                if (previousAnswer != null)
                {
                    var info = engine.AddAnswer(previousAnswer.FieldName, previousAnswer.SelectedAnswer);
                    tts.Speak(string.Join(".", info));
                }
                if (!engine.IsDialogFinished)
                {
                    var dbValuesRequired = new List<string>();
                    var dbHandler = new DbDataHandler();
                    var dialog = engine.GetQuestion(out dbValuesRequired);
                    if (!string.IsNullOrEmpty(dialog.OtherText))
                    {
                        tts.Speak(dialog.OtherText);
                    }
                    foreach (var value in dbValuesRequired)
                    {
                        engine.AddAnswer(value, dbHandler.GetVarValue(value, engine.AnswersCopy));
                    }
                    if (dbValuesRequired != null && dbValuesRequired.Any())
                    {
                        engine.ReLoadPompt(dialog.Model);
                    }

                    if (dialog.Model is null)
                    {
                        viewModel.Dialog.Question = dialog.OtherText;
                        viewModel.Dialog.Answers = new DictationAnswerViewModel();
                    }
                    else
                    {
                        viewModel.Dialog = dialog.Model;
                        StartRecognitionOfCurrentQuestion();
                        tts.Speak(viewModel.Dialog.Question);
                    }
                }
                else
                {
                    viewModel.Dialog.Question = "Goodbye";
                    viewModel.Dialog.Answers = new DictationAnswerViewModel();
                    tts.Speak("Goodbye");
                }
            })
            .ContinueWith((t) =>
            {
                if (t.Status == TaskStatus.Faulted)
                {
                    var exception = t.Exception.Flatten();
                    Console.WriteLine("Error :" + Environment.NewLine + exception.ToString());
                    tts.Speak("Error occured, please try again");
                    StartRecognitionOfCurrentQuestion();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void StartRecognitionOfCurrentQuestion()
        {
            string[] options = null;
            switch (viewModel.Dialog.Answers)
            {
                case DictationAnswerViewModel d:
                    options = new[] { d.Label };
                    break;
                case OneOfAnswersViewModel many:
                    options = (viewModel.Dialog.Answers as OneOfAnswersViewModel).Answers.Select(s => s).ToArray();
                    break;
                default:
                    break;
            }
            asr.StartRecognition(new AnswerModel
            {
                FieldName = viewModel.Dialog.FieldName,
                Options = options,
            }, viewModel.Dialog.XMLGrammar);
        }
    }
}
