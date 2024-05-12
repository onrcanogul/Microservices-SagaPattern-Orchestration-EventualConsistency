using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Contexts;
using Shared.OrderEvents;

namespace Order.API.Consumers
{
    public class OrderFailedEventConsumer(OrderAPIDbContext _context) : IConsumer<OrderFailedEvent>
    {
        public async Task Consume(ConsumeContext<OrderFailedEvent> context)
        {
           Entities.Order? order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == context.Message.OrderId);
            if (order != null)
            {
                order.OrderStatu = Enums.OrderStatus.Failed;
                await _context.SaveChangesAsync();
            }
            else throw new NullReferenceException();
        }
    }
}
