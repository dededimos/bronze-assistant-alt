using BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels;
using CommonInterfacesBronze;
using MirrorsRepositoryMongoDB.Entities;
using MongoDbCommonLibrary.CommonInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.HelperViewModels
{
    public interface IUndoViewModel : IBaseViewModel
    {
        public bool HasUnsavedChanges { get; }
        public bool HasChanges { get; }
        IRelayCommand UndoCommand { get; }
        IRelayCommand RedoCommand { get; }
        IRelayCommand FullUndoCommand { get; }
        IRelayCommand SaveChangesCommand { get; }
    }

    public interface IUndoEditorViewModel<T> : IUndoViewModel , IEditorViewModel<T>
        where T : class, IDeepClonable<T>, IEqualityComparerCreator<T>
    {

    }
    public interface IMirrorEntityEditorViewModel<T> : IUndoEditorViewModel<T>
        where T : class, IDeepClonable<T>, IEqualityComparerCreator<T>, IDatabaseEntity
    {
        MongoDatabaseEntityEditorViewModel BaseEntity { get; }
    }
    public interface IMirrorElementEntityEditorViewModel<T> : IMirrorEntityEditorViewModel<T>
        where T : class, IDeepClonable<T>, IEqualityComparerCreator<T>, IDatabaseEntity , IMirrorElementEntity
    {
        //To Add the Properties of the Mirror Element Info ViewModel
    }

    /// <summary>
    /// An <see cref="IEditorViewModel{T}"/> implementation with Undo/Redo Functionality as a <see cref="BaseViewModel"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class UndoEditorViewModelBase<T> : BaseViewModel, IUndoEditorViewModel<T>
        where T : class, IDeepClonable<T>, IEqualityComparerCreator<T>
    {
        private readonly EditItemContext<T> _editContext = new(T.GetComparer());

        public bool HasUnsavedChanges { get => _editContext.HasUnsavedChanges(); }
        public bool HasChanges { get => _editContext.HasChanges; }
        public bool CanUndo { get => _editContext.CanUndo(); }
        public bool CanRedo { get => _editContext.CanRedo(); }

        public virtual T CopyPropertiesToModel(T model)
        {
            throw new NotSupportedException();
        }

        public virtual T GetModel()
        {
            throw new NotSupportedException();
        }

        public void SetModel(T model)
        {
            StopTrackingUndoChanges();
            _editContext.SetUndoStore(model);
            SetModelWithoutUndoStore(model);
            NotifyUndoRedoChanges();
            StartTrackingUndoChanges();
        }
        protected virtual void SetModelWithoutUndoStore(T model)
        {
            throw new NotSupportedException();
        }

        private void NotifyUndoRedoChanges()
        {
            OnPropertyChanged(nameof(HasChanges));
            OnPropertyChanged(nameof(HasUnsavedChanges));
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
            SaveChangesCommand.NotifyCanExecuteChanged();
            UndoCommand.NotifyCanExecuteChanged();
            RedoCommand.NotifyCanExecuteChanged();
            FullUndoCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Stops pushing edits into the Undo/Redo Stack
        /// </summary>
        protected void StopTrackingUndoChanges()
        {
            this.PropertyChanged -= UndoEditorViewModelBase_PropertyChanged;
        }
        /// <summary>
        /// Starts pushing edits into the Undo/Redo Stack
        /// </summary>
        protected void StartTrackingUndoChanges()
        {
            this.PropertyChanged += UndoEditorViewModelBase_PropertyChanged;
        }

        private void UndoEditorViewModelBase_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //Avoid Recursion Stack Overflow Exception otherwise it pushes changes indefenately!!!!
            StopTrackingUndoChanges();
            _editContext.PushEdit(GetModel());
            NotifyUndoRedoChanges();
            StartTrackingUndoChanges();
        }

        [RelayCommand(CanExecute = nameof(CanUndo))]
        private void Undo()
        {
            StopTrackingUndoChanges();
            ExecuteUndo();
            NotifyUndoRedoChanges();
            StartTrackingUndoChanges();
        }
        protected virtual void ExecuteUndo()
        {
            try
            {
                SetModelWithoutUndoStore(_editContext.Undo());
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
        }
        [RelayCommand(CanExecute =nameof(CanRedo))]
        private void Redo()
        {
            StopTrackingUndoChanges();
            ExecuteRedo();
            NotifyUndoRedoChanges();
            StartTrackingUndoChanges();
        }
        protected virtual void ExecuteRedo()
        {
            try
            {
                SetModelWithoutUndoStore(_editContext.Redo());
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
        }
        [RelayCommand(CanExecute = nameof(HasChanges))]
        private void FullUndo()
        {
            StopTrackingUndoChanges();
            ExecuteFullUndo();
            NotifyUndoRedoChanges();
            StartTrackingUndoChanges();
        }
        protected virtual void ExecuteFullUndo()
        {
            try
            {
                T? fullUndo = _editContext.FullUndo();
                if (fullUndo is null) throw new Exception($"{nameof(fullUndo)} encountered a Null Reference error , Possibly UndoStore has not been set...");
                SetModelWithoutUndoStore(fullUndo);
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
        }

        [RelayCommand(CanExecute = nameof(HasUnsavedChanges))]
        private void SaveChanges()
        {
            if(HasUnsavedChanges)
            {
                StopTrackingUndoChanges();
                ExecuteSaveChanges();
                NotifyUndoRedoChanges();
                StartTrackingUndoChanges();
            }
        }
        protected virtual void ExecuteSaveChanges()
        {
            _editContext.SaveCurrentState();
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
                this.PropertyChanged -= UndoEditorViewModelBase_PropertyChanged;
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
