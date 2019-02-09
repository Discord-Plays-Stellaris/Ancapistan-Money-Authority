using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VIR.Objects.Company;
using VIR.Services;

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
        public Dictionary<string, Position> positions;
        public double Money;
        public int shares;
        public double SharePrice;
        public Dictionary<string, JobOffer> jobOffers;
        public Dictionary<string, JobRequest> jobRequests;
        public string role;

        public Company()
        {
            SharePrice = 0;
            Money = 0;
            role = null;
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
            Money = tempObj.Money;
            shares = tempObj.shares;
            SharePrice = tempObj.SharePrice;
            employee = tempObj.employee;
            positions = tempObj.positions;
            jobOffers = tempObj.jobOffers;
            role = tempObj.role;
        }
        public JObject SerializeIntoJObject()
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
        public int CompareEmployees(Employee x, Employee y)
        {
            if (x.position.level > y.position.level)
            {
                return 1;
            }
            else if (x.position.level < y.position.level)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        public int ComparePositions(Position x, Position y)
        {
            if (x.level > y.level)
            {
                return 1;
            }
            else if (x.level < y.level)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
