using System.ComponentModel.DataAnnotations;

public class Address
{
    [Key]
    public Guid Id { get; set; }
    public Guid? RestaurantId { get; set; }
    public Guid? UserId { get; set; }
    public string AddressLine { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public User User { get; set; }
}