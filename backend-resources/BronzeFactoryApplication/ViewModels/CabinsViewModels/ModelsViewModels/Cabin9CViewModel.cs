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
    public partial class Cabin9CViewModel : CabinViewModel<Cabin9C>
    {
        private readonly Constraints9CViewModel constraints;
        public override Constraints9CViewModel Constraints => constraints;

        private readonly Parts9CViewModel parts;
        public override Parts9CViewModel Parts => parts;

        public Cabin9CViewModel(Constraints9CViewModel constraints9CVM, Parts9CViewModel partsVM) 
            : base(constraints9CVM, partsVM)
        {
            constraints = constraints9CVM;
            parts = partsVM;
        }
#nullable disable
        public Cabin9CViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as Cabin9C ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(Cabin9C)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}
