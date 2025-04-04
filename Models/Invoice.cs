
namespace INF27523_TP1_ML_SS.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public DateTime InvoiceDate { get; set; }
        public decimal Amount { get; set; }
    }
}
