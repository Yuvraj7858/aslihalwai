CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "AddressTypeTable" (
    "AdressTypeID" INTEGER NOT NULL CONSTRAINT "PK_AddressTypeTable" PRIMARY KEY AUTOINCREMENT,
    "AddressType" TEXT COLLATE NOCASE NOT NULL
);

CREATE TABLE "CartDealTable" (
    "CartDealId" INTEGER NOT NULL CONSTRAINT "PK_CartDealTable" PRIMARY KEY AUTOINCREMENT,
    "CartId" INTEGER NOT NULL,
    "StockDealId" INTEGER NOT NULL,
    "Qty" INTEGER NOT NULL
);

CREATE TABLE "CartTable" (
    "CartId" INTEGER NOT NULL CONSTRAINT "PK_CartTable" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL
);

CREATE TABLE "DiscountTable" (
    "DiscountID" INTEGER NOT NULL CONSTRAINT "PK_DiscountTable" PRIMARY KEY AUTOINCREMENT,
    "DiscountCode" TEXT COLLATE NOCASE NOT NULL,
    "DiscountPercentage" REAL NOT NULL,
    "IsUseStatus" INTEGER NOT NULL
);

CREATE TABLE "GenderTable" (
    "GenderID" INTEGER NOT NULL CONSTRAINT "PK_GenderTable" PRIMARY KEY AUTOINCREMENT,
    "Gender" TEXT COLLATE NOCASE NOT NULL
);

CREATE TABLE "OrderPaymentTable" (
    "OrderPaymentId" INTEGER NOT NULL CONSTRAINT "PK_OrderPaymentTable" PRIMARY KEY AUTOINCREMENT,
    "OrderPaymentStatus" TEXT COLLATE NOCASE NOT NULL
);

CREATE TABLE "OrderStatusTable" (
    "OrderStatusID" INTEGER NOT NULL CONSTRAINT "PK_OrderStatusTable" PRIMARY KEY AUTOINCREMENT,
    "OrderStatus" TEXT COLLATE NOCASE NOT NULL
);

CREATE TABLE "OrderTypeTable" (
    "OrderTypeID" INTEGER NOT NULL CONSTRAINT "PK_OrderTypeTable" PRIMARY KEY AUTOINCREMENT,
    "OrderTypeName" TEXT COLLATE NOCASE NOT NULL
);

CREATE TABLE "ReservationStatusTable" (
    "ReservationStatusID" INTEGER NOT NULL CONSTRAINT "PK_ReservationStatusTable" PRIMARY KEY AUTOINCREMENT,
    "ReservationStatus" TEXT COLLATE NOCASE NOT NULL
);

CREATE TABLE "StockMenuCategoryTable" (
    "StockMenuCategoryID" INTEGER NOT NULL CONSTRAINT "PK_StockMenuCategoryTable" PRIMARY KEY AUTOINCREMENT,
    "StockMenuCategory" TEXT COLLATE NOCASE NOT NULL,
    "CreatedBy_UserID" INTEGER NOT NULL
);

CREATE TABLE "UserStatusTable" (
    "UserStatusID" INTEGER NOT NULL CONSTRAINT "PK_UserStatusTable" PRIMARY KEY AUTOINCREMENT,
    "UserStatus" TEXT COLLATE NOCASE NOT NULL
);

CREATE TABLE "UserTypeTable" (
    "UserTypeID" INTEGER NOT NULL CONSTRAINT "PK_UserTypeTable" PRIMARY KEY AUTOINCREMENT,
    "UserType" TEXT COLLATE NOCASE NOT NULL
);

CREATE TABLE "VisibleStatusTable" (
    "VisibleStatusID" INTEGER NOT NULL CONSTRAINT "PK_VisibleStatusTable" PRIMARY KEY AUTOINCREMENT,
    "VisibleStatus" TEXT COLLATE NOCASE NOT NULL
);

