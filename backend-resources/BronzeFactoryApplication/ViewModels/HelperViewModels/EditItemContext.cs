using CommonInterfacesBronze;
using CommunityToolkit.Diagnostics;

namespace BronzeFactoryApplication.ViewModels
{
    /// <summary>
    /// A helper UndoStore-Saved Changes Object to Determine weather there are changes in an item
    /// An IEqualityComparer implementation must be provided for the "T" of the EditContext in order 
    /// to determine weather the item has changes in any given time
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditItemContext<T>
        where T : class , IDeepClonable<T>
    {
        /// <summary>
        /// The Undo Store of the Item being edited (Initial Item before Edit)
        /// </summary>
        private T? undoStore;
        /// <summary>
        /// The item in its latest Edited State
        /// </summary>
        private T? lastEdit;
        /// <summary>
        /// The item in its latest Saved State
        /// </summary>
        private T? lastSave;

        private readonly Stack<T> undoStack = new();
        private readonly Stack<T> redoStack = new();

        /// <summary>
        /// Weather the object has undergone any changes 
        /// </summary>
        public bool HasChanges { get => editItemComparer.Equals(lastEdit, undoStore) is false; }

        /// <summary>
        /// The Equality Comparer of the Item
        /// </summary>
        private readonly IEqualityComparer<T> editItemComparer;

        public EditItemContext(IEqualityComparer<T> editItemComparer)
        {
            this.editItemComparer = editItemComparer;
        }

        /// <summary>
        /// Set the Undo Store for the item being Edited . The State of the Item if needs to be Resotred to it
        /// </summary>
        /// <param name="undoStore"></param>
        public void SetUndoStore(T undoStore)
        {
            this.undoStore = undoStore.GetDeepClone();
            this.lastEdit = undoStore.GetDeepClone();
            this.lastSave = undoStore.GetDeepClone();
            undoStack.Clear();
            redoStack.Clear();
        }

        /// <summary>
        /// Retrieves the Initial State of the Item before any Edits Saved or Not Saved
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public T RetriveUndoStore()
        {
            if (undoStore is null)
            {
                throw new Exception("Undo Object has not been Set...");
            }
            return undoStore.GetDeepClone();
        }

        public void PushEdit(T newEditedState)
        {
            if (lastEdit is null) throw new Exception($"Unexpected Null Reference of {nameof(lastEdit)} ,it Should at least be Equal to the UndoStore...");
            
            if (!editItemComparer.Equals(lastEdit,newEditedState))
            {
                undoStack.Push(lastEdit);
                lastEdit = newEditedState.GetDeepClone();

                //Clear redo stack on Edits
                redoStack.Clear();
            }
        }
        public bool CanUndo() => undoStack.Count > 0;
        public bool CanRedo() => redoStack.Count > 0;

        public T Undo()
        {
            if (CanUndo())
            {
                T undoState = undoStack.Pop();
                if(lastEdit != null) redoStack.Push(lastEdit.GetDeepClone());// Save current state to redo stack before undoing
                lastEdit = undoState.GetDeepClone();
                return undoState.GetDeepClone();
            }
            throw new InvalidOperationException("Undo Stack is Empty , no more History to Undo...");
        }

        public T Redo()
        {
            if (!CanRedo())
                throw new InvalidOperationException("Redo Stack is Empty, no more Actions to Redo...");

            T redoState = redoStack.Pop();
            if (lastEdit != null) undoStack.Push(lastEdit);  // Save the current state to undo stack before redoing
            lastEdit = redoState.GetDeepClone();
            return redoState.GetDeepClone();
        }

        public T? FullUndo()
        {
            undoStack.Clear();
            redoStack.Clear();
            lastEdit = undoStore?.GetDeepClone();
            return undoStore?.GetDeepClone();
        }

        public bool HasUnsavedChanges()
        {
            if (lastEdit is null && lastSave is null)
            {
                return false;
            }
            // If they are not the Same there are unsaved Changes
            return !editItemComparer.Equals(lastEdit, lastSave);
        }

        /// <summary>
        /// Stores the latest edit to the last save field
        /// </summary>
        /// <returns></returns>
        public T? SaveCurrentState()
        {
            lastSave = lastEdit?.GetDeepClone();
            return lastSave?.GetDeepClone();
        }

        public T? GetLastSave()
        {
            return lastSave?.GetDeepClone();
        }
    }
}
