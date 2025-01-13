using BlazingDev.BlazorToolkit.Components.Integrations;
using Fluxor;

namespace BlazingDev.BlazorToolkit.FluxorIntegration;

public class FluxorIntegration(IActionSubscriber actionSubscriber) : IBzComponentIntegration
{
    private BzComponentIntegrationInitializationContext _ctx = null!;

    Task IBzComponentIntegration.InitializeAsync(BzComponentIntegrationInitializationContext ctx)
    {
        _ctx = ctx;

        var stateSubscription = StateSubscriber.Subscribe(ctx.Component, _ =>
        {
            if (!_ctx.ComponentInternals.IsDisposed)
            {
                _ctx.ComponentInternals.InvokeAsync(_ctx.ComponentInternals.StateHasChanged);
            }
        });

        // disposing
        ctx.Disposer.Add(stateSubscription);
        ctx.Disposer.Add(() => actionSubscriber.UnsubscribeFromAllActions(this));

        return Task.CompletedTask;
    }

    public void SubscribeToAction<TAction>(Action<TAction> callback)
    {
        actionSubscriber.SubscribeToAction<TAction>(this, action =>
        {
            _ctx.ComponentInternals.InvokeAsync(() =>
            {
                if (!_ctx.ComponentInternals.IsDisposed)
                {
                    callback(action);
                    _ctx.ComponentInternals.StateHasChanged();
                }
            });
        });
    }

    public void SubscribeToAction<TAction>(Func<TAction, Task> callback)
    {
        actionSubscriber.SubscribeToAction<TAction>(this, action =>
        {
            _ctx.ComponentInternals.InvokeAsync(async () =>
            {
                if (!_ctx.ComponentInternals.IsDisposed)
                {
                    await callback(action);
                    _ctx.ComponentInternals.StateHasChanged();
                }
            });
        });
    }
}