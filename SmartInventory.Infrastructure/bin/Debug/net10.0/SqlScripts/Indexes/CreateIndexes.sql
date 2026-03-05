-- High frequency SKU lookup (filtered unique index via EF migration, also defined here for reference)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_SKU' AND object_id = OBJECT_ID('Products'))
    CREATE UNIQUE INDEX IX_Products_SKU ON Products(SKU) WHERE IsActive = 1;

-- Stock mutation report by product + date
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_StockMutations_ProductId_CreatedAt' AND object_id = OBJECT_ID('StockMutations'))
    CREATE INDEX IX_StockMutations_ProductId_CreatedAt ON StockMutations(ProductId, CreatedAt DESC);

-- Price history by keyword + date with covering columns
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PriceHistory_ProductName_ScrapedAt' AND object_id = OBJECT_ID('PriceHistory'))
    CREATE INDEX IX_PriceHistory_ProductName_ScrapedAt ON PriceHistory(ProductName, ScrapedAt DESC) INCLUDE (Price, Source);
