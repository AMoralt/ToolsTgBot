using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace TelegramBot;

public class CommandHandlingService
{
    private readonly List<TelegramCommand> _commands;
    public CommandHandlingService()
    {
        _commands = new List<TelegramCommand>
        {
            new StartCommand(),
            new HelpCommand(),
            new ZoomCreateCommand(), 
            new ZoomSheduleCommand() 
        };
    }
    public async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
    {
        if (update.Type == UpdateType.InlineQuery)
        {
            Logger.Debug("User","Inline");
            foreach (var command in _commands)
            {
                if (command is ZoomCreateCommand)
                {
                    await command.Execute(update, bot);
                    break;
                }
            }
        }
        if (update.Message.Type == MessageType.Text)
        {
            Logger.Debug(update.Message.Chat.Username, update.Message.Text);
            
            foreach (var command in _commands)
            {
                if (command.Contains(update.Message))
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