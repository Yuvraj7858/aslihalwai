using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.Models
{
    public class TableReservationVM
    {
        // ===== Primary Key (for edit / process) =====
        public int TableReservationId { get; set; }

        // ===== Customer Info =====
        [Required]
        [StringLength(250)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(300)]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(14)]
        public string MobileNo { get; set; } = string.Empty;

        // ===== Reservation Info =====
        [Required]
        [DataType(DataType.Date)]
        public DateTime? ReservationDate { get; set; }

        [Required]
        public string ReservationTime { get; set; } = string.Empty;

        [Required]
        public int? NoOfPerson { get; set; }

        // ===== Admin / Process Info =====
        public int ReservationStatudId { get; set; }   // spelling same as Model
        public string? Description { get; set; }

        // ===== Display Only (List View) =====
        public string ReservationStatus { get; set; } = string.Empty;
        public string BookingUserName { get; set; } = string.Empty;
        public string ProcessByUser { get; set; } = string.Empty;
        public DateTime ReservationDateTime { get; set; }

        // ===== List =====
        public List<TableReservationVM> ReservationList { get; set; }
            = new List<TableReservationVM>();
    }
}
