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
    public override string Name => Config.CommandNames["AddCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Handling AddCommand");

        await bot.SendTextMessageAsync(update.Message.Chat.Id, "Введите название вашего занятия следующим образом:\nНазвание занятия - дата");
        
        Logger.Debug("Bot", "End AddCommand");
        return Config.CommandNames["AddGoalCommand"];
    }
    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type != UpdateType.Message)
            return false;

        return update.Message.Text.Contains(Name);
    }
}