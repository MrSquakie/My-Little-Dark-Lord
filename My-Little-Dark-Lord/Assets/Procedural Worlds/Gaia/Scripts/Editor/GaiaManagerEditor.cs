using Gaia.Internal;
using PWCommon2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.PackageManager;
using UnityEngine.Experimental.Rendering;
using System.Linq;
using UnityEditorInternal;
using UnityEditor.Callbacks;
#endif
#if UNITY_2018_3_OR_NEWER
using UnityEngine.Networking;
#endif
using UnityEditor.SceneManagement;
using Gaia.Pipeline.LWRP;
using Gaia.Pipeline.HDRP;
using Gaia.Pipeline;

namespace Gaia
{
    /// <summary>
    /// Handy helper for all things Gaia
    /// </summary>
    public class GaiaManagerEditor : EditorWindow, IPWEditor
    {
        #region Variables, Properties
        private GUIStyle m_boxStyle;
        private GUIStyle m_wrapStyle;
        private GUIStyle m_titleStyle;
        private GUIStyle m_headingStyle;
        private GUIStyle m_bodyStyle;
        private GUIStyle m_linkStyle;
        private GaiaSettings m_settings;
        private UnityPipelineProfile m_gaiaPipelineSettings;
        private IEnumerator m_updateCoroutine;
        private EditorUtils m_editorUtils;

        private TabSet m_mainTabs;
        private TabSet m_moreTabs;

        //Extension manager
        bool m_needsScan = true;
        GaiaExtensionManager m_extensionMgr = new GaiaExtensionManager();
        //private bool m_foldoutSession = false;
        //private bool m_foldoutTerrain = false;
        //private bool m_foldoutSpawners = false;
        //private bool m_foldoutCharacters = false;
        //private bool m_foldoutUtils = false;
        private GaiaConstants.EnvironmentSize m_targetSize;
        private GaiaConstants.EnvironmentTarget m_targetEnv;
        private bool m_foldoutTerrainResolutionSettings = false;

        // Icon tests
        private Texture2D m_stdIcon;
        private Texture2D m_advIcon;
        private Texture2D m_gxIcon;
        private Texture2D m_moreIcon;

        //Bool system checks
        private bool m_shadersNotImported;
        private bool m_showSetupPanel;
        private bool m_enableGUI;
        private Color m_defaultPanelColor;

        //Water Profiles
        private string m_unityVersion;
        private List<string> m_profileList = new List<string>();
        private List<Material> m_allMaterials = new List<Material>();
        private int newProfileListIndex = 0;
        private const string m_materialLocation = "/Procedural Worlds/Gaia/Gaia Lighting and Water/Gaia Water/Ocean Water/Resources/Material";

        //Terrain resolution settings
        private GaiaConstants.HeightmapResolution m_heightmapResolution;
        private GaiaConstants.TerrainTextureResolution m_controlTextureResolution;
        private GaiaConstants.TerrainTextureResolution m_basemapResolution;
        private int m_detailResolutionPerPatch;
        private int m_detailResolution;
        private int m_biomePresetSelection = int.MinValue;
        private List<BiomePresetDropdownEntry> m_allBiomePresets = new List<BiomePresetDropdownEntry>();
        private List<BiomeSpawnerListEntry> m_BiomeSpawnersToCreate = new List<BiomeSpawnerListEntry>();
        private List<BiomeSpawnerListEntry> m_advancedTabAllSpawners = new List<BiomeSpawnerListEntry>();
        private List<AdvancedTabBiomeListEntry> m_advancedTabAllBiomes = new List<AdvancedTabBiomeListEntry>();
        private UnityEditorInternal.ReorderableList m_biomeSpawnersList;
        private UnityEditorInternal.ReorderableList m_advancedTabBiomesList;
        private UnityEditorInternal.ReorderableList m_advancedTabSpawnersList;
        private bool m_foldoutSpawnerSettings;
        private GaiaConstants.EnvironmentSizePreset m_targeSizePreset = GaiaConstants.EnvironmentSizePreset.Large;
        private bool m_foldOutWorldSizeSettings;
        private GUIStyle m_helpStyle;
        private bool m_foldoutExtrasSettings;
        private bool m_advancedTabFoldoutSpawners;
        private bool m_advancedTabFoldoutBiomes;

        //Private const strings
        private const string m_vegetationShaderKeyWord = "Vegetation";
        private const string m_waterShaderKeyWord = "Water";
        private const string m_builtInKeyWord = "SP";
        private const string m_lightweightKeyWord = "LW";
        private const string m_highDefinitionKeyWord = "HD";

        public bool PositionChecked { get; set; }
        #endregion

        #region Gaia Menu Items
        /// <summary>
        /// Show Gaia Manager editor window
        /// </summary>
        [MenuItem("Window/" + PWConst.COMMON_MENU + "/Gaia/Show Gaia Manager... %g", false, 40)]
        public static void ShowGaiaManager()
        {
            var manager = EditorWindow.GetWindow<Gaia.GaiaManagerEditor>(false, "Gaia Manager");
            //Manager can be null if the dependency package installation is started upon opening the manager window.
            if (manager != null)
            {
                manager.Show();
            }
        }

        ///// <summary>
        ///// Show the forum
        ///// </summary>
        //[MenuItem("Window/Gaia/Show Forum...", false, 60)]
        //public static void ShowForum()
        //{
        //    Application.OpenURL(
        //        "http://www.procedural-worlds.com/forum/gaia/");
        //}

        /// <summary>
        /// Show documentation
        /// </summary>
        [MenuItem("Window/" + PWConst.COMMON_MENU + "/Gaia/Show Extensions...", false, 65)]
        public static void ShowExtensions()
        {
            Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=gaia-extensions");
        }
        #endregion

        #region Constructors destructors and related delegates

        /// <summary>
        /// Setup on destroy
        /// </summary>
        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }
        }

        /// <summary>
        /// See if we can preload the manager with existing settings
        /// </summary>
        public void OnEnable()
        {
            m_defaultPanelColor = GUI.backgroundColor;

            if (EditorGUIUtility.isProSkin)
            {
                if (m_stdIcon == null)
                {
                    m_stdIcon = Resources.Load("gstdIco_p") as Texture2D;
                }
                if (m_advIcon == null)
                {
                    m_advIcon = Resources.Load("gadvIco_p") as Texture2D;
                }
                if (m_gxIcon == null)
                {
                    m_gxIcon = Resources.Load("ggxIco_p") as Texture2D;
                }
                if (m_moreIcon == null)
                {
                    m_moreIcon = Resources.Load("gmoreIco_p") as Texture2D;
                }
            }
            else
            {
                if (m_stdIcon == null)
                {
                    m_stdIcon = Resources.Load("gstdIco") as Texture2D;
                }
                if (m_advIcon == null)
                {
                    m_advIcon = Resources.Load("gadvIco") as Texture2D;
                }
                if (m_gxIcon == null)
                {
                    m_gxIcon = Resources.Load("ggxIco") as Texture2D;
                }
                if (m_moreIcon == null)
                {
                    m_moreIcon = Resources.Load("gmoreIco") as Texture2D;
                }
            }

            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }

            var mainTabs = new Tab[] {
                new Tab ("Standard", m_stdIcon, StandardTab),
                new Tab ("Advanced", m_advIcon, AdvancedTab),
                new Tab ("GX", m_gxIcon, ExtensionsTab),
                new Tab ("More...", m_moreIcon, MoreTab),
            };

            var moreTabs = new Tab[] {
                new Tab ("Tutorials & Support", TutorialsAndSupportTab),
                new Tab ("Partners & Extensions", MoreOnProceduralWorldsTab),
            };

            m_mainTabs = new TabSet(m_editorUtils, mainTabs);
            m_moreTabs = new TabSet(m_editorUtils, moreTabs);

            //Signal we need a scan
            m_needsScan = true;

            //Set the Gaia directories up
            GaiaUtils.CreateGaiaStampDirectories();

            //Get or create existing settings object
            if (m_settings == null)
            {
                m_settings = (GaiaSettings)PWCommon2.AssetUtils.GetAssetScriptableObject("GaiaSettings");
                if (m_settings == null)
                {
                    m_settings = CreateSettingsAsset();
                }
            }

            GaiaManagerStatusCheck();

            SetupWaterList(m_settings);

            //Make sure we have defaults
            if (m_settings.m_currentDefaults == null)
            {
                m_settings.m_currentDefaults = (GaiaDefaults)PWCommon2.AssetUtils.GetAssetScriptableObject("GaiaDefaults");
                EditorUtility.SetDirty(m_settings);
            }

            //Initialize editor resolution settings with defaults
            if (m_settings.m_currentDefaults != null)
            {
                m_targetSize = (GaiaConstants.EnvironmentSize)m_settings.m_currentSize;
                m_targetEnv = (GaiaConstants.EnvironmentTarget)m_settings.m_currentEnvironment;
                m_heightmapResolution = (GaiaConstants.HeightmapResolution)m_settings.m_currentDefaults.m_heightmapResolution;
                m_controlTextureResolution = (GaiaConstants.TerrainTextureResolution)m_settings.m_currentDefaults.m_controlTextureResolution;
                m_basemapResolution = (GaiaConstants.TerrainTextureResolution)m_settings.m_currentDefaults.m_baseMapSize;
                m_detailResolutionPerPatch = m_settings.m_currentDefaults.m_detailResolutionPerPatch;
                m_detailResolution = m_settings.m_currentDefaults.m_detailResolution;
            }


            //Not required anymore with new spawner system
            //Grab first resource we can find
            //if (m_settings.m_currentResources == null)
            //{
            //    m_settings.m_currentResources = (GaiaResource)PWCommon1.AssetUtils.GetAssetScriptableObject("GaiaResources");
            //    EditorUtility.SetDirty(m_settings);
            //}

            ////Grab first game object resource we can find
            //if (m_settings.m_currentGameObjectResources == null)
            //{
            //    m_settings.m_currentGameObjectResources = m_settings.m_currentResources;
            //    EditorUtility.SetDirty(m_settings);
            //}

            if (!Application.isPlaying)
            {
                StartEditorUpdates();
                m_updateCoroutine = GetNewsUpdate();
            }

            m_settings = GaiaUtils.GetGaiaSettings();
            if (m_settings == null)
            {
                Debug.Log("Gaia Settings are missing from our project, please make sure Gaia settings is in your project.");
                return;
            }

            m_gaiaPipelineSettings = m_settings.m_pipelineProfile;

            //Sets up the render to the correct pipeline
            if (GraphicsSettings.renderPipelineAsset == null)
            {
                m_settings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
                m_settings.m_pipelineProfile.m_activePipelineInstalled = GaiaConstants.EnvironmentRenderer.BuiltIn;
            }
            else if (GraphicsSettings.renderPipelineAsset.GetType().ToString().Contains("HDRenderPipelineAsset"))
            {
                m_settings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.HighDefinition;
            }
            else
            {
                m_settings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.Lightweight;
            }

            string[] allSpawnerPresetGUIDs = AssetDatabase.FindAssets("t:BiomePreset");

            for (int i = 0; i < allSpawnerPresetGUIDs.Length; i++)
            {
                BiomePreset sp = (BiomePreset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(allSpawnerPresetGUIDs[i]), typeof(BiomePreset));
                if (sp != null)
                {
                    m_allBiomePresets.Add(new BiomePresetDropdownEntry { ID = i, name = sp.name, biomePreset = sp });
                }
            }
            m_allBiomePresets.Sort();
            m_biomePresetSelection = m_allBiomePresets[0].ID;
            //Add the artifical "Custom" option
            m_allBiomePresets.Add(new BiomePresetDropdownEntry { ID = -999, name = "Custom", biomePreset = null });

            if (m_biomePresetSelection != int.MinValue)
            {
                //Fill in initial content
                AddBiomeSpawnersForSelectedPreset();




                CreateBiomePresetList();
            }

            CreateAdvancedTabBiomesList();

            CreateAdvancedTabSpawnersList();
        }

        private void AddBiomeSpawnersForSelectedPreset()
        {
            m_BiomeSpawnersToCreate.Clear();

            BiomePresetDropdownEntry entry = m_allBiomePresets.Find(x => x.ID == m_biomePresetSelection);
            if (entry.biomePreset != null)
            {

                //Need to create a deep copy of the preset list, otherwise the users will overwrite it when they add custom spawners
                foreach (BiomeSpawnerListEntry spawnerListEntry in entry.biomePreset.m_spawnerPresetList)
                {
                    if (spawnerListEntry.m_spawnerSettings != null)
                    {
                        m_BiomeSpawnersToCreate.Add(spawnerListEntry);
                    }
                }
            }
        }

        private void CreateAdvancedTabBiomesList()
        {
            m_advancedTabAllBiomes.Clear();
            string[] allBiomeGUIDS = AssetDatabase.FindAssets("t:BiomePreset");

            for (int i = 0; i < allBiomeGUIDS.Length; i++)
            {
                BiomePreset biomePreset = (BiomePreset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(allBiomeGUIDS[i]), typeof(BiomePreset));
                if (biomePreset != null)
                {
                    m_advancedTabAllBiomes.Add(new AdvancedTabBiomeListEntry { m_autoAssignPrototypes = true, m_biomePreset = biomePreset });
                }
            }
            m_advancedTabAllBiomes.Sort();

            m_advancedTabBiomesList = new UnityEditorInternal.ReorderableList(m_advancedTabAllBiomes, typeof(AdvancedTabBiomeListEntry), false, true, false, false);
            m_advancedTabBiomesList.drawElementCallback = DrawAdvancedTabBiomeListElement;
            m_advancedTabBiomesList.drawHeaderCallback = DrawAdvancedTabBiomeListHeader;
            m_advancedTabBiomesList.elementHeightCallback = OnElementHeightSpawnerPresetListEntry;


        }

        private void DrawAdvancedTabBiomeListHeader(Rect rect)
        {
            BiomeListEditor.DrawListHeader(rect, true, m_advancedTabAllBiomes, m_editorUtils, "AdvancedTabBiomeListHeader");
        }

        private void DrawAdvancedTabBiomeListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            BiomeListEditor.DrawListElement_AdvancedTab(rect, m_advancedTabAllBiomes[index], m_editorUtils);
        }

        private void CreateAdvancedTabSpawnersList()
        {
            m_advancedTabAllSpawners.Clear();
            string[] allSpawnerGUIDs = AssetDatabase.FindAssets("t:SpawnerSettings");

            for (int i = 0; i < allSpawnerGUIDs.Length; i++)
            {
                SpawnerSettings spawnerSettings = (SpawnerSettings)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(allSpawnerGUIDs[i]), typeof(SpawnerSettings));
                if (spawnerSettings != null)
                {
                    m_advancedTabAllSpawners.Add(new BiomeSpawnerListEntry { m_autoAssignPrototypes = true, m_spawnerSettings = spawnerSettings });
                }
            }
            m_advancedTabAllSpawners.Sort();


            m_advancedTabSpawnersList = new UnityEditorInternal.ReorderableList(m_advancedTabAllSpawners, typeof(BiomeSpawnerListEntry), false, true, false, false);
            m_advancedTabSpawnersList.elementHeightCallback = OnElementHeightSpawnerPresetListEntry;
            m_advancedTabSpawnersList.drawElementCallback = DrawAdvancedTabSpawnerListElement;
            m_advancedTabSpawnersList.drawHeaderCallback = DrawAdvancedTabSpawnerPresetListHeader;
            m_advancedTabSpawnersList.onAddCallback = OnAddSpawnerPresetListEntry;
            m_advancedTabSpawnersList.onRemoveCallback = OnRemoveSpawnerPresetListEntry;
            m_advancedTabSpawnersList.onReorderCallback = OnReorderSpawnerPresetList;
        }

        /// <summary>
        /// Settings up settings on disable
        /// </summary>
        void OnDisable()
        {
            StopEditorUpdates();
        }

        #region Spawner Preset List

        void CreateBiomePresetList()
        {
            m_biomeSpawnersList = new UnityEditorInternal.ReorderableList(m_BiomeSpawnersToCreate, typeof(BiomeSpawnerListEntry), true, true, true, true);
            m_biomeSpawnersList.elementHeightCallback = OnElementHeightSpawnerPresetListEntry;
            m_biomeSpawnersList.drawElementCallback = DrawSpawnerPresetListElement; ;
            m_biomeSpawnersList.drawHeaderCallback = DrawSpawnerPresetListHeader;
            m_biomeSpawnersList.onAddCallback = OnAddSpawnerPresetListEntry;
            m_biomeSpawnersList.onRemoveCallback = OnRemoveSpawnerPresetListEntry;
            m_biomeSpawnersList.onReorderCallback = OnReorderSpawnerPresetList;
        }

        private void OnReorderSpawnerPresetList(ReorderableList list)
        {
            //Do nothing, changing the order does not immediately affect anything in this window
        }

        private void OnRemoveSpawnerPresetListEntry(ReorderableList list)
        {
            m_BiomeSpawnersToCreate = SpawnerPresetListEditor.OnRemoveListEntry(m_BiomeSpawnersToCreate, m_biomeSpawnersList.index);
            list.list = m_BiomeSpawnersToCreate;
            m_biomePresetSelection = -999;
        }

        private void OnAddSpawnerPresetListEntry(ReorderableList list)
        {
            m_BiomeSpawnersToCreate = SpawnerPresetListEditor.OnAddListEntry(m_BiomeSpawnersToCreate);
            list.list = m_BiomeSpawnersToCreate;
            m_biomePresetSelection = -999;
        }

        private void DrawSpawnerPresetListHeader(Rect rect)
        {
            SpawnerPresetListEditor.DrawListHeader(rect, true, m_BiomeSpawnersToCreate, m_editorUtils, "SpawnerAdded");
        }

        private void DrawSpawnerPresetListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SpawnerPresetListEditor.DrawListElement_GaiaManager(rect, m_BiomeSpawnersToCreate[index], m_editorUtils);
        }

        private void DrawAdvancedTabSpawnerPresetListHeader(Rect rect)
        {
            SpawnerPresetListEditor.DrawListHeader(rect, true, m_BiomeSpawnersToCreate, m_editorUtils, "AdvancedTabSpawnerListHeader");
        }

        private void DrawAdvancedTabSpawnerListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SpawnerPresetListEditor.DrawListElement_AdvancedTab(rect, m_advancedTabAllSpawners[index], m_editorUtils);
        }

        private float OnElementHeightSpawnerPresetListEntry(int index)
        {
            return SpawnerPresetListEditor.OnElementHeight();
        }



        #endregion

        /// <summary>
        /// Creates a new Gaia settings asset
        /// </summary>
        /// <returns>New gaia settings asset</returns>
        public static GaiaSettings CreateSettingsAsset()
        {
            GaiaSettings settings = ScriptableObject.CreateInstance<Gaia.GaiaSettings>();
            AssetDatabase.CreateAsset(settings, GaiaDirectories.GetDataDirectory() + "/GaiaSettings.asset");
            AssetDatabase.SaveAssets();
            return settings;
        }

        #endregion

        #region Tabs
        /// <summary>
        /// Draw the brief editor
        /// </summary>
        void StandardTab()
        {
#if !UNITY_2019_1_OR_NEWER

            EditorGUILayout.HelpBox(Application.unityVersion + " Is not supported in Gaia, please use 2019.1+.", MessageType.Error);
            GUI.enabled = false;

#endif

            if (!m_enableGUI)
            {
                GUI.backgroundColor = new Color(1f, 0.7311321f, 0.7311321f, 1f);
            }

            //Show the Setup panel settings
            m_editorUtils.Panel("PanelSetup", SetupSettingsEnabled, m_showSetupPanel);

            GUI.backgroundColor = m_defaultPanelColor;

            GUI.enabled = m_enableGUI;

            m_editorUtils.Panel("NewWorldSettings", NewWorldSettings, m_enableGUI);

            EditorGUI.indentLevel++;

            if (m_editorUtils.ClickableText("Follow the workflow to create your scene. Click here for tutorials."))
            {
                Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=tutorials");
            }
            GUILayout.Space(5f);

            //Add in a check for linear deferred lighting
            if (m_settings.m_currentEnvironment == GaiaConstants.EnvironmentTarget.Desktop ||
              m_settings.m_currentEnvironment == GaiaConstants.EnvironmentTarget.PowerfulDesktop ||
              m_settings.m_currentEnvironment == GaiaConstants.EnvironmentTarget.Custom)
            {
                var tier1 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier1);
                var tier2 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier2);
                var tier3 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier3);
                if (PlayerSettings.colorSpace != ColorSpace.Linear || tier1.renderingPath != RenderingPath.DeferredShading)
                {
                    if (m_editorUtils.ButtonAutoIndent("0. Set Linear Deferred"))
                    {
                        var manager = GetWindow<GaiaManagerEditor>();

                        if (EditorUtility.DisplayDialog(
                        m_editorUtils.GetTextValue("SettingLinearDeferred"),
                        m_editorUtils.GetTextValue("SetLinearDeferred"),
                        m_editorUtils.GetTextValue("Yes"), m_editorUtils.GetTextValue("Cancel")))
                        {
                            manager.Close();

                            PlayerSettings.colorSpace = ColorSpace.Linear;

                            tier1.renderingPath = RenderingPath.DeferredShading;
                            EditorGraphicsSettings.SetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier1, tier1);

                            tier2.renderingPath = RenderingPath.DeferredShading;
                            EditorGraphicsSettings.SetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier2, tier2);

                            tier3.renderingPath = RenderingPath.DeferredShading;
                            EditorGraphicsSettings.SetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier3, tier3);

