using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Contexts;
using Shared.OrderEvents;

namespace Order.API.Consumers
{
    public class OrderCompletedEventConsumer(OrderAPIDbContext _context) : IConsumer<OrderCompletedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
        {
          Entities.Order? order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == Guid.Parse(context.Message.OrderId));
            if (order != null)
            {
                order.OrderStatu = Enums.OrderStatus.Completed;
                await _context.SaveChangesAsync();
            }
            else throw new NullReferenceException();
        }
    }
}
