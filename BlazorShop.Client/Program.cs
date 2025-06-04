using BlazorShop.Client;
using BlazorShop.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Настройка логирования
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Настройка HttpClient - используем правильный URL сервера
builder.Services.AddScoped(sp => {
    var client = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7006/") // URL из launchSettings.json сервера
    };

    Console.WriteLine($"HttpClient настроен с базовым URL: {client.BaseAddress}");

    return client;
});

// Регистрация сервисов
builder.Services.AddScoped<IProductService, ProductService>();

// Добавляем обработку ошибок
builder.Services.AddScoped<IExceptionHandler>(sp =>
    new DefaultExceptionHandler(sp.GetRequiredService<ILogger<DefaultExceptionHandler>>()));

await builder.Build().RunAsync();