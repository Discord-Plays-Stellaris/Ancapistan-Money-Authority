using Discord.Commands;
using Discord;
using Discord.WebSocket;
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

namespace VIR.Modules
{
    public class StockMarketCommands : ModuleBase
    {
        private readonly DataBaseHandlingService db;
        private readonly CommandHandlingService CommandService;
        private readonly StockMarketService MarketService;

        public StockMarketCommands(DataBaseHandlingService _db, CommandHandlingService _CommandService, StockMarketService _MarketService)
        {
            db = _db;
            CommandService = _CommandService;
            MarketService = _MarketService;
        }

        [Command("namemarket")]
        [HasMasterOfBots]
        public async Task NameMarketTask(string acronym, [Remainder]string marketName)
        {
            StockMarketObject marketObj = new StockMarketObject(acronym, marketName);

            JObject JSONObj = db.SerializeObject<StockMarketObject>(marketObj);

            db.SetJObjectAsync(JSONObj, "system");

            await ReplyAsync("Market renamed to " + marketName + ", with the acronym of " + acronym);
        }

        [Command("market")]
        [Alias("marketinfo")]
        public async Task MarketInfoTask()
        {
            string name = Convert.ToString(await db.GetFieldAsync("MarketInfo", "marketName", "system"));
            string acronym = Convert.ToString(await db.GetFieldAsync("MarketInfo", "acronym", "system"));

            EmbedFieldBuilder marketNameField = new EmbedFieldBuilder().WithIsInline(false).WithName("Market Name:").WithValue(name + " (" + acronym + ")");
            EmbedFieldBuilder marketChannelField = new EmbedFieldBuilder().WithIsInline(false).WithName("Market Channel").WithValue($"<#{await db.GetFieldAsync("MarketChannel", "channel", "system")}>");
            Embed embd = new EmbedBuilder().WithTitle("Stock Market Info").AddField(marketNameField).AddField(marketChannelField).Build();

            await ReplyAsync("", false, embd);
        }

        [Command("marketchannel")]
        [Alias("setmarketchannel","transactionchannel","settransactionchannel")]
        [HasMasterOfBots]
        public async Task MarketChannelTask(string channel)
        {
            channel = channel.Remove(channel.Length - 1, 1);
            channel = channel.Remove(0, 2);

            StockMarketChannel channelObj = new StockMarketChannel(channel);
            JObject JSONChannel = db.SerializeObject<StockMarketChannel>(channelObj);
            db.SetJObjectAsync(JSONChannel, "system");

            await CommandService.PostMessageTask(channel, "This channel has been set as the transaction announcement channel!");

            await ReplyAsync($"Market channel set to <#{channel}>");
            
        }

        [Command("setshares")]
        [HasMasterOfBots]
        public async Task SetSharesTask(string user, string ticker, string amount)
        {
            user = user.Remove(user.Length - 1, 1);
            user = user.Remove(0, 2);

            await MarketService.SetShares(user, ticker, Convert.ToInt32(amount));
            await ReplyAsync($"<@{user}>'s shares in {ticker} set to {amount}");
        }

        [Command("getshares")]
        public async Task GetSharesAsync(string user, string ticker)
        {
            user = user.Remove(user.Length - 1, 1);
            user = user.Remove(0, 2);

            await ReplyAsync($"<@{user}> has {Convert.ToString(await MarketService.GetShares(user, ticker))} shares in {ticker}");
        }

        [Command("transaction")]
        [HasMasterOfBots]
        public async Task ManualTransactionAsync(string type, string ticker, int shares, double price)
        {
            Transaction transaction = new Transaction(price, shares, type, Context.User.Id.ToString(), ticker, db);

            try
            {
                JObject tmp = db.SerializeObject(transaction);
                tmp["id"] = Guid.NewGuid();
                await db.SetJObjectAsync(tmp, "transactions");
            }
            catch (Exception e)
            {
                await Log.Logger(Log.Logs.ERROR, e.Message);
                await ReplyAsync("Something went wrong: " + e.Message);
            }
            await ReplyAsync("Manual transaction complete");
        }
    }
}
