using System.Text;

namespace BlazingDev.BlazorToolkit.Tests;

public class BzReflectionTests
{
    private class DemoClass
    {
        public string? StringProperty { get; set; }
        public string? StringField;
        public int IntProperty { get; set; }
        public int IntField;
        public int? NullableIntProperty { get; set; }
        public int? NullableIntField;
    }

    private DemoClass FilledDemo { get; set; } = new()
    {
        StringProperty = "property",
        StringField = "field",
        IntProperty = 42,
        IntField = 42,
        NullableIntProperty = 42,
        NullableIntField = 42
    };

    private DemoClass NullDemo { get; set; } = new();

    [Fact]
    public void GetValue_FindsFieldsAndProperties()
    {
        BzReflection.GetValue<string>(FilledDemo, nameof(FilledDemo.StringProperty)).Should().Be("property");
        BzReflection.GetValue<string>(FilledDemo, nameof(FilledDemo.StringField)).Should().Be("field");
        BzReflection.GetValue<int>(FilledDemo, nameof(FilledDemo.IntProperty)).Should().Be(42);
        BzReflection.GetValue<int>(FilledDemo, nameof(FilledDemo.IntField)).Should().Be(42);
        BzReflection.GetValue<int?>(FilledDemo, nameof(FilledDemo.NullableIntProperty)).Should().Be(42);
        BzReflection.GetValue<int?>(FilledDemo, nameof(FilledDemo.NullableIntField)).Should().Be(42);

        BzReflection.GetValue<string>(NullDemo, nameof(NullDemo.StringProperty)).Should().BeNull();
        BzReflection.GetValue<string>(NullDemo, nameof(NullDemo.StringField)).Should().BeNull();
        BzReflection.GetValue<int>(NullDemo, nameof(NullDemo.IntProperty)).Should().Be(0);
        BzReflection.GetValue<int>(NullDemo, nameof(NullDemo.IntField)).Should().Be(0);
        BzReflection.GetValue<int?>(NullDemo, nameof(NullDemo.NullableIntProperty)).Should().BeNull();
        BzReflection.GetValue<int?>(NullDemo, nameof(NullDemo.NullableIntField)).Should().BeNull();
    }

    [Fact]
    public void GetValue_Throws_IfFieldIsNotFound()
    {
        var demo = new DemoClass();
        Assert.Throws<InvalidOperationException>(() => BzReflection.GetValue<string>(demo, "UnknownField"));
    }

    [Fact]
    public void GetValue_Throws_IfTypeMismatches()
    {
        // test not-null case
        Assert.Throws<InvalidCastException>(() =>
            BzReflection.GetValue<int>(FilledDemo, nameof(FilledDemo.StringProperty)));
        Assert.Throws<InvalidCastException>(
            () => BzReflection.GetValue<string>(FilledDemo, nameof(FilledDemo.IntField)));
        // test null case
        Assert.Throws<InvalidCastException>(() => BzReflection.GetValue<int>(NullDemo, nameof(NullDemo.StringField)));

        // commented out because this behavior needs to be defined
        // Assert.Throws<InvalidCastException>(() => BzReflection.GetValue<string>(NullDemo, nameof(NullDemo.NullableIntField)));
    }

    [Fact]
    public void SetValue_FindsFieldsAndProperties()
    {
        BzReflection.SetValue(NullDemo, nameof(NullDemo.StringProperty), FilledDemo.StringProperty);
        BzReflection.SetValue(NullDemo, nameof(NullDemo.StringField), FilledDemo.StringField);
        BzReflection.SetValue(NullDemo, nameof(NullDemo.IntProperty), FilledDemo.IntProperty);
        BzReflection.SetValue(NullDemo, nameof(NullDemo.IntField), FilledDemo.IntField);
        BzReflection.SetValue(NullDemo, nameof(NullDemo.NullableIntProperty), FilledDemo.NullableIntProperty);
        BzReflection.SetValue(NullDemo, nameof(NullDemo.NullableIntField), FilledDemo.NullableIntField);

        NullDemo.Should().BeEquivalentTo(FilledDemo);

        // test setting to null
        BzReflection.SetValue(FilledDemo, nameof(FilledDemo.StringProperty), null);
        FilledDemo.StringProperty.Should().BeNull();
    }

    [Fact]
    public void SetValue_Throws_IfFieldIsNotFound()
    {
        var demo = new DemoClass();
        Assert.Throws<InvalidOperationException>(() => BzReflection.SetValue(demo, "UnknownField", "someValue"));
    }

    [Theory]
    [InlineData(typeof(string), "string")]
    [InlineData(typeof(byte), "byte")]
    [InlineData(typeof(sbyte), "sbyte")]
    [InlineData(typeof(short), "short")]
    [InlineData(typeof(ushort), "ushort")]
    [InlineData(typeof(int), "int")]
    [InlineData(typeof(uint), "uint")]
    [InlineData(typeof(long), "long")]
    [InlineData(typeof(ulong), "ulong")]
    [InlineData(typeof(float), "float")]
    [InlineData(typeof(double), "double")]
    [InlineData(typeof(decimal), "decimal")]
    [InlineData(typeof(bool), "bool")]
    [InlineData(typeof(char), "char")]
    [InlineData(typeof(object), "object")]
    [InlineData(typeof(DateTime), "DateTime")]
    [InlineData(typeof(DateTimeOffset), "DateTimeOffset")]
    [InlineData(typeof(TimeSpan), "TimeSpan")]
    public void GetShortTypeName_WorksForBasicTypes(Type input, string expectedName)
    {
        BzReflection.GetShortTypeName(input).Should().Be(expectedName);
    }

    [Theory]
    [InlineData(typeof(StringBuilder), nameof(StringBuilder))]
    [InlineData(typeof(MemoryStream), nameof(MemoryStream))]
    [InlineData(typeof(BzReflection), nameof(BzReflection))]
    public void GetShortTypeName_WorksForClasses(Type input, string expectedName)
    {
        BzReflection.GetShortTypeName(input).Should().Be(expectedName);
    }

    [Theory]
    [InlineData(typeof(List<int>), "List<int>")]
    [InlineData(typeof(Dictionary<string, int>), "Dictionary<string, int>")]
    [InlineData(typeof(KeyValuePair<int, string>), "KeyValuePair<int, string>")]
    [InlineData(typeof(List<Dictionary<int, HashSet<string>>>), "List<Dictionary<int, HashSet<string>>>")]
    [InlineData(typeof((int, string, float)), "ValueTuple<int, string, float>")]
    public void GetShortTypeName_WorksForGenericTypes(Type input, string expectedName)
    {
        BzReflection.GetShortTypeName(input).Should().Be(expectedName);
    }

    [Theory]
    [InlineData(typeof(object[]), "object[]")]
    [InlineData(typeof(int[]), "int[]")]
    [InlineData(typeof(string[]), "string[]")]
    [InlineData(typeof(StringBuilder[]), "StringBuilder[]")]
    [InlineData(typeof(List<int[]>), "List<int[]>")]
    public void GetShortTypeName_WorksForArrays(Type input, string expectedName)
    {
        BzReflection.GetShortTypeName(input).Should().Be(expectedName);
    }
}