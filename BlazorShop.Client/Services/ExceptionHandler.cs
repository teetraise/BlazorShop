using Microsoft.Extensions.Logging;

namespace BlazorShop.Client.Services
{
    // Интерфейс для обработки исключений
    public interface IExceptionHandler
    {
        void HandleException(Exception ex);
    }

    // Реализация обработчика исключений по умолчанию
    public class DefaultExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<DefaultExceptionHandler> _logger;

        public DefaultExceptionHandler(ILogger<DefaultExceptionHandler> logger)
        {
            _logger = logger;
        }

        public void HandleException(Exception ex)
        {
            _logger.LogError(ex, "Необработанное исключение");
        }
    }
}