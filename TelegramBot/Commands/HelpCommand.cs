using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

public class HelpCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["HelpCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Handling HelpCommand");
        
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "\U0001F4D6 Список доступных команд:\n");
        
        foreach (var x in Config.CommandNames)
        {
            if(x.Value != "-")
                await bot.SendTextMessageAsync(update.Message.Chat.Id,$" {x}\n");
        }
        Logger.Debug("Bot", "End HelpCommand");
        return Name;
    }
    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type != UpdateType.Message)
            return false;

        return update.Message.Text.Contains(Name);
    }
}