using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsliHalwaiWebApp.Models;

[Table("OrderTable")]
public partial class OrderTable
{
    [Key]
    [Column("OrderID")]
    public int OrderId { get; set; }

    [Column("OrderBy_UserID")]
    public int OrderByUserId { get; set; }

    public DateTime OrderDateTime { get; set; }

    [Column("OrderTypeID")]
    public int OrderTypeId { get; set; }

    [Column("DeliveryAddress_UserAddressID")]
    public int DeliveryAddressUserAddressId { get; set; }

    [Column("OrderReceivedBy_ContactNo")]
    [StringLength(20)]
    public string OrderReceivedByContactNo { get; set; } = null!;

    public string? PaymentMode { get; set; }
    public string? PaymentStatus { get; set; }
    public string? PaymentUTR { get; set; }

    [Column("OrderReceivedBy_FullName")]
    [StringLength(250)]
    public string OrderReceivedByFullName { get; set; } = null!;

    [Column("OrderStatusID")]
    public int OrderStatusId { get; set; }

    public string? Description { get; set; }

    [Column("ProccessBy_UserID")]
    public int? ProccessByUserId { get; set; }

    // Address
    [ForeignKey(nameof(DeliveryAddressUserAddressId))]
    public virtual UserAddressTable? DeliveryAddressUserAddress { get; set; }

    // Order Type
    [ForeignKey(nameof(OrderTypeId))]
    public virtual OrderTypeTable? OrderType { get; set; }

    public string OrderTypeDisplay => OrderType?.OrderTypeName ?? "N/A";
    // Status
    [ForeignKey(nameof(OrderStatusId))]
    public virtual OrderStatusTable? OrderStatus { get; set; }

    // Items
    public virtual ICollection<OrderItemDetailTable> OrderItemDetailTables { get; set; }
        = new List<OrderItemDetailTable>();

    // Deals
    [InverseProperty(nameof(OrderDealDetailTable.Order))]
    public virtual ICollection<OrderDealDetailTable> OrderDealDetailTables { get; set; }
        = new List<OrderDealDetailTable>();

   
}
