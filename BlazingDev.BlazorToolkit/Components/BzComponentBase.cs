using BlazingDev.BlazingExtensions.BlazingUtilities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BlazingDev.BlazorToolkit.Components;

public class BzComponentBase : ComponentBase, IAsyncDisposable
{
    [Inject] private ILoggerFactory LoggerFactory { get; set; } = null!;
    private ILogger? _componentLogger;
    private BzAsyncDisposer? _disposer;

    protected ILogger Logger
    {
        get
        {
            if (_componentLogger != null)
            {
                return _componentLogger;
            }

            _componentLogger = LoggerFactory.CreateLogger(GetType().FullName!);
            return _componentLogger;
        }
    }

    protected BzAsyncDisposer Disposer => _disposer ??= new();

    protected void InvokeAsyncStateHasChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_disposer != null)
        {
            await _disposer.DisposeAsync().ConfigureAwait(false);
        }
        
        // ReSharper disable once MethodHasAsyncOverload
        OnDispose();
        await OnDisposeAsync().ConfigureAwait(false);
    }

    protected virtual void OnDispose()
    {
    }

    protected virtual ValueTask OnDisposeAsync()
    {
        return default;
    }
}