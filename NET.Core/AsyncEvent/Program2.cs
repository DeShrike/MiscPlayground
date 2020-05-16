using System;
using System.Threading.Tasks;

namespace AsyncEvent
{
/*     class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            MyClass myClass = new MyClass();
            myClass.DoingSomething += MyClass_DoingSomething;

        }

        async static void MyClass_DoingSomething(object sender, DeferrableEventArgs e)
        {
            using (var deferral = e.GetDeferral())
            {
                await Task.Delay(200); // run an async operation
            }            
        }
    }

    class MyClass
    {
        public event EventHandler<DeferrableEventArgs> DoingSomething;
 
        public async Task DoSomethingAsync()
        {
            if (DoingSomething != null)
            {
                var args = new DeferrableEventArgs();
                DoingSomething(this, args);
                await args.DeferAsync(); // completes once all deferrals are completed.
            }
        }
    }

    /* public sealed class DeferrableEventArgs
    {
        readonly List<TaskCompletionSource<object>> taskCompletionSources;
        
        public DeferrableEventArgs()
        {
            taskCompletionSources = new List<TaskCompletionSource<object>>();
        }
        
        public Deferral GetDeferral()
        {
            var tcs = new TaskCompletionSource<object>();
            var deferral = new Deferral(() => tcs.SetResult(null));
            taskCompletionSources.Add(tcs);
            return deferral;
        }
        
        public IAsyncAction DeferAsync()
        {
            return Task.WhenAll(taskCompletionSources.Select(tcs => tcs.Task)).AsAsyncAction();
        }
    }    */
}
