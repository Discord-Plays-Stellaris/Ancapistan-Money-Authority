using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VIR.Objects
{
    public class User
    {
        public string Id; // Discord snowflake ID
        public double Money; // Money they have, duh
        public int Age; // Age of the player
        public int PP; // PP hehe
        public string MainCompany; // Primary company of the user, three letter ticket
        public int Utils; // default max 100
        public Collection<string> corps;

        public User(string id, double money, int age, int pp, string mainCompany, int Utils)
        {
            Id = id;
            Money = money;
            Age = age;
            PP = pp;
            MainCompany = mainCompany;
        }

        public User(JObject dbInput)
        {
            Id = (string)dbInput["id"];
            Money = (double) dbInput["money"];
            Age = (int) dbInput["age"];
            PP = (int) dbInput["pp"];
            MainCompany = (string) dbInput["maincompany"];
            Utils = (int) dbInput["utils"];
            foreach (string x in dbInput["corps"].ToArray())
            {
                corps.Add(x);
            }
        }

        public JObject SerializeIntoJObject()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            var jObject = JObject.Parse(jsonString);
            return jObject;
        }
    }
}
