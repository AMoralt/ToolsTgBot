using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using TelegramBot.Data;

namespace TelegramBot;

public class HandlingService
{
    private readonly List<TelegramCommand> _commands;
    public HandlingService(IServiceProvider services)
    {
        _commands = new List<TelegramCommand>
        {
            new StartCommand(),
            new HelpCommand(),
            new ZoomCreateCommand(), 
            new ZoomSheduleCommand(),
            new AddCommand(services)
        };
    }
    public async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
    {
        if (update.Type == UpdateType.Message || update.Type == UpdateType.InlineQuery)
        {
            Logger.Debug("User", update.Type.ToString());
            
            foreach (var command in _commands)
            {
                if (update.Type == UpdateType.InlineQuery)
                {
                    if (command is ZoomCreateCommand)
                    {
                        await command.Execute(update, bot);
                        break;
                    }
                }
                else if (command.Contains(update.Message))
                {
                    await command.Execute(update, bot);
                    break;
                }
            }
        }
        
    }
    public async Task ErrorHandler(ITelegramBotClient bot, Exception exception, CancellationToken arg3)
    {
        throw new NotImplementedException();
    }
}