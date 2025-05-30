using System.Reflection;

namespace BlazingDev.BlazorToolkit;

public static class BzReflection
{
    private const BindingFlags InstanceBindingFlags =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static T? GetValue<T>(object carrierObject, string memberName)
    {
        var carrierType = carrierObject.GetType();
        var desiredType = typeof(T);
        
        var member = GetRequiredPropertyOrField(carrierType, memberName);
        var value = member.Item1?.GetValue(carrierObject) ?? member.Item2?.GetValue(carrierObject);

        if (value == null && desiredType.IsValueType)
        {
            var isNullable = Nullable.GetUnderlyingType(desiredType) != null;
            if (isNullable)
            {
                // then returning null is OK
                return default;
            }
            
            // better than a NullReferenceException
            throw new InvalidCastException();
        }
        
        return (T?)value;
    }

    public static void SetValue(object carrierObject, string memberName, object? memberValue)
    {
        var type = carrierObject.GetType();
        var member = GetRequiredPropertyOrField(type, memberName);
        member.Item1?.SetValue(carrierObject, memberValue);
        member.Item2?.SetValue(carrierObject, memberValue);
    }

    private static (PropertyInfo?, FieldInfo?) GetRequiredPropertyOrField(Type type, string memberName)
    {
        var typeOfBaseType = type;
        while (typeOfBaseType != null)
        {
            var property = typeOfBaseType.GetProperty(memberName, InstanceBindingFlags);
            if (property != null)
            {
                return (property, null);
            }

            var field = typeOfBaseType.GetField(memberName, InstanceBindingFlags);
            if (field != null)
            {
                return (null, field);
            }

            typeOfBaseType = typeOfBaseType.BaseType;
        }

        throw new InvalidOperationException(
            $"The property or field '{memberName}' could not be found on type {type.Name}.");
    }
}