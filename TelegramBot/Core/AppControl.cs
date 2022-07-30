using TelegramBot;

public class AppControl
{
    public static void Exit()
    {
        Task.Delay(1000).Wait();
        Logger.LogAsync("Bot", "This application will be closed automatically in 15sec.");
        Task.Delay(15000).Wait();
        Environment.Exit(0);
    }
}