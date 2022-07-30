using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot;
using TelegramBot.Data;

public class ShowCommand : TelegramCommand
{
     private readonly IServiceProvider _services;
     public ShowCommand(IServiceProvider services)
     {
         _services = services;
     }
    public override string Name => Config.CommandNames["ShowCommand"];

    public override async Task<string> Execute(Update update, ITelegramBotClient bot)
    {
        var database = _services.GetRequiredService<GoalDataContext>();
        
        var listOfGoal = await CreateListOfGoal(update.Message.Chat.Id, database);
        
        foreach (var goal in listOfGoal)
        {
            if(IsOutdated(goal))
                goal.ArchiveDate = goal.DueDate;
            
            if(goal.ArchiveDate == null)
                await bot.SendTextMessageAsync(update.Message.Chat.Id,
                    "Название занятия:\n" + goal.Name + "\nДата занятия:\n" + goal.DueDate);
        }
        
        return Name;
    }
    private bool IsOutdated(Goal goal)
    {
        return goal.ArchiveDate == null && DateTime.Now > goal.DueDate; //if goal is not archived, but outdated
    }
    private async Task<IQueryable<Goal>> CreateListOfGoal(long ChatId, GoalDataContext database)
    {
        var list = from user in database.Users
            join goal in database.Goals on user.Id equals goal.UserId
            where user.ChatId == ChatId.ToString()
            select goal // for example 'smth selected' can be look that way  
        ;               //select new { Name=user.GoalName, Date = user.DueDate, chatid=goal.ChatId };
        
        return list;
    }
    public override bool Contains(Update update, string lastmessage)
    {
        if (update.Type != UpdateType.Message)
            return false;

        return update.Message.Text.Contains(Name);
    }
}