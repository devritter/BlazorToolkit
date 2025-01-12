using System.Diagnostics.CodeAnalysis;
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

    public static BzMenuItemModel GetMenuItem<T>() where T : IComponent
    {
        return TryGetMenuItemCore(typeof(T), true)!;
    }

    public static BzMenuItemModel GetMenuItem(Type type)
    {
        return TryGetMenuItemCore(type, true)!;
    }

    public static BzMenuItemModel? TryGetMenuItem<T>() where T : IComponent
    {
        return TryGetMenuItemCore(typeof(T), false);
    }

    public static BzMenuItemModel? TryGetMenuItem(Type type)
    {
        return TryGetMenuItemCore(type, false);
    }

    private static BzMenuItemModel? TryGetMenuItemCore(Type type, bool allowExceptions)
    {
        var route = allowExceptions ? GetRoute(type) : TryGetRoute(type);
        if (route == null && !allowExceptions)
        {
            return null;
        }

        var attribute = type.GetCustomAttribute<BzMenuItemAttribute>();
        if (attribute == null)
        {
            if (allowExceptions)
            {
                throw new InvalidOperationException($"Component '{type.Name}' has no BzMenuItemAttribute defined");
            }

            return null;
        }

        return new BzMenuItemModel(
            attribute.Name,
            attribute.Icon,
            attribute.Sorting,
            route,
            attribute.MatchMode,
            type);
    }

    /// <summary>
    /// Returns BzMenuItemModels for every public type inside the given assembly
    /// that has a Route (@page "/this-is-the-route" or [RouteAttribute]) and [BzMenuItemAttribute] specified.
    /// The returned list is sorted by the BzMenuItemAttribute.Sorting property.
    /// </summary>
    public static List<BzMenuItemModel> GetAllMenuItemsFromAssembly(Assembly assembly)
    {
        var types = assembly.GetExportedTypes();
        var result = types
            .Select(TryGetMenuItem)
            .WhereNotNull()
            .OrderBy(x => x.Sorting)
            .ToList();
        return result;
    }
}