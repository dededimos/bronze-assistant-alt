namespace GalaxyStockHelper
{
    public class GalaxyStockServiceFilters
    {
        /// <summary>
        /// The number of items to skip from the database (ex. 0)
        /// </summary>
        public int SkipFilter { get; set; } = 0;
        /// <summary>
        /// The number of items to take from the database (ex. 1000)
        /// </summary>
        public int TakeFilter { get; set; } = 1000000;
        /// <summary>
        /// Which field to search for (ex. Code , Description e.t.c.)
        /// </summary>
        public string SearchFilterField { get; set; } = "Code";
        /// <summary>
        /// The Value to search for (ex. 1234 , 1234% , 1234* e.t.c.)
        /// </summary>
        public string SearchFilterValue { get; set; } = string.Empty;
        /// <summary>
        /// The Comparison Filter to use (ex. Equal , Contains , StartsWith e.t.c.)
        /// </summary>
        public GalaxyComparisonFilter SearchFilterOperator { get; set; } = GalaxyComparisonFilter.Equal;
        /// <summary>
        /// The Filter to Sort the Results (ex. Code , Description e.t.c.)
        /// </summary>
        public string SortByFilter { get; set; } = "Code";
    }

}
