using Discord.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VIR.Modules.Objects.Company;
using VIR.Modules.Preconditions;
using VIR.Services;

namespace VIR.Modules
{
    /// <summary>
    /// Contains async methods regarding corporations
    /// </summary>
    public class CompanyCommands : ModuleBase<SocketCommandContext>
    {
        private readonly CompanyService CompanyService;
        private readonly DataBaseHandlingService dataBaseService;

        public CompanyCommands(CompanyService com, DataBaseHandlingService db)
        {
            CompanyService = com;
            dataBaseService = db;
        }

        [Command("createcompany")]
        [Alias("createcorporation", "addcompany", "addcorporation")]
        [HasMasterOfBots]
        public async Task CreateCompanyTask(string ticker, int startingShares, [Remainder]string name)
        {
            Company company = new Company();
            company.name = name;
            company.shares = startingShares;
            company.id = ticker;
            company.employee = new Dictionary<string, Employee>();
            Employee employee = new Employee();
            employee.userID = Context.User.Id.ToString();
            employee.salary = 0;
            employee.wage = 0;
            employee.wageEarned = 0;
            Position position = new Position();
            position.ID = "CEO";
            position.level = 99;
            position.manages = new Collection<Position>();
            position.name = "CEO";
            employee.position = position;
            company.employee.Add(Context.User.Id.ToString(), employee);
            await CompanyService.setCompany(company);
            JObject user = await dataBaseService.getJObjectAsync(Context.User.Id.ToString(), "users");
            Collection<string> corps = new Collection<string>();
            try {
                foreach (string x in (Array) user["corps"])
                {
                    corps.Add(x);
                }
            } catch {}
            corps.Add(ticker);
            await dataBaseService.SetFieldAsync(Context.User.Id.ToString(), "corps", JArray.FromObject(corps.ToArray()), "users");
            await ReplyAsync("Company successfully created!");
        }

        [Command("companies")]
        [Alias("corporations")]
        public async Task GetCompaniesTask()
        {
            string tmp = "";
            Collection<string> ids = await dataBaseService.getIDs("companies");
            foreach(string x in ids)
            {
                tmp += ids + "\n";
            }
            await ReplyAsync($"Current Companies:\n{tmp}");
        }
    }
}
