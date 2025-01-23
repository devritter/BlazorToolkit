# BlazorToolkit

Useful components and utilities for Blazor developers

## BzComponentBase

Your new component base class with:

* typed `Logger` instance
* `IsInitialized` property to simplify your rendering logic for asynchronously initialized components!
* `Disposer` object to register disposables at creation time!
* `InvokeAsyncStateHasChanged()` method
* Support for [Integrations](https://github.com/devritter/BlazorToolkit/blob/main/docs/integrations.md)

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

### Example

```csharp
FileStream _fileStream = null;

protected override void OnInitialized()
{
    _fileStream = GetFileStream();
    
    // BzAsyncDisposer from base class
    Disposer.Add(_fileStream);
    
    var subscription = SubscriptionService.Subscribe("important-messages", HandleImportantMessage);
    // Logger from base class
    Logger.LogInformation("Got subscription {SubscriptionId}", subscription.Id);
    Disposer.Add(subscription);
    
    SubscriptionService.ConnectionLost += HandleConnectionLost;
    Disposer.Add(() => SubscriptionService.ConnectionLost -= HandleConnectionLost);
    Disposer.Add(SayGoodbyeAsync);
}

private void HandleConnectionLost(object sender, EventArgs e)
{
    ShowReconnectOverlay = true;
    // little simplified method from base class
    InvokeAsyncStateHasChanged();
}
```

## BzComponentTool

Provides utility methods regarding components.

### GetRoute

With the following methods you can retrieve a component's route (`@page "/this-is-the-route"`). \
Useful if you want to create links to other components and you don't want to have magic strings in your code.

```csharp
 // returns null for "simple" components or the first defined rounte
BzComponentTool.TryGetRoute<PotentiallyRoutableComponent>();
BzComponentTool.TryGetRoute(typeof(PotentiallyRoutableComponent));
// returns the first defined route or throws for "simple" components
BzComponentTool.GetRoute<SomePage>();
BzComponentTool.GetRoute(typeof(SomePage));
// returns zero-to-many items
BzComponentTool.TryGetRoutes<PageWithMultipleRoutes>();
BzComponentTool.TryGetRoutes(typeof(PageWithMultipleRoutes));
```

### GetMenuItem

Use the `BzMenuItemAttribute` to specify menu items at component level. \
For extra laziness you can use the `BzComponentTool.GetAllMenuItemsFromAssembly(assembly)` method :)

```csharp
@page "/about"
@attribute [BzMenuItem(Name = "About", Icon = "people", Sorting = 500)]
```

```csharp
BzComponentTool.GetMenuItem<AboutPage>();    // requires ( @page OR [Route] ) AND [BzMenuItem]
BzComponentTool.TryGetMenuItem<AboutPage>(); // returns null if anything required is missing

// NavMenu.razor
@foreach (var item in BzComponentTool.GetAllMenuItemsFromAssembly(GetType().Assembly))
{
    <MyNavItem MenuItem="item"/>
}
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

## BzCssClassBuilder

A utility class to help creating CSS classes lists.

* static `Create` methods to not use spaces (sometimes handy in razor files)
* `Add(className)`, `Add(listOfClassNames)`, `AddIf(condition, className)`
* `Remove(className)`, `RemoveIf(condition, className)`
* automatically trimes classNames
* ignores duplicates and no-content classNames
* use `Build()` or `ToString()` to get your final string

### Usage Example:

```csharp
@{
    var cssClasses = BzCssClassBuilder.Create("my-button")
        .Add("button-primary")
        .AddIf(isOutline, "button-outline")
        .Add(SomeSettings.AdditionalButtonClasses) // e.g. theme-specific
        .Build()
}
<button class="@cssClasses">...</button>
```

## BzCssStyleBuilder

A utility class for building CSS styles conditionally and fluently.

### Usage:

```csharp
var style = new BzCssStyleBuilder()
    .Add("color", "red")
    .Add("font-size", UserFontSize, "em")
    .AddIf(isBold, "font-weight", "bold")
    .AddIf(concreteWidth, "width", CalculateWidthFunction, "px")
    .Add(Style) // from component parameter
    .Build();
```

## BzRenderMode

Predefined render modes without prerender

### Usage

**per component, e.g. in `App.razor`:**

```
<Routes @rendermode="BzRenderMode.InteractiveServerNoPrerender"/>
```

If you add the following line in `_Imports.razor`, you can omit the `BzRenderMode` prefix:

```
@using static BlazingDev.BlazorToolkit.Components.BzRenderMode
```
