using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace VIR.Modules.Preconditions
{
    public class HasMasterOfBotsAttribute : PreconditionAttribute
    {
        //TODO: Comments required
        [IsInDPSGuild]
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var user = context.User as SocketGuildUser;
            var role2 = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Master of Bots");
            var role3 = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Developer");
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Bot Dev");
            var roleSkipper = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Owner");
            if (user.Roles.Contains(role) || user.Roles.Contains(role2) || user.Roles.Contains(role3) || user.Roles.Contains(roleSkipper))
            {
                return PreconditionResult.FromSuccess();
            } else
            {
                return PreconditionResult.FromError("You must have Master of Bots role to run this command.");
            }
        }
    }
}
