using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using GeNa.Internal;
using PWCommon2;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if VEGETATION_STUDIO || VEGETATION_STUDIO_PRO
using AwesomeTechnologies;
using AwesomeTechnologies.VegetationStudio;
using AwesomeTechnologies.VegetationSystem;
#endif

namespace GeNa
{
    /// <summary>
    /// Core GeNa spawner class
    /// </summary>
    public class Spawner : MonoBehaviour, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Used for queueing paint task, so for example with biome spawning the different prototypes don't affect each other with collisions, etc.
        /// </summary>
        private struct PaintTask
        {
            public Prototype Prototype { get; private set; }
            public Vector3 HitLocation { get; private set; }
            public Vector3 HitNormal { get; private set; }
            public float HitAlpha { get; private set; }

            public PaintTask(Prototype prototype, Vector3 hitLocation, Vector3 hitNormal, float hitAlpha)
            {
                Prototype = prototype;
                HitLocation = hitLocation;
                HitNormal = hitNormal;
                HitAlpha = hitAlpha;
            }
        }

        //u260D opposition (link lookin thing)
        //u22B6 u22B7 connection 1 black 1 white
        public const string LINKED_SYMBOL = "\u260D";
        protected const string CACHE_SYMBOL = " =CACHE=";

        #region Spawner variables - public so that they can be accessed via editor - see documentation

        //Versioning
        [SerializeField] private int m_majorVersion = 0;
        [SerializeField] private int m_minorVersion = 0;
        [SerializeField] private int m_patchVersion = 0;

        /// <summary>
        /// True if this is only a cache
        /// </summary>
        public bool IsCache
        {
            get
            {
                return m_isCache;
            }
        }
        [SerializeField] private bool m_isCache = false;

        /// <summary>
        /// The cache of this Spawner
        /// </summary>
        public Spawner Cache { get; private set; }

        /// <summary>
        /// Is this spawner linked to a Resource which will get it to Spawn?
        /// </summary>
        public bool IsLinked
        {
            get
            {
                return m_isLinked;
            }
        }
        [SerializeField] private bool m_isLinked = false;

        public Constants.SpawnerType m_type = Constants.SpawnerType.Regular;

        /// <summary>
        /// Determines the way Child Spawners are spawned. - E.g.: All Child Spawners or just a random one.
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
        [SerializeField] private Constants.ChildSpawnMode m_childSpawnMode = Constants.ChildSpawnMode.All;

        public Constants.SpawnMode m_spawnMode = Constants.SpawnMode.Single;
		public float m_flowDistanceSqr = 30f * 30f;
		public string m_parentName = "GeNa Spawner";
        public bool m_mergeSpawns = true;
        public bool m_spawnToTarget = true;
#if SECTR_CORE_PRESENT
		public bool m_doSectorise = true;
		public SECTR_Constants.ReparentingMode m_sectorReparentingMode = SECTR_Constants.ReparentingMode.Bounds;
#endif

        //Prototypes
        public bool m_sortPrototypes = false;
		public List<Prototype> m_spawnPrototypes = new List<Prototype>();

		// These are set during initialisation to account for Extensions
		[NonSerialized] public bool m_affectsHeight = false;
		[NonSerialized] public bool m_affectsTrees = false;
		[NonSerialized] public bool m_affectsGrass = false;
		[NonSerialized] public bool m_affectsTexture = false;

		// Spawner has any active terrain protos
		public bool m_hasActiveHeightProtos = false;
		public bool m_hasActiveTreeProtos = false;
        public bool m_hasActiveGrassProtos = false;
        public bool m_hasActiveTextureProtos = false;

		// Children of spawner have any active terrain protos
		public bool m_hasChildWithActiveHeightProtos = false;
        public bool m_hasChildWithActiveTreeProtos = false;
        public bool m_hasChildWithActiveGrassProtos = false;
        public bool m_hasChildWithActiveTextureProtos = false;

        //Spawn origin
        /// <summary>
        /// Used by <seealso cref="m_spawnToTarget"/>
        /// </summary>
        private GameObject m_spawnTarget;
        public Vector3 m_spawnOriginLocation = Vector3.zero;
        public Vector3 m_spawnOriginNormal = Vector3.up;
        public int     m_spawnOriginObjectID = int.MinValue;
        public bool    m_spawnOriginIsTerrain = false;
        public Bounds  m_spawnOriginBounds = new Bounds();
        public Transform m_spawnOriginGroundTransform = null;

        //Spawn algorithm 
        public Constants.LocationAlgorithm m_spawnAlgorithm = Constants.LocationAlgorithm.Organic;

        //Spawn controls
        public float m_globalSpawnJitterPct = 0.75f;
        public long m_minInstances = 1;
        public long m_maxInstances = 1;
        public long m_instancesSpawned = 0;
        public float m_seedThrowRange = 5f;
        public float m_seedThrowJitter = 1f;
        public float m_maxSpawnRange = 50f;
        public Constants.SpawnRangeShape m_spawnRangeShape = Constants.SpawnRangeShape.Circle;

        //Spawn criteria - based on similarity / variance from where you clicked
        public Constants.VirginCheckType m_critVirginCheckType = Constants.VirginCheckType.Point;
		public LayerMask m_critSpawnCollisionLayers;
		public float m_critBoundsBorder = 0f;
        //This replaces the m_critCheckHeight switch.
        public Constants.CriteriaRangeType m_critCheckHeightType = Constants.CriteriaRangeType.Mixed;
        //Now legacy - replaced by CheckHeightType.
        public bool m_critCheckHeight = true;
        public float m_critMinSpawnHeight = -20f;
        public float m_critMaxSpawnHeight = 5000f;
		public float m_critHeightVariance = 30f;
		[SerializeField] private float m_terrainYPos = float.NaN;
        [SerializeField] private float m_terrainHeight = float.NaN;
        public float m_bottomBoundary = -20f;
        public float m_topBoundary = 5000f;
        //This replaces the m_critCheckSlope switch.
        public Constants.CriteriaRangeType m_critCheckSlopeType = Constants.CriteriaRangeType.Range;
        //Now legacy - replaced by CheckSlopeType.
        public bool m_critCheckSlope = true;
        //Private fields accessible by Properties - Find the properties in the Properties section
        [SerializeField] private float m_critMinSpawnSlope = 0f;
        [SerializeField] private float m_critMaxSpawnSlope = 90f;
        public float m_critSlopeVariance = 30f;
        public bool m_critCheckTextures = false;
        public float m_critTextureStrength = 0f;
        public float m_critTextureVariance = 0.1f;
        public int m_critSelectedTextureIdx = 0;
        public string m_critSelectedTextureName = "";
        public int m_critMaxSelectedTexture = 1;
        public bool m_critCheckMask = false;
        public Constants.MaskType m_critMaskType = Constants.MaskType.Perlin;
        public Fractal m_critMaskFractal = new Fractal();
        public float m_critMaskFractalMidpoint = 0.5f;
        public float m_critMaskFractalRange = 0.5f;
        public Texture2D m_critMaskImage;
        public Heightmap m_critMaskImageData;
        public Heightmap m_critMaskAlphaData;
        public bool m_critMaskInvert = false;

        //Variables for ranges
        public Vector3 m_rangesOriginLocation = Vector3.zero;
        public Vector3 m_rangesOriginNormal = Vector3.up;
        //These 4 are public so their values can be displayed in GUI
        public float m_critMinHeight = 0f;
        public float m_critMaxHeight = 10000f;
        public float m_critMinSlope = 0f;
        public float m_critMaxSlope = 90f;
        private float m_critMinTextureStrength = 0f;
        private float m_critMaxTextureStrength = 1f;
        private float m_critMaskFractalMin = 0f;
        private float m_critMaskFractalMax = 1f;

        //Placement criteria
        public GeNa.Constants.RotationAlgorithm m_rotationAlgorithm = Constants.RotationAlgorithm.Ranged;
        public float m_minRotationY = 0f;
        public float m_maxRotationY = 360f;
        public bool m_sameScale = true;
        public bool m_scaleToNearestInt = false;
        public float m_minScaleX = 0.7f;
        public float m_maxScaleX = 1.3f;
        public float m_minScaleY = 0.7f;
        public float m_maxScaleY = 1.3f;
        public float m_minScaleZ = 0.7f;
        public float m_maxScaleZ = 1.3f;
        public bool  m_useGravity = false;
        public bool m_enableRotationDragUpdate = false;
        public bool m_autoOptimise = false;
        public float m_maxSizeToOptimise = Constants.MaximumOptimisationSize;
        public float m_minProbeGroupDistance = Constants.MinimimProbeGroupDistance;
        public float m_minProbeDistance = Constants.MinimimProbeDistance;
        public bool m_autoProbe = true;

        //Gravity so
        public Gravity m_gravity;

        //Advanced settings
        public bool m_advUseLargeRanges = false;
        public bool m_advShowMouseOverHelp = false;
        public bool m_advShowDetailedHelp = true;
        public bool m_advForcePlaceAtClickLocation = false;
        public bool m_advAddColliderToSpawnedPrefabs = false;
        public float m_advSpawnCheckOffset = 2000f;

        //Editor section view flags
        public bool m_showOverview = true;
        public bool m_showSpawnCriteria = false;
        public bool m_showPlacementCriteria = false;
        public bool m_showPrototypes = false;
        public bool m_showGizmos = false;
        public bool m_showAdvancedSettings = false;
		public bool m_showCritMinSpawnHeight = false;
		public bool m_showCritMaxSpawnHeight = false;

        //Gizmo / visialisation related
        public bool VisualizationActive { get; set; }
        public Vector3 m_visualizationLocation = Vector3.zero;
        public bool m_needsVisualisationUpdate = true;
        public int m_maxVisualisationDimensions = 50;
        public float m_visProcessLimit = 0.5f;
        private float[,] m_fitnessArray = new float[1, 1];

        //Control the resolution of the bounds checker
        public float m_metersPerScan = 1f;
        public float m_metersPerScanVisualisation = 4f;

        //Tree manager - for tree related collisions
        private TreeManager m_treeManager = new TreeManager();

        //Probe manager - for light probe related queries
        private ProbeManager m_probeManager = new ProbeManager();
        private GameObject m_probeParent = null;

        //Last spawn object - for prefab spawner rotation
        public GameObject m_lastSpawnedObject = null;

        //List of lists of spawned game objects for undo all support
        public List<GameObject> m_prefabUndoList = new List<GameObject>();

		// To clean up after ourselves when we do UnspawnAll().
		// Scenario: We have a good number of sectors and we don't merge spawns. Spawning and 
		// unspawning can quickly get messy in a way that's difficult to clean up for the user.
		public List<GameObject> m_parentsUndoList = new List<GameObject>();
		public List<GameObject> m_probeUndoList = new List<GameObject>();

        /// <summary>
        /// Child spawners of this spawner. It's worth ensuring that this list is up to date with <see cref="UpdateChildSpawners"/>().
        /// </summary>
		public List<Spawner> ChildSpawners { get { return m_childSpawners; } }
        //Child spawners - used when childing children for chained spawns
        private List<Spawner> m_childSpawners = new List<Spawner>();
		public List<Spawner> ActiveChildSpawners { get { return m_activeChildSpawners; } }
        private List<Spawner> m_activeChildSpawners = new List<Spawner>();

		/// <summary>
		/// Child Spawners to be spawned next.
		/// </summary>
		public List<Spawner> ChildrenToSpawnNext
        {
            get
            {
                switch (m_childSpawnMode)
                {
                    case Constants.ChildSpawnMode.All:
                        return m_activeChildSpawners;
                    case Constants.ChildSpawnMode.Random:
                        if (m_nextChildIndex < 0)
                        {
                            m_nextChildIndex = m_randomGenerator.Next(0, m_activeChildSpawners.Count - 1);
                        }
                        return new List<Spawner> { m_activeChildSpawners[m_nextChildIndex] };
                    default:
                        Debug.LogErrorFormat("[GeNa] Spawner *{0}* is set to an unknown Child Spawn Mode: <b>{1}</b>. Going to spawn <b>All</b> instead.", name, m_childSpawnMode);
                        return m_activeChildSpawners;
                }
            }
        }
        private int m_nextChildIndex = -1;

        /// <summary>
        /// Indexes of Child Spawners that are linked to a Resource (Spawn through the resource)
        /// </summary>
        private IDictionary<int, int> m_linkedSpawners = new Dictionary<int, int>();
        [SerializeField] List<int> _linkedSpawnersIx;
        [SerializeField] List<int> _linkedSpawnersCount;

        //When last updated - used for gravity calculations
        private DateTime m_lastUpdated = DateTime.MinValue;

        //Defaults for key presses etc
        public GenaDefaults m_defaults;
        public bool m_defaultsSet = false;

        //Random number generation
        public int m_randomSeed = 1000;
        public GeNa.XorshiftPlus m_randomGenerator = new XorshiftPlus(1000);

        //Use AVS
        public bool m_spawnToAVS = false;

        // Paint Texture wip
        private SplatPainter m_splatPainter;

        // Custom parent of spawns used by API calls for example by Origami
        private GameObject m_spawnParent;

		// Undo system

		/// <summary>
		/// The Undo Stack
		/// </summary>
		internal UndoDropStack UndoStack
		{
			get
			{
				if (m_undoComponent == null)
				{
                    m_undoComponent = gameObject.GetComponent<UndoComponent>();

                    if (m_undoComponent == null)
                    {
                        m_undoComponent = gameObject.AddComponent<UndoComponent>();
                        m_undoComponent.hideFlags = HideFlags.HideInInspector;
                    }

                    if (m_undoComponent == null)
                    {
                        Debug.LogErrorFormat("[GeNa] Unable to get the undo component for spawner '{0}'.", name);
                        return null;
                    }

                    // It was reported that a Unity crash caused spawners to possibly start constantly serialising this                        
                    if (m_undoComponent.hideFlags != HideFlags.HideInInspector)
                    {
                        m_undoComponent.hideFlags = HideFlags.HideInInspector;
                    }
                }

				return m_undoComponent.UndoStack;
			}
		}
		private UndoComponent m_undoComponent;

		/// <summary>
		/// The current number of recorded Undo steps.
		/// </summary>
		public int UndoCount { get { return UndoStack.Count; } }

		/// <summary>
		/// Returns an Array copy of the Undo Stack.
		/// </summary>
		public UndoRecord[] UndoArrayCopy
		{
			get
			{
				return UndoStack.ToArray();
			}
		}

		/// <summary>
		/// The maximum possible undo steps for this Spawner.
		/// </summary>
		public int UndoSteps { get { return UndoStack.Capacity; } }

		//public Stack<UndoRecord> RedoStack;
		// Used to build the Record to store for Undo
		[NonSerialized] private UndoRecord m_undoRecord;

		/// <summary>
		/// Used to track extension activity during spawning.
		/// </summary>
		internal List<ExtensionInstance> m_extensionUndoList;

        // Ingestion variables
        public float m_ingestionGroundLevel = float.NaN;

        // Spawn Optimizations
        private bool m_gobalSpawning = false;
#if UNITY_2018_1_OR_NEWER
        private bool m_turnedOnAutoSyncTransforms = false;
#endif

        /// <summary>
        /// Delegate that can be used to update caches of this Spawner
        /// </summary>
        public Action OnUpdate;

        /// <summary>
        /// Minimum Slope Limit. Value has to be between 0f and 90f.
        /// </summary>
        public float MinSpawnSlope
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

        /// <summary>
        /// MAximum Slope Limit. Value has to be between 0f and 90f.
        /// </summary>
        public float MaxSpawnSlope
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

        /// <summary>
        /// An ID given to Prototypes when created. Used to identify the prototype during serialization and possibly deep copy.
        /// </summary>
        public int NextProtoId { get { return m_nextProtoId++; } }
        [SerializeField] private int m_nextProtoId = 0;

        #endregion

        #region Constructors and deep copy

        public void CacheCopy(Spawner src)
        {
            src.Cache = this;
            m_isCache = true;
            DeepCopy(src);
        }

        /// <summary>
        /// Only GUI alterable stuff is guaranteed to be deep copied.
        /// Things like Gravity, TreeManager, ProbeManager, Transform and GameObject instances are not copied for obvious reasons.
        /// </summary>
        public void DeepCopy(Spawner src)
        {
            m_isLinked = src.m_isLinked;

            // Remove any symbols from the name...
            name = src.name.Replace(LINKED_SYMBOL, "");
            name = name.Replace(CACHE_SYMBOL, "");

            // ...and add them only if appropriate.
            if (m_isLinked)
            {
                name = LINKED_SYMBOL + name;
            }
            if (m_isCache)
            {
                name += CACHE_SYMBOL;
            }

            m_parentName = src.m_parentName;
            m_type = src.m_type;
            m_childSpawnMode = src.m_childSpawnMode;

            m_spawnMode = src.m_spawnMode;
            m_flowDistanceSqr = src.m_flowDistanceSqr;
            m_mergeSpawns = src.m_mergeSpawns;
            m_spawnToTarget = src.m_spawnToTarget;
#if SECTR_CORE_PRESENT
		    m_doSectorise = src.m_doSectorise;
		    m_sectorReparentingMode = src.m_sectorReparentingMode;
#endif
            //Prototypes
            m_sortPrototypes = src.m_sortPrototypes;
            m_spawnPrototypes = new List<Prototype> ();
            if (src.m_spawnPrototypes != null)
            {
                for (int i = 0; i < src.m_spawnPrototypes.Count; i++)
                {
                    m_spawnPrototypes.Add(new Prototype(this, src.m_spawnPrototypes[i]));
                }
            }

			m_hasActiveHeightProtos = src.m_hasActiveHeightProtos;
            m_hasActiveTreeProtos = src.m_hasActiveTreeProtos;
            m_hasActiveGrassProtos = src.m_hasActiveGrassProtos;
            m_hasActiveTextureProtos = src.m_hasActiveTextureProtos;

            //Spawn origin
            m_spawnTarget = src.m_spawnTarget;
            m_spawnOriginLocation = src.m_spawnOriginLocation;
            m_spawnOriginNormal = src.m_spawnOriginNormal;
            m_spawnOriginObjectID = src.m_spawnOriginObjectID;
            m_spawnOriginIsTerrain = src.m_spawnOriginIsTerrain;
            m_spawnOriginBounds = src.m_spawnOriginBounds;
            m_spawnOriginGroundTransform = src.m_spawnOriginGroundTransform;

           //Spawn algorithm 
            m_spawnAlgorithm = src.m_spawnAlgorithm;

            //Spawn controls
            m_minInstances = src.m_minInstances;
            m_maxInstances = src.m_maxInstances;
            m_instancesSpawned = src.m_instancesSpawned;
            m_seedThrowRange = src.m_seedThrowRange;
            m_seedThrowJitter = src.m_seedThrowJitter;
            m_maxSpawnRange = src.m_maxSpawnRange;
            m_spawnRangeShape = src.m_spawnRangeShape;

            //Spawn criteria - based on similarity / variance from where you clicked
            m_critVirginCheckType = src.m_critVirginCheckType;
            m_critSpawnCollisionLayers = src.m_critSpawnCollisionLayers;
            m_critBoundsBorder = src.m_critBoundsBorder;
            m_critCheckHeightType = src.m_critCheckHeightType;
            //Now legacy - replaced by CheckHeightType.
            m_critCheckHeight = src.m_critCheckHeight;
            m_critMinSpawnHeight = src.m_critMinSpawnHeight;
            m_critMaxSpawnHeight = src.m_critMaxSpawnHeight;
            m_critHeightVariance = src.m_critHeightVariance;
            
            m_terrainYPos = src.m_terrainYPos;
            m_terrainHeight = src.m_terrainHeight;
            m_bottomBoundary = src.m_bottomBoundary;
            m_topBoundary = src.m_topBoundary;

            m_critCheckSlopeType = src.m_critCheckSlopeType;
            //Now legacy - replaced by CheckSlopeType.
            m_critCheckSlope = src.m_critCheckSlope;
            MinSpawnSlope = src.MinSpawnSlope;
            MaxSpawnSlope = src.MaxSpawnSlope;
            m_critSlopeVariance = src.m_critSlopeVariance;
            m_critCheckTextures = src.m_critCheckTextures;
            m_critTextureStrength = src.m_critTextureStrength;
            m_critTextureVariance = src.m_critTextureVariance;
            m_critSelectedTextureIdx = src.m_critSelectedTextureIdx;
            m_critSelectedTextureName = src.m_critSelectedTextureName;
            m_critMaxSelectedTexture = src.m_critMaxSelectedTexture;
            m_critCheckMask = src.m_critCheckMask;
            m_critMaskType = src.m_critMaskType;
            if (src.m_critMaskFractal != null)
            {
                m_critMaskFractal = new Fractal(src.m_critMaskFractal);
            }
            m_critMaskFractalMidpoint = src.m_critMaskFractalMidpoint;
            m_critMaskFractalRange = src.m_critMaskFractalRange;
            m_critMaskImage = src.m_critMaskImage;
            if (src.m_critMaskImageData != null)
            {
                m_critMaskImageData = new Heightmap(src.m_critMaskImageData);
            }
            if (src.m_critMaskAlphaData != null)
            {
                m_critMaskAlphaData = new Heightmap(src.m_critMaskAlphaData);
            }
            m_critMaskInvert = src.m_critMaskInvert;

            //Variables for ranges
            m_rangesOriginLocation = src.m_rangesOriginLocation;
            m_rangesOriginNormal = src.m_rangesOriginNormal;
            //These 4 are public so their values can be displayed in GUI
            m_critMinHeight = src.m_critMinHeight;
            m_critMaxHeight = src.m_critMaxHeight;
            m_critMinSlope = src.m_critMinSlope;
            m_critMaxSlope = src.m_critMaxSlope;

            //Placement criteria
            m_rotationAlgorithm = src.m_rotationAlgorithm;
            m_minRotationY = src.m_minRotationY;
            m_maxRotationY = src.m_maxRotationY;
            m_sameScale = src.m_sameScale;
            m_scaleToNearestInt = src.m_scaleToNearestInt;
            m_minScaleX = src.m_minScaleX;
            m_maxScaleX = src.m_maxScaleX;
            m_minScaleY = src.m_minScaleY;
            m_maxScaleY = src.m_maxScaleY;
            m_minScaleZ = src.m_minScaleZ;
            m_maxScaleZ = src.m_maxScaleZ;
            m_useGravity = src.m_useGravity;
            m_enableRotationDragUpdate = src.m_enableRotationDragUpdate;
            m_autoOptimise = src.m_autoOptimise;
            m_maxSizeToOptimise = src.m_maxSizeToOptimise;
            m_minProbeGroupDistance = src.m_minProbeGroupDistance;
            m_minProbeDistance = src.m_minProbeDistance;
            m_autoProbe = src.m_autoProbe;

            //Gravity so
            m_gravity = src.m_gravity;

            //Advanced settings
            m_advUseLargeRanges = src.m_advUseLargeRanges;
            m_advShowMouseOverHelp = src.m_advShowMouseOverHelp;
            m_advShowDetailedHelp = src.m_advShowDetailedHelp;
            m_advForcePlaceAtClickLocation = src.m_advForcePlaceAtClickLocation;
            m_advAddColliderToSpawnedPrefabs = src.m_advAddColliderToSpawnedPrefabs;
            m_advSpawnCheckOffset = src.m_advSpawnCheckOffset;

            //Editor section view flags
            m_showOverview = src.m_showOverview;
            m_showSpawnCriteria = src.m_showSpawnCriteria;
            m_showPlacementCriteria = src.m_showPlacementCriteria;
            m_showPrototypes = src.m_showPrototypes;
            m_showGizmos = src.m_showGizmos;
            m_showAdvancedSettings = src.m_showAdvancedSettings;
            m_showCritMinSpawnHeight = src.m_showCritMinSpawnHeight;
            m_showCritMaxSpawnHeight = src.m_showCritMaxSpawnHeight;

            //Gizmo / visialisation related
            m_needsVisualisationUpdate = src.m_needsVisualisationUpdate;
            m_maxVisualisationDimensions = src.m_maxVisualisationDimensions;
            m_visProcessLimit = src.m_visProcessLimit;
            if (src.m_fitnessArray != null)
            {
                m_fitnessArray = new float[src.m_fitnessArray.GetLength(0), src.m_fitnessArray.GetLength(1)];
                for (int x = 0; x < src.m_fitnessArray.GetLength(0); x++)
                {
                    for (int y = 0; y < src.m_fitnessArray.GetLength(1); y++)
                    {
                        m_fitnessArray[x, y] = src.m_fitnessArray[x, y];
                    }
                }
            }

            //Control the resolution of the bounds checker
            m_metersPerScan = src.m_metersPerScan;
            m_metersPerScanVisualisation = src.m_metersPerScanVisualisation;

            //Tree manager - for tree related collisions
            m_treeManager = src.m_treeManager;

            //Probe manager - for light probe related queries
            m_probeManager = src.m_probeManager;
            m_probeParent = src.m_probeParent;

            //Last spawn object - for prefab spawner rotation
            m_lastSpawnedObject = src.m_lastSpawnedObject;

            //List of lists of spawned game objects for undo all support
            m_prefabUndoList = new List<GameObject>();
            if (src.m_prefabUndoList != null)
            {
                for (int i = 0; i < src.m_prefabUndoList.Count; i++)
                {
                    m_prefabUndoList.Add(src.m_prefabUndoList[i]);
                }
            }

            // To clean up after ourselves when we do UnspawnAll().
            // Scenario: We have a good number of sectors and we don't merge spawns. Spawning and 
            // unspawning can quickly get messy in a way that's difficult to clean up for the user.
            m_parentsUndoList = new List<GameObject>();
            if (src.m_parentsUndoList != null)
            {
                for (int i = 0; i < src.m_parentsUndoList.Count; i++)
                {
                    m_parentsUndoList.Add(src.m_parentsUndoList[i]);
                }
            }
            m_probeUndoList = new List<GameObject>();
            if (src.m_probeUndoList != null)
            {
                for (int i = 0; i < src.m_probeUndoList.Count; i++)
                {
                    m_probeUndoList.Add(src.m_probeUndoList[i]);
                }
            }

            //Child spawners - used when childing children for chained spawns
            m_childSpawners = (src.m_childSpawners != null) ? new List<Spawner>(src.m_childSpawners) : new List<Spawner>();
            m_activeChildSpawners = (src.m_activeChildSpawners != null) ? new List<Spawner>(src.m_activeChildSpawners) : new List<Spawner>();

            //Linked spawners
            m_linkedSpawners = (src.m_linkedSpawners != null) ? new Dictionary<int, int>(src.m_linkedSpawners) : new Dictionary<int, int>();

            //When last updated - used for gravity calculations
            m_lastUpdated = src.m_lastUpdated;

            //Defaults for key presses etc
            m_defaults = src.m_defaults;

            //Random number generation
            m_randomSeed = src.m_randomSeed;
            m_randomGenerator = new XorshiftPlus(src.m_randomSeed);

            //Use AVS
            m_spawnToAVS = src.m_spawnToAVS;

            //Visualization
            m_visualizationLocation = src.m_visualizationLocation;
            m_splatPainter = src.m_splatPainter;

            // Custom parent of spawns used by API calls for example by Origami
            m_spawnParent = src.m_spawnParent;

            // Not caching undos because we don't want Unity to undo their changes
        }

