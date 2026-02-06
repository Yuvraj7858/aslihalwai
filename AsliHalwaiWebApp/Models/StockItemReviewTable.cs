using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsliHalwaiWebApp.Models
{
    [Table("StockItemReviewTable")]
    public class StockItemReviewTable
    {
        [Key]
        public int ItemReviewID { get; set; }

        [Required]
        public int StockItemID { get; set; }

        [Required]
        public int ReviewBy_UserID { get; set; }

        [Required]
        public DateTime ReviewDateTime { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string ReviewDetails { get; set; }

        // ================= NAVIGATION (OPTIONAL BUT BEST) =================

        [ForeignKey("StockItemID")]
        public StockItemTable StockItem { get; set; }

        [ForeignKey("ReviewBy_UserID")]
        public UserTable ReviewByUser { get; set; }
    }
}
