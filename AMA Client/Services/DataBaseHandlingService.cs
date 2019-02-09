using Newtonsoft.Json.Linq;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System;
using AMA_Client.Properties;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMA_Client.Services
{
    public static class db
    {
        //TODO: Needs comments
        private static readonly RethinkDB r = RethinkDB.R;
        private static Connection conn;
        

        public static async Task Connect()
        {
            try
            {
                conn = r.Connection().Hostname("127.0.0.1").Timeout(60).Connect();
            } catch(Exception e)
            {
                Error.Throw("A connection to the database could not be established. This may be due to a bad internet connection, or the database may be down.");
            }
        }

        /// <summary>
        /// Sets a specific field within an object.
        /// </summary>
        /// <typeparam name="T">The object type of the field's value.</typeparam>
        /// <param name="userid">The ID of the document</param>
        /// <param name="fieldName">The name of the field</param>
        /// <param name="value">The value of the field</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        public static async Task SetFieldAsync<T>(string userid, string fieldName, T value, string tableName) {
            await r.Db("root").Table(tableName).Insert(r.HashMap("id",userid).With(fieldName, value)).OptArg("conflict", "update").RunAsync(conn);
        }

        /// <summary>
        /// Inserts a JObject into a table. If an object with that ID already exists, then update it.
        /// </summary>
        /// <param name="obj">The JObject to be inserted</param>
        /// <param name="tableName">The table of the object.</param>
        /// <param name="strict">Whether or not should an id conflict throw an exception. Useful when you know obj is a new addition to the db.</param>
        /// <returns></returns>
        public static async Task SetJObjectAsync(JObject obj, string tableName, bool strict = false)
        {
            if(strict)
            {
                bool contains = r.Db("root").Table(tableName).Contains(obj["id"]).Run(conn);
                if(contains)
                {
                    throw new Exception("IdMismatchException");
                } else
                {
                    await r.Db("root").Table(tableName).Insert(obj).OptArg("conflict", "update").RunAsync(conn);
                }
            } else
            {
                await r.Db("root").Table(tableName).Insert(obj).OptArg("conflict", "update").RunAsync(conn);
            }
        }

        /// <summary>
        /// Gets a specific field from a document.
        /// </summary>
        /// <param name="userid">The document ID</param>
        /// <param name="fieldName">The field to be retrived</param>
        /// <param name="tableName">The name of the table of the document.</param>
        /// <returns></returns>
        public static JToken GetField(string userid, string fieldName, string tableName)
        {
            JObject rawStr = r.Db("root").Table(tableName).Get(userid).Run(conn);
            if (rawStr == null)
                return null;

            if(rawStr[fieldName] == null)
                return null;

            return rawStr[fieldName];
        } 
        
        /// <summary>
        /// Removes a user from the DB
        /// </summary>
        /// <param name="userid">The ID of the user</param>
        /// <returns></returns>
        public static async Task RemoveUserAsync(string userid)
        {
            await r.Db("wealth").Table("users").Get(userid).Delete().RunAsync(conn);
        }

        /// <summary>
        /// Removes a document from the DB
        /// </summary>
        /// <param name="id">The ID of the document</param>
        /// <param name="table">The table of the document</param>
        /// <returns></returns>
        public static async Task RemoveObjectAsync(string id, string table)
        {
            await r.Db("root").Table(table).Get(id).Delete().RunAsync(conn);
        }

        /// <summary>
        /// Gets a JObject from the database.
        /// </summary>
        /// <param name="id">The ID of the document</param>
        /// <param name="tableName">The table of the document</param>
        /// <returns>A JObject</returns>
        public static JObject getJObject(string id, string tableName)
        {
            return r.Db("root").Table(tableName).Get(id).Run(conn);
        }

        /// <summary>
        /// Converts an object to a JObject
        /// </summary>
        /// <typeparam name="T">The type of the input object.</typeparam>
        /// <param name="input">The input object</param>
        /// <returns>A JObject</returns>
        public static JObject SerializeObject<T>(T input)
        {
            string JSONString = JsonConvert.SerializeObject(input);
            JObject jObject = JObject.Parse(JSONString);
            return jObject;
        }

        /// <summary>
        /// Returns all the IDs in a table in an array
        /// </summary>
        /// <param name="tableName">The table to get IDs from</param>
        /// <returns>A collection with all of the IDs</returns>
        public static async Task<Collection<string>> getIDs(string tableName)
        {
            Collection<string> ids = new Collection<string>();
            Cursor<JObject> allEntries = await r.Db("root").Table(tableName).RunAsync<JObject>(conn);
            foreach(JObject x in allEntries)
            {
                ids.Add((string)x["id"]);
            }
            return ids;
        }
        /// <summary>
        /// Returns all the JObjects in a table in an array
        /// </summary>
        /// <param name="tableName">The table to get JObjects from</param>
        /// <returns>A collection with all of the JObjects</returns>
        public static async Task<Collection<JObject>> getJObjects(string tableName)
        {
            Collection<JObject> ids = new Collection<JObject>();
            Cursor<JObject> allEntries = await r.Db("root").Table(tableName).RunAsync<JObject>(conn);
            foreach (JObject x in allEntries)
            {
                ids.Add(x);
            }
            return ids;
        }
    }
}
