using BlazorShop.Client;
using BlazorShop.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Настройка логирования
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Настройка HttpClient
builder.Services.AddScoped<HttpClientHandler>(sp => new HttpClientHandler
{
    // Для разработки: игнорируем ошибки SSL
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
    // Разрешаем автоматическое перенаправление
    AllowAutoRedirect = true,
    // Увеличиваем таймаут
    UseCookies = true
});

builder.Services.AddScoped(sp => {
    var handler = sp.GetRequiredService<HttpClientHandler>();
    var client = new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7006/"), // URL сервера API
        Timeout = TimeSpan.FromSeconds(30) // Увеличиваем таймаут
    };
    
    // Добавляем заголовки для отладки
    client.DefaultRequestHeaders.Add("X-Client-App", "BlazorShop.Client");
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    
    Console.WriteLine($"HttpClient настроен с базовым URL: {client.BaseAddress}");
    
    return client;
});

// Регистрация сервисов
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ProductService>();

// Добавляем обработку ошибок
builder.Services.AddScoped<IExceptionHandler>(sp => 
    new DefaultExceptionHandler(sp.GetRequiredService<ILogger<DefaultExceptionHandler>>()));

await builder.Build().RunAsync();