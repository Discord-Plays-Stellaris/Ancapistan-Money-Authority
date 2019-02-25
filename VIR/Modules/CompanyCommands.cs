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
using VIR.Properties;

// MANY THINGS WERE COMMENTED OUT DUE TO
// THE FACT THAT THEY REFERED TO NON-EXISTANT
// VARIABLES AND I COULDN'T COMPILE.
// PLEASE CHECK THROUGHT AND UNCOMMENT
// THINGS ONCE YOU FIX IT.
// --Skipper

// IT ALL WAS PROBABLY VISUAL STUDIO BEING SHITE
// PING ME NEXT TIME
// --Towergame

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
        private readonly ulong guild;
        public readonly List<int> r = new List<int> { 4, 5, 6, 7 };
        public readonly List<int> w = new List<int> { 2, 3, 6, 7 };
        public readonly List<int> e = new List<int> { 1, 3, 5, 7 };

        public CompanyCommands(CompanyService com, DataBaseHandlingService db, CommandHandlingService comm, StockMarketService markserv)
        {
            CompanyService = com;
            dataBaseService = db;
            CommandService = comm;
            MarketService = markserv;
            guild = ulong.Parse(Resources.guild);
#if DEBUG
            guild = ulong.Parse(Resources.devguild);
#endif
        }

        [Command("createcompany")]
        [Alias("createcorporation", "addcompany", "addcorporation")]
        [HasMasterOfBots]
        [IsInDPSGuild]
        public async Task CreateCompanyTask(IUser CEO, string ticker, [Remainder]string name)
        {
            Company company = new Company();
            company.name = name;
            company.shares = 0;
            company.id = ticker;
            company.employee = new Dictionary<string, Employee>();
            company.positions = new Dictionary<string, Position>();
            company.jobOffers = new Dictionary<string, JobOffer>();
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
            try
            {
                foreach (string x in user["corps"].ToArray())
                {
                    corps.Add(x);
                }
            }
            catch { }
            corps.Add(ticker);
            await dataBaseService.SetFieldAsync(Context.User.Id.ToString(), "corps", corps, "users");
            await ReplyAsync("Company successfully created!");
        }

        [Command("companies")]
        [Alias("corporations")]
        [Summary("Lists all companies.")]
        public async Task GetCompaniesTask()
        {

            Collection<string> ids = await dataBaseService.getIDs("companies");
            int companyCount = ids.Count;
            Collection<EmbedFieldBuilder> companyEmbedList = new Collection<EmbedFieldBuilder>();

            foreach (string ID in ids)
            {
                Company temp = new Company(await dataBaseService.getJObjectAsync(ID, "companies"));

                EmbedFieldBuilder tempEmb = new EmbedFieldBuilder().WithIsInline(true).WithName($"{temp.name} ({temp.id})").WithValue($"Share Price: ${Math.Round(temp.SharePrice, 2)}. Total Value: ${Math.Round(temp.SharePrice * temp.shares, 2)}. Amount of Shares: {await MarketService.CorpShares(temp.id)}");
                if (companyEmbedList.Count < 25)
                {
                    companyEmbedList.Add(tempEmb);
                }
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
                }
                else
                {
                    if (company.positions.ContainsKey(positionID))
                    {
                        if (company.positions[positionID].level > company.employee[Context.User.Id.ToString()].position.level)
                            await ReplyAsync("You cannot add a position higher than you.");
                        company.employee[user.Id.ToString()].position = company.positions[positionID];
                        await ReplyAsync($"Successfully changed {user.Mention} to position of {company.positions[positionID].name}");
                    }
                    else
                    {
                        await ReplyAsync("The position id you specified is invalid.");
                    }
                }
            } else
            {
                await ReplyAsync("You are not allowed to manage positions.");
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
        public async Task createPosition([Summary("Company ticker.")]string ticker, [Summary("ID of the new position.")]string positionID, [Summary("Level in the hierarchy of the company(1-99).")]int level, [Summary("Permissions(7 is all permitted, 0 is no permissions).")]int manages, [Summary("Position name.")][Remainder] string name)
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
            if (!r.Contains(company.employee[Context.User.Id.ToString()].position.manages))
            {
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
                foreach (string x in user["corps"].ToArray())
                {
                    corps.Add(x);
                }
            }
            catch { }
            corps.Add(ticker);
            await dataBaseService.SetFieldAsync(Context.User.Id.ToString(), "corps", JArray.FromObject(corps.ToArray()), "users");
            if (((string)await dataBaseService.GetFieldAsync(Context.User.Id.ToString(), "maincorp", "users")) == null)
            {
                await dataBaseService.SetFieldAsync(Context.User.ToString(), "maincorp", ticker, "users");
                if (company.role != null)
                {
                    await Context.Client.GetGuild(guild).GetUser(Context.User.Id).AddRoleAsync(Context.Client.GetGuild(guild).GetRole(ulong.Parse(company.role))); //481865251688808469
                }
            }
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
            if (!company.employee.ContainsKey(user.Id.ToString()))
            {
                await ReplyAsync($"{user.Username} is not an employee in {company.name}");
                return;
            }
            if (company.employee[user.Id.ToString()].position.level > company.employee[Context.User.Id.ToString()].position.level)
            {
                await ReplyAsync("You cannot modify an employee higher than you.");
                return;
            }
            if (user.Id == Context.User.Id)
            {
                await ReplyAsync("You cannot modify yourself.");
                return;
            }
            company.employee[user.Id.ToString()].salary = salary;
            company.employee[user.Id.ToString()].wage = wage;
            await CompanyService.setCompany(company);
            await ReplyAsync("Employee modified successfully.");
        }

        [Command("bindrole")]
        [Summary("Binds a role to a corp membership.")]
        [HasMasterOfBots]
        public async Task bindRole([Summary("The ticker of the target company")]string ticker, [Summary("Role to bind")]IRole rrole)
        {
            Company company = await CompanyService.getCompany(ticker);
            company.role = rrole.Id.ToString();
            foreach (Employee x in company.employee.Values)
            {
                if (((string)await dataBaseService.GetFieldAsync(x.userID, "maincorp", "users")) == ticker)
                {
                    await Context.Guild.GetUser(ulong.Parse(x.userID)).AddRoleAsync(Context.Guild.GetRole(ulong.Parse(company.role)));
                }
            }
            await CompanyService.setCompany(company);
            await ReplyAsync("Role been bound.");
        }
        [Command("unbindrole")]
        [Summary("Unbinds a role to a corp membership.")]
        [HasMasterOfBots]
        public async Task unbindRole([Summary("The ticker of the target company")]string ticker)
        {
            Company company = await CompanyService.getCompany(ticker);
            foreach (Employee x in company.employee.Values)
            {
                await Context.Guild.GetUser(ulong.Parse(x.userID)).RemoveRoleAsync(Context.Guild.GetRole(ulong.Parse(company.role)));
            }
            company.role = null;
            await CompanyService.setCompany(company);
            await ReplyAsync("Role been unbound.");
        }
        [Command("employed")]
        [Summary("Gets a list of companies you are employed in.")]
        public async Task employedList()
        {
            JObject user = await dataBaseService.getJObjectAsync(Context.User.Id.ToString(), "users");
            Collection<string> corps = new Collection<string>();
            foreach (string x in user["corps"].ToArray())
            {
                corps.Add(x);
            }
            EmbedBuilder embed = new EmbedBuilder().WithColor(Color.Orange).WithTitle($"Corporations which currently employ {Context.Guild.GetUser(Context.User.Id).Nickname}").WithDescription($"Total of {corps.Count} corps");
            foreach (string x in corps)
            {
                Company comp = await CompanyService.getCompany(x);
                embed.AddField(new EmbedFieldBuilder().WithName(comp.name).WithValue(comp.employee[Context.User.Id.ToString()].position.name));
            }
            await ReplyAsync(null, false, embed.Build());
        }
        [Command("pickmaincorporation")]
        [Summary("Picks a main corporation(among the ones you are in)")]
        public async Task pickMainCorp([Summary("Company Ticker")] string ticker)
        {
            JObject user = await dataBaseService.getJObjectAsync(Context.User.Id.ToString(), "users");
            Collection<string> corps = new Collection<string>();
            foreach (string x in user["corps"].ToArray())
            {
                corps.Add(x);
            }
            if (!corps.Contains(ticker))
            {
                await ReplyAsync("You cannot make a corp you are not part of your main!");
            }
            else
            {
                if (user["maincorp"] != null)
                {
                    Company comp2 = await CompanyService.getCompany((string)user["maincorp"]);
                    if (comp2.role != null)
                        await Context.Client.GetGuild(guild).GetUser(Context.User.Id).RemoveRoleAsync(Context.Client.GetGuild(guild).GetRole(ulong.Parse(comp2.role)));
                    if (comp2.employee[Context.User.Id.ToString()].position.name == "CEO")
                        await Context.Client.GetGuild(guild).GetUser(Context.User.Id).RemoveRoleAsync(Context.Client.GetGuild(guild).GetRole((ulong)533379268906975232));
                }
                user["maincorp"] = ticker;
                await dataBaseService.SetJObjectAsync(user, "users");
                Company company = await CompanyService.getCompany(ticker);
                if (company.role != null)
                    await Context.Client.GetGuild(guild).GetUser(Context.User.Id).AddRoleAsync(Context.Client.GetGuild(guild).GetRole(ulong.Parse(company.role)));
                if (company.employee[Context.User.Id.ToString()].position.name == "CEO")
                    await Context.Client.GetGuild(guild).GetUser(Context.User.Id).AddRoleAsync(Context.Client.GetGuild(guild).GetRole((ulong)533379268906975232));
                await ReplyAsync("Main Corporation set to " + ticker);
            }
        }
        [Command("requestjob")]
        [Summary("Requests to work at a company.")]
        public async Task requestJob([Summary("Company ticker where you wish to work.")] string ticker, [Summary("Wage you wish to have.")] int wage = 0, [Summary("Salary you wish to have")] int salary = 0)
        {
            Company company = await CompanyService.getCompany(ticker);
            JobRequest request = new JobRequest();
            request.wage = wage;
            request.salary = salary;
            request.user = Context.User.Id.ToString();
            string id = Guid.NewGuid().ToString();
            if (company.jobRequests == null)
            {
                company.jobRequests = new Dictionary<string, JobRequest>();
            }
            company.jobRequests.Add(id, request);
            IDMChannel chan = await Context.Client.GetUser(ulong.Parse(company.employee.FirstOrDefault(x => x.Value.position.ID == "CEO").Value.userID)).GetOrCreateDMChannelAsync();
            EmbedBuilder embed = new EmbedBuilder().WithTitle("A new job request has come in.").WithDescription($"Job Request Author: {Context.User.Username}#{Context.User.Discriminator}").AddField(new EmbedFieldBuilder().WithName("Minimum required salary:").WithValue(salary)).AddField(new EmbedFieldBuilder().WithName("Minimum required wage:").WithValue(wage)).WithColor(Color.Orange).WithFooter(new EmbedFooterBuilder().WithText($"To accept, type &acceptrequest {ticker} {id} [desired salary] [desired wage] [desired position ID]"));
            await chan.SendMessageAsync(null, false, embed.Build());
            await ReplyAsync("Your job request has been sent in.");
            await CompanyService.setCompany(company);
        }
        [Command("acceptrequest")]
        [Summary("Accepts a job request")]
        public async Task acceptRequest([Summary("Company ticker")] string ticker, [Summary("UUID of the request")] string id, [Summary("Desired salary")]int salary, [Summary("Desired wage")]int wage, [Summary("Desired position to grant(id of position)")] string positionid)
        {
            Company company = await CompanyService.getCompany(ticker);
            if (company.employee.ContainsKey(Context.User.Id.ToString()))
            {
                if (r.Contains(company.employee[Context.User.Id.ToString()].position.manages))
                {
                    if (salary >= company.jobRequests[id].salary && wage >= company.jobRequests[id].wage)
                    {
                        if (company.positions.ContainsKey(positionid))
                        {
                            if (company.positions[positionid].level < company.employee[Context.User.Id.ToString()].position.level)
                            {
                                Employee employee = new Employee();
                                employee.salary = salary;
                                employee.wage = wage;
                                employee.position = company.positions[positionid];
                                employee.wageEarned = 0;
                                employee.userID = company.jobRequests[id].user;
                                company.employee.Add(company.jobRequests[id].user, employee);
                                if ((await dataBaseService.getJObjectAsync(employee.userID, "users"))["maincorp"] == null)
                                {
                                    await dataBaseService.SetFieldAsync(employee.userID, "maincorp", ticker, "users");
                                    if (company.role != null)
                                        await Context.Client.GetGuild(guild).GetUser(ulong.Parse(employee.userID)).RemoveRoleAsync(Context.Client.GetGuild(guild).GetRole(ulong.Parse(company.role)));
                                }
                                JObject user = await dataBaseService.getJObjectAsync(employee.userID, "users");
                                Collection<string> corps = new Collection<string>();
                                try
                                {
                                    foreach (string x in user["corps"].ToArray())
                                    {
                                        corps.Add(x);
                                    }
                                }
                                catch { }
                                corps.Add(ticker);
                                await dataBaseService.SetFieldAsync(Context.User.Id.ToString(), "corps", JArray.FromObject(corps.ToArray()), "users");
                                await CompanyService.setCompany(company);
                                await (await Context.Client.GetUser(ulong.Parse(employee.userID)).GetOrCreateDMChannelAsync()).SendMessageAsync("Your request to work at " + company.name + " has been accepted.");
                                await ReplyAsync("User has been accepted to work in " + company.name);
                            }
                            else
                            {
                                await ReplyAsync("You cannot grant positions higher than you!");
                            }
                        }
                        else
                        {
                            await ReplyAsync("Invalid position ID.");
                        }
                    }
                    else
                    {
                        await ReplyAsync("You cannot hire for less wage than requested, consider denying the request using &denyrequest " + ticker + " " + id);
                    }
                }
                else
                {
                    await ReplyAsync("You do not have the permission to manage employees.");
                }
            }
            else
            {
                await ReplyAsync("You cannot accept job requests in a company you don't work in.");
            }
        }
        [Command("denyrequest")]
        [Summary("Denies a job request")]
        public async Task denyRequest([Summary("Company ticker")] string ticker, [Summary("Offer UUID")] string id)
        {
            Company company = await CompanyService.getCompany(ticker);
            if (company.employee.ContainsKey(Context.User.Id.ToString()))
            {
                if (r.Contains(company.employee[Context.User.Id.ToString()].position.manages))
                {
                    string userID = company.jobRequests[id].user;
                    company.jobRequests.Remove(id);
                    await (await Context.Client.GetUser(ulong.Parse(userID)).GetOrCreateDMChannelAsync()).SendMessageAsync("Your request to work at " + company.name + " has been denied.");
                    await CompanyService.setCompany(company);
                    await ReplyAsync("Job Request denied.");
                }
                else
                {
                    await ReplyAsync("You do not have the permission to manage employees.");
                }
            }
            else
            {
                await ReplyAsync("You cannot deny job requests in a company you don't work in.");
            }
        }
        [Command("fire")]
        [Summary("Fires someone from the company.")]
        public async Task fire([Summary("Company ticker")] string ticker, [Summary("Person to fire")]IUser user, [Summary("Reason, if any")][Remainder] string reason = "")
        {
            Company company = await CompanyService.getCompany(ticker);
            if (company.employee.ContainsKey(Context.User.Id.ToString()))
            {
                if (r.Contains(company.employee[Context.User.Id.ToString()].position.manages))
                {
                    if (company.employee[user.Id.ToString()].position.level < company.employee[Context.User.Id.ToString()].position.level)
                    {
                        company.employee.Remove(user.Id.ToString());
                        await CompanyService.setCompany(company);
                        if (reason != "")
                        {
                            await ReplyAsync($"{user.Username} has been fired from {company.name} with reason {reason}");
                            await user.GetOrCreateDMChannelAsync().GetAwaiter().GetResult().SendMessageAsync($"You have been fired from {company.name} with reason {reason}");
                        }
                        else
                        {
                            await ReplyAsync($"{user.Username} has been fired from {company.name}");
                            await user.GetOrCreateDMChannelAsync().GetAwaiter().GetResult().SendMessageAsync($"You have been fired from {company.name} with no reason.");
                        }
                    }
                    else
                    {
                        await ReplyAsync("You cannot fire someone higher than you.");
                    }
                }
                else
                {
                    await ReplyAsync("You do not have the permission to manage employees.");
                }
            }
            else
            {
                await ReplyAsync("You cannot fire people in a company you don't work in.");
            }
        }
        [Command("leave")]
        [Summary("Leaves a company.")]
        public async Task leave([Summary("Company ticker")] string ticker, [Summary("Reason, if any")][Remainder] string reason = "")
        {
            Company company = await CompanyService.getCompany(ticker);
            IUser user = Context.User;
            if (company.employee.ContainsKey(Context.User.Id.ToString()))
            {
                company.employee.Remove(user.Id.ToString());
                await CompanyService.setCompany(company);
                if (reason != "")
                {
                    await ReplyAsync($"You have left {company.name} with reason {reason}");
                    await Context.Client.GetUser(ulong.Parse(company.employee.FirstOrDefault(x => x.Value.position.ID == "CEO").Value.userID))
                        .GetOrCreateDMChannelAsync().GetAwaiter().GetResult()
                        .SendMessageAsync($"{user.Username} has left {company.name} with reason {reason}");
                }
                else
                {
                    await ReplyAsync($"{user.Username} has been fired from {company.name}");
                    await Context.Client.GetUser(ulong.Parse(company.employee.FirstOrDefault(x => x.Value.position.ID == "CEO").Value.userID))
                        .GetOrCreateDMChannelAsync().GetAwaiter().GetResult()
                        .SendMessageAsync($"{user.Username} has left {company.name} with no reason.");
                }
            }
            else
            {
                await ReplyAsync("You cannot fire people in a company you don't work in.");
            }
        }
        [Command("employees")]
        [Summary("Lists all employees in a corp and their positions.")]
        public async Task employeeList([Summary("The ticker of the target corporation")] string ticker)
        {
            Company company = await CompanyService.getCompany(ticker);
            if (company.employee.ContainsKey(Context.User.Id.ToString()))
            {
                if (r.Contains(company.employee[Context.User.Id.ToString()].position.manages))
                {
                    EmbedBuilder embed = new EmbedBuilder()
                        .WithColor(Color.Orange)
                        .WithTitle("Employees working at " + company.name)
                        .WithDescription("Sorted by their level in the company.");
                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
                    List<Employee> employees = company.employee.Values.ToList();
                    employees.Sort(company.CompareEmployees);
                    foreach (Employee x in employees)
                    {
                        if(fields.Count < 26)
                        {
                            fields.Add(new EmbedFieldBuilder().WithName(Context.Client.GetUser(ulong.Parse(x.userID)).Username).WithValue("Working as " + x.position.name).WithIsInline(true));
                        }
                    }
                    embed.WithFields(fields);
                    await ReplyAsync(null, false, embed.Build());
                }
                else
                {
                    await ReplyAsync("You do not have the permission to manage employees.");
                }
            }
            else
            {
                await ReplyAsync("You cannot list people in a company you don't work in.");
            }
        }
        [Command("positions")]
        [Summary("Lists all positions in a corp and their IDs.")]
        public async Task positionList([Summary("The ticker of the target corporation")] string ticker)
        {
            Company company = await CompanyService.getCompany(ticker);
            if (company.employee.ContainsKey(Context.User.Id.ToString()))
            {
                if (w.Contains(company.employee[Context.User.Id.ToString()].position.manages))
                {
                    EmbedBuilder embed = new EmbedBuilder()
                        .WithColor(Color.Orange)
                        .WithTitle("Positions in " + company.name)
                        .WithDescription("Sorted by their level in the company.");
                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
                    List<Position> positions = company.positions.Values.ToList();
                    positions.Sort(company.ComparePositions);
                    foreach (Position x in positions)
                    {
                        if (fields.Count < 26)
                        {
                            fields.Add(new EmbedFieldBuilder().WithName(x.name).WithValue("ID: " + x.ID).WithIsInline(true));
                        }
                    }
                    embed.WithFields(fields);
                    await ReplyAsync(null, false, embed.Build());
                }
                else
                {
                    await ReplyAsync("You do not have the permission to manage positions.");
                }
            }
            else
            {
                await ReplyAsync("You cannot list positions in a company you don't work in.");
            }
        }
    }
}
