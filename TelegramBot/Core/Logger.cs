namespace TelegramBot;

public static class Logger
{
    public static void Debug(string source, string message) => 
        Console.WriteLine($"{DateTime.Now.ToString("T"),-10} {source,-10} {message}");
}