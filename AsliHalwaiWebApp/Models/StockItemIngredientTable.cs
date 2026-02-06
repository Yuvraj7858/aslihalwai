using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("StockItemIngredientTable")]
public partial class StockItemIngredientTable
{
    [Key]
    [Column("StockItemIngredientID")]
    public int StockItemIngredientId { get; set; }

    [Column("StockItemID")]
    public int StockItemId { get; set; }

    [StringLength(500)]
    public string StockItemIngredientPhoto { get; set; }

    [StringLength(350)]
    public string StockItemIngredientTitle { get; set; } = null!;

    [Column("CreatedBy_UserID")]
    public int CreatedByUserId { get; set; }

    [ForeignKey("StockItemId")]
    [InverseProperty("StockItemIngredientTables")]
    public virtual StockItemTable StockItem { get; set; } = null!;
}
