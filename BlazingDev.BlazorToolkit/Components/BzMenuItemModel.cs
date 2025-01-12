using Microsoft.AspNetCore.Components.Routing;

namespace BlazingDev.BlazorToolkit.Components;

public record BzMenuItemModel(
    string Name,
    string? Icon,
    int? Sorting,
    string? Route,
    NavLinkMatch MatchMode,
    Type Type);