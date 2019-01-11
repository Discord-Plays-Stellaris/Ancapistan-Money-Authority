using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VIR.Modules.Objects.Company
{
    public class Company
    {
        public string ticker;
        public string name;
        public Dictionary<string, Employee> employee; //Format: userid, title
        //public Dictionary<string, int> assets; //thing, amount
        public int money;
        public Company(JObject companyDbEntry)
        {
            ticker = (string)companyDbEntry["id"];
            name = (string)companyDbEntry["name"];
            money = (int)companyDbEntry["money"];
        }
        public JObject serializeIntoJObject()
        {
            JObject temp = new JObject();
            temp["name"] = name;
            temp["money"] = money;
            return temp;
        }
    }
}
