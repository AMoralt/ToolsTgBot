using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Polling;

namespace TelegramBot;

public static class Config
{
    public static string? TelegramToken { get; }
    public static string? APIkey { get; }
    public static string? APISecret { get; }
    public static string? JWTToken { get; }
    
    public static Dictionary<string, string>? CommandNames;

    static Config()
    {
        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json").Build();
            
            TelegramToken = config["TelegramToken"];
            APIkey = config["APIkey"];
            JWTToken = config["JWTToken"];
            APISecret = config["APISecret"];
            CommandNames = config.GetSection("CommandNames").Get<Dictionary<string, string>>();
        }
        catch (Exception ex)
        {
            Logger.Debug("Exception", ex.Message);
            AppControl.Exit();
        }
    }
}