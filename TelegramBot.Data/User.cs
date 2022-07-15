using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TelegramBot.Data;

namespace TelegramBot;

public class User
{
    public int Id { get; set; }
    public string ChatId { get; set; }
    public string? LastCommand { get; set; }
}