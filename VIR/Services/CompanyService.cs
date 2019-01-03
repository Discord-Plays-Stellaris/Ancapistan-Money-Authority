using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIR.Services
{
    class CompanyService
    {
        private readonly DataBaseHandlingService __database;
        class Company
        {
            string name;
            string[] employees;

        }
        public CompanyService(IServiceProvider services, DataBaseHandlingService database)
        {
            __database = database;
        }

        public async Task<string> getCompany(string userid)
        {
            return "yes";
        }
    }
}
