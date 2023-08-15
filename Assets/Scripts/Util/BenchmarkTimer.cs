

using System;
using System.Diagnostics;

class BenchmarkTimer
{
    private Stopwatch m_Stopwatch = new Stopwatch();

    private bool m_Stopped = false;

    private string m_EndMsg;

    public BenchmarkTimer(string msg = "in {}.\n")
    {
        m_EndMsg = msg;

        m_Stopwatch.Start();
    }

    ~BenchmarkTimer()
    {
        if (!m_Stopped)
        {
            Stop();
        }
    }

    public TimeSpan Stop()
    {
        m_Stopped = true;

        m_Stopwatch.Stop();

        TimeSpan elapsed = m_Stopwatch.Elapsed;

        if (m_EndMsg != null)
        {
            Console.WriteLine(m_EndMsg, elapsed);
        }

        return elapsed;
    }
}