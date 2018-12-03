using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIR
{
    class Log
    {
        //TODO: Comment this
        public static Exception UnknownTypeException { get; private set; }

        public enum Logs
        {
            INFO,
            WARN,
            ERROR
        }
        public static Task Logger(Logs type, String text)
        {
            switch (type)
            {
                case Logs.INFO:
                    Console.WriteLine("[INFO] " + text);
                    return Task.CompletedTask;
                case Logs.WARN:
                    Console.WriteLine("[WARN] " + text);
                    return Task.CompletedTask;
                case Logs.ERROR:
                    Console.WriteLine("[ERROR] " + text);
                    return Task.CompletedTask;
                default:
                    throw UnknownTypeException;
            }
        }
    }
}
