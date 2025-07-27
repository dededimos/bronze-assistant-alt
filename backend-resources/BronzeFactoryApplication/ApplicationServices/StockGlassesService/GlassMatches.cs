using MongoDB.Driver;

namespace BronzeFactoryApplication.ApplicationServices.StockGlassesService
{
    public class GlassMatches
    {
        public CabinIdentifier ConcerningIdentifier { get; set; }
        public Glass ConcerningGlass { get; set; }
        public IEnumerable<StockedGlassRow> MatchedGlasses { get; set; }
        public bool HasMatches { get => MatchedGlasses.Any(); }

        public GlassMatches(CabinIdentifier concerningIdentifier, Glass concerningGlass, IEnumerable<StockedGlassRow> matchedGlasses)
        {
            ConcerningIdentifier = concerningIdentifier;
            ConcerningGlass = concerningGlass;
            MatchedGlasses = matchedGlasses;
        }
    }
}
