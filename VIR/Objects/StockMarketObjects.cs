using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VIR.Services;

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

        public StockMarketChannel(string _channel)
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
        public string id;
        public double price;
        public int shares;
        public string type; // buy, sell, private
        public string author;
        public string ticker;


        public Transaction(JObject JSONInput)
        {
            id = (string)JSONInput["id"];
            price = (double)JSONInput["price"];
            shares = (int)JSONInput["shares"];
            type = (string)JSONInput["type"];
            author = (string)JSONInput["author"];
            ticker = (string)JSONInput["ticker"];
        }

        /// <summary>
        /// The constructor for a new transaction.
        /// </summary>
        /// <param name="_price">The price per share</param>
        /// <param name="_shares">The total amount of shares</param>
        /// <param name="_type">Buy, sell, or private</param>
        /// <param name="_author">The user who initiated the transaction</param>
        /// <param name="_ticker">The ticker of the company who's shares are being traded</param>
        /// <param name="db">A DataBaseHandlingService object.</param>
        public Transaction(double _price, int _shares, string _type, string _author, string _ticker, DataBaseHandlingService db)
        {
            price = _price;
            shares = _shares;
            type = _type;
            author = _author;
            ticker = _ticker;

        }

        private async Task Save(DataBaseHandlingService db)
        {
            await db.SetJObjectAsync(db.SerializeObject<Transaction>(this), "transactions");
            Console.Write("");
        }
    }

    public class UsersWithShares
    {
        public string id = "UsersWithShares";
        public List<string> UserList = new List<string>();

        public UsersWithShares()
        {
            // code
        }

        public UsersWithShares(JObject input)
        {
            UsersWithShares tempObj = JsonConvert.DeserializeObject<UsersWithShares>(input.ToString());

            id = tempObj.id;
            UserList = tempObj.UserList;
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
}