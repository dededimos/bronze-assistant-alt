using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels
{
    public partial class MirrorBackLidModuleEditorViewModel : MirrorModuleInfoBaseViewModel, IEditorViewModel<MirrorBackLidModuleInfo>
    {
        public MirrorBackLidModuleEditorViewModel()
        {
            model = new();
            SetBaseProperties(model);
        }
        protected MirrorBackLidModuleInfo model;

        public RectangleInfo LidDimensions { get => model.LidDimensions; }

        public double MinDistanceFromSupport
        {
            get => model.MinDistanceFromSupport;
            set => SetProperty(model.MinDistanceFromSupport, value, model, (m, v) => model.MinDistanceFromSupport = v);
        }

        public double MinDistanceFromSandblast
        {
            get => model.MinDistanceFromSandblast;
            set => SetProperty(model.MinDistanceFromSandblast, value, model, (m, v) => model.MinDistanceFromSandblast = v);
        }

        public double MinDistanceFromOtherModules
        {
            get => model.MinDistanceFromOtherModules;
            set => SetProperty(model.MinDistanceFromOtherModules, value, model, (m, v) => model.MinDistanceFromOtherModules = v);
        }


        public MirrorBackLidModuleInfo CopyPropertiesToModel(MirrorBackLidModuleInfo model)
        {
            model.LidDimensions = this.model.LidDimensions.GetDeepClone();
            model.MinDistanceFromSupport = this.MinDistanceFromSupport;
            model.MinDistanceFromSandblast = this.MinDistanceFromSandblast;
            model.MinDistanceFromOtherModules = this.MinDistanceFromOtherModules;
            return model;
        }

        public MirrorBackLidModuleInfo GetModel()
        {
            return model.GetDeepClone();
        }

        public void SetModel(MirrorBackLidModuleInfo model)
        {
            SetBaseProperties(model);
            this.model = model.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }
    }
}
