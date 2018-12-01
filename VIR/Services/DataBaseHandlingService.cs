using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIR.Properties;

namespace VIR.Services
{
    public class DataBaseHandlingService
    {
        private readonly RethinkDB r = RethinkDB.R;
        private readonly Connection conn;
        

        public DataBaseHandlingService()
        {
            try
            {
                conn = r.Connection().Hostname("127.0.0.1").Timeout(60).Connect();
            } catch(Exception e)
            {
                Process rdb = new Process();
                rdb.StartInfo.UseShellExecute = false;
                rdb.StartInfo.FileName = Resources.RethinkDBExec + "rethinkdb.exe";
                rdb.StartInfo.CreateNoWindow = true;
                rdb.StartInfo.WorkingDirectory = Resources.RethinkDBExec;
                bool work = rdb.Start();
                if(work)
                {
                    conn = r.Connection().Hostname("127.0.0.1").Timeout(60).Connect();
                } else
                {
                    throw new Exception();
                }
            }
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
