// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using PWCommon2;

namespace GeNa.Internal
{
    public static class GeNaUndo
    {
        internal static UndoDropStack NewStack()
        {
            return new UndoDropStack(Preferences.UndoSteps);
        }

		internal static UndoDropStack NewStack(List<UndoRecord> records)
        {
            return new UndoDropStack(Preferences.UndoSteps, records);
        }

        /// <summary>
        /// Records undo.
        /// </summary>
        public static void RecordUndo(UndoRecord record)
        {
            Spawner spawner = record.Before.Spawner;
            if (spawner == null)
            {
                Debug.LogErrorFormat("[GeNa]: Spawner is missing for record '{0}'", record.Description);
                return;
            }

            if (spawner.UndoStack == null)
            {
                spawner.ClearUndoStack();
            }

            if (record.ContainSpawns)
            {
                spawner.UndoStack.Push(record);
            }
        }

        /// <summary>
        /// Records redo.
        /// </summary>
        public static void RecordRedo(UndoRecord record)
        {
            //**Not used for now        
            //Spawner spawner = record.Before.Spawner;
            //if (spawner == null)
            //{
            //    Debug.LogErrorFormat("[GeNa]: Spawner is missing for record '{0}'", record.Description);
            //    return;
            //}
            //
            //if (spawner.RedoStack == null)
            //{
            //    spawner.RedoStack = NewStack();
            //}
            //spawner.RedoStack.Push(record);
        }

        /// <summary>
        /// Returns the description of the next undo record or null.
        /// </summary>
        public static string GetUndoDescription(Spawner spawner)
        {
            UndoRecord record = spawner.UndoStack.Peek();
            if (record != null)
            {
                return record.Description;
            }

            return null;
        }

        ///// <summary>
        ///// Returns the description of the next redo record or null.
        ///// </summary>
        //public static string GetRedoDescription(Spawner spawner)
        //{
        //    UndoRecord record = spawner.RedoStack.Peek();
        //    if (record != null)
        //    {
        //        return record.Description;
        //    }

        //    return null;
        //}

        /// <summary>
        /// Undo the last record for a spawner.
        /// </summary>
        public static void Undo(Spawner spawner)
        {
            if (spawner == null || spawner.UndoStack == null || spawner.UndoStack.Count < 1)
            {
                return;
            }

            UndoRecord record = spawner.UndoStack.Pop();
            if (record == null)
            {
                Debug.LogErrorFormat("[GeNa] Undo record is null.");
                return;
            }

            ProcessUndo(record);

            // Do the children as well.
            foreach (Spawner child in record.Childs)
            {
                Undo(child);
            }

            RecordRedo(record);
        }

        ///// <summary>
        ///// Redo the last undone record for a spawner.
        ///// </summary>
        //public static void Redo(Spawner spawner)
        //{
        //    UndoRecord record = spawner.RedoStack.Pop();
        //    if (record == null)
        //    {
        //        return;
        //    }

        //    if (spawner.UndoStack == null)
        //    {
        //        spawner.UndoStack = NewStack();
        //    }
        //    spawner.UndoStack.Push(record);
        //}

        /// <summary>
        /// Undo a number of records for a spawner.
        /// </summary>
        public static void Undo(Spawner spawner, int count)
        {
            //Could go by calculating diff across many records, however different targets (e.g. terrain and non terrain) would be a problem
            //So need to go step by step even though the performance won't be as good.

#if UNITY_EDITOR
            float stepsCount = count;
            int step = 0;
#endif

            while (count > 0)
            {
#if UNITY_EDITOR
                if (EditorUtility.DisplayCancelableProgressBar("GeNa", string.Format("Undo {0}...", spawner.UndoStack.Peek().Description), step++ / stepsCount))
                {
                    break;
                }
#endif
                Undo(spawner);
				count--;
            }
#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
        }

