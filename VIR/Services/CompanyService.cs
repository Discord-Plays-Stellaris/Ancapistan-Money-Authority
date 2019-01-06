using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIR.Services
{
    class CompanyService
    {
        private readonly DataBaseHandlingService __database;
        public class Company
        {
            public string ticker;
            public string name;
            public Dictionary<string, string> employee; //Format: userid, title
            public Dictionary<string, int> assets; //thing, amount
            public int money;
            public Company(JObject companyDbEntry)
            {
                ticker = (string)companyDbEntry["id"];
                name = (string)companyDbEntry["name"];
                foreach(JObject x in (Array)companyDbEntry["employees"])
                {
                    employee.Add((string)x["id"], (string)x["titles"]);
                }
                foreach (JObject x in (Array)companyDbEntry["assets"])
                {
                    assets.Add((string)x["id"], (int)x["amount"]);
                }
                money = (int)companyDbEntry["money"];
            }
            public JObject serializeIntoJObject()
            {
                JObject temp = new JObject();
                temp["name"] = name;
                Collection<JObject> tmp = new Collection<JObject>();
                foreach (string key in employee.Keys)
                {
                    JObject x = new JObject();
                    x["id"] = key;
                    x["titles"] = employee[key];
                }
                temp["employees"] = new JArray(tmp.ToArray());
                Collection<JObject> tmp2 = new Collection<JObject>();
                foreach (string key in assets.Keys)
                {
                    JObject x = new JObject();
                    x["id"] = key;
                    x["amount"] = employee[key];
                }
                temp["assets"] = new JArray(tmp.ToArray());
                temp["money"] = money;
                return temp;
            }
        }
        public CompanyService(IServiceProvider services, DataBaseHandlingService database)
        {
            __database = database;
        }

        public async Task<Company> getCompany(string ticker)
        {
            JObject company = await __database.getJObjectAsync(ticker, "companies");
            return new Company(company);
        }
        public async Task setCompany(Company company)
        {
            await __database.SetJObjectAsync(company.ticker, company.serializeIntoJObject(), "companies");
        }
    }
}
