using System;
using System.Threading.Tasks;

namespace AsyncEvent
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new Tester();
            t.RunCompleted += RunCompleet;
            t.Run();

            Console.WriteLine("Press <Enter> to exit...");
            Console.ReadLine();
        }

        private static void RunCompleet(object sender, RunCompletedEventArgs e)
        {

        }
    }

    public class Tester
    {
        public event RunCompletedEventHandler RunCompleted; 

        public void Run()
        {

        }

        public delegate void RunCompletedEventHandler(object sender, RunCompletedEventArgs e);  
    }

    public class RunCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs  
    {  
        public RunCompletedEventArgs(Exception error, bool b, object o)
            : base(error, b,o)
        {

        }
        
        public int Result { get; }  
    }  
}