using System.Text.Json.Nodes;
using RestSharp;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot;
using JsonSerializer = System.Text.Json.JsonSerializer;


public class SheduleZoomCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["SheduleZoomCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
        if (!ValidateMessage(update.Message.Text)) // if got wrong format message
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Некорректная дата. Формат создание запланированных конференций:\ndd.MM.yyyy hh:mm");
            return Name;
        }
        
        var date = DateTime.Parse(update.Message.Text).ToUniversalTime();
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
        
        return string.Empty; //TODO Swap null and Name, It turns out unintuitive
    }

    private bool ValidateMessage(string messageText)
    {
        var messageTextSplit = messageText.Split('-');
        if (messageTextSplit.Length != 2)
        {
            return false;
        }
        return DateTime.TryParse(messageTextSplit[1], out _);
    }
    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type != UpdateType.Message)
            return false;
        
        return !update.Message.Text.StartsWith("/") && lastmessage == Name;
    }
}