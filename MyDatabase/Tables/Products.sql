CREATE TABLE [dbo].[Products]
(
	[Id] bigint NOT NULL PRIMARY KEY,
	[Sku] varchar(255),
	[Name] varchar(255),
	[EAN] bigint,
	[Producer_name] varchar(255),
	[Category] varchar(255),
	[Is_wire] bit,
	[Shipping] varchar(255),
	[Available] bit,
	[Is_vendor] bit,
	[Default_image] varchar(1000),
)