CREATE TABLE "UserTable" (
    "UserID" INTEGER NOT NULL CONSTRAINT "PK_UserTable" PRIMARY KEY AUTOINCREMENT,
    "UserTypeID" INTEGER NOT NULL,
    "UserName" TEXT COLLATE NOCASE NOT NULL,
    "Password" TEXT COLLATE NOCASE NOT NULL,
    "FirstName" TEXT COLLATE NOCASE NOT NULL,
    "LastName" TEXT COLLATE NOCASE NULL,
    "ContactNo" TEXT COLLATE NOCASE NOT NULL,
    "EmailAddress" TEXT COLLATE NOCASE NOT NULL,
    "RegistrationDate" TEXT NOT NULL,
    "UserStatusID" INTEGER NOT NULL,
    "UserStatusChangeDate" TEXT NOT NULL,
    "GenderID" INTEGER NOT NULL,
    CONSTRAINT "FK_UserTable_GenderTable_GenderID" FOREIGN KEY ("GenderID") REFERENCES "GenderTable" ("GenderID"),
    CONSTRAINT "FK_UserTable_UserStatusTable_UserStatusID" FOREIGN KEY ("UserStatusID") REFERENCES "UserStatusTable" ("UserStatusID"),
    CONSTRAINT "FK_UserTable_UserTypeTable_UserTypeID" FOREIGN KEY ("UserTypeID") REFERENCES "UserTypeTable" ("UserTypeID")
);

CREATE TABLE "StockDealTable" (
    "StockDealID" INTEGER NOT NULL CONSTRAINT "PK_StockDealTable" PRIMARY KEY AUTOINCREMENT,
    "StockDealTitle" TEXT COLLATE NOCASE NOT NULL,
    "DealPrice" REAL NOT NULL,
    "VisibleStatusID" INTEGER NOT NULL,
    "StockDealStartDate" TEXT NOT NULL,
    "Discount" REAL NOT NULL,
    "StockDealEndDate" TEXT NOT NULL,
    "StockDealRegisterDate" TEXT NOT NULL,
    "DealPhoto" TEXT COLLATE NOCASE NOT NULL,
    CONSTRAINT "FK_StockDealTable_VisibleStatusTable_VisibleStatusID" FOREIGN KEY ("VisibleStatusID") REFERENCES "VisibleStatusTable" ("VisibleStatusID")
);

CREATE TABLE "StockItemCategoryTable" (
    "StockItemCategoryID" INTEGER NOT NULL CONSTRAINT "PK_StockItemCategoryTable" PRIMARY KEY AUTOINCREMENT,
    "StockItemCategory" TEXT COLLATE NOCASE NOT NULL,
    "CreatedBy_UserID" INTEGER NOT NULL,
    "VisibleStatusID" INTEGER NOT NULL,
    CONSTRAINT "FK_StockItemCategoryTable_VisibleStatusTable_VisibleStatusID" FOREIGN KEY ("VisibleStatusID") REFERENCES "VisibleStatusTable" ("VisibleStatusID")
);

CREATE TABLE "TableReservationTable" (
    "TableReservationID" INTEGER NOT NULL CONSTRAINT "PK_TableReservationTable" PRIMARY KEY AUTOINCREMENT,
    "Reservation_UserID" INTEGER NULL,
    "FullName" TEXT COLLATE NOCASE NOT NULL,
    "EmailAddress" TEXT COLLATE NOCASE NOT NULL,
    "MobileNo" TEXT COLLATE NOCASE NOT NULL,
    "ReservationRequestDate" TEXT NOT NULL,
    "ReservationDateTime" TEXT NOT NULL,
    "NoOfPerson" INTEGER NOT NULL,
    "ProccessBy_UserID" INTEGER NULL,
    "ReservationStatudID" INTEGER NOT NULL,
    "Description" TEXT COLLATE NOCASE NULL,
    CONSTRAINT "FK_TableReservationTable_ReservationStatusTable_ReservationStatudID" FOREIGN KEY ("ReservationStatudID") REFERENCES "ReservationStatusTable" ("ReservationStatusID"),
    CONSTRAINT "FK_TableReservationTable_UserTable_ProccessBy_UserID" FOREIGN KEY ("ProccessBy_UserID") REFERENCES "UserTable" ("UserID"),
    CONSTRAINT "FK_TableReservationTable_UserTable_Reservation_UserID" FOREIGN KEY ("Reservation_UserID") REFERENCES "UserTable" ("UserID")
);

