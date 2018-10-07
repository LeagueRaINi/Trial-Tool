using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AdobeReset.Helpers
{
    internal static class Logger
    {
        public static void Info(string message, params object[] args)
        {
            Log(message, ConsoleColor.Gray, args: args);
        }

        public static void Success(string message, params object[] args)
        {
            Log(message, ConsoleColor.Green, args: args);
        }

        [Conditional("DEBUG")]
        public static void Debug(string message, params object[] args)
        {
            Log(message, ConsoleColor.Cyan, args: args);
        }

        public static void Error(string message, params object[] args)
        {
            Log(message, ConsoleColor.Red, args: args);
        }

        public static void Warning(string message, params object[] args)
        {
            Log(message, ConsoleColor.Magenta, args: args);
        }

        public static void Exception(Exception exception)
        {
            Log("Oops! a wild exception appeared!\n\n" +
                "Type: {0}\n" +
                "Message: {1}\n\n" +
                "Stacktrace:\n{2}",
                ConsoleColor.DarkRed,
                args: new object[] { exception.GetType().FullName, exception.Message, exception.StackTrace });
        }

        private static void Log(string message, ConsoleColor color, [CallerMemberName] string caller = "", params object[] args)
        {
            lock("logLock")
            {
                Console.ForegroundColor = color;
                Console.WriteLine(@"[{0:H:mm:ss}]{1}| {2}", DateTime.Now, caller.PadLeft(10, ' '), string.Format(message, args));
                Console.ResetColor();
            }
        }
    }
}
