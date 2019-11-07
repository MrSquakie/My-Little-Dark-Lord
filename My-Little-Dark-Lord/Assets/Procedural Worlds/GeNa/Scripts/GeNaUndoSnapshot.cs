// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using System;

namespace GeNa.Internal
{
	internal class GeNaUndoSnapshot
    {
        /// <summary>
        /// The count of the prefab undo list of the spawner to undo to.
        /// </summary>
        public int PrefabUndoCount { get; private set; }

        /// <summary>
        /// The count of the probe undo list of the spawner to undo to.
        /// </summary>
        public int ProbeUndoCount { get; private set; }

        /// <summary>
        /// The count of the parent undo list of the spawner to undo to.
        /// </summary>
        public int ParentUndoCount { get; private set; }

        /// <details>
        /// Need to record spawned instances for
        ///   Spawner
        ///     Prototype1
        ///        Resource1
        ///        Resource2
        ///        ...
        ///     Prototype2
        ///        Resource1
        ///        Resource2
        ///        ...
        ///     ...
        /// </details> 

        /// <summary>
        /// Spawned Instance Count of the Spawner.
        /// </summary>
        public long SpawnerInstanceCount { get; private set; }

        /// <summary>
        /// Spawned Instance counts of the Prototypes of the Spawner.
        /// </summary>
        public long[] ProtoInstanceCounts { get; private set; }

        /// <summary>
        /// Spawned Instance counts of each Resource of each Prototypes of the Spawner.
        /// </summary>
        // Multi-dimensional array with custom serialization
        public long[][] ResourceInstanceCounts { get; private set; }

        /// <summary>
        /// Will record the heightmap if a target is a terrain and the spawner has height altering resources.
        /// </summary>
        public float[,] Heightmap { get; private set; }

        /// <summary>
        /// Will record the splatmap if a target is a terrain and the spawner has texture resources.
        /// </summary>
        public float[,,] Splatmap { get; private set; }

        /// <summary>
        /// Will record the detailmap if a target is a terrain and the spawner has grass resources.
        /// </summary>
        public int[][,] Detailmap { get; private set; }

        /// <summary>
        /// Will record terrain tree instances if a target is a terrain and the spawner has terrain tree resources.
        /// </summary>
        public TreeInstance[] TreeInstances { get; private set; }

        /// <summary>
        /// The acting spawner.
        /// </summary>
        public Spawner Spawner { get; private set; }

        /// <summary>
        /// The target of spawning actions.
        /// </summary>
        public Transform Target { get; private set; }

        /// <summary>
        /// Create a snapshot for a spawner and an action target.
        /// </summary>
        /// <param name="spawner">The spawner that this undo snapshot belongs to.</param>
        /// <param name="target">The spawn target which can be terrain or other stuff.</param>
        public GeNaUndoSnapshot(Spawner spawner, Transform target)
        {
            Init(spawner, target);
        }

        /// <summary>
        /// Initialises the object
        /// </summary>
        /// <param name="spawner">The spawner that this undo snapshot belongs to.</param>
        /// <param name="target">The spawn target which can be terrain or other stuff.</param>
        public void Init(Spawner spawner, Transform target)
        {
            Spawner = spawner;
            Target = target;

            PrefabUndoCount = spawner.m_prefabUndoList.Count;
            ProbeUndoCount = spawner.m_probeUndoList.Count;
            ParentUndoCount = spawner.m_parentsUndoList.Count;

            SpawnerInstanceCount = spawner.m_instancesSpawned;

            ProtoInstanceCounts = new long[spawner.m_spawnPrototypes.Count];
            ResourceInstanceCounts = new long[ProtoInstanceCounts.Length][];
            for (int i = 0; i < ProtoInstanceCounts.Length; i++)
            {
                ProtoInstanceCounts[i] = spawner.m_spawnPrototypes[i].m_instancesSpawned;

                ResourceInstanceCounts[i] = new long[spawner.m_spawnPrototypes[i].m_resourceTree.Count];
                for (int ri = 0; ri < ResourceInstanceCounts[i].Length; ri++)
                {
                    ResourceInstanceCounts[i][ri] = spawner.m_spawnPrototypes[i].m_resourceTree[ri].m_instancesSpawned;
                }
            }

            // Don't record target (like terrain) stuff if this snapshot is for a child spawner - the root Spawner will take care of all that.
            if (target == null)
            {
                return;
            }

            Terrain terrain = target.GetComponent<Terrain>();
            if (terrain != null)
            {
                TerrainData td = terrain.terrainData;

				// Height
                if (spawner.m_affectsHeight)
                {
                    Heightmap = td.GetHeights(0, 0, td.heightmapWidth, td.heightmapHeight);
                }

				// Splat
                if (spawner.m_affectsTexture)
                {
                    Splatmap = td.GetAlphamaps(0, 0, td.alphamapWidth, td.alphamapHeight);
                }

				// Detail
                if (spawner.m_affectsGrass)
                {
                    Detailmap = new int[td.detailPrototypes.Length][,];
                    for (int l = 0; l < Detailmap.Length; l++)
                    {
                        Detailmap[l] = td.GetDetailLayer(0, 0, td.detailWidth, td.detailHeight, l);
                    }
                }

				// Trees
                if (spawner.m_affectsTrees)
                {
                    TreeInstances = new TreeInstance[td.treeInstanceCount];
                    Array.Copy(td.treeInstances, TreeInstances, td.treeInstanceCount);
                }
            }
        }

