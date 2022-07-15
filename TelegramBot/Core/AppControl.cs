namespace TelegramBot;

public class AppControl
{
    public static void Exit()
    {
        Task.Delay(1000).Wait();
        Logger.Debug("Bot", "This application will be closed automatically in 5sec.");
        Task.Delay(5000).Wait();
        Environment.Exit(0);
    }
}