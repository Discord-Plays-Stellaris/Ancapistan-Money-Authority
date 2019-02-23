using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VIR.Modules.Objects.Company;
using VIR.Modules.Preconditions;
using VIR.Objects;
using VIR.Objects.Company;
using VIR.Services;

namespace VIR.Modules
{
    public class IndustryCommands : ModuleBase
    {
        private readonly CompanyService _companyService;
        private readonly DataBaseHandlingService _dataBaseService;
        private readonly CommandHandlingService _commandService;
        private readonly StockMarketService _marketService;
        private readonly IndustryService _industryService;

        public IndustryCommands(IndustryService ind, CompanyService com, DataBaseHandlingService db, CommandHandlingService comm, StockMarketService markserv)
        {
            _companyService = com;
            _dataBaseService = db;
            _commandService = comm;
            _marketService = markserv;
            _industryService = ind;
        }

        [Command("work")]
        public async Task WorkAtIndustry(string industryId, int utilSpent)
        {
            var response = _industryService.WorkAtIndustryForUtils(industryId, utilSpent, Context.User.Id.ToString());
            await ReplyAsync(response);
        }

        //Fancy shit
        //if this breaks, blame tower
        [Command("buyoffer")]
        [Alias("buyindustry")]
        [Summary("Set up an offer to buy")]
        public async Task BuyOfferAsync([Summary("Company ticker")]string ticker, [Summary("Industry ID")]string industryID, [Summary("Price")]double price)
        {
            string AuthorMoneyt = (string)await _dataBaseService.GetFieldAsync(Context.User.Id.ToString(), "money", "users");
            double AuthorMoney;

            if (price < 0)
            {
                await ReplyAsync("You cant sell for a negative amount of money!");
                return;
            }

            if (AuthorMoneyt == null)
            {
                AuthorMoney = 50000;
                await _dataBaseService.SetFieldAsync(Context.User.Id.ToString(), "money", AuthorMoney, "users");
            }
            else
            {
                AuthorMoney = double.Parse(AuthorMoneyt);
            }

            Industry industry = new Industry(await _dataBaseService.getJObjectAsync(industryID, "industries"));
            Company company = await _companyService.getCompany(ticker);
            if (!company.employee.Keys.Contains(Context.User.Id.ToString()))
            {
                await ReplyAsync("You cannot buy industries for corps you are not an employee of.");
                return;
            }
            if (!new List<int> { 1, 3, 5, 7 }.Contains(company.employee[Context.User.Id.ToString()].position.manages))
            {
                await ReplyAsync($"You do not have the permission to buy industry in {company.name}");
            }
            IndustryTransaction transaction = new IndustryTransaction(price, "buy", Context.User.Id.ToString(), industryID, _dataBaseService, _commandService);

            try
            {
                JObject tmp = _dataBaseService.SerializeObject(transaction);
                await _dataBaseService.SetJObjectAsync(tmp, "transactions");
                await ReplyAsync($"Sell offer lodged in <#{await _dataBaseService.GetFieldAsync("MarketChannel", "channel", "system")}>");
            }
            catch (Exception e)
            {
                await Log.Logger(Log.Logs.ERROR, e.Message);
                await ReplyAsync("Something went wrong: " + e.Message);
            }
        }

