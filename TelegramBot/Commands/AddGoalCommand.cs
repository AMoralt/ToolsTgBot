using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;

namespace TelegramBot;

public class AddGoalCommand : TelegramCommand
{
    private readonly IServiceProvider _services;
    public AddGoalCommand(IServiceProvider services)
    {
        _services = services;
    }
    public override string Name => Config.CommandNames["AddGoalCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Handling AddGoalCommand");
        
        if (!ValidateMessage(update.Message.Text)) // if got wrong format message
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Неправильный формат, введите следующим образом:\nНазвание занятия - дата");
            return Name;
        }
        
        var _database = _services.GetRequiredService<GoalDataContext>();
        
        var user = (from u in _database.Users
            where u.ChatId == update.Message.Chat.Id.ToString()
            select u).SingleOrDefault();
        
        var goal = CreateGoal(update, user);
        _database.Goals.Add(goal);
        
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "🤖 Занятие создано.");
        
        Logger.Debug("Bot", "End AddGoalCommand");
        return null; //TODO Swap null and Name, It turns out unintuitive
    }
    private bool ValidateMessage(string? messageText)
    {
        try
        {
            return DateTime.TryParse(messageText.Split("-")[1], out var date);
        }
        catch
        {
            return false;
        }
    }
    private Goal CreateGoal(Update update, User user)
    {
        return new Goal()
        {
            User = user,
            UserId = user.Id,
            GoalName = update.Message.Text.Split("-")[0],
            DueDate = Convert.ToDateTime(update.Message.Text.Split("-")[1]).ToUniversalTime().AddHours(5),
            ArchiveDate = null
        };
    }
    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type != UpdateType.Message)
            return false;

        return !update.Message.Text.StartsWith("/") && lastmessage == Name;
    }
}