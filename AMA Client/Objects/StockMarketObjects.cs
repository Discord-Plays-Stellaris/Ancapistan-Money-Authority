using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Discord;

namespace AMA_Client.Objects
{
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
        public ulong messageID; 


        public Transaction(JObject JSONInput)
        {
            id = (Guid)JSONInput["id"];
            price = (double)JSONInput["price"];
            shares = (int)JSONInput["shares"];
            type = (string)JSONInput["type"];
            author = (string)JSONInput["author"];
            ticker = (string)JSONInput["ticker"];
            messageID = (ulong)JSONInput["messageID"];
        }
    }

    public class UserShares
    {
        public string id;
        public Dictionary<string, int> ownedShares = new Dictionary<string, int>(); // Format: Ticker, Shares

        public UserShares()
        {

        }

        public UserShares(JObject input)
        {
            if(input != null)
            {
                UserShares tempObj = JsonConvert.DeserializeObject<UserShares>(input.ToString());
                id = tempObj.id;
                ownedShares = tempObj.ownedShares;
            }
            if(input == null)
            {
                return;
            }
        }
    }

    public class ShareholderVote
    {
        public Guid id { get; private set; }
        public Collection<string> choices { get; private set; }
        public Dictionary<ulong,ulong> messages { get; private set; }
        public string ticker { get; private set; }

        public void JSON(JObject input)
        {
            ShareholderVote temp = JsonConvert.DeserializeObject<ShareholderVote>(input.ToString());

            id = temp.id;
            choices = temp.choices;
            ticker = temp.ticker;
            messages = temp.messages;
        }
    }
}