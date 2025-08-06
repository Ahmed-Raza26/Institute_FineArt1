using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Institute_FineArt1.Models
{
    public class BestArts
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key
        [ForeignKey("Painting")]
        public int ArtId { get; set; }

        // Navigation Property
        public Painting Painting { get; set; }
      
        public string Status { get; set; }
        public DateTime UploadDate { get; set; }

        public BestArts() { }
		public BestArts(int artId, string status, DateTime uploadDate)
		{
			ArtId = artId;
			Status = status;
			UploadDate = uploadDate;
		}
	}
}
