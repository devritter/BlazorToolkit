using System.Reflection;

namespace BlazingDev.BlazorToolkit.Components.Integrations;

internal static class BzComponentIntegrationHelper
{
    internal static async Task InitializeIntegrations(BzComponentBase component)
    {
        BzComponentIntegrationInitializationContext? ctx = null;

        foreach (var field in GetIntegrationFields(component.GetType()))
        {
            if (field.GetValue(component) is IBzComponentIntegration integration)
            {
                ctx ??= new BzComponentIntegrationInitializationContext()
                {
                    Component = component,
                    ComponentInternals = component
                };

                await integration.InitializeAsync(ctx);
            }
        }
    }

    private static IEnumerable<FieldInfo> GetIntegrationFields(Type? type)
    {
        if (type != null)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                        BindingFlags.DeclaredOnly)
                .Where(field => field.FieldType.IsAssignableTo(typeof(IBzComponentIntegration)));

            return fields.Concat(GetIntegrationFields(type.BaseType));
        }

        return [];
    }
}