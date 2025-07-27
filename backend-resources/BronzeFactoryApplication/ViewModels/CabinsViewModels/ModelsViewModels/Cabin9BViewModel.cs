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
    public partial class Cabin9BViewModel : CabinViewModel<Cabin9B>
    {
        private readonly Constraints9BViewModel constraints;
        public override Constraints9BViewModel Constraints => constraints;

        private readonly Parts9BViewModel parts;
        public override Parts9BViewModel Parts => parts;

        public Cabin9BViewModel(Constraints9BViewModel constraints9BVM, Parts9BViewModel partsVM) 
            : base(constraints9BVM, partsVM)
        {
            constraints = constraints9BVM;
            parts = partsVM;
        }
#nullable disable
        public Cabin9BViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as Cabin9B ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(Cabin9B)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}
