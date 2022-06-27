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

public class ZoomCreateCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["ZoomCreateCommand"];

    public override async Task Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Handling ZoomCreateCommand");
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
                    Value = JsonSerializer.Serialize(new //Zoom properties
                    {
                        topic = "Zoom meeting",
                        duration = 60, //in minutes
                        start_time = DateTime.Now.AddSeconds(30),
                        type = 2, //Enum: 1 - Instant meeting. 2 - Scheduled meeting.
                        //8 - Repeat meeting with fixed time, 3 - "same" with no fixed time
                        settings = new
                        {
                            join_before_host = true,
                            waiting_room = false,
                            jbh_time = 0 //Enum: 0 - Allow join before host at anytime
                            //5 - Allow 5 minutes before start. 10 - Same, but 10
                        }
                    })
                },
                new Parameter()
                {
                    Name = "Authorization",
                    Type = ParameterType.HttpHeader,
                    Value = String.Format("Bearer {0}", Config.JWTToken )
                    //Bearer schema Authorization 
                }
            }
        };
        var response = client.Execute(request);
        var jsonNode = JsonNode.Parse(response.Content);
        
        if (update.Type == UpdateType.InlineQuery)
        {
            var textMessage = new InputTextMessageContent($"{jsonNode["join_url"]}"); //Bot's text output
            var array = new InlineQueryResultArticle[]
            {
                new ("1", "Ссылка на Zoom конференцию", textMessage) 
            };
            
            await bot.AnswerInlineQueryAsync(update.InlineQuery.Id, array);
            
            Logger.Debug("Bot", "End ZoomCreateCommand");
        }
        else
        {
            InlineKeyboardMarkup inline = new InlineKeyboardMarkup(
                new InlineKeyboardButton("") 
                {
                    Text = "👤Ссылка для входа",
                    Url = jsonNode["join_url"].ToString()
                });
        
            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Конференция Zoom", replyMarkup: inline );
            Logger.Debug("Bot", "End ZoomCreateCommand");
        }
    }
    public override bool Contains(Message message)
    {
        if (message.Type != MessageType.Text)
            return false;

        return message.Text.Contains(Name);
    }
}