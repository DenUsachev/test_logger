namespace TestApp
{
    public enum LoggerOutput
    {
        File, Stdout
    }

    public enum LogLevel
    {
        Info,
        Debug,
        Warn,
        Error,
        Trace
    }

    internal interface ILogger
    {
        bool TryLog(LogLevel level, string message);
    }


}
