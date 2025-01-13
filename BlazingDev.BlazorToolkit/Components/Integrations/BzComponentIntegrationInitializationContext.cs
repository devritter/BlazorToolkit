using BlazingDev.BlazingExtensions.BlazingUtilities;

namespace BlazingDev.BlazorToolkit.Components.Integrations;

public class BzComponentIntegrationInitializationContext
{
    public required BzComponentBase Component { get; set; }
    public required IBzComponentInternals ComponentInternals { get; set; }
    public BzAsyncDisposer Disposer => ComponentInternals.Disposer;
}