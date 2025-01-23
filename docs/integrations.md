# Integrations

Have you ever needed your injected service to be notified immediately when a component is initialized? \
If so, this solution is for you.

BlazorToolkit enables the enhancement of your components with additional base functionality, eliminating the need to
create custom base classes or override `OnInitialized`.

Simply inject the service relevant to your component, either directly within the component or via `_Imports.razor` (yes,
that works too).

## How It Works

This integration functionality is facilitated through the `IBzComponentIntegration` interface.

The `BzComponentBase` will automatically detect all local fields (including all injected services) and invoke the
`Task InitializeAsync(BzComponentIntegrationInitializationContext ctx)` method.

This `InitializeAsync` method is called before `OnInitialized` or `OnInitializedAsync`, ensuring that you don’t need to
worry about whether overridden components call the base function at the beginning or end of their implementation—or even
if `base.OnInitialized()` is invoked at all.

## Features

Using the `BzComponentIntegrationInitializationContext` object, you can access the following:

- The `Component` instance
- The component's `Disposer` instance
- Typically protected `ComponentInternals` members:
    - Typed `Logger`
    - `IsDisposed`
    - `InvokeAsync()`
    - `StateHasChanged()`

## Example

An integration for Fluxor has already been
created ([GitHub](https://github.com/mrpmorris/Fluxor), [NuGet](https://www.nuget.org/packages/Fluxor.Blazor.Web)).

In Fluxor, you typically inherit from `FluxorComponentBase` to automatically trigger component re-renders when the
injected state changes. Since multiple base classes aren’t supported, you can now use `BzComponentBase` as your base
class and inject `FluxorIntegration` for Fluxor-specific functionality.

```csharp
// Program.cs
// Transient, as each component will receive a "fresh" instance
builder.Services.AddTransient<FluxorIntegration>();

// In your component
@inject FluxorIntegration FluxorIntegration
```

## Need Assistance?

If you encounter any issues or have questions, please feel free to open an issue or reach out to me
at [devritter@hotmail.com](mailto:devritter@hotmail.com).