        [Command("selloffer")]
        [Alias("sellindustry")]
        [Summary("Put up industry for sale.")]
        public async Task SellOfferAsync([Summary("The id of the industry you wish to sell")] string industryID, [Summary("The price for which you wish to sell")]int price)
        {
            string AuthorMoneyt = (string)await _dataBaseService.GetFieldAsync(Context.User.Id.ToString(), "money", "users");
            double AuthorMoney;

            if (price < 0)
            {
                await ReplyAsync("You cant sell for a negative amount of money!");
                return;
            }

            if (AuthorMoneyt == null)
            {
                AuthorMoney = 50000;
                await _dataBaseService.SetFieldAsync(Context.User.Id.ToString(), "money", AuthorMoney, "users");
            }
            else
            {
                AuthorMoney = double.Parse(AuthorMoneyt);
            }

            Industry industry = new Industry(await _dataBaseService.getJObjectAsync(industryID, "industries"));
            Company company = await _companyService.getCompany(industry.CompanyId);
            if(!company.employee.Keys.Contains(Context.User.Id.ToString()))
            {
                await ReplyAsync("You cannot sell industries you do not own.");
                return;
            }
            if(!new List<int> { 1, 3, 5, 7 }.Contains(company.employee[Context.User.Id.ToString()].position.manages))
            {
                await ReplyAsync($"You do not have the permission to sell industry in {company.name}");
            }
            IndustryTransaction transaction = new IndustryTransaction(price, "sell", Context.User.Id.ToString(), industryID, _dataBaseService, _commandService);

            try
            {
            JObject tmp = _dataBaseService.SerializeObject(transaction);
            await _dataBaseService.SetJObjectAsync(tmp, "transactions");
            await ReplyAsync($"Sell offer lodged in <#{await _dataBaseService.GetFieldAsync("MarketChannel", "channel", "system")}>");
            }
            catch (Exception e)
            {
            await Log.Logger(Log.Logs.ERROR, e.Message);
            await ReplyAsync("Something went wrong: " + e.Message);
            }
        }
        [Command("putonmarket")]
        [Alias("putindustry")]
        [Summary("Puts an unclaimed industry up for sale")]
        [HasMasterOfBots]
        public async Task PutOnMarket([Summary("Industry ID")] string industryID, [Summary("Price")]double price, [Summary("Hours for the auction to last")]int hours, [Summary("Minutes for the auction to last")]int minutes)
        {
            Industry industry = new Industry(await _dataBaseService.getJObjectAsync(industryID, "industries"));
            if(industry.CompanyId.Length > 0)
            {
                await ReplyAsync("You cannot sell an industry owned by someone.");
                return;
            }
            IndustryAuction transaction = new IndustryAuction(price, "sell", industryID, _dataBaseService, _commandService, _marketService, hours, minutes);

            try
            {
                await _dataBaseService.SetJObjectAsync(transaction.SerializeIntoJObject(), "transactions");
                await ReplyAsync($"Auction lodged in <#{await _dataBaseService.GetFieldAsync("MarketChannel", "channel", "system")}>");
            }
            catch (Exception e)
            {
                await Log.Logger(Log.Logs.ERROR, e.Message);
                await ReplyAsync("Something went wrong: " + e.Message);
            }

        }
        [Command("accept")]
        [Alias("acceptoffer")]
        [Summary("Accepts an offer")]
        public async Task AcceptOfferAsync([Summary("Company Ticker")]string ticker, [Summary("The ID of the offer.")]string offerID)
        {
            Collection<string> IDs = await _dataBaseService.getIDs("transactions");
            string userMoneyt;
            string authorMoneyt;
            double userMoney;
            double authorMoney;

            if (IDs.Contains(offerID) == true)
            {
                IndustryTransaction transaction = new IndustryTransaction(await _dataBaseService.getJObjectAsync(offerID, "transactions"));
                JObject obj = await _dataBaseService.getJObjectAsync(transaction.industryID, "industries");
                Industry ind = new Industry(obj);
                Company exeCom = await _companyService.getCompany(ticker);
                Company transCom = null;
                if (transaction.type == "auction")
                    await ReplyAsync("You must bid for auctions.");
                if (transaction.type == "buy")
                {
                    transCom = await _companyService.getCompany(ind.CompanyId);
                    if (!exeCom.employee.Keys.Contains(Context.User.Id.ToString()))
                    {
                        await ReplyAsync("You cannot sell industries for corps you are not an employee of.");
                        return;
                    }
                    if (!new List<int> { 1, 3, 5, 7 }.Contains(exeCom.employee[Context.User.Id.ToString()].position.manages))
                    {
                        await ReplyAsync($"You do not have the permission to sell industry in {exeCom.name}");
                    }
                }
                if (!exeCom.employee.Keys.Contains(Context.User.Id.ToString()))
                {
                    await ReplyAsync("You cannot buy industries for corps you are not an employee of.");
                    return;
                }
                if (!new List<int> { 1, 3, 5, 7 }.Contains(exeCom.employee[Context.User.Id.ToString()].position.manages))
                {
                    await ReplyAsync($"You do not have the permission to buy industry in {exeCom.name}");
                }
                if (transaction.author != Context.User.Id.ToString())
                {
                    // Gets money values of the command user and the transaction author
                    userMoneyt = (string)await _dataBaseService.GetFieldAsync(Context.User.Id.ToString(), "money", "users");
                    if (userMoneyt == null)
                    {
                        userMoney = 50000;
                    }
                    else
                    {
                        userMoney = double.Parse(userMoneyt);
                    }

                    authorMoneyt = (string)await _dataBaseService.GetFieldAsync(transaction.author, "money", "users");
                    if (authorMoneyt == null)
                    {
                        authorMoney = 50000;
                    }
                    else
                    {
                        authorMoney = double.Parse(authorMoneyt);
                    }

                    // Transfers the money
                    if (transaction.type == "buy")
                    {
                        userMoney += transaction.price;
                        authorMoney -= transaction.price;
                    }
                    else
                    {
                        userMoney -= transaction.price;
                        authorMoney += transaction.price;
                    }

                    if (transaction.type == "sell")
                    {
                        ind.CompanyId = ticker;
                        await _dataBaseService.SetJObjectAsync(ind.SerializeIntoJObject(), "industries");
                    }
                    else
                    {
                        ind.CompanyId = transCom.id;
                        await _dataBaseService.SetJObjectAsync(ind.SerializeIntoJObject(), "industries");
                    }

                    if (userMoney < 0)
                    {
                        await ReplyAsync("You cannot complete this transaction as it would leave you with a negative amount on money.");
                    }
                    else
                    {

                        await _dataBaseService.SetFieldAsync(Context.User.Id.ToString(), "money", userMoney, "users");
                        await _dataBaseService.SetFieldAsync(transaction.author, "money", authorMoney, "users");
                        await _dataBaseService.RemoveObjectAsync(offerID, "transactions");

                        ITextChannel chnl = (ITextChannel)await Context.Client.GetChannelAsync((UInt64)await _dataBaseService.GetFieldAsync("MarketChannel", "channel", "system"));
                        await chnl.DeleteMessageAsync(Convert.ToUInt64(transaction.messageID));

                        await ReplyAsync("Transaction complete!");
                        await _commandService.PostMessageTask((string)await _dataBaseService.GetFieldAsync("MarketChannel", "channel", "system"), $"<@{transaction.author}>'s Transaction with ID {transaction.id} has been accepted by <@{Context.User.Id}>!");
                        //await transaction.authorObj.SendMessageAsync($"Your transaction with the id {transaction.id} has been completed by {Context.User.Username.ToString()}");
                    }
                }
                else
                {
                    await ReplyAsync("You cannot accept your own transaction!");
                }
            }
            else
            {
                await ReplyAsync("That is not a valid transaction ID");
            }
        }

