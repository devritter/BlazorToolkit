# BlazorToolkit

Useful components and utilities for Blazor developers

## BzComponentBase

Your new component base class with:

* `Logger` property
* `InvokeAsyncStateHasChanged()` method
* `OnDispose()` method to override

and more to come!

### Registration:

**Per component:**

```
@inherits BlazingDev.BlazorToolkit.Components.BzComponentBase
```

**Globally:**

Locate `_Imports.razor` and add the following line to set the base component for all `*.razor` files in the same
directory and subdirectories:

```
@inherits BlazingDev.BlazorToolkit.Components.BzComponentBase
```

## BzTimerComponent

A blazor component that wraps a Timer instance. \
Automatically handles:

* Creating the `Timer`
* Calling `InvokeAsync()`
* Calling `StateHasChanged()` when using the `OnElapsed` `EventCallback`
* `OnElapsedAction` when you want to manually decide if any re-rendering is needed
* `try-catch` of the elapsed handlers
* disposing the timer when the component is unmounted

And you can use `ShowControls` for testing purposes which let you override the `Enabled` and `Interval` setting!

```xml
<BzTimerComponent Name="PriceRefreshTimer" Interval="5000" OnElapsed="HandleUpdatePriceTimerElapsed" />
```
