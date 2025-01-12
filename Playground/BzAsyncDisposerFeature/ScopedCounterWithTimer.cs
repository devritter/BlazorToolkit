using System.Timers;
using Timer = System.Timers.Timer;

namespace Playground.BzAsyncDisposerFeature;

public class ScopedCounterWithTimer : IDisposable
{
    public ScopedCounterWithTimer()
    {
        _timer.Interval = 1000;
        _timer.Start();
        _timer.Elapsed += TimerOnElapsed;
    }

    private Timer _timer = new();
    public int Counter { get; private set; }
    public int AttachedEventHandlers => CounterChanged?.GetInvocationList().Length ?? 0;

    public event EventHandler? CounterChanged;

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        Counter++;
        CounterChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        _timer.Stop();
    }
}