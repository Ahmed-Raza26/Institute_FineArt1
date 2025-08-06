using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Mail;
using System.Net;
using Institute_FineArt1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Institute_FineArt1.Controllers
{
    public class DashboardController : Controller
    {
        DbConnection connection = new DbConnection();
        public IActionResult UserDetails()
        {
            var userId = HttpContext.Session.GetInt32("staffId");
            if (userId == null)
            {
                TempData["Error"] = "You must log in first";
                return RedirectToAction("login");
            }
            else
            {
                var user = connection.Users.ToList();
                return View(user);
            }
        }
        public IActionResult viewart()
        {
            var userId = HttpContext.Session.GetInt32("staffId");
            if (userId == null)
            {
                TempData["Error"] = "You must log in first";
                return RedirectToAction("login");
            }
            else
            {


                var art = connection.StudentArt?.Include(r => r.User1).ToList();

                // Retrieve the list of selected artwork IDs from BestArts
                var selectedArtIds = connection.BestArt?.Select(b => b.ArtId).ToList();

                // Pass the list of selected art IDs to the view using ViewBag
                ViewBag.selectart = selectedArtIds;

                return View(art);
            }
            // Retrieve the list of paintings, including the associated user information
        }

        [HttpPost]
        public IActionResult viewart(int artId)
        {
            // Check if the painting exists before proceeding
            var painting = connection.StudentArt.Include(p => p.User1).FirstOrDefault(p => p.ArtId == artId);
            if (painting == null)
            {
                TempData["error"] = "Error: Painting not found.";
                return RedirectToAction("viewart");  // Redirect to the same action if painting not found
            }

            // Add to BestArts Table
            DateTime currentdate = DateTime.Now;
            BestArts topart = new BestArts(artId, "Available", currentdate);
            connection.BestArt.Add(topart);
            int result = connection.SaveChanges();

            if (result > 0)
            {
                // Fetch BestArt with Painting and User details
                var best = connection.BestArt
                    .Include(b => b.Painting)
                    .ThenInclude(p => p.User1) // Ensure User1 is included
                    .FirstOrDefault(b => b.ArtId == artId);

                // Retrieve the updated list of selected art IDs
                var selectedArtIds = connection.BestArt.Select(b => b.ArtId).ToList();

                // Pass the selected art IDs to the ViewBag for the view
                ViewBag.selectart = selectedArtIds;

                if (best != null && best.Painting != null && best.Painting.User1 != null)
                {
                    // Extract painting and user details
                    var email = best.Painting.User1.Email ?? "no-email@example.com";
                    var name = best.Painting.User1.Name ?? "Unknown Artist";
                    var image = best.Painting.ArtImage ?? "default-image.jpg";

                    // Send email notification
                    // Send Email
                    HomeController newmail = new HomeController();
                    newmail.SendEmail(email,
                        "Art Selection Notification",
                        $@"
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 20px;'>
    <div style='max-width: 600px; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); margin: auto;'>
        <h2 style='color: #333; text-align: center;'>🎨 Congratulations, Your Artwork Has Been Selected! 🏆</h2>
        <p style='color: #555; font-size: 16px; line-height: 1.6;'>Dear <strong>{name}</strong>,</p>
       <p style='color: #555; font-size: 16px; line-height: 1.6;'>We are thrilled to inform you that your artwork titled <strong>\""{best.Painting.ArtTitle}\""</strong> has been selected for our upcoming exhibition and is now available for sale.</p>
        
        <p style='color: #555; font-size: 16px; line-height: 1.6;'>As a part of our FineArt Exhibition, your artwork will be showcased to a wide audience and potential buyers.</p>
        <p style='text-align: center;'>
            <a href='your-website-link' style='display: inline-block; padding: 10px 20px; margin-top: 15px; color: #fff; background-color: #007bff; text-decoration: none; border-radius: 5px; font-size: 16px;'>View My Artwork</a>
        </p>
        <p style='color: #555; font-size: 16px; line-height: 1.6;'>If you have any questions, feel free to contact our support team.</p>
        <p style='color: #555; font-size: 16px; line-height: 1.6;'>Best regards,<br><strong>FineArt Team</strong></p>
        <div style='margin-top: 20px; font-size: 14px; color: #777; text-align: center;'>&copy; 2025 FineArt | All Rights Reserved.</div>
    </div>
</body>"
                    );

                    // Set success message and return to the view
                    TempData["success"] = "Successfully Selected";
                    return RedirectToAction("viewart");
                }
                else
                {
                    TempData["error1"] = "Error: Painting or User data is missing.";
                    return RedirectToAction("viewart");
                }
            }
            else
            {
                TempData["error2"] = "Something went wrong while saving the artwork.";
                return RedirectToAction("viewart");
            }
        }


        public IActionResult selectedart()
        {
            var userId = HttpContext.Session.GetInt32("staffId");
            if (userId == null)
            {
                TempData["Error"] = "You must log in first";
                return RedirectToAction("login");
            }
            else
            {

                var selectedarts = connection.BestArt.Include(b => b.Painting).ThenInclude(r => r.User1).ToList();
                return View(selectedarts);

            }
        }

        [HttpPost]
        public IActionResult selectedart(int Id, string Status)
        {
            var selectedarts = connection.BestArt.Find(Id);
            if (selectedarts != null)
            {
                selectedarts.Status = Status;
                connection.SaveChanges();
                return RedirectToAction("selectedart"); // Redirecting back to the selected art list
            }
            else
            {
                TempData["errormessage"] = "Something is wrong! Data not updated.";
                return RedirectToAction("selectedart"); // Redirect to same page
            }
        }

        public IActionResult Deleteselection(int Id)
        {
            var data = connection.BestArt.Find(Id);
            connection.BestArt.Remove(data);
            connection.SaveChanges();

            return RedirectToAction("selectedart");
        }

        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("staffId");
            if (userId == null)
            {
                TempData["Error"] = "You must log in first";
                return RedirectToAction("login");
            }

            var orders = connection.Orders.Include(o => o.topart).Include(r => r.topart.Painting.User1).ToList();

            // Total Orders
            ViewBag.TotalOrders = orders.Count();

            // Total Earnings (Handling nullable double for PlatformEarnings)
            ViewBag.TotalEarnings = orders.Sum(o => o.PlatformEarnings);

            // Pending Orders
            ViewBag.PendingOrders = orders.Count(o => o.OrderStatus == "Pending");

            // Completed Orders
            ViewBag.CompletedOrders = orders.Count(o => o.OrderStatus == "Completed");

            // Active Users: Count distinct users with non-null UserId
            ViewBag.ActiveUsers = orders
                .Where(o => o.topart?.Painting?.User1?.UserId != null) // Ensure that UserId exists
                .Select(o => o.topart.Painting.User1.UserId) // Select the UserId
                .Distinct() // Get unique users
                .Count(); // Count distinct active users

            // Total Sales Calculation (handling null prices)
            //ViewBag.TotalSales = orders.Sum(o => o.topart?.Painting?.Price ?? 0);

            // Orders in the last 7 days
            ViewBag.WeeklyOrders = orders.Count(o => o.OrderDate >= DateTime.Now.AddDays(-7));

            return View(orders);
        }
        public IActionResult orderdetails()
        {
            var userId = HttpContext.Session.GetInt32("staffId");
            if (userId == null)
            {
                TempData["Error"] = "You must log in first";
                return RedirectToAction("login");
            }
            else
            {
                // Include Painting and User1 data to get the artwork title and image
                var orders = connection.Orders
                    .Include(o => o.topart)
                    .ThenInclude(t => t.Painting)  // Include Painting details (Title, Image)
                    .ThenInclude(p => p.User1) // You can include the User1 if necessary
                    .ToList();

                return View(orders);
            }
        }
        [HttpPost]
        public IActionResult orderdetails(int OrderId, string Status)
        {
            var selectedarts = connection.Orders.Find(OrderId);
            if (selectedarts != null)
            {
                selectedarts.OrderStatus = Status;
                connection.SaveChanges();
                if (selectedarts != null && selectedarts.OrderStatus == "Cancelled")
                {

                    HomeController cancelmail = new HomeController();
                    cancelmail.SendEmail(
        selectedarts.Email,
        "Order Cancellation",
        $@"
<h2>Order Cancellation</h2>
<p>Dear {selectedarts.FullName},</p>
<p>We are sorry to inform you that your order on <strong>FineArt</strong> has been cancelled. We apologize for any inconvenience this may have caused. Below are the details of your order:</p>
<p><strong>Order Number:</strong> #{selectedarts.OrderId}</p>
<p><strong>Order Date:</strong> {selectedarts.OrderDate}</p>
<p><strong>Artwork Title:</strong> {selectedarts.topart?.Painting?.ArtTitle}</p>
<p><strong>Price:</strong> {selectedarts.Price}</p>
<p><strong>Delivery Address:</strong> {selectedarts.Address}</p>
<p>If you have any questions or need assistance, please don't hesitate to reach out to us.</p>
<p>We hope to serve you again in the future.</p>
<p>Best regards,<br>The FineArt Team</p>"
    );
                }

                return RedirectToAction("orderdetails"); // Redirecting back to the selected art list
            }
            else
            {
                TempData["errormessage"] = "Something is wrong! Data not updated.";
                return RedirectToAction("orderdetails"); // Redirect to same page
            }
        }



        public IActionResult register()
        {
            var userId = HttpContext.Session.GetInt32("staffId");

            if (userId == null)
            {
                TempData["Error"] = "You must log in first.";
                return RedirectToAction("login");
            }
            else
            {

                return View();
            }

        }
		[HttpPost]
		public IActionResult register(string Name, string Email, string ContactNumber, string Role, string Password, string ConfirmPassword, IFormFile Image)
		{
			DateTime currentDateTime = DateTime.Now;
			var existingUser = connection.Users.FirstOrDefault(u => u.Email == Email);
			if (existingUser != null)
			{
				TempData["exist1"] = "This user already registered!";
				return View(); // Yahan aap error message show karne ke liye View par wapas bhej rahe hain
			}

			// Check if the image is null
			if (Image == null || Image.Length == 0)
			{
				TempData["imageError"] = "Please upload an image.";
				return View(); // You can display this error message on the view
			}

			string imagename = Guid.NewGuid() + "_" + Image.FileName;
			Users newuser = new Users(Name, Email, Password, ContactNumber, Role, currentDateTime, "Active", imagename);
			var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/asset/StaffImages", imagename);

			using (FileStream newstream = new FileStream(path, FileMode.Create))
			{
				Image.CopyTo(newstream);
			}

			connection.Users.Add(newuser);
			connection.SaveChanges();

			if (ModelState.IsValid)
			{
				ModelState.Clear();
			}

			HomeController newemail = new HomeController();
			newemail.SendEmail(Email, $"Welcome to FineArt – Where Creativity Meets Opportunity!",
				$"<p>Hi {Name},</p>\r\n<p>Welcome to FineArt! 🎨</p>\r\n<p>We’re thrilled to have you join our vibrant community of art enthusiasts and creators. FineArt is a space where students showcase their creativity, the best artwork shines in exhibitions, and art lovers discover pieces they’ll cherish forever.</p>\r\n<p><strong>Here’s how you can get started:</strong></p>\r\n<ul>\r\n  <li><strong>For Artists:</strong> Upload your paintings and share your talent with the world. Who knows? Your masterpiece might be the next to feature in an exhibition!</li>\r\n  <li><strong>For Buyers:</strong> Explore unique creations from talented students and bring home a piece that inspires you.</li>\r\n</ul>\r\n<p>At FineArt, we believe every stroke tells a story, and we’re here to help share yours.</p>\r\n<p>If you have any questions or need assistance, feel free to contact us.</p>\r\n<p>Let’s make art together!</p>\r\n<p>Warm regards,<br>The FineArt Team</p>");

			TempData["message"] = "Account has been Created Successfully!";
			return View();
		}

		public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult login(string Email, string Password)
        {
            var data = connection.Users.Where(m => m.Email == Email && m.Password == Password).FirstOrDefault();
            if (data != null && data.Role != "Student")
            {
                data.LastLogin = DateTime.Now;
                connection.SaveChanges();
                HttpContext.Session.SetString("staffimage", data.Image);
                HttpContext.Session.SetString("staffuser", data.Name);
                HttpContext.Session.SetString("staffemail", data.Email);
                HttpContext.Session.SetString("staffphone", data.ContactNumber);
                HttpContext.Session.SetString("staffrole", data.Role);
                HttpContext.Session.SetInt32("staffId", data.UserId);



                return RedirectToAction("dashboard", "Dashboard");


            }
            else
            {
                TempData["message"] = "Invalid Login Credential";
                return View();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
