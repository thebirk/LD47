using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LD47
{
    public static class Debug
    {
        private static readonly DefaultTraceListener _tracer = new DefaultTraceListener();

        public static void Trace(string str)
        {
            _tracer.WriteLine(str);
        }
    }
}
