using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using TelegramBot.Data;

namespace TelegramBot;

public class HandlingService
{
    private readonly List<TelegramCommand> _commands;
    private readonly GoalDataContext _database;
    public HandlingService(IServiceProvider services)
    {
        _commands = new List<TelegramCommand>
        {
            new StartCommand(),
            new HelpCommand(),
            new ZoomCreateCommand(), 
            new ZoomSheduleCommand(),
            new AddCommand(services),
            new ShowCommand(services),
            new ShowArchiveCommand(services),
        };
        _database = services.GetRequiredService<GoalDataContext>();
    }
    public async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
    {
        var inDataBase = _database.Users
            .Where(x => x.ChatId == update.Message.Chat.Id.ToString());
        if (update.Type == UpdateType.Message)
        {
            if (inDataBase.IsNullOrEmpty())
            {
                _database.Users.Add(new User
                {
                    ChatId = update.Message.Chat.Id.ToString(),
                    LastMessage = null
                });
                _database.SaveChanges();
            }
        }
        
        foreach (var command in _commands)
        {
            if (command.Contains(update))
            {
                await command.Execute(update, bot);
                inDataBase.First().LastMessage = update.Message.Text;
                _database.SaveChanges();
                break;
            }
        }
    }

    public async Task ErrorHandler(ITelegramBotClient bot, Exception exception, CancellationToken arg3)
    {
        throw new NotImplementedException();
    }
}