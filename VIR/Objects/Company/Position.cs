using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VIR.Modules.Objects.Company
{
    public class Position
    {
        /*
         * Level code explanation
         * 99 is max, 1 is min, determines whether or not a role can edit another role
         * Manages code explanation
         * If the role can manage other roles, add 2
         * If the role can read permissions, add 4
         * If the role can execute administrative commands, add 1
         */
        public string ID;
        public int level;
        public string name;
        public int manages;
        public Position()
        {
             
        }
        public Position(JObject obj)
        {
            ID = (string)obj["id"];
            level = (int)obj["level"];
            name = (string)obj["name"];
            manages = (int)obj["manages"];
        }
        public JObject Deserialise()
        {
            JObject obj = new JObject();
            obj["id"] = ID;
            obj["level"] = level;
            obj["name"] = name;
            obj["manages"] = manages;
            return obj;
        }
    }
}
