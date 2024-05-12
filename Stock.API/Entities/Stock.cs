namespace Stock.API.Entities
{
    public class Stock
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Count { get; set; }
    }
}
