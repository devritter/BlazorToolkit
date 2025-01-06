using System.Timers;
using BlazingDev.BlazorToolkit.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace BlazingDev.BlazorToolkit.Timers;

/// <summary>
/// Wraps a native Timer inside so you don't have to care about creation, event registration, and disposing!
/// </summary>
public partial class BzTimerComponent : BzComponentBase
{
    /// <summary>
    /// To enable or disable the timer. Default value: true (= enabled)
    /// </summary>
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// The timer interval in milliseconds
    /// </summary>
    [Parameter]
    [EditorRequired]
    public int Interval { get; set; }

    /// <summary>
    /// Name of the timer. For your personal documentation as well as future features ;)
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// EventCallback to handle tick events. Is invoked on the UI thread, will rerender your component automatically.
    /// </summary>
    [Parameter]
    public EventCallback OnElapsed { get; set; }

    /// <summary>
    /// Basic tick "callback" without UI thread dispatching and without automatic component rerendering.
    /// Useful if you want to decide if rerendering is really needed. 
    /// </summary>
    [Parameter]
    public Action? OnElapsedAction { get; set; }

    [Parameter] public bool ShowControls { get; set; }

    private readonly Timer _timer = new();
    private bool? _enableOverride;
    private int? _intervalOverride;

    protected override void OnInitialized()
    {
        _timer.Elapsed += HandleElapsed;
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        // need to check for real changes, otherwise the time passed is always reset
        var intervalToUse = _intervalOverride ?? Interval;
        if (Math.Abs(_timer.Interval - intervalToUse) > 0.1)
        {
            _timer.Interval = intervalToUse;
        }

        _timer.Enabled = _enableOverride ?? Enabled;

        base.OnParametersSet();
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