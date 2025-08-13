using MassTransit;

namespace Shared.Kafka;

public class TopicProducerInMemoryOverride<TMessage>(ISendEndpointProvider sendEndpointProvider) : ITopicProducer<TMessage>
        where TMessage : class
{
    private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;

    public async Task Produce(TMessage message, CancellationToken cancellationToken = default)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(GetTopicUrl());
        await endpoint.Send(message, cancellationToken);
    }

    public async Task Produce(TMessage message, IPipe<KafkaSendContext<TMessage>> pipe, CancellationToken cancellationToken = default)
        => await Produce(message, cancellationToken);

    public async Task Produce(object message, CancellationToken cancellationToken = default)
        => await Produce((TMessage) message, cancellationToken);

    public async Task Produce(object message, IPipe<KafkaSendContext<TMessage>> pipe, CancellationToken cancellationToken = default)
        => await Produce((TMessage) message, cancellationToken);

    public ConnectHandle ConnectSendObserver(ISendObserver observer) => new NoOpConnectHandle();

    private class NoOpConnectHandle : ConnectHandle
    {
        public void Disconnect() { }

        public void Dispose() { }
    }
    
    public static Uri GetTopicUrl()
        => new Uri($"queue:{typeof(TMessage).Name}");
    
}