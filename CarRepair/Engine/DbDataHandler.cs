using CarRepair.Db;
using CarRepair.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Engine
{
    public class DbDataHandler
    {
        public DataHandleResult Handle(Dictionary<string, string> dialogData, KeyValuePair<string, string> newData)
        {
            //pracuj na kopi danych
            dialogData = new Dictionary<string, string>(dialogData);
            if (dialogData.ContainsKey(newData.Key))
            {
                dialogData.Remove(newData.Key);
            }
            dialogData.Add(newData.Key, newData.Value);
            DataHandleResult addedResult = null;
            var fieldsToAddUser = new string[] { "imie", "nazwisko", "telefon" };
            if (fieldsToAddUser.All(a => dialogData.ContainsKey(a)))
            {
                addedResult = AddUser(dialogData);
                if (!addedResult.Success)
                {
                    return addedResult;
                }
            }
            if (dialogData.ContainsKey("pin"))
            {
                var repairResult = AddRepiar(dialogData);
                return new DataHandleResult
                {
                    Success = repairResult.Success,
                    Error = repairResult.Error,
                    ValuesToAdd = repairResult.ValuesToAdd.Concat(addedResult.ValuesToAdd).ToArray()
                };
            }

            //nic nie robimy
            return new DataHandleResult { Success = true };
        }

        private DataHandleResult AddUser(Dictionary<string, string> dialogData)
        {
            var name = dialogData["imie"];
            var surname = dialogData["nazwisko"];
            var phone = string.Join("", dialogData["telefon"].Split(' '));
            try
            {
                using (var clientRepo = new ClientRepository())
                {
                    var pin = clientRepo.AddUser(name, surname, phone);
                    dialogData.Add("pin", pin);
                    return new DataHandleResult { Success = true, ValuesToAdd = new [] { new KeyValuePair<string,string>("pin",pin)} };
                }
            }
            catch (Exception e)
            {
                return new DataHandleResult { Success = false, Error = e.ToString() };
            }
        }

        private DataHandleResult AddRepiar(Dictionary<string, string> dialogData)
        {
            try
            {
                var faultKey = "usterka";
                var faultData = new[] {
                    new { Fault = "engine", Fields = new string[] { "naped", "spalanie" } },
                    new { Fault = "tires", Fields = new string[] { "rodzajOpon" } },
                    new { Fault = "doors", Fields = new string[] { "drzwi" } },
                }.ToDictionary(d => d.Fault);
                var fault = dialogData[faultKey];
                var descripton = new StringBuilder($"Usterka  - {fault}: ");
                foreach (var additionalData in faultData[fault].Fields)
                {
                    if (dialogData.ContainsKey(additionalData))
                    {
                        descripton.AppendLine(additionalData + ":" + dialogData[additionalData]);
                    }
                }
                var pin = string.Join("", dialogData["pin"].Split(' '));
                var brand = dialogData["marka"];
                var productionYear = dialogData["rocznik"];
                var price = dialogData["cost"];
                var date = dialogData["date"];
                using (var repairRepo = new RepairRepository())
                {
                    var success = repairRepo.AddRepair(pin, brand, productionYear, fault, descripton.ToString(), DateTime.Parse(date), decimal.Parse(price, System.Globalization.CultureInfo.InvariantCulture));
                    return new DataHandleResult { Success = true };
                }
            }
            catch (Exception e)
            {
                return new DataHandleResult { Success = false, Error = e.ToString() };
            }
        }

        public string GetVarValue(string val, Dictionary<string, string> dialogData)
        {
            switch (val)
            {
                case "cost":
                    return GetCost(dialogData);
                case "date":
                    using(var repo = new RepairRepository())
                    {
                        return repo.GetNextPossibleDate().ToString("yyyy-MM-dd");
                    }
                default:
                    return null;
            }
        }

        private string GetCost(Dictionary<string, string> dialogData)
        {
            var fault = dialogData["usterka"];
            var brand = dialogData["marka"];
            using (var repo = new RepairRepository())
            {
                return repo.GetPrice(fault, brand).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}
