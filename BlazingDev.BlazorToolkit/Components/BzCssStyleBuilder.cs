using System.Text;

namespace BlazingDev.BlazorToolkit.Components;

public class BzCssStyleBuilder
{
    private readonly StringBuilder _sb = new();

    public static BzCssStyleBuilder Create()
    {
        return new BzCssStyleBuilder();
    }

    public static BzCssStyleBuilder Create(string initialStyles)
    {
        return new BzCssStyleBuilder().Add(initialStyles);
    }

    public BzCssStyleBuilder Add(string property, string value)
    {
        _sb.Append($"{property}:{value};");
        return this;
    }

    public BzCssStyleBuilder AddIf(bool condition, string property, string value)
    {
        if (condition)
        {
            _sb.Append($"{property}:{value};");
        }

        return this;
    }

    public BzCssStyleBuilder AddIf(bool condition, string property, Func<string> valueFactory)
    {
        if (condition)
        {
            _sb.Append($"{property}:{valueFactory()};");
        }

        return this;
    }

    public BzCssStyleBuilder Add(string property, double value, string unit)
    {
        _sb.Append($"{property}:{value.ToInvariantString()}{unit};");
        return this;
    }

    public BzCssStyleBuilder AddIf(bool condition, string property, double value, string unit)
    {
        if (condition)
        {
            _sb.Append($"{property}:{value.ToInvariantString()}{unit};");
        }

        return this;
    }

    public BzCssStyleBuilder AddIf(bool condition, string property, Func<double> valueFactory, string unit)
    {
        if (condition)
        {
            _sb.Append($"{property}:{valueFactory().ToInvariantString()}{unit};");
        }

        return this;
    }

    public BzCssStyleBuilder Add(string? userStyles)
    {
        if (userStyles.HasContent())
        {
            _sb.Append(userStyles);

            if (userStyles.EndsWith(';') ||
                userStyles.EndsWith("; "))
            {
                // no additional ; needed
            }
            else
            {
                _sb.Append(';');
            }
        }

        return this;
    }

    public string Build()
    {
        return _sb.ToString();
    }

    public override string ToString()
    {
        return Build();
    }
}