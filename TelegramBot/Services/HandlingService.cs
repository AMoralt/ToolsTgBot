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
            new CreateZoomCommand(),
            new SheduleCommand(),
            new SheduleZoomCommand(),
            new AddCommand(),
            new AddGoalCommand(services),
            new ShowCommand(services),
            new ShowArchiveCommand(services),
        };
        _database = services.GetRequiredService<GoalDataContext>();
    }
    public async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
    {
        if (update.Type == UpdateType.Message)
            if (update.Message.Type == MessageType.Text)
            {
                var user = GetUserFromDB(update);

                foreach (var command in _commands)
                {
                    if (command.Contains(update, user.LastCommand))
                    {
                        user.LastCommand = await command.Execute(update, bot); //Execute returns string Name of command TODO change method's name 
                        _database.SaveChanges();
                        break;
                    }
                }
            }
    }
    public async Task ErrorHandler(ITelegramBotClient bot, Exception exception, CancellationToken arg3)
    {
        Logger.Debug("HandlingService", exception.Message);
        AppControl.Exit();
    }

    private User GetUserFromDB(Update update)
    {
        var user = (from u in _database.Users
            where u.ChatId == update.Message.Chat.Id.ToString()
            select u).SingleOrDefault();
        
        if (user == null) //if user is not register in our database
        {
            user = new User()
            {
                ChatId = update.Message.Chat.Id.ToString(),
                LastCommand = null
            };
            _database.Users.Add(user);
            _database.SaveChanges();
        }
        return user;
    }
}