namespace CW_7_s2991.Models.DTOs;

public class ClientTripGetDTO
{
    public int IdTrip { get; set; }
    public int RegisteredAt { get; set; }
    public int? PaymentDay { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
}