CREATE TABLE "UserAddressTable" (
    "UserAddressID" INTEGER NOT NULL CONSTRAINT "PK_UserAddressTable" PRIMARY KEY AUTOINCREMENT,
    "UserID" INTEGER NOT NULL,
    "AddressTypeID" INTEGER NOT NULL,
    "FullAddress" TEXT COLLATE NOCASE NOT NULL,
    "VisibleStatusID" INTEGER NOT NULL,
    "CreatedBy_UserID" INTEGER NOT NULL,
    CONSTRAINT "FK_UserAddressTable_AddressTypeTable_AddressTypeID" FOREIGN KEY ("AddressTypeID") REFERENCES "AddressTypeTable" ("AdressTypeID"),
    CONSTRAINT "FK_UserAddressTable_UserTable_UserID" FOREIGN KEY ("UserID") REFERENCES "UserTable" ("UserID"),
    CONSTRAINT "FK_UserAddressTable_VisibleStatusTable_VisibleStatusID" FOREIGN KEY ("VisibleStatusID") REFERENCES "VisibleStatusTable" ("VisibleStatusID")
);

CREATE TABLE "UserDetailTable" (
    "UserDetailId" INTEGER NOT NULL CONSTRAINT "PK_UserDetailTable" PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "UserDetailProvideDate" TEXT NOT NULL,
    "PhotoPath" TEXT COLLATE NOCASE NULL,
    "AadharCard" TEXT COLLATE NOCASE NULL,
    "EducationLevel" TEXT COLLATE NOCASE NULL,
    "ExperenceLevel" TEXT COLLATE NOCASE NULL,
    "EducationLastDegreeScanPhoto" TEXT COLLATE NOCASE NULL,
    "LastExperienceScanPhoto" TEXT COLLATE NOCASE NULL,
    "CreatedBy_UserID" INTEGER NOT NULL,
    CONSTRAINT "FK_UserDetailTable_UserTable_UserDetailId" FOREIGN KEY ("UserDetailId") REFERENCES "UserTable" ("UserID")
);

CREATE TABLE "UserPasswordRecoveryTable" (
    "UserPasswordRecoveryID" INTEGER NOT NULL CONSTRAINT "PK_UserPasswordRecoveryTable" PRIMARY KEY AUTOINCREMENT,
    "UserID" INTEGER NOT NULL,
    "RecoveryCode" TEXT COLLATE NOCASE NOT NULL,
    "ExpiryDateTime" TEXT NOT NULL,
    "RecoveryStatus" INTEGER NOT NULL,
    "OldPassword" TEXT COLLATE NOCASE NOT NULL,
    CONSTRAINT "FK_UserPasswordRecoveryTable_UserTable_UserID" FOREIGN KEY ("UserID") REFERENCES "UserTable" ("UserID") ON DELETE CASCADE
);

CREATE TABLE "StockItemTable" (
    "StockItemID" INTEGER NOT NULL CONSTRAINT "PK_StockItemTable" PRIMARY KEY AUTOINCREMENT,
    "StockItemCategoryID" INTEGER NOT NULL,
    "ItemPhotoPath" TEXT COLLATE NOCASE NOT NULL,
    "StockItemTitle" TEXT COLLATE NOCASE NOT NULL,
    "ItemSize" TEXT COLLATE NOCASE NOT NULL,
    "UnitPrice" REAL NOT NULL,
    "RegisterDate" TEXT NOT NULL,
    "VisibleStatusID" INTEGER NOT NULL,
    "CreatedBy_UserID" INTEGER NOT NULL,
    "OrderTypeID" INTEGER NOT NULL,
    CONSTRAINT "FK_StockItemTable_OrderTypeTable_OrderTypeID" FOREIGN KEY ("OrderTypeID") REFERENCES "OrderTypeTable" ("OrderTypeID"),
    CONSTRAINT "FK_StockItemTable_StockItemCategoryTable_StockItemCategoryID" FOREIGN KEY ("StockItemCategoryID") REFERENCES "StockItemCategoryTable" ("StockItemCategoryID"),
    CONSTRAINT "FK_StockItemTable_VisibleStatusTable_VisibleStatusID" FOREIGN KEY ("VisibleStatusID") REFERENCES "VisibleStatusTable" ("VisibleStatusID")
);

