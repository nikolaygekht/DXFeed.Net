using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DXFeed.Net.Test
{
    internal static class Tools
    {
        public static void Wait(Func<bool> predicate, int timeout)
        {
            var limit = DateTime.Now.AddMilliseconds(timeout);
            while (DateTime.Now < limit)
            {
                if (predicate())
                    return;

                Thread.Sleep(10);
            }
        }
    }
}
