using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VIR.Services;
using Discord;
using System.Timers;
using Quartz;
using Quartz.Impl;
using VIR.Objects.Company;

namespace VIR.Objects
{
    /// <summary>
    /// An object to contain information about the Stock Market.
    /// </summary>
    public class StockMarketObject
    {
        public string acronym { get; private set; }
        public string marketName { get; private set; }
        public string id { get; private set; }

        /// <summary>
        /// The StockMarketObject constructer
        /// </summary>
        /// <param name="acro">The acronym for the stock market</param>
        /// <param name="name">The name of the stock market.</param>
        public StockMarketObject(string acro, string name)
        {
            acronym = acro;
            marketName = name;
            id = "MarketInfo";
        }
    }

    public class StockMarketChannel
    {
        public string id;
        public string channel;

        public StockMarketChannel(string _channel, string _server)
        {
            id = "MarketChannel";
            channel = _channel;
        }
    }

    /// <summary>
    /// An object representing a pending transaction on the stock market.
    /// </summary>
    public class Transaction
    {
        public Guid id;
        public double price;
        public int shares;
        public string type; // buy, sell, private
        public string author;
        public string ticker;
        public string messageID; 


        public Transaction(JObject JSONInput)
        {
            id = (Guid)JSONInput["id"];
            price = (double)JSONInput["price"];
            shares = (int)JSONInput["shares"];
            type = (string)JSONInput["type"];
            author = (string)JSONInput["author"];
            ticker = (string)JSONInput["ticker"];
            messageID = (string)JSONInput["messageID"];
        }

        /// <summary>
        /// The constructor for a new transaction.
        /// </summary>
        /// <param name="_price">The price per share</param>
        /// <param name="_shares">The total amount of shares</param>
        /// <param name="_type">Buy, sell, or private</param>
        /// <param name="_author">The user who initiated the transaction</param>
        /// <param name="_ticker">The ticker of the company who's shares are being traded</param>
        /// <param name="db">A DataBaseHandlingService object</param>
        /// <param name="command">A CommandHandlingService object</param>77
        public Transaction(double _price, int _shares, string _type, string _author, string _ticker, DataBaseHandlingService db, CommandHandlingService command)
        {
            price = _price;
            shares = _shares;
            type = _type;
            author = _author;
            ticker = _ticker;
            id = Guid.NewGuid();

            messageID = Convert.ToString(LodgeTransactionTask(db, command).GetAwaiter().GetResult());

        }

        private async Task<ulong> LodgeTransactionTask(DataBaseHandlingService db, CommandHandlingService CommandService)
        {
            EmbedFieldBuilder typeField = new EmbedFieldBuilder().WithIsInline(true).WithName("Type:").WithValue($"Looking to {type} shares");
            EmbedFieldBuilder companyField = new EmbedFieldBuilder().WithIsInline(true).WithName("Company:").WithValue(await db.GetFieldAsync(ticker, "name", "companies"));
            EmbedFieldBuilder amountField = new EmbedFieldBuilder().WithIsInline(true).WithName("Amount of shares being bought/sold:").WithValue(shares);
            EmbedFieldBuilder priceField = new EmbedFieldBuilder().WithIsInline(true).WithName("Price per share:").WithValue("$" + price);
            EmbedFieldBuilder totalPriceField = new EmbedFieldBuilder().WithIsInline(true).WithName("Total Price:").WithValue("$" + shares * price);

            EmbedBuilder emb = new EmbedBuilder().WithTitle("Stock Market Offer").WithDescription($"Use the command `&accept {id.ToString()}` to accept this offer.").WithFooter($"Transaction ID: {id.ToString()}").AddField(typeField).AddField(companyField).AddField(amountField).AddField(priceField).AddField(totalPriceField).WithColor(Color.Green);

            Discord.Rest.RestUserMessage message = await CommandService.PostEmbedTask((string)await db.GetFieldAsync("MarketChannel","channel","system"), emb.Build());
            return message.Id;
        }
    }

    /// <summary>
    /// Literally the Transaction class, just refit to work with industries.
    /// </summary>
    public class IndustryTransaction
    {
        public string id;
        public double price;
        public string type; // buy, sell, private
        public string author;
        public string industryID;
        public string messageID;

