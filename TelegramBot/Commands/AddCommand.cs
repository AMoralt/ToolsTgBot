using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;

namespace TelegramBot;

public class AddCommand : TelegramCommand
{
    private readonly IServiceProvider _services;
    public AddCommand(IServiceProvider services)
    {
        _services = services;
    }
    public override string Name => Config.CommandNames["AddCommand"];

    public override async Task Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Handling AddCommand");
        var _database = _services.GetRequiredService<GoalDataContext>();
        
        var user = _database.Users
            .Single(x => x.ChatId == update.Message.Chat.Id.ToString());
        
        if (update.Message.Text.Contains(Name)) // if got "/show" message
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Введите название вашего занятия следующим образом:\n Название занятия - дата");
            user.LastMessage = Name; //remember that we wrote "/show"
            _database.SaveChanges();
        }
        else 
        {
            try
            {
                _database.Goals.Add(new Goal()
                {
                    User = user,
                    UserId = user.Id,
                    GoalName = update.Message.Text.Split("-")[0],
                    DueDate = Convert.ToDateTime(update.Message.Text.Split("-")[1]).ToUniversalTime().AddHours(5),
                    ArchiveDate = null
                });
                await bot.SendTextMessageAsync(update.Message.Chat.Id, "🤖 Занятие создано.");
                user.LastMessage = update.Message.Text;
                _database.SaveChanges();
            }
            catch(Exception ex)
            {
                await bot.SendTextMessageAsync(update.Message.Chat.Id, "Неправильный формат, введите следующим образом:\nНазвание занятия - дата");
            }
        }
        Logger.Debug("Bot", "End AddCommand");
    }

    public override bool Contains(Update update)
    {
        if (update.Type != UpdateType.Message)
            return false;
        
        var user = _services.GetRequiredService<GoalDataContext>().Users
            .Single(x => x.ChatId == update.Message.Chat.Id.ToString());

        return update.Message.Text.Contains(Name) ? true :  !update.Message.Text.Contains("/") && user.LastMessage == Name;
    }
}