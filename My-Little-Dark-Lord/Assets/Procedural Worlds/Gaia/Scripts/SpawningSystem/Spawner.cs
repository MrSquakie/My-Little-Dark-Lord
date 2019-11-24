using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.TerrainAPI;
using System.Linq;
using static Gaia.GaiaConstants;
using System.Text;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

namespace Gaia
{
    
    /// <summary>
    /// A generic spawning system.
    /// </summary>
    [ExecuteInEditMode]
    [System.Serializable]
    public class Spawner : MonoBehaviour
    {
        /// <summary>
        /// A class that stores spawn locations
        /// </summary>
        //private class SpawnLocation
        //{
        //    public Vector3 m_location;
        //    //public float m_seedDistance;
        //}
        [SerializeField]
        private SpawnerSettings settings;
        /// <summary>
        /// The current spawner settings
        /// </summary>
        public SpawnerSettings m_settings
        {
            get
            {
                if (settings == null)
                {
                    settings = ScriptableObject.CreateInstance<SpawnerSettings>();
                    settings.m_resources = new GaiaResource();
                    settings.m_resources.m_name = "NewResources";
                }
                return settings;
            }
            set
            {
                settings = value;
            }

        }


        /// <summary>
        /// The spawner ID
        /// </summary>
        public string m_spawnerID = Guid.NewGuid().ToString();

        /// <summary>
        /// Operational mode of the spawner
        /// </summary>
        public Gaia.GaiaConstants.OperationMode m_mode = GaiaConstants.OperationMode.DesignTime;

        /// <summary>
        /// Source for the random number generator
        /// </summary>
        public int m_seed = DateTime.Now.Millisecond;

        /// <summary>
        /// The shape of the spawner
        /// </summary>
        public Gaia.GaiaConstants.SpawnerShape m_spawnerShape = GaiaConstants.SpawnerShape.Box;

        /// <summary>
        /// The rule selection approach
        /// </summary>
        public Gaia.GaiaConstants.SpawnerRuleSelector m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.WeightedFittest;
        
        /// <summary>
        /// The type of spawner
        /// </summary>
        public Gaia.GaiaConstants.SpawnerLocation m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.RandomLocation;

        /// <summary>
        /// The type of check performed at every location
        /// </summary>
        public Gaia.GaiaConstants.SpawnerLocationCheckType m_spawnLocationCheckType = GaiaConstants.SpawnerLocationCheckType.PointCheck;

        /// <summary>
        /// The step amount used when EveryLocation is selected
        /// </summary>
        public float m_locationIncrement = 1f;

        /// <summary>
        /// The maximum random offset on a jittered location
        /// </summary>
        public float m_maxJitteredLocationOffsetPct = 0.9f;

        /// <summary>
        /// Number of times a check is made for a new spawn location every interval 
        /// </summary>
        public int m_locationChecksPerInt = 1;

        /// <summary>
        /// In seeded mode, this will be the maximum number of individual spawns in a cluster before another locaiton is chosen
        /// </summary>
        public int m_maxRandomClusterSize = 50;

        //public GaiaResource m_resources;

        /// <summary>
        /// This will allow the user to filter the relative strength of items spawned by distance from the center
        /// </summary>
        public AnimationCurve m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

        /// <summary>
        /// The image fitness filter mode to apply
        /// </summary>
        public Gaia.GaiaConstants.ImageFitnessFilterMode m_areaMaskMode = Gaia.GaiaConstants.ImageFitnessFilterMode.None;

        /// <summary>
        /// This will enable ot disable the collider cache at runtime - can be quite handy to keep them on some spawners
        /// </summary>
        public bool m_enableColliderCacheAtRuntime = false;

        /// <summary>
        /// This is used to filter the fitness based on the supplied texture, can be used in conjunction with th fitness attenuator
        /// </summary>
        public Texture2D m_imageMask;

        /// <summary>
        /// This is used to invert the fitness based on the supplied texture, can also be used in conjunction with the fitness attenuator
        /// </summary>
        public bool m_imageMaskInvert = false;

        /// <summary>
        /// This is used to normalise the fitness based on the supplied texture, can also be used in conjunction with the fitness attenuator
        /// </summary>
        public bool m_imageMaskNormalise = false;

        /// <summary>
        /// Flip the x, z of the image texture - sometimes required to match source with unity terrain
        /// </summary>
        public bool m_imageMaskFlip = false;

        /// <summary>
        /// This is used to smooth the supplied image mask texture
        /// </summary>
        public int m_imageMaskSmoothIterations = 3;

        /// <summary>
        /// The heightmap for the image filter
        /// </summary>
        [NonSerialized]
        public HeightMap m_imageMaskHM;

        /// <summary>
        /// Our noise generator
        /// </summary>
        private Gaia.FractalGenerator m_noiseGenerator;

        /// <summary>
        /// Seed for noise based fractal
        /// </summary>
        public float m_noiseMaskSeed = 0;

        /// <summary>
        /// The amount of detail in the fractal - more octaves mean more detail and longer calc time.
        /// </summary>
        public int m_noiseMaskOctaves = 8;

        /// <summary>
        /// The roughness of the fractal noise. Controls how quickly amplitudes diminish for successive octaves. 0..1.
        /// </summary>
        public float m_noiseMaskPersistence = 0.25f;

        /// <summary>
        /// The frequency of the first octave
        /// </summary>
        public float m_noiseMaskFrequency = 1f;

        /// <summary>
        /// The frequency multiplier between successive octaves. Experiment between 1.5 - 3.5.
        /// </summary>
        public float m_noiseMaskLacunarity = 1.5f;

        /// <summary>
        /// The zoom level of the noise
        /// </summary>
        public float m_noiseZoom = 10f;

        /// <summary>
        /// Invert the boise value
        /// </summary>
        public bool m_noiseInvert = false;

        /// <summary>
        /// How often the spawner should check to release new instances in seconds
        /// </summary>
        public float m_spawnInterval = 5f;

        /// <summary>
        /// The player to use for distance checks
        /// </summary>
        public string m_triggerTags = "Player";

        /// <summary>
        /// System will only iterate through spawn rules if the player / trigger object is closer than this distance
        /// </summary>
        public float m_triggerRange = 130f;

        /// <summary>
        /// Used to constrain which layers the spawner will attempt to get collisions on - used for virgin detection, terrain detection, tree detection and game object detection
        /// </summary>
        public LayerMask m_spawnCollisionLayers;

        /// <summary>
        /// Set to the terrain layer so that colliders are correctly setup
        /// </summary>
        public int m_spawnColliderLayer = 0;

        /// <summary>
        /// Whether or not to show gizmos
        /// </summary>
        public bool m_showGizmos = true;

        /// <summary>
        /// Whether or not to show debug messages
        /// </summary>
        public bool m_showDebug = false;

        /// <summary>
        /// Whether or not to show statistics
        /// </summary>
        //public bool m_showStatistics = true;

        /// <summary>
        /// Whether or not to show the terrain helper
        /// </summary>
        //public bool m_showTerrainHelper = true;

        /// <summary>
        /// Random number generator for this spawner - generates locations
        /// </summary>
        public Gaia.XorshiftPlus m_rndGenerator;

        /// <summary>
        /// Whether or not we are currently caching texures
        /// </summary>
        private bool m_cacheDetails = false;

        /// <summary>
        /// Detail map cache - used when doing area updates on details - indexed by the ID of the terrain it comes from
        /// </summary>
        private Dictionary<int, List<HeightMap>> m_detailMapCache = new Dictionary<int, List<HeightMap>>();

        /// <summary>
        /// Whether or not we are currently caching texures
        /// </summary>
        private bool m_cacheTextures = false;

        /// <summary>
        /// Set to true if the texture map is modified and needs to be written back to the terrain
        /// </summary>
        private bool m_textureMapsDirty = false;

        /// <summary>
        /// Texture map cache - used when doing area updates / reads on textures - indexed by the ID of the terrain it comes from
        /// </summary>
        private Dictionary<int, List<HeightMap>> m_textureMapCache = new Dictionary<int, List<HeightMap>>();

        /// <summary>
        /// Whether or not we are currently caching tags
        /// </summary>
        private bool m_cacheTags = false;

        /// <summary>
        /// Tagged game object cache
        /// </summary>
        private Dictionary<string, Quadtree<GameObject>> m_taggedGameObjectCache = new Dictionary<string,Quadtree<GameObject>>();

        /// <summary>
        /// Whether or not the trees are cached
        /// </summary>
        //private bool m_cacheTrees = false;

        /// <summary>
        /// Tree cache
        /// </summary>
        public TreeManager m_treeCache = new TreeManager();

        /// <summary>
        /// Whether or not we are currently caching height maps
        /// </summary>
        private bool m_cacheHeightMaps = false;

        /// <summary>
        /// Set to true if the height map is modified and needs to be written back to the terrain
        /// </summary>
        private bool m_heightMapDirty = false;

        /// <summary>
        /// Height map cache - used when doing area updates / reads on heightmaps - indexed by the ID of the terrain it comes from
        /// </summary>
        private Dictionary<int, UnityHeightMap> m_heightMapCache = new Dictionary<int, UnityHeightMap>();

        /// <summary>
        /// Whether or not we are currently caching height maps
        /// </summary>
        //private bool m_cacheStamps = false;

        /// <summary>
        /// Stamp cache - used to cache stamps when interacting with heightmaps - activated when heightmap cache is activated
        /// </summary>
        private Dictionary<string, HeightMap> m_stampCache = new Dictionary<string, HeightMap>();

        /// <summary>
        /// The sphere collider cache - used to test for area bounds
        /// </summary>
        [NonSerialized]
        public GameObject m_areaBoundsColliderCache;

        /// <summary>
        /// The game object collider cache - used to test for game object collisions
        /// </summary>
        [NonSerialized]
        public GameObject m_goColliderCache;

        /// <summary>
        /// The game object parent transform - used to make it easier to rehome spawned game objects
        /// </summary>
        [NonSerialized]
        public GameObject m_goParentGameObject;

        /// <summary>
        /// Set to true to cancel the spawn
        /// </summary>
        private bool m_cancelSpawn = false;

        /// <summary>
        /// Handy counters for statistics
        /// </summary>
        //public int m_totalRuleCnt = 0;
        //public int m_activeRuleCnt = 0;
        //public int m_inactiveRuleCnt = 0;
        //public ulong m_maxInstanceCnt = 0;
        //public ulong m_activeInstanceCnt = 0;
        //public ulong m_inactiveInstanceCnt = 0;
        //public ulong m_totalInstanceCnt = 0;

        /// <summary>
        /// Handy check results - only one check at a time will ever be performed
        /// </summary>
        private float m_terrainHeight = 0f;
        private RaycastHit m_checkHitInfo = new RaycastHit();

        /// <summary>
        /// Use for co-routine simulation
        /// </summary>
        public IEnumerator m_updateCoroutine;
        /// <summary>


        public IEnumerator m_updateCoroutine2;
        /// Amount of time per allowed update
        /// </summary>
        public float m_updateTimeAllowed = 1f / 30f; 

        /// <summary>
        /// Current status
        /// </summary>
        public float m_spawnProgress = 0f;

        /// <summary>
        /// Whether or not its completed processing
        /// </summary>
        public bool m_spawnComplete = true;

        /// <summary>
        /// The spawner bounds
        /// </summary>
        public Bounds m_spawnerBounds = new Bounds();

        /// <summary>
        /// Controls whether the spawn Preview needs to be redrawn
        /// </summary>
        public bool m_spawnPreviewDirty;

        /// <summary>
        /// The last active terrain this spawner was displayed for.
        /// </summary>
        public Terrain m_lastActiveTerrain;

        /// <summary>
        /// Cached settings that are configired during the init call
        /// </summary>
        private bool m_isTextureSpawner = false;
        private bool m_isDetailSpawner = false;
        private bool m_isTreeSpawnwer = false;
        private bool m_isGameObjectSpawner = false;
        
        private RenderTexture m_cachedPreviewHeightmapRenderTexture;
        private RenderTexture[] m_cachedPreviewColorRenderTextures = new RenderTexture[GaiaConstants.maxPreviewedTextures];

        private GaiaSettings m_gaiaSettings;

        public bool m_drawPreview = false;
        public List<int> m_previewRuleIds = new List<int>();
        public float m_maxWorldHeight;
        public float m_minWorldHeight;
        public  bool m_showSeaLevelinStampPreview = true;
        public bool m_rulePanelUnfolded;
        public bool m_createdfromBiomePreset;
        public bool m_createdFromGaiaManager;
        public bool m_showSeaLevelPlane = true;
        public bool m_showBoundingBox = true;


        /// <summary>
        /// Called by unity in editor when this is enabled - unity initialisation is quite opaque!
        /// </summary>
        void OnEnable()
        {
            if (m_gaiaSettings == null)
            {
                m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            }
            
            //Check layer mask
            if (m_spawnCollisionLayers.value == 0)
            {
                m_spawnCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();
            }

            m_spawnColliderLayer = Gaia.TerrainHelper.GetActiveTerrainLayerAsInt();

            //Create the random generator if we dont have one
            if (m_rndGenerator == null)
            {
                m_rndGenerator = new XorshiftPlus(m_seed);
            }

            //Get the min max height from the current terrain
            UpdateMinMaxHeight();
            
        }

        void OnDisable()
        {
        }

        /// <summary>
        /// Start editor updates
        /// </summary>
        public void StartEditorUpdates()
        {
            #if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
            #endif
        }

        //Stop editor updates
        public void StopEditorUpdates()
        {
            #if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
            #endif
        }

        public void UpdateMinMaxHeight()
        {
            GaiaSessionManager gsm = GaiaSessionManager.GetSessionManager();
            gsm.GetWorldMinMax(ref m_minWorldHeight, ref m_maxWorldHeight);
            float seaLevel = gsm.GetSeaLevel();
            //Iterate through all image masks and set up the current min max height
            //This is fairly important to display the height-dependent mask settings correctly
            //General spawner mask first
            foreach (ImageMask mask in m_settings.m_imageMasks)
            {
                mask.m_maxWorldHeight = m_maxWorldHeight;
                mask.m_minWorldHeight = m_minWorldHeight;
                mask.m_seaLevel = seaLevel;
            }
            //Now the individual resource masks
            for (int i = 0; i < m_settings.m_spawnerRules.Count; i++)
            {
                ImageMask[] maskStack = m_settings.m_spawnerRules[i].m_imageMasks;
                if (maskStack!=null && maskStack.Length > 0)
                {
                    foreach (ImageMask mask in maskStack)
                    {
                        mask.m_maxWorldHeight = m_maxWorldHeight;
                        mask.m_minWorldHeight = m_minWorldHeight;
                        mask.m_seaLevel = seaLevel;
                    }
                }
            }
        }

        /// <summary>
        /// This is executed only in the editor - using it to simulate co-routine execution and update execution
        /// </summary>
        void EditorUpdate()
        {
            #if UNITY_EDITOR
            if (m_updateCoroutine == null)
            {
                StopEditorUpdates();
                return;
            }
            else
            {
                if (EditorWindow.mouseOverWindow != null)
                {
                    m_updateTimeAllowed = 1 / 30f;
                }
                else
                {
                    m_updateTimeAllowed = 1 / 2f;
                }
                if (m_updateCoroutine2 != null)
                {
                    m_updateCoroutine2.MoveNext();
                }
                m_updateCoroutine.MoveNext();
               

            }
#endif
        }

        /// <summary>
        /// Use this for initialization - this will kick the spawner off 
        /// </summary>
        void Start()
        {
            //Disable the colliders
            if (Application.isPlaying)
            {
                //Disable area bounds colliders
                Transform collTrans = this.transform.Find("Bounds_ColliderCache");
                if (collTrans != null)
                {
                    m_areaBoundsColliderCache = collTrans.gameObject;
                    m_areaBoundsColliderCache.SetActive(false);
                }
                
                if (!m_enableColliderCacheAtRuntime)
                {
                    collTrans = this.transform.Find("GameObject_ColliderCache");
                    if (collTrans != null)
                    {
                        m_goColliderCache = collTrans.gameObject;
                        m_goColliderCache.SetActive(false);
                    }
                }
            }

            if (m_mode == GaiaConstants.OperationMode.RuntimeInterval || m_mode == GaiaConstants.OperationMode.RuntimeTriggeredInterval)
            {
                //Initialise the spawner
                Initialise();

                //Start spawner checks in random period of time after game start, then every check interval
                InvokeRepeating("RunSpawnerIteration", 1f, m_spawnInterval);
            }
        }

