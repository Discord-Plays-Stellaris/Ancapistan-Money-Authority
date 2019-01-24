using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VIR.Objects.Company
{
    class Industry
    {
        public string Id; // 
        public string Type; /* What resource this industry produces 
                            MNRL - Minerals
                            FOOD - Food
                            ALLY - Alloys
                            CSGD - Consumer Goods
                            RFML - Refined Minerals
                            RFFD - Refined Food
                            */
        public ulong YearlyOutput; // Output of resources per year
        public ulong UtilOutput; // Output of resources per Util spent
        public int Utils; // Amount of Utils the industry has left to spend (max 100)

        public Industry(string id, string type, ulong yearlyOutput, ulong utilOutput, int utils)
        {
            Id = id;
            Type = type;
            YearlyOutput = yearlyOutput;
            UtilOutput = utilOutput;
            Utils = utils;
        }

        public Industry(JObject dbInput)
        {
            Id = (string)dbInput["id"];
            Type = (string)dbInput["type"];
            YearlyOutput = (ulong)dbInput["yearlyOutput"];
            UtilOutput = (ulong) dbInput["utilOutput"];
            Utils = (int) dbInput["utils"];
        }
    }

    class Assets
    {
        public string id; // ticker
        public Dictionary<string,int> industries;
        public Dictionary<string,int> resources;

        public Assets(string ticker)
        {
            id = ticker;
            industries = new Dictionary<string, int>();
            resources = new Dictionary<string, int>();
        }

        public Assets(JObject json, bool isJSON)
        {
            Assets obj = JsonConvert.DeserializeObject<Assets>(json.ToString());

            id = obj.id;
            industries = obj.industries;
        }
    }

    class Resource
    {
        public string id;
        public bool processed; // false: raw resources, true: refined resource
        public double demandModifier; // a number which is mulitplied by a random int to get the resource's demand
        public int demand; 

        public Resource(string _id, bool _processed, double _demandmod, int _demand)
        {
            demand = _demand;
            demandModifier = _demandmod;
            id = _id;
            processed = _processed;
        }

        public Resource(JObject JSON)
        {
            demand = (int)JSON["demand"];
            demandModifier = (double)JSON["demandModifier"];
            id = (string)JSON["id"];
            processed = (bool)JSON["processed"];
        }
    }
}
