using Discord.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VIR.Modules.Objects.Company;
using VIR.Modules.Preconditions;
using VIR.Services;
using VIR.Objects;

namespace VIR.Modules
{
    public class StockMarketCommands : ModuleBase
    {
        DataBaseHandlingService db;

        public StockMarketCommands()
        {
            db = new DataBaseHandlingService();
        }

        [Command("namemarket")]
        [HasMasterOfBots]
        public async Task NameMarketTask(string acronym, [Remainder]string marketName)
        {
            StockMarketObject marketObj = new StockMarketObject(acronym, marketName);

            JObject JSONObj = await db.SerializeObject<StockMarketObject>(marketObj);

            db.SetJObjectAsync(JSONObj, "system");

            await ReplyAsync("Market renamed to " + marketName + ", with the acronym of " + acronym);
        }
    }
}
