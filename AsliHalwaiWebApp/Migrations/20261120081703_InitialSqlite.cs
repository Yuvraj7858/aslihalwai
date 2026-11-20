using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsliHalwaiWebApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddressTypeTable",
                columns: table => new
                {
                    AdressTypeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddressType = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressTypeTable", x => x.AdressTypeID);
                });

            migrationBuilder.CreateTable(
                name: "CartDealTable",
                columns: table => new
                {
                    CartDealId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CartId = table.Column<int>(type: "INTEGER", nullable: false),
                    StockDealId = table.Column<int>(type: "INTEGER", nullable: false),
                    Qty = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartDealTable", x => x.CartDealId);
                });

            migrationBuilder.CreateTable(
                name: "CartTable",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartTable", x => x.CartId);
                });

            migrationBuilder.CreateTable(
                name: "DiscountTable",
                columns: table => new
                {
                    DiscountID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscountCode = table.Column<string>(type: "TEXT", unicode: false, maxLength: 50, nullable: false, collation: "NOCASE"),
                    DiscountPercentage = table.Column<double>(type: "REAL", nullable: false),
                    IsUseStatus = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountTable", x => x.DiscountID);
                });

            migrationBuilder.CreateTable(
                name: "GenderTable",
                columns: table => new
                {
                    GenderID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Gender = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenderTable", x => x.GenderID);
                });

            migrationBuilder.CreateTable(
                name: "OrderPaymentTable",
                columns: table => new
                {
                    OrderPaymentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderPaymentStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPaymentTable", x => x.OrderPaymentId);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatusTable",
                columns: table => new
                {
                    OrderStatusID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderStatus = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusTable", x => x.OrderStatusID);
                });

            migrationBuilder.CreateTable(
                name: "OrderTypeTable",
                columns: table => new
                {
                    OrderTypeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderTypeName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTypeTable", x => x.OrderTypeID);
                });

            migrationBuilder.CreateTable(
                name: "ReservationStatusTable",
                columns: table => new
                {
                    ReservationStatusID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReservationStatus = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationStatusTable", x => x.ReservationStatusID);
                });

            migrationBuilder.CreateTable(
                name: "StockMenuCategoryTable",
                columns: table => new
                {
                    StockMenuCategoryID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StockMenuCategory = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false, collation: "NOCASE"),
                    CreatedBy_UserID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMenuCategoryTable", x => x.StockMenuCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "UserStatusTable",
                columns: table => new
                {
                    UserStatusID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserStatus = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatusTable", x => x.UserStatusID);
                });

            migrationBuilder.CreateTable(
                name: "UserTypeTable",
                columns: table => new
                {
                    UserTypeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypeTable", x => x.UserTypeID);
                });

            migrationBuilder.CreateTable(
                name: "VisibleStatusTable",
                columns: table => new
                {
                    VisibleStatusID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VisibleStatus = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisibleStatusTable", x => x.VisibleStatusID);
                });

            migrationBuilder.CreateTable(
                name: "UserTable",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserTypeID = table.Column<int>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false, collation: "NOCASE"),
                    Password = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false, collation: "NOCASE"),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false, collation: "NOCASE"),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true, collation: "NOCASE"),
                    ContactNo = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false, collation: "NOCASE"),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false, collation: "NOCASE"),
                    RegistrationDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    UserStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    UserStatusChangeDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    GenderID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTable", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_UserTable_GenderTable_GenderID",
                        column: x => x.GenderID,
                        principalTable: "GenderTable",
                        principalColumn: "GenderID");
                    table.ForeignKey(
                        name: "FK_UserTable_UserStatusTable_UserStatusID",
                        column: x => x.UserStatusID,
                        principalTable: "UserStatusTable",
                        principalColumn: "UserStatusID");
                    table.ForeignKey(
                        name: "FK_UserTable_UserTypeTable_UserTypeID",
                        column: x => x.UserTypeID,
                        principalTable: "UserTypeTable",
                        principalColumn: "UserTypeID");
                });

            migrationBuilder.CreateTable(
                name: "StockDealTable",
                columns: table => new
                {
                    StockDealID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StockDealTitle = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false, collation: "NOCASE"),
                    DealPrice = table.Column<double>(type: "REAL", nullable: false),
                    VisibleStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    StockDealStartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Discount = table.Column<double>(type: "REAL", nullable: false),
                    StockDealEndDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    StockDealRegisterDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DealPhoto = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockDealTable", x => x.StockDealID);
                    table.ForeignKey(
                        name: "FK_StockDealTable_VisibleStatusTable_VisibleStatusID",
                        column: x => x.VisibleStatusID,
                        principalTable: "VisibleStatusTable",
                        principalColumn: "VisibleStatusID");
                });

            migrationBuilder.CreateTable(
                name: "StockItemCategoryTable",
                columns: table => new
                {
                    StockItemCategoryID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StockItemCategory = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, collation: "NOCASE"),
                    CreatedBy_UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    VisibleStatusID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItemCategoryTable", x => x.StockItemCategoryID);
                    table.ForeignKey(
                        name: "FK_StockItemCategoryTable_VisibleStatusTable_VisibleStatusID",
                        column: x => x.VisibleStatusID,
                        principalTable: "VisibleStatusTable",
                        principalColumn: "VisibleStatusID");
                });

            migrationBuilder.CreateTable(
                name: "TableReservationTable",
                columns: table => new
                {
                    TableReservationID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Reservation_UserID = table.Column<int>(type: "INTEGER", nullable: true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false, collation: "NOCASE"),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false, collation: "NOCASE"),
                    MobileNo = table.Column<string>(type: "TEXT", unicode: false, maxLength: 14, nullable: false, collation: "NOCASE"),
                    ReservationRequestDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ReservationDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NoOfPerson = table.Column<int>(type: "INTEGER", nullable: false),
                    ProccessBy_UserID = table.Column<int>(type: "INTEGER", nullable: true),
                    ReservationStatudID = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableReservationTable", x => x.TableReservationID);
                    table.ForeignKey(
                        name: "FK_TableReservationTable_ReservationStatusTable_ReservationStatudID",
                        column: x => x.ReservationStatudID,
                        principalTable: "ReservationStatusTable",
                        principalColumn: "ReservationStatusID");
                    table.ForeignKey(
                        name: "FK_TableReservationTable_UserTable_ProccessBy_UserID",
                        column: x => x.ProccessBy_UserID,
                        principalTable: "UserTable",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_TableReservationTable_UserTable_Reservation_UserID",
                        column: x => x.Reservation_UserID,
                        principalTable: "UserTable",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "UserAddressTable",
                columns: table => new
                {
                    UserAddressID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    AddressTypeID = table.Column<int>(type: "INTEGER", nullable: false),
                    FullAddress = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE"),
                    VisibleStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy_UserID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddressTable", x => x.UserAddressID);
                    table.ForeignKey(
                        name: "FK_UserAddressTable_AddressTypeTable_AddressTypeID",
                        column: x => x.AddressTypeID,
                        principalTable: "AddressTypeTable",
                        principalColumn: "AdressTypeID");
                    table.ForeignKey(
                        name: "FK_UserAddressTable_UserTable_UserID",
                        column: x => x.UserID,
                        principalTable: "UserTable",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_UserAddressTable_VisibleStatusTable_VisibleStatusID",
                        column: x => x.VisibleStatusID,
                        principalTable: "VisibleStatusTable",
                        principalColumn: "VisibleStatusID");
                });

            migrationBuilder.CreateTable(
                name: "UserDetailTable",
                columns: table => new
                {
                    UserDetailId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserDetailProvideDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    PhotoPath = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE"),
                    AadharCard = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE"),
                    EducationLevel = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE"),
                    ExperenceLevel = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE"),
                    EducationLastDegreeScanPhoto = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE"),
                    LastExperienceScanPhoto = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE"),
                    CreatedBy_UserID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetailTable", x => x.UserDetailId);
                    table.ForeignKey(
                        name: "FK_UserDetailTable_UserTable_UserDetailId",
                        column: x => x.UserDetailId,
                        principalTable: "UserTable",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "UserPasswordRecoveryTable",
                columns: table => new
                {
                    UserPasswordRecoveryID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    RecoveryCode = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE"),
                    ExpiryDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RecoveryStatus = table.Column<bool>(type: "INTEGER", nullable: false),
                    OldPassword = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswordRecoveryTable", x => x.UserPasswordRecoveryID);
                    table.ForeignKey(
                        name: "FK_UserPasswordRecoveryTable_UserTable_UserID",
                        column: x => x.UserID,
                        principalTable: "UserTable",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockItemTable",
                columns: table => new
                {
                    StockItemID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StockItemCategoryID = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemPhotoPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, collation: "NOCASE"),
                    StockItemTitle = table.Column<string>(type: "TEXT", maxLength: 350, nullable: false, collation: "NOCASE"),
                    ItemSize = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false, collation: "NOCASE"),
                    UnitPrice = table.Column<double>(type: "REAL", nullable: false),
                    RegisterDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    VisibleStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy_UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderTypeID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItemTable", x => x.StockItemID);
                    table.ForeignKey(
                        name: "FK_StockItemTable_OrderTypeTable_OrderTypeID",
                        column: x => x.OrderTypeID,
                        principalTable: "OrderTypeTable",
                        principalColumn: "OrderTypeID");
                    table.ForeignKey(
                        name: "FK_StockItemTable_StockItemCategoryTable_StockItemCategoryID",
                        column: x => x.StockItemCategoryID,
                        principalTable: "StockItemCategoryTable",
                        principalColumn: "StockItemCategoryID");
                    table.ForeignKey(
                        name: "FK_StockItemTable_VisibleStatusTable_VisibleStatusID",
                        column: x => x.VisibleStatusID,
                        principalTable: "VisibleStatusTable",
                        principalColumn: "VisibleStatusID");
                });

            migrationBuilder.CreateTable(
                name: "OrderTable",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderBy_UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OrderTypeID = table.Column<int>(type: "INTEGER", nullable: false),
                    DeliveryAddress_UserAddressID = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderReceivedBy_ContactNo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, collation: "NOCASE"),
                    PaymentMode = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE"),
                    PaymentStatus = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE"),
                    PaymentUTR = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE"),
                    OrderReceivedBy_FullName = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false, collation: "NOCASE"),
                    OrderStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE"),
                    ProccessBy_UserID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTable", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_OrderTable_OrderStatusTable_OrderStatusID",
                        column: x => x.OrderStatusID,
                        principalTable: "OrderStatusTable",
                        principalColumn: "OrderStatusID");
                    table.ForeignKey(
                        name: "FK_OrderTable_OrderTypeTable_OrderTypeID",
                        column: x => x.OrderTypeID,
                        principalTable: "OrderTypeTable",
                        principalColumn: "OrderTypeID");
                    table.ForeignKey(
                        name: "FK_OrderTable_UserAddressTable_DeliveryAddress_UserAddressID",
                        column: x => x.DeliveryAddress_UserAddressID,
                        principalTable: "UserAddressTable",
                        principalColumn: "UserAddressID");
                });

            migrationBuilder.CreateTable(
                name: "CartItemDetailTable",
                columns: table => new
                {
                    CartItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CartId = table.Column<int>(type: "INTEGER", nullable: false),
                    StockItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Qty = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemDetailTable", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_CartItemDetailTable_StockItemTable_StockItemId",
                        column: x => x.StockItemId,
                        principalTable: "StockItemTable",
                        principalColumn: "StockItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockDealDetailTable",
                columns: table => new
                {
                    StockDealDetailID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StockDealID = table.Column<int>(type: "INTEGER", nullable: false),
                    StockItemID = table.Column<int>(type: "INTEGER", nullable: false),
                    Discount = table.Column<double>(type: "REAL", nullable: false),
                    VisibleStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy_UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockDealDetailTable", x => x.StockDealDetailID);
                    table.ForeignKey(
                        name: "FK_StockDealDetailTable_StockDealTable_StockDealID",
                        column: x => x.StockDealID,
                        principalTable: "StockDealTable",
                        principalColumn: "StockDealID");
                    table.ForeignKey(
                        name: "FK_StockDealDetailTable_StockItemTable_StockItemID",
                        column: x => x.StockItemID,
                        principalTable: "StockItemTable",
                        principalColumn: "StockItemID");
                    table.ForeignKey(
                        name: "FK_StockDealDetailTable_VisibleStatusTable_VisibleStatusID",
                        column: x => x.VisibleStatusID,
                        principalTable: "VisibleStatusTable",
                        principalColumn: "VisibleStatusID");
                });

            migrationBuilder.CreateTable(
                name: "StockItemIngredientTable",
                columns: table => new
                {
                    StockItemIngredientID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StockItemID = table.Column<int>(type: "INTEGER", nullable: false),
                    StockItemIngredientPhoto = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, collation: "NOCASE"),
                    StockItemIngredientTitle = table.Column<string>(type: "TEXT", maxLength: 350, nullable: false, collation: "NOCASE"),
                    CreatedBy_UserID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItemIngredientTable", x => x.StockItemIngredientID);
                    table.ForeignKey(
                        name: "FK_StockItemIngredientTable_StockItemTable_StockItemID",
                        column: x => x.StockItemID,
                        principalTable: "StockItemTable",
                        principalColumn: "StockItemID");
                });

            migrationBuilder.CreateTable(
                name: "StockItemReviewTable",
                columns: table => new
                {
                    ItemReviewID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StockItemID = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewBy_UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewDetails = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItemReviewTable", x => x.ItemReviewID);
                    table.ForeignKey(
                        name: "FK_StockItemReviewTable_StockItemTable_StockItemID",
                        column: x => x.StockItemID,
                        principalTable: "StockItemTable",
                        principalColumn: "StockItemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockItemReviewTable_UserTable_ReviewBy_UserID",
                        column: x => x.ReviewBy_UserID,
                        principalTable: "UserTable",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockMenuItemTable",
                columns: table => new
                {
                    StockMenuItemID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StockMenuCategoryID = table.Column<int>(type: "INTEGER", nullable: false),
                    StockItemID = table.Column<int>(type: "INTEGER", nullable: false),
                    VisibleStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy_UserID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMenuItemTable", x => x.StockMenuItemID);
                    table.ForeignKey(
                        name: "FK_StockMenuItemTable_StockItemTable_StockItemID",
                        column: x => x.StockItemID,
                        principalTable: "StockItemTable",
                        principalColumn: "StockItemID");
                    table.ForeignKey(
                        name: "FK_StockMenuItemTable_StockMenuCategoryTable_StockMenuCategoryID",
                        column: x => x.StockMenuCategoryID,
                        principalTable: "StockMenuCategoryTable",
                        principalColumn: "StockMenuCategoryID");
                    table.ForeignKey(
                        name: "FK_StockMenuItemTable_VisibleStatusTable_VisibleStatusID",
                        column: x => x.VisibleStatusID,
                        principalTable: "VisibleStatusTable",
                        principalColumn: "VisibleStatusID");
                });

            migrationBuilder.CreateTable(
                name: "OrderDealDetailTable",
                columns: table => new
                {
                    OrderDealDetailID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderID = table.Column<int>(type: "INTEGER", nullable: false),
                    StockDealID = table.Column<int>(type: "INTEGER", nullable: false),
                    Qty = table.Column<int>(type: "INTEGER", nullable: false),
                    DealPrice = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDealDetailTable", x => x.OrderDealDetailID);
                    table.ForeignKey(
                        name: "FK_OrderDealDetailTable_OrderTable_OrderID",
                        column: x => x.OrderID,
                        principalTable: "OrderTable",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK_OrderDealDetailTable_StockDealTable_StockDealID",
                        column: x => x.StockDealID,
                        principalTable: "StockDealTable",
                        principalColumn: "StockDealID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemDetailTable",
                columns: table => new
                {
                    OrderItemDetailID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderID = table.Column<int>(type: "INTEGER", nullable: false),
                    StockItemID = table.Column<int>(type: "INTEGER", nullable: false),
                    Qty = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<double>(type: "REAL", nullable: false),
                    DiscountID = table.Column<int>(type: "INTEGER", nullable: true),
                    DiscountAmount = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemDetailTable", x => x.OrderItemDetailID);
                    table.ForeignKey(
                        name: "FK_OrderItemDetailTable_DiscountTable_DiscountID",
                        column: x => x.DiscountID,
                        principalTable: "DiscountTable",
                        principalColumn: "DiscountID");
                    table.ForeignKey(
                        name: "FK_OrderItemDetailTable_OrderTable_OrderID",
                        column: x => x.OrderID,
                        principalTable: "OrderTable",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK_OrderItemDetailTable_StockItemTable_StockItemID",
                        column: x => x.StockItemID,
                        principalTable: "StockItemTable",
                        principalColumn: "StockItemID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItemDetailTable_StockItemId",
                table: "CartItemDetailTable",
                column: "StockItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDealDetailTable_OrderID",
                table: "OrderDealDetailTable",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDealDetailTable_StockDealID",
                table: "OrderDealDetailTable",
                column: "StockDealID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemDetailTable_DiscountID",
                table: "OrderItemDetailTable",
                column: "DiscountID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemDetailTable_OrderID",
                table: "OrderItemDetailTable",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemDetailTable_StockItemID",
                table: "OrderItemDetailTable",
                column: "StockItemID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_DeliveryAddress_UserAddressID",
                table: "OrderTable",
                column: "DeliveryAddress_UserAddressID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_OrderStatusID",
                table: "OrderTable",
                column: "OrderStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_OrderTypeID",
                table: "OrderTable",
                column: "OrderTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StockDealDetailTable_StockDealID",
                table: "StockDealDetailTable",
                column: "StockDealID");

            migrationBuilder.CreateIndex(
                name: "IX_StockDealDetailTable_StockItemID",
                table: "StockDealDetailTable",
                column: "StockItemID");

            migrationBuilder.CreateIndex(
                name: "IX_StockDealDetailTable_VisibleStatusID",
                table: "StockDealDetailTable",
                column: "VisibleStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_StockDealTable_VisibleStatusID",
                table: "StockDealTable",
                column: "VisibleStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_StockItemCategoryTable_VisibleStatusID",
                table: "StockItemCategoryTable",
                column: "VisibleStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_StockItemIngredientTable_StockItemID",
                table: "StockItemIngredientTable",
                column: "StockItemID");

            migrationBuilder.CreateIndex(
                name: "IX_StockItemReviewTable_ReviewBy_UserID",
                table: "StockItemReviewTable",
                column: "ReviewBy_UserID");

            migrationBuilder.CreateIndex(
                name: "IX_StockItemReviewTable_StockItemID",
                table: "StockItemReviewTable",
                column: "StockItemID");

            migrationBuilder.CreateIndex(
                name: "IX_StockItemTable_OrderTypeID",
                table: "StockItemTable",
                column: "OrderTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StockItemTable_StockItemCategoryID",
                table: "StockItemTable",
                column: "StockItemCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_StockItemTable_VisibleStatusID",
                table: "StockItemTable",
                column: "VisibleStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMenuItemTable_StockItemID",
                table: "StockMenuItemTable",
                column: "StockItemID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMenuItemTable_StockMenuCategoryID",
                table: "StockMenuItemTable",
                column: "StockMenuCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMenuItemTable_VisibleStatusID",
                table: "StockMenuItemTable",
                column: "VisibleStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_TableReservationTable_ProccessBy_UserID",
                table: "TableReservationTable",
                column: "ProccessBy_UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TableReservationTable_Reservation_UserID",
                table: "TableReservationTable",
                column: "Reservation_UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TableReservationTable_ReservationStatudID",
                table: "TableReservationTable",
                column: "ReservationStatudID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddressTable_AddressTypeID",
                table: "UserAddressTable",
                column: "AddressTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddressTable_UserID",
                table: "UserAddressTable",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddressTable_VisibleStatusID",
                table: "UserAddressTable",
                column: "VisibleStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswordRecoveryTable_UserID",
                table: "UserPasswordRecoveryTable",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTable_GenderID",
                table: "UserTable",
                column: "GenderID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTable_UserStatusID",
                table: "UserTable",
                column: "UserStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTable_UserTypeID",
                table: "UserTable",
                column: "UserTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartDealTable");

            migrationBuilder.DropTable(
                name: "CartItemDetailTable");

            migrationBuilder.DropTable(
                name: "CartTable");

            migrationBuilder.DropTable(
                name: "OrderDealDetailTable");

            migrationBuilder.DropTable(
                name: "OrderItemDetailTable");

            migrationBuilder.DropTable(
                name: "OrderPaymentTable");

            migrationBuilder.DropTable(
                name: "StockDealDetailTable");

            migrationBuilder.DropTable(
                name: "StockItemIngredientTable");

            migrationBuilder.DropTable(
                name: "StockItemReviewTable");

            migrationBuilder.DropTable(
                name: "StockMenuItemTable");

            migrationBuilder.DropTable(
                name: "TableReservationTable");

            migrationBuilder.DropTable(
                name: "UserDetailTable");

            migrationBuilder.DropTable(
                name: "UserPasswordRecoveryTable");

            migrationBuilder.DropTable(
                name: "DiscountTable");

            migrationBuilder.DropTable(
                name: "OrderTable");

            migrationBuilder.DropTable(
                name: "StockDealTable");

            migrationBuilder.DropTable(
                name: "StockItemTable");

            migrationBuilder.DropTable(
                name: "StockMenuCategoryTable");

            migrationBuilder.DropTable(
                name: "ReservationStatusTable");

            migrationBuilder.DropTable(
                name: "OrderStatusTable");

            migrationBuilder.DropTable(
                name: "UserAddressTable");

            migrationBuilder.DropTable(
                name: "OrderTypeTable");

            migrationBuilder.DropTable(
                name: "StockItemCategoryTable");

            migrationBuilder.DropTable(
                name: "AddressTypeTable");

            migrationBuilder.DropTable(
                name: "UserTable");

            migrationBuilder.DropTable(
                name: "VisibleStatusTable");

            migrationBuilder.DropTable(
                name: "GenderTable");

            migrationBuilder.DropTable(
                name: "UserStatusTable");

            migrationBuilder.DropTable(
                name: "UserTypeTable");
        }
    }
}
