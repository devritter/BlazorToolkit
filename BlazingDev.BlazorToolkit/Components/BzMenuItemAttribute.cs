using Microsoft.AspNetCore.Components.Routing;

namespace BlazingDev.BlazorToolkit.Components;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class BzMenuItemAttribute : Attribute
{
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public int Sorting { get; set; }

    /// <summary>
    /// Defines how route matching (to detect the currently active menu item) should work.
    /// Defaults to "Prefix".
    /// </summary>
    public NavLinkMatch MatchMode { get; set; } = NavLinkMatch.Prefix;
}