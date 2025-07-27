using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class SynthesisDrawModalViewModel : ModalViewModel
    {
        [ObservableProperty]
        private SynthesisDrawViewModel? synthesisDraw;

        public SynthesisDrawModalViewModel()
        {
            Title = "lngDraw".TryTranslateKey();
        }

        public void SetDraws(SynthesisDrawViewModel synthesisDraw)
        {
            this.SynthesisDraw = synthesisDraw;
        }
    }
}
