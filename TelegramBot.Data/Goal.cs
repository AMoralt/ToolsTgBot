using System.ComponentModel.DataAnnotations;

namespace TelegramBot.Data;

public class Goal
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }  
    public string GoalName { get; set; }
    public string GoalDescription { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ArchiveDate { get; set; }
    public bool IsArchive
    {
        get => !ArchiveDate.HasValue;
    }
}