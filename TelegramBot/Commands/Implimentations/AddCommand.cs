using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot;

public class AddCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["AddCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "Введите название вашего занятия следующим образом:\nНазвание занятия - дата");
        
        return Config.CommandNames["AddGoalCommand"];
    }
    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type != UpdateType.Message)
            return false;
        
        return update.Message.Text.Contains(Name);
    }
}