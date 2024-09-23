using System.Threading;

namespace Semaphore_WPF;

public class CustomThread
{

    public string ThreadName { get; set; }
    public Thread Thread { get; set; }

    public CustomThread(string threadName)
    {
        ThreadName = threadName;
    }

    public CustomThread(string threadName, Thread thread)
    {
        ThreadName = threadName;
        Thread = thread;
    }

    public override bool Equals(object? obj)
    {
        if (obj is CustomThread other)
        {
            return this.ThreadName.Equals(other.ThreadName);
        }

        return false;
    }

    public override string ToString() => ThreadName;

}