#if UNITY_2018_1_OR_NEWER && !UNITY_2019_1_OR_NEWER
                            LightmapEditorSettings.lightmapper = LightmapEditorSettings.Lightmapper.ProgressiveCPU;
#elif UNITY_2019_1_OR_NEWER
                            LightmapEditorSettings.lightmapper = LightmapEditorSettings.Lightmapper.ProgressiveGPU;
#endif

#if UNITY_2018_1_OR_NEWER
                            Lightmapping.realtimeGI = true;
                            Lightmapping.bakedGI = true;
                            LightmapEditorSettings.realtimeResolution = 2f;
                            LightmapEditorSettings.bakeResolution = 40f;
                            Lightmapping.indirectOutputScale = 2f;
                            RenderSettings.defaultReflectionResolution = 256;
                            if (QualitySettings.shadowDistance < 350f)
                            {
                                QualitySettings.shadowDistance = 350f;
                            }
#else
                            if (QualitySettings.shadowDistance < 250f)
                            {
                                QualitySettings.shadowDistance = 250f;
                            }
#endif
                            if (Lightmapping.giWorkflowMode == Lightmapping.GIWorkflowMode.Iterative)
                            {
                                Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
                            }

                            if (GameObject.Find("Directional light") != null)
                            {
                                RenderSettings.sun = GameObject.Find("Directional light").GetComponent<Light>();
                            }
                            else if (GameObject.Find("Directional Light") != null)
                            {
                                RenderSettings.sun = GameObject.Find("Directional Light").GetComponent<Light>();
                            }
                        }
                    }
                }
            }

            if (m_editorUtils.ButtonAutoIndent("1. Create Terrains & Show Tools"))
            {


                int actualTerrainCount = Gaia.TerrainHelper.GetActiveTerrainCount();
                if (actualTerrainCount != 0)
                {

                    if (EditorUtility.DisplayDialog(m_editorUtils.GetTextValue("AlreadyFoundTerrainHeader"), string.Format(m_editorUtils.GetTextValue("AlreadyFoundTerrain"), actualTerrainCount, 0), m_editorUtils.GetTextValue("OK"), m_editorUtils.GetTextValue("Cancel")))
                    {
                        BiomePresetDropdownEntry selectedPresetEntry = m_allBiomePresets.Find(x => x.ID == m_biomePresetSelection);
                        CreateBiome(selectedPresetEntry);
                        //prepare resource prototype arrays once, so the same prototypes can be added to all the tiles.
                        TerrainLayer[] terrainLayers = new TerrainLayer[0];
                        DetailPrototype[] terrainDetails = new DetailPrototype[0];
                        TreePrototype[] terrainTrees = new TreePrototype[0];

                        GaiaDefaults.GetPrototypes(m_BiomeSpawnersToCreate, ref terrainLayers, ref terrainDetails, ref terrainTrees, Terrain.activeTerrain);

                        foreach (Terrain t in Terrain.activeTerrains)
                        {
                            GaiaDefaults.ApplyPrototypesToTerrain(t, terrainLayers, terrainDetails, terrainTrees);
                        }

                    }
                }
                else
                {
                    //No terrain yet, create everything as usual
                    //Check lighting first
#if HDPipeline
                    GaiaHDRPPipelineUtils.SetupDefaultSceneLighting(m_settings.m_pipelineProfile);
#else
                    GaiaLighting.SetDefaultAmbientLight();
#endif

                    float totalSteps = 3;
                    float currentStep = 0f;
                    EditorUtility.DisplayProgressBar("Creating Terrain", "Creating Terrain", ++currentStep / totalSteps);
                    CreateTerrain();
                    //Create the spawners


                    //Check if there already exist a fitting biome Game Object to group our spawners under
                    BiomePresetDropdownEntry selectedPresetEntry = m_allBiomePresets.Find(x => x.ID == m_biomePresetSelection);


                    EditorUtility.DisplayProgressBar("Creating Tools", "Creating Biome " + selectedPresetEntry.name, ++currentStep / totalSteps);
                    List<Spawner> createdSpawners = CreateBiome(selectedPresetEntry);

                    #region Old Spawner Setup
                    //EditorUtility.DisplayProgressBar("Creating Tools", "Creating Texture Spawner...", ++currentStep / totalSteps);
                    //Spawner textureSpawner = CreateTextureSpawner().GetComponent<Spawner>();
                    //if (textureSpawner.m_activeRuleCnt == 0)
                    //{
                    //    DestroyImmediate(textureSpawner.gameObject);
                    //    textureSpawner = null;
                    //}
                    //else
                    //{
                    //    textureSpawner.FitToAllTerrains();
                    //}
                    //EditorUtility.DisplayProgressBar("Creating Tools", "Creating Game Object Spawner...", ++currentStep / totalSteps);
                    //spawner = CreateCoverageGameObjectSpawner().GetComponent<Spawner>();
                    //if (spawner.m_activeRuleCnt == 0)
                    //{
                    //    DestroyImmediate(spawner.gameObject);
                    //}
                    //else
                    //{
                    //    spawner.FitToAllTerrains();
                    //}
                    ////EditorUtility.DisplayProgressBar("Creating Tools", "Creating Game Object Spawner...", ++currentStep / totalSteps);
                    ////spawner = CreateClusteredTreeSpawnerFromTerrainTrees().GetComponent<Spawner>();
                    ////if (spawner.m_activeRuleCnt == 0)
                    ////{
                    ////    DestroyImmediate(spawner.gameObject);
                    ////}
                    ////EditorUtility.DisplayProgressBar("Creating Tools", "Creating Game Object Spawner...", ++currentStep / totalSteps);
                    ////spawner = CreateClusteredTreeSpawnerFromGameObjects().GetComponent<Spawner>();
                    ////if (spawner.m_activeRuleCnt == 0)
                    ////{
                    ////    DestroyImmediate(spawner.gameObject);
                    ////}
                    //EditorUtility.DisplayProgressBar("Creating Tools", "Creating Tree Spawners...", ++currentStep / totalSteps);
                    //spawner = CreateCoverageTreeSpawner().GetComponent<Spawner>();
                    //if (spawner.m_activeRuleCnt == 0)
                    //{
                    //    DestroyImmediate(spawner.gameObject);
                    //}
                    //else
                    //{
                    //    spawner.FitToAllTerrains();
                    //}
                    //spawner = CreateCoverageTreeSpawnerFromGameObjects().GetComponent<Spawner>();
                    //if (spawner.m_activeRuleCnt == 0)
                    //{
                    //    DestroyImmediate(spawner.gameObject);
                    //}
                    //else
                    //{
                    //    spawner.FitToAllTerrains();
                    //}
                    //EditorUtility.DisplayProgressBar("Creating Tools", "Creating Terrain Detail Spawner...", ++currentStep / totalSteps);
                    //spawner = CreateDetailSpawner().GetComponent<Spawner>();
                    //if (spawner.m_activeRuleCnt == 0)
                    //{
                    //    DestroyImmediate(spawner.gameObject);
                    //}
                    //else
                    //{
                    //    spawner.FitToAllTerrains();
                    //}
                    #endregion
                    EditorUtility.DisplayProgressBar("Creating Tools", "Creating Stamper...", ++currentStep / totalSteps);
                    ShowStamper(createdSpawners);
                    EditorUtility.ClearProgressBar();
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }

            //EditorGUI.indentLevel++;
            //if (m_editorUtils.ButtonAutoIndent("1A. Enhance Terrain"))
            //{
            //    ShowTerrainUtilties();
            //}
            //EditorGUI.indentLevel--;

            //if (m_editorUtils.ButtonAutoIndent("2. Create Spawners"))
            //{
            //    //Only do this if we have at least 1 terrain
            //    if (!DisplayErrorIfNotMinimumTerrainCount(1, "Spawners"))
            //    {
            //       
            //    }
            //}

            string buttonLabel;

            buttonLabel = m_editorUtils.GetTextValue("CreateExtrasButtonStart");
            bool first = true;

            if (m_settings.m_currentController != GaiaConstants.EnvironmentControllerType.None)
            {
                first = false;
                buttonLabel += m_editorUtils.GetTextValue("CreateExtrasButtonPlayer");
            }
            if (m_settings.m_currentSkies != GaiaConstants.GaiaLightingProfileType.None)
            {
                if (!first)
                    buttonLabel += ", ";
                first = false;
                buttonLabel += m_editorUtils.GetTextValue("CreateExtrasButtonSkies");
            }
            //if (m_settings.m_currentPostFX != GaiaConstants.PostFX.None)
            //{
            //    if (!first)
            //        buttonLabel += ", ";
            //    first = false;
            //    buttonLabel += m_editorUtils.GetTextValue("CreateExtrasButtonPostFX");
            //}

            if (m_settings.m_currentWaterPro != GaiaConstants.GaiaWaterProfileType.None)
            {
                if (!first)
                    buttonLabel += ", ";
                first = false;
                buttonLabel += m_editorUtils.GetTextValue("CreateExtrasButtonWater");
            }

            if (m_settings.m_createWind)
            {
                if (!first)
                    buttonLabel += ", ";
                first = false;
                buttonLabel += m_editorUtils.GetTextValue("CreateExtrasButtonWind");
            }
            if (m_settings.m_createScreenShotter)
            {
                if (!first)
                    buttonLabel += ", ";
                first = false;
                buttonLabel += m_editorUtils.GetTextValue("CreateExtrasButtonScreenShotter");
            }

            //only display the button if there is actually anything being to be added to the scene
            if (!first)
            {
                if (m_editorUtils.ButtonAutoIndent(new GUIContent(buttonLabel, m_editorUtils.GetTooltip("CreateExtrasButtonStart"))))
                {
                    //Only do this if we have 1 terrain
                    if (DisplayErrorIfNotMinimumTerrainCount(1))
                    {
                        return;
                    }

                    if (m_settings.m_currentSkies != GaiaConstants.GaiaLightingProfileType.None)
                    {
                        if (!EditorUtility.DisplayDialog("Adding Ambient Skies Samples", "You're about to add a sky with lighting and post processing setup to your scene that might overwrite existing lighting and post processing settings. Continue?", "Yes", "No"))
                        {
                            return;
                        }
                    }

                    if (m_settings.m_currentController != GaiaConstants.EnvironmentControllerType.None)
                    {
                        CreatePlayer();
                    }
                    if (m_settings.m_currentSkies != GaiaConstants.GaiaLightingProfileType.None)
                    {
                        CreateSky();
                    }
                    else
                    {
                        GaiaLighting.RemoveSystems();
                    }
                    if (m_settings.m_currentWater != GaiaConstants.Water.None || m_settings.m_currentWaterPro != GaiaConstants.GaiaWaterProfileType.None)
                    {
                        CreateWater();
                    }
                    else
                    {
                        GaiaWater.RemoveSystems();
                    }
                    if (m_settings.m_createScreenShotter)
                    {
                        CreateScreenShotter();
                    }
                    if (m_settings.m_createWind)
                    {
                        CreateWindZone();
                    }
                }
            }

            string buttonCount = "3. ";
            if (first)
            {
                buttonCount = "2. ";
            }

            if (Lightmapping.isRunning)
            {
                if (m_editorUtils.ButtonAutoIndent(new GUIContent(buttonCount + m_editorUtils.GetTextValue("Cancel Bake"))))
                {
                    Lightmapping.Cancel();
                    Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
                }
            }
            else
            {
                if (m_editorUtils.ButtonAutoIndent(new GUIContent(buttonCount + m_editorUtils.GetTextValue("Bake Lighting"))))
                {
                    if (EditorUtility.DisplayDialog(
                        m_editorUtils.GetTextValue("BakingLightmaps!"),
                        m_editorUtils.GetTextValue("BakingLightmapsInfo"),
                        m_editorUtils.GetTextValue("Bake"), m_editorUtils.GetTextValue("Cancel")))
                    {
                        RenderSettings.ambientMode = AmbientMode.Skybox;
                        Lightmapping.bakedGI = true;
                        Lightmapping.realtimeGI = true;
#if UNITY_2018_2_OR_NEWER
                        LightmapEditorSettings.directSampleCount = 32;
                        LightmapEditorSettings.indirectSampleCount = 500;
                        LightmapEditorSettings.bounces = 3;
                        LightmapEditorSettings.filteringMode = LightmapEditorSettings.FilterMode.Auto;
                        LightmapEditorSettings.lightmapsMode = LightmapsMode.CombinedDirectional;
#endif
                        LightmapEditorSettings.realtimeResolution = 2;
                        LightmapEditorSettings.bakeResolution = 16;
                        LightmapEditorSettings.padding = 2;
                        LightmapEditorSettings.textureCompression = false;
                        LightmapEditorSettings.enableAmbientOcclusion = true;
                        LightmapEditorSettings.aoMaxDistance = 1f;
                        LightmapEditorSettings.aoExponentIndirect = 1f;
                        LightmapEditorSettings.aoExponentDirect = 1f;
                        Lightmapping.BakeAsync();
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;

            GUI.enabled = true;
        }

        private List<Spawner> CreateBiome(BiomePresetDropdownEntry selectedPresetEntry)
        {
            float totalSteps = m_BiomeSpawnersToCreate.Where(x => x.m_spawnerSettings != null).Count();
            float currentStep = 0f;
            List<Spawner> createdSpawners = new List<Spawner>();
            GameObject sessionManager = ShowSessionManager();
            Transform gaiaTransform = sessionManager.transform.parent;
            Transform target = gaiaTransform.Find(selectedPresetEntry.name);
            if (target == null)
            {
                GameObject newGO = new GameObject();
                newGO.name = selectedPresetEntry.name + " Biome";
                newGO.transform.parent = gaiaTransform;
                target = newGO.transform;
            }

            BiomeController biomeController = target.GetComponent<BiomeController>();
            if (biomeController == null)
            {
                biomeController = target.gameObject.AddComponent<BiomeController>();
            }
#if UNITY_POST_PROCESSING_STACK_V2
            if (selectedPresetEntry.biomePreset != null)
            {
                biomeController.m_postProcessProfile = selectedPresetEntry.biomePreset.postProcessProfile;
            }
#endif

            //Track created spawners 

            foreach (BiomeSpawnerListEntry spawnerListEntry in m_BiomeSpawnersToCreate.Where(x => x.m_spawnerSettings != null))
            {
                createdSpawners.Add(spawnerListEntry.m_spawnerSettings.CreateSpawner(false, biomeController.transform));
                EditorUtility.DisplayProgressBar("Creating Tools", "Creating Biome " + selectedPresetEntry.name, ++currentStep / totalSteps);
            }
            if (createdSpawners.Count > 0)
            {
                biomeController.m_range = createdSpawners[0].m_settings.m_spawnRange;
            }

            foreach (Spawner spawner in createdSpawners)
            {
                biomeController.m_autoSpawners.Add(new AutoSpawner() { isActive = true, status = AutoSpawnerStatus.Initial, spawner = spawner });
            }
            EditorUtility.ClearProgressBar();
            return createdSpawners;

        }

        /// <summary>
        /// Draw the detailed editor
        /// </summary>
        void AdvancedTab()
        {
#if !UNITY_2019_1_OR_NEWER

            EditorGUILayout.HelpBox(Application.unityVersion + " Is not supported in Gaia, please use 2019.1+.", MessageType.Error);
            GUI.enabled = false;

#endif

            EditorGUI.indentLevel++;

            //            if (DrawLinkHeaderText("Advanced Workflow"))
            //            {
            //                Application.OpenURL("http://www.procedural-worlds.com/gaia/tutorials/import-real-world-terrain/");
            //            }

            if (m_editorUtils.ClickableText("Pick and choose your tasks. Click here for tutorials."))
            {
                Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=tutorials");
            }

            if (!m_shadersNotImported)
            {
                EditorGUILayout.HelpBox("Shader library folder is missing. Please reimport Gaia and insure that 'PW Shader Library' is improted.", MessageType.Error);
            }

            GUI.enabled = m_enableGUI;

            GUILayout.Space(5f);
            m_editorUtils.Panel("PanelTerrain", AdvancedPanelTerrain, false);
            m_editorUtils.Panel("PanelBiomesAndSpawners", AdvancedPanelBiomesAndSpawners, false);
            m_editorUtils.Panel("PanelExtras", AdvancedPanelExtras, false);
            m_editorUtils.Panel("PanelUtilities", AdvancedPanelUtilities, false);
            m_editorUtils.Panel("SystemInfoSettings", SystemInfoSettingsEnabled, false);

            GUI.enabled = false;

            //if (m_foldoutSpawners = m_editorUtils.Foldout(m_foldoutSpawners, "AdvancedFoldoutSpawners"))
            //{
            //    EditorGUI.indentLevel++;
            //    ////if (m_editorUtils.ButtonAutoIndent("Create Stamp Spawner"))
            //    ////{
            //    ////    Selection.activeObject = CreateStampSpawner();
            //    ////}
            //    //if (m_editorUtils.ButtonAutoIndent("Create Coverage Texture Spawner"))
            //    //{
            //    //    Selection.activeObject = CreateTextureSpawner();
            //    //}
            //    //if (m_editorUtils.ButtonAutoIndent("Create Clustered Grass Spawner"))
            //    //{
            //    //    Selection.activeObject = CreateClusteredDetailSpawner();
            //    //}
            //    //if (m_editorUtils.ButtonAutoIndent("Create Coverage Grass Spawner"))
            //    //{
            //    //    Selection.activeObject = CreateDetailSpawner();
            //    //}
            //    //if (m_editorUtils.ButtonAutoIndent("Create Clustered Terrain Tree Spawner"))
            //    //{
            //    //    Selection.activeObject = CreateClusteredTreeSpawnerFromTerrainTrees();
            //    //}
            //    //if (m_editorUtils.ButtonAutoIndent("Create Clustered Prefab Tree Spawner"))
            //    //{
            //    //    Selection.activeObject = CreateClusteredTreeSpawnerFromGameObjects();
            //    //}
            //    //if (m_editorUtils.ButtonAutoIndent("Create Coverage Terrain Tree Spawner"))
            //    //{
            //    //    Selection.activeObject = CreateCoverageTreeSpawner();
            //    //}
            //    //if (m_editorUtils.ButtonAutoIndent("Create Coverage Prefab Tree Spawner"))
            //    //{
            //    //    Selection.activeObject = CreateCoverageTreeSpawnerFromGameObjects();
            //    //}
            //    //if (m_editorUtils.ButtonAutoIndent("Create Clustered Prefab Spawner"))
            //    //{
            //    //    Selection.activeObject = CreateClusteredGameObjectSpawner();
            //    //}
            //    //if (m_editorUtils.ButtonAutoIndent("Create Coverage Prefab Spawner"))
            //    //{
            //    //    Selection.activeObject = CreateCoverageGameObjectSpawner();
            //    //}
            //    ////if (m_editorUtils.ButtonAutoIndent("Create Group Spawner"))
            //    ////{
            //    ////    Selection.activeObject = FindOrCreateGroupSpawner();
            //    ////}
            //    //EditorGUILayout.Space();
            //    //EditorGUI.indentLevel--;
            //}

        }

        private void AdvancedPanelUtilities(bool helpEnabled)
        {
            EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("ComingSoonInfo"), MessageType.Info);

            if (m_editorUtils.ButtonAutoIndent("Show Stamp Converter"))
            {
                ShowGaiaStampConverter();
            }

            if (m_editorUtils.ButtonAutoIndent("Terrain Mesh Export"))
            {
                ShowTerrainObjExporter();
            }

            GUI.enabled = false;



            if (m_editorUtils.ButtonAutoIndent("ComingSoonScanner"))
            {
                //Selection.activeGameObject = CreateScanner();
            }
            if (m_editorUtils.ButtonAutoIndent("ComingSoonTerrainUtilities"))
            {
                //ShowTerrainUtilties();
            }
            if (m_editorUtils.ButtonAutoIndent("ComingSoonExporters"))
            {
                //ShowTexureMaskExporter();
            }
            //if (m_editorUtils.ButtonAutoIndent("ComingSoonShowGrassExporter"))
            //{
            //    //ShowGrassMaskExporter();
            //}
            //if (m_editorUtils.ButtonAutoIndent("ComingSoonShowMeshExporter"))
            //{
            //    //ShowTerrainObjExporter();
            //}
            //if (m_editorUtils.ButtonAutoIndent("ComingSoonShowShoreExporter"))
            //{
            //    //ExportShoremaskAsPNG();
            //}

            //if (m_editorUtils.ButtonAutoIndent("ComingSoonShowExtensionExporter"))
            //{
            //    //ShowExtensionExporterEditor();
            //}
            GUI.enabled = true;
        }

        private void AdvancedPanelExtras(bool helpEnabled)
        {
            if (m_editorUtils.ButtonAutoIndent("Add Character"))
            {

                Selection.activeGameObject = CreatePlayer();

                //#if GAIA_PRESENT
                //                    GameObject underwaterFX = GameObject.Find("Directional Light");
                //                    GaiaReflectionProbeUpdate theProbeUpdater = FindObjectOfType<GaiaReflectionProbeUpdate>();
                //                    GaiaUnderWaterEffects effectsSettings = underwaterFX.GetComponent<GaiaUnderWaterEffects>();
                //                    if (theProbeUpdater != null && effectsSettings != null)
                //                    {
                //#if UNITY_EDITOR
                //                        effectsSettings.player = effectsSettings.GetThePlayer();
                //#endif
                //                    }
                //#endif
            }
            if (m_editorUtils.ButtonAutoIndent("Add Wind Zone"))
            {
                Selection.activeGameObject = CreateWindZone();
            }
            if (m_editorUtils.ButtonAutoIndent("Add Water"))
            {
                Selection.activeGameObject = CreateWater();
            }
            if (m_editorUtils.ButtonAutoIndent("Add Screen Shotter"))
            {
                Selection.activeGameObject = CreateScreenShotter();
            }
        }

        private void AdvancedPanelBiomesAndSpawners(bool helpEnabled)
        {
            Rect biomesFoldOutRect = EditorGUILayout.GetControlRect();
            m_advancedTabFoldoutBiomes = EditorGUI.Foldout(biomesFoldOutRect, m_advancedTabFoldoutBiomes, m_editorUtils.GetContent("AdvancedFoldoutAddBiomes"));
            if (m_advancedTabFoldoutBiomes)
            {
                if (m_allBiomePresets.Exists(x => x.biomePreset == null && x.ID != -999))
                {
                    CreateAdvancedTabBiomesList();
                }

                //the hardcoded 15 are for some indent below the foldout label
                biomesFoldOutRect.x += 15;
                biomesFoldOutRect.width -= 15;
                biomesFoldOutRect.y += EditorGUIUtility.singleLineHeight * 1.5f;
                m_advancedTabBiomesList.DoList(biomesFoldOutRect);

                biomesFoldOutRect.y += m_advancedTabBiomesList.GetHeight() - 5;
                biomesFoldOutRect.x += biomesFoldOutRect.width * 0.65f;
                biomesFoldOutRect.width = biomesFoldOutRect.width * 0.35f;

                if (GUI.Button(biomesFoldOutRect, m_editorUtils.GetContent("CreateNewBiomeButton")))
                {
                    GaiaUtils.CreateAsset<BiomePreset>(GaiaDirectories.GetDataDirectory(), "Biome Preset");
                }

                GUILayout.Space(m_advancedTabBiomesList.GetHeight() + EditorGUIUtility.singleLineHeight * 2f);
            }


            Rect spawnerFoldOutRect = EditorGUILayout.GetControlRect();
            m_advancedTabFoldoutSpawners = EditorGUI.Foldout(spawnerFoldOutRect, m_advancedTabFoldoutSpawners, m_editorUtils.GetContent("AdvancedFoldoutAddSpawners"));
            if (m_advancedTabFoldoutSpawners)
            {
                if (m_advancedTabAllSpawners.Exists(x => x.m_spawnerSettings == null))
                {
                    CreateAdvancedTabSpawnersList();
                }

                //the hardcoded 15 are for some indent below the foldout label
                spawnerFoldOutRect.x += 15;
                spawnerFoldOutRect.width -= 15;
                spawnerFoldOutRect.y += EditorGUIUtility.singleLineHeight * 1.5f;
                m_advancedTabSpawnersList.DoList(spawnerFoldOutRect);
                spawnerFoldOutRect.y += m_advancedTabSpawnersList.GetHeight() - 5;
                spawnerFoldOutRect.x += spawnerFoldOutRect.width * 0.65f;
                spawnerFoldOutRect.width = spawnerFoldOutRect.width * 0.35f;

                if (GUI.Button(spawnerFoldOutRect, m_editorUtils.GetContent("CreateNewSpawnerButton")))
                {
                    GameObject spawnerObj = new GameObject("New Spawner");
                    Spawner spawner = spawnerObj.AddComponent<Spawner>();
                    spawner.m_createdFromGaiaManager = true;
                    spawner.FitToAllTerrains();
                    Selection.activeGameObject = spawnerObj;
                }

                GUILayout.Space(m_advancedTabSpawnersList.GetHeight() + EditorGUIUtility.singleLineHeight * 2f);
            }
        }

        private void AdvancedPanelTerrain(bool helpEnabled)
        {
            if (m_editorUtils.ButtonAutoIndent("Show Session Manager"))
            {
                ShowSessionManager();
            }
            if (m_editorUtils.ButtonAutoIndent("Create Terrain"))
            {
                CreateTerrain();
            }
            if (m_editorUtils.ButtonAutoIndent("Show Stamper"))
            {
                ShowStamper();
            }
        }

        /// <summary>
        /// Draw the extension editor
        /// </summary>
        void ExtensionsTab()
        {
#if !UNITY_2019_1_OR_NEWER

            EditorGUILayout.HelpBox(Application.unityVersion + " Is not supported in Gaia, please use 2019.1+.", MessageType.Error);
            GUI.enabled = false;

#endif

            EditorGUI.indentLevel++;

            if (m_editorUtils.ClickableText(
                "Gaia eXtensions accelerate and simplify development by integrating quality assets. This tab shows the extensions for the products you've installed. Click here to see more extensions.")
            )
            {
                Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=gaia-extensions");
            }
            GUILayout.Space(5f);

            //And scan if something has changed
            if (m_needsScan)
            {
                m_extensionMgr.ScanForExtensions();
                if (m_extensionMgr.GetInstalledExtensionCount() != 0)
                {
                    m_needsScan = false;
                }
            }

            int methodIdx = 0;
            string cmdName;
            string currFoldoutName = "";
            string prevFoldoutName = "";
            MethodInfo command;
            string[] cmdBreakOut = new string[0];
            List<GaiaCompatiblePackage> packages;
            List<GaiaCompatiblePublisher> publishers = m_extensionMgr.GetPublishers();

            foreach (GaiaCompatiblePublisher publisher in publishers)
            {
                if (publisher.InstalledPackages() > 0)
                {
                    if (publisher.m_installedFoldedOut = m_editorUtils.Foldout(publisher.m_installedFoldedOut, new GUIContent(publisher.m_publisherName)))
                    {
                        EditorGUI.indentLevel++;

                        packages = publisher.GetPackages();
                        foreach (GaiaCompatiblePackage package in packages)
                        {
                            if (package.m_isInstalled)
                            {
                                if (package.m_installedFoldedOut = m_editorUtils.Foldout(package.m_installedFoldedOut, new GUIContent(package.m_packageName)))
                                {
                                    EditorGUI.indentLevel++;
                                    methodIdx = 0;
                                    //Now loop thru and process
                                    while (methodIdx < package.m_methods.Count)
                                    {
                                        command = package.m_methods[methodIdx];
                                        cmdBreakOut = command.Name.Split('_');

                                        //Ignore if we are not a valid thing
                                        if ((cmdBreakOut.GetLength(0) != 2 && cmdBreakOut.GetLength(0) != 3) || cmdBreakOut[0] != "GX")
                                        {
                                            methodIdx++;
                                            continue;
                                        }

                                        //Get foldout and command name
                                        if (cmdBreakOut.GetLength(0) == 2)
                                        {
                                            currFoldoutName = "";
                                        }
                                        else
                                        {
                                            currFoldoutName = Regex.Replace(cmdBreakOut[1], "(\\B[A-Z])", " $1");
                                        }
                                        cmdName = Regex.Replace(cmdBreakOut[cmdBreakOut.GetLength(0) - 1], "(\\B[A-Z])", " $1");

                                        if (currFoldoutName == "")
                                        {
                                            methodIdx++;
                                            if (m_editorUtils.ButtonAutoIndent(new GUIContent(cmdName)))
                                            {
                                                command.Invoke(null, null);
                                            }
                                        }
                                        else
                                        {
                                            prevFoldoutName = currFoldoutName;

                                            //Make sure we have it in our dictionary
                                            if (!package.m_methodGroupFoldouts.ContainsKey(currFoldoutName))
                                            {
                                                package.m_methodGroupFoldouts.Add(currFoldoutName, false);
                                            }

                                            if (package.m_methodGroupFoldouts[currFoldoutName] = m_editorUtils.Foldout(package.m_methodGroupFoldouts[currFoldoutName], new GUIContent(currFoldoutName)))
                                            {
                                                EditorGUI.indentLevel++;

                                                while (methodIdx < package.m_methods.Count && currFoldoutName == prevFoldoutName)
                                                {
                                                    command = package.m_methods[methodIdx];
                                                    cmdBreakOut = command.Name.Split('_');

                                                    //Drop out if we are not a valid thing
                                                    if ((cmdBreakOut.GetLength(0) != 2 && cmdBreakOut.GetLength(0) != 3) || cmdBreakOut[0] != "GX")
                                                    {
                                                        methodIdx++;
                                                        continue;
                                                    }

                                                    //Get foldout and command name
                                                    if (cmdBreakOut.GetLength(0) == 2)
                                                    {
                                                        currFoldoutName = "";
                                                    }
                                                    else
                                                    {
                                                        currFoldoutName = Regex.Replace(cmdBreakOut[1], "(\\B[A-Z])", " $1");
                                                    }
                                                    cmdName = Regex.Replace(cmdBreakOut[cmdBreakOut.GetLength(0) - 1], "(\\B[A-Z])", " $1");

                                                    if (currFoldoutName != prevFoldoutName)
                                                    {
                                                        continue;
                                                    }

                                                    if (m_editorUtils.ButtonAutoIndent(new GUIContent(cmdName)))
                                                    {
                                                        command.Invoke(null, null);
                                                    }

                                                    methodIdx++;
                                                }

                                                EditorGUI.indentLevel--;
                                            }
                                            else
                                            {
                                                while (methodIdx < package.m_methods.Count && currFoldoutName == prevFoldoutName)
                                                {
                                                    command = package.m_methods[methodIdx];
                                                    cmdBreakOut = command.Name.Split('_');

                                                    //Drop out if we are not a valid thing
                                                    if ((cmdBreakOut.GetLength(0) != 2 && cmdBreakOut.GetLength(0) != 3) || cmdBreakOut[0] != "GX")
                                                    {
                                                        methodIdx++;
                                                        continue;
                                                    }

                                                    //Get foldout and command name
                                                    if (cmdBreakOut.GetLength(0) == 2)
                                                    {
                                                        currFoldoutName = "";
                                                    }
                                                    else
                                                    {
                                                        currFoldoutName = Regex.Replace(cmdBreakOut[1], "(\\B[A-Z])", " $1");
                                                    }
                                                    cmdName = Regex.Replace(cmdBreakOut[cmdBreakOut.GetLength(0) - 1], "(\\B[A-Z])", " $1");

                                                    if (currFoldoutName != prevFoldoutName)
                                                    {
                                                        continue;
                                                    }

                                                    methodIdx++;
                                                }
                                            }
                                        }
                                    }

                                    /*
                                    foreach (MethodInfo command in package.m_methods)
                                    {
                                        cmdBreakOut = command.Name.Split('_');

                                        if ((cmdBreakOut.GetLength(0) == 2 || cmdBreakOut.GetLength(0) == 3) && cmdBreakOut[0] == "GX")
                                        {
                                            if (cmdBreakOut.GetLength(0) == 2)
                                            {
                                                currFoldoutName = "";
                                            }
                                            else
                                            {
                                                currFoldoutName = cmdBreakOut[1];
                                                Debug.Log(currFoldoutName);
                                            }

                                            cmdName = Regex.Replace(cmdBreakOut[cmdBreakOut.GetLength(0) - 1], "(\\B[A-Z])", " $1");
                                            if (m_editorUtils.ButtonAutoIndent(new GUIContent(cmdName)))
                                            {
                                                command.Invoke(null, null);
                                            }
                                        }
                                    }
                                        */

                                    EditorGUI.indentLevel--;
                                }
                            }
                        }

                        EditorGUI.indentLevel--;
                    }
                }
            }

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draw the show more editor
        /// </summary>
        void MoreTab()
        {
            m_editorUtils.Tabs(m_moreTabs);
        }

        void TutorialsAndSupportTab()
        {
            EditorGUI.indentLevel++;
            m_editorUtils.Text("Review the QuickStart guide and other product documentation in the Gaia / Documentation directory.");
            GUILayout.Space(5f);

            if (m_settings.m_hideHeroMessage)
            {
                if (m_editorUtils.ClickableHeadingNonLocalized(m_settings.m_latestNewsTitle))
                {
                    Application.OpenURL(m_settings.m_latestNewsUrl);
                }

                m_editorUtils.TextNonLocalized(m_settings.m_latestNewsBody);
                GUILayout.Space(5f);
            }

            if (m_editorUtils.ClickableHeading("Video Tutorials"))
            {
                Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=tutorials");
            }
            m_editorUtils.Text("With over 45 video tutorials we cover everything you need to become an expert.");
            GUILayout.Space(5f);

            if (m_editorUtils.ClickableHeading("Join Our Community"))
            {
                Application.OpenURL("https://discord.gg/rtKn8rw");
            }
            m_editorUtils.Text("Whether you need an answer now or feel like a chat our friendly discord community is a great place to learn!");
            GUILayout.Space(5f);

            if (m_editorUtils.ClickableHeading("Ticketed Support"))
            {
                Application.OpenURL("https://proceduralworlds.freshdesk.com/support/home");
            }
            m_editorUtils.Text("Don't let your question get lost in the noise. All ticketed requests are answered, and usually within 48 hours.");
            GUILayout.Space(5f);

            if (m_editorUtils.ClickableHeading("Help us Grow - Rate & Review!"))
            {
                Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/42618?aid=1101lSqC");
            }
            m_editorUtils.Text("Quality products are a huge investment to create & support. Please take a moment to show your appreciation by leaving a rating & review.");
            GUILayout.Space(5f);

            if (m_settings.m_hideHeroMessage)
            {
                if (m_editorUtils.ClickableHeading("Show Hero Message"))
                {
                    m_settings.m_hideHeroMessage = false;
                    EditorUtility.SetDirty(m_settings);
                }
                m_editorUtils.Text("Show latest news and hero messages in Gaia.");
                GUILayout.Space(5f);
            }
            EditorGUI.indentLevel--;
        }

        void MoreOnProceduralWorldsTab()
        {
            EditorGUI.indentLevel++;
            m_editorUtils.Text("Super charge your development with our amazing partners & extensions.");
            GUILayout.Space(5f);

            if (m_settings.m_hideHeroMessage)
            {
                if (m_editorUtils.ClickableHeadingNonLocalized(m_settings.m_latestNewsTitle))
                {
                    Application.OpenURL(m_settings.m_latestNewsUrl);
                }

                m_editorUtils.TextNonLocalized(m_settings.m_latestNewsBody);
                GUILayout.Space(5f);
            }

            if (m_editorUtils.ClickableHeading("Our Partners"))
            {
                Application.OpenURL("http://www.procedural-worlds.com/partners/");
            }
            m_editorUtils.Text("The content included with Gaia is an awesome starting point for your game, but that's just the tip of the iceberg. Learn more about how these talented publishers can help you to create amazing environments in Unity.");
            GUILayout.Space(5f);

            if (m_editorUtils.ClickableHeading("Gaia eXtensions (GX)"))
            {
                Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=gaia-extensions");
            }
            m_editorUtils.Text("Gaia eXtensions accelerate and simplify your development by automating asset setup in your scene. Check out the quality assets we have integrated for you!");
            GUILayout.Space(5f);

            if (m_editorUtils.ClickableHeading("Help Us to Grow - Spread The Word!"))
            {
                Application.OpenURL("https://www.facebook.com/proceduralworlds/");
            }
            m_editorUtils.Text("Get regular news updates and help us to grow by liking and sharing our Facebook page!");
            GUILayout.Space(5f);

            if (m_settings.m_hideHeroMessage)
            {
                if (m_editorUtils.ClickableHeading("Show Hero Message"))
                {
                    m_settings.m_hideHeroMessage = false;
                    EditorUtility.SetDirty(m_settings);
                }
                m_editorUtils.Text("Show latest news and hero messages in Gaia.");
                GUILayout.Space(5f);
            }
            EditorGUI.indentLevel--;
        }
        #endregion

        #region On GUI
        void OnGUI()
        {
            m_editorUtils.Initialize(); // Do not remove this!

            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            //Setup the wrap style
            if (m_wrapStyle == null)
            {
                m_wrapStyle = new GUIStyle(GUI.skin.label);
                m_wrapStyle.fontStyle = FontStyle.Normal;
                m_wrapStyle.wordWrap = true;
            }

            if (m_bodyStyle == null)
            {
                m_bodyStyle = new GUIStyle(GUI.skin.label);
                m_bodyStyle.fontStyle = FontStyle.Normal;
                m_bodyStyle.wordWrap = true;
            }

            if (m_titleStyle == null)
            {
                m_titleStyle = new GUIStyle(m_bodyStyle);
                m_titleStyle.fontStyle = FontStyle.Bold;
                m_titleStyle.fontSize = 20;
            }

            if (m_headingStyle == null)
            {
                m_headingStyle = new GUIStyle(m_bodyStyle);
                m_headingStyle.fontStyle = FontStyle.Bold;
            }

            if (m_linkStyle == null)
            {
                m_linkStyle = new GUIStyle(m_bodyStyle);
                m_linkStyle.wordWrap = false;
                m_linkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
                m_linkStyle.stretchWidth = false;
            }


            //Check for state of compiler
            if (EditorApplication.isCompiling)
            {
                m_needsScan = true;
            }

            m_editorUtils.GUIHeader();

            GUILayout.Space(10);

            m_editorUtils.TabsNoBorder(m_mainTabs);




            //EditorGUILayout.BeginVertical(m_boxStyle);



            //EditorGUILayout.EndVertical();
            //for(int i=0; i < m_spawnerSettingsToCreate.Count; i++)
            //{
            //    m_spawnerSettingsToCreate[i] = (SpawnerSettings)m_editorUtils.ObjectField("NoLabel", m_spawnerSettingsToCreate[i], typeof(SpawnerSettings), false);
            //}
            //Rect addButtonRect = EditorGUILayout.BeginHorizontal();
            //addButtonRect.x += EditorGUIUtility.labelWidth;
            //addButtonRect.y += 0;
            //addButtonRect.width = 17;
            //addButtonRect.height += EditorGUIUtility.singleLineHeight;
            //if (GUI.Button(addButtonRect, "+"))
            //{
            //    m_spawnerSettingsToCreate.Add(null);
            //}
            //EditorGUILayout.EndHorizontal();
            //GUILayout.Space(EditorGUIUtility.singleLineHeight);


            //EditorGUILayout.BeginHorizontal();
            //m_settings.m_currentDefaults = (GaiaDefaults)m_editorUtils.ObjectField("Terrain Defaults", m_settings.m_currentDefaults, typeof(GaiaDefaults), false);
            //if (m_editorUtils.Button("New", GUILayout.Width(45), GUILayout.Height(16f)))
            //{
            //    m_settings.m_currentDefaults = CreateDefaultsAsset();
            //}
            //EditorGUILayout.EndHorizontal();

            //GUILayout.Space(2);

            //EditorGUILayout.BeginHorizontal();
            //m_settings.m_currentResources = (GaiaResource)m_editorUtils.ObjectField("Terrain Resources", m_settings.m_currentResources, typeof(GaiaResource), false);
            //if (m_editorUtils.Button("New", GUILayout.Width(45), GUILayout.Height(16f)))
            //{
            //    m_settings.m_currentResources = CreateResourcesAsset();
            //}
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //m_settings.m_currentGameObjectResources = (GaiaResource)m_editorUtils.ObjectField("GameObject Resources", m_settings.m_currentGameObjectResources, typeof(GaiaResource), false);
            //if (m_editorUtils.Button("New", GUILayout.Width(45), GUILayout.Height(16f)))
            //{
            //    m_settings.m_currentGameObjectResources = CreateResourcesAsset();
            //}
            //EditorGUILayout.EndHorizontal();



            //Bottom section
            //if (!m_settings.m_hideHeroMessage)
            //{
            //    GUILayout.BeginVertical();
            //    {
            //        GUILayout.BeginHorizontal(m_boxStyle);
            //        {
            //            //                GUILayout.BeginVertical();
            //            //                GUILayout.Space(3f);
            //            //                DrawImage(m_settings.m_latestNewsImage, 50f, 50f);
            //            //                GUILayout.EndVertical();
            //            GUILayout.BeginVertical();
            //            {
            //                GUILayout.BeginHorizontal();
            //                {
            //                    if (m_editorUtils.ClickableHeadingNonLocalized(m_settings.m_latestNewsTitle))
            //                    {
            //                        Application.OpenURL(m_settings.m_latestNewsUrl);
            //                    }

            //                    if (m_editorUtils.ClickableHeading("Hide", GUILayout.Width(33f)))
            //                    {
            //                        m_settings.m_hideHeroMessage = true;
            //                        EditorUtility.SetDirty(m_settings);
            //                    }
            //                }
            //                GUILayout.EndHorizontal();
            //                m_editorUtils.TextNonLocalized(m_settings.m_latestNewsBody);
            //            }
            //            GUILayout.EndVertical();
            //        }
            //        GUILayout.EndHorizontal();
            //    }
            //    GUILayout.EndVertical();
            //}

            if (m_settings.m_pipelineProfile.m_pipelineSwitchUpdates)
            {
                EditorApplication.update -= EditorPipelineUpdate;
                EditorApplication.update += EditorPipelineUpdate;
            }
            else
            {
                EditorApplication.update -= EditorPipelineUpdate;
            }
        }

        private void NewWorldSettings(bool helpEnabled)
        {
            m_editorUtils.InlineHelp("World Size", helpEnabled);
            Rect rect = EditorGUILayout.GetControlRect();

            float lineHeight = EditorGUIUtility.singleLineHeight + 3;
            Rect labelRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, lineHeight);
            Rect fieldRect = new Rect(labelRect.x + labelRect.width, rect.y, (rect.width - EditorGUIUtility.labelWidth - labelRect.width), lineHeight);

            EditorGUI.LabelField(new Rect(labelRect.x - EditorGUIUtility.labelWidth, labelRect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("World Size").text, m_editorUtils.Styles.heading);
            m_targeSizePreset = (GaiaConstants.EnvironmentSizePreset)EditorGUI.EnumPopup(new Rect(labelRect.x, labelRect.y, labelRect.width + fieldRect.width, labelRect.height), m_targeSizePreset);

            switch (m_targeSizePreset)
            {
                case GaiaConstants.EnvironmentSizePreset.Tiny:
                    m_settings.m_tilesX = 1;
                    m_settings.m_tilesZ = 1;
                    m_targetSize = GaiaConstants.EnvironmentSize.Is256MetersSq;
                    break;
                case GaiaConstants.EnvironmentSizePreset.Small:
                    m_settings.m_tilesX = 1;
                    m_settings.m_tilesZ = 1;
                    m_targetSize = GaiaConstants.EnvironmentSize.Is512MetersSq;
                    break;
                case GaiaConstants.EnvironmentSizePreset.Medium:
                    m_settings.m_tilesX = 1;
                    m_settings.m_tilesZ = 1;
                    m_targetSize = GaiaConstants.EnvironmentSize.Is1024MetersSq;
                    break;
                case GaiaConstants.EnvironmentSizePreset.Large:
                    m_settings.m_tilesX = 1;
                    m_settings.m_tilesZ = 1;
                    m_targetSize = GaiaConstants.EnvironmentSize.Is2048MetersSq;
                    break;
                case GaiaConstants.EnvironmentSizePreset.XLarge:
                    m_settings.m_tilesX = 1;
                    m_settings.m_tilesZ = 1;
                    m_targetSize = GaiaConstants.EnvironmentSize.Is4096MetersSq;
                    break;
            }


            if (m_targeSizePreset == GaiaConstants.EnvironmentSizePreset.Custom)
            {
                m_foldOutWorldSizeSettings = true;
            }



            Rect foldOutWorldSizeRect = EditorGUILayout.GetControlRect();
            m_foldOutWorldSizeSettings = EditorGUI.Foldout(new Rect(foldOutWorldSizeRect.x + EditorGUIUtility.labelWidth, foldOutWorldSizeRect.y, foldOutWorldSizeRect.width, foldOutWorldSizeRect.height), m_foldOutWorldSizeSettings, m_editorUtils.GetContent("AdvancedWorldSize"));
            if (m_foldOutWorldSizeSettings)
            {
                EditorGUI.indentLevel++;
                //Label
                //EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("World Size"), m_editorUtils.Styles.heading);
                //X Label
                Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y + lineHeight * 2, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(numFieldRect, m_editorUtils.GetContent("X Tiles"));
                // X Entry Field
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, numFieldRect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                int tilesX = EditorGUI.IntField(numFieldRect, m_settings.m_tilesX);
                //Empty Label Field for Spacing
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, numFieldRect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(numFieldRect, " ");
                //Z Label
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, numFieldRect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(numFieldRect, m_editorUtils.GetContent("Z Tiles"));
                // Z Entry Field
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, numFieldRect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                int tilesZ = EditorGUI.IntField(numFieldRect, m_settings.m_tilesZ);
                //Empty Label Field for Spacing

                labelRect.y = numFieldRect.y + lineHeight;
                fieldRect.y = labelRect.y;
                GUILayout.Space(lineHeight * 3);


                EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("Terrain Size"));
                m_targetSize = (GaiaConstants.EnvironmentSize)EditorGUI.EnumPopup(fieldRect, m_targetSize);

                labelRect.y += lineHeight * 1.5f;
                fieldRect.y += lineHeight * 1.5f;
                GUILayout.Space(lineHeight);

                int world_xDimension = m_settings.m_tilesX * m_settings.m_currentDefaults.m_terrainSize;
                int world_zDimension = m_settings.m_tilesZ * m_settings.m_currentDefaults.m_terrainSize;
                int numberOfTerrains = m_settings.m_tilesX * m_settings.m_tilesZ;

                GUIContent worldSizeInfo = new GUIContent(m_editorUtils.GetContent("TotalWorldSize").text + String.Format(": {0} x {1}, " + m_editorUtils.GetContent("Terrains").text + ": {2}", world_xDimension, world_zDimension, numberOfTerrains));
                EditorGUI.LabelField(new Rect(labelRect.x, labelRect.y, labelRect.width + fieldRect.width, labelRect.height), worldSizeInfo, m_editorUtils.Styles.heading);

                if (tilesX != m_settings.m_tilesX || tilesZ != m_settings.m_tilesZ || m_targetSize != m_settings.m_currentSize)
                {

                    m_settings.m_tilesX = tilesX;
                    m_settings.m_tilesZ = tilesZ;

                    if (m_settings.m_tilesX > 1 || m_settings.m_tilesZ > 1 || m_targetSize == GaiaConstants.EnvironmentSize.Is8192MetersSq || m_targetSize == GaiaConstants.EnvironmentSize.Is16384MetersSq)
                    {
                        m_targeSizePreset = GaiaConstants.EnvironmentSizePreset.Custom;
                    }
                    else
                    {
                        switch (m_targetSize)
                        {
                            case GaiaConstants.EnvironmentSize.Is256MetersSq:
                                m_targeSizePreset = GaiaConstants.EnvironmentSizePreset.Tiny;
                                break;
                            case GaiaConstants.EnvironmentSize.Is512MetersSq:
                                m_targeSizePreset = GaiaConstants.EnvironmentSizePreset.Small;
                                break;
                            case GaiaConstants.EnvironmentSize.Is1024MetersSq:
                                m_targeSizePreset = GaiaConstants.EnvironmentSizePreset.Medium;
                                break;
                            case GaiaConstants.EnvironmentSize.Is2048MetersSq:
                                m_targeSizePreset = GaiaConstants.EnvironmentSizePreset.Large;
                                break;
                            case GaiaConstants.EnvironmentSize.Is4096MetersSq:
                                m_targeSizePreset = GaiaConstants.EnvironmentSizePreset.XLarge;
                                break;
                        }

                    }

                    EditorUtility.SetDirty(m_settings);
                }
                EditorGUI.indentLevel--;
            } //end of foldout

            labelRect.y += lineHeight * 2.5f;
            fieldRect.y += lineHeight * 2.5f;

            if (helpEnabled)
            {

                GUILayout.Space(lineHeight * 0.5f);
                m_editorUtils.InlineHelp("Quality Header", helpEnabled);
                labelRect.y += GUILayoutUtility.GetLastRect().height;
                fieldRect.y += GUILayoutUtility.GetLastRect().height;
                GUILayout.Space(lineHeight * 1.5f);
            }
            else
            {
                GUILayout.Space(lineHeight * 2.5f);
            }


            EditorGUI.LabelField(new Rect(labelRect.x - EditorGUIUtility.labelWidth, labelRect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("Quality Header").text, m_editorUtils.Styles.heading);
            //EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("Environment"));
            m_targetEnv = (GaiaConstants.EnvironmentTarget)EditorGUI.EnumPopup(new Rect(labelRect.x, labelRect.y, labelRect.width + fieldRect.width, labelRect.height), m_targetEnv);

            labelRect.y += lineHeight;
            fieldRect.y += lineHeight;
            //GUILayout.Space(lineHeight);

            if (m_targetEnv == GaiaConstants.EnvironmentTarget.Custom)
            {
                m_foldoutTerrainResolutionSettings = true;
            }

            bool resSettingsChangeCheck = false;

            Rect resSettingsRect = EditorGUILayout.GetControlRect();
            resSettingsRect.y = labelRect.y;
            m_foldoutTerrainResolutionSettings = EditorGUI.Foldout(new Rect(resSettingsRect.x + EditorGUIUtility.labelWidth, resSettingsRect.y, resSettingsRect.width, resSettingsRect.height), m_foldoutTerrainResolutionSettings, m_editorUtils.GetContent("AdvancedQuality"));
            if (m_foldoutTerrainResolutionSettings)
            {
                EditorGUI.indentLevel++;
                resSettingsChangeCheck = TerrainResolutionSettingsEnabled(resSettingsRect, false);
                EditorGUI.indentLevel--;
                labelRect.y += EditorGUIUtility.singleLineHeight * 5;
                fieldRect.y += EditorGUIUtility.singleLineHeight * 5;
                GUILayout.Space(EditorGUIUtility.singleLineHeight * 5);
            }


            labelRect.y += lineHeight * 1.5f;
            fieldRect.y += lineHeight * 1.5f;

            if (helpEnabled)
            {
                m_editorUtils.InlineHelp("BiomePreset", helpEnabled);
                labelRect.y += GUILayoutUtility.GetLastRect().height;
                fieldRect.y += GUILayoutUtility.GetLastRect().height;
                //GUILayout.Space(lineHeight * 2f);
            }


            EditorGUI.LabelField(new Rect(labelRect.x - EditorGUIUtility.labelWidth, labelRect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("BiomePreset").text, m_editorUtils.Styles.heading);
            int lastBiomePresetSelection = m_biomePresetSelection;
            if (m_biomePresetSelection == int.MinValue)
            {
                m_biomePresetSelection = 0;
            }

            m_biomePresetSelection = EditorGUI.IntPopup(new Rect(labelRect.x, labelRect.y, labelRect.width + fieldRect.width, labelRect.height), m_biomePresetSelection, m_allBiomePresets.Select(x => x.name).ToArray(), m_allBiomePresets.Select(x => x.ID).ToArray());

            if (lastBiomePresetSelection != m_biomePresetSelection)
            {
                AddBiomeSpawnersForSelectedPreset();
                //re-create the reorderable list with the new contents
                CreateBiomePresetList();
            }

            labelRect.y += lineHeight;
            fieldRect.y += lineHeight;
            GUILayout.Space(lineHeight);

            if (m_biomePresetSelection == -999)
            {
                m_foldoutSpawnerSettings = true;
            }

            Rect spawnerFoldOutRect = EditorGUILayout.GetControlRect();
            spawnerFoldOutRect.y = labelRect.y;
            m_foldoutSpawnerSettings = EditorGUI.Foldout(new Rect(spawnerFoldOutRect.x + EditorGUIUtility.labelWidth, spawnerFoldOutRect.y, spawnerFoldOutRect.width, spawnerFoldOutRect.height), m_foldoutSpawnerSettings, m_editorUtils.GetContent("AdvancedSpawners"));
            if (m_foldoutSpawnerSettings)
            {
                //the hardcoded 15 are for some indent below the foldout label
                Rect listRect = new Rect(spawnerFoldOutRect.x + EditorGUIUtility.labelWidth + 15, spawnerFoldOutRect.y + EditorGUIUtility.singleLineHeight, spawnerFoldOutRect.width - EditorGUIUtility.labelWidth - 15, m_biomeSpawnersList.GetHeight()); //EditorGUILayout.GetControlRect(true, m_spawnerPresetList.GetHeight());
                m_biomeSpawnersList.DoList(listRect);
                GUILayout.Space(m_biomeSpawnersList.GetHeight());
                labelRect.y += m_biomeSpawnersList.GetHeight();
                fieldRect.y += m_biomeSpawnersList.GetHeight();
            }


            if (helpEnabled)
            {
                GUILayout.Space(lineHeight * 0.5f);
                m_editorUtils.InlineHelp("Extras", helpEnabled);
                labelRect.y += GUILayoutUtility.GetLastRect().height;
                fieldRect.y += GUILayoutUtility.GetLastRect().height;
                labelRect.y += lineHeight * 1.5f;
                fieldRect.y += lineHeight * 1.5f;
                GUILayout.Space(GUILayoutUtility.GetLastRect().height);

            }
            else
            {
                labelRect.y += lineHeight * 2;
                fieldRect.y += lineHeight * 2;
                GUILayout.Space(lineHeight * 2f);
            }



            EditorGUI.LabelField(new Rect(labelRect.x - EditorGUIUtility.labelWidth, labelRect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("Extras").text, m_editorUtils.Styles.heading);

            Rect extrasFoldOut = EditorGUILayout.GetControlRect();
            extrasFoldOut.y = labelRect.y;
            m_foldoutExtrasSettings = EditorGUI.Foldout(new Rect(extrasFoldOut.x + EditorGUIUtility.labelWidth, extrasFoldOut.y, extrasFoldOut.width, extrasFoldOut.height), m_foldoutExtrasSettings, m_editorUtils.GetContent("AdvancedExtras"));
            if (m_foldoutExtrasSettings)
            {
                EditorGUI.indentLevel++;
                ExtrasSettingsEnabled(extrasFoldOut, helpEnabled);
                EditorGUI.indentLevel--;
                labelRect.y += EditorGUIUtility.singleLineHeight * 4;
                fieldRect.y += EditorGUIUtility.singleLineHeight * 4;
                if (m_settings.m_pipelineProfile.m_activePipelineInstalled != m_settings.m_currentRenderer)
                {
                    //Need more space when the change pipeline button is drawn
                    GUILayout.Space(EditorGUIUtility.singleLineHeight * 12);
                }
                else
                {
                    GUILayout.Space(EditorGUIUtility.singleLineHeight * 9);
                }
            }
            //int lastBiomePresetSelection = m_biomePresetSelection;
            //if (m_biomePresetSelection == int.MinValue)
            //{
            //    m_biomePresetSelection = 0;
            //}



            //            EditorGUILayout.EndVertical();




            //            if (targetRenderer != m_settings.m_currentRenderer)
            //            {
            //                if (Application.isPlaying)
            //                {
            //                    Debug.LogWarning("Can't switch render pipelines in play mode.");
            //                    targetRenderer = m_settings.m_currentRenderer;
            //                }
            //                else
            //                {
            //#if !UNITY_2018_3_OR_NEWER
            //                    EditorUtility.DisplayDialog("Pipeline change not supported", "Lightweight and High Definition is only supported in 2018.3 or higher. To use Gaia selected pipeline, please install and upgrade your project to 2018.3.x. Switching back to Built-In Pipeline.", "OK");
            //                    targetRenderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
            //#else
            //                    if (EditorUtility.DisplayDialog("CHANGE RENDER PIPELINE",
            //                                "You are about to install a new render pipeline!" +
            //                                "\nPlease BACKUP your project first!" +
            //                                "\nAre you sure?",
            //                                "Yes", "No"))
            //                    {

            //                        bool upgradeMaterials = false;
            //                        if (EditorUtility.DisplayDialog("UPGRADE MATERIALS",
            //                            "Upgrade materials to the " + targetRenderer.ToString() + " pipeline?" +
            //                            "\nWARNING: THIS PROCESS CAN NOT BE UNDONE!" +
            //                            "\nSay NO and change pipeline back if unsure!",
            //                            "Yes", "No"))
            //                        {
            //                            upgradeMaterials = true;
            //                        }

            //                        /*
            //                        bool finalizeEnvironment = false;
            //                        if (EditorUtility.DisplayDialog("FINALIZE ENVIRONMENT",
            //                            "Finalizing the environment will configure your scenes lighting and setup post processing. This will overwrite your current lighting and skybox settings. Would you like to finalize your environment?",
            //                            "Yes", "No"))
            //                        {
            //                            finalizeEnvironment = true;
            //                        }
            //                        */

            //                        GaiaPipelineUtilsEditor.ShowGaiaPipelineUtilsEditor(m_settings.m_currentRenderer, targetRenderer, upgradeMaterials, this, true);


            //                    }
            //#endif
            //                }
            //}



            if (resSettingsChangeCheck || m_targetEnv != m_settings.m_currentEnvironment || m_targetSize != m_settings.m_currentSize)
            {
                if (m_targetSize != m_settings.m_currentSize)
                {
                    m_settings.m_currentSize = m_targetSize;
                }

                switch (m_settings.m_currentSize)
                {
                    case GaiaConstants.EnvironmentSize.Is256MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 256;
                        break;
                    case GaiaConstants.EnvironmentSize.Is512MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 512;
                        break;
                    case GaiaConstants.EnvironmentSize.Is1024MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 1024;
                        break;
                    case GaiaConstants.EnvironmentSize.Is2048MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 2048;
                        break;
                    case GaiaConstants.EnvironmentSize.Is4096MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 4096;
                        break;
                    case GaiaConstants.EnvironmentSize.Is8192MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 8192;
                        break;
                    case GaiaConstants.EnvironmentSize.Is16384MetersSq:
                        m_settings.m_currentDefaults.m_terrainSize = 16384;
                        break;
                }


                if (m_targetEnv != m_settings.m_currentEnvironment)
                {
                    switch (m_targetEnv)
                    {
                        case GaiaConstants.EnvironmentTarget.UltraLight:
                            m_settings.m_currentDefaults = m_settings.m_ultraLightDefaults;
                            m_settings.m_currentResources = m_settings.m_ultraLightResources;
                            m_settings.m_currentGameObjectResources = m_settings.m_ultraLightGameObjectResources;
                            m_settings.m_currentWaterPrefabName = m_settings.m_waterMobilePrefabName;
                            //m_settings.m_currentSize = GaiaConstants.EnvironmentSize.Is512MetersSq;
                            break;
                        case GaiaConstants.EnvironmentTarget.MobileAndVR:
                            m_settings.m_currentDefaults = m_settings.m_mobileDefaults;
                            m_settings.m_currentResources = m_settings.m_mobileResources;
                            m_settings.m_currentGameObjectResources = m_settings.m_mobileGameObjectResources;
                            m_settings.m_currentWaterPrefabName = m_settings.m_waterMobilePrefabName;
                            //m_settings.m_currentSize = GaiaConstants.EnvironmentSize.Is1024MetersSq;
                            break;
                        case GaiaConstants.EnvironmentTarget.Desktop:
                            m_settings.m_currentDefaults = m_settings.m_desktopDefaults;
                            m_settings.m_currentResources = m_settings.m_desktopResources;
                            m_settings.m_currentGameObjectResources = m_settings.m_desktopGameObjectResources;
                            m_settings.m_currentWaterPrefabName = m_settings.m_waterPrefabName;
                            //m_settings.m_currentSize = GaiaConstants.EnvironmentSize.Is2048MetersSq;
                            break;
                        case GaiaConstants.EnvironmentTarget.PowerfulDesktop:
                            m_settings.m_currentDefaults = m_settings.m_powerDesktopDefaults;
                            m_settings.m_currentResources = m_settings.m_powerDesktopResources;
                            m_settings.m_currentGameObjectResources = m_settings.m_powerDesktopGameObjectResources;
                            m_settings.m_currentWaterPrefabName = m_settings.m_waterPrefabName;
                            //m_settings.m_currentSize = GaiaConstants.EnvironmentSize.Is2048MetersSq;
                            break;
                    }
                }

                switch (m_targetEnv)
                {
                    case GaiaConstants.EnvironmentTarget.UltraLight:
                        m_settings.m_currentDefaults.m_heightmapResolution = 33;
                        m_settings.m_currentDefaults.m_baseMapDist = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 8, 256, 512);
                        m_settings.m_currentDefaults.m_detailResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 8, 128, 512);
                        m_settings.m_currentDefaults.m_controlTextureResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 8, 64, 512);
                        m_settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 8, 64, 512);
                        break;
                    case GaiaConstants.EnvironmentTarget.MobileAndVR:
                        m_settings.m_currentDefaults.m_heightmapResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 4, 64, 512) + 1;
                        m_settings.m_currentDefaults.m_baseMapDist = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 4, 256, 512);
                        m_settings.m_currentDefaults.m_detailResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 4, 64, 512);
                        m_settings.m_currentDefaults.m_controlTextureResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 4, 64, 512);
                        m_settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 4, 64, 512);
                        break;
                    case GaiaConstants.EnvironmentTarget.Desktop:
                        m_settings.m_currentDefaults.m_heightmapResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 2, 256, 2048) + 1;
                        m_settings.m_currentDefaults.m_baseMapDist = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 2, 256, 2048);
                        m_settings.m_currentDefaults.m_detailResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 2, 256, 4096);
                        m_settings.m_currentDefaults.m_controlTextureResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 2, 256, 2048);
                        m_settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / 2, 256, 2048);
                        break;
                    case GaiaConstants.EnvironmentTarget.PowerfulDesktop:
                        m_settings.m_currentDefaults.m_heightmapResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize, 256, 4096) + 1;
                        m_settings.m_currentDefaults.m_baseMapDist = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize, 256, 2048);
                        m_settings.m_currentDefaults.m_detailResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize, 256, 4096);
                        m_settings.m_currentDefaults.m_controlTextureResolution = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize, 256, 2048);
                        m_settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize, 256, 2048);
                        break;
                    case GaiaConstants.EnvironmentTarget.Custom:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (m_targetEnv != GaiaConstants.EnvironmentTarget.Custom)
                {
                    m_heightmapResolution = (GaiaConstants.HeightmapResolution)m_settings.m_currentDefaults.m_heightmapResolution;
                    m_controlTextureResolution = (GaiaConstants.TerrainTextureResolution)m_settings.m_currentDefaults.m_controlTextureResolution;
                    m_basemapResolution = (GaiaConstants.TerrainTextureResolution)m_settings.m_currentDefaults.m_baseMapSize;
                    m_detailResolutionPerPatch = m_settings.m_currentDefaults.m_detailResolutionPerPatch;
                    m_detailResolution = m_settings.m_currentDefaults.m_detailResolution;
                }
                m_settings.m_currentEnvironment = m_targetEnv;
                m_settings.m_currentDefaults.m_heightmapResolution = (int)m_heightmapResolution;
                m_settings.m_currentDefaults.m_controlTextureResolution = (int)m_controlTextureResolution;
                m_settings.m_currentDefaults.m_baseMapSize = (int)m_basemapResolution;
                m_detailResolutionPerPatch = Mathf.RoundToInt(Mathf.Clamp(m_detailResolutionPerPatch, 8, 128));
                m_detailResolution = Mathf.RoundToInt(Mathf.Clamp(m_detailResolution, 0, 4096));
                m_settings.m_currentDefaults.m_detailResolutionPerPatch = m_detailResolutionPerPatch;
                m_settings.m_currentDefaults.m_detailResolution = m_detailResolution;
                EditorUtility.SetDirty(m_settings);
                EditorUtility.SetDirty(m_settings.m_currentDefaults);
            }


            //if (targetControllerType != m_settings.m_currentController)
            //{
            //    m_settings.m_currentController = targetControllerType;
            //    switch (targetControllerType)
            //    {
            //        case GaiaConstants.EnvironmentControllerType.FirstPerson:
            //            m_settings.m_currentPlayerPrefabName = m_settings.m_fpsPlayerPrefabName;
            //            break;
            //        case GaiaConstants.EnvironmentControllerType.ThirdPerson:
            //            m_settings.m_currentPlayerPrefabName = m_settings.m_3pPlayerPrefabName;
            //            break;
            //        //case GaiaConstants.EnvironmentControllerType.Rollerball:
            //        //m_settings.m_currentPlayerPrefabName = m_settings.m_rbPlayerPrefabName;
            //        //break;
            //        case GaiaConstants.EnvironmentControllerType.FlyingCamera:
            //            m_settings.m_currentPlayerPrefabName = "Flycam";
            //            break;
            //    }
            //    EditorUtility.SetDirty(m_settings);
            //}


            //     EditorGUILayout.BeginVertical(m_boxStyle);

            //GUILayout.Label(m_editorUtils.GetContent("SpawnerHeader"), m_editorUtils.Styles.heading);

            //if the preset selection still is in initialized state, switch it to 0 now.



            //GUILayout.Label(m_editorUtils.GetContent("SpawnerAdded"));

            //GUILayout.Space(6);
        }

        internal void UpdateAllSpawnersList()
        {
            CreateAdvancedTabSpawnersList();
        }

        /// <summary>
        /// Terrain resolution settings foldout
        /// </summary>
        /// <param name="helpEnabled"></param>
        private bool TerrainResolutionSettingsEnabled(Rect rect, bool helpEnabled)
        {
            EditorGUI.BeginChangeCheck();



            Rect labelRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, EditorGUIUtility.labelWidth, rect.height);
            Rect fieldRect = new Rect(labelRect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - labelRect.width - EditorGUIUtility.labelWidth, rect.height);

            labelRect.y += EditorGUIUtility.singleLineHeight;
            fieldRect.y += EditorGUIUtility.singleLineHeight;

            ////Display notice that these fields cannot be edited if the custom setting is not chosen
            //if (m_targetEnv != GaiaConstants.EnvironmentTarget.Custom)
            //{
            //    EditorGUI.LabelField(fieldRect, m_editorUtils.GetContent("QualityCustomNotice"));
            //    labelRect.y += EditorGUIUtility.singleLineHeight;
            //    fieldRect.y += EditorGUIUtility.singleLineHeight;
            //    GUI.enabled = false;
            //}

            EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("Heightmap Resolution"));
            m_heightmapResolution = (GaiaConstants.HeightmapResolution)EditorGUI.EnumPopup(fieldRect, m_heightmapResolution);

            labelRect.y += EditorGUIUtility.singleLineHeight;
            fieldRect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("Control Texture Resolution"));
            m_controlTextureResolution = (GaiaConstants.TerrainTextureResolution)EditorGUI.EnumPopup(fieldRect, m_controlTextureResolution);

            labelRect.y += EditorGUIUtility.singleLineHeight;
            fieldRect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("Basemap Resolution"));
            m_basemapResolution = (GaiaConstants.TerrainTextureResolution)EditorGUI.EnumPopup(fieldRect, m_basemapResolution);

            labelRect.y += EditorGUIUtility.singleLineHeight;
            fieldRect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("Detail Resolution Per Patch"));
            m_detailResolutionPerPatch = EditorGUI.IntField(fieldRect, m_detailResolutionPerPatch);

            labelRect.y += EditorGUIUtility.singleLineHeight;
            fieldRect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("Detail Resolution"));
            m_detailResolution = EditorGUI.IntField(fieldRect, m_detailResolution);



            //m_heightmapResolution = (GaiaConstants.HeightmapResolution)m_editorUtils.EnumPopup("Heightmap Resolution", m_heightmapResolution, helpEnabled);
            //m_controlTextureResolution = (GaiaConstants.TerrainTextureResolution)m_editorUtils.EnumPopup("Control Texture Resolution", m_controlTextureResolution, helpEnabled);
            //m_basemapResolution = (GaiaConstants.TerrainTextureResolution)m_editorUtils.EnumPopup("Basemap Resolution", m_basemapResolution, helpEnabled);
            //m_detailResolutionPerPatch = m_editorUtils.IntField("Detail Resolution Per Patch", m_detailResolutionPerPatch, helpEnabled);
            //m_detailResolution = m_editorUtils.IntField("Detail Resolution", m_detailResolution, helpEnabled);

            bool changeCheckTriggered = false;

            if (EditorGUI.EndChangeCheck())
            {
                m_targetEnv = GaiaConstants.EnvironmentTarget.Custom;
                changeCheckTriggered = true;
            }

            return changeCheckTriggered;


        }

        private bool ExtrasSettingsEnabled(Rect rect, bool helpEnabled)
        {
            EditorGUI.BeginChangeCheck();

            Rect labelRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, EditorGUIUtility.labelWidth, rect.height);
            Rect fieldRect = new Rect(labelRect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - labelRect.width - EditorGUIUtility.labelWidth, rect.height);
            Rect buttonRect = labelRect;

            labelRect.y += EditorGUIUtility.singleLineHeight;
            fieldRect.y += EditorGUIUtility.singleLineHeight;


            labelRect.y += EditorGUIUtility.singleLineHeight;
            fieldRect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("Controller"));
            m_settings.m_currentController = (GaiaConstants.EnvironmentControllerType)EditorGUI.EnumPopup(fieldRect, m_settings.m_currentController);
            switch (m_settings.m_currentController)
            {
                case GaiaConstants.EnvironmentControllerType.FirstPerson:
                    m_settings.m_currentPlayerPrefabName = "FPSController";
                    break;
                case GaiaConstants.EnvironmentControllerType.ThirdPerson:
                    m_settings.m_currentPlayerPrefabName = "ThirdPersonController";
                    break;
                case GaiaConstants.EnvironmentControllerType.FlyingCamera:
                    m_settings.m_currentPlayerPrefabName = "FlyCam";
                    break;
            }

            labelRect.y += EditorGUIUtility.singleLineHeight;
            fieldRect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("Skies"));
            m_settings.m_currentSkies = (GaiaConstants.GaiaLightingProfileType)EditorGUI.EnumPopup(fieldRect, m_settings.m_currentSkies);
            if (m_settings.m_currentSkies != GaiaConstants.GaiaLightingProfileType.None)
            {
#if UNITY_POST_PROCESSING_STACK_V2
                labelRect.y += EditorGUIUtility.singleLineHeight;
                fieldRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("PostProcessing"));
                m_settings.m_enablePostProcessing = EditorGUI.Toggle(fieldRect, m_settings.m_enablePostProcessing);
