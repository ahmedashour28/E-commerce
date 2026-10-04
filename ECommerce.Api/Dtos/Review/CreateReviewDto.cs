using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Dtos.Review
{
    public class CreateReviewDto
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Comment must be between 3 and 1000 characters")]
        public string Comment { get; set; } = string.Empty;
    }
}