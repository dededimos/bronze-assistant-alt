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
    public partial class CabinWViewModel : CabinViewModel<CabinW>
    {
        private readonly ConstraintsWViewModel constraints;
        public override ConstraintsWViewModel Constraints => constraints;

        private readonly PartsWViewModel parts;
        public override PartsWViewModel Parts => parts;

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
        public CabinWViewModel(ConstraintsWViewModel constraintsWVM, PartsWViewModel partsVM) 
            : base(constraintsWVM, partsVM)
        {
            constraints = constraintsWVM;
            parts = partsVM;
        }
#nullable disable
        public CabinWViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as CabinW ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(CabinW)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}
