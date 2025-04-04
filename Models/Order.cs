namespace INF27523_TP1_ML_SS.Models
{
    public class Order
    {
        public int Id { get; set; } 
        public int UserId { get; set; } 
        public User User { get; set; } = null!;

        public DateTime OrderDate { get; set; } 
        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
