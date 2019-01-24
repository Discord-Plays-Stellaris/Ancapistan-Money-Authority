using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using VIR.Modules.Preconditions;
using VIR.Objects.Company;
using VIR.Services;

namespace VIR.Modules
{
    class IndustryCommands : ModuleBase
    {
        private readonly CompanyService CompanyService;
        private readonly DataBaseHandlingService dataBaseService;
        private readonly CommandHandlingService CommandService;
        private readonly StockMarketService MarketService;

        public IndustryCommands(CompanyService com, DataBaseHandlingService db, CommandHandlingService comm, StockMarketService markserv)
        {
            CompanyService = com;
            dataBaseService = db;
            CommandService = comm;
            MarketService = markserv;
        }

        [Command("createindustry")]
        [Alias("addindustry")]
        [HasMasterOfBots]
        public async Task AddIndustryAsync(string type, ulong yearlyOutput, ulong utilOutput)
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

            Industry industry = new Industry(id, type, yearlyOutput, utilOutput, 100);

            await dataBaseService.SetJObjectAsync(dataBaseService.SerializeObject<Industry>(industry), "industries");

            await ReplyAsync($"Industry with ID {id} created");
        }

        [Command("industries")]
        public async Task IndustryListAsync()
        {
            Collection<string> IDs = await dataBaseService.getIDs("industries");

            EmbedBuilder embed = new EmbedBuilder().WithTitle("List of Industries").WithDescription("This is a list of all industries").WithColor(Color.Blue);

            foreach (string id in IDs)
            {
                Industry industry = new Industry(await dataBaseService.getJObjectAsync(id, "industries"));
                EmbedFieldBuilder embedField = new EmbedFieldBuilder().WithName(industry.Id).WithValue($"Resource Produced: {industry.Type}. Yearly output: {industry.YearlyOutput}. Output per Util spent: {industry.UtilOutput}.");
                embed.AddField(embedField);
            }

            await ReplyAsync("", false, embed.Build());
        }

        [Command("getindustries")]
        [HasMasterOfBots]
        public async Task GetIndustriesAsync(string ticker, string id = null)
        {
            Assets OwnedIndustries;
            try
            {
                OwnedIndustries = new Assets(await dataBaseService.getJObjectAsync(ticker, "assets"), true);
            }
            catch (System.NullReferenceException)
            {
                OwnedIndustries = new Assets(ticker);
                await dataBaseService.SetJObjectAsync(dataBaseService.SerializeObject<Assets>(OwnedIndustries), "assets");
                await ReplyAsync("This company owns no industries");
                return;
            }

            if (id == null)
            {
                await ReplyAsync(OwnedIndustries.industries.ToString());
            }
            else
            {
                if (OwnedIndustries.industries[id] == 1)
                {
                    await ReplyAsync($"{ticker} holds {OwnedIndustries.industries[id]} {id} industry.");
                }
                else
                {
                    await ReplyAsync($"{ticker} holds {OwnedIndustries.industries[id]} {id} industries.");
                }
            }
        }

        [Command("setindustries")]
        [HasMasterOfBots]
        public async Task SetIndustriesAsync(string ticker, string industry, int amount)
        {
            Assets OwnedIndustries;
            try
            {
                OwnedIndustries = new Assets(await dataBaseService.getJObjectAsync(ticker, "assets"), true);
            }
            catch (System.NullReferenceException)
            {
                OwnedIndustries = new Assets(ticker);
                await dataBaseService.SetJObjectAsync(dataBaseService.SerializeObject<Assets>(OwnedIndustries), "assets");
            }

            if (OwnedIndustries.industries.ContainsKey(industry))
            {
                OwnedIndustries.industries.Remove(industry);
            }

            OwnedIndustries.industries.Add(industry, amount);
            if (amount != 1)
            {
                await ReplyAsync($"{ticker} now has {amount} {industry} industries");
            }
            else
            {
                await ReplyAsync($"{ticker} now has {amount} {industry} industry");
            }

            await dataBaseService.SetJObjectAsync(dataBaseService.SerializeObject<Assets>(OwnedIndustries), "assets");
        }
    }
}
