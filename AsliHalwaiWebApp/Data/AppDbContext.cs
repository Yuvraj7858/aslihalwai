
using Microsoft.EntityFrameworkCore;
using AsliHalwaiWebApp.Models;

namespace AsliHalwaiWebApp.Data;

public partial class AppDbContext : DbContext
{
   
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UserPasswordRecoveryTable> UserPasswordRecoveryTables { get; set; }
    public virtual DbSet<StockItemReviewTable> StockItemReviewTables { get; set; }
    public virtual DbSet<AddressTypeTable> AddressTypeTables { get; set; }
    public virtual DbSet<DiscountTable> DiscountTables { get; set; }

    public virtual DbSet<CartTable> CartTables { get; set; }
    public virtual DbSet<CartItemDetailTable> CartItemDetailTables { get; set; }
    public virtual DbSet<CartDealTable> CartDealTables { get; set; }

    public virtual DbSet<OrderPaymentTable> OrderPaymentTables { get; set; }
    public virtual DbSet<OrderDealDetailTable> OrderDealDetailTables { get; set; }
    public virtual DbSet<OrderItemDetailTable> OrderItemDetailTables { get; set; }
    public virtual DbSet<OrderStatusTable> OrderStatusTables { get; set; }
    public virtual DbSet<OrderTable> OrderTables { get; set; }
    public virtual DbSet<OrderTypeTable> OrderTypeTables { get; set; }
    public virtual DbSet<ReservationStatusTable> ReservationStatusTables { get; set; }
    public virtual DbSet<StockDealDetailTable> StockDealDetailTables { get; set; }
    public virtual DbSet<StockDealTable> StockDealTables { get; set; }
    public virtual DbSet<StockItemCategoryTable> StockItemCategoryTables { get; set; }
    public virtual DbSet<StockItemIngredientTable> StockItemIngredientTables { get; set; }
    public virtual DbSet<StockItemTable> StockItemTables { get; set; }
    public virtual DbSet<StockMenuCategoryTable> StockMenuCategoryTables { get; set; }
    public virtual DbSet<StockMenuItemTable> StockMenuItemTables { get; set; }

    public virtual DbSet<TableReservationTable> TableReservationTables { get; set; }
    public virtual DbSet<UserAddressTable> UserAddressTables { get; set; }
    public virtual DbSet<UserDetailTable> UserDetailTables { get; set; }
    public virtual DbSet<UserStatusTable> UserStatusTables { get; set; }
    public virtual DbSet<UserTable> UserTables { get; set; }
    public virtual DbSet<GenderTable> GenderTables { get; set; }

