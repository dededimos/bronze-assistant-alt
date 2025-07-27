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
    public partial class CabinNPViewModel : CabinViewModel<CabinNP>
    {
        private readonly ConstraintsNPViewModel constraints;
        public override ConstraintsNPViewModel Constraints => constraints;

        private readonly PartsNPViewModel parts;
        public override PartsNPViewModel Parts => parts;


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

        public CabinNPViewModel(ConstraintsNPViewModel constraintsNPVM, PartsNPViewModel partsVM) 
            : base(constraintsNPVM, partsVM)
        {
            constraints = constraintsNPVM;
            parts = partsVM;
        }
#nullable disable
        public CabinNPViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as CabinNP ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(CabinNP)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}
