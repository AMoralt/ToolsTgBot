using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Data;

public class GoalDataContext : DbContext
{
    public GoalDataContext()
    {            
    }
    public GoalDataContext(DbContextOptions<GoalDataContext> options) :
        base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Server=localhost;Database=tododb;Port=5432;Username=postgres;Password=AMoralt");
        optionsBuilder.UseLoggerFactory(MyLoggerFactory);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new GoalConfiguration());
    }
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
        
            builder.HasKey(u => u.Id).HasName("UsersPrimaryKey");;
        
            builder.HasIndex(u => u.ChatId).IsUnique();
        
            builder.Property(u => u.LastCommand).HasDefaultValue(null);
        
            builder.Property(u => u.ChatId).IsRequired();
        
            builder.Property(u => u.LastCommand).HasMaxLength(300);
        }
    }
    public class GoalConfiguration : IEntityTypeConfiguration<Goal>
    {
        public void Configure(EntityTypeBuilder<Goal> builder)
        {
            builder.ToTable("task");
        
            builder.HasKey(g => g.Id).HasName("TasksPrimaryKey");;
        
            builder.Property(g => g.ArchiveDate).HasDefaultValue(null);
        
            builder.Property(g => g.GoalName).HasMaxLength(60);
        }
    }
    
    public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name
                    && level == LogLevel.Error)
               .AddConsole();
    });
    
    public DbSet<Goal> Goals { get; set; }
    public DbSet<User> Users { get; set; }
}