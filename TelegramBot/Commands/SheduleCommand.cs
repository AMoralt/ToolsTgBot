using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TelegramBot;

public class SheduleCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["SheduleCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Start SheduleZoomCommand");
        
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "Введите дату конференции следующим образом:\ndd.MM.yyyy hh:mm");
        
        Logger.Debug("Bot", "End SheduleZoomCommand");
        return Config.CommandNames["SheduleZoomCommand"];
    }
    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type != UpdateType.Message)
            return false;
        
        return !update.Message.Text.StartsWith("/") && lastmessage == Name; 
    }
}