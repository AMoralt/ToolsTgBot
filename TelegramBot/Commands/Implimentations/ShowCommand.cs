using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;

namespace TelegramBot;

public class ShowCommand : TelegramCommand
{
     private readonly IServiceProvider _services;
     public ShowCommand(IServiceProvider services)
     {
         _services = services;
     }
    public override string Name => Config.CommandNames["ShowCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Handling ShowCommand");

        var _database = _services.GetRequiredService<GoalDataContext>();
        
        var list = from user in _database.Users
            join goal in _database.Goals on user.Id equals goal.UserId  
            where user.ChatId == update.Message.Chat.Id.ToString()      
            select goal;// for example 'smth selected' can be look that way  
                        //select new { Name=user.GoalName, Date = user.DueDate, chatid=goal.ChatId };
        foreach (var goal in list)
        {
            if (goal.ArchiveDate == null && DateTime.Now > goal.DueDate) //if goal is not archived, but outdated
                goal.ArchiveDate = goal.DueDate;
            
            if(goal.ArchiveDate == null)
                await bot.SendTextMessageAsync(update.Message.Chat.Id,
                    "Название занятия:\n" + goal.GoalName + "\nДата занятия:\n" + goal.DueDate);
        }
        Logger.Debug("Bot", "End ShowCommand");
        return Name;
    }

    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type != UpdateType.Message)
            return false;

        return update.Message.Text.Contains(Name);
    }
}