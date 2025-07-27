using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels
{
    public partial class DrawContainerOptionsVm : BaseViewModel, IEditorViewModel<DrawContainerOptions>
    {
        private DrawContainerOptions model = new();
        private DrawContainerOptions modelDefaults = new();

        public double ContainerLength
        {
            get => model.ContainerLength;
            set => SetProperty(model.ContainerLength, value, model, (m, v) => m.ContainerLength = v, nameof(ContainerLength));
        }
        public double ContainerHeight
        {
            get => model.ContainerHeight;
            set => SetProperty(model.ContainerHeight, value, model, (m, v) => m.ContainerHeight = v, nameof(ContainerHeight));
        }
        public double ContainerPaddingTop
        {
            get => model.ContainerMarginTop;
            set
            {
                if (SetProperty(model.ContainerMarginTop, value, model, (m, v) => m.ContainerMarginTop = v, nameof(ContainerPaddingTop)))
                {
                    OnPropertyChanged(nameof(ContainerPadding));
                }
            }
        }
        public double ContainerPaddingBottom
        {
            get => model.ContainerMarginBottom;
            set 
            {
                if (SetProperty(model.ContainerMarginBottom, value, model, (m, v) => m.ContainerMarginBottom = v, nameof(ContainerPaddingBottom)))
                {
                    OnPropertyChanged(nameof(ContainerPadding));
                }
            }
        }
        public double ContainerPaddingRight
        {
            get => model.ContainerMarginRight;
            set
            {
                if (SetProperty(model.ContainerMarginRight, value, model, (m, v) => m.ContainerMarginRight = v, nameof(ContainerPaddingRight)))
                {
                    OnPropertyChanged(nameof(ContainerPadding));
                }
            }
        }
        public double ContainerPaddingLeft
        {
            get => model.ContainerMarginLeft;
            set 
            {
                if (SetProperty(model.ContainerMarginLeft, value, model, (m, v) => m.ContainerMarginLeft = v, nameof(ContainerPaddingLeft)))
                {
                    OnPropertyChanged(nameof(ContainerPadding));
                }
            }
        }
        public double MaxDimensionsDepictedToScale
        {
            get => model.MaxDimensionDepictedToScale;
            set => SetProperty(model.MaxDimensionDepictedToScale, value, model, (m, v) => m.MaxDimensionDepictedToScale = v, nameof(MaxDimensionsDepictedToScale));
        }
        public Thickness ContainerPadding => new(ContainerPaddingLeft,
                                                ContainerPaddingTop,
                                                ContainerPaddingRight,
                                                ContainerPaddingBottom);

        public DrawContainerOptions CopyPropertiesToModel(DrawContainerOptions model)
        {
            model.ContainerLength = this.ContainerLength;
            model.ContainerHeight = this.ContainerHeight;
            model.ContainerMarginTop = this.ContainerPaddingTop;
            model.ContainerMarginBottom = this.ContainerPaddingBottom;
            model.ContainerMarginRight = this.ContainerPaddingRight;
            model.ContainerMarginLeft = this.ContainerPaddingLeft;
            model.MaxDimensionDepictedToScale = this.MaxDimensionsDepictedToScale;
            return model;
        }
        public DrawContainerOptions GetModel()
        {
            return model;
        }
        public void SetModel(DrawContainerOptions model)
        {
            this.model = model;
            OnPropertyChanged("");
        }
        public void SetDefaults(DrawContainerOptions defaults)
        {
            modelDefaults = defaults.GetDeepClone();
        }

        [RelayCommand]
        private void ResetToDefaults()
        {
            this.model = modelDefaults.GetDeepClone();
            OnPropertyChanged("");
        }
    }
}
