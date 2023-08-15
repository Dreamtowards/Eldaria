

using System;
using System.Diagnostics;

class BenchmarkTimer : IDisposable
{
    private Stopwatch m_Stopwatch = new Stopwatch();

    private bool m_Stopped = false;

    private string m_EndMsg;

    public BenchmarkTimer(string msg = "in {0}ms.\n")
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

    public void Dispose()
    {
        Stop();
    }

    public TimeSpan Stop()
    {
        m_Stopped = true;

        m_Stopwatch.Stop();

        TimeSpan elapsed = m_Stopwatch.Elapsed;

        if (m_EndMsg != null)
        {
            Log.info(string.Format(m_EndMsg, elapsed.TotalMilliseconds));
        }

        return elapsed;
    }
}