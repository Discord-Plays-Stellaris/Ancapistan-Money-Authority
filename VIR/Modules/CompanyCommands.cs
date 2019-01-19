using Discord;
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
            company.positions = new Dictionary<string, Position>();
            Employee employee = new Employee();
            employee.userID = Context.User.Id.ToString();
            employee.salary = 0;
            employee.wage = 0;
            employee.wageEarned = 0;
            Position position = new Position();
            position.ID = "CEO";
            position.level = 99;
            position.manages = 7;
            position.name = "CEO";
            employee.position = position;
            company.employee.Add(Context.User.Id.ToString(), employee);
            company.positions.Add(position.ID, position);
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
            /*string tmp = "";
            Collection<JObject> ids = await dataBaseService.getJObjects("companies");
            foreach(JObject x in ids)
            {
                tmp += (string)x["id"] + " - " + (string)x["name"] + "\n";
            }
            await ReplyAsync($"Current Companies:\n{tmp}");*/

            Collection<string> ids = await dataBaseService.getIDs("companies");
            int companyCount = ids.Count;
            Collection<EmbedFieldBuilder> companyEmbedList;

            foreach(string ID in ids)
            {
                Company temp = new Company(await dataBaseService.getJObjectAsync(ID, "companies"));

                EmbedFieldBuilder tempEmb = new EmbedFieldBuilder().WithIsInline(true).WithName($"{temp.name} ({temp.id})").WithValue("");
            }
        }

        [Command("addposition")]
        public async Task AddPositionToPlayer(IUser user, string companyTicker, string positionID)
        {
            Company company = await CompanyService.getCompany(companyTicker);
            if (!company.employee.ContainsKey(Context.User.Id.ToString()))
                await ReplyAsync("You are not part of this corporation!");
            if (company.employee[Context.User.Id.ToString()].position.manages%2 == 1) 
            {
                if(!company.employee.ContainsKey(user.Id.ToString()))
                {
                    await ReplyAsync("The User specified is not a part of this corporation!");
                } else
                {
                    if (company.positions.ContainsKey(positionID)) {
                        company.employee[user.Id.ToString()].position = company.positions[positionID];
                        await ReplyAsync($"Successfully changed {user.Mention} to position of {company.positions[positionID].name}");
                    } else
                    {
                        await ReplyAsync("The position id you specified is invalid.");
                    }
                }
            }
        }
    }
}
