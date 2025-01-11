using Microsoft.AspNetCore.Components;

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
}