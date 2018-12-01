using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using VIR.Services;

namespace VIR
{
    class Root
    {

        static void Main(string[] args) => new Root().MainAsync().GetAwaiter().GetResult(); //Start Main Task, this logs the bot in.

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var __client = services.GetRequiredService<DiscordSocketClient>(); //Create a new client in the Client Variable
                
                __client.Log += LogAsync;
                await __client.LoginAsync(TokenType.Bot, Properties.Resources.token); //Log in the bot
                await __client.StartAsync(); //Start the bot

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                await Task.Delay(-1); //Prevent stopping the program until it is closed
            }
        }

        //Logging task
        private Task LogAsync(LogMessage log)
        {
            Log.Logger(Log.Logs.INFO, log.ToString());
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}
