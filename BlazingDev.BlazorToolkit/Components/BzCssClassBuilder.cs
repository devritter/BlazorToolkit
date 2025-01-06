namespace BlazingDev.BlazorToolkit.Components;

public class BzCssClassBuilder
{
    private readonly HashSet<string> _classNames = new();

    public static BzCssClassBuilder Create()
    {
        return new BzCssClassBuilder();
    }

    public static BzCssClassBuilder Create(string? className)
    {
        return new BzCssClassBuilder().Add(className);
    }

    public static BzCssClassBuilder Create(IEnumerable<string?> classNames)
    {
        return new BzCssClassBuilder().Add(classNames);
    }

    public BzCssClassBuilder Add(string? className)
    {
        if (className.HasContent())
        {
            _classNames.Add(className.Trim());
        }

        return this;
    }

    public BzCssClassBuilder Add(IEnumerable<string?> classNames)
    {
        foreach (var className in classNames)
        {
            Add(className);
        }

        return this;
    }

    public BzCssClassBuilder AddIf(bool condition, string className)
    {
        if (condition)
        {
            Add(className);
        }

        return this;
    }

    public BzCssClassBuilder Remove(string? className)
    {
        if (className.HasContent())
        {
            _classNames.Remove(className.Trim());
        }

        return this;
    }

    public BzCssClassBuilder RemoveIf(bool condition, string className)
    {
        if (condition)
        {
            Remove(className);
        }

        return this;
    }

    public string Build()
    {
        return string.Join(" ", _classNames);
    }

    public override string ToString()
    {
        return Build();
    }
}