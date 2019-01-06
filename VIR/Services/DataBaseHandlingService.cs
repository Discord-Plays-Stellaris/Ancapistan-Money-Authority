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
        //TODO: Needs comments
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

        public async Task SetFieldAsync<T>(string userid, string fieldName, T value, string tableName) {
            await r.Db("root").Table(tableName).Insert(r.HashMap("id",userid).With(fieldName, value)).OptArg("conflict", "update").RunAsync(conn);
        }

        public async Task SetJObjectAsync(string userid, JObject obj, string tableName)
        {
            await r.Db("root").Table(tableName).Insert(r.HashMap("id", userid).With(obj)).OptArg("conflict", "update").RunAsync(conn);
        }

        public async Task<string> GetFieldAsync(string userid, string fieldName, string tableName)
        {
            JObject rawStr = await r.Db("root").Table(tableName).Get(userid).RunAsync(conn);
            if (rawStr == null)
                return null;

            if(rawStr[fieldName] == null)
                return null;

            return rawStr[fieldName].ToString();
        } 
        
        public async Task RemoveUserAsync(string userid)
        {
            await r.Db("wealth").Table("users").Get(userid).Delete().RunAsync(conn);
        }

        public async Task<JObject> getJObjectAsync(string id, string tableName)
        {
            return await r.Db("root").Table(tableName).Get(id).RunAsync(conn);
        }
    }
}
