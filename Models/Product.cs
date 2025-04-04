using System.ComponentModel.DataAnnotations;

namespace INF27523_TP1_ML_SS.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Image { get; set; } = null!;

        public Rating? Rating { get; set; } 

        [Required]
        public string Category { get; set; } = null!;

        public int SellerId { get; set; }
        public User? Seller { get; set; }
    }

    public class Rating
    {
        public double Rate { get; set; }
        public int Count { get; set; }
    }
}
