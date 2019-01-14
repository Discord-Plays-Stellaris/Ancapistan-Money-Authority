using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace VIR.Modules.Objects.Company
{
    public class Position
    {
        public string ID;
        public int level;
        public string name;
        public Collection<Position> manages;
        public Position()
        {
             
        }
        public Position(JObject obj)
        {
            ID = (string)obj["id"];
            level = (int)obj["level"];
            name = (string)obj["name"];
            foreach(JObject entry in (Array)obj["manages"])
            {
                manages.Add(new Position(entry));
            }
        }
        public JObject Deserialise()
        {
            JObject obj = new JObject();
            obj["id"] = ID;
            obj["level"] = level;
            obj["name"] = name;
            Collection<JObject> managesDeserialise = new Collection<JObject>();
            foreach(Position x in manages)
            {
                managesDeserialise.Add(x.Deserialise());
            }
            obj["manages"] = JArray.FromObject(managesDeserialise.ToArray());
            return obj;
        }
    }
}
