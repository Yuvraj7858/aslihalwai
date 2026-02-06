using AsliHalwaiWebApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModel
{
    public class ItemDetailVM
    {
        public ItemDetailVM()
        {
            Ingredients = new List<StockItemIngredientVM>();
            Reviews = new List<ItemReviewVM>();
            RelatedItems = new List<ItemVM>();

            // ⭐ Rating Dropdown (OLD tutorial SAME)
            RatingList = new List<RatingVM>
            {
                new RatingVM { Id = "1", Type = "Worst" },
                new RatingVM { Id = "2", Type = "Bad" },
                new RatingVM { Id = "3", Type = "Neutral" },
                new RatingVM { Id = "4", Type = "Good" },
                new RatingVM { Id = "5", Type = "Excellent" }
            };
        }

        // ================= PRODUCT =================
        public ItemVM Item { get; set; }

        // ================= DISPLAY =================
        public List<StockItemIngredientVM> Ingredients { get; set; }
        public List<ItemReviewVM> Reviews { get; set; }
        public List<ItemVM> RelatedItems { get; set; }

        // ================= ADD REVIEW (FORM) =================
        public int StockItemID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Rating Required")]
        [Display(Name = "Rating")]
        public string Rating { get; set; }

        public List<RatingVM> RatingList { get; set; }

        [Required(ErrorMessage = "Review is required")]
        public string ReviewDetails { get; set; }
    }

    // ⭐ Rating Dropdown Helper
    public class RatingVM
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }
}
