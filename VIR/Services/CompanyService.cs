using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using VIR.Modules.Objects.Company;
using Discord;
using System.Collections.ObjectModel;

namespace VIR.Services
{
    public class CompanyService
    {
        private readonly DataBaseHandlingService __database;
        private readonly CommandHandlingService __commands;
        public CompanyService(IServiceProvider services, DataBaseHandlingService database, CommandHandlingService commands)
        {
            __database = database;
            __commands = commands;
        }

        public async Task<Company> getCompany(string ticker)
        {
            JObject company = await __database.getJObjectAsync(ticker, "companies");
            return new Company(company);
        }
        public async Task setCompany(Company company)
        {
            await __database.SetJObjectAsync(company.SerializeIntoJObject(), "companies");
        }
        public async Task<Collection<Company>> findEmployee(IUser user)
        {
            Collection<Company> companies = new Collection<Company>();
            foreach (JObject x in JArray.FromObject(await __database.GetFieldAsync(user.Id.ToString(), "companies", "users")))
            {
                companies.Add(await getCompany((string)x));
            }
            return companies;
        }
        public async Task paySalaries(int year)
        {
            Collection<JObject> companies = await __database.getJObjects("companies");
            foreach(JObject x in companies)
            {
                Company company = new Company(x);
                string owner = null;
                foreach(string c in company.employee.Keys)
                {
                    if(company.employee[c].position.level == 99)
                    {
                        owner = c;
                    }
                }
                double ownermoney = double.Parse((string)await __database.GetFieldAsync(owner, "money", "users"));
                foreach (string c in company.employee.Keys)
                {
                    if (company.Money - company.employee[c].salary >= 0)
                    {
                        company.Money -= company.employee[c].salary;
                        await __database.SetFieldAsync(c, "money", (double.Parse((string)await __database.GetFieldAsync(c, "money", "users")))+ company.employee[c].salary, "users");
                    } else if (ownermoney - company.employee[c].salary >= 0)
                    {
                        ownermoney -= company.employee[c].salary;
                        await __database.SetFieldAsync(c, "money", (double.Parse((string)await __database.GetFieldAsync(c, "money", "users"))) + company.employee[c].salary, "users");
                    } else
                    {
                        await __commands.sendDMTask(c, $"The CEO of company {company.name} has been unable to pay your salary");
                    }
                }
                await __database.SetJObjectAsync(company.SerializeIntoJObject(), "companies");
                await __database.SetFieldAsync(owner, "money", ownermoney, "users");
            }
        }
    } 
}
