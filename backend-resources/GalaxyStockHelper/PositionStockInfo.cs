namespace GalaxyStockHelper
{
    public class PositionStockInfo
    {
        /// <summary>
        /// The Type of the Wharehouse RF, Apothiki3 e.t.c.
        /// </summary>
        public string WharehouseType { get; set; } = string.Empty;
        /// <summary>
        /// The Site where the item is located (ex. Magoula Factory) 
        /// </summary>
        public string Site { get; set; } = string.Empty;
        /// <summary>
        /// The Zone that item is being kept (ex. Pano Ktirio , Ergastirio1 e.t.c.)
        /// </summary>
        public string Zone { get; set; } = string.Empty;
        /// <summary>
        /// The Quantity of the Item in the Current Position
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// The Aisle where the Item is Being kept
        /// </summary>
        public string Aisle { get; set; } = string.Empty;
        /// <summary>
        /// The Position in the Aisle where the Item is being kept
        /// </summary>
        public string Position { get; set; } = string.Empty;
    }

}
