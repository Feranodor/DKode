CREATE TABLE [dbo].[Stock]
(
	[Id] bigint NOT NULL PRIMARY KEY,
	[Sku] varchar(255),
	[Unit] varchar(255),
	[Qty] float,
	[Shipping_cost] money,
)
