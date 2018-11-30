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
        public Task PingAsync()
            => ReplyAsync("Pong! Ping! Wait what?");
    }
}
