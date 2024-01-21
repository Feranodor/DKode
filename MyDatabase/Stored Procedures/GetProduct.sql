CREATE PROCEDURE [dbo].[GetProduct]
	@SKU varchar(255)
AS
	SELECT
       p.[Name]--a. Product Name
      ,p.[EAN]--b. EAN
      ,p.[Producer_name] --c. Supplier name 
      ,p.[Category]--d. Category
      ,p.[Default_image]--e. Image URL
	   ,s.[Unit]--g. Logistic unit
      ,s.[Qty]--f. Stock level
      ,s.[Shipping_cost]--i. Despatch cost
      ,c.[Price]--h. Net cost
  FROM [Products] as p 
  INNER JOIN [Stock] as s on p.sku=s.sku
  INNER JOIN [Prices] as c on p.sku=c.sku
  where p.Sku = @SKU
RETURN 0
