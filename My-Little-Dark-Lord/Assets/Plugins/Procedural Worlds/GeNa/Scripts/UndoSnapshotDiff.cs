// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GeNa.Internal
{
    [Serializable]
    internal class UndoSnapshotDiff : ISerializationCallbackReceiver
    {
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
        /// Spawned Instance Count diff of the Spawner.
        /// </summary>
        public long SpawnerInstanceCount { get { return m_spawnerInstanceCount; } }
        [SerializeField] private long m_spawnerInstanceCount;

        /// <summary>
        /// Spawned Instance counts diff of the Prototypes of the Spawner.
        /// </summary>
        public long[] ProtoInstanceCounts { get { return m_protoInstanceCounts; } }
        [SerializeField] private long[] m_protoInstanceCounts;

        /// <summary>
        /// Spawned Instance counts diffs of each Resource of each Prototypes of the Spawner.
        /// </summary>
        // Jagged array with custom serialization
        public long[][] ResourceInstanceCounts { get; private set; }
        [SerializeField] private long[] _allResourceInstanceCounts;
        [SerializeField] private int[] _resourceInstanceCountsLengths;

        /// <summary>
        /// Width of the terrain's heightmap when this diff was recorded.
        /// </summary>
        public int HeightmapWidth { get { return m_heightmapWidth; } }
        [SerializeField] private int m_heightmapWidth;
        /// <summary>
        /// Height of the terrain's heightmap when this diff was recorded.
        /// </summary>
        public int HeightmapHeight { get { return m_heightmapHeight; } }
        [SerializeField] private int m_heightmapHeight;

        /// <summary>
        /// Contains the diffs to the height map in a two dimensional array that maps to the actual 
        /// heightmap as follows: (<see cref="HeightsBaseX"/>, <see cref="HeightsBaseY"/>, 
        /// <see cref="HeightsWidth"/>, <see cref="HeightsHeight"/>).
        /// </summary>
        public float[,] HeightDiffs { get; private set; }
        [SerializeField] private float[] _flatHeightmap;
        /// <summary>
        /// The x coordinate on the terrain heightmap where the diffs (<see cref="HeightDiffs"/>) base map to.
        /// </summary>
        public int HeightsBaseX { get { return m_heightsBaseX; } }
        [SerializeField] private int m_heightsBaseX;
        /// <summary>
        /// The y coordinate on the terrain heightmap where the diffs (<see cref="HeightDiffs"/>) base map to.
        /// </summary>
        public int HeightsBaseY { get { return m_heightsBaseY; } }
        [SerializeField] private int m_heightsBaseY;
        /// <summary>
        /// The width of the height diffs (<see cref="HeightDiffs"/>).
        /// </summary>
        public int HeightsWidth { get { return m_heightsWidth; } }
        [SerializeField] private int m_heightsWidth;
        /// <summary>
        /// The height of the height diffs (<see cref="HeightDiffs"/>).
        /// </summary>
        public int HeightsHeight { get { return m_heightsHeight; } }
        [SerializeField] private int m_heightsHeight;

        /// <summary>
        /// Width of the terrain's splatmap when this diff was recorded.
        /// </summary>
        public int SplatmapWidth { get { return m_splatmapWidth; } }
        [SerializeField] private int m_splatmapWidth;
        /// <summary>
        /// Height of the terrain's splatmap when this diff was recorded.
        /// </summary>
        public int SplatmapHeight { get { return m_splatmapHeight; } }
        [SerializeField] private int m_splatmapHeight;
        /// <summary>
        /// Layer count of the terrain's splatmap when this diff was recorded.
        /// </summary>
        public int SplatmapLayers { get { return m_splatmapLayers; } }
        [SerializeField] private int m_splatmapLayers;

        /// <summary>
        /// Contains the diffs to the splatmap in a three dimensional array that maps to the actual 
        /// splatmap as follows: (<see cref="SplatBaseX"/>, <see cref="SplatBaseY"/>, 
        /// <see cref="SplatWidth"/>, <see cref="SplatHeight"/>).
        /// </summary>
        public float[,,] SplatDiffs { get; private set; }
        [SerializeField] private float[] _flatSplatmap;
        /// <summary>
        /// The x coordinate on the terrain splatmap where the diffs (<see cref="SplatDiffs"/>) base map to.
        /// </summary>
        public int SplatBaseX { get { return m_splatBaseX; } }
        [SerializeField] private int m_splatBaseX;
        /// <summary>
        /// The y coordinate on the terrain splatmap where the diffs (<see cref="SplatDiffs"/>) base map to.
        /// </summary>
        public int SplatBaseY { get { return m_splatBaseY; } }
        [SerializeField] private int m_splatBaseY;
        /// <summary>
        /// The width of the splat diffs (<see cref="SplatDiffs"/>).
        /// </summary>
        public int SplatWidth { get { return m_splatWidth; } }
        [SerializeField] private int m_splatWidth;
        /// <summary>
        /// The height of the splat diffs (<see cref="SplatDiffs"/>).
        /// </summary>
        public int SplatHeight { get { return m_splatHeight; } }
        [SerializeField] private int m_splatHeight;

        /// <summary>
        /// Width of the terrain's detailmap when this diff was recorded.
        /// </summary>
        public int DetailmapWidth { get { return m_detailmapWidth; } }
        [SerializeField] private int m_detailmapWidth;
        /// <summary>
        /// Height of the terrain's detailmap when this diff was recorded.
        /// </summary>
        public int DetailmapHeight { get { return m_detailmapHeight; } }
        [SerializeField] private int m_detailmapHeight;
        /// <summary>
        /// Layer count of the terrain's detailmap when this diff was recorded.
        /// </summary>
        public int DetailmapLayers { get { return m_detailmapLayers; } }
        [SerializeField] private int m_detailmapLayers;

        /// <summary>
        /// The detailmap diffs per layer.
        /// </summary>
        public List<DetailDiff> DetailDiffs { get { return m_detailDiffs; } }
        [SerializeField] private List<DetailDiff> m_detailDiffs;

        /// <summary>
        /// Tree instances added diff.
        /// </summary>
        public HashSet<STreeInstance> TreesAdded { get; private set; }
        [SerializeField] private STreeInstance[] _treesAdded;

        /// <summary>
        /// Tree instances removed diff.
        /// </summary>
        public HashSet<STreeInstance> TreesRemoved { get; private set; }
        [SerializeField] private STreeInstance[] _treesRemoved;

        /// <summary>
        /// The acting spawner.
        /// </summary>
        public Spawner Spawner { get { return m_spawner; } }
        [SerializeField] private Spawner m_spawner;

        /// <summary>
        /// The target of spawning actions.
        /// </summary>
        public Transform Target { get { return m_target; } }
        [SerializeField] private Transform m_target;

        /// <summary>
        /// Prefabs that were spawned.
        /// </summary>
        internal HashSet<GameObject> SpawnedPrefabs { get; private set; }
        [SerializeField] private GameObject[] _spawnedPrefabsArray;

        /// <summary>
        /// Probes that were spawned.
        /// </summary>
        internal HashSet<GameObject> SpawnedProbes { get; private set; }
        [SerializeField] private GameObject[] _spawnedProbesArray;

        /// <summary>
        /// Parent objects that were added during this spawn action.
        /// </summary>
        internal HashSet<GameObject> ParentsAdded { get; private set; }
        [SerializeField] private GameObject[] _parentsAddedArray;

        /// <summary>
        /// Create a caluculated diff for a before and after snapsot.
        /// </summary>
        public UndoSnapshotDiff(GeNaUndoSnapshot before, GeNaUndoSnapshot after)
        {
            Init(before, after);
        }

        /// <summary>
        /// Initialises the object
        /// </summary>
        public void Init(GeNaUndoSnapshot before, GeNaUndoSnapshot after)
        {
            bool valid = true;
            if (after.Spawner != before.Spawner)
            {
                Debug.LogError("[GeNa]: Start/end spawner is not the same for undo/redo action.");
                valid = false;
            }

            if (after.Target != before.Target)
            {
                Debug.LogError("[GeNa]: Start/end target is not the same for undo/redo action.");
                valid = false;
            }

            if (after.ProtoInstanceCounts.Length != before.ProtoInstanceCounts.Length || after.ResourceInstanceCounts.Length != before.ResourceInstanceCounts.Length)
            {
                Debug.LogError("[GeNa]: Prototype count was changed between the two states. Aborting undo/redo.");
                valid = false;
            }

            if (!valid)
            {
                return;
            }

            m_spawner = after.Spawner;
            m_target = after.Target;

            //Instance counts
            m_spawnerInstanceCount = after.SpawnerInstanceCount - before.SpawnerInstanceCount;

            if (before.ProtoInstanceCounts.Length != after.ProtoInstanceCounts.Length || before.ResourceInstanceCounts.Length != after.ResourceInstanceCounts.Length)
            {
                Debug.LogWarningFormat("[GeNa]: Prototype count has changed unexpectedly ({0}/{1} vs {2}/{3}). Won't be able to undo/redo spawned instance counts for this record.",
                    before.ProtoInstanceCounts.Length, before.ResourceInstanceCounts.Length, after.ProtoInstanceCounts.Length, after.ResourceInstanceCounts.Length);
            }
            else
            {
                m_protoInstanceCounts = new long[before.ProtoInstanceCounts.Length];
                ResourceInstanceCounts = new long[before.ProtoInstanceCounts.Length][];
                for (int i = 0; i < before.ProtoInstanceCounts.Length; i++)
                {
                    m_protoInstanceCounts[i] = after.ProtoInstanceCounts[i] - before.ProtoInstanceCounts[i];

                    if (before.ResourceInstanceCounts[i].Length != after.ResourceInstanceCounts[i].Length)
                    {
                        Debug.LogWarningFormat("[GeNa]: Resource count has changed unexpectedly in prototype '{0}' ({1} vs {2}). Won't be able to undo/redo spawned instance counts in this prototype.",
                            m_spawner.m_spawnPrototypes[i].m_name, before.ResourceInstanceCounts[i].Length, after.ResourceInstanceCounts[i].Length);
                        continue;
                    }

                    ResourceInstanceCounts[i] = new long[before.ResourceInstanceCounts[i].Length];
                    for (int ri = 0; ri < before.ResourceInstanceCounts[i].Length; ri++)
                    {
                        ResourceInstanceCounts[i][ri] = after.ResourceInstanceCounts[i][ri] - before.ResourceInstanceCounts[i][ri];
                    }
                }
            }

            // Spawned prefabs
            SpawnedPrefabs = new HashSet<GameObject>();
            for (int i = before.PrefabUndoCount; i < after.PrefabUndoCount; i++)
            {
                SpawnedPrefabs.Add(m_spawner.m_prefabUndoList[i]);
            }

            // Spawned Probes
            SpawnedProbes = new HashSet<GameObject>();
            for (int i = before.ProbeUndoCount; i < after.ProbeUndoCount; i++)
            {
                SpawnedProbes.Add(m_spawner.m_probeUndoList[i]);
            }            

            // Added parents
            ParentsAdded = new HashSet<GameObject>();
            for (int i = before.ParentUndoCount; i < after.ParentUndoCount; i++)
            {
                ParentsAdded.Add(m_spawner.m_parentsUndoList[i]);
            }

            if (m_target == null)
            {
                return;
            }

            Terrain terrain = m_target.GetComponent<Terrain>();
            if (terrain != null)
            {
                // Height
                if (before.Heightmap != null)
                {
                    if (before.Heightmap.GetLength(0) == after.Heightmap.GetLength(0) && before.Heightmap.GetLength(1) == after.Heightmap.GetLength(1))
                    {
                        float[,] height = new float[before.Heightmap.GetLength(0), before.Heightmap.GetLength(1)];
                        int minX = before.Heightmap.GetLength(1);
                        int minY = before.Heightmap.GetLength(0);
                        int maxX = -1;
                        int maxY = -1;

                        for (int y = 0; y < before.Heightmap.GetLength(0); y++)
                        {
                            for (int x = 0; x < before.Heightmap.GetLength(1); x++)
                            {
                                float diff = after.Heightmap[y, x] - before.Heightmap[y, x];

                                if (MeaningfulDiff(diff))
                                {
                                    if (x < minX) { minX = x; }
                                    if (x > maxX) { maxX = x; }
                                    if (y < minY) { minY = y; }
                                    if (y > maxY) { maxY = y; }

                                    height[y, x] = diff;
                                }
                            }
                        }

                        if (minX <= maxX && minY <= maxY)
                        {
                            m_heightmapHeight = before.Heightmap.GetLength(0);
                            m_heightmapWidth = before.Heightmap.GetLength(1);

                            m_heightsBaseX = minX;
                            m_heightsBaseY = minY;
                            m_heightsWidth = maxX - minX + 1;
                            m_heightsHeight = maxY - minY + 1;

                            HeightDiffs = new float[m_heightsHeight, m_heightsWidth];

                            // Array.Copy won't work for 2D, so have to go about it as would in most cases
                            for (int y = minY; y <= maxY; y++)
                            {
                                for (int x = minX; x <= maxX; x++)
                                {
                                    HeightDiffs[y - minY, x - minX] = height[y, x];
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarningFormat("[GeNa]: Heightmap dimenions of the terrain changed ({0} x {1} vs {2} x {3}). Won't be able to undo/redo height changes.",
                            before.Heightmap.GetLength(1), before.Heightmap.GetLength(0), after.Heightmap.GetLength(1), after.Heightmap.GetLength(0));
                    }
                }

                // Splat
                if (before.Splatmap != null)
                {
                    if (before.Splatmap.GetLength(1) == after.Splatmap.GetLength(1) && before.Splatmap.GetLength(0) == after.Splatmap.GetLength(0) && 
                        before.Splatmap.GetLength(2) == after.Splatmap.GetLength(2))
                    {
                        float[,,] splat = new float[before.Splatmap.GetLength(0), before.Splatmap.GetLength(1), before.Splatmap.GetLength(2)];
                        int minX = before.Splatmap.GetLength(1);
                        int minY = before.Splatmap.GetLength(0);
                        int maxX = -1;
                        int maxY = -1;

                        for (int y = 0; y < before.Splatmap.GetLength(0); y++)
                        {
                            for (int x = 0; x < before.Splatmap.GetLength(1); x++)
                            {
                                for (int l = 0; l < before.Splatmap.GetLength(2); l++)
                                {
                                    float diff = after.Splatmap[y, x, l] - before.Splatmap[y, x, l];

                                    if (MeaningfulDiff(diff))
                                    {
                                        if (x < minX) { minX = x; }
                                        if (x > maxX) { maxX = x; }
                                        if (y < minY) { minY = y; }
                                        if (y > maxY) { maxY = y; }

                                        splat[y, x, l] = diff;
                                    }
                                }
                            }
                        }

                        if (minX <= maxX && minY <= maxY)
                        {
                            m_splatmapHeight = before.Splatmap.GetLength(0);
                            m_splatmapWidth = before.Splatmap.GetLength(1);
                            m_splatmapLayers = before.Splatmap.GetLength(2);

                            m_splatBaseX = minX;
                            m_splatBaseY = minY;
                            m_splatWidth = maxX - minX + 1;
                            m_splatHeight = maxY - minY + 1;

                            SplatDiffs = new float[m_splatHeight, m_splatWidth, before.Splatmap.GetLength(2)];

                            // Array.Copy won't work for 2D, so have to go about it as would in most cases
                            for (int y = minY; y <= maxY; y++)
                            {
                                for (int x = minX; x <= maxX; x++)
                                {
                                    for (int l = 0; l < SplatDiffs.GetLength(2); l++)
                                    {
                                        SplatDiffs[y - minY, x - minX, l] = splat[y, x, l];
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarningFormat("[GeNa]: Splat dimenions of the terrain changed ([{0} x {1} x {2}] vs [{3} x {4} x {5}]). Won't be able to undo/redo texture paint.",
                            before.Splatmap.GetLength(1), before.Splatmap.GetLength(0), before.Splatmap.GetLength(2), after.Splatmap.GetLength(1), after.Splatmap.GetLength(0), after.Splatmap.GetLength(2));
                    }
                }

                // Detail
                if (before.Detailmap != null)
                {
                    if (before.Detailmap.Length == after.Detailmap.Length)
                    {
                        for (int l = 0; l < before.Detailmap.Length; l++)
                        {
                            int[,] details = new int[before.Detailmap[l].GetLength(0), before.Detailmap[l].GetLength(1)];
                            int minX = before.Detailmap[l].GetLength(1);
                            int minY = before.Detailmap[l].GetLength(0);
                            int maxX = -1;
                            int maxY = -1;

                            if (before.Detailmap[l].GetLength(0) != after.Detailmap[l].GetLength(0) || before.Detailmap[l].GetLength(1) != after.Detailmap[l].GetLength(1))
                            {
                                Debug.LogWarningFormat("[GeNa]: Detail layer dimensions changed for '{0}' ({1} x {2} vs {3} x {4}). " +
                                    "Won't be able to undo/redo grass spawn for this prototype.", terrain.terrainData.detailPrototypes[l].prototype.name, 
                                    before.Detailmap[l].GetLength(1), before.Detailmap[l].GetLength(0), after.Detailmap[l].GetLength(1), after.Detailmap[l].GetLength(0));
                                continue;
                            }

                            for (int y = 0; y < before.Detailmap[l].GetLength(0); y++)
                            {
                                for (int x = 0; x < before.Detailmap[l].GetLength(1); x++)
                                {
                                    int diff = after.Detailmap[l][y, x] - before.Detailmap[l][y, x];

                                    if (diff != 0)
                                    {
                                        if (x < minX) { minX = x; }
                                        if (x > maxX) { maxX = x; }
                                        if (y < minY) { minY = y; }
                                        if (y > maxY) { maxY = y; }

                                        details[y, x] = diff;
                                    }
                                }
                            }

                            if (minX <= maxX && minY <= maxY)
                            {
                                int width = maxX - minX + 1;
                                int height = maxY - minY + 1;

                                int[,] detDiff = new int[height, width];

                                // Array.Copy won't work for 2D, so have to go about it as would in most cases
                                for (int y = minY; y <= maxY; y++)
                                {
                                    for (int x = minX; x <= maxX; x++)
                                    {
                                        detDiff[y - minY, x - minX] = details[y, x];
                                    }
                                }

                                if (m_detailDiffs == null)
                                {
                                    m_detailDiffs = new List<DetailDiff>();
                                }
                                m_detailDiffs.Add(new DetailDiff(l, minX, minY, detDiff));
                            }
                        }

                        if (m_detailDiffs != null && m_detailDiffs.Count > 0)
                        {
                            m_detailmapHeight = before.Detailmap[0].GetLength(0);
                            m_detailmapWidth = before.Detailmap[0].GetLength(1);
                            m_detailmapLayers = before.Detailmap.Length;
                        }
                    }
                    else
                    {
                        Debug.LogWarningFormat("[GeNa]: Detail prototypes count changed ({0} vs {1}). Won't be able to undo/redo grass spawn.",
                            before.Detailmap.Length, after.Detailmap.Length);
                    }
                }

                // Trees
                if (before.TreeInstances != null)
                {
                    HashSet<TreeInstance> treesAdded = null;

                    //According to the current functionality we can either add trees
                    if (after.TreeInstances.Length > before.TreeInstances.Length)
                    {
                        treesAdded = GetTreeArrayDiff(after.TreeInstances, before.TreeInstances);
                    }

                    if (treesAdded != null && treesAdded.Count > 0)
                    {
                        TreeInstance[] addedArray = new List<TreeInstance>(treesAdded).ToArray();
                        STreeInstance[] serialisableArray = Array.ConvertAll(addedArray, item => (STreeInstance)item);

                        TreesAdded = new HashSet<STreeInstance>(serialisableArray);
                    }
                }
            }
        }

        /// <summary>
        /// Only interested if it's an actual diff and not float inaccuraccy 1E-5f is a fairly safe bet.
        /// </summary>
        private static bool MeaningfulDiff(float diff)
        {
            return Mathf.Abs(diff) > 1E-5f;
        }

        /// <summary>
        /// Create a blank snapshot. This is only used by snapshot diff.
        /// </summary>
        protected UndoSnapshotDiff()
        {
        }

        #region Helper methods

        /// <summary>
        /// Get the difference between tree arrays.
        /// </summary>
        private static HashSet<TreeInstance> GetTreeArrayDiff(TreeInstance[] superset, TreeInstance[] smallerSet)
        {
            HashSet<TreeInstance> supSet = new HashSet<TreeInstance>(superset);
            HashSet<TreeInstance> smallSet = new HashSet<TreeInstance>(smallerSet);

            if (supSet.IsSupersetOf(smallSet) == false)
            {
                Debug.LogError("Unable to undo/redo tree changes. The snapshots seem to be disconnected.");
                return null;
            }

            supSet.ExceptWith(smallSet);
            return supSet;
        }

        #endregion

        #region Custom Serialization

        /// <summary>
        /// Berore serialisation
        /// </summary>
        public void OnBeforeSerialize()
        {
            // First flatten the resource counts to have the counts for all protos in a single array
            List<long> allCounts = new List<long>();
            List<int> lengths = new List<int>();
            if (ResourceInstanceCounts != null)
            {
                foreach (long[] resCountsInProto in ResourceInstanceCounts)
                {
                    allCounts.AddRange(resCountsInProto);
                    lengths.Add(resCountsInProto.Length);
                }
            }
            _allResourceInstanceCounts = allCounts.ToArray();
            _resourceInstanceCountsLengths = lengths.ToArray();

            // Flatten the heightmap
            if (HeightDiffs != null && HeightDiffs.Length > 0)
            {
                _flatHeightmap = new float[m_heightsWidth * m_heightsHeight];

                int i = 0;
                for (int y = 0; y < m_heightsHeight; y++)
                {
                    for (int x = 0; x < m_heightsWidth; x++)
                    {
                        _flatHeightmap[i] = HeightDiffs[y, x];
                        i++;
                    }
                }
            }

            // Flatten the splat layers
            if (SplatDiffs != null && SplatDiffs.Length > 0)
            {
                _flatSplatmap = new float[SplatDiffs.GetLength(2) * m_splatWidth * m_splatHeight];

                int i = 0;
                for (int l = 0; l < SplatDiffs.GetLength(2); l++)
                {
                    for (int y = 0; y < m_splatHeight; y++)
                    {
                        for (int x = 0; x < m_splatWidth; x++)
                        {
                            _flatSplatmap[i] = SplatDiffs[y, x, l];
                            i++;
                        }
                    }
                }
            }

            // Trees
            if (TreesAdded != null)
            {
                _treesAdded = new List<STreeInstance>(TreesAdded).ToArray();
            }
            if (TreesRemoved != null)
            {
                _treesRemoved = new List<STreeInstance>(TreesRemoved).ToArray();
            }

            // GOs
            if (SpawnedPrefabs != null)
            {
                _spawnedPrefabsArray = new List<GameObject>(SpawnedPrefabs).ToArray();
            }
            if (SpawnedProbes != null)
            {
                _spawnedProbesArray = new List<GameObject>(SpawnedProbes).ToArray();
            }
            if (ParentsAdded != null)
            {
                _parentsAddedArray = new List<GameObject>(ParentsAdded).ToArray();
            }
        }

        /// <summary>
        /// After Deserialise
        /// </summary>
        public void OnAfterDeserialize()
        {
            // Extract the Resource Intance Counts
            ResourceInstanceCounts = new long[_resourceInstanceCountsLengths.Length][];
            int protoIx = 0;
            int i = 0;
            foreach (int length in _resourceInstanceCountsLengths)
            {
                ResourceInstanceCounts[protoIx] = new long[length];
                for (int resIx = 0; resIx < length; resIx++)
                {
                    ResourceInstanceCounts[protoIx][resIx] = _allResourceInstanceCounts[i + resIx];
                }
                protoIx++;
                i += length;
            }

            // Extract the Splat layers
            int mapArea = m_heightsWidth * m_heightsHeight;
            if (_flatHeightmap != null && mapArea > 0)
            {
                if (_flatHeightmap.Length % mapArea == 0)
                {
                    HeightDiffs = new float[m_heightsWidth, m_heightsHeight];
                    i = 0;
                    for (int y = 0; y < m_heightsHeight; y++)
                    {
                        for (int x = 0; x < m_heightsWidth; x++)
                        {
                            HeightDiffs[y, x] = _flatHeightmap[i];
                            i++;
                        }
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Serialisation Error: Heightmap Sequence length({0}) doesn't indicate that it holds a map of size {1} x {2}",
                        _flatHeightmap.Length, m_heightsWidth, m_heightsWidth);
                }
            }

            // Extract the Splat layers
            mapArea = m_splatWidth * m_splatHeight;
            if (_flatSplatmap != null && mapArea > 0)
            {
                if (_flatSplatmap.Length % mapArea == 0)
                {
                    SplatDiffs = new float[m_splatHeight, m_splatWidth, _flatSplatmap.Length / mapArea];
                    i = 0;
                    for (int l = 0; l < SplatDiffs.GetLength(2); l++)
                    {
                        for (int y = 0; y < m_splatHeight; y++)
                        {
                            for (int x = 0; x < m_splatWidth; x++)
                            {
                                SplatDiffs[y, x, l] = _flatSplatmap[i];
                                i++;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Serialisation Error: Splatmap Sequence length({0}) doesn't indicate that it holds layers of size {1} x {2}",
                        _flatSplatmap.Length, m_splatWidth, m_splatHeight);
                }
            }

            // Trees
            if (_treesAdded != null && _treesAdded.Length > 0)
            {
                TreesAdded = new HashSet<STreeInstance>(_treesAdded);
            }
            if (_treesRemoved != null && _treesRemoved.Length > 0)
            {
                TreesRemoved = new HashSet<STreeInstance>(_treesRemoved);
            }

            // GOs
            if (_spawnedPrefabsArray != null && _spawnedPrefabsArray.Length > 0)
            {
                SpawnedPrefabs = new HashSet<GameObject>(_spawnedPrefabsArray);
            }
            if (_spawnedProbesArray != null && _spawnedProbesArray.Length > 0)
            {
                SpawnedProbes = new HashSet<GameObject>(_spawnedProbesArray);
            }
            if (_parentsAddedArray != null && _parentsAddedArray.Length > 0)
            {
                ParentsAdded = new HashSet<GameObject>(_parentsAddedArray);
            }
        }

        #endregion

        #region Helper classes, structs

        /// <summary>
        /// Diffs of a detail layer.
        /// </summary>
        [Serializable]
        internal struct DetailDiff : ISerializationCallbackReceiver
        {
            /// <summary>
            /// The detail layer which this diff applies to.
            /// </summary>
            public int LayerIndex { get { return m_layerIndex; } }
            [SerializeField] private int m_layerIndex;

            /// <summary>
            /// The x base base coordinate where this diff maps to the terrain's detail map.
            /// </summary>
            public int BaseX { get { return m_baseX; } }
            [SerializeField] private int m_baseX;

            /// <summary>
            /// The y base base coordinate where this diff maps to the terrain's detail map.
            /// </summary>
            public int BaseY { get { return m_baseY; } }
            [SerializeField] private int m_baseY;

            /// <summary>
            /// Witdht of diff.
            /// </summary>
            public int Width { get { return m_width; } }
            [SerializeField] private int m_width;

            /// <summary>
            /// Height of diff.
            /// </summary>
            public int Height { get { return m_height; } }
            [SerializeField] private int m_height;

            /// <summary>
            /// The diff values.
            /// </summary>
            public int[,] Values { get; private set; }
            [SerializeField] private int[] _flatValues;

            /// <summary>
            /// Create diff for a detail layer.
            /// </summary>
            public DetailDiff(int layerIndex, int baseX, int baseY, int[,] values)
            {
                m_layerIndex = layerIndex;
                m_baseX = baseX;
                m_baseY = baseY;
                m_width = values.GetLength(1);
                m_height = values.GetLength(0);
                Values = values;
                _flatValues = null;
            }

            /// <summary>
            /// OnBeforeSerialize
            /// </summary>
            public void OnBeforeSerialize()
            {
                _flatValues = new int[m_width * m_height];
                for (int y = 0; y < m_height; y++)
                {
                    for (int x = 0; x < m_width; x++)
                    {
                        _flatValues[y * m_width + x] = Values[y, x];
                    }
                }
            }

            /// <summary>
            /// OnAfterDeserialize
            /// </summary>
            public void OnAfterDeserialize()
            {
                if (_flatValues == null)
                {
                    Debug.LogWarningFormat("[GeNa] No values to deserialise for DetailDiff [{0}] ({1}, {2}, {3}, {4})",
                        m_layerIndex, m_baseX, m_baseY, m_width, m_height);
                    return;
                }

                Values = new int[m_height, m_width];
                for (int y = 0; y < m_height; y++)
                {
                    for (int x = 0; x < m_width; x++)
                    {
                        Values[y, x] = _flatValues[y * m_width + x];
                    }
                }
            }
        }

        #endregion
    }
}
