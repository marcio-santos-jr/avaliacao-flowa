using OrderUI.Clients;
using OrderUI.Interfaces;
using OrderUI.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddRefitClient<IOrderGeneratorApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["OrderGeneratorApiBaseUrl"]!));

builder.Services.AddScoped<IOrderGeneratorService, OrderGeneratorService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
