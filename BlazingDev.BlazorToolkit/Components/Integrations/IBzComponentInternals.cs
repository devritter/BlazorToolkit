using BlazingDev.BlazingExtensions.BlazingUtilities;
using Microsoft.Extensions.Logging;

namespace BlazingDev.BlazorToolkit.Components.Integrations;

/// <summary>
/// provides access to internal methods and properties of a BzComponentBase
/// </summary>
public interface IBzComponentInternals
{
    ILogger Logger { get; }
    Task InvokeAsync(Action workItem);
    Task InvokeAsync(Func<Task> workItem);
    void StateHasChanged();
    BzAsyncDisposer Disposer { get; }
    bool IsDisposed { get; }
}