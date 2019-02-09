using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIR.Objects
{
    class IndustrySellOffer
    {
        public string Id; // Id of the offer
        public string IndustryId; // Id of the industry being put up for sale
        public string MessageId; // Id of the sale offer
        public double Price; // Price of the industry

        public IndustrySellOffer(string id, string industryId, string messageId, double price)
        {
            Id = id;
            IndustryId = industryId;
            MessageId = messageId;
            Price = price;
        }

        public IndustrySellOffer(JObject dbInput)
        {
            Id = (string) dbInput["id"];
            IndustryId = (string) dbInput["industryid"];
            MessageId = (string) dbInput["messageid"];
            Price = (double) dbInput["price"];
        }

        public JObject SerializeIntoJObject()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            var jObject = JObject.Parse(jsonString);
            return jObject;
        }
    }
}
