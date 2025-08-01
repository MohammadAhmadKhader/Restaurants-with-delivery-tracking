using MassTransit;

namespace Shared.Kafka;

public class MockTopicProducer<T> : ITopicProducer<T>
        where T : class
    {
        public Task Produce(T message, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Produce(T message, IPipe<KafkaSendContext<T>> pipe, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Produce(object message, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Produce(object message, IPipe<KafkaSendContext<T>> pipe, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer) => new NoOpConnectHandle();
        public void Dispose() { }

        private class NoOpConnectHandle : ConnectHandle
        {
            public void Disconnect() { }

            public void Dispose() { }

            public bool IsConnected => false;
        }
    }