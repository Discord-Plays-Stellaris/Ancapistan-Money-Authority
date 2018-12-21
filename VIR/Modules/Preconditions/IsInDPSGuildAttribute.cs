using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIR.Modules.Preconditions
{
    class IsInDPSGuildAttribute : PreconditionAttribute
    {
        //TODO: Comment Me pls
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if(context.Guild.Id.Equals(481865251688808469) || context.Guild.Id.Equals(498512980284276746) || context.Guild.Id.Equals(463607272191295489) || context.Guild.Id.Equals(454357669495701514))
            {
                return PreconditionResult.FromSuccess();
            } else
            {
                return PreconditionResult.FromError("This command must be run in a DPS guild.");
            }
        }
    }
}
