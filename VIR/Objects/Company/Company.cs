using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VIR.Modules.Objects.Company
{
    /// <summary>
    /// A class for an instance of a company
    /// </summary>
    public class Company
    {
        public string id;
        public string name;
        public Dictionary<string, Employee> employee; //Format: userid, title
        //public Dictionary<string, int> assets; //thing, amount
        public Dictionary<string, Position> positions;
        public int money;
        public int shares;
        public double SharePrice;

        public Company()
        {
            SharePrice = 0;
            money = 0;
        }
        public Company(JObject companyDbEntry)
        {
            /*ticker = (string)companyDbEntry["id"];
            name = (string)companyDbEntry["name"];
            money = (int)companyDbEntry["money"];
            shares = (int)companyDbEntry["shares"];
            SharePrice = (double)companyDbEntry["SharePrice"];
            foreach(JObject entry in (Array) companyDbEntry["employee"])
            {
                employee.Add((string)entry["id"], new Employee(entry));
            }
            shareholders = JsonConvert.DeserializeObject<Dictionary<string, int>>(companyDbEntry["shareholders"].ToString());*/

            Company tempObj = JsonConvert.DeserializeObject<Company>(companyDbEntry.ToString());

            id = tempObj.id;
            name = tempObj.name;
            money = tempObj.money;
            shares = tempObj.shares;
            SharePrice = tempObj.SharePrice;
            employee = tempObj.employee;
            positions = tempObj.positions;
        }
        public JObject serializeIntoJObject()
        {
            /*JObject temp = new JObject();
            temp["name"] = name;
            temp["money"] = money;
            temp["id"] = ticker;
            temp["shares"] = shares;
            temp["SharePrice"] = SharePrice;
            Collection<JObject> employeeDeserialise = new Collection<JObject>();
            foreach(Employee x in employee.Values)
            {
                employeeDeserialise.Add(x.Deserialise());
            }
            temp["employee"] = JArray.FromObject(employeeDeserialise.ToArray());
            return temp;*/

            string JSONString = JsonConvert.SerializeObject(this);
            JObject jObject = JObject.Parse(JSONString);
            return jObject;
        }
    }
}
