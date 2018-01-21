﻿using Jint.Native;
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
        string nextField = "";

        public bool IsDialogFinished { get; private set; } = false;

        public void Init()
        {
            var parser = new Parser.VXMLParser();
            var forms = parser.Parse(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, currentFile));
            currentForm = forms.First().Id;
            filesForms.Add(currentFile, forms);
            IsDialogFinished = false;
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
                Question = GetPrompt(field),
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
            return question;
        }

        private string GetPrompt(Field field)
        {
            const string valueRegex = "@@(.*?)@@";
            var newPrompt = Regex.Replace(field.Prompt, valueRegex, m => {
                var val = m.Groups[1].Value;
                if (answers.ContainsKey(val))
                {
                    return answers[val];
                }
                return "undefined";
            });
            return newPrompt;
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

            var nextFieldInForm = form.Fields.Where(w => !answers.ContainsKey(w.Name) && CheckCond(w)).FirstOrDefault();
            nextField = nextFieldInForm?.Name;
            if(nextFieldInForm is null)
            {
                IsDialogFinished = true;
            }
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


        private DialogViewModel GetRecordQuestion(Field field)
        {
            throw new NotImplementedException();
        }
    }
}
