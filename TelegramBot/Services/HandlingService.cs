using System.Net.Mime;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Data;
using File = System.IO.File;

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
                var user = await GetUserFromDb(update);//TODO checking user in DB without a handle command is not good
                foreach (var command in _commands)
                {
                    if (command.Contains(update, user.LastCommand))
                    {
                        await Logger.LogAsync(command.GetType().Name ,"Start command");
                        
                        var executedCommand = command.Execute(update, bot);//Execute returns string Name of command 
                        user.LastCommand = await executedCommand; 
                        await _database.SaveChangesAsync();
                        
                        await Logger.LogAsync(command.GetType().Name ,"End command");
                        break;
                    }
                }
            }
    }
    public async Task ErrorHandler(ITelegramBotClient bot, Exception exception, CancellationToken arg3)
    {
        await Logger.LogAsync("Error", exception.Message);
        var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent + "\\errors.txt";
        var text = "Location:\n" + JsonSerializer.Serialize(exception.StackTrace);
        File.WriteAllText(path, text);
        AppControl.Exit();
    }

    private async Task<User> GetUserFromDb(Update update)
    {
        var user = (from u in _database.Users 
            where u.ChatId == update.Message.Chat.Id.ToString() 
            select u)
            .FirstOrDefault()
        ;
        
        if (user == null) //if user is not register in our database
        {
            user = new User()
            {
                ChatId = update.Message.Chat.Id.ToString(),
                LastCommand = null
            };
            await _database.Users.AddAsync(user);
            await _database.SaveChangesAsync();
        }
        return user;
    }
}