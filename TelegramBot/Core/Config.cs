using Microsoft.Extensions.Configuration;
using TelegramBot;

public static class Config
{
    public static string? TelegramToken { get; }
    public static string? JWTToken { get; }
    public static string? DbConnection { get; }
    
    public static Dictionary<string, string>? CommandNames;

    static Config()
    {
        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json").Build();
            
            TelegramToken = config["TelegramToken"];
            JWTToken = config["JWTToken"];
            DbConnection = config["ToDoDB"];
            CommandNames = config.GetSection("CommandNames").Get<Dictionary<string, string>>();
        }
        catch (Exception ex)
        {
            Logger.LogAsync("Exception", ex.Message);
            AppControl.Exit();
        }
    }
}