        [Command("bid")]
        public async Task BidOfferAsync([Summary("Company Ticker")]string ticker, [Summary("The ID of the offer.")]string offerID, [Summary("bid")]double bid)
        {
            Collection<string> IDs = await _dataBaseService.getIDs("transactions");
            string userMoneyt;
            string authorMoneyt;
            double userMoney;
            double authorMoney;

            if (IDs.Contains(offerID) == true)
            {
                IndustryAuction transaction = new IndustryAuction(await _dataBaseService.getJObjectAsync(offerID, "transactions"));
                JObject obj = await _dataBaseService.getJObjectAsync(transaction.industryID, "industries");
                Industry ind = new Industry(obj);
                Company exeCom = await _companyService.getCompany(ticker);
                if (!exeCom.employee.Keys.Contains(Context.User.Id.ToString()))
                {
                    await ReplyAsync("You cannot buy industries for corps you are not an employee of.");
                    return;
                }
                if (!new List<int> { 1, 3, 5, 7 }.Contains(exeCom.employee[Context.User.Id.ToString()].position.manages))
                {
                    await ReplyAsync($"You do not have the permission to buy industry in {exeCom.name}");
                }
                if (transaction.price > bid)
                {
                    await ReplyAsync("Your bid is too low!");
                    return;
                }
                if (transaction.currentUser != Context.User.Id.ToString())
                {
                    // Gets money values of the command user and the transaction author
                    userMoneyt = (string)await _dataBaseService.GetFieldAsync(Context.User.Id.ToString(), "money", "users");
                    if (userMoneyt == null)
                    {
                        userMoney = 50000;
                    }
                    else
                    {
                        userMoney = double.Parse(userMoneyt);
                    }

                    authorMoneyt = (string)await _dataBaseService.GetFieldAsync(transaction.currentUser, "money", "users");
                    if (authorMoneyt == null)
                    {
                        authorMoney = 50000;
                    }
                    else
                    {
                        authorMoney = double.Parse(authorMoneyt);
                    }

                    authorMoney += transaction.price;
                    userMoney -= bid;

                    if (userMoney < 0)
                    {
                        await ReplyAsync("You cannot complete this transaction as it would leave you with a negative amount on money.");
                    }
                    else
                    {

                        await _dataBaseService.SetFieldAsync(Context.User.Id.ToString(), "money", userMoney, "users");
                        await _dataBaseService.SetFieldAsync(transaction.currentUser, "money", authorMoney, "users");
                        string oldWinner = transaction.currentUser;
                        await transaction.Bid(bid, ticker, Context.User.Id.ToString(), _dataBaseService, _commandService);
                        await _dataBaseService.SetJObjectAsync(transaction.SerializeIntoJObject(), "transactions");

                        await ReplyAsync("Transaction complete!");
                        await _commandService.PostMessageTask((string)await _dataBaseService.GetFieldAsync("MarketChannel", "channel", "system"), $"<@{oldWinner}>'s bid in the Auction with ID {transaction.id} has been outbid by <@{Context.User.Id}>!");
                        //await transaction.authorObj.SendMessageAsync($"Your transaction with the id {transaction.id} has been completed by {Context.User.Username.ToString()}");
                    }
                }
                else
                {
                    await ReplyAsync("You cannot accept your own transaction!");
                }
            }
            else
            {
                await ReplyAsync("That is not a valid transaction ID");
            }
        }

