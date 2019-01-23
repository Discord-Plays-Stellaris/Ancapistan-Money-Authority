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
using VIR.Objects;
using VIR.Objects.Company;

namespace VIR.Modules
{
    /// <summary>
    /// Contains async methods regarding corporations
    /// </summary>
    public class CompanyCommands : ModuleBase<SocketCommandContext>
    {
        private readonly CompanyService CompanyService;
        private readonly DataBaseHandlingService dataBaseService;
        private readonly CommandHandlingService CommandService;
        private readonly StockMarketService MarketService;
        public readonly List<int> r = new List<int> { 4, 5, 6, 7 };
        public readonly List<int> w = new List<int> { 2, 3, 6, 7 };
        public readonly List<int> e = new List<int> { 1, 3, 5, 7 };

        public CompanyCommands(CompanyService com, DataBaseHandlingService db, CommandHandlingService comm, StockMarketService markserv)
        {
            CompanyService = com;
            dataBaseService = db;
            CommandService = comm;
            MarketService = markserv;
        }

        [Command("createcompany")]
        [Alias("createcorporation", "addcompany", "addcorporation")]
        [HasMasterOfBots]
        public async Task CreateCompanyTask(IUser CEO, string ticker, [Remainder]string name)
        {
            Company company = new Company();
            company.name = name;
            company.shares = 0;
            company.id = ticker;
            company.employee = new Dictionary<string, Employee>();
            company.positions = new Dictionary<string, Position>();
            company.jobOffers = new Dictionary<string, JobOffer>();
            company.industries = new Dictionary<string, int>();
            Employee employee = new Employee();
            employee.userID = CEO.Id.ToString();
            employee.salary = 0;
            employee.wage = 0;
            employee.wageEarned = 0;
            Position position = new Position();
            position.ID = "CEO";
            position.level = 99;
            position.manages = 7;
            position.name = "CEO";
            employee.position = position;
            company.employee.Add(CEO.Id.ToString(), employee);
            company.positions.Add(position.ID, position);
            await CompanyService.setCompany(company);
            JObject user = await dataBaseService.getJObjectAsync(Context.User.Id.ToString(), "users");
            Collection<string> corps = new Collection<string>();
            try {
                foreach (string x in (Array)user["corps"])
                {
                    corps.Add(x);
                }
            } catch { }
            corps.Add(ticker);
            await dataBaseService.SetFieldAsync(Context.User.Id.ToString(), "corps", JArray.FromObject(corps.ToArray()), "users");
            await ReplyAsync("Company successfully created!");
        }

        [Command("companies")]
        [Alias("corporations")]
        [Summary("Lists all companies.")]
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
            Collection<EmbedFieldBuilder> companyEmbedList = new Collection<EmbedFieldBuilder>();

            foreach (string ID in ids)
            {
                Company temp = new Company(await dataBaseService.getJObjectAsync(ID, "companies"));

                EmbedFieldBuilder tempEmb = new EmbedFieldBuilder().WithIsInline(true).WithName($"{temp.name} ({temp.id})").WithValue($"Share Price: ${Math.Round(temp.SharePrice, 2)}. Total Value: ${Math.Round(temp.SharePrice * temp.shares, 2)}. Amount of Shares: {await MarketService.CorpShares(temp.id)}");

                companyEmbedList.Add(tempEmb);
            }

            EmbedBuilder embed = new EmbedBuilder().WithColor(Color.Gold).WithTitle("Companies").WithDescription("This is a list of all companies").WithFooter($"Total amount of companies: {companyCount}");

            foreach (EmbedFieldBuilder field in companyEmbedList)
            {
                embed.AddField(field);
            }

            await CommandService.PostEmbedTask(Context.Channel.Id.ToString(), embed.Build());
        }

