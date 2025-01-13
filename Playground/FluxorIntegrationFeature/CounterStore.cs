using Fluxor;

namespace Playground.FluxorIntegrationFeature;

[FeatureState]
public record CounterState
{
    public int CurrentCount { get; init; } = 10;
}

public record IncrementCounterAction;

public record IncrementCounterWithDelayAction(int DelayMs);

public static class CounterReducers
{
    [ReducerMethod]
    public static CounterState OnIncrement(CounterState state, IncrementCounterAction action)
    {
        return state with { CurrentCount = state.CurrentCount + 1 };
    }
}

public class CounterEffects
{
    [EffectMethod]
    public async Task HandleIncrementCounterWithDelay(IncrementCounterWithDelayAction action, IDispatcher dispatcher)
    {
        await Task.Delay(action.DelayMs);
        dispatcher.Dispatch(new IncrementCounterAction());
    }
}