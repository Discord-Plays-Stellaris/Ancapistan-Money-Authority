using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using VIR.Modules.Objects;

namespace VIR.Services
{
    class CompanyService
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
            await __database.SetJObjectAsync(company.ticker, company.serializeIntoJObject(), "companies");
        }
    }
}
