using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VIR.Services;

namespace VIR.Objects.Company
{
    class Industry
    {
        public string Id; // Ticker
        public string Type; /* What resource this industry produces 
                            MNRL - Minerals
                            FOOD - Food
                            ALLY - Alloys
                            CSGD - Consumer Goods
                            RFML - Refined Minerals
                            RFFD - Refined Food
                            */
        public ulong MonthlyOutput; // Output of resources per year
        public int Utils; // Amount of Utils the industry has left to spend (max 100)
        public string PlanetId; // What planet the industry is on

        public Industry(string id, string type, ulong monthlyOutput, int utils, string planetId)
        {
            Id = id;
            Type = type;
            MonthlyOutput = monthlyOutput;
            Utils = utils;
            PlanetId = planetId;
        }

        public Industry(JObject dbInput)
        {
            Id = (string)dbInput["id"];
            Type = (string)dbInput["type"];
            MonthlyOutput = (ulong)dbInput["monthlyOutput"];
            Utils = (int) dbInput["utils"];
            PlanetId = (string) dbInput["planetId"];
        }

        public JObject SerializeIntoJObject()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            var jObject = JObject.Parse(jsonString);
            return jObject;
        }
    }

    public class Resource
    {
        public string Id; // Id of the owner, can either be a company or a person
        public ulong Minerals;
        public ulong Food;
        public ulong Alloys;
        public ulong ConsumerGoods;
        public ulong RefinedMinerals;
        public ulong RefinedFood;

        // This makes a new entry into the Resources table, only use this when making a new entry
        public Resource(string id)
        {
            Id = id;
            Minerals = 0L;
            Food = 0L;
            Alloys = 0L;
            ConsumerGoods = 0L;
            RefinedMinerals = 0L;
            RefinedFood = 0L;

            var check = ResourceHandlingService._instance.GetResource(id);

            if (check!=null)
            {
                var alreadyExists = new Resource(check);
                Minerals = alreadyExists.Minerals;
                Food = alreadyExists.Food;
                Alloys = alreadyExists.Alloys;
                ConsumerGoods = alreadyExists.ConsumerGoods;
                RefinedMinerals = alreadyExists.RefinedMinerals;
                RefinedFood = alreadyExists.RefinedFood;
            }

            ResourceHandlingService._instance.InsertIntoResources(this.SerializeIntoJObject());
        }

        public Resource(JObject JSON)
        {
            Id = (string) JSON["id"];
            Minerals = (ulong) JSON["minerals"];
            Food = (ulong)JSON["food"];
            Alloys = (ulong)JSON["alloys"];
            ConsumerGoods = (ulong)JSON["consumergoods"];
            RefinedMinerals = (ulong)JSON["refinedminerals"];
            RefinedFood = (ulong)JSON["refinedfood"];
        }

        public JObject SerializeIntoJObject()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            var jObject = JObject.Parse(jsonString);
            return jObject;
        }
    }
}
