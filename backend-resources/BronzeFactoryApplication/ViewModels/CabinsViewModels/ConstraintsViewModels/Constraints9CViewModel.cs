using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class Constraints9CViewModel : ConstraintsViewModel<Constraints9C>
    {
        public IEnumerable<int> AllowableSerigraphyLengths
        {
            get => ConstraintsObject?.AllowableSerigraphyLengths ?? Array.Empty<int>();
        }

        public int? SealerL0LengthCorrection
        {
            get => ConstraintsObject?.SealerL0LengthCorrection;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.SealerL0LengthCorrection != value)
                {
                    ConstraintsObject.SealerL0LengthCorrection = value ?? 0;
                    OnPropertyChanged(nameof(SealerL0LengthCorrection));
                }
            }
        }
        /// <summary>
        /// Always zero , Setter is Empty
        /// </summary>
        public override int FinalHeightCorrection { get => 0; set { } }

        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as Constraints9C ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(Constraints9C)}");
            //Inform all Changed in the Cabin ViewModel
            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}
