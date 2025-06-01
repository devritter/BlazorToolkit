using System.Globalization;

namespace BlazingDev.BlazorToolkit.Tests.Components;

public class BzDumpVmTests
{
    [Fact]
    public void DetectsCollections()
    {
        Test(null, false, null);
        Test(new int[0], true, 0);
        Test(new List<string>(), true, 0);
        Test("h e l l o".Split(), true, 5);
        Test(Enumerable.Empty<int>(), true, null); // null count because it does not inherit ICollection
        Test(Enumerable.Repeat(false, 1000).ToList(), true, 1000);

        // difficult to enumerate
        Test(Enumerable.Repeat("x", 1000).Where(x => true), true, null);

        void Test(object input, bool isCollection, int? collectionCount)
        {
            var vm = new BzDumpVm(input);
            vm.IsCollection.Should().Be(isCollection);
            vm.CollectionCount.Should().Be(collectionCount);
        }
    }

    [Fact]
    public void DetectsStrings()
    {
        var vm = new BzDumpVm("hello world");
        vm.IsCollection.Should().BeFalse(); // because it's better displayed as simple type
        vm.IsPrimitive.Should().BeTrue(); // also consider strings as "primitive"
        vm.Tooltip.Should().Be("string Length=11");
    }

    [Fact]
    public void FillsTooltipWithDateTimeKind()
    {
        new BzDumpVm(DateTime.Now).Tooltip.Should().Be("DateTime Kind=Local");
        new BzDumpVm(DateTime.UtcNow).Tooltip.Should().Be("DateTime Kind=Utc");
        var parsed = DateTime.Parse(DateTime.Now.ToString(CultureInfo.CurrentCulture));
        new BzDumpVm(parsed).Tooltip.Should().Be("DateTime Kind=Unspecified");

        // keep DateTimes as "complex" types so they can be expanded
        new BzDumpVm(DateTime.Now).IsPrimitive.Should().BeFalse();
    }

    [Fact]
    public void FillsTooltipWithShortTypeName()
    {
        new BzDumpVm(false).Tooltip.Should().Be("bool");
        new BzDumpVm(new MemoryStream()).Tooltip.Should().Be("MemoryStream");
        new BzDumpVm(new Lazy<string>()).Tooltip.Should().Be("Lazy<string>");
    }

    [Theory]
    [InlineData(5, true)]
    [InlineData((byte)5, true)]
    [InlineData(Math.PI, true)]
    [InlineData("hello", true)]
    [InlineData('\t', true)]
    [InlineData(true, true)]
    public void DetectsPrimitives(object input, bool isPrimitive)
    {
        new BzDumpVm(input).IsPrimitive.Should().Be(isPrimitive);
    }

    [Fact]
    public void FillsProperties()
    {
        var vm = new BzDumpVm(new MemoryStream());
        var properties = typeof(MemoryStream).GetProperties();
        vm.Properties.Should().BeEquivalentTo(properties);
    }

    [Fact]
    public void GetPropertyValue_CatchesExceptions()
    {
        var value = new MemoryStream();
        var vm = new BzDumpVm(value);
        var property = vm.Properties!.Single(x => x.Name == nameof(MemoryStream.ReadTimeout));
        var propertyValue = vm.GetPropertyValue(property);
        propertyValue.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void ValueToString_DefaultsToDefaultToString()
    {
        new BzDumpVm(5).ValueToString().Should().Be("5");
        new BzDumpVm(new CultureInfo("de-DE")).ValueToString().Should().Be("de-DE");
    }

    [Fact]
    public void ValueToString_IsRedirected_ForCollections()
    {
        var list = new List<string>();
        var vm = new BzDumpVm(list);
        vm.ValueToString().Should().Be("List<string> Count=0");
    }

    [Fact]
    public void ValueToString_IsRedirected_ForExceptions()
    {
        var ex = new InvalidOperationException("see inner exception", new NotImplementedException());
        var vm = new BzDumpVm(ex);
        vm.ValueToString().Should().Be(ex.Message);
        vm.ValueToString().Should().NotBe(ex.ToString());
    }

    [Fact]
    public void GetCollectionItems_ForArrays()
    {
        var value = new[] { 1, 2, 3 };
        var vm = new BzDumpVm(value);
        vm.GetCollectionItems().Should().BeEquivalentTo(value);
    }

    [Fact]
    public void GetCollectionItems_ForLists()
    {
        var value = new List<string> { "a", "b", "c" };
        var vm = new BzDumpVm(value);
        vm.GetCollectionItems().Should().BeEquivalentTo(value);
    }

    [Fact]
    public void GetCollectionItems_CatchesExceptions()
    {
        var ints = new List<int> { 1 };
        var memoryStreams = new List<MemoryStream>() { new() };
        var dangerousList = ints.Concat(memoryStreams.Select(x => x.WriteTimeout));
        var vm = new BzDumpVm(dangerousList);
        var values = vm.GetCollectionItems().OfType<object>().ToList();
        values[0].Should().Be(1);
        values[1].Should().BeOfType<InvalidOperationException>();
    }
}