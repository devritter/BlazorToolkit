using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BlazingDev.BlazorToolkit.Components;

public class BzComponentBase : ComponentBase, IDisposable
{
    [Inject] private ILoggerFactory LoggerFactory { get; set; } = null!;
    private ILogger? _componentLogger;

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

    protected void InvokeAsyncStateHasChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    void IDisposable.Dispose()
    {
        OnDispose();
    }

    protected virtual void OnDispose()
    {
    }

    // todo IAsyncDisposable auch implementieren?
}