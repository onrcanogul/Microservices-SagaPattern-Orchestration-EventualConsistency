using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.OrderEvents;
using Shared.Settings;
using Shared.StockEvents;
using Stock.API.Contexts;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer(StockAPIDbContext _context, ISendEndpointProvider sendEndpointProvider) : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResults = new();

            foreach(var orderItem in context.Message.OrderItems)
                stockResults.Add(await _context.Stocks.AnyAsync(s => s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count));

            var sendEndPoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));
            if(stockResults.TrueForAll(s => s.Equals(true)))
            {
                foreach(var orderItem in context.Message.OrderItems)
                {
                    Entities.Stock stock =  await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == orderItem.ProductId);
                    stock.Count -= orderItem.Count;
                    await _context.SaveChangesAsync();
                }
                StockReservedEvent stockReservedEvent = new(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems,
                };
                await sendEndPoint.Send(stockReservedEvent);
            }
            else
            {

                StockNotReservedEvent stockNotReservedEvent = new(context.Message.CorrelationId)
                {
                    Message = "stock-not-reserved"
                };
                await sendEndPoint.Send(stockNotReservedEvent);
            }
        }
    }
}