        public IndustryTransaction(JObject JSONInput)
        {
            id = (string)JSONInput["id"];
            price = (double)JSONInput["price"];
            type = (string)JSONInput["type"];
            author = (string)JSONInput["author"];
            industryID = (string)JSONInput["industryID"];
            messageID = (string)JSONInput["messageID"];
        }

        /// <summary>
        /// The constructor for a new transaction.
        /// </summary>
        /// <param name="_price">The price per share</param>
        /// <param name="_shares">The total amount of shares</param>
        /// <param name="_type">Buy, sell, or private</param>
        /// <param name="_author">The user who initiated the transaction</param>
        /// <param name="_ticker">The ticker of the company who's shares are being traded</param>
        /// <param name="db">A DataBaseHandlingService object</param>
        /// <param name="command">A CommandHandlingService object</param>77
        public IndustryTransaction(double _price, string _type, string _author, string _industryID, DataBaseHandlingService db, CommandHandlingService command)
        {
            price = _price;
            type = _type;
            author = _author;
            industryID = _industryID;
            id = "ind-" + Guid.NewGuid().ToString();

            messageID = Convert.ToString(LodgeTransactionTask(db, command).GetAwaiter().GetResult());

        }
        private async Task<ulong> LodgeTransactionTask(DataBaseHandlingService db, CommandHandlingService CommandService)
        {
            EmbedFieldBuilder typeField = new EmbedFieldBuilder().WithIsInline(true).WithName("Type:").WithValue($"Looking to {type} industry");
            EmbedFieldBuilder companyField = new EmbedFieldBuilder().WithIsInline(true).WithName("ID:").WithValue(industryID);
            EmbedFieldBuilder amountField = new EmbedFieldBuilder().WithIsInline(true).WithName("Type:").WithValue(db.GetFieldAsync(industryID, "type", "industry").ToString());
            EmbedFieldBuilder priceField = new EmbedFieldBuilder().WithIsInline(true).WithName("Price:").WithValue("$" + price);

            EmbedBuilder emb = new EmbedBuilder().WithTitle("Stock Market Offer").WithDescription($"Use the command `&accept {id.ToString()}` to accept this offer.").WithFooter($"Transaction ID: {id.ToString()}").AddField(typeField).AddField(companyField).AddField(amountField).AddField(priceField).WithColor(Color.Green);

            Discord.Rest.RestUserMessage message = await CommandService.PostEmbedTask((string)await db.GetFieldAsync("MarketChannel", "channel", "system"), emb.Build());
            return message.Id;
        }
    }

    /// <summary>
    /// Literally the Transaction class, just refit to work with industries, and with an auction system
    /// </summary>
    public class IndustryAuction
    {
        public string id;
        public double price;
        public string type;
        public string industryID;
        public string messageID;
        public string currentWinner; //leading company ticker
        public DateTime plannedEnd;
        public string currentUser;

        public IndustryAuction(JObject JSONInput)
        {
            id = (string)JSONInput["id"];
            price = (double)JSONInput["price"];
            industryID = (string)JSONInput["industryID"];
            messageID = (string)JSONInput["messageID"];
            currentWinner = (string)JSONInput["currentWinner"];
            plannedEnd = DateTime.FromFileTimeUtc(long.Parse((string)JSONInput["plannedEnd"]));
            currentUser = (string)JSONInput["currentUser"];
            type = (string)JSONInput["type"];
        }