        #endregion

        #region Upgraders

        /// <summary>
        /// Updgrade Spawner if needed
        /// </summary>
        public void Upgrade(string majorVer, string minorVer, string patchVer)
        {
            int verCheckSum = (m_majorVersion * 1000000) + (m_minorVersion * 1000) + m_patchVersion;

            // Conver versions to the comperable format
            int v210 = (2 * 1000000) + (1 * 1000) + 0;

            // The latest that needs upgrade
            int latest = v210;

            // If this version is at least that, no need to upgrade
            if (verCheckSum >= latest)
            {
                return;
            }

            // v2.1.0
            if (verCheckSum < v210)
            {
                To210();
            }

            // Need to parse at the moment
            int majorVersion, minorVersion, patchVersion;
            if (!ParseVersion(majorVer, minorVer, patchVer, out majorVersion, out minorVersion, out patchVersion))
            {
                return;
            }

            m_majorVersion = majorVersion;
            m_minorVersion = minorVersion;
            m_patchVersion = patchVersion;
        }

        /// <summary>
        /// 2.1.0 upgrades
        /// </summary>
        private void To210()
        {
            for (int i = 0; i < m_spawnPrototypes.Count; i++)
            {
                m_spawnPrototypes[i].SetSpawner(this);
            }
        }

        /// <summary>
        /// Attempts to parse the version numbers into int and returns true if successful
        /// </summary>
        private bool ParseVersion(string mav, string miv, string pav, out int majorVer, out int minorVer, out int patchVer)
        {
            majorVer = -1;
            minorVer = -1;
            patchVer = -1;

            int.TryParse(mav, out majorVer);
            int.TryParse(miv, out minorVer);
            int.TryParse(pav, out patchVer);

            if (majorVer < 0 || minorVer < 0 || patchVer < 0)
            {
                Debug.LogErrorFormat("Ignored invalid current version received: {0}.{1}.{2}", mav, miv, pav);
                return false;
            }

            return true;
        }

        #endregion

        #region Public Spawner and Proto Initialisation methods

        /// <summary>
        /// Set or create the gena defaults - GenaDefaults is a scriptable object allows people to override keys,
        /// do other startup here as well.
        /// </summary>
        /// <returns></returns>
        public void SetDefaults(string majorVer, string minorVer, string patchVer)
        {

            //Dont reset if we already have them
            if (m_defaults == null)
            {
#if UNITY_EDITOR
                string[] guids = AssetDatabase.FindAssets("GenaDefaults");
                for (int idx = 0; idx < guids.Length; idx++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[idx]);
                    if (path.Contains("GenaDefaults.asset"))
                    {
                        m_defaults = AssetDatabase.LoadAssetAtPath<GenaDefaults>(path);
                    }
                }
#endif
                if (m_defaults == null)
                {
                    m_defaults = ScriptableObject.CreateInstance<GenaDefaults>();
#if UNITY_EDITOR
                    AssetDatabase.CreateAsset(m_defaults, "Assets/GenaDefaults.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
#endif
                }
            }

            //Dont reset the defaults if already done
            if (m_defaultsSet)
            {
                return;
            }

            m_defaultsSet = true;
            int majorVersion, minorVersion, patchVersion;
            if (ParseVersion(majorVer, minorVer, patchVer, out majorVersion, out minorVersion, out patchVersion))
            {
                m_majorVersion = majorVersion;
                m_minorVersion = minorVersion;
                m_patchVersion = patchVersion;
            }
            m_advShowDetailedHelp = m_defaults.m_showDetailedHelp;
            m_advShowMouseOverHelp = m_defaults.m_showTooltips;
            m_autoProbe = m_defaults.m_autoLightProbe;
            m_minProbeGroupDistance = m_defaults.m_minProbeGroupDistance;
            m_minProbeDistance = m_defaults.m_minProbeDistance;
            m_autoOptimise = m_defaults.m_autoOptimize;
            m_maxSizeToOptimise = m_defaults.m_maxOptimizeSize;
            m_randomSeed = UnityEngine.Random.Range(100, 999999);
            m_randomGenerator = new XorshiftPlus(m_randomSeed);
			m_critSpawnCollisionLayers = 1 << LayerMask.NameToLayer("Default");

            // Default Spawn To Target
            m_spawnToTarget = Preferences.DefaultSpawnToTarget;
        }

        /// <summary>
        /// Add a prototype and initialise some of its settings
        /// </summary>
        public void AddProto(Prototype proto)
        {
            m_spawnPrototypes.Add(proto);

            // Init its Spawn Crits
            SetProtoSpawnCrits(proto);

            // Make sure the visualization resolution is optimized
            UpdateVisualisationResolution();
        }

        /// <summary>
        /// Delete a prototype
        /// </summary>
        public void DeleteProto(Prototype proto)
        {
            proto.Delete();
        }

