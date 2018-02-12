using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.Communication.Extensions
{
    public static class TaskExtensions
    {

        public static async Task TimeoutAfter(this Task task, int millisecondsTimeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout)))
                await task;
            else
                throw new TimeoutException();
        }

        public static Task Then(this Task parent, Task next)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            parent.ContinueWith(pt =>
            {
                if (pt.IsFaulted)
                {
                    tcs.SetException(pt.Exception.InnerException);
                }
                else
                {
                    next.ContinueWith(nt =>
                    {
                        if (nt.IsFaulted)
                        {
                            tcs.SetException(nt.Exception.InnerException);
                        }
                        else { tcs.SetResult(null); }
                    });
                    next.Start();
                }
            });
            return tcs.Task;
        }
       
    }
}