        /// <summary>
        /// The constructor for a new transaction.
        /// </summary>
        /// <param name="_price">The price per share</param>
        /// <param name="_shares">The total amount of shares</param>
        /// <param name="_type">Buy, sell, or private</param>
        /// <param name="_author">The user who initiated the transaction</param>
        /// <param name="_ticker">The ticker of the company who's shares are being traded</param>
        /// <param name="db">A DataBaseHandlingService object</param>
        /// <param name="command">A CommandHandlingService object</param>
        /// <param name="hours">Hours until auction ends</param>
        /// <param name="mins">Minutes until auction ends</param>
        /// <param name="_industryID">Id of industry being sold</param>
        public IndustryAuction(double _price, string _type, string _industryID, DataBaseHandlingService db, CommandHandlingService command, StockMarketService marketService, int hours, int mins)
        {
            price = _price;
            currentWinner = "";
            industryID = _industryID;
            id = "ind-" + Guid.NewGuid().ToString();
            currentUser = "";
            type = "auction";

            messageID = Convert.ToString(LodgeTransactionTask(db, command, marketService, hours, mins).GetAwaiter().GetResult());

        }
        private async Task<ulong> LodgeTransactionTask(DataBaseHandlingService db, CommandHandlingService CommandService, StockMarketService marketService, int hours, int mins)
        {
            EmbedFieldBuilder typeField = new EmbedFieldBuilder().WithIsInline(true).WithName("Type:").WithValue($"Industry Auction");
            EmbedFieldBuilder companyField = new EmbedFieldBuilder().WithIsInline(true).WithName("ID:").WithValue(industryID);
            EmbedFieldBuilder amountField = new EmbedFieldBuilder().WithIsInline(true).WithName("Type:").WithValue((string)await db.GetFieldAsync(industryID, "Type", "industries"));
            EmbedFieldBuilder priceField = new EmbedFieldBuilder().WithIsInline(true).WithName("Price:").WithValue("$" + price);

            EmbedBuilder emb = new EmbedBuilder().WithTitle("Stock Market Offer").WithDescription($"Use the command `&bid [ticker] {id.ToString()} [price]` to accept this offer.").WithFooter($"Transaction ID: {id.ToString()}").AddField(typeField).AddField(companyField).AddField(amountField).AddField(priceField).WithColor(Color.Green);

            Discord.Rest.RestUserMessage message = await CommandService.PostEmbedTask((string)await db.GetFieldAsync("MarketChannel", "channel", "system"), emb.Build());
            DateTime now = DateTime.UtcNow;
            DateTime scheduled = now.AddHours(hours).AddMinutes(mins);
            plannedEnd = scheduled;
            JobDataMap map = new JobDataMap();
            map.Add("market", marketService);
            map.Add("auction", this);
            map.Add("command", CommandService);
            map.Add("db", db);
            IJobDetail job = JobBuilder.Create<Job>().SetJobData(map).Build();
            ITrigger trigger = TriggerBuilder.Create().WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromMilliseconds(1)).WithRepeatCount(1)).StartAt(plannedEnd).Build();
            await CommandService.scheduler.ScheduleJob(job, trigger);

