using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIR.Modules
{
    public class BasicCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Alias("pong")]
        public async Task PingAsync()
        {
            System.Threading.Thread.Sleep(15000);
            await ReplyAsync("Pong! Ping! Wait what?");
        }
    }
}
