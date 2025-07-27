using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.Graph.Communications.Calls.Item.KeepAlive;
using MirrorsLib;
using MirrorsLib.Enums;
using MirrorsLib.Helpers;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using System.Collections.ObjectModel;
using System.Linq;
using static MirrorsModelsLibrary.StaticData.MirrorStandards;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels
{
    public partial class MirrorSeriesInfoEditorViewModel : BaseViewModel, IEditorViewModel<MirrorSeriesInfo>
    {
        public MirrorSeriesInfoEditorViewModel(Func<MirrorSynthesisEditorWithDrawViewModel> mirrorEditorVmFactory,
                                               Func<MirrorConstraintsEditorViewModel> constraintsEditorVmFactory,
                                               IWrappedViewsModalsGenerator modalsGenerator)
        {
            MirrorEditor = mirrorEditorVmFactory.Invoke();
            ConstraintsEditor = constraintsEditorVmFactory.Invoke();
            this.modalsGenerator = modalsGenerator;
            //Have to hook this otherwise consumers will not be able to see that the Editor has changed (only wpf ui will know)
            ConstraintsEditor.PropertyChanged += ConstraintsEditor_PropertyChanged;
        }

        private void ConstraintsEditor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ConstraintsEditor));
        }

        private readonly IWrappedViewsModalsGenerator modalsGenerator;
        private readonly MirrorSynthesisEqualityComparer mirrorComparer = new();

        [ObservableProperty]
        private bool isCustomizedMirrorSeries;
        [ObservableProperty]
        private bool allowsTransitionToCustomizedMirror;

        public ObservableCollection<MirrorModificationDescriptor> CustomizationTriggers { get; } = [];
        [ObservableProperty]
        private MirrorElementModification? selectedMirrorElementModification;
        [ObservableProperty]
        private MirrorModificationType? selectedModificationType;

        public ObservableCollection<MirrorSynthesis> StandardMirrors { get; } = [];
        public MirrorSynthesisEditorWithDrawViewModel MirrorEditor { get; }
        private int indexOfUnderEditMirror = -1;

        public MirrorConstraintsEditorViewModel ConstraintsEditor { get; set; }


        [RelayCommand]
        private void AddMirrorCustomizationTrigger()
        {
            if (SelectedMirrorElementModification is null)
            {
                MessageService.Warning($"Please Select a Mirror Element Option for Modification", $"Element Option Not Selected");
                return;
            }
            else if (SelectedModificationType is null)
            {
                MessageService.Warning($"Please select a Modification Type", $"Modification Type not Selected");
                return;
            }
            else if (CustomizationTriggers.Any(t => t.ModificationType == SelectedModificationType && t.Modification == SelectedMirrorElementModification))
            {
                MessageService.Warning($"The Pair you are trying to add is already inside the List of Triggers{Environment.NewLine}{SelectedModificationType}-{SelectedMirrorElementModification}", $"Pair Already Exists");
                return;
            }
            CustomizationTriggers.Add(new((MirrorModificationType)SelectedModificationType, (MirrorElementModification)SelectedMirrorElementModification));
            OnPropertyChanged(nameof(CustomizationTriggers));
            SelectedModificationType = null;
            SelectedMirrorElementModification = null;
        }
        [RelayCommand]
        private void RemoveMirrorCustomizationTrigger(MirrorModificationDescriptor triggerDescriptor)
        {
            bool removed = CustomizationTriggers.Remove(triggerDescriptor);
            if (removed)
            {
                OnPropertyChanged(nameof(CustomizationTriggers));
            }
            else MessageService.Warning($"Unexpected Error the Trigger Pair you are Trying to Remove was not Found in the List of pairs", "Unexpected Error - Trigger Pair Not Found");
        }

        [RelayCommand]
        private void OpenAddMirror()
        {
            if (ConstraintsEditor.ConcerningMirrorShape is null)
            {
                MessageService.Warning($"Please Select a Shape for the Series First", "Shape Not Selected");
                return;
            }
            modalsGenerator.OpenModal(MirrorEditor,
                                      "lngAddMirrorSynthesisToSeries".TryTranslateKeyWithoutError(),
                                      AddMirrorToSeries,
                                      WrappedModalCustomActionButtonOption.AddAndClose);
        }

        [RelayCommand]
        private void OpenEditMirror(MirrorSynthesis mirror)
        {
            indexOfUnderEditMirror = StandardMirrors.IndexOf(mirror);
            if (indexOfUnderEditMirror == -1) { MessageService.Error($"Unexpected Error , Mirror Was not Found in the List of Standard Mirrors", "Mirror not Found"); return; }

            MirrorEditor.MirrorEditor.SetModel(mirror);

            modalsGenerator.OpenModal(MirrorEditor,
                                      "lngEditMirrorSynthesisToSeries".TryTranslateKeyWithoutError(),
                                      EditMirrorOfSeries,
                                      WrappedModalCustomActionButtonOption.SaveAndClose,
                                      false,
                                      CloseIfMirrorHasNotEdits);
        }

        [RelayCommand]
        private void RemoveMirror(MirrorSynthesis mirror)
        {
            if (MessageService.Question($"Remove Mirror :{mirror.Code} ?", "Remove Mirror", "Ok", "Cancel") == MessageBoxResult.OK)
            {
                bool removed = StandardMirrors.Remove(mirror);
                if (removed)
                {
                    OnPropertyChanged(nameof(StandardMirrors));
                }
                else MessageService.Warning($"Unexpected Error , Mirror {mirror.Code} was not Found in the Mirrors of this Series", "Unexpected Error");
            }
        }

        [RelayCommand]
        private void OpenEditConstraints()
        {
            modalsGenerator.OpenModal(ConstraintsEditor,"lngEditConstraints".TryTranslateKeyWithoutError());
        }

        /// <summary>
        /// Used by the Wrapped Vm to Add the Mirror
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void AddMirrorToSeries()
        {
            //Check if the Mirror Has The Correct Shape
            if (ConstraintsEditor.ConcerningMirrorShape != MirrorEditor.MirrorEditor.Shape)
            {
                throw new Exception($"{MirrorEditor.MirrorEditor.Shape} is not a Valid Shape for this Series{Environment.NewLine}{Environment.NewLine}Select {MirrorEditor.MirrorEditor.Shape} as a Series Shape First");
            }
            if ((MirrorEditor.MirrorEditor.Sandblast is null && ConstraintsEditor.AcceptsMirrorsWithoutSandblast is false) 
                && !ConstraintsEditor.SelectedSandblasts.Any(s => s.ElementId == MirrorEditor.MirrorEditor.Sandblast?.ElementId))
            {
                if (MirrorEditor.MirrorEditor.Sandblast is null) throw new Exception($"This Series Does not Accept Mirrors Without Sandblast...");
                else throw new Exception($"Sandblast with Code : {MirrorEditor.MirrorEditor.Sandblast?.Code} is not a Valid Sandblast for this Series{Environment.NewLine}{Environment.NewLine}Add it as a Series Sandblast First");
            }
            if ((MirrorEditor.MirrorEditor.Support is null && ConstraintsEditor.AcceptsMirrorsWithoutSupport is false) 
                && !ConstraintsEditor.SelectedSupports.Any(s => s.ElementId == MirrorEditor.MirrorEditor.Support?.ElementId))
            {
                if (MirrorEditor.MirrorEditor.Support is null) throw new Exception("This Series Does not Accept Mirrors Without Support...");
                else throw new Exception($"Support with Code : {MirrorEditor.MirrorEditor.Support?.Code} is not a Valid Support for this Series{Environment.NewLine}{Environment.NewLine}Add it as a Series Support First");
            }
            if((StandardMirrors.Any(m=> m.Code == MirrorEditor.MirrorEditor.MirrorCode)))
            {
                throw new Exception($"There is already a Standard Mirror with the Same Code ,{Environment.NewLine}{Environment.NewLine}Code: {MirrorEditor.MirrorEditor.MirrorCode}");
            }
            foreach (var obligatoryModule in ConstraintsEditor.SelectedModules.Where(m=> m.IsObligatory))
            {
                if (MirrorEditor.MirrorEditor.Modules.Any(m=>m.Module.ElementId == obligatoryModule.Module.ElementId) == false)
                {
                    throw new Exception($"This Series Requires the Module : {obligatoryModule.Module.Code} to be Added to the Mirror");
                }
            }

            //TODO ADD VALIDATION FOR CUSTOM ELEMENTS (HAVE TO BE IMPLEMENTED FIRST IN THE VIEWMODEL)
            //foreach (var obligatoryCustomElement in ConstraintsEditor.SelectedCustomElements.Where(e=>e.IsObligatory))
            //{
            //    if (MirrorEditor.MirrorEditor.CustomElements.Any(e => e.CustomElement.ElementId == obligatoryCustomElement.CustomElement.ElementId) == false)
            //    {
            //        throw new Exception($"This Series Requires the Custom Element : {obligatoryCustomElement.CustomElement.Code} to be Added to the Mirror");
            //    }

            //}

            //TODO - ADD VALIDATION HERE FOR LIGHTS !!!! (EDITOR DOES NOT HAVE LIGHTS YET)

            //Check if the Mirror Has Errors and do not add otherwise (the Modal will display the exception)
            if (MirrorEditor.MirrorDrawing.HasErrors)
            {
                var errors = string.Join(Environment.NewLine, [.. MirrorEditor.MirrorDrawing.ValidationErrors]);
                throw new Exception(errors);
            }
            var mirror = MirrorEditor.MirrorEditor.GetModel();
            StandardMirrors.Add(mirror);
            OnPropertyChanged(nameof(StandardMirrors));
        }
        private void EditMirrorOfSeries()
        {
            //Check if the Mirror Has The Correct Shape
            if (ConstraintsEditor.ConcerningMirrorShape != MirrorEditor.MirrorEditor.Shape)
            {
                throw new Exception($"{MirrorEditor.MirrorEditor.Shape} is not a Valid Shape for this Series{Environment.NewLine}{Environment.NewLine}Select {MirrorEditor.MirrorEditor.Shape} as a Series Shape First");
            }
            if ((MirrorEditor.MirrorEditor.Sandblast is null && ConstraintsEditor.AcceptsMirrorsWithoutSandblast is false) 
                && !ConstraintsEditor.SelectedSandblasts.Any(s => s.ElementId == MirrorEditor.MirrorEditor.Sandblast?.ElementId))
            {
                if (MirrorEditor.MirrorEditor.Sandblast is null) throw new Exception($"This Series Does not Accept Mirrors Without Sandblast...");
                else throw new Exception($"Sandblast with Code : {MirrorEditor.MirrorEditor.Sandblast?.Code} is not a Valid Sandblast for this Series{Environment.NewLine}{Environment.NewLine}Add it as a Series Sandblast First");
            }
            if ((MirrorEditor.MirrorEditor.Support is null && ConstraintsEditor.AcceptsMirrorsWithoutSupport is false) 
                && !ConstraintsEditor.SelectedSupports.Any(s => s.ElementId == MirrorEditor.MirrorEditor.Support?.ElementId))
            {
                if (MirrorEditor.MirrorEditor.Support is null) throw new Exception("This Series Does not Accept Mirrors Without Support...");
                else throw new Exception($"Support with Code : {MirrorEditor.MirrorEditor.Support?.Code} is not a Valid Support for this Series{Environment.NewLine}{Environment.NewLine}Add it as a Series Support First");
            }
            if (StandardMirrors.Count <= (indexOfUnderEditMirror) || indexOfUnderEditMirror <= -1) throw new Exception("Mirror Under Edit did not store its index Number Correctly before Edits...");

            if (StandardMirrors.Any(m => m.Code == MirrorEditor.MirrorEditor.MirrorCode && StandardMirrors.IndexOf(m) != indexOfUnderEditMirror)) //must be different from the one being edited currently
            {
                throw new Exception($"There is already a Standard Mirror with the Same Code ,{Environment.NewLine}{Environment.NewLine}Code: {MirrorEditor.MirrorEditor.MirrorCode}");
            }


            //TODO - ADD VALIDATION HERE FOR LIGHTS !!!! (EDITOR DOES NOT HAVE LIGHTS YET)

            //Check if the Mirror Has Errors and do not add otherwise (the Modal will display the exception)
            if (MirrorEditor.MirrorDrawing.HasErrors)
            {
                var errors = string.Join(Environment.NewLine, [.. MirrorEditor.MirrorDrawing.ValidationErrors]);
                throw new Exception(errors);
            }

            var editedMirror = MirrorEditor.MirrorEditor.GetModel();
            StandardMirrors[indexOfUnderEditMirror] = editedMirror;
            OnPropertyChanged(nameof(StandardMirrors));
            //Reset the Editor after editting 
            MirrorEditor.MirrorEditor.ResetMirrorCommand.Execute(null);
            //Reset the underEdit Mirror Index
            indexOfUnderEditMirror = -1;
        }
        private bool CloseIfMirrorHasNotEdits()
        {
            if (indexOfUnderEditMirror == -1) return true; //the mirror has already been replaced and the index was reset

            var mirrorReplacement = MirrorEditor.MirrorEditor.GetModel();
            var mirrorUnderEdit = StandardMirrors[indexOfUnderEditMirror];
            var hasEdits = !mirrorComparer.Equals(mirrorReplacement, mirrorUnderEdit);
            if (hasEdits && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
            {
                return false;
            }

            //reset index of under edit mirror
            indexOfUnderEditMirror = -1;
            //reset editor
            MirrorEditor.MirrorEditor.ResetMirrorCommand.Execute(null);
            return true;
        }

        public MirrorSeriesInfo CopyPropertiesToModel(MirrorSeriesInfo model)
        {
            model.StandardMirrors = this.StandardMirrors.Select(m => m.GetDeepClone()).ToList();
            model.IsCustomizedMirrorsSeries = this.IsCustomizedMirrorSeries;
            model.AllowsTransitionToCustomizedMirror = this.AllowsTransitionToCustomizedMirror;
            model.Constraints = this.ConstraintsEditor.GetModel();
            model.CustomizationTriggers = new(this.CustomizationTriggers);
            return model;
        }

        public MirrorSeriesInfo GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(MirrorSeriesInfo model)
        {
            this.SuppressPropertyNotifications();
            IsCustomizedMirrorSeries = model.IsCustomizedMirrorsSeries;
            AllowsTransitionToCustomizedMirror = model.AllowsTransitionToCustomizedMirror;
            ConstraintsEditor.SetModel(model.Constraints);

            StandardMirrors.Clear();
            foreach (var mirror in model.StandardMirrors)
            {
                StandardMirrors.Add(mirror);
            }

            CustomizationTriggers.Clear();
            foreach (var trigger in model.CustomizationTriggers)
            {
                CustomizationTriggers.Add(trigger);
            }

            this.ResumePropertyNotifications();
            OnPropertyChanged("");
        }
        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;

        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                MirrorEditor.Dispose();
                ConstraintsEditor.PropertyChanged -= ConstraintsEditor_PropertyChanged;
                ConstraintsEditor.Dispose();
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }
}
