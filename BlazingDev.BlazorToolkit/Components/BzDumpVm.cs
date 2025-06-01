using System.Collections;
using System.Reflection;

namespace BlazingDev.BlazorToolkit.Components;

internal class BzDumpVm
{
    internal static readonly BzDumpVm NullValue = new(null);

    public BzDumpVm(object? value)
    {
        Value = value;
        ValueToString = DefaultToString;

        if (value == null)
        {
            Type = typeof(void);
            Tooltip = "";
            return;
        }

        Type = value.GetType();
        Tooltip = BzReflection.GetShortTypeName(Type);

        if (value is string aString)
        {
            IsPrimitive = true;
            Tooltip += " Length=" + aString.Length;
        }
        else if (value is IEnumerable enumerable)
        {
            IsCollection = true;

            if (Value is Array array)
            {
                CollectionCount = array.Length;
            }
            else if (Value is ICollection collection)
            {
                CollectionCount = collection.Count;
            }
            else if (enumerable.OfType<object>().TryGetNonEnumeratedCount(out var count))
            {
                CollectionCount = count;
            }

            Tooltip += " Count=" + (CollectionCount?.ToString() ?? "?");
            ValueToString = TooltipToString;
        }
        else if (Type.IsPrimitive || Value is Guid)
        {
            IsPrimitive = true;
        }
        else if (Type.IsEnum)
        {
            IsPrimitive = true;
            var numericValue = Convert.ToInt64(Value);
            ValueToString = () => $"{Value} ({numericValue})";
            Tooltip = $"{Type.Name}.{Value} = {numericValue}";
        }

        // else complex type
        Properties = Type.GetProperties();

        if (Value is DateTime dateTime)
        {
            Tooltip += " Kind=" + dateTime.Kind;
        }
        else if (Value is Exception ex)
        {
            ValueToString = () => ex.Message;
        }
    }

    public object? Value { get; }
    public Func<string?> ValueToString { get; }

    public string Tooltip { get; }
    public bool IsPrimitive { get; }
    public bool IsCollection { get; }
    public int? CollectionCount { get; }

    private Type Type { get; }
    public PropertyInfo[]? Properties { get; }

    public bool IsExpanded { get; set; }

    public object? GetPropertyValue(PropertyInfo property)
    {
        try
        {
            return property.GetValue(Value);
        }
        catch (TargetInvocationException targetEx)
        {
            return targetEx.InnerException;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private string? DefaultToString()
    {
        return Value?.ToString();
    }

    private string? TooltipToString()
    {
        return Tooltip;
    }

    public IEnumerable GetCollectionItems()
    {
        var list = new List<object>();
        if (Value is IEnumerable enumerable)
        {
            try
            {
                foreach (var item in enumerable)
                {
                    list.Add(item);
                }
            }
            catch (Exception ex)
            {
                list.Add(ex);
            }
        }

        return list;
    }
}