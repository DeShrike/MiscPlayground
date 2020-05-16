using System;
using System.Threading.Tasks;

namespace Testing
{
    class Program
    {
        /* 
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Program prog = new Program();
            prog.Execute();

        }

        public void Execute()
        {
            
            this.MethodNameCompleted += DoTheEvent;


            var args = new MethodNameCompletedEventHandler();
            this.MethodNameCompleted    (this, args )
            
        }

        public async Task DoTheEvent(object sender, MethodNameCompletedEventHandler e)
        {
            Console.WriteLine("Before {0}" , System.Threading.Thread.CurrentThread.ManagedThreadId); // on which thread does this run?

            await DoWorkAsync().ConfigureAwait(continueOnCapturedContext: false);

            Console.WriteLine("After {0}" , System.Threading.Thread.CurrentThread.ManagedThreadId); // on which thread does this run?
        }

        public async Task DoWorkAsync()
        {
            await System.Threading.Tasks.Task.Delay(1000);
        }

        public delegate void MethodNameCompletedEventHandler(object sender, MethodNameCompletedEventArgs e);  

        public event MethodNameCompletedEventHandler MethodNameCompleted;  

        public class MethodNameCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs  
        {  
            public int Result { get; }
            public string Arg2 { get; }  
            public string Arg3 { get; }  
        }  

        public class MyReturnType
        {
            public bool Success { get; set; }
        }
        */
    }
}