using Discord.Commands;
using System.Threading.Tasks;

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
    }
}
