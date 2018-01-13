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
            engine = new AppEngine();
            viewModel = new MainWindowViewModel();
            DataContext = viewModel;
            tts = new TTS.SpeechSynth();
            asr = new SpeechRecognition();
            asr.SpeechRecognized += SpeechRecognized;
            asr.SpeechNotRecognized += SpeechNotRecognized;
            InitializeComponent();
            Loaded += StartEngine;
        }

        private async void SpeechNotRecognized(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                tts.Speak(viewModel.Dialog.ErrorMessage ?? "Not recoginized");
                asr.StartRecognition(new AnswerModel
                {
                    FieldName = viewModel.Dialog.FieldName,
                    Options = (viewModel.Dialog.Answers as OneOfAnswersViewModel).Answers.Select(s => s).ToArray()
                });
            });
        }

        private void SpeechRecognized(object sender, ASR.Events.AnswerSelectedEventArgs e)
        {
            SetNewQuestion(e);
        }

        private void StartEngine(object sender, RoutedEventArgs e)
        {
            engine.Init();
            SetNewQuestion();
        }

        private Task SetNewQuestion(ASR.Events.AnswerSelectedEventArgs previousAnswer = null)
        {
            return Task.Run(() =>
            {
                if(previousAnswer != null)
                {
                    engine.AddAnswer(previousAnswer.FieldName, previousAnswer.SelectedAnswer);
                }
                viewModel.Dialog = engine.GetQuestion();
                //todo obsługa spelling
                asr.StartRecognition(new AnswerModel
                {
                    FieldName = viewModel.Dialog.FieldName,
                    Options = (viewModel.Dialog.Answers as OneOfAnswersViewModel).Answers.Select(s => s).ToArray()
                });
                tts.Speak(viewModel.Dialog.Question);
            })
            .ContinueWith((t) =>
            {
                if (t.Status == TaskStatus.Faulted)
                {
                    var exception = t.Exception.Flatten();
                    Console.WriteLine("Error :" + Environment.NewLine + exception.ToString());
                    tts.Speak("Error occured, please try again");
                    asr.StartRecognition(new AnswerModel
                    {
                        FieldName = viewModel.Dialog.FieldName,
                        Options = (viewModel.Dialog.Answers as OneOfAnswersViewModel).Answers.Select(s => s).ToArray()
                    });
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
