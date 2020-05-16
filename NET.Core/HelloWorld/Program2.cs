using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Threading;
// using System.Runtime.Remoting.Messaging;

namespace Rextester
{
    /* 
    public class Program
    {
        public static void Main(string[] args)
        {
            //It's a pain to create an async method using the old mechanism.
         
            bool exited = false;
            var worker = new AsyncWorker();
            worker.MyTaskCompleted += (a, b) => 
            {
                Console.WriteLine("Task completed. Completed event is on a thread: {0}", Thread.CurrentThread.ManagedThreadId);
                exited = true;
            };            
            worker.MyTaskAsync(123); //non-blocking!
            
            Console.WriteLine("Main thread is: {0}", Thread.CurrentThread.ManagedThreadId);
            while(!exited)
            {
                Thread.Sleep(100);
            }
            
            //As you can see the async code is run on a different thread than main (not suprisingly). 
            //And when it's done yet another thread is used from ThreadPool to run the completed event handler.
            //The good news is that nothing blocks here and threads are only used when there is some code to execute on them.
            //When there is no such code (for example when the system is waiting for an interrupt from the file system indicating the end of write)
            //threads are available at the ThreadPool.
        }
    }
    
    public class AsyncWorker
    {      
        private void MyTask(long number)
        {
            //this code will be executed asynchronously
            Console.WriteLine("I'm an async code and I'm on a thread: {0}", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(3000);
        }        

        private delegate void MyTaskDelegate(long number);

        private bool _myTaskIsRunning = false;

        public bool IsBusy
        {
            get { return _myTaskIsRunning; }
        }

        private readonly object _sync = new object();
        
        public event AsyncCompletedEventHandler MyTaskCompleted;        

        public void MyTaskAsync(long number)
        {
            MyTaskDelegate worker = new MyTaskDelegate(MyTask);
            AsyncCallback completedCallback = new AsyncCallback(MyTaskCompletedCallback);
            
            lock (_sync)
            {
                if (_myTaskIsRunning)
                    throw new InvalidOperationException("This instance is already busy.");
                
                AsyncOperation async = AsyncOperationManager.CreateOperation(null);
                worker.BeginInvoke(number, completedCallback, async);
                _myTaskIsRunning = true;
            }
        }        

        private void MyTaskCompletedCallback(IAsyncResult ar)
        {
            // get the original worker delegate and the AsyncOperation instance
            MyTaskDelegate worker = (MyTaskDelegate)((AsyncResult)ar).AsyncDelegate;
            AsyncOperation async = (AsyncOperation)ar.AsyncState;
            
            // finish the asynchronous operation
            worker.EndInvoke(ar);
            
            // clear the running task flag
            lock (_sync)
            {
                _myTaskIsRunning = false;
            }
            
            // raise the completed event
            AsyncCompletedEventArgs completedArgs = new AsyncCompletedEventArgs(null, false, null);
            async.PostOperationCompleted(f => OnMyTaskCompleted((AsyncCompletedEventArgs)f), completedArgs);
        }

        protected virtual void OnMyTaskCompleted(AsyncCompletedEventArgs e)
        {
            if (MyTaskCompleted != null)
                MyTaskCompleted(this, e);
        }
    }
    */
}