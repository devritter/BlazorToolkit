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

    private DemoClass FilledDemo { get; set; } = new DemoClass()
    {
        StringProperty = "property",
        StringField = "field",
        IntProperty = 42,
        IntField = 42,
        NullableIntProperty = 42,
        NullableIntField = 42,
    };

    private DemoClass NullDemo { get; set; } = new DemoClass();
    
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
        Assert.Throws<InvalidCastException>(() => BzReflection.GetValue<int>(FilledDemo, nameof(FilledDemo.StringProperty)));
        Assert.Throws<InvalidCastException>(() => BzReflection.GetValue<string>(FilledDemo, nameof(FilledDemo.IntField)));
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
}