        /// <summary>
        /// Add a tree prototype with default settings
        /// </summary>
        public void AddTreeProto()
        {
            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null && terrain.terrainData.treePrototypes.Length > 0)
            {
                Prototype proto = new Prototype(this);

                Resource res = new Resource(this);
                res.m_prefab = null;
                res.m_resourceType = Constants.ResourceType.TerrainTree;
                // Only prefab Resources can be static
                res.Static = Constants.ResourceStatic.Dynamic;

                TreePrototype treeProto = terrain.terrainData.treePrototypes[res.m_terrainProtoIdx];
                if (treeProto.prefab != null)
                {
                    res.m_name = treeProto.prefab.name;
                    res.m_baseSize = GetInstantiatedBounds(treeProto.prefab).size;
                    res.m_baseScale = treeProto.prefab.transform.localScale;
                    res.m_hasMeshes = HasMeshes(treeProto.prefab);
                    res.m_hasRootCollider = HasRootCollider(treeProto.prefab);
                    res.m_hasColliders = HasColliders(treeProto.prefab);
                    res.m_hasMeshes = HasMeshes(treeProto.prefab);
                }
                else
                {
                    res.m_name = "Unknown asset";
                }
                res.m_conformToSlope = false;

                proto.m_resourceTree.Add(res);
                proto.m_name = res.m_name;
                proto.m_size = res.m_baseSize;
                proto.m_extents = proto.m_size * 0.5f;
                proto.m_resourceType = Constants.ResourceType.TerrainTree;
                proto.m_hasMeshes = res.m_hasMeshes;
                proto.m_hasColliders = res.m_hasColliders;
                proto.m_hasRigidBody = res.m_hasRigidBody;

                //If first one, then update some settings to be more tree friendly
                if (m_spawnPrototypes.Count == 0)
                {
                    m_parentName = res.m_name;
                    m_scaleToNearestInt = false;
                    m_critVirginCheckType = Constants.VirginCheckType.Bounds;
                    m_critBoundsBorder = -0.2f;
                    m_seedThrowRange = Mathf.Min(proto.m_size.x, proto.m_size.z) * 2f;
                }

                // Add proto, set its spawn crits, and set HasActiveTerrainProto
                AddProto(proto);
                m_hasActiveTreeProtos = true;
            }
            else
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("GeNa", "ERROR: You must have an active terrain that also has trees defined in order to add tree resources.", "OK"); 
#endif
            }
        }
        
        /// <summary>
        /// Add a grass prototype with default settings
        /// </summary>
        public void AddGrassProto()
        {
            Terrain terrain = Terrain.activeTerrain;

            if (terrain != null && terrain.terrainData.detailPrototypes.Length > 0)
            {
                Prototype proto = new Prototype(this);

                Resource res = new Resource(this);
                res.m_prefab = null;
                res.m_resourceType = Constants.ResourceType.TerrainGrass;
                // Only prefab Resources can be static
                res.Static = Constants.ResourceStatic.Dynamic;

                DetailPrototype detailProto = terrain.terrainData.detailPrototypes[res.m_terrainProtoIdx];
                if (detailProto.prototypeTexture != null)
                {
                    res.m_name = detailProto.prototypeTexture.name;
                }
                else
                {
                    res.m_name = detailProto.prototype.name;
                }
                res.m_conformToSlope = false;
                res.m_baseSize = new Vector3(detailProto.minWidth, detailProto.minHeight, detailProto.minWidth);
                proto.m_resourceTree.Add(res);
                proto.m_name = res.m_name;
                proto.m_size = res.m_baseSize;
                proto.m_extents = proto.m_size * 0.5f;
                proto.m_resourceType = Constants.ResourceType.TerrainGrass;

                //Set scale to int to false
                m_scaleToNearestInt = false;

                //If first one, then update some settings to be more grass friendly
                if (m_spawnPrototypes.Count == 0)
                {
                    m_parentName = res.m_name;
                    if (m_minInstances < ((m_maxSpawnRange * m_maxSpawnRange) / (0.33f * m_seedThrowRange)))
                    {
                        m_minInstances = (long)((m_maxSpawnRange * m_maxSpawnRange) / (0.33f * m_seedThrowRange));
                        m_maxInstances = m_minInstances;
                    }
                    m_seedThrowRange = Mathf.Min(proto.m_size.x, proto.m_size.z) * 3f;
                    m_critVirginCheckType = Constants.VirginCheckType.Bounds;
                    m_critBoundsBorder = 0.5f;
                }

                // Add proto and set HasActiveTerrainProto
                AddProto(proto);
                m_hasActiveGrassProtos = true;
            }
            else
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("ERROR!", "You must have an active terrain that also has grass defined in order to add grass resources.", "OK"); 
#endif
            }
        }

        /// <summary>
        /// Add a texture prototype with default settings and return the created texture resource.
        /// </summary>
        public void AddTextureProto()
        {
            Terrain terrain = Terrain.activeTerrain;

            if (terrain != null && terrain.terrainData.alphamapLayers > 0)
            {
                Prototype proto = new Prototype(this);

                Resource res = new Resource(this)
                {
                    m_prefab = null,
                    m_resourceType = Constants.ResourceType.TerrainTexture,
                    // Only prefab Resources can be static
                    Static = Constants.ResourceStatic.Dynamic
                };

#if UNITY_2018_3_OR_NEWER
                res.m_name = terrain.terrainData.terrainLayers[res.m_terrainProtoIdx].diffuseTexture.name;
#else
                res.m_name = terrain.terrainData.splatPrototypes[res.m_terrainProtoIdx].texture.name;
#endif
                res.m_conformToSlope = false;

                res.m_baseSize = Vector3.zero;

                //Calculate the minimum final scale
                float splatPixelSize = terrain.terrainData.size.x / terrain.terrainData.alphamapResolution;
                float minScale = (Constants.MIN_TX_BRUSH_SIZE_IN_PIX * splatPixelSize) / (m_sameScale ? m_minScaleX : 0.5f * (m_minScaleX + m_minScaleZ));
                res.m_minScale = res.m_maxScale = minScale * Vector3.one;

                proto.m_resourceTree.Add(res);
                proto.m_name = res.m_name;
                proto.m_size = res.m_baseSize;
                proto.m_extents = proto.m_size * 0.5f;
                proto.m_resourceType = Constants.ResourceType.TerrainTexture;

                //Set scale to int to false
                m_scaleToNearestInt = false;

                // Do presets if this is the only proto
                if (m_spawnPrototypes.Count == 1)
                {
                    // Scale
                    m_minScaleX = m_maxScaleX = 1f;
                    m_minScaleY = m_maxScaleY = 1f;
                    m_minScaleZ = m_maxScaleZ = 1f;
                }

                //If first one, then update some settings to be more texture friendly
                if (m_spawnPrototypes.Count == 0)
                {
                    m_parentName = res.m_name;
                    m_seedThrowRange = res.m_maxScale.x * 3f;
                    m_critVirginCheckType = Constants.VirginCheckType.Point;
                    m_sameScale = true;
                    m_minScaleX = m_maxScaleX = 1f;
                }

                // Add proto and set HasActiveTextureProto
                AddProto(proto);
                m_hasActiveTextureProtos = true;
            }
            else
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("ERROR!", "You must have an active terrain that also has textures defined in order to add texture resources.", "OK");
#endif
            }
        }

        #endregion

        #region Public updaters and utility methods

        /// <summary>
        /// Updates the name of the GameObject according to the parent name.
        /// </summary>
        public void UpdateGoName()
        {
            name = (m_isLinked ? LINKED_SYMBOL : "") + "GeNa Spawner - " + m_parentName + (m_isCache ? CACHE_SYMBOL : "");
        }

        /// <summary>
        /// Child Spawner at that index is being linked to a Resrource.
        /// </summary>
        public void LinkChildSpawner(int index)
        {
            if (m_linkedSpawners.ContainsKey(index))
            {
                m_linkedSpawners[index]++;
            }
            else
            {
                m_linkedSpawners[index] = 1;
                m_childSpawners[index].Link();
            }

            UpdateWhichProtosHaveLinkedSpawners();
        }

        /// <summary>
        /// Child Spawner at that index is being detached from a Resrource.
        /// </summary>
        public void DetachChildSpawner(int index)
        {
            if (!m_linkedSpawners.ContainsKey(index))
            {
                Debug.LogErrorFormat("[GeNA] Index {0} was not marked as a Linked Spawner in {1}", index, name);
            }
            else
            {
                if (m_linkedSpawners[index] < 2)
                {
                    m_linkedSpawners.Remove(index);
                }
                else
                {
                    m_linkedSpawners[index]--;
                }
            }

            m_childSpawners[index].Detach();
            UpdateWhichProtosHaveLinkedSpawners();
        }

        /// <summary>
        /// Looks through the child <seealso cref="Resources"/> and Updates <see cref="=m_hasLinkedSpawner"/> for the <seealso cref="Prototype"/>s.
        /// </summary>
        public void UpdateWhichProtosHaveLinkedSpawners()
        {
            for (int i = 0; i < m_spawnPrototypes.Count; i++)
            {
                m_spawnPrototypes[i].UpdateHasLinkedSpawner();
            }
        }

        /// <summary>
        /// Looks through the child <seealso cref="Resources"/> and Updates <see cref="=m_hasExtensions"/> for the <seealso cref="Prototype"/>s.
        /// </summary>
        public void UpdateWhichProtosHaveExtensions()
        {
            for (int i = 0; i < m_spawnPrototypes.Count; i++)
            {
                m_spawnPrototypes[i].UpdateHasExtensions();
            }
        }

        /// <summary>
        /// Looks through the child <seealso cref="Resources"/> and removes any stale links to Child Spawners (Checks if the indexes are in the Spawner's <seealso cref="m_linkedSpawners"/>.
        /// </summary>
        public void RemoveStaleLinks()
        {
            bool removedAny = false;

            for (int i = 0; i < m_spawnPrototypes.Count; i++)
            {
                removedAny |= m_spawnPrototypes[i].RemoveStaleLinks();
            }

            if (removedAny)
            {
                UpdateWhichProtosHaveLinkedSpawners();
            }
        }

        /// <summary>
        /// Checks if an index is in the <seealso cref="m_linkedSpawners"/> table.
        /// </summary>
        public bool IsChildLinked(int childSpawnerIndex)
        {
            return m_linkedSpawners.ContainsKey(childSpawnerIndex);
        }

        /// <summary>
        /// This Spawner is linked
        /// </summary>
        public void Link()
        {
            m_isLinked = true;
            //Update names to indicate that this Spawner is linked
            name = LINKED_SYMBOL + name.Replace(LINKED_SYMBOL, "");
        }

        /// <summary>
        /// This Spawner is no longer linked
        /// </summary>
        public void Detach()
        {
            m_isLinked = false;
            //Update names to indicate that this Spawner is no longer linked
            name = name.Replace(LINKED_SYMBOL, "");
        }

        /// <summary>
        /// Initialises things right before a spawning event (single, paint or global) and optionally records undo before state.
        /// </summary>
        public void Initialise(Transform target, string description, bool recordUndo = true)
		{
			Initialise(this, target, description, recordUndo);
		}

        /// <summary>
        /// Initialises things right before a spawning event and must be called before it 
		/// (single, paint or global) and optionally records undo before state.
        /// </summary>
        private void Initialise(Spawner rootSpawner, Transform target, string description, bool recordUndo = true)
		{
            // Only do this on active spawners - there could be some inactive linked spawners that get here
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

			// Only need to update the child spawners once from the top level.
			if (target != null)
			{
				//Setup any child spawner dependencies
				UpdateChildSpawners();
			}

			// Do below any initialisations needed, including GeNa extensions.
			InitExtensionsAndAffects(rootSpawner);

			// Call this on the active children
			for (int i = 0; i < m_activeChildSpawners.Count; i++)
			{
				// Not forwarding the target to children, since the parent will record changes
				// on terrains and we don't want to duplicate that effort.
				m_activeChildSpawners[i].Initialise(rootSpawner, null, description, recordUndo);
			}

            // ...and linked children
            foreach (int index in m_linkedSpawners.Keys)
			{
                // Not forwarding the target to children, since the parent will record changes
                // on terrains and we don't want to duplicate that effort.
                ChildSpawners[index].Initialise(rootSpawner, null, description, recordUndo);
			}

			m_extensionUndoList = new List<ExtensionInstance>();

			// Return if not recording undo.
			if (!recordUndo)
			{
				return;
			}

			m_undoRecord = new UndoRecord(this, target, description);
		}

		/// <summary>
		/// Takes care of any needed initialisations, including GeNa extensions.
		/// </summary>
		private void InitExtensionsAndAffects(Spawner rootSpawner)
		{
			if (rootSpawner == this)
			{
				m_affectsHeight = m_hasActiveHeightProtos || m_hasChildWithActiveHeightProtos;
				m_affectsTrees = m_hasActiveTreeProtos || m_hasChildWithActiveTreeProtos;
				m_affectsGrass = m_hasActiveGrassProtos || m_hasChildWithActiveGrassProtos;
				m_affectsTexture = m_hasActiveTextureProtos || m_hasChildWithActiveTextureProtos;
			}

			// Initialise the prototypes - they will init their resources and any GeNa extensions that may need it.
			for (int i = 0; i < m_spawnPrototypes.Count; i++)
			{
				m_spawnPrototypes[i].Initialise(rootSpawner);
			}
		}

		/// <summary>
		/// Record undo using GeNa Undo. Needs to be calle after the Spawning event completed.
		/// </summary>
		public void RecordUndo(string description)
        {
            if (m_undoRecord == null)
            {
                return;
            }

            // Record only if there is something to record
            if (!SpawnedSomething())
            {
                m_undoRecord = null;
                return;
            }

            m_undoRecord.RecordAfter(description);
            GeNaUndo.RecordUndo(m_undoRecord);

#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
#endif

			m_undoRecord = null;
        }

		/// <summary>
		/// Clears the Undo Stack
		/// </summary>
		public void ClearUndoStack()
		{
			m_undoComponent.ClearUndoStack();
		}

		/// <summary>
		/// Clears the Undo Stack and converts the list provided into it.
		/// </summary>
		public void UpdateUndoStack(List<UndoRecord> list)
		{
			m_undoComponent.UpdateUndoStack(list);
		}

		/// <summary>
		/// Was something spawned since Undo Before was recorded?
		/// </summary>
		private bool SpawnedSomething()
        {
            if (m_undoRecord == null)
            {
                Debug.LogWarningFormat("[GeNa] Undo record is null when checking if something was spawned in Spawner '{0}'. " +
                    "Was the pre-spawn init missed, or was there any spawning at all?", name);
                return false;
            }

            GeNaUndoSnapshot before = m_undoRecord.Before;
            for (int i = 0; i < m_spawnPrototypes.Count; i++)
            {
                if (before.ProtoInstanceCounts[i] < m_spawnPrototypes[i].m_instancesSpawned)
                {
                    // This proto spawned
                    return true;
                }
            }

            // Nothing was spawned
            return false;
        }

        /// <summary>
        /// Sets conform slope for all the Prefab Prototypes 
        /// (only the root level Resources are affected since lower levels have it off by default).
        /// </summary>
        public void SetAllProtoConformSlope(bool value)
        {
            for (int i = 0; i < m_spawnPrototypes.Count; i++)
            {
                if (m_spawnPrototypes[i].m_resourceType != Constants.ResourceType.Prefab)
                {
                    continue;
                }

                m_spawnPrototypes[i].SetRootConformSlope(value);
            }
        }

        /// <summary>
        /// Sorts the prototypes alphabetically.
        /// </summary>
        public void SortPrototypesAZ()
        {
#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar("GeNa", "Sorting prototypes...", 0.5f);
#endif

            m_spawnPrototypes.Sort((p1, p2) => p1.m_name.CompareTo(p2.m_name));

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
        }

        /// <summary>
        /// Ensures that all the protoypes have the correct Spawn Criterias set.
        /// </summary>
        public void UpdateSpawnCritOverrides()
        {
            for (int i = 0; i < m_spawnPrototypes.Count; i++)
            {
                SetProtoSpawnCrits(m_spawnPrototypes[i]);
            }
        }

        /// <summary>
        /// Ensures that a protoype has the correct Spawn Criterias set.
        /// </summary>
        private void SetProtoSpawnCrits(Prototype proto)
        {
            proto.m_critVirginCheckType = proto.m_overrideCritVirginCheckType ? proto.m_critVirginCheckType : m_critVirginCheckType;
            proto.m_critSpawnCollisionLayers = proto.m_overrideCritSpawnCollisionLayers ? proto.m_critSpawnCollisionLayers : m_critSpawnCollisionLayers;
            proto.m_critBoundsBorder = proto.m_overrideCritBoundsBorder ? proto.m_critBoundsBorder : m_critBoundsBorder;

            //Check Height
            proto.m_critCheckHeightType = proto.m_overrideCritCheckHeight ? proto.m_critCheckHeightType : m_critCheckHeightType;
            proto.m_critMinSpawnHeight = proto.m_overrideCritMinSpawnHeight ? proto.m_critMinSpawnHeight : m_critMinSpawnHeight;
            proto.m_critMaxSpawnHeight = proto.m_overrideCritMaxSpawnHeight ? proto.m_critMaxSpawnHeight : m_critMaxSpawnHeight;
            proto.m_critHeightVariance = proto.m_overrideCritHeightVariance ? proto.m_critHeightVariance : m_critHeightVariance;

            //Check Slope
            proto.m_critCheckSlopeType = proto.m_overrideCritCheckSlope ? proto.m_critCheckSlopeType : m_critCheckSlopeType;
            proto.MinSpawnSlope = proto.m_overrideCritMinSpawnSlope ? proto.MinSpawnSlope : MinSpawnSlope;
            proto.MaxSpawnSlope = proto.m_overrideCritMaxSpawnSlope ? proto.MaxSpawnSlope : MaxSpawnSlope;
            proto.m_critSlopeVariance = proto.m_overrideCritSlopeVariance ? proto.m_critSlopeVariance : m_critSlopeVariance;

            proto.m_critCheckTextures = proto.m_overrideCritCheckTextures ? proto.m_critCheckTextures : m_critCheckTextures;
            proto.m_critTextureStrength = proto.m_overrideCritTextureStrength ? proto.m_critTextureStrength : m_critTextureStrength;
            proto.m_critTextureVariance = proto.m_overrideCritTextureVariance ? proto.m_critTextureVariance : m_critTextureVariance;
            proto.m_critMaskInvert = proto.m_overrideCritMaskInvert ? proto.m_critMaskInvert : m_critMaskInvert;
        }

        /// <summary>
        /// Ensures that the <see cref="Spawner.m_hasActiveTerrainProto"/> fields of the spawner are up to date.
        /// </summary>
        public void UpdateHasActiveTerrainProto()
        {
			m_hasActiveHeightProtos = false;
            m_hasActiveTreeProtos = false;
            m_hasActiveGrassProtos = false;
            m_hasActiveTextureProtos = false;
            for (int i = 0; i < m_spawnPrototypes.Count; i++)
            {
                if (m_spawnPrototypes[i].m_active)
                {
                    switch (m_spawnPrototypes[i].m_resourceType)
                    {
                        case Constants.ResourceType.TerrainTree:
                            m_hasActiveTreeProtos = true;

                            //If all true, nothing else to do.
                            if (m_hasActiveGrassProtos && m_hasActiveTextureProtos)
                            {
                                return;
                            }
                            break;
                        case Constants.ResourceType.TerrainGrass:
                            m_hasActiveGrassProtos = true;

                            //If all true, nothing else to do.
                            if (m_hasActiveTreeProtos && m_hasActiveTextureProtos)
                            {
                                return;
                            }
                            break;
                        case Constants.ResourceType.TerrainTexture:
                            m_hasActiveTextureProtos = true;

                            //If all true, nothing else to do.
                            if (m_hasActiveGrassProtos && m_hasActiveTreeProtos)
                            {
                                return;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Optimises visualisation according to the prototypes
        /// </summary>
        public void UpdateVisualisationResolution()
        {
            UpdateVisualisationResolution(CalculateMinExtents(true));
        }

        /// <summary>
        /// Optimises visualisation according to the prototypes
        /// </summary>
        public void UpdateVisualisationResolution(Vector3 minExtents)
        {
            float work = m_maxVisualisationDimensions * m_maxVisualisationDimensions * (minExtents.x * minExtents.z);

#if UNITY_EDITOR
            float limit = m_visProcessLimit * EditorPrefs.GetFloat("GeNa Performance Rating", 120000f);
#else
            float limit = m_visProcessLimit * 120000f;
#endif

            if (work > limit)
            {
                int before = m_maxVisualisationDimensions;
                m_maxVisualisationDimensions = Mathf.RoundToInt(Mathf.Sqrt((limit / work) * m_maxVisualisationDimensions * m_maxVisualisationDimensions));
                if (m_maxVisualisationDimensions != before)
                {
                    if (OnUpdate != null)
                    {
                        OnUpdate.Invoke();
                    }
                    Debug.Log("[GeNa] Visualisation resolution was optimised for large objects. You can change the 'Max Visualisation Time' Limit on the Advanced tab.");
                }
            }
        }

        #endregion

        #region Static Utility methods

        /// <summary>
        /// Check to see if the object has a renderer
        /// </summary>
        /// <param name="go">Game object to check</param>
        /// <returns>true if is has a mesh renderer</returns>
        public static bool HasMeshes(GameObject go)
        {
            Renderer[] r = go.GetComponentsInChildren<Renderer>();
            if (r.Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check to see if the object has root collider
        /// </summary>
        /// <param name="go">Game object to check</param>
        /// <returns>true if it has root collider</returns>
        public static bool HasRootCollider(GameObject go)
        {
            Collider c = go.GetComponent<Collider>();
            if (c != null)
            {
                if (c is MeshCollider)
                {
                    return ((MeshCollider)c).convex;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check to see if the object has colliders 
        /// </summary>
        /// <param name="go">Game object to check</param>
        /// <returns>true if it has colliders</returns>
        public static bool HasColliders(GameObject go)
        {
            Collider[] c = go.GetComponentsInChildren<Collider>();
            if (c.Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return the bounds of an instantiated instance of the supplied object
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static Bounds GetInstantiatedBounds(GameObject prefab)
        {
            //Let's make sure that we get identity results - we need the position and parent position hack because the collider bounds are actually not Worlds Space
            Vector3 parentPos = Vector3.zero;
            if (prefab.transform.parent != null)
            {
                parentPos = prefab.transform.parent.position;
                prefab.transform.parent.position = Vector3.zero;
            }                
            Vector3 localPosition = prefab.transform.localPosition;
            Quaternion rotation = prefab.transform.rotation;
            Vector3 scale = prefab.transform.localScale;

            prefab.transform.position = Vector3.zero;
            prefab.transform.rotation = Quaternion.identity;
            prefab.transform.localScale = Vector3.one;

            GameObject go = Instantiate(prefab);
            go.transform.position = prefab.transform.position;

            Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                // Doing this way, so boundless items up in the hierarchy don't stuff up the bounds with their pivot
                if (bounds.size == Vector3.zero)
                {
                    bounds = new Bounds(r.bounds.center, r.bounds.size);
                }
                else
                {
                    bounds.Encapsulate(r.bounds);
                }
            }
            foreach (Collider c in go.GetComponentsInChildren<Collider>())
            {
                // Doing this way, so boundless items up in the hierarchy don't stuff up the bounds with their pivot
                if (bounds.size == Vector3.zero)
                {
                    bounds = new Bounds(c.bounds.center, c.bounds.size);
                }
                else
                {
                    bounds.Encapsulate(c.bounds);
                }
            }

            DestroyImmediate(go);

            if (prefab.transform.parent != null)
            {
                prefab.transform.parent.position = parentPos;
            }
            prefab.transform.localPosition = localPosition;
            prefab.transform.rotation = rotation;
            prefab.transform.localScale = scale;
            return bounds;
        }

        /// <summary>
        /// Return the local collider bounds of the supplied game object
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static Bounds GetLocalColliderBounds(GameObject go)
        {
            Bounds bounds = new Bounds();
            GameObject newGo = UnityEngine.Object.Instantiate(go);
            Collider c;
            foreach (Renderer r in newGo.GetComponentsInChildren<Renderer>())
            {
                c = r.gameObject.GetComponent<Collider>();
                if (c != null)
                {
                    bounds.Encapsulate(c.bounds);
                }
                else
                {
                    c = r.gameObject.AddComponent<BoxCollider>();
                    bounds.Encapsulate(c.bounds);
                }
            }
            UnityEngine.Object.DestroyImmediate(newGo);
            return bounds;
        }

        /// <summary>
        /// Check to see if the object has a rigid body
        /// </summary>
        /// <param name="go">Game object to check</param>
        /// <returns>true if it has a rigid body</returns>
        public static bool HasRigidBody(GameObject go)
        {
            Rigidbody[] r = go.GetComponentsInChildren<Rigidbody>();
            if (r.Length > 0)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Pre-Spawning

        /// <summary>
        /// Set the parent(this overrides Spawn to Target), the location of the spawn and update the target ranges. Must be called prior to a spawn in order to ensure
        /// that the correct information will be used in order to control where the spawn will be applied. 
        /// </summary>
        /// <param name="parent">The parent of the spawn. This overrides Spawn to Target.</param>
        /// <param name="groundObject">This is the object that will be treated as ground by the spawn.</param>
        /// <param name="location">The location of the spawn</param>
        /// <param name="normal">The normal of the spawn at that location</param>
        public void SetSpawnOriginAndParent(GameObject parent, Transform groundObject, Vector3 location, Vector3 normal)
        {
            m_spawnParent = parent;
            SetSpawnOrigin(this, groundObject, location, normal, true);
        }

        /// <summary>
        /// Set the location of the spawn and update the target ranges. Must be called prior to a spawn in order to ensure
        /// that the correct information will be used in order to control where the spawn will be applied. 
        /// </summary>
        /// <param name="groundObject">This is the object that will be treated as ground by the spawn.</param>
        /// <param name="location">The location of the spawn</param>
        /// <param name="normal">The normal of the spawn at that location</param>
        /// <param name="parentedSpawn">True if this spawn uses a predefined parent.</param>
        public void SetSpawnOrigin(Transform groundObject, Vector3 location, Vector3 normal, bool parentedSpawn = false)
		{
			SetSpawnOrigin(this, groundObject, location, normal, true);
		}

        /// <summary>
        /// Set the parent(this overrides Spawn to Target), the location of the spawn and update the target ranges. Must be called prior to a spawn in order to ensure
        /// that the correct information will be used in order to control where the spawn will be applied. 
        /// </summary>
        /// <param name="parent">The parent of the spawn. This overrides Spawn to Target.</param>
        /// <param name="groundObject">This is the object that will be treated as ground by the spawn.</param>
        /// <param name="location">The location of the spawn</param>
        /// <param name="normal">The normal of the spawn at that location</param>
        public void SetSpawnOriginAndParent(Spawner rootSpawner, GameObject parent, Transform groundObject, Vector3 location, Vector3 normal)
        {
            m_spawnParent = parent;
            SetSpawnOrigin(rootSpawner, groundObject, location, normal, true);
        }

        /// <summary>
        /// Set the location of the spawn and update the target ranges. Must be called prior to a spawn in order to ensure
        /// that the correct information will be used in order to control where the spawn will be applied. 
        /// </summary>
        /// <param name="groundObject">This is the object that will be treated as ground by the spawn.</param>
        /// <param name="location">The location of the spawn</param>
        /// <param name="normal">The normal of the spawn at that location</param>
        /// <param name="parentedSpawn">True if this spawn uses a predefined parent.</param>
        public void SetSpawnOrigin(Spawner rootSpawner, Transform groundObject, Vector3 location, Vector3 normal, bool parentedSpawn = false)
        {
			// Only do this only on active the spawners
			if (!gameObject.activeInHierarchy)
			{
				return;
			}

            if (!parentedSpawn)
            {
                // In case we had a spawn with API calls but later we want to do some manual spawning without a predefined parent
                m_spawnParent = null;
            }

            m_spawnTarget = groundObject.gameObject;
            m_spawnOriginLocation = location;
            m_spawnOriginNormal = normal;
            m_spawnOriginBounds = new Bounds(location, new Vector3(m_maxSpawnRange, 5000f, m_maxSpawnRange));
            m_spawnOriginGroundTransform = groundObject;
            if (groundObject != null)
            {
                m_spawnOriginObjectID = groundObject.GetInstanceID();
				Terrain terrainGround = groundObject.GetComponent<Terrain>();
				if (terrainGround != null)
                {
                    m_spawnOriginIsTerrain = true;

                    bool boundaryCritUpdateNeeded = false;
                    if (float.IsNaN(m_terrainYPos) || float.IsNaN(m_terrainHeight))
                    {
                        m_showCritMinSpawnHeight = true;
                        boundaryCritUpdateNeeded = true;
                    }
//#if UNITY_EDITOR
//                    else if (m_terrainYPos != groundObject.transform.position.y || m_terrainHeight != terrainGround.terrainData.size.y)
//                    {
//                        string msg = "This spawner has been initialised with different terrain Y position and height than the current target terrain.\n" +
//                            "(pos: {0} vs {1} & height: {2} vs {3}).\n\n" +
//                            "would you like to adapt your spawn height criterias and their possible ranges to this terrain?\n\n" +
//                            "Top slider boundary: {4} -> {5}\n" +
//                            "Max height: {6} -> {7}\n" +
//                            "Min height: {8} -> {9}\n" +
//                            "Bottom slider boundary: {10} -> {11}\n\n";
//                        msg = string.Format(msg,
//                            m_terrainYPos, groundObject.transform.position.y, m_terrainHeight, terrainGround.terrainData.size.y,
//                            m_topBoundary, groundObject.transform.position.y + terrainGround.terrainData.size.y + 500f,
//                            Mathf.Min(m_critMaxSpawnHeight, groundObject.transform.position.y + terrainGround.terrainData.size.y + 500f),
//                            m_critMinSpawnHeight, m_critMinSpawnHeight - m_terrainYPos + groundObject.transform.position.y,
//                            groundObject.transform.position.y - 100f);
//                        if (EditorUtility.DisplayDialog("GeNa", msg, "Yes", "No"))
//                        {
//                            boundaryCritUpdateNeeded = true;
//                        }
//                        else
//                        {

//                        }
//                    }
//#endif

                    if (boundaryCritUpdateNeeded)
                    {
                        m_critMinSpawnHeight -= float.IsNaN(m_terrainYPos) ? groundObject.transform.position.y : m_terrainYPos;

                        m_terrainYPos = groundObject.transform.position.y;
                        m_bottomBoundary = m_terrainYPos - 100f;

                        m_terrainHeight = terrainGround.terrainData.size.y;
                        m_topBoundary = m_terrainYPos + m_terrainHeight + 4000f;

                        m_critMinSpawnHeight += m_terrainYPos;
                        m_critMaxSpawnHeight = Mathf.Min(m_critMaxSpawnHeight, m_topBoundary);
                        for (int i = 0; i < m_spawnPrototypes.Count; i++)
                        {
                            if (m_spawnPrototypes[i].m_overrideCritMinSpawnHeight == false)
                            {
                                m_spawnPrototypes[i].m_critMinSpawnHeight = m_critMinSpawnHeight;
                            }
                            if (m_spawnPrototypes[i].m_overrideCritMaxSpawnHeight == false)
                            {
                                m_spawnPrototypes[i].m_critMaxSpawnHeight = m_critMaxSpawnHeight;
                            }
                        }
					}					
                }
                else
                {
                    m_spawnOriginIsTerrain = false;
				}
			}
            else
            {
                m_spawnOriginObjectID = int.MinValue;
                m_spawnOriginIsTerrain = false;
            }

            //Update the tree & probe managers
            if (!Application.isPlaying)
            {
                m_treeManager.LoadTreesFromTerrain();
            }

            //Load image mask if we are using one
            if (m_critCheckMask && m_critMaskType == Constants.MaskType.Image)
            {
                if (m_critMaskImage != null)
                {
                    int width = m_critMaskImage.width;
                    int height = m_critMaskImage.height;
                    Color color;
                    m_critMaskImageData = new Heightmap(width, height);
                    m_critMaskAlphaData = new Heightmap(width, height);
                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < height; z++)
                        {
                            //Color.RGBToHSV(m_critMaskImage.GetPixel(x, z), out h, out s, out v);
                            color = m_critMaskImage.GetPixel(x, z);
                            m_critMaskImageData[x, z] = (color.r * 255000000f) + (color.g * 255000f) + (color.b * 255f);
                            m_critMaskAlphaData[x, z] = color.a;
                        }
                    }
                }
            }

            //Setup any child spawner dependencies
            UpdateChildSpawners();

            //Iterate through the children and pass this information on 
            foreach (Spawner spawner in m_childSpawners)
            {
                if (spawner != null)
                {
                    spawner.SetSpawnOrigin(rootSpawner, groundObject, location, normal);
                }
            }
        }

        /// <summary>
        /// Update the spawner ranges that are used to target where the spawner will spawn. Assumes that primary settings have already been made.
        /// </summary>
        /// <param name="hitInfo">The raycast hit info to be used to update the ranges.</param>
        /// <param name="doUpdateChildSpawners">Child spawners will also get updated if true.</param>
        public void UpdateTargetSpawnerRanges(RaycastHit hitInfo, bool doUpdateChildSpawners)
        {
            m_rangesOriginLocation = hitInfo.point;
            m_rangesOriginNormal = hitInfo.normal;
            UpdateTextureTargetAndRange();

            //Now always doing this in case there is a prototype with override texture checking.
            UpdateRangesForSpawnerAndPrototypes();

            if (doUpdateChildSpawners)
            {
                //Setup any child spawner dependencies
                UpdateChildSpawners();

                //Iterate through the children and pass this information on 
                foreach (Spawner spawner in m_childSpawners)
                {
                    if (spawner != null)
                    {
                        spawner.UpdateTargetSpawnerRanges(hitInfo, doUpdateChildSpawners);
                    }
                }
            }
        }

        /// <summary>
        /// Update the spawner ranges that are used to target where the spawner will spawn. Assumes that primary settings have already been made.
        /// </summary>
        /// <param name="doUpdateChildSpawners">Child spawners will also get updated if true.</param>
        public void UpdateTargetSpawnerRanges(bool doUpdateChildSpawners)
        {
            UpdateRangesForSpawnerAndPrototypes();

            if (doUpdateChildSpawners)
            {
                //Setup any child spawner dependencies
                UpdateChildSpawners();

                //Iterate through the children and pass this information on 
                foreach (Spawner spawner in m_childSpawners)
                {
                    if (spawner != null)
                    {
                        spawner.UpdateTargetSpawnerRanges(doUpdateChildSpawners);
                    }
                }
            }
        }

        /// <summary>
        /// Update the spawner ranges for the spawner and the prototypes.
        /// </summary>
        private void UpdateRangesForSpawnerAndPrototypes()
        {
            UpdateSpawnerRanges(m_critCheckHeightType, m_critMinSpawnHeight, m_critMaxSpawnHeight, m_critHeightVariance,
                            m_critCheckSlopeType, MinSpawnSlope, MaxSpawnSlope, m_critSlopeVariance, m_critTextureStrength, m_critTextureVariance,
                            out m_critMinHeight, out m_critMaxHeight, out m_critMinSlope, out m_critMaxSlope,
                            out m_critMinTextureStrength, out m_critMaxTextureStrength,
                            out m_critMaskFractalMin, out m_critMaskFractalMax);

            //Now do it at the prototype level as well
            for (int i = 0; i < m_spawnPrototypes.Count; i++)
            {
                UpdateSpawnerRanges(m_spawnPrototypes[i].m_critCheckHeightType, m_spawnPrototypes[i].m_critMinSpawnHeight, m_spawnPrototypes[i].m_critMaxSpawnHeight,
                    m_spawnPrototypes[i].m_critHeightVariance, m_spawnPrototypes[i].m_critCheckSlopeType, m_spawnPrototypes[i].MinSpawnSlope, m_spawnPrototypes[i].MaxSpawnSlope,
                    m_spawnPrototypes[i].m_critSlopeVariance, m_spawnPrototypes[i].m_critTextureStrength, m_spawnPrototypes[i].m_critTextureVariance,
                    out m_spawnPrototypes[i].m_critMinHeight, out m_spawnPrototypes[i].m_critMaxHeight, out m_spawnPrototypes[i].m_critMinSlope, out m_spawnPrototypes[i].m_critMaxSlope,
                    out m_spawnPrototypes[i].m_critMinTextureStrength, out m_spawnPrototypes[i].m_critMaxTextureStrength,
                    out m_spawnPrototypes[i].m_critMaskFractalMin, out m_spawnPrototypes[i].m_critMaskFractalMax);
            }

            m_needsVisualisationUpdate = true;
        }

        /// <summary>
        /// Update the spawner ranges that are used to target where the spawner will spawn. Assumes that primary settings have already been made.
        /// Single implementation to take care of the spawner and all the prototypes as well.
        /// </summary>
        private void UpdateSpawnerRanges(Constants.CriteriaRangeType checkHeight, float minSpawnHeight, float maxSpawnHeight, float heightVariance,
            Constants.CriteriaRangeType checkSlope, float minSlope, float maxSlope, float slopeVariance, float textureStrength, float textureVariance, 
            out float o_MinHeight, out float o_MaxHeight, out float o_MinSlope, out float o_MaxSlope, 
            out float o_MinTextureStrength, out float o_MaxTextureStrength,
            out float o_MaskFractalMin, out float o_MaskFractalMax)
        {
            //Height
            switch(checkHeight)
            {
                case Constants.CriteriaRangeType.Range:
                    o_MinHeight = m_rangesOriginLocation.y - (0.5f * heightVariance);
                    o_MaxHeight = m_rangesOriginLocation.y + (0.5f * heightVariance);
                    break;
                case Constants.CriteriaRangeType.MinMax:
                    o_MinHeight = minSpawnHeight;
                    o_MaxHeight = maxSpawnHeight;
                    break;
                case Constants.CriteriaRangeType.Mixed:
                    o_MinHeight = m_rangesOriginLocation.y - (0.5f * heightVariance);
                    o_MaxHeight = m_rangesOriginLocation.y + (0.5f * heightVariance);
                    //Apply min/max as well
                    o_MinHeight = Mathf.Clamp(o_MinHeight, minSpawnHeight, maxSpawnHeight);
                    o_MaxHeight = Mathf.Clamp(o_MaxHeight, minSpawnHeight, maxSpawnHeight);
                    break;
                case Constants.CriteriaRangeType.None:
                    o_MinHeight = float.MinValue;
                    o_MaxHeight = float.MaxValue;
                    break;
                default:
                    throw new NotImplementedException("[GeNa] doesn't know this Criteria Range Type (applied to height): " + checkHeight.ToString());
            }

            //Slope
            float slope = Vector3.Angle(Vector3.up, m_rangesOriginNormal);
            switch (checkSlope)
            {
                case Constants.CriteriaRangeType.Range:
                    o_MinSlope = Mathf.Clamp(slope - (slopeVariance * 0.5f), 0f, 90f);
                    o_MaxSlope = Mathf.Clamp(slope + (slopeVariance * 0.5f), 0f, 90f);
                    break;
                case Constants.CriteriaRangeType.MinMax:
                    o_MinSlope = Mathf.Clamp(minSlope, 0f, 90f);
                    o_MaxSlope = Mathf.Clamp(maxSlope, 0f, 90f);
                    break;
                case Constants.CriteriaRangeType.Mixed:
                    //Clamp to min/max as well
                    o_MinSlope = Mathf.Clamp(slope - (slopeVariance * 0.5f), minSlope, maxSlope);
                    o_MaxSlope = Mathf.Clamp(slope + (slopeVariance * 0.5f), minSlope, maxSlope);
                    break;
                case Constants.CriteriaRangeType.None:
                    o_MinSlope = 0f;
                    o_MaxSlope = 90f;
                    break;
                default:
                    throw new NotImplementedException("[GeNa] doesn't know this Criteria Range Type (applied to slope): " + checkSlope.ToString());
            }

            //Texture
            o_MinTextureStrength = Mathf.Clamp01(textureStrength - (textureVariance / 2f));
            o_MaxTextureStrength = Mathf.Clamp01(textureStrength + (textureVariance / 2f));

            //Fractal
            o_MaskFractalMin = Mathf.Clamp01(m_critMaskFractalMidpoint - (m_critMaskFractalRange / 2f));
            o_MaskFractalMax = Mathf.Clamp01(m_critMaskFractalMidpoint + (m_critMaskFractalRange / 2f));
        }

        /// <summary>
        /// Set the location for the spawn, but dont update the target ranges. Ensures that spawning is properly bounds checked from this location.
        /// </summary>
        /// <param name="location"></param>
        private void SetSpawnOrigin(Vector3 location)
        {
            m_spawnOriginLocation = location;
            if (m_spawnOriginIsTerrain)
            {
                Terrain terrain = GetTerrain(location);
                if (terrain != null)
                {
                    //Needed to handle the distance criterion
                    m_spawnOriginLocation.y = terrain.transform.position.y + terrain.SampleHeight(location);
                }
            }
            m_spawnOriginBounds = new Bounds(location, new Vector3(m_maxSpawnRange, 5000f, m_maxSpawnRange));
        }

        /// <summary>
        /// Pick up the textures at the selected location and update them if there is nothing there
        /// </summary>
        private void UpdateTextureTargetAndRange()
        {
            Terrain terrain = GetTerrain(m_rangesOriginLocation);
            if (terrain != null)
            {
                Vector3 terrainLocalPos = terrain.transform.InverseTransformPoint(m_rangesOriginLocation);
                Vector3 normalizedLocalPos =
                    new Vector3(Mathf.InverseLerp(0f, terrain.terrainData.size.x, terrainLocalPos.x),
                        Mathf.InverseLerp(0f, terrain.terrainData.size.y, terrainLocalPos.y),
                        Mathf.InverseLerp(0f, terrain.terrainData.size.z, terrainLocalPos.z));
                float[,,] hms =
                    terrain.terrainData.GetAlphamaps(
                        (int)(normalizedLocalPos.x * (float)(terrain.terrainData.alphamapWidth - 1)),
                        (int)(normalizedLocalPos.z * (float)(terrain.terrainData.alphamapHeight - 1)), 1, 1);
                m_critMaxSelectedTexture = hms.GetLength(2) - 1;
                // Preset these in case the terrain has no texture layers
                m_critSelectedTextureIdx = -1;
                m_critSelectedTextureName = "None";
                float max = 0f;
                for (int idx = 0; idx <= m_critMaxSelectedTexture; idx++)
                {
                    if (hms[0, 0, idx] > max)
                    {
                        max = hms[0, 0, idx];
                        m_critTextureStrength = max;
                        m_critSelectedTextureIdx = idx;
                    }
                }
                if (m_critSelectedTextureIdx >= 0)
                {
#if UNITY_2018_3_OR_NEWER
                    m_critSelectedTextureName =
                        terrain.terrainData.terrainLayers[m_critSelectedTextureIdx].diffuseTexture.name;
#else
                        m_critSelectedTextureName =
                            terrain.terrainData.splatPrototypes[m_critSelectedTextureIdx].texture.name;
#endif
                }
            }
            else
            {
                m_critSelectedTextureName = "Missing terrain";
            }
        }

        /// <summary>
        /// Update the array used for visualisation. Edit mode function that does the hard work for visualisation.
        /// </summary>
        private void UpdateSpawnerVisualisation()
        {
            //Signal that we have been updated
            m_needsVisualisationUpdate = false;
            m_visualizationLocation = m_spawnOriginLocation;

            //Variables
            int x, z;
            Vector3 position = Vector3.zero;
            float halfSpawnRange = m_maxSpawnRange / 2f;
            Vector3 maxPosition = m_spawnOriginLocation + (Vector3.one * halfSpawnRange);
            Vector3 hitLocation = Vector3.zero;
            Vector3 hitNormal = Vector3.zero;
            float hitAlpha = 1f;
            Vector3 minExtents;

            //Calculate minimum extents and make sure visualization resolution is ok
            minExtents = CalculateMinExtents();
            UpdateVisualisationResolution(minExtents);

            //Build active proto list
            List<Prototype> protoList = new List<Prototype>();
            foreach (Prototype proto in m_spawnPrototypes)
            {
                if (proto.m_active == true)
                {
                    protoList.Add(proto);
                }
            }

            //Calculate steps and update array size to handle different dimensions
            int dimensions = (int)m_maxSpawnRange + 1;
            if (dimensions > m_maxVisualisationDimensions)
            {
                dimensions = m_maxVisualisationDimensions + 1;
            }
            float stepIncrement = m_maxSpawnRange / ((float)dimensions - 1f);
            if (dimensions != m_fitnessArray.GetLength(0))
            {
                m_fitnessArray = new float[dimensions, dimensions];
            }
            for (x = 0; x < dimensions; x++)
            {
                for (z = 0; z < dimensions; z++)
                {
                    m_fitnessArray[x, z] = float.MinValue;
                }
            }

            //Exit if nothing to do
            if (protoList.Count == 0)
            {
                return;
            }

#if UNITY_EDITOR
            float t = Time.realtimeSinceStartup; 
#endif

            //Iterate through and calculate the visualisation
            float rotation = Mathf.Clamp(0f, m_minRotationY, m_maxRotationY);

            for (x = 0, position.x = m_spawnOriginLocation.x - halfSpawnRange; position.x < maxPosition.x; x++, position.x += stepIncrement)
            {
                for (z = 0, position.z = m_spawnOriginLocation.z - halfSpawnRange; position.z < maxPosition.z; z++, position.z += stepIncrement)
                {
                    if (CheckLocationForSpawn(position, rotation, protoList, out hitLocation, out hitNormal, out hitAlpha))
                    {
                        m_fitnessArray[x, z] = hitLocation.y;

                        if (m_critVirginCheckType == Constants.VirginCheckType.Bounds)
                        {
                            if (CheckBoundedLocationForSpawn(hitLocation, rotation, minExtents, true))
                            {
                                m_fitnessArray[x, z] = hitLocation.y;
                            }
                            else
                            {
                                m_fitnessArray[x, z] = float.MinValue;
                            }
                        }
                    }
                }
            }

#if UNITY_EDITOR
            if (m_critVirginCheckType == Constants.VirginCheckType.Bounds)
            {
                t = Time.realtimeSinceStartup - t;
                if (m_maxVisualisationDimensions > 25 && t > 0.01f)
                {
                    double work = m_maxVisualisationDimensions * m_maxVisualisationDimensions * (minExtents.x * minExtents.z);
                    double r = work / t;
                    //Debug.LogFormat("{0} in {1}s, rating {2}", work, t, r);

                    float oneWeek = 60 * 60 * 24 * 7;
                    if (Utils.GetFrapoch() - oneWeek > EditorPrefs.GetFloat("GeNa Performance Rating Time", 0f))
                    {
                        EditorPrefs.SetFloat("GeNa Performance Rating", (float)r);
                        EditorPrefs.SetFloat("GeNa Performance Rating Time", Utils.GetFrapoch());
                    }
                    else
                    {
                        EditorPrefs.SetFloat("GeNa Performance Rating", 0.5f * (EditorPrefs.GetFloat("GeNa Performance Rating", 120000f) + (float)r));
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Goes through the prototypes and calculates and returns the min extents the spawner will spawn.
        /// </summary>
        /// <returns></returns>
        private Vector3 CalculateMinExtents(bool useScales = true)
        {
            Vector3 minExtents = Vector3.zero;

            //Calculate extents if its bounds checked
            if (m_critVirginCheckType == Constants.VirginCheckType.Bounds)
            {
                float area = 0f;
                float minArea = 0f;
                Vector3 extents = Vector3.zero;

                // Placement Criteria Scaling
                Vector3 minScale = Vector3.zero;
                if (m_sameScale)
                {
                    minScale = Vector3.one * m_minScaleX;
                }
                else
                {
                    minScale = new Vector3(m_minScaleX, m_minScaleY, m_minScaleZ);
                }

                for (int idx = 0; idx < m_spawnPrototypes.Count; idx++)
                {
                    // No collision for texture
                    if (m_spawnPrototypes[idx].m_resourceType == Constants.ResourceType.TerrainTexture)
                    {
                        continue;
                    }
                                        
                    extents = m_spawnPrototypes[idx].GetMinExtents(minScale);

                    area = extents.x * extents.z;
                    if (area > 0f && (minArea == 0f || area < minArea))
                    {
                        minArea = area;
                        minExtents = extents;
                    }
                }
            }

            //Setting a minimum, because we don't want to use zero extents with visualisation
            minExtents.x = Mathf.Max(0.01f, minExtents.x);
            minExtents.y = Mathf.Max(0.01f, minExtents.y);
            minExtents.z = Mathf.Max(0.01f, minExtents.z);

            return minExtents;
        }

        /// <summary>
        /// Iterate through any children and update the child spawner lists - needed to support childed spawning
        /// </summary>
        public void UpdateChildSpawners()
        {
            // Reset the values so they can be set by the children when appropriate
            m_hasChildWithActiveGrassProtos = false;
            m_hasChildWithActiveTextureProtos = false;
            m_hasChildWithActiveTreeProtos = false;

            UpdateChildSpawners(this);
        }

        /// <summary>
        /// Iterate through any children and update the child spawner lists - needed to support childed spawning
        /// </summary>
        private void UpdateChildSpawners(Spawner rootParent)
		{
			// Only do this only on active the spawners
			if (!gameObject.activeInHierarchy)
			{
				return;
			}

			// If not the top Spawner, update the terrain type switches of the top Spawner.
			if (rootParent != this)
            {
                if (m_hasActiveHeightProtos)
                {
                    rootParent.m_hasChildWithActiveHeightProtos = true;
                }
                if (m_hasActiveGrassProtos)
                {
                    rootParent.m_hasChildWithActiveGrassProtos = true;
                }
                if (m_hasActiveTextureProtos)
                {
                    rootParent.m_hasChildWithActiveTextureProtos = true;
                }
                if (m_hasActiveTreeProtos)
                {
                    rootParent.m_hasChildWithActiveTreeProtos = true;
                }
            }

            m_childSpawners.Clear();
            m_activeChildSpawners.Clear();
            Spawner childSpawner = null;

            int i = 0;
            foreach (Transform child in transform)
            {
                childSpawner = child.gameObject.GetComponent<Spawner>();
                if (childSpawner != null && !childSpawner.IsCache)
                {
                    m_childSpawners.Add(childSpawner);
                    childSpawner.m_showGizmos = false;

                    //Validate linked Spawners
                    if (childSpawner.IsLinked)
                    {
                        if (m_linkedSpawners.ContainsKey(i) == false)
                        {
                            List<string> linkedIndexes = new List<string>();
                            foreach (int key in m_linkedSpawners.Keys)
                            {
                                linkedIndexes.Add(key.ToString());
                            }
                            Debug.LogWarningFormat("[GeNa] *{0}* was marked as a Linked Spawner, but its index [{1}] was not found in the table ({2}).", childSpawner.m_parentName, i, string.Join(", ", linkedIndexes.ToArray()));
                            childSpawner.Detach();
                        }
                    }
                    else if (m_linkedSpawners.ContainsKey(i))
                    {
                        List<string> linkedIndexes = new List<string>();
                        foreach (int key in m_linkedSpawners.Keys)
                        {
                            linkedIndexes.Add(key.ToString());
                        }
                        Debug.LogWarningFormat("[GeNa] *{0}* was not marked as a Linked Spawner, but its index [{1}] is in the table ({2}).", childSpawner.m_parentName, i, string.Join(", ", linkedIndexes.ToArray()));
                        childSpawner.Link();
                    }

                    // Add to active list if not linked and active
                    if (!childSpawner.IsLinked && childSpawner.gameObject.activeInHierarchy)
                    {
                        m_activeChildSpawners.Add(childSpawner);
                    }

                    childSpawner.UpdateChildSpawners(rootParent);

                    i++;
                }
            }

            // Check that we don't have a stale Linked Spawner with an out of range index
            bool staleFound = false;
            List<int> childIndexes = new List<int>(m_linkedSpawners.Keys);
            for (i = 0; i < childIndexes.Count; i++)
            {
                // Remove if stale
                if (m_childSpawners.Count <= childIndexes[i])
                {
                    m_linkedSpawners.Remove(childIndexes[i]);
                    staleFound = true;
                }
            }

            // If any found, we need to update the Resources
            if (staleFound)
            {
                RemoveStaleLinks();
            }
        }

        #endregion

        #region Spawning methods

        /// <summary>
        /// Do things before spawning.
        /// </summary>
        private void OnBeforeSpawn()
        {
#if UNITY_2018_1_OR_NEWER
            if (Physics.autoSyncTransforms == false)
            {
                Physics.autoSyncTransforms = m_turnedOnAutoSyncTransforms = true;
            } 
#endif
        }

        /// <summary>
        /// Do things when spawning is complete.
        /// </summary>
        private void OnAfterSpawn()
        {
#if UNITY_2018_1_OR_NEWER
            if (m_turnedOnAutoSyncTransforms)
            {
                Physics.autoSyncTransforms = m_turnedOnAutoSyncTransforms = false;
            }
#endif
		}

		/// <summary>
		/// Initiate Global Spawn by the hit info provided. Used by the Global Spawn Dialogue.
		/// </summary>
		public void GlobalSpawn(RaycastHit hitInfo)
		{
			GlobalSpawn(hitInfo.point, hitInfo.normal, hitInfo.transform);
		}

		/// <summary>
		/// Initiate Global Spawn by the hit info provided. Used by the Global Spawn Dialogue.
		/// </summary>
		public void GlobalSpawn(Vector3 hitLocation, Vector3 hitNormal, Transform hitTransform)
		{
			Initialise(hitTransform, "Global Spawn");

			SetSpawnOrigin(hitTransform, hitLocation, hitNormal);
			SpawnGlobally();

			RecordUndo("Global Spawn");
#if UNITY_EDITOR
			SceneView.RepaintAll();
			GUIUtility.hotControl = 0;
#endif
			if (OnUpdate != null)
			{
				OnUpdate.Invoke();
			}
		}

		/// <summary>
		/// Run a spawn instance across the entire target object
		/// </summary>
		private void SpawnGlobally()
        {
            //Work out the bounds of the environment
            float minX = float.NaN;
            float minY = float.NaN;
            float minZ = float.NaN;
            float maxX = float.NaN;
            float maxZ = float.NaN;

            // We need inverse percentage for the jitter
            float invJitterPercent = 1f - m_globalSpawnJitterPct;

            if (m_spawnOriginIsTerrain)
            {
                foreach (Terrain terrain in Terrain.activeTerrains)
                {
                    if (float.IsNaN(minY))
                    {
                        minY = terrain.transform.position.y;
                        minX = terrain.transform.position.x;
                        minZ = terrain.transform.position.z;
                        maxX = minX + terrain.terrainData.size.x;
                        maxZ = minZ + terrain.terrainData.size.z;
                    }
                    else
                    {
                        if (terrain.transform.position.x < minX)
                        {
                            minX = terrain.transform.position.x;
                        }
                        if (terrain.transform.position.z < minZ)
                        {
                            minZ = terrain.transform.position.z;
                        }
                        if ((terrain.transform.position.x + terrain.terrainData.size.x) > maxX)
                        {
                            maxX = terrain.transform.position.x + terrain.terrainData.size.x;
                        }
                        if ((terrain.transform.position.z + terrain.terrainData.size.z) > maxZ)
                        {
                            maxZ = terrain.transform.position.z + terrain.terrainData.size.z;
                        }
                    }
                }
            }
            else
            {
                if (m_spawnOriginGroundTransform != null)
                {
                    Bounds b = new Bounds();
                    if (GetObjectBounds(m_spawnOriginGroundTransform.gameObject, ref b))
                    {
                        minX = b.min.x;
                        minY = m_spawnOriginGroundTransform.position.y;
                        minZ = b.min.z;
                        maxX = b.max.x;
                        maxZ = b.max.z;
                    }
                }
            }

            //Grab the original location
            Vector3 originalLocation = m_spawnOriginLocation;

            //Display a progress bar
            #if UNITY_EDITOR
            string displayName = string.Format("Global {0} spawn progress...", m_parentName);
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayProgressBar("Spawning", displayName, 0f);
            }
            #endif

            //Get trees from terrain
            if (!Application.isPlaying)
            {
                m_treeManager.LoadTreesFromTerrain();
                if (m_autoProbe)
                {
                    m_probeManager.LoadProbesFromScene();
                    if (m_probeParent == null)
                    {
                        m_probeParent = GameObject.Find("GeNa Light Probes");
                        if (m_probeParent == null)
                        {
                            m_probeParent = new GameObject("GeNa Light Probes");
                        }
                    }
                }
            }

            OnBeforeSpawn();
            m_gobalSpawning = true;

            //Step across the entire terrain and run a series of spawns across it
            bool cancelled = false;
            Vector3 newLocation = new Vector3(minX, minY, minZ);
            for (float x = minX + JitterAsPct(m_maxSpawnRange, invJitterPercent);
                x < maxX;
                x += JitterAsPct(m_maxSpawnRange, invJitterPercent))
            {
                for (float z = minZ + JitterAsPct(m_maxSpawnRange, invJitterPercent);
                    z < maxZ;
                    z += JitterAsPct(m_maxSpawnRange, invJitterPercent))
                {
                    newLocation.x = JitterAround(x, m_maxSpawnRange * m_globalSpawnJitterPct);
                    newLocation.z = JitterAround(z, m_maxSpawnRange * m_globalSpawnJitterPct);
                    Spawn(newLocation, true);

                    //Display a progress bar
                    #if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        if (EditorUtility.DisplayCancelableProgressBar("Spawning", displayName,
                            (x - minX)/(maxX - minX)))
                        {
                            Debug.LogWarning("[GeNa] Spawn cancelled by user.");
                            cancelled = true;
                            break;
                        }
                    }
                    #endif
                }
                if (cancelled)
                {
                    break;
                }
            }

            m_gobalSpawning = false;
            OnAfterSpawn();

            //Restore original location for visualisation
            SetSpawnOrigin(originalLocation);

            //Hide progress bar
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorUtility.ClearProgressBar();
            }
            #endif
        }

        /// <summary>
        /// Run a spawn instance at this point in the terrain with the specified rotation
        /// </summary>
        /// <param name="location">Location to spawn at</param>
        /// <param name="rotation">Overall Y rotation to be spawned at</param>
        /// <param name="subSpawn">Set to true if this is a sub spawn</param>
        public void Spawn(Vector3 location, float rotation, bool subSpawn)
        {
            m_minRotationY = m_maxRotationY = rotation;
            Spawn(location, subSpawn);
        }

        /// <summary>
        /// Run a spawn instance at this point on the terrain
        /// </summary>
        /// <param name="location">Location to start from</param>
        /// <param name="subSpawn">Set to true if this is a sub spawn</param>
        public void Spawn(Vector3 location, bool subSpawn)
        {
            Spawn(location, 0f, Vector3.one, subSpawn);
        }

        /// <summary>
        /// Run a spawn instance at this point on the terrain
        /// </summary>
        /// <param name="location">Location to start from</param>
        /// <param name="subSpawn">Set to true if this is a sub spawn</param>
        public void Spawn(Vector3 location, float parentRotation, Vector3 parentScale, bool subSpawn)
        {
            if (m_spawnPrototypes.Count == 0)
            {
                Debug.LogWarningFormat("[GeNa] No prototypes to spawn in spawner '{0}'.", name);
                return;
            }

            //Set the spawn origin
            SetSpawnOrigin(location);

            //Set location and normal
            float halfRange = m_maxSpawnRange / 2f;
            Vector3 newLocation = location;
            Vector3 hitLocation = location;
            float   hitAlpha = 1f;
            Vector3 minLocation = new Vector3(-halfRange, 0f, -halfRange);
            Vector3 maxLocation = new Vector3(halfRange, 0f, halfRange);
            Vector3 spawnOffset = minLocation;
            Vector3 spawnProgress = minLocation;
            spawnProgress.x -= m_seedThrowRange; //Will be fixed in first loop iteration
            Vector3 scaleMultiplier = parentScale;
            Vector3 extents = Vector3.zero;
            Vector3 hitNormal = m_spawnOriginNormal;
            Prototype selectedPrototype = null;

            //Iterate through until we have spawned the requested number of instances or exceeded a safety factor
            long currLoop = 0;
            long instancesSpawned = 0;
            long instancesToSpawn = m_randomGenerator.Next((int)m_minInstances, (int)m_maxInstances);
            long maxLoops = instancesToSpawn * 20;
            int currSpawnIdx = 0;
            float rotation = (parentRotation + m_randomGenerator.Next(m_minRotationY, m_maxRotationY)) % 360f;
            GameObject spawnedGo = null;
            List<Vector3> spawnLocations = new List<Vector3>();
            List<Prototype> protoList = new List<Prototype>();
            List<GameObject> goList = new List<GameObject>();

            //Copy the active prototypes - dont want to waste spawn cycles on inactive prototypes
            foreach (Prototype proto in m_spawnPrototypes)
            {
                if (proto.m_active == true)
                {
                    protoList.Add(proto);
                }
            }

            //Exit if nothing to spawn
            if (protoList.Count == 0)
            {
                Debug.LogWarning("No active prototypes to spawn.");
                return;
            }

            if (m_gobalSpawning == false)
            {
                OnBeforeSpawn();
            }

            //Increase the resolution of the physics system to improve collisions - only when in editor - dont want to mess with actual game
            float physicsContactOffset = Physics.defaultContactOffset;
            #if UNITY_5_4_OR_NEWER
            int physicsIterationCount = Physics.defaultSolverIterations;
            #else
            int physicsIterationCount = Physics.solverIterationCount;
            #endif
            if (!Application.isPlaying)
            {
                Physics.defaultContactOffset = 0.003f;
                #if UNITY_5_4_OR_NEWER
                Physics.defaultSolverIterations = 25;
                #else
                Physics.solverIterationCount = 25;
                #endif
            }

            //Load the trees - when a sub spawn this is done by higher level method
            if (!subSpawn && !Application.isPlaying)
            {
                m_treeManager.LoadTreesFromTerrain();
                if (m_autoProbe)
                {
                    m_probeManager.LoadProbesFromScene();
                    if (m_probeParent == null)
                    {
                        m_probeParent = GameObject.Find("GeNa Light Probes");
                        if (m_probeParent == null)
                        {
                            m_probeParent = new GameObject("GeNa Light Probes");
                        }
                    }
                }
            }

            //Update if we are every
            if (m_spawnAlgorithm == Constants.LocationAlgorithm.Every)
            {
                maxLoops = (long)((m_maxSpawnRange / m_seedThrowRange) + 1f);
                maxLoops *= maxLoops;
            }

            //Display a progress bar
#if UNITY_EDITOR
            bool displayProgress = (!Application.isPlaying) && (!subSpawn) && (instancesToSpawn > 200);
			int aboutOnePercent = Mathf.RoundToInt(instancesToSpawn * 0.01f);
			string displayName = string.Format("Local {0} spawn progress...", m_parentName);
            if (displayProgress)
            {
                EditorUtility.DisplayCancelableProgressBar("Spawning", displayName, 0f);
            }
#endif

            //Now iterate until all instances spawned or spawnlimit exceeded
            for (; (instancesSpawned < instancesToSpawn) && (currLoop < maxLoops); currLoop++)
            {
                bool spawnedSomething = false;
                spawnedGo = null;

                //Select a prototype
                selectedPrototype = protoList[m_randomGenerator.Next(0, protoList.Count-1)];

                //Update rotation
                rotation = (parentRotation + m_randomGenerator.Next(m_minRotationY, m_maxRotationY)) % 360f;

                //Update rotation based on last position and new position
                if (m_lastSpawnedObject != null && m_rotationAlgorithm > Constants.RotationAlgorithm.Fixed)
                {
                    if (m_rotationAlgorithm == Constants.RotationAlgorithm.LastSpawnClosest)
                    {
						//See if we can get the closest point - thats what we will use as our from position
						Vector3 closest = Utils.GetNearestVertice(newLocation, m_lastSpawnedObject);
						Quaternion r = Quaternion.LookRotation(closest - newLocation);
                        rotation = r.eulerAngles.y;
					}
					else
                    {
                        Quaternion r = Quaternion.LookRotation(m_lastSpawnedObject.transform.position - newLocation);
                        rotation = r.eulerAngles.y;
                    }
                }

                //Set the new scale
                if (m_sameScale)
                {
                    float scale = m_randomGenerator.Next(m_minScaleX, m_maxScaleX);
                    scaleMultiplier = new Vector3(scale, scale, scale);
                }
                else
                {
                    scaleMultiplier = new Vector3(m_randomGenerator.Next(m_minScaleX, m_maxScaleX),
                        m_randomGenerator.Next(m_minScaleY, m_maxScaleY),
                        m_randomGenerator.Next(m_minScaleZ, m_maxScaleZ));
                }
                
                // Apply parent rotation if this is a Linked Spawner
                if (parentScale != Vector3.one)
                {
                    scaleMultiplier = Vector3.Scale(scaleMultiplier, parentScale);
                }

                //Do Forced Spawn if appropriate
                if (instancesSpawned == 0 && !subSpawn && m_advForcePlaceAtClickLocation)
                {                    
                    int numberOfProtoToSpawn = 0;

                    // For Structured Spawners try to spawn all the prototypes
                    if (m_type == Constants.SpawnerType.Structured)
                    {
                        numberOfProtoToSpawn = protoList.Count;
                    }
                    // Otherwise do a single spawn if we have at least one prototypes
                    else if (protoList.Count > 0)
                    {
                        numberOfProtoToSpawn = 1;
                    }

                    Queue<PaintTask> paintQueue = new Queue<PaintTask>();

                    for (int i = 0; i < numberOfProtoToSpawn; i++)
                    {
                        // For Structured Spawners select each of the prototypes in order
                        if (m_type == Constants.SpawnerType.Structured)
                        {
                            selectedPrototype = protoList[i];
                        }
                        // Otherwise a random prototype was already selected at the beginning of the instance loop

                        // This will contain a precalculated position offset for the top level resource in a tree (not used in now legacy POI prototypes)
                        Vector3 precalculatedRootOffset = Vector3.zero;

                        Vector3 loc = newLocation;
                        // Precalculate location by offsets, if not a (now legacy) POI prototype which have more than one top level resources
                        if (selectedPrototype.m_resourceTree.Count == 1)
                        {
                            if (selectedPrototype.m_resourceTree[0].Static < Constants.ResourceStatic.Dynamic)
                            {
                                // Account for scale as well
                                precalculatedRootOffset = new Vector3(
                                    selectedPrototype.m_resourceTree[0].m_basePosition.x * scaleMultiplier.x,
                                    selectedPrototype.m_resourceTree[0].m_basePosition.y * scaleMultiplier.y,
                                    selectedPrototype.m_resourceTree[0].m_basePosition.z * scaleMultiplier.z);
                            }
                            else
                            {
                                precalculatedRootOffset = new Vector3(
                                    m_randomGenerator.Next(selectedPrototype.m_resourceTree[0].m_minOffset.x, selectedPrototype.m_resourceTree[0].m_maxOffset.x),
                                    m_randomGenerator.Next(selectedPrototype.m_resourceTree[0].m_minOffset.y, selectedPrototype.m_resourceTree[0].m_maxOffset.y),
                                    m_randomGenerator.Next(selectedPrototype.m_resourceTree[0].m_minOffset.z, selectedPrototype.m_resourceTree[0].m_maxOffset.z));

                                // Account for scale as well
                                precalculatedRootOffset.x *= scaleMultiplier.x;
                                precalculatedRootOffset.y *= scaleMultiplier.y;
                                precalculatedRootOffset.z *= scaleMultiplier.z;
                            }

                            // Structured spawners need the root offsets rotated
                            precalculatedRootOffset = RotatePointAroundPivot(precalculatedRootOffset, Vector3.zero, new Vector3(0f, rotation, 0f));

                            // For resource trees we want to consider the top level offset for our location checking
                            loc += new Vector3(precalculatedRootOffset.x, 0f, precalculatedRootOffset.z);
                        }

                        paintQueue.Enqueue(new PaintTask(selectedPrototype, loc, hitNormal, hitAlpha));
                    }

                    while (paintQueue.Count > 0)
                    {
                        PaintTask task = paintQueue.Dequeue();
                        if (PaintPrototype(task.Prototype, task.HitLocation, task.HitNormal, task.HitAlpha, scaleMultiplier, new Vector3(0f, rotation, 0f), true, out spawnedGo))
                        {
                            spawnedSomething = true;
                        }
                        spawnLocations.Add(task.HitLocation);

                        if (spawnedGo != null)
                        {
                            goList.Add(spawnedGo);
                            m_lastSpawnedObject = spawnedGo;
                        }
                    }
                }
                else
                {
                    //Create a spawn offset
                    if (m_spawnAlgorithm == Constants.LocationAlgorithm.Every)
                    {
                        //Incremented constrained spawn offset
                        if (spawnProgress.x < maxLocation.x)
                        {
                            spawnProgress.x += m_seedThrowRange;
                        }
                        else
                        {
                            spawnProgress.x = minLocation.x;
                            spawnProgress.z += m_seedThrowRange;
                            if (spawnProgress.z > maxLocation.z)
                            {
                                currLoop = maxLoops; //Exit out
                                continue;
                            }
                        }

                        //Apply jitter
                        spawnOffset.x = spawnProgress.x + (m_seedThrowJitter * m_randomGenerator.Next(-m_seedThrowRange, m_seedThrowRange));
                        spawnOffset.z = spawnProgress.z + (m_seedThrowJitter * m_randomGenerator.Next(-m_seedThrowRange, m_seedThrowRange));
                    }
                    else
                    {
                        //Random spawn offset
                        spawnOffset = new Vector3(m_randomGenerator.Next(-m_seedThrowRange, m_seedThrowRange), 0f, m_randomGenerator.Next(-m_seedThrowRange, m_seedThrowRange));
                    }

                    //Then work out new location
                    if (m_spawnAlgorithm == Constants.LocationAlgorithm.LastSpawn)
                    {
                        //Create new location
                        if (spawnLocations.Count > 0)
                        {
                            newLocation = spawnLocations[spawnLocations.Count - 1] + spawnOffset;
                        }
                        else
                        {
                            newLocation = location + spawnOffset;
                        }
                    }
                    else if (m_spawnAlgorithm == Constants.LocationAlgorithm.Organic)
                    {
                        //Create new location
                        if (spawnLocations.Count > 0)
                        {
                            newLocation = spawnLocations[currSpawnIdx++] + spawnOffset;
                            if (currSpawnIdx >= spawnLocations.Count)
                            {
                                currSpawnIdx = 0;
                            }
                        }
                        else
                        {
                            newLocation = location + spawnOffset;
                        }
                    }
                    else
                    {
                        //Centred or all
                        newLocation = location + spawnOffset;
                    }

                    //Update rotation based on last position and new position
                    if (m_lastSpawnedObject != null && m_rotationAlgorithm > Constants.RotationAlgorithm.Fixed)
                    {
                        if (m_rotationAlgorithm == Constants.RotationAlgorithm.LastSpawnClosest)
                        {
                            //See if we can get the closest point - thats what we will use as our from position
                            Vector3 closest = Utils.GetNearestVertice(newLocation, m_lastSpawnedObject);
                            Quaternion r = Quaternion.LookRotation(closest - newLocation);
                            rotation = r.eulerAngles.y;
                        }
                        else
                        {
                            Quaternion r = Quaternion.LookRotation(m_lastSpawnedObject.transform.position - newLocation);
                            rotation = r.eulerAngles.y + m_randomGenerator.Next(m_minRotationY, m_maxRotationY);
                        }
                    }

                    int numberOfProtoToSpawn = 0;

                    // For Structured Spawners try to spawn all the prototypes
                    if (m_type == Constants.SpawnerType.Structured)
                    {
                        numberOfProtoToSpawn = protoList.Count;
                    }
                    // Otherwise do a single spawn if we have at least one prototypes
                    else if (protoList.Count > 0)
                    {
                        numberOfProtoToSpawn = 1;
                    }

                    Queue<PaintTask> paintQueue = new Queue<PaintTask>();

                    for (int i = 0; i < numberOfProtoToSpawn; i++)
                    {
                        // For Structured Spawners select each of the prototypes in order
                        if (m_type == Constants.SpawnerType.Structured)
                        {
                            selectedPrototype = protoList[i];
                        }
                        // Otherwise select initial spawned resource
                        else
                        {
                            //Select the prototype we will check - might be overridden further down in the mask tests of CheckLocationForSpawn()
                            selectedPrototype = protoList[m_randomGenerator.Next(0, protoList.Count - 1)];
                        }

                        Vector3 loc = newLocation;
                        Vector3 hitLoc = hitLocation;

                        // This will contain a precalculated position offset for the top level resource in a tree (not used in now legacy POI prototypes)
                        Vector3 precalculatedRootOffset = Vector3.zero;

                        // Precalculate location by offsets, if not a (now legacy) POI prototype which have more than one top level resources
                        if (selectedPrototype.m_resourceTree.Count == 1)
                        {
                            precalculatedRootOffset = new Vector3(
                                m_randomGenerator.Next(selectedPrototype.m_resourceTree[0].m_minOffset.x, selectedPrototype.m_resourceTree[0].m_maxOffset.x),
                                m_randomGenerator.Next(selectedPrototype.m_resourceTree[0].m_minOffset.y, selectedPrototype.m_resourceTree[0].m_maxOffset.y),
                                m_randomGenerator.Next(selectedPrototype.m_resourceTree[0].m_minOffset.z, selectedPrototype.m_resourceTree[0].m_maxOffset.z));


                            // Structured spawners need the root offsets rotated
                            precalculatedRootOffset = RotatePointAroundPivot(precalculatedRootOffset, Vector3.zero, new Vector3(0f, rotation, 0f));

                            // For resource trees we want to consider the top level offset for our location checking
                            loc += new Vector3(precalculatedRootOffset.x, 0f, precalculatedRootOffset.z);
                        }

                        //First make initial check for spawn location and select a prototype
                        if (CheckLocationForSpawn(loc, rotation, protoList, ref selectedPrototype, out hitLoc, out hitNormal, out hitAlpha))
                        {
                            //Precalculate offsets for resources that will be used for both spawning and bounds checking
                            selectedPrototype.PrecalculateOffsets(m_randomGenerator, scaleMultiplier, m_scaleToNearestInt, precalculatedRootOffset);

                            //Then choose path based on whether bounded or not
                            if (selectedPrototype.m_critVirginCheckType != Constants.VirginCheckType.Bounds || selectedPrototype.m_resourceType == Constants.ResourceType.TerrainTexture)
                            {
                                paintQueue.Enqueue(new PaintTask(selectedPrototype, hitLoc, hitNormal, hitAlpha));
                            }
                            else
                            {
                                //Get extents from the prototype
                                extents = selectedPrototype.GetNextExtents(scaleMultiplier);
                                //We go for more accurate results by applying top level resource rotation to the box cast instead of its bounds when getting the extents above
                                float boundRotation = rotation + selectedPrototype.TopRotation;
                                if (CheckBoundedLocationForSpawn(hitLoc, boundRotation, selectedPrototype, extents, false) == true)
                                {
                                    paintQueue.Enqueue(new PaintTask(selectedPrototype, hitLoc, hitNormal, hitAlpha));
                                }
                            }
                        }
                    }

                    while (paintQueue.Count > 0)
                    {
                        PaintTask task = paintQueue.Dequeue();
                        if (PaintPrototype(task.Prototype, task.HitLocation, task.HitNormal, task.HitAlpha, scaleMultiplier, new Vector3(0f, rotation, 0f), false, out spawnedGo))
                        {
                            spawnedSomething = true;
                            spawnLocations.Add(task.HitLocation);
                        }

                        if (spawnedGo != null)
                        {
                            goList.Add(spawnedGo);
                            m_lastSpawnedObject = spawnedGo;
                        }
                    }
                }

                if (spawnedSomething)
                {
                    instancesSpawned++;
                }

#if UNITY_EDITOR
                if (displayProgress && (instancesSpawned % aboutOnePercent == 0))
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Spawning", displayName, (float) instancesSpawned/(float)instancesToSpawn))
                    {
                        currLoop = maxLoops; //Exit out
                        continue;
                    }
                }
#endif
            }

			//Parent them
			if (goList.Count > 0)
            {
                GameObject spawnParent = (m_spawnParent != null) ? m_spawnParent : (m_spawnToTarget ? m_spawnTarget : null);
#if SECTR_CORE_PRESENT
				// When using gravity, only sectorise at FinaliseGravity()
				if (m_doSectorise == false || m_useGravity)
                {
                    ParentSpawned(spawnParent, location, ref goList);
                }
				else
				{
					SECTR_SectorUtils.SendObjectsIntoSectors(
						parentsUndoList: ref m_parentsUndoList,
						gameObjects: goList,
						parentLocation: location,
						hierarchy: new string[] { m_parentName },
						localizeBy: m_sectorReparentingMode,
						mergeSpawns: m_mergeSpawns,
						doGlobalParenting: true
						);
				}
#else
                ParentSpawned(spawnParent, location, ref goList);
#endif
            }

            //Restore physics
            if (!Application.isPlaying)
            {
                Physics.defaultContactOffset = physicsContactOffset;
#if UNITY_5_4_OR_NEWER
                Physics.defaultSolverIterations = physicsIterationCount;
#else
                Physics.solverIterationCount = physicsIterationCount;
#endif
            }

#if UNITY_EDITOR
            if (displayProgress)
            {
                EditorUtility.ClearProgressBar();
            }
#endif

            //Now call this on the children to be spawned and set the random index to -1, so new children can be selected for the next spawn.
            for (int i = 0; i < ChildrenToSpawnNext.Count; i++)
            {
                // Not spawning linked spawners here. They only spawn via the link from a Resource.
                if (ChildrenToSpawnNext[i] != null)
                {
                    ChildrenToSpawnNext[i].Spawn(location, rotation, true);
                }
            }
            m_nextChildIndex = -1;

            //Temp hack for AVS
            if (m_spawnToAVS && m_prefabUndoList.Count > 0)
            {
                Debug.LogWarning("AVS Undo Hack - FIX THIS");
                UnspawnAllPrefabs();
            }

            if (m_gobalSpawning == false)
            {
                OnAfterSpawn();
            }
        }

        /// <summary>
        /// Parent what was spawned
        /// </summary>
        private void ParentSpawned(GameObject spawnParent, Vector3 spawnLocation, ref List<GameObject> goList)
        {
            GameObject parent = null;
            if (m_mergeSpawns)
            {
                // If there is a spawn parent, we want to add the hierarchy to that
                if (spawnParent != null)
                {
#if UNITY_2017_1_OR_NEWER
                    Transform parentTransform = spawnParent.transform.Find(m_parentName);
#else
                    Transform parentTransform = spawnParent.transform.FindChild(m_parentName);
#endif
                    if (parentTransform != null)
                    {
                        parent = parentTransform.gameObject;
                    }
                }
                else
                {
                    parent = GameObject.Find(m_parentName);
                }
            }

            if (parent == null)
            {
                parent = new GameObject(m_parentName);
                // If there is a spawn parent, we want to add the hierarchy to that
                if (spawnParent != null)
                {
                    parent.transform.parent = spawnParent.transform;
                }
                parent.transform.position = spawnLocation;
                m_parentsUndoList.Add(parent);
            }

            //Now add a bunch of stuff to it
            for (int idx = 0; idx < goList.Count; idx++)
            {
                goList[idx].transform.parent = parent.transform;
            }
        }

        /// <summary>
        /// Load the light probes into memory
        /// </summary>
        public void LoadLightProbes()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (m_autoProbe)
                {
                    m_probeManager.LoadProbesFromScene();
                    if (m_probeParent == null)
                    {
                        m_probeParent = GameObject.Find("GeNa Light Probes");
                        if (m_probeParent == null)
                        {
                            m_probeParent = new GameObject("GeNa Light Probes");
                        }
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Instantiate the prototype at the location provided
        /// </summary>
        /// <param name="prototype">The prototype of be spawned</param>
        /// <param name="location">The location to spawn at</param>
        /// <param name="normal">The normal at the location to spawn at</param>
        /// <param name="alpha">The alpha value at the location - can be used to overide some aspects of the spawn</param>
        /// <param name="scaleFactor">The scale factor to apply to what has been spawned</param>
        /// <param name="rotation">The overal rotation to apply to what has been spawned</param>
        /// <param name="forceSpawn">Force at least one instance to be spawned</param>
        /// <param name="spawnedInstance">The spawned instance</param>
        /// <returns>True if something was spawned</returns>
        private bool PaintPrototype(Prototype prototype, Vector3 location, Vector3 normal, float alpha, Vector3 scaleFactor, Vector3 rotation, bool forceSpawn, out GameObject spawnedInstance)
        {
            //Zap out the spawned instance
            spawnedInstance = null;

            //Exit if no prototype supplied
            if (prototype == null)
            {
                Debug.Log("Missing prototype - aborting paint");
                return false;
            }

            List<GameObject> gos = new List<GameObject>();
            List<Gravity.GravityInstance> gis = new List<Gravity.GravityInstance>();

            //Update rotation to account for original spawn rotation
            rotation.y += prototype.m_forwardRotation;

            //Check for prototype level image mask alpha based overrides
            if (!forceSpawn && m_critMaskType == Constants.MaskType.Image)
            {
                //Invert alpha if necessary
                if (prototype.m_invertMaskedAlpha)
                {
                    alpha = 1f - alpha;
                }

                //Do a success check on the alpha
                if (prototype.m_successOnMaskedAlpha)
                {
                    if (m_randomGenerator.Next() > alpha)
                    {
                        return false;
                    }
                }

                //Modify scale based on alpha
                if (prototype.m_scaleOnMaskedAlpha)
                {
                    //Translate alpha to scale
                    float scale = prototype.m_scaleOnMaskedAlphaMin + ((prototype.m_scaleOnMaskedAlphaMax - prototype.m_scaleOnMaskedAlphaMin) * alpha);
                    if (ApproximatelyEqual(scale, 0f))
                    {
                        return false;
                    }
                    scaleFactor *= scale;
                }
            }

            bool spawnedSomething = SpawnResourceTree(null, prototype.m_resourceTree, location, normal, scaleFactor, rotation, new GTransform(Vector3.zero, Vector3.zero, Vector3.one), forceSpawn, ref gos, ref gis, prototype.IdCode);

            //Pass gravity instances to the gravity object for tracking
            if (m_gravity != null)
            {
                m_gravity.AddInstances(gis);
            }

            if (gos.Count == 1)
            {
                spawnedInstance = gos[0];
                m_prefabUndoList.Add(spawnedInstance);
            }
            else if (gos.Count > 1)
            {
                GameObject go = new GameObject(prototype.m_name);
                go.transform.position = location;
                foreach (GameObject g in gos)
                {
                    g.transform.parent = go.transform;
                }
                //Add a sphere collider - this enables bounds radius to be honoured
                if (m_advAddColliderToSpawnedPrefabs == true)
                {
                    //Add the collider to the parent
                    SphereCollider sc = go.AddComponent<SphereCollider>();
                    Vector3 sff = Vector3.Scale(prototype.m_extents, scaleFactor);
                    sc.radius = Mathf.Max(sff.x, sff.z);

                    //Add a disable on awake to stop them from influencing game play
                    go.AddComponent<DisableColliderOnAwake>();

#if UNITY_EDITOR
                    //Make it navigation static
                    GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.NavigationStatic);
#endif
                }
                spawnedInstance = go;
                m_prefabUndoList.Add(spawnedInstance);
            }

            //Update counters
            if (spawnedSomething)
            {
                prototype.m_instancesSpawned++;
                m_instancesSpawned++;
            }

            //And let caller know we spawned something
            return spawnedSomething;
        }

        /// <summary>
        /// Spawns a Resource Tree
        /// </summary>
        private bool SpawnResourceTree(GameObject parent, List<Resource> resourcesAtLevel, Vector3 location, Vector3 normal, Vector3 scale, Vector3 rotation, GTransform cumulativeTransform, bool forceSpawn, 
            ref List<GameObject> topLevelGOs, ref List<Gravity.GravityInstance> gis, string protoIdCode)
        {
            Terrain terrain = null;

            Vector3 resPosition = Vector3.zero;
            Vector3 resScale = Vector3.one;
            Vector3 resRotation = Vector3.zero;
            Vector3 resNormal = normal;

            Vector3 terrainLocalPos = Vector3.zero;
            Vector3 normalizedLocalPos = Vector3.zero;
            bool spawnedSomething = false;

            for (int i = 0; i < resourcesAtLevel.Count; i++)
            {
                Resource res = resourcesAtLevel[i];

                //Work out if we want to run a success failure check
                if (forceSpawn)
                {
                    //Run the success check only if we already have at least one thing spawned
                    if (topLevelGOs.Count > 0)
                    {
                        if (!res.NextSuccess)
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    //Otherwise always run success check
                    if (!res.NextSuccess)
                    {
                        continue;
                    }
                }

                // At the top-level next pos is already calculated into the location
                resPosition = location;
                // Add if not top-level
                if (parent != null)
                {
                    resPosition += res.NextPosition;
                }
                resRotation = res.NextRotation;
                resScale = res.NextScale;

                // Apply spawner scale if it's not one (top-level can have different from one)
                if (scale != Vector3.one)
                {
                    resScale = Vector3.Scale(scale, resScale);
                    //resPosition = location + Vector3.Scale(scale, res.NextPosition);
                }

                // Apply spawner rotation if it's not zero (top-level can have non-zero)
                if (rotation != Vector3.zero)
                {
                    resRotation.x = (rotation.x + resRotation.x) % 360f;
                    resRotation.y = (rotation.y + resRotation.y) % 360f;
                    resRotation.z = (rotation.z + resRotation.z) % 360f;
                    resPosition = RotatePointAroundPivot(resPosition, location, rotation);
                }

                //If terrain, then what is the terrain height and normal at this location - update accordingly
                if (parent == null)
                {
                    if (m_spawnOriginIsTerrain)
                    {
                        terrain = GetTerrain(resPosition);
                        if (terrain != null)
                        {
                            //Set the height based on height at that location
                            resPosition.y = terrain.SampleHeight(resPosition) + terrain.transform.position.y + res.NextPosition.y;

                            //Set the normal based on normal at that location
                            terrainLocalPos = terrain.transform.InverseTransformPoint(resPosition);
                            normalizedLocalPos =
                                new Vector3(Mathf.InverseLerp(0f, terrain.terrainData.size.x, terrainLocalPos.x),
                                    Mathf.InverseLerp(0f, terrain.terrainData.size.y, terrainLocalPos.y),
                                    Mathf.InverseLerp(0f, terrain.terrainData.size.z, terrainLocalPos.z));
                            resNormal = terrain.terrainData.GetInterpolatedNormal(normalizedLocalPos.x, normalizedLocalPos.z);
                        }
                    }
                    else
                    {
                        resPosition.y += res.NextPosition.y;
                    }
                }

                //Calculate the cumulative transform that can be passed to Linked Spawners and Extensions
                CalculateCumulativeTransform(ref cumulativeTransform, resPosition, resScale, resRotation);

                // Spawn the special stuff
                SpawnLinkedSpawners(res, cumulativeTransform.Postition, cumulativeTransform.Rotation, cumulativeTransform.Scale);
                SpawnExtensions(res, new SpawnDetails(parent, cumulativeTransform.Postition, resPosition, cumulativeTransform.Rotation, resRotation, cumulativeTransform.Scale, resScale));

                //Now instantiate it
                switch (res.m_resourceType)
                {
                    //Prefabs
                    case Constants.ResourceType.Prefab:
                        GameObject go = null;

                        //If not an empty GO
                        if (res.m_prefab != null)
                        {
#if UNITY_EDITOR
                            go = PrefabUtility.InstantiatePrefab(res.m_prefab) as GameObject;
#else
                            go = Instantiate(res.m_prefab);
#endif
                            go.name = "_Sp_" + res.m_name;
                        }
                        //Empty GO - container only
                        else
                        {
                            go = new GameObject("_Sp_" + res.m_name + protoIdCode);
                        }

                        if (res.m_conformToSlope == true)
                        {
                            go.name += " C";
                        }

                        // If it has children traverse down the tree
                        if (res.HasChildren)
                        {
                            spawnedSomething |= SpawnResourceTree(go, res.Children, Vector3.zero, resNormal, Vector3.one, Vector3.zero, cumulativeTransform, forceSpawn, ref topLevelGOs, ref gis, protoIdCode);
                        }

                        //If no parent, this is the GO of a top level resource
                        if (parent == null)
                        {
                            topLevelGOs.Add(go);
                        }
                        //Otherwise add the tree node to its parent
                        else
                        {
                            go.transform.parent = parent.transform;
                        }

                        // Set its transform
                        go.transform.localPosition = resPosition;
                        go.transform.localScale = resScale;
                        go.transform.localEulerAngles = resRotation;

                        // Only top level resources conform to slope
                        if (res.m_conformToSlope && parent == null)
                        {
                            go.transform.rotation = Quaternion.FromToRotation(Vector3.up, resNormal) * go.transform.rotation;
                        }

                        //Set up for gravity if necessary
                        if (m_gravity != null)
                        {
#if UNITY_EDITOR
                            //Set it and its childrens flags to 'nothing' so that it drops properly
                            GameObjectUtility.SetStaticEditorFlags(go, 0);
                            for (int childIdx = go.transform.childCount - 1; childIdx > -1; childIdx--)
                            {
                                GameObjectUtility.SetStaticEditorFlags(go.transform.GetChild(childIdx).gameObject, 0);
                            }
#endif

                            if (!res.m_hasRootCollider)
                            {
                                BoxCollider bc = go.AddComponent<BoxCollider>();
                                bc.center = res.m_baseColliderCenter;
                                if (res.m_baseColliderUseConstScale)
                                {
                                    bc.size = res.m_baseColliderScale * res.m_baseColliderConstScaleAmount;
                                }
                                else
                                {
                                    bc.size = res.m_baseColliderScale;
                                }
                            }

                            if (!res.m_hasRigidBody)
                            {
                                go.AddComponent<Rigidbody>();
                            }

                            Gravity.GravityInstance gi = new Gravity.GravityInstance();
                            gi.m_resource = res;
                            gi.m_instance = go;
                            gi.m_startPosition = go.transform.position;
                            gi.m_startRotation = go.transform.rotation.eulerAngles;
                            gis.Add(gi);
                        }
                        else
                        {
                            //Handle optimisation and flags
                            AutoOptimiseGameObject(res, go);

                            //Handle light probes
                            AutoProbeGameObject(res, go);
                        }

                        //Another AVS Tmp HACK
#if VEGETATION_STUDIO || VEGETATION_STUDIO_PRO
                        if (m_spawnToAVS)
                        {
                            if (!string.IsNullOrEmpty(res.m_assetID))
                            {
                                Debug.Log(res.m_assetID);
                                res.m_avsID = VegetationStudioManager.GetVegetationItemID(res.m_assetID);
                                if (string.IsNullOrEmpty(res.m_avsID))
                                {
                                    res.m_avsID = VegetationStudioManager.AddVegetationItem(res.m_prefab, (VegetationType)res.m_avsVegetationType, false);
                                    Debug.Log("Added object " + res.m_avsID);
                                }
                                if (!string.IsNullOrEmpty(res.m_avsID))
                                {
                                    VegetationStudioManager.AddVegetationItemInstance(res.m_avsID, go.transform.position,
                                        go.transform.localScale, go.transform.rotation, true, (byte)11, 1f, true);
                                    Debug.Log("Spawned object " + res.m_avsID);
                                }
                                else
                                {
                                    Debug.Log("VS having a moment");
                                }
                            }
                            else
                            {
                                Debug.Log("Asset ID missing");
                            }
                        }
#endif

                        res.m_instancesSpawned++;
                        spawnedSomething = true;
                        break;

                    //Terrain trees
                    case Constants.ResourceType.TerrainTree:
                        if (terrain != null && res.m_terrainProtoIdx < terrain.terrainData.treePrototypes.Length)
                        {
                            TreeInstance t = new TreeInstance();
                            t.prototypeIndex = res.m_terrainProtoIdx;
                            t.position = normalizedLocalPos;

                            t.widthScale = Mathf.Max(resScale.x, resScale.z);
                            t.heightScale = resScale.y;

                            t.rotation = resRotation.y * (Mathf.PI / 180f); //In radians
                            t.color = Color.white;
                            t.lightmapColor = Color.white;
                            terrain.AddTreeInstance(t);

                            //Add into the tree manager
                            m_treeManager.AddTree(resPosition, t.prototypeIndex);

                            res.m_instancesSpawned++;
                            spawnedSomething = true;
                        }
                        break;

                    //Terrain grass
                    case Constants.ResourceType.TerrainGrass:
                        if (terrain != null && res.m_terrainProtoIdx < terrain.terrainData.detailPrototypes.Length)
                        {
                            int x = (int)(normalizedLocalPos.x * (float)(terrain.terrainData.detailWidth - 1));
                            int z = (int)(normalizedLocalPos.z * (float)(terrain.terrainData.detailHeight - 1));
                            terrain.terrainData.SetDetailLayer(x, z, res.m_terrainProtoIdx,
                                new int[1, 1] { { (int)(resScale.x * 16f) } });
                            res.m_instancesSpawned++;
                            spawnedSomething = true;
                        }
                        break;

                    //Terrain texture
                    case Constants.ResourceType.TerrainTexture:
                        if (terrain != null && res.m_terrainProtoIdx < terrain.terrainData.alphamapLayers)
                        {
                            if (m_splatPainter == null || m_splatPainter.TerrainData == null)
                            {
                                m_splatPainter = new SplatPainter(terrain.terrainData);
                            }

                            float scaleFactorFloat = resScale.x;
                            if (!m_sameScale)
                            {
                                // Brushes are uniform, so taking the max in case the Plaqcement Criteria is not uniform (same scale XYZ)
                                scaleFactorFloat = Mathf.Max(resScale.x, resScale.z);
                            }
                            int size = Mathf.FloorToInt(scaleFactorFloat);
                            m_splatPainter.Size = size;
                            m_splatPainter.Opacity = res.m_opacity;
                            m_splatPainter.TargetStrength = res.m_targetStrength;

                            // Ensure the base brush is not null.
                            if (res.m_baseBrush == null)
                            {
                                res.UpdateBrushTexture();
                            }

                            // Ensure the brush cash is not null
                            if (res.m_brushCache == null)
                            {
                                res.m_brushCache = new Dictionary<int, UBrush>();
                            }

                            // Use cached if possible
                            if (res.m_brushCache.ContainsKey(size) == false)
                            {
                                res.m_brushCache[size] = res.m_baseBrush.GetInSize(size);
                            }
                            m_splatPainter.Brush = res.m_brushCache[size];

                            m_splatPainter.Paint(normalizedLocalPos.x, normalizedLocalPos.z, res.m_terrainProtoIdx);

                            res.m_instancesSpawned++;
                            spawnedSomething = true;
                        }
                        break;
                    default:
#if UNITY_EDITOR
                        EditorUtility.ClearProgressBar();
#endif
                        throw new NotImplementedException("Not sure what to do with ResourceType." + res.m_resourceType);
                }
            }

            return spawnedSomething;
        }

        /// <summary>
        /// Spawn Linked Spawners
        /// </summary>
        private void SpawnLinkedSpawners(Resource res, Vector3 loc, Vector3 rotation, Vector3 scale)
        {
            for (int i = 0; i < res.LinkedSpawners.Count; i++)
            {
                int spawnerIndex = res.LinkedSpawners[i];

                // Spawn only active Spawners
                if (!m_childSpawners[spawnerIndex].gameObject.activeInHierarchy)
                {
                    continue;
                }

                m_childSpawners[spawnerIndex].Spawn(loc, rotation.y, scale, true);
            }
        }

        /// <summary>
        /// Spawn Extensions
        /// </summary>
        private void SpawnExtensions(Resource res, SpawnDetails spawnDetails)
        {
            if (res.ExtensionInstances == null)
            {
                // This should never be null since in Resource.Initialise() it's instantiated with at least an empty list
                Debug.LogErrorFormat("[GeNa] Extension instances is null for Resource '{0}' -> '{1}'", name, res.m_name);
                return;
            }

            for (int i = 0; i < res.ExtensionInstances.Length; i++)
            {
                if (res.ExtensionInstances[i].Instance == null)
                {
                    // Something went wrong
                    Debug.LogErrorFormat("[GeNa-{0}] GeNa Extension instance is null at index" +
						" ['<b>{1}</b>'] (Resource '<b>{2}</b>'.", name, i, res.m_name);
                    continue;
                }

				res.ExtensionInstances[i].Instance.Spawn(this, res, spawnDetails);
				m_extensionUndoList.Add(res.ExtensionInstances[i]);
            }
        }

#if GENA_DEBUG
        /// <summary>
        /// Debug
        /// </summary>
        private void LinkedSpawnerDebug(Vector3 location, Vector3 rotation, Vector3 scale)
        {
            Resource.DebugBounds(location, scale, rotation);
        } 
#endif

        /// <summary>
        /// Calculates the cummulativeTransform
        /// </summary>
        private static GTransform CalculateCumulativeTransform(ref GTransform cumulativeTransform, Vector3 resPosition, Vector3 resScale, Vector3 resRotation)
        {
            //Work out the cummulative Position
            if (resPosition != Vector3.zero)
            {
                if (cumulativeTransform.Rotation != Vector3.zero)
                {
                    Vector3 pivot = cumulativeTransform.Postition;
                    cumulativeTransform.Postition += Vector3.Scale(resPosition, cumulativeTransform.Scale);
                    cumulativeTransform.Postition = RotatePointAroundPivot(cumulativeTransform.Postition, pivot, cumulativeTransform.Rotation);
                }
                else
                {
                    cumulativeTransform.Postition += Vector3.Scale(resPosition, cumulativeTransform.Scale);
                }
            }

            //Work out the cummulative Rotation
            if (resRotation != Vector3.zero)
            {
                cumulativeTransform.Rotation = new Vector3(
                    cumulativeTransform.Rotation.x + resRotation.x % 360f,
                    cumulativeTransform.Rotation.y + resRotation.y % 360f,
                    cumulativeTransform.Rotation.y + resRotation.y % 360f
                    );
            }

            //Work out the cummulative Scale
            if (resScale != Vector3.one)
            {
                cumulativeTransform.Scale = Vector3.Scale(cumulativeTransform.Scale, resScale);
            }

            return cumulativeTransform;
        }

        /// <summary>
        /// Check if the spawned game object can be optimised
        /// </summary>
        /// <param name="resource">The type of resource this game object is an instance of</param>
        /// <param name="go">The game object being checked</param>
        /// <returns>True if it can be optimised, false otherwise</returns>
        private bool CanOptimiseGameObject(Resource resource, GameObject go)
        {
            //Check if spawner optimisation switched off
            if (m_autoOptimise == false)
            {
                return false;
            }

            //Check force optimise flag
            if (resource.m_flagForceOptimise == true)
            {
                return true;
            }

            //Check for can optimise flag
            if (resource.m_flagCanBeOptimised == false)
            {
                return false;
            }

            //Work out size to determine if to auto optimise
            Vector3 newSize = Vector3.Scale(resource.m_baseSize, go.transform.localScale);
            if (newSize.x < m_maxSizeToOptimise && newSize.y < m_maxSizeToOptimise && newSize.z < m_maxSizeToOptimise)
            {
                return true;
            }

            //Ok, we are too small
            return false;
        }

        /// <summary>
        /// Optimise this game object based on spawner and resource settings 
        /// </summary>
        /// <param name="resource">The resource that this game object is an instance of</param>
        /// <param name="go">The game object being optimised</param>
        private void OptimiseGameObject(Resource resource, GameObject go)
        {
#if UNITY_EDITOR
            //If there is are any mesh renderers, then set blend probes on, lightprobes need them to work
            MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
            for (int renderIdx = 0; renderIdx < renderers.Length; renderIdx++)
            {
#if UNITY_5_4_OR_NEWER
                renderers[renderIdx].lightProbeUsage = LightProbeUsage.BlendProbes;
#endif
                if (resource.m_flagIsOutdoorObject)
                {
                    renderers[renderIdx].reflectionProbeUsage = ReflectionProbeUsage.BlendProbesAndSkybox;
                }
                else
                {
                    renderers[renderIdx].reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
                }
            }

            //Then see if we can optimise the editor flags
            StaticEditorFlags flag = 0;

            //Mark everything as static as possible if not moving
            if (!resource.m_flagMovingObject)
            {
                flag |= StaticEditorFlags.BatchingStatic;
                flag |= StaticEditorFlags.OccludeeStatic;
                flag |= StaticEditorFlags.OccluderStatic;
                flag |= StaticEditorFlags.NavigationStatic;
                flag |= StaticEditorFlags.OffMeshLinkGeneration;
                flag |= StaticEditorFlags.ReflectionProbeStatic;
            }

            //And do it to the game object and all its children
            SetGoFlags(go, flag);
#endif
        }

        /// <summary>
        /// Handles optimisation and flag settings for the sepcified game object - editor mode only
        /// </summary>
        /// <param name="resource">The resource this game object is an instance of</param>
        /// <param name="go"></param>
        public void AutoOptimiseGameObject(Resource resource, GameObject go)
        {
#if UNITY_EDITOR
            if (!CanOptimiseGameObject(resource, go))
            {
                //Non optimised - set the flags as specified in its settings
                StaticEditorFlags flag = 0;
                flag |= resource.m_flagBatchingStatic ? StaticEditorFlags.BatchingStatic : flag;
                flag |= resource.m_flagOccludeeStatic ? StaticEditorFlags.OccludeeStatic : flag;
                flag |= resource.m_flagOccluderStatic ? StaticEditorFlags.OccluderStatic : flag;
#if UNITY_5 || UNITY_2017 || UNITY_2018 || UNITY_2019_1
                flag |= resource.m_flagLightmapStatic ? StaticEditorFlags.LightmapStatic : flag;
#else
                flag |= resource.m_flagLightmapStatic ? StaticEditorFlags.ContributeGI : flag; 
#endif
                flag |= resource.m_flagNavigationStatic ? StaticEditorFlags.NavigationStatic : flag;
                flag |= resource.m_flagOffMeshLinkGeneration ? StaticEditorFlags.OffMeshLinkGeneration : flag;
                flag |= resource.m_flagReflectionProbeStatic ? StaticEditorFlags.ReflectionProbeStatic : flag;

                //And do same to game object children
                SetGoFlags(go, flag);
            }
            else
            {
                OptimiseGameObject(resource, go);
            }
#endif
        }

        /// <summary>
        /// Return true if this game object could potentially have a light probe added
        /// </summary>
        /// <param name="resource">Resource the game object is an instance of</param>
        /// <param name="go">The game object</param>
        /// <returns></returns>
        private bool CanProbeGameObject(Resource resource, GameObject go)
        {
            if (m_autoProbe != true)
            {
                return false;
            }

            if (Application.isPlaying == true)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Try and add light probes to this game object - but only if area constraints are met
        /// </summary>
        /// <param name="resource">Resource being probed</param>
        /// <param name="go">Game object being probed</param>
        private void ProbeGameObject(Resource resource, GameObject go)
        {
#if UNITY_EDITOR
            bool canProbe = false;
            LightProbeGroup lpg = GetOrCreateNearestProbeGroup(go.transform.position, out canProbe);
            if (canProbe == true)
            {
                Vector3 newSize = Vector3.Scale(resource.m_baseSize, go.transform.localScale);
                List<Vector3> probePositions = new List<Vector3>(lpg.probePositions);
                Vector3 position = go.transform.position - lpg.transform.position; //Translate to local space relative to lpg
                position += new Vector3(0f, 0.5f, 0f);
                probePositions.Add(position);
                position += new Vector3(0f, newSize.y, 0f);
                probePositions.Add(position);
                position += new Vector3(0f, 5f, 0f);
                probePositions.Add(position);
                lpg.probePositions = probePositions.ToArray();
                m_probeManager.AddProbe(go.transform.position, lpg);
            }
#endif
        }

        /// <summary>
        /// Return probe group with the nearest probe to the given position, or create a new one
        /// </summary>
        /// <param name="position">Position to check for in world coordinates</param>
        /// <param name="canAddNewProbes">Returns whether or not can add new probes at this location</param>
        /// <returns>Nearest probe group or null</returns>
        private LightProbeGroup GetOrCreateNearestProbeGroup(Vector3 position, out bool canAddNewProbes)
        {
            #if UNITY_EDITOR
            List<LightProbeGroup> probeGroups = m_probeManager.GetProbeGroups(position, m_minProbeDistance);
            if (probeGroups.Count != 0)
            {
                canAddNewProbes = false;
                return probeGroups[0];
            }
            else
            {
                canAddNewProbes = true;
                probeGroups = m_probeManager.GetProbeGroups(position, m_minProbeGroupDistance);
                if (probeGroups.Count != 0)
                {
                    return probeGroups[0];
                }
                else
                {
                    //Create new probe group and return it
                    GameObject probeGo = new GameObject(string.Format("Light Probe Group {0:0}x {1:0}z", position.x, position.z));
                    probeGo.transform.position = position;
                    if (m_probeParent == null)
                    {
                        m_probeParent = GameObject.Find("GeNa Light Probes");
                        if (m_probeParent == null)
                        {
                            m_probeParent = new GameObject("GeNa Light Probes");
                        }
                    }
                    probeGo.transform.parent = m_probeParent.transform;
                    LightProbeGroup lpg = probeGo.AddComponent<LightProbeGroup>();
                    lpg.probePositions = new Vector3[0];
                    m_probeUndoList.Add(probeGo);
                    return lpg;
                }
            }
#else
            throw new InvalidOperationException("This method is only available inside the Unity editor");
#endif
        }

        /// <summary>
        /// Automatically handle light probing for this game object - editor mode only
        /// </summary>
        /// <param name="resource">Resource the game object is an instance of</param>
        /// <param name="go">The game object being probed</param>
        public void AutoProbeGameObject(Resource resource, GameObject go)
        {
#if UNITY_EDITOR
            if (CanProbeGameObject(resource, go))
            {
                ProbeGameObject(resource, go);
            }
#endif
        }


        /// <summary>
        /// Set the editor flags on the game object and all its children (recursively)
        /// </summary>
        /// <param name="go">Game object to set flags on</param>
        /// <param name="flags">Flags to set</param>
#if UNITY_EDITOR
        private void SetGoFlags(GameObject go, StaticEditorFlags flags)
        {
            //Handle rubbish
            if (go == null)
            {
                return;
            }
            //Set on this game object
            GameObjectUtility.SetStaticEditorFlags(go, flags);

            //Do same for all child game objects
            for (int childIdx = 0; childIdx < go.transform.childCount; childIdx++)
            {
                SetGoFlags(go.transform.GetChild(childIdx).gameObject, flags);
            }
        }
#endif

        /// <summary>
        /// Check a location to see if it can be spawned in. Overload that's used by the visualization method only.
        /// This method doesn't look at prototypes' overrides. Only the spawner Spawn Criterias.
        /// </summary>
        /// <param name="location">The location the be checked</param>
        /// <param name="rotation">The Y rotation of the prototype be checked</param>
        /// <param name="prototypes">The list of protoypes we have to choose from - assumed to be active</param>
        /// <param name="hitLocation">The location of the spot the colision occurred</param>
        /// <param name="hitNormal">The normal of the spot the colision occurred</param>
        /// <param name="hitAlhpaMask">The alpha mask value of the spot the colision occurred</param>
        /// <returns>True if the location is ok to spawn in</returns>
        private bool CheckLocationForSpawn(Vector3 location, float rotation, List<Prototype> prototypes, out Vector3 hitLocation, out Vector3 hitNormal, out float hitAlpha)
        {
            //Setup output
            hitLocation = location;
            hitNormal = Vector3.up;
            hitAlpha = 0f;

            // This will never be true but leaving it here in case the code that calls this changes.
            if (prototypes.Count <= 0)
            {
                //Cant spawn if no resources
                return false;
            }

            //Lift the ray check point slightly as we may already be on the ground
            Ray ray = new Ray(new Vector3(location.x, location.y + m_advSpawnCheckOffset, location.z), Vector3.down);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, m_advSpawnCheckOffset + 1000f, m_critSpawnCollisionLayers))
            {
                //Assign the output values
                hitLocation = hitInfo.point;
                hitNormal = hitInfo.normal;

                //Check range
                if (m_spawnRangeShape == Constants.SpawnRangeShape.Circle)
                {
                    //if (Vector3.Distance(m_spawnOriginLocation, hitLocation) > (m_maxSpawnRange / 2f))
                    float xDistance = m_spawnOriginLocation.x - hitLocation.x;
                    float zDistance = m_spawnOriginLocation.z - hitLocation.z;
                    float spawnRadius = m_maxSpawnRange * 0.5f;
                    float sqrDistance = xDistance * xDistance + zDistance * zDistance;
                    if (sqrDistance > spawnRadius * spawnRadius)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!m_spawnOriginBounds.Contains(hitLocation))
                    {
                        return false;
                    }
                }

                //Check height
                if (m_critCheckHeight == true)
                {
                    if (hitLocation.y < m_critMinHeight || hitLocation.y > m_critMaxHeight)
                    {
                        return false;
                    }
                }

                //Check slope
                if (m_critCheckSlope == true)
                {
                    float slope = Vector3.Angle(Vector3.up, hitNormal);
                    if (slope < m_critMinSlope || slope > m_critMaxSlope)
                    {
                        return false;
                    }
                }

                //Check mask - may result in a different prototype being selected
                if (m_critCheckMask == true)
                {
                    if (m_critMaskType != Constants.MaskType.Image)
                    {
                        float value = m_critMaskFractal.GetNormalisedValue(100000f + hitLocation.x, 100000f + hitLocation.z);
                        if (m_critMaskInvert == true)
                        {
                            if (value >= m_critMaskFractalMin && value <= m_critMaskFractalMax)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (value < m_critMaskFractalMin || value > m_critMaskFractalMax)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (m_critMaskImageData != null && m_critMaskImageData.HasData())
                        {
                            //Need to rotate the image mask to remain consistent with overall spawm rotation
                            Vector3 newLocation = RotatePointAroundPivot(hitLocation, m_spawnOriginLocation, new Vector3(0f, 180f - rotation, 0f));
                            float xN = ((m_spawnOriginLocation.x - newLocation.x) / m_maxSpawnRange) + 0.5f;
                            float zN = ((m_spawnOriginLocation.z - newLocation.z) / m_maxSpawnRange) + 0.5f;

                            //Drop out if out of bounds
                            if (xN < 0f || xN >= 1f || zN < 0f || zN > 1f)
                            {
                                return false;
                            }

                            //Grab the alpha at this point
                            hitAlpha = m_critMaskAlphaData[xN, zN];

                            //Recreate the colour at this point in the map
                            float v = m_critMaskImageData[xN, zN];
                            Color c = new Color();
                            c.b = v % 1000f;
                            v -= c.b;
                            v /= 1000f;
                            c.b /= 255f;
                            c.g = v % 1000f;
                            v -= c.g;
                            v /= 1000f;
                            c.g /= 255f;
                            c.r = v;
                            c.r /= 255f;
                            
                            //Find a proto that can be spawned here - We don't do specific proto check for Structured spawners here
                            //Build a cut down list of prototypes that can match this colour
                            Prototype proto;
                            List<Prototype> newProtoList = new List<Prototype>();
                            for (int idx = 0; idx < prototypes.Count; idx++)
                            {
                                proto = prototypes[idx];
                                if (RGBDifference(c, proto.m_imageFilterColour) < ((1f - proto.m_imageFilterFuzzyMatch) * 100f))
                                {
                                    newProtoList.Add(proto);
                                }
                            }

                            //And exit false if nothing matches
                            if (newProtoList.Count == 0)
                            {
                                return false;
                            }

                            //Further refine this list for alpha success when it has been chosen
                            for (int idx = 0; idx < newProtoList.Count;)
                            {
                                if (newProtoList[idx].m_successOnMaskedAlpha == true)
                                {
                                    if (!newProtoList[idx].m_invertMaskedAlpha)
                                    {
                                        if (ApproximatelyEqual(hitAlpha, 0f))
                                        {
                                            newProtoList.RemoveAt(idx);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (ApproximatelyEqual(1f - hitAlpha, 0f))
                                        {
                                            newProtoList.RemoveAt(idx);
                                            continue;
                                        }
                                    }
                                }
                                idx++;
                            }

                            //Then exit if no resources left
                            if (newProtoList.Count == 0)
                            {
                                return false;
                            }                            
                        }
                    }
                }

                //Get the terrain if we are a hit on terrain, and store it for later
                Terrain terrain = null;
                if (hitInfo.collider is TerrainCollider)
                {
                    terrain = hitInfo.transform.GetComponent<Terrain>();
                }

                //Virgin is 'when we collide with ourselves', except for terrain, in which it is 'when we collide with ourselves, and its not a tree'
                if (m_critVirginCheckType != Constants.VirginCheckType.None)
                {
                    //If the original was a terrain, then we must have any terrain, or fail
                    if (m_spawnOriginIsTerrain)
                    {
                        //If we started on terrain, and are no longer on terrain, then exit false
                        if (terrain == null)
                        {
                            return false;
                        }
                        //Use the tree manager to do hits on trees
                        if (m_treeManager.Count(hitLocation, 0.5f) > 0)
                        {
                            return false;
                        }
                    }
                    //Otherwise we have to hit the original object to be considered virgin
                    else
                    {
                        //If we hit something else then we are not virgin, except when its terrain, as we want to spawn across all terrains
                        if (hitInfo.transform.GetInstanceID() != m_spawnOriginObjectID)
                        {
                            return false;
                        }
                    }
                }

                //Do the texture test
                if (m_critCheckTextures == true && m_critSelectedTextureIdx >= 0)
                {
                    if (terrain != null)
                    {
                        Vector3 terrainLocalPos = terrain.transform.InverseTransformPoint(hitLocation);
                        Vector3 normalizedLocalPos =
                            new Vector3(Mathf.InverseLerp(0f, terrain.terrainData.size.x, terrainLocalPos.x),
                                Mathf.InverseLerp(0f, terrain.terrainData.size.y, terrainLocalPos.y),
                                Mathf.InverseLerp(0f, terrain.terrainData.size.z, terrainLocalPos.z));
                        float[,,] hms =
                            terrain.terrainData.GetAlphamaps(
                                (int)(normalizedLocalPos.x * (float)(terrain.terrainData.alphamapWidth - 1)),
                                (int)(normalizedLocalPos.z * (float)(terrain.terrainData.alphamapHeight - 1)), 1, 1);
                        if (hms.GetLength(2) - 1 < m_critSelectedTextureIdx)
                        {
                            return false;
                        }
                        if (hms[0, 0, m_critSelectedTextureIdx] < m_critMinTextureStrength ||
                            hms[0, 0, m_critSelectedTextureIdx] > m_critMaxTextureStrength)
                        {
                            return false;
                        }
                    }
                }

                //Ok, all tests passed, return true
                return true;
            }

            //Failed raycast hit test, return false
            return false;
        }

        /// <summary>
        /// Check a location to see if it can be spawned in.
        /// Checks
        /// 1. Spawn Range
        /// 2. Height
        /// 3. Slope
        /// 4. Mask
        /// 5. Virgin (Point check)
        /// 6. Texture
        /// </summary>
        /// <param name="location">The location the be checked</param>
        /// <param name="rotation">The Y rotation of the prototype be checked</param>
        /// <param name="prototypes">The list of protoypes we have to choose from - assumed to be active</param>
        /// <param name="selectedPrototype">The protoype what was chose for spawning</param>
        /// <param name="hitLocation">The location of the spot the colision occurred</param>
        /// <param name="hitNormal">The normal of the spot the colision occurred</param>
        /// <param name="hitAlhpaMask">The alpha mask value of the spot the colision occurred</param>
        /// <returns>True if the location is ok to spawn in</returns>
        private bool CheckLocationForSpawn(Vector3 location, float rotation, List<Prototype> prototypes, ref Prototype selectedPrototype, out Vector3 hitLocation, out Vector3 hitNormal, out float hitAlpha)
        {
            //Setup output
            hitLocation = location;
            hitNormal = Vector3.up;
            hitAlpha = 0f;

            // This will never be true but leaving it here in case the code that calls this changes.
            if (prototypes.Count <= 0)
            {
                //Cant spawn if no resources
                selectedPrototype = null;
                return false;
            }

            //Lift the ray check point slightly as we may already be on the ground
            Ray ray = new Ray(new Vector3(location.x, location.y + m_advSpawnCheckOffset, location.z), Vector3.down);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, m_advSpawnCheckOffset + 1000f, selectedPrototype.m_critSpawnCollisionLayers))
            {
                //Assign the output values
                hitLocation = hitInfo.point;
                hitNormal = hitInfo.normal;

                //Check range
                if (m_spawnRangeShape == Constants.SpawnRangeShape.Circle)
                {
                    if (Vector3.Distance(m_spawnOriginLocation, hitLocation) > (m_maxSpawnRange / 2f))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!m_spawnOriginBounds.Contains(hitLocation))
                    {
                        return false;
                    }
                }

                //Check height
                if (selectedPrototype.m_critCheckHeight == true)
                {
                    if (hitLocation.y < selectedPrototype.m_critMinHeight || hitLocation.y > selectedPrototype.m_critMaxHeight)
                    {
                        return false;
                    }
                }

                //Check slope
                if (selectedPrototype.m_critCheckSlope == true)
                {
                    float slope = Vector3.Angle(Vector3.up, hitNormal);
                    if (slope < selectedPrototype.m_critMinSlope || slope > selectedPrototype.m_critMaxSlope)
                    {
                        return false;
                    }
                }

                //Check mask - may result in a different prototype being selected
                bool checkMask = selectedPrototype.m_disableCritCheckMask ? false : m_critCheckMask;
                if (checkMask == true)
                {
                    if (m_critMaskType != Constants.MaskType.Image)
                    {
                        float value = m_critMaskFractal.GetNormalisedValue(100000f + hitLocation.x, 100000f + hitLocation.z);
                        if (selectedPrototype.m_critMaskInvert == true)
                        {
                            if (value >= selectedPrototype.m_critMaskFractalMin && value <= selectedPrototype.m_critMaskFractalMax)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (value < selectedPrototype.m_critMaskFractalMin || value > selectedPrototype.m_critMaskFractalMax)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (m_critMaskImageData != null && m_critMaskImageData.HasData())
                        {
                            //Need to rotate the image mask to remain consistent with overall spawm rotation
                            Vector3 newLocation = RotatePointAroundPivot(hitLocation, m_spawnOriginLocation, new Vector3(0f, 180f - rotation, 0f));
                            float xN = ((m_spawnOriginLocation.x - newLocation.x) / m_maxSpawnRange) + 0.5f;
                            float zN = ((m_spawnOriginLocation.z - newLocation.z) / m_maxSpawnRange) + 0.5f;

                            //Drop out if out of bounds
                            if (xN < 0f || xN >= 1f || zN < 0f || zN > 1f)
                            {
                                return false;
                            }

                            //Grab the alpha at this point
                            hitAlpha = m_critMaskAlphaData[xN, zN];

                            //Recreate the colour at this point in the map
                            float v = m_critMaskImageData[xN, zN];
                            Color c = new Color();
                            c.b = v % 1000f;
                            v -= c.b;
                            v /= 1000f;
                            c.b /= 255f;
                            c.g = v % 1000f;
                            v -= c.g;
                            v /= 1000f;
                            c.g /= 255f;
                            c.r = v;
                            c.r /= 255f;
                            
                            // For Structured Spawners only care about this proto
                            if (m_type == Constants.SpawnerType.Structured)
                            {
                                // Check if the selected proto can match this colour
                                if (RGBDifference(c, selectedPrototype.m_imageFilterColour) >= ((1f - selectedPrototype.m_imageFilterFuzzyMatch) * 100f))
                                {
                                    //And exit false if not
                                    selectedPrototype = null;
                                    return false;
                                }

                                //Further refine this for alpha success if  it has been chosen
                                if (selectedPrototype.m_successOnMaskedAlpha == true)
                                {
                                    if (!selectedPrototype.m_invertMaskedAlpha)
                                    {
                                        if (ApproximatelyEqual(hitAlpha, 0f))
                                        {
                                            selectedPrototype = null;
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        if (ApproximatelyEqual(1f - hitAlpha, 0f))
                                        {
                                            selectedPrototype = null;
                                            return false;
                                        }
                                    }
                                }
                            }
                            // Otherwise find a proto that can be spawned here
                            else
                            {
                                //Build a cut down list of prototypes that can match this colour
                                Prototype proto;
                                List<Prototype> newProtoList = new List<Prototype>();
                                for (int idx = 0; idx < prototypes.Count; idx++)
                                {
                                    proto = prototypes[idx];
                                    if (RGBDifference(c, proto.m_imageFilterColour) < ((1f - proto.m_imageFilterFuzzyMatch) * 100f))
                                    {
                                        newProtoList.Add(proto);
                                    }
                                }

                                //And exit false if nothing matches
                                if (newProtoList.Count == 0)
                                {
                                    selectedPrototype = null;
                                    return false;
                                }

                                //Further refine this list for alpha success when it has been chosen
                                for (int idx = 0; idx < newProtoList.Count;)
                                {
                                    if (newProtoList[idx].m_successOnMaskedAlpha == true)
                                    {
                                        if (!newProtoList[idx].m_invertMaskedAlpha)
                                        {
                                            if (ApproximatelyEqual(hitAlpha, 0f))
                                            {
                                                newProtoList.RemoveAt(idx);
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            if (ApproximatelyEqual(1f - hitAlpha, 0f))
                                            {
                                                newProtoList.RemoveAt(idx);
                                                continue;
                                            }
                                        }
                                    }
                                    idx++;
                                }

                                //Then exit if no resources left
                                if (newProtoList.Count == 0)
                                {
                                    selectedPrototype = null;
                                    return false;
                                }

                                //Select the prototype we will use from whats left
                                selectedPrototype = newProtoList[m_randomGenerator.Next(0, newProtoList.Count - 1)];
                            }
                        }
                    }
                }

                //Get the terrain if we are a hit on terrain, and store it for later
                Terrain terrain = null;
                if (hitInfo.collider is TerrainCollider)
                {
                    terrain = hitInfo.transform.GetComponent<Terrain>();
                }

                //Virgin is 'when we collide with the spawn target', except for terrain, in which it is 'when we collide with the terrain, and its not a terrain tree'
                if (selectedPrototype.m_critVirginCheckType != Constants.VirginCheckType.None)
                {
                    //If the original was a terrain, then we must have any terrain, or fail
                    if (m_spawnOriginIsTerrain)
                    {
                        //If we started on terrain, and are no longer on terrain, then exit false
                        if (terrain == null)
                        {
                            return false;
                        }
                        //Use the tree manager to do hits on trees
                        if (m_treeManager.Count(hitLocation, 0.5f) > 0)
                        {
                            return false;
                        }
                    }
                    //Otherwise we have to hit the spawn target object to be considered virgin
                    else
                    {
                        //If we hit something else then we are not virgin, except when its terrain, as we want to spawn across all terrains
                        if (hitInfo.transform.GetInstanceID() != m_spawnOriginObjectID)
                        {
                            return false;
                        }
                    }
                }

                //Do the texture test
                if (selectedPrototype.m_critCheckTextures == true && m_critSelectedTextureIdx >= 0)
                {
                    if (terrain != null)
                    {
                        Vector3 terrainLocalPos = terrain.transform.InverseTransformPoint(hitLocation);
                        Vector3 normalizedLocalPos =
                            new Vector3(Mathf.InverseLerp(0f, terrain.terrainData.size.x, terrainLocalPos.x),
                                Mathf.InverseLerp(0f, terrain.terrainData.size.y, terrainLocalPos.y),
                                Mathf.InverseLerp(0f, terrain.terrainData.size.z, terrainLocalPos.z));
                        float[,,] hms =
                            terrain.terrainData.GetAlphamaps(
                                (int)(normalizedLocalPos.x * (float)(terrain.terrainData.alphamapWidth - 1)),
                                (int)(normalizedLocalPos.z * (float)(terrain.terrainData.alphamapHeight - 1)), 1, 1);
                        if (hms.GetLength(2) - 1 < m_critSelectedTextureIdx)
                        {
                            return false;
                        }
                        if (hms[0, 0, m_critSelectedTextureIdx] < selectedPrototype.m_critMinTextureStrength ||
                            hms[0, 0, m_critSelectedTextureIdx] > selectedPrototype.m_critMaxTextureStrength)
                        {
                            return false;
                        }
                    }
                }

                //Ok, all tests passed, return true
                return true;
            }

            //Failed raycast hit test, return false
            return false;
        }

        /// <summary>
        /// Check a bounded location to see if it can be spawned in - assumes crit check type is bounds. Overload that's used by the visualization method only.
        /// This method doesn't look at prototypes' overrides. Only the spawner Spawn Criterias.
        /// </summary>
        /// <param name="location">The location the be checked</param>
        /// <param name="rotation">The Y rotation of the prototype be checked</param>
        /// <param name="prototype">The prototype that has been selected</param>
        /// <param name="extents">The extents of the area to be checked</param>
        /// <param name="visualising">If this is a check for visualisation - will lower the resolution and cpu overhead</param>
        /// <returns>True if the location is ok to spawn in</returns>
        private bool CheckBoundedLocationForSpawn(Vector3 location, float rotation, Vector3 extents, bool visualising)
        {
            //First things first - do an extents based tree collision check - will bypass a bunch of other expensive operations
            if (m_spawnOriginIsTerrain)
            {
                //Use the tree manager to do approximate hits
                if (m_treeManager.Count(location, Mathf.Max(extents.x, extents.z)) > 0)
                {
                    return false;
                }
            }

			//Use BoxCast for the bounds
            //Lift the ray check point slightly as we may already be on the ground
			RaycastHit hitInfo;
			if (Physics.BoxCast(new Vector3(location.x, location.y + m_advSpawnCheckOffset, location.z), extents, 
				Vector3.down, out hitInfo, Quaternion.Euler(0f, rotation, 0f), m_advSpawnCheckOffset+1000f,
                m_critSpawnCollisionLayers))
            {
                //Do the height test
                if (m_critCheckHeight == true)
                {
                    if (hitInfo.point.y < m_critMinHeight || hitInfo.point.y > m_critMaxHeight)
                    {
                        return false;
                    }
                }

                //Do the slope test
                if (m_critCheckSlope == true)
                {
                    float slope = Vector3.Angle(Vector3.up, hitInfo.normal);
                    if (slope < m_critMinSlope || slope > m_critMaxSlope)
                    {
                        return false;
                    }
                }

                //Get the terrain if we are a hit on terrain, and store it for later
                Terrain terrain = null;
                if (hitInfo.collider is TerrainCollider)
                {
                    terrain = hitInfo.transform.GetComponent<Terrain>();
                }

                //If the original was a terrain, then we must have any terrain, or fail
                if (m_spawnOriginIsTerrain)
                {
                    if (terrain == null)
                    {
                        return false;
                    }
                }
                //Otherwise we have to hit the original object to be considered virgin
                else
                {
                    //If we hit something else then we are not virgin, except when its terrain, as we want to spawn across all terrains
                    if (hitInfo.transform.GetInstanceID() != m_spawnOriginObjectID)
                    {
                        return false;
                    }
                }

                //Not checking mask - That's only done in the overload where we check protos

                //Do the texture test
                if (m_critCheckTextures == true && m_critSelectedTextureIdx >= 0)
                {
                    //Introduce a random sampling for performance
                    if (terrain != null && UnityEngine.Random.Range(0, 5) == 1)
                    {
                        Vector3 terrainLocalPos = terrain.transform.InverseTransformPoint(hitInfo.point);
                        Vector3 normalizedLocalPos =
                            new Vector3(Mathf.InverseLerp(0f, terrain.terrainData.size.x, terrainLocalPos.x),
                                Mathf.InverseLerp(0f, terrain.terrainData.size.y, terrainLocalPos.y),
                                Mathf.InverseLerp(0f, terrain.terrainData.size.z, terrainLocalPos.z));
                        float[,,] hms =
                            terrain.terrainData.GetAlphamaps(
                                (int)(normalizedLocalPos.x * (float)(terrain.terrainData.alphamapWidth - 1)),
                                (int)(normalizedLocalPos.z * (float)(terrain.terrainData.alphamapHeight - 1)), 1,
                                1);
                        if (hms.GetLength(2) - 1 < m_critSelectedTextureIdx)
                        {
                            return false;
                        }
                        if (hms[0, 0, m_critSelectedTextureIdx] < m_critMinTextureStrength ||
                            hms[0, 0, m_critSelectedTextureIdx] > m_critMaxTextureStrength)
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check a bounded location to see if it can be spawned in - assumes crit check type is bounds
        /// </summary>
        /// <param name="location">The location the be checked</param>
        /// <param name="yRotation">The Y rotation of the prototype be checked</param>
        /// <param name="prototype">The prototype that has been selected</param>
        /// <param name="extents">The extents of the area to be checked</param>
        /// <param name="visualising">If this is a check for visualisation - will lower the resolution and cpu overhead</param>
        /// <returns>True if the location is ok to spawn in</returns>
        private bool CheckBoundedLocationForSpawn(Vector3 location, float yRotation, Prototype prototype, Vector3 extents, bool visualising)
        {
            //First things first - do an extents based tree collision check - will bypass a bunch of other expensive operations
            if (m_spawnOriginIsTerrain)
            {
                //Use the tree manager to do approximate hits
                if (m_treeManager.Count(location, Mathf.Max(extents.x, extents.z)) > 0)
                {
                    return false;
                }
            }

            Quaternion rotation = Quaternion.Euler(0f, yRotation + prototype.m_forwardRotation, 0f);
            //Account for the pivot of the prototype(or rather its top resource)
            location = RotatePointAroundPivot(location + prototype.NextBoundsCenter, location, rotation);
#if GENA_DEBUG && UNITY_EDITOR
            Vector3 debugLocation = location;
#endif
            location.y += m_advSpawnCheckOffset;

            //Use BoxCast for the bounds
            //Lift the ray check point slightly as we may already be on the ground
            RaycastHit hitInfo;
            if (Physics.BoxCast(location, extents, Vector3.down, out hitInfo, rotation, m_advSpawnCheckOffset+1000f, prototype.m_critSpawnCollisionLayers))
            {
                //Do the height test
                if (prototype.m_critCheckHeight == true)
                {
                    if (hitInfo.point.y < prototype.m_critMinHeight || hitInfo.point.y > prototype.m_critMaxHeight)
                    {
#if GENA_DEBUG && UNITY_EDITOR
                        Resource.DebugBounds(debugLocation, extents, rotation, Color.red);
#endif
                        return false;
                    }
                }

                //Do the slope test
                if (prototype.m_critCheckSlope == true)
                {
                    float slope = Vector3.Angle(Vector3.up, hitInfo.normal);
                    if (slope < prototype.m_critMinSlope || slope > prototype.m_critMaxSlope)
                    {
#if GENA_DEBUG && UNITY_EDITOR
                        Resource.DebugBounds(debugLocation, extents, rotation, Color.red);
#endif
                        return false;
                    }
                }

                //Get the terrain if we are a hit on terrain, and store it for later
                Terrain terrain = null;
                if (hitInfo.collider is TerrainCollider)
                {
                    terrain = hitInfo.transform.GetComponent<Terrain>();
                }

                //If the original was a terrain, then we must have any terrain, or fail
                if (m_spawnOriginIsTerrain)
                {
                    if (terrain == null)
                    {
#if GENA_DEBUG && UNITY_EDITOR
                        Resource.DebugBounds(debugLocation, extents, rotation, Color.red);
#endif
                        return false;
                    }
                }
                //Otherwise we have to hit the original object to be considered virgin
                else
                {
                    //If we hit something else then we are not virgin, except when its terrain, as we want to spawn across all terrains
                    if (hitInfo.transform.GetInstanceID() != m_spawnOriginObjectID)
                    {
#if GENA_DEBUG && UNITY_EDITOR
                        Resource.DebugBounds(debugLocation, extents, rotation, Color.red);
#endif
                        return false;
                    }
                }

                //Check mask
                bool checkMask = prototype.m_disableCritCheckMask ? false : m_critCheckMask;
                if (checkMask == true && m_critMaskType == Constants.MaskType.Image && prototype != null && prototype.m_constrainWithinMaskedBounds)
                {
                    if (m_critMaskImageData != null && m_critMaskImageData.HasData())
                    {
                        //Need to rotate the image mask to remain consistent with overall spawn rotation
                        Vector3 newLocation = RotatePointAroundPivot(hitInfo.point, m_spawnOriginLocation, new Vector3(0f, 180f - yRotation, 0f));
                        float xN = ((m_spawnOriginLocation.x - newLocation.x) / m_maxSpawnRange) + 0.5f;
                        float zN = ((m_spawnOriginLocation.z - newLocation.z) / m_maxSpawnRange) + 0.5f;

                        //Drop out if out of bounds
                        if (xN < 0f || xN >= 1f || zN < 0f || zN > 1f)
                        {
                            return false;
                        }

                        float v = m_critMaskImageData[xN, zN];
                        Color c = new Color();
                        c.b = v % 1000f;
                        v -= c.b;
                        v /= 1000f;
                        c.b /= 255f;
                        c.g = v % 1000f;
                        v -= c.g;
                        v /= 1000f;
                        c.g /= 255f;
                        c.r = v;
                        c.r /= 255f;

                        //Check the colour match
                        if (RGBDifference(c, prototype.m_imageFilterColour) > ((1f - prototype.m_imageFilterFuzzyMatch) * 100f))
                        {
                            return false;
                        }

                        //Check the alpha success match
                        if (prototype.m_successOnMaskedAlpha == true)
                        {
                            float hitAlpha = m_critMaskAlphaData[xN, zN];
                            if (!prototype.m_invertMaskedAlpha)
                            {
                                if (ApproximatelyEqual(hitAlpha, 0f))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (ApproximatelyEqual(1f - hitAlpha, 0f))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }

                //Do the texture test
                if (prototype.m_critCheckTextures == true && m_critSelectedTextureIdx >= 0)
                {
                    //Introduce a random sampling for performance
                    if (terrain != null && UnityEngine.Random.Range(0, 5) == 1)
                    {
                        Vector3 terrainLocalPos = terrain.transform.InverseTransformPoint(hitInfo.point);
                        Vector3 normalizedLocalPos =
                            new Vector3(Mathf.InverseLerp(0f, terrain.terrainData.size.x, terrainLocalPos.x),
                                Mathf.InverseLerp(0f, terrain.terrainData.size.y, terrainLocalPos.y),
                                Mathf.InverseLerp(0f, terrain.terrainData.size.z, terrainLocalPos.z));
                        float[,,] hms =
                            terrain.terrainData.GetAlphamaps(
                                (int)(normalizedLocalPos.x * (float)(terrain.terrainData.alphamapWidth - 1)),
                                (int)(normalizedLocalPos.z * (float)(terrain.terrainData.alphamapHeight - 1)), 1,
                                1);
                        if (hms.GetLength(2) - 1 < m_critSelectedTextureIdx)
                        {
#if GENA_DEBUG && UNITY_EDITOR
                            Resource.DebugBounds(debugLocation, extents, rotation, Color.red);
#endif
                            return false;
                        }
                        if (hms[0, 0, m_critSelectedTextureIdx] < prototype.m_critMinTextureStrength ||
                            hms[0, 0, m_critSelectedTextureIdx] > prototype.m_critMaxTextureStrength)
                        {
#if GENA_DEBUG && UNITY_EDITOR
                            Resource.DebugBounds(debugLocation, extents, rotation, Color.red);
#endif
                            return false;
                        }
                    }
                }

#if GENA_DEBUG && UNITY_EDITOR
                Resource.DebugBounds(debugLocation, extents, rotation, Color.green);
#endif
                //All ok if we got here
                return true;
            }

#if GENA_DEBUG && UNITY_EDITOR
            Resource.DebugBounds(debugLocation, extents, rotation, Color.grey);
#endif
            //Nothing hit
            return false;
        }

        /// <summary>
        /// Used when spawning using gravity feature
        /// </summary>
        private void LateUpdate()
        {
            if (m_gravity != null)
            {
                if ((DateTime.Now - m_lastUpdated).TotalSeconds > 2)
                {
                    m_lastUpdated = DateTime.Now;
                    m_gravity.UpdateInstances();
                }
            }
        }

#endregion

#region Unspawning methods

        /// <summary>
        /// Cleanup process that destroys a created spawn parent if it's empty.
        /// </summary>
        /// <param name="parent">The parent to cleanup.</param>
        public void UnspawnParentIfEmpty(GameObject parent)
        {
            if (parent == null)
            {
                return;
            }

            if (parent.transform.childCount < 1)
            {
                m_parentsUndoList.Remove(parent);
                DestroyImmediate(parent);
            }
        }

        /// <summary>
        /// Cleanup process that checks the created spawn parents and destorys the empty ones.
        /// </summary>
        public void UnspawnEmptyParents()
		{
            List<GameObject> newList = new List<GameObject>();

			foreach (var parent in m_parentsUndoList)
            {
                if (parent == null)
                {
                    continue;
                }

                if (parent.transform.childCount < 1)
                {
                    DestroyImmediate(parent);
                    continue;
                }

                newList.Add(parent);
            }

            m_parentsUndoList = newList;
        }

        /// <summary>
        /// Cleanup process that checks the created spawn parents and destorys the empty ones.
        /// </summary>
        public void UnspawnProbes()
        {
            //Delete all probe GOs that were created
            foreach (var probe in m_probeUndoList)
            {
                DestroyImmediate(probe);
            }
            m_probeManager.Clear();
            m_probeUndoList.Clear();
        }

        /// <summary>
        /// Unspawn every resource referred to by this spawner
        /// </summary>
        public void UnspawnAllPrefabs()
        {
            //Clear the undo redo stacks
            ClearUndoStack();

            m_randomGenerator.Reset();

            //Delete all the game objects in the prefab list
            foreach (var go in m_prefabUndoList)
            {
                DestroyImmediate(go);
            }
            m_prefabUndoList.Clear();
            UnspawnProbes();

            //Delete all the parents that were created
            foreach (var parent in m_parentsUndoList)
			{
				DestroyImmediate(parent);
			}
			m_parentsUndoList.Clear();

			foreach (var prototype in m_spawnPrototypes)
            {
                //Skip if not prefab
                if (prototype.m_resourceType != Constants.ResourceType.Prefab)
                {
                    continue;
                }

                //Hard code a counter reset
                m_instancesSpawned -= prototype.m_instancesSpawned;
                prototype.m_instancesSpawned = 0;
                foreach (var resource in prototype.m_resourceTree)
                {
                    resource.ResetInstancesSpawned();
                }
            }

            //Now call this on all the children
            foreach (Spawner spawner in m_childSpawners)
            {
                if (spawner != null && spawner.gameObject.activeInHierarchy)
                {
                    spawner.UnspawnAllPrefabs();
                }
            }
        }

        /// <summary>
        /// Unspawn all instances of this game obtect type from the scene
        /// </summary>
        /// <param name="protoIdx">Zero based index of the referenced game object</param>
        public void UnspawnGameObject(int protoIdx)
        {
            if (protoIdx < 0)
            {
                return;
            }
            Prototype proto = m_spawnPrototypes[protoIdx];

            //Make sure we are in right place
            if (proto.m_resourceType != Constants.ResourceType.Prefab)
            {
                return;
            }


#if UNITY_EDITOR
            //Clear the undo redo stacks
            ClearUndoStack();

            List<GameObject> newList = new List<GameObject>();
            GameObject prefabParent;

            foreach (var resource in proto.m_resourceTree)
            {
                if (resource.m_resourceType != Constants.ResourceType.Prefab)
                {
                    continue;
                }

                for (int i = 0; i < m_prefabUndoList.Count; i++)
                {
                    // In case it's corrupted data
                    if (m_prefabUndoList[i] == null)
                    {
                        continue;
                    }

                    bool prefabDeleted = false;

                    prefabParent = GetPrefabAsset(m_prefabUndoList[i]);
                    if (prefabParent != null)
                    {
                        if (prefabParent.GetInstanceID() == resource.m_prefab.GetInstanceID())
                        {
                            DestroyImmediate(m_prefabUndoList[i]);
                            proto.m_instancesSpawned -= resource.m_instancesSpawned;
                            m_instancesSpawned -= resource.m_instancesSpawned;
                            prefabDeleted = true;
                        }
                    }
                    // This still could be a container
                    else
                    {
                        if (m_prefabUndoList[i].name.StartsWith("_Sp_" + resource.m_name + proto.IdCode))
                        {
                            DestroyImmediate(m_prefabUndoList[i]);
                            proto.m_instancesSpawned -= resource.m_instancesSpawned;
                            m_instancesSpawned -= resource.m_instancesSpawned;
                            prefabDeleted = true;
                        }
                    }

                    if (!prefabDeleted)
                    {
                        newList.Add(m_prefabUndoList[i]);
                    }
                }
                resource.ResetInstancesSpawned();
            }

            m_prefabUndoList = newList;

            UnspawnEmptyParents();
#endif
        }

        /// <summary>
        /// Unspawn all instances of this grass from the terrains in the scene
        /// </summary>
        /// <param name="proto">The prototype of the referenced grass object</param>
        public void UnspawnGrass(int protoIdx)
        {
            if (protoIdx < 0)
            {
                return;
            }
            Prototype proto = m_spawnPrototypes[protoIdx];

            //Clear the undo redo stacks
            ClearUndoStack();

            //Make sure we are in right place
            if (proto.m_resourceType != Constants.ResourceType.TerrainGrass)
            {
                return;
            }
            Resource res = proto.m_resourceTree[0];
            foreach (var terrain in Terrain.activeTerrains)
            {
                terrain.terrainData.SetDetailLayer(0, 0, res.m_terrainProtoIdx, new int[terrain.terrainData.detailWidth, terrain.terrainData.detailWidth]);
            }
            proto.m_instancesSpawned -= res.m_instancesSpawned;
            m_instancesSpawned -= res.m_instancesSpawned;            
            res.ResetInstancesSpawned();
        }

        /// <summary>
        /// Unspawn all instances of this tree from the terrains in the scene
        /// </summary>
        /// <param name="proto">The prototype of the referenced tree object</param>
        public void UnspawnTree(int protoIdx)
        {
            if (protoIdx < 0)
            {
                return;
            }
            Prototype proto = m_spawnPrototypes[protoIdx];

            //Clear the undo redo stacks
            ClearUndoStack();

            //Make sure we are in right place
            if (proto.m_resourceType != Constants.ResourceType.TerrainTree)
            {
                return;
            }
            TreeInstance instance;
            List<TreeInstance> trees = new List<TreeInstance>();
            Resource res = proto.m_resourceTree[0];
            foreach (var terrain in Terrain.activeTerrains)
            {
                for (int idx = 0; idx < terrain.terrainData.treeInstances.Length; idx++)
                {
                    instance = terrain.terrainData.treeInstances[idx];
                    if (instance.prototypeIndex != res.m_terrainProtoIdx)
                    {
                        trees.Add(instance);
                    }
                }
                terrain.terrainData.treeInstances = trees.ToArray();
                trees.Clear();
            }
            proto.m_instancesSpawned -= res.m_instancesSpawned;
            m_instancesSpawned -= res.m_instancesSpawned;
            res.ResetInstancesSpawned();
        }

#endregion

#region General helper methods

        /// <summary>
        /// Handy random functions
        /// </summary>
        /// <param name="value"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        private float JitterAsPct(float value, float percent)
        {
            return m_randomGenerator.Next(Mathf.Clamp01(percent) * value, value);
        }

        /// <summary>
        /// Handy random functions
        /// </summary>
        /// <param name="value"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        private float JitterAround(float value, float delta)
        {
            return m_randomGenerator.Next(value - delta, value + delta);
        }

        /// <summary>
        /// Scale the suplied vector to the nearest integer number, same number if already there
        /// </summary>
        /// <param name="sourceScale"></param>
        /// <returns></returns>
        public static Vector3 ScaleToNearestInt(Vector3 sourceScale)
        {
            float x = sourceScale.x;
            float y = sourceScale.y;
            float z = sourceScale.z;

            if (x - Mathf.Floor(x) < Mathf.Ceil(x) - x)
            {
                x = Mathf.Floor(x);
            }
            else
            {
                x = Mathf.Ceil(x);
            }
            if (x < 1f)
            {
                x = 1f;
            }

            if (y - Mathf.Floor(y) < Mathf.Ceil(y) - y)
            {
                y = Mathf.Floor(y);
            }
            else
            {
                y = Mathf.Ceil(y);
            }
            if (y < 1f)
            {
                y = 1f;
            }

            if (z - Mathf.Floor(z) < Mathf.Ceil(z) - z)
            {
                z = Mathf.Floor(z);
            }
            else
            {
                z = Mathf.Ceil(z);
            }
            if (z < 1f)
            {
                z = 1f;
            }

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Combine meshes 
        /// </summary>
        /// <param name="go"></param>
        private void CombineMeshes(GameObject go)
        {
            //Zero transformation is needed because of localToWorldMatrix transform
            Vector3 position = go.transform.position;
            go.transform.position = Vector3.zero;

            //whatever man
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
                i++;
            }
            if (go.transform.GetComponent<MeshFilter>() == null)
            {
                go.AddComponent<MeshFilter>();
            }
            go.transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            //obj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
            go.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine, true, true);
            go.transform.gameObject.SetActive(true);

            //Reset position
            go.transform.position = position;

            //Adds collider to mesh
            go.AddComponent<MeshCollider>();
        }

        /// <summary>
        /// Get the terrain that matches this location, otherwise return null
        /// </summary>
        /// <param name="location">Location to check in world units</param>
        /// <returns>Terrain here or null</returns>
        private Terrain GetTerrain(Vector3 location)
        {
            Terrain terrain;
            Vector3 terrainMin = new Vector3();
            Vector3 terrainMax = new Vector3();

            //First check active terrain - most likely already selected
            terrain = Terrain.activeTerrain;
            if (terrain != null)
            {
                terrainMin = terrain.GetPosition();
                terrainMax = terrainMin + terrain.terrainData.size;
                if (location.x >= terrainMin.x && location.x <= terrainMax.x)
                {
                    if (location.z >= terrainMin.z && location.z <= terrainMax.z)
                    {
                        return terrain;
                    }
                }
            }

            //Then check rest of terrains
            for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
            {
                terrain = Terrain.activeTerrains[idx];
                terrainMin = terrain.GetPosition();
                terrainMax = terrainMin + terrain.terrainData.size;
                if (location.x >= terrainMin.x && location.x <= terrainMax.x)
                {
                    if (location.z >= terrainMin.z && location.z <= terrainMax.z)
                    {
                        return terrain;
                    }
                }
            }
            return null;
		}

		/// <summary>
		/// Get the bounds of a terrain
		/// </summary>
		/// <param name="terrain">The terrain</param>
		/// <returns>Bounds of selected terrain or null if invalid for some reason</returns>
		private bool GetTerrainBounds(Terrain terrain, ref Bounds bounds)
		{
			//if (terrain == null)
			//{
			//	return false;
			//}
			bounds.center = terrain.transform.position;
			bounds.size = terrain.terrainData.size;
			bounds.center += bounds.extents;
			return true;
		}

		/// <summary>
		/// Get the bounds of the terrain at this location or fail with a null
		/// </summary>
		/// <param name="location">Location to check and get terrain for</param>
		/// <returns>Bounds of selected terrain or null if invalid for some reason</returns>
		private bool GetTerrainBounds(Vector3 location, ref Bounds bounds)
        {
            Terrain terrain = GetTerrain(location);
            if (terrain == null)
            {
                return false;
            }
            bounds.center = terrain.transform.position;
            bounds.size = terrain.terrainData.size;
            bounds.center += bounds.extents;
            return true;
        }

        /// <summary>
        /// Get the bounds of the object supplied or fail with a null
        /// </summary>
        /// <param name="go">The object to check and get bounds for</param>
        /// <returns>Bounds of selected object or null if invalid for some reason</returns>
        private bool GetObjectBounds(GameObject go, ref Bounds bounds)
        {
            if (go == null)
            {
                return false;
            }
            bounds.center = go.transform.position;
            bounds.size = Vector3.zero;
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(r.bounds);
            }
            foreach (Collider c in go.GetComponentsInChildren<Collider>())
            {
                bounds.Encapsulate(c.bounds);
            }
            return true;
        }

        /// <summary>
        /// Return true if the values are approximately equal
        /// </summary>
        /// <param name="a">Parameter A</param>
        /// <param name="b">Parameter B</param>
        /// <returns>True if approximately equal</returns>
        public static bool ApproximatelyEqual(float a, float b, float delta = float.Epsilon)
        {
            if (a == b || Mathf.Abs(a - b) < delta)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// Rotate the point around the pivot - used to handle rotation
        /// </summary>
        /// <param name="point">Point to move</param>
        /// <param name="pivot">Pivot</param>
        /// <param name="angle">Angle to pivot</param>
        /// <returns>New location</returns>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
        {
            return RotatePointAroundPivot(point, pivot, Quaternion.Euler(angle));
        }

        /// Rotate the point around the pivot - used to handle rotation
        /// </summary>
        /// <param name="point">Point to move</param>
        /// <param name="pivot">Pivot</param>
        /// <param name="angle">Angle to pivot</param>
        /// <returns>New location</returns>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
        {
            Vector3 dir = point - pivot;
            dir = angle * dir;
            point = dir + pivot;
            return point;
        }

        #endregion

#region Editor Only helper methods
#if UNITY_EDITOR

        /// <summary>
        /// Returns the prefab asset of a GameObject or null if none found. ONLY in Unity Editor.
        /// </summary>
        public static T GetPrefabAsset<T>(T @object) where T : UnityEngine.Object
        {
            T prefab;
#if UNITY_2018_2_OR_NEWER
            if (PrefabUtility.GetPrefabAssetType(@object) == PrefabAssetType.Variant)
            {
                prefab = PrefabUtility.GetCorrespondingObjectFromSourceAtPath<T>(@object, AssetDatabase.GetAssetPath(@object));
                if (prefab == null)
                {
                    prefab = PrefabUtility.GetCorrespondingObjectFromSource<T>(@object);
                }
            }
            else
            {
                prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource<T>(@object);
            }
#else
            prefab = PrefabUtility.GetPrefabParent(@object) as T;
#endif
            return prefab;
        }

        /// <summary>
        /// Checks if the provided GameObject is a Prefab Asses or Instance
        /// </summary>
        public static bool IsPrefab(GameObject go)
        {
#if UNITY_2018_3_OR_NEWER
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(go);
            return (prefabType == PrefabAssetType.Regular ||
                prefabType == PrefabAssetType.Model ||
                prefabType == PrefabAssetType.Variant);
#else
            PrefabType prefabType = PrefabUtility.GetPrefabType(go);
            return (prefabType == PrefabType.Prefab ||
                prefabType == PrefabType.ModelPrefab ||
                prefabType == PrefabType.PrefabInstance ||
                prefabType == PrefabType.ModelPrefabInstance);
#endif
        }

        /// <summary>
        /// Returns the asset name (without extension) for a given path.
        /// </summary>
        public static string GetAssetName(string path)
        {
            if (string.IsNullOrEmpty(path) == false)
            {
                string[] filename = System.IO.Path.GetFileName(path).Split('.');
                if (filename.Length == 2)
                {
                    return filename[0];
                }
            }

            Debug.LogErrorFormat("Unable to determine prefab filename for path '{0}'", path);
            return null;
        }

#endif
        #endregion

        #region Colour conversion utilties used by image masking

        /// <summary>
        /// Calculate the CIE76 difference between two colours
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private float RGBDifference(Color c1, Color c2)
        {
            if (ApproximatelyEqual(c1.r, c2.r) && ApproximatelyEqual(c1.g, c2.g) && ApproximatelyEqual(c1.b, c2.b))
            {
                return 0f;
            }
            Vector3 l1 = RGBtoLAB(c1);
            Vector3 l2 = RGBtoLAB(c2);
            float sum = 0f;
            sum += Mathf.Pow(l1.x - l2.x, 2f);
            sum += Mathf.Pow(l1.y - l2.y, 2f);
            sum += Mathf.Pow(l1.z - l2.z, 2f);
            return Mathf.Max(Mathf.Min(Mathf.Sqrt(sum), 100f), 0f);
        }

        /// <summary>
        /// Convert rgb to lab colour
        /// </summary>
        /// <param name="c">Source colour to convert</param>
        /// <returns>Lab colour x = L, y = a, z = b</returns>
        private Vector3 RGBtoLAB(Color c)
        {
            return XYZtoLAB(RGBtoXYZ(c));
        }

        /// <summary>
        /// Conver RGB to XYZ colour
        /// </summary>
        /// <param name="c">Source colour to convert</param>
        /// <returns>Source colour as xyz colour</returns>
        private Vector3 RGBtoXYZ(Color c)
        {
            // Based on http://www.easyrgb.com/index.php?X=MATH&H=02
            float R = c.r;
            float G = c.g;
            float B = c.b;

            if (R > 0.04045f)
                R = Mathf.Pow(((R + 0.055f) / 1.055f), 2.4f);
            else
                R = R / 12.92f;
            if (G > 0.04045f)
                G = Mathf.Pow(((G + 0.055f) / 1.055f), 2.4f);
            else
                G = G / 12.92f;
            if (B > 0.04045f)
                B = Mathf.Pow(((B + 0.055f) / 1.055f), 2.4f);
            else
                B = B / 12.92f;

            R *= 100f;
            G *= 100f;
            B *= 100f;

            // Observer. = 2°, Illuminant = D65
            float X = R * 0.4124f + G * 0.3576f + B * 0.1805f;
            float Y = R * 0.2126f + G * 0.7152f + B * 0.0722f;
            float Z = R * 0.0193f + G * 0.1192f + B * 0.9505f;
            return new Vector3(X, Y, Z);
        }

        /// <summary>
        /// Convert XYZ colour to LAB colour, where resulting x = L, y = a, z = b
        /// </summary>
        /// <param name="c">Source xyz colour</param>
        /// <returns>LAB colour where x = L, y = a, z = b</returns>
        private Vector3 XYZtoLAB(Vector3 c)
        {
            // Based on http://www.easyrgb.com/index.php?X=MATH&H=07
            float ref_Y = 100f;
            float ref_Z = 108.883f;
            float ref_X = 95.047f; // Observer= 2°, Illuminant= D65
            float Y = c.y / ref_Y;
            float Z = c.z / ref_Z;
            float X = c.x / ref_X;
            if (X > 0.008856f)
                X = Mathf.Pow(X, 1f / 3f);
            else
                X = (7.787f * X) + (16f / 116f);
            if (Y > 0.008856)
                Y = Mathf.Pow(Y, 1f / 3f);
            else
                Y = (7.787f * Y) + (16f / 116f);
            if (Z > 0.008856f)
                Z = Mathf.Pow(Z, 1f / 3f);
            else
                Z = (7.787f * Z) + (16f / 116f);
            float L = (116f * Y) - 16f;
            float a = 500f * (X - Y);
            float b = 200f * (Y - Z);
            return new Vector3(L, a, b);
        }
#endregion

#region Gizmos

        /// <summary>
        /// Draw our gizmos
        /// </summary>
        void OnDrawGizmosSelected()
        {
            //Exit if we dont want to display gizmos
            if (m_showGizmos != true)
            {
                return;
            }

            //Exit if we have not chosen an origin yet
            if (m_spawnOriginLocation == Vector3.zero)
            {
                return;
            }

            if (VisualizationActive)
            {
                //Calculate steps and work out if we need to recalculate
                float halfSpawnRange = m_maxSpawnRange / 2f;
                int dimensions = (int)m_maxSpawnRange + 1;
                if (m_needsVisualisationUpdate == true)
                {
                    UpdateSpawnerVisualisation();
                }
                if (dimensions > m_maxVisualisationDimensions)
                {
                    dimensions = m_maxVisualisationDimensions + 1;
                }
                if (dimensions != m_fitnessArray.GetLength(0))
                {
                    UpdateSpawnerVisualisation();
                }
                float stepIncrement = m_maxSpawnRange / ((float)dimensions - 1f);

                //Now draw it
                int x, z;
                Vector3 position = Vector3.zero;
                Vector3 maxPosition = m_visualizationLocation + (Vector3.one * halfSpawnRange);

                Gizmos.color = new Color(0f, 255f, 0f, 1f);

                for (x = 0, position.x = m_visualizationLocation.x - halfSpawnRange;
                    position.x < maxPosition.x;
                    x++, position.x += stepIncrement)
                {
                    for (z = 0, position.z = m_visualizationLocation.z - halfSpawnRange;
                        position.z < maxPosition.z;
                        z++, position.z += stepIncrement)
                    {
                        position.y = m_fitnessArray[x, z];
                        if (position.y > float.MinValue)
                        {
                            Gizmos.DrawSphere(position, stepIncrement / 6f);
                        }
                    }
                } 
            }

            //If we are doing min/max height, then show them as well
            if (m_critCheckHeightType > Constants.CriteriaRangeType.Range && (m_showCritMinSpawnHeight == true || m_showCritMaxSpawnHeight == true))
            {
                Bounds bounds = new Bounds();

				foreach (Terrain terrain in Terrain.activeTerrains)
				{
					if (GetTerrainBounds(terrain, ref bounds) == true)
					{
						bounds.size = new Vector3(bounds.size.x, 0.05f, bounds.size.z);

						if (m_showCritMinSpawnHeight == true)
						{
							bounds.center = new Vector3(bounds.center.x, m_critMinSpawnHeight, bounds.center.z);
							Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
							Gizmos.DrawCube(bounds.center, bounds.size);
						}

						if (m_showCritMaxSpawnHeight == true)
						{
							bounds.center = new Vector3(bounds.center.x, m_critMaxSpawnHeight, bounds.center.z);
							Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
							Gizmos.DrawCube(bounds.center, bounds.size);
						}
					}
				}
			}
		}

        #endregion

        #region Serialisation

        /// <summary>
        /// OnBeforeSerialize
        /// </summary>
        public void OnBeforeSerialize()
        {
            _linkedSpawnersIx = (m_linkedSpawners != null) ? new List<int>(m_linkedSpawners.Keys) : new List<int>();
            _linkedSpawnersCount = new List<int>();
            for (int i = 0; i < _linkedSpawnersIx.Count; i++)
            {
                _linkedSpawnersCount.Add(m_linkedSpawners[_linkedSpawnersIx[i]]);
            }
        }

        /// <summary>
        /// OnAfterDeserialize
        /// </summary>
        public void OnAfterDeserialize()
        {
            m_linkedSpawners = new Dictionary<int, int>();

            for (int i = 0; i < _linkedSpawnersIx.Count; i++)
            {
                m_linkedSpawners[_linkedSpawnersIx[i]] = _linkedSpawnersCount[i];
            }            
        }

        #endregion
    }
}
