using BlazingDev.BlazorToolkit.FluxorIntegration;
using Fluxor;
using Playground.AppFrame;
using Playground.BzAsyncDisposerFeature;
using Playground.FluxorIntegrationFeature;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluxor(o => o.ScanAssemblies(typeof(CounterState).Assembly));
builder.Services.AddTransient<FluxorIntegration>();

builder.Services.AddScoped<ScopedCounterWithTimer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();