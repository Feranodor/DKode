CREATE PROCEDURE [dbo].[InsertStock]
   @product_id  bigint,
   @sku varchar(255),
   @unit  varchar(255),
   @qty float,
   @shipping varchar(255),
   @shipping_cost money
AS
	INSERT INTO [Stock] (Id, Sku, Unit, Qty, Shipping_cost)
                             VALUES (@product_id, @sku, @unit, @qty, @shipping_cost)
RETURN 0
