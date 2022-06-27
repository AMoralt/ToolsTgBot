using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

public class StartCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["StartCommand"];

    public override async Task Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Handling StartCommand");
        
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "🤖 Вас приветствует ToolsBot,\nЯ предназначен я работы с Zoom, а также для создания и отслеживания списка дел.");

        await new HelpCommand().Execute(update, bot);
    }

    public override bool Contains(Message message)
    {
        if (message.Type != MessageType.Text)
            return false;

        return message.Text.Contains(Name);
    }
}