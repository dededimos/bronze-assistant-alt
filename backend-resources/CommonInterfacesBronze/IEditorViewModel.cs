using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfacesBronze
{
    /// <summary>
    /// A ViewModel to Edit objects of type : <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEditorViewModel<T> : IModelGetterViewModel<T>
    {
        /// <summary>
        /// Sets the Properties of the Current state of the Model to the ViewModel
        /// </summary>
        /// <param name="model">The Current state of the Model</param>
        void SetModel(T model);

        /// <summary>
        /// Copies the Properties of the ViewModel according to its current state to a model
        /// </summary>
        /// <param name="model">The Model to which to copy the Properties to</param>
        /// <returns>The Model with the Copied Properties or a Copy of it if its Immutable</returns>
        T CopyPropertiesToModel(T model);

        public static IEditorViewModel<T> EmptyEditor()
        {
            return EmptyEditorViewModel<T>.Instance;
        }
    }

    //The out Keyword makes this interface covariant which means ANY deriving class of TModel is also an interface of IModelGetterViewModel<derivedClass> and IModelGetterViewModel<TModel>
    public interface IModelGetterViewModel<out TModel> : IBaseViewModel
    {
        /// <summary>
        /// Returns the current State of the Model : <typeparamref name="TModel"/> represented by this ViewModel
        /// </summary>
        /// <returns></returns>
        TModel GetModel();

        public static IModelGetterViewModel<TModel> EmptyGetter()
        {
            return EmptyModelGetterViewModel<TModel>.Instance;
        }
    }

    // Empty implementation
    internal class EmptyModelGetterViewModel<TModel> : IModelGetterViewModel<TModel>
    {
        private static readonly EmptyModelGetterViewModel<TModel> _instance = new();
        public static EmptyModelGetterViewModel<TModel> Instance => _instance;

        public string Title { get => string.Empty; }
        public bool IsDisposable { get => false; }
        public bool IsBusy { get => false; }
        public bool IsNotBusy { get => true; }
        public string BusyPrompt { get => string.Empty; }
        public bool Initilized { get => true; }

        public TModel GetModel()
        {
            throw new InvalidOperationException($"{nameof(IModelGetterViewModel<TModel>)} Is Empty...");
            //return default(TModel);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;

        public void Dispose()
        {
            // Nothing to dispose , Never Disposed
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    internal class EmptyEditorViewModel<TModel> : IEditorViewModel<TModel>
    {
        private static readonly EmptyEditorViewModel<TModel> _instance = new();
        public static EmptyEditorViewModel<TModel> Instance => _instance;

        public string Title { get => string.Empty; }
        public bool IsDisposable { get => false; }
        public bool IsBusy { get => false; }
        public bool IsNotBusy { get => true; }
        public string BusyPrompt { get => string.Empty; }
        public bool Initilized { get => true; }

        public TModel GetModel()
        {
            throw new InvalidOperationException($"{nameof(IEditorViewModel<TModel>)} Is Empty...");
            //return default(TModel);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;

        public void Dispose()
        {
            // Nothing to dispose
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetModel(TModel model)
        {
            return;
        }

        public TModel CopyPropertiesToModel(TModel model)
        {
            throw new InvalidOperationException($"{nameof(IEditorViewModel<TModel>)} Is Empty...");
        }
    }

}
