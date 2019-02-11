using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using VIR.Modules.Objects.Company;
using VIR.Modules.Preconditions;
using VIR.Objects;
using VIR.Objects.Company;
using VIR.Services;

namespace VIR.Modules
{
    public class ResourceCommands : ModuleBase
    {
        private readonly CompanyService _companyService;
        private readonly DataBaseHandlingService _dataBaseService;
        private readonly CommandHandlingService _commandService;
        private readonly StockMarketService _marketService;
        private readonly ResourceHandlingService _resourceHandlingService;

        public ResourceCommands(CompanyService com, DataBaseHandlingService db, CommandHandlingService comm, StockMarketService markserv, ResourceHandlingService res)
        {
            _companyService = com;
            _dataBaseService = db;
            _commandService = comm;
            _marketService = markserv;
            _resourceHandlingService = res;
        }

        [Command("market")]
        public async Task ViewResourceMarket(string resourceType)
        {
            var types = new string[] {"MNRL", "FOOD", "ALLY", "CSGD", "RFML", "RFFD"};

            if (!types.Contains(resourceType))
            {
                await ReplyAsync($"{resourceType} is not a valid resource type.");
            }

            var embed = new EmbedBuilder().WithTitle($"All open resource listings of type {resourceType}.").WithColor(Color.Blue);

            var listingsJson = _dataBaseService.getJObjects("resource_market_listings").Result;
            ResourceMarketListing currentListing;

            foreach (var x in listingsJson)
            {
                currentListing = new ResourceMarketListing(x);

                string seller = "";
                if (currentListing.IdSeller.Length <=3)
                {
                    seller = new Company(_dataBaseService.getJObjectAsync(currentListing.IdSeller, "companies").Result).name;
                }
                else
                {
                    var members = Context.Guild.GetUsersAsync().Result.ToList();

                    seller = members.First(t => t.Id.ToString() == currentListing.IdSeller).Nickname;
                }
                embed.AddField(new EmbedFieldBuilder().WithName($"ID: {currentListing.Id} being sold by {seller}.").WithValue($"Amount: {currentListing.Amount}. Price per unit: {currentListing.Price}."));
            }

            await ReplyAsync("", false, embed.Build());
        }

        [Command("sellresource")]
        public async Task CreateResourceListing(string type, ulong amount, double pricePerUnit, string ticker = null)
        {
            var sellerId = Context.User.Id.ToString();
            if (ticker != null)
            {
                sellerId = _companyService.getCompany(ticker).Result.id;
            }
            var response = _resourceHandlingService.PutResourceUpForSale(sellerId, type, amount, pricePerUnit).Result;

            await ReplyAsync(response);
        }

        [Command("giveresource")]
        public async Task GiveResources(string type, ulong amount)
        {
            var user = Context.User.Id.ToString();
            var userGivenTo = Context.Message.MentionedUserIds.First().ToString();

            var response = _resourceHandlingService.TransferResources(user, userGivenTo, type, amount).Result;

            await ReplyAsync(response);
        }

        [Command("giveresource")]
        [HasMasterOfBots]
        public async Task AddResource(string type, ulong amount)
        {
            var usersGivenTo = Context.Message.MentionedUserIds.ToList();

            foreach (var x in usersGivenTo)
            {
                _resourceHandlingService.AddResource(x.ToString(), type, amount);
            }
        }

        [Command("buyresource")]
        public async Task BuyResourcesFromMarket(string type, ulong amount, string ticker = null)
        {
            string response;
            if (ticker == null)
            {
                var user = Context.User.Id.ToString();
                response = _resourceHandlingService.BuyResourceFromMarketAsUserAsync(type, user, amount).Result;
            }
            else
            {
                var company = _companyService.getCompany(ticker).Result;
                response = _resourceHandlingService.BuyResourceFromMarketAsCompanyAsync(type, company.id, amount).Result;
            }
            await ReplyAsync(response);
        }

        [Command("listingbuy")]
        public async Task BuyResourcesFromListing(string listingId, ulong amount, string ticker = null)
        {
            string response;
            if (ticker == null)
            {
                var user = Context.User.Id.ToString();
                response = _resourceHandlingService.BuyResourcesFromSpecificOfferOnMarketAsUser(listingId, user, amount).Result;
            }
            else
            {
                var company = _companyService.getCompany(ticker).Result;
                response = _resourceHandlingService.BuyResourcesFromSpecificOfferOnMarketAsCompany(listingId, company.id, amount).Result;
            }
            await ReplyAsync(response);
        }

        [Command("resources")]
        public async Task ViewResourcesAsync(string ticker = null)
        {
            var user = Context.User;

            Resource resources;

            if (ticker == null)
            {
                try
                {
                    resources = new Resource(_dataBaseService.getJObjectAsync(Context.User.Id.ToString(), "resources").Result);
                }
                catch (Exception e)
                {
                    var newRes = new Resource(user.Id.ToString());
                    _resourceHandlingService.SetResources(newRes.SerializeIntoJObject());
                    Console.WriteLine(e);
                }
                resources = new Resource(_dataBaseService.getJObjectAsync(Context.User.Id.ToString(), "resources").Result);
            }
            else
            {
                var company = _companyService.getCompany(ticker).Result;
                try
                {
                    resources = new Resource(_dataBaseService.getJObjectAsync(company.id, "resources").Result);
                }
                catch (Exception e)
                {
                    var newRes = new Resource(company.id);
                    _resourceHandlingService.SetResources(newRes.SerializeIntoJObject());
                    Console.WriteLine(e);
                }
                resources = new Resource(_dataBaseService.getJObjectAsync(company.id, "resources").Result);
            }

            var embed = new EmbedBuilder().WithTitle("Resources").WithDescription("All your resources").WithColor(Color.Blue);

            var embedFieldMinerals = new EmbedFieldBuilder().WithName("Minerals").WithValue(resources.Minerals);
            var embedFieldFood = new EmbedFieldBuilder().WithName("Food").WithValue(resources.Food);
            var embedFieldAlloys = new EmbedFieldBuilder().WithName("Alloys").WithValue(resources.Alloys);
            var embedFieldConsumerGoods = new EmbedFieldBuilder().WithName("Consumer Goods").WithValue(resources.ConsumerGoods);
            var embedFieldRefinedMinerals = new EmbedFieldBuilder().WithName("Refined Minerals").WithValue(resources.RefinedMinerals);
            var embedFieldRefinedFood = new EmbedFieldBuilder().WithName("Refined Food").WithValue(resources.RefinedFood);

            embed.AddField(embedFieldMinerals);
            embed.AddField(embedFieldFood);
            embed.AddField(embedFieldAlloys);
            embed.AddField(embedFieldConsumerGoods);
            embed.AddField(embedFieldRefinedMinerals);
            embed.AddField(embedFieldRefinedFood);

            await user.SendMessageAsync("", false, embed.Build());
        }
    }
}
