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
    public partial class CabinVSViewModel : CabinViewModel<CabinVS>
    {
        private readonly ConstraintsVSViewModel constraints;
        public override ConstraintsVSViewModel Constraints => constraints;

        private readonly PartsVSViewModel parts;
        public override PartsVSViewModel Parts => parts;

        public CabinVSViewModel(ConstraintsVSViewModel constraintsVSVM, PartsVSViewModel partsVM)
            : base(constraintsVSVM, partsVM)
        {
            constraints = constraintsVSVM;
            parts = partsVM;
        }
#nullable disable
        public CabinVSViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as CabinVS ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(CabinVS)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}
