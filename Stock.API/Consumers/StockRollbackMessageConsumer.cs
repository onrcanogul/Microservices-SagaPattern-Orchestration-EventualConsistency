using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Messages;
using Stock.API.Contexts;

namespace Stock.API.Consumers
{
    public class StockRollbackMessageConsumer(StockAPIDbContext _context) : IConsumer<StockRollbackMessage>
    {
        public async Task Consume(ConsumeContext<StockRollbackMessage> context)
        {
            foreach(var orderItem in context.Message.OrderItems)
            {
                Entities.Stock? stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == orderItem.ProductId);
                if (stock != null)
                {
                    stock.Count += orderItem.Count;
                    await _context.SaveChangesAsync();
                }
                else throw new NullReferenceException();
            }
        }
    }
}
