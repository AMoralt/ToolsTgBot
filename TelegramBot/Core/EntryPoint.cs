using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TelegramBot;
using TelegramBot.Data;
using Task = System.Threading.Tasks.Task;


public class EntryPoint
{
    public async Task EntryAsync()
    {
        await Logger.LogAsync("Bot", "Initializing");
        using var services = ConfigureServices();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new []
            {
                UpdateType.Message,
                UpdateType.InlineQuery
            }
        };
        
        try
        {
            await Logger.LogAsync("Bot", "StartReceiving");
            var client = services.GetRequiredService<TelegramBotClient>();
            var handler = services.GetRequiredService<HandlingService>();

            client.StartReceiving(handler.UpdateHandler, handler.ErrorHandler, receiverOptions);
        }
        catch (Exception ex)
        {
            await Logger.LogAsync("Exception", ex.Message);
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