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
            engine.DialogFinished += HandleDialogFinished;
        }

        private async void SpeechNotRecognized(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                tts.Speak(viewModel.Dialog.ErrorMessage ?? "Not recoginized");
                StartRecognitionOfCurrentQuestion();
            });
        }

        private void SpeechRecognized(object sender, ASR.Events.AnswerSelectedEventArgs e)
        {
            currentVarrialbes.Varriables.Items.Add(new { Name = e.FieldName, Value = e.SelectedAnswer });
            SetNewQuestion(e);
        }

        private void StartEngine(object sender, RoutedEventArgs e)
        {
            engine.Init();
            viewModel.DialogRunning = true;
            SetNewQuestion();
        }

        private void HandleDialogFinished(object sender, DialogFinishedEvent e)
        {
            viewModel.DialogRunning = false;
            viewModel.InfoText = string.IsNullOrEmpty(e.Prompt) ? "You have been registered for repair." : e.Prompt;
            tts.Speak(viewModel.InfoText);
        }

        private Task SetNewQuestion(ASR.Events.AnswerSelectedEventArgs previousAnswer = null)
        {
            return Task.Run(() =>
            {
                if (previousAnswer != null)
                {
                    engine.AddAnswer(previousAnswer.FieldName, previousAnswer.SelectedAnswer);
                }
                if (!engine.IsDialogFinished)
                {
                    viewModel.Dialog = engine.GetQuestion();
                    StartRecognitionOfCurrentQuestion();
                    tts.Speak(viewModel.Dialog.Question);
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