        //done
        [Command("createindustry")]
        [Alias("addindustry")]
        [HasMasterOfBots]
        public async Task AddIndustryAsync(string type, ulong monthlyOutput, string planetId)
        {
            /* Types: 
            MNRL - Minerals
            FOOD - Food
            ALLY - Alloys
            CSGD - Consumer Goods
            RFML - Refined Minerals
            RFFD - Refined Food
            */

            var types = new string[] {"MNRL", "FOOD", "ALLY", "CSGD", "RFML", "RFFD"};

            var id = Guid.NewGuid().ToString();

            Industry industry = new Industry(id, type, monthlyOutput, 100, planetId);

            await _dataBaseService.SetJObjectAsync(_dataBaseService.SerializeObject<Industry>(industry), "industries");

            await ReplyAsync($"Industry with ID {id} created");
        }

        //done
        [Command("industries")]
        public async Task IndustryListAsync()
        {
            Collection<string> IDs = await _dataBaseService.getIDs("industries");

            var embed = new EmbedBuilder().WithTitle("List of Industries").WithDescription("This is a list of all industries").WithColor(Color.Blue);

            foreach (string id in IDs)
            {
                Industry industry = new Industry(await _dataBaseService.getJObjectAsync(id, "industries"));

                EmbedFieldBuilder embedField = new EmbedFieldBuilder().WithName(industry.id).WithValue($"Resource Produced: {industry.Type}. Yearly output: {industry.MonthlyOutput}. Belongs to {industry.CompanyId} on planet {industry.Planet}.");
                embed.AddField(embedField);
            }

            await ReplyAsync("", false, embed.Build());
        }

        [Command("getindustries")]
        [HasMasterOfBots]
        public async Task GetIndustriesAsync(string ticker, string id = null)
        {
            var allIndustries = new List<Industry>();
            var ownedIndustries = new List<Industry>();
            var allIndustriesJson = new Collection<JObject>();

            try
            {
                allIndustriesJson = _dataBaseService.getJObjects("industries").Result;
            }
            catch (NullReferenceException e)
            {
                await ReplyAsync("Uh something went wrong with looking it up, please ping a nerd.");
                return;
            }

            foreach (var iJson in allIndustriesJson)
            {
                allIndustries.Add(new Industry(iJson));
            }

            ownedIndustries = allIndustries.Where(x => x.CompanyId == ticker).ToList();

            EmbedBuilder embed = new EmbedBuilder().WithTitle($"All industries owned by {ticker}.")
                .WithDescription($"This is a list of all industries owned by the company with the ticker {ticker}.")
                .WithColor(Color.Blue);

            foreach (var x in ownedIndustries)
            {
                var embedField = new EmbedFieldBuilder().WithName(x.id)
                    .WithValue($"ID: {x.id}. Resource Produced: {x.Type}. Yearly output: {x.MonthlyOutput}. On the planet {x.Planet}.");
                embed.AddField(embedField);
            }

            await ReplyAsync("", false, embed.Build());

        }

        [Command("setindustries")]
        [HasMasterOfBots]
        public async Task SetIndustriesAsync(string ticker, string industryId)
        {
            Industry industry;
            try
            {
                industry = new Industry(_dataBaseService.getJObjectAsync(industryId, "industries").Result);
                
                var companies = _dataBaseService.getIDs("companies").Result;
                if (companies.Contains(ticker))
                {
                    industry.CompanyId = ticker;
                    await _dataBaseService.SetJObjectAsync(industry.SerializeIntoJObject(), "industries");
                    await ReplyAsync($"Successfully set ownership of {industryId} to {ticker}.");
                }

                await ReplyAsync($"Unable to find company with ticker {ticker}.");
            }
            catch (Exception e)
            {
                await ReplyAsync($"Unable to find industry with Id {industryId}.");
            }
        }
    }
}
