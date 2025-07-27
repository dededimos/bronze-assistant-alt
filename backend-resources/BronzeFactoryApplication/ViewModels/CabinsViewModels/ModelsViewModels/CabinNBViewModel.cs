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
    public partial class CabinNBViewModel : CabinViewModel<CabinNB>
    {
        private readonly ConstraintsNBViewModel constraints;
        public override ConstraintsNBViewModel Constraints => constraints;

        private readonly PartsNBViewModel parts;
        public override PartsNBViewModel Parts => parts;

        public override CabinThicknessEnum? Thicknesses
        {
            get => base.Thicknesses;
            set
            {
                base.Thicknesses = value switch
                {
                    CabinThicknessEnum.Thick6mm8mm => CabinThicknessEnum.Thick6mm,
                    CabinThicknessEnum.Thick8mm10mm => CabinThicknessEnum.Thick8mm,
                    _ => value,
                };
            }
        }

        public CabinNBViewModel(ConstraintsNBViewModel constraintsNBVM, PartsNBViewModel partsVM) 
            : base(constraintsNBVM, partsVM)
        {
            constraints = constraintsNBVM;
            parts = partsVM;
        }
#nullable disable
        public CabinNBViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as CabinNB ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(CabinNB)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}
