using CarRepair.ViewModels;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRepair.Parser.Models;

namespace CarRepair.Engine
{
    class AppEngine
    {
        Dictionary<string, string> answers = new Dictionary<string, string>();
        Dictionary<string, Form[]> filesForms = new Dictionary<string, Form[]>();
        string currentFile = "app-root.vxml";
        string currentForm = "";
        string nextField = "";


        public void Init()
        {
            var parser = new Parser.VXMLParser();
            var forms = parser.Parse(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, currentFile));
            currentForm = forms.First().Id;
            filesForms.Add(currentFile, forms);
        }

        public DialogViewModel GetQuestion()
        {
            Field field = GetNextField();
            if (field.FieldType == FieldType.Record)
            {
                return GetRecordQuestion(field);
            }
            var question = new DialogViewModel
            {
                Question = field.Prompt,
                FieldName = field.Name,
                ErrorMessage = field.NoMatchErrorPrompt,
            };
            //dla tego typu bedzie raczej jeden rule
            var rule = field.Grammar.Rules.FirstOrDefault();
            //jeżeli ma oneOf to raczej tylko jedno
            var oneOfGrammar = rule.Items.OfType<OneOfRule>().FirstOrDefault();
            if (oneOfGrammar != null)
            {
                //skupiamy się tylko na tym reszte pomijamy?
                question.Answers = new OneOfAnswersViewModel
                {
                    Answers = new System.Collections.ObjectModel.ObservableCollection<string>(oneOfGrammar.Items.Select(s => s.Content))
                };

            }
            return question;
        }

        private Field GetNextField()
        {
            var form = filesForms[currentFile].FirstOrDefault(f => f.Id == currentForm);
            if (string.IsNullOrEmpty(nextField))
            {
                return form.Fields.First();
            }
            else
            {
                return form.Fields.First(f => f.Name == nextField);
            }
        }

        public void AddAnswer(string fieldName, string answer)
        {
            if (answers.ContainsKey(fieldName))
            {
                answers.Remove(fieldName);
            }
            answers.Add(fieldName, answer);
            var form = filesForms[currentFile].First(w => w.Id == currentForm);
            var field = form.Fields.First(f => f.Name == fieldName);
            if (string.IsNullOrEmpty(field.FilledJavascript))
            {
                //jeżeli nie ma kodu do switcha to idziemy do nastepnego pola
                SetNextFieldInSequence(fieldName, form);
                return;
            }

            var intr = new Javascript.Interpreter().GetGotoValue(field.GotoVariable, field.FilledJavascript, answers);
            //id do innego formularza
            if (intr.StartsWith("#"))
            {
                foreach (var f in filesForms[currentFile])
                {
                    if ("#" + f.Id == intr)
                    {
                        currentForm = f.Id;
                        nextField = null;
                        return;
                    }
                }

            }
            else if (intr.EndsWith(".vxml"))
            {
                if (!filesForms.ContainsKey(intr))
                {
                    var forms = new Parser.VXMLParser().Parse(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, intr));
                    filesForms.Add(intr, forms);
                    currentFile = intr;
                }
            }
        }

        private void SetNextFieldInSequence(string fieldName, Form form)
        {
            var nextFieldInForm = form.Fields.SkipWhile(s => s.Name != fieldName).ElementAtOrDefault(1);
            if (nextFieldInForm != null)
            {
                nextField = nextFieldInForm.Name;
            }
            else
            {
                var nextForm = filesForms[currentFile].SkipWhile(s => s.Id != currentForm).ElementAt(1);
                currentForm = nextForm.Id;
            }
        }

        private DialogViewModel GetRecordQuestion(Field field)
        {
            throw new NotImplementedException();
        }
    }
}
