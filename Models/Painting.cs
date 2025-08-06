using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Institute_FineArt1.Models
{
    public class Painting
    {
        [Key]
        public int ArtId { get; set; }
        [ForeignKey("User1")]
        public int UserId { get; set; }
        public Users? User1 { get; set; } 
        [Required]
        public string ArtImage { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z\\s]{5,}$",
    ErrorMessage = "The Title must contain only alphabets and spaces, and must be at least 5 characters long.")]
        public string ArtTitle { get; set; }
        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$",
    ErrorMessage = "The price must be a valid number with up to 2 decimal places.")]

        public string ArtPrice { get; set; }
        [Required]
        [RegularExpression(@"^.{20,}$",
    ErrorMessage = "The details must be at least 20 characters long.")]

        public string ArtDetail { get; set; }
        public DateTime UploadedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

		public Painting(int userId, string artImage, string artTitle, string artPrice, string artDetail, DateTime uploadedDate, DateTime modifiedDate)
		{
			UserId = userId;
			ArtImage = artImage;
			ArtTitle = artTitle;
			ArtPrice = artPrice;
			ArtDetail = artDetail;
			UploadedDate = uploadedDate;
			ModifiedDate = modifiedDate;
		}
	}
}
