using Discord.Commands;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
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
        [Summary("Gets basic info of the market.")]
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
        [Alias("setmarketchannel", "transactionchannel", "settransactionchannel")]
        [HasMasterOfBots]
        public async Task MarketChannelTask(string channel)
        {
            channel = channel.Remove(channel.Length - 1, 1);
            channel = channel.Remove(0, 2);

            StockMarketChannel channelObj = new StockMarketChannel(channel,Context.Guild.Id.ToString());
            JObject JSONChannel = db.SerializeObject<StockMarketChannel>(channelObj);
            await db.SetJObjectAsync(JSONChannel, "system");

            await CommandService.PostMessageTask(channel, "This channel has been set as the transaction announcement channel!");

            await ReplyAsync($"Market channel set to <#{channel}>");

        }
        [Command("setshares")]
        [HasMasterOfBots]
        public async Task SetSharesTask(IUser iuser, string ticker, string amount)
        {
            string user = iuser.Id.ToString();

            await MarketService.SetShares(user, ticker, Convert.ToInt32(amount));
            await ReplyAsync($"<@{user}>'s shares in {ticker} set to {amount}");
        }

        [Command("getshares")]
        [Summary("Gets the shares owned in a company currently owned by an user.")]
        public async Task GetSharesAsync([Summary("The target user.")]IUser user, [Summary("The specific company.")]string ticker)
        {
            await ReplyAsync($"{user.Mention} has {Convert.ToString(await MarketService.GetShares(user.Id.ToString(), ticker))} shares in {ticker}");
        }

        [Command("getcorpshares")]
        [HasMasterOfBots]
        public async Task GetCorpSharesAsync(string tickerOwner, string tickerShare)
        {
            if (tickerOwner == tickerShare)
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
            await ReplyAsync($"{tickerOwner}'s shares in {tickerShare} set to {amount}");
        }

        [Command("accept")]
        [Alias("acceptoffer")]
        [Summary("Accepts an offer")]
        public async Task AcceptOfferAsync([Summary("The ID of the offer.")]string offerID)
        {
            Collection<string> IDs = await db.getIDs("transactions");
            string userMoneyt;
            string authorMoneyt;
            double userMoney;
            double authorMoney;

            if (IDs.Contains(offerID) == true)
            {
                if(offerID.Substring(0,4) == "ind-")
                {
                    await ReplyAsync("You need to provide a ticker of the company which will get the industry.");
                    return;
                }
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

                        ITextChannel chnl = (ITextChannel)await Context.Client.GetChannelAsync((UInt64)await db.GetFieldAsync("MarketChannel", "channel", "system"));
                        await chnl.DeleteMessageAsync(Convert.ToUInt64(transaction.messageID));

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
        [Summary("Set up an offer to buy")]
        public async Task BuyOfferAsync([Summary("Comapny ticker")]string ticker, [Summary("Share amount you wish to buy")]int shares, [Summary("Price per share")]double price)
        {
            string AuthorMoneyt = (string)await db.GetFieldAsync(Context.User.Id.ToString(), "money", "users");
            double AuthorMoney;

            if (shares < 0 || price < 0)
            {
                await ReplyAsync("You cant buy negative shares nor buy them for a negative amount of money!");
                return;
            }

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
        [Summary("Put up shares for sale.")]
        public async Task SellOfferAsync([Summary("The ticker of the company whose shares you want to sell")] string ticker, [Summary("The amount of shares you wish to sell")]int shares, [Summary("Price per share")]double price)
        {
            string AuthorMoneyt = (string)await db.GetFieldAsync(Context.User.Id.ToString(), "money", "users");
            double AuthorMoney;

            if (shares < 0 || price < 0)
            {
                await ReplyAsync("You cant sell negative shares sell buy them for a negative amount of money!");
                return;
            }

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
        [Summary("Gets the shares owned by you.")]
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

        [HasMasterOfBots]
        [Command("shareholders")]
        public async Task ShareholdersAsync(string ticker)
        {
            string output = $"{ticker}'s shareholders are: ";

            foreach (ulong userID in await MarketService.GetShareholders(ticker))
            {
                IGuildUser user = await Context.Guild.GetUserAsync(userID);
                output = output + $"<@{user.Id.ToString()}> ({await MarketService.GetShares(userID.ToString(), ticker)} shares), ";
            }

            await ReplyAsync(output);
        }

        [HasMasterOfBots]
        [Command("shareholdervote")]
        public async Task VoteAsync(string ticker, string description, [Remainder] string choicesInput)
        {
            string[] choicesArray = choicesInput.Split( new[] { ", " }, StringSplitOptions.None);
            Collection<string> choices = new Collection<string>();

            foreach (string _choice in choicesArray)
            {
                choices.Add(_choice);
            }

            if (choices.Count > 9)
            {
                await ReplyAsync("You may only have a maxiumum of 9 choices for the vote.");
                return;
            }

            EmbedBuilder embedBuilder = new EmbedBuilder().WithTitle($"Shareholder vote for {ticker}").WithDescription(description).WithColor(Color.Red).WithFooter("If you vote for more than one option, the bot will pick a random option from the ones you picked as your vote.");
            
            foreach(string choice in choices)
            {
                string emoji = "";
                switch (choices.IndexOf(choice))
                {
                    case 0:
                        emoji = ":one:";
                        break;
                    case 1:
                        emoji = ":two:";
                        break;
                    case 2:
                        emoji = ":three:";
                        break;
                    case 3:
                        emoji = ":four:";
                        break;
                    case 4:
                        emoji = ":five:";
                        break;
                    case 5:
                        emoji = ":six:";
                        break;
                    case 6:
                        emoji = ":seven:";
                        break;
                    case 7:
                        emoji = ":eight:";
                        break;
                    case 8:
                        emoji = ":nine:";
                        break;
                }

                EmbedFieldBuilder embedField = new EmbedFieldBuilder().WithName(choice).WithValue($"React with {emoji} to vote for '{choice}'");
                embedBuilder.AddField(embedField);
            }

            await ReplyAsync("Sending DMs. This may take a while...");
            Dictionary<ulong, ulong> DMs = new Dictionary<ulong, ulong>();

            foreach(ulong ID in await MarketService.GetShareholders(ticker))
            {
                IUser user = await Context.Guild.GetUserAsync(ID);
                IUserMessage message = await user.SendMessageAsync("", false, embedBuilder.Build());
                foreach(string choice in choices)
                {
                    string emojiString = "";
                    switch (choices.IndexOf(choice))
                    {
                        case 0:
                            emojiString = "\u0031\u20E3"; // 1
                            break;
                        case 1:
                            emojiString = "\u0032\u20E3"; // 2
                            break;
                        case 2:
                            emojiString = "\u0033\u20E3"; // 3
                            break;
                        case 3:
                            emojiString = "\u0034\u20E3"; // 4
                            break;
                        case 4:
                            emojiString = "\u0035\u20E3"; // 5
                            break;
                        case 5:
                            emojiString = "\u0036\u20E3"; // 6
                            break;
                        case 6:
                            emojiString = "\u0037\u20E3"; // 7
                            break;
                        case 7:
                            emojiString = "\u0038\u20E3"; // 8
                            break;
                        case 8:
                            emojiString = "\u0039\u20E3"; // 9
                            break;
                    }

                    IEmote emoji = new Emoji(emojiString);

                    await message.AddReactionAsync(emoji);
                }

                DMs[ID] = message.Id;
            }

            ShareholderVote vote = new ShareholderVote();
            vote.NewVote(choices, ticker, DMs);
            await db.SetJObjectAsync(db.SerializeObject<ShareholderVote>(vote), "votes");
            await ReplyAsync("", false, embedBuilder.Build());
            await ReplyAsync($"Vote initiated with the above embed. Use the command `&endvote {vote.id}` to end the vote.");
        }

        [HasMasterOfBots]
        [Command("endvote")]
        public async Task EndVoteTask(string guidString)
        {
            JObject temp = await db.getJObjectAsync(guidString, "votes");
            ShareholderVote vote = new ShareholderVote(temp);
            //vote.JSON(await db.getJObjectAsync(guidString, "votes"));
            Dictionary<ulong, string> votes = new Dictionary<ulong, string>(); // user, vote
            foreach(ulong ID in vote.messages.Keys)
            {
                IDMChannel dmChannel = await Context.Client.GetDMChannelAsync(ID);
                IUserMessage message = (IUserMessage)await dmChannel.GetMessageAsync(vote.messages[ID]);

                Collection<IEmote> reactionsRaw = (Collection<IEmote>)message.Reactions.Keys;
                Collection<IEmote> reactions = new Collection<IEmote>();

                foreach(IEmote reaction in reactionsRaw)
                {

                }
            }
        }
    }
}
