﻿CREATE PROCEDURE [dbo].[IsStockTableEmpty]
AS
	SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM [Stock]) THEN 0 ELSE 1 END AS BIT)
RETURN 0