        /// <summary>
        /// Undo all the records for a spawner.
        /// </summary>
        public static void UndoAll(Spawner spawner)
        {
            //Could go by calculating diff across many records, however different targets (e.g. terrain and non terrain) would be a problem
            //So need to go step by step even though the performance won't be as good.

#if UNITY_EDITOR
            float stepsCount = spawner.UndoStack.Count;
            int step = 0;
#endif

            while (spawner.UndoStack.Count > 0)
            {
#if UNITY_EDITOR
                if (EditorUtility.DisplayCancelableProgressBar("GeNa", string.Format("Undo {0}...", spawner.UndoStack.Peek().Description), step++ / stepsCount))
                {
                    break;
                }
#endif
                Undo(spawner);
            }
#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
        }

        ///// <summary>
        ///// Redo all undone records for a spawner.
        ///// </summary>
        //public static void RedoAll(Spawner spawner)
        //{

        //    if (spawner.UndoStack == null)
        //    {
        //        spawner.UndoStack = NewStack();
        //    }
        //}

        #region Helper methods

        private static void ProcessUndo(UndoRecord record)
        {
            UndoSnapshotDiff diff = record.Diffs;

            Spawner spawner = diff.Spawner;
            Transform target = diff.Target;

            //Instance counts
            spawner.m_instancesSpawned -= diff.SpawnerInstanceCount;

            if (diff.ProtoInstanceCounts.Length != spawner.m_spawnPrototypes.Count || diff.ResourceInstanceCounts.Length != spawner.m_spawnPrototypes.Count)
            {
                Debug.LogWarningFormat("[GeNa]: Prototype count has changed since the undo was recorded ({0}/{1} vs {2}). Can not undo/redo spawned instance counts.",
                    diff.ProtoInstanceCounts.Length, diff.ResourceInstanceCounts.Length, spawner.m_spawnPrototypes.Count);
            }
            else
            {
                for (int i = 0; i < diff.ProtoInstanceCounts.Length; i++)
                {
                    spawner.m_spawnPrototypes[i].m_instancesSpawned -= diff.ProtoInstanceCounts[i];

                    if (diff.ResourceInstanceCounts[i].Length != spawner.m_spawnPrototypes[i].m_resourceTree.Count)
                    {
                        Debug.LogWarningFormat("[GeNa]: Resource count has changed in prototype '{0}' since the undo was recorded ({1} vs {2}). Can not undo/redo spawned instance counts in this prototype.",
                            spawner.m_spawnPrototypes[i].m_name, diff.ResourceInstanceCounts[i].Length, spawner.m_spawnPrototypes[i].m_resourceTree.Count);
                        continue;
                    }

                    for (int ri = 0; ri < diff.ResourceInstanceCounts[i].Length; ri++)
                    {
                        spawner.m_spawnPrototypes[i].m_resourceTree[ri].m_instancesSpawned -= diff.ResourceInstanceCounts[i][ri];
                    }
                }
            }

            // Prefab changes
            if (diff.SpawnedPrefabs != null && spawner.m_prefabUndoList != null && diff.SpawnedPrefabs.Count > 0)
            {
                List<GameObject> newList = new List<GameObject>();

                for (int i = 0; i < spawner.m_prefabUndoList.Count; i++)
                {
                    if (diff.SpawnedPrefabs.Contains(spawner.m_prefabUndoList[i]))
                    {
                        Object.DestroyImmediate(spawner.m_prefabUndoList[i]);
                    }
                    else
                    {
                        newList.Add(spawner.m_prefabUndoList[i]);
                    }
                }

                spawner.m_prefabUndoList = newList;
            }

            // Probe changes
            if (diff.SpawnedProbes != null && spawner.m_probeUndoList != null && diff.SpawnedProbes.Count > 0)
            {
                List<GameObject> newList = new List<GameObject>();

                for (int i = 0; i < spawner.m_probeUndoList.Count; i++)
                {
                    if (diff.SpawnedProbes.Contains(spawner.m_probeUndoList[i]))
                    {
                        Object.DestroyImmediate(spawner.m_probeUndoList[i]);
                    }
                    else
                    {
                        newList.Add(spawner.m_probeUndoList[i]);
                    }
                }

                spawner.m_probeUndoList = newList;
            }

            // Parent objects that were added
            if (diff.ParentsAdded != null && spawner.m_parentsUndoList != null && diff.ParentsAdded.Count > 0)
            {
                List<GameObject> newList = new List<GameObject>();

                for (int i = 0; i < spawner.m_parentsUndoList.Count; i++)
                {
                    if (diff.ParentsAdded.Contains(spawner.m_parentsUndoList[i]))
                    {
                        Object.DestroyImmediate(spawner.m_parentsUndoList[i]);
                    }
                    else
                    {
                        newList.Add(spawner.m_parentsUndoList[i]);
                    }
                }

                spawner.m_parentsUndoList = newList;
            }

            // Signal the Extensions
            if (record.SpawnedExtensions != null)
            {
                for (int i = 0; i < record.SpawnedExtensions.Length; i++)
                {
                    IGeNaExtension extension = record.SpawnedExtensions[i].Instance;

                    // If null, there may have been a serialisation
                    if (extension == null)
                    {
                        if (record.SpawnedExtensions[i].Index < 0)
                        {
                            Debug.LogErrorFormat("[GeNa] Unable to trigger Extension Undo: Invalid Extension index: {0} (Parent Res: {1}; Stateless: {2})",
                                record.SpawnedExtensions[i].Index, record.SpawnedExtensions[i].Parent.m_name, record.SpawnedExtensions[i].IsStateless);
                            continue;
                        }

                        Resource res = record.SpawnedExtensions[i].Parent;

                        if (res == null)
                        {
                            Debug.LogErrorFormat("[GeNa] Unable to trigger Extension Undo: Parent Res null (Index: {0}; Stateless: {1})",
                                record.SpawnedExtensions[i].Index, record.SpawnedExtensions[i].IsStateless);
                            continue;
                        }

                        if (record.SpawnedExtensions[i].IsStateless)
                        {
                            if (record.SpawnedExtensions[i].Index > res.StatelessExtensions.Length - 1)
                            {
                                Debug.LogErrorFormat("[GeNa] Unable to trigger Extension Undo: Invalid Extension index: {0} > {1} (Parent Res: {2}; Stateless: {3})",
                                    record.SpawnedExtensions[i].Index, res.StatelessExtensions.Length - 1, record.SpawnedExtensions[i].Parent.m_name, record.SpawnedExtensions[i].IsStateless);
                                continue;
                            }

                            extension = (IGeNaExtension)System.Activator.CreateInstance(res.StatelessExtensions[record.SpawnedExtensions[i].Index]);
                        }
                        else
                        {
                            if (record.SpawnedExtensions[i].Index > res.Extensions.Length - 1)
                            {
                                Debug.LogErrorFormat("[GeNa] Unable to trigger Extension Undo: Invalid Extension index: {0} > {1} (Parent Res: {2}; Stateless: {3})",
                                    record.SpawnedExtensions[i].Index, res.Extensions.Length - 1, record.SpawnedExtensions[i].Parent.m_name, record.SpawnedExtensions[i].IsStateless);
                                continue;
                            }

                            extension = res.Extensions[record.SpawnedExtensions[i].Index];
                        }
                    }

                    if (extension == null)
                    {
                        Debug.LogErrorFormat("[GeNa] Unable to trigger Extension Undo: Extension instance is null (Parent Res: {0}; Stateless: {1}; Index: {2})",
                            record.SpawnedExtensions[i].Parent.m_name, record.SpawnedExtensions[i].IsStateless, record.SpawnedExtensions[i].Index);
                        continue;
                    }

                    extension.Undo();
                }
            }

            if (target == null)
            {
                return;
            }

            Terrain terrain = target.GetComponent<Terrain>();
            if (terrain != null)
            {
                TerrainData td = terrain.terrainData;

                // Height
                if (diff.HeightDiffs != null)
                {
                    if (diff.HeightmapHeight == td.heightmapHeight && diff.HeightmapWidth == td.heightmapWidth)
                    {
                        float[,] heights = td.GetHeights(diff.HeightsBaseX, diff.HeightsBaseY, diff.HeightsWidth, diff.HeightsHeight);

                        for (int y = 0; y < diff.HeightsHeight ; y++)
                        {
                            for (int x = 0; x < diff.HeightsWidth; x++)
                            {
                                heights[y, x] -= diff.HeightDiffs[y, x];
                            }
                        }

                        td.SetHeights(diff.HeightsBaseX, diff.HeightsBaseY, heights);
                    }
                    else
                    {
                        Debug.LogWarningFormat("[GeNa]: Heightmap dimenions of the terrain changed since the undo was recorded ({0} x {1} vs {2} x {3}). Aborting height undo/redo...",
                            diff.HeightmapWidth, diff.HeightmapHeight, td.heightmapWidth, td.heightmapHeight);
                    }
                }

                // Splat
                if (diff.SplatDiffs != null)
                {
                    if (diff.SplatmapWidth == td.alphamapWidth && diff.SplatmapHeight == td.alphamapHeight && diff.SplatmapLayers == td.alphamapLayers)
                    {
                        float[,,] splat = td.GetAlphamaps(diff.SplatBaseX, diff.SplatBaseY, diff.SplatWidth, diff.SplatHeight);

                        for (int y = 0; y < diff.SplatHeight; y++)
                        {
                            for (int x = 0; x < diff.SplatWidth ; x++)
                            {
                                for (int l = 0; l < diff.SplatmapLayers; l++)
                                {
                                    splat[y, x, l] -= diff.SplatDiffs[y, x, l];
                                }
                            }
                        }

                        td.SetAlphamaps(diff.SplatBaseX, diff.SplatBaseY, splat);
                    }
                    else
                    {
                        Debug.LogWarningFormat("[GeNa]: Splat dimenions of the terrain changed since the undo was recorded ({0} x {1} x {2} vs {3} x {4} x {5}). Aborting splat undo/redo...",
                            diff.SplatmapWidth, diff.SplatmapHeight, diff.SplatmapLayers, td.alphamapWidth, td.alphamapHeight, td.alphamapLayers);
                    }
                }

                // Detail
                if (diff.DetailDiffs != null && diff.DetailDiffs.Count > 0)
                {
                    if (diff.DetailmapLayers == td.detailPrototypes.Length)
                    {

                        if (diff.DetailmapWidth == td.detailWidth && diff.DetailmapHeight == td.detailHeight)
                        {
                            for (int l = 0; l < diff.DetailDiffs.Count; l++)
                            {
                                UndoSnapshotDiff.DetailDiff layerDiff = diff.DetailDiffs[l];

                                int[,] details = td.GetDetailLayer(layerDiff.BaseX, layerDiff.BaseY, layerDiff.Width, layerDiff.Height, layerDiff.LayerIndex);

                                for (int y = 0; y < layerDiff.Height; y++)
                                {
                                    for (int x = 0; x < layerDiff.Width; x++)
                                    {
                                        details[y, x] -= layerDiff.Values[y, x];
                                    }
                                }

                                td.SetDetailLayer(layerDiff.BaseX, layerDiff.BaseY, layerDiff.LayerIndex, details);
                            }
                        }
                        else
                        {
                            Debug.LogWarningFormat("[GeNa]: Detail layer dimensions of the terrain changed since the undo was recorded ({1} x {2} vs {3} x {4}).  Aborting details undo/redo...",
                                diff.DetailmapWidth, diff.DetailmapHeight, td.detailWidth, td.detailHeight);
                        }
                    }
                    else
                    {
                        Debug.LogWarningFormat("[GeNa]: Detail prototypes count of the terrain changed since the undo was recorded ({0} vs {1}). Aborting details undo/redo...",
                            diff.DetailmapLayers, td.detailPrototypes.Length);
                    }
                }

                // Trees
                if (diff.TreesAdded != null && diff.TreesAdded.Count > 0)
                {
                    TreeInstance[] trees = td.treeInstances;
                    List<TreeInstance> newTrees = new List<TreeInstance>();

                    for (int i = 0; i < trees.Length; i++)
                    {
                        if (diff.TreesAdded.Contains(trees[i]) == false)
                        {
                            newTrees.Add(trees[i]);
                        }
                    }

                    td.treeInstances = newTrees.ToArray();
                }
            }
        }

        #endregion
    }
}
