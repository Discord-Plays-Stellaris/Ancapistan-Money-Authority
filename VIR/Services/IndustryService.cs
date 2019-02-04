using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIR.Objects;
using VIR.Objects.Company;

namespace VIR.Services
{
    class IndustryService
    {
        private readonly CompanyService _companyService;
        private readonly DataBaseHandlingService _database;
        private readonly ResourceHandlingService _resourceHandlingService;

        public IndustryService(IServiceProvider services, DataBaseHandlingService database, CompanyService com, ResourceHandlingService resser)
        {
            _companyService = com;
            _database = database;
            _resourceHandlingService = resser;
        }

        public string WorkAtIndustryForUtils(string industryId, int utilSpent, string workerId)
        {
            var worker = new User(_database.getJObjectAsync(workerId, "users").Result);

            if (worker.Utils < utilSpent)
            {
                return $"Worker has insufficient utils. Worker has {worker.Utils} left.";
            }

            var industry = new Industry(_database.getJObjectAsync(industryId, "industries").Result);

            if (industry.Utils < utilSpent)
            {
                return $"Industry has insufficient utils. Industry has {industry.Utils} left.";
            }

            var company = _companyService.getCompany(industry.CompanyId).Result;

            industry.Utils -= utilSpent;
            worker.Utils -= utilSpent;

            ulong amount = ((ulong)utilSpent * industry.MonthlyOutput) / 2;

            _resourceHandlingService.AddResource(company.id, industry.Type, amount);

            return
                $"Successfully spent {utilSpent} Utils working at Industry {industryId}. You now have {worker.Utils} Utils left, and the industry has {industry.Utils} Utils left.";
        }
    }
}
