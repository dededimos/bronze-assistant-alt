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
    public partial class CabinHBViewModel : CabinViewModel<CabinHB>
    {
        private readonly ConstraintsHBViewModel constraints;
        public override ConstraintsHBViewModel Constraints => constraints;

        private readonly PartsHBViewModel parts;
        public override PartsHBViewModel Parts => parts;

        public CabinHBViewModel(ConstraintsHBViewModel constraintsHBVM, PartsHBViewModel partsVM) 
            : base(constraintsHBVM, partsVM)
        {
            constraints = constraintsHBVM;
            parts = partsVM;
        }
#nullable disable
        public CabinHBViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as CabinHB ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(CabinHB)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}