CREATE TABLE "OrderTable" (
    "OrderID" INTEGER NOT NULL CONSTRAINT "PK_OrderTable" PRIMARY KEY AUTOINCREMENT,
    "OrderBy_UserID" INTEGER NOT NULL,
    "OrderDateTime" TEXT NOT NULL,
    "OrderTypeID" INTEGER NOT NULL,
    "DeliveryAddress_UserAddressID" INTEGER NOT NULL,
    "OrderReceivedBy_ContactNo" TEXT COLLATE NOCASE NOT NULL,
    "PaymentMode" TEXT COLLATE NOCASE NULL,
    "PaymentStatus" TEXT COLLATE NOCASE NULL,
    "PaymentUTR" TEXT COLLATE NOCASE NULL,
    "OrderReceivedBy_FullName" TEXT COLLATE NOCASE NOT NULL,
    "OrderStatusID" INTEGER NOT NULL,
    "Description" TEXT COLLATE NOCASE NULL,
    "ProccessBy_UserID" INTEGER NULL,
    CONSTRAINT "FK_OrderTable_OrderStatusTable_OrderStatusID" FOREIGN KEY ("OrderStatusID") REFERENCES "OrderStatusTable" ("OrderStatusID"),
    CONSTRAINT "FK_OrderTable_OrderTypeTable_OrderTypeID" FOREIGN KEY ("OrderTypeID") REFERENCES "OrderTypeTable" ("OrderTypeID"),
    CONSTRAINT "FK_OrderTable_UserAddressTable_DeliveryAddress_UserAddressID" FOREIGN KEY ("DeliveryAddress_UserAddressID") REFERENCES "UserAddressTable" ("UserAddressID")
);

CREATE TABLE "CartItemDetailTable" (
    "CartItemId" INTEGER NOT NULL CONSTRAINT "PK_CartItemDetailTable" PRIMARY KEY AUTOINCREMENT,
    "CartId" INTEGER NOT NULL,
    "StockItemId" INTEGER NOT NULL,
    "Qty" INTEGER NOT NULL,
    CONSTRAINT "FK_CartItemDetailTable_StockItemTable_StockItemId" FOREIGN KEY ("StockItemId") REFERENCES "StockItemTable" ("StockItemID") ON DELETE CASCADE
);

CREATE TABLE "StockDealDetailTable" (
    "StockDealDetailID" INTEGER NOT NULL CONSTRAINT "PK_StockDealDetailTable" PRIMARY KEY AUTOINCREMENT,
    "StockDealID" INTEGER NOT NULL,
    "StockItemID" INTEGER NOT NULL,
    "Discount" REAL NOT NULL,
    "VisibleStatusID" INTEGER NOT NULL,
    "CreatedBy_UserID" INTEGER NOT NULL,
    "Quantity" INTEGER NOT NULL,
    CONSTRAINT "FK_StockDealDetailTable_StockDealTable_StockDealID" FOREIGN KEY ("StockDealID") REFERENCES "StockDealTable" ("StockDealID"),
    CONSTRAINT "FK_StockDealDetailTable_StockItemTable_StockItemID" FOREIGN KEY ("StockItemID") REFERENCES "StockItemTable" ("StockItemID"),
    CONSTRAINT "FK_StockDealDetailTable_VisibleStatusTable_VisibleStatusID" FOREIGN KEY ("VisibleStatusID") REFERENCES "VisibleStatusTable" ("VisibleStatusID")
);

CREATE TABLE "StockItemIngredientTable" (
    "StockItemIngredientID" INTEGER NOT NULL CONSTRAINT "PK_StockItemIngredientTable" PRIMARY KEY AUTOINCREMENT,
    "StockItemID" INTEGER NOT NULL,
    "StockItemIngredientPhoto" TEXT COLLATE NOCASE NOT NULL,
    "StockItemIngredientTitle" TEXT COLLATE NOCASE NOT NULL,
    "CreatedBy_UserID" INTEGER NOT NULL,
    CONSTRAINT "FK_StockItemIngredientTable_StockItemTable_StockItemID" FOREIGN KEY ("StockItemID") REFERENCES "StockItemTable" ("StockItemID")
);

