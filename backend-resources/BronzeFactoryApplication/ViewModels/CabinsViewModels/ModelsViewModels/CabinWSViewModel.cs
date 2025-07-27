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
    public partial class CabinWSViewModel : CabinViewModel<CabinWS>
    {
        private readonly ConstraintsWSViewModel constraints;
        public override ConstraintsWSViewModel Constraints => constraints;

        private readonly PartsWSViewModel parts;
        public override PartsWSViewModel Parts => parts;

        public CabinWSViewModel(ConstraintsWSViewModel constraintsWSVM, PartsWSViewModel partsVM) 
            : base(constraintsWSVM, partsVM)
        {
            constraints = constraintsWSVM;
            parts = partsVM;
        }
#nullable disable
        public CabinWSViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as CabinWS ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(CabinWS)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }

        [RelayCommand]
        private void Test123()
        {
            Log.Information("Handle:{handle}", Parts?.Handle?.Code);
            Log.Information("Closer:{closer}", Parts?.SelectedCloser?.Code);
        }
    }
}
