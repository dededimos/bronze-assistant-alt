using BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels;
using CommunityToolkit.Mvvm.Messaging;
using ShowerEnclosuresModelsLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ModelsViewModels
{
    public partial class CabinVFViewModel : CabinViewModel<CabinVF>
    {
        private readonly ConstraintsVFViewModel constraints;
        public override ConstraintsVFViewModel Constraints => constraints;

        private readonly PartsVFViewModel parts;
        public override PartsVFViewModel Parts => parts;

        public override CabinThicknessEnum? Thicknesses
        {
            get => base.Thicknesses;
            set
            {
                base.Thicknesses = value switch
                {
                    CabinThicknessEnum.Thick6mm8mm => CabinThicknessEnum.Thick8mm,
                    CabinThicknessEnum.Thick8mm10mm => CabinThicknessEnum.Thick10mm,
                    _ => value,
                };
            }
        }
        public CabinVFViewModel(ConstraintsVFViewModel constraintsVFVM, PartsVFViewModel partsVM) 
            : base(constraintsVFVM, partsVM)
        {
            constraints = constraintsVFVM;
            parts = partsVM;
        }
#nullable disable
        public CabinVFViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as CabinVF ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(CabinVF)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}
