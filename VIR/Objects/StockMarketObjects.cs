using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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

        public Transaction(double _price, int _shares, string _type, string _author, string _ticker, string marketChannel)
        {
            price = _price;
            shares = _shares;
            type = _type;
            author = _author;
            ticker = _ticker;
        }
    }
}