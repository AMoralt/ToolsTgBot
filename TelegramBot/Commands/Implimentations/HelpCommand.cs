using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot;

public class HelpCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["HelpCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "\U0001F4D6 Список доступных команд:\n");
        
        foreach (var x in Config.CommandNames)
        {
            if(!x.Value.StartsWith("!"))
                await bot.SendTextMessageAsync(update.Message.Chat.Id,$" {x.Value} - {x.Key}\n");
        }

        return Name;
    }
    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type != UpdateType.Message)
            return false;

        return update.Message.Text.Contains(Name);
    }
}