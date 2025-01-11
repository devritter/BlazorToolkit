using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace BlazingDev.BlazorToolkit.Components;

public static class BzComponentTool
{
    /// <summary>
    /// Returns the first defined route (@page "/this-is-the-route") of a component.
    /// </summary>
    /// <throws>InvalidOperationException when no route is defined.</throws>
    public static string GetRoute<T>() where T : IComponent
    {
        return GetRoute(typeof(T));
    }

    /// <summary>
    /// Returns the first defined route (@page "/this-is-the-route") of a component.
    /// </summary>
    /// <throws>InvalidOperationException when no route is defined.</throws>
    public static string GetRoute(Type type)
    {
        var route = TryGetRoutes(type).FirstOrDefault();
        if (route == null)
        {
            throw new InvalidOperationException($"Component '{type.Name}' has no route defined");
        }

        return route;
    }

    /// <summary>
    /// Returns the configured routes of a component. Returns an empty list for non-routable components.
    /// </summary>
    public static IEnumerable<string> TryGetRoutes<T>() where T : IComponent
    {
        return TryGetRoutes(typeof(T));
    }

    /// <summary>
    /// Returns the configured routes of a component. Returns an empty list for non-routable components.
    /// </summary>
    public static IEnumerable<string> TryGetRoutes(Type type)
    {
        return type.GetCustomAttributes<RouteAttribute>().Select(x => x.Template);
    }

    /// <summary>
    /// Tries to return the first defined route (@page "/this-is-the-route") of a component. Returns null if no route is defined.
    /// </summary>
    public static string? TryGetRoute<T>() where T : IComponent
    {
        return TryGetRoute(typeof(T));
    }

    /// <summary>
    /// Tries to return the first defined route (@page "/this-is-the-route") of a component. Returns null if no route is defined.
    /// </summary>
    public static string? TryGetRoute(Type type)
    {
        return TryGetRoutes(type).FirstOrDefault();
    }
}