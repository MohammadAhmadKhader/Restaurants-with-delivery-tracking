using MassTransit;
using Shared.Kafka;
namespace Restaurants.Sagas;

public class RestaurantCreateHandler : 
    IConsumer<RestaurantCreateCommand>,
    IConsumer<OwnerCreateCommand>
{
    private readonly ILogger<RestaurantCreateHandler> _logger;
    public RestaurantCreateHandler(ILogger<RestaurantCreateHandler> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<RestaurantCreateCommand> ctx)
    {
        _logger.LogInformation("Received Restarant create command {@RestaurantCreateCommand}", ctx.Message);
        await Task.CompletedTask;
    }

    public async Task Consume(ConsumeContext<OwnerCreateCommand> ctx)
    {
        _logger.LogInformation("Received Owner created command {@OwnerCreateCommand}", ctx.Message);
        await Task.CompletedTask;
    }
}