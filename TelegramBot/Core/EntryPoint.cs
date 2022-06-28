using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using TelegramBot.Data;
using Task = System.Threading.Tasks.Task;

namespace TelegramBot.Core;

public class EntryPoint
{
    //private CommandService _commandService;
    public async Task EntryAsync()
    {
        Logger.Debug("Bot", "Initializing");
        using var services = ConfigureServices();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new UpdateType[]
            {
                UpdateType.Message,
                UpdateType.EditedMessage,
                UpdateType.InlineQuery,
                UpdateType.CallbackQuery
            }
        };
        
        try
        {
            Logger.Debug("Bot", "StartReceiving");
            var client = services.GetRequiredService<TelegramBotClient>();
            var handler = services.GetRequiredService<HandlingService>();
            var db = services.GetRequiredService<GoalDataContext>();
            db.Users.RemoveRange(db.Users);
                
                
            client.StartReceiving(handler.UpdateHandler, handler.ErrorHandler, receiverOptions);
        }
        catch (Exception ex)
        {
            Logger.Debug("Exception", ex.Message);
            AppControl.Exit();
        }
        
        await Task.Delay(Timeout.Infinite);
    }
    private static ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton(x => { return new TelegramBotClient(Config.TelegramToken); })
            .AddDbContext<GoalDataContext>(options => options.UseNpgsql(Config.DbConnection))
            .AddSingleton<HandlingService>()
            .BuildServiceProvider();
    }
}