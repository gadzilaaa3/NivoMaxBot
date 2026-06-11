using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Extensions;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Messaging.Routing;
using System.Reflection;

namespace NivoMaxBot.Messaging.Dispatchers
{
    public class GeneralUpdateDispatcher : IMessengerUpdateDispatcher
    {
        private readonly List<RouteEntry> _routes;
        private readonly IEnumerable<IMessageHandler> _messageHandlers;
        private readonly ILogger<GeneralUpdateDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMessengerClient _messengerClient;
        private readonly IErrorHandler _errorHandler;

        public GeneralUpdateDispatcher(
            IEnumerable<IMessageHandler> messageHandlers,
            ILogger<GeneralUpdateDispatcher> logger,
            IServiceProvider serviceProvider,
            ICurrentUserService currentUserService,
            IMessengerClient messengerClient,
            IErrorHandler errorHandler,
            Assembly handlersAssembly) // передаём сборку, где лежат классы с CallbackRoute
        {
            _messageHandlers = messageHandlers;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _currentUserService = currentUserService;
            _messengerClient = messengerClient;
            _errorHandler = errorHandler;
            _routes = BuildRoutes(handlersAssembly);
        }

        private List<RouteEntry> BuildRoutes(Assembly assembly)
        {
            var routes = new List<RouteEntry>();
            var types = assembly.GetTypes().Where(t => t.GetCustomAttributes<CallbackRouteAttribute>().Any());
            foreach (var type in types)
            {
                var method = type.GetMethod("HandleAsync", BindingFlags.Public | BindingFlags.Instance);
                if (method == null) continue;
                var attrs = type.GetCustomAttributes<CallbackRouteAttribute>();
                foreach (var attr in attrs)
                {
                    routes.Add(new RouteEntry
                    {
                        Template = new RouteTemplate(attr.Template),
                        HandlerType = type,
                        HandleMethod = method
                    });
                }
            }
            return routes;
        }

        public async Task HandleAsync(IMessengerUpdate update, CancellationToken cancellationToken)
        {
            var userId = update.GetUserId();
            if (userId.HasValue) _currentUserService.SetUser(userId.Value);
            try
            {
                // Сначала обрабатываем callback (если есть)
                if (update.CallbackQuery != null)
                {
                    await OnCallbackQuery(update.CallbackQuery, cancellationToken);
                }
                // Иначе обрабатываем сообщение
                else if (update.Message != null)
                {
                    await OnMessage(update.Message, cancellationToken);
                }
            }
            finally
            {
                _currentUserService.Clear();
            }
        }

        private async Task OnMessage(IMessage message, CancellationToken ct)
        {
            var handler = _messageHandlers.FirstOrDefault(h => h.CanHandle(message));
            if (handler != null)
            {
                try { await handler.HandleAsync(message, ct); }
                catch (Exception ex) { await _errorHandler.HandleError(message.ChatId.Value, ex, ct); }
            }
        }

        private async Task OnCallbackQuery(ICallbackQuery callback, CancellationToken ct)
        {
            foreach (var route in _routes)
            {
                if (route.Template.Match(callback.Data, out var values))
                {
                    var handler = _serviceProvider.GetRequiredService(route.HandlerType);
                    var parameters = BuildMethodParameters(route.HandleMethod, callback, ct, values);
                    try
                    {
                        await (Task)route.HandleMethod.Invoke(handler, parameters.ToArray());
                    }
                    catch (Exception ex)
                    {
                        await _errorHandler.HandleError(callback.Message.ChatId.Value, ex, ct);
                    }
                    finally
                    {
                        await _messengerClient.AnswerCallbackQueryAsync(callback.Id, ct: ct);
                    }
                    return;
                }
            }
            _logger.LogWarning("Unknown callback data: {Data}", callback.Data);
            await _messengerClient.AnswerCallbackQueryAsync(callback.Id, "Неизвестная команда", ct);
        }

        private List<object> BuildMethodParameters(MethodInfo method, ICallbackQuery callback, CancellationToken ct, Dictionary<string, object> routeValues)
        {
            var parameters = new List<object>();
            foreach (var param in method.GetParameters())
            {
                if (param.ParameterType == typeof(ICallbackQuery))
                    parameters.Add(callback);
                else if (param.ParameterType == typeof(CancellationToken))
                    parameters.Add(ct);
                else if (routeValues.TryGetValue(param.Name, out var val))
                    parameters.Add(val == null || param.ParameterType.IsInstanceOfType(val) ? val : Convert.ChangeType(val, param.ParameterType));
                else if (param.ParameterType.IsClass || Nullable.GetUnderlyingType(param.ParameterType) != null)
                    parameters.Add(null);
                else
                    throw new InvalidOperationException($"Не удалось найти значение для параметра {param.Name}");
            }
            return parameters;
        }
    }
}
