﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VIR.Modules.Objects.Company
{
    public class Company
    {
        public string ticker;
        public string name;
        public Dictionary<string, Employee> employee; //Format: userid, title
        //public Dictionary<string, int> assets; //thing, amount
        public Dictionary<string, Position> positions;
        public int money;
        public Company()
        {

        }
        public Company(JObject companyDbEntry)
        {
            ticker = (string)companyDbEntry["id"];
            name = (string)companyDbEntry["name"];
            money = (int)companyDbEntry["money"];
            foreach(JObject entry in (Array) companyDbEntry["employee"])
            {
                employee.Add((string)entry["id"], new Employee(entry));
            }
        }
        public JObject serializeIntoJObject()
        {
            JObject temp = new JObject();
            temp["name"] = name;
            temp["money"] = money;
            temp["id"] = ticker;
            Collection<JObject> employeeDeserialise = new Collection<JObject>();
            foreach(Employee x in employee.Values)
            {
                employeeDeserialise.Add(x.Deserialise());
            }
            temp["employee"] = JArray.FromObject(employeeDeserialise.ToArray());
            return temp;
        }
    }
}