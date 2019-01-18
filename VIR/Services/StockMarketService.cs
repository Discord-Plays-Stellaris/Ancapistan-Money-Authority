﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIR.Services;
using VIR.Objects;
using VIR.Modules.Objects.Company;

namespace VIR.Services
{
    /// <summary>
    /// Contains methods for the stock market
    /// </summary>
    public class StockMarketService
    {
        private readonly DataBaseHandlingService db;

        public StockMarketService(DataBaseHandlingService _db)
        {
            db = _db;
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
            string _ticker = transaction.ticker;
            Company company = new Company(await db.getJObjectAsync(_ticker, "companies"));

            double newPrice = (transaction.shares / company.shares) * (transaction.price - company.SharePrice) + company.SharePrice;

            company.SharePrice = newPrice;
            await db.SetJObjectAsync(db.SerializeObject<Company>(company), "companies");
        }


    }
}
