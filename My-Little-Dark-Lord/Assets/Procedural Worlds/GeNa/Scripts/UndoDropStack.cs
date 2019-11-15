using UnityEngine;
using System;
using System.Collections.Generic;

namespace GeNa.Internal
{
    /// <summary>
    /// A limited size Undo stack where the oldest items start "falling out at the bottom"
    /// when more items are added after the <see cref="Capacity"/> was reached.
    /// DO make sure that the content is serializable by unity if you want the stack to be serialized.
    /// </summary>
    [Serializable]
	public class UndoDropStack : PWCommon2.DropStack<UndoRecord>
    {
        /// <summary>
        /// Create a new Undo stack.
        /// </summary>
        /// <param name="capacity">Max capacity of the stack.</param>
        public UndoDropStack(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// Create a new Undo stack and fill it with records from a List.
        /// </summary>
        /// <param name="capacity">Max capacity of the stack.</param>
        public UndoDropStack(int capacity, List<UndoRecord> records) : base(capacity, records)
        {
        }
    }
}