using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot;

public class StartCommand : TelegramCommand
{
    public override string Name => Config.CommandNames["StartCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
        await bot.SendTextMessageAsync(update.Message.Chat.Id, "🤖 Вас приветствует ToolsBot,\nЯ предназначен я работы с Zoom, а также для создания и отслеживания списка дел.");
        await new HelpCommand().Execute(update, bot);
        
        return Name;
    }

    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type != UpdateType.Message)
            return false;

        return update.Message.Text.Contains(Name);
    }
}