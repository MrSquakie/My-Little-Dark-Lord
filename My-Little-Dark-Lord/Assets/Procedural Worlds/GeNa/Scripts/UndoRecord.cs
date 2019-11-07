// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PWCommon2;

namespace GeNa.Internal
{
    [System.Serializable]
	public class UndoRecord : ISerializationCallbackReceiver
    {
        /// <summary>
        /// Before Snapshot
        /// </summary>
        internal GeNaUndoSnapshot Before { get; private set; }
        /// <summary>
        /// After Snapshot
        /// </summary>
        internal GeNaUndoSnapshot After { get; private set; }

        /// <summary>
        /// The diffs in this record
        /// </summary>
        internal UndoSnapshotDiff Diffs
        {
            get
            {
                if (m_diffs == null)
                {
                    GetDiffs();
                }
                return m_diffs;
            }
        }
        [SerializeField] private UndoSnapshotDiff m_diffs;

        internal ExtensionInstance[] SpawnedExtensions { get { return m_spawnedExtensions; } }
        [SerializeField] private ExtensionInstance[] m_spawnedExtensions;

        [SerializeField] private Spawner m_actingSpawner;
        [SerializeField] private Transform m_target;

        /// <summary>
        /// Can add a description to the action that can be undone.
        /// </summary>
        public string Description { get { return m_description; } }
        [SerializeField] private string m_description = "";

        /// <summary>
        /// Frapoch time of when this was recorded (<seealso cref="Utils.GetFrapoch()"/>).
        /// </summary>
        public int Time { get { return m_time; } }
        [SerializeField] private int m_time = 0;

        /// <summary>
        /// Childs of the spawner which were activated for the sapwn event and also recorded Undo.
        /// </summary>
        public Spawner[] Childs { get { return m_childSpawners; } }
        [SerializeField] private Spawner[] m_childSpawners;

        public bool ContainSpawns
        {
            get
            {
                bool spawnDiff = false;

                if (Before.ProtoInstanceCounts.Length != After.ProtoInstanceCounts.Length || Before.ResourceInstanceCounts.Length != After.ResourceInstanceCounts.Length)
                {
                    Debug.LogWarningFormat("[GeNa]: Prototype count discrepancy in before and after snapshots ({0} \u2260 {1}; {2} \u2260 {3}). Can not record undo.",
                        Before.ProtoInstanceCounts.Length, After.ProtoInstanceCounts.Length, Before.ResourceInstanceCounts.Length, After.ResourceInstanceCounts.Length);
                }
                else
                {
                    for (int i = 0; i < Before.ProtoInstanceCounts.Length; i++)
                    {
                        spawnDiff |= (Before.ProtoInstanceCounts[i] != After.ProtoInstanceCounts[i]);

                        if (Before.ResourceInstanceCounts[i].Length != After.ResourceInstanceCounts[i].Length)
                        {
                            Debug.LogWarningFormat("[GeNa]: Resource count discrepancy in before and after snapshots ({0} \u2260 {1}). Can not record undo.",
                                Before.ResourceInstanceCounts[i].Length, After.ResourceInstanceCounts[i].Length);
                            continue;
                        }

                        for (int ri = 0; ri < Before.ResourceInstanceCounts[i].Length; ri++)
                        {
                            spawnDiff |= (Before.ResourceInstanceCounts[i][ri] != After.ResourceInstanceCounts[i][ri]);
                        }
                    }
                }

                return spawnDiff;
            }
        }

        /// <summary>
        /// Default ctor.
        /// </summary>
        private UndoRecord()
        {
        }

        /// <summary>
        /// Create a new undo record.
        /// </summary>
        /// <param name="spawner">The acting spawner the record will belong to.</param>
        /// <param name="target">The target of the spawning action.</param>
        /// <param name="description">Description of the action.</param>
        public UndoRecord(Spawner spawner, Transform target, string description)
        {
            RecordBefore(spawner, target, description);
        }

        /// <summary>
        /// Records the before states.
        /// </summary>
        public void RecordBefore(Spawner spawner, Transform target, string description)
        {
            m_description = description;
            m_actingSpawner = spawner;
            m_target = target;

            Before = new GeNaUndoSnapshot(m_actingSpawner, m_target);

            // Record active children and Record 'Before' for them as well
            m_childSpawners = spawner.ActiveChildSpawners.ToArray();
        }

        /// <summary>
        /// Records the after states and updates the description.
        /// </summary>
        public void RecordAfter(string description)
        {
            After = new GeNaUndoSnapshot(m_actingSpawner, m_target);
            m_description = description;
			m_time = Utils.GetFrapoch();

            // Record for the children as well
            for (int i = 0; i < m_childSpawners.Length; i++)
            {
                m_childSpawners[i].RecordUndo(description);
            }

			// Record Extension activity
			if (m_actingSpawner.m_extensionUndoList != null)
			{
				int count = m_actingSpawner.m_extensionUndoList.Count;
				m_spawnedExtensions = new ExtensionInstance[count];
				for (int i = 0; i < m_spawnedExtensions.Length; i++)
				{
					ExtensionInstance extension = m_actingSpawner.m_extensionUndoList[count - 1 - i];

					// Add it to the appropriate list depending on if it's stateful (component) or stateless (can't serialise stateless
					m_spawnedExtensions[i] = m_actingSpawner.m_extensionUndoList[count - 1 - i];
				}
			}
			else
			{
				m_spawnedExtensions = new ExtensionInstance[0];
			}
		}

        /// <summary>
        /// Calculates the diffs.
        /// </summary>
        protected void GetDiffs()
        {
            m_diffs = After - Before;
        }

        #region Custom Serialization

        /// <summary>
        /// Berore serialisation
        /// </summary>
        public void OnBeforeSerialize()
        {
            if (m_diffs == null)
            {
                GetDiffs();
            }
        }

        /// <summary>
        /// After Deserialise
        /// </summary>
        public void OnAfterDeserialize()
        {
        }

        #endregion
    }
}
