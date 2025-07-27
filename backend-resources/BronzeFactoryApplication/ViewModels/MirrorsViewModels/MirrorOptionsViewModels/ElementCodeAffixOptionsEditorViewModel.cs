using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Services.CodeBuldingService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOptionsViewModels
{
    public partial class ElementCodeAffixOptionsEditorViewModel : BaseViewModel , IEditorViewModel<ElementCodeAffixOptions>
    {
        [ObservableProperty]
        private MirrorElementAffixCodeType codeType;

        public bool IsCodeTypeOverriden { get => !string.IsNullOrEmpty(OverrideCodeString); }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCodeTypeOverriden))]
        private string? overrideCodeString;
        [ObservableProperty]
        private string? replacementCodeAffixWhenEmpty = string.Empty;

        public ObservableCollection<MirrorShapeReplacementAffixHelper> ReplacementStringPerShape { get; set; } = [];

        [ObservableProperty]
        private BronzeMirrorShape? shapeToAddToReplacements;
        [ObservableProperty]
        private string? replacementStringToAddToReplacements;

        [RelayCommand]
        private void AddSelectedShapeReplacement()
        {
            if (ShapeToAddToReplacements is null) 
            {
                MessageService.Warning("Please select a Shape First", "Selected Shape is NULL");
                return;
            }
            if (ReplacementStringPerShape.Any(x=>x.MirrorShape == ShapeToAddToReplacements))
            {
                MessageService.Warning($"Shape {ShapeToAddToReplacements} is already in the List , Please remove it first if you want to replace it", "Shape already present in the List");
                return;
            }
            if (string.IsNullOrWhiteSpace(ReplacementStringToAddToReplacements))
            {
                MessageService.Warning("Please insert a replacement String for the selected Shape first", "Shape Replacement string is EMPTY");
                return;
            }
            var replacementStringToAdd = new MirrorShapeReplacementAffixHelper((BronzeMirrorShape)ShapeToAddToReplacements, ReplacementStringToAddToReplacements);
            ReplacementStringPerShape.Add(replacementStringToAdd);
            ShapeToAddToReplacements = null;
            ReplacementStringToAddToReplacements = null;
            OnPropertyChanged(nameof(ReplacementStringPerShape));
        }
        [RelayCommand]
        private void RemoveReplacementString(MirrorShapeReplacementAffixHelper toRemove)
        {
            var removed = ReplacementStringPerShape.Remove(toRemove);
            if (!removed)
            {
                MessageService.Error($"The Replacement String you are trying to remove was not found and could not be Removed", "Removal Error");
                return;
            }
            OnPropertyChanged(nameof(ReplacementStringPerShape));
        }

        [ObservableProperty]
        private int positionOrder;
        [ObservableProperty]
        private int minimumNumberOfCharachters;
        [ObservableProperty]
        private string fillerCharachter = string.Empty;

        [ObservableProperty]
        private bool includeElementPositionCode;
        [ObservableProperty]
        private MirrorElementAffixCodeType elementPositionCodeTypeOption;
        [ObservableProperty]
        private int elementPositionMinimumNumberOfCharachters;
        [ObservableProperty]
        private string elementPositionFillerCharachter = string.Empty;

        public void SetModel(ElementCodeAffixOptions model)
        {
            SuppressPropertyNotifications();
            CodeType = model.CodeType;
            OverrideCodeString = model.OverrideCodeString;
            ReplacementCodeAffixWhenEmpty = model.ReplacementCodeAffixWhenEmpty;
            
            ReplacementStringPerShape.Clear();
            foreach (var item in model.ReplacementCodeAffixBasedOnShape)
            {
                var replacementString = new MirrorShapeReplacementAffixHelper(item.Key, item.Value);
                ReplacementStringPerShape.Add(replacementString);
            }

            PositionOrder = model.PositionOrder;
            MinimumNumberOfCharachters = model.NumberOfCharachters;
            FillerCharachter = model.FillerCharachter.ToString();
            IncludeElementPositionCode = model.IncludeElementPositionCode;
            ElementPositionCodeTypeOption = model.ElementPositionCodeTypeOption;
            ElementPositionMinimumNumberOfCharachters = model.ElementPositionMinimumNumberOfCharachters;
            ElementPositionFillerCharachter = model.ElementPositionFillerCharachter.ToString();
            ResumePropertyNotifications();
            OnPropertyChanged("");
        }

        public ElementCodeAffixOptions CopyPropertiesToModel(ElementCodeAffixOptions model)
        {
            model.CodeType = this.CodeType;
            model.OverrideCodeString = this.OverrideCodeString;
            model.ReplacementCodeAffixWhenEmpty = this.ReplacementCodeAffixWhenEmpty;
            model.ReplacementCodeAffixBasedOnShape = this.ReplacementStringPerShape.ToDictionary(x => x.MirrorShape, x => x.ReplacementAffix);
            model.PositionOrder = this.PositionOrder;
            model.NumberOfCharachters = this.MinimumNumberOfCharachters;
            model.FillerCharachter = this.FillerCharachter.FirstOrDefault(); //default charachter value is /0 which is the empty Equivalent of a string
            model.IncludeElementPositionCode = this.IncludeElementPositionCode;
            model.ElementPositionCodeTypeOption = this.ElementPositionCodeTypeOption;
            model.ElementPositionMinimumNumberOfCharachters = this.ElementPositionMinimumNumberOfCharachters;
            model.ElementPositionFillerCharachter = this.ElementPositionFillerCharachter.FirstOrDefault();
            return model;
        }

        public ElementCodeAffixOptions GetModel()
        {
            return CopyPropertiesToModel(new());
        }
    }
    /// <summary>
    /// Helper object storing a MirrorShape and a Replacement Affix String
    /// </summary>
    public class MirrorShapeReplacementAffixHelper
    {
        public MirrorShapeReplacementAffixHelper(BronzeMirrorShape mirrorShape, string replacementAffix)
        {
            MirrorShape = mirrorShape;
            ReplacementAffix = replacementAffix;
        }
        public BronzeMirrorShape MirrorShape { get; set; }
        public string ReplacementAffix { get; set; }
    }
}
