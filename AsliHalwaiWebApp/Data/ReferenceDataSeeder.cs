using AsliHalwaiWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Data;

/// <summary>
/// Inserts the lookup rows the application assumes when its hard-coded IDs
/// (for example UserStatusId = 2 for "Active") are used.
///
/// Seeding only happens when a lookup table is completely empty. Imported
/// production data is therefore never modified or overwritten.
/// </summary>
public static class ReferenceDataSeeder
{
    public static IReadOnlyList<string> Seed(AppDbContext db)
    {
        var seeded = new List<string>();

        if (!db.UserTypeTables.Any())
        {
            db.UserTypeTables.AddRange(
                new UserTypeTable { UserTypeId = 1, UserType = "Admin" },
                new UserTypeTable { UserTypeId = 2, UserType = "Staff" },
                new UserTypeTable { UserTypeId = 3, UserType = "Customer" });
            seeded.Add("UserTypeTable");
        }

        if (!db.UserStatusTables.Any())
        {
            db.UserStatusTables.AddRange(
                new UserStatusTable { UserStatusId = 1, UserStatus = "Inactive" },
                new UserStatusTable { UserStatusId = 2, UserStatus = "Active" },
                new UserStatusTable { UserStatusId = 3, UserStatus = "Blocked" });
            seeded.Add("UserStatusTable");
        }

        if (!db.GenderTables.Any())
        {
            db.GenderTables.AddRange(
                new GenderTable { GenderId = 1, Gender = "Male" },
                new GenderTable { GenderId = 2, Gender = "Female" },
                new GenderTable { GenderId = 3, Gender = "Other" });
            seeded.Add("GenderTable");
        }

        if (!db.OrderStatusTables.Any())
        {
            db.OrderStatusTables.AddRange(
                new OrderStatusTable { OrderStatusId = 1, OrderStatus = "Pending" },
                new OrderStatusTable { OrderStatusId = 2, OrderStatus = "Accept" },
                new OrderStatusTable { OrderStatusId = 3, OrderStatus = "On the way" },
                new OrderStatusTable { OrderStatusId = 4, OrderStatus = "Delivered" },
                new OrderStatusTable { OrderStatusId = 5, OrderStatus = "Cancelled" });
            seeded.Add("OrderStatusTable");
        }

        if (!db.OrderTypeTables.Any())
        {
            db.OrderTypeTables.AddRange(
                new OrderTypeTable { OrderTypeId = 1, OrderTypeName = "Delivery" },
                new OrderTypeTable { OrderTypeId = 2, OrderTypeName = "Takeaway" },
                new OrderTypeTable { OrderTypeId = 3, OrderTypeName = "Dine-in" });
            seeded.Add("OrderTypeTable");
        }

        if (!db.VisibleStatusTables.Any())
        {
            db.VisibleStatusTables.AddRange(
                new VisibleStatusTable { VisibleStatusId = 1, VisibleStatus = "Visible" },
                new VisibleStatusTable { VisibleStatusId = 2, VisibleStatus = "Hidden" });
            seeded.Add("VisibleStatusTable");
        }

        if (!db.AddressTypeTables.Any())
        {
            db.AddressTypeTables.AddRange(
                new AddressTypeTable { AdressTypeId = 1, AddressType = "Home" },
                new AddressTypeTable { AdressTypeId = 2, AddressType = "Work" },
                new AddressTypeTable { AdressTypeId = 3, AddressType = "Other" });
            seeded.Add("AddressTypeTable");
        }

        if (!db.ReservationStatusTables.Any())
        {
            db.ReservationStatusTables.AddRange(
                new ReservationStatusTable { ReservationStatusId = 1, ReservationStatus = "Pending" },
                new ReservationStatusTable { ReservationStatusId = 2, ReservationStatus = "Confirmed" },
                new ReservationStatusTable { ReservationStatusId = 3, ReservationStatus = "Completed" },
                new ReservationStatusTable { ReservationStatusId = 4, ReservationStatus = "Cancelled" });
            seeded.Add("ReservationStatusTable");
        }

        if (seeded.Count > 0)
            db.SaveChanges();

        return seeded;
    }
}
