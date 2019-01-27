using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VIR.Objects
{
    class ResourceMarketListing
    {
        public string Id; // Id of the listing
        public string Type;
        public string IdSeller;
        public ulong Amount;
        public double Price; // Price per unit
        public int YearPosted;
        public ulong UnixTimePosted;
        
        public ResourceMarketListing(string type, string idSeller, ulong amount, double price, int yearPosted)
        {
            Id = Guid.NewGuid().ToString();
            Type = type;
            IdSeller = idSeller;
            Amount = amount;
            Price = price;
            YearPosted = yearPosted;
            UnixTimePosted = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public ResourceMarketListing(JObject json)
        {
            Id = (string)json["id"];
            Type = (string)json["type"];
            IdSeller = (string)json["idseller"];
            Amount = (ulong)json["amount"];
            Price = (double)json["price"];
            YearPosted = (int)json["yearposted"];
            UnixTimePosted = (ulong)json["unixtimeposted"];
        }

        public JObject SerializeIntoJObject()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            var jObject = JObject.Parse(jsonString);
            return jObject;
        }
    }
}