            return message.Id;
        }

        public async Task Bid(double _price, string ticker, string user, DataBaseHandlingService db, CommandHandlingService comm)
        {
            price = _price;
            currentWinner = ticker;
            currentUser = user;
            string type = (string)await db.GetFieldAsync(industryID, "Type", "industries");
            EmbedFieldBuilder typeField = new EmbedFieldBuilder().WithIsInline(true).WithName("Type:").WithValue($"Industry Auction");
            EmbedFieldBuilder companyField = new EmbedFieldBuilder().WithIsInline(true).WithName("ID:").WithValue(industryID);
            EmbedFieldBuilder amountField = new EmbedFieldBuilder().WithIsInline(true).WithName("Type:").WithValue(type);
            EmbedFieldBuilder priceField = new EmbedFieldBuilder().WithIsInline(true).WithName("Price:").WithValue("$" + price);
            EmbedFieldBuilder winnerField = new EmbedFieldBuilder().WithIsInline(true).WithName("Highest Bidder: ").WithValue(currentWinner);

            EmbedBuilder emb = new EmbedBuilder().WithTitle("Stock Market Offer").WithDescription($"Use the command `&bid [ticker] {id.ToString()} [price]` to accept this offer.").WithFooter($"Transaction ID: {id.ToString()}").AddField(typeField).AddField(companyField).AddField(amountField).AddField(priceField).AddField(winnerField).WithColor(Color.Green);
            
            await comm.EditEmbedTask((string)await db.GetFieldAsync("MarketChannel", "channel", "system"), messageID, emb.Build());
        }

        public JObject SerializeIntoJObject()
        {
            JObject jObject = new JObject();
            jObject["id"] = id;
            jObject["price"] = price;
            jObject["industryID"] = industryID;
            jObject["messageID"] = messageID;
            jObject["currentWinner"] = currentWinner;
            jObject["currentUser"] = currentUser;
            jObject["type"] = type;
            jObject["plannedEnd"] = plannedEnd.ToFileTimeUtc().ToString();
            return jObject;
        }

        public async Task schedule(DataBaseHandlingService db, CommandHandlingService CommandService, StockMarketService marketService)
        {
            if (plannedEnd.Ticks <= DateTime.Now.Ticks)
            {
                Industry ind = new Industry(await db.getJObjectAsync(this.industryID, "industries"));
                ind.CompanyId = this.currentWinner;
                await db.SetJObjectAsync(ind.SerializeIntoJObject(), "industries");
                await db.RemoveObjectAsync(this.id, "transactions");
                string markchan = (string)await db.GetFieldAsync("MarketChannel", "channel", "system");

                await CommandService.deleteMessageTask(markchan, this.messageID);

                await CommandService.PostMessageTask(markchan, $"Auction with ID {this.id} has been won by <@{(string)await db.GetFieldAsync(this.currentWinner, "name", "companies")}>!");
                return;
            }
            JobDataMap map = new JobDataMap();
            map.Add("market", marketService);
            map.Add("auction", this);
            map.Add("command", CommandService);
            map.Add("db", db);
            IJobDetail job = JobBuilder.Create<Job>().SetJobData(map).Build();
            ITrigger trigger = TriggerBuilder.Create().WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromMilliseconds(1)).WithRepeatCount(1)).StartAt(plannedEnd).Build();
            await CommandService.scheduler.ScheduleJob(job, trigger);
        }

        class Job : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                /*Industry ind = new Industry(await db.getJObjectAsync(auction.industryID, "industries"));
                ind.CompanyId = auction.currentWinner;
                await db.SetJObjectAsync(ind.SerializeIntoJObject(), "industries");
                string markchan = (string)await db.GetFieldAsync("MarketChannel", "channel", "system");

                await comm.deleteMessageTask(markchan, auction.messageID);

                await comm.PostMessageTask(markchan, $"Auction with ID {auction.id} has been accepted by <@{(string)await db.GetFieldAsync(auction.currentWinner, "name", "companies")}>!");
                await db.RemoveObjectAsync(auction.id, "transactions");*/
                IndustryAuction auction = (IndustryAuction) context.JobDetail.JobDataMap.Get("auction");
                StockMarketService market = (StockMarketService)context.JobDetail.JobDataMap.Get("market");
                CommandHandlingService comm = (CommandHandlingService)context.JobDetail.JobDataMap.Get("command");
                DataBaseHandlingService db = (DataBaseHandlingService)context.JobDetail.JobDataMap.Get("db");
                auction = new IndustryAuction(await db.getJObjectAsync(auction.id, "transactions"));
                Industry ind = new Industry(await db.getJObjectAsync(auction.industryID, "industries"));
                ind.CompanyId = auction.currentWinner;
                await db.SetJObjectAsync(ind.SerializeIntoJObject(), "industries");
                await db.RemoveObjectAsync(auction.id, "transactions");
                string markchan = (string)await db.GetFieldAsync("MarketChannel", "channel", "system");

                await comm.deleteMessageTask(markchan, auction.messageID);

                await comm.PostMessageTask(markchan, $"Auction with ID {auction.id} has been accepted by {(string)await db.GetFieldAsync(auction.currentWinner, "name", "companies")}!");
            }
        }
    }

    public class UserShares
    {
        public string id;
        public Dictionary<string, int> ownedShares = new Dictionary<string, int>(); // Format: Ticker, Shares

        public UserShares()
        {
        }

        public UserShares(string userID)
        {
            id = userID;
        }

        public UserShares(JObject input, bool isJSON)
        {
            UserShares tempObj = JsonConvert.DeserializeObject<UserShares>(input.ToString());

            id = tempObj.id;
            ownedShares = tempObj.ownedShares;
        }
    }

    public class ShareholderVote
    {
        public Guid id { get; set; }
        public Collection<string> choices { get; set; }
        public Dictionary<ulong,ulong> messages { get; set; }
        public string ticker { get; set; }

        public ShareholderVote()
        {

        }

        public ShareholderVote(JObject input)
        {
            ShareholderVote temp = JsonConvert.DeserializeObject<ShareholderVote>(input.ToString());

            id = temp.id;
            choices = temp.choices;
            ticker = temp.ticker;
            messages = temp.messages;
        }

        public void NewVote(Collection<string> _choices, string _ticker, Dictionary<ulong,ulong> _messages)
        {
            choices = _choices;
            ticker = _ticker;
            messages = _messages;
            id = Guid.NewGuid();
        }

        /*public void JSON(JObject input)
        {
            ShareholderVote temp = JsonConvert.DeserializeObject<ShareholderVote>(input.ToString());

            id = temp.id;
            choices = temp.choices;
            ticker = temp.ticker;
            messages = temp.messages;
        }*/
    }
}