using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;

namespace TelegramBot;

public class AddCommand : TelegramCommand
{
    private readonly GoalDataContext _dataStorage;
    public AddCommand(IServiceProvider services)
    {
        _dataStorage = services.GetRequiredService<GoalDataContext>();
    }
    public override string Name => Config.CommandNames["AddCommand"];

    public override async Task Execute(Update update, ITelegramBotClient bot)
    {
        Logger.Debug("Bot", "Handling StartCommand");

        var x = _dataStorage.Users.FromSqlRaw("SELECT * FROM Users").Where(x => x.ChatId == update.Message.Chat.Id.ToString());
        if (!x.IsNullOrEmpty())
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Уже есть");
            return;
        }
        _dataStorage.Users.Add(new User
        {
            ChatId = update.Message.Chat.Id.ToString(),
            LastMessage = update.Message.Text
        });
        _dataStorage.SaveChanges();
        Logger.Debug("Bot", "End StartCommand");
    }

    public override bool Contains(Message message)
    {
        if (message.Type != MessageType.Text)
            return false;

        return message.Text.Contains(Name);
    }
}