using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace LooksToTheMoon {
    public static class Core {

        public static DiscordSocketClient Client;
        public static ServiceProvider ServiceProvider;
        public static CommandService CommandService;
        public static DateTime Startup = DateTime.Now;
        
        public static void Main() {
            
            AppConfiguration.FromJson();
            MainAsync().GetAwaiter().GetResult();
            
        }

        public static async Task MainAsync() {

            Client = new DiscordSocketClient(new DiscordSocketConfig {
                AlwaysDownloadUsers = true,
            });
            Client.Log += LoggingService.StaticLogger.FormatLogAsync; 
            CommandProcessed += LoggingService.StaticLogger.FormatLogAsync;
            ErrorEvent += LoggingService.StaticLogger.FormatExceptionAsync;
            Client.MessageReceived += OnCommandAsync;

            await Client.LoginAsync(TokenType.Bot, AppConfiguration.Key);
            await Client.StartAsync();
            
            ServiceProvider = new ServiceCollection()
                .AddSingleton(Client)
                .BuildServiceProvider();

            CommandService = new CommandService(new CommandServiceConfig {
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            });
            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);

            await Task.Delay(-1);

        }

        public static async Task OnCommandAsync(SocketMessage socketMessage) {

            if (socketMessage is not SocketUserMessage socketUserMessage) return;
            var context = new SocketCommandContext(Client, socketUserMessage);
            int readPosition = 0;
            StringComparison caseSensitive = AppConfiguration.PrefixCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            if (socketUserMessage.HasStringPrefix(AppConfiguration.Prefix, ref readPosition, caseSensitive) || socketUserMessage.HasMentionPrefix(Client.CurrentUser, ref readPosition)) {
                var result = await CommandService.ExecuteAsync(context, readPosition, ServiceProvider);
                if (!result.IsSuccess && result.Error == CommandError.UnknownCommand) {
                    await socketMessage.Channel.SendMessageAsync("Sorry, I didn't recognize the command, try again!");
                }

                LogSeverity logSeverity = result.IsSuccess ? LogSeverity.Info : LogSeverity.Warning;
                const string source = "Command Handler";
                string message = $"{socketMessage.Author.Username}#{socketMessage.Author.DiscriminatorValue} invoked a command in {socketMessage.Channel.Name}";
                if (!result.IsSuccess) message += ", but failed!";
                LogMessage logMessage = new LogMessage(logSeverity, source, message);
                CommandProcessed?.Invoke(logMessage);
                
                if (result.Error == CommandError.Exception) {
                    ErrorEvent?.Invoke((result as ExecuteResult? ?? default).Exception);
                }
            }

        }

        public static event Func<LogMessage, Task> CommandProcessed;
        public static event Func<Exception, Task> ErrorEvent;
    }
}