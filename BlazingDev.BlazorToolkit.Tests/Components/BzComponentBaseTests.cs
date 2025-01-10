using BlazingDev.BlazingExtensions.BlazingUtilities;
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
        sut.IsInitialized.Should().BeFalse();
        await sut.SetParametersAsync(ParameterView.Empty);
        sut.IsInitialized.Should().BeTrue();

        // on subsequent calls everything should already be initialized
        sut.ExpectedIsInitialized = true;
        await sut.SetParametersAsync(ParameterView.Empty);
    }

    private class CheckIsInitializedComponent : BzComponentBase
    {
        public new bool IsInitialized => base.IsInitialized;
        public bool ExpectedIsInitialized { get; set; }

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
}