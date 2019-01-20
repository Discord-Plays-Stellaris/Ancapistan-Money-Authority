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

            JObject JSONObj = db.SerializeObject(marketObj);

            await db.SetJObjectAsync(JSONObj, "system");

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
            await db.SetJObjectAsync(JSONChannel, "system");

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

        [Command("getcorpshares")]
        [HasMasterOfBots]
        public async Task GetCorpSharesAsync(string tickerOwner, string tickerShare)
        {
            if(tickerOwner == tickerShare)
            {
                await ReplyAsync("A company can't own shares in itself, silly!");
                return;
            }

            await ReplyAsync($"{tickerOwner} has {Convert.ToString(await MarketService.GetShares(tickerOwner, tickerShare))} shares in {tickerShare}");
        }

        [Command("setcorpshares")]
        [HasMasterOfBots]
        public async Task SetCorpSharesAsync(string tickerOwner, string tickerShare, string amount)
        {
            if (tickerOwner == tickerShare)
            {
                await ReplyAsync("A company can't own shares in itself, silly!");
                return;
            }

            await MarketService.SetShares(tickerOwner, tickerShare, Convert.ToInt32(amount));
            await ReplyAsync($"<@{tickerOwner}>'s shares in {tickerShare} set to {amount}");
        }

        [Command("transaction")]
        [HasMasterOfBots]
        public async Task ManualTransactionAsync(string type, string ticker, int shares, double price)
        {
            Transaction transaction = new Transaction(price, shares, type, Context.User.Id.ToString(), ticker, db, CommandService);

            try
            {
                Guid GUID = Guid.NewGuid();
                transaction.id = GUID;
                JObject tmp = db.SerializeObject(transaction);
                await db.SetJObjectAsync(tmp, "transactions");
                await ReplyAsync($"Manual transaction lodged in <#{await db.GetFieldAsync("MarketChannel", "channel", "system")}>");
            }
            catch (Exception e)
            {
                await Log.Logger(Log.Logs.ERROR, e.Message);
                await ReplyAsync("Something went wrong: " + e.Message);
            }
        }

        [Command("accept")]
        [Alias("acceptoffer")]
        public async Task AcceptOfferAsync(string offerID)
        {
            Collection<string> IDs = await db.getIDs("transactions");
            string userMoneyt;
            string authorMoneyt;
            double userMoney;
            double authorMoney;

            if (IDs.Contains(offerID) == true)
            {
                Transaction transaction = new Transaction(await db.getJObjectAsync(offerID, "transactions"));

                if (transaction.author != Context.User.Id.ToString())
                {
                    // Gets money values of the command user and the transaction author
                    userMoneyt = (string)await db.GetFieldAsync(Context.User.Id.ToString(), "money", "users");
                    if (userMoneyt == null)
                    {
                        userMoney = 50000;
                    }
                    else
                    {
                        userMoney = double.Parse(userMoneyt);
                    }

                    authorMoneyt = (string)await db.GetFieldAsync(transaction.author, "money", "users");
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
                        userMoney += (transaction.shares * transaction.price);
                        authorMoney -= (transaction.shares * transaction.price);
                    }
                    else
                    {
                        userMoney -= (transaction.shares * transaction.price);
                        authorMoney += (transaction.shares * transaction.price);
                    }

                    // Transfers the shares
                    int _userShares = await MarketService.GetShares(Context.User.Id.ToString(), transaction.ticker);
                    int _authorShares = await MarketService.GetShares(transaction.author, transaction.ticker);

                    if (transaction.type == "buy")
                    {
                        _authorShares += transaction.shares;
                        _userShares -= transaction.shares;
                    }
                    else
                    {
                        _authorShares -= transaction.shares;
                        _userShares += transaction.shares;
                    }

                    if (_userShares < 0)
                    {
                        await ReplyAsync("You cannot complete this transaction as it would leave you with a negative amount of shares in the specified company.");
                    }
                    else if (userMoney < 0)
                    {
                        await ReplyAsync("You cannot complete this transaction as it would leave you with a negative amount on money.");
                    }
                    else
                    {
                        await MarketService.SetShares(Context.User.Id.ToString(), transaction.ticker, _userShares);
                        await MarketService.SetShares(transaction.author, transaction.ticker, _authorShares);
                        await MarketService.UpdateSharePrice(transaction);

                        await db.SetFieldAsync(Context.User.Id.ToString(), "money", userMoney, "users");
                        await db.SetFieldAsync(transaction.author, "money", authorMoney, "users");
                        await db.RemoveObjectAsync(offerID, "transactions");

                        await ReplyAsync("Transaction complete!");
                        await CommandService.PostMessageTask((string)await db.GetFieldAsync("MarketChannel", "channel", "system"), $"<@{transaction.author}>'s Transaction with ID {transaction.id} has been accepted by <@{Context.User.Id}>!");
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

        [Command("buyoffer")]
        [Alias("buyshares")]
        public async Task BuyOfferAsync(string ticker, int shares, double price)
        {
            string AuthorMoneyt = (string) await db.GetFieldAsync(Context.User.Id.ToString(), "money", "users");
            double AuthorMoney;

            if (AuthorMoneyt == null)
            {
                AuthorMoney = 50000;
                await db.SetFieldAsync(Context.User.Id.ToString(), "money", AuthorMoney, "users");
            }
            else
            {
                AuthorMoney = double.Parse(AuthorMoneyt);
            }

            if ((shares * price) > AuthorMoney)
            {
                await ReplyAsync("You do not have enough money for this transaction");
            }
            else
            {
                Transaction transaction = new Transaction(price, shares, "buy", Context.User.Id.ToString(), ticker, db, CommandService);

                try
                {
                    Guid GUID = Guid.NewGuid();
                    transaction.id = GUID;
                    JObject tmp = db.SerializeObject(transaction);
                    await db.SetJObjectAsync(tmp, "transactions");
                    await ReplyAsync($"Buy offer lodged in <#{await db.GetFieldAsync("MarketChannel", "channel", "system")}>");
                }
                catch (Exception e)
                {
                    await Log.Logger(Log.Logs.ERROR, e.Message);
                    await ReplyAsync("Something went wrong: " + e.Message);
                }
            }
        }

        [Command("selloffer")]
        [Alias("sellshares")]
        public async Task SellOfferAsync(string ticker, int shares, double price)
        {
            string AuthorMoneyt = (string)await db.GetFieldAsync(Context.User.Id.ToString(), "money", "users");
            double AuthorMoney;

            if (AuthorMoneyt == null)
            {
                AuthorMoney = 50000;
                await db.SetFieldAsync(Context.User.Id.ToString(), "money", AuthorMoney, "users");
            }
            else
            {
                AuthorMoney = double.Parse(AuthorMoneyt);
            }

            UserShares tempObj;

            try
            {
                tempObj = new UserShares(await db.getJObjectAsync(Context.User.Id.ToString(), "shares"), true);
            }
            catch (System.NullReferenceException)
            {
                tempObj = new UserShares(Context.User.Id.ToString());
                await ReplyAsync("You cannot complete this transaction as you own no shares in the specified company");
                return;
            }
            Dictionary<string, int> ownedShares = tempObj.ownedShares;

            if (ownedShares.ContainsKey(ticker) == false)
            {
                await ReplyAsync("You cannot complete this transaction as you own no shares in the specified company");
                return;
            }

            int outcomeAmount = ownedShares[ticker] - shares;

            if (outcomeAmount < 0)
            {
                await ReplyAsync("You cannot complete this transaction as it would leave you with a negative amount of shares in the specified company.");
            }
            else if (ownedShares[ticker] == 0)
            {
                await ReplyAsync("You cannot complete this transaction as you own no shares in the specified company.");
            }
            else
            {
                Transaction transaction = new Transaction(price, shares, "sell", Context.User.Id.ToString(), ticker, db, CommandService);

                try
                {
                    Guid GUID = Guid.NewGuid();
                    transaction.id = GUID;
                    JObject tmp = db.SerializeObject(transaction);
                    await db.SetJObjectAsync(tmp, "transactions");
                    await ReplyAsync($"Sell offer lodged in <#{await db.GetFieldAsync("MarketChannel", "channel", "system")}>");
                }
                catch (Exception e)
                {
                    await Log.Logger(Log.Logs.ERROR, e.Message);
                    await ReplyAsync("Something went wrong: " + e.Message);
                }
            }
        }

        [Command("shares")]
        public async Task SharesAsync()
        {
            UserShares sharesObj;
            try
            {
                sharesObj = new UserShares(await db.getJObjectAsync(Context.User.Id.ToString(), "shares"), true);
            }
            catch (System.NullReferenceException)
            {
                await ReplyAsync("You shares have been sent to you privately");
                await Context.User.SendMessageAsync("You do not own any shares");
                return;
            }
            Dictionary<string, int> ownedShares = sharesObj.ownedShares;

            Collection<EmbedFieldBuilder> embedFields = new Collection<EmbedFieldBuilder>();

            foreach(string ticker in ownedShares.Keys)
            {
                EmbedFieldBuilder embedField = new EmbedFieldBuilder().WithIsInline(false).WithName($"{(string)await db.GetFieldAsync(ticker, "name", "companies")} ({ticker}) ").WithValue(ownedShares[ticker]);
                embedFields.Add(embedField);
            }

            EmbedBuilder embed = new EmbedBuilder().WithColor(Color.Gold).WithTitle($"Shares Owned by {Context.User.Username.ToString()}").WithDescription($"This is a list of all shares owned by {Context.User.Username.ToString()}");

            foreach(EmbedFieldBuilder field in embedFields)
            {
                embed.AddField(field);
            }

            await ReplyAsync("Your shares have been sent to you privately");
            await Context.User.SendMessageAsync("", false, embed.Build());
        }
    }
}
