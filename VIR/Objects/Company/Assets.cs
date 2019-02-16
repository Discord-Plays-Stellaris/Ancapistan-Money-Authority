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
        public string id; // Id
        public string CompanyId; // Company ticker
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
        public string Planet; // What planet the industry is on

        public Industry(string _id, string type, ulong monthlyOutput, int utils, string planet)
        {
            id = _id;
            CompanyId = "";
            Type = type;
            MonthlyOutput = monthlyOutput;
            Utils = utils;
            Planet = planet;
        }

        public Industry(JObject dbInput)
        {
            id = (string)dbInput["id"];
            Type = (string)dbInput["Type"];
            MonthlyOutput = (ulong)dbInput["MonthlyOutput"];
            Utils = (int) dbInput["Utils"];
            Planet = (string) dbInput["Planet"];
            CompanyId = (string)dbInput["CompanyId"];
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
        
        public Resource(string id)
        {
            Id = id;
            Minerals = 0L;
            Food = 0L;
            Alloys = 0L;
            ConsumerGoods = 0L;
            RefinedMinerals = 0L;
            RefinedFood = 0L;
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
