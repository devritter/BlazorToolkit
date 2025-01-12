using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using IComponent = System.ComponentModel.IComponent;

namespace BlazingDev.BlazorToolkit.Tests.Components;

public class BzComponentToolTests
{
    [Fact]
    public void GetRoute_ReturnsTheFirstDefinedRouteTemplate()
    {
        BzComponentTool.GetRoute<SingleRouteComponent>().Should().Be("/my-route");
        BzComponentTool.GetRoute(typeof(SingleRouteComponent)).Should().Be("/my-route");

        BzComponentTool.GetRoute<MultipleRouteComponent>().Should().Be("/my-first-route");
        BzComponentTool.GetRoute(typeof(MultipleRouteComponent)).Should().Be("/my-first-route");
    }

    [Fact]
    public void GetRoute_ThrowsException_WhenNoRouteIsDefined()
    {
        var exceptionMessage = "Component 'NoRouteComponent' has no route defined";
        Assert.Throws<InvalidOperationException>(() => BzComponentTool.GetRoute<NoRouteComponent>())
            .Message.Should().Match(exceptionMessage);
        Assert.Throws<InvalidOperationException>(() => BzComponentTool.GetRoute(typeof(NoRouteComponent)))
            .Message.Should().Match(exceptionMessage);
    }

    [Fact]
    public void TryGetRoutes_ReturnsNoneToManyRoutes()
    {
        BzComponentTool.TryGetRoutes<NoRouteComponent>().Should().BeEmpty();
        BzComponentTool.TryGetRoutes(typeof(NoRouteComponent)).Should().BeEmpty();

        BzComponentTool.TryGetRoutes<SingleRouteComponent>().Should().BeEquivalentTo(["/my-route"]);
        BzComponentTool.TryGetRoutes(typeof(SingleRouteComponent)).Should().BeEquivalentTo(["/my-route"]);

        BzComponentTool.TryGetRoutes<MultipleRouteComponent>().Should()
            .BeEquivalentTo(["/my-first-route", "/my-second-route"]);
        BzComponentTool.TryGetRoutes(typeof(MultipleRouteComponent)).Should()
            .BeEquivalentTo(["/my-first-route", "/my-second-route"]);
    }

    [Fact]
    public void TryGetRoute_TriesToReturnTheFirstRoute()
    {
        BzComponentTool.TryGetRoute<NoRouteComponent>().Should().BeNull();
        BzComponentTool.TryGetRoute(typeof(NoRouteComponent)).Should().BeNull();

        BzComponentTool.TryGetRoute<SingleRouteComponent>().Should().Be("/my-route");
        BzComponentTool.TryGetRoute(typeof(SingleRouteComponent)).Should().Be("/my-route");

        BzComponentTool.TryGetRoute<MultipleRouteComponent>().Should().Be("/my-first-route");
        BzComponentTool.TryGetRoute(typeof(MultipleRouteComponent)).Should().Be("/my-first-route");
    }

    [Fact]
    public void GetMenuItem_ReturnsRouteAndMenuItemAttributeData()
    {
        var menuItem = BzComponentTool.GetMenuItem<ShoppingCartPage>();
        menuItem.Name.Should().Be("Shopping Cart");
        menuItem.Icon.Should().Be("cart");
        menuItem.Sorting.Should().Be(900);
        menuItem.Route.Should().Be("/cart");
        menuItem.Type.Should().Be(typeof(ShoppingCartPage));
        menuItem.MatchMode.Should().Be(NavLinkMatch.All);

        BzComponentTool.GetMenuItem(typeof(ShoppingCartPage)).Should().Be(menuItem);
    }

    [Fact]
    public void GetMenuItem_ThrowsIfComponentHasNoRouteOrNoBzMenuItemAttribute()
    {
        Assert.Throws<InvalidOperationException>(() => BzComponentTool.GetMenuItem<ComponentWithOnlyBzMenuAttribute>())
            .Message.Should().Match("*no*route*");
        Assert.Throws<InvalidOperationException>(() => BzComponentTool.GetMenuItem<SingleRouteComponent>())
            .Message.Should().Match("*no*BzMenuItemAttribute*");
    }

    [Fact]
    public void TryGetMenuItem_ReturnsNullInsteadOfThrowingException()
    {
        // the "good case" first
        var menuItem = BzComponentTool.TryGetMenuItem<ShoppingCartPage>();
        menuItem.Should().NotBeNull();
        menuItem!.Name.Should().Be("Shopping Cart");
        menuItem.Icon.Should().Be("cart");
        menuItem.Sorting.Should().Be(900);
        menuItem.Route.Should().Be("/cart");
        menuItem.MatchMode.Should().Be(NavLinkMatch.All);
        menuItem.Type.Should().Be(typeof(ShoppingCartPage));

        BzComponentTool.TryGetMenuItem(typeof(ShoppingCartPage)).Should().Be(menuItem);

        BzComponentTool.TryGetMenuItem<ComponentWithOnlyBzMenuAttribute>().Should().BeNull();
        BzComponentTool.TryGetMenuItem(typeof(ComponentWithOnlyBzMenuAttribute)).Should().BeNull();

        BzComponentTool.TryGetMenuItem<SingleRouteComponent>().Should().BeNull();
        BzComponentTool.TryGetMenuItem(typeof(SingleRouteComponent)).Should().BeNull();
    }

    private class NoRouteComponent : ComponentBase
    {
    }

    [Route("/my-route")]
    private class SingleRouteComponent : ComponentBase
    {
    }

    [Route("/my-first-route")]
    [Route("/my-second-route")]
    private class MultipleRouteComponent : ComponentBase
    {
    }

    [Route("/cart")]
    [BzMenuItem(Name = "Shopping Cart", Icon = "cart", Sorting = 900, MatchMode = NavLinkMatch.All)]
    private class ShoppingCartPage : ComponentBase
    {
    }

    [BzMenuItem(Name = "About")]
    private class ComponentWithOnlyBzMenuAttribute : ComponentBase
    {
    }
}