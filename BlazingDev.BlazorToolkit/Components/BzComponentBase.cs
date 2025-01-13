using BlazingDev.BlazingExtensions.BlazingUtilities;
using BlazingDev.BlazorToolkit.Components.Integrations;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BlazingDev.BlazorToolkit.Components;

public class BzComponentBase : ComponentBase, IAsyncDisposable, IBzComponentInternals
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

    protected bool IsDisposed { get; set; }

    /// <summary>
    /// return true when all component initialization methods have been called once.
    /// </summary>
    protected bool IsInitialized { get; private set; }

    private bool _isFirstSetParametersAsyncCall = true;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        var isThisMethodCallResponsibleForSettingIsInitialized = false;
        if (_isFirstSetParametersAsyncCall)
        {
            _isFirstSetParametersAsyncCall = false;
            isThisMethodCallResponsibleForSettingIsInitialized = true;
            await BzComponentIntegrationHelper.InitializeIntegrations(this);
        }

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
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;

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

    #region IComponentInternals

    Task IBzComponentInternals.InvokeAsync(Action workItem)
    {
        return InvokeAsync(workItem);
    }

    Task IBzComponentInternals.InvokeAsync(Func<Task> workItem)
    {
        return InvokeAsync(workItem);
    }

    void IBzComponentInternals.StateHasChanged()
    {
        StateHasChanged();
    }

    ILogger IBzComponentInternals.Logger => Logger;
    bool IBzComponentInternals.IsDisposed => IsDisposed;
    BzAsyncDisposer IBzComponentInternals.Disposer => Disposer;

    #endregion IComponentInternals
}