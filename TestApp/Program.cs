using TestApp;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello, logger! Feel free to fire messages, typing whatever you want and pressing Enter to send. Send empty string to exit.");
        string message;
        using var logger = new Logger(5, LoggerOutput.File);
        do
        {
            message = Console.ReadLine();
            if (!string.IsNullOrEmpty(message))
            {
                if (!logger.TryLog(LogLevel.Info, message))
                {
                    Console.WriteLine("Threshold was not reached, message was not logged");
                }
            }
        } while (!string.IsNullOrEmpty(message));
        Console.WriteLine("Bye");
    }
}


