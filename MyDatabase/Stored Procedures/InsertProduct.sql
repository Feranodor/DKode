CREATE PROCEDURE [dbo].[InsertProduct]
	@ID bigint,
	@SKU varchar(255),
	@name varchar(255),
	@EAN bigint,
	@producer_name varchar(255),
	@category varchar(255),
	@is_wire bit,
	@shipping varchar(255),
	@available bit,
	@is_vendor bit,
	@default_image varchar(1000)
AS
	INSERT INTO [Products] (Id, Sku, Name, EAN, Producer_name, Category, Is_wire, Shipping, Available, Is_vendor, Default_image)
                                    VALUES (@ID, @SKU, @name, @EAN, @producer_name, @category, @is_wire, @shipping, @available, @is_vendor, @default_image)
RETURN 0
