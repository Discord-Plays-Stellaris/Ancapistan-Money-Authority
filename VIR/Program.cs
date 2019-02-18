using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using VIR.Services;
using VIR.Properties;

namespace VIR
{
    class Root
    {
        static ServiceProvider servicess;
        static void Main(string[] args) => new Root().MainAsync().GetAwaiter().GetResult(); //Start Main Task, this logs the bot in.

        public async Task MainAsync()
        {
            bool isDebugMode = false;
            #if DEBUG
            isDebugMode = true;
            #endif

            using (var services = ConfigureServices())
            {
                var __client = services.GetRequiredService<DiscordSocketClient>(); //Create a new client in the Client Variable

                servicess = services;

                __client.Log += LogAsync;
                __client.Connected += onConnected;
                string botToken;
                if (isDebugMode)
                {
                    botToken = Resources.tokendev;
                } else
                {
                    botToken = Resources.tokenpublish;
                }

                await __client.LoginAsync(TokenType.Bot, botToken); //Log in the bot
                await __client.StartAsync(); //Start the bot

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                await Task.Delay(-1); //Prevent stopping the program until it is closed
            }
        }
        
        private async Task onConnected()
        {
            await servicess.GetRequiredService<StockMarketService>().InitAuctionSchedulers();
        }

        //Logging task
        private Task LogAsync(LogMessage log)
        {
            Log.Logger(Log.Logs.INFO, log.ToString());
            return Task.CompletedTask;
        }

        //Initialise all services
        private ServiceProvider ConfigureServices()
        {
            DiscordSocketClient tmp2 = new DiscordSocketClient(new DiscordSocketConfig { HandlerTimeout = null, WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance });
            CommandService tmp = new CommandService(new CommandServiceConfig{CaseSensitiveCommands = false, DefaultRunMode = RunMode.Async, }); //Command Handling Service
            return new ServiceCollection()
                .AddSingleton(tmp2) //Discord Client Service
                .AddSingleton(tmp) //Command Service
                .AddSingleton<CommandHandlingService>() //Command Handling Service
                .AddSingleton<HttpClient>() //HTTP client
                .AddSingleton<DataBaseHandlingService>() //Database Handling Service, manipulates rDB data
                .AddSingleton<AgeService>() //Age Service, sort of a wrapper for Database Handling Service
                .AddSingleton<CompanyService>()
                .AddSingleton<StockMarketService>() // Methods and shit for the Stock Market. #bestservice
                .AddSingleton<ResourceHandlingService>()
                .AddSingleton<IndustryService>()
                .BuildServiceProvider();
        }
    }
}
