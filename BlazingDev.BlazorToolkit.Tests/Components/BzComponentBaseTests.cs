using BlazingDev.BlazingExtensions.BlazingUtilities;
using BlazingDev.BlazorToolkit.Components.Integrations;
using BlazingDev.BlazorToolkit.Tests.TestSupport;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlazingDev.BlazorToolkit.Tests.Components;

public class BzComponentBaseTests
{
    [Fact]
    public void LoggerFactory_IsCreatedOnFirstAccess()
    {
        var sut = new BzComponentBase();
        var loggerFactory = new NullLoggerFactory();
        BzReflection.SetValue(sut, "LoggerFactory", loggerFactory);

        BzReflection.GetValue<ILogger?>(sut, "_componentLogger").Should().BeNull();

        var logger = BzReflection.GetValue<ILogger>(sut, "Logger");
        logger.Should().NotBeNull();
        BzReflection.GetValue<ILogger?>(sut, "_componentLogger").Should().Be(logger);

        // continuous access should return same instance
        BzReflection.GetValue<ILogger>(sut, "Logger").Should().Be(logger);
    }

    [Fact]
    public void Disposer_IsCreatedOnFirstAccess()
    {
        var sut = new BzComponentBase();

        BzReflection.GetValue<BzAsyncDisposer?>(sut, "_disposer").Should().BeNull();

        var disposer = BzReflection.GetValue<BzAsyncDisposer>(sut, "Disposer");
        disposer.Should().NotBeNull();
        BzReflection.GetValue<BzAsyncDisposer?>(sut, "_disposer").Should().Be(disposer);

        // continuous access should return same instance
        BzReflection.GetValue<BzAsyncDisposer>(sut, "Disposer").Should().Be(disposer);
    }

    [Fact]
    public async Task IsInitialized_IsSet_WhenAllSyncAndAsyncMethodsHaveBeenCalled()
    {
        var sut = new CheckIsInitializedComponent();
        BzReflection.SetValue(sut, "_hasPendingQueuedRender", true); // to skip rendering
        sut.IsInitialized.Should().BeFalse();

        var task = sut.SetParametersAsync(ParameterView.Empty);
        sut.IsInitialized.Should().BeFalse();
        sut.OnInitializedSemaphore.Release();
        await task;
        sut.IsInitialized.Should().BeTrue();

        // on subsequent calls everything should already be initialized
        sut.ExpectedIsInitialized = true;
        await sut.SetParametersAsync(ParameterView.Empty);
    }

    [Fact]
    public async Task IsInitialized_IsNotPrematurelySet_IfNewParametersAreReceived_BeforeInitializationHasFinished()
    {
        var sut = new CheckIsInitializedComponent();
        BzReflection.SetValue(sut, "_hasPendingQueuedRender", true); // to skip rendering

        var task = sut.SetParametersAsync(ParameterView.Empty);
        sut.IsInitialized.Should().BeFalse();

        // can be awaited because it does not run through IsInitializedAsync any more
        // internally asserts against IsInitialized=false
        await sut.SetParametersAsync(ParameterView.Empty);

        sut.OnInitializedSemaphore.Release();
        await task;
        sut.IsInitialized.Should().BeTrue();

        sut.ExpectedIsInitialized = true;
        await sut.SetParametersAsync(ParameterView.Empty);
    }

    [Fact]
    public async Task SetParametersAsync_InitializesPossibleIntegrations()
    {
        // Arrange
        var sut = new ComponentWithIntegrationsSubclass();
        BzReflection.SetValue(sut, "_hasPendingQueuedRender", true); // to skip rendering

        // Act
        await sut.SetParametersAsync(ParameterView.Empty);

        // Assert
        sut.Integration1.Ctx.Should().NotBeNull();
        sut.Integration1.Ctx!.Component.Should().BeSameAs(sut);
        BzReflection.GetValue<TestIntegration>(sut, "_integration2")!.Ctx.Should().NotBeNull();
        BzReflection.GetValue<TestIntegration>(sut, "_integration3").Should().BeNull();
    }

    private class CheckIsInitializedComponent : BzComponentBase
    {
        public new bool IsInitialized => base.IsInitialized;
        public bool ExpectedIsInitialized { get; set; }
        public SemaphoreSlim OnInitializedSemaphore { get; } = new(0);

        protected override void OnInitialized()
        {
            // we wait before and after to show that it has no relevance for IsInitialized
            IsInitialized.Should().Be(ExpectedIsInitialized);
            base.OnInitialized();
            IsInitialized.Should().Be(ExpectedIsInitialized);
        }

        protected override async Task OnInitializedAsync()
        {
            IsInitialized.Should().Be(ExpectedIsInitialized);
            await OnInitializedSemaphore.WaitAsync();
            await base.OnInitializedAsync();
            IsInitialized.Should().Be(ExpectedIsInitialized);
        }

        protected override void OnParametersSet()
        {
            IsInitialized.Should().Be(ExpectedIsInitialized);
            base.OnParametersSet();
            IsInitialized.Should().Be(ExpectedIsInitialized);
        }

        protected override async Task OnParametersSetAsync()
        {
            IsInitialized.Should().Be(ExpectedIsInitialized);
            await base.OnParametersSetAsync();
            IsInitialized.Should().Be(ExpectedIsInitialized);
        }
    }

    private class ComponentWithIntegrations : BzComponentBase
    {
        public TestIntegration Integration1 { get; set; } = new();
        private TestIntegration _integration2 = new();
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private TestIntegration _integration3 = null!;
#pragma warning restore CS0414 // Field is assigned but its value is never used
    }

    // just to also test inheritance
    private class ComponentWithIntegrationsSubclass : ComponentWithIntegrations
    {
    }

    private class TestIntegration : IBzComponentIntegration
    {
        public BzComponentIntegrationInitializationContext? Ctx { get; set; }

        public Task InitializeAsync(BzComponentIntegrationInitializationContext ctx)
        {
            Ctx = ctx;
            return Task.CompletedTask;
        }
    }
}