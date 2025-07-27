namespace BronzeArtWebApplication.Shared.Helpers
{
    public static class RoutesStash
    {
        /// <summary>
        /// The Route to the Main Page
        /// </summary>
        public const string AccessoriesMain = "/Accessories";
        /// <summary>
        /// The Route to the AccessoryCard
        /// </summary>
        public const string DetailedAccessoryCard = $"{AccessoriesMain}/AccessoryCard";
        public const string BasketPage = $"{AccessoriesMain}/QuoteBasket";

        public const string CodeParamName = "code" ;
        public const string FinishParamName = "finish";
        public const string SearchTermCodeParamName = "search";

        public const string FinishFilterParamName = "finishFilter";
        public const string SecondaryTypeFilterParamName = "secondaryTypeFilter";
        public const string SeriesFilterParamName = "seriesFilter";
    }
}