        /// <summary>
        /// Build the spawner dictionary - allows for efficient updating of instances etc based on name
        /// </summary>
        public void Initialise()
        {
            if (m_showDebug)
            {
                Debug.Log("Initialising spawner");
            }

            //Set up layer for spawner collisions
            m_spawnColliderLayer = Gaia.TerrainHelper.GetActiveTerrainLayerAsInt();

            //Destroy any children
            List<Transform> transList = new List<Transform>();
            foreach (Transform child in transform)
            {
                transList.Add(child);
            }
            foreach (Transform child in transList)
            {
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            //Set up the spawner type flags
            SetUpSpawnerTypeFlags();

            //Create the game object parent transform
            if (IsGameObjectSpawner())
            {
                m_goParentGameObject = new GameObject("Spawned_GameObjects");
                m_goParentGameObject.transform.parent = this.transform;
                m_areaBoundsColliderCache = new GameObject("Bounds_ColliderCache");
                m_areaBoundsColliderCache.transform.parent = this.transform;
                m_goColliderCache = new GameObject("GameObject_ColliderCache");
                m_goColliderCache.transform.parent = this.transform;
            }

           //Reset the random number generator
           ResetRandomGenertor();

            //Get terrain height - assume all terrains same height
            Terrain t = TerrainHelper.GetTerrain(transform.position);
            if (t != null)
            {
                m_terrainHeight = t.terrainData.size.y;    
            }

            //Set the spawner bounds
            m_spawnerBounds = new Bounds(transform.position, new Vector3(m_settings.m_spawnRange * 2f, m_settings.m_spawnRange * 2f, m_settings.m_spawnRange * 2f));

            //Update the rule counters
            foreach (SpawnRule rule in m_settings.m_spawnerRules)
            {
                rule.m_currInstanceCnt = 0;
                rule.m_activeInstanceCnt = 0;
                rule.m_inactiveInstanceCnt = 0;
            }

            //Update the counters
            UpdateCounters();
        }

        /// <summary>
        /// Call this prior to doing a Spawn to do any setup required - particularly relevant for re-constituted spanwes
        /// </summary>
        private void PreSpawnInitialise()
        {
            //Update bounds
            m_spawnerBounds = new Bounds(transform.position, new Vector3(m_settings.m_spawnRange * 2f, m_settings.m_spawnRange * 2f, m_settings.m_spawnRange * 2f));

            //Make sure random number generator is online
            if (m_rndGenerator == null)
            {
                ResetRandomGenertor();
            }
            //Debug.Log(string.Format("RNG {0} Seed = {1} State A = {2} State B = {3}", gameObject.name, m_rndGenerator.m_seed, m_rndGenerator.m_stateA, m_rndGenerator.m_stateB));

            //Set up layer for spawner collisions
            m_spawnColliderLayer = Gaia.TerrainHelper.GetActiveTerrainLayerAsInt();

            //Set up the spawner type flags
            SetUpSpawnerTypeFlags();

            //Create the game object parent transform
            if (IsGameObjectSpawner())
            {
                if (transform.Find("Spawned_GameObjects") == null)
                {
                    m_goParentGameObject = new GameObject("Spawned_GameObjects");
                    m_goParentGameObject.transform.parent = this.transform;
                }
                if (transform.Find("Bounds_ColliderCache") == null)
                {
                    m_areaBoundsColliderCache = new GameObject("Bounds_ColliderCache");
                    m_areaBoundsColliderCache.transform.parent = this.transform;
                }
                if (transform.Find("GameObject_ColliderCache") == null)
                {
                    m_goColliderCache = new GameObject("GameObject_ColliderCache");
                    m_goColliderCache.transform.parent = this.transform;
                }
            }

            //Initialise spawner themselves
            foreach (SpawnRule rule in m_settings.m_spawnerRules)
            {
                rule.Initialise(this);
            }

            //Create and initialise the noise generator
            if (m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.PerlinNoise)
            {
                m_noiseGenerator = new FractalGenerator(m_noiseMaskFrequency, m_noiseMaskLacunarity, m_noiseMaskOctaves, m_noiseMaskPersistence, m_noiseMaskSeed, FractalGenerator.Fractals.Perlin);
            }
            else if (m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.BillowNoise)
            {
                m_noiseGenerator = new FractalGenerator(m_noiseMaskFrequency, m_noiseMaskLacunarity, m_noiseMaskOctaves, m_noiseMaskPersistence, m_noiseMaskSeed, FractalGenerator.Fractals.Billow);
            }
            else if (m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.RidgedNoise)
            {
                m_noiseGenerator = new FractalGenerator(m_noiseMaskFrequency, m_noiseMaskLacunarity, m_noiseMaskOctaves, m_noiseMaskPersistence, m_noiseMaskSeed, FractalGenerator.Fractals.RidgeMulti);
            }

            //Update the counters
            UpdateCounters();
        }

        /// <summary>
        /// Caching spawner type flags
        /// </summary>
        public void SetUpSpawnerTypeFlags()
        {
            m_isDetailSpawner = false;
            for (int ruleIdx = 0; ruleIdx < m_settings.m_spawnerRules.Count; ruleIdx++)
            {
                if (m_settings.m_spawnerRules[ruleIdx].m_resourceType == GaiaConstants.SpawnerResourceType.TerrainDetail)
                {
                    m_isDetailSpawner = true;
                    break;
                }
            }

            m_isTextureSpawner = false;
            for (int ruleIdx = 0; ruleIdx < m_settings.m_spawnerRules.Count; ruleIdx++)
            {
                if (m_settings.m_spawnerRules[ruleIdx].m_resourceType == GaiaConstants.SpawnerResourceType.TerrainTexture)
                {
                    m_isTextureSpawner = true;
                    break;
                }
            }

            for (int ruleIdx = 0; ruleIdx < m_settings.m_spawnerRules.Count; ruleIdx++)
            {
                if (m_settings.m_spawnerRules[ruleIdx].m_resourceType == GaiaConstants.SpawnerResourceType.TerrainTree)
                {
                    m_isTreeSpawnwer = true;
                    break;
                }
            }

            m_isGameObjectSpawner = false;
            for (int ruleIdx = 0; ruleIdx < m_settings.m_spawnerRules.Count; ruleIdx++)
            {
                if (m_settings.m_spawnerRules[ruleIdx].m_resourceType == GaiaConstants.SpawnerResourceType.GameObject)
                {
                    m_isGameObjectSpawner = true;
                    break;
                }
            }
        }


        /// <summary>
        /// Make sure the assets are set up properly in the resources file
        /// </summary>
        public void AssociateAssets()
        {
            if (m_settings.m_resources != null)
            {
                m_settings.m_resources.AssociateAssets();
            }
            else
            {
                Debug.LogWarning("Could not associated assets for " + name + " - resources file was missing");
            }
        }

        /// <summary>
        /// Get the index of any rules that are missing resources
        /// </summary>
        /// <returns>Array of the resources that are missing</returns>
        public int[] GetMissingResources()
        {
            List<int> missingRes = new List<int>();

            //Initialise spawner themselves
            for (int ruleIdx = 0; ruleIdx < m_settings.m_spawnerRules.Count; ruleIdx++)
            {
                if (m_settings.m_spawnerRules[ruleIdx].m_isActive == true) //Only care about active resources
                {
                    if (!m_settings.m_spawnerRules[ruleIdx].ResourceIsLoadedInTerrain(this))
                    {
                        missingRes.Add(ruleIdx);
                    }
                }
            }

            return missingRes.ToArray();
        }

        /// <summary>
        /// Add the resources related to the ruels passed in into the terrain if they are not already there
        /// </summary>
        /// <param name="rules">Index of rules with resources that should be added to the terrain</param>
        public void AddResourcesToTerrain(int [] rules)
        {
            for (int ruleIdx = 0; ruleIdx < rules.GetLength(0); ruleIdx++)
            {
                if (!m_settings.m_spawnerRules[rules[ruleIdx]].ResourceIsLoadedInTerrain(this))
                {
                    m_settings.m_spawnerRules[rules[ruleIdx]].AddResourceToTerrain(this);
                }
            }
        }

        /// <summary>
        /// Call this at the end of a spawn
        /// </summary>
        private void PostSpawn()
        {
            //Signal that everything has stopped
            m_spawnProgress = 0f;
            m_spawnComplete = true;
            m_updateCoroutine = null;

            //Update the counters
            UpdateCounters();
        }

        /// <summary>
        /// Return true if this spawner spawns textures
        /// </summary>
        /// <returns>True if we spawn textures</returns>
        public bool IsTextureSpawner()
        {
            return m_isTextureSpawner;
        }

        /// <summary>
        /// Return true if this spawner spawns details
        /// </summary>
        /// <returns>True if we spawn details</returns>
        public bool IsDetailSpawner()
        {
            return m_isDetailSpawner;
        }

        /// <summary>
        /// Return true if this spawner spawns trees
        /// </summary>
        /// <returns>True if we spawn trees</returns>
        public bool IsTreeSpawner()
        {
            return m_isTreeSpawnwer;
        }

        /// <summary>
        /// Return true if this spawner spawns game objects
        /// </summary>
        /// <returns>True if we spawn game objects</returns>
        public bool IsGameObjectSpawner()
        {
            return m_isGameObjectSpawner;
        }

        /// <summary>
        /// Reste the spawner and delete everything it points to
        /// </summary>
        public void ResetSpawner()
        {
            Initialise();
        }

        /// <summary>
        /// Cause any active spawn to cancel itself
        /// </summary>
        public void CancelSpawn()
        {
            m_cancelSpawn = true;
            m_spawnComplete = true;
            m_spawnProgress = 0f;
            GaiaUtils.ClearProgressBarNoEditor();
        }

        /// <summary>
        /// Returns true if we are currently in process of spawning
        /// </summary>
        /// <returns>True if spawning, false otherwise</returns>
        public bool IsSpawning()
        {
            return (m_spawnComplete != true);
        }

        /// <summary>
        /// Check to see if this spawner can spawn instances
        /// </summary>
        /// <returns>True if it can spawn instances, false otherwise</returns>
        private bool CanSpawnInstances()
        {
            SpawnRule rule;
            bool canSpawnInstances = false;
            for (int ruleIdx = 0; ruleIdx < m_settings.m_spawnerRules.Count; ruleIdx++)
            {
                rule = m_settings.m_spawnerRules[ruleIdx];
                if (rule.m_isActive)
                {
                    if (rule.m_ignoreMaxInstances)
                    {
                        return true;
                    }

                    if (rule.m_activeInstanceCnt < rule.m_maxInstances)
                    {
                        return true;
                    }
                }
            }
            return canSpawnInstances;
        }


        public void DrawSpawnerPreview()
        {
            if (m_drawPreview)
            {

                //early out if no preview rule is active 
                bool foundActive = false;
                for (int i = 0; i < m_previewRuleIds.Count; i++)
                {
                    if (m_settings.m_spawnerRules[m_previewRuleIds[i]].m_isActive)
                    {
                        foundActive = true;
                    }
                }
                if (!foundActive)
                {
                    return;
                }

                //Set up a multi-terrain operation once, all rules can then draw from the data collected here
                Terrain currentTerrain = GetCurrentTerrain();
                GaiaMultiTerrainOperation operation = new GaiaMultiTerrainOperation(currentTerrain, transform, m_settings.m_spawnRange * 2f);
                operation.GetHeightmap();

                //only re-generate all textures etc. if settings have changed and the preview is dirty, otherwise we can just use the cached textures
                if (m_spawnPreviewDirty == true)
                {
                    //To get a combined preview of different textures on a single mesh we need one color texture each per previewed 
                    // rule to determine the color areas on the heightmap mesh
                    // We need to iterate over the rules that are previewed, and build those color textures in this process
                    
                    //Get additional op data (required for certain image masks)
                    operation.GetNormalmap();
                    operation.CollectTerrainCollisions();
                    //Preparing a simple add operation on the image mask shader for the combined heightmap texture
                    //Material filterMat = new Material(Shader.Find("Hidden/Gaia/FilterImageMask"));
                    //filterMat.SetFloat("_Strength", 1f);
                    //filterMat.SetInt("_Invert", 0);

                    //Store the currently active render texture here before we start manipulating
                    RenderTexture currentRT = RenderTexture.active;

                    //Clear texture cache first
                    ClearColorTextureCache();

                    //bool firstActiveRule = true;

                    for (int i=0; i< m_previewRuleIds.Count;i++)
                    {
     
                        if (m_settings.m_spawnerRules[m_previewRuleIds[i]].m_isActive)
                        {
                            //Initialise our color texture cache at this index with this context
                            InitialiseColorTextureCache(i, operation.RTheightmap);
                            //Store result for this rule in our cache render texture array
                            Graphics.Blit(ApplyBrush(operation, MultiTerrainOperationType.Heightmap, m_previewRuleIds[i]), m_cachedPreviewColorRenderTextures[i]);
                            RenderTexture.active = currentRT;
                        }
                    }

                    //Everything processed, preview not dirty anymore
                    m_spawnPreviewDirty = false;
                }

                //Now draw the preview according to the cached textures
                Material material = GaiaMultiTerrainOperation.GetDefaultGaiaSpawnerPreviewMaterial();
                material.SetInt("_zTestMode", (int)UnityEngine.Rendering.CompareFunction.Always);

                //assign all color textures in the material
                for (int i = 0; i < m_cachedPreviewColorRenderTextures.Length; i++)
                {
                    material.SetTexture("_colorTexture" + i, m_cachedPreviewColorRenderTextures[i]);
                }

                //iterate through spawn rules, and if it is a previewed texture set its color accordingly in the color slot
                int colorIndex = 0;
                for (int i = 0; i < m_settings.m_spawnerRules.Count; i++)
                {
                    if (m_previewRuleIds.Contains(i))
                    {
                        material.SetColor("_previewColor" + colorIndex.ToString(), m_settings.m_spawnerRules[m_previewRuleIds[colorIndex]].m_visualisationColor);
                        colorIndex++;
                    }
                }

                for (; colorIndex < GaiaConstants.maxPreviewedTextures; colorIndex++)
                {
                    Color transparentColor = Color.white;
                    transparentColor.a = 0f;
                    material.SetColor("_previewColor" + colorIndex.ToString(), transparentColor);
                }


                Color seaLevelColor = m_gaiaSettings.m_stamperSeaLevelTintColor;
                if (!m_showSeaLevelinStampPreview)
                {
                    seaLevelColor.a = 0f;
                }
                material.SetColor("_seaLevelTintColor", seaLevelColor);
                material.SetFloat("_seaLevel", GaiaSessionManager.GetSessionManager(false).m_session.m_seaLevel);
                operation.Visualize(MultiTerrainOperationType.Heightmap, operation.RTheightmap, material, 1);

                //Clean up
                operation.CloseOperation();
                //Clean up temp textures
                GaiaUtils.ReleaseAllTempRenderTextures();
            }
        }

        private void ClearCachedTexture(RenderTexture cachedRT)
        {
            if (cachedRT != null)
            {
                cachedRT.Release();
                DestroyImmediate(cachedRT);
            }

            cachedRT = new RenderTexture(1, 1, 1);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = cachedRT;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = currentRT;

        }

        private void ClearColorTextureCache()
        {
            for (int i = 0; i < m_cachedPreviewColorRenderTextures.Length; i++)
            {
                ClearCachedTexture(m_cachedPreviewColorRenderTextures[i]);
            }
        }

        /// <summary>
        /// Inizialises or "resets" a color texture in the cache
        /// </summary>
        /// <param name="index">The index at which to initialise.</param>
        /// <param name="rtToInitialiseFrom">A sample render texture with the correct resolution & format settings etc. to initialise from</param>
        private void InitialiseColorTextureCache(int index, RenderTexture rtToInitialiseFrom)
        {
            ClearCachedTexture(m_cachedPreviewColorRenderTextures[index]);
            m_cachedPreviewColorRenderTextures[index] = new RenderTexture(rtToInitialiseFrom);
        }

        private RenderTexture ApplyBrush(GaiaMultiTerrainOperation operation, MultiTerrainOperationType opType, int spawnRuleID = 0)
        {
            Terrain currentTerrain = GetCurrentTerrain();

            RenderTextureDescriptor rtDescriptor;

            switch (opType)
            {
                case MultiTerrainOperationType.Heightmap:
                    rtDescriptor = operation.RTheightmap.descriptor;
                    break;
                case MultiTerrainOperationType.Texture:
                    rtDescriptor = operation.RTtextureSplatmap.descriptor;
                    break;
                case MultiTerrainOperationType.TerrainDetail:
                    rtDescriptor = operation.RTdetailmap.descriptor;
                    break;
                case MultiTerrainOperationType.Tree:
                    rtDescriptor = operation.RTterrainTree.descriptor;
                    break;
                case MultiTerrainOperationType.GameObject:
                    rtDescriptor = operation.RTgameObject.descriptor;
                    break;
                default:
                    rtDescriptor = operation.RTheightmap.descriptor;
                    break;
            }

            rtDescriptor.enableRandomWrite = true;

            RenderTexture inputTexture = RenderTexture.GetTemporary(rtDescriptor);
            RenderTexture inputTexture2 = RenderTexture.GetTemporary(rtDescriptor);

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = inputTexture;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = inputTexture2;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = currentRT;

            //fetch the biome mask stack (if any)
            BiomeController biomeController = Resources.FindObjectsOfTypeAll<BiomeController>().FirstOrDefault(x=>x.m_autoSpawners.Find(y=>y.spawner==this)!=null);

            ImageMask[] spawnerStack = new ImageMask[0];
            //set up the spawner mask stack, only if it has masks or a biome controller exists with masks
            if (m_settings.m_imageMasks.Length > 0 || (biomeController!=null && biomeController.m_biomeMasks.Length>0))
            {
                if (biomeController != null)
                {
                    //use a combined mask stack of both biome 
                    spawnerStack = biomeController.m_biomeMasks;
                    spawnerStack = spawnerStack.Concat(m_settings.m_imageMasks).ToArray();
                }
                else
                {
                    spawnerStack = m_settings.m_imageMasks;
                }

                //We start from a white texture, so we need the first mask action in the stack to always be "Multiply", otherwise there will be no result.
                spawnerStack[0].m_blendMode = ImageMaskBlendMode.Multiply;

                //Iterate through all image masks and set up the current paint context in case the shader uses heightmap data
                foreach (ImageMask mask in spawnerStack)
                {
                    mask.m_multiTerrainOperation = operation;
                    mask.m_seaLevel = GaiaSessionManager.GetSessionManager(false).GetSeaLevel();
                    mask.m_maxWorldHeight = m_maxWorldHeight;
                    mask.m_minWorldHeight = m_minWorldHeight;
                }

            }

            //set up the resource mask stack
            ImageMask[] maskStack = m_settings.m_spawnerRules[spawnRuleID].m_imageMasks;
            if (maskStack.Length > 0)
            {
                //We start from a white texture, so we need the first mask action in the stack to always be "Multiply", otherwise there will be no result.
                maskStack[0].m_blendMode = ImageMaskBlendMode.Multiply;

                //Iterate through all image masks and set up the current paint context in case the shader uses heightmap data
                foreach (ImageMask mask in maskStack)
                {
                    //mask.m_heightmapContext = heightmapContext;
                    //mask.m_normalmapContext = normalmapContext;
                    //mask.m_collisionContext = collisionContext;
                    mask.m_multiTerrainOperation = operation;
                    mask.m_seaLevel = GaiaSessionManager.GetSessionManager(false).GetSeaLevel();
                    mask.m_maxWorldHeight = m_maxWorldHeight;
                    mask.m_minWorldHeight = m_minWorldHeight;
                }

            }


            //Get the combined masks from the spawner
            RenderTexture spawnerOutputTexture = RenderTexture.GetTemporary(rtDescriptor);
            Graphics.Blit(ImageProcessing.ApplyMaskStack(inputTexture, spawnerStack, ImageMaskInfluence.Local), spawnerOutputTexture);

            //Get the combined masks from the rule
            RenderTexture ruleOutputTexture = RenderTexture.GetTemporary(rtDescriptor);
            Graphics.Blit(ImageProcessing.ApplyMaskStack(inputTexture2, maskStack, ImageMaskInfluence.Local), ruleOutputTexture);

            //if (opType == MultiTerrainOperationType.Texture)
            //{
            //    ImageProcessing.WriteRenderTexture("D:\\spawnerOutputTexture.png", spawnerOutputTexture);
            //    ImageProcessing.WriteRenderTexture("D:\\ruleOutputTexture.png", ruleOutputTexture);
            //} 

            //Run them through the image mask shader for a simple multiply
            Material filterMat = new Material(Shader.Find("Hidden/Gaia/FilterImageMask"));
            filterMat.SetTexture("_InputTex", spawnerOutputTexture);
            filterMat.SetFloat("_Strength", 1f);
            filterMat.SetInt("_Invert", 0);
            filterMat.SetTexture("_ImageMaskTex", ruleOutputTexture);

            RenderTexture combinedOutputTexture = RenderTexture.GetTemporary(rtDescriptor);
            Graphics.Blit(inputTexture, combinedOutputTexture, filterMat, 0);


            //clean up temporary textures
            ReleaseRenderTexture(inputTexture);
            inputTexture = null;
            ReleaseRenderTexture(inputTexture2);
            inputTexture2 = null;
            ReleaseRenderTexture(spawnerOutputTexture);
            spawnerOutputTexture = null;
            ReleaseRenderTexture(ruleOutputTexture);
            ruleOutputTexture = null;

            
            return combinedOutputTexture;
        }



        public Terrain GetCurrentTerrain()
        {
            Terrain currentTerrain = Gaia.TerrainHelper.GetTerrain(transform.position);
            //Check if the stamper is over a terrain currently
            //if not, we will draw a preview based on the last active terrain we were over
            //if that is null either we can't draw a stamp preview
            if (currentTerrain)
            {
                //Update last active terrain with current
                if (m_lastActiveTerrain != currentTerrain)
                {
                    //if the current terrain is a new terrain, we should refresh the min max values in case this terrain has never been calculated before
                    GaiaSessionManager.GetSessionManager().GetWorldMinMax(ref m_minWorldHeight, ref m_maxWorldHeight);
                }
                m_lastActiveTerrain = currentTerrain;
            }
            else
            {
                if (m_lastActiveTerrain)
                    currentTerrain = m_lastActiveTerrain;
                else
                    return null;
            }
            return currentTerrain;
        }

        private void ReleaseRenderTexture(RenderTexture texture)
        {
            if (texture != null)
            {
                RenderTexture.ReleaseTemporary(texture);
                texture = null;
            }
        }


        //public ImageMask[] GetSpawnRuleImageMasksByIndex(int spawnRuleIndex)
        //{
        //    //Get the right mask list from the resources according to the resource type that is used
        //    switch (m_spawnerRules[spawnRuleIndex].m_resourceType)
        //    {
        //        case GaiaConstants.SpawnerResourceType.TerrainTexture:
        //            return m_resources.m_texturePrototypes[m_spawnerRules[spawnRuleIndex].m_resourceIdx].m_imageMasks;
        //        case GaiaConstants.SpawnerResourceType.TerrainDetail:
        //            return m_resources.m_detailPrototypes[m_spawnerRules[spawnRuleIndex].m_resourceIdx].m_imageMasks;
        //        case GaiaConstants.SpawnerResourceType.TerrainTree:
        //            return m_resources.m_treePrototypes[m_spawnerRules[spawnRuleIndex].m_resourceIdx].m_imageMasks;
        //        case GaiaConstants.SpawnerResourceType.GameObject:
        //            return m_resources.m_gameObjectPrototypes[m_spawnerRules[spawnRuleIndex].m_resourceIdx].m_imageMasks;
        //    }
        //    return null;
        //}


        //public CollisionMask[] GetSpawnRuleCollisionMasksByIndices(int spawnRuleIndex, int maskIndex)
        //{
        //    //Get the right collision mask list from the resources according to the resource type that is used
        //    return GetSpawnRuleImageMasksByIndex(spawnRuleIndex)[maskIndex].m_collisionMasks;
        //}

        //public void SetSpawnRuleImageMasksByIndex(int spawnRuleIndex, ImageMask[] imageMasks)
        //{
        //    //Get the right mask list from the resources according to the resource type that is used
        //    switch (m_spawnerRules[spawnRuleIndex].m_resourceType)
        //    {
        //        case GaiaConstants.SpawnerResourceType.TerrainTexture:
        //            m_resources.m_texturePrototypes[m_spawnerRules[spawnRuleIndex].m_resourceIdx].m_imageMasks = imageMasks;
        //            break;
        //        case GaiaConstants.SpawnerResourceType.TerrainDetail:
        //            m_resources.m_detailPrototypes[m_spawnerRules[spawnRuleIndex].m_resourceIdx].m_imageMasks = imageMasks;
        //            break;
        //        case GaiaConstants.SpawnerResourceType.TerrainTree:
        //            m_resources.m_treePrototypes[m_spawnerRules[spawnRuleIndex].m_resourceIdx].m_imageMasks = imageMasks;
        //            break;
        //        case GaiaConstants.SpawnerResourceType.GameObject:
        //            m_resources.m_gameObjectPrototypes[m_spawnerRules[spawnRuleIndex].m_resourceIdx].m_imageMasks = imageMasks;
        //            break;
        //    }
        //}

        //public void SetSpawnRuleCollisionMasksByIndices(int spawnRuleIndex, int maskIndex, CollisionMask[] collisionMasks)
        //{
        //    m_spawnerRules[spawnRuleIndex].m_imageMasks[maskIndex].m_collisionMasks = collisionMasks;
        //}

        //public Color GetVisualisationColorBySpawnRuleIndex(int spawnRuleIndex)
        //{
        //    switch (m_settings.m_spawnerRules[spawnRuleIndex].m_resourceType)
        //    {
        //        case GaiaConstants.SpawnerResourceType.TerrainTexture:
        //            ResourceProtoTexture protoTexture = (ResourceProtoTexture)GetResourceProtoBySpawnRuleIndex(spawnRuleIndex);
        //            if (protoTexture != null)
        //                return protoTexture.m_visualisationColor;
        //            else
        //                return Color.red;
        //        case GaiaConstants.SpawnerResourceType.TerrainTree:
        //            ResourceProtoTree protoTree = (ResourceProtoTree)GetResourceProtoBySpawnRuleIndex(spawnRuleIndex);
        //            if (protoTree != null)
        //                return protoTree.m_visualisationColor;
        //            else
        //                return Color.red;
        //        case GaiaConstants.SpawnerResourceType.TerrainDetail:
        //            ResourceProtoDetail protoDetail = (ResourceProtoDetail)GetResourceProtoBySpawnRuleIndex(spawnRuleIndex);
        //            if (protoDetail != null)
        //                return protoDetail.m_visualisationColor;
        //            else
        //                return Color.red;
        //        case GaiaConstants.SpawnerResourceType.GameObject:
        //            ResourceProtoGameObject protoGameObject = (ResourceProtoGameObject)GetResourceProtoBySpawnRuleIndex(spawnRuleIndex);
        //            if (protoGameObject != null)
        //                return protoGameObject.m_visualisationColor;
        //            else
        //                return Color.red;
        //    }
        //    return Color.red;
        //}

        //public void SetVisualisationColorBySpawnRuleIndex(Color color, int spawnRuleIndex)
        //{
        //    switch (m_settings.m_spawnerRules[spawnRuleIndex].m_resourceType)
        //    {
        //        case GaiaConstants.SpawnerResourceType.TerrainTexture:
        //            ResourceProtoTexture protoTexture = (ResourceProtoTexture)GetResourceProtoBySpawnRuleIndex(spawnRuleIndex);
        //            if (protoTexture != null)
        //            {
        //                protoTexture.m_visualisationColor = color;
        //            }
        //            break;
        //        case GaiaConstants.SpawnerResourceType.TerrainTree:
        //            ResourceProtoTree protoTree = (ResourceProtoTree)GetResourceProtoBySpawnRuleIndex(spawnRuleIndex);
        //            if (protoTree != null)
        //            {
        //                protoTree.m_visualisationColor = color;
        //            }
        //            break;
        //        case GaiaConstants.SpawnerResourceType.TerrainDetail:
        //            ResourceProtoDetail protoDetail = (ResourceProtoDetail)GetResourceProtoBySpawnRuleIndex(spawnRuleIndex);
        //            if (protoDetail != null)
        //            {
        //                protoDetail.m_visualisationColor = color;
        //            }
        //            break;
        //        case GaiaConstants.SpawnerResourceType.GameObject:
        //            ResourceProtoGameObject protoGameObject = (ResourceProtoGameObject)GetResourceProtoBySpawnRuleIndex(spawnRuleIndex);
        //            if (protoGameObject != null)
        //            {
        //                protoGameObject.m_visualisationColor = color;
        //            }
        //            break;
        //    }
            
        //}

        public object GetResourceProtoBySpawnRuleIndex(int spawnRuleIndex)
        {
            switch (m_settings.m_spawnerRules[spawnRuleIndex].m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    if (m_settings.m_spawnerRules[spawnRuleIndex].m_resourceIdx < m_settings.m_resources.m_texturePrototypes.Length)
                        return m_settings.m_resources.m_texturePrototypes[m_settings.m_spawnerRules[spawnRuleIndex].m_resourceIdx];
                    else
                        return null;
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    if (m_settings.m_spawnerRules[spawnRuleIndex].m_resourceIdx < m_settings.m_resources.m_treePrototypes.Length)
                        return m_settings.m_resources.m_treePrototypes[m_settings.m_spawnerRules[spawnRuleIndex].m_resourceIdx];
                    else
                        return null;
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    if (m_settings.m_spawnerRules[spawnRuleIndex].m_resourceIdx < m_settings.m_resources.m_detailPrototypes.Length)
                        return m_settings.m_resources.m_detailPrototypes[m_settings.m_spawnerRules[spawnRuleIndex].m_resourceIdx];
                    else
                        return null;
                case GaiaConstants.SpawnerResourceType.GameObject:
                    if (m_settings.m_spawnerRules[spawnRuleIndex].m_resourceIdx < m_settings.m_resources.m_gameObjectPrototypes.Length)
                        return m_settings.m_resources.m_gameObjectPrototypes[m_settings.m_spawnerRules[spawnRuleIndex].m_resourceIdx];
                    else
                        return null;
            }
            return null;
        }

        /// <summary>
        /// Toggle the preview mesh on and off
        /// </summary>
        public void TogglePreview()
        {
            m_drawPreview = !m_drawPreview;
            DrawSpawnerPreview();
        }


        /// <summary>
        /// Run a spawner iteration - called by timed invoke or manually
        /// </summary>
        //        public void RunSpawnerIteration()
        //        {
        //            //Reset status
        //            m_cancelSpawn = false;
        //            m_spawnComplete = false;

        //            //Perform a spawner iteration preinitialisation
        //            PreSpawnInitialise();

        //			//Check that there are rules that can be applied
        //			if (m_activeRuleCnt <= 0)
        //			{
        //				if (m_showDebug)
        //				{
        //					Debug.Log(string.Format("{0}: There are no active spawn rules. Can't spawn without rules.", gameObject.name));
        //				}
        //                m_spawnComplete = true;
        //				return;
        //			}

        //			//Check that we can actually add new instances
        //			if (!CanSpawnInstances())
        //			{
        //				if (m_showDebug)
        //				{
        //					Debug.Log(string.Format("{0}: Can't spawn or activate new instance - max instance count reached.", gameObject.name));
        //				}
        //                m_spawnComplete = true;
        //				return;
        //			}

        //            //Call out any issues with terrain height
        //            Terrain t = TerrainHelper.GetTerrain(transform.position);
        //            if (t != null)
        //            {
        //                m_terrainHeight = t.terrainData.size.y;
        //                if (m_resources != null && m_resources.m_terrainHeight != m_terrainHeight)
        //                {
        //                    Debug.LogWarning(string.Format("There is a mismatch between your resources Terrain Height {0} and your actual Terrain Height {1}. Your Spawn may not work as intended!", m_resources.m_terrainHeight, m_terrainHeight));
        //                }
        //            }

        //            //Look for any tagged objects that are acting as triggers and check if they were in range
        //            if (m_mode == GaiaConstants.OperationMode.RuntimeTriggeredInterval)
        //            {
        //                m_checkDistance = m_triggerRange + 1f;
        //                List<GameObject> triggerObjects = new List<GameObject>();
        //                string[] tags = new string[0];
        //                if (!string.IsNullOrEmpty(m_triggerTags))
        //                {
        //                    tags = m_triggerTags.Split(',');
        //                }
        //                else
        //                {
        //                    Debug.LogError("You have not supplied a trigger tag. Spawner will not spawn!");
        //                }
        //                int idx = 0;
        //                if (m_triggerTags.Length > 0 &&  tags.Length > 0)
        //                {
        //                    //Grab the tagged objects
        //                    for (idx = 0; idx < tags.Length; idx++)
        //                    {
        //                        triggerObjects.AddRange(GameObject.FindGameObjectsWithTag(tags[idx]));
        //                    }

        //                    //Now look for anything in range
        //                    for (idx = 0; idx < triggerObjects.Count; idx++)
        //                    {
        //                        m_checkDistance = Vector3.Distance(transform.position, triggerObjects[idx].transform.position);
        //                        if (m_checkDistance <= m_triggerRange)
        //                        {
        //                            break;
        //                        }
        //                    }

        //                    //And if its wasnt found then drop out
        //                    if (m_checkDistance > m_triggerRange)
        //                    {
        //                        if (m_showDebug)
        //                        {
        //                            Debug.Log(string.Format("{0}: No triggers were close enough", gameObject.name));
        //                        }
        //                        m_spawnComplete = true;
        //                        return; //Nothing to do - trigger is too far away
        //                    }
        //                }
        //                else
        //                {
        //                    //Nothing to see, drop out
        //                    if (m_showDebug)
        //                    {
        //                        Debug.Log(string.Format("{0}: No triggers found", gameObject.name));
        //                    }
        //                    m_spawnComplete = true;
        //                    return;
        //                }
        //            }

        //            //Update the session - but only of we are not playing
        //            if (!Application.isPlaying)
        //            {
        //                AddToSession(GaiaOperation.OperationType.Spawn, "Spawning " + transform.name);
        //            }

        //            //Run the spawner based on the location selection method chosen
        //            if (m_spawnLocationAlgorithm == GaiaConstants.SpawnerLocation.RandomLocation || m_spawnLocationAlgorithm == GaiaConstants.SpawnerLocation.RandomLocationClustered)
        //            {
        //                #if UNITY_EDITOR
        //                    if (!Application.isPlaying)
        //                    {
        //                        m_updateCoroutine = RunRandomSpawnerIteration();
        //                        StartEditorUpdates();
        //                    }
        //                    else
        //                    {
        //                        StartCoroutine(RunRandomSpawnerIteration());
        //                    }
        //#else
        //                    StartCoroutine(RunRandomSpawnerIteration(terrainID));
        //#endif
        //            }
        //            else
        //            {
        //                #if UNITY_EDITOR
        //                    if (!Application.isPlaying)
        //                    {
        //                        m_updateCoroutine = RunAreaSpawnerIteration();
        //                        StartEditorUpdates();
        //                    }
        //                    else
        //                    {
        //                        StartCoroutine(RunAreaSpawnerIteration());
        //                    }
        //#else
        //                    StartCoroutine(RunAreaSpawnerIteration(terrainID));
        //#endif
        //            }
        //        }

        public float GetMaxSpawnerRange()
        {
            Terrain currentTerrain = GetCurrentTerrain();
            if (currentTerrain != null)
            {
                return Mathf.Round((float)4097 / (float)currentTerrain.terrainData.heightmapResolution * currentTerrain.terrainData.size.x / 2f);
            }
            else
            {
                return 1000;
            }
            //limit the width to not overstep the max internal resolution value of 4097 of the spawner preview
            //float range = Mathf.RoundToInt(Mathf.Clamp(m_settings.m_spawnRange, 1, (float)4097 / (float)currentTerrain.terrainData.heightmapResolution * 100));
            //float widthfactor = currentTerrain.terrainData.size.x / 100f /2f;

            //return range * widthfactor;
        }


        public IEnumerator RunSpawnerIteration(bool worldSpawn)
        {
            m_cancelSpawn = false;
            //Terrain[] terrainsToProcess;

            //store all rules for which we have performed an asset clearing already
            List<int> clearedRuleIndices = new List<int>();

            //store all collision masks that were baked already
            List<CollisionMask> bakedCollisionMasks = new List<CollisionMask>();

            Vector3 originalPosition = transform.position;
            Quaternion originalRotation = transform.rotation;
            float originalSpawnRange = m_settings.m_spawnRange;

            Bounds spawnBounds = new Bounds();
            Vector3 startPosition = transform.position;
            //Determine the bounds of the spawning operation, for a worldspawn we need to consider all terrains,
            //for a local spawn we just take the spawner range into consideration
            if (worldSpawn)
            {
                TerrainHelper.GetTerrainBounds(ref spawnBounds);
                m_settings.m_spawnRange = Mathf.Min(m_gaiaSettings.m_spawnerWorldSpawnRange, Mathf.Min(spawnBounds.extents.x,spawnBounds.extents.z));
                startPosition = new Vector3(spawnBounds.min.x + m_settings.m_spawnRange, startPosition.y , spawnBounds.min.z + m_settings.m_spawnRange);
                //for world spawns we use the maximal spawner range defined in Gaia settings
                // terrainsToProcess = Terrain.activeTerrains;
            }
            else
            {
                spawnBounds = new Bounds(transform.position, new Vector3(m_settings.m_spawnRange * 2f, 1000f, m_settings.m_spawnRange * 2f));
            }

            
          

            bool needSpawnerRefresh = false;
            bool spawnedGameObjects = false;

            //Calculate the maximum amount of rules that need to be processed in all iterations for the progress bar
            int maxIterations = Mathf.FloorToInt(spawnBounds.extents.x / m_settings.m_spawnRange) * Mathf.FloorToInt(spawnBounds.extents.z / m_settings.m_spawnRange);
            int maxRules = m_settings.m_spawnerRules.FindAll(x => x.m_isActive).Count * maxIterations;
            int completedRules = 0;

            for (Vector3 currentSpawnCenter = startPosition; currentSpawnCenter.x <= spawnBounds.max.x; currentSpawnCenter += new Vector3(m_gaiaSettings.m_spawnerWorldSpawnRange * 2,0f,0f))
            {
                for (currentSpawnCenter = new Vector3(currentSpawnCenter.x, currentSpawnCenter.y, startPosition.z); currentSpawnCenter.z <= spawnBounds.max.z; currentSpawnCenter += new Vector3(0f, 0f, m_gaiaSettings.m_spawnerWorldSpawnRange * 2))
                {
                    if (m_cancelSpawn)
                    {
                        break;
                    }
                    UpdateMinMaxHeight();
                    //Time control for enumeration
                    //float currentTime = Time.realtimeSinceStartup;
                    //float accumulatedTime = 0.0f;



                    //Terrain currentTerrain = terrainsToProcess[terrainIndex];

                    //If we are doing a world spawn, adjust the spanwer to the terrain we are iterating on, otherwise we just leave it as the user placed it
                    //if (worldSpawn)
                    //{
                    //    FitToTerrain(currentTerrain);
                    //}

                    transform.position = currentSpawnCenter;


                    Terrain currentTerrain = GetCurrentTerrain();

                    //Set up a multi-terrain operation once, all rules can then draw from the data collected here

                    GaiaMultiTerrainOperation operation = new GaiaMultiTerrainOperation(currentTerrain, transform, m_settings.m_spawnRange * 2f, true);
                    operation.GetHeightmap();
                    operation.GetNormalmap();
                    operation.CollectTerrainDetails();
                    operation.CollectTerrainTrees();
                    operation.CollectTerrainGameObjects();
                    operation.CollectTerrainCollisions();



                    for (int i = 0; i < m_settings.m_spawnerRules.Count; i++)
                    {
                        if (m_cancelSpawn)
                        {
                            break;
                        }


                        GaiaUtils.DisplayProgressBarNoEditor("Running Spawner", "Running spawner " + transform.name, (float)completedRules / (float)maxRules);

                        if (m_settings.m_spawnerRules[i].m_isActive)
                        {
                            //BrushTransform brushXform = new BrushTransform();

                            //Check first if any of the masks is a collision mask and re-bake them, otherwise spawns will not work properly if collision data is outdated
                            //We have to do this in here because previous spawns in this loop might require re-baking of collision data for a correct result
                            foreach (ImageMask imageMask in m_settings.m_spawnerRules[i].m_imageMasks.Where(x => x.m_active && x.m_operation == ImageMaskOperation.CollisionMask))
                            {
                                foreach (CollisionMask collisionMask in imageMask.m_collisionMasks)
                                {
                                    bool bakedAlready = false;
                                    switch (collisionMask.m_type)
                                    {
                                        case CollisionMaskType.Tag:
                                            if (bakedCollisionMasks.Exists(x => x.m_tag == collisionMask.m_tag && x.m_Radius == collisionMask.m_Radius))
                                            {
                                                bakedAlready = true;
                                            }
                                            break;
                                        case CollisionMaskType.TerrainTree:
                                            if (bakedCollisionMasks.Exists(x => x.m_treePrototypeId == collisionMask.m_treePrototypeId && x.m_Radius == collisionMask.m_Radius))
                                            {
                                                bakedAlready = true;
                                            }
                                            break;
                                    }

                                    if (!bakedAlready)
                                    {
                                        collisionMask.Bake();
                                        bakedCollisionMasks.Add(collisionMask);
                                    }
                                    m_spawnProgress = (float)completedRules / (float)maxRules;
                                }
                            }

                            switch (m_settings.m_spawnerRules[i].m_resourceType)
                            {
                                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                                    //Get correct terrain layer id from spawner Rule id
                                    int layerID = m_settings.m_resources.PrototypeIdxInTerrain(m_settings.m_spawnerRules[i].m_resourceType, m_settings.m_spawnerRules[i].m_resourceIdx);
                                    //Splatmap deliberately handled without a sub-coroutine - creates issues during applying the splatmaps
                                    if (layerID != -1)
                                    {
                                        operation.GetSplatmap(currentTerrain.terrainData.terrainLayers[layerID]);
                                        operation.SetSplatmap(ApplyBrush(operation, MultiTerrainOperationType.Texture, i), false);
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Could not find texture " + m_settings.m_resources.m_texturePrototypes[m_settings.m_spawnerRules[i].m_resourceIdx].m_name + " on the terrain! Skipping this texture for spawning...");
                                    }
                                    //RenderTexture textureResult = new RenderTexture(operation.RTtextureSplatmap);
                                    //RenderTexture combinedOutputTexture = RenderTexture.GetTemporary(operation.RTtextureSplatmap.descriptor);
                                    //Graphics.Blit(ApplyBrush(operation, MultiTerrainOperationType.Texture, i), textureResult);
                                    //switch (m_settings.spawnMode)
                                    //{
                                    //    case SpawnMode.Add:
                                    //        //Take the original splatmap for the texture and run the result through the image mask shader for a simple add (pass 3)
                                    //        Material filterMat = new Material(Shader.Find("Hidden/Gaia/FilterImageMask"));
                                    //        filterMat.SetTexture("_InputTex", operation.RTtextureSplatmap);
                                    //        ImageProcessing.WriteRenderTexture("D:\\RTtexturesplatmap.png", operation.RTtextureSplatmap);
                                    //        filterMat.SetFloat("_Strength", 1f);
                                    //        filterMat.SetInt("_Invert", 0);
                                    //        filterMat.SetTexture("_ImageMaskTex", textureResult);
                                    //        ImageProcessing.WriteRenderTexture("D:\\textureResult.png", textureResult);
                                    //        Graphics.Blit(textureResult,combinedOutputTexture, filterMat, 3);
                                    //        ImageProcessing.WriteRenderTexture("D:\\combinedOutput.png", combinedOutputTexture);
                                    //        operation.SetSplatmap(combinedOutputTexture);
                                    //        break;
                                    //    case SpawnMode.Remove:
                                    //        //Take the original splatmap for the texture and run the result through the image mask shader for a simple subtract (pass 4)
                                    //        Material filterMat2 = new Material(Shader.Find("Hidden/Gaia/FilterImageMask"));
                                    //        filterMat2.SetTexture("_InputTex", operation.RTtextureSplatmap);
                                    //        ImageProcessing.WriteRenderTexture("D:\\RTtexturesplatmap.png", operation.RTtextureSplatmap);
                                    //        filterMat2.SetFloat("_Strength", 1f);
                                    //        filterMat2.SetInt("_Invert", 0);
                                    //        filterMat2.SetTexture("_ImageMaskTex", textureResult);
                                    //        ImageProcessing.WriteRenderTexture("D:\\textureResult.png", textureResult);
                                    //        Graphics.Blit(textureResult, combinedOutputTexture, filterMat2, 4);
                                    //        ImageProcessing.WriteRenderTexture("D:\\combinedOutput.png", combinedOutputTexture);
                                    //        operation.SetSplatmap(combinedOutputTexture);
                                    //        break;
                                    //    case SpawnMode.Replace:
                                    //        //Just overwrite with the mask result directly
                                    //        operation.SetSplatmap(textureResult);
                                    //               break;
                                    //}
                                    //RenderTexture.ReleaseTemporary(textureResult);
                                    //textureResult = null;
                                    //RenderTexture.ReleaseTemporary(combinedOutputTexture);
                                    //combinedOutputTexture = null;
                                    break;
                                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                                    int detailLayerID = m_settings.m_resources.PrototypeIdxInTerrain(m_settings.m_spawnerRules[i].m_resourceType, m_settings.m_spawnerRules[i].m_resourceIdx);
                                    if (detailLayerID != -1)
                                    {
                                        operation.SetTerrainDetails(ApplyBrush(operation, MultiTerrainOperationType.TerrainDetail, i), detailLayerID, m_settings.spawnMode, ref m_settings.m_spawnerRules[i].m_spawnedInstances, false);
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Could not find terrain detail " + m_settings.m_resources.m_detailPrototypes[m_settings.m_spawnerRules[i].m_resourceIdx].m_name + " on the terrain! Skipping this terrain detail for spawning...");
                                    }
                                    break;
                                case GaiaConstants.SpawnerResourceType.TerrainTree:
                                    needSpawnerRefresh = true;
                                    int treePrototypeIndex = m_settings.m_resources.PrototypeIdxInTerrain(m_settings.m_spawnerRules[i].m_resourceType, m_settings.m_spawnerRules[i].m_resourceIdx);
                                    ResourceProtoTree protoTree = m_settings.m_resources.m_treePrototypes[m_settings.m_spawnerRules[i].m_resourceIdx];
                                    if (treePrototypeIndex != -1)
                                    {

                                        //We only may remove / reset this tree rule once - otherwise this would destroy earlier tree spawn results in world spawns!
                                        if (m_settings.spawnMode == SpawnMode.Replace && !clearedRuleIndices.Contains(i))
                                        {
                                            foreach (Terrain t in Terrain.activeTerrains)
                                            {
                                                t.terrainData.SetTreeInstances(t.terrainData.treeInstances.Where(x => x.prototypeIndex != treePrototypeIndex).ToArray(), true);
                                                m_settings.m_spawnerRules[i].m_spawnedInstances = 0;
                                                clearedRuleIndices.Add(i);
                                            }
                                        }
                                        operation.SetTerrainTrees(ApplyBrush(operation, MultiTerrainOperationType.Tree, i), treePrototypeIndex, protoTree, m_settings.m_spawnerRules[i], m_settings.spawnMode, ref m_settings.m_spawnerRules[i].m_spawnedInstances, m_settings.m_spawnerRules[i].m_minRequiredFitness, false);
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Could not find terrain tree " + m_settings.m_resources.m_detailPrototypes[m_settings.m_spawnerRules[i].m_resourceIdx].m_name + " on the terrain! Skipping this terrain tree for spawning...");
                                    }
                                    break;
                                case GaiaConstants.SpawnerResourceType.GameObject:
                                    needSpawnerRefresh = true;
                                    spawnedGameObjects = true;
                                    int goPrototypeIndex = m_settings.m_resources.PrototypeIdxInTerrain(m_settings.m_spawnerRules[i].m_resourceType, m_settings.m_spawnerRules[i].m_resourceIdx);
                                    ResourceProtoGameObject protoGO = m_settings.m_resources.m_gameObjectPrototypes[m_settings.m_spawnerRules[i].m_resourceIdx];
                                    Transform target = GetGOSpawnTarget(m_settings.m_spawnerRules[i], protoGO);
                                    if (target != null)
                                    {
                                        //We only may remove / reset the Game Object spawner once - otherwise this would destroy earlier spawn results in world spawns!
                                        if (m_settings.spawnMode == SpawnMode.Replace && !clearedRuleIndices.Contains(i))
                                        {
                                            for (int g = target.childCount - 1; g >= 0; g--)
                                            {
                                                DestroyImmediate(target.GetChild(g).gameObject);
                                            }
                                            m_settings.m_spawnerRules[i].m_spawnedInstances = 0;
                                            clearedRuleIndices.Add(i);
                                        }
                                    }
                                    operation.SetTerrainGameObjects(ApplyBrush(operation, MultiTerrainOperationType.GameObject, i), protoGO, m_settings.m_spawnerRules[i], target, m_settings.spawnMode, ref m_settings.m_spawnerRules[i].m_spawnedInstances, m_settings.m_spawnerRules[i].m_minRequiredFitness, false);
                                    break;
                            }

                        }

                        completedRules++;
                        //GaiaUtils.ReleaseAllTempRenderTextures();
                        //Update progress and yield periodiocally
                        m_spawnProgress = (float)completedRules / (float)maxRules;
                        yield return null;


                    }
                    //Update progress and yield periodiocally
                    m_spawnProgress = (float)completedRules / (float)maxRules;
                    yield return null;

                    //Reset spawn progress to unlock the GUI in the spawner editor
                    m_spawnProgress = 0;

                    //Clean up
                    operation.CloseOperation();
                    GaiaSessionManager.GetSessionManager().m_collisionMaskCache.ClearCache();
                    GaiaUtils.ReleaseAllTempRenderTextures();
                }
            }

            //all done, if we spawned game objects we need to mark this scene as dirty

            if (spawnedGameObjects)
            {
#if UNITY_EDITOR
                EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
            }

            //restore original positon & rotation for this spawner
            transform.position = originalPosition;
            transform.rotation = originalRotation;


            GaiaUtils.ClearProgressBarNoEditor();
            
            if (needSpawnerRefresh)
            {
                m_spawnPreviewDirty = true;
            }
            m_spawnComplete = true;
            m_updateCoroutine = null;
        }

        public Transform GetGOSpawnTarget(SpawnRule rule, ResourceProtoGameObject protoGO)
        {
            if (rule.m_goSpawnTarget == null)
            {
                Transform gaiaTransform = GaiaUtils.GetGaiaGameObject().transform;
                Transform parentTransform = gaiaTransform.Find(GaiaConstants.defaultGOSpawnTarget);

                if (parentTransform == null)
                {
                    GameObject newParent = new GameObject();
                    newParent.name = GaiaConstants.defaultGOSpawnTarget;
                    newParent.transform.parent = gaiaTransform;
                    parentTransform = newParent.transform;
                }

                GameObject newGO = new GameObject();
                newGO.name = protoGO.m_name;
                newGO.transform.parent = parentTransform;
                rule.m_goSpawnTarget = newGO.transform;
            }
            return rule.m_goSpawnTarget;
        }

        private float GetMaxTerrainHeight()
        {
            float max = 0f;
            for (int x = 0; x <= m_lastActiveTerrain.terrainData.heightmapWidth; x++)
            {
                for (int z = 0; z <= m_lastActiveTerrain.terrainData.heightmapWidth; z++)
                {
                    float value = m_lastActiveTerrain.terrainData.GetHeight(x, z);
                    if (value > max)
                        max = value;
                }
            }
            return max;
        }

        /// <summary>
        /// Run a random location based spawner iteration - the spawner is always trying to spawn something on the underlying terrain
        /// </summary>
        public IEnumerator RunRandomSpawnerIteration()
        {
            //if (m_showDebug)
            //{
            //    Debug.Log(string.Format("{0}: Running random iteration", gameObject.name));
            //}

            ////Start iterating
            //int ruleIdx;
            //float fitness, maxFitness, selectedFitness;
            //SpawnRule rule, fittestRule, selectedRule;
            //SpawnInfo spawnInfo = new SpawnInfo();
            //SpawnLocation spawnLocation;
            //List<SpawnLocation> spawnLocations = new List<SpawnLocation>();
            //int spawnLocationsIdx = 0;
            //int failedSpawns = 0;

            ////Set progress
            //m_spawnProgress = 0f;
            //m_spawnComplete = false;

            ////Time control for enumeration
            //float currentTime = Time.realtimeSinceStartup;
            //float accumulatedTime = 0.0f;

            ////Create spawn caches
            //CreateSpawnCaches();

            ////Load image filter
            //LoadImageMask();

            //for (int terrainID = 0; terrainID < Terrain.activeTerrains.Length; terrainID++)
            //{
            //    //Set up the texture layer array in spawn info
            //    spawnInfo.m_textureStrengths = new float[Terrain.activeTerrains[terrainID].terrainData.alphamapLayers];

            //    //Run the location checks
            //    for (int checks = 0; checks < m_locationChecksPerInt; checks++)
            //    {
            //        //Create the spawn location
            //        spawnLocation = new SpawnLocation();

            //        //Choose a random location around the spawner
            //        if (m_spawnLocationAlgorithm == GaiaConstants.SpawnerLocation.RandomLocation)
            //        {
            //            spawnLocation.m_location = GetRandomV3(m_settings.m_spawnRange);
            //            spawnLocation.m_location = transform.position + spawnLocation.m_location;
            //        }
            //        else
            //        {
            //            if (spawnLocations.Count == 0 || spawnLocations.Count > m_maxRandomClusterSize || failedSpawns > m_maxRandomClusterSize)
            //            {
            //                spawnLocation.m_location = GetRandomV3(m_settings.m_spawnRange);
            //                spawnLocation.m_location = transform.position + spawnLocation.m_location;
            //                failedSpawns = 0;
            //                spawnLocationsIdx = 0;
            //                spawnLocations.Clear();
            //            }
            //            else
            //            {
            //                if (spawnLocationsIdx >= spawnLocations.Count)
            //                {
            //                    spawnLocationsIdx = 0;
            //                }
            //                spawnLocation.m_location = GetRandomV3(spawnLocations[spawnLocationsIdx].m_seedDistance);
            //                spawnLocation.m_location = spawnLocations[spawnLocationsIdx++].m_location + spawnLocation.m_location;
            //            }
            //        }

            //        //Run a ray traced hit check to see what we have hit, use rules to determine fitness and select a rule to spawn
            //        if (CheckLocation(spawnLocation.m_location, ref spawnInfo))
            //        {
            //            //Now perform a rule check based on the selected algorithm

            //            //All rules
            //            if (m_spawnRuleSelector == GaiaConstants.SpawnerRuleSelector.All)
            //            {
            //                for (ruleIdx = 0; ruleIdx < m_settings.m_spawnerRules.Count; ruleIdx++)
            //                {
            //                    rule = m_settings.m_spawnerRules[ruleIdx];
            //                    spawnInfo.m_fitness = rule.GetFitness(ref spawnInfo);
            //                    if (TryExecuteRule(ref rule, ref spawnInfo) == true)
            //                    {
            //                        failedSpawns = 0;
            //                        //spawnLocation.m_seedDistance = rule.GetSeedThrowRange(ref spawnInfo);
            //                        spawnLocations.Add(spawnLocation);
            //                    }
            //                    else
            //                    {
            //                        failedSpawns++;
            //                    }
            //                }
            //            }

            //            //Random spawn rule
            //            else if (m_spawnRuleSelector == GaiaConstants.SpawnerRuleSelector.Random)
            //            {
            //                rule = m_settings.m_spawnerRules[GetRandomInt(0, m_settings.m_spawnerRules.Count - 1)];
            //                spawnInfo.m_fitness = rule.GetFitness(ref spawnInfo);
            //                if (TryExecuteRule(ref rule, ref spawnInfo) == true)
            //                {
            //                    failedSpawns = 0;
            //                    //spawnLocation.m_seedDistance = rule.GetSeedThrowRange(ref spawnInfo);
            //                    spawnLocations.Add(spawnLocation);
            //                }
            //                else
            //                {
            //                    failedSpawns++;
            //                }
            //            }

            //            //Fittest spawn rule
            //            else if (m_spawnRuleSelector == GaiaConstants.SpawnerRuleSelector.Fittest)
            //            {
            //                fittestRule = null;
            //                maxFitness = 0f;
            //                for (ruleIdx = 0; ruleIdx < m_settings.m_spawnerRules.Count; ruleIdx++)
            //                {
            //                    rule = m_settings.m_spawnerRules[ruleIdx];
            //                    fitness = rule.GetFitness(ref spawnInfo);
            //                    if (fitness > maxFitness)
            //                    {
            //                        maxFitness = fitness;
            //                        fittestRule = rule;
            //                    }
            //                    else
            //                    {
            //                        //If they are approx equal then give another rule a chance as well to add interest
            //                        if (Gaia.GaiaUtils.Math_ApproximatelyEqual(fitness, maxFitness, 0.005f))
            //                        {
            //                            if (GetRandomFloat(0f, 1f) > 0.5f)
            //                            {
            //                                maxFitness = fitness;
            //                                fittestRule = rule;
            //                            }
            //                        }
            //                    }
            //                }
            //                spawnInfo.m_fitness = maxFitness;
            //                if (TryExecuteRule(ref fittestRule, ref spawnInfo) == true)
            //                {
            //                    failedSpawns = 0;
            //                    spawnLocation.m_seedDistance = fittestRule.GetSeedThrowRange(ref spawnInfo);
            //                    spawnLocations.Add(spawnLocation);
            //                }
            //                else
            //                {
            //                    failedSpawns++;
            //                }
            //            }

            //            //Weighted fittest spawn rule - this implementation will favour fittest
            //            else
            //            {
            //                fittestRule = selectedRule = null;
            //                maxFitness = selectedFitness = 0f;
            //                for (ruleIdx = 0; ruleIdx < m_settings.m_spawnerRules.Count; ruleIdx++)
            //                {
            //                    rule = m_settings.m_spawnerRules[ruleIdx];
            //                    fitness = rule.GetFitness(ref spawnInfo);
            //                    if (GetRandomFloat(0f, 1f) < fitness)
            //                    {
            //                        selectedRule = rule;
            //                        selectedFitness = fitness;
            //                    }
            //                    if (fitness > maxFitness)
            //                    {
            //                        fittestRule = rule;
            //                        maxFitness = fitness;
            //                    }
            //                }
            //                //Check to see if we randomly bombed out - if so then choose fittest
            //                if (selectedRule == null)
            //                {
            //                    selectedRule = fittestRule;
            //                    selectedFitness = maxFitness;
            //                }
            //                //We could still bomb, check for this and avoid it
            //                if (selectedRule != null)
            //                {
            //                    spawnInfo.m_fitness = selectedFitness;
            //                    if (TryExecuteRule(ref selectedRule, ref spawnInfo) == true)
            //                    {
            //                        failedSpawns = 0;
            //                        spawnLocation.m_seedDistance = selectedRule.GetSeedThrowRange(ref spawnInfo);
            //                        spawnLocations.Add(spawnLocation);
            //                    }
            //                    else
            //                    {
            //                        failedSpawns++;
            //                    }
            //                }
            //            }
            //        }

            //        //Update progress and yield periodiocally
            //        m_spawnProgress = (float)checks / (float)m_locationChecksPerInt;
            //        float newTime = Time.realtimeSinceStartup;
            //        float stepTime = newTime - currentTime;
            //        currentTime = newTime;
            //        accumulatedTime += stepTime;
            //        if (accumulatedTime > m_updateTimeAllowed)
            //        {
            //            accumulatedTime = 0f;
            //            yield return null;
            //        }

            //        //Check the instance count, exit if necessary
            //        if (!CanSpawnInstances())
            //        {
            //            break;
            //        }

            //        //Check for cancellation
            //        if (m_cancelSpawn)
            //        {
            //            break;
            //        }
            //    }
            //}
            ////Delete spawn caches
            //DeleteSpawnCaches();

            ////Perform final operations
            //PostSpawn();
            yield return null;
        }

        /// <summary>
        /// Run an area spawner iteration
        /// </summary>
        public IEnumerator RunAreaSpawnerIteration()
        {
            if (m_showDebug)
            {
                Debug.Log(string.Format("{0}: Running area iteration", gameObject.name));
            }

            int ruleIdx;
            float fitness, maxFitness, selectedFitness;
            SpawnRule rule, fittestRule, selectedRule;
            SpawnInfo spawnInfo = new SpawnInfo();
            Vector3 location = new Vector3();
            long currChecks, totalChecks;
            float xWUMin, xWUMax, yMid, zWUMin, zWUMax, jitMin, jitMax;
            float xWU, zWU;

            //Set progress
            m_spawnProgress = 0f;
            m_spawnComplete = false;

            //Time control for enumeration
            float currentTime = Time.realtimeSinceStartup;
            float accumulatedTime = 0.0f;

            //Create spawn caches
            CreateSpawnCaches();

            //Load image filter
            LoadImageMask();

            //for (int terrainID = 0; terrainID < Terrain.activeTerrains.Length; terrainID++)
            //{
                

                //Determine check ranges
                xWUMin = transform.position.x - m_settings.m_spawnRange + (m_locationIncrement / 2f);
                xWUMax = xWUMin + (m_settings.m_spawnRange * 2f);
                yMid = transform.position.y;
                zWUMin = transform.position.z - m_settings.m_spawnRange + (m_locationIncrement / 2f);
                zWUMax = zWUMin + (m_settings.m_spawnRange * 2f);
                jitMin = (-1f * m_maxJitteredLocationOffsetPct) * m_locationIncrement;
                jitMax = (1f * m_maxJitteredLocationOffsetPct) * m_locationIncrement;

                //Update checks
                currChecks = 0;
                totalChecks = (long)(((xWUMax - xWUMin) / m_locationIncrement) * ((zWUMax - zWUMin) / m_locationIncrement));

                //Iterate across these ranges
                for (xWU = xWUMin; xWU < xWUMax; xWU += m_locationIncrement)
                {
                    for (zWU = zWUMin; zWU < zWUMax; zWU += m_locationIncrement)
                    {
                        currChecks++;

                        //Set the location we want to test
                        location.x = xWU;
                        location.y = yMid;
                        location.z = zWU;

                        //Jitter it
                        if (m_spawnLocationAlgorithm == GaiaConstants.SpawnerLocation.EveryLocationJittered)
                        {
                            location.x += GetRandomFloat(jitMin, jitMax);
                            location.z += GetRandomFloat(jitMin, jitMax);
                        }

                        //Run a ray traced hit check to see what we have hit, use rules to determine fitness and select a rule to spawn
                        if (CheckLocation(location, ref spawnInfo))
                        {
                       

                        //Now perform a rule check based on the selected algorithm

                        //All rules
                        if (m_spawnRuleSelector == GaiaConstants.SpawnerRuleSelector.All)
                            {
                                for (ruleIdx = 0; ruleIdx < m_settings.m_spawnerRules.Count; ruleIdx++)
                                {
                                    rule = m_settings.m_spawnerRules[ruleIdx];
                                    spawnInfo.m_fitness = rule.GetFitness(ref spawnInfo);
                                    TryExecuteRule(ref rule, ref spawnInfo);
                                }
                            }

                            //Random spawn rule
                            else if (m_spawnRuleSelector == GaiaConstants.SpawnerRuleSelector.Random)
                            {
                                ruleIdx = GetRandomInt(0, m_settings.m_spawnerRules.Count - 1);
                                rule = m_settings.m_spawnerRules[ruleIdx];
                                spawnInfo.m_fitness = rule.GetFitness(ref spawnInfo);
                                TryExecuteRule(ref rule, ref spawnInfo);
                            }

                            //Fittest spawn rule
                            else if (m_spawnRuleSelector == GaiaConstants.SpawnerRuleSelector.Fittest)
                            {
                                fittestRule = null;
                                maxFitness = 0f;
                                for (ruleIdx = 0; ruleIdx < m_settings.m_spawnerRules.Count; ruleIdx++)
                                {
                                    rule = m_settings.m_spawnerRules[ruleIdx];
                                    fitness = rule.GetFitness(ref spawnInfo);
                                    if (fitness > maxFitness)
                                    {
                                        maxFitness = fitness;
                                        fittestRule = rule;
                                    }
                                    else
                                    {
                                        //If they are approx equal then give another rule a chance as well to add interest
                                        if (Gaia.GaiaUtils.Math_ApproximatelyEqual(fitness, maxFitness, 0.005f))
                                        {
                                            if (GetRandomFloat(0f, 1f) > 0.5f)
                                            {
                                                maxFitness = fitness;
                                                fittestRule = rule;
                                            }
                                        }
                                    }
                                }
                                spawnInfo.m_fitness = maxFitness;
                                TryExecuteRule(ref fittestRule, ref spawnInfo);
                            }

                            //Weighted fittest spawn rule - this implementation will favour fittest
                            else
                            {
                                fittestRule = selectedRule = null;
                                maxFitness = selectedFitness = 0f;
                                for (ruleIdx = 0; ruleIdx < m_settings.m_spawnerRules.Count; ruleIdx++)
                                {
                                    rule = m_settings.m_spawnerRules[ruleIdx];
                                    fitness = rule.GetFitness(ref spawnInfo);
                                    if (GetRandomFloat(0f, 1f) < fitness)
                                    {
                                        selectedRule = rule;
                                        selectedFitness = fitness;
                                    }
                                    if (fitness > maxFitness)
                                    {
                                        fittestRule = rule;
                                        maxFitness = fitness;
                                    }
                                }
                                //Check to see if we randomly bombed out - if so then choose fittest
                                if (selectedRule == null)
                                {
                                    selectedRule = fittestRule;
                                    selectedFitness = maxFitness;
                                }
                                //We could still bomb, check for this and avoid it
                                if (selectedRule != null)
                                {
                                    spawnInfo.m_fitness = selectedFitness;
                                    TryExecuteRule(ref selectedRule, ref spawnInfo);
                                }
                            }

                            //If it caused textures to be updated then apply them
                            if (m_textureMapsDirty)
                            {
                                List<HeightMap> txtMaps = spawnInfo.m_spawner.GetTextureMaps(spawnInfo.m_hitTerrain.GetInstanceID());
                                if (txtMaps != null)
                                {
                                    for (int idx = 0; idx < spawnInfo.m_textureStrengths.Length; idx++)
                                    {
                                        //if ((int)spawnInfo.m_hitLocationWU.z == 1023)
                                        //{
                                        //    Debug.Log("Woopee");
                                        //}

                                        txtMaps[idx][spawnInfo.m_hitLocationNU.z, spawnInfo.m_hitLocationNU.x] = spawnInfo.m_textureStrengths[idx];
                                    }
                                }
                            }

                        }

                        //Update progress and yield periodiocally
                        m_spawnProgress = (float)currChecks / (float)totalChecks;
                        float newTime = Time.realtimeSinceStartup;
                        float stepTime = newTime - currentTime;
                        currentTime = newTime;
                        accumulatedTime += stepTime;
                        if (accumulatedTime > m_updateTimeAllowed)
                        {
                            accumulatedTime = 0f;
                            yield return null;
                        }

                        //Check the instance count, exit if necessary
                        if (!CanSpawnInstances())
                        {
                            break;
                        }

                        //Check for cancelation
                        if (m_cancelSpawn == true)
                        {
                            break;
                        }
                    }
                }
            //}
            //Determine whether or not we need to delete and apply spawn caches
            DeleteSpawnCaches(true);

            //Perform final operations
            PostSpawn();
        }

        /// <summary>
        /// Ground the spawner to the terrain
        /// </summary>
        public void GroundToTerrain()
        {
            Terrain t = Gaia.TerrainHelper.GetTerrain(transform.position);
            if (t == null)
            {
                t = Terrain.activeTerrain;
            }
            if (t == null)
            {
                Debug.LogError("Could not fit to terrain - no terrain present");
                return;
            }

            Bounds b = new Bounds();
            if (TerrainHelper.GetTerrainBounds(t, ref b))
            {
                transform.position = new Vector3(transform.position.x, t.transform.position.y, transform.position.z);
            }
        }

        /// <summary>
        /// Position and fit the spawner to the terrain
        /// </summary>
        public void FitToTerrain(Terrain t = null)
        {
            if (t == null)
            {
                t = Gaia.TerrainHelper.GetTerrain(transform.position);
                if (t == null)
                {
                    t = Terrain.activeTerrain;
                }
                if (t == null)
                {
                    Debug.LogError("Could not fit to terrain - no terrain present");
                    return;
                }
            }

            Bounds b = new Bounds();
            if (TerrainHelper.GetTerrainBounds(t, ref b))
            {
                transform.position = new Vector3(b.center.x, t.transform.position.y, b.center.z);
                m_settings.m_spawnRange = b.extents.x;
            }
        }


        /// <summary>
        /// Position and fit the spawner to the terrain
        /// </summary>
        public void FitToAllTerrains()
        {
            if (Terrain.activeTerrain == null)
            {
                Debug.LogError("Could not fit to terrain - no active terrain present");
                return;
            }

            Bounds b = new Bounds();
            if (TerrainHelper.GetTerrainBounds(ref b))
            {
                transform.position = b.center;
                m_settings.m_spawnRange = b.extents.x;
            }
        }

        /// <summary>
        /// Check if the spawner has been fit to the terrain - ignoring height
        /// </summary>
        /// <returns>True if its a match</returns>
        public bool IsFitToTerrain()
        {
            Terrain t = Gaia.TerrainHelper.GetTerrain(transform.position);
            if (t == null)
            {
                t = Terrain.activeTerrain;
            }
            if (t == null)
            {
                Debug.LogError("Could not check if fit to terrain - no terrain present");
                return false;
            }

            Bounds b = new Bounds();
            if (TerrainHelper.GetTerrainBounds(t, ref b))
            {
                if (
                    b.center.x != transform.position.x ||
                    b.center.z != transform.position.z ||
                    b.extents.x != m_settings.m_spawnRange)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Load the image mask if one was specified
        /// </summary>
        public bool LoadImageMask()
        {
            //Kill old image height map
            m_imageMaskHM = null;

            //Check mode & exit 
            if (m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.None || m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.PerlinNoise)
            {
                return false;
            }

            //Load the supplied image
            if (m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageRedChannel || m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageGreenChannel ||
                m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageBlueChannel || m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageAlphaChannel ||
                m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageGreyScale)
            {
                if (m_imageMask == null)
                {
                    Debug.LogError("You requested an image mask but did not supply one. Please select mask texture.");
                    return false;
                }

                //Check the image rw
                Gaia.GaiaUtils.MakeTextureReadable(m_imageMask);

                //Make it uncompressed
                Gaia.GaiaUtils.MakeTextureUncompressed(m_imageMask);

                //Load the image
                m_imageMaskHM = new HeightMap(m_imageMask.width, m_imageMask.height);
                for (int x = 0; x < m_imageMaskHM.Width(); x++)
                {
                    for (int z = 0; z < m_imageMaskHM.Depth(); z++)
                    {
                        switch (m_areaMaskMode)
                        {
                            case GaiaConstants.ImageFitnessFilterMode.ImageGreyScale:
                                m_imageMaskHM[x, z] = m_imageMask.GetPixel(x, z).grayscale;
                                break;
                            case GaiaConstants.ImageFitnessFilterMode.ImageRedChannel:
                                m_imageMaskHM[x, z] = m_imageMask.GetPixel(x, z).r;
                                break;
                            case GaiaConstants.ImageFitnessFilterMode.ImageGreenChannel:
                                m_imageMaskHM[x, z] = m_imageMask.GetPixel(x, z).g;
                                break;
                            case GaiaConstants.ImageFitnessFilterMode.ImageBlueChannel:
                                m_imageMaskHM[x, z] = m_imageMask.GetPixel(x, z).b;
                                break;
                            case GaiaConstants.ImageFitnessFilterMode.ImageAlphaChannel:
                                m_imageMaskHM[x, z] = m_imageMask.GetPixel(x, z).a;
                                break;
                        }
                    }
                }
            }
            else
            {
                //Or get a new one
                if (Terrain.activeTerrain == null)
                {
                    Debug.LogError("You requested an terrain texture mask but there is no active terrain.");
                    return false;
                }


                Terrain t = Terrain.activeTerrain;
                var splatPrototypes = GaiaSplatPrototype.GetGaiaSplatPrototypes(t);


                switch (m_areaMaskMode)
                {
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture0:
                        if (splatPrototypes.Length < 1)
                        {
                            Debug.LogError("You requested an terrain texture mask 0 but there is no active texture in slot 0.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 0);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture1:
                        if (splatPrototypes.Length < 2)
                        {
                            Debug.LogError("You requested an terrain texture mask 1 but there is no active texture in slot 1.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 1);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture2:
                        if (splatPrototypes.Length < 3)
                        {
                            Debug.LogError("You requested an terrain texture mask 2 but there is no active texture in slot 2.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 2);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture3:
                        if (splatPrototypes.Length < 4)
                        {
                            Debug.LogError("You requested an terrain texture mask 3 but there is no active texture in slot 3.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 3);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture4:
                        if (splatPrototypes.Length < 5)
                        {
                            Debug.LogError("You requested an terrain texture mask 4 but there is no active texture in slot 4.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 4);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture5:
                        if (splatPrototypes.Length < 6)
                        {
                            Debug.LogError("You requested an terrain texture mask 5 but there is no active texture in slot 5.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 5);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture6:
                        if (splatPrototypes.Length < 7)
                        {
                            Debug.LogError("You requested an terrain texture mask 6 but there is no active texture in slot 6.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 6);
                        break;
                    case GaiaConstants.ImageFitnessFilterMode.TerrainTexture7:
                        if (splatPrototypes.Length < 8)
                        {
                            Debug.LogError("You requested an terrain texture mask 7 but there is no active texture in slot 7.");
                            return false;
                        }
                        m_imageMaskHM = new HeightMap(t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight), 7);
                        break;
                }

                //It came from terrain so flip it
                m_imageMaskHM.Flip();
            }

            //Because images are noisy, smooth it
            if (m_imageMaskSmoothIterations > 0)
            {
                m_imageMaskHM.Smooth(m_imageMaskSmoothIterations);
            }

            //Flip it
            if (m_imageMaskFlip == true)
            {
                m_imageMaskHM.Flip();
            }

            //Normalise it if necessary
            if (m_imageMaskNormalise == true)
            {
                m_imageMaskHM.Normalise();
            }

            //Invert it if necessessary
            if (m_imageMaskInvert == true)
            {
                m_imageMaskHM.Invert();
            }

            return true;
        }

        public void Spawn(bool allTerrains)
        {
            m_spawnComplete = false;

#if UNITY_EDITOR

            if (!Application.isPlaying)
            {
                m_updateCoroutine = RunSpawnerIteration(allTerrains);
                StartEditorUpdates();
            }
            else
            {
                StartCoroutine(RunSpawnerIteration(allTerrains));
            }
#else
                    StartCoroutine(RunRandomSpawnerIteration());
#endif
        }

        /// <summary>
        /// Create spawn caches
        /// </summary>
        /// <param name="checkResources">Base on resources or base on rules, takes active state into account</param>
        public void CreateSpawnCaches()
        {
            //Determine whether or not we need to cache updates, in which case we needs to get the relevant caches
            int idx;
            m_cacheTextures = false;
            m_textureMapsDirty = false;
            for (idx = 0; idx < m_settings.m_spawnerRules.Count; idx++)
            {
                if (m_settings.m_spawnerRules[idx].CacheTextures(this))
                {
                    foreach (Terrain t in Terrain.activeTerrains)
                    {
                        CacheTextureMapsFromTerrain(t.GetInstanceID());
                    }
                    m_cacheTextures = true;
                    break;
                }
            }

            m_cacheDetails = false;
            for (idx = 0; idx < m_settings.m_spawnerRules.Count; idx++)
            {
                if (m_settings.m_spawnerRules[idx].CacheDetails(this))
                {
                    foreach (Terrain t in Terrain.activeTerrains)
                    {
                        CacheDetailMapsFromTerrain(t.GetInstanceID());
                    }
                    m_cacheDetails = true;
                    break;
                }
            }

            CacheTreesFromTerrain();

            m_cacheTags = false;
            List<string> tagList = new List<string>();
            for (idx = 0; idx < m_settings.m_spawnerRules.Count; idx++)
            {
                m_settings.m_spawnerRules[idx].AddProximityTags(this, ref tagList);
            }
            if (tagList.Count > 0)
            {
                CacheTaggedGameObjectsFromScene(tagList);
                m_cacheTags = true;
            }

            m_cacheHeightMaps = false;
            for (idx = 0; idx < m_settings.m_spawnerRules.Count; idx++)
            {
                if (m_settings.m_spawnerRules[idx].CacheHeightMaps(this))
                {
                    CacheHeightMapFromTerrain(Terrain.activeTerrain.GetInstanceID());
                    m_cacheHeightMaps = true;
                    break;
                }
            }

            /*
            m_cacheStamps = false;
            List<string> stampList = new List<string>();
            for (idx = 0; idx < m_spawnerRules.Count; idx++)
            {
                m_spawnerRules[idx].AddStamps(this, ref stampList);
            }
            if (stampList.Count > 0)
            {
                CacheStamps(stampList);
                m_cacheStamps = true;
            } */
        }

        /// <summary>
        /// Create spawn cache fore specific resources
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="resourceIdx"></param>
        public void CreateSpawnCaches(Gaia.GaiaConstants.SpawnerResourceType resourceType, int resourceIdx)
        {
            m_cacheTextures = false;
            m_textureMapsDirty = false;
            m_cacheDetails = false;
            m_cacheTags = false;

            switch (resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        //Check indexes
                        if (resourceIdx >= m_settings.m_resources.m_texturePrototypes.Length)
                        {
                            break;
                        }

                        //If we are working with textures, then always cache the texture
                        foreach (Terrain t in Terrain.activeTerrains)
                        {
                            CacheTextureMapsFromTerrain(t.GetInstanceID());
                        }
                        m_cacheTextures = true;

                        //Check for proximity tags
                        List<string> tagList = new List<string>();
                        m_settings.m_resources.m_texturePrototypes[resourceIdx].AddTags(ref tagList);
                        if (tagList.Count > 0)
                        {
                            CacheTaggedGameObjectsFromScene(tagList);
                            m_cacheTags = true;
                        }

                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        //Check indexes
                        if (resourceIdx >= m_settings.m_resources.m_detailPrototypes.Length)
                        {
                            break;
                        }

                        //If we are working with details, always cache details
                        foreach (Terrain t in Terrain.activeTerrains)
                        {
                            CacheDetailMapsFromTerrain(t.GetInstanceID());
                        }
                        m_cacheDetails = true;

                        //Check for textures
                        if (m_settings.m_resources.m_detailPrototypes[resourceIdx].ChecksTextures())
                        {
                            foreach (Terrain t in Terrain.activeTerrains)
                            {
                                CacheTextureMapsFromTerrain(t.GetInstanceID());
                            }
                            m_cacheTextures = true;
                        }

                        //Check for proximity tags
                        List<string> tagList = new List<string>();
                        m_settings.m_resources.m_detailPrototypes[resourceIdx].AddTags(ref tagList);
                        if (tagList.Count > 0)
                        {
                            CacheTaggedGameObjectsFromScene(tagList);
                            m_cacheTags = true;
                        }

                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        //Check indexes
                        if (resourceIdx >= m_settings.m_resources.m_treePrototypes.Length)
                        {
                            break;
                        }

                        //Cache textures
                        if (m_settings.m_resources.m_treePrototypes[resourceIdx].ChecksTextures())
                        {
                            foreach (Terrain t in Terrain.activeTerrains)
                            {
                                CacheTextureMapsFromTerrain(t.GetInstanceID());
                            }
                            m_cacheTextures = true;
                        }

                        //Cache trees
                        CacheTreesFromTerrain();

                        //Cache proximity tags
                        List<string> tagList = new List<string>();
                        m_settings.m_resources.m_treePrototypes[resourceIdx].AddTags(ref tagList);
                        if (tagList.Count > 0)
                        {
                            CacheTaggedGameObjectsFromScene(tagList);
                            m_cacheTags = true;
                        }

                        break;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        //Check indexes
                        if (resourceIdx >= m_settings.m_resources.m_gameObjectPrototypes.Length)
                        {
                            break;
                        }

                        //Check for textures
                        if (m_settings.m_resources.m_gameObjectPrototypes[resourceIdx].ChecksTextures())
                        {
                            foreach (Terrain t in Terrain.activeTerrains)
                            {
                                CacheTextureMapsFromTerrain(t.GetInstanceID());
                            }
                            m_cacheTextures = true;
                        }

                        //Check for proximity tags
                        List<string> tagList = new List<string>();
                        m_settings.m_resources.m_gameObjectPrototypes[resourceIdx].AddTags(ref tagList);
                        if (tagList.Count > 0)
                        {
                            CacheTaggedGameObjectsFromScene(tagList);
                            m_cacheTags = true;
                        }

                        break;
                    }

                    /*
                default:
                    {
                        //Check indexes
                        if (resourceIdx >= m_resources.m_stampPrototypes.Length)
                        {
                            break;
                        }

                        //Check for textures
                        if (m_resources.m_stampPrototypes[resourceIdx].ChecksTextures())
                        {
                            CacheTextureMapsFromTerrain(Terrain.activeTerrain.GetInstanceID());
                            m_cacheTextures = true;
                        }

                        //Check for proximity tags
                        List<string> tagList = new List<string>();
                        m_resources.m_gameObjectPrototypes[resourceIdx].AddTags(ref tagList);
                        if (tagList.Count > 0)
                        {
                            CacheTaggedGameObjectsFromScene(tagList);
                            m_cacheTags = true;
                        }

                        //We are influencing terrain - so we always cache terrain
                        CacheHeightMapFromTerrain(Terrain.activeTerrain.GetInstanceID());
                        m_cacheHeightMaps = true;

                        break;
                    }
                     */
            }
        }


        /// <summary>
        /// Destroy spawn caches
        /// </summary>
        /// <param name="flush">Fluch changes back to the environment</param>
        public void DeleteSpawnCaches(bool flushDirty = false)
        {
            //Determine whether or not we need to apply cache updates
            if (m_cacheTextures)
            {
                if (flushDirty && m_textureMapsDirty && m_cancelSpawn != true)
                {
                    m_textureMapsDirty = false;
                    foreach (Terrain t in Terrain.activeTerrains)
                    {
                        SaveTextureMapsToTerrain(t.GetInstanceID());
                    }
                }
                DeleteTextureMapCache();
                m_cacheTextures = false;
            }

            if (m_cacheDetails)
            {
                if (m_cancelSpawn != true)
                {
                    foreach (Terrain t in Terrain.activeTerrains)
                    {
                        SaveDetailMapsToTerrain(t.GetInstanceID());
                    }
                }
                DeleteDetailMapCache();
                m_cacheDetails = false;
            }

            if (m_cacheTags)
            {
                DeleteTagCache();
                m_cacheTags = false;
            }

            if (m_cacheHeightMaps)
            {
                if (flushDirty && m_heightMapDirty && m_cancelSpawn != true)
                {
                    m_heightMapDirty = false;
                    SaveHeightMapToTerrain(Terrain.activeTerrain.GetInstanceID());
                }
                DeleteHeightMapCache();
                m_cacheHeightMaps = false;
            }
        }

        /// <summary>
        /// Attempt to execute a rule taking fitness, failure rate and instances into account
        /// </summary>
        /// <param name="rule">The rule to execute</param>
        /// <param name="spawnInfo">The related spawninfo</param>
        public bool TryExecuteRule(ref SpawnRule rule, ref SpawnInfo spawnInfo)
        {
            //Check null
            if (rule != null)
            {
                //Check instances
                if (rule.m_ignoreMaxInstances || (rule.m_activeInstanceCnt < rule.m_maxInstances))
                {
                    //Update fitness based on distance evaluation
                    spawnInfo.m_fitness *= m_spawnFitnessAttenuator.Evaluate(Mathf.Clamp01(spawnInfo.m_hitDistanceWU / m_settings.m_spawnRange));

                    //Udpate fitness based on area mask 
                    if (m_areaMaskMode != GaiaConstants.ImageFitnessFilterMode.None)
                    {
                        if (m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.PerlinNoise || 
                            m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.BillowNoise ||
                            m_areaMaskMode == GaiaConstants.ImageFitnessFilterMode.RidgedNoise)
                        {
                            if (!m_noiseInvert)
                            {
                                spawnInfo.m_fitness *= m_noiseGenerator.GetNormalisedValue(100000f + (spawnInfo.m_hitLocationWU.x * (1f / m_noiseZoom)), 100000f + (spawnInfo.m_hitLocationWU.z * (1f / m_noiseZoom)));
                            }
                            else
                            {
                                spawnInfo.m_fitness *= (1f - m_noiseGenerator.GetNormalisedValue(100000f + (spawnInfo.m_hitLocationWU.x * (1f / m_noiseZoom)), 100000f + (spawnInfo.m_hitLocationWU.z * (1f / m_noiseZoom))));
                            }
                        }
                        else
                        {
                            if (m_imageMaskHM.HasData())
                            {
                                float x = (spawnInfo.m_hitLocationWU.x - (transform.position.x - m_settings.m_spawnRange)) / (m_settings.m_spawnRange * 2f);
                                float z = (spawnInfo.m_hitLocationWU.z - (transform.position.z - m_settings.m_spawnRange)) / (m_settings.m_spawnRange * 2f);
                                spawnInfo.m_fitness *= m_imageMaskHM[x, z];
                            }
                        }
                    }

                    //Check fitness
                    if (spawnInfo.m_fitness > rule.m_minRequiredFitness)
                    {
                        //Only spawn if we pass a random failure check
                        if (GetRandomFloat(0f, 1f) > rule.m_failureRate)
                        {
                            rule.Spawn(ref spawnInfo);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This is a fairly expensive raycast based location check that is capable of detecting things like tree collider hits on the terrain.
        /// It will return the name and height of the thing that was hit, plus some underlying terrain information. In the scenario of terrain tree
        /// hits you can comparing height of the rtaycast hit against the height of the terrain to detect this.
        /// It will return true plus details if something is hit, otherwise false.
        /// </summary>
        /// <param name="locationWU">The location we are checking in world units</param>
        /// <param name="spawnInfo">The information we gather about this location</param>
        /// <returns>True if we hit something, false otherwise</returns>
        public bool CheckLocation(Vector3 locationWU, ref SpawnInfo spawnInfo)
        {
            //Some initialisation
            spawnInfo.m_spawner = this;
            spawnInfo.m_outOfBounds = true;
            spawnInfo.m_wasVirginTerrain = false;
            spawnInfo.m_spawnRotationY = 0f;
            spawnInfo.m_hitDistanceWU = Vector3.Distance(transform.position, locationWU);
            spawnInfo.m_hitLocationWU = locationWU;
            spawnInfo.m_hitNormal = Vector3.zero;
            spawnInfo.m_hitObject = null;
            spawnInfo.m_hitTerrain = null;
            spawnInfo.m_terrainNormalWU = Vector3.one;
            spawnInfo.m_terrainHeightWU = 0f;
            spawnInfo.m_terrainSlopeWU = 0f;
            spawnInfo.m_areaHitSlopeWU = 0f;
            spawnInfo.m_areaMinSlopeWU = 0f;
            spawnInfo.m_areaAvgSlopeWU = 0f;
            spawnInfo.m_areaMaxSlopeWU = 0f;

            //Make sure we are above it
            locationWU.y = m_terrainHeight + 1000f;

            //Run a ray traced hit check to see what we have hit - if we dont get a hit then we are off terrain and will ignore
            if (Physics.Raycast(locationWU, Vector3.down, out m_checkHitInfo, Mathf.Infinity, m_spawnCollisionLayers))
            {
                //If its a grass spawner, and we got a sphere collider, try again so that we ignore the sphere collider
                if (spawnInfo.m_spawner.IsDetailSpawner())
                {
                    if ((m_checkHitInfo.collider is SphereCollider || m_checkHitInfo.collider is CapsuleCollider) && m_checkHitInfo.collider.name == "_GaiaCollider_Grass")
                    {
                        //Drop it slightly and run it again
                        locationWU.y = m_checkHitInfo.point.y - 0.01f;

                        //Run the raycast again - it should hit something
                        if (!Physics.Raycast(locationWU, Vector3.down, out m_checkHitInfo, Mathf.Infinity, m_spawnCollisionLayers))
                        {
                            return false;
                        }
                    }
                }


                //Update spawnInfo
                spawnInfo.m_hitLocationWU = m_checkHitInfo.point;
                spawnInfo.m_hitDistanceWU = Vector3.Distance(transform.position, spawnInfo.m_hitLocationWU);
                spawnInfo.m_hitNormal = m_checkHitInfo.normal;
                spawnInfo.m_hitObject = m_checkHitInfo.transform;

                //Check distance - bomb out if out of range
                if (m_spawnerShape == GaiaConstants.SpawnerShape.Box)
                {
                    if (!m_spawnerBounds.Contains(spawnInfo.m_hitLocationWU))
                    {
                        return false;
                    }
                }
                else
                {
                    if (spawnInfo.m_hitDistanceWU > m_settings.m_spawnRange)
                    {
                        return false;
                    }
                }
                spawnInfo.m_outOfBounds = false;

                //Gather some terrain info at this location
                Terrain terrain;
                if (m_checkHitInfo.collider is TerrainCollider)
                {
                    terrain = m_checkHitInfo.transform.GetComponent<Terrain>();
                    spawnInfo.m_wasVirginTerrain = true; //It might be virgin terrain
                }
                else
                {
                    terrain = Gaia.TerrainHelper.GetTerrain(m_checkHitInfo.point);
                }

                if (terrain != null)
                {
                    spawnInfo.m_hitTerrain = terrain;
                    spawnInfo.m_terrainHeightWU = terrain.SampleHeight(m_checkHitInfo.point);
                    Vector3 terrainLocalPos = terrain.transform.InverseTransformPoint(m_checkHitInfo.point);
                    Vector3 normalizedPos = new Vector3(Mathf.InverseLerp(0.0f, terrain.terrainData.size.x, terrainLocalPos.x),
                                                        Mathf.InverseLerp(0.0f, terrain.terrainData.size.y, terrainLocalPos.y),
                                                        Mathf.InverseLerp(0.0f, terrain.terrainData.size.z, terrainLocalPos.z));
                    spawnInfo.m_hitLocationNU = normalizedPos;
                    spawnInfo.m_terrainSlopeWU = terrain.terrainData.GetSteepness(normalizedPos.x, normalizedPos.z);
                    spawnInfo.m_areaHitSlopeWU = spawnInfo.m_areaMinSlopeWU = spawnInfo.m_areaAvgSlopeWU = spawnInfo.m_areaMaxSlopeWU = spawnInfo.m_terrainSlopeWU;
                    spawnInfo.m_terrainNormalWU = terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.z);

                    //Check for virgin terrain now that we know actual terrain height - difference will be tree colliders
                    if (spawnInfo.m_wasVirginTerrain == true)
                    {
                        //Use the tree manager to do hits on trees
                        if (spawnInfo.m_spawner.m_treeCache.Count(spawnInfo.m_hitLocationWU, 0.5f) > 0)
                        {
                            spawnInfo.m_wasVirginTerrain = false;
                        }
                    }

                    //Set up the texture layer array in spawn info
                    spawnInfo.m_textureStrengths = new float[spawnInfo.m_hitTerrain.terrainData.alphamapLayers];

                    //Grab the textures
                    if (m_textureMapCache != null && m_textureMapCache.Count > 0)
                    {
                        List<HeightMap> hms = m_textureMapCache[terrain.GetInstanceID()];
                        for (int i = 0; i < spawnInfo.m_textureStrengths.Length; i++)
                        {
                            spawnInfo.m_textureStrengths[i] = hms[i][normalizedPos.z, normalizedPos.x];
                        }
                    }
                    else
                    {
                        float[, ,] hms = terrain.terrainData.GetAlphamaps((int)(normalizedPos.x * (float)(terrain.terrainData.alphamapWidth-1)), (int)(normalizedPos.z * (float)(terrain.terrainData.alphamapHeight-1)), 1, 1);
                        for (int i = 0; i < spawnInfo.m_textureStrengths.Length; i++)
                        {
                            spawnInfo.m_textureStrengths[i] = hms[0, 0, i];
                        }
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// This will do a bounded location check in order to calculate bounded slopes and checkd for bounded collisions
        /// </summary>
        /// <param name="spawnInfo"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public bool CheckLocationBounds(ref SpawnInfo spawnInfo, float distance)
        {
            //Initialise
            spawnInfo.m_areaHitSlopeWU = spawnInfo.m_areaMinSlopeWU = spawnInfo.m_areaAvgSlopeWU = spawnInfo.m_areaMaxSlopeWU = spawnInfo.m_terrainSlopeWU;
            if (spawnInfo.m_areaHitsWU == null)
            {
                spawnInfo.m_areaHitsWU = new Vector3[4];
            }
            spawnInfo.m_areaHitsWU[0] = new Vector3(spawnInfo.m_hitLocationWU.x + distance, spawnInfo.m_hitLocationWU.y + 3000f, spawnInfo.m_hitLocationWU.z);
            spawnInfo.m_areaHitsWU[1] = new Vector3(spawnInfo.m_hitLocationWU.x - distance, spawnInfo.m_hitLocationWU.y + 3000f, spawnInfo.m_hitLocationWU.z);
            spawnInfo.m_areaHitsWU[2] = new Vector3(spawnInfo.m_hitLocationWU.x , spawnInfo.m_hitLocationWU.y + 3000f, spawnInfo.m_hitLocationWU.z + distance);
            spawnInfo.m_areaHitsWU[3] = new Vector3(spawnInfo.m_hitLocationWU.x, spawnInfo.m_hitLocationWU.y + 3000f, spawnInfo.m_hitLocationWU.z - distance);

            //Run ray traced hits to check the lay of the land - if we dont get a hit then we are off terrain and will fail
            RaycastHit hit;

            //First check the main volume under the original position for non terrain related hits
            Vector3 extents = new Vector3(distance, 0.1f, distance);
            if (!Physics.BoxCast(new Vector3(spawnInfo.m_hitLocationWU.x, spawnInfo.m_hitLocationWU.y + 3000f, spawnInfo.m_hitLocationWU.z), extents, Vector3.down, out hit, Quaternion.identity, Mathf.Infinity, m_spawnCollisionLayers))
            //if (!Physics.SphereCast(new Vector3(spawnInfo.m_hitLocationWU.x, spawnInfo.m_hitLocationWU.y + 3000f, spawnInfo.m_hitLocationWU.z), distance, Vector3.down, out hit, Mathf.Infinity, m_spawnCollisionLayers))
            {
                return false;
            }

            //Test virginity
            if (spawnInfo.m_wasVirginTerrain == true)
            {
                if (hit.collider is TerrainCollider)
                {
                    //Use the tree manager to do hits on trees
                    if (spawnInfo.m_spawner.m_treeCache.Count(hit.point, 0.5f) > 0)
                    {
                        spawnInfo.m_wasVirginTerrain = false;
                    }
                }
                else
                {
                    spawnInfo.m_wasVirginTerrain = false;
                }
            }

            //Now test the first corner
            if (!Physics.Raycast(spawnInfo.m_areaHitsWU[0], Vector3.down, out hit, Mathf.Infinity, m_spawnCollisionLayers))
            {
                return false;
            }

            //Update hit location
            spawnInfo.m_areaHitsWU[0] = hit.point;

            //Update slope calculations
            Terrain terrain = hit.transform.GetComponent<Terrain>();
            if (terrain == null)
            {
                terrain = Gaia.TerrainHelper.GetTerrain(hit.point);
            }
            Vector3 localPos = Vector3.zero;
            Vector3 normPos = Vector3.zero;
            //float terrainHeight = 0f;
            float terrainSlope = 0f;

            if (terrain != null)
            {
                //terrainHeight = terrain.SampleHeight(hit.point);
                localPos = terrain.transform.InverseTransformPoint(hit.point);
                normPos = new Vector3(Mathf.InverseLerp(0.0f, terrain.terrainData.size.x, localPos.x),
                                                    Mathf.InverseLerp(0.0f, terrain.terrainData.size.y, localPos.y),
                                                    Mathf.InverseLerp(0.0f, terrain.terrainData.size.z, localPos.z));
                terrainSlope = terrain.terrainData.GetSteepness(normPos.x, normPos.z);
                spawnInfo.m_areaAvgSlopeWU += terrainSlope;
                if (terrainSlope > spawnInfo.m_areaMaxSlopeWU)
                {
                    spawnInfo.m_areaMaxSlopeWU = terrainSlope;
                }
                if (terrainSlope < spawnInfo.m_areaMinSlopeWU)
                {
                    spawnInfo.m_areaMinSlopeWU = terrainSlope;
                }

                //Check for virginity
                if (spawnInfo.m_wasVirginTerrain == true)
                {
                    if (hit.collider is TerrainCollider)
                    {
                        if (spawnInfo.m_spawner.m_treeCache.Count(hit.point, 0.5f) > 0)
                        {
                            spawnInfo.m_wasVirginTerrain = false;
                        }
                    }
                    else
                    {
                        spawnInfo.m_wasVirginTerrain = false;
                    }
                }
            }

            //Now test the next corner
            if (!Physics.Raycast(spawnInfo.m_areaHitsWU[1], Vector3.down, out hit, Mathf.Infinity, m_spawnCollisionLayers))
            {
                return false;
            }

            //Update hit location
            spawnInfo.m_areaHitsWU[1] = hit.point;

            //Update slope calculations
            terrain = hit.transform.GetComponent<Terrain>();
            if (terrain == null)
            {
                terrain = Gaia.TerrainHelper.GetTerrain(hit.point);
            }
            if (terrain != null)
            {
                //terrainHeight = terrain.SampleHeight(hit.point);
                localPos = terrain.transform.InverseTransformPoint(hit.point);
                normPos = new Vector3(Mathf.InverseLerp(0.0f, terrain.terrainData.size.x, localPos.x),
                                                    Mathf.InverseLerp(0.0f, terrain.terrainData.size.y, localPos.y),
                                                    Mathf.InverseLerp(0.0f, terrain.terrainData.size.z, localPos.z));
                terrainSlope = terrain.terrainData.GetSteepness(normPos.x, normPos.z);
                spawnInfo.m_areaAvgSlopeWU += terrainSlope;
                if (terrainSlope > spawnInfo.m_areaMaxSlopeWU)
                {
                    spawnInfo.m_areaMaxSlopeWU = terrainSlope;
                }
                if (terrainSlope < spawnInfo.m_areaMinSlopeWU)
                {
                    spawnInfo.m_areaMinSlopeWU = terrainSlope;
                }

                //Check for virginity
                if (spawnInfo.m_wasVirginTerrain == true)
                {
                    if (hit.collider is TerrainCollider)
                    {
                        if (spawnInfo.m_spawner.m_treeCache.Count(hit.point, 0.5f) > 0)
                        {
                            spawnInfo.m_wasVirginTerrain = false;
                        }
                    }
                    else
                    {
                        spawnInfo.m_wasVirginTerrain = false;
                    }
                }
            }

            //Now test the next corner
            if (!Physics.Raycast(spawnInfo.m_areaHitsWU[2], Vector3.down, out hit, Mathf.Infinity, m_spawnCollisionLayers))
            {
                return false;
            }

            //Update hit location
            spawnInfo.m_areaHitsWU[2] = hit.point;

            //Update slope calculations
            terrain = hit.transform.GetComponent<Terrain>();
            if (terrain == null)
            {
                terrain = Gaia.TerrainHelper.GetTerrain(hit.point);
            }
            if (terrain != null)
            {
                //terrainHeight = terrain.SampleHeight(hit.point);
                localPos = terrain.transform.InverseTransformPoint(hit.point);
                normPos = new Vector3(Mathf.InverseLerp(0.0f, terrain.terrainData.size.x, localPos.x),
                                                    Mathf.InverseLerp(0.0f, terrain.terrainData.size.y, localPos.y),
                                                    Mathf.InverseLerp(0.0f, terrain.terrainData.size.z, localPos.z));
                terrainSlope = terrain.terrainData.GetSteepness(normPos.x, normPos.z);
                spawnInfo.m_areaAvgSlopeWU += terrainSlope;
                if (terrainSlope > spawnInfo.m_areaMaxSlopeWU)
                {
                    spawnInfo.m_areaMaxSlopeWU = terrainSlope;
                }
                if (terrainSlope < spawnInfo.m_areaMinSlopeWU)
                {
                    spawnInfo.m_areaMinSlopeWU = terrainSlope;
                }

                //Check for virginity
                if (spawnInfo.m_wasVirginTerrain == true)
                {
                    if (hit.collider is TerrainCollider)
                    {
                        if (spawnInfo.m_spawner.m_treeCache.Count(hit.point, 0.5f) > 0)
                        {
                            spawnInfo.m_wasVirginTerrain = false;
                        }
                    }
                    else
                    {
                        spawnInfo.m_wasVirginTerrain = false;
                    }
                }
            }

            //Now test the next corner
            if (!Physics.Raycast(spawnInfo.m_areaHitsWU[3], Vector3.down, out hit, Mathf.Infinity, m_spawnCollisionLayers))
            {
                return false;
            }

            //Update hit location
            spawnInfo.m_areaHitsWU[3] = hit.point;

            //Update slope calculations
            terrain = hit.transform.GetComponent<Terrain>();
            if (terrain == null)
            {
                terrain = Gaia.TerrainHelper.GetTerrain(hit.point);
            }
            if (terrain != null)
            {
                //terrainHeight = terrain.SampleHeight(hit.point);
                localPos = terrain.transform.InverseTransformPoint(hit.point);
                normPos = new Vector3(Mathf.InverseLerp(0.0f, terrain.terrainData.size.x, localPos.x),
                                                    Mathf.InverseLerp(0.0f, terrain.terrainData.size.y, localPos.y),
                                                    Mathf.InverseLerp(0.0f, terrain.terrainData.size.z, localPos.z));
                terrainSlope = terrain.terrainData.GetSteepness(normPos.x, normPos.z);
                spawnInfo.m_areaAvgSlopeWU += terrainSlope;
                if (terrainSlope > spawnInfo.m_areaMaxSlopeWU)
                {
                    spawnInfo.m_areaMaxSlopeWU = terrainSlope;
                }
                if (terrainSlope < spawnInfo.m_areaMinSlopeWU)
                {
                    spawnInfo.m_areaMinSlopeWU = terrainSlope;
                }

                //Check for virginity
                if (spawnInfo.m_wasVirginTerrain == true)
                {
                    if (hit.collider is TerrainCollider)
                    {
                        if (spawnInfo.m_spawner.m_treeCache.Count(hit.point, 0.5f) > 0)
                        {
                            spawnInfo.m_wasVirginTerrain = false;
                        }
                    }
                    else
                    {
                        spawnInfo.m_wasVirginTerrain = false;
                    }
                }
            }

            //Now update the slopes and spawninfo
            spawnInfo.m_areaAvgSlopeWU = spawnInfo.m_areaAvgSlopeWU / 5f;
            float dx = spawnInfo.m_areaHitsWU[0].y - spawnInfo.m_areaHitsWU[1].y;
            float dz = spawnInfo.m_areaHitsWU[2].y - spawnInfo.m_areaHitsWU[3].y;
            spawnInfo.m_areaHitSlopeWU = Gaia.GaiaUtils.Math_Clamp(0f, 90f, (float)(Math.Sqrt((dx * dx) + (dz * dz))));
            
            return true;
        }


        public bool CheckForMissingResources()
        {
            AssociateAssets();
            int[] missingResources = GetMissingResources();
            if (missingResources.GetLength(0) > 0)
            {
                SpawnRule missingRule;
                StringBuilder sb = new StringBuilder();
                for (int idx = 0; idx < missingResources.GetLength(0); idx++)
                {
                    missingRule = m_settings.m_spawnerRules[missingResources[idx]];
                    if (idx != 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(missingRule.m_name);
                }
#if UNITY_EDITOR
                if (EditorUtility.DisplayDialog("WARNING!", "The following resources are missing from the terrain! " + sb.ToString(), "Add Them Now ?", "Cancel"))
                {
                    AddResourcesToTerrain(missingResources);
                }
                else
                {
                    return true;
                }
#endif
            }

            return false;

        }




        /// <summary>
        /// Update statistics counters
        /// </summary>
        public void UpdateCounters()
        {
            //m_totalRuleCnt = 0;
            //m_activeRuleCnt = 0;
            //m_inactiveRuleCnt = 0;
            //m_maxInstanceCnt = 0;
            //m_activeInstanceCnt = 0;
            //m_inactiveInstanceCnt = 0;
            //m_totalInstanceCnt = 0;

            //foreach (SpawnRule rule in m_settings.m_spawnerRules)
            //{
            //    m_totalRuleCnt++;
            //    if (rule.m_isActive)
            //    {
            //        m_activeRuleCnt++;
            //        m_maxInstanceCnt += rule.m_maxInstances;
            //        m_activeInstanceCnt += rule.m_activeInstanceCnt;
            //        m_inactiveInstanceCnt += rule.m_inactiveInstanceCnt;
            //        m_totalInstanceCnt += (rule.m_activeInstanceCnt + rule.m_inactiveInstanceCnt);
            //    }
            //    else
            //    {
            //        m_inactiveRuleCnt++;
            //    }
            //}
        }

        /// <summary>
        /// Draw gizmos
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (m_showGizmos)
            {
                if (m_showBoundingBox)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(transform.position, new Vector3(m_settings.m_spawnRange * 2f, m_settings.m_spawnRange * 2f, m_settings.m_spawnRange * 2f));
                }
               
                //Water
                if (m_settings.m_resources != null && m_showSeaLevelPlane)
                {
                    Bounds bounds = new Bounds();
                    if (TerrainHelper.GetTerrainBounds(ref bounds) == true)
                    {
                        bounds.center = new Vector3(bounds.center.x, GaiaSessionManager.GetSessionManager(false).GetSeaLevel(), bounds.center.z);
                        bounds.size = new Vector3(bounds.size.x, 0.05f, bounds.size.z);
                        Gizmos.color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, Color.blue.a / 4f);
                        Gizmos.DrawCube(bounds.center, bounds.size);
                    }
                }
            }

            //Update the counters
            UpdateCounters();
        }

#region Texture map management

        /// <summary>
        /// Cache the texture maps for the terrain object id supplied - this is very memory intensive so use with care!
        /// </summary>
        public void CacheTextureMapsFromTerrain(int terrainID)
        {
            //Construct them of we dont have them
            if (m_textureMapCache == null)
            {
                m_textureMapCache = new Dictionary<int, List<HeightMap>>();
            }

            //Now find the terrain and load them for the specified terrain
            Terrain terrain;
            for (int terrIdx = 0; terrIdx < Terrain.activeTerrains.Length; terrIdx++)
            {
                terrain = Terrain.activeTerrains[terrIdx];
                if (terrain.GetInstanceID() == terrainID)
                {
                    float[, ,] splatMaps = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
                    List<HeightMap> textureMapList = new List<HeightMap>();
                    for (int txtIdx = 0; txtIdx < terrain.terrainData.alphamapLayers; txtIdx++)
                    {
                        HeightMap txtMap = new HeightMap(splatMaps, txtIdx);
                        textureMapList.Add(txtMap);
                    }
                    m_textureMapCache[terrainID] = textureMapList;
                    return;
                }
            }
            Debug.LogError("Attempted to get textures on terrain that does not exist!");
        }

        /// <summary>
        /// Get the detail map list for the terrain
        /// </summary>
        /// <param name="terrainID">Object id of the terrain</param>
        /// <returns>Detail map list or null</returns>
        public List<HeightMap> GetTextureMaps(int terrainID)
        {
            List<HeightMap> mapList;
            if (!m_textureMapCache.TryGetValue(terrainID, out mapList))
            {
                return null;
            }
            return mapList;
        }

        /// <summary>
        /// Save the texture maps back into the terrain
        /// </summary>
        /// <param name="terrainID">ID of the terrain to do this for</param>
        public void SaveTextureMapsToTerrain(int terrainID)
        {
            Terrain terrain;
            HeightMap txtMap;
            List<HeightMap> txtMapList;

            //Make sure we can find it
            if (!m_textureMapCache.TryGetValue(terrainID, out txtMapList))
            {
                Debug.LogError("Texture map list was not found for terrain ID : " + terrainID + " !");
                return;
            }

            //Abort if we dont have anything in the list
            if (txtMapList.Count <= 0)
            {
                Debug.LogError("Texture map list was empty for terrain ID : " + terrainID + " !");
                return;
            }

            //Locate the terrain
            for (int terrIdx = 0; terrIdx < Terrain.activeTerrains.Length; terrIdx++)
            {
                terrain = Terrain.activeTerrains[terrIdx];
                if (terrain.GetInstanceID() == terrainID)
                {
                    //Make sure that the number of prototypes matches up
                    if (txtMapList.Count != terrain.terrainData.alphamapLayers)
                    {
                        Debug.LogError("Texture map prototype list does not match terrain prototype list for terrain ID : " + terrainID + " !");
                        return;
                    }

                    float[,,] splatMaps = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, terrain.terrainData.alphamapLayers];
                    for (int txtIdx = 0; txtIdx < terrain.terrainData.alphamapLayers; txtIdx++)
                    {
                        txtMap = txtMapList[txtIdx];
                        for (int x = 0; x < txtMap.Width(); x++)
                        {
                            for (int z = 0; z < txtMap.Depth(); z++)
                            {
                                splatMaps[x, z, txtIdx] = txtMap[x, z];
                            }
                        }
                    }
                    terrain.terrainData.SetAlphamaps(0,0, splatMaps);
                    return;
                }
            }
            Debug.LogError("Attempted to locate a terrain that does not exist!");
        }

        /// <summary>
        /// Remove the texture maps from memory
        /// </summary>
        public void DeleteTextureMapCache()
        {
            m_textureMapCache = new Dictionary<int, List<HeightMap>>();
        }

        /// <summary>
        /// Set the texture maps dirty if we modified them
        /// </summary>
        public void SetTextureMapsDirty()
        {
            m_textureMapsDirty = true;
        }

#endregion

#region Detail map management

        /// <summary>
        /// Get the detail maps for the terrain object id supplied - this is very memory intensive so use with care!
        /// </summary>
        public void CacheDetailMapsFromTerrain(int terrainID)
        {
            //Construct them of we dont have them
            if (m_detailMapCache == null)
            {
                m_detailMapCache = new Dictionary<int, List<HeightMap>>();
            }

            //Now find the terrain and load them for the specified terrain
            Terrain terrain;
            for (int terrIdx = 0; terrIdx < Terrain.activeTerrains.Length; terrIdx++)
            {
                terrain = Terrain.activeTerrains[terrIdx];
                if (terrain.GetInstanceID() == terrainID)
                {
                    List<HeightMap> detailMapList = new List<HeightMap>();
                    for (int dtlIdx = 0; dtlIdx < terrain.terrainData.detailPrototypes.Length; dtlIdx++)
                    {
                        HeightMap dtlMap = new HeightMap(terrain.terrainData.GetDetailLayer(0,0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, dtlIdx));
                        detailMapList.Add(dtlMap);
                    }
                    m_detailMapCache[terrainID] = detailMapList;
                    return;
                }
            }
            Debug.LogError("Attempted to get details on terrain that does not exist!");
        }

        /// <summary>
        /// Save the detail maps back into the terrain
        /// </summary>
        /// <param name="terrainID">ID of the terrain to do this for</param>
        public void SaveDetailMapsToTerrain(int terrainID)
        {
            Terrain terrain;
            HeightMap dtlMap;
            List<HeightMap> dtlMapList;

            //Make sure we can find it
            if (!m_detailMapCache.TryGetValue(terrainID, out dtlMapList))
            {
                Debug.LogWarning(gameObject.name + "Detail map list was not found for terrain ID : " + terrainID + " !");
                return;
            }

            //Abort if we dont have anything in the list
            if (dtlMapList.Count <= 0)
            {
                Debug.LogWarning(gameObject.name + ": Detail map list was empty for terrain ID : " + terrainID + " !");
                return;
            }

            //Locate the terrain
            for (int terrIdx = 0; terrIdx < Terrain.activeTerrains.Length; terrIdx++)
            {
                terrain = Terrain.activeTerrains[terrIdx];
                if (terrain.GetInstanceID() == terrainID)
                {
                    //Make sure that the number of prototypes matches up
                    if (dtlMapList.Count != terrain.terrainData.detailPrototypes.Length)
                    {
                        Debug.LogError("Detail map protoype list does not match terrain prototype list for terrain ID : " + terrainID + " !");
                        return;
                    }

                    //Mow iterate thru and apply back
                    int[,] dtlMapArray = new int[dtlMapList[0].Width(), dtlMapList[0].Depth()];
                    for (int dtlIdx = 0; dtlIdx < terrain.terrainData.detailPrototypes.Length; dtlIdx++)
                    {
                        dtlMap = dtlMapList[dtlIdx];
                        for (int x = 0; x < dtlMap.Width(); x++)
                        {
                            for (int z = 0; z < dtlMap.Depth(); z++)
                            {
                                dtlMapArray[x,z] = (int)dtlMap[x, z];
                            }
                        }
                        terrain.terrainData.SetDetailLayer(0, 0, dtlIdx, dtlMapArray);
                    }
                    terrain.Flush();
                    return;
                }
            }
            Debug.LogError("Attempted to locate a terrain that does not exist!");
        }

        /// <summary>
        /// Get the detail map list for the terrain
        /// </summary>
        /// <param name="terrainID">Object id of the terrain</param>
        /// <returns>Detail map list or null</returns>
        public List<HeightMap> GetDetailMaps(int terrainID)
        {
            List<HeightMap> mapList;
            if (!m_detailMapCache.TryGetValue(terrainID, out mapList))
            {
                return null;
            }
            return mapList;
        }

        /// <summary>
        /// Get the detail map for the specific detail
        /// </summary>
        /// <param name="terrainID">Terrain to query</param>
        /// <param name="detailIndex">Detail prototype index</param>
        /// <returns>Detail heightmap or null if not found</returns>
        public HeightMap GetDetailMap(int terrainID, int detailIndex)
        {
            List<HeightMap> dtlMapList;
            if (!m_detailMapCache.TryGetValue(terrainID, out dtlMapList))
            {
                return null;
            }
            if (detailIndex >= 0 && detailIndex < dtlMapList.Count)
            {
                return dtlMapList[detailIndex];
            }
            return null;
        }

        /// <summary>
        /// Remove the detail maps from memory
        /// </summary>
        public void DeleteDetailMapCache()
        {
            m_detailMapCache = new Dictionary<int, List<HeightMap>>();
        }

#endregion

#region Tree Management

        public void CacheTreesFromTerrain()
        {
            m_treeCache.LoadTreesFromTerrain();
        }

        public void DeleteTreeCache()
        {
            m_treeCache = new TreeManager();
        }

#endregion

#region Sessions and Serialisation

        /// <summary>
        /// Add the operationm to the session manager
        /// </summary>
        /// <param name="opType">The type of operation to add</param>
        public void AddToSession(GaiaOperation.OperationType opType, string opName)
        {
            //Update the session
            GaiaSessionManager sessionMgr = GaiaSessionManager.GetSessionManager();
            if (sessionMgr != null && sessionMgr.IsLocked() != true)
            {
                GaiaOperation op = new GaiaOperation();
                op.m_description = opName;
                op.m_generatedByID = m_spawnerID;
                op.m_generatedByName = transform.name;
                op.m_generatedByType = this.GetType().ToString();
                op.m_isActive = true;
                op.m_operationDateTime = DateTime.Now.ToString();
                op.m_operationType = opType;
                op.m_operationDataJson = new string[1];
                op.m_operationDataJson[0] = this.SerialiseJson();
                sessionMgr.AddOperation(op);
                sessionMgr.AddResource(m_settings.m_resources);
            }
        }

        /// <summary>
        /// Serialise this as json
        /// </summary>
        /// <returns></returns>
        public string SerialiseJson()
        {
            //Grab the various paths
            //#if UNITY_EDITOR
            //            m_settings.m_resourcesPath = AssetDatabase.GetAssetPath(m_settings.m_resources);
            //#endif

            //            fsData data;
            //            fsSerializer serializer = new fsSerializer();
            //            serializer.TrySerialize(this, out data);

            //            //Debug.Log(fsJsonPrinter.PrettyJson(data));

            //           return fsJsonPrinter.CompressedJson(data);
            return "";
        }

        /// <summary>
        /// Deserialise the suplied json into this object
        /// </summary>
        /// <param name="json">Source json</param>
        public void DeSerialiseJson(string json)
        {
            //fsData data = fsJsonParser.Parse(json);
            //fsSerializer serializer = new fsSerializer();
            //var spawner = this;
            //serializer.TryDeserialize<Spawner>(data, ref spawner);
            //spawner.m_settings.m_resources = GaiaUtils.GetAsset(m_settings.m_resourcesPath, typeof(Gaia.GaiaResource)) as Gaia.GaiaResource;
        }

#endregion

#region Handy helpers

        /// <summary>
        /// Flatten all active terrains
        /// </summary>
        public void FlattenTerrain()
        {
            //Update the session
            AddToSession(GaiaOperation.OperationType.FlattenTerrain, "Flattening terrain");

            //Get an undo buffer
            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            mgr.FlattenWorld();
        }

        /// <summary>
        /// Smooth all active terrains
        /// </summary>
        public void SmoothTerrain()
        {
            //Update the session
            AddToSession(GaiaOperation.OperationType.SmoothTerrain, "Smoothing terrain");

            //Smooth the world
            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            mgr.SmoothWorld();
        }

        /// <summary>
        /// Clear trees off all actiove terrains
        /// </summary>
        public void ClearTrees()
        {
            //Update the session
            AddToSession(GaiaOperation.OperationType.ClearTrees, "Clearing terrain trees");
            TerrainHelper.ClearTrees();
            //iterate through all spawners, reset counter for tree rules
            foreach (Spawner spawner in Resources.FindObjectsOfTypeAll<Spawner>())
            {
                foreach (SpawnRule spawnRule in spawner.m_settings.m_spawnerRules)
                {
                    if (spawnRule.m_resourceType == SpawnerResourceType.TerrainTree)
                    {
                        spawnRule.m_spawnedInstances = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Clear all the grass off all the terrains
        /// </summary>
        public void ClearDetails()
        {
            //Update the session
            AddToSession(GaiaOperation.OperationType.ClearDetails, "Clearing terrain details");
            TerrainHelper.ClearDetails();
            //iterate through all spawners, reset counter for terrain detail rules
            foreach (Spawner spawner in Resources.FindObjectsOfTypeAll<Spawner>())
            {
                foreach (SpawnRule spawnRule in spawner.m_settings.m_spawnerRules)
                {
                    if (spawnRule.m_resourceType == SpawnerResourceType.TerrainDetail)
                    {
                        spawnRule.m_spawnedInstances = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Clear all the GameObjects created by this spawner off all the terrains
        /// </summary>
        public void ClearGameObjects()
        {
            //Update the session
            AddToSession(GaiaOperation.OperationType.ClearGameObjects, "Clearing game objects");
            //Just iterate through the child transforms and delete them all, done.
            for(int i = transform.childCount-1;i>=0;i--) 
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            //reset the counters for all game object spawn rules for this spawner only
            foreach (SpawnRule spawnRule in m_settings.m_spawnerRules)
            {
                if (spawnRule.m_resourceType == SpawnerResourceType.GameObject)
                {
                    spawnRule.m_spawnedInstances = 0;
                }
            }
        }

        public static bool HandleAutoSpawnerStack(List<AutoSpawner> autoSpawners, bool allTerrains)
        {
            AutoSpawner nextSpawner = autoSpawners.Find(x => x.status == AutoSpawnerStatus.Spawning);
            if (nextSpawner != null)
            {
                if (nextSpawner.spawner.IsSpawning())
                {
                    return false;
                    //Do Nothing, still spawning
                }
                else
                {
                    //Auto Spawner is done, look for next spawner
                    GaiaUtils.DisplayProgressBarNoEditor("Spawning", "Preparing next Spawner...",0);
                    nextSpawner.status = AutoSpawnerStatus.Done;
                    nextSpawner = autoSpawners.Find(x => x.status == AutoSpawnerStatus.Queued);
                    
                }
            }
            else
            {
                //No spawner spawning atm, let's pick the first queued one
                nextSpawner = autoSpawners.Find(x => x.status == AutoSpawnerStatus.Queued);
            }

            if (nextSpawner != null)
            {
                if (!nextSpawner.spawner.IsSpawning())
                {
                    //nextSpawner.spawner.transform.position = new Vector3(m_stamper.transform.position.x, nextSpawner.spawner.transform.position.y, m_stamper.transform.position.z);
                    //Terrain terrain = nextSpawner.spawner.GetCurrentTerrain();
                    //nextSpawner.spawner.m_settings.m_spawnRange = terrain.terrainData.size.x * (m_stamper.m_settings.m_width / 100f);
                    nextSpawner.spawner.UpdateMinMaxHeight();
                    nextSpawner.spawner.Spawn(allTerrains);
                    nextSpawner.status = AutoSpawnerStatus.Spawning;
                }
                return false;
                
            }
            else
            {
                //no spawners left
               GaiaUtils.ClearProgressBarNoEditor();
               GaiaUtils.ReleaseAllTempRenderTextures();
                return true;
                
            }
        }
      
#endregion

#region Height map management

        /// <summary>
        /// Cache the height map for the terrain object id supplied - this is very memory intensive so use with care!
        /// </summary>
        public void CacheHeightMapFromTerrain(int terrainID)
        {
            //Construct them of we dont have them
            if (m_heightMapCache == null)
            {
                m_heightMapCache = new Dictionary<int, UnityHeightMap>();
            }

            //Now find the terrain and load them for the specified terrain
            Terrain terrain;
            for (int terrIdx = 0; terrIdx < Terrain.activeTerrains.Length; terrIdx++)
            {
                terrain = Terrain.activeTerrains[terrIdx];
                if (terrain.GetInstanceID() == terrainID)
                {
                    m_heightMapCache[terrainID] = new UnityHeightMap(terrain);
                    return;
                }
            }
            Debug.LogError("Attempted to get height maps on a terrain that does not exist!");
        }

        /// <summary>
        /// Get the height map for the terrain
        /// </summary>
        /// <param name="terrainID">Object id of the terrain</param>
        /// <returns>Heightmap or null</returns>
        public UnityHeightMap GetHeightMap(int terrainID)
        {
            UnityHeightMap heightmap;
            if (!m_heightMapCache.TryGetValue(terrainID, out heightmap))
            {
                return null;
            }
            return heightmap;
        }

        /// <summary>
        /// Save the height map back into the terrain
        /// </summary>
        /// <param name="terrainID">ID of the terrain to do this for</param>
        public void SaveHeightMapToTerrain(int terrainID)
        {
            Terrain terrain;
            UnityHeightMap heightmap;

            //Make sure we can find it
            if (!m_heightMapCache.TryGetValue(terrainID, out heightmap))
            {
                Debug.LogError("Heightmap was not found for terrain ID : " + terrainID + " !");
                return;
            }

            //Locate the terrain and update it
            for (int terrIdx = 0; terrIdx < Terrain.activeTerrains.Length; terrIdx++)
            {
                terrain = Terrain.activeTerrains[terrIdx];
                if (terrain.GetInstanceID() == terrainID)
                {
                    heightmap.SaveToTerrain(terrain);
                    return;
                }
            }
            Debug.LogError("Attempted to locate a terrain that does not exist!");
        }

        /// <summary>
        /// Remove the texture maps from memory
        /// </summary>
        public void DeleteHeightMapCache()
        {
            m_heightMapCache = new Dictionary<int, UnityHeightMap>();
        }

        /// <summary>
        /// Set the height maps dirty if we modified them
        /// </summary>
        public void SetHeightMapsDirty()
        {
            m_heightMapDirty = true;
        }

#endregion

#region Stamp management

        public void CacheStamps(List<string> stampList)
        {
            //Construct them of we dont have them
            if (m_stampCache == null)
            {
                m_stampCache = new Dictionary<string, HeightMap>();
            }

            //Get the list of stamps for this spawner
            for (int idx = 0; idx < stampList.Count; idx++)
            {



            }
        }


#endregion

#region Tag management

        /// <summary>
        /// Load all the tags in the scene into the tag cache
        /// </summary>
        /// <param name="tagList"></param>
        private void CacheTaggedGameObjectsFromScene(List<string>tagList)
        {
            //Create a new cache (essentially releasing the old one)
            m_taggedGameObjectCache = new Dictionary<string, Quadtree<GameObject>>();

            //Now load all the tagged objects into the cache
            string tag;
            bool foundTag;
            Quadtree<GameObject> quadtree;
            Rect pos = new Rect(Terrain.activeTerrain.transform.position.x, Terrain.activeTerrain.transform.position.z, 
                Terrain.activeTerrain.terrainData.size.x, Terrain.activeTerrain.terrainData.size.z);

            for (int tagIdx = 0; tagIdx < tagList.Count; tagIdx++)
            {
                //Check that unity knows about the tag

                tag = tagList[tagIdx].Trim();
                foundTag = false;
                if (!string.IsNullOrEmpty(tag))
                {
#if UNITY_EDITOR
                    for (int idx = 0; idx < UnityEditorInternal.InternalEditorUtility.tags.Length; idx++)
                    {
                        if (UnityEditorInternal.InternalEditorUtility.tags[idx].Contains(tag))
                        {
                            foundTag = true;
                            break;
                        }
                    }
#else
                    foundTag = true;
#endif
                }

                //If its good then cache it
                if (foundTag)
                {
                    quadtree = null;
                    if (!m_taggedGameObjectCache.TryGetValue(tag, out quadtree))
                    {
                        quadtree = new Quadtree<GameObject>(pos);
                        m_taggedGameObjectCache.Add(tag, quadtree);
                    }
                    GameObject go;
                    Vector2 go2DPos;
                    GameObject[] gos = GameObject.FindGameObjectsWithTag(tag);
                    for (int goIdx = 0; goIdx < gos.Length; goIdx++)
                    {
                        go = gos[goIdx];

                        //Only add it if within our bounds
                        go2DPos = new Vector2(go.transform.position.x, go.transform.position.z);
                        if (pos.Contains(go2DPos))
                        {
                            quadtree.Insert(go2DPos, go);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete the tag cache
        /// </summary>
        private void DeleteTagCache()
        {
            m_taggedGameObjectCache = null;
        }

        /// <summary>
        /// Get the objects that match the tag list within the defined area
        /// </summary>
        /// <param name="tagList">List of tags to search</param>
        /// <param name="area">Area to search</param>
        /// <returns></returns>
        public List<GameObject> GetNearbyObjects(List<string>tagList, Rect area)
        {
            string tag;
            List<GameObject> gameObjects = new List<GameObject>();
            Quadtree<GameObject> quadtree;
            for (int tagIdx = 0; tagIdx < tagList.Count; tagIdx++)
            {
                quadtree = null;
                tag = tagList[tagIdx];

                //Process each tag
                if (m_taggedGameObjectCache.TryGetValue(tag, out quadtree))
                {
                    IEnumerable<GameObject> gameObjs = quadtree.Find(area);
                    foreach (GameObject go in gameObjs)
                    {
                        gameObjects.Add(go);
                    }
                }
            }
            return gameObjects;
        }

        /// <summary>
        /// Get the closest gameobject to the centre of the area supplied that matches the tag list
        /// </summary>
        /// <param name="tagList">List of tags to search</param>
        /// <param name="area">The area to search</param>
        /// <returns></returns>
        public GameObject GetClosestObject(List<string> tagList, Rect area)
        {
            string tag;
            float distance;
            float closestDistance = float.MaxValue;
            GameObject closestGo = null;
            Quadtree<GameObject> quadtree;
            for (int tagIdx = 0; tagIdx < tagList.Count; tagIdx++)
            {
                quadtree = null;
                tag = tagList[tagIdx];

                //Process each tag
                if (m_taggedGameObjectCache.TryGetValue(tag, out quadtree))
                {
                    IEnumerable<GameObject> gameObjs = quadtree.Find(area);
                    foreach (GameObject go in gameObjs)
                    {
                        distance = Vector2.Distance(area.center, new Vector2(go.transform.position.x, go.transform.position.z));
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestGo = go;
                        }
                    }
                }
            }
            return closestGo;
        }

        /// <summary>
        /// Get the closest gameobject to the centre of the area supplied that matches the tag 
        /// </summary>
        /// <param name="tagList">Tag to search for</param>
        /// <param name="area">The area to search</param>
        /// <returns></returns>
        public GameObject GetClosestObject(string tag, Rect area)
        {
            float distance, closestDistance = float.MaxValue;
            GameObject closestGo = null;
            Quadtree<GameObject> quadtree = null;

            if (m_taggedGameObjectCache.TryGetValue(tag, out quadtree))
            {
                IEnumerable<GameObject> gameObjs = quadtree.Find(area);
                foreach (GameObject go in gameObjs)
                {
                    distance = Vector2.Distance(area.center, new Vector2(go.transform.position.x, go.transform.position.z));
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestGo = go;
                    }
                }
            }
            return closestGo;
        }


        #endregion

        #region Saving and Loading

        public void LoadSettings(SpawnerSettings settingsToLoad)
        {
            //Set existing settings = null to force a new scriptable object
            m_settings = null;

            m_settings = Instantiate(settingsToLoad); 
            //GaiaUtils.CopyFields(settingsToLoad, m_settings);

            //close down all foldouts neatly when freshly loaded
            foreach (SpawnRule rule in m_settings.m_spawnerRules)
            {
                rule.m_isFoldedOut = false;
                rule.m_resourceSettingsFoldedOut = false;
            }
            m_rulePanelUnfolded = true;
        }
        #endregion

        #region Random number utils

        /// <summary>
        /// Reset the random number generator
        /// </summary>
        public void ResetRandomGenertor()
        {
            m_rndGenerator = new XorshiftPlus(m_seed);
        }

        /// <summary>
        /// Get a random integer
        /// </summary>
        /// <param name="min">Minimum value inclusive</param>
        /// <param name="max">Maximum value inclusive</param>
        /// <returns>Random integer between minimum and maximum values</returns>
        public int GetRandomInt(int min, int max)
        {
            return m_rndGenerator.Next(min, max);
        }

        /// <summary>
        /// Get a random float
        /// </summary>
        /// <param name="min">Minimum value inclusive</param>
        /// <param name="max">Maximum value inclusive</param>
        /// <returns>Random float between minimum and maximum values</returns>
        public float GetRandomFloat(float min, float max)
        {
            return m_rndGenerator.Next(min, max);
        }

        /// <summary>
        /// Get a random vector 3
        /// </summary>
        /// <param name="range">Range of values to return</param>
        /// <returns>Vector 3 in the +- range supplied</returns>
        public Vector3 GetRandomV3(float range)
        {
            return m_rndGenerator.NextVector(-range, range);
        }

#endregion
    }
}
