IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetPriceTrend')
    DROP PROCEDURE sp_GetPriceTrend
GO

CREATE PROCEDURE sp_GetPriceTrend
    @Keyword NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ProductName,
        CAST(ScrapedAt AS DATE) AS [Date],
        AVG(Price)              AS AvgPrice,
        MIN(Price)              AS MinPrice,
        MAX(Price)              AS MaxPrice
    FROM PriceHistory
    WHERE ProductName LIKE '%' + @Keyword + '%'
    GROUP BY ProductName, CAST(ScrapedAt AS DATE)
    ORDER BY [Date] DESC;
END
GO
