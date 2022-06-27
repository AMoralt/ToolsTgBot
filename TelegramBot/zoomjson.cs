namespace TelegramBot;

public class zoomjson
{
    public string topic { get; set; }
    public int duration { get; set; }
    public string start_time { get; set; }
    public int type { get; set; }
    public Settings settings { get; set; }
}
public class Settings
{
    public bool join_before_host { get; set; }
    public bool waiting_room { get; set; }
    public int jbh_time { get; set; }
}