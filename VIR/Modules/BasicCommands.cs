using Discord.Commands;
using VIR.Modules.Preconditions;
using System.Threading.Tasks;
using System;

namespace VIR.Modules
{
    public class BasicCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Alias("pong")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong! Ping! Wait what? :ping_pong:");
        }

        [Command("quit")]
        [HasMasterOfBots]
        public async Task SkipperAsync()
        {
            await ReplyAsync("Quitting...");
            Environment.Exit(0);
        }
    }
}
