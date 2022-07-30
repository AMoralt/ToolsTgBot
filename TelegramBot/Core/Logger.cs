public static class Logger
{
    //task with async console output source and message
    public static async Task LogAsync(string source, string message)
    {
        await Task.Run(() =>
        {
            Console.WriteLine($"{DateTime.Now.ToString("T"),-20} {source,-0} {message}");
        });
    }
        
}