IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetLowStockProducts')
    DROP PROCEDURE sp_GetLowStockProducts
GO

CREATE PROCEDURE sp_GetLowStockProducts
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.Id           AS ProductId,
        p.Name         AS ProductName,
        p.SKU          AS SKU,
        s.Quantity     AS CurrentQuantity,
        p.MinStock     AS MinStock
    FROM Products p
    INNER JOIN Stocks s ON s.ProductId = p.Id
    WHERE p.IsActive = 1
      AND s.Quantity < p.MinStock
    ORDER BY (p.MinStock - s.Quantity) DESC;
END
GO
