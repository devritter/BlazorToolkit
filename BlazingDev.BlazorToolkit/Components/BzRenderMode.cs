using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazingDev.BlazorToolkit.Components;

public class BzRenderMode
{
    /// <summary>
    /// Gets an <see cref="IComponentRenderMode"/> that represents rendering interactively on the server via Blazor Server hosting
    /// without server-side prerendering.
    /// </summary>
    public static InteractiveServerRenderMode InteractiveServerNoPrerender { get; } = new(false);

    /// <summary>
    /// Gets an <see cref="IComponentRenderMode"/> that represents rendering interactively on the client via Blazor WebAssembly hosting
    /// without server-side prerendering.
    /// </summary>
    public static InteractiveWebAssemblyRenderMode InteractiveWebAssemblyNoPrerender { get; } = new(false);

    /// <summary>
    /// Gets an <see cref="IComponentRenderMode"/> that means the render mode will be determined automatically based on a policy.
    /// </summary>
    public static InteractiveAutoRenderMode InteractiveAutoNoPrerender { get; } = new(false);
}