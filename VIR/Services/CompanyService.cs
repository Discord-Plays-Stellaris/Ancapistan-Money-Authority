using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using VIR.Modules.Objects.Company;

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
            await __database.SetJObjectAsync(company.serializeIntoJObject(), "companies");
        }
    }
}
