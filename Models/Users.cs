using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Institute_FineArt1.Models
{
	public class Users
	{
		[Key]
		public int UserId { get; set; }

		[Required(ErrorMessage = "The name field is mandatory")]
        [RegularExpression("^[a-zA-Z\\s]{3,}$",
    ErrorMessage = "The name must contain only alphabets and spaces, and must be at least 3 characters long.")]

        public string Name { get; set; }

		[Required(ErrorMessage = "The email field is required.")]
		[RegularExpression("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$",
							ErrorMessage = "The email should be in the standard format: abc@example.com.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "The password field is required.")]
		[MinLength(8)]
		// [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
		//   ErrorMessage = "Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a digit, and a special character.")]
		public string Password { get; set; }
		[Required(ErrorMessage = "This field is mandatory")]
		[NotMapped]
		[Compare("Password", ErrorMessage = "Password is not matched")]
		public string? ConfirmPassword { get; set; }

		[Required]
        [RegularExpression(@"^03\d{2}-\d{7}$|^03\d{9}$",
    ErrorMessage = "Invalid contact number format. Must be either 0312-4567878 or 03267894561.")]

        public string ContactNumber { get; set; }

		public string Role { get; set; }
		public DateTime CreatedDate { get; set; }
		public string Status {  get; set; }
		[Required(ErrorMessage = "Please select profile, This field is mandatory")]
		public string Image { get; set; }
		public DateTime? LastLogin { get; set; }


        public Users(string name, string email, string password, string contactNumber, string role, DateTime createdDate, string status, string image)
		{
			Name = name;
			Email = email;
			Password = password;
			ContactNumber = contactNumber;
			Role = role;
			CreatedDate = createdDate;
			Status = status;
			Image = image;
		}
	}
}
