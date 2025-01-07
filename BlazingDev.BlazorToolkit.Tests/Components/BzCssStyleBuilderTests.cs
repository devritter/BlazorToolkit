using System.Globalization;

namespace BlazingDev.BlazorToolkit.Tests.Components;

public class BzCssStyleBuilderTests
{
    [Fact]
    public void Add_AddsStyle()
    {
        var builder = new BzCssStyleBuilder();
        builder.Add("color", "red");
        builder.Build().Should().Be("color:red;");
    }

    [Fact]
    public void AddIf_AddsStyleWhenConditionIsTrue()
    {
        var builder = new BzCssStyleBuilder();
        builder.AddIf(true, "color", "red");
        builder.Build().Should().Be("color:red;");
    }

    [Fact]
    public void AddIf_DoesNotAddStyleWhenConditionIsFalse()
    {
        var builder = new BzCssStyleBuilder();
        builder.AddIf(false, "color", "red");
        builder.Build().Should().BeEmpty();
    }

    [Fact]
    public void AddIfWithFactory_AddsStyleWhenConditionIsTrue()
    {
        var builder = new BzCssStyleBuilder();
        builder.AddIf(true, "color", () => "red");
        builder.AddIf(false, "background-color", () => "red");
        builder.Build().Should().Be("color:red;");
    }

    [Fact]
    public void Add_AddsStyleWithNumberAndUnit()
    {
        var builder = new BzCssStyleBuilder();
        builder.Add("width", 100, "px");
        builder.Build().Should().Be("width:100px;");
    }

    [Fact]
    public void AddWithUnit_UsesInvariantCultureForFloatingPointNumbers()
    {
        var culture = CultureInfo.CreateSpecificCulture("de-DE");
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        new BzCssStyleBuilder()
            .Add("width", 33.33, "px")
            .Build()
            .Should().Be("width:33.33px;");
    }

    [Fact]
    public void AddIf_AddsStyleWithNumberAndUnitWhenConditionIsTrue()
    {
        var builder = new BzCssStyleBuilder();
        builder.AddIf(true, "width", 100, "px");
        builder.AddIf(false, "height", 100, "px");
        builder.Build().Should().Be("width:100px;");
    }

    [Fact]
    public void AddIfWithFactory_AddsStyleWithNumberAndUnitWhenConditionIsTrue()
    {
        var builder = new BzCssStyleBuilder();
        builder.AddIf(true, "width", () => 100, "px");
        builder.AddIf(false, "height", () => 100, "px");
        builder.Build().Should().Be("width:100px;");
    }

    [Fact]
    public void Add_AddsUserStylesWithSemicolon()
    {
        var builder = new BzCssStyleBuilder();
        builder.Add("display:  flex"); // auto added semicolon here
        builder.Add("gap:  10px;"); // not here
        builder.Add("flex-wrap:wrap; "); // also not here, ignoring 1 additional space
        builder.Build().Should().Be("display:  flex;gap:  10px;flex-wrap:wrap; ");
    }

    [Fact]
    public void Create_ReturnsNewInstance()
    {
        BzCssStyleBuilder.Create().Build().Should().BeEmpty();
        BzCssStyleBuilder.Create("some:style;").Build().Should().Be("some:style;");
        BzCssStyleBuilder.Create().Add("color", "blue").Build().Should().Be("color:blue;");
    }

    [Fact]
    public void ToString_IsEqualToBuild()
    {
        var builder = BzCssStyleBuilder.Create().Add("color", "red");
        builder.Build().Should().Be(builder.ToString());
    }

    [Fact]
    public void ReadmeExampleWorks()
    {
        // ReSharper disable once InconsistentNaming
        var UserFontSize = 10;
        var isBold = false;
        var concreteWidth = true;
        // ReSharper disable once InconsistentNaming
        string? Style = null;

        double CalculateWidthFunction() => 10.5;

        new BzCssStyleBuilder()
            .Add("color", "red")
            .Add("font-size", UserFontSize, "em")
            .AddIf(isBold, "font-weight", "bold")
            .AddIf(concreteWidth, "width", CalculateWidthFunction, "px")
            .Add(Style) // from component parameter
            .Build()
            .Should().Be("color:red;font-size:10em;width:10.5px;");
    }
}
