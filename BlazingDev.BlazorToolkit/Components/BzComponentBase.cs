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

    /// <summary>
    /// Collects disposables (and Actions) that will be disposed when the component gets disposed
    /// </summary>
    protected BzAsyncDisposer Disposer => _disposer ??= new BzAsyncDisposer();

    /// <summary>
    /// return true when all component initialization methods have been called once.
    /// </summary>
    protected bool IsInitialized { get; private set; }

    private bool _isFirstSetParametersAsyncCall = true;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        var isThisMethodCallResponsibleForSettingIsInitialized = _isFirstSetParametersAsyncCall;
        _isFirstSetParametersAsyncCall = false;

        await base.SetParametersAsync(parameters);

        if (isThisMethodCallResponsibleForSettingIsInitialized)
        {
            IsInitialized = true;
            StateHasChanged(); // needed because the last render was with IsInitialized=false
        }
    }

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