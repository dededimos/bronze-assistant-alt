using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class ConstraintsNPViewModel : ConstraintsViewModel<ConstraintsNP>
    {
        public int? CornerRadiusTopEdge
        {
            get => ConstraintsObject?.CornerRadiusTopEdge;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.CornerRadiusTopEdge != value)
                {
                    ConstraintsObject.CornerRadiusTopEdge = value ?? 0;
                    OnPropertyChanged(nameof(CornerRadiusTopEdge));
                }
            }
        }

        /// <summary>
        /// The Length of the Structure when itsFolded - This should be Calculatable, but CBA
        /// </summary>
        public int? FoldedLength
        {
            get => ConstraintsObject?.FoldedLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.FoldedLength != value)
                {
                    ConstraintsObject.FoldedLength = value ?? 0;
                    OnPropertyChanged(nameof(FoldedLength));
                }
            }
        }
        public int? DoorsLengthDifference
        {
            get => ConstraintsObject?.DoorsLengthDifference;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.DoorsLengthDifference != value)
                {
                    ConstraintsObject.DoorsLengthDifference = value ?? 0;
                    OnPropertyChanged(nameof(DoorsLengthDifference));
                }
            }
        }
        public int? DoorSealerLengthCorrection
        {
            get => ConstraintsObject?.DoorSealerLengthCorrection;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.DoorSealerLengthCorrection != value)
                {
                    ConstraintsObject.DoorSealerLengthCorrection = value ?? 0;
                    OnPropertyChanged(nameof(DoorSealerLengthCorrection));
                }
            }
        }
        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as ConstraintsNP ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(ConstraintsNP)}");
            //Inform all Changed in the Cabin ViewModel
            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}
