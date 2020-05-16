namespace Fire
{
    using Microsoft.GotDotNet;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            ConsoleEx.CursorVisible = false;
            var f = new Fire();

            while (true)
            {
                f.AddFuel();
                f.Burn();
                f.Draw();
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey();
                    if (key.Key== ConsoleKey.Escape)
                    {
                        break;
                    }
                }
            }
            
            ConsoleEx.CursorVisible = true;
            ConsoleEx.CursorX = 0;
            Console.WriteLine();
        }
    }
}
