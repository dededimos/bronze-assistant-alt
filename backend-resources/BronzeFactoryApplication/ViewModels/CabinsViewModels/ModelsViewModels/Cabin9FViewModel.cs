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
    public partial class Cabin9FViewModel : CabinViewModel<Cabin9F>
    {
        private readonly Constraints9FViewModel constraints;
        public override Constraints9FViewModel Constraints => constraints;

        private readonly Parts9FViewModel parts;
        public override Parts9FViewModel Parts => parts;

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


        public Cabin9FViewModel(Constraints9FViewModel constraints9FVM, Parts9FViewModel partsVM) 
            : base(constraints9FVM, partsVM)
        {
            constraints = constraints9FVM;
            parts = partsVM;
        }
#nullable disable
        public Cabin9FViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as Cabin9F ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(Cabin9F)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}
