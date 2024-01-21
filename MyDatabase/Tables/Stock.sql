CREATE TABLE [dbo].[Stock]
(
	[Id] bigint NOT NULL PRIMARY KEY,
	[Sku] varchar(255),
	[Unit] varchar(255),
	[Qty] float,
	[Manufacturer_name] varchar(255),
	[Manufacturer_ref_num] varchar(255),
	[Shipping] varchar(255),
	[Shipping_cost] money,
)
