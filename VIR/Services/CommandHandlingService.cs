using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VIR.Services
{
    //TODO: Comment this mess
    public class CommandHandlingService
    {
        private readonly CommandService __commands;
        private readonly DiscordSocketClient __client;
        private readonly IServiceProvider __services;

        public CommandHandlingService(IServiceProvider services)
        {
            __commands = services.GetRequiredService<CommandService>();
            __client = services.GetRequiredService<DiscordSocketClient>();
            __services = services;

            __commands.CommandExecuted += CommandExecutedAsync;
            __commands.Log += Logging;
            __client.MessageReceived += MessageReceivedAsync;

        }

        public async Task InitializeAsync()
        {
            await __commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task MessageReceivedAsync(SocketMessage msg)
        {
            if (!(msg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if ((!message.HasMentionPrefix(__client.CurrentUser, ref argPos)) || !message.HasCharPrefix('&', ref argPos)) return;

            var context = new SocketCommandContext(__client, message);
            await __commands.ExecuteAsync(context, argPos, __services);
        }

        public async Task CommandExecutedAsync(CommandInfo command, ICommandContext context, IResult result)
        {
            /*if (!command.IsSpecified)
                return;*/

            if (result.IsSuccess)
                return;

            await Log.Logger(Log.Logs.ERROR, $"A problem occured running a command: {result.ToString()}.");
        }

        private Task Logging(LogMessage log)
        {
            Log.Logger(Log.Logs.INFO, log.ToString());
            return Task.CompletedTask;
        }
    }
}
