using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoscopeClient
{
    public static class Debug
    {
        public static void Write(char c) { Console.Write(c); }

        public static void Write(string txt) { Console.Write(txt); }

        public static void WriteLine(string txt) { Console.WriteLine(txt); }

        public static void Info(string txt)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.Write('[');
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("  >>  ");
            Console.ForegroundColor = old;
            Console.WriteLine("] " + txt);
        }

        public static void OK(string txt)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.Write('[');
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("  OK  ");
            Console.ForegroundColor = old;
            Console.WriteLine("] " + txt);
        }

        public static void Warning(string txt)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.Write('[');
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  ??  ");
            Console.ForegroundColor = old;
            Console.WriteLine("] " + txt);
        }

        public static void Error(string txt)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.Write('[');
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("  !!  ");
            Console.ForegroundColor = old;
            Console.WriteLine("] " + txt);
            Console.Read();
            Environment.Exit(1);
        }
    }
}
