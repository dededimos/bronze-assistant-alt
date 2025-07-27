using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementosLibrary
{
    /// <summary>
    /// Inherit from this Service and provide implementations to the abstract Methods to get Undo or StateSave Functionality
    /// Create a <typeparamref name="TState"/> object which will hold the information of the State of the <typeparamref name="TOriginal"/>
    /// </summary>
    /// <typeparam name="TOriginal">The Object whose state must be Saved/Undone </typeparam>
    /// <typeparam name="TState">The Object which will keep the state information of the <typeparamref name="TOriginal"/> , <para>The TState can be also the same type as the TOriginator for an item that can be stored in a clone</para></typeparam>
    public class MementoServiceBase<TOriginal,TState>
        where TOriginal: ITransformable<TState> , IRestorable<TState>
    {
        private readonly Stack<TState> history = new();

        /// <summary>
        /// Pushes the Current State of the Original Object into the Undo History
        /// </summary>
        /// <param name="originalObject">The <typeparamref name="TOriginal"/> Object whose state will be saved into the undo stack</param>
        public void PushToHistory(TOriginal originalObject)
        {
            var state = originalObject.GetTransformation();
            history.Push(state);
        }

        /// <summary>
        /// Scraps the latest Saved State and Modifies the provided object into the previous State (Undo)
        /// </summary>
        /// <param name="originalToUndo">The Current Object which to Undo into its previous state</param>
        public void Undo(TOriginal originalToUndo)
        {
            if (history.Count > 0)
            {
                history.Pop();//remove the State on top of the Stack 
                //(this would essentially be the current state of the originalToUndo if any change made is saved . Otherwise the code should be changed to only use Pop instead of Pop => Peek)

                UndoToLatestState(originalToUndo);
            }
        }

        /// <summary>
        /// Modifies the State of the provided object into the latest saved State
        /// </summary>
        /// <param name="originalToUndo"></param>
        public void UndoToLatestState(TOriginal originalToUndo)
        {
            if (history.Count > 0) //Check if there is still previous history
            {
                var state = history.Peek();
                originalToUndo.Restore(state);//Get the now Top of the Stack Item and Restore it to the Originator
            }
        }

        /// <summary>
        /// Clears history
        /// </summary>
        public void ClearHistory() => history.Clear();
    }
}
