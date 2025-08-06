using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Institute_FineArt1.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } // Foreign key for the user
        public Users User { get; set; }

        [ForeignKey("BPainting")]
        public int PaintingId { get; set; } // Foreign key for the painting
        public BestArts BPainting { get; set; }

        public string Comment { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }

        // Parameterless constructor (required for EF)
        public Review() { }

        // Parameterized constructor for convenience
        public Review(int userId, int paintingId, string comment, DateTime reviewDate)
        {
            UserId = userId;
            PaintingId = paintingId;
            Comment = comment;
            ReviewDate = reviewDate;
        }
    }
}