        /// <summary>
        /// Create a blank snapshot. This is only used by snapshot diff.
        /// </summary>
        protected GeNaUndoSnapshot()
        {
        }

        /// <summary>
        /// Subtraction operator to get diff between two snapshots.
        /// </summary>
        public static UndoSnapshotDiff operator -(GeNaUndoSnapshot a, GeNaUndoSnapshot b)
        {
            return new UndoSnapshotDiff(b, a);
        //    bool valid = true;
        //    if (a.Spawner != b.Spawner)
        //    {
        //        Debug.LogError("[GeNa]: Start/end spawner is not the same for undo/redo action.");
        //        valid = false;
        //    }

        //    if (a.Target != b.Target)
        //    {
        //        Debug.LogError("[GeNa]: Start/end target is not the same for undo/redo action.");
        //        valid = false;
        //    }

        //    if (a.ProtoInstanceCounts.Length != b.ProtoInstanceCounts.Length || a.ResourceInstanceCounts.Length != b.ResourceInstanceCounts.Length)
        //    {
        //        Debug.LogError("[GeNa]: Prototype count was changed between the two states. Aborting undo/redo.");
        //        valid = false;
        //    }

        //    if (!valid)
        //    {
        //        return null;
        //    }

        //    GeNaUndoSnapshotDiff diff = new GeNaUndoSnapshotDiff();

        //    if (diff == null)
        //    {
        //        diff = new GeNaUndoSnapshotDiff();
        //    }

        //    diff.m_prefabUndoCount = a.PrefabUndoCount - b.PrefabUndoCount;
        //    diff.m_probeUndoCount = a.ProbeUndoCount - b.ProbeUndoCount;

        //    //Instance counts
        //    diff.m_spawnerInstanceCount = a.SpawnerInstanceCount - b.SpawnerInstanceCount;

        //    diff.m_protoInstanceCounts = new long[a.ProtoInstanceCounts.Length];
        //    diff.ResourceInstanceCounts = new long[a.ResourceInstanceCounts.Length][];
        //    for (int i = 0; i < diff.ProtoInstanceCounts.Length; i++)
        //    {
        //        diff.ProtoInstanceCounts[i] = a.ProtoInstanceCounts[i] - b.ProtoInstanceCounts[i];
                
        //        if (a.ResourceInstanceCounts[i].Length != b.ResourceInstanceCounts[i].Length)
        //        {
        //            Debug.LogError("[GeNa]: Resource count was changed between the two states. Aborting undo/redo.");
        //            return null;
        //        }

        //        diff.ResourceInstanceCounts[i] = new long[a.ResourceInstanceCounts[i].Length];
        //        for (int ri = 0; ri < diff.ResourceInstanceCounts[i].Length; ri++)
        //        {
        //            diff.ResourceInstanceCounts[i][ri] = a.ResourceInstanceCounts[i][ri] - b.ResourceInstanceCounts[i][ri];
        //        }
        //    }

        //    //Splat
        //    if (a.Splatmap != null)
        //    {
        //        diff.m_splatmap = new float[a.Splatmap.GetLength(0), a.Splatmap.GetLength(1), a.Splatmap.GetLength(2)];
        //        for (int y = 0; y < diff.Splatmap.GetLength(0); y++)
        //        {
        //            for (int x = 0; x < diff.Splatmap.GetLength(1); x++)
        //            {
        //                for (int l = 0; l < diff.Splatmap.GetLength(2); l++)
        //                {
        //                    diff.Splatmap[y, x, l] = b.Splatmap[y, x, l] - a.Splatmap[y, x, l];
        //                }
        //            }
        //        }
        //    }

        //    //Detail
        //    if (a.Detailmap != null)
        //    {
        //        diff.Detailmap = new int[a.Detailmap.Length][,];
        //        for (int l = 0; l < diff.Detailmap.Length; l++)
        //        {
        //            diff.Detailmap[l] = new int[a.Detailmap[l].GetLength(0), a.Detailmap[l].GetLength(1)];
        //            for (int y = 0; y < diff.Detailmap[l].GetLength(0); y++)
        //            {
        //                for (int x = 0; x < diff.Detailmap[l].GetLength(1); x++)
        //                {
        //                    diff.Detailmap[l][y, x] = b.Detailmap[l][y, x] - a.Detailmap[l][y, x];
        //                }
        //            }
        //        }
        //    }

        //    //Tree diffs
        //    if (a.TreeInstances != null)
        //    {
        //        //According to the current functionality we can either add trees
        //        if (a.TreeInstances.Length > b.TreeInstances.Length)
        //        {
        //            diff.SetTreeInstancesAdded(GetTreeArrayDiff(a.TreeInstances, b.TreeInstances));
        //        }
        //        //or remove them (while doind undo)
        //        else if (a.TreeInstances.Length < b.TreeInstances.Length)
        //        {
        //            diff.SetTreeInstancesRemoved(GetTreeArrayDiff(b.TreeInstances, a.TreeInstances));
        //        }
        //    }
        //  return diff;
        }

        #region Helper methods

        #endregion
    }
}
