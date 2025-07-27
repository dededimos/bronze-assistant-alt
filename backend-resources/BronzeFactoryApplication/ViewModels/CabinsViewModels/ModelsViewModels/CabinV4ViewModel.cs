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
    public partial class CabinV4ViewModel : CabinViewModel<CabinV4>
    {
        private readonly ConstraintsV4ViewModel constraints;
        public override ConstraintsV4ViewModel Constraints => constraints;

        private readonly PartsV4ViewModel parts;
        public override PartsV4ViewModel Parts => parts;

        public CabinV4ViewModel(ConstraintsV4ViewModel constraintsV4VM, PartsV4ViewModel partsVM) 
            : base(constraintsV4VM, partsVM)
        {
            constraints = constraintsV4VM;
            parts = partsVM;
        }
#nullable disable
        public CabinV4ViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as CabinV4 ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(CabinV4)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}
