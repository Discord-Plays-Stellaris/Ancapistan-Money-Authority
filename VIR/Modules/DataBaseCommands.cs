using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using VIR.Modules.Preconditions;
using VIR.Services;

namespace VIR.Modules
{
    public class DataBaseCommands : ModuleBase<SocketCommandContext>
    {
        //TODO: Comment this
        private readonly DataBaseHandlingService DataBaseHandlingService;
        private readonly AgeService AgeService;
        private readonly CompanyService CompanyService;

        public DataBaseCommands(DataBaseHandlingService db, AgeService age, CompanyService cs)
        {
            DataBaseHandlingService = db;
            AgeService = age;
            CompanyService = cs;
        }

        [Command("balance")]
        [Summary("Returns your balance")]
        public async Task BalanceAsync([Summary("Optional: An user whose age you wish to get")]IUser user = null)
        {
            string aget;
            int age;
            string moneyt;
            double money;
            if (user == null)
            {
                user = user ?? Context.User;
                aget = (string) await DataBaseHandlingService.GetFieldAsync(user.Id.ToString(), "age", "users");
                moneyt = (string) await DataBaseHandlingService.GetFieldAsync(user.Id.ToString(), "money", "users");

                if (aget == null)
                {
                    Random rand = new Random(); //Set up a RNG
                    age = rand.Next(20, 25); //Get num between 20 and 25
                    await DataBaseHandlingService.SetFieldAsync<int>(user.Id.ToString(), "age", age, "users");
                }
                else
                {
                    age = int.Parse(aget);
                }

                if (moneyt == null)
                {
                    money = 50000;
                    await DataBaseHandlingService.SetFieldAsync<double>(user.Id.ToString(), "money", money, "users");
                }
                else
                {
                    money = double.Parse(moneyt);
                }

                string PPt;
                int pp;
                PPt = (string) await DataBaseHandlingService.GetFieldAsync(user.Id.ToString(), "pp", "users");

                if(PPt == null) {
                    pp = 0;
                    await DataBaseHandlingService.SetFieldAsync(user.Id.ToString(), "pp", pp, "users");
                } else
                {
                    pp = int.Parse(PPt);
                }

                EmbedFieldBuilder ppField = new EmbedFieldBuilder().WithIsInline(false).WithName("PI:").WithValue(pp.ToString());
                EmbedFieldBuilder moneyField = new EmbedFieldBuilder().WithIsInline(false).WithName("Money:").WithValue($"${money.ToString()}");
                Embed embedd = new EmbedBuilder().WithImageUrl(Context.User.GetAvatarUrl()).WithFooter($"User ID: {Context.User.Id.ToString()}").WithTitle($"Inventory of {Context.User.Username}").WithDescription($"Age: {age.ToString()}").AddField(ppField).AddField(moneyField).WithColor(Color.Green).Build();

                await user.SendMessageAsync("", false, embedd);
                await ReplyAsync("Your balance was sent to you privately.");
                return;
            }
            aget = (string) await DataBaseHandlingService.GetFieldAsync(user.Id.ToString(), "age", "users");
            if(aget == null) {
                Random rand = new Random(); //Set up a RNG
                age = rand.Next(20, 25); //Get num between 20 and 25
                await DataBaseHandlingService.SetFieldAsync(user.Id.ToString(), "age", age, "users");
            } else
            {
                age = int.Parse(aget);
            }
            await ReplyAsync($"Age of {user.Mention}: {age.ToString()}");
        }

        [Command("transfer")]
        public async Task Transfer(IUser user, string amount)
        {
            string moneyt = (string)await DataBaseHandlingService.GetFieldAsync(user.Id.ToString(), "money", "users");
            double money;
            if (moneyt == null)
            {
                money = 50000;
                await DataBaseHandlingService.SetFieldAsync<double>(user.Id.ToString(), "money", money, "users");
            }
            else
            {
                money = double.Parse(moneyt);
            }
            string money2t = (string)await DataBaseHandlingService.GetFieldAsync(Context.User.Id.ToString(), "money", "users");
            double money2;
            if (money2t == null)
            {
                money2 = 50000;
                await DataBaseHandlingService.SetFieldAsync<double>(Context.User.Id.ToString(), "money", money, "users");
            }
            else
            {
                money2 = double.Parse(money2t);
            }
            if(double.Parse(amount) <= 0)
            {
                await ReplyAsync("You cannot give less than or equal to 0 credits");
            } else if(money2 - double.Parse(amount) < 0)
            {
                await ReplyAsync("You can not give more money than you have");
            } else
            {
                money += double.Parse(amount);
                await DataBaseHandlingService.SetFieldAsync(user.Id.ToString(), "money", money, "users");
                money2 -= double.Parse(amount);
                await DataBaseHandlingService.SetFieldAsync(Context.User.Id.ToString(), "money", money2, "users");
                await ReplyAsync($"{amount} sent to {user.Username}!");
            }
        }

        [Command("get")]
        [HasMasterOfBots]
        [IsInDPSGuild]
        public async Task GetAsync([Summary("Field to get")] string field, [Summary("User")] IUser user)
        {
            string result = (string) await DataBaseHandlingService.GetFieldAsync(user.Id.ToString(), field, "users");
            await ReplyAsync($"{field} value: {result}");
        }

        [Command("set")]
        [HasMasterOfBots]
        [IsInDPSGuild]
        public async Task SetAsync([Summary("Field to get")] string field, [Summary("User")] IUser user, [Summary("Value to set to")] int value)
        {
            await DataBaseHandlingService.SetFieldAsync(user.Id.ToString(), field, value, "users");
            await ReplyAsync($"{field} value set to {value}. If you updated PI, please remember to update the sheet.");
        }

        [Command("add")]
        [Alias("modify")]
        [HasMasterOfBots]
        [IsInDPSGuild]
        public async Task AddAsync([Summary("Field to get")] string field, [Summary("User")] IUser user, [Summary("Value to add")] int value)
        {
            string x = (string) await DataBaseHandlingService.GetFieldAsync(user.Id.ToString(), field, "users");
            int val;
            if (x == null)
            {
                val = 0;
            }
            else
            {
                val = int.Parse(x);
            }
            await DataBaseHandlingService.SetFieldAsync(user.Id.ToString(), field, value + val, "users");
            await ReplyAsync($"{field} value modified by {value}. If you updated PI, please remember to update the sheet.");
        }

        [Command("advance")]
        [HasMasterOfBots]
        [IsInDPSGuild]
        public async Task AdvanceAsync([Summary("Years to advance")] string years)
        {
            int yearsi = int.Parse(years);
            SocketGuild guild = Context.Guild;
            await AgeService.AdvanceAllAsync(guild, yearsi);
            await CompanyService.paySalaries(yearsi);
            await ReplyAsync($"Time advanced by {years}");
        }

        [Command("kill")]
        [HasMasterOfBots]
        [IsInDPSGuild]
        public async Task KillAsync([Summary("Person to kill")] IUser user)
        {
            var userg = user as IGuildUser;
            await AgeService.KillAsync(userg);
            await ReplyAsync($"Killed {user.Mention}");
        }
    }
}
