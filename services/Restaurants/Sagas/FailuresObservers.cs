using MassTransit;
using Shared.Kafka;

namespace Restaurants.Sagas;

public class FailuresObserver : IConsumeObserver
{
    private readonly ILogger<FailuresObserver> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    public FailuresObserver(ILogger<FailuresObserver> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }
    public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
    {
        _logger.LogInformation("Unhandled fault typeof({EventType} was received at ConsumeObserver)", typeof(T));
        return Task.CompletedTask;
    }

    public Task PreConsume<T>(ConsumeContext<T> context) where T : class => Task.CompletedTask;
    public Task PostConsume<T>(ConsumeContext<T> context) where T : class => Task.CompletedTask;
}