using System.Text;
using Exception = System.Exception;

namespace TestApp
{
    internal class Logger : ILogger, IDisposable
    {
        private readonly Dictionary<int, DateTime> _logStash = new();
        private readonly int _cooldown;
        private readonly LoggerOutput _output;
        private readonly FileStream? _fileWriter;

        public Logger(int cooldown, LoggerOutput output)
        {
            _cooldown = cooldown;
            _output = output;
            if (output == LoggerOutput.File)
            {
                var fileDate = DateTime.Now.ToString("G").Replace(':', '-').Replace('/', '-');
                _fileWriter = File.Create($"log_{fileDate}.log");
            }
        }

        public bool TryLog(LogLevel level, string message)
        {
            return LogInternal(level, message);
        }

        private void WriteStringToFile(string data)
        {
            if ((bool)_fileWriter?.CanWrite)
            {
                var stringBytes = Encoding.UTF8.GetBytes($"{data}\n");
                _fileWriter.Write(stringBytes, 0, stringBytes.Length);
            }
            else
            {
                throw new Exception("I/O Error: file write unavailable.");
            }
        }

        private void WriteStringToConsole(string data)
        {
            Console.WriteLine(data);
        }

        private void DispatchMessage(string message)
        {
            switch (_output)
            {
                case LoggerOutput.File:
                    WriteStringToFile(message);
                    break;
                case LoggerOutput.Stdout:
                    WriteStringToConsole(message);
                    break;
            }
        }

        private bool LogInternal(LogLevel level, string message)
        {
            var eventTime = DateTime.Now;
            var messageHash = message.GetHashCode();
            var logString = $"[{eventTime:G}:{level}]\t{message}";
            if (_logStash.TryGetValue(messageHash, out var lastTime))
            {
                if ((eventTime - lastTime).Seconds >= _cooldown)
                {
                    DispatchMessage(logString);
                    _logStash.Remove(messageHash);
                    return true;
                }
                return false;
            }
            DispatchMessage(logString);
            _logStash.Add(messageHash, eventTime);
            return true;
        }

        public void Dispose()
        {
            if (_fileWriter != null)
            {
                _fileWriter.Flush();
                _fileWriter.Close();
                _fileWriter.Dispose();
            }
        }
    }
}
