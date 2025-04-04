using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INF27523_TP1_ML_SS.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public int Quantity { get; set; } = 1; 
        public decimal Price { get; set; }  

        public string Title { get; set; } = null!;
        public string Image { get; set; } = null!;
    }
}
