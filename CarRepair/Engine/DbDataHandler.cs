using CarRepair.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Engine
{
    public class DbDataHandler
    {
        public (bool Success, string ErrorMsg) Handle(Dictionary<string, string> dialogData, KeyValuePair<string, string> newData)
        {
            dialogData = new Dictionary<string, string>(dialogData);
            dialogData.Add(newData.Key, newData.Value);
            var fieldsToAddUser = new string[] { "imie", "nazwisko", "telefon" };
            if (fieldsToAddUser.All(a => dialogData.ContainsKey(a)))
            {
                var added = AddUser(dialogData);
                if (!added.Success)
                {
                    return added;
                }
            }
            if (dialogData.ContainsKey("pin"))
            {
                return AddRepiar(dialogData);
            }

            //nic nie robimy
            return (true, string.Empty);
        }

        private (bool Success, string ErrorMsg) AddUser(Dictionary<string, string> dialogData)
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
                    return (true, string.Empty);
                }
            }
            catch (Exception e)
            {
                return (false, e.ToString());
            }
        }

        private (bool Success, string ErrorMsg) AddRepiar(Dictionary<string, string> dialogData)
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
                    descripton.AppendLine(additionalData);
                }
                var pin = string.Join("", dialogData["pin"].Split(' '));
                var brand = dialogData["marka"];
                var productionYear = dialogData["rocznik"];
                var price = dialogData["cost"];
                var date = dialogData["date"];
                using (var repairRepo = new RepairRepository())
                {
                    var success = repairRepo.AddRepair(pin, brand, productionYear, fault, descripton.ToString(), DateTime.Parse(date), decimal.Parse(price, System.Globalization.CultureInfo.InvariantCulture));
                    return (success, string.Empty);
                }
            }
            catch (Exception e)
            {
                return (false, e.ToString());
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
