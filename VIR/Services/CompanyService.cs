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
    } 
}
