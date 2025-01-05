using System.Timers;
using BlazingDev.BlazorToolkit.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace BlazingDev.BlazorToolkit.Timers;

public class BzTimerComponent : BzComponentBase
{
    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] [EditorRequired] public double Interval { get; set; }
    [Parameter] public string? Name { get; set; }

    [Parameter] public Action? OnElapsedAction { get; set; }
    [Parameter] public EventCallback OnElapsed { get; set; }

    private readonly Timer _timer = new();

    protected override void OnParametersSet()
    {
        // need to check for real changes, otherwise the time passed is always reset 
        if (Math.Abs(_timer.Interval - Interval) > 0.1)
        {
            _timer.Interval = Interval;
        }

        _timer.Enabled = Enabled;

        base.OnParametersSet();
    }

    protected override void OnInitialized()
    {
        _timer.Elapsed += HandleElapsed;
        base.OnInitialized();
    }

    private async void HandleElapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            OnElapsedAction?.Invoke();
            if (OnElapsed.HasDelegate)
            {
                await InvokeAsync(OnElapsed.InvokeAsync);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Elapsed handler execution failed for timer '{Name}'", Name ?? "no-name-specified");
        }

        // todo stop while executing?
    }

    protected override void OnDispose()
    {
        _timer.Dispose();
        base.OnDispose();
    }
}