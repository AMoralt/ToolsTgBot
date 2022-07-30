using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot;

public class SheduleCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["SheduleCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "Введите дату конференции следующим образом:\ndd.MM.yyyy hh:mm");
        
        return Config.CommandNames["SheduleZoomCommand"];
    }
    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type != UpdateType.Message)
            return false;
        
        return update.Message.Text.Contains(Name); 
    }
}