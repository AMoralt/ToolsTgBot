using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot;

public abstract class TelegramCommand
{
    public abstract string Name { get; }
    public abstract Task Execute(Update update, ITelegramBotClient bot);
    public abstract bool Contains(Update update);
}