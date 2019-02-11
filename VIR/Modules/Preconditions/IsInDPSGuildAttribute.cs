using Discord.Commands;
using System;
using System.Threading.Tasks;
using VIR.Properties;

namespace VIR.Modules.Preconditions
{
    class IsInDPSGuildAttribute : PreconditionAttribute
    {
        //TODO: Comment Me pls
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            ulong guild;
            guild = ulong.Parse(Resources.guild);
            #if DEBUG
            guild = ulong.Parse(Resources.devguild);
            #endif
            if(context.Guild.Id.Equals(guild) || context.Guild.Id.Equals(498512980284276746))
            {
                return PreconditionResult.FromSuccess();
            } else
            {
                return PreconditionResult.FromError("This command must be run in a DPS guild.");
            }
        }
    }
}
