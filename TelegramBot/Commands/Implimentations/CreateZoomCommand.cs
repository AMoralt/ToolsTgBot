﻿using System.Text.Json.Nodes;
using RestSharp;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot;
using JsonSerializer = System.Text.Json.JsonSerializer;

public class CreateZoomCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["CreateZoomCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
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
        }
        
        if (update.Type == UpdateType.Message)
        {
            InlineKeyboardMarkup inline = new InlineKeyboardMarkup(
                new InlineKeyboardButton("")
                {
                    Text = "👤Ссылка для входа",
                    Url = jsonNode["join_url"].ToString()
                });

            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Конференция Zoom", replyMarkup: inline);
        }

        return Name;
    }
    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type == UpdateType.InlineQuery)
            return true;
        if (update.Type != UpdateType.Message)
            return false;
        
        return update.Message.Text.Contains(Name);
    }
}