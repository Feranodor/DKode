CREATE PROCEDURE [dbo].[InsertPrice]
@ID varchar(255),
@SKU varchar(255),
@price money
AS
	INSERT INTO [Prices] (Id, Sku, Price)
                                       VALUES (@ID, @SKU, @price)
RETURN 0
