using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIR.Services;

namespace VIR.Modules
{
    public class DataBaseCommands : ModuleBase<SocketCommandContext>
    {
        public DataBaseHandlingService DataBaseHandlingService { get; set; }

        [Command("balance")]
        public async Task BalanceAsync(IUser user = null)
        {
            user = user ?? Context.User;
            string age = DataBaseHandlingService.getFieldAsync(user.Id.ToString(), "age");
            await ReplyAsync(age);
        }
    }
}