#endif

                labelRect.y += EditorGUIUtility.singleLineHeight;
                fieldRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("AmbientAudio"));
                m_settings.m_enableAmbientAudio = EditorGUI.Toggle(fieldRect, m_settings.m_enableAmbientAudio);
            }

            //labelRect.y += EditorGUIUtility.singleLineHeight;
            //fieldRect.y += EditorGUIUtility.singleLineHeight;

            //EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("PostFX"));
            //m_settings.m_currentPostFX = (GaiaConstants.PostFX)EditorGUI.EnumPopup(fieldRect, m_settings.m_currentPostFX);

            labelRect.y += EditorGUIUtility.singleLineHeight;
            fieldRect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("Water"));
            if (newProfileListIndex > m_profileList.Count)
            {
                newProfileListIndex = 0;
            }
            newProfileListIndex = EditorGUI.Popup(fieldRect, newProfileListIndex, m_profileList.ToArray());

            if (m_settings.m_selectedWaterProfile != newProfileListIndex)
            {
                m_settings.m_selectedWaterProfile = newProfileListIndex;
                m_settings.m_gaiaWaterProfile.m_activeWaterMaterial = m_allMaterials[newProfileListIndex];
            }
            if (m_settings.m_currentWaterPro != GaiaConstants.GaiaWaterProfileType.None)
            {
                labelRect.y += EditorGUIUtility.singleLineHeight;
                fieldRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("UnderwaterEffects"));
                m_settings.m_enableUnderwaterEffects = EditorGUI.Toggle(fieldRect, m_settings.m_enableUnderwaterEffects);
            }

            labelRect.y += EditorGUIUtility.singleLineHeight;
            fieldRect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("Wind"));
            m_settings.m_createWind = EditorGUI.Toggle(fieldRect, m_settings.m_createWind);
            if (m_settings.m_createWind)
            {
                labelRect.y += EditorGUIUtility.singleLineHeight;
                fieldRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("WindType"));
                m_settings.m_windType = (GaiaConstants.GaiaGlobalWindType)EditorGUI.EnumPopup(fieldRect, m_settings.m_windType);
            }

            labelRect.y += EditorGUIUtility.singleLineHeight;
            fieldRect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("Screenshotter"));
            m_settings.m_createScreenShotter = EditorGUI.Toggle(fieldRect, m_settings.m_createScreenShotter);

            //m_heightmapResolution = (GaiaConstants.HeightmapResolution)m_editorUtils.EnumPopup("Heightmap Resolution", m_heightmapResolution, helpEnabled);
            //m_controlTextureResolution = (GaiaConstants.TerrainTextureResolution)m_editorUtils.EnumPopup("Control Texture Resolution", m_controlTextureResolution, helpEnabled);
            //m_basemapResolution = (GaiaConstants.TerrainTextureResolution)m_editorUtils.EnumPopup("Basemap Resolution", m_basemapResolution, helpEnabled);
            //m_detailResolutionPerPatch = m_editorUtils.IntField("Detail Resolution Per Patch", m_detailResolutionPerPatch, helpEnabled);
            //m_detailResolution = m_editorUtils.IntField("Detail Resolution", m_detailResolution, helpEnabled);

            bool changeCheckTriggered = false;

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_settings.m_gaiaLightingProfile);
                m_settings.m_gaiaLightingProfile.m_enableAmbientAudio = m_settings.m_enableAmbientAudio;
                m_settings.m_gaiaLightingProfile.m_enablePostProcessing = m_settings.m_enablePostProcessing;

                EditorUtility.SetDirty(m_settings.m_gaiaWaterProfile);
                m_settings.m_gaiaWaterProfile.m_supportUnderwaterEffects = m_settings.m_enableUnderwaterEffects;

                changeCheckTriggered = true;
            }

            return changeCheckTriggered;


        }

        /// <summary>
        /// Editor Update
        /// </summary>
        public void EditorPipelineUpdate()
        {
            if (m_settings == null)
            {
                m_settings = GaiaUtils.GetGaiaSettings();
            }
            if (m_settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                GaiaLWRPPipelineUtils.StartLWRPSetup(m_settings.m_pipelineProfile).MoveNext();
            }
            else if (m_settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition)
            {
                GaiaHDRPPipelineUtils.StartHDRPSetup(m_settings.m_pipelineProfile).MoveNext();
            }
        }

        #endregion

        #region Gaia Main Function Calls
        /// <summary>
        /// Create and returns a defaults asset
        /// </summary>
        /// <returns>New defaults asset</returns>
        public static GaiaDefaults CreateDefaultsAsset()
        {
            GaiaDefaults defaults = ScriptableObject.CreateInstance<Gaia.GaiaDefaults>();
            AssetDatabase.CreateAsset(defaults, string.Format(GaiaDirectories.GetDataDirectory() + "/GD-{0:yyyyMMdd-HHmmss}.asset", DateTime.Now));
            AssetDatabase.SaveAssets();
            return defaults;
        }

        /// <summary>
        /// Create and returns a resources asset
        /// </summary>
        /// <returns>New resources asset</returns>
        //public static GaiaResource CreateResourcesAsset()
        //{
        //    GaiaResource resources = ScriptableObject.CreateInstance<Gaia.GaiaResource>();
        //    AssetDatabase.CreateAsset(resources, string.Format(GaiaDirectories.GetDataDirectory() + "/GR-{0:yyyyMMdd-HHmmss}.asset", DateTime.Now));
        //    AssetDatabase.SaveAssets();
        //    return resources;
        //}

        /// <summary>
        /// Set up the Gaia Present defines
        /// </summary>
        public static void SetGaiaDefinesStatic()
        {
            string currBuildSettings = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            //Check for and inject GAIA_PRESENT
            if (!currBuildSettings.Contains("GAIA_PRESENT"))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currBuildSettings + ";GAIA_PRESENT");
            }
        }

        /// <summary>
        /// Create the terrain
        /// </summary>
        void CreateTerrain()
        {
            //Only do this if we have < 1 terrain
            int actualTerrainCount = Gaia.TerrainHelper.GetActiveTerrainCount();
            if (actualTerrainCount != 0)
            {
                EditorUtility.DisplayDialog(m_editorUtils.GetTextValue("Already a terrain in the scene"), string.Format(m_editorUtils.GetTextValue("You currently have {0} active terrains in your scene, Gaia will not create a terrain for you, but will create the biome spawner you selected."), actualTerrainCount, 0), m_editorUtils.GetTextValue("OK"));
            }
            else
            {
                //Disable automatic light baking - this kills perf on most systems
                Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;

                //Create the terrain
                m_settings.m_currentDefaults.CreateTerrain(m_BiomeSpawnersToCreate);

                //Adjust the scene view so you can see the terrain
                if (SceneView.lastActiveSceneView != null)
                {
                    if (m_settings != null)
                    {
                        SceneView.lastActiveSceneView.LookAtDirect(new Vector3(0f, 300f, -1f * (m_settings.m_currentDefaults.m_terrainSize / 2f)), Quaternion.Euler(30f, 0f, 0f));
                        Repaint();
                    }
                }
            }
        }

        /// <summary>
        /// Create / show the session manager
        /// </summary>
        GameObject ShowSessionManager(bool pickupExistingTerrain = false)
        {
            GameObject mgrObj = GaiaSessionManager.GetSessionManager(pickupExistingTerrain).gameObject;
            Selection.activeGameObject = mgrObj;
            return mgrObj;
        }

        /// <summary>
        /// Select or create a stamper
        /// </summary>
        void ShowStamper(List<Spawner> autoSpawnerCandidates = null)
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return;
            }

            //Make sure we have a session manager
            //m_sessionManager = m_resources.CreateOrFindSessionManager().GetComponent<GaiaSessionManager>();

            //Make sure we have gaia object
            GameObject gaiaObj = m_settings.m_currentResources.CreateOrFindGaia();

            //Create or find the stamper
            GameObject stamperObj = GameObject.Find("Stamper");
            if (stamperObj == null)
            {
                stamperObj = new GameObject("Stamper");
                stamperObj.transform.parent = gaiaObj.transform;
                Stamper stamper = stamperObj.AddComponent<Stamper>();
                stamper.m_settings = ScriptableObject.CreateInstance<StamperSettings>();
                //stamper.m_resources = m_settings.m_currentResources;
                stamper.FitToAllTerrains();
                //Add an image mask as start configuration
                stamper.m_settings.m_imageMasks = new ImageMask[1];
                stamper.m_settings.m_imageMasks[0] = new ImageMask();

                if (m_settings != null && m_settings.m_defaultStamp != null)
                {
                    stamper.m_settings.m_imageMasks[0].m_imageMaskTexture = m_settings.m_defaultStamp;
                }
                ImageMaskListEditor.OpenStampBrowser(stamper.m_settings.m_imageMasks[0]);
                stamper.UpdateStamp();
                stamperObj.transform.position = new Vector3(stamper.m_settings.m_x, stamper.m_settings.m_y, stamper.m_settings.m_z);
                stamper.UpdateMinMaxHeight();
                stamper.m_seaLevel = m_settings.m_currentDefaults.m_seaLevel;

                //if spawners are supplied, set them up for automatic spawning
                if (m_BiomeSpawnersToCreate != null && m_BiomeSpawnersToCreate.Count > 0)
                {
                    foreach (Spawner autoSpawnerCandidate in autoSpawnerCandidates)
                    {
                        AutoSpawner newAutoSpawner = new AutoSpawner()
                        {
                            isActive = true,
                            status = AutoSpawnerStatus.Initial,
                            spawner = autoSpawnerCandidate
                        };
                        stamper.m_autoSpawners.Add(newAutoSpawner);
                    }
                }
            }
            Selection.activeGameObject = stamperObj;
        }

        /// <summary>
        /// Select or create a scanner
        /// </summary>
        GameObject CreateScanner()
        {
            GameObject gaiaObj = m_settings.m_currentResources.CreateOrFindGaia();
            GameObject scannerObj = GameObject.Find("Scanner");
            if (scannerObj == null)
            {
                scannerObj = new GameObject("Scanner");
                scannerObj.transform.parent = gaiaObj.transform;
                scannerObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter(false);
                Scanner scanner = scannerObj.AddComponent<Scanner>();

                //Load the material to draw it
                string matPath = GetAssetPath("GaiaScannerMaterial");
                if (!string.IsNullOrEmpty(matPath))
                {
                    scanner.m_previewMaterial = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                }
            }
            return scannerObj;
        }

        /// <summary>
        /// Create or select the existing visualiser
        /// </summary>
        /// <returns>New or exsiting visualiser - or null if no terrain</returns>
        GameObject ShowVisualiser()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return null;
            }

            GameObject gaiaObj = m_settings.m_currentResources.CreateOrFindGaia();
            GameObject visualiserObj = GameObject.Find("Visualiser");
            if (visualiserObj == null)
            {
                visualiserObj = new GameObject("Visualiser");
                visualiserObj.AddComponent<ResourceVisualiser>();
                visualiserObj.transform.parent = gaiaObj.transform;

                //Center it on the terrain
                visualiserObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();
            }
            ResourceVisualiser visualiser = visualiserObj.GetComponent<ResourceVisualiser>();
            visualiser.m_resources = m_settings.m_currentResources;
            return visualiserObj;
        }

        /// <summary>
        /// Show a normal exporter
        /// </summary>
        void ShowNormalMaskExporter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<GaiaNormalExporterEditor>(false, m_editorUtils.GetTextValue("Normalmap Exporter"));
            export.Show();
        }

        /// <summary>
        /// Show the terrain height adjuster
        /// </summary>
        void ShowTerrainHeightAdjuster()
        {
            var export = EditorWindow.GetWindow<GaiaTerrainHeightAdjuster>(false, m_editorUtils.GetTextValue("Height Adjuster"));
            export.Show();
        }

        /// <summary>
        /// Show the terrain explorer helper
        /// </summary>
        void ShowTerrainUtilties()
        {
            var export = EditorWindow.GetWindow<GaiaTerrainExplorerEditor>(false, m_editorUtils.GetTextValue("Terrain Utilities"));
            export.Show();
        }

        /// <summary>
        /// Show a texture mask exporter
        /// </summary>
        void ShowTexureMaskExporter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<GaiaMaskExporterEditor>(false, m_editorUtils.GetTextValue("Splatmap Exporter"));
            export.Show();
        }

        /// <summary>
        /// Show a grass mask exporter
        /// </summary>
        void ShowGrassMaskExporter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<GaiaGrassMaskExporterEditor>(false, m_editorUtils.GetTextValue("Grassmask Exporter"));
            export.Show();
        }

        /// <summary>
        /// Show flowmap exporter
        /// </summary>
        void ShowFlowMapMaskExporter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<GaiaWaterflowMapEditor>(false, m_editorUtils.GetTextValue("Flowmap Exporter"));
            export.Show();
        }

        /// <summary>
        /// Show a terrain obj exporter
        /// </summary>
        void ShowTerrainObjExporter()
        {
            //if (DisplayErrorIfNotMinimumTerrainCount(1))
            //{
            //    return;
            //}

            var export = EditorWindow.GetWindow<ExportTerrain>(false, m_editorUtils.GetTextValue("Export Terrain"));
            export.Show();
        }

        /// <summary>
        /// Export the world as a PNG heightmap
        /// </summary>
        void ExportWorldAsHeightmapPNG()
        {
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return;
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                string path = GaiaDirectories.GetMaskDirectory();
                path = Path.Combine(path, PWCommon2.Utils.FixFileName(string.Format("Terrain-Heightmap-{0:yyyyMMdd-HHmmss}", DateTime.Now)));
                mgr.ExportWorldAsPng(path);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog(
                    m_editorUtils.GetTextValue("Export complete"),
                    m_editorUtils.GetTextValue(" Your heightmap has been saved to : ") + path,
                    m_editorUtils.GetTextValue("OK"));
            }
        }

        /// <summary>
        /// Export the shore mask as a png file
        /// </summary>
        void ExportShoremaskAsPNG()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<ShorelineMaskerEditor>(false, m_editorUtils.GetTextValue("Export Shore"));
            export.m_seaLevel = GaiaSessionManager.GetSessionManager(false).GetSeaLevel();
            export.Show();
        }

        /// <summary>
        /// Show the Gaia Stamp converter
        /// </summary>
        void ShowGaiaStampConverter()
        {
            var convert = EditorWindow.GetWindow<ConvertStamps>(false, m_editorUtils.GetTextValue("Convert Stamps"));
            convert.Show();
        }


        /// <summary>
        /// Show the extension exporter
        /// </summary>
        void ShowExtensionExporterEditor()
        {
            var export = EditorWindow.GetWindow<GaiaExtensionExporterEditor>(false, m_editorUtils.GetTextValue("Export GX"));
            export.Show();
        }

        /// <summary>
        /// Display an error if there is not exactly one terrain
        /// </summary>
        /// <param name="requiredTerrainCount">The amount required</param>
        /// <param name="feature">The feature name</param>
        /// <returns>True if an error, false otherwise</returns>
        private bool DisplayErrorIfInvalidTerrainCount(int requiredTerrainCount, string feature = "")
        {
            int actualTerrainCount = Gaia.TerrainHelper.GetActiveTerrainCount();
            if (actualTerrainCount != requiredTerrainCount)
            {
                if (string.IsNullOrEmpty(feature))
                {
                    if (actualTerrainCount < requiredTerrainCount)
                    {
                        EditorUtility.DisplayDialog(
                            m_editorUtils.GetTextValue("OOPS!"),
                            string.Format(m_editorUtils.GetTextValue("You currently have {0} active terrains in your scene, but to " +
                            "use this feature you need {1}. Please create a terrain!"), actualTerrainCount, requiredTerrainCount),
                            m_editorUtils.GetTextValue("OK"));
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(
                            m_editorUtils.GetTextValue("OOPS!"),
                            string.Format(m_editorUtils.GetTextValue("You currently have {0} active terrains in your scene, but to " +
                            "use this feature you need {1}. Please remove terrain!"), actualTerrainCount, requiredTerrainCount),
                            m_editorUtils.GetTextValue("OK"));
                    }
                }
                else
                {
                    if (actualTerrainCount < requiredTerrainCount)
                    {
                        EditorUtility.DisplayDialog(
                            m_editorUtils.GetTextValue("OOPS!"),
                            string.Format(m_editorUtils.GetTextValue("You currently have {0} active terrains in your scene, but to " +
                            "use {2} you need {1}. Please create terrain!"), actualTerrainCount, requiredTerrainCount, feature),
                            m_editorUtils.GetTextValue("OK"));
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(
                            m_editorUtils.GetTextValue("OOPS!"),
                            string.Format(m_editorUtils.GetTextValue("You currently have {0} active terrains in your scene, but to " +
                            "use {2} you need {1}. Please remove terrain!"), actualTerrainCount, requiredTerrainCount, feature),
                            m_editorUtils.GetTextValue("OK"));
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Display an error if there is not exactly one terrain
        /// </summary>
        /// <param name="requiredTerrainCount">The amount required</param>
        /// <param name="feature">The feature name</param>
        /// <returns>True if an error, false otherwise</returns>
        private bool DisplayErrorIfNotMinimumTerrainCount(int requiredTerrainCount, string feature = "")
        {
            int actualTerrainCount = Gaia.TerrainHelper.GetActiveTerrainCount();
            if (actualTerrainCount < requiredTerrainCount)
            {
                if (string.IsNullOrEmpty(feature))
                {
                    if (actualTerrainCount < requiredTerrainCount)
                    {
                        EditorUtility.DisplayDialog(
                            m_editorUtils.GetTextValue("OOPS!"),
                            string.Format(m_editorUtils.GetTextValue("You currently have {0} active terrains in your scene, but to " +
                            "use this feature you need at least {1}. Please create a terrain!"), actualTerrainCount, requiredTerrainCount),
                            m_editorUtils.GetTextValue("OK"));
                    }
                }
                else
                {
                    if (actualTerrainCount < requiredTerrainCount)
                    {
                        EditorUtility.DisplayDialog(
                            m_editorUtils.GetTextValue("OOPS!"),
                            string.Format(m_editorUtils.GetTextValue("You currently have {0} active terrains in your scene, but to " +
                            "use {2} you need at least {1}. Please create a terrain!"), actualTerrainCount, requiredTerrainCount, feature),
                            m_editorUtils.GetTextValue("OK"));
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the range from the terrain
        /// </summary>
        /// <returns></returns>
        private float GetRangeFromTerrain()
        {
            float range = (m_settings.m_currentDefaults.m_terrainSize / 2) * m_settings.m_tilesX;
            Terrain t = Gaia.TerrainHelper.GetActiveTerrain();
            if (t != null)
            {
                range = (Mathf.Max(t.terrainData.size.x, t.terrainData.size.z) / 2f) * m_settings.m_tilesX;
            }
            return range;
        }

        /// <summary>
        /// Create a texture spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateTextureSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentResources.CreateCoverageTextureSpawner(GetRangeFromTerrain(), Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / (float)m_settings.m_currentDefaults.m_controlTextureResolution, 0.2f, 100f));
        }

        /// <summary>
        /// Create a detail spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateDetailSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentResources.CreateCoverageDetailSpawner(GetRangeFromTerrain(), Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / (float)m_settings.m_currentDefaults.m_detailResolution, 0.2f, 100f));
        }

        /// <summary>
        /// Create a clustered detail spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateClusteredDetailSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentResources.CreateClusteredDetailSpawner(GetRangeFromTerrain(), Mathf.Clamp(m_settings.m_currentDefaults.m_terrainSize / (float)m_settings.m_currentDefaults.m_detailResolution, 0.2f, 100f));
        }

        /// <summary>
        /// Create a tree spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateClusteredTreeSpawnerFromTerrainTrees()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentResources.CreateClusteredTreeSpawner(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a tree spawner from game objecxts
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateClusteredTreeSpawnerFromGameObjects()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentGameObjectResources.CreateClusteredGameObjectSpawnerForTrees(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a tree spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateCoverageTreeSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentResources.CreateCoverageTreeSpawner(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a tree spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateCoverageTreeSpawnerFromGameObjects()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentGameObjectResources.CreateCoverageGameObjectSpawnerForTrees(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a game object spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateCoverageGameObjectSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentGameObjectResources.CreateCoverageGameObjectSpawner(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a game object spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateClusteredGameObjectSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return null;
            }

            return m_settings.m_currentGameObjectResources.CreateClusteredGameObjectSpawner(GetRangeFromTerrain());
        }
        #endregion

        #region Create Step 3 (Player, water, sky etc)
        /// <summary>
        /// Create a player
        /// </summary>
        GameObject CreatePlayer()
        {
            //Gaia Settings to check pipeline selected
            if (m_settings == null)
            {
                Debug.LogWarning("Gaia Settings are missing from your project, please make sure Gaia settings is in your project.");
                return null;
            }

            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return null;
            }

            GameObject playerObj = null;

            //If nothing selected then make the default the fly cam
            string playerPrefabName = m_settings.m_currentPlayerPrefabName;
            if (string.IsNullOrEmpty(playerPrefabName))
            {
                playerPrefabName = "FlyCam";
            }

            GameObject mainCam = GameObject.Find("Main Camera");
            if (mainCam == null)
            {
                mainCam = GameObject.Find("Camera");
            }
            GameObject firstPersonController = GameObject.Find("FPSController");
            GameObject thirdPersonController = GameObject.Find("ThirdPersonController");
            GameObject flyCamController = GameObject.Find("FlyCam");
            GameObject flyCamControllerUI = GameObject.Find("FlyCamera UI");

            if (mainCam != null)
            {
                DestroyImmediate(mainCam);
            }
            if (firstPersonController != null)
            {
                DestroyImmediate(firstPersonController);
            }
            if (thirdPersonController != null)
            {
                DestroyImmediate(thirdPersonController);
            }
            if (flyCamController != null)
            {
                DestroyImmediate(flyCamController);
            }
            if (flyCamControllerUI != null)
            {
                DestroyImmediate(flyCamControllerUI);
            }

            //Get the centre of world at game height plus a bit
            Vector3 location = Gaia.TerrainHelper.GetActiveTerrainCenter(true);

            //Get the suggested camera far distance based on terrain scale
            Terrain terrain = GetActiveTerrain();
            float cameraDistance = Mathf.Clamp(terrain.terrainData.size.x, 250f, 2048) + 200f;

            GaiaSceneInfo sceneinfo = GaiaSceneInfo.GetSceneInfo();

            GameObject parentObject = GetOrCreateEnvironmentParent();

            //Create the player
            if (playerPrefabName == "FlyCam")
            {
                playerObj = new GameObject();
                playerObj.name = "FlyCam";
                playerObj.tag = "MainCamera";
                playerObj.AddComponent<FlareLayer>();
#if !UNITY_2017_1_OR_NEWER
                playerObj.AddComponent<GUILayer>();
#endif
                playerObj.AddComponent<AudioListener>();
                playerObj.AddComponent<FreeCamera>();

                Camera cameraComponent = playerObj.GetComponent<Camera>();
                cameraComponent.farClipPlane = cameraDistance;
                if (m_settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.Lightweight)
                {
                    cameraComponent.allowHDR = false;
                    cameraComponent.allowMSAA = true;
                }
                else
                {
                    cameraComponent.allowHDR = true;

                    var tier1 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier1);
                    var tier2 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier2);
                    var tier3 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier3);
                    if (tier1.renderingPath == RenderingPath.DeferredShading || tier2.renderingPath == RenderingPath.DeferredShading || tier3.renderingPath == RenderingPath.DeferredShading)
                    {
                        cameraComponent.allowMSAA = false;
                    }
                    else
                    {
                        cameraComponent.allowMSAA = true;
                    }
                }

                //Lift it to about eye height above terrain
                location.y += 1.8f;
                if (sceneinfo != null)
                {
                    if (location.y < sceneinfo.m_seaLevel)
                    {
                        location.y = sceneinfo.m_seaLevel + 1.8f;
                    }
                }
                playerObj.transform.position = location;

                //Set up UI
                if (m_settings.m_flyCamUI == null)
                {
                    Debug.LogError("[CreatePlayer()] Fly cam UI has not been assigned in the gaia settings. Assign it then try again");
                }
                else
                {
                    flyCamControllerUI = PrefabUtility.InstantiatePrefab(m_settings.m_flyCamUI) as GameObject;
                    flyCamControllerUI.name = "FlyCamera UI";
                    if (parentObject != null)
                    {
                        flyCamControllerUI.transform.SetParent(parentObject.transform);
                    }
                    else
                    {
                        flyCamControllerUI.transform.SetParent(playerObj.transform);
                    }
                }
            }
            else if (playerPrefabName == "FPSController")
            {
                GameObject playerPrefab = PWCommon2.AssetUtils.GetAssetPrefab(playerPrefabName);
                if (playerPrefab != null)
                {
                    location.y += 1f;
                    playerObj = Instantiate(playerPrefab, location, Quaternion.identity) as GameObject;
                    playerObj.name = "FPSController";
                    playerObj.tag = "Player";
                    playerObj.transform.position = location;
                    if (playerObj.GetComponent<AudioSource>() != null)
                    {
                        AudioSource theAudioSource = playerObj.GetComponent<AudioSource>();
                        theAudioSource.volume = 0.125f;
                    }
                    Camera cameraComponent = playerObj.GetComponentInChildren<Camera>();
                    if (cameraComponent != null)
                    {
                        cameraComponent.farClipPlane = cameraDistance;
                        if (m_settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.Lightweight)
                        {
                            cameraComponent.allowHDR = false;
                            cameraComponent.allowMSAA = true;
                        }
                        else
                        {
                            cameraComponent.allowHDR = true;

                            var tier1 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier1);
                            var tier2 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier2);
                            var tier3 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier3);
                            if (tier1.renderingPath == RenderingPath.DeferredShading || tier2.renderingPath == RenderingPath.DeferredShading || tier3.renderingPath == RenderingPath.DeferredShading)
                            {
                                cameraComponent.allowMSAA = false;
                            }
                            else
                            {
                                cameraComponent.allowMSAA = true;
                            }
                        }
                    }
                }
            }
            else if (playerPrefabName == "ThirdPersonController")
            {
                GameObject playerPrefab = PWCommon2.AssetUtils.GetAssetPrefab(playerPrefabName);
                if (playerPrefab != null)
                {
                    location.y += 0.05f;
                    playerObj = Instantiate(playerPrefab, location, Quaternion.identity) as GameObject;
                    playerObj.name = "ThirdPersonController";
                    playerObj.tag = "Player";
                    playerObj.transform.position = location;
                }

                mainCam = new GameObject("Main Camera");
                location.y += 1.5f;
                location.z -= 5f;
                mainCam.transform.position = location;
                Camera cameraComponent = mainCam.AddComponent<Camera>();
                cameraComponent.farClipPlane = cameraDistance;
                if (m_settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.Lightweight)
                {
                    cameraComponent.allowHDR = false;
                    cameraComponent.allowMSAA = true;
                }
                else
                {
                    cameraComponent.allowHDR = true;

                    var tier1 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier1);
                    var tier2 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier2);
                    var tier3 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier3);
                    if (tier1.renderingPath == RenderingPath.DeferredShading || tier2.renderingPath == RenderingPath.DeferredShading || tier3.renderingPath == RenderingPath.DeferredShading)
                    {
                        cameraComponent.allowMSAA = false;
                    }
                    else
                    {
                        cameraComponent.allowMSAA = true;
                    }
                }

