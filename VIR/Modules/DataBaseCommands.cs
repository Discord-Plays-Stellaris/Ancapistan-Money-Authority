using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIR.Modules.Preconditions;
using VIR.Services;

namespace VIR.Modules
{
    public class DataBaseCommands : ModuleBase<SocketCommandContext>
    {
        public DataBaseHandlingService DataBaseHandlingService { get; set; }

        [Command("balance")]
        public async Task BalanceAsync(IUser user = null)
        {
            if(user == null)
            {
                user = user ?? Context.User;
                string PP = DataBaseHandlingService.getFieldAsync(user.Id.ToString(), "pp");
                await user.SendMessageAsync($"Your current PP is: {PP}");
                await ReplyAsync("Your balance was sent to you privately.");
                return;
            }
            string age = DataBaseHandlingService.getFieldAsync(user.Id.ToString(), "age");
            await ReplyAsync($"Age of {user.Mention}: {age}");
        }

        [Command("get")]
        [HasMasterOfBots]
        [IsInDPSGuild]
        public async Task GetAsync([Summary("Field to get")] string field, [Summary("User")] IUser user)
        {
            string result = DataBaseHandlingService.getFieldAsync(user.Id.ToString(), field);
            await ReplyAsync($"{field} value: {result}");
        }

        [Command("set")]
        [HasMasterOfBots]
        [IsInDPSGuild]
        public async Task SetAsync<T>([Summary("Field to get")] string field, [Summary("User")] IUser user, [Summary("Value to set to")] T value)
        {
            await DataBaseHandlingService.setFieldAsync(user.Id.ToString(), field, value);
            await ReplyAsync($"{field} value set to {value}");
        }
    }
}
