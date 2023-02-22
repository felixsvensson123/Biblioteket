using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grupp15.Shared.Models
{
    public class BorrowingModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime BorrowingDate { get; set; }
        public DateTime ReturnDate { get; set; }

        //Relations
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public ProductBase? Product { get; set; }

        [ForeignKey(nameof(User))]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
