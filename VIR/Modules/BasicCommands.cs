using Discord.Commands;
using VIR.Modules.Preconditions;
using System.Threading.Tasks;
using System;
using VIR.Services;
using System.Collections.Generic;
using Discord;

namespace VIR.Modules
{
    public class BasicCommands : ModuleBase<SocketCommandContext>
    {
        public readonly CommandHandlingService comhan;
        
        public BasicCommands(CommandHandlingService comm)
        {
            comhan = comm;
        }

        [Command("ping")]
        [Alias("pong")]
        [Summary("A basic ping command.")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong! Ping! Wait what? :ping_pong:");
        }

        [Command("quit")]
        [HasMasterOfBots]
        public async Task QuitAsync()
        {
            await ReplyAsync("Quitting...");
            Environment.Exit(0);
        }

        [Command("help")]
        [Summary("Returns this command.")]
        public async Task ThereIsNoHelp()
        {
            IEnumerable<CommandInfo> commands = await comhan.getCommands();
            EmbedBuilder embed = new EmbedBuilder();
            int fieldCounter = 0;
            foreach(CommandInfo x in commands)
            {
                bool doable = true;
                foreach(PreconditionAttribute y in x.Preconditions)
                {
                    if(!(await y.CheckPermissionsAsync(Context, x, null)).IsSuccess)
                    {
                        doable = false;
                    }
                }
                if(doable && fieldCounter < 25)
                {
                    string tmpp = "";
                    if (x.Parameters.Count > 0)
                    {
                        foreach (ParameterInfo z in x.Parameters)
                        {
                            tmpp = tmpp + $"{z.Name}: {z.Summary}({z.Type})\n";
                        }
                    } else
                    {
                        tmpp = "None.";
                    }
                    EmbedFieldBuilder tmp = new EmbedFieldBuilder().WithName(x.Name).WithValue($"{x.Summary} Arguments: {tmpp}");
                    embed.AddField(tmp);
                    fieldCounter++;
                }
            }
            await ReplyAsync("The help list has been DMed to you.");
            await Context.User.SendMessageAsync(null, false, embed.Build());
        }
    }
}
