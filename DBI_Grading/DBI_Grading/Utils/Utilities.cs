using System;
using System.Threading;

namespace DBI_Grading.Utils
{
    class Utilities
    {
        public static T WithTimeout<T>(Func<T> proc, int duration)
        {
            var reset = new AutoResetEvent(false);
            var r = default(T);
            Exception ex = null;

            var t = new Thread(() =>
            {
                try
                {
                    r = proc();
                }
                catch (Exception e)
                {
                    ex = e;
                }
                reset.Set();
            });

            t.Start();

            // not sure if this is really needed in general
            while (t.ThreadState != ThreadState.Running)
            {
                Thread.Sleep(0);
            }

            if (!reset.WaitOne(duration*1000))
            {
                t.Abort();
                throw new TimeoutException();
            }

            if (ex != null)
            {
                throw ex;
            }
            return r;
        }
    }
}
