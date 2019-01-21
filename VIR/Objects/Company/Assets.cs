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
        public string id; // MINE, FARM, ALLOY, CG
        public string resource; // What resource this industry produces
        public int price; // The price of the industry

        public Industry(string _id, string _resource, int _price)
        {
            id = _id;
            resource = _resource;
            price = _price;
        }

        public Industry(JObject dbInput)
        {
            id = (string)dbInput["id"];
            resource = (string)dbInput["resiyrce"];
            price = (int)dbInput["price"];
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
