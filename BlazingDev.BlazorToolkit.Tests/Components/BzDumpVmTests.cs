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
        new BzDumpVm(new List<string>()).Tooltip.Should().Be("List<string>");
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
}