#if !UNITY_2017_1_OR_NEWER
                mainCam.AddComponent<GUILayer>();
#endif
                mainCam.AddComponent<FlareLayer>();
                mainCam.AddComponent<AudioListener>();
                mainCam.tag = "MainCamera";

                CameraController cameraController = mainCam.AddComponent<CameraController>();
                cameraController.target = playerObj;
                cameraController.targetHeight = 1.8f;
                cameraController.distance = 5f;
                cameraController.maxDistance = 20f;
                cameraController.minDistance = 2.5f;
            }

            if (playerObj != null)
            {
                //Adjust the scene view to see the camera
                if (SceneView.lastActiveSceneView != null)
                {
                    SceneView.lastActiveSceneView.LookAtDirect(playerObj.transform.position, playerObj.transform.rotation);
                    Repaint();
                }
            }


            return playerObj;
        }

        /// <summary>
        /// Create a scene exporter object
        /// </summary>
        /*
        GameObject ShowSceneExporter()
        {
            GameObject exporterObj = GameObject.Find("Exporter");
            if (exporterObj == null)
            {
                exporterObj = new GameObject("Exporter");
                exporterObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter(false);
                GaiaExporter exporter = exporterObj.AddComponent<GaiaExporter>();
                GameObject gaiaObj = GameObject.Find("Gaia");
                if (gaiaObj != null)
                {
                    exporterObj.transform.parent = gaiaObj.transform;
                    exporter.m_rootObject = gaiaObj;
                }
                exporter.m_defaults = m_defaults;
                exporter.m_resources = m_resources;
                exporter.IngestGaiaSetup();
            }
            return exporterObj;
                     */

        /// <summary>
        /// Create a wind zone
        /// </summary>
        private GameObject CreateWindZone()
        {
            WindZone globalWind = FindObjectOfType<WindZone>();
            if (globalWind == null)
            {
                GameObject windZoneObj = new GameObject("Wind Zone");
                windZoneObj.transform.Rotate(new Vector3(25f, 0f, 0f));
                globalWind = windZoneObj.AddComponent<WindZone>();
                switch (m_settings.m_windType)
                {
                    case GaiaConstants.GaiaGlobalWindType.Calm:
                        globalWind.windMain = 0.2f;
                        globalWind.windTurbulence = 0.2f;
                        globalWind.windPulseMagnitude = 0.2f;
                        globalWind.windPulseFrequency = 0.05f;
                        break;
                    case GaiaConstants.GaiaGlobalWindType.Moderate:
                        globalWind.windMain = 0.45f;
                        globalWind.windTurbulence = 0.35f;
                        globalWind.windPulseMagnitude = 0.2f;
                        globalWind.windPulseFrequency = 0.1f;
                        break;
                    case GaiaConstants.GaiaGlobalWindType.Strong:
                        globalWind.windMain = 0.65f;
                        globalWind.windTurbulence = 0.3f;
                        globalWind.windPulseMagnitude = 0.2f;
                        globalWind.windPulseFrequency = 0.25f;
                        break;
                    case GaiaConstants.GaiaGlobalWindType.None:
                        globalWind.windMain = 0f;
                        globalWind.windTurbulence = 0f;
                        globalWind.windPulseMagnitude = 0f;
                        globalWind.windPulseFrequency = 0f;
                        break;
                }

                GameObject gaiaObj = GetOrCreateEnvironmentParent();
                windZoneObj.transform.SetParent(gaiaObj.transform);
            }
            else
            {
                switch (m_settings.m_windType)
                {
                    case GaiaConstants.GaiaGlobalWindType.Calm:
                        globalWind.windMain = 0.2f;
                        globalWind.windTurbulence = 0.2f;
                        globalWind.windPulseMagnitude = 0.2f;
                        globalWind.windPulseFrequency = 0.05f;
                        break;
                    case GaiaConstants.GaiaGlobalWindType.Moderate:
                        globalWind.windMain = 0.45f;
                        globalWind.windTurbulence = 0.35f;
                        globalWind.windPulseMagnitude = 0.2f;
                        globalWind.windPulseFrequency = 0.1f;
                        break;
                    case GaiaConstants.GaiaGlobalWindType.Strong:
                        globalWind.windMain = 0.65f;
                        globalWind.windTurbulence = 0.3f;
                        globalWind.windPulseMagnitude = 0.2f;
                        globalWind.windPulseFrequency = 0.25f;
                        break;
                    case GaiaConstants.GaiaGlobalWindType.None:
                        globalWind.windMain = 0f;
                        globalWind.windTurbulence = 0f;
                        globalWind.windPulseMagnitude = 0f;
                        globalWind.windPulseFrequency = 0f;
                        break;
                }
            }

            GaiaLighting.AddGlobalWindShader(m_settings.m_windType);

            GameObject returingObject = globalWind.gameObject;

            return returingObject;
        }

        /// <summary>
        /// Create water
        /// </summary>
        GameObject CreateWater()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfNotMinimumTerrainCount(1))
            {
                return null;
            }
