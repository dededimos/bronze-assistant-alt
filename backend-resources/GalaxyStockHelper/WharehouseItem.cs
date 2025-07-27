namespace GalaxyStockHelper
{
    public class WharehouseItem
    {
        public string Code { get; set; } = string.Empty;
        public string FullCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ItemCategory { get; set; } = string.Empty;
        public string Attribute1Description { get; set; } = string.Empty;
        public string Attribute2Description { get; set; } = string.Empty;
        public string Attribute3Description { get; set; } = string.Empty;

        public string Attribute1Code { get; set; } = string.Empty;
        public string Attribute2Code { get; set; } = string.Empty;
        public string Attribute3Code { get; set; } = string.Empty;

        public decimal Attribute1Decimal { get; set; }
        public decimal Attribute2Decimal { get; set; }
        public decimal Attribute3Decimal { get; set; }

        public List<PositionStockInfo> StockInfo { get; set; } = [];

        public decimal TotalStock { get => StockInfo.Sum(i => i.Quantity); }
        public decimal TotalStockWithoutRedunduncies { get => StockInfo.Where(i=>i.Position != "MB" && i.Position != "E1000" && i.Position != "E2000" && i.Position != "E3000").Sum(i => i.Quantity); }
        public string ItemPositionsString { get => string.Join(" , ", StockInfo.Select(i => i.Position)); }
    }

}
