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
    public partial class CabinDBViewModel : CabinViewModel<CabinDB>
    {
        private readonly ConstraintsDBViewModel constraints;
        public override ConstraintsDBViewModel Constraints => constraints;

        private readonly PartsDBViewModel parts;
        public override PartsDBViewModel Parts => parts;

        public override CabinThicknessEnum? Thicknesses 
        { 
            get => base.Thicknesses;
            //Set to 8mm when 8-10 is selected from anywhere
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

        public CabinDBViewModel(ConstraintsDBViewModel constraintsDBVM, PartsDBViewModel partsVM) 
            : base(constraintsDBVM, partsVM)
        {
            constraints = constraintsDBVM;
            parts = partsVM;
        }
#nullable disable
        public CabinDBViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as CabinDB ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(CabinDB)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }

    }
}
