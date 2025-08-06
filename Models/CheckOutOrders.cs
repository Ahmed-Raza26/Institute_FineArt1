using Institute_FineArt1.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class CheckOutOrders
{
    // Database Fields
    [Key]
    public int OrderId { get; set; }
    [ForeignKey("topart")]
    public int TopArtId { get; set; }
    public BestArts topart { get; set; }
    [Required]
    public string FullName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Phone { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string Price { get; set; }
    [Required]
    public string PaymentMethod { get; set; }
    [Required]
    public double SellerEarnings { get; set; }
    [Required]
    public double PlatformEarnings { get; set; }
    [Required]
    public string OrderStatus { get; set; }
    [Required]
    [NotMapped]
    public string PaymentStatus { get; set; }
    [Required]
    public DateTime OrderDate { get; set; }

  
    // Parameterized constructor
    public CheckOutOrders(int topArtId, string fullName, string email, string phone, string address,
        string price, string paymentMethod, double sellerEarnings, double platformEarnings,
        string orderStatus, DateTime orderDate)
    {
        TopArtId = topArtId;
        FullName = fullName;
        Email = email;
        Phone = phone;
        Address = address;
        Price = price;
        PaymentMethod = paymentMethod;
        SellerEarnings = sellerEarnings;
        PlatformEarnings = platformEarnings;
        OrderStatus = orderStatus;
        OrderDate = orderDate;
    }
}
