using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

public class HelpCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["HelpCommand"];

    public override async Task Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Handling HelpCommand");
        /*var keyBoard = new ReplyKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        new KeyboardButton("\U0001F3E0 Главная")
                    },
                    new[]
                    {
                        new KeyboardButton("\U0001F4D6 Помощь")
                    }
                }
            );*/
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "\U0001F4D6 Список доступных команд:\n");
        foreach (var x in Config.CommandNames)
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id,$" {x}\n");
        }
    }
    public override bool Contains(Update update)
    {
        if (update.Type != UpdateType.Message)
            return false;

        return update.Message.Text.Contains(Name);
    }
}