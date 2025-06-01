using System.Collections;

namespace BlazingDev.BlazorToolkit.Components;

internal class BzDumpVm
{
    internal static readonly BzDumpVm NullValue = new(null);

    public BzDumpVm(object? value)
    {
        Value = value;
        if (value == null)
        {
            Type = typeof(void);
            Tooltip = "";
            return;
        }

        Type = value.GetType();
        Tooltip = BzReflection.GetShortTypeName(Type);
        
        if (value is String aString)
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
        }
        else if (Type.IsPrimitive)
        {
            IsPrimitive = true;
        }
        else if (Value is DateTime dateTime)
        {
            Tooltip += " Kind=" + dateTime.Kind;
        }
    }

    public object? Value { get; }
    public Type Type { get; }
    public string Tooltip { get; }
    public bool IsPrimitive { get; }
    public bool IsCollection { get; }
    public int? CollectionCount { get; }
}