        [Command("addposition")]
        [Summary("Changes the position of the employee.")]
        public async Task AddPositionToPlayer([Summary("The employee")]IUser user, [Summary("Company ticker")]string companyTicker, [Summary("Position ID.")]string positionID)
        {
            Company company = await CompanyService.getCompany(companyTicker);
            if (!company.employee.ContainsKey(Context.User.Id.ToString()))
                await ReplyAsync("You are not part of this corporation!");
            if (e.Contains(company.employee[Context.User.Id.ToString()].position.manages))
            {
                if (!company.employee.ContainsKey(user.Id.ToString()))
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

        [Command("deletecompany")]
        [Alias("deletecorporation", "removecorporation", "removecompany")]
        [HasMasterOfBots]
        public async Task RemoveCorpAsync(string ticker)
        {
            Collection<string> tickers = await dataBaseService.getIDs("companies");

            if (!tickers.Contains(ticker))
            {
                await ReplyAsync("That company does not exist");
            }
            else
            {
                await dataBaseService.RemoveObjectAsync(ticker, "companies");

                Collection<string> shareholderIDs = await dataBaseService.getIDs("shares");

                foreach (string ID in shareholderIDs)
                {
                    UserShares shares = new UserShares(await dataBaseService.getJObjectAsync(ID, "shares"), true);

                    shares.ownedShares.Remove(ticker);

                    await dataBaseService.RemoveObjectAsync(ID, "shares");
                    await dataBaseService.SetJObjectAsync(dataBaseService.SerializeObject<UserShares>(shares), "shares");
                }

                await ReplyAsync("Company deleted");
            }
        }

        [Command("createPosition")]
        [Summary("Create a position.")]
        public async Task createPosition([Summary("Company ticker.")]string ticker, [Summary("ID of the new position.")]string positionID, [Summary("Level in the hierarchy of the compan(1-99).")]int level, [Summary("Permissions(7 is all permitted, 0 is no permissions).")]int manages, [Summary("Position name.")][Remainder] string name)
        {
            Company company = await CompanyService.getCompany(ticker);
            if (!company.employee.ContainsKey(Context.User.Id.ToString()))
            {
                await ReplyAsync($"You are not an employee in {company.name}");
                return;
            }
            if (!w.Contains(company.employee[Context.User.Id.ToString()].position.manages))
            {
                await ReplyAsync("You do not have the permission to make/manage positions.");
                return;
            }
            if (manages > 7 || manages < 0)
            {
                await ReplyAsync("Manages must be in range of 0 to 7.");
                return;
            }
            Position pos = new Position();
            pos.ID = positionID;
            pos.level = level;
            pos.manages = manages;
            pos.name = name;
            company.positions.Add(positionID, pos);
            await CompanyService.setCompany(company);
            await ReplyAsync($"{name} successfully created.");
        }

        [Command("hire")]
        [Summary("Hire an user.")]
        public async Task hireEmployee([Summary("User you wish to hire.")]IUser user, [Summary("Company ticker.")]string ticker, [Summary("The ID of the position you want to hire the user to.")]string positionid, [Summary("Desired wage.")]int wage, [Summary("Desired salary.")]int salary)
        {
            Company company = await CompanyService.getCompany(ticker);


            if (!company.employee.ContainsKey(Context.User.Id.ToString()))
            {
                await ReplyAsync($"You are not an employee in {company.name}");
                return;
            }
            if (!r.Contains(company.employee[Context.User.Id.ToString()].position.manages)) {
                await ReplyAsync("You do not have the permission to fire/hire positions.");
                return;
            }
            if (!company.positions.ContainsKey(positionid))
            {
                await ReplyAsync("The position id you specified is invalid.");
                return;
            }
            if (company.employee[Context.User.Id.ToString()].position.level < company.positions[positionid].level)
            {
                await ReplyAsync("You cannot give someone a higher role than you have.");
                return;
            }
            if (wage < 0 || salary < 0)
            {
                await ReplyAsync("Wage or salary cannot be less than 0");
                return;
            }
            JobOffer offer = new JobOffer();
            offer.user = user.Id.ToString();
            offer.positionid = positionid;
            offer.wage = wage;
            offer.salary = salary;
            string inviteID = Guid.NewGuid().ToString();
            company.jobOffers.Add(inviteID, offer);
            await CompanyService.setCompany(company);
            IDMChannel dm = await user.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync($"You have been invited to work for {company.name} as {company.positions[positionid].name}. To accept, type &acceptjob {ticker} {inviteID}");
            await ReplyAsync($"{user.Username} has been invited to {company.name} to work as {company.positions[positionid].name}.");
        }

        [Command("acceptjob")]
        [Summary("Accept a job offer.")]
        public async Task acceptJob([Summary("Company ticker.")] string ticker, [Summary("Invite ID.")] string inviteID)
        {
            Company company = await CompanyService.getCompany(ticker);
            if (!company.jobOffers.ContainsKey(inviteID))
            {
                await ReplyAsync("Invalid invite ID.");
                return;
            }
            if (company.jobOffers[inviteID].user != Context.User.Id.ToString())
            {
                await ReplyAsync("Invalid invite ID.");
                return;
            }
            Employee employee = new Employee();
            JobOffer offer = company.jobOffers[inviteID];
            employee.position = company.positions[offer.positionid];
            employee.userID = offer.user;
            employee.wage = offer.wage;
            employee.salary = offer.salary;
            company.employee.Add(Context.User.Id.ToString(), employee);
            company.jobOffers.Remove(inviteID);
            await CompanyService.setCompany(company);
            JObject user = await dataBaseService.getJObjectAsync(Context.User.Id.ToString(), "users");
            Collection<string> corps = new Collection<string>();
            try
            {
                foreach (string x in (Array)user["corps"])
                {
                    corps.Add(x);
                }
            }
            catch { }
            corps.Add(ticker);
            await dataBaseService.SetFieldAsync(Context.User.Id.ToString(), "corps", JArray.FromObject(corps.ToArray()), "users");
            await CompanyService.setCompany(company);
            await ReplyAsync($"You are now part of {company.name}!");
        }

        [Command("modifyemployee")]
        [Summary("Lets you modify an employees salary.")]
        public async Task modifyEmployee([Summary("The user you want to modify")]IUser user, [Summary("Company ticker.")] string ticker, [Summary("The wage you want to set to.")]int wage, [Summary("Salary you want to set to")]int salary)
        {
            Company company = await CompanyService.getCompany(ticker);
            if (!company.employee.ContainsKey(Context.User.Id.ToString()))
            {
                await ReplyAsync($"You are not an employee in {company.name}");
                return;
            }
            if (!r.Contains(company.employee[Context.User.Id.ToString()].position.manages))
            {
                await ReplyAsync("You do not have the permission to make/manage employees.");
                return;
            }
            if(!company.employee.ContainsKey(user.Id.ToString()))
            {
                await ReplyAsync($"{user.Username} is not an employee in {company.name}");
                return;
            }
            if(company.employee[user.Id.ToString()].position.level > company.employee[Context.User.Id.ToString()].position.level)
            {
                await ReplyAsync("You cannot modify an employee higher than you.");
                return;
            }
            if(user.Id == Context.User.Id)
            {
                await ReplyAsync("You cannot modify yourself.");
                return;
            }
            company.employee[user.Id.ToString()].salary = salary;
            company.employee[user.Id.ToString()].wage = wage;
            await CompanyService.setCompany(company);
            await ReplyAsync("Employee modified successfully.");
        }
    }
}
