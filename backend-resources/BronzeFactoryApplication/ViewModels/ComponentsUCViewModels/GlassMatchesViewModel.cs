using BronzeFactoryApplication.ApplicationServices.StockGlassesService;
using System.Collections.ObjectModel;

namespace BronzeFactoryApplication.ViewModels.ComponentsUCViewModels
{
    /// <summary>
    /// A ViewModel Representing Glass matches for a Certain Glass from a Certain Cabin
    /// </summary>
    public partial class GlassMatchesViewModel : BaseViewModel
    {
        /// <summary>
        /// The Identifier of the Cabin for which there are Matched Glasses
        /// </summary>
        [ObservableProperty]
        private CabinIdentifier concerningIdentifier;
        /// <summary>
        /// The Glass for which there are Potential Matched Glasses to swap it
        /// </summary>
        [ObservableProperty]
        private Glass concerningGlass;
        /// <summary>
        /// The Glasses Matching the Concerning Glass for swapping
        /// </summary>
        public ObservableCollection<StockedGlassViewModel> MatchedGlasses { get; } = new();
        /// <summary>
        /// Weather there are any Matches for the Concerning Glass
        /// </summary>
        public bool HasMatches { get => MatchedGlasses.Any(); }


        public GlassMatchesViewModel(GlassMatches model)
        {
            this.concerningIdentifier = model.ConcerningIdentifier;
            this.concerningGlass = model.ConcerningGlass;
            foreach (var matchedGlass in model.MatchedGlasses)
            {
                MatchedGlasses.Add(new(matchedGlass));
            }
        }
    }
}
