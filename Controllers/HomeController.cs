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
    public class HomeController : Controller
    {
        readonly DbConnection connection = new DbConnection();

        public IActionResult Index()
        {
            var studentpainting = connection.StudentArt.Include(b => b.User1).ToList();



            var best = connection.BestArt.Include(b => b.Painting).ToList();



            // Pass reviews to the view
            ViewBag.bestart = best;

            // If no arts are found, pass a default message
            if (!best.Any())
            {
                ViewBag.Message = "No Art Found.";
            }


            return View(studentpainting);
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Team()
        {
            return View();
        }
        public IActionResult TeamDetails()
        {
            return View();
        }
        public IActionResult Location()
        {
            return View();
        }
        public IActionResult OpeningHr()
        {
            return View();
        }


        public IActionResult Event()
        {
            return View();
        }
        public IActionResult EventDetails()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        // Credential Section
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(string Name, string Email, string ContactNumber, string Password, string ConfirmPassword, IFormFile Image)
        {
            DateTime currentDateTime = DateTime.Now;
            var existingUser = connection.Users.FirstOrDefault(u => u.Email == Email);
            if (existingUser != null)
            {
                TempData["exist2"] = "This user already exists!";
                return View(); // Yahan aap error message show karne ke liye View par wapas bhej rahe hain
            }
            //Console.WriteLine("{0}\n{1}\n{2}", name, email, password);
            string imagename = Guid.NewGuid() + "_" + Image.FileName;
            Users newuser = new Users(Name, Email, Password, ContactNumber, "Student", currentDateTime, "Active", imagename);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserImages", imagename);
            // var path = Path.Combine("C:\\Users\\AHMED\\Desktop\\Ahmed\\DotNet\\Lecture2\\Lecture#2\\wwwroot/Images", imagename);
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
            SendEmail(Email, $"Welcome to FineArt – Where Creativity Meets Opportunity!",
                $"<p>Hi {Name},</p>\r\n<p>Welcome to FineArt! 🎨</p>\r\n<p>We’re thrilled to have you join our vibrant community of art enthusiasts and creators. FineArt is a space where students showcase their creativity, the best artwork shines in exhibitions, and art lovers discover pieces they’ll cherish forever.</p>\r\n<p><strong>Here’s how you can get started:</strong></p>\r\n<ul>\r\n  <li><strong>For Artists:</strong> Upload your paintings and share your talent with the world. Who knows? Your masterpiece might be the next to feature in an exhibition!</li>\r\n  <li><strong>For Buyers:</strong> Explore unique creations from talented students and bring home a piece that inspires you.</li>\r\n</ul>\r\n<p>At FineArt, we believe every stroke tells a story, and we’re here to help share yours.</p>\r\n<p>If you have any questions or need assistance, feel free to contact us.</p>\r\n<p>Let’s make art together!</p>\r\n<p>Warm regards,<br>The FineArt Team</p>");

            TempData["message"] = "Account has been Created Successfully!";

            return View();


        }
		public void SendEmail(string to, string subject, string body)
		{
			MailMessage newmail = new MailMessage();
			newmail.From = new MailAddress("ahmedahmedraza896@gmail.com");
			newmail.To.Add(to);
			newmail.Subject = subject;
			newmail.Body = body;
			newmail.IsBodyHtml = true;

			using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
			{
				smtp.Credentials = new NetworkCredential("ahmedahmedraza896@gmail.com", "ugstoamnhxuzurbu"); // Your app password
				smtp.EnableSsl = true;

				try
				{
					smtp.Send(newmail);
				}
				catch (SmtpException ex)
				{
					// Log or throw the error with full details
					throw new Exception("Email sending failed: " + ex.Message, ex);
				}
			}
		}

		public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            var data = connection.Users.Where(m => m.Email == Email && m.Password == Password).FirstOrDefault();
            if (data != null && data.Role == "Student")
            {
                data.LastLogin = DateTime.Now;
                connection.SaveChanges();

                HttpContext.Session.SetString("image", data.Image);
                HttpContext.Session.SetString("user", data.Name);
                HttpContext.Session.SetString("email", data.Email);
                HttpContext.Session.SetString("phone", data.ContactNumber);
                HttpContext.Session.SetString("role", data.Role);
                HttpContext.Session.SetInt32("UserId", data.UserId);



                return RedirectToAction("Index", "Home");


            }
            else
            {
                TempData["message"] = "Invalid Login Credential";
                return View();
            }
        }
        public IActionResult findaccount()
        {
            return View();
        }
        [HttpPost]
        public IActionResult findaccount(string Email)

        {
            var dbemail = connection.Users.Where(x => x.Email == Email).FirstOrDefault();
            if (dbemail != null)
            {
                Random random = new Random();
                int otp = random.Next(1265, 9876);
                // HttpContext.Session.SetInt32("otp", otp);
                var newcookie = new CookieOptions
                {
                    Expires = DateTime.Now.AddSeconds(50)
                };
                Response.Cookies.Append("otp", otp.ToString(), newcookie);
                HttpContext.Session.SetInt32("Id", dbemail.UserId);
                SendEmail(Email, "Verification msg", $"Here is verification code : {otp}");
                return RedirectToAction("OTP");
            }
            else
            {
                TempData["emailmsg"] = "Invalid Email";
                return View();
            }
        }

        public IActionResult OTP()
        {
            return View();
        }
        [HttpPost]
        public IActionResult OTP(string num)
        {
            //if (HttpContext.Session.GetInt32("otp") == num)

            if (Request.Cookies["otp"] == num)
            {
                ModelState.Clear();
                return RedirectToAction("changepassword");
            }
            else
            {
                TempData["otpmsg"] = "Invalid OTP";
                return View();
            }


        }
        public IActionResult changepassword()
        {
            var data = connection.Users.Where(x => x.UserId == HttpContext.Session.GetInt32("Id")).FirstOrDefault();
            return View(data);
        }
        [HttpPost]
        public IActionResult changepassword(int Id, string Password)
        {
            var data = connection.Users.Where(x => x.UserId == Id).FirstOrDefault();
            if (data != null)
            {
                data.Password = Password;
                connection.SaveChanges();
                TempData["passwordupdate"] = "Password Updated Successfully";
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // student painting section
        public IActionResult StudentUploads()
        // Check if user is logged in
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "You must log in to post a review.";
                return RedirectToAction("Login");
            }
            else
            {

                return View();
            }
        }
        [HttpPost]
        public IActionResult StudentUploads(IFormFile ArtImage, string ArtTitle, string ArtPrice, string ArtDetail)

        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");

            int UserId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            DateTime currentDateTime = DateTime.Now;
            string Artname = Guid.NewGuid() + "_" + ArtImage.FileName;
            Painting NewArt = new Painting(UserId, Artname, ArtTitle, ArtPrice, ArtDetail, currentDateTime, currentDateTime);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/StudentPainting", Artname);
            // var path = Path.Combine("C:\\Users\\AHMED\\Desktop\\Ahmed\\DotNet\\Lecture2\\Lecture#2\\wwwroot/Images", imagename);
            using (FileStream newstream = new FileStream(path, FileMode.Create))
            {
                ArtImage.CopyTo(newstream);
            }
            connection.StudentArt.Add(NewArt);
            connection.SaveChanges();
            if (ModelState.IsValid)
            {
                ModelState.Clear();

            }


            TempData["ArtUpload"] = "Art Uploaded Successfully";


            return View();
        }

        // GET: Display the student's image gallery
        public IActionResult MyArt()

        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "You must log in to post a review.";
                return RedirectToAction("Login");
            }
            else
            {

                int loggedInStudentId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId")); // Implement a method to get logged-in student ID

                var arts = connection.StudentArt.Where(a => a.UserId == loggedInStudentId).ToList();



                return View(arts);
            }


        }


        public IActionResult ModifyArt(int Id)

        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "You must log in to post a review.";
                return RedirectToAction("Login");
            }
            else
            {

                var data = connection.StudentArt.Where(m => m.ArtId == Id).FirstOrDefault();

                return View(data);
            }
        }
        [HttpPost]
        public IActionResult ModifyArt(int Id, IFormFile ArtImage, string ArtTitle,string ArtPrice, string ArtDetail)
        {
            var dbdata = connection.StudentArt.Where(m => m.ArtId == Id).FirstOrDefault();

            dbdata.ArtImage = Guid.NewGuid() + "_" + ArtImage.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/StudentPainting", dbdata.ArtImage);
            using (FileStream newstream = new FileStream(path, FileMode.Create))
            {
                ArtImage.CopyTo(newstream);
            }
            dbdata.ArtTitle = ArtTitle;
            dbdata.ArtPrice = ArtPrice;
            dbdata.ArtDetail = ArtDetail;
            dbdata.ModifiedDate = DateTime.Now;

            connection.SaveChanges();

            return RedirectToAction("MyArt");

        }
        // POST: Delete a painting
        public IActionResult delete(int id)
        {
            //var row = connection.StudentRegistration.Where(model => model.id == id).FirstOrDefault();
            var row = connection.StudentArt.Find(id);
            connection.StudentArt.Remove(row);
            //   var path = Path.Combine("C:\\Users\\AHMED\\Desktop\\Ahmed\\DotNet\\Lecture2\\Lecture#2\\wwwroot/StudentPainting", row.ArtImage);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/StudentPainting", row.ArtImage);
            System.IO.File.Delete(path);
            var a = connection.SaveChanges();
            if (a > 0)
            {
                TempData["deleted"] = $"Art has been Deleted Successfully";
            }
            return RedirectToAction("MyArt");

        }


        // Best Art Selected by Teacher
        public IActionResult TopArtUpload()
        {

            return View();

        }

        //public IActionResult TopArtUpload(int artId,string artprice)
        //{
        //    DateTime currentdate = new DateTime();
        //    BestArts topart = new BestArts(artId,artprice,currentdate);
        //    connection.BestArt.Add(topart);
        //    connection.SaveChanges();
        //    return View();

        //}
        public IActionResult Shop()
        {
            var bestArts = connection.BestArt
        .Include(b => b.Painting).ToList();

            return View(bestArts);


        }

        public IActionResult ShopDetails(int id)
        {
            // Fetch the BestArt record with associated Painting and User1
            var data = connection.BestArt
                .Where(m => m.Id == id)
                .Include(b => b.Painting)
                .ThenInclude(p => p.User1) // Ensure User1 is loaded if it exists
                .FirstOrDefault();

            if (data != null)
            {
                // Fetch reviews related to the painting
                var reviews = connection.Reviews
                    .Where(r => r.PaintingId == id)
                    .Include(r => r.User) // Include user data for reviews
                    .ToList();

                // Pass reviews to the view
                ViewBag.Reviews = reviews;

                // If no reviews are found, pass a default message
                if (!reviews.Any())
                {
                    ViewBag.Message = "No reviews found for this painting.";
                }

                return View(data); // Pass the BestArt object to the view
            }

            // If no data found, redirect to the Shop page
            return RedirectToAction("Shop", "Home");
        }

        [HttpPost]
        public IActionResult ShopDetails(int id, string comment)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "You must log in to post a review.";
                return RedirectToAction("Login");
            }

            // Save the new review
            var newReview = new Review
            {
                UserId = userId.Value,
                PaintingId = id,
                Comment = comment,
                ReviewDate = DateTime.Now
            };
            connection.Reviews.Add(newReview);
            connection.SaveChanges();
            ModelState.Clear();

            // Fetch updated BestArt record and reviews
            var data = connection.BestArt
                .Where(m => m.Id == id)
                .Include(b => b.Painting)
                .ThenInclude(p => p.User1)
                .FirstOrDefault();

            if (data != null)
            {
                var reviews = connection.Reviews
                    .Where(r => r.PaintingId == id)
                    .Include(r => r.User)
                    .ToList();

                ViewBag.Reviews = reviews;

                return View(data); // Return the updated BestArt object with reviews
            }

            return RedirectToAction("Shop", "Home");
        }


        // Checkout Order Section
        public IActionResult OrderCheckout(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["buyError"] = "You must be log in to buy Art.";
                return RedirectToAction("Login");
            }
            else
            {

                var data = connection.BestArt
                    .Where(m => m.Id == id)
                    .Include(b => b.Painting)
                    .Include(b => b.Painting.User1)
                    .FirstOrDefault();
                if (data != null)
                {
                    var price = Convert.ToInt32(data.Painting.ArtPrice);
                    ViewBag.gst = price * 0.05m;
                    ViewBag.delivery = 100;
                    ViewBag.total = price + ViewBag.delivery + ViewBag.gst;
                }
                return View(data);
            }
        }
        [HttpPost]
        public IActionResult OrderCheckOut(int TopArtId, string FullName, string Email, string Phone, string Address, string price, string PaymentMethod)
        {
            DateTime orderdate = DateTime.Now;
            double sellerearning = Convert.ToDouble(price) * 0.8;
            double platformearning = Convert.ToDouble(price) * 0.2;
            string orderstatus = "In Process";
           

            try
            {
                // Update session with user input
                HttpContext.Session.SetString("user", FullName);
                HttpContext.Session.SetString("email", Email);
                HttpContext.Session.SetString("phone", Phone);

                // Create a new order
                CheckOutOrders newOrder = new CheckOutOrders
                (
                    TopArtId, FullName, Email, Phone, Address, price, PaymentMethod, sellerearning, platformearning, orderstatus, orderdate
                );

                connection.Orders.Add(newOrder);
                var b = connection.SaveChanges();

                if (b > 0)
                {
                    // Find the corresponding artwork
                    var find = connection.BestArt
                        .Include(b => b.Painting)
                        .ThenInclude(p => p.User1) // Ensure navigation property is included
                        .FirstOrDefault(m => m.Id == TopArtId);

                    if (find != null && find.Painting != null && find.Painting.User1 != null)
                    {
                        // Find the newly created order by OrderId (not TopArtId)
                        var orderdata = connection.Orders
                            .Where(o => o.TopArtId == TopArtId && o.OrderDate == orderdate)
                            .FirstOrDefault(); // Use FirstOrDefault to avoid NullReferenceException

                        if (orderdata != null)
                        {
                            // Send the email to the user
                            SendEmail(
                                Email,
                                "Order Placement",
                                $@"
                        <h2>Order Confirmation</h2>
                        <p>Dear {find.Painting.User1.Name},</p>
                        <p>Thank you for shopping with us on <strong>FineArt</strong>! Below are the details of your order:</p>
                        <p><strong>Order Number:</strong> #{orderdata.OrderId}</p>
                        <p><strong>Order Date:</strong> {orderdate}</p>
                        <p><strong>Artwork Title:</strong>{find.Painting.ArtTitle}</p>
                        <p><strong>Price:</strong> {price}</p>
                        <p><strong>Delivery Address:</strong> {Address}</p>
                        <p>Best regards,<br>The FineArt Team</p>"
                            );

                            // Send the email to the admin
                            string adminEmail = "ahmedahmedraza896@gmail.com";  // Admin's email address
                            SendEmail(
                                adminEmail,  // Send to admin instead of the user
                                "New Order Placed",  // Email subject
                                $@"
                        <h2>New Order Placed</h2>
                        <p>Dear Admin,</p>
                        <p>A new order has been placed on <strong>FineArt</strong>. Below are the details of the order:</p>
                        <p><strong>Order Number:</strong> #{orderdata.OrderId}</p>
                        <p><strong>Order Date:</strong> {orderdate}</p>
                        <p><strong>Artwork Title:</strong> {find.Painting.ArtTitle}</p>
                        <p><strong>Price:</strong> {price}</p>
                        <p><strong>Delivery Address:</strong> {Address}</p>
                        <p>Best regards,<br>The FineArt Team</p>"
                            );

                            // Store success message in TempData
                            TempData["orderplaced"] = "Your order has been placed successfully!";
                        }
                        else
                        {
                            TempData["orderError"] = "Could not find the order after placing it.";
                        }
                    }
                    else
                    {
                        TempData["orderError"] = "Could not process the order due to missing artwork data.";
                    }
                }
                else
                {
                    TempData["orderError"] = "An error occurred while saving the order.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception and show an error message
                TempData["orderError"] = "An error occurred while processing your order. Please try again.";
            }

            // Redirect to the Shop page after placing the order
            return RedirectToAction("Shop", "Home");
        }

    }
}
