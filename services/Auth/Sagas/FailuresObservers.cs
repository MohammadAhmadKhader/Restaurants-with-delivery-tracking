using MassTransit;
using Shared.Kafka;

namespace Auth.Sagas;

public class FailuresObserver : IConsumeObserver
{
    private readonly ILogger<FailuresObserver> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    public FailuresObserver(ILogger<FailuresObserver> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }
    public async Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
    {
        _logger.LogInformation("Fault typeof({EventType} was received at ConsumeObserver)", typeof(T));
        // if the consumer method for InvitationAcceptedEvent has faulted then owner creating has failed
        if (typeof(T) == typeof(InvitationAcceptedEvent))
        {
            var msg = context.Message as InvitationAcceptedEvent;

            using var scope = _scopeFactory.CreateScope();
            var producer = scope.ServiceProvider
                        .GetRequiredService<ITopicProducer<OwnerCreatingFailedEvent>>();

            await producer.Produce(new(msg!.InvitationId));
            _logger.LogInformation("Fault typeof({EventType} was handled)", typeof(InvitationAcceptedEvent));

            return;
        }
    }

    public Task PreConsume<T>(ConsumeContext<T> context) where T : class => Task.CompletedTask;
    public Task PostConsume<T>(ConsumeContext<T> context) where T : class => Task.CompletedTask;
}