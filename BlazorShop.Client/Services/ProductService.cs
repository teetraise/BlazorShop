using BlazorShop.Shared.Models;
using System.Net.Http.Json;

namespace BlazorShop.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<Product>>("api/products");
                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении товаров: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Product>($"api/products/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении товара: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool Success, string ErrorMessage)> CreateProductAsync(Product product)
    {
        try
        {
            Console.WriteLine($"Отправка запроса на создание товара: {product.Name}");
            Console.WriteLine($"URL запроса: {_httpClient.BaseAddress}api/products");
            
            var response = await _httpClient.PostAsJsonAsync("api/products", product);
            
            Console.WriteLine($"Получен ответ с кодом: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
                Console.WriteLine($"Товар успешно создан с ID: {createdProduct?.Id}");
                return (true, string.Empty);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка при создании товара. Статус: {response.StatusCode}, Сообщение: {errorContent}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    // Попытка десериализовать ошибки валидации, если они есть
                    try
                    {
                        var validationErrors = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(errorContent);
                        var formattedErrors = string.Join("; ", validationErrors.SelectMany(kvp => kvp.Value.Select(v => $"{kvp.Key}: {v}")));
                        return (false, $"Ошибка валидации: {formattedErrors}");
                    }
                    catch
                    {
                        // Если не удалось десериализовать как ошибки валидации, возвращаем общий текст
                        return (false, $"Ошибка: {errorContent}");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return (false, "Ошибка 404: API-маршрут не найден. Проверьте URL и маршрутизацию.");
                }
                else
                {
                    return (false, $"Произошла ошибка: {response.StatusCode} - {errorContent}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Исключение при создании товара: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Внутреннее исключение: {ex.InnerException.Message}");
            }
            
            if (ex is System.Net.Http.HttpRequestException)
            {
                return (false, "Ошибка HTTP-запроса: возможно, сервер недоступен или неправильно настроен CORS.");
            }
            
            return (false, $"Произошла непредвиденная ошибка: {ex.Message}");
        }
    }

        public async Task<bool> UpdateProductAsync(Product product)
    {
        try
        {
            Console.WriteLine($"Отправка запроса на обновление товара с ID: {product.Id}");
            Console.WriteLine($"URL запроса: {_httpClient.BaseAddress}api/products/{product.Id}");
            
            var response = await _httpClient.PutAsJsonAsync($"api/products/{product.Id}", product);
            
            Console.WriteLine($"Получен ответ с кодом: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Товар успешно обновлен: {product.Name}");
                return true;
            }
            else
            {
                // Получаем подробную информацию об ошибке
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка при обновлении товара. Статус: {response.StatusCode}, Сообщение: {errorContent}");
                
                // Проверяем, является ли ошибка связанной с маршрутизацией
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Ошибка 404: Товар с ID {product.Id} не найден или API-маршрут неверен.");
                }
                
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Исключение при обновлении товара: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Внутреннее исключение: {ex.InnerException.Message}");
            }
            
            // Проверяем, является ли ошибка связанной с сетью
            if (ex is System.Net.Http.HttpRequestException)
            {
                Console.WriteLine("Ошибка HTTP-запроса: возможно, сервер недоступен или неправильно настроен CORS.");
            }
            
            return false;
        }
    }

        public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            Console.WriteLine($"Отправка запроса на удаление товара с ID: {id}");
            Console.WriteLine($"URL запроса: {_httpClient.BaseAddress}api/products/{id}");
            
            var response = await _httpClient.DeleteAsync($"api/products/{id}");
            
            Console.WriteLine($"Получен ответ с кодом: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Товар с ID {id} успешно удален");
                return true;
            }
            else
            {
                // Получаем подробную информацию об ошибке
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка при удалении товара. Статус: {response.StatusCode}, Сообщение: {errorContent}");
                
                // Проверяем, является ли ошибка связанной с маршрутизацией
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Ошибка 404: Товар с ID {id} не найден или API-маршрут неверен.");
                }
                
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Исключение при удалении товара: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Внутреннее исключение: {ex.InnerException.Message}");
            }
            
            // Проверяем, является ли ошибка связанной с сетью
            if (ex is System.Net.Http.HttpRequestException)
            {
                Console.WriteLine("Ошибка HTTP-запроса: возможно, сервер недоступен или неправильно настроен CORS.");
            }
            
            return false;
        }
    }
    }
}