#if GAIA_PRESENT && !AMBIENT_WATER
            if (m_settings.m_currentWaterPro == GaiaConstants.GaiaWaterProfileType.None)
            {
                GaiaWater.RemoveSystems();
            }
            else
            {
                GaiaWater.GetProfile(m_settings.m_currentWaterPro, m_settings.m_gaiaWaterProfile, m_settings.m_pipelineProfile.m_activePipelineInstalled, true);
            }
#endif
            return GameObject.Find("Ambient Water Sample");
        }

        /// <summary>
        /// Create the sky
        /// </summary>
        void CreateSky()
        {
#if GAIA_PRESENT
            GaiaLighting.GetProfile(m_settings.m_currentSkies, m_settings.m_gaiaLightingProfile, m_settings.m_pipelineProfile, m_settings.m_pipelineProfile.m_activePipelineInstalled);
#else
            Debug.Log("Lighting could not be created because Ambient Skies exists in this project! Please use Ambinet Skies to set up lighting in your scene!");
#endif
        }

        /// <summary>
        /// Create and return a screen shotter object
        /// </summary>
        /// <returns></returns>
        GameObject CreateScreenShotter()
        {
            GameObject shotterObj = GameObject.Find("Screen Shotter");
            if (shotterObj == null)
            {
                shotterObj = new GameObject("Screen Shotter");
                Gaia.ScreenShotter shotter = shotterObj.AddComponent<Gaia.ScreenShotter>();
                shotter.m_watermark = PWCommon2.AssetUtils.GetAsset("Made With Gaia Watermark.png", typeof(Texture2D)) as Texture2D;

                GameObject gaiaObj = GameObject.Find("Gaia Environment");
                if (gaiaObj == null)
                {
                    gaiaObj = new GameObject("Gaia Environment");
                }
                shotterObj.transform.parent = gaiaObj.transform;
                shotterObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter(false);
            }

            return shotterObj;
        }

        /// <summary>
        /// Gets and returns the parent gameobject
        /// </summary>
        /// <returns>Parent Object</returns>
        private static GameObject GetOrCreateEnvironmentParent()
        {
            GameObject parent = GameObject.Find("Gaia Environment");
            if (parent == null)
            {
                parent = new GameObject("Gaia Environment");
            }
            return parent;
        }
        #endregion

        #region Setup Tab Functions

        /// <summary>
        /// Setup panel settings
        /// </summary>
        /// <param name="helpEnabled"></param>
        private void SetupSettingsEnabled(bool helpEnabled)
        {
            if (!m_shadersNotImported)
            {
                EditorGUILayout.HelpBox("Shader library folder is missing. Please reimport Gaia and insure that 'PW Shader Library' is improted.", MessageType.Error);
            }
            else
            {
                bool enablePackageButton = false;

#if !UNITY_POST_PROCESSING_STACK_V2
                if (m_settings.m_currentRenderer != GaiaConstants.EnvironmentRenderer.HighDefinition)
                {
                    m_editorUtils.InlineHelp("PostProcessingSetupHelp", helpEnabled);
                    m_editorUtils.Heading("PostProcessingSettings");
                    EditorGUILayout.HelpBox("Install Post Processing from the package manager. Use Help to learn how to install Post Processing From Package Manager", MessageType.Error);
                    GUILayout.Space(5f);
                }
#endif

                if (m_editorUtils.ClickableText("InstallPipelineHelp"))
                {
                    Application.OpenURL("https://docs.unity3d.com/Manual/ScriptableRenderPipeline.html");
                }

                m_editorUtils.InlineHelp("InstallPipelineHelp", helpEnabled);

                EditorGUILayout.Space();

                m_editorUtils.Heading("RenderPipelineSettings");
                if (m_settings.m_pipelineProfile.m_activePipelineInstalled == GaiaConstants.EnvironmentRenderer.BuiltIn)
                {
                    EditorGUILayout.BeginHorizontal();
                    m_editorUtils.LabelField("RenderPipeline");
                    m_settings.m_currentRenderer = (GaiaConstants.EnvironmentRenderer)EditorGUILayout.EnumPopup(m_settings.m_currentRenderer);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    m_editorUtils.LabelField("RenderPipeline");
                    EditorGUILayout.LabelField("Installed " + m_settings.m_pipelineProfile.m_activePipelineInstalled.ToString());
                    EditorGUILayout.EndHorizontal();
                }

                m_editorUtils.InlineHelp("RenderPipelineSetupHelp", helpEnabled);

                //Revert back to built-in renderer
                if (m_settings.m_pipelineProfile.m_activePipelineInstalled != m_settings.m_currentRenderer)
                {
                    if (m_settings.m_pipelineProfile.m_activePipelineInstalled != GaiaConstants.EnvironmentRenderer.BuiltIn && m_settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.BuiltIn)
                    {
                        if (m_editorUtils.Button("RevertGaiaBackToBuiltIn"))
                        {
                            if (EditorUtility.DisplayDialog("Upgrading Gaia Pipeline!", "You're about to revert gaia back to use Built-In render pipeline. Are you sure you want to proceed?", "Yes", "No"))
                            {
                                m_enableGUI = true;
                                if (m_settings.m_pipelineProfile.m_activePipelineInstalled == GaiaConstants.EnvironmentRenderer.Lightweight)
                                {
                                    m_settings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
                                    GaiaLWRPPipelineUtils.CleanUpLWRP(m_settings.m_pipelineProfile, m_settings);
                                }
                                else if (m_settings.m_pipelineProfile.m_activePipelineInstalled == GaiaConstants.EnvironmentRenderer.HighDefinition)
                                {
                                    m_settings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
                                    GaiaHDRPPipelineUtils.CleanUpHDRP(m_settings.m_pipelineProfile, m_settings);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Disable Install button
                        enablePackageButton = true;
                        //LWRP Version Limit
                        if (m_settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.Lightweight)
                        {
                            if (Application.unityVersion.Contains("2019.1"))
                            {
                                EditorGUILayout.BeginHorizontal();
                                m_editorUtils.LabelField("MinLWRP");
                                EditorGUILayout.LabelField(m_settings.m_pipelineProfile.m_min2019_1LWVersion);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                m_editorUtils.LabelField("MaxLWRP");
                                EditorGUILayout.LabelField(m_settings.m_pipelineProfile.m_max2019_1LWVersion);
                                EditorGUILayout.EndHorizontal();
                            }
                            else if (Application.unityVersion.Contains("2019.2"))
                            {
                                EditorGUILayout.BeginHorizontal();
                                m_editorUtils.LabelField("MinLWRP");
                                EditorGUILayout.LabelField(m_settings.m_pipelineProfile.m_min2019_2LWVersion);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                m_editorUtils.LabelField("MaxLWRP");
                                EditorGUILayout.LabelField(m_settings.m_pipelineProfile.m_max2019_2LWVersion);
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        //HDRP Version Limit
                        else if (m_settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition)
                        {
                            if (Application.unityVersion.Contains("2019.1"))
                            {
                                EditorGUILayout.BeginHorizontal();
                                m_editorUtils.LabelField("MinHDRP");
                                EditorGUILayout.LabelField(m_settings.m_pipelineProfile.m_min2019_1HDVersion);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                m_editorUtils.LabelField("MaxHDRP");
                                EditorGUILayout.LabelField(m_settings.m_pipelineProfile.m_max2019_1HDVersion);
                                EditorGUILayout.EndHorizontal();
                            }
                            else if (Application.unityVersion.Contains("2019.2"))
                            {
                                EditorGUILayout.BeginHorizontal();
                                m_editorUtils.LabelField("MinHDRP");
                                EditorGUILayout.LabelField(m_settings.m_pipelineProfile.m_min2019_2HDVersion);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                m_editorUtils.LabelField("MaxHDRP");
                                EditorGUILayout.LabelField(m_settings.m_pipelineProfile.m_max2019_2HDVersion);
                                EditorGUILayout.EndHorizontal();
                            }
                        }

                        //Upgrade to LWRP/HDRP
                        if (m_editorUtils.Button("UpgradeGaiaTo" + m_settings.m_currentRenderer.ToString()))
                        {
                            if (EditorUtility.DisplayDialog("Upgrading Gaia Pipeline", "You're about to change Gaia to use " + m_settings.m_currentRenderer.ToString() + " render pipeline. Are you sure you want to proceed?", "Yes", "No"))
                            {
                                m_enableGUI = true;
                                if (EditorUtility.DisplayDialog("Save Scene", "Would you like to save your scene before switching render pipeline?", "Yes", "No"))
                                {
                                    if (EditorSceneManager.GetActiveScene().isDirty)
                                    {
                                        EditorSceneManager.SaveOpenScenes();
                                    }

                                    AssetDatabase.SaveAssets();
                                }


                                if (m_settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.Lightweight)
                                {
                                    EditorUtility.DisplayProgressBar("Installing " + m_settings.m_currentRenderer.ToString(), "Preparing To Install " + m_settings.m_currentRenderer.ToString(), 0f);
                                    GaiaLWRPPipelineUtils.m_waitTimer1 = 1f;
                                    GaiaLWRPPipelineUtils.m_waitTimer2 = 3f;
                                    GaiaLWRPPipelineUtils.SetPipelineAsset(m_settings.m_pipelineProfile);
                                }
                                else if (m_settings.m_currentRenderer == GaiaConstants.EnvironmentRenderer.HighDefinition)
                                {
                                    EditorUtility.DisplayProgressBar("Installing " + m_settings.m_currentRenderer.ToString(), "Preparing To Install " + m_settings.m_currentRenderer.ToString(), 0f);
                                    GaiaHDRPPipelineUtils.m_waitTimer1 = 1f;
                                    GaiaHDRPPipelineUtils.m_waitTimer2 = 3f;
                                    GaiaHDRPPipelineUtils.SetPipelineAsset(m_settings.m_pipelineProfile);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Revert back to built-in renderer
                    if (m_settings.m_pipelineProfile.m_activePipelineInstalled != GaiaConstants.EnvironmentRenderer.BuiltIn)
                    {
                        if (m_editorUtils.Button("RevertGaiaBackToBuiltIn"))
                        {
                            if (EditorUtility.DisplayDialog("Upgrading Gaia Pipeline", "You're about to revert Gaia back to use Built-In render pipeline. Are you sure you want to proceed?", "Yes", "No"))
                            {
                                m_enableGUI = true;
                                if (EditorUtility.DisplayDialog("Save Scene", "Would you like to save your scene before switching render pipeline?", "Yes", "No"))
                                {
                                    if (EditorSceneManager.GetActiveScene().isDirty)
                                    {
                                        EditorSceneManager.SaveOpenScenes();
                                    }

                                    AssetDatabase.SaveAssets();
                                }


                                if (m_settings.m_pipelineProfile.m_activePipelineInstalled == GaiaConstants.EnvironmentRenderer.Lightweight)
                                {
                                    m_settings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
                                    GaiaLWRPPipelineUtils.CleanUpLWRP(m_settings.m_pipelineProfile, m_settings);
                                }
                                else if (m_settings.m_pipelineProfile.m_activePipelineInstalled == GaiaConstants.EnvironmentRenderer.HighDefinition)
                                {
                                    m_settings.m_currentRenderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
                                    GaiaHDRPPipelineUtils.CleanUpHDRP(m_settings.m_pipelineProfile, m_settings);
                                }
                            }
                        }
                    }
                }

                if (!m_enableGUI)
                {
                    Material[] vegetationMaterialLibrary = null;
                    Material[] waterMaterialLibrary = null;
                    GaiaPackageVersion unityVersion = GaiaPackageVersion.Unity2019_1;

                    //Installation setup
                    if (Application.unityVersion.Contains("2019.2"))
                    {
                        unityVersion = GaiaPackageVersion.Unity2019_2;
                    }
                    else if (Application.unityVersion.Contains("2019.3"))
                    {
                        unityVersion = GaiaPackageVersion.Unity2019_3;
                    }

                    GetPackages(unityVersion, out vegetationMaterialLibrary, out waterMaterialLibrary, out enablePackageButton);

                    GUILayout.Space(5f);

                    m_editorUtils.Heading("PackagesThatWillBeInstalled");

                    if (!enablePackageButton)
                    {
                        EditorGUILayout.HelpBox("Shader Installtion is not yet supported on this version of Unity.", MessageType.Info);
                        GUI.enabled = false;
                    }

                    GUI.backgroundColor = Color.red;

                    if (EditorApplication.isCompiling)
                    {
                        GUI.enabled = false;
                    }

                    if (m_editorUtils.Button("InstallPackages"))
                    {
                        ProcedualWorlds.Gaia.PackageSystem.PackageInstallerUtils.m_installShaders = true;
                        ProcedualWorlds.Gaia.PackageSystem.PackageInstallerUtils.m_timer = 7f;
                        ProcedualWorlds.Gaia.PackageSystem.PackageInstallerUtils.StartInstallation(Application.unityVersion, m_settings.m_currentRenderer, vegetationMaterialLibrary, waterMaterialLibrary, m_settings.m_pipelineProfile);
                    }

                    m_editorUtils.InlineHelp("PackageInstallSetupHelp", helpEnabled);

                    GUI.enabled = true;
                }

                GUI.backgroundColor = m_defaultPanelColor;
            }
        }

        /// <summary>
        /// System info settings
        /// </summary>
        /// <param name="helpEnabled"></param>
        private void SystemInfoSettingsEnabled(bool helpEnabled)
        {
            m_editorUtils.Heading("UnityInfo");
            EditorGUILayout.LabelField("Unity Version: " + Application.unityVersion);
            EditorGUILayout.LabelField("Company Name: " + Application.companyName);
            EditorGUILayout.LabelField("Product Name: " + Application.productName);
            EditorGUILayout.LabelField("Project Version: " + Application.version);
            EditorGUILayout.LabelField("Project Data Path: " + Application.dataPath);

            EditorGUILayout.Space();
            m_editorUtils.Heading("SystemInfo");
            EditorGUILayout.LabelField("Graphics Card Name: " + SystemInfo.graphicsDeviceName);
            EditorGUILayout.LabelField("Graphics Card Version: " + SystemInfo.graphicsDeviceVersion);
            EditorGUILayout.LabelField("Graphics Card Memory: " + SystemInfo.graphicsMemorySize + " MB");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Processor Name: " + SystemInfo.processorType);
            EditorGUILayout.LabelField("Processor Core Count: " + SystemInfo.processorCount);
            EditorGUILayout.LabelField("Processor Speed: " + SystemInfo.processorFrequency + " GHz");
        }

        /// <summary>
        /// Check if shaders need to be installed
        /// </summary>
        /// <returns></returns>
        private bool MissingShaders()
        {
            bool exist = false;
            m_enableGUI = false;

            string[] folders = Directory.GetDirectories(Application.dataPath, ".", SearchOption.AllDirectories);
            foreach (string folder in folders)
            {
                if (folder.Contains("PW Shader Library"))
                {
                    m_enableGUI = true;
                    exist = true;
                }
            }

            return exist;
        }

        /// <summary>
        /// Check the project for files and check if needs to be installed
        /// </summary>
        /// <returns></returns>
        private bool DoesPackagesNeedInstalled(GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            bool doesNeedInstalling = true;

            string shaderRootFolder = GaiaDirectories.GetShaderDirectory();
            if (shaderRootFolder == null)
            {
                return doesNeedInstalling;
            }
            string[] folders = Directory.GetDirectories(shaderRootFolder, ".", SearchOption.AllDirectories);

            string unityVersion = Application.unityVersion;
            unityVersion = unityVersion.Remove(unityVersion.LastIndexOf(".")).Replace(".", "_0");

            DirectoryInfo dirInfo = new DirectoryInfo(shaderRootFolder);
            var files = dirInfo.GetFiles();

            string keyWordToSearch = "";
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                keyWordToSearch = m_builtInKeyWord;
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                keyWordToSearch = m_lightweightKeyWord;
            }
            else
            {
                keyWordToSearch = m_highDefinitionKeyWord;
            }

            foreach (string folderName in folders)
            {
                if (!folderName.Contains("PWS Functions"))
                {
                    dirInfo = new DirectoryInfo(folderName);
                    files = dirInfo.GetFiles();

                    foreach (FileInfo file in files)
                    {
                        if (folderName.Contains(keyWordToSearch + " " + unityVersion))
                        {
                            if (file.Extension.EndsWith("shaderfile"))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            if (m_settings != null)
            {
                Material[] vegetationMaterials = m_settings.m_pipelineProfile.m_vegetationMaterialLibrary;
                foreach (Material material in vegetationMaterials)
                {
                    if (material != null)
                    {
                        if (material.shader.name.StartsWith("Hidden/InternalErrorShader"))
                        {
                            return false;

                        }
                    }
                }

                string[] waterFolders = Directory.GetDirectories(Application.dataPath + m_materialLocation, ".", SearchOption.AllDirectories);
                string mainFolder = "";
                foreach (string folderName in waterFolders)
                {
                    if (folderName.Contains(keyWordToSearch + " " + unityVersion))
                    {
                        mainFolder = folderName;
                        break;
                    }
                }

                m_profileList.Clear();
                List<Material> allMaterials = GetMaterials(mainFolder);
                if (allMaterials != null)
                {
                    foreach (Material mat in allMaterials)
                    {
                        if (mat.shader.name.StartsWith("Hidden/InternalErrorShader") && m_settings.m_pipelineProfile.m_waterMaterialLibrary.Contains(mat))
                        {
                            return false;
                        }
                    }
                }
            }

            return doesNeedInstalling;
        }

        /// <summary>
        /// Gets the unity packages shaders/setup
        /// </summary>
        /// <param name="unityVersion"></param>
        /// <param name="shaderLibrary"></param>
        private void GetPackages(GaiaPackageVersion unityVersion, out Material[] vegetationMaterialLibrary, out Material[] waterMaterialLibrary, out bool isSupported)
        {
            vegetationMaterialLibrary = null;
            waterMaterialLibrary = null;
            isSupported = false;

            if (m_settings == null)
            {
                m_settings = GaiaUtils.GetGaiaSettings();
            }

            if (m_gaiaPipelineSettings == null)
            {
                m_gaiaPipelineSettings = m_settings.m_pipelineProfile;
            }

            if (m_gaiaPipelineSettings == null)
            {
                Debug.LogError("Gaia Pipeline Profile is empty, check the Gaia Settings to insure the profile is defined.");
                return;
            }

            foreach (GaiaPackageSettings packageSettings in m_gaiaPipelineSettings.m_packageSetupSettings)
            {
                if (packageSettings.m_unityVersion == unityVersion)
                {
                    vegetationMaterialLibrary = m_gaiaPipelineSettings.m_vegetationMaterialLibrary;
                    waterMaterialLibrary = m_gaiaPipelineSettings.m_waterMaterialLibrary;

                    if (m_settings.m_pipelineProfile.m_activePipelineInstalled == GaiaConstants.EnvironmentRenderer.BuiltIn)
                    {
                        isSupported = packageSettings.m_isSupported;
                    }
                    else if (m_settings.m_pipelineProfile.m_activePipelineInstalled == GaiaConstants.EnvironmentRenderer.Lightweight)
                    {
                        isSupported = packageSettings.m_isSupported;
                    }
                    else
                    {
                        isSupported = packageSettings.m_isSupported;
                    }
                }
            }

        }

        /// <summary>
        /// Checks the gaia manager and updates the bool checks
        /// </summary>
        public void GaiaManagerStatusCheck()
        {
            //Check if shaders are missing
            m_shadersNotImported = MissingShaders();

            m_enableGUI = DoesPackagesNeedInstalled(m_settings.m_pipelineProfile.m_activePipelineInstalled);

            if (m_enableGUI)
            {
                m_showSetupPanel = false;
            }
            else
            {
                m_showSetupPanel = true;
            }

#if !UNITY_2019_1_OR_NEWER

            m_enableGUI = false;

#endif

            this.Repaint();
        }

        /// <summary>
        /// Setup the water list in the Extra settings
        /// </summary>
        /// <param name="gaiaSettings"></param>
        private void SetupWaterList(GaiaSettings gaiaSettings)
        {
            if (gaiaSettings != null)
            {
                newProfileListIndex = gaiaSettings.m_selectedWaterProfile;
                if (newProfileListIndex < 0)
                {
                    newProfileListIndex = 1;
                }

                SetupMaterials(gaiaSettings.m_currentRenderer, gaiaSettings, newProfileListIndex);

                gaiaSettings.m_gaiaWaterProfile.m_activeWaterMaterial = m_allMaterials[newProfileListIndex];
            }
        }

        /// <summary>
        /// Setup the material name list
        /// </summary>
        private bool SetupMaterials(GaiaConstants.EnvironmentRenderer renderPipeline, GaiaSettings gaiaSettings, int profileIndex)
        {
            bool successful = false;

            string[] folders = Directory.GetDirectories(Application.dataPath + m_materialLocation, ".", SearchOption.AllDirectories);
            m_unityVersion = Application.unityVersion;
            m_unityVersion = m_unityVersion.Remove(m_unityVersion.LastIndexOf(".")).Replace(".", "_0");
            string keyWordToSearch = "";

            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                keyWordToSearch = m_builtInKeyWord;
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                keyWordToSearch = m_lightweightKeyWord;
            }
            else
            {
                keyWordToSearch = m_highDefinitionKeyWord;
            }

            string mainFolder = "";
            foreach (string folderName in folders)
            {
                if (folderName.Contains(keyWordToSearch + " " + m_unityVersion))
                {
                    mainFolder = folderName;
                    break;
                }
            }

            m_profileList.Clear();
            List<Material> allMaterials = GetMaterials(mainFolder);
            if (allMaterials != null)
            {
                foreach (Material mat in allMaterials)
                {
                    m_profileList.Add(mat.name);
                }
            }

            if (allMaterials.Count > 0)
            {
                successful = true;
            }

            gaiaSettings.m_gaiaWaterProfile.m_activeWaterMaterial = m_allMaterials[profileIndex];
            return successful;
        }

        /// <summary>
        /// Removes Suffix in file formats required
        /// </summary>
        /// <param name="path"></param>
        private List<Material> GetMaterials(string path)
        {
            List<Material> materials = new List<Material>();

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var files = dirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension.EndsWith("mat"))
                {
                    materials.Add(AssetDatabase.LoadAssetAtPath<Material>(GaiaUtils.GetAssetPath(file.Name)));
                }
            }

            m_allMaterials = materials;

            return materials;
        }

        #endregion

        #region System Helpers
        /// <summary>
        /// Get a clamped size value
        /// </summary>
        /// <param name="newSize"></param>
        /// <returns></returns>
        float GetClampedSize(float newSize)
        {
            return Mathf.Clamp(newSize, 32f, m_settings.m_currentDefaults.m_size);
        }

        #region Helper methods

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns></returns>
        private static string GetAssetPath(string name)
        {
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets(name, null);
            if (assets.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(assets[0]);
            }
#endif
            return null;
        }

        /// <summary>
        /// Get the currently active terrain - or any terrain
        /// </summary>
        /// <returns>A terrain if there is one</returns>
        public static Terrain GetActiveTerrain()
        {
            //Grab active terrain if we can
            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null && terrain.isActiveAndEnabled)
            {
                return terrain;
            }

            //Then check rest of terrains
            for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
            {
                terrain = Terrain.activeTerrains[idx];
                if (terrain != null && terrain.isActiveAndEnabled)
                {
                    return terrain;
                }
            }
            return null;
        }

        #endregion

        /// <summary>
        /// Get the latest news from the web site at most once every 24 hours
        /// </summary>
        /// <returns></returns>
        IEnumerator GetNewsUpdate()
        {
            TimeSpan elapsed = new TimeSpan(DateTime.Now.Ticks - m_settings.m_lastWebUpdate);
            if (elapsed.TotalHours < 24.0)
            {
                StopEditorUpdates();
            }
            else
            {
                if (PWApp.CONF != null)
                {
#if UNITY_2018_3_OR_NEWER
                    using (UnityWebRequest www = new UnityWebRequest("http://www.procedural-worlds.com/gaiajson.php?gv=gaia-" + PWApp.CONF.Version))
                    {
                        while (!www.isDone)
                        {
                            yield return www;
                        }

                        if (!string.IsNullOrEmpty(www.error))
                        {
                            //Debug.Log(www.error);
                        }
                        else
                        {
                            try
                            {
                                string result = www.url;
                                int first = result.IndexOf("####");
                                if (first > 0)
                                {
                                    result = result.Substring(first + 10);
                                    first = result.IndexOf("####");
                                    if (first > 0)
                                    {
                                        result = result.Substring(0, first);
                                        result = result.Replace("<br />", "");
                                        result = result.Replace("&#8221;", "\"");
                                        result = result.Replace("&#8220;", "\"");
                                        var message = JsonUtility.FromJson<GaiaMessages>(result);
                                        m_settings.m_latestNewsTitle = message.title;
                                        m_settings.m_latestNewsBody = message.bodyContent;
                                        m_settings.m_latestNewsUrl = message.url;
                                        m_settings.m_lastWebUpdate = DateTime.Now.Ticks;
                                        EditorUtility.SetDirty(m_settings);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                //Debug.Log(e.Message);
                            }
                        }
                    }
#else
                    using (WWW www = new WWW("http://www.procedural-worlds.com/gaiajson.php?gv=gaia-" + PWApp.CONF.Version))
                    {
                        while (!www.isDone)
                        {
                            yield return www;
                        }

                        if (!string.IsNullOrEmpty(www.error))
                        {
                            //Debug.Log(www.error);
                        }
                        else
                        {
                            try
                            {
                                string result = www.text;
                                int first = result.IndexOf("####");
                                if (first > 0)
                                {
                                    result = result.Substring(first + 10);
                                    first = result.IndexOf("####");
                                    if (first > 0)
                                    {
                                        result = result.Substring(0, first);
                                        result = result.Replace("<br />", "");
                                        result = result.Replace("&#8221;", "\"");
                                        result = result.Replace("&#8220;", "\"");
                                        var message = JsonUtility.FromJson<GaiaMessages>(result);
                                        m_settings.m_latestNewsTitle = message.title;
                                        m_settings.m_latestNewsBody = message.bodyContent;
                                        m_settings.m_latestNewsUrl = message.url;
                                        m_settings.m_lastWebUpdate = DateTime.Now.Ticks;
                                        EditorUtility.SetDirty(m_settings);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                //Debug.Log(e.Message);
                            }
                        }
                    }
                
#endif
                }
            }
            StopEditorUpdates();
        }

        /// <summary>
        /// Import Package
        /// </summary>
        /// <param name="packageName"></param>
        public static void ImportPackage(string packageName)
        {
            string packageGaia = AssetUtils.GetAssetPath(packageName + ".unitypackage");
            Debug.Log(packageGaia);
            if (!string.IsNullOrEmpty(packageGaia))
            {
                AssetDatabase.ImportPackage(packageGaia, true);
            }
            else
                Debug.Log("Unable to find Gaia Dependencies.unitypackage");
        }

        /// <summary>
        /// Start editor updates
        /// </summary>
        public void StartEditorUpdates()
        {
            EditorApplication.update += EditorUpdate;
        }

        //Stop editor updates
        public void StopEditorUpdates()
        {
            EditorApplication.update -= EditorUpdate;
        }

        /// <summary>
        /// This is executed only in the editor - using it to simulate co-routine execution and update execution
        /// </summary>
        void EditorUpdate()
        {
            if (m_updateCoroutine == null)
            {
                StopEditorUpdates();
            }
            else
            {
                m_updateCoroutine.MoveNext();
            }
        }
        #endregion

        #region GAIA eXtensions GX
        public static List<Type> GetTypesInNamespace(string nameSpace)
        {
            List<Type> gaiaTypes = new List<Type>();

            int assyIdx, typeIdx;
            System.Type[] types;
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (assyIdx = 0; assyIdx < assemblies.Length; assyIdx++)
            {
                if (assemblies[assyIdx].FullName.StartsWith("Assembly"))
                {
                    types = assemblies[assyIdx].GetTypes();
                    for (typeIdx = 0; typeIdx < types.Length; typeIdx++)
                    {
                        if (!string.IsNullOrEmpty(types[typeIdx].Namespace))
                        {
                            if (types[typeIdx].Namespace.StartsWith(nameSpace))
                            {
                                gaiaTypes.Add(types[typeIdx]);
                            }
                        }
                    }
                }
            }
            return gaiaTypes;
        }

        /// <summary>
        /// Return true if image FX have been included
        /// </summary>
        /// <returns></returns>
        public static bool GotImageFX()
        {
            List<Type> types = GetTypesInNamespace("UnityStandardAssets.ImageEffects");
            if (types.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Commented out tooltips
        ///// <summary>
        ///// The tooltips
        ///// </summary>
        //static Dictionary<string, string> m_tooltips = new Dictionary<string, string>
        //{
        //    { "Execution Mode", "The way this spawner runs. Design time : At design time only. Runtime Interval : At run time on a timed interval. Runtime Triggered Interval : At run time on a timed interval, and only when the tagged game object is closer than the trigger range from the center of the spawner." },
        //    { "Controller", "The type of control method that will be set up. " },
        //    { "Environment", "The type of environment that will be set up. This pre-configures your terrain settings to be better suited for the environment you are targeting. You can modify these setting by modifying the relevant terrain default settings." },
        //    { "Renderer", "The terrain renderer you are targeting. The 2018x renderers are only relevent when using Unity 2018 and above." },
        //    { "Terrain Size", "The size of the terrain you are setting up. Please be aware that larger terrain sizes are harder for Unity to render, and will result in slow frame rates. You also need to consider your target environment as well. A mobile or VR device will have problems with large terrains." },
        //    { "Terrain Defaults", "The default settings that will be used when creating new terrains." },
        //    { "Terrain Resources", "The texture, detail and tree resources that will be used when creating new terrains." },
        //    { "GameObject Resources", "The game object resources that will be passed to your GameObject spawners when creating new spawners." },
        //    { "1. Create Terrain & Show Stamper", "Creates your terrain based on the setting in the panel above. You use the stamper to terraform your terrain." },
        //    { "2. Create Spawners", "Creates the spawners based on your resources in the panel above. You use spawners to inject these resources into your scene." },
        //    { "3. Create Player, Water and Screenshotter", "Creates the things you most commonly need in your scene to make it playable." },
        //    { "3. Create Player, Wind, Water and Screenshotter", "Creates the things you most commonly need in your scene to make it playable." },
        //    { "Show Session Manager", "The session manager records stamping and spawning operations so that you can recreate your terrain later." },
        //    { "Create Terrain", "Creates a terrain based on your settings." },
        //    { "Create Coverage Texture Spawner", "Creates a texture spawner so you can paint your terrain." },
        //    { "Create Coverage Grass Spawner", "Creates a grass (terrain details) spawner so you can cover your terrain with grass." },
        //    { "Create Clustered Grass Spawner", "Creates a grass (terrain details) spawner so you can cover your terrain with patches with grass." },
        //    { "Create Coverage Terrain Tree Spawner", "Creates a terrain tree spawner so you can cover your terrain with trees." },
        //    { "Create Clustered Terrain Tree Spawner", "Creates a terrain tree spawner so you can cover your terrain with clusters with trees." },
        //    { "Create Coverage Prefab Tree Spawner", "Creates a tree spawner from prefabs so you can cover your terrain with trees." },
        //    { "Create Clustered Prefab Tree Spawner", "Creates a tree spawner from prefabs so you can cover your terrain with clusters with trees." },
        //    { "Create Coverage Prefab Spawner", "Creates a spawner from prefabs so you can cover your terrain with instantiations of those prefabs." },
        //    { "Create Clustered Prefab Spawner", "Creates a spawner from prefabs so you can cover your terrain with clusters of those prefabs." },
        //    { "Show Stamper", "Shows a stamper. Use the stamper to terraform your terrain." },
        //    { "Show Scanner", "Shows the scanner. Use the scanner to create new stamps from textures, world machine .r16 files, IBM 16 bit RAW file, MAC 16 bit RAW files, Terrains, and Meshes (with mesh colliders)." },
        //    { "Show Visualiser", "Shows the visualiser. Use the visualiser to visualise and configure fitness values for your resources." },
        //    { "Show Terrain Utilities", "Shows terrain utilities. These are a great way to add additional interest to your terrains." },
        //    { "Show Splatmap Exporter", "Shows splatmap exporter. Exports your texture splatmaps." },
        //    { "Show Grass Exporter", "Shows grass exporter. Exports your grass control maps." },
        //    { "Show Mesh Exporter", "Shows mesh exporter. Exports your terrain as a low poly mesh. Use in conjunction with Base Map Exporter and Normal Map Exporter in Terrain Utilties to create cool mesh features to use in the distance." },
        //    { "Show Shore Exporter", "Shows shore exporter. Exports a mask of your terrain shoreline." },
        //    { "Show Extension Exporter", "Shows extension exporter. Use extensions to save resource and spawner configurations for later use via the GX tab." },
        //};
        #endregion
    }
}