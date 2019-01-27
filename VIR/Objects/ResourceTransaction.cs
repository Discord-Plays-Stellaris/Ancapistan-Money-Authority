using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VIR.Objects
{
    public class ResourceTransaction
    {
        public string Id;
        public string IdSender;
        public string IdRecipient;
        public string Type;
        public ulong Amount;
        public ulong UnixTime;
        public int Year;

        public ResourceTransaction(string idSender, string idRecipient, string type, ulong amount, int year)
        {
            Id = Guid.NewGuid().ToString();
            IdSender = idSender;
            IdRecipient = idRecipient;
            Type = type;
            Amount = amount;
            UnixTime = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Year = year;
        }

        public ResourceTransaction(JObject json)
        {
            Id = (string) json["id"];
            IdSender = (string) json["idsender"];
            IdRecipient = (string) json["idrecipient"];
            Type = (string) json["type"];
            Amount = (ulong) json["amount"];
            UnixTime = (ulong) json["unixtime"];
            Year = (int) json["year"];
        }

        public JObject SerializeIntoJObject()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            var jObject = JObject.Parse(jsonString);
            return jObject;
        }
    }
}
