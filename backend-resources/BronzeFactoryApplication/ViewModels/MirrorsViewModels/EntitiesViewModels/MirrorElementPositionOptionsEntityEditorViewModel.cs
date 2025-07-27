using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using MirrorsRepositoryMongoDB.Entities;
using MirrorsRepositoryMongoDB.Repositories;
using System.Collections.ObjectModel;
using System.Linq;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorElementPositionOptionsEntityEditorViewModel : MongoEntityBaseUndoEditorViewModel<MirrorElementPositionOptionsEntity>, IMirrorEntityEditorViewModel<MirrorElementPositionOptionsEntity>
    {
        private readonly IMirrorsDataProvider dataProvider;

        [ObservableProperty]
        private MirrorModule? concerningElement;

        public ObservableCollection<DefaultPositionObject> DefaultPositions { get; set; } = new();

        [ObservableProperty]
        private MirrorOrientedShape? selectedShapeToAddDefaultPosition;
        [ObservableProperty]
        private MirrorElementPosition? selectedPositionToAddDefaultPosition;

        [ObservableProperty]
        private MirrorOrientedShape? selectedShapeToAddAdditionalPosition;
        [ObservableProperty]
        private MirrorElementPosition? selectedPositionToAddAdditionalPosition;

        [ObservableProperty]
        private MirrorModule? selectedElementToCopyPositions;

        /// <summary>
        /// Positions that can be selected to add
        /// </summary>
        public IEnumerable<MirrorElementPosition> SelectablePositions { get => dataProvider.GetAllPositions(); }
        public IEnumerable<MirrorModule> SelectableModulePositionables { get => dataProvider.GetAllModules(); }

        /// <summary>
        /// The shapes under which to register a Position , already selected shapes are not present
        /// </summary>
        public IEnumerable<MirrorOrientedShape> SelectableShapesToAddInDefaultPosition { get => Enum.GetValues(typeof(MirrorOrientedShape)).Cast<MirrorOrientedShape>().Except(DefaultPositions.Select(dp => dp.ConcerningShape).ToList()); }

        public ObservableCollection<PositionsListObject> AdditionalPositions { get; set; } = new();

        /// <summary>
        /// Adds a Default Position to the Default Positions
        /// </summary>
        [RelayCommand]
        private void TryAddDefaultPosition()
        {
            if (SelectedShapeToAddDefaultPosition is null)
            {
                MessageService.Warning("Please select a Shape for this Position", "Mirror Shape Not Selected to Add Position");
                return;
            }
            if (SelectedPositionToAddDefaultPosition is null)
            {
                MessageService.Warning("Please select a Position Element for this DefaultPosition", "Position Element Not Selected to Add Position");
                return;
            }
            if (DefaultPositions.Any(dp => dp.ConcerningShape == SelectedShapeToAddDefaultPosition))
            {
                MessageService.Warning($"There is already a Default Position for this Shape{Environment.NewLine}{SelectedShapeToAddDefaultPosition}", "Position with this Shape Already Exists");
                return;
            }

            var posObj = new DefaultPositionObject()
            {
                ConcerningShape = SelectedShapeToAddDefaultPosition ?? MirrorOrientedShape.Undefined,
                Position = SelectedPositionToAddDefaultPosition.GetDeepClone(),
            };
            DefaultPositions.Add(posObj);
            OnPropertyChanged(nameof(DefaultPositions));
            OnPropertyChanged(nameof(SelectableShapesToAddInDefaultPosition));
        }
        /// <summary>
        /// Removes a Default Position from the Default Positions List
        /// </summary>
        /// <param name="objectToRemove">The Default position to Remove</param>
        [RelayCommand]
        private void RemoveDefaultPosition(DefaultPositionObject objectToRemove)
        {
            DefaultPositions.Remove(objectToRemove);
            OnPropertyChanged(nameof(DefaultPositions));
            OnPropertyChanged(nameof(SelectableShapesToAddInDefaultPosition));
        }
        /// <summary>
        /// Adds a Position to the Additional Positions List in the Specified MirrorOriented Shape
        /// </summary>
        [RelayCommand]
        private void AddAdditionalPosition()
        {
            if(SelectedShapeToAddAdditionalPosition is null || SelectedShapeToAddAdditionalPosition == MirrorOrientedShape.Undefined)
            {
                MessageService.Warning($"Please first select a Mirror Shape to add Additional Positions too.", "Mirror Shape not Selected");
                return;
            }
            if (SelectedPositionToAddAdditionalPosition is null)
            {
                MessageService.Warning($"Please first select a Position to add", "Position not Selected");
                return;
            }

            //Check if the Selected position is already a default Position for this element
            var isDefaultPosition = DefaultPositions.FirstOrDefault(dp => dp.ConcerningShape == SelectedShapeToAddAdditionalPosition && dp.Position.ElementId == SelectedPositionToAddAdditionalPosition.ElementId) is not null;
            if (isDefaultPosition)
            {
                MessageService.Warning($"The Selected Position :{SelectedPositionToAddAdditionalPosition.Code} is already a Default position for the Mirror Shape {SelectedShapeToAddAdditionalPosition}", "Additional Position is Already Default Position");
                return;
            }

            //Find if there is already an additional position at the designated MirrorShape
            var additionalPositionsListObject = AdditionalPositions.FirstOrDefault(p => p.ConcerningShape == SelectedShapeToAddAdditionalPosition);

            //If its null then a new Object should be created with that shape and a List of additional positions
            if (additionalPositionsListObject is null)
            {
                additionalPositionsListObject = new()
                {
                    ConcerningShape = (MirrorOrientedShape)SelectedShapeToAddAdditionalPosition,
                    Positions = new() { SelectedPositionToAddAdditionalPosition.GetDeepClone() }
                };
                AdditionalPositions.Add(additionalPositionsListObject);
                OnPropertyChanged(nameof(AdditionalPositions));
                return;
            }
            //Else add the selected position to the list that is already present
            //First check that the position to be added is not already included
            else
            {
                //If already present prompt and return
                if (additionalPositionsListObject.Positions.Any(p=> p.ElementId == SelectedPositionToAddAdditionalPosition.ElementId))
                {
                    MessageService.Warning($"The Position with Code:{SelectedPositionToAddAdditionalPosition.Code} is already present as an Additional position for shape {SelectedShapeToAddAdditionalPosition}", "Position already present");
                    return;
                }
                //If not present then create again a whole list object and replace the old one so that the list is properly updated
                //Clone the old listObject
                var newListObject = additionalPositionsListObject.GetDeepClone();
                //Add the new Position to each list
                newListObject.Positions.Add(SelectedPositionToAddAdditionalPosition.GetDeepClone());

                //Replace
                var index = AdditionalPositions.IndexOf(additionalPositionsListObject);
                AdditionalPositions[index] = newListObject;
                OnPropertyChanged(nameof(AdditionalPositions));
            }

        }
        /// <summary>
        /// Removes the Whole Additional Positions List for a certain mirror Oriented Shape
        /// </summary>
        /// <param name="shapeToRemoveAdditionalPositionsFor">The Shape for which to remove all the Additional Positions</param>
        [RelayCommand]
        private void RemoveAdditionalPositionList(MirrorOrientedShape shapeToRemoveAdditionalPositionsFor)
        {
            if (shapeToRemoveAdditionalPositionsFor is MirrorOrientedShape.Undefined)
            {
                MessageService.Warning($"Unexpected Error , The Selected Concerning Shape to remove is Undefined", "Unexpected Error");
                return;
            }

            //find the listObject , then remove it from the list and update
            var listObjectToRemove = AdditionalPositions.FirstOrDefault(lo => lo.ConcerningShape == shapeToRemoveAdditionalPositionsFor);
            if (listObjectToRemove is null)
            {
                MessageService.Warning($"Unexpected Error , The Selected Concerning Shape to remove was not found in the List of Additional Positions", "Unexpected Error");
                return;
            }
            else
            {
                AdditionalPositions.Remove(listObjectToRemove);
                OnPropertyChanged(nameof(AdditionalPositions));
            }

        }
        /// <summary>
        /// Removes a single position from the list of additional positions for a certain Mirror Oriented Shape (The Selected List Object defines the shape)
        /// </summary>
        /// <param name="positionToRemove">Which position to remove from the list</param>
        [RelayCommand]
        private void RemovePositionFromAdditionalPositionsList((MirrorOrientedShape shape , MirrorElementPosition positionToRemove) commandParam)
        {
            //First check that a PositionsListObject has been selected
            if (commandParam.positionToRemove is null)
            {
                MessageService.Warning($"Unexpected Error Position to remove was Null", "Unexpected Error");
                return;
            }
            //find the list object clone it . Remove from it the designated item and replace the old ListObject with the clone
            var clone = AdditionalPositions.FirstOrDefault(ap=> ap.ConcerningShape == commandParam.shape)?.GetDeepClone();
            if(clone is null)
            {
                MessageService.Warning($"Unexpected Error , no additional Position Found to remove with Shape{commandParam.shape}", "Unexpected Error");
                return;
            }

            //Have to do it with id as its a clone
            var foundPositionToRemove = clone.Positions.FirstOrDefault(p => p.ElementId == commandParam.positionToRemove.ElementId);
            if (foundPositionToRemove is null)
            {
                MessageService.Warning("Unexpected Error , the Selected position was not found in the Selected Positions List", "Unexpected Error");
                return;
            }
            //Remove it
            clone.Positions.Remove(foundPositionToRemove);
            //Then found the index of the initial list and replace it with the clone
            var index = AdditionalPositions.IndexOf(AdditionalPositions.First(p=> p.ConcerningShape == clone.ConcerningShape));
            AdditionalPositions[index] = clone;
            OnPropertyChanged(nameof(AdditionalPositions));
        }
        [RelayCommand]
        private void CopyDefaultPositionsFromSelectedElement()
        {
            if (SelectedElementToCopyPositions is null)
            {
                MessageService.Warning("Please select an Element from which to Copy the Positions first", "Element to Copy not selected");
                return;
            }
            //get the position of the selected Module
            var positionOptions = dataProvider.GetPositionOptionsOfElement(SelectedElementToCopyPositions.ElementId);
            if (positionOptions is null)
            {
                MessageService.Warning($"There where not any Position Options found for the Selected Element : {Environment.NewLine}{Environment.NewLine}Code:{SelectedElementToCopyPositions.Code}{Environment.NewLine}{SelectedElementToCopyPositions.LocalizedDescriptionInfo.Name.DefaultValue}", "Position Options not Found");
                return;
            }

            StopTrackingUndoChanges();

            //Copy Positions
            DefaultPositions.Clear();
            foreach (var defPos in positionOptions.DefaultPositions.DefaultPositions)
            {
                DefaultPositionObject defPosition = new() { ConcerningShape = defPos.Key, Position = defPos.Value.GetDeepClone() };
                DefaultPositions.Add(defPosition);
            }
            AdditionalPositions.Clear();
            foreach (var pos in positionOptions.AdditionalPositions)
            {
                PositionsListObject posList = new() { ConcerningShape = pos.Key, Positions = new(pos.Value.Select(p => p.GetDeepClone())) };
                this.AdditionalPositions.Add(posList);
            }
            
            OnPropertyChanged(nameof(DefaultPositions));
            OnPropertyChanged(nameof(SelectableShapesToAddInDefaultPosition));

            //Capture only a single Change not both of them seperately
            StartTrackingUndoChanges();
            OnPropertyChanged(nameof(AdditionalPositions));
        }

        public MirrorElementPositionOptionsEntityEditorViewModel(
            Func<MongoDatabaseEntityEditorViewModel> baseEntityVmFactory, IMirrorsDataProvider dataProvider)
            : base(baseEntityVmFactory)
        {
            this.dataProvider = dataProvider;
        }

        public override MirrorElementPositionOptionsEntity CopyPropertiesToModel(MirrorElementPositionOptionsEntity model)
        {
            base.CopyPropertiesToModel(model);
            model.ConcerningElementId = this.ConcerningElement?.ElementId ?? string.Empty;
            model.DefaultPositions = this.DefaultPositions.ToDictionary(dpo => dpo.ConcerningShape, dpo => dpo.Position.ElementId);
            model.AdditionalPositions = this.AdditionalPositions.ToDictionary(ap => ap.ConcerningShape, ap => new List<string>(ap.Positions.Select(p => p.ElementId)));
            return model;
        }
        protected override void SetModelWithoutUndoStore(MirrorElementPositionOptionsEntity model)
        {
            base.SetModelWithoutUndoStore(model);
            this.ConcerningElement = dataProvider.GetModule(model.ConcerningElementId);
            this.DefaultPositions.Clear();
            foreach (var pos in model.DefaultPositions)
            {
                var newPosObj = new DefaultPositionObject() { ConcerningShape = pos.Key, Position = dataProvider.GetPosition(pos.Value)};
                this.DefaultPositions.Add(newPosObj);
            }
            OnPropertyChanged(nameof(DefaultPositions));

            this.AdditionalPositions.Clear();
            foreach (var addPos in model.AdditionalPositions)
            {
                var addPosObj = new PositionsListObject() { ConcerningShape = addPos.Key, Positions = new(dataProvider.GetPositions([.. addPos.Value])) };
                this.AdditionalPositions.Add(addPosObj);
            }
            OnPropertyChanged(nameof(AdditionalPositions));
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

            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }
    public class DefaultPositionObject
    {
        public MirrorOrientedShape ConcerningShape { get; set; }
        public MirrorElementPosition Position { get; set; } = MirrorElementPosition.DefaultPositionElement();
    }
    public class PositionsListObject : IDeepClonable<PositionsListObject>
    {
        public MirrorOrientedShape ConcerningShape { get; set; }
        public List<MirrorElementPosition> Positions { get; set; } = new();

        public PositionsListObject()
        {
            
        }
        public PositionsListObject(MirrorOrientedShape concerningShape, IEnumerable<MirrorElementPosition> positions)
        {
            ConcerningShape = concerningShape;
            Positions = new List<MirrorElementPosition>(positions);
        }

        public PositionsListObject GetDeepClone()
        {
            PositionsListObject clone = (PositionsListObject)this.MemberwiseClone();
            clone.Positions = Positions.Select(p => p.GetDeepClone()).ToList();
            return clone;
        }
    }
}
