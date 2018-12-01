using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIR.Services
{
    public class DataBaseHandlingService
    {
        private readonly RethinkDB r = RethinkDB.R;
        private readonly Connection conn;

        private Exception NonexistantFieldException;

        public DataBaseHandlingService()
        {
            conn = r.Connection().Hostname("127.0.0.1").Timeout(60).Connect();
        }

        public async Task setFieldAsync<T>(string userid, string fieldName, T value) {
            await r.Db("wealth").Table("users").Get(userid).Update(r.HashMap(fieldName,value)).RunAsync(conn);
        }

        public string getFieldAsync(string userid, string fieldName)
        {
            var rawStr = r.Db("wealth").Table("users").Get(userid).Run(conn).ToString();
            JObject Str = JObject.Parse(rawStr);
            return (string) Str[fieldName];
        } 
    }
}
