using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TelegramBot;

public class ZoomSheduleCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["ZoomSheduleCommand"];

    public override async Task Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Start ZoomSheduleCommand");
        DateTime date;
        var errorMessage = "Некорректная дата. Формат создание запланированных конференций:\n/shedule dd.MM.yyyy hh:mm";
        var text = update.Message.Text.Split(" ").Skip(1);
        
        if (text.Count() == 0)
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id, errorMessage);
            return;
        }

        try
        {
            date = Convert.ToDateTime(text.Aggregate((x, y) => x + "T" + y +"Z"))
                                                        .Add(new TimeSpan(-5,0,0));
        }
        catch (Exception e)
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id, errorMessage);
            return;
        }
        
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
    
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "Конференция Zoom на " + Convert.ToDateTime(date), replyMarkup: inline );
        Logger.Debug("Bot", "End ZoomSheduleCommand");
    }
    public override bool Contains(Message message)
    {
        if (message.Type != MessageType.Text)
            return false;

        return message.Text.Contains(Name);
    }
}