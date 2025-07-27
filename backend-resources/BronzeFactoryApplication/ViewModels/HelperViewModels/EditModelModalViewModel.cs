using BronzeFactoryApplication.ApplicationServices.MessangerService;
using CommonInterfacesBronze;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public interface IEditModelModalViewModel<TModel> : IModalViewModel
        where TModel : class, IDeepClonable<TModel>, IEqualityComparerCreator<TModel>
    {
        public IEditorViewModel<TModel> EditorViewModel { get; }
        public void InitilizeModal(EditModelMessage<TModel> modelMessage, string modalTitle = "");

        public IRelayCommand UndoCommand { get; }
        public IRelayCommand FullUndoCommand { get; }
        public IRelayCommand SaveAndCloseCommand { get; }
    }

    /// <summary>
    /// A Message to Request the Edit of a Property of an Object
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class EditModelMessage<TModel> where TModel : class, IDeepClonable<TModel>, IEqualityComparerCreator<TModel>
    {
        /// <summary>
        /// The property value that before being edited
        /// </summary>
        public TModel PropertyValueToEdit { get; set; }
        /// <summary>
        /// The name of the Property that holds the property value
        /// </summary>
        public string PropertyNameToEdit { get; private set; }
        /// <summary>
        /// The Object that holds the property that will be edited
        /// </summary>
        public object PropertyOwner { get; set; }

        /// <summary>
        /// Initilizes a new instance of the <see cref="EditModelMessage{TModel}"/>
        /// </summary>
        /// <param name="propertyValueToEdit">The Current Value of the Property that will be Edited</param>
        /// <param name="propertyOwner">The object that holds the property that will be edited</param>
        public EditModelMessage(TModel propertyValueToEdit, object propertyOwner)
        {
            PropertyValueToEdit = propertyValueToEdit;
            PropertyOwner = propertyOwner;
            PropertyNameToEdit = GetPropertyNameFromValue();
        }

        /// <summary>
        /// Returns the property Name of the Value that was passed to be edited 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private string GetPropertyNameFromValue()
        {
            //Check if there are matching types first 
            var matchingProperties = PropertyOwner.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.PropertyType == PropertyValueToEdit.GetType());
            if (matchingProperties.Any())
            {
                foreach (var property in matchingProperties)
                {
                    var propValue = property.GetValue(PropertyOwner);
                    if (Equals(propValue, PropertyValueToEdit)) return property.Name;
                }
            }
            //otherwise check against all properties (if the passed object is a subclass of the PropertyType of the Owner)
            else
            {
                foreach (var property in PropertyOwner.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var propValue = property.GetValue(PropertyOwner);
                    if(Equals(propValue, PropertyValueToEdit)) return property.Name;
                }
            }
            throw new InvalidOperationException($"The passed value of Type {PropertyValueToEdit.GetType().Name} was not a value of any of the properties of the ownerObject with type : {PropertyOwner.GetType().Name}");
        }
    }

    //Base class only for WPF Relevance
    public partial class EditModelModalViewModel : ModalViewModel
    {
        public virtual INotifyPropertyChanged? EditorViewModel { get; }
    };

    /// <summary>
    /// A Modal ViewModel used to Edit Object properties of other ViewModels
    /// </summary>
    /// <typeparam name="TModel">The Object Type that is being Edited</typeparam>
    public partial class EditModelModalViewModel<TModel> : EditModelModalViewModel, IEditModelModalViewModel<TModel>
        where TModel : class, IDeepClonable<TModel>, IEqualityComparerCreator<TModel>
    {
        /// <summary>
        /// The Context that keeps track of changes of the Model
        /// </summary>
        private readonly EditItemContext<TModel> _editItemContext = new(TModel.GetComparer());
        /// <summary>
        /// The Service that orders closure of modals
        /// </summary>
        private readonly CloseModalService closeModalService;
        /// <summary>
        /// The Edit Message that was sent to open this Modal
        /// </summary>
        private EditModelMessage<TModel>? editMessage;

        public bool HasUnsavedChanges { get => _editItemContext.HasUnsavedChanges(); }
        public bool HasChanges { get => _editItemContext.HasChanges; }
        public bool CanUndo { get => _editItemContext.CanUndo(); }
        public bool CanRedo { get => _editItemContext.CanRedo(); }

        public override IEditorViewModel<TModel> EditorViewModel { get; }

        public EditModelModalViewModel(Func<IEditorViewModel<TModel>> vmFactory, CloseModalService closeModalService)
        {
            EditorViewModel = vmFactory.Invoke();
            this.closeModalService = closeModalService;
            closeModalService.ModalClosing += CloseModalService_ModalClosing;
        }

        private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
        {
            if (e.ClosingModal == this && _editItemContext.HasUnsavedChanges() &&
                MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
            {
                e.ShouldCancelClose = true;
            }
            //else the modal closes and passes changes if there are any , otherwise it simply closes
            else if (_editItemContext.HasChanges)
            {
                if (editMessage is null) throw new Exception($"Unexpected Error , {nameof(editMessage)} was null during a modal Opening of Type {this.GetType().Name}");
                //Get The Last save
                var lastSave = _editItemContext.GetLastSave() ?? throw new Exception("Unexpected Null Reference of Last Save");

                //Get the Property that needs setting
                var property = editMessage.PropertyOwner.GetType().GetProperty(editMessage.PropertyNameToEdit)
                ?? throw new Exception($"Unexpected Error , Property with Name {editMessage.PropertyNameToEdit} was not found in parent of type {editMessage.PropertyOwner.GetType().Name}");
                //Apply the new Value
                property.SetValue(editMessage.PropertyOwner, lastSave);

                //NOT NEEDED THE SETTER INVOKED BY REFLECTION ALSO INVOKES THE PROPERTY CHANGED EVENT!!
                //Invoke the Property change of the owner if there is one
                //var propChangeMethod = editMessage.PropertyOwner.GetType().GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);
                //propChangeMethod?.Invoke(editMessage.PropertyOwner, new object[] { editMessage.PropertyNameToEdit });
            }
            //otherwise modal closes there are no changes anyway
        }

        /// <summary>
        /// Initilizes the Modal ViewModel
        /// </summary>
        /// <param name="editMessage">The Edit Message that was sent containing information about the Edits of the Model that will happen in the modal</param>
        /// <param name="modalTitle">The Title of the Modal</param>
        /// <exception cref="Exception"></exception>
        public void InitilizeModal(EditModelMessage<TModel> editMessage, string modalTitle = "")
        {
            this.editMessage = editMessage;
            //Cancel any subscription of a previous edit model
            EditorViewModel.PropertyChanged -= EditorViewModel_PropertyChanged;

            //Set the new Model
            EditorViewModel.SetModel(editMessage.PropertyValueToEdit);
            //Set the undo store
            _editItemContext.SetUndoStore(EditorViewModel.GetModel());
            //Set the Title
            if (string.IsNullOrEmpty(modalTitle))
            {
                modalTitle = $"lngEdit{editMessage.PropertyNameToEdit}".TryTranslateKey();
            }

            Title = modalTitle;
            //Subscribe to Property Changes to push edits to the context
            EditorViewModel.PropertyChanged += EditorViewModel_PropertyChanged;
        }

        private void EditorViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //Unsubscribe to avoid recursion
            EditorViewModel.PropertyChanged -= EditorViewModel_PropertyChanged;
            //Push edits to the Context
            _editItemContext.PushEdit(EditorViewModel.GetModel());
            NotifyUndoRedoChanges();
            //Unsubscribe to avoid recursion
            EditorViewModel.PropertyChanged += EditorViewModel_PropertyChanged;
        }
        private void NotifyUndoRedoChanges()
        {
            OnPropertyChanged(nameof(HasUnsavedChanges));
            OnPropertyChanged(nameof(HasChanges));
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
            UndoCommand.NotifyCanExecuteChanged();
            RedoCommand.NotifyCanExecuteChanged();
            FullUndoCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanUndo))]
        private void Undo()
        {
            try
            {
                //Unsubscribe to avoid recursion
                EditorViewModel.PropertyChanged -= EditorViewModel_PropertyChanged;

                EditorViewModel.SetModel(_editItemContext.Undo());
                NotifyUndoRedoChanges();

                //Resubscribe to capture further changes
                EditorViewModel.PropertyChanged += EditorViewModel_PropertyChanged;
            }
            catch (Exception ex)
            {
                MessageService.DisplayException(ex);
            }
        }
        [RelayCommand(CanExecute = nameof(CanRedo))]
        private void Redo()
        {
            try
            {
                //Unsubscribe to avoid recursion
                EditorViewModel.PropertyChanged -= EditorViewModel_PropertyChanged;

                EditorViewModel.SetModel(_editItemContext.Redo());
                NotifyUndoRedoChanges();

                //Resubscribe to capture further changes
                EditorViewModel.PropertyChanged += EditorViewModel_PropertyChanged;
            }
            catch (Exception ex)
            {
                MessageService.DisplayException(ex);
            }
        }

        [RelayCommand(CanExecute = nameof(HasChanges))]
        private void FullUndo()
        {
            var fullUndoModel = _editItemContext.FullUndo();
            if (fullUndoModel is not null)
            {
                //Unsubscribe to avoid recursion
                EditorViewModel.PropertyChanged -= EditorViewModel_PropertyChanged;

                EditorViewModel.SetModel(fullUndoModel);
                NotifyUndoRedoChanges();

                //Resubscribe to capture further changes
                EditorViewModel.PropertyChanged += EditorViewModel_PropertyChanged;
            }
            else
            {
                throw new Exception("Unexpected Null Undo Store");
            }
        }

        [RelayCommand]
        private void SaveAndClose()
        {
            _editItemContext.SaveCurrentState();
            NotifyUndoRedoChanges();
            closeModalService.CloseModal(this);
        }

        private bool _disposed;
        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                closeModalService.ModalClosing -= CloseModalService_ModalClosing;
                EditorViewModel.PropertyChanged -= EditorViewModel_PropertyChanged;
                EditorViewModel.Dispose();
            }

            //object has been disposed
            _disposed = true;
            base.Dispose(disposing);
        }
    }
}
