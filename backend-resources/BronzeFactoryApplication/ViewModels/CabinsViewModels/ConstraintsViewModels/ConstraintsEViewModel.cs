
namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class ConstraintsEViewModel : ConstraintsViewModel<ConstraintsE>
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

        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as ConstraintsE ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(ConstraintsE)}");
            //Inform all Changed in the Cabin ViewModel
            
            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}