    public virtual DbSet<UserTypeTable> UserTypeTables { get; set; }
    public virtual DbSet<VisibleStatusTable> VisibleStatusTables { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SQL Server used a case-insensitive collation by default. SQLite is
        // case-sensitive for equality, so apply NOCASE to every text column.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string))
                    property.SetCollation("NOCASE");
            }
        }

        // ================= ORDER DEAL DETAIL =================
        modelBuilder.Entity<OrderDealDetailTable>(entity =>
        {
            entity.HasKey(e => e.OrderDealDetailId);

            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderDealDetailTables)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ================= ORDER ITEM DETAIL =================
        modelBuilder.Entity<OrderItemDetailTable>(entity =>
        {
            entity.HasOne(d => d.Discount)
                .WithMany(p => p.OrderItemDetailTables)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderItemDetailTables)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StockItem)
                .WithMany(p => p.OrderItemDetailTables)
                .HasForeignKey(d => d.StockItemId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ================= ORDER =================
        modelBuilder.Entity<OrderTable>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.HasOne(d => d.OrderType)
                .WithMany(p => p.OrderTables) // ✅ FIXED NAVIGATION
                .HasForeignKey(d => d.OrderTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.DeliveryAddressUserAddress)
                .WithMany(p => p.OrderTables)
                .HasForeignKey(d => d.DeliveryAddressUserAddressId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrderStatus)
                .WithMany(p => p.OrderTables)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ================= STOCK DEAL DETAIL =================
        modelBuilder.Entity<StockDealDetailTable>(entity =>
        {
            entity.HasOne(d => d.StockDeal)
                .WithMany(p => p.StockDealDetailTables)
                .HasForeignKey(d => d.StockDealId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StockItem)
                .WithMany(p => p.StockDealDetailTables)
                .HasForeignKey(d => d.StockItemId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.VisibleStatus)
                .WithMany(p => p.StockDealDetailTables)
                .HasForeignKey(d => d.VisibleStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ================= STOCK DEAL =================
        modelBuilder.Entity<StockDealTable>(entity =>
        {
            entity.HasOne(d => d.VisibleStatus)
                .WithMany(p => p.StockDealTables)
                .HasForeignKey(d => d.VisibleStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ================= STOCK ITEM CATEGORY =================
        modelBuilder.Entity<StockItemCategoryTable>(entity =>
        {
            entity.HasOne(d => d.VisibleStatus)
                .WithMany(p => p.StockItemCategoryTables)
                .HasForeignKey(d => d.VisibleStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ================= STOCK ITEM INGREDIENT =================
        modelBuilder.Entity<StockItemIngredientTable>(entity =>
        {
            entity.HasOne(d => d.StockItem)
                .WithMany(p => p.StockItemIngredientTables)
                .HasForeignKey(d => d.StockItemId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ================= STOCK ITEM =================
        modelBuilder.Entity<StockItemTable>(entity =>
        {
            entity.HasOne(d => d.OrderTypeName)
                .WithMany(p => p.StockItemTables)
                .HasForeignKey(d => d.OrderTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StockItemCategory)
                .WithMany(p => p.StockItemTables)
                .HasForeignKey(d => d.StockItemCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.VisibleStatus)
                .WithMany(p => p.StockItemTables)
                .HasForeignKey(d => d.VisibleStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ================= STOCK MENU ITEM =================
        modelBuilder.Entity<StockMenuItemTable>(entity =>
        {
            entity.HasOne(d => d.StockItem)
                .WithMany(p => p.StockMenuItemTables)
                .HasForeignKey(d => d.StockItemId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StockMenuCategory)
                .WithMany(p => p.StockMenuItemTables)
                .HasForeignKey(d => d.StockMenuCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.VisibleStatus)
                .WithMany(p => p.StockMenuItemTables)
                .HasForeignKey(d => d.VisibleStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ================= TABLE RESERVATION =================
        modelBuilder.Entity<TableReservationTable>(entity =>
        {
            entity.HasKey(e => e.TableReservationId);

            entity.HasOne(d => d.ProccessByUser)
                .WithMany(p => p.TableReservationTableProccessByUsers)
                .HasForeignKey(d => d.ProccessByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReservationStatud)
                .WithMany(p => p.TableReservationTables)
                .HasForeignKey(d => d.ReservationStatudId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReservationUser)
                .WithMany(p => p.TableReservationTableReservationUsers)
                .HasForeignKey(d => d.ReservationUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ================= USER ADDRESS =================
        modelBuilder.Entity<UserAddressTable>(entity =>
        {
            entity.HasOne(d => d.AddressType)
                .WithMany(p => p.UserAddressTables)
                .HasForeignKey(d => d.AddressTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserAddressTables)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.VisibleStatus)
                .WithMany(p => p.UserAddressTables)
                .HasForeignKey(d => d.VisibleStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ================= USER DETAIL (1–1) =================
        modelBuilder.Entity<UserDetailTable>(entity =>
        {
            entity.HasKey(e => e.UserDetailId);

            entity.Property(e => e.UserDetailId).ValueGeneratedNever();

            entity.HasOne(d => d.User)
                .WithOne(p => p.UserDetailTable)
                .HasForeignKey<UserDetailTable>(d => d.UserDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ================= USER =================
        modelBuilder.Entity<UserTable>(entity =>
        {
            entity.HasOne(d => d.Gender)
                .WithMany(p => p.UserTables)
                .HasForeignKey(d => d.GenderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UserStatus)
                .WithMany(p => p.UserTables)
                .HasForeignKey(d => d.UserStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UserType)
                .WithMany(p => p.UserTables)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
