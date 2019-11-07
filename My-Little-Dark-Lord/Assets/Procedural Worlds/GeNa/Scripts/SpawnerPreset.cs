using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeNa
{
    [CreateAssetMenu(fileName = "New Preset", menuName = "Procedural Worlds/GeNa/Preset")]
    public class SpawnerPreset : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Static accessors

        /// <summary>
        /// The subfolder in Resources that contains Presets.
        /// </summary>
        public const string PRESETS_FOLDER = "GeNa Presets";

        /// <summary>
        /// Contains the available presets in the project.
        /// Presets are stored in the 'GeNa/Resources/GeNa Presets' folder.
        /// </summary>
        public static HashSet<SpawnerPreset> AvailablePresets
        {
            get
            {
                if (ms_dirty || ms_availablePresets == null)
                {
                    ms_availablePresets = new HashSet<SpawnerPreset>(Resources.LoadAll<SpawnerPreset>(PRESETS_FOLDER));
                    ms_dirty = false;
                }
                else
                {
                    // In case some presets were deleted
                    ms_availablePresets.RemoveWhere(preset => (preset == null));
                }


                return ms_availablePresets;
            }
        }
        protected static HashSet<SpawnerPreset> ms_availablePresets;

        /// <summary>
        /// Indicates that the available presets list needs to be updated.
        /// </summary>
        public static bool Dirty { set { ms_dirty = true; } }
        protected static bool ms_dirty = false;

        #endregion

        #region Non-Spawner Properties, members, and variables

        /// <summary>
        /// Name interface.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        public List<string> ActiveOptions
        {
            get
            {
                return new List<string>(m_activeProperties);
            }
        }

        /// <summary>
        /// Set that holds the active options.
        /// (Only active options will be applied to the Spawner).
        /// </summary>
        protected HashSet<string> m_activeProperties = new HashSet<string>();
        [SerializeField] private string[] _activeProperties;

        /// <summary>
        /// The number of active options in this preset.
        /// (Only active options will be applied to the Spawner).
        /// </summary>
        public int NumberOfActiveOptions
        {
            get
            {
                return m_activeProperties.Count;
            }
        }

        /// <summary>
        /// Check if a option is active in this preset 
        /// (Only active options will be applied to the Spawner).
        /// </summary>
        public bool IsActiveProperty(string propertyName)
        {
            return m_activeProperties.Contains(propertyName);
        }

        /// <summary>
        /// Activates an option.
        /// (Only active options will be applied to the Spawner).
        /// </summary>
        public void ActivateOption(string propertyName)
        {
            m_activeProperties.Add(propertyName);
        }

        /// <summary>
        /// Deactivates an option.
        /// (Only active options will be applied to the Spawner).
        /// </summary>
        public void DeactivateOption(string propertyName)
        {
            m_activeProperties.Remove(propertyName);
        }

        #endregion

        #region Overview

        /// <summary>
        /// Special property with the sole purpose of providing a header name
        /// </summary>
        protected bool CTGRHDR_Overview { get; }

        /// <summary>
        /// ParentName
        /// </summary>
        public string ParentName
        {
            get
            {
                return m_parentName;
            }
            set
            {
                m_parentName = value;
            }
        }
        [SerializeField] protected string m_parentName = "GeNa Spawner";

        /// <summary>
        /// Type
        /// </summary>
        public Constants.SpawnerType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }
        [SerializeField] protected Constants.SpawnerType m_type = Constants.SpawnerType.Regular;

        /// <summary>
        /// SpawnMode
        /// </summary>
        public Constants.SpawnMode SpawnMode
        {
            get
            {
                return m_spawnMode;
            }
            set
            {
                m_spawnMode = value;
            }
        }
        [SerializeField] protected Constants.SpawnMode m_spawnMode = Constants.SpawnMode.Single;
        /// <summary>
        /// FlowDistance
        /// </summary>
        public float FlowDistance
        {
            get
            {
                return m_flowDistance;
            }
            set
            {
                m_flowDistance = value;
            }
        }
        [SerializeField] protected float m_flowDistance = 30f;

        /// <summary>
        /// SpawnRangeShape
        /// </summary>
        public Constants.SpawnRangeShape SpawnRangeShape
        {
            get
            {
                return m_spawnRangeShape;
            }
            set
            {
                m_spawnRangeShape = value;
            }
        }
        [SerializeField] protected Constants.SpawnRangeShape m_spawnRangeShape = Constants.SpawnRangeShape.Circle;

        /// <summary>
        /// RandomSeed
        /// </summary>
        public int RandomSeed
        {
            get
            {
                return m_randomSeed;
            }
            set
            {
                m_randomSeed = value;
            }
        }
        [SerializeField] protected int m_randomSeed = 1000;

        /// <summary>
        /// MaxSpawnRange
        /// </summary>
        public float MaxSpawnRange
        {
            get
            {
                return m_maxSpawnRange;
            }
            set
            {
                m_maxSpawnRange = value;
            }
        }
        [SerializeField] protected float m_maxSpawnRange = 50f;
        /// <summary>
        /// SeedThrowRange
        /// </summary>
        public float SeedThrowRange
        {
            get
            {
                return m_seedThrowRange;
            }
            set
            {
                m_seedThrowRange = value;
            }
        }
        [SerializeField] protected float m_seedThrowRange = 5f;

        /// <summary>
        /// MinInstances
        /// </summary>
        public long MinInstances
        {
            get
            {
                return m_minInstances;
            }
            set
            {
                m_minInstances = value;
            }
        }
        [SerializeField] protected long m_minInstances = 1;
        /// <summary>
        /// MaxInstances
        /// </summary>
        public long MaxInstances
        {
            get
            {
                return m_maxInstances;
            }
            set
            {
                m_maxInstances = value;
            }
        }
        [SerializeField] protected long m_maxInstances = 1;

        /// <summary>
        /// MergeSpawns
        /// </summary>
        public bool MergeSpawns
        {
            get
            {
                return m_mergeSpawns;
            }
            set
            {
                m_mergeSpawns = value;
            }
        }
        [SerializeField] protected bool m_mergeSpawns = true;

        /// <summary>
        /// DoSectorise
        /// </summary>
        public bool DoSectorise
        {
            get
            {
                return m_doSectorise;
            }
            set
            {
                m_doSectorise = value;
            }
        }
        [SerializeField] protected bool m_doSectorise = true;
        /// <summary>
        /// SectorReparentingMode
        /// </summary>
        public int SectorReparentingMode
        {
            get
            {
                return m_sectorReparentingMode;
            }
            set
            {
                m_sectorReparentingMode = value;
            }
        }
        [SerializeField] protected int m_sectorReparentingMode = 0;

        #endregion

        #region Placement Criteria

        /// <summary>
        /// Special property with the sole purpose of providing a header name
        /// </summary>
        protected bool CTGRHDR_PlacementCriteria { get; }

        /// <summary>
        /// SpawnAlgorithm
        /// </summary>
        public Constants.LocationAlgorithm SpawnAlgorithm
        {
            get
            {
                return m_spawnAlgorithm;
            }
            set
            {
                m_spawnAlgorithm = value;
            }
        }
        [SerializeField] protected Constants.LocationAlgorithm m_spawnAlgorithm = Constants.LocationAlgorithm.Organic;
        /// <summary>
        /// SeedThrowJitter
        /// </summary>
        public float SeedThrowJitter
        {
            get
            {
                return m_seedThrowJitter;
            }
            set
            {
                m_seedThrowJitter = value;
            }
        }
        [SerializeField] protected float m_seedThrowJitter = 1f;

        /// <summary>
        /// RotationAlgorithm
        /// </summary>
        public GeNa.Constants.RotationAlgorithm RotationAlgorithm
        {
            get
            {
                return m_rotationAlgorithm;
            }
            set
            {
                m_rotationAlgorithm = value;
            }
        }
        [SerializeField] protected GeNa.Constants.RotationAlgorithm m_rotationAlgorithm = Constants.RotationAlgorithm.Ranged;
        /// <summary>
        /// EnableRotationDragUpdate
        /// </summary>
        public bool EnableRotationDragUpdate
        {
            get
            {
                return m_enableRotationDragUpdate;
            }
            set
            {
                m_enableRotationDragUpdate = value;
            }
        }
        [SerializeField] protected bool m_enableRotationDragUpdate = false;
        /// <summary>
        /// MinRotationY
        /// </summary>
        public float MinRotationY
        {
            get
            {
                return m_minRotationY;
            }
            set
            {
                m_minRotationY = value;
            }
        }
        [SerializeField] protected float m_minRotationY = 0f;
        /// <summary>
        /// MaxRotationY
        /// </summary>
        public float MaxRotationY
        {
            get
            {
                return m_maxRotationY;
            }
            set
            {
                m_maxRotationY = value;
            }
        }
        [SerializeField] protected float m_maxRotationY = 360f;

        /// <summary>
        /// SameScale
        /// </summary>
        public bool SameScale
        {
            get
            {
                return m_sameScale;
            }
            set
            {
                m_sameScale = value;
            }
        }
        [SerializeField] protected bool m_sameScale = true;
        /// <summary>
        /// MinScaleX
        /// </summary>
        public float MinScaleX
        {
            get
            {
                return m_minScaleX;
            }
            set
            {
                m_minScaleX = value;
            }
        }
        [SerializeField] protected float m_minScaleX = 0.7f;
        /// <summary>
        /// MaxScaleX
        /// </summary>
        public float MaxScaleX
        {
            get
            {
                return m_maxScaleX;
            }
            set
            {
                m_maxScaleX = value;
            }
        }
        [SerializeField] protected float m_maxScaleX = 1.3f;
        /// <summary>
        /// MinScaleY
        /// </summary>
        public float MinScaleY
        {
            get
            {
                return m_minScaleY;
            }
            set
            {
                m_minScaleY = value;
            }
        }
        [SerializeField] protected float m_minScaleY = 0.7f;
        /// <summary>
        /// MaxScaleY
        /// </summary>
        public float MaxScaleY
        {
            get
            {
                return m_maxScaleY;
            }
            set
            {
                m_maxScaleY = value;
            }
        }
        [SerializeField] protected float m_maxScaleY = 1.3f;
        /// <summary>
        /// MinScaleZ
        /// </summary>
        public float MinScaleZ
        {
            get
            {
                return m_minScaleZ;
            }
            set
            {
                m_minScaleZ = value;
            }
        }
        [SerializeField] protected float m_minScaleZ = 0.7f;
        /// <summary>
        /// MaxScaleZ
        /// </summary>
        public float MaxScaleZ
        {
            get
            {
                return m_maxScaleZ;
            }
            set
            {
                m_maxScaleZ = value;
            }
        }
        [SerializeField] protected float m_maxScaleZ = 1.3f;

        /// <summary>
        /// UseGravity
        /// </summary>
        public bool UseGravity
        {
            get
            {
                return m_useGravity;
            }
            set
            {
                m_useGravity = value;
            }
        }
        [SerializeField] protected bool m_useGravity = false;

        #endregion

        #region Spawn Criteria

        /// <summary>
        /// Special property with the sole purpose of providing a header name
        /// </summary>
        protected bool CTGRHDR_SpawnCriteria { get; }

        /// <summary>
        /// CritVirginCheckType
        /// </summary>
        public Constants.VirginCheckType CritVirginCheckType
        {
            get
            {
                return m_critVirginCheckType;
            }
            set
            {
                m_critVirginCheckType = value;
            }
        }
        [SerializeField] protected Constants.VirginCheckType m_critVirginCheckType = Constants.VirginCheckType.Point;
        /// <summary>
        /// CritBoundsBorder
        /// </summary>
        public float CritBoundsBorder
        {
            get
            {
                return m_critBoundsBorder;
            }
            set
            {
                m_critBoundsBorder = value;
            }
        }
        [SerializeField] protected float m_critBoundsBorder = 0f;
        public LayerMask m_critSpawnCollisionLayers;

        /// <summary>
        /// CritCheckHeightType
        /// </summary>
        public Constants.CriteriaRangeType CritCheckHeightType
        {
            get
            {
                return m_critCheckHeightType;
            }
            set
            {
                m_critCheckHeightType = value;
            }
        }
        [SerializeField] protected Constants.CriteriaRangeType m_critCheckHeightType = Constants.CriteriaRangeType.Mixed;
        /// <summary>
        /// CritMinSpawnHeight
        /// </summary>
        public float CritMinSpawnHeight
        {
            get
            {
                return m_critMinSpawnHeight;
            }
            set
            {
                m_critMinSpawnHeight = value;
            }
        }
        [SerializeField] protected float m_critMinSpawnHeight = -20f;
        /// <summary>
        /// CritMaxSpawnHeight
        /// </summary>
        public float CritMaxSpawnHeight
        {
            get
            {
                return m_critMaxSpawnHeight;
            }
            set
            {
                m_critMaxSpawnHeight = value;
            }
        }
        [SerializeField] protected float m_critMaxSpawnHeight = 5000f;
        /// <summary>
        /// CritHeightVariance
        /// </summary>
        public float CritHeightVariance
        {
            get
            {
                return m_critHeightVariance;
            }
            set
            {
                m_critHeightVariance = value;
            }
        }
        [SerializeField] protected float m_critHeightVariance = 30f;

        /// <summary>
        /// CritCheckSlopeType
        /// </summary>
        public Constants.CriteriaRangeType CritCheckSlopeType
        {
            get
            {
                return m_critCheckSlopeType;
            }
            set
            {
                m_critCheckSlopeType = value;
            }
        }
        [SerializeField] protected Constants.CriteriaRangeType m_critCheckSlopeType = Constants.CriteriaRangeType.Range;
        /// <summary>
        /// CritMinSpawnSlope
        /// </summary>
        public float CritMinSpawnSlope
        {
            get
            {
                return m_critMinSpawnSlope;
            }
            set
            {
                m_critMinSpawnSlope = Mathf.Clamp(value, 0f, 90f);
            }
        }
        [SerializeField] protected float m_critMinSpawnSlope = 0f;
        /// <summary>
        /// CritMaxSpawnSlope
        /// </summary>
        public float CritMaxSpawnSlope
        {
            get
            {
                return m_critMaxSpawnSlope;
            }
            set
            {
                m_critMaxSpawnSlope = Mathf.Clamp(value, 0f, 90f);
            }
        }
        [SerializeField] protected float m_critMaxSpawnSlope = 90f;
        /// <summary>
        /// CritSlopeVariance
        /// </summary>
        public float CritSlopeVariance
        {
            get
            {
                return m_critSlopeVariance;
            }
            set
            {
                m_critSlopeVariance = value;
            }
        }
        [SerializeField] protected float m_critSlopeVariance = 30f;

        /// <summary>
        /// CritCheckTextures
        /// </summary>
        public bool CritCheckTextures
        {
            get
            {
                return m_critCheckTextures;
            }
            set
            {
                m_critCheckTextures = value;
            }
        }
        [SerializeField] protected bool m_critCheckTextures = false;
        /// <summary>
        /// CritTextureStrength
        /// </summary>
        public float CritTextureStrength
        {
            get
            {
                return m_critTextureStrength;
            }
            set
            {
                m_critTextureStrength = value;
            }
        }
        [SerializeField] protected float m_critTextureStrength = 0f;
        /// <summary>
        /// CritTextureVariance
        /// </summary>
        public float CritTextureVariance
        {
            get
            {
                return m_critTextureVariance;
            }
            set
            {
                m_critTextureVariance = value;
            }
        }
        [SerializeField] protected float m_critTextureVariance = 0.1f;

        /// <summary>
        /// CritCheckMask
        /// </summary>
        public bool CritCheckMask
        {
            get
            {
                return m_critCheckMask;
            }
            set
            {
                m_critCheckMask = value;
            }
        }
        [SerializeField] protected bool m_critCheckMask = false;
        /// <summary>
        /// CritMaskType
        /// </summary>
        public Constants.MaskType CritMaskType
        {
            get
            {
                return m_critMaskType;
            }
            set
            {
                m_critMaskType = value;
            }
        }
        [SerializeField] protected Constants.MaskType m_critMaskType = Constants.MaskType.Perlin;
        /// <summary>
        /// CritMaskFractalMidpoint
        /// </summary>
        public float CritMaskFractalMidpoint
        {
            get
            {
                return m_critMaskFractalMidpoint;
            }
            set
            {
                m_critMaskFractalMidpoint = value;
            }
        }
        [SerializeField] protected float m_critMaskFractalMidpoint = 0.5f;
        /// <summary>
        /// CritMaskFractalRange
        /// </summary>
        public float CritMaskFractalRange
        {
            get
            {
                return m_critMaskFractalRange;
            }
            set
            {
                m_critMaskFractalRange = value;
            }
        }
        [SerializeField] protected float m_critMaskFractalRange = 0.5f;
        /// <summary>
        /// CritMaskInvert
        /// </summary>
        public bool CritMaskInvert
        {
            get
            {
                return m_critMaskInvert;
            }
            set
            {
                m_critMaskInvert = value;
            }
        }
        [SerializeField] protected bool m_critMaskInvert = false;

        #endregion

        #region Prototypes

        /// <summary>
        /// Special property with the sole purpose of providing a header name
        /// </summary>
        protected bool CTGRHDR_PrototypesPanel { get; }

        /// <summary>
        /// SortPrototypes
        /// </summary>
        public bool SortPrototypes
        {
            get
            {
                return m_sortPrototypes;
            }
            set
            {
                m_sortPrototypes = value;
            }
        }
        [SerializeField] protected bool m_sortPrototypes = false;

        #endregion

        #region Advanced

        /// <summary>
        /// Special property with the sole purpose of providing a header name
        /// </summary>
        protected bool CTGRHDR_Advanced { get; }

        /// <summary>
        /// SpawnToAVS
        /// </summary>
        public bool SpawnToAVS
        {
            get
            {
                return m_spawnToAVS;
            }
            set
            {
                m_spawnToAVS = value;
            }
        }
        [SerializeField] protected bool m_spawnToAVS = false;

        /// <summary>
        /// ChildSpawmMode
        /// </summary>
        public Constants.ChildSpawnMode ChildSpawnMode
        {
            get
            {
                return m_childSpawnMode;
            }
            set
            {
                m_childSpawnMode = value;
            }
        }
        [SerializeField] protected Constants.ChildSpawnMode m_childSpawnMode = Constants.ChildSpawnMode.All;

        /// <summary>
        /// SpawnToTarget
        /// </summary>
        public bool SpawnToTarget
        {
            get
            {
                return m_spawnToTarget;
            }
            set
            {
                m_spawnToTarget = value;
            }
        }
        [SerializeField] protected bool m_spawnToTarget = true;

        /// <summary>
        /// AdvForcePlaceAtClickLocation
        /// </summary>
        public bool AdvForcePlaceAtClickLocation
        {
            get
            {
                return m_advForcePlaceAtClickLocation;
            }
            set
            {
                m_advForcePlaceAtClickLocation = value;
            }
        }
        [SerializeField] protected bool m_advForcePlaceAtClickLocation = false;

        /// <summary>
        /// AdvUseLargeRanges
        /// </summary>
        public bool AdvUseLargeRanges
        {
            get
            {
                return m_advUseLargeRanges;
            }
            set
            {
                m_advUseLargeRanges = value;
            }
        }
        [SerializeField] protected bool m_advUseLargeRanges = false;

        /// <summary>
        /// ScaleToNearestInt
        /// </summary>
        public bool ScaleToNearestInt
        {
            get
            {
                return m_scaleToNearestInt;
            }
            set
            {
                m_scaleToNearestInt = value;
            }
        }
        [SerializeField] protected bool m_scaleToNearestInt = false;

        /// <summary>
        /// GlobalSpawnJitterPct
        /// </summary>
        public float GlobalSpawnJitterPct
        {
            get
            {
                return m_globalSpawnJitterPct;
            }
            set
            {
                m_globalSpawnJitterPct = value;
            }
        }
        [SerializeField] protected float m_globalSpawnJitterPct = 0.75f;

        /// <summary>
        /// AdvSpawnCheckOffset
        /// </summary>
        public float AdvSpawnCheckOffset
        {
            get
            {
                return m_advSpawnCheckOffset;
            }
            set
            {
                m_advSpawnCheckOffset = value;
            }
        }
        [SerializeField] protected float m_advSpawnCheckOffset = 2000f;

        /// <summary>
        /// AutoProbe
        /// </summary>
        public bool AutoProbe
        {
            get
            {
                return m_autoProbe;
            }
            set
            {
                m_autoProbe = value;
            }
        }
        [SerializeField] protected bool m_autoProbe = true;
        /// <summary>
        /// MinProbeGroupDistance
        /// </summary>
        public float MinProbeGroupDistance
        {
            get
            {
                return m_minProbeGroupDistance;
            }
            set
            {
                m_minProbeGroupDistance = value;
            }
        }
        [SerializeField] protected float m_minProbeGroupDistance = Constants.MinimimProbeGroupDistance;
        /// <summary>
        /// MinProbeDistance
        /// </summary>
        public float MinProbeDistance
        {
            get
            {
                return m_minProbeDistance;
            }
            set
            {
                m_minProbeDistance = value;
            }
        }
        [SerializeField] protected float m_minProbeDistance = Constants.MinimimProbeDistance;

        /// <summary>
        /// AutoOptimise
        /// </summary>
        public bool AutoOptimise
        {
            get
            {
                return m_autoOptimise;
            }
            set
            {
                m_autoOptimise = value;
            }
        }
        [SerializeField] protected bool m_autoOptimise = false;
        /// <summary>
        /// MaxSizeToOptimise
        /// </summary>
        public float MaxSizeToOptimise
        {
            get
            {
                return m_maxSizeToOptimise;
            }
            set
            {
                m_maxSizeToOptimise = value;
            }
        }
        [SerializeField] protected float m_maxSizeToOptimise = Constants.MaximumOptimisationSize;

        #endregion

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Awake
        /// </summary>
        public void Awake()
        {
            // Get the set reloaded if this is a newly created Preset (name == "")
            // but also in every awake, so although we may get occasional reloads that are 
            // not needed, however also when a preset is duplicated for example
            ms_dirty = true;
        }

        /// <summary>
        /// Apply to the <seealso cref="Spawner"/> provided.
        /// </summary>
        public void Apply(Spawner target)
        {
            // Overview
            if (IsActiveProperty("ParentName")) { target.m_parentName = ParentName; }
            if (IsActiveProperty("Type")) { target.m_type = Type; }
            if (IsActiveProperty("SpawnMode")) { target.m_spawnMode = SpawnMode; }
            if (IsActiveProperty("FlowDistance")) { target.m_flowDistanceSqr = FlowDistance * FlowDistance; }
            if (IsActiveProperty("SpawnRangeShape")) { target.m_spawnRangeShape = SpawnRangeShape; }
            if (IsActiveProperty("RandomSeed")) { target.m_randomSeed = RandomSeed; }
            if (IsActiveProperty("MaxSpawnRange")) { target.m_maxSpawnRange = MaxSpawnRange; }
            if (IsActiveProperty("SeedThrowRange")) { target.m_seedThrowRange = SeedThrowRange; }
            if (IsActiveProperty("MinInstances")) { target.m_minInstances = MinInstances; }
            if (IsActiveProperty("MaxInstances")) { target.m_maxInstances = MaxInstances; }
            if (IsActiveProperty("MergeSpawns")) { target.m_mergeSpawns = MergeSpawns; }
#if SECTR_CORE_PRESENT
            if (IsActiveProperty("DoSectorise")) { target.m_doSectorise = DoSectorise; }
            if (IsActiveProperty("SectorReparentingMode")) { target.m_sectorReparentingMode = (SECTR_Constants.ReparentingMode)SectorReparentingMode; } 
#endif

            // Placement Criteria
            if (IsActiveProperty("SpawnAlgorithm")) { target.m_spawnAlgorithm = SpawnAlgorithm; }
            if (IsActiveProperty("SeedThrowJitter")) { target.m_seedThrowJitter = SeedThrowJitter; }
            if (IsActiveProperty("RotationAlgorithm")) { target.m_rotationAlgorithm = RotationAlgorithm; }
            if (IsActiveProperty("EnableRotationDragUpdate")) { target.m_enableRotationDragUpdate = EnableRotationDragUpdate; }
            if (IsActiveProperty("MinRotationY")) { target.m_minRotationY = MinRotationY; }
            if (IsActiveProperty("MaxRotationY")) { target.m_maxRotationY = MaxRotationY; }
            if (IsActiveProperty("SameScale")) { target.m_sameScale = SameScale; }
            if (IsActiveProperty("MinScaleX")) { target.m_minScaleX = MinScaleX; }
            if (IsActiveProperty("MaxScaleX")) { target.m_maxScaleX = MaxScaleX; }
            if (IsActiveProperty("MinScaleY")) { target.m_minScaleY = MinScaleY; }
            if (IsActiveProperty("MaxScaleY")) { target.m_maxScaleY = MaxScaleY; }
            if (IsActiveProperty("MinScaleZ")) { target.m_minScaleZ = MinScaleZ; }
            if (IsActiveProperty("MaxScaleZ")) { target.m_maxScaleZ = MaxScaleZ; }
            if (IsActiveProperty("UseGravity")) { target.m_useGravity = UseGravity; }

            // Spawn Criteria
            if (IsActiveProperty("CritVirginCheckType")) { target.m_critVirginCheckType = CritVirginCheckType; }
            if (IsActiveProperty("CritBoundsBorder")) { target.m_critBoundsBorder = CritBoundsBorder; }
            if (IsActiveProperty("CritCheckHeightType")) { target.m_critCheckHeightType = CritCheckHeightType; }
            if (IsActiveProperty("CritMinSpawnHeight")) { target.m_critMinSpawnHeight = CritMinSpawnHeight; }
            if (IsActiveProperty("CritMaxSpawnHeight")) { target.m_critMaxSpawnHeight = CritMaxSpawnHeight; }
            if (IsActiveProperty("CritHeightVariance")) { target.m_critHeightVariance = CritHeightVariance; }
            if (IsActiveProperty("CritCheckSlopeType")) { target.m_critCheckSlopeType = CritCheckSlopeType; }
            if (IsActiveProperty("CritMinSpawnSlope")) { target.MinSpawnSlope = CritMinSpawnSlope; }
            if (IsActiveProperty("CritMaxSpawnSlope")) { target.MaxSpawnSlope = CritMaxSpawnSlope; }
            if (IsActiveProperty("CritSlopeVariance")) { target.m_critSlopeVariance = CritSlopeVariance; }
            if (IsActiveProperty("CritCheckTextures")) { target.m_critCheckTextures = CritCheckTextures; }
            if (IsActiveProperty("CritTextureStrength")) { target.m_critTextureStrength = CritTextureStrength; }
            if (IsActiveProperty("CritTextureVariance")) { target.m_critTextureVariance = CritTextureVariance; }
            if (IsActiveProperty("CritCheckMask")) { target.m_critCheckMask = CritCheckMask; }
            if (IsActiveProperty("CritMaskType")) { target.m_critMaskType = CritMaskType; }
            if (IsActiveProperty("CritMaskFractalMidpoint")) { target.m_critMaskFractalMidpoint = CritMaskFractalMidpoint; }
            if (IsActiveProperty("CritMaskFractalRange")) { target.m_critMaskFractalRange = CritMaskFractalRange; }
            if (IsActiveProperty("CritMaskInvert")) { target.m_critMaskInvert = CritMaskInvert; }

            // Prototypes Panel
            if (IsActiveProperty("SortPrototypes")) { target.m_sortPrototypes = SortPrototypes; }

            // Advanced
            if (IsActiveProperty("SpawnToAVS")) { target.m_spawnToAVS = SpawnToAVS; }
            if (IsActiveProperty("ChildSpawmMode")) { target.ChildSpawnMode = ChildSpawnMode; }
            if (IsActiveProperty("SpawnToTarget")) { target.m_spawnToTarget = SpawnToTarget; }
            if (IsActiveProperty("AdvForcePlaceAtClickLocation")) { target.m_advForcePlaceAtClickLocation = AdvForcePlaceAtClickLocation; }
            if (IsActiveProperty("AdvUseLargeRanges")) { target.m_advUseLargeRanges = AdvUseLargeRanges; }
            if (IsActiveProperty("ScaleToNearestInt")) { target.m_scaleToNearestInt = ScaleToNearestInt; }
            if (IsActiveProperty("GlobalSpawnJitterPct")) { target.m_globalSpawnJitterPct = GlobalSpawnJitterPct; }
            if (IsActiveProperty("AdvSpawnCheckOffset")) { target.m_advSpawnCheckOffset = AdvSpawnCheckOffset; }
            if (IsActiveProperty("AutoProbe")) { target.m_autoProbe = AutoProbe; }
            if (IsActiveProperty("MinProbeGroupDistance")) { target.m_minProbeGroupDistance = MinProbeGroupDistance; }
            if (IsActiveProperty("MinProbeDistance")) { target.m_minProbeDistance = MinProbeDistance; }
            if (IsActiveProperty("AutoOptimise")) { target.m_autoOptimise = AutoOptimise; }
            if (IsActiveProperty("MaxSizeToOptimise")) { target.m_maxSizeToOptimise = MaxSizeToOptimise; }
        }

        /// <summary>
        /// Upate values from a <seealso cref="Spawner"/> provided.
        /// </summary>
        public void FromSpawner(Spawner source)
        {
            // Overview
            ParentName = source.m_parentName;
            Type = source.m_type;
            SpawnMode = source.m_spawnMode;
            FlowDistance = Mathf.Sqrt(source.m_flowDistanceSqr);
            SpawnRangeShape = source.m_spawnRangeShape;
            RandomSeed = source.m_randomSeed;
            MaxSpawnRange = source.m_maxSpawnRange;
            SeedThrowRange = source.m_seedThrowRange;
            MinInstances = source.m_minInstances;
            MaxInstances = source.m_maxInstances;
            MergeSpawns = source.m_mergeSpawns;
#if SECTR_CORE_PRESENT
            DoSectorise = source.m_doSectorise;
            SectorReparentingMode = (int)source.m_sectorReparentingMode;
#endif

            // Placement Criteria
            SpawnAlgorithm = source.m_spawnAlgorithm;
            SeedThrowJitter = source.m_seedThrowJitter;
            RotationAlgorithm = source.m_rotationAlgorithm;
            EnableRotationDragUpdate = source.m_enableRotationDragUpdate;
            MinRotationY = source.m_minRotationY;
            MaxRotationY = source.m_maxRotationY;
            SameScale = source.m_sameScale;
            MinScaleX = source.m_minScaleX;
            MaxScaleX = source.m_maxScaleX;
            MinScaleY = source.m_minScaleY;
            MaxScaleY = source.m_maxScaleY;
            MinScaleZ = source.m_minScaleZ;
            MaxScaleZ = source.m_maxScaleZ;
            UseGravity = source.m_useGravity;

            // Spawn Criteria
            CritVirginCheckType = source.m_critVirginCheckType;
            CritBoundsBorder = source.m_critBoundsBorder;
            CritCheckHeightType = source.m_critCheckHeightType;
            CritMinSpawnHeight = source.m_critMinSpawnHeight;
            CritMaxSpawnHeight = source.m_critMaxSpawnHeight;
            CritHeightVariance = source.m_critHeightVariance;
            CritCheckSlopeType = source.m_critCheckSlopeType;
            CritMinSpawnSlope = source.MinSpawnSlope;
            CritMaxSpawnSlope = source.MaxSpawnSlope;
            CritSlopeVariance = source.m_critSlopeVariance;
            CritCheckTextures = source.m_critCheckTextures;
            CritTextureStrength = source.m_critTextureStrength;
            CritTextureVariance = source.m_critTextureVariance;
            CritCheckMask = source.m_critCheckMask;
            CritMaskType = source.m_critMaskType;
            CritMaskFractalMidpoint = source.m_critMaskFractalMidpoint;
            CritMaskFractalRange = source.m_critMaskFractalRange;
            CritMaskInvert = source.m_critMaskInvert;

            // Prototypes Panel
            SortPrototypes = source.m_sortPrototypes;

            // Advanced
            SpawnToAVS = source.m_spawnToAVS;
            ChildSpawnMode = source.ChildSpawnMode;
            SpawnToTarget = source.m_spawnToTarget;
            AdvForcePlaceAtClickLocation = source.m_advForcePlaceAtClickLocation;
            AdvUseLargeRanges = source.m_advUseLargeRanges;
            ScaleToNearestInt = source.m_scaleToNearestInt;
            GlobalSpawnJitterPct = source.m_globalSpawnJitterPct;
            AdvSpawnCheckOffset = source.m_advSpawnCheckOffset;
            AutoProbe = source.m_autoProbe;
            MinProbeGroupDistance = source.m_minProbeGroupDistance;
            MinProbeDistance = source.m_minProbeDistance;
            AutoOptimise = source.m_autoOptimise;
            MaxSizeToOptimise = source.m_maxSizeToOptimise;
        }

        #region Custom Serialisation

        /// <summary>
        /// OnBeforeSerialize
        /// </summary>
        public void OnBeforeSerialize()
        {
            if (m_activeProperties != null)
            {
                _activeProperties = new List<string>(m_activeProperties).ToArray();
            }
        }

        /// <summary>
        /// OnAfterDeserialize
        /// </summary>
        public void OnAfterDeserialize()
        {
            if (_activeProperties != null)
            {
                m_activeProperties = new HashSet<string>(_activeProperties);
            }
            else
            {
                m_activeProperties = new HashSet<string>();
            }
        }

        #endregion
    }
}
