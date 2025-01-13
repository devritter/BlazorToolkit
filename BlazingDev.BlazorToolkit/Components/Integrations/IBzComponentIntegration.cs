namespace BlazingDev.BlazorToolkit.Components.Integrations;

public interface IBzComponentIntegration
{
    Task InitializeAsync(BzComponentIntegrationInitializationContext ctx);
}