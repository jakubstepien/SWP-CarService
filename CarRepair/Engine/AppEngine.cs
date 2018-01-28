using System.IO;
using CarRepair.Engine.Events;
using Jint.Native;
using System.Text.RegularExpressions;
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

        public bool IsDialogFinished { get; private set; } = false;
        public Dictionary<string, string> AnswersCopy { get { return new Dictionary<string, string>(answers); } }

        public void Init()
        {
            var parser = new Parser.VXMLParser();
            var forms = parser.Parse(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, currentFile));
            currentForm = forms.First().Id;
            filesForms.Add(currentFile, forms);
            IsDialogFinished = false;
        }

        public (DialogViewModel Model, string OtherText) GetQuestion(out List<string> dbValues)
        {
            var otherInfo = new List<string>();
            dbValues = new List<string>();
            Field field = GetNextField();
            while (field != null && field.FieldType != FieldType.RegularField)
            {
                if(field.FieldType == FieldType.Var)
                {
                    dbValues.Add(field.Name);
                }
                else if(field.FieldType == FieldType.Block)
                {
                    otherInfo.Add(GetPrompt(field.Prompt));
                    //block dodany do wypowiedzi dodawnay jest do wykożystanych
                    answers.Add(field.Name, "true");
                }
                field = GetNextField(dbValues.ToArray());
            }
            if(field is null)
            {
                return (null, string.Join(".",otherInfo));
            }
            var question = new DialogViewModel
            {
                Question = GetPrompt(field.Prompt),
                FieldName = field.Name,
                ErrorMessage = field.NoMatchErrorPrompt,
                XMLGrammar = field.Grammar.XMLGrammar,
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
            else if(rule.Items.Length == 1)
            {
                question.Answers = new DictationAnswerViewModel
                {
                    Label = rule.Items[0].Content,
                };
            }
            return (question, string.Join(".", otherInfo));
        }

        public void ReLoadPompt(DialogViewModel dialog)
        {
            var field = filesForms.Values.SelectMany(s => s).SelectMany(s => s.Fields).FirstOrDefault(f => f.Name == dialog.FieldName);
            dialog.Question = GetPrompt(field.Prompt);
        }

        private string GetPrompt(string prompt)
        {
            const string valueRegex = "@@(.*?)@@";
            var newPrompt = Regex.Replace(prompt, valueRegex, m => {
                var val = m.Groups[1].Value;
                if (answers.ContainsKey(val))
                {
                    return answers[val];
                }
                return "undefined";
            });
            return newPrompt;
        }

        private Field GetNextField(string[] fieldsToExclude = null)
        {
            fieldsToExclude = fieldsToExclude ?? new string[0];
            var form = filesForms[currentFile].FirstOrDefault(f => f.Id == currentForm);
            return form.Fields.Where(w => (!answers.ContainsKey(w.Name) && !fieldsToExclude.Contains(w.Name)) && CheckCond(w)).FirstOrDefault();
        }

        public List<string> AddAnswer(string fieldName, string answer)
        {
            var info = new List<string>();
            //dodaj odpowiedz
            if (answers.ContainsKey(fieldName))
            {
                answers.Remove(fieldName);
            }
            answers.Add(fieldName, answer);

            //znajdz pole
            var form = filesForms[currentFile].First(w => w.Id == currentForm);
            var field = form.Fields.First(f => f.Name == fieldName);

            //uruchom ify jeśli są
            if(field.FilledIfs != null)
            {
                var jsInterpreter = new Javascript.Interpreter();
                var ifsToRun = field.FilledIfs.Where(w => jsInterpreter.GetCondResult(w.Cond, answers));

                //Jak dojdą kolejne ify to trzeba dodać obsługe, dla other nic nie robimy?
                var exitIf = ifsToRun.FirstOrDefault(a => a.Type == FilledIfType.ExitIf);
                if (exitIf != null)
                {
                    IsDialogFinished = true;
                    return info;
                }

                foreach (var assignIf in ifsToRun.Where(w => w.Type == FilledIfType.Assign))
                {
                    var newValue = jsInterpreter.GetAssignScriptResult(assignIf.Expr, assignIf.Name, answers);
                    answers.Remove(assignIf.Name);
                    if(newValue != null)
                    {
                        answers.Add(assignIf.Name, newValue);
                    }
                    info.Add(assignIf.Prompt);
                }
            }

            //ustaw następne pole
            var nextFieldInForm = GetNextField();
            if (nextFieldInForm is null)
            {
                IsDialogFinished = true;
            }
            return info;
        }

        private bool CheckCond(Field field)
        {
            if (string.IsNullOrEmpty(field.Cond))
            {
                return true;
            }
            var jsInterpreter = new Javascript.Interpreter();
            return jsInterpreter.GetCondResult(field.Cond, answers);
        }
    }
}
