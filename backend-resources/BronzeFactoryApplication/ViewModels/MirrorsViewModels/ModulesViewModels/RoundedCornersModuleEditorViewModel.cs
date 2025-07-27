using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels
{
    public partial class RoundedCornersModuleEditorViewModel : MirrorModuleInfoBaseViewModel, IEditorViewModel<RoundedCornersModuleInfo>
    {
        public RoundedCornersModuleEditorViewModel()
        {
            model = new();
            SetBaseProperties(model);
        }
        protected RoundedCornersModuleInfo model;

        public double TopLeft
        {
            get => model.TopLeft;
            set => SetProperty(model.TopLeft, value, model, (m, v) => model.TopLeft = v);
        }

        public double TopRight
        {
            get => model.TopRight;
            set => SetProperty(model.TopRight, value, model, (m, v) => model.TopRight = v);
        }

        public double BottomLeft
        {
            get => model.BottomLeft;
            set => SetProperty(model.BottomLeft, value, model, (m, v) => model.BottomLeft = v);
        }

        public double BottomRight
        {
            get => model.BottomRight;
            set => SetProperty(model.BottomRight, value, model, (m, v) => model.BottomRight = v);
        }

        public RoundedCornersModuleInfo CopyPropertiesToModel(RoundedCornersModuleInfo model)
        {
            model.TopLeft = this.TopLeft;
            model.TopRight = this.TopRight;
            model.BottomLeft = this.BottomLeft;
            model.BottomRight = this.BottomRight;
            return model;
        }

        public RoundedCornersModuleInfo GetModel()
        {
            return model.GetDeepClone();
        }

        public void SetModel(RoundedCornersModuleInfo model)
        {
            SetBaseProperties(model);
            this.model = model.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }
    }
}
