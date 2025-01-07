using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace BlazingDev.BlazorToolkit.Components;

public partial class BzObjectInspector : ComponentBase
{
    [Parameter] public object ObjectToInspect { get; set; }

    private HashSet<string> expandedNodes = new();

    private RenderFragment RenderObjectTree(object obj) => builder =>
    {
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            var isExpandable = value != null && !value.GetType().IsPrimitive && value.GetType() != typeof(string);

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "bz-object-inspector-node");

            if (isExpandable)
            {
                builder.OpenElement(2, "button");
                builder.AddAttribute(3, "onclick", EventCallback.Factory.Create(this, () => ToggleExpandCollapse(property.Name)));
                builder.AddContent(4, IsExpanded(property.Name) ? "-" : "+");
                builder.CloseElement();
            }

            builder.AddContent(5, $"{property.Name}: {value}");

            if (isExpandable && IsExpanded(property.Name))
            {
                builder.OpenElement(6, "div");
                builder.AddAttribute(7, "class", "bz-object-inspector-children");
                builder.AddContent(8, RenderObjectTree(value));
                builder.CloseElement();
            }

            builder.CloseElement();
        }
    };

    private void ToggleExpandCollapse(string nodeName)
    {
        if (expandedNodes.Contains(nodeName))
        {
            expandedNodes.Remove(nodeName);
        }
        else
        {
            expandedNodes.Add(nodeName);
        }
    }

    private bool IsExpanded(string nodeName) => expandedNodes.Contains(nodeName);
}
