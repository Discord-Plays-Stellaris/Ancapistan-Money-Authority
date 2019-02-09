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
using VIR.Modules.Preconditions;
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
        }

        [Command("sellindustry")]
        public async Task SellIndustry(string industryId, double price)
        {

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

                EmbedFieldBuilder embedField = new EmbedFieldBuilder().WithName(industry.Id).WithValue($"Resource Produced: {industry.Type}. Yearly output: {industry.MonthlyOutput}. Belongs to {industry.CompanyId} on planet {industry.Planet}.");
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
                var embedField = new EmbedFieldBuilder().WithName(x.Id)
                    .WithValue($"ID: {x.Id}. Resource Produced: {x.Type}. Yearly output: {x.MonthlyOutput}. On the planet {x.Planet}.");
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
