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
    public partial class Cabin9AViewModel : CabinViewModel<Cabin9A>
    {
        private readonly Constraints9AViewModel constraints;
        public override Constraints9AViewModel Constraints => constraints;

        private readonly Parts9AViewModel parts;
        public override Parts9AViewModel Parts => parts;

        public Cabin9AViewModel(Constraints9AViewModel constraints9AVM, Parts9AViewModel partsVM) 
            : base(constraints9AVM, partsVM)
        {
            constraints = constraints9AVM;
            parts = partsVM;
        }
#nullable disable
        public Cabin9AViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as Cabin9A ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(Cabin9A)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts,cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}
