using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class EditCabinConstraintsModalViewModel : ModalViewModel
    {
        [ObservableProperty]
        private ConstraintsViewModel? constraints;

        public EditCabinConstraintsModalViewModel()
        {
            
        }

        public void SetConstraints(CabinViewModel cabin)
        {
            Title = cabin.Model?.ToString().TryTranslateKey() ?? "Undefined - Model";
            this.Constraints = cabin.Constraints;
        }
    }
}
