using Order.API.Enums;

namespace Order.API.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid BuyerId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public OrderStatus OrderStatu { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
