# BlazorToolkit

Useful components and utilities for Blazor developers

## BzComponentBase

Your new component base class with:

* `Logger` property
* `InvokeAsyncStateHasChanged()` method
* `OnDispose()` method to override

and more to come!


## BzTimerComponent

A blazor component that wraps a Timer instance. \
The good thing? You don't have to handle creating the timer, calling StateHasChanged, and disposing the timer!

```xml
<BzTimerComponent Name="PriceRefreshTimer" Interval="5000" OnElapsed="HandleUpdatePriceTimerElapsed" />
```
