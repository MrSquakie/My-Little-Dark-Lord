using UnityEngine;
using System.Collections.Generic;

namespace GeNa.Internal
{
    /// <summary>
    /// Need to store undo stacks in this to avoid Unity doing crazy unecessary serialisation
    /// </summary>
    public class UndoComponent : MonoBehaviour
    {
        /// <summary>
        /// Undo Stack
        /// </summary>
		public UndoDropStack UndoStack
        {
            get
            {
                if (m_undoStack == null)
                {
                    ClearUndoStack();
                }
                return m_undoStack;
            }
        }
		[SerializeField] private UndoDropStack m_undoStack;

        /// <summary>
        /// Clears the Undo Stack
        /// </summary>
        public void ClearUndoStack()
        {
            m_undoStack = GeNaUndo.NewStack();
        }

        /// <summary>
        /// Clears the Undo Stack and converts the list provided into it.
        /// </summary>
        public void UpdateUndoStack(List<UndoRecord> list)
        {
            m_undoStack = GeNaUndo.NewStack(list);
        }
    }
}
