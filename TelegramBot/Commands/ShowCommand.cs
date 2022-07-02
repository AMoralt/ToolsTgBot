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

    public override async Task Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Handling ShowCommand");

        var _database = _services.GetRequiredService<GoalDataContext>();
        var inDataBase = _database.Users
            .Single(x => x.ChatId == update.Message.Chat.Id.ToString());
        
        var list = _database.Goals.Where(x => x.UserId == inDataBase.Id);
        
        foreach (var goal in list)
        {
            if (goal.ArchiveDate == null)
            {
                if(DateTime.Now.Date > goal.DueDate.Date) 
                    goal.ArchiveDate = goal.DueDate;
                
                await bot.SendTextMessageAsync(update.Message.Chat.Id,
                    "Название занятия:\n" + goal.GoalName + "\nДата занятия:\n" + goal.DueDate);
            }
        }

        //_database.SaveChanges();
        Logger.Debug("Bot", "End ShowCommand");
    }

    public override bool Contains(Update update)
    {
        if (update.Type != UpdateType.Message)
            return false;

        return update.Message.Text.Contains(Name);
    }
}