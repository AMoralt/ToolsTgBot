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

public class SheduleZoomCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["SheduleZoomCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Start SheduleZoomCommand");
        
        if (!ValidateMessage(update.Message.Text)) // if got wrong format message
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Некорректная дата. Формат создание запланированных конференций:\ndd.MM.yyyy hh:mm");
            return Name;
        }
        
        var date = Convert.ToDateTime(update.Message.Text).ToUniversalTime();
        var client = new RestClient("https://api.zoom.us/v2/users/dias_galym@bk.ru/meetings");
        var request = new RestRequest()
        {
            Method = Method.POST,
            RequestFormat = DataFormat.Json,
            Parameters =
            {
                new Parameter()
                {
                    Name = "application/json",
                    Type = ParameterType.RequestBody,
                    Value = JsonSerializer.Serialize(new
                    {
                        topic = "Zoom meeting",
                        duration = 60,
                        start_time = date,
                        type = 2,
                        settings = new
                        {
                            join_before_host = true,
                            waiting_room = false,
                            jbh_time = 10
                        }
                    })
                },
                new Parameter()
                {
                    Name = "Authorization",
                    Type = ParameterType.HttpHeader,
                    Value = String.Format("Bearer {0}", Config.JWTToken )
                }
            }
        };
        var response = client.Execute(request);
        var jsonNode = JsonNode.Parse(response.Content);
    
        InlineKeyboardMarkup inline = new InlineKeyboardMarkup(
            new InlineKeyboardButton("") 
            {
                Text = "👤Ссылка для входа",
                Url = jsonNode["join_url"].ToString()
            });
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "Конференция Zoom на " + date.AddHours(5), replyMarkup: inline );
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "Вход разрешен за 10 минут до начала конференции.");
        
        Logger.Debug("Bot", "End SheduleZoomCommand");
        return string.Empty; //TODO Swap null and Name, It turns out unintuitive
    }

    private bool ValidateMessage(string messageText)
    {
        try
        {
            return DateTime.TryParse(messageText, out var date);
        }
        catch
        {
            return false;
        }
    }
    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type != UpdateType.Message)
            return false;
        
        return !update.Message.Text.StartsWith("/") && lastmessage == Name;
    }
}