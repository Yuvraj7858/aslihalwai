using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsliHalwaiWebApp.Models
{
    [Table("OrderPaymentTable")]
    public class OrderPaymentTable
    {
        [Key]
        public int OrderPaymentId { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderPaymentStatus { get; set; }
    }
}
