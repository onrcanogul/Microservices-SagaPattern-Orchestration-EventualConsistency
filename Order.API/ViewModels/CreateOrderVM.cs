namespace Order.API.ViewModels
{
    public class CreateOrderVM
    {
        public string BuyerId  { get; set; }
        public List<OrderItemVM> OrderItems { get; set; }

    }

    public class OrderItemVM
    {
        public string ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }  
    }
}
