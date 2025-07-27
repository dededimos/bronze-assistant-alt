using BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ShowerEnclosuresModelsLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ModelsViewModels
{
    public partial class CabinVAViewModel : CabinViewModel<CabinVA>
    {
        private readonly ConstraintsVAViewModel constraints;
        public override ConstraintsVAViewModel Constraints => constraints;

        private readonly PartsVAViewModel parts;
        public override PartsVAViewModel Parts => parts;

        public CabinVAViewModel(ConstraintsVAViewModel constraintsVAVM, PartsVAViewModel partsVM)
            : base(constraintsVAVM, partsVM)
        {
            constraints = constraintsVAVM;
            parts = partsVM;
        }
#nullable disable
        public CabinVAViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as CabinVA ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(CabinVA)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }

        [RelayCommand]
        private void Test123()
        {
            //Log.Information("Hinge : {hinge}", parts?.Hinge?.Code);
            Log.Information("SupportBar : {supportbar}", parts?.SupportBar?.Code);
            //Log.Information("Radius : {radius}mm", constraints?.CornerRadiusTopEdge);
            Log.Information("Handle : {handle}mm", parts?.Handle?.Code);
            Log.Information("WallFixer : {fixer}mm", parts?.WallSideFixer?.Code);
            //Log.Information("WallFixer2 : {fixer}mm", parts?.WallFixer2?.Code);
            Log.Information("BottomFixer : {fixer}mm", parts?.BottomFixer?.Code);
            //Log.Information("CloseProfile : {closer}mm", parts?.CloseProfile?.Code);
            Log.Information("Strip : {strip}mm", parts?.CloseStrip?.Code);
        }
    }
}
