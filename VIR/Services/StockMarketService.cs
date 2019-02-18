using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using VIR.Services;
using VIR.Objects;
using VIR.Modules.Objects.Company;
using VIR.Objects.Company;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using VIR.Properties;

namespace VIR.Services
{
    /// <summary>
    /// Contains methods for the stock market
    /// </summary>
    public class StockMarketService
    {
        private readonly DataBaseHandlingService db;
        private readonly CommandHandlingService comm;
        private readonly DiscordSocketClient __client;

        public StockMarketService(DataBaseHandlingService _db, CommandHandlingService com)
        {
            db = _db;
            comm = com;
        }

        public async Task InitAuctionSchedulers()
        {
            Collection<JObject> transaction = await db.getJObjects("transactions");
            foreach (JObject x in transaction)
            {
                if ((string)x["type"] == "auction")
                {
                    IndustryAuction auction = new IndustryAuction(x);
                    await auction.schedule(db, comm, this);
                }
            }
        }

        public async Task SetShares(string userID, string ticker, int amount)
        {
            UserShares SharesObj;

            try
            {
                SharesObj = new UserShares(await db.getJObjectAsync(userID, "shares"), true);
            }
            catch(System.NullReferenceException)
            {
                SharesObj = new UserShares(userID);
            }

            if (SharesObj.ownedShares.ContainsKey(ticker) == true)
            {
                SharesObj.ownedShares.Remove(ticker);
            }
            SharesObj.ownedShares.Add(ticker, amount);

            await db.SetJObjectAsync(db.SerializeObject<UserShares>(SharesObj), "shares");
        }

        public async Task<int> GetShares(string userID, string ticker)
        {
            UserShares SharesObj;

            try
            {
                SharesObj = new UserShares(await db.getJObjectAsync(userID, "shares"), true);
            }
            catch (System.NullReferenceException)
            {
                return 0;
            }

            if (SharesObj.ownedShares.ContainsKey(ticker) == false)
            {
                return 0;
            }
            else
            {
                return SharesObj.ownedShares[ticker];
            }
        }

        public async Task UpdateSharePrice(Transaction transaction)
        {
            Company company = new Company(await db.getJObjectAsync(transaction.ticker, "companies"));

            double newPrice = ((double)transaction.shares / (double)company.shares * (transaction.price - company.SharePrice)) + company.SharePrice;

            company.SharePrice = newPrice;
            await db.SetFieldAsync(transaction.ticker, "SharePrice", company.SharePrice, "companies");
        }

        public async Task<int> CorpShares(string ticker)
        {
            Company company = new Company(await db.getJObjectAsync(ticker, "companies"));
            Collection<string> shareholders = await db.getIDs("shares");
            Dictionary<string, int> ownedShares;
            int shares = 0;

            foreach(string ID in shareholders)
            {
                UserShares userShares = new UserShares(await db.getJObjectAsync(ID, "shares"), true);
                ownedShares = userShares.ownedShares;

                if (ownedShares.ContainsKey(ticker))
                {
                    shares += ownedShares[ticker];
                }
            }

            company.shares = shares;
            await db.SetJObjectAsync(db.SerializeObject<Company>(company), "companies");

            return shares;
        }

        public async Task<Collection<ulong>> GetShareholders(string ticker)
        {
            Collection<string> shareholders = await db.getIDs("shares");
            Dictionary<string, int> ownedShares;
            Collection<ulong> corpShareholders = new Collection<ulong>();
            
            foreach (string ID in shareholders)
            {
                UserShares userShares = new UserShares(await db.getJObjectAsync(ID, "shares"), true);
                ownedShares = userShares.ownedShares;

                if (ownedShares.ContainsKey(ticker))
                {
                    corpShareholders.Add(Convert.ToUInt64(ID));
                }
            }

            return corpShareholders;
        }
    }
}