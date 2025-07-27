using BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ModelsViewModels
{
    public partial class CabinWFlipperViewModel : CabinViewModel<CabinWFlipper>
    {
        private readonly ConstraintsWFlipperViewModel constraints;
        public override ConstraintsWFlipperViewModel Constraints => constraints;

        private readonly PartsWFlipperViewModel parts;
        public override PartsWFlipperViewModel Parts => parts;


        public override CabinThicknessEnum? Thicknesses
        {
            get => base.Thicknesses;
            set
            {
                base.Thicknesses = value switch
                {
                    //Even when 8mm is passed convert it to 6mm
                    CabinThicknessEnum.Thick8mm => CabinThicknessEnum.Thick6mm,
                    CabinThicknessEnum.Thick6mm8mm => CabinThicknessEnum.Thick6mm,
                    //Even when 10mm is passed convert it to 8mm
                    CabinThicknessEnum.Thick10mm => CabinThicknessEnum.Thick8mm,
                    CabinThicknessEnum.Thick8mm10mm => CabinThicknessEnum.Thick8mm,
                    _ => value,
                };
            }
        }

        public CabinWFlipperViewModel(ConstraintsWFlipperViewModel constraintsWFlipperVM, PartsWFlipperViewModel partsVM) 
            : base(constraintsWFlipperVM, partsVM)
        {
            constraints = constraintsWFlipperVM;
            parts = partsVM;
        }
#nullable disable
        public CabinWFlipperViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as CabinWFlipper ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(CabinWFlipper)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}
