using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
        if (!ValidateMessage(update.Message.Text)) // if got wrong format message
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Неправильный формат, введите следующим образом:\nНазвание занятия - дата");
            return Name;
        }
        
        var database = _services.GetRequiredService<GoalDataContext>();
        var goal = await CreateGoal(update, database);
        
        await database.Goals.AddAsync(goal);
        
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "🤖 Занятие создано.");
        
        return string.Empty; //TODO Swap null and Name, It turns out unintuitive
    }
    private bool ValidateMessage(string? messageText)
    {
        //get message text, split it to array by '-' and validate it
        var messageTextSplit = messageText.Split('-');
        if (messageTextSplit.Length != 2)
        {
            return false;
        }
        return DateTime.TryParse(messageTextSplit[1], out _);
    }
    private async Task<Goal> CreateGoal(Update update, GoalDataContext database)
    {
        var text = update.Message.Text.Split("-");
        var user = (from u in database.Users
            where u.ChatId == update.Message.Chat.Id.ToString()
            select u)
            .FirstOrDefault()
        ;
        
        return new Goal()
        {
            User = user,
            UserId = user.Id,
            Name = text[0].Length > 60 ? text[0].Remove(60) : text[0],
            DueDate = DateTime.Parse(text[1]).ToUniversalTime().AddHours(5),
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