using System.ComponentModel.DataAnnotations;

public class TableReservationCreateVM
{
    [Required]
    public string FullName { get; set; }

    [Required, EmailAddress]
    public string EmailAddress { get; set; }

    [Required]
    public string MobileNo { get; set; }

    [Required]
    public DateTime ReservationDate { get; set; }

    [Required]
    public string ReservationTime { get; set; }

    [Required]
    public int NoOfPerson { get; set; }
}
