IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetStockMutationReport')
    DROP PROCEDURE sp_GetStockMutationReport
GO

CREATE PROCEDURE sp_GetStockMutationReport
    @StartDate DATETIME,
    @EndDate   DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.Name                 AS ProductName,
        sm.Type                AS MutationType,
        COUNT(*)               AS TransactionCount,
        SUM(sm.Quantity)       AS TotalQuantity,
        CAST(sm.CreatedAt AS DATE) AS MutationDate
    FROM StockMutations sm
    INNER JOIN Products p ON p.Id = sm.ProductId
    WHERE sm.CreatedAt BETWEEN @StartDate AND @EndDate
    GROUP BY p.Name, sm.Type, CAST(sm.CreatedAt AS DATE)
    ORDER BY MutationDate DESC, p.Name;
END
GO
