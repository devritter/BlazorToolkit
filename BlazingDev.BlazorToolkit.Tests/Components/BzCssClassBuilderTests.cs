namespace BlazingDev.BlazorToolkit.Tests.Components;

public class BzCssClassBuilderTests
{
    [Fact]
    public void Add_ShouldAdd()
    {
        var builder = new BzCssClassBuilder();
        builder.Add("class1");
        builder.Build().Should().Be("class1");
    }

    [Fact]
    public void Add_ShouldIgnoreEmptyOrNullClass()
    {
        var builder = new BzCssClassBuilder();
        builder.Add((string?)null);
        builder.Add(string.Empty);
        builder.Build().Should().BeEmpty();
    }

    [Fact]
    public void Add_ShouldTrimClass()
    {
        var builder = new BzCssClassBuilder();
        builder.Add(" class1 ");
        builder.Build().Should().Be("class1");
    }

    [Fact]
    public void AddMultiple_ShouldAddMultipleClasses()
    {
        var builder = new BzCssClassBuilder();
        builder.Add(new List<string> { "class1", "class2" });
        builder.Build().Should().Be("class1 class2");
    }

    [Fact]
    public void AddMultiple_ShouldIgnoreEmptyOrNullClasses()
    {
        var builder = new BzCssClassBuilder();
        builder.Add(new List<string?> { null, string.Empty, "class1" });
        builder.Build().Should().Be("class1");
    }

    [Fact]
    public void AddMultiple_ShouldTrimClasses()
    {
        var builder = new BzCssClassBuilder();
        builder.Add(new List<string> { " class1 ", " class2 " });
        builder.Build().Should().Be("class1 class2");
    }

    [Fact]
    public void AddMultiple_ShouldAvoidDuplicateClasses()
    {
        var builder = new BzCssClassBuilder();
        builder.Add(new List<string> { "class1", "class1", "class2" });
        builder.Build().Should().Be("class1 class2");
    }

    [Fact]
    public void RemoveClass_ShouldRemoveClass()
    {
        var builder = new BzCssClassBuilder();
        builder.Add(new List<string> { "class1", "class2" });
        builder.Remove("class1");
        builder.Build().Should().Be("class2");
    }

    [Fact]
    public void AddIf_ShouldAddDependingOnCondition()
    {
        var builder = new BzCssClassBuilder();
        builder.AddIf(true, "\nclass1").AddIf(false, "class2");
        builder.Build().Should().Be("class1");
    }

    [Fact]
    public void RemoveIf_ShouldNotAddWhenConditionIsFalse()
    {
        var builder = new BzCssClassBuilder();
        builder.Add("class1").Add("class2");
        builder.RemoveIf(true, "class1\t").RemoveIf(false, "class2");
        builder.Build().Should().Be("class2");
    }

    [Fact]
    public void CreateMethods()
    {
        BzCssClassBuilder.Create().Build().Should().BeEmpty();
        BzCssClassBuilder.Create("class1").Build().Should().Be("class1");
        BzCssClassBuilder.Create(["class1", "class2\n", null]).Build().Should().Be("class1 class2");
    }

    [Fact]
    public void ToString_EqualsBuild()
    {
        var builder = new BzCssClassBuilder().Add("class1");
        builder.ToString().Should().Be(builder.Build());
    }
}