CREATE TABLE "StockItemReviewTable" (
    "ItemReviewID" INTEGER NOT NULL CONSTRAINT "PK_StockItemReviewTable" PRIMARY KEY AUTOINCREMENT,
    "StockItemID" INTEGER NOT NULL,
    "ReviewBy_UserID" INTEGER NOT NULL,
    "ReviewDateTime" TEXT NOT NULL,
    "Rating" INTEGER NOT NULL,
    "ReviewDetails" TEXT COLLATE NOCASE NOT NULL,
    CONSTRAINT "FK_StockItemReviewTable_StockItemTable_StockItemID" FOREIGN KEY ("StockItemID") REFERENCES "StockItemTable" ("StockItemID") ON DELETE CASCADE,
    CONSTRAINT "FK_StockItemReviewTable_UserTable_ReviewBy_UserID" FOREIGN KEY ("ReviewBy_UserID") REFERENCES "UserTable" ("UserID") ON DELETE CASCADE
);

CREATE TABLE "StockMenuItemTable" (
    "StockMenuItemID" INTEGER NOT NULL CONSTRAINT "PK_StockMenuItemTable" PRIMARY KEY AUTOINCREMENT,
    "StockMenuCategoryID" INTEGER NOT NULL,
    "StockItemID" INTEGER NOT NULL,
    "VisibleStatusID" INTEGER NOT NULL,
    "CreatedBy_UserID" INTEGER NOT NULL,
    CONSTRAINT "FK_StockMenuItemTable_StockItemTable_StockItemID" FOREIGN KEY ("StockItemID") REFERENCES "StockItemTable" ("StockItemID"),
    CONSTRAINT "FK_StockMenuItemTable_StockMenuCategoryTable_StockMenuCategoryID" FOREIGN KEY ("StockMenuCategoryID") REFERENCES "StockMenuCategoryTable" ("StockMenuCategoryID"),
    CONSTRAINT "FK_StockMenuItemTable_VisibleStatusTable_VisibleStatusID" FOREIGN KEY ("VisibleStatusID") REFERENCES "VisibleStatusTable" ("VisibleStatusID")
);

CREATE TABLE "OrderDealDetailTable" (
    "OrderDealDetailID" INTEGER NOT NULL CONSTRAINT "PK_OrderDealDetailTable" PRIMARY KEY AUTOINCREMENT,
    "OrderID" INTEGER NOT NULL,
    "StockDealID" INTEGER NOT NULL,
    "Qty" INTEGER NOT NULL,
    "DealPrice" REAL NOT NULL,
    CONSTRAINT "FK_OrderDealDetailTable_OrderTable_OrderID" FOREIGN KEY ("OrderID") REFERENCES "OrderTable" ("OrderID"),
    CONSTRAINT "FK_OrderDealDetailTable_StockDealTable_StockDealID" FOREIGN KEY ("StockDealID") REFERENCES "StockDealTable" ("StockDealID") ON DELETE CASCADE
);

CREATE TABLE "OrderItemDetailTable" (
    "OrderItemDetailID" INTEGER NOT NULL CONSTRAINT "PK_OrderItemDetailTable" PRIMARY KEY AUTOINCREMENT,
    "OrderID" INTEGER NOT NULL,
    "StockItemID" INTEGER NOT NULL,
    "Qty" INTEGER NOT NULL,
    "UnitPrice" REAL NOT NULL,
    "DiscountID" INTEGER NULL,
    "DiscountAmount" REAL NOT NULL,
    CONSTRAINT "FK_OrderItemDetailTable_DiscountTable_DiscountID" FOREIGN KEY ("DiscountID") REFERENCES "DiscountTable" ("DiscountID"),
    CONSTRAINT "FK_OrderItemDetailTable_OrderTable_OrderID" FOREIGN KEY ("OrderID") REFERENCES "OrderTable" ("OrderID"),
    CONSTRAINT "FK_OrderItemDetailTable_StockItemTable_StockItemID" FOREIGN KEY ("StockItemID") REFERENCES "StockItemTable" ("StockItemID")
);

CREATE INDEX "IX_CartItemDetailTable_StockItemId" ON "CartItemDetailTable" ("StockItemId");

CREATE INDEX "IX_OrderDealDetailTable_OrderID" ON "OrderDealDetailTable" ("OrderID");

CREATE INDEX "IX_OrderDealDetailTable_StockDealID" ON "OrderDealDetailTable" ("StockDealID");

CREATE INDEX "IX_OrderItemDetailTable_DiscountID" ON "OrderItemDetailTable" ("DiscountID");

CREATE INDEX "IX_OrderItemDetailTable_OrderID" ON "OrderItemDetailTable" ("OrderID");

CREATE INDEX "IX_OrderItemDetailTable_StockItemID" ON "OrderItemDetailTable" ("StockItemID");

CREATE INDEX "IX_OrderTable_DeliveryAddress_UserAddressID" ON "OrderTable" ("DeliveryAddress_UserAddressID");

CREATE INDEX "IX_OrderTable_OrderStatusID" ON "OrderTable" ("OrderStatusID");

CREATE INDEX "IX_OrderTable_OrderTypeID" ON "OrderTable" ("OrderTypeID");

CREATE INDEX "IX_StockDealDetailTable_StockDealID" ON "StockDealDetailTable" ("StockDealID");

CREATE INDEX "IX_StockDealDetailTable_StockItemID" ON "StockDealDetailTable" ("StockItemID");

CREATE INDEX "IX_StockDealDetailTable_VisibleStatusID" ON "StockDealDetailTable" ("VisibleStatusID");

CREATE INDEX "IX_StockDealTable_VisibleStatusID" ON "StockDealTable" ("VisibleStatusID");

CREATE INDEX "IX_StockItemCategoryTable_VisibleStatusID" ON "StockItemCategoryTable" ("VisibleStatusID");

CREATE INDEX "IX_StockItemIngredientTable_StockItemID" ON "StockItemIngredientTable" ("StockItemID");

CREATE INDEX "IX_StockItemReviewTable_ReviewBy_UserID" ON "StockItemReviewTable" ("ReviewBy_UserID");

CREATE INDEX "IX_StockItemReviewTable_StockItemID" ON "StockItemReviewTable" ("StockItemID");

CREATE INDEX "IX_StockItemTable_OrderTypeID" ON "StockItemTable" ("OrderTypeID");

CREATE INDEX "IX_StockItemTable_StockItemCategoryID" ON "StockItemTable" ("StockItemCategoryID");

CREATE INDEX "IX_StockItemTable_VisibleStatusID" ON "StockItemTable" ("VisibleStatusID");

CREATE INDEX "IX_StockMenuItemTable_StockItemID" ON "StockMenuItemTable" ("StockItemID");

CREATE INDEX "IX_StockMenuItemTable_StockMenuCategoryID" ON "StockMenuItemTable" ("StockMenuCategoryID");

CREATE INDEX "IX_StockMenuItemTable_VisibleStatusID" ON "StockMenuItemTable" ("VisibleStatusID");

CREATE INDEX "IX_TableReservationTable_ProccessBy_UserID" ON "TableReservationTable" ("ProccessBy_UserID");

CREATE INDEX "IX_TableReservationTable_Reservation_UserID" ON "TableReservationTable" ("Reservation_UserID");

CREATE INDEX "IX_TableReservationTable_ReservationStatudID" ON "TableReservationTable" ("ReservationStatudID");

CREATE INDEX "IX_UserAddressTable_AddressTypeID" ON "UserAddressTable" ("AddressTypeID");

CREATE INDEX "IX_UserAddressTable_UserID" ON "UserAddressTable" ("UserID");

CREATE INDEX "IX_UserAddressTable_VisibleStatusID" ON "UserAddressTable" ("VisibleStatusID");

CREATE INDEX "IX_UserPasswordRecoveryTable_UserID" ON "UserPasswordRecoveryTable" ("UserID");

CREATE INDEX "IX_UserTable_GenderID" ON "UserTable" ("GenderID");

CREATE INDEX "IX_UserTable_UserStatusID" ON "UserTable" ("UserStatusID");

CREATE INDEX "IX_UserTable_UserTypeID" ON "UserTable" ("UserTypeID");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261120081703_InitialSqlite', '10.0.10');

COMMIT;

