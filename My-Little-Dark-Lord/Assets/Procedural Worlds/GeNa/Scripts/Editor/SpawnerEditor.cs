using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using GeNa.Internal;
using PWCommon2;
using System.Linq;

namespace GeNa
{
    /// <summary>
    /// Editor for GeNa spawning system
    /// </summary>
    [CustomEditor(typeof(Spawner))]
    public class SpawnerEditor : PWEditor
    {
        private static Styles styles;

        private Vector2 m_scrollPosition = Vector2.zero;
        private EditorUtils m_editorUtils;

        private Spawner m_spawner;
        private List<SpawnCall> m_spawnLocations = new List<SpawnCall>();
        private SpawnCall m_globalIteration;
        private int m_editor_control_id = 0;

        // Caches used for settings undo recording by Unity
        private Spawner m_tmpSpawner;
        private float m_flowRate;

        // Helpers
        private bool m_needsSpawnCritOverrideUpdate = false;
        private bool m_hasPrefabs;
        private bool m_hasTrees;
        private bool m_hasTextures;
        private bool m_mouseDownForSpawn = false;
        private bool m_paintSpawn = false;
        private Vector2 m_lastMousePos = Vector2.zero;

        private Color m_separatorColor = Color.black;

		// Using this member so we don't keep querying EditorPrefs during each GUI draw
		private bool m_showQuickStart = true;

		// This is needed because in latest Unity displaying the progress bar mid-GUI results in GUI exceptions popping up in that cycle.
		int m_undoSteps = 0;

        //GUI items
        private Texture2D m_overridesIco;
        private Texture2D m_ChildSpawnerIco;

        // This will be used to iterate
        [SerializeField] private List<SpawnCall> m_lastSpawn = new List<SpawnCall>();

        private const string LINKED_ICON = "<color=#598ad3ff>" + Spawner.LINKED_SYMBOL + "</color>";
        private const string GX_ICON = "<color=#ff0000ff>GX</color>";
        //private const string UNDO_SYMBOL = "<color=#00ff00ff>\u21B6</color>";

        // Presets
        // This will hold the name of a target preset which the user can update with the spawner's settings
        [SerializeField] private int m_selectedPreset = -1;
        private string m_targetPreset;
        private string m_silentOverwritePreset;

        // Switch to drop custom ground level for ingestion
        bool m_dropGround = false;

        /// <summary>
        /// For queued spawn calls
        /// </summary>
        [Serializable]
        private class SpawnCall
        {
            public bool m_globalSpawn;
            public Vector3 m_location;
            public bool m_isSubspawn;
            public Transform m_transform;
            public Vector3 m_normal;

            public SpawnCall(Vector3 location)
            {
                m_isSubspawn = true;
                m_location = location;
            }

            public SpawnCall(Transform transform, Vector3 location, Vector3 normal, bool globalSpawn = false)
            {
                m_globalSpawn = globalSpawn;
                m_isSubspawn = false;
                m_transform = transform;
                m_location = location;
                m_normal = normal;
            }
        }

        #region Menu Commands
        /// <summary>
        /// Add spawner
        /// </summary>
        [MenuItem("GameObject/GeNa/Add Spawner", false, 14)]
        public static void AddGeNaSpawner(MenuCommand menuCommand)
        {
            //Create the spawner
            GameObject genaGo = new GameObject("GeNa Spawner");
            Spawner spawner = genaGo.AddComponent<Spawner>();
            spawner.SetDefaults(PWApp.CONF.MajorVersion, PWApp.CONF.MinorVersion, PWApp.CONF.PatchVersion);

            GameObject parent = menuCommand.context as GameObject;
            if (parent == null)
            {
                parent = GameObject.Find("GeNa Spawners");
                if (parent == null)
                {
                    parent = new GameObject("GeNa Spawners");
                }
            }

            // Reparent it
            GameObjectUtility.SetParentAndAlign(genaGo, parent);

            // Register the creation in the undo system

            UnityEditor.Undo.RegisterCreatedObjectUndo(genaGo, string.Format("[{0}] Created '{1}'", PWApp.CONF.Name, genaGo.name));

            //Make it active
            Selection.activeObject = genaGo;
        }

        #endregion

        #region Public Controls

        /// <summary>
        /// Set the target preset for this Spawner Editor
        /// </summary>
        public void SetTargetPreset(string target)
        {
            m_targetPreset = target;
        }

        #endregion

        #region Constructors destructors and related delegates

        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }

            if (styles != null)
            {
                styles.Dispose();
            }

			// Unity in rare cases can miss mouseUps - Let's make sure that we tie any loose ends.
			if (m_mouseDownForSpawn)
			{
				m_mouseDownForSpawn = false;

				if (m_spawnLocations.Count < 1)
				{
					if (m_paintSpawn)
					{
						m_spawner.RecordUndo("Painted Spawn");
						m_paintSpawn = false;
					}
					else
					{
						m_spawner.RecordUndo("Single Spawn");
					}
				}
			}
			if (GUIUtility.hotControl == m_editor_control_id)
			{
				GUIUtility.hotControl = 0;
			}
		}

        /// <summary>
        /// Called when we select this in the scene
        /// </summary>
        void OnEnable()
		{
			if (m_editorUtils == null)
			{
				// Get editor utils for this
				m_editorUtils = PWApp.GetEditorUtils(this);
			}

			//Check for target
			if (target == null)
			{
				return;
			}

			//Setup target
			m_spawner = (Spawner)target;

			//Update the Child Spawner List
			m_spawner.UpdateChildSpawners();

			// The below are safety to catch a weird Unity fluke I was not able to reproduce.
			if (m_spawner.m_seedThrowRange <= 0f)
			{
				m_spawner.m_minInstances = m_spawner.m_maxInstances = 1;
			}
			if (m_spawner.m_minInstances > m_spawner.m_maxInstances)
			{
				m_spawner.m_minInstances = m_spawner.m_maxInstances;
			}

			//Setup defaults
			m_spawner.SetDefaults(PWApp.CONF.MajorVersion, PWApp.CONF.MinorVersion, PWApp.CONF.PatchVersion);

			//Hide its transform
			m_spawner.transform.hideFlags = HideFlags.HideInInspector;

			//This will replace the other upgrade methods below.
			m_spawner.Upgrade(PWApp.CONF.MajorVersion, PWApp.CONF.MinorVersion, PWApp.CONF.PatchVersion);

			//Ensure that we have the correct Spawn Criteria values in each Proto in case this is a legacy spawner
			UpdateSpawnCritOverrides();

			//Update from legacy prtotype bounds modifier to the new override system
			UpdateLegacyProtoBoundsModifier();

			//Ensure that we know if the spawner has terrain protos
			m_spawner.UpdateHasActiveTerrainProto();

			//Add the delegate to cache spawner updates
			m_spawner.OnUpdate -= SpawnerToCache;
			m_spawner.OnUpdate += SpawnerToCache;

			//Get the control id
			m_editor_control_id = GUIUtility.GetControlID(this.GetHashCode(), FocusType.Passive);

			// Using this member so we don't keep querying EditorPrefs during each GUI draw
			m_showQuickStart = EditorPrefs.GetBool("GeNa_DoShowQuickStart", true);

			//Load textures
			if (EditorGUIUtility.isProSkin)
			{
				if (m_overridesIco == null)
				{
					m_overridesIco = Resources.Load("fschklicop") as Texture2D;
				}

				if (m_ChildSpawnerIco == null)
				{
					m_ChildSpawnerIco = Resources.Load("protoparentp") as Texture2D;
				}

				m_separatorColor = new Color(0.341f, 0.341f, 0.341f);
			}
			// or Unity Personal
			else
			{
				if (m_overridesIco == null)
				{
					m_overridesIco = Resources.Load("fschklico") as Texture2D;
				}

				if (m_ChildSpawnerIco == null)
				{
					m_ChildSpawnerIco = Resources.Load("protoparent") as Texture2D;
				}
			}

            // Get rid of old Undo records
            PurgeStaleUndo();
		}

		/// <summary>
		/// Removes the Undo records according to the Undo Purge Time settings
		/// </summary>
		private void PurgeStaleUndo()
		{
			UndoRecord[] array = m_spawner.UndoArrayCopy;
            if (array == null)
            {
                return;
            }

			List<UndoRecord> list = new List<UndoRecord>();
			int purgeLimit = Utils.GetFrapoch() - 60 * Preferences.UndoPurgeTime;

            int count = 0;
			foreach (UndoRecord record in array)
			{
				if (record.Time < purgeLimit)
				{
                    count++;
					continue;
				}

				list.Add(record);
			}

            if (count > 0 && Preferences.UndoExpiredMessages)
            {
                TimeSpan span = TimeSpan.FromMinutes(Preferences.UndoPurgeTime);
                Debug.LogFormat(m_editorUtils.GetTextValue("Undo records expired message"), count, string.Format("{0:00}:{1:00}", (int)span.TotalHours, span.Minutes));
            }

			m_spawner.UpdateUndoStack(list);
		}

		#endregion

		private void InitGUI()
        {
            if (SpawnerEditor.styles == null || styles.Inited == false)
            {
                if (styles != null)
                {
                    styles.Dispose();
                }

                SpawnerEditor.styles = new Styles();
            }
        }

        /// <summary>
        /// Create a copy of the spawner to be used by the GUI controls to allow recording settings Undo.
        /// </summary>
        private void SpawnerToCache()
        {
            if (m_spawner.IsCache)
            {
                Debug.LogErrorFormat("[GeNa] Cache [{0}-CACHE] is trying to create its own cache. How did we get here?", m_spawner.name);
                return;
            }

            if (m_tmpSpawner == null)
            {
                m_tmpSpawner = m_spawner.Cache;
            }

            // If new cache needed
            if (m_tmpSpawner == null)
            {
                GameObject cacheGO = new GameObject();

                //Parent it under the Spawner it belongs to, so if the spawner is deleted, this get deleted as well
                cacheGO.transform.SetParent(m_spawner.transform);
                cacheGO.hideFlags = HideFlags.HideAndDontSave;

                m_tmpSpawner = cacheGO.AddComponent<Spawner>();
                m_tmpSpawner.hideFlags = HideFlags.HideAndDontSave;
                m_tmpSpawner.CacheCopy(m_spawner);
            }
            else
            {
                m_tmpSpawner.DeepCopy(m_spawner);
            }
            m_flowRate = Mathf.Sqrt(m_tmpSpawner.m_flowDistanceSqr);
        }

        /// <summary>
        /// Update the Spawner with the cached values.
        /// </summary>
        private void CacheToSpawner()
        {
            m_tmpSpawner.m_flowDistanceSqr = m_flowRate * m_flowRate;
            m_spawner.DeepCopy(m_tmpSpawner);
        }

        /// <summary>
        /// Editor UX
        /// </summary>
        public override void OnInspectorGUI()
        {
            m_editorUtils.Initialize(); // Do not remove this!

            //Set the target
            //TODO: Do we really need to do this during every loop?
            m_spawner = (Spawner)target;
            if (m_tmpSpawner == null)
            {
                SpawnerToCache();
            }

            InitGUI();

            GUILayout.Space(3f);
            m_editorUtils.GUIHeader();

            #region Quick Start

            if (ActiveEditorTracker.sharedTracker.isLocked)
            {
                EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("Inspector locked warning"), MessageType.Warning);
            }

            GUILayout.BeginVertical(m_editorUtils.Styles.panelFrame);
            {
				GUILayout.BeginHorizontal();
                {
                    m_editorUtils.Label("Quick Start", styles.helpNoWrap);

                    GUILayout.FlexibleSpace();
					bool show = m_showQuickStart;
					m_editorUtils.HelpToggle(ref show);
                    if (show != m_showQuickStart)
                    {
						EditorPrefs.SetBool("GeNa_DoShowQuickStart", show);
						m_showQuickStart = show;
					}
                }
                GUILayout.EndHorizontal();

                if (m_showQuickStart)
                {
                    if (m_tmpSpawner.m_advShowDetailedHelp)
                    {
                        m_editorUtils.Label("Visualise Help", m_editorUtils.Styles.help);
                        m_editorUtils.Label("Rotation Help", m_editorUtils.Styles.help);
                        m_editorUtils.Label("Range Help", m_editorUtils.Styles.help);
                        m_editorUtils.Label("Instances Help", m_editorUtils.Styles.help);
                        m_editorUtils.Label("Move Last Help", m_editorUtils.Styles.help);
                        m_editorUtils.Label("Height&Rot Last Help", m_editorUtils.Styles.help);
                        m_editorUtils.Label("Delete All Help", m_editorUtils.Styles.help);
                        m_editorUtils.Label("Single Spawn Help", m_editorUtils.Styles.help);
                        m_editorUtils.Label("Global Spawn Help", m_editorUtils.Styles.help);
                        m_editorUtils.Label("Iterate Help", m_editorUtils.Styles.help);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Visualise: Shift + Left click.\nSingle Spawn: Ctrl + Left click.\nGlobal Spawn: Ctrl + Shift + Left click.", SpawnerEditor.styles.wrappedText);
                    }
                    if (m_editorUtils.Button("View Tutorials Btn"))
                    {
                        Application.OpenURL(PWApp.CONF.TutorialsLink);
                    }

                }
            }
            GUILayout.EndVertical();

            #endregion

            m_hasPrefabs = HasPrefabs(m_tmpSpawner.m_spawnPrototypes);
            m_hasTrees = HasTrees(m_tmpSpawner.m_spawnPrototypes);
            m_hasTextures = HasTextures(m_tmpSpawner.m_spawnPrototypes);

            //Scroll
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, false);
            {
                //Monitor for changes
                EditorGUI.BeginChangeCheck();
                {
                    // Overview Panel
                    string linkedIcon = "";
                    GUIStyle overviewLabelStyle = styles.panelLabel;
                    if (m_tmpSpawner.IsLinked)
                    {
                        linkedIcon = "<size=14>" + LINKED_ICON + "</size>";
                        overviewLabelStyle = styles.linkPanelLabel;
                    }
                    string overviewText = string.Format("{0} : {1} {2}", m_editorUtils.GetTextValue("Overview Panel"), m_tmpSpawner.m_parentName, linkedIcon);
                    GUIContent overviewPanelLabel = new GUIContent(overviewText, m_editorUtils.GetTooltip("Overview Panel"));
                    m_tmpSpawner.m_showOverview = m_editorUtils.Panel(overviewPanelLabel, OverviewPanel, overviewLabelStyle, m_tmpSpawner.m_showOverview);

                    // Placement Criteria Panel
                    m_tmpSpawner.m_showPlacementCriteria = m_editorUtils.Panel("Placement Criteria Panel Label", PlacementCritPanel, m_tmpSpawner.m_showPlacementCriteria);

                    // Spawn Criteria Panel
                    m_tmpSpawner.m_showSpawnCriteria = m_editorUtils.Panel("Spawn Criteria Panel Label", SpawnCritPanel, m_tmpSpawner.m_showSpawnCriteria);

                    // Prototypes Panel
                    GUIContent protoPanelLabel = new GUIContent(string.Format("{0} ({1}) [{2}]",
                        m_editorUtils.GetTextValue("Spawn Prototypes"), m_tmpSpawner.m_spawnPrototypes.Count, m_spawner.m_instancesSpawned),
                        m_editorUtils.GetTooltip("Spawn Prototypes"));
                    m_tmpSpawner.m_showPrototypes = m_editorUtils.Panel(protoPanelLabel, PrototypesPanel, m_tmpSpawner.m_showPrototypes);

                    // Advanced Panel
                    m_tmpSpawner.m_showAdvancedSettings = m_editorUtils.Panel("Advanced Panel Label", AdvancedPanel, m_tmpSpawner.m_showAdvancedSettings);

                    // Add Panel
                    AddPrototypesPanel();

                    #region Spawner Repeat & Reset

                    /*
                    if (m_spawner.m_spawnOriginLocation != Vector3.zero && m_spawner.m_spawnPrototypes.Count > 0)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(GetLabel("Spawn"), GUILayout.Height(30)))
                        {
                            m_spawner.Spawn(m_spawner.m_spawnOriginLocation, false);
                        }
                        if (GUILayout.Button(GetLabel("Reset Spawner"), GUILayout.Height(30)))
                        {
                            if (EditorUtility.DisplayDialog("WARNING!",
                                "Are you sure you want to delete all instances of the resources referred to by this spawner from your scene?",
                                "OK", "Cancel"))
                            {
                                m_spawner.UnspawnAll();
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    */

                    #endregion
                }

                #region Change handling

                //Check for changes, make undo record, make changes and let editor know we are dirty
                if (EditorGUI.EndChangeCheck())
                {
                    //UnityEditor.Undo.RecordObject(m_spawner, string.Format("[{0}] Spawner Edited", PWApp.CONF.Name));
                    EditorUtility.SetDirty(m_spawner);

                    if (m_spawner.m_parentName != m_tmpSpawner.m_parentName)
                    {
                        m_tmpSpawner.UpdateGoName();
                    }

                    if (m_spawner.m_minInstances != m_tmpSpawner.m_minInstances)
                    {
                        if (m_tmpSpawner.m_minInstances > m_tmpSpawner.m_maxInstances)
                        {
                            m_tmpSpawner.m_maxInstances = m_tmpSpawner.m_minInstances;
                        }
                    }

                    if (m_spawner.m_maxInstances != m_tmpSpawner.m_maxInstances)
                    {
                        if (m_tmpSpawner.m_maxInstances < m_tmpSpawner.m_minInstances)
                        {
                            m_tmpSpawner.m_minInstances = m_tmpSpawner.m_maxInstances;
                        }
                    }

                    if (m_spawner.m_randomSeed != m_tmpSpawner.m_randomSeed)
                    {
                        if (m_tmpSpawner.m_randomGenerator == null)
                        {
                            m_tmpSpawner.m_randomGenerator = new XorshiftPlus(m_tmpSpawner.m_randomSeed);
                        }
                        else
                        {
                            m_tmpSpawner.m_randomGenerator.Reset(m_tmpSpawner.m_randomSeed);
                        }
                    }

                    if (m_spawner.m_seedThrowRange != m_tmpSpawner.m_seedThrowRange)
                    {
                        if (m_tmpSpawner.m_seedThrowRange == 0)
                        {
                            m_tmpSpawner.m_minInstances = m_tmpSpawner.m_maxInstances = 1;
                        }
                    }

                    if (m_spawner.m_maxSpawnRange != m_tmpSpawner.m_maxSpawnRange)
                    {
                        m_flowRate = Mathf.Clamp(m_flowRate, 0.01f * m_tmpSpawner.m_maxSpawnRange, 5f * m_tmpSpawner.m_maxSpawnRange);
                    }

                    if (m_spawner.m_spawnAlgorithm != m_tmpSpawner.m_spawnAlgorithm)
                    {
                        if (m_tmpSpawner.m_spawnAlgorithm == Constants.LocationAlgorithm.Every && m_tmpSpawner.m_seedThrowRange < 0.5f)
                        {
                            m_tmpSpawner.m_seedThrowRange = 0.5f;
                        }
                    }

                    switch (m_tmpSpawner.m_rotationAlgorithm)
                    {
                        case Constants.RotationAlgorithm.Fixed:
                            if (m_spawner.m_rotationAlgorithm != m_tmpSpawner.m_rotationAlgorithm || m_spawner.m_minRotationY != m_tmpSpawner.m_minRotationY)
                            {
                                m_tmpSpawner.m_maxRotationY = m_tmpSpawner.m_minRotationY;
                            }
                            break;
                        case Constants.RotationAlgorithm.LastSpawnCenter:
                        case Constants.RotationAlgorithm.LastSpawnClosest:
                            if (m_spawner.m_rotationAlgorithm != m_tmpSpawner.m_rotationAlgorithm)
                            {
                                m_tmpSpawner.m_minRotationY = m_tmpSpawner.m_maxRotationY = 0f;
                            }
                            break;
                    }
                    m_tmpSpawner.m_minRotationY = Mathf.Min(m_tmpSpawner.m_minRotationY, m_tmpSpawner.m_maxRotationY);
                    m_tmpSpawner.m_maxRotationY = Mathf.Max(m_tmpSpawner.m_minRotationY, m_tmpSpawner.m_maxRotationY);

                    if (m_spawner.m_critVirginCheckType != m_tmpSpawner.m_critVirginCheckType && m_tmpSpawner.m_critVirginCheckType != Constants.VirginCheckType.Bounds)
                    {
                        m_tmpSpawner.m_critSpawnCollisionLayers = 1 << LayerMask.NameToLayer("Default");
                    }

                    if (m_spawner.m_critMinSpawnHeight != m_tmpSpawner.m_critMinSpawnHeight)
                    {
                        if (m_tmpSpawner.m_critMaxSpawnHeight < m_tmpSpawner.m_critMinSpawnHeight)
                        {
                            m_tmpSpawner.m_critMaxSpawnHeight = m_tmpSpawner.m_critMinSpawnHeight;
                        }
                    }

                    if (m_spawner.m_critMaxSpawnHeight != m_tmpSpawner.m_critMaxSpawnHeight)
                    {
                        if (m_tmpSpawner.m_critMaxSpawnHeight < m_tmpSpawner.m_critMinSpawnHeight)
                        {
                            m_tmpSpawner.m_critMinSpawnHeight = m_tmpSpawner.m_critMaxSpawnHeight;
                        }
                    }

                    if (m_spawner.m_critMaskType != m_tmpSpawner.m_critMaskType)
                    {
                        switch (m_tmpSpawner.m_critMaskType)
                        {
                            case Constants.MaskType.Perlin:
                                m_tmpSpawner.m_critMaskFractal.FractalType = Fractal.GeneratedFractalType.Perlin;
                                break;
                            case Constants.MaskType.Billow:
                                m_tmpSpawner.m_critMaskFractal.FractalType = Fractal.GeneratedFractalType.Billow;
                                break;
                            case Constants.MaskType.Ridged:
                                m_tmpSpawner.m_critMaskFractal.FractalType = Fractal.GeneratedFractalType.RidgeMulti;
                                break;
                        }
                    }

                    if (m_tmpSpawner.m_critMaskImage != null)
                    {
                        if (m_spawner.m_critMaskImage == null || m_spawner.m_critMaskImage.GetInstanceID() != m_tmpSpawner.m_critMaskImage.GetInstanceID())
                        {
                            MakeTextureReadable(m_tmpSpawner.m_critMaskImage);
                        }
                    }

                    if (m_tmpSpawner.m_maxScaleX != m_spawner.m_maxScaleX)
                    {
                        // Max could be pushing down min
                        m_tmpSpawner.m_minScaleX = Mathf.Min(m_tmpSpawner.m_minScaleX, m_tmpSpawner.m_maxScaleX);
                    }
                    if (m_tmpSpawner.m_minScaleX != m_spawner.m_minScaleX)
                    {
                        // Min could be pushing up max
                        m_tmpSpawner.m_maxScaleX = Mathf.Max(m_tmpSpawner.m_minScaleX, m_tmpSpawner.m_maxScaleX);
                    }

                    if (m_tmpSpawner.m_maxScaleY != m_spawner.m_maxScaleY)
                    {
                        // Max could be pushing down min
                        m_tmpSpawner.m_minScaleY = Mathf.Min(m_tmpSpawner.m_minScaleY, m_tmpSpawner.m_maxScaleY);
                    }
                    if (m_tmpSpawner.m_minScaleY != m_spawner.m_minScaleY)
                    {
                        // Min could be pushing up max
                        m_tmpSpawner.m_maxScaleY = Mathf.Max(m_tmpSpawner.m_minScaleY, m_tmpSpawner.m_maxScaleY);
                    }

                    if (m_tmpSpawner.m_maxScaleZ != m_spawner.m_maxScaleZ)
                    {
                        // Max could be pushing down min
                        m_tmpSpawner.m_minScaleZ = Mathf.Min(m_tmpSpawner.m_minScaleZ, m_tmpSpawner.m_maxScaleZ);
                    }
                    if (m_tmpSpawner.m_minScaleZ != m_spawner.m_minScaleZ)
                    {
                        // Min could be pushing up max
                        m_tmpSpawner.m_maxScaleZ = Mathf.Max(m_tmpSpawner.m_minScaleZ, m_tmpSpawner.m_maxScaleZ);
                    }

                    //Handle sorting
                    if (m_tmpSpawner.m_sortPrototypes != m_spawner.m_sortPrototypes || m_spawner.m_spawnPrototypes.Count != m_tmpSpawner.m_spawnPrototypes.Count)
                    {
                        if (m_tmpSpawner.m_sortPrototypes == true)
                        {
                            m_tmpSpawner.SortPrototypesAZ();
                        }
                    }

                    //Set name based on the first thing added
                    if (m_spawner.m_spawnPrototypes.Count == 0 && m_tmpSpawner.m_spawnPrototypes.Count > 0)
                    {
                        m_tmpSpawner.m_parentName = m_tmpSpawner.m_spawnPrototypes[0].m_name;
                        m_tmpSpawner.UpdateGoName();

                        //Also set the flow rate to be the first prototype's max horizontal size + 5cm
                        m_flowRate = Mathf.Round(100f * Mathf.Max(m_tmpSpawner.m_spawnPrototypes[0].m_size.x, m_tmpSpawner.m_spawnPrototypes[0].m_size.z)) * 0.01f + 0.05f;
                    }

                    //Update their ID's
                    UpdateResourceIDs();

                    //Tidy up any old gravity settings
                    if (m_spawner.m_useGravity != m_tmpSpawner.m_useGravity)
                    {
                        if (!m_tmpSpawner.m_useGravity)
                        {
                            AssetDatabase.DeleteAsset(string.Format("Assets/Gravity-{0}.asset", m_spawner.m_gravity.GetInstanceID()));
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            m_tmpSpawner.m_gravity = null;
                        }
                        else
                        {
                            //Check heights and add to them if necessary
                            bool canUpdateHeights = false;
                            Prototype proto;
                            Resource res;
                            for (int protoIdx = 0; protoIdx < m_tmpSpawner.m_spawnPrototypes.Count; protoIdx++)
                            {
                                proto = m_tmpSpawner.m_spawnPrototypes[protoIdx];
                                for (int resIdx = 0; resIdx < proto.m_resourceTree.Count; resIdx++)
                                {
                                    res = proto.m_resourceTree[resIdx];
                                    if (res.m_resourceType == Constants.ResourceType.Prefab && res.m_minOffset.y <= 5f)
                                    {
                                        canUpdateHeights = true;
                                        break;
                                    }
                                }
                            }
                            if (canUpdateHeights == true)
                            {
                                if (EditorUtility.DisplayDialog("Can I help?",
                                    "You are doing a Gravity spawn and it looks like you forgot to update your height offsets. Want me to fix them for you?",
                                    "Yes", "No"))
                                {
                                    for (int protoIdx = 0; protoIdx < m_tmpSpawner.m_spawnPrototypes.Count; protoIdx++)
                                    {
                                        proto = m_tmpSpawner.m_spawnPrototypes[protoIdx];
                                        for (int resIdx = 0; resIdx < proto.m_resourceTree.Count; resIdx++)
                                        {
                                            res = proto.m_resourceTree[resIdx];
                                            if (res.m_resourceType == Constants.ResourceType.Prefab)
                                            {
                                                res.m_minOffset.y = 20f;
                                                res.m_maxOffset.y = 20f;
                                            }
                                        }
                                    }
                                }
                            }

                            m_tmpSpawner.m_gravity = ScriptableObject.CreateInstance<Gravity>();
                            AssetDatabase.CreateAsset(m_tmpSpawner.m_gravity, string.Format("Assets/Gravity-{0}.asset", m_tmpSpawner.m_gravity.GetInstanceID()));
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            //gravity = (Gravity)EditorGUILayout.ObjectField(GetLabel("Gravity"), m_tmpSpawner.m_gravity, typeof(Gravity), false);
                        }
                    }

                    CacheToSpawner();

                    //Settings changed, let's update ranges - probably no need to update child spawners, since their settings did not change.
                    m_spawner.UpdateTargetSpawnerRanges(false);
                    //Let's make sure that's also reflected in the GUI cache
                    SpawnerToCache();
                }

                #endregion

                GUILayout.Space(5);


            } //End scroll
            GUILayout.EndScrollView();

            //By this point the spawner should be up to date. Update the overrides in protos if needed
            if (m_needsSpawnCritOverrideUpdate)
            {
                UpdateSpawnCritOverrides();
            }

            m_editorUtils.GUIFooter();

            // This was necessary because in latest Unity displaying the progress bar mid-GUI results in GUI exceptions popping up in that cycle.
            if (m_undoSteps > 0 && Event.current.type == EventType.Repaint)
            {
                Undo(m_undoSteps);
				m_undoSteps = 0;
			}
        }

		#region Panels

        /// <summary>
        /// Overview Panel
        /// </summary>
        private void OverviewPanel(bool helpEnabled)
        {
            m_editorUtils.InlineHelp("Overview Panel", helpEnabled);

            // Presets
            PresetsGUI(helpEnabled);

            // Undo
            bool undoEmpty = true;
            string undoBtnLabel = " (-)";
            UndoRecord[] undoArray = m_spawner.UndoArrayCopy;
            GUIContent[] undoLabelsArray = null;

            undoLabelsArray = new GUIContent[undoArray.Length + 1];
            undoLabelsArray[0] = m_editorUtils.GetContent("Undo List");
            undoBtnLabel = string.Format(" ({0}/{1})", undoArray.Length, m_spawner.UndoSteps);

            if (undoArray.Length > 0)
            {
                undoEmpty = false;
                UndoRecord topRecord = undoArray[0];
                undoBtnLabel += string.Format(" - [{0}] {1} ", GetTimeDelta(topRecord), topRecord.Description);

                int groupingLimitSeconds = Preferences.UndoGroupingTime;

                int lastTime = 0;
                int burstIndex = 0;

                for (int i = 0; i < undoArray.Length; i++)
                {
                    if (lastTime - undoArray[i].Time > groupingLimitSeconds)
                    {
                        burstIndex++;
                    }
                    undoLabelsArray[i + 1] = new GUIContent(string.Format("{0}.  ({1}) [{2}] {3}", i + 1, burstIndex + 1, GetTimeDelta(undoArray[i]), undoArray[i].Description), "Undo");

                    lastTime = undoArray[i].Time;
                }
            }

            // Undo Section
            EditorGUI.BeginDisabledGroup(undoEmpty);
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(new GUIContent(undoBtnLabel, styles.undoIco, m_editorUtils.GetTooltip("Undo")), styles.richButtonMini, GUILayout.MaxHeight(15f)))
                    {
                        Undo();
                    }
                    m_undoSteps = EditorGUILayout.Popup(m_undoSteps, undoLabelsArray);
                }
                GUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();
            m_editorUtils.InlineHelp("Undo", helpEnabled);

            // Controls
            m_tmpSpawner.m_parentName = m_editorUtils.TextField("Spawner Name", m_tmpSpawner.m_parentName, helpEnabled);
            m_tmpSpawner.m_type = (Constants.SpawnerType)m_editorUtils.EnumPopup("Spawner Type", m_tmpSpawner.m_type, helpEnabled);
            m_tmpSpawner.m_spawnMode = (Constants.SpawnMode)m_editorUtils.EnumPopup("Spawn Mode", m_tmpSpawner.m_spawnMode, helpEnabled);

            if (!m_tmpSpawner.m_advUseLargeRanges)
            {
                if (m_tmpSpawner.m_spawnMode > Constants.SpawnMode.Single)
                {
                    EditorGUI.indentLevel += 1;
                    m_flowRate = m_editorUtils.Slider("Flow Rate", m_flowRate, 0.01f * m_tmpSpawner.m_maxSpawnRange, 5f * m_tmpSpawner.m_maxSpawnRange, helpEnabled);
                    EditorGUI.indentLevel -= 1;
                }

                m_tmpSpawner.m_spawnRangeShape = (Constants.SpawnRangeShape)m_editorUtils.EnumPopup("Spawn Shape", m_tmpSpawner.m_spawnRangeShape, helpEnabled);
                m_tmpSpawner.m_randomSeed = m_editorUtils.IntField("Spawn Seed", m_tmpSpawner.m_randomSeed, helpEnabled);

                m_tmpSpawner.m_maxSpawnRange = m_editorUtils.Slider("Spawn Range", m_tmpSpawner.m_maxSpawnRange, 1f, 200f, helpEnabled);
                if (m_tmpSpawner.m_spawnAlgorithm == Constants.LocationAlgorithm.Every)
                {
                    m_tmpSpawner.m_seedThrowRange = m_editorUtils.Slider("Throw Distance", m_tmpSpawner.m_seedThrowRange, 0.5f, m_tmpSpawner.m_maxSpawnRange, helpEnabled);
                }
                else
                {
                    m_tmpSpawner.m_seedThrowRange = m_editorUtils.Slider("Throw Distance", m_tmpSpawner.m_seedThrowRange, 0f, m_tmpSpawner.m_maxSpawnRange, helpEnabled);
                }

                if (m_tmpSpawner.m_seedThrowRange > 0)
                {
                    int instanceLimit = GetInstancesTopLimit();
                    int min = (int)m_tmpSpawner.m_minInstances;
                    int max = (int)m_tmpSpawner.m_maxInstances;
                    m_editorUtils.MinMaxSliderWithFields("Instances", ref min, ref max, 1, instanceLimit, helpEnabled);
                    m_tmpSpawner.m_minInstances = min;
                    m_tmpSpawner.m_maxInstances = max;
                }
            }
            else
            {
                if (m_tmpSpawner.m_spawnMode > Constants.SpawnMode.Single)
                {
                    EditorGUI.indentLevel += 1;
                    m_flowRate = m_editorUtils.FloatField("Flow Rate", m_flowRate, helpEnabled);
                    EditorGUI.indentLevel -= 1;
                }

                m_tmpSpawner.m_spawnRangeShape = (Constants.SpawnRangeShape)m_editorUtils.EnumPopup("Spawn Shape", m_tmpSpawner.m_spawnRangeShape, helpEnabled);
                m_tmpSpawner.m_randomSeed = m_editorUtils.IntField("Spawn Seed", m_tmpSpawner.m_randomSeed, helpEnabled);

                m_tmpSpawner.m_maxSpawnRange = m_editorUtils.FloatField("Spawn Range", m_tmpSpawner.m_maxSpawnRange, helpEnabled);
                m_tmpSpawner.m_seedThrowRange = m_editorUtils.FloatField("Throw Distance", m_tmpSpawner.m_seedThrowRange, helpEnabled);

                if (m_tmpSpawner.m_seedThrowRange > 0)
                {
                    m_tmpSpawner.m_minInstances = m_editorUtils.LongField("Min Instances", (long)m_tmpSpawner.m_minInstances, helpEnabled);
                    m_tmpSpawner.m_maxInstances = m_editorUtils.LongField("Max Instances", (long)m_tmpSpawner.m_maxInstances, helpEnabled);
                }
            }

            if (m_hasPrefabs == true)
            {
                m_tmpSpawner.m_mergeSpawns = m_editorUtils.Toggle("Merge Instances", m_tmpSpawner.m_mergeSpawns, helpEnabled);
            }

#if SECTR_CORE_PRESENT
            m_tmpSpawner.m_doSectorise = EditorGUILayout.BeginToggleGroup(m_editorUtils.GetContent("SECTR Sectorisation"), m_tmpSpawner.m_doSectorise);
            {
                m_editorUtils.InlineHelp("SECTR Sectorisation", helpEnabled);
                m_tmpSpawner.m_sectorReparentingMode = (SECTR_Constants.ReparentingMode)m_editorUtils.EnumPopup("Localize Ojbects by", m_tmpSpawner.m_sectorReparentingMode, helpEnabled);
            }
            EditorGUILayout.EndToggleGroup();
#endif
        }

        /// <summary>
        /// Presets GUI
        /// </summary>
        private void PresetsGUI(bool helpEnabled)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(2f);

                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_targetPreset) || m_targetPreset.Contains(PresetSaveDialog.BUILTIN_SIGNATURE));
                {
                    if (m_editorUtils.SaveButtonMini(m_editorUtils.GetTooltip("Save Preset Btn") +
                        (string.IsNullOrEmpty(m_targetPreset) ? "-" : m_targetPreset)))
                    {
                        if (ShouldOverwritePreset(m_targetPreset))
                        {
                            bool saved = false;

                            foreach (SpawnerPreset preset in SpawnerPreset.AvailablePresets)
                            {
                                if (preset.ToString() == m_targetPreset)
                                {
                                    SaveToPreset(preset);
                                    saved = true;
                                    break;
                                }
                            }

                            if (!saved)
                            {
                                EditorUtility.DisplayDialog("GeNa", string.Format("Could not find '{0}' in the available presets.", m_targetPreset), m_editorUtils.GetTextValue("OkBtn"));
                            }
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();

                if (m_editorUtils.SaveAsButtonMini(m_editorUtils.GetTooltip("Save Preset As Btn")))
                {
                    PresetSaveDialog window = EditorWindow.GetWindow<PresetSaveDialog>(true, "GeNa: Save Spawner Preset", true);
                    window.Init(this);
                }

                GUILayout.Space(-2f);

                List<SpawnerPreset> presetList = new List<SpawnerPreset>(SpawnerPreset.AvailablePresets);

                List<GUIContent> list = new List<GUIContent>();
                list.Add(m_editorUtils.GetContent("Presets"));
                for (int i = 0; i < presetList.Count; i++)
                {
                    string presetName = presetList[i].ToString();
                    if (presetName == m_targetPreset)
                    {
                        m_selectedPreset = i;
                    }

                    list.Add(new GUIContent(presetName));
                }

                int selected = EditorGUILayout.Popup(m_selectedPreset + 1, list.ToArray());
                if (selected != m_selectedPreset + 1)
                {
                    if (selected > 0)
                    {
                        SpawnerPreset preset = presetList[selected - 1];
                        if (preset.ActiveOptions.Count > 0)
                        {
                            string msg = string.Format(m_editorUtils.GetTextValue("Sure to apply preset dialog"), preset.Name, string.Join("\n", preset.ActiveOptions.ToArray()));
                            if (EditorUtility.DisplayDialog("GeNa", msg, m_editorUtils.GetTextValue("OkBtn"), m_editorUtils.GetTextValue("CancelBtn")))
                            {
                                m_selectedPreset = selected - 1;
                                m_targetPreset = preset.ToString();

                                preset.Apply(m_tmpSpawner);
                                m_flowRate = Mathf.Sqrt(m_tmpSpawner.m_flowDistanceSqr);
                                GUI.changed = true;
                            }
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("GeNa", m_editorUtils.GetTextValue("Preset does nothing dialog"), m_editorUtils.GetTextValue("OkBtn"));                                
                        }
                    }
                    else
                    {
                        m_selectedPreset = -1;
                        m_targetPreset = null;
                    }
                }
            }
            GUILayout.EndHorizontal();
            m_editorUtils.InlineHelp("Presets", helpEnabled);
        }
        
        /// <summary>
        /// Returns 0: overwrite; 1: Overwrite and don't show warning for the file in this session; 2; Don't overwrite
        /// </summary>
        /// <returns></returns>
        public bool ShouldOverwritePreset(string targetName)
        {
            if (targetName == m_silentOverwritePreset)
            {
                return true;
            }

            int choice = EditorUtility.DisplayDialogComplex("GeNa: Save Preset", string.Format(m_editorUtils.GetTextValue("Sure to overwrite preset Dialog"), targetName),
                m_editorUtils.GetTextValue("Yes Btn"), m_editorUtils.GetTextValue("Yes - Don't show warning Btn"), m_editorUtils.GetTextValue("CancelBtn"));

            if (choice == 1)
            {
                m_silentOverwritePreset = targetName;
            }

            return choice < 2;
        }

        /// <summary>
        /// Saves the spawner settings into the provided preset.
        /// </summary>
        public void SaveToPreset(SpawnerPreset targetPreset)
        {
            targetPreset.FromSpawner(m_tmpSpawner);
            EditorUtility.SetDirty(targetPreset);
        }

        /// <summary>
        /// Calculates roughly the max number of instances that fit into the spawn range.
        /// </summary>
        private int GetInstancesTopLimit()
		{
            // With bounds checked spawning, the actual extents will give a good indication of
            // the maximum number of items we can fit into the spawn range
            if (m_tmpSpawner.m_critVirginCheckType == Constants.VirginCheckType.Bounds)
            {
                // Placement Criteria Scaling
                Vector3 minScale = Vector3.zero;
                if (m_tmpSpawner.m_sameScale)
                {
                    minScale = Vector3.one * m_tmpSpawner.m_minScaleX;
                }
                else
                {
                    minScale = new Vector3(m_tmpSpawner.m_minScaleX, m_tmpSpawner.m_minScaleY, m_tmpSpawner.m_minScaleZ);
                }

                float minExtent = float.MaxValue;

                for (int i = 0; i < m_tmpSpawner.m_spawnPrototypes.Count; i++)
                {
                    Vector3 extent = m_tmpSpawner.m_spawnPrototypes[i].GetMinExtents(minScale);
                    minExtent = Mathf.Min(minExtent, extent.x, extent.z);

                    // Let's apply a minimum 1mm
                    if (minExtent <= 0f)
                    {
                        minExtent = 0.001f;
                    }
                }

                float minDimension = minExtent * 2f;
                // +1 for the edge cases (0.5 * 2) and multiply it by 1.25 to add some leeway
                float instanceLimit = (m_tmpSpawner.m_maxSpawnRange / minDimension + 1f) * 1.25f;
                // Square of that is pretty much what we can fit in the spawn range
                instanceLimit *= instanceLimit;

                if (m_tmpSpawner.m_spawnAlgorithm == Constants.LocationAlgorithm.Organic)
                {
                    // And in the case of Organic spawning add even more leeway
                    instanceLimit *= 2f;
                }

                return  (int)Mathf.Clamp(instanceLimit, 1, int.MaxValue);
            }

            // Otherwise we just calculate like before
            float limit = (m_tmpSpawner.m_maxSpawnRange * m_tmpSpawner.m_maxSpawnRange) / (0.33f * m_tmpSpawner.m_seedThrowRange);
            return (int)Mathf.Clamp(limit, 1, int.MaxValue);

        }

		/// <summary>
		/// Returns the formatted time since the UndoRecord was recorded.
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		private static string GetTimeDelta(UndoRecord record)
		{
			TimeSpan delta = TimeSpan.FromSeconds(Utils.GetFrapoch() - record.Time);
			return string.Format("{0}{1}m", (int)delta.TotalHours > 0 ? (int)delta.TotalHours + "h " : "", delta.Minutes);
		}

		/// <summary>
		/// Placement Crit Panel
		/// </summary>
		private void PlacementCritPanel(bool helpEnabled)
        {
            m_editorUtils.Label("Control how and where we can spawn.", SpawnerEditor.styles.wrappedText);

            m_tmpSpawner.m_spawnAlgorithm = (Constants.LocationAlgorithm)m_editorUtils.EnumPopup("Spawn Type", m_tmpSpawner.m_spawnAlgorithm, helpEnabled);

            if (m_tmpSpawner.m_spawnAlgorithm == Constants.LocationAlgorithm.Every)
            {
                EditorGUI.indentLevel++;
                m_tmpSpawner.m_seedThrowJitter = m_editorUtils.Slider("Jitter Strength", m_tmpSpawner.m_seedThrowJitter, 0f, 1f, helpEnabled);
                EditorGUI.indentLevel--;
            }

            if (m_hasPrefabs == true || m_hasTrees == true || (m_tmpSpawner.m_critCheckMask && m_tmpSpawner.m_critMaskType == Constants.MaskType.Image))
            {
                m_tmpSpawner.m_rotationAlgorithm = (Constants.RotationAlgorithm)m_editorUtils.EnumPopup("Rotation Type", m_tmpSpawner.m_rotationAlgorithm, helpEnabled);

                switch (m_tmpSpawner.m_rotationAlgorithm)
                {
                    case Constants.RotationAlgorithm.Ranged:
                        EditorGUI.indentLevel++;
                        m_editorUtils.MinMaxSliderWithFields("Rotation", ref m_tmpSpawner.m_minRotationY, ref m_tmpSpawner.m_maxRotationY, 0f, 360f, helpEnabled);
                        EditorGUI.indentLevel--;
                        break;
                    case Constants.RotationAlgorithm.Fixed:
                        EditorGUI.indentLevel++;
                        m_tmpSpawner.m_minRotationY = m_editorUtils.Slider("Fixed Rotation", m_tmpSpawner.m_minRotationY, 0f, 360f, helpEnabled);
                        m_tmpSpawner.m_enableRotationDragUpdate = m_editorUtils.Toggle("Draggable Rotation", m_tmpSpawner.m_enableRotationDragUpdate, helpEnabled);
                        EditorGUI.indentLevel--;
                        break;
                    default:
                        break;
                }

                PlacementScale(helpEnabled);
            }
            else if (m_hasTextures)
            {
                PlacementScale(helpEnabled);
            }

            if (m_hasPrefabs == true)
            {
                m_tmpSpawner.m_useGravity = m_editorUtils.Toggle("Use Gravity", m_tmpSpawner.m_useGravity, helpEnabled);
            }

            //And show the new ones
            if (m_tmpSpawner.m_useGravity == true && m_spawner.m_gravity != null)
            {
                if (m_tmpSpawner.m_gravity.m_haveGravity == false)
                {
                    if (m_tmpSpawner.m_gravity.m_instances.Count == 0)
                    {
                        m_editorUtils.Label("Gravity Nothing Spawned");
                    }
                    else
                    {
                        m_editorUtils.Label("Gravity Needs Runtime");
                    }
                }
                else
                {
                    if (m_editorUtils.Button("Gravity Original Position Btn", helpEnabled))
                    {
                        m_spawner.m_gravity.UpdateOriginalsToStart();
                    }
                    if (m_editorUtils.Button("Gravity Final Position Btn", helpEnabled))
                    {
                        m_spawner.m_gravity.UpdateOriginalsToEnd();
                    }
                    if (m_editorUtils.Button("Gravity Apply Gravity Btn", helpEnabled))
                    {
                        m_spawner.m_gravity.FinaliseGravity(m_spawner);
                    }
                }
            }
        }

        /// <summary>
        /// Scale controls for the placement panel
        /// </summary>
        private void PlacementScale(bool helpEnabled)
        {
            m_tmpSpawner.m_sameScale = m_editorUtils.Toggle("Same Scale XYZ", m_tmpSpawner.m_sameScale, helpEnabled);
            EditorGUI.indentLevel++;
            if (m_tmpSpawner.m_sameScale)
            {
                if (!m_tmpSpawner.m_scaleToNearestInt)
                {
                    m_editorUtils.MinMaxSliderWithFields("Scale", ref m_tmpSpawner.m_minScaleX, ref m_tmpSpawner.m_maxScaleX, 0.1f, 100f, helpEnabled);
                }
                else
                {
                    int min = (int)m_tmpSpawner.m_minScaleX;
                    int max = (int)m_tmpSpawner.m_maxScaleX;
                    m_editorUtils.MinMaxSliderWithFields("Scale", ref min, ref max, 1, 100, helpEnabled);
                    m_tmpSpawner.m_minScaleX = min;
                    m_tmpSpawner.m_maxScaleX = max;
                }
            }
            else
            {
                if (!m_tmpSpawner.m_scaleToNearestInt)
                {
                    m_tmpSpawner.m_minScaleX = m_editorUtils.Slider("Min Scale X", m_tmpSpawner.m_minScaleX, 0.1f, 1000f, helpEnabled);
                    m_tmpSpawner.m_maxScaleX = m_editorUtils.Slider("Max Scale X", m_tmpSpawner.m_maxScaleX, 0.1f, 1000f, helpEnabled);
                    m_tmpSpawner.m_minScaleY = m_editorUtils.Slider("Min Scale Y", m_tmpSpawner.m_minScaleY, 0.1f, 1000f, helpEnabled);
                    m_tmpSpawner.m_maxScaleY = m_editorUtils.Slider("Max Scale Y", m_tmpSpawner.m_maxScaleY, 0.1f, 1000f, helpEnabled);
                    m_tmpSpawner.m_minScaleZ = m_editorUtils.Slider("Min Scale Z", m_tmpSpawner.m_minScaleZ, 0.1f, 1000f, helpEnabled);
                    m_tmpSpawner.m_maxScaleZ = m_editorUtils.Slider("Max Scale Z", m_tmpSpawner.m_maxScaleZ, 0.1f, 1000f, helpEnabled);
                }
                else
                {
                    m_tmpSpawner.m_minScaleX = m_editorUtils.IntSlider("Min Scale X", (int)m_tmpSpawner.m_minScaleX, 1, 1000, helpEnabled);
                    m_tmpSpawner.m_maxScaleX = m_editorUtils.IntSlider("Max Scale X", (int)m_tmpSpawner.m_maxScaleX, 1, 1000, helpEnabled);
                    m_tmpSpawner.m_minScaleY = m_editorUtils.IntSlider("Min Scale Y", (int)m_tmpSpawner.m_minScaleY, 1, 1000, helpEnabled);
                    m_tmpSpawner.m_maxScaleY = m_editorUtils.IntSlider("Max Scale Y", (int)m_tmpSpawner.m_maxScaleY, 1, 1000, helpEnabled);
                    m_tmpSpawner.m_minScaleZ = m_editorUtils.IntSlider("Min Scale Z", (int)m_tmpSpawner.m_minScaleZ, 1, 1000, helpEnabled);
                    m_tmpSpawner.m_maxScaleZ = m_editorUtils.IntSlider("Max Scale Z", (int)m_tmpSpawner.m_maxScaleZ, 1, 1000, helpEnabled);
                }
            }
            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Spawn Criteria Panel
        /// </summary>
        /// <param name="helpEnabled"></param>
        private void SpawnCritPanel(bool helpEnabled)
        {
            m_editorUtils.Label("Control when we can spawn.", SpawnerEditor.styles.wrappedText);

            // Safest way to check if any spawn criteria was changed
            EditorGUI.BeginChangeCheck();
            {
                m_tmpSpawner.m_critVirginCheckType = (Constants.VirginCheckType)m_editorUtils.EnumPopup("Check Collisions", m_tmpSpawner.m_critVirginCheckType, helpEnabled);
                if (m_tmpSpawner.m_critVirginCheckType != Constants.VirginCheckType.None)
                {
                    if (m_tmpSpawner.m_critVirginCheckType == Constants.VirginCheckType.Bounds)
                    {
                        EditorGUI.indentLevel += 1;
                        m_tmpSpawner.m_critBoundsBorder = m_editorUtils.Slider("Modify Bounds", m_tmpSpawner.m_critBoundsBorder, -0.9f, +5f, helpEnabled);
                        EditorGUI.indentLevel -= 1;
                    }
                    m_tmpSpawner.m_critSpawnCollisionLayers = LayerMaskField(m_editorUtils.GetContent("Collision Layers"), m_tmpSpawner.m_critSpawnCollisionLayers);
                    m_editorUtils.InlineHelp("Collision Layers", helpEnabled);
                }

                //Check Height
                m_tmpSpawner.m_critCheckHeightType = (Constants.CriteriaRangeType)m_editorUtils.EnumPopup("Check Height Type", m_tmpSpawner.m_critCheckHeightType, helpEnabled);
                if (m_tmpSpawner.m_critCheckHeightType != Constants.CriteriaRangeType.None)
                {
                    EditorGUI.indentLevel++;
                    m_tmpSpawner.m_showCritMinSpawnHeight = m_tmpSpawner.m_showCritMaxSpawnHeight = m_editorUtils.Toggle("Visualize", m_tmpSpawner.m_showCritMinSpawnHeight, helpEnabled);
                    if (m_tmpSpawner.m_advUseLargeRanges)
                    {
                        if (m_tmpSpawner.m_critCheckHeightType >= Constants.CriteriaRangeType.MinMax)
                        {
                            m_tmpSpawner.m_critMinSpawnHeight = m_editorUtils.FloatField("Min Height", m_tmpSpawner.m_critMinSpawnHeight, helpEnabled);
                            m_tmpSpawner.m_critMaxSpawnHeight = m_editorUtils.FloatField("Max Height", m_tmpSpawner.m_critMaxSpawnHeight, helpEnabled);
                        }
                        else
                        {
                            m_editorUtils.LabelField("Min Height", new GUIContent(m_tmpSpawner.m_critMinHeight.ToString()), helpEnabled);
                            m_editorUtils.LabelField("Max Height", new GUIContent(m_tmpSpawner.m_critMaxHeight.ToString()), helpEnabled);
                        }

                        switch (m_tmpSpawner.m_critCheckHeightType)
                        {
                            case Constants.CriteriaRangeType.Range:
                            case Constants.CriteriaRangeType.Mixed:
                                m_tmpSpawner.m_critHeightVariance = m_editorUtils.FloatField("Height Range", m_tmpSpawner.m_critHeightVariance, helpEnabled);
                                break;
                        }
                    }
                    else
                    {
                        if (m_tmpSpawner.m_critCheckHeightType >= Constants.CriteriaRangeType.MinMax)
                        {
                            m_tmpSpawner.m_critMinSpawnHeight = m_editorUtils.Slider("Min Height", m_tmpSpawner.m_critMinSpawnHeight, m_spawner.m_bottomBoundary, m_spawner.m_topBoundary, helpEnabled);
                            m_tmpSpawner.m_critMaxSpawnHeight = m_editorUtils.Slider("Max Height", m_tmpSpawner.m_critMaxSpawnHeight, m_spawner.m_bottomBoundary, m_spawner.m_topBoundary, helpEnabled);
                        }
                        else
                        {
                            m_editorUtils.LabelField("Min Height", new GUIContent(m_tmpSpawner.m_critMinHeight.ToString()), helpEnabled);
                            m_editorUtils.LabelField("Max Height", new GUIContent(m_tmpSpawner.m_critMaxHeight.ToString()), helpEnabled);
                        }

                        switch (m_tmpSpawner.m_critCheckHeightType)
                        {
                            case Constants.CriteriaRangeType.Range:
                            case Constants.CriteriaRangeType.Mixed:
                                m_tmpSpawner.m_critHeightVariance = m_editorUtils.Slider("Height Range", m_tmpSpawner.m_critHeightVariance, 0.1f, 200f, helpEnabled);
                                break;
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                //Check Slope
                m_tmpSpawner.m_critCheckSlopeType = (Constants.CriteriaRangeType)m_editorUtils.EnumPopup("Check Slope Type", m_tmpSpawner.m_critCheckSlopeType, helpEnabled);
                if (m_tmpSpawner.m_critCheckSlopeType != Constants.CriteriaRangeType.None)
                {
                    EditorGUI.indentLevel++;
                    if (m_tmpSpawner.m_critCheckSlopeType >= Constants.CriteriaRangeType.MinMax)
                    {
                        m_tmpSpawner.MinSpawnSlope = m_editorUtils.Slider("Min Slope", m_tmpSpawner.MinSpawnSlope, 0f, 90f, helpEnabled);
                        m_tmpSpawner.MaxSpawnSlope = m_editorUtils.Slider("Max Slope", m_tmpSpawner.MaxSpawnSlope, 0f, 90f, helpEnabled);
                    }
                    else
                    {
                        m_editorUtils.LabelField("Min Slope", new GUIContent(m_tmpSpawner.m_critMinSlope.ToString()), helpEnabled);
                        m_editorUtils.LabelField("Max Slope", new GUIContent(m_tmpSpawner.m_critMaxSlope.ToString()), helpEnabled);
                    }

                    switch (m_tmpSpawner.m_critCheckSlopeType)
                    {
                        case Constants.CriteriaRangeType.Range:
                        case Constants.CriteriaRangeType.Mixed:
                            m_tmpSpawner.m_critSlopeVariance = m_editorUtils.Slider("Slope Range", m_tmpSpawner.m_critSlopeVariance, 0.1f, 180f, helpEnabled);
                            break;
                    }
                    EditorGUI.indentLevel--;
                }

                //Check Textures
                m_tmpSpawner.m_critCheckTextures = m_editorUtils.Toggle("Check Textures", m_tmpSpawner.m_critCheckTextures, helpEnabled);
                if (m_tmpSpawner.m_critCheckTextures)
                {
                    EditorGUI.indentLevel++;
                    m_editorUtils.LabelField("Texture Crit", new GUIContent(string.Format("({0}) {1}", m_tmpSpawner.m_critSelectedTextureIdx, m_spawner.m_critSelectedTextureName)), helpEnabled);
                    m_tmpSpawner.m_critTextureStrength = m_editorUtils.Slider("Texture Strength", m_tmpSpawner.m_critTextureStrength, 0.0f, 1f, helpEnabled);
                    m_tmpSpawner.m_critTextureVariance = m_editorUtils.Slider("Texture Range", m_tmpSpawner.m_critTextureVariance, 0.0f, 1f, helpEnabled);
                    EditorGUI.indentLevel--;
                }

                //Check Mask
                m_tmpSpawner.m_critCheckMask = m_editorUtils.Toggle("Check Mask", m_tmpSpawner.m_critCheckMask, helpEnabled);
                if (m_tmpSpawner.m_critCheckMask)
                {
                    EditorGUI.indentLevel++;
                    m_tmpSpawner.m_critMaskType = (Constants.MaskType)m_editorUtils.EnumPopup("Mask Type", m_tmpSpawner.m_critMaskType, helpEnabled);
                    if (m_tmpSpawner.m_critMaskType != Constants.MaskType.Image)
                    {
                        m_tmpSpawner.m_critMaskFractal.Seed = m_editorUtils.IntSlider("Seed", (int)m_tmpSpawner.m_critMaskFractal.Seed, 0, 65000, helpEnabled);
                        m_tmpSpawner.m_critMaskFractal.Octaves = m_editorUtils.IntSlider("Octaves", m_tmpSpawner.m_critMaskFractal.Octaves, 1, 12, helpEnabled);
                        if (m_tmpSpawner.m_advUseLargeRanges)
                        {
                            m_tmpSpawner.m_critMaskFractal.Frequency = m_editorUtils.Slider("Frequency", m_tmpSpawner.m_critMaskFractal.Frequency, 0f, 1f, helpEnabled);
                        }
                        else
                        {
                            m_tmpSpawner.m_critMaskFractal.Frequency = m_editorUtils.Slider("Frequency", m_tmpSpawner.m_critMaskFractal.Frequency, 0f, 0.3f, helpEnabled);
                        }
                        m_tmpSpawner.m_critMaskFractal.Persistence = m_editorUtils.Slider("Persistence", m_tmpSpawner.m_critMaskFractal.Persistence, 0f, 1f, helpEnabled);
                        m_tmpSpawner.m_critMaskFractal.Lacunarity = m_editorUtils.Slider("Lacunarity", m_tmpSpawner.m_critMaskFractal.Lacunarity, 1.5f, 3.5f, helpEnabled);
                        m_tmpSpawner.m_critMaskFractalMidpoint = m_editorUtils.Slider("Midpoint", m_tmpSpawner.m_critMaskFractalMidpoint, 0f, 1f, helpEnabled);
                        m_tmpSpawner.m_critMaskFractalRange = m_editorUtils.Slider("Range", m_tmpSpawner.m_critMaskFractalRange, 0f, 1f, helpEnabled);
                        m_tmpSpawner.m_critMaskInvert = m_editorUtils.Toggle("Invert Mask", m_tmpSpawner.m_critMaskInvert, helpEnabled);
                    }
                    else
                    {
                        m_tmpSpawner.m_critMaskImage = (Texture2D)m_editorUtils.ObjectField("Image Mask", m_tmpSpawner.m_critMaskImage, typeof(Texture2D), false, helpEnabled);
                    }
                    EditorGUI.indentLevel--;
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                // Update the overrides to ensure that the prototypes have the new spawn criteria values (unless overriden ov course)
                m_needsSpawnCritOverrideUpdate = true;
            }
        }

        /// <summary>
        /// Spawn Prototypes Panel
        /// </summary>
        /// <param name="helpEnabled"></param>
        private void PrototypesPanel(bool helpEnabled)
        {
            m_editorUtils.InlineHelp("Spawn Prototypes", helpEnabled);
            GUILayout.BeginHorizontal();
            {
                m_editorUtils.LabelField("Spawn Proto Panel Intro");

                GUILayout.FlexibleSpace();
                int selection = 0;
                selection = m_editorUtils.Popup(selection, new string[] { "Conform Dropdown", "Conform All", "Conform None" }, GUILayout.Width(65f));
                if(selection != 0)
                {
                    switch (selection)
                    {
                        case 1:
                            m_tmpSpawner.SetAllProtoConformSlope(true);
                            break;
                        case 2:
                            m_tmpSpawner.SetAllProtoConformSlope(false);
                            break;
                        default:
                            throw new NotImplementedException("[GeNa] No idea what was selected here: " + selection);
                    }
                    GUI.changed = true;
                }
                m_editorUtils.ToggleButtonNonLocalized(" A-Z↓", ref m_tmpSpawner.m_sortPrototypes, GUILayout.Height(15f));
            }
            GUILayout.EndHorizontal();
            m_editorUtils.InlineHelp("Conform Dropdown", helpEnabled);
            ProtoPanel(m_tmpSpawner.m_spawnPrototypes, helpEnabled);
        }

        /// <summary>
        /// Draws a Prototypes Panel for a list of Prototypes
        /// </summary>
        /// <param name="helpEnabled"></param>
        private void ProtoPanel(List<Prototype> prototypes, bool helpEnabled)
        {
            Rect protoPanelWidthRect = GUILayoutUtility.GetLastRect();

            for (int protoIdx = 0; protoIdx < prototypes.Count; protoIdx++)
            {
                GUILayout.BeginVertical(SpawnerEditor.styles.gpanel);
                {
                    Prototype proto = prototypes[protoIdx];
                    string protoName = " <b>" + proto.m_name;
                    switch (proto.m_resourceType)
                    {
                        case Constants.ResourceType.Prefab:
                            string typeCode = " (P)";
                            if (proto.m_resourceTree[0] != null && proto.m_resourceTree[0].ContainerOnly)
                            {
                                typeCode = " (G)";
                            }
                            protoName += typeCode;
                            if (proto.m_resourceTree.Count == 1)
                            {
                                if (proto.m_resourceTree[0].m_conformToSlope)
                                {
                                    protoName += " *C*";
                                }
                            }
                            break;
                        case Constants.ResourceType.TerrainTree:
                            protoName += " (T)";
                            break;
                        case Constants.ResourceType.TerrainGrass:
                            protoName += " (G)";
                            break;
                        case Constants.ResourceType.TerrainTexture:
                            protoName += " (Tx)";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    if (proto.m_active != true)
                    {
                        protoName += " [inactive]";
                    }
                    else
                    {
                        protoName += string.Format(" {0:0}% [{1}]", proto.GetSuccessChance() * 100f, proto.m_instancesSpawned);
                    }
                    GUIStyle protoLabelStyle = styles.richLabel;
                    if (proto.HasLinkedSpawner)
                    {
                        protoName += " <size=14>" + LINKED_ICON + "</size>";
                        protoLabelStyle = styles.linkLabel;
                    }
                    if (proto.HasExtensions)
                    {
                        protoName += " " + GX_ICON;
                    }
                    protoName += "</b>";

                    //Let's check we are just changing the active state now
                    bool active = proto.m_active;
                    GUILayout.BeginHorizontal();
                    {
                        proto.m_active = EditorGUILayout.Toggle(proto.m_active, GUILayout.Width(10f));
                        if (GUILayout.Button(protoName, protoLabelStyle))
                        {
                            proto.m_active = !proto.m_active;
                        }

                        //Prep the rect if any ico needs to be drawn
                        Rect r = GUILayoutUtility.GetLastRect();
                        r = new Rect(r.xMax + 2f, r.yMin - 1f, 18f, 18f);

                        if (proto.m_anyCritOverrideApplies)
                        {
                            if (m_overridesIco != null)
                            {
                                GUI.DrawTexture(r, m_overridesIco);
                                //Add to rect in case another ico needs drawing after this
                                r.x += 20f;
                            }
                            else
                            {
                                Debug.LogWarningFormat("[GeNa] Missing overrides icon.");
                            }
                        }

                        //if (proto.m_hasLinkedSpawner)
                        //{
                        //    if (m_ChildSpawnerIco != null)
                        //    {
                        //        r.width = r.height = 16f;
                        //        r.y += 2f;
                        //        GUI.DrawTexture(r, m_ChildSpawnerIco);
                        //        //Add to rect in case another ico needs drawing after this
                        //        r.x += 20f;
                        //    }
                        //    else
                        //    {
                        //        Debug.LogWarningFormat("[GeNa] Missing Proto Child icon.");
                        //    }
                        //}

                        GUILayout.FlexibleSpace();
                        m_editorUtils.ToggleButton("Advanced Toggle", ref proto.m_showAdvancedOptions, styles.advancedToggle, styles.advancedToggleDown);

                        //GUILayout.Space(10f);
                        if (m_editorUtils.DeleteButton())
                        {
                            if (EditorUtility.DisplayDialog("WARNING!", string.Format("Are you sure you want to delete the prototype [{0}]?", proto.m_name), "OK", "Cancel"))
                            {
                                proto.Delete();
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2f);

                    //In here we can process different things if the active state changed.
                    if (active != proto.m_active)
                    {
                        //Update the spaqwners HasActiveTerrainProto property if this is a terrain proto
                        if (proto.m_resourceType > Constants.ResourceType.Prefab)
                        {
                            m_tmpSpawner.UpdateHasActiveTerrainProto();
                        }
                    }

                    //Display the the proto properties if active
                    if (proto.m_active == true)
                    {
                        EditorGUI.indentLevel++;

                        //Only show help for static switches if they are actually drawn
                        bool staticSwitchesDrawn = false;
                        GUILayout.BeginHorizontal((proto.m_resourceTree.Count < 1 || proto.m_resourceTree[0].Static != Constants.ResourceStatic.Dynamic) ? styles.staticResHeader : styles.dynamicResHeader);
                        {
                            proto.m_displayedInEditor = m_editorUtils.Foldout(proto.m_displayedInEditor, "Details Foldout");

                            GUILayout.FlexibleSpace();
                            // Add the static switch if not POI
                            if (proto.m_resourceTree.Count == 1)
                            {
                                staticSwitchesDrawn = StaticSwitch(proto.m_resourceTree[0]);
                            }
                        }
                        GUILayout.EndHorizontal();

                        // Adding help here because it should not be in the horizontal area. This way we also have the help only once to avoid cluttering the GUI.
                        if (staticSwitchesDrawn)
                        {
                            m_editorUtils.InlineHelp(Enum.GetNames(typeof(Constants.ResourceStatic)), helpEnabled);
                        }
                        GUILayout.Space(3f);

                        if (proto.m_displayedInEditor == true)
                        {
                            if (proto.m_showAdvancedOptions)
                            {
                                ProtoSpawnCritOverrides(proto);
                            }

                            if (m_tmpSpawner.m_critCheckMask && m_tmpSpawner.m_critMaskType == Constants.MaskType.Image)
                            {
                                m_editorUtils.LabelField("Mask Settings:");
                                EditorGUI.indentLevel++;
                                proto.m_imageFilterColour = m_editorUtils.ColorField("Selection Color", proto.m_imageFilterColour, helpEnabled);
                                proto.m_imageFilterFuzzyMatch = m_editorUtils.Slider("Selection Accuracy", proto.m_imageFilterFuzzyMatch, 0f, 1f, helpEnabled);
                                proto.m_constrainWithinMaskedBounds = m_editorUtils.Toggle("Fit Within Mask", proto.m_constrainWithinMaskedBounds, helpEnabled);
                                proto.m_invertMaskedAlpha = m_editorUtils.Toggle("Invert Alpha", proto.m_invertMaskedAlpha, helpEnabled);
                                proto.m_successOnMaskedAlpha = m_editorUtils.Toggle("Success By Alpha", proto.m_successOnMaskedAlpha, helpEnabled);
                                proto.m_scaleOnMaskedAlpha = m_editorUtils.Toggle("Scale By Alpha", proto.m_scaleOnMaskedAlpha, helpEnabled);
                                if (proto.m_scaleOnMaskedAlpha)
                                {
                                    EditorGUI.indentLevel++;
                                    proto.m_scaleOnMaskedAlphaMin = m_editorUtils.Slider("Mask Alpha Min Scale", proto.m_scaleOnMaskedAlphaMin, 0f, 10f, helpEnabled);
                                    proto.m_scaleOnMaskedAlphaMax = m_editorUtils.Slider("Mask Alpha Max Scale", proto.m_scaleOnMaskedAlphaMax, 0f, 10f, helpEnabled);
                                    EditorGUI.indentLevel--;
                                }
                                EditorGUI.indentLevel--;
                            }

                            if (proto.m_resourceType == Constants.ResourceType.Prefab)
                            {
                                proto.m_forwardRotation = m_editorUtils.Slider("Forward Rotation", proto.m_forwardRotation, -360f, 360f, helpEnabled);
                            }

                            if (proto.m_resourceTree.Count == 1)
                            {
                                Separator(protoPanelWidthRect);
                                EditResource(proto, proto.m_resourceTree[0], false, helpEnabled, proto.m_showAdvancedOptions);
                            }
                            else
                            {
                                proto.m_name = m_editorUtils.TextField("Proto Name", proto.m_name, helpEnabled);
                                m_editorUtils.LabelField("Proto Size", new GUIContent(string.Format("{0:0.00}, {1:0.00}, {2:0.00}", proto.m_size.x, proto.m_size.y, proto.m_size.z)), helpEnabled);

                                Separator(protoPanelWidthRect);

                                for (int resIdx = 0; resIdx < proto.m_resourceTree.Count; resIdx++)
                                {
                                    Resource res = proto.m_resourceTree[resIdx];

                                    string resName = res.m_name;
                                    if (res.m_conformToSlope == true)
                                    {
                                        resName += " *C*";
                                    }

                                    GUILayout.BeginHorizontal((res.Static == Constants.ResourceStatic.Dynamic) ? styles.dynamicResHeader : styles.staticResHeader);
                                    {
                                        res.m_displayedInEditor = EditorGUILayout.Foldout(res.m_displayedInEditor, resName, true);
                                        GUILayout.FlexibleSpace();
                                        StaticSwitch(res);
                                    }
                                    GUILayout.EndHorizontal();

                                    if (res.m_displayedInEditor == true)
                                    {
                                        EditResource(proto, res, true, helpEnabled, proto.m_showAdvancedOptions);
                                    }
                                }
                            }

                            GUILayout.BeginHorizontal();

                            GUILayout.Space(20);

                            //if (proto.m_resourceType == Constants.ResourceType.TerrainGrass)
                            //{
                            //    if (m_editorUtils.Button("Delete Instances", helpEnabled))
                            //    {
                            //        if (EditorUtility.DisplayDialog("WARNING!", "Are you sure you want to delete all instances of the grass [" + proto.m_name + "] from your scene?\n\n" +
                            //            "WARNING! This is destructive and non-recoverable. This deletes ALL of these details and not only those which were spawned by GeNa.\n\n" +
                            //            "NOTE: This will also clear the Undo History.", "OK", "Cancel"))
                            //        {
                            //            m_spawner.UnspawnGrass(protoIdx);
                            //            SpawnerToCache();
                            //        }
                            //    }
                            //}

                            //if (proto.m_resourceType == Constants.ResourceType.TerrainTree)
                            //{
                            //    if (m_editorUtils.Button("Delete Instances", helpEnabled))
                            //    {
                            //        if (EditorUtility.DisplayDialog("WARNING!", "Are you sure you want to delete all instances of the tree [" + proto.m_name + "] from your scene?\n\n" +
                            //            "WARNING! This is destructive and non-recoverable. This deletes ALL of these trees and not only those which were spawned by GeNa.\n\n" +
                            //            "NOTE: This will also clear the Undo History.", "OK", "Cancel"))
                            //        {
                            //            m_spawner.UnspawnTree(protoIdx);
                            //            SpawnerToCache();
                            //        }
                            //    }
                            //}

                            if (proto.m_resourceType == Constants.ResourceType.Prefab)
                            {
                                if (m_editorUtils.Button("Delete Instances", helpEnabled))
                                {
                                    if (EditorUtility.DisplayDialog("WARNING!", "Are you sure you want to delete all instances of [" + proto.m_name + "] prefabs from your scene?\n\n" +
                                        "NOTE: This will also clear the Undo History.", "OK", "Cancel"))
                                    {
                                        m_spawner.UnspawnGameObject(protoIdx);
                                        SpawnerToCache();
                                    }
                                }
                            }

                            GUILayout.EndHorizontal();
                        }

                        EditorGUI.indentLevel--;
                    }

                    GUILayout.Space(3);
                }
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }
        }

        /// <summary>
        /// Options to Edit the Spawn Criteria Overrides of a prototype
        /// </summary>
        /// <param name="proto"></param>
        private void ProtoSpawnCritOverrides(Prototype proto)
        {
            GUILayout.BeginVertical(SpawnerEditor.styles.gpanel);
            {
                if (proto.m_anyCritOverrideApplies)
                {
                    GUILayout.BeginHorizontal();
                    {
                        proto.ShowSpawnCriteriaOverrides = m_editorUtils.Foldout(proto.ShowSpawnCriteriaOverrides, "Proto SpCrit Overrides Label");

                        if (m_overridesIco != null)
                        {
                            Rect r = GUILayoutUtility.GetLastRect();
                            GUI.DrawTexture(new Rect(r.x + EditorStyles.foldout.CalcSize(m_editorUtils.GetContent("Proto SpCrit Overrides Label")).x + 3f, r.y - 1f, 18f, 18f), m_overridesIco);
                        }
                        else
                        {
                            Debug.LogWarningFormat("[GeNa] Missing overrides icon.");
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    proto.ShowSpawnCriteriaOverrides = m_editorUtils.Foldout(proto.ShowSpawnCriteriaOverrides, "Proto SpCrit Overrides Label");
                }

                if (proto.ShowSpawnCriteriaOverrides)
                {
                    if (BeginControlToggle(ref proto.m_overrideCritVirginCheckType))
                    {
                        proto.m_critVirginCheckType = m_tmpSpawner.m_critVirginCheckType;
                    }
                    {
                        proto.m_critVirginCheckType = (Constants.VirginCheckType)m_editorUtils.EnumPopup("Check Collisions", proto.m_critVirginCheckType);
                    }
                    EndControlToggle();
                    if (proto.m_critVirginCheckType != Constants.VirginCheckType.None && proto.m_resourceType < Constants.ResourceType.TerrainGrass)
                    {
                        if (proto.m_critVirginCheckType == Constants.VirginCheckType.Bounds)
                        {
                            EditorGUI.indentLevel += 1;
                            if (BeginControlToggle(ref proto.m_overrideCritBoundsBorder))
                            {
                                proto.m_critBoundsBorder = m_tmpSpawner.m_critBoundsBorder;
                            }
                            {
                                proto.m_critBoundsBorder = m_editorUtils.Slider("Modify Bounds", proto.m_critBoundsBorder, -0.9f, +5f);
                            }
                            EndControlToggle();
                            EditorGUI.indentLevel -= 1;
                        }
                        if (BeginControlToggle(ref proto.m_overrideCritSpawnCollisionLayers))
                        {
                            proto.m_critSpawnCollisionLayers = m_tmpSpawner.m_critSpawnCollisionLayers;
                        }
                        {
                            proto.m_critSpawnCollisionLayers = LayerMaskField(m_editorUtils.GetContent("Collision Layers"), proto.m_critSpawnCollisionLayers);
                        }
                        EndControlToggle();
                    }

                    //Check Height
                    if (BeginControlToggle(ref proto.m_overrideCritCheckHeight))
                    {
                        proto.m_critCheckHeightType = m_tmpSpawner.m_critCheckHeightType;
                    }
                    {
                        proto.m_critCheckHeightType = (Constants.CriteriaRangeType)m_editorUtils.EnumPopup("Check Height Type", proto.m_critCheckHeightType);
                    }
                    EndControlToggle();
                    if (proto.m_critCheckHeightType != Constants.CriteriaRangeType.None)
                    {
                        EditorGUI.indentLevel++;
                        if (m_tmpSpawner.m_advUseLargeRanges)
                        {
                            if (proto.m_critCheckHeightType >= Constants.CriteriaRangeType.MinMax)
                            {
                                if (BeginControlToggle(ref proto.m_overrideCritMinSpawnHeight))
                                {
                                    proto.m_critMinSpawnHeight = m_tmpSpawner.m_critMinSpawnHeight;
                                }
                                {
                                    proto.m_critMinSpawnHeight = m_editorUtils.FloatField("Min Height", proto.m_critMinSpawnHeight);
                                }
                                EndControlToggle();
                                if (BeginControlToggle(ref proto.m_overrideCritMaxSpawnHeight))
                                {
                                    proto.m_critMaxSpawnHeight = m_tmpSpawner.m_critMaxSpawnHeight;
                                }
                                {
                                    proto.m_critMaxSpawnHeight = m_editorUtils.FloatField("Max Height", proto.m_critMaxSpawnHeight);
                                }
                                EndControlToggle();
                            }

                            switch (proto.m_critCheckHeightType)
                            {
                                case Constants.CriteriaRangeType.Range:
                                case Constants.CriteriaRangeType.Mixed:
                                    if (BeginControlToggle(ref proto.m_overrideCritHeightVariance))
                                    {
                                        proto.m_critHeightVariance = m_tmpSpawner.m_critHeightVariance;
                                    }
                                    {
                                        proto.m_critHeightVariance = m_editorUtils.FloatField("Height Range", proto.m_critHeightVariance);
                                    }
                                    EndControlToggle();
                                    break;
                            }
                        }
                        else
                        {
                            if (proto.m_critCheckHeightType >= Constants.CriteriaRangeType.MinMax)
                            {
                                if (BeginControlToggle(ref proto.m_overrideCritMinSpawnHeight))
                                {
                                    proto.m_critMinSpawnHeight = m_tmpSpawner.m_critMinSpawnHeight;
                                }
                                {
                                    proto.m_critMinSpawnHeight = m_editorUtils.Slider("Min Height", proto.m_critMinSpawnHeight, m_spawner.m_bottomBoundary, m_spawner.m_topBoundary);
                                }
                                EndControlToggle();
                                if (BeginControlToggle(ref proto.m_overrideCritMaxSpawnHeight))
                                {
                                    proto.m_critMaxSpawnHeight = m_tmpSpawner.m_critMaxSpawnHeight;
                                }
                                {
                                    proto.m_critMaxSpawnHeight = m_editorUtils.Slider("Max Height", proto.m_critMaxSpawnHeight, m_spawner.m_bottomBoundary, m_spawner.m_topBoundary);
                                }
                                EndControlToggle();
                            }

                            switch (proto.m_critCheckHeightType)
                            {
                                case Constants.CriteriaRangeType.Range:
                                case Constants.CriteriaRangeType.Mixed:
                                    if (BeginControlToggle(ref proto.m_overrideCritHeightVariance))
                                    {
                                        proto.m_critHeightVariance = m_tmpSpawner.m_critHeightVariance;
                                    }
                                    {
                                        proto.m_critHeightVariance = m_editorUtils.Slider("Height Range", proto.m_critHeightVariance, 0.1f, 200f);
                                    }
                                    EndControlToggle();
                                    break;
                            }
                        }
                        EditorGUI.indentLevel--;
                    }

                    //Check Slope
                    if (BeginControlToggle(ref proto.m_overrideCritCheckSlope))
                    {
                        proto.m_critCheckSlopeType = m_tmpSpawner.m_critCheckSlopeType;
                    }
                    {
                        proto.m_critCheckSlopeType = (Constants.CriteriaRangeType)m_editorUtils.EnumPopup("Check Slope Type", proto.m_critCheckSlopeType);
                    }
                    EndControlToggle();
                    if (proto.m_critCheckSlopeType != Constants.CriteriaRangeType.None)
                    {
                        EditorGUI.indentLevel++;
                        if (proto.m_critCheckSlopeType >= Constants.CriteriaRangeType.MinMax)
                        {
                            if (BeginControlToggle(ref proto.m_overrideCritMinSpawnSlope))
                            {
                                proto.MinSpawnSlope = m_tmpSpawner.MinSpawnSlope;
                            }
                            {
                                proto.MinSpawnSlope = m_editorUtils.Slider("Min Slope", proto.MinSpawnSlope, 0f, 90f);
                            }
                            EndControlToggle();
                            if (BeginControlToggle(ref proto.m_overrideCritMaxSpawnSlope))
                            {
                                proto.MaxSpawnSlope = m_tmpSpawner.MaxSpawnSlope;
                            }
                            {
                                proto.MaxSpawnSlope = m_editorUtils.Slider("Max Slope", proto.MaxSpawnSlope, 0f, 90f);
                            }
                            EndControlToggle();
                        }

                        switch (proto.m_critCheckSlopeType)
                        {
                            case Constants.CriteriaRangeType.Range:
                            case Constants.CriteriaRangeType.Mixed:
                                if (BeginControlToggle(ref proto.m_overrideCritSlopeVariance))
                                {
                                    proto.m_critSlopeVariance = m_tmpSpawner.m_critSlopeVariance;
                                }
                                {
                                    proto.m_critSlopeVariance = m_editorUtils.Slider("Slope Range", proto.m_critSlopeVariance, 0.1f, 180f);
                                }
                                EndControlToggle();
                                break;
                        }
                        EditorGUI.indentLevel--;
                    }

                    if (BeginControlToggle(ref proto.m_overrideCritCheckTextures))
                    {
                        proto.m_critCheckTextures = m_tmpSpawner.m_critCheckTextures;
                    }
                    {
                        proto.m_critCheckTextures = m_editorUtils.Toggle("Check Textures", proto.m_critCheckTextures);
                    }
                    EndControlToggle();
                    if (proto.m_critCheckTextures)
                    {
                        EditorGUI.indentLevel++;
                        m_editorUtils.LabelField("Texture Crit", new GUIContent(string.Format("({0}) {1}", m_tmpSpawner.m_critSelectedTextureIdx, m_spawner.m_critSelectedTextureName)));
                        if (BeginControlToggle(ref proto.m_overrideCritTextureStrength))
                        {
                            proto.m_critTextureStrength = m_tmpSpawner.m_critTextureStrength;
                        }
                        {
                            proto.m_critTextureStrength = m_editorUtils.Slider("Texture Strength", proto.m_critTextureStrength, 0.0f, 1f);
                        }
                        EndControlToggle();
                        if (BeginControlToggle(ref proto.m_overrideCritTextureVariance))
                        {
                            proto.m_critTextureVariance = m_tmpSpawner.m_critTextureVariance;
                        }
                        {
                            proto.m_critTextureVariance = m_editorUtils.Slider("Texture Range", proto.m_critTextureVariance, 0.0f, 1f);
                        }
                        EndControlToggle();
                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.BeginDisabledGroup(!m_tmpSpawner.m_critCheckMask);
                    {
                        EditorGUIUtility.labelWidth += 16f;
                        proto.m_disableCritCheckMask = EditorGUILayout.Toggle(m_editorUtils.GetContent("Proto Disable Check Mask"), proto.m_disableCritCheckMask);
                        EditorGUIUtility.labelWidth -= 16f;
                    }
                    EditorGUI.EndDisabledGroup();
                    if (proto.m_disableCritCheckMask ? false : m_tmpSpawner.m_critCheckMask)
                    {
                        EditorGUI.indentLevel++;
                        if (m_tmpSpawner.m_critMaskType != Constants.MaskType.Image)
                        {
                            if (BeginControlToggle(ref proto.m_overrideCritMaskInvert))
                            {
                                proto.m_critMaskInvert = m_tmpSpawner.m_critMaskInvert;
                            }
                            {
                                proto.m_critMaskInvert = m_editorUtils.Toggle("Invert Mask", proto.m_critMaskInvert);
                            }
                            EndControlToggle();
                        }
                        EditorGUI.indentLevel--;
                    }
                }

                GUILayout.Space(3);
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Edit the selected resource
        /// </summary>
        /// <param name="proto"></param>
        /// <param name="res"></param>
        /// <param name="child"></param>
        private void EditResource(Prototype proto, Resource res, bool child, bool helpEnabled, bool advanced)
        {
            GUILayout.Space(3f);
            if (child == true)
            {
                EditorGUI.indentLevel++;
            }

            res.m_name = m_editorUtils.TextField("Resource Name", res.m_name, helpEnabled);

            if (res.Static > Constants.ResourceStatic.Static)
            {
                res.m_successRate = 0.01f * m_editorUtils.Slider("Res Success", res.m_successRate * 100f, 0f, 100f, helpEnabled);
            }

            switch (res.m_resourceType)
            {
                case Constants.ResourceType.Prefab:
                    //AVS
                    if (m_tmpSpawner.m_spawnToAVS)
                    {
                        res.m_avsVegetationType = (Constants.AVSVegetationType)m_editorUtils.EnumPopup("AVS Type", res.m_avsVegetationType, helpEnabled);
                    }

                    if (res.Static > Constants.ResourceStatic.Static)
                    {
                        if (!child)
                        {
                            res.m_conformToSlope = m_editorUtils.Toggle("Conform Slope", res.m_conformToSlope, helpEnabled);
                        }

                        if (advanced)
                        {
                            PrefabField(res, helpEnabled);

                            res.m_minRotation = m_editorUtils.Vector3Field("Min Rotation Offset", res.m_minRotation, helpEnabled);
                            res.m_maxRotation = m_editorUtils.Vector3Field("Max Rotation Offset", res.m_maxRotation, helpEnabled);
                        }
                        else
                        {
                            m_editorUtils.MinMaxSliderWithFields("Y Rotation Offset", ref res.m_minRotation.y, ref res.m_maxRotation.y, 0f, 360f, helpEnabled);
                        }
                    }
                    else
                    {
                        PrefabField(res, helpEnabled);
                        m_editorUtils.LabelField("Static Position Offset", new GUIContent(string.Format("{0:0.00}, {1:0.00}, {2:0.00}", res.m_basePosition.x, res.m_basePosition.y, res.m_basePosition.z)));
                        m_editorUtils.LabelField("Static Rotation Offset", new GUIContent(string.Format("{0:0.00}, {1:0.00}, {2:0.00}", res.m_baseRotation.x, res.m_baseRotation.y, res.m_baseRotation.z)));
                        m_editorUtils.LabelField("Static Scale", new GUIContent(string.Format("{0:0.00}, {1:0.00}, {2:0.00}", res.m_baseScale.x, res.m_baseScale.y, res.m_baseScale.z)));
                    }

                    if (advanced)
                    {
                        if (res.Static > Constants.ResourceStatic.Static)
                        {
                            res.m_minOffset = m_editorUtils.Vector3Field("Min Position Offset", res.m_minOffset, helpEnabled);
                            res.m_maxOffset = m_editorUtils.Vector3Field("Max Position Offset", res.m_maxOffset, helpEnabled);

                            res.m_sameScale = m_editorUtils.Toggle("Same O Scale", res.m_sameScale, helpEnabled);
                            if (res.m_sameScale)
                            {
                                m_editorUtils.MinMaxSliderWithFields("Res Scale", ref res.m_minScale.x, ref res.m_maxScale.x, 0.1f, 100f, helpEnabled);
                            }
                            else
                            {
                                res.m_minScale = m_editorUtils.Vector3Field("Res Min Scale", res.m_minScale, helpEnabled);
                                res.m_maxScale = m_editorUtils.Vector3Field("Res Max Scale", res.m_maxScale, helpEnabled);
                            }
                        }

                        res.m_baseColliderUseConstScale = m_editorUtils.Toggle("Same C Scale", res.m_baseColliderUseConstScale, helpEnabled);
                        if (res.m_baseColliderUseConstScale)
                        {
                            res.m_baseColliderConstScaleAmount = m_editorUtils.Slider("Collider Scale", res.m_baseColliderConstScaleAmount, 0.25f, 2f, helpEnabled);
                        }
                        else
                        {
                            res.m_baseColliderScale = m_editorUtils.Vector3Field("Collider Scale", res.m_baseColliderScale, helpEnabled);
                        }

                        // Need a bunch of weird stuff here
                        int indent = EditorGUI.indentLevel;
                        EditorGUI.indentLevel = 0;
                        float labelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth -= 5f + 10f * indent;
                        float fieldsWidth = EditorGUIUtility.labelWidth;
                        float indentWidth = 12f * indent;

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(indentWidth + 12f);

                            GUILayout.BeginVertical(SpawnerEditor.styles.resFlagsPanel, GUILayout.Width(fieldsWidth));
                            {
                                res.m_flagBatchingStatic = m_editorUtils.Toggle("Static Batching", res.m_flagBatchingStatic, helpEnabled, GUILayout.Width(fieldsWidth));
                                res.m_flagLightmapStatic = m_editorUtils.Toggle("Static Lightmap", res.m_flagLightmapStatic, helpEnabled, GUILayout.Width(fieldsWidth));
                                res.m_flagNavigationStatic = m_editorUtils.Toggle("Static Navigation", res.m_flagNavigationStatic, helpEnabled, GUILayout.Width(fieldsWidth));
                                res.m_flagOccludeeStatic = m_editorUtils.Toggle("Static Occludee", res.m_flagOccludeeStatic, helpEnabled, GUILayout.Width(fieldsWidth));
                                res.m_flagOccluderStatic = m_editorUtils.Toggle("Static Occluder", res.m_flagOccluderStatic, helpEnabled, GUILayout.Width(fieldsWidth));
                                res.m_flagOffMeshLinkGeneration = m_editorUtils.Toggle("Offmesh Link Gen", res.m_flagOffMeshLinkGeneration, helpEnabled, GUILayout.Width(fieldsWidth));
                            }
                            GUILayout.EndVertical();

                            GUILayout.Space(20f);
                            GUILayout.BeginVertical(SpawnerEditor.styles.resFlagsPanel, GUILayout.Width(fieldsWidth));
                            {
                                res.m_flagReflectionProbeStatic = m_editorUtils.Toggle("Static Ref Probe", res.m_flagReflectionProbeStatic, helpEnabled, GUILayout.Width(fieldsWidth));
                                res.m_flagMovingObject = m_editorUtils.Toggle("Moving Object", res.m_flagMovingObject, helpEnabled, GUILayout.Width(fieldsWidth));
                                res.m_flagIsOutdoorObject = m_editorUtils.Toggle("Outdoor Object", res.m_flagIsOutdoorObject, helpEnabled, GUILayout.Width(fieldsWidth));
                                res.m_flagForceOptimise = m_editorUtils.Toggle("Force Optimise", res.m_flagForceOptimise, helpEnabled, GUILayout.Width(fieldsWidth));
                                res.m_flagCanBeOptimised = m_editorUtils.Toggle("Can Optimise", res.m_flagCanBeOptimised, helpEnabled, GUILayout.Width(fieldsWidth));
                                EditorGUI.BeginDisabledGroup(res.m_hasColliders && res.Static == Constants.ResourceStatic.Dynamic);
                                {
                                    res.m_useColliderBounds = m_editorUtils.Toggle("Use Collider Bounds", res.m_useColliderBounds, helpEnabled, GUILayout.Width(fieldsWidth));
                                }
                                EditorGUI.EndDisabledGroup();
                                if (res.m_flagForceOptimise == true)
                                {
                                    res.m_flagCanBeOptimised = true;
                                }
                            }
                            GUILayout.EndVertical();
                            GUILayout.Space(-40f);
                        }
                        GUILayout.EndHorizontal();
                        EditorGUIUtility.labelWidth = labelWidth;
                        EditorGUI.indentLevel = indent;
                    }
                    // if not advanced mode and not static, Height Offset only if not 
                    else if (res.Static > Constants.ResourceStatic.Static)
                    {
                        if (m_tmpSpawner.m_advUseLargeRanges)
                        {
							res.m_minOffset.y = res.m_maxOffset.y = m_editorUtils.FloatField("Height Offset", res.m_maxOffset.y, helpEnabled);
                        }
                        else
                        {
                            // Add a range that's minimum 2f and considers the base position
                            float height = Mathf.Max(2f, Mathf.Abs(res.m_basePosition.y), Mathf.Abs(res.m_baseSize.y * res.m_baseScale.y));

                            if (res.m_sameScale)
                            {
                                height *= res.m_maxScale.x;
                            }
                            else
                            {
                                height *= res.m_maxScale.y;
                            }

                            if (m_tmpSpawner.m_sameScale)
                            {
                                height *= m_tmpSpawner.m_maxScaleX;
                            }
                            else
                            {
                                height *= m_tmpSpawner.m_maxScaleY;
                            }

							res.m_minOffset.y = res.m_maxOffset.y = m_editorUtils.Slider("Height Offset", res.m_maxOffset.y, -height, height * 1.2f, helpEnabled);
						}
                    }

                    m_editorUtils.LabelField("Base Size", new GUIContent(string.Format("{0:0.00}, {1:0.00}, {2:0.00}", res.m_baseSize.x, res.m_baseSize.y, res.m_baseSize.z)), helpEnabled);
                    if (res.Static >= Constants.ResourceStatic.Dynamic)
                    {
                        m_editorUtils.LabelField("Base Scale", new GUIContent(string.Format("{0:0.00}, {1:0.00}, {2:0.00}", res.m_baseScale.x, res.m_baseScale.y, res.m_baseScale.z)), helpEnabled);
                    }
                    m_editorUtils.LabelField("Res Spawned", new GUIContent(string.Format("{0}", res.m_instancesSpawned)), helpEnabled);
                    break;

                case Constants.ResourceType.TerrainGrass:
                    Terrain terrain = Terrain.activeTerrain;
                    if (terrain != null)
                    {
                        GUIContent[] assetChoices = new GUIContent[terrain.terrainData.detailPrototypes.Length];
                        DetailPrototype detailProto;
                        for (int assetIdx = 0; assetIdx < assetChoices.Length; assetIdx++)
                        {
                            detailProto = terrain.terrainData.detailPrototypes[assetIdx];
                            if (detailProto.prototypeTexture != null)
                            {
                                assetChoices[assetIdx] = new GUIContent(detailProto.prototypeTexture.name);
                            }
                            else if (detailProto.prototype != null)
                            {
                                assetChoices[assetIdx] = new GUIContent(detailProto.prototype.name);
                            }
                            else
                            {
                                assetChoices[assetIdx] = new GUIContent("Unknown asset");
                            }
                        }
                        int oldIdx = res.m_terrainProtoIdx;
                        res.m_terrainProtoIdx = m_editorUtils.Popup("Grass", res.m_terrainProtoIdx, assetChoices, helpEnabled);

                        res.m_sameScale = true;
                        m_editorUtils.MinMaxSliderWithFields("Grass Strength", ref res.m_minScale.x, ref res.m_maxScale.x, 0f, 1f, helpEnabled);

                        if (res.m_terrainProtoIdx != oldIdx)
                        {
                            detailProto = terrain.terrainData.detailPrototypes[res.m_terrainProtoIdx];
                            if (detailProto.prototypeTexture != null)
                            {
                                res.m_name = detailProto.prototypeTexture.name;
                                proto.m_name = res.m_name;
                            }
                            else if (detailProto.prototype != null)
                            {
                                res.m_name = detailProto.prototype.name;
                                proto.m_name = res.m_name;
                            }
                            else
                            {
                                res.m_name = "Unknown asset";
                                proto.m_name = res.m_name;
                            }
                            res.m_baseSize = new Vector3(detailProto.minWidth, detailProto.minHeight, detailProto.minWidth);
                            proto.m_size = res.m_baseSize;
                            proto.m_extents = res.m_baseSize * 0.5f;
                        }
                        m_editorUtils.LabelField("Base Size", new GUIContent(string.Format("{0:0.00}, {1:0.00}, {2:0.00}", res.m_baseSize.x, res.m_baseSize.y, res.m_baseSize.z)), helpEnabled);
                        //m_editorUtils.LabelField(GetLabel("Spawned"), GetLabel(string.Format("{0}", res.m_instancesSpawned)), helpEnabled);
                    }
                    break;

                case Constants.ResourceType.TerrainTree:
                    terrain = Terrain.activeTerrain;
                    if (terrain != null)
                    {
                        GUIContent[] assetChoices = new GUIContent[terrain.terrainData.treePrototypes.Length];
                        TreePrototype treeProto;
                        for (int assetIdx = 0; assetIdx < assetChoices.Length; assetIdx++)
                        {
                            treeProto = terrain.terrainData.treePrototypes[assetIdx];
                            if (treeProto.prefab != null)
                            {
                                assetChoices[assetIdx] = new GUIContent(treeProto.prefab.name);
                            }
                            else
                            {
                                assetChoices[assetIdx] = new GUIContent("Unknown asset");
                            }
                        }
                        int oldIdx = res.m_terrainProtoIdx;
                        res.m_terrainProtoIdx = m_editorUtils.Popup("Tree", res.m_terrainProtoIdx, assetChoices, helpEnabled);
                        if (res.m_terrainProtoIdx != oldIdx)
                        {
                            treeProto = terrain.terrainData.treePrototypes[res.m_terrainProtoIdx];
                            if (treeProto.prefab != null)
                            {
                                res.m_name = treeProto.prefab.name;
                                proto.m_name = res.m_name;
                                res.m_baseSize = Spawner.GetInstantiatedBounds(treeProto.prefab).size;
                                res.m_baseScale = treeProto.prefab.transform.localScale;
                            }
                            else
                            {
                                res.m_name = "Unknown asset";
                                proto.m_name = res.m_name;
                            }
                            res.m_minScale = res.m_baseScale;
                            res.m_maxScale = res.m_baseScale;
                            proto.m_size = res.m_baseSize;
                            proto.m_extents = res.m_baseSize * 0.5f;
                        }
                        m_editorUtils.MinMaxSliderWithFields("Y Rotation Offset", ref res.m_minRotation.y, ref res.m_maxRotation.y, 0f, 360f, helpEnabled);
                        if (advanced)
                        {
                            res.m_sameScale = m_editorUtils.Toggle("Same O Scale", res.m_sameScale, helpEnabled);
                            if (res.m_sameScale)
                            {
                                m_editorUtils.MinMaxSliderWithFields("Res Scale", ref res.m_minScale.x, ref res.m_maxScale.x, 0.1f, 100f, helpEnabled);
                            }
                            else
                            {
                                res.m_minScale = m_editorUtils.Vector3Field("Res Min Scale", res.m_minScale, helpEnabled);
                                res.m_maxScale = m_editorUtils.Vector3Field("Res Max Scale", res.m_maxScale, helpEnabled);
                            }
                        }

                        m_editorUtils.LabelField("Base Size", new GUIContent(string.Format("{0:0.00}, {1:0.00}, {2:0.00}", res.m_baseSize.x, res.m_baseSize.y, res.m_baseSize.z)), helpEnabled);
                        m_editorUtils.LabelField("Base Scale", new GUIContent(string.Format("{0:0.00}, {1:0.00}, {2:0.00}", res.m_baseScale.x, res.m_baseScale.y, res.m_baseScale.z)), helpEnabled);
                        //m_editorUtils.LabelField(GetLabel("Spawned"), GetLabel(string.Format("{0}", res.m_instancesSpawned)), helpEnabled);
                    }
                    break;

                case Constants.ResourceType.TerrainTexture:
                    terrain = Terrain.activeTerrain;
                    if (terrain != null)
                    {
                        GUIContent[] assetChoices = new GUIContent[terrain.terrainData.alphamapLayers];
                        for (int assetIdx = 0; assetIdx < assetChoices.Length; assetIdx++)
                        {
#if UNITY_2018_3_OR_NEWER
                            assetChoices[assetIdx] = new GUIContent(terrain.terrainData.terrainLayers[assetIdx].diffuseTexture.name);
#else
                            assetChoices[assetIdx] = new GUIContent(terrain.terrainData.splatPrototypes[assetIdx].texture.name);
#endif
                        }
                        int oldIdx = res.m_terrainProtoIdx;
                        res.m_terrainProtoIdx = m_editorUtils.Popup("Texture", res.m_terrainProtoIdx, assetChoices, helpEnabled);

                        if (res.m_terrainProtoIdx != oldIdx)
                        {
#if UNITY_2018_3_OR_NEWER
                            res.m_name = terrain.terrainData.terrainLayers[res.m_terrainProtoIdx].diffuseTexture.name;
#else
                            res.m_name = terrain.terrainData.splatPrototypes[res.m_terrainProtoIdx].texture.name;
#endif
                            proto.m_name = res.m_name;
                        }

                        // Shape
                        bool doubleClick;
                        res.BrushIndex = m_editorUtils.BrushSelectionGrid("TxBrush Shape", res.BrushIndex, out doubleClick, res.BrushTextures, res.AddBrushTexture, res.RemoveBrushTexture, res.ClearBrushTextures, helpEnabled);

                        res.m_sameScale = true;

                        //Calculate the minimum and a maximum 100 final scale
                        float splatPixelSize = terrain.terrainData.size.x / terrain.terrainData.alphamapResolution;
                        int lowerScaleLimit = Mathf.CeilToInt((Constants.MIN_TX_BRUSH_SIZE_IN_PIX * splatPixelSize) / (m_tmpSpawner.m_sameScale ? m_tmpSpawner.m_minScaleX : 0.5f * (m_tmpSpawner.m_minScaleX + m_tmpSpawner.m_minScaleZ)));
                        int higherScaleLimit = Mathf.FloorToInt((100f * splatPixelSize) / (m_tmpSpawner.m_sameScale ? m_tmpSpawner.m_maxScaleX : 0.5f * (m_tmpSpawner.m_maxScaleX + m_tmpSpawner.m_maxScaleZ)));
                        int minScale = (int)res.m_minScale.x;
                        int maxScale = (int)res.m_maxScale.x;
                        m_editorUtils.MinMaxSliderWithFields("Texture Size", ref minScale, ref maxScale, lowerScaleLimit, higherScaleLimit, helpEnabled);
                        res.m_minScale.x = minScale;
                        res.m_maxScale.x = maxScale;

                        res.m_opacity = 0.01f * m_editorUtils.Slider("Opacity", res.m_opacity * 100f, 0, 100f, helpEnabled);
                        res.m_targetStrength = m_editorUtils.Slider("Target Strength", res.m_targetStrength, 0, 1f, helpEnabled);

                        //m_editorUtils.LabelField(GetLabel("Base Size"), GetLabel(string.Format("{0:0.00}, {1:0.00}, {2:0.00}", res.m_baseSize.x, res.m_baseSize.y, res.m_baseSize.z, helpEnabled)));
                    }
                    break;
                default:
                    throw new NotImplementedException("Not sure what to do with ResourceType '" + res.m_resourceType + "'");
            }

            // Child Proto controls
            if (advanced || res.HasLinkedSpawners)
            {
                LinkedSpawnersControl(proto, res, helpEnabled);
            }

            // Child Proto controls
            if (advanced || res.HasExtensions)
            {
                ExtensionsControl(proto, res, helpEnabled);
            }

            // Keep traversing down the tree
            ChildResources(proto, res, helpEnabled, advanced);

            if (child == true)
            {
                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// Draws the Linked Spawners Controls for the Resource
        /// </summary>
        private void LinkedSpawnersControl(Prototype proto, Resource res, bool helpEnabled)
        {
            string linkIcon = "<b><size=14>" + LINKED_ICON + "</size></b>";
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(EditorGUI.indentLevel * 15f);

                GUILayout.BeginVertical(SpawnerEditor.styles.gpanel);
                {
                    List<GUIContent> spawnerSelectionList = new List<GUIContent>(new GUIContent[] { new GUIContent("-") });
                    for (int i = 0; i < m_tmpSpawner.ChildSpawners.Count; i++)
                    {
                        spawnerSelectionList.Add(new GUIContent(string.Format("[{0}] ({1})", i, m_tmpSpawner.ChildSpawners[i].m_parentName)));
                    }

                    //Need to remove the indent for the popup label to show correctly
                    int indent = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;
                    float lw = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 19f;

                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PrefixLabel(new GUIContent(string.Format("{0}", linkIcon), m_editorUtils.GetTooltip("Linked Spawners List")), EditorStyles.popup, styles.linkLabel);
                        int selectedSpawner = EditorGUILayout.Popup(0, spawnerSelectionList.ToArray());
                        if (selectedSpawner != 0)
                        {
                            res.LinkChildSpawner(selectedSpawner - 1);
                        }
                    }
                    GUILayout.EndHorizontal();

                    EditorGUIUtility.labelWidth = lw;
                    EditorGUI.indentLevel = indent;

                    if (res.HasLinkedSpawners)
                    {
                        for (int i = 0; i < res.LinkedSpawners.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                if (m_tmpSpawner.ChildSpawners.Count <= res.LinkedSpawners[i])
                                {
                                    // We no longer have enough child spawners
                                    m_tmpSpawner.UpdateChildSpawners();
                                }
                                GUILayout.Label(new GUIContent(string.Format("{0} <b>[{1}]</b> ({2})", linkIcon, res.LinkedSpawners[i], m_tmpSpawner.ChildSpawners[res.LinkedSpawners[i]].m_parentName)), SpawnerEditor.styles.richLabel);
                                GUILayout.FlexibleSpace();

                                //GUILayout.Space(10f);
                                if (m_editorUtils.DeleteButton())
                                {
                                    string dialogText = string.Format(m_editorUtils.GetTextValue("Detach Spawner Dialog"), res.LinkedSpawners[i], m_tmpSpawner.ChildSpawners[res.LinkedSpawners[i]].m_parentName, res.m_name);
                                    if (EditorUtility.DisplayDialog("GeNa WARNING!", dialogText, m_editorUtils.GetTextValue("OkBtn"), m_editorUtils.GetTextValue("CancelBtn")))
                                    {
                                        res.DetachLinkedSpawner(res.LinkedSpawners[i]);
                                        GUI.changed = true;
                                    }
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            m_editorUtils.InlineHelp("Linked Spawners List", helpEnabled);
        }

        /// <summary>
        /// Draws the GeNa Extensions Controls for the Resource
        /// </summary>
        private void ExtensionsControl(Prototype proto, Resource res, bool helpEnabled)
        {
            string gxIcon = "<b>" + GX_ICON + "</b>";
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(EditorGUI.indentLevel * 15f);

                GUILayout.BeginVertical(SpawnerEditor.styles.boxLabelLeft);
                {
                    GUILayout.Label(new GUIContent(gxIcon, m_editorUtils.GetTooltip("ExtensionsDrop")), styles.richLabel, GUILayout.Width(20f));
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical(SpawnerEditor.styles.boxWithLeftLabel, GUILayout.MinHeight(50f));
                {
                    if (res.HasExtensions)
                    {
                        ExtensionList(res, res.Extensions);
                        ExtensionList(res, res.StatelessExtensions);
                    }
                    else
                    {
                        m_editorUtils.Label("ExtensionsDrop", SpawnerEditor.styles.body);
                    }
                }
                GUILayout.EndVertical();

                // Handle drag and drop
                if (LastAreaExtensionDrop(res))
                {
                    GUI.changed = true;
                }
            }
            GUILayout.EndHorizontal();

            m_editorUtils.InlineHelp("ExtensionsDrop", helpEnabled);
        }

		/// <summary>
		/// Displays the Extension (statefull or stateless)
		/// </summary>
		private void ExtensionList(Resource res, IGeNaExtension[]array)
		{
			// Nothing to do if no extensions
			if (array == null)
			{
				return;
			}

			// Simpler to only delete stuff once we finished displaying the array.
			int toDelete = -1;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					Debug.LogErrorFormat("[GeNa] Extension component (attached to Resource '{0}') does not " +
						"exist in the project.", res.m_name);
					continue;
				}

				if(DisplayExtension(res.m_name, array[i].Name.Replace(".", "\n  \u2514")))
				{
					toDelete = i;
				}
			}

			// Simpler to only delete stuff once we finished displaying the array.
			if (toDelete > -1)
			{
				res.RemoveExtension(toDelete);
				GUI.changed = true;
			}
		}

		/// <summary>
		/// Displays the Extension (statefull or stateless)
		/// </summary>
		private void ExtensionList(Resource res, Type[]array)
		{
			// Nothing to do if no extensions
			if (array == null)
			{
				return;
			}

			// Simpler to only delete stuff once we finished displaying the array.
			int toDelete = -1;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					Debug.LogErrorFormat("[GeNa] Extension script (attached to Resource '{0}') does not " +
						"exist in the project.", res.m_name);
					continue;
				}

				if (DisplayExtension(res.m_name, array[i].Name))
				{
					toDelete = i;
				}
			}

			// Simpler to only delete stuff once we finished displaying the array.
			if (toDelete > -1)
			{
				res.RemoveStatelessExtension(toDelete);
				GUI.changed = true;
			}
		}

		/// <summary>
		/// Display an extension with delete option. Returns true if the user selected deletion.
		/// </summary>
		/// <param name="resName"></param>
		/// <param name="extensionName"></param>
		/// <returns></returns>
		private bool DisplayExtension(string resName, string extensionName)
		{
			bool delete = false;

			GUILayout.BeginHorizontal();
			{
				if (m_editorUtils.DeleteButton())
				{
					string dialogText = string.Format(m_editorUtils.GetTextValue("Remove Extension Dialog"), extensionName, resName);
					if (EditorUtility.DisplayDialog("GeNa WARNING!", dialogText, m_editorUtils.GetTextValue("OkBtn"), m_editorUtils.GetTextValue("CancelBtn")))
					{
						delete = true;
					}
				}
				GUILayout.Label(new GUIContent("<b>" + extensionName + "</b>", m_editorUtils.GetTooltip("ExtensionsDrop")), SpawnerEditor.styles.richLabel);
			}
			GUILayout.EndHorizontal();

			return delete;
		}

		/// <summary>
		/// Handle drop area for extensions
		/// </summary>
		public bool LastAreaExtensionDrop(Resource res)
        {
            //Ok - set up for drag and drop
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetLastRect();

            bool extensionsAdded = false;

            if (evt.type == EventType.DragPerform || evt.type == EventType.DragUpdated)
            {
                if (!drop_area.Contains(evt.mousePosition))
                {
                    return false;
                }

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                    {
						// Handle stateless extensions (script only)
                        if (draggedObject is MonoScript)
                        {
                            MonoScript script = draggedObject as MonoScript;
                            if (m_tmpSpawner != null)
                            {
                                extensionsAdded = res.AddExtension(script.GetClass());                                
                            }
                        }
						// Handle extensions attached to prefabs/gameobjects
                        else if (draggedObject is GameObject)
                        {
                            GameObject go = draggedObject as GameObject;
                            if (m_tmpSpawner != null)
                            {
                                extensionsAdded =  res.AddExtension(go);
                            }
                        }
                        else
                        {
                            Debug.LogWarningFormat("<b>{0}</b> is not a valid GeNa Extension, or Prefab, or GameObject.", draggedObject.name);
                        }                        
                    }
                }
            }
            return extensionsAdded;
        }

        /// <summary>
        /// Displays the Children of the Resource in a Resource Tree, if there are any.
        /// </summary>
        private void ChildResources(Prototype proto, Resource res, bool helpEnabled, bool advanced)
        {
            // Child Resources (if Resource tree)
            for (int i = 0; i < res.Children.Count; i++)
            {
                Resource childRes = res.Children[i];
                string childName = childRes.m_name;
                switch (childRes.m_resourceType)
                {
                    case Constants.ResourceType.Prefab:
                        childName += childRes.ContainerOnly ? " (G)" : " (P)";
                        if (childRes.m_conformToSlope)
                        {
                            childName += " *C*";
                        }
                        break;
                    case Constants.ResourceType.TerrainTree:
                        childName += " (T)";
                        break;
                    case Constants.ResourceType.TerrainGrass:
                        childName += " (G)";
                        break;
                    case Constants.ResourceType.TerrainTexture:
                        childName += " (Tx)";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                childName += string.Format(" {0:0}%", childRes.m_successRate * 100f);

                GUILayout.BeginVertical(SpawnerEditor.styles.gpanel);
                {
                    GUILayout.BeginHorizontal((childRes.Static == Constants.ResourceStatic.Dynamic) ? styles.dynamicResHeader : styles.staticResHeader);
                    {
                        childRes.OpenedInGuiHierarchy = EditorGUILayout.Foldout(childRes.OpenedInGuiHierarchy, childName, true, SpawnerEditor.styles.resTreeFoldout);

                        GUILayout.FlexibleSpace();
                        StaticSwitch(childRes);
                    }
                    GUILayout.EndHorizontal();

                    if (childRes.OpenedInGuiHierarchy == true)
                    {
                        // Proto won't be changed since we can't mix terrain resources into the mix.
                        // If we do later we can avoid changes being made to the proto by children of trees.
                        EditResource(proto, childRes, true, helpEnabled, advanced);
                    }
                }
                GUILayout.EndVertical();
            }
        }

        /// <summary>
        /// Advanced Panel
        /// </summary>
        /// <param name="helpEnabled"></param>
        private void AdvancedPanel(bool helpEnabled)
        {
            if (m_tmpSpawner.ChildSpawners != null && m_tmpSpawner.ChildSpawners.Count > 0)
            {
                m_tmpSpawner.ChildSpawnMode = (Constants.ChildSpawnMode)m_editorUtils.EnumPopup("Child Spawn Mode", m_tmpSpawner.ChildSpawnMode, helpEnabled);
            }
            m_tmpSpawner.m_spawnToTarget = m_editorUtils.Toggle("Spawn to Target", m_tmpSpawner.m_spawnToTarget, helpEnabled);
#if VEGETATION_STUDIO || VEGETATION_STUDIO_PRO
            m_tmpSpawner.m_spawnToAVS = m_editorUtils.Toggle("Spawn To AVS", m_tmpSpawner.m_spawnToAVS, helpEnabled);
#endif
            m_tmpSpawner.m_advForcePlaceAtClickLocation = m_editorUtils.Toggle("Force Spawn", m_tmpSpawner.m_advForcePlaceAtClickLocation, helpEnabled);
            m_tmpSpawner.m_advUseLargeRanges = m_editorUtils.Toggle("Use Large Ranges", m_tmpSpawner.m_advUseLargeRanges, helpEnabled);
            m_tmpSpawner.m_scaleToNearestInt = m_editorUtils.Toggle("Scale Nearest Int", m_tmpSpawner.m_scaleToNearestInt, helpEnabled);
            //m_tmpSpawner.m_advAddColliderToSpawnedPrefabs = m_editorUtils.Toggle("Add Collider To POI", m_tmpSpawner.m_advAddColliderToSpawnedPrefabs, helpEnabled);
            m_tmpSpawner.m_globalSpawnJitterPct = m_editorUtils.Slider("Global Spawn Jitter", m_tmpSpawner.m_globalSpawnJitterPct * 100f, 0f, 100f, helpEnabled) * 0.01f;
            m_tmpSpawner.m_advSpawnCheckOffset = m_editorUtils.FloatField("Collision Test Offset", m_tmpSpawner.m_advSpawnCheckOffset, helpEnabled);
            m_tmpSpawner.m_autoProbe = m_editorUtils.Toggle("Add Light Probes", m_tmpSpawner.m_autoProbe, helpEnabled);
            if (m_tmpSpawner.m_autoProbe)
            {
                m_tmpSpawner.m_minProbeGroupDistance = m_editorUtils.Slider("Min PG Dist", m_tmpSpawner.m_minProbeGroupDistance, 10f, 200f, helpEnabled);
                m_tmpSpawner.m_minProbeDistance = m_editorUtils.Slider("Min Probe Dist", m_tmpSpawner.m_minProbeDistance, 5f, 50f, helpEnabled);
            }
            m_tmpSpawner.m_autoOptimise = m_editorUtils.Toggle("Spawn Optimizer", m_tmpSpawner.m_autoOptimise, helpEnabled);
            if (m_tmpSpawner.m_autoOptimise)
            {
                m_tmpSpawner.m_maxSizeToOptimise = m_editorUtils.Slider("Smaller Than (m)", m_tmpSpawner.m_maxSizeToOptimise, 5f, 50f, helpEnabled);
            }
            m_tmpSpawner.m_metersPerScan = m_editorUtils.Slider("Actual Bounds Res", m_tmpSpawner.m_metersPerScan, 0.5f, 5f, helpEnabled);
            m_tmpSpawner.m_metersPerScanVisualisation = m_editorUtils.Slider("Visualiser Bounds Res", m_tmpSpawner.m_metersPerScanVisualisation, 0.5f, 5f, helpEnabled);
            m_tmpSpawner.m_maxVisualisationDimensions = m_editorUtils.IntSlider("Visualiser Resolution", m_tmpSpawner.m_maxVisualisationDimensions, 1, 100, helpEnabled);
            if (m_tmpSpawner.m_advUseLargeRanges)
            {
                m_tmpSpawner.m_visProcessLimit = m_editorUtils.FloatField("Process Limit", m_tmpSpawner.m_visProcessLimit, helpEnabled);
            }
            else
            {
                m_tmpSpawner.m_visProcessLimit = m_editorUtils.Slider("Process Limit", m_tmpSpawner.m_visProcessLimit, 0.1f, 10f, helpEnabled);
            }
        }

        /// <summary>
        /// Panel to add prototypes
        /// </summary>
        private void AddPrototypesPanel()
        {
            GUILayout.BeginVertical(m_editorUtils.Styles.panelFrame);
            {
                //Add prototypes
                GUILayout.BeginHorizontal();
                if (DrawPrefabGUI())
                {
                    GUI.changed = true;
                }
                if (m_editorUtils.Button("Add Tree", styles.addBtn, GUILayout.Width(50), GUILayout.Height(49)))
                {
                    m_tmpSpawner.AddTreeProto();
                    GUI.changed = true;
                }
                if (m_editorUtils.Button("Add Grass", styles.addBtn, GUILayout.Width(50), GUILayout.Height(49)))
                {
                    m_tmpSpawner.AddGrassProto();
                    GUI.changed = true;
                };
                if (m_editorUtils.Button("Add Tx", styles.addBtn, GUILayout.Width(50), GUILayout.Height(49)))
                {
                    //Add and init the brushsets for it
                    m_tmpSpawner.AddTextureProto();
                    GUI.changed = true;
                };
                GUILayout.EndHorizontal();

                // Set custom ground level
                GUILayout.BeginHorizontal();
                {
                    m_editorUtils.ToggleButton("Set Custom Ground Lvl Btn", ref m_dropGround, styles.inlineToggleBtn, styles.inlineToggleBtnDown, GUILayout.Width(60f), GUILayout.Height(17f));
                    EditorGUI.BeginDisabledGroup(float.IsNaN(m_spawner.m_ingestionGroundLevel));
                    {
                        if (m_editorUtils.DeleteButton(m_editorUtils.GetTooltip("Unset Custom Ground Lvl Btn")))
                        {
                            // Unset the Custom Ground Level and turn off the set toggle if it's on.
                            m_spawner.m_ingestionGroundLevel = float.NaN;
                            m_dropGround = false;
                        }
                    }
                    EditorGUI.EndDisabledGroup();

                    GUIContent lbl = null;
                    if (float.IsNaN(m_spawner.m_ingestionGroundLevel))
                    {
                        lbl = m_editorUtils.GetContent("No Custom Ground lvl");
                    }
                    else
                    {
                        lbl = new GUIContent(
                            string.Format("{0} {1:0.00}", m_editorUtils.GetTextValue("Custom Ground lvl"), m_spawner.m_ingestionGroundLevel),
                            m_editorUtils.GetTooltip("Custom Ground lvl"));
                    }
                    m_editorUtils.Label(lbl);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        #endregion

        /// <summary>
        /// Ensures that all the protoypes have the correct Spawn Criterias set.
        /// This is only called when the spawner is up to date, so working directly on it instead of the cache.
        /// which may not exist the first time this is called.
        /// </summary>
        private void UpdateSpawnCritOverrides()
        {
            m_needsSpawnCritOverrideUpdate = false;
            Spawner[] spawners;
            if (m_tmpSpawner != null)
            {
                spawners = new Spawner[] { m_spawner, m_tmpSpawner };
            }
            else
            {
                spawners = new Spawner[] { m_spawner };
            }

            foreach (Spawner spawner in spawners)
            {
                spawner.UpdateSpawnCritOverrides();
            }
        }

        /// <summary>
        /// Update from legacy prtotype bounds modifier to the new override system.
        /// </summary>
        private void UpdateLegacyProtoBoundsModifier()
        {
            Spawner[] spawners;
            if (m_tmpSpawner != null)
            {
                spawners = new Spawner[] { m_spawner, m_tmpSpawner };
            }
            else
            {
                spawners = new Spawner[] { m_spawner };
            }

#pragma warning disable 0618
            foreach (Spawner spawner in spawners)
            {
                for (int i = 0; i < spawner.m_spawnPrototypes.Count; i++)
                {
                    Prototype proto = spawner.m_spawnPrototypes[i];
                    if (proto.m_boundsBorder != 0f)
                    {
                        if (proto.m_overrideCritBoundsBorder || proto.m_critBoundsBorder != spawner.m_critBoundsBorder)
                        {
                            Debug.LogWarningFormat("[GeNa] '{0}' prototype '{1}' contains both a legacy Bounds Modifier additive modifier and a Bounds Modifier Override. The legacy modifier has been discarded. See Spawen Criteria Overrides.",
                                m_spawner.name, proto.m_name);
                        }
                        else
                        {
                            proto.m_overrideCritBoundsBorder = true;
                            proto.m_critBoundsBorder = spawner.m_critBoundsBorder + proto.m_boundsBorder;
                            Debug.LogFormat("[GeNa] '{0}' prototype '{1}' contained a legacy Bounds Modifier additive modifier. This was converted to a Bounds Modifier Override. See Spawen Criteria Overrides.", m_spawner.name, proto.m_name);
                        }
                        proto.m_boundsBorder = 0f;
                    }
                }
            }
#pragma warning restore 0618
        }

        /// <summary>
        /// Return true if the resource list provided has prefabs
        /// </summary>
        /// <param name="sourcePrototypes"></param>
        /// <returns></returns>
        bool HasPrefabs(List<Prototype> sourcePrototypes)
        {
            foreach (var srcProto in sourcePrototypes)
            {
                if (srcProto.m_resourceType == Constants.ResourceType.Prefab)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if the resource list provided has trees
        /// </summary>
        /// <param name="sourcePrototypes"></param>
        /// <returns></returns>
        bool HasTrees(List<Prototype> sourcePrototypes)
        {
            foreach (var srcProto in sourcePrototypes)
            {
                if (srcProto.m_resourceType == Constants.ResourceType.TerrainTree)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if the resource list provided has textures
        /// </summary>
        /// <param name="sourcePrototypes"></param>
        /// <returns></returns>
        bool HasTextures(List<Prototype> sourcePrototypes)
        {
            foreach (var srcProto in sourcePrototypes)
            {
                if (srcProto.m_resourceType == Constants.ResourceType.TerrainTexture)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Draw a prefab field that handles prefab replacement
        /// </summary>
        /// <param name="res"></param>
        private void PrefabField(Resource res, bool helpEnabled)
        {
            GameObject prefab = res.m_prefab;
            prefab = (GameObject)m_editorUtils.ObjectField("Prefab", prefab, typeof(GameObject), false, helpEnabled);
            if (prefab != res.m_prefab)
            {
                if (prefab != null)
                {
                    res.ReplacePrefab(prefab);
                }
                else
                {
                    Debug.LogWarningFormat("[GeNa] Prefab was set to null for Resource [{0}]. This is an invalid operation and was ignored. You can delete the Resource or replace it with a blank GameObject if you wish.", res.m_name);
                }
            }
        }

        /// <summary>
        /// Draw the static switch for a res
        /// </summary>
        private bool StaticSwitch(Resource res)
        {
            // We only use this for prefab resources
            if (res.m_resourceType == Constants.ResourceType.Prefab)
            {
                Constants.ResourceStatic val = res.Static;
                val = (Constants.ResourceStatic)m_editorUtils.Toolbar((int)res.Static, Enum.GetNames(typeof(Constants.ResourceStatic)), GUILayout.ExpandWidth(false));

                if (val != res.Static)
                {
                    res.Static = val;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Create toggle to enable/disable a control and returns true if the value of the toggle changes.
        /// </summary>
        /// <param name="enabled"></param>
        private bool BeginControlToggle(ref bool enabled)
        {
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            {
                enabled = EditorGUILayout.Toggle(enabled, GUILayout.Width(60f), GUILayout.ExpandWidth(false));
            }
            bool changed = EditorGUI.EndChangeCheck();
            // Toggle works arkwardly with the clickable area shifting out to the left. It becomes non-clickable with smaller width, so using this trick.
            GUILayout.Space(-48f);
            EditorGUI.BeginDisabledGroup(!enabled);
            return changed;
        }

        /// <summary>
        /// Create toggle to enable/disable a control.
        /// </summary>
        private void EndControlToggle()
        {
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Handle drop area for new objects
        /// </summary>
        public bool DrawPrefabGUI()
        {
            //Ok - set up for drag and drop
            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));

            string dropMsg = m_dropGround ? m_editorUtils.GetTextValue("Drop ground lvl box msg") : m_editorUtils.GetTextValue("Add proto drop box msg");
            GUI.Box(dropArea, dropMsg, SpawnerEditor.styles.gpanel);

            if (evt.type == EventType.DragPerform || evt.type == EventType.DragUpdated)
            {
                if (!dropArea.Contains(evt.mousePosition))
                    return false;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    List<GameObject> resources = new List<GameObject>();

                    //Handle game objects / prefabs
                    foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is GameObject)
                        {
                            GameObject go = draggedObject as GameObject;
                            if (go.GetComponent<Spawner>() != null)
                            {
                                //We dont want to spawn spawners
                                Debug.LogWarning("You can not add spawners.");
                            }
                            else
                            {
                                if (m_tmpSpawner != null)
                                {
                                    resources.Add(go);
                                }
                            }
                        }
                    }

                    if (m_dropGround)
                    {
                        if (resources.Count == 1)
                        {
                            m_spawner.m_ingestionGroundLevel = resources[0].transform.position.y;
                            m_dropGround = false;
                        }
                        else if (resources.Count < 1)
                        {
                            EditorUtility.DisplayDialog("GeNa", m_editorUtils.GetTextValue("Ground Level Ingestion - No GO"), m_editorUtils.GetTextValue("OkBtn"));
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("GeNa", m_editorUtils.GetTextValue("Ground Level Ingestion - Too many GOs"), m_editorUtils.GetTextValue("OkBtn"));
                        }

                        return false;
                    }

                    //Handle speedtrees
                    foreach (var path in DragAndDrop.paths)
                    {
                        //Update in case unity has messed with it 
                        if (path.StartsWith("Assets"))
                        {
                            //Check file type and process as we can
                            string fileType = Path.GetExtension(path).ToLower();

                            //Check for speed trees - and add them
                            if (fileType == ".spm")
                            {
                                GameObject speedTree = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                                if (speedTree != null)
                                {
                                    resources.Add(speedTree);
                                }
                                else
                                {
                                    Debug.LogWarning("Unable to load " + path);
                                }
                            }
                        }
                    }

                    //Start managing them
                    AddGameObjects(m_tmpSpawner, resources);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add the game object from a list of prefabs instantiated as game objects.
        /// </summary>
        /// <param name="protoList">The list of prototypes to add the object to.</param>
        /// <param name="ingestList">The list of resources, resource trees to generate the prototypes from.</param>
        public void AddGameObjects(Spawner spawner, List<GameObject> ingestList)
        {
            if (ingestList == null || ingestList.Count < 1)
            {
                Debug.LogWarning("Can't add null or empty resource list");
                return;
            }

            if (m_tmpSpawner == null)
            {
                Debug.LogWarning("Can't add resources because spawner is missing");
                return;
            }

            bool ingestStructure = false;
            if (ingestList.Count > 1)
            {
                // Ask the user if they want to import a structure or just multiple individual items
                if (EditorUtility.DisplayDialog("GeNa Ingestion", m_editorUtils.GetTextValue("Structure Ingestion Dialog Text"), m_editorUtils.GetTextValue("Structure Btn"), m_editorUtils.GetTextValue("Individual Items Btn")))
                {
                    ingestStructure = true;
                    //Automatically set type to structured
                    m_tmpSpawner.m_type = Constants.SpawnerType.Structured;
                    m_tmpSpawner.m_rotationAlgorithm = Constants.RotationAlgorithm.Fixed;
                    m_tmpSpawner.m_maxRotationY = m_tmpSpawner.m_minRotationY = 0f;
                }
            }

            Bounds globalBounds = new Bounds();

            //Ingest prototypes
            foreach (GameObject go in ingestList)
            {
                float rotation = 0;

                //Create and add the prototype
                Prototype proto = new Prototype(spawner);
                proto.m_name = go.name;
                proto.m_resourceType = Constants.ResourceType.Prefab;

                //Used to track unique names in a prototype
                HashSet<string> names = new HashSet<string>();

                //Now add in the resource tree
                Bounds protoBounds = new Bounds();
                bool treeContainsPrefab = false;

                //This adds the resource tree and calculates global bounds - everything will be relative to this
                proto.m_resourceTree.Add(IngestResource(proto, null, go, ref names, ref protoBounds, ref treeContainsPrefab, ingestStructure));

                if (treeContainsPrefab == false)
                {
                    Debug.LogWarningFormat("{0} contains no prefab and was ignored.", go.name);
                    continue;
                }

                // Add proto
                spawner.AddProto(proto);

                proto.m_size = protoBounds.size;
                proto.m_extents = proto.m_size * 0.5f;
                proto.m_forwardRotation = rotation;

                //If ingested several things (a structure), we want to set their position offset relative to the center of their collective bounds
                if (ingestStructure)
                {
                    // Proto bounds are world origin based, let's adjust
                    protoBounds.center = new Vector3(
                        protoBounds.center.x + proto.m_resourceTree[0].m_minOffset.x,
                        protoBounds.center.y + proto.m_resourceTree[0].m_minOffset.y,
                        protoBounds.center.z + proto.m_resourceTree[0].m_minOffset.z);

                    //If first time then set bounds up
                    if (globalBounds.size == Vector3.zero)
                    {
                        globalBounds = new Bounds(protoBounds.center, protoBounds.size);
                    }
                    //Otherwise expand on it
                    else
                    {
                        globalBounds.Encapsulate(protoBounds);
                    }
                }

                //If first one, then update some settings to be more prefab friendly
                if (m_tmpSpawner.m_spawnPrototypes.Count == 1)
                {
                    //Activate bounds checking
                    m_tmpSpawner.m_critVirginCheckType = Constants.VirginCheckType.Bounds;
                    m_tmpSpawner.m_scaleToNearestInt = false;
                    m_tmpSpawner.m_seedThrowRange = Mathf.Min(proto.m_size.x, proto.m_size.z) * 2f;
                }
            }

            //If ingested several things, we want to set their position offset relative to the center of their collective bounds
            if (ingestStructure)
            {
                //Then process each resource
                foreach (Prototype proto in spawner.m_spawnPrototypes)
                {
                    proto.m_resourceTree[0].m_basePosition.x = proto.m_resourceTree[0].m_minOffset.x = proto.m_resourceTree[0].m_maxOffset.x = proto.m_resourceTree[0].m_minOffset.x - globalBounds.center.x;
                    //// Assume zero is ground
                    //proto.m_resources[0].m_minOffset.y = proto.m_resources[0].m_maxOffset.y = proto.m_resources[0].m_minOffset.y;
                    proto.m_resourceTree[0].m_basePosition.z = proto.m_resourceTree[0].m_minOffset.z = proto.m_resourceTree[0].m_maxOffset.z = proto.m_resourceTree[0].m_minOffset.z - globalBounds.center.z;
                }
            }

            // Update the overrides to ensure that the new prototypes have the correct spawn criteria values
            m_needsSpawnCritOverrideUpdate = true;
        }

        /// <summary>
        /// Ingest a resource tree or a single resource.
        /// </summary>
        /// <param name="proto">The prototype the resource tree belongs to.</param>
        /// <param name="parentResource">Null if a top level resource. Used by the method recursively to build the resource tree.</param>
        /// <param name="go">The object to be ingested as resource.</param>
        /// <param name="names">Reference to the nameset that's used to ensure unique resource names inside a prototype (May not be needed for resource trees).</param>
        /// <param name="protoBounds">Reference to Bounds for the whole prototype.</param>
        /// <param name="structureIngestion">True if ingesting multiple prototypes to be used in a structured spawner.</param>
        /// <returns>Returns the top level resource of the tree.</returns>
        private Resource IngestResource(Prototype proto, Resource parentResource, GameObject go, ref HashSet<string> names, ref Bounds protoBounds, ref bool treeContainsPrefab, bool structureIngestion)
        {
            Resource res = new Resource(proto.Spawner);
            res.m_name = GetUniqueName(go.name, ref names);
            res.SetParent(parentResource);

            //Get bounds
            Bounds localBounds = Spawner.GetInstantiatedBounds(go);
            Bounds localColliderBounds = Spawner.GetLocalColliderBounds(go);

            //Debug.LogFormat("[{0}]: {1}", go.name, localBounds.size);

            //If first time then set bounds up
            if (protoBounds.size == Vector3.zero)
            {
                protoBounds = new Bounds(localBounds.center, localBounds.size);
            }
            //Otherwise expand on it
            else
            {
                protoBounds.Encapsulate(localBounds);
            }

            //Get colliders
            res.m_hasRootCollider = Spawner.HasRootCollider(go);
            res.m_hasColliders = Spawner.HasColliders(go);
            if (res.m_hasColliders)
            {
                proto.m_hasColliders = true;
            }

            //Get meshes
            res.m_hasMeshes = Spawner.HasMeshes(go);
            if (res.m_hasMeshes)
            {
                proto.m_hasMeshes = true;
            }

            //Get rigid body
            res.m_hasRigidBody = Spawner.HasRigidBody(go);
            if (res.m_hasRigidBody)
            {
                proto.m_hasRigidBody = true;
            }

            //If top level resource
            if (parentResource == null)
            {
                // Top level is not static by default, but descendants are
                res.Static = Constants.ResourceStatic.Dynamic;

                if (structureIngestion)
                {
                    //Offsets
                    //Using global positions for x and z because the offsets for structure ingestion will be 
                    //calculated by global bounds center.
                    res.m_minOffset.x = res.m_maxOffset.x = go.transform.position.x;
                    if (float.IsNaN(m_spawner.m_ingestionGroundLevel))
                    {
                        res.m_minOffset.y = res.m_maxOffset.y = 0f;
                    }
                    else
                    {
                        res.m_minOffset.y = res.m_maxOffset.y = go.transform.position.y - m_spawner.m_ingestionGroundLevel;
                    }
                    res.m_minOffset.z = res.m_maxOffset.z = go.transform.position.z;

                    //Rotation
                    //When ingesting structures the default is to use exact rotations which are handy for example for fences
                    res.m_minRotation.x = res.m_maxRotation.x = go.transform.localEulerAngles.x;
                    res.m_minRotation.y = res.m_maxRotation.y = go.transform.localEulerAngles.y;
                    res.m_minRotation.z = res.m_maxRotation.z = go.transform.localEulerAngles.z;
                }
                else
                {
                    //Offsets
                    //If importing a single proto, it gets no offset
                    res.m_minOffset = res.m_maxOffset = Vector3.zero;
                    if (float.IsNaN(m_spawner.m_ingestionGroundLevel) == false)
                    {
                        res.m_minOffset.y = res.m_maxOffset.y = go.transform.position.y - m_spawner.m_ingestionGroundLevel;
                    }

                    //Rotation
                    res.m_minRotation.x = res.m_maxRotation.x = go.transform.localEulerAngles.x;
                    res.m_minRotation.y = 0f;
                    res.m_maxRotation.y = 360f;
                    res.m_minRotation.z = res.m_maxRotation.z = go.transform.localEulerAngles.z;
                }

                //Top level conforms to slope
                res.m_conformToSlope = true;
            }
            else
            {
                //For child resources in a tree we take the location  and rotation relative to the parent
                //Offsets
                res.m_minOffset.x = res.m_maxOffset.x = go.transform.localPosition.x;
                res.m_minOffset.y = res.m_maxOffset.y = go.transform.localPosition.y;
                res.m_minOffset.z = res.m_maxOffset.z = go.transform.localPosition.z;

                //Rotation
                res.m_minRotation.x = res.m_maxRotation.x = go.transform.localEulerAngles.x;
                res.m_minRotation.y = res.m_maxRotation.y = go.transform.localEulerAngles.y;
                res.m_minRotation.z = res.m_maxRotation.z = go.transform.localEulerAngles.z;

                //Non top level doesn't conform to slope
                res.m_conformToSlope = false;
            }

            if (Spawner.ApproximatelyEqual(go.transform.localScale.x, go.transform.localScale.y, 0.000001f) && Spawner.ApproximatelyEqual(go.transform.localScale.x, go.transform.localScale.z, 0.000001f))
            {
                res.m_sameScale = true;
            }
            else
            {
                res.m_sameScale = false;
            }
            res.m_minScale.x = res.m_maxScale.x = go.transform.localScale.x;
            res.m_minScale.y = res.m_maxScale.y = go.transform.localScale.y;
            res.m_minScale.z = res.m_maxScale.z = go.transform.localScale.z;

            res.m_basePosition = res.m_minOffset;
            res.m_baseRotation = go.transform.localEulerAngles;
            res.m_baseScale = go.transform.localScale;
            res.m_baseSize = localBounds.size;
            res.m_boundsCenter = localBounds.center;
            res.m_baseColliderCenter = localColliderBounds.center;
            res.m_baseColliderScale = localColliderBounds.size;

            //We can only determine if it is a prefab in the editor
#if UNITY_EDITOR
            if (Spawner.IsPrefab(go))
            {
#if UNITY_2018_3_OR_NEWER
                res.m_prefab = Spawner.GetPrefabAsset(go);
                if (res.m_prefab == null)
                {
                    res.m_prefab = go;
                }
#else
                if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
                {
                    res.m_prefab = GetPrefabAsset(go);
                }
                else
                {
                    res.m_prefab = go;
                }
#endif

                if (res.m_prefab != null)
                {
                    //We got a prefab here
                    treeContainsPrefab = true;

                    //Get its asset ID
                    string path = AssetDatabase.GetAssetPath(res.m_prefab);
                    if (!string.IsNullOrEmpty(path))
                    {
                        res.m_assetID = AssetDatabase.AssetPathToGUID(path);
                        res.m_assetName = Spawner.GetAssetName(path);
                    }

                    //Get flags
                    StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(res.m_prefab);
                    res.m_flagBatchingStatic = (flags & StaticEditorFlags.BatchingStatic) == StaticEditorFlags.BatchingStatic;
#if UNITY_5 || UNITY_2017 || UNITY_2018 || UNITY_2019_1
                    res.m_flagLightmapStatic = (flags & StaticEditorFlags.LightmapStatic) == StaticEditorFlags.LightmapStatic;
#else
                    res.m_flagLightmapStatic = (flags & StaticEditorFlags.ContributeGI) == StaticEditorFlags.ContributeGI;
#endif
                    res.m_flagNavigationStatic = (flags & StaticEditorFlags.NavigationStatic) == StaticEditorFlags.NavigationStatic;
                    res.m_flagOccludeeStatic = (flags & StaticEditorFlags.OccludeeStatic) == StaticEditorFlags.OccludeeStatic;
                    res.m_flagOccluderStatic = (flags & StaticEditorFlags.OccluderStatic) == StaticEditorFlags.OccluderStatic;
                    res.m_flagOffMeshLinkGeneration = (flags & StaticEditorFlags.OffMeshLinkGeneration) == StaticEditorFlags.OffMeshLinkGeneration;
                    res.m_flagReflectionProbeStatic = (flags & StaticEditorFlags.ReflectionProbeStatic) == StaticEditorFlags.ReflectionProbeStatic;
                }
                else
                {
                    Debug.LogErrorFormat("Unable to get prefab for '{0}'", res.m_name);
                }

                if (go.transform.childCount < res.m_prefab.transform.childCount)
                {
                    Debug.LogErrorFormat("What's going on here? The Prefab Instance seems to have less childs than the Prefab Asset: {0} < {1}", go.transform.childCount, res.m_prefab.transform.childCount);
                }
                else
                {
                    HashSet<Transform> prefabAssetChilds = new HashSet<Transform>(res.m_prefab.transform.Cast<Transform>());

                    foreach (Transform child in go.transform)
                    {
                        Transform prefabAssetTransform = Spawner.GetPrefabAsset(child);
                        if (Spawner.IsPrefab(child.gameObject))
                        {
                            // If an instance of a Prefab Asset. Let's check if it's children of the Prefab in the Asset itself
                            if (prefabAssetTransform != null)
                            {
                                //If it's part of the prefab, skip it
                                if (prefabAssetChilds.Contains(prefabAssetTransform))
                                {
                                    continue;
                                }
                            }
                            // This doesn't even have its own Prefab Asset. It must belong inside the Prefab that is being ingested.
                            else
                            {
                                continue;
                            }
                        }

                        // This GO or Prefab is not part of the Prefab that's being ingested. Let's process it.
                        res.Children.Add(IngestResource(proto, res, child.gameObject, ref names, ref protoBounds, ref treeContainsPrefab, false));
                    }
                }
            }
            //Else this is just a GO (container in the tree) not a prefab: Keep traversing the tree.
            else
            {
                res.ContainerOnly = true;

                // Warn the user if it has more components than just the Transform since it's not a prefab.
                Component[] components = go.GetComponents<Component>();
                if (components != null && components.Length > 1)
                {
                    Debug.LogWarningFormat("[GeNa]: Warning! Gameobject '{0}' has Components but it's not a Prefab Instance. Make it into a Prefab if you wish to keep its Components information for spawning.",
                        go.name);
                }

                // Keep traversing the tree.
                if (go.transform.childCount > 0)
                {
                    foreach (Transform child in go.transform)
                    {
                        res.Children.Add(IngestResource(proto, res, child.gameObject, ref names, ref protoBounds, ref treeContainsPrefab, false));
                    }
                }
            }
#else // (not UNITY_EDITOR)
            if (go.transform.childCount > 0)
            {
                foreach (Transform child in go.transform)
                {
                    res.Children.Add(IngestResource(proto, res, child.gameObject, ref names, ref globalBounds, ref treeContainsPrefab, false));
                }
            }
#endif
            return res;
        }

        /// <summary>
        /// Display a button that takes editor indentation into account
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        bool DisplayButton(GUIContent content)
        {
            TextAnchor oldalignment = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            Rect btnR = EditorGUILayout.BeginHorizontal();
            btnR.xMin += (EditorGUI.indentLevel * 18f);
            btnR.height += 20f;
            btnR.width -= 4f;
            bool result = GUI.Button(btnR, content);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(22);
            GUI.skin.button.alignment = oldalignment;
            return result;
        }

        /// <summary>
        /// Handy layer mask interface
        /// </summary>
        /// <param name="label"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        static LayerMask LayerMaskField(GUIContent label, LayerMask layerMask)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                    maskWithoutEmpty |= (1 << i);
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;
            return layerMask;
        }

        /// <summary>
        /// Return the bounds of the supplied game object
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        Bounds GetBounds(GameObject go)
        {
            Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(r.bounds);
            }
            foreach (Collider c in go.GetComponentsInChildren<Collider>())
            {
                bounds.Encapsulate(c.bounds);
            }
            return bounds;
        }

        /// <summary>
        /// Get a unique name
        /// </summary>
        /// <param name="name">The original name</param>
        /// <param name="names">The names dictionary</param>
        /// <returns>The new unique name</returns>
        string GetUniqueName(string name, ref HashSet<string> names)
        {
            int idx = 0;
            string newName = name;
            while (names.Contains(newName))
            {
                newName = name + " " + idx.ToString();
                idx++;
            }
            names.Add(newName);
            return newName;
        }

        /// <summary>
        /// Make the texture supplied readable
        /// </summary>
        /// <param name="texture">Texture to convert</param>
        public static void MakeTextureReadable(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
#if UNITY_5_5_OR_NEWER
                if (tImporter.textureType != TextureImporterType.Default || tImporter.isReadable != true)
                {
                    tImporter.textureType = TextureImporterType.Default;
                    tImporter.isReadable = true;
                    AssetDatabase.ImportAsset(assetPath);
                    AssetDatabase.Refresh();
                }
#else
                if (tImporter.textureType != TextureImporterType.Advanced || tImporter.isReadable != true)
                {
                    tImporter.textureType = TextureImporterType.Advanced;
                    tImporter.isReadable = true;
                    AssetDatabase.ImportAsset(assetPath);
                    AssetDatabase.Refresh();
                }
#endif
            }
        }

        /// <summary>
        /// Update the resource ID's of the spawner
        /// </summary>
        private void UpdateResourceIDs()
        {
            if (m_tmpSpawner == null)
            {
                return;
            }
            string path;
            Terrain terrain = Terrain.activeTerrain;
            foreach (var proto in m_tmpSpawner.m_spawnPrototypes)
            {
                foreach (var resource in proto.m_resourceTree)
                {
                    switch (resource.m_resourceType)
                    {
                        case Constants.ResourceType.Prefab:
                            if (string.IsNullOrEmpty(resource.m_assetID) || string.IsNullOrEmpty(resource.m_assetName))
                            {
                                path = AssetDatabase.GetAssetPath(resource.m_prefab);
                                if (!string.IsNullOrEmpty(path))
                                {
                                    resource.m_assetID = AssetDatabase.AssetPathToGUID(path);
                                    resource.m_assetName = Spawner.GetAssetName(path);
                                }
                            }
                            break;
                        case Constants.ResourceType.TerrainTree:
                            if (string.IsNullOrEmpty(resource.m_assetID) && terrain != null)
                            {
                                TreePrototype[] treePrototypes = terrain.terrainData.treePrototypes;
                                if (resource.m_terrainProtoIdx < treePrototypes.Length)
                                {
                                    path = AssetDatabase.GetAssetPath(treePrototypes[resource.m_terrainProtoIdx].prefab);
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        resource.m_assetID = AssetDatabase.AssetPathToGUID(path);
                                    }
                                }
                            }
                            break;
                        case Constants.ResourceType.TerrainGrass:
                            if (string.IsNullOrEmpty(resource.m_assetID) && terrain != null)
                            {
                                DetailPrototype[] dtlPrototypes = terrain.terrainData.detailPrototypes;
                                if (resource.m_terrainProtoIdx < dtlPrototypes.Length)
                                {
                                    path = AssetDatabase.GetAssetPath(dtlPrototypes[resource.m_terrainProtoIdx].prototype);
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        resource.m_assetID = AssetDatabase.AssetPathToGUID(path);
                                    }
                                    else
                                    {
                                        path = AssetDatabase.GetAssetPath(dtlPrototypes[resource.m_terrainProtoIdx].prototypeTexture);
                                        if (!string.IsNullOrEmpty(path))
                                        {
                                            resource.m_assetID = AssetDatabase.AssetPathToGUID(path);
                                        }
                                    }
                                }
                            }
                            break;
                        case Constants.ResourceType.TerrainTexture:
                            if (string.IsNullOrEmpty(resource.m_assetID) && terrain != null)
                            {
                                Texture2D[] terrainTextures = terrain.terrainData.alphamapTextures;
                                if (resource.m_terrainProtoIdx < terrainTextures.Length)
                                {
                                    path = AssetDatabase.GetAssetPath(terrainTextures[resource.m_terrainProtoIdx]);
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        resource.m_assetID = AssetDatabase.AssetPathToGUID(path);
                                    }
                                }
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        /// <summary>
        /// Detect and handle mouse and keyboard events for current spawner
        /// </summary>
        void OnSceneGUI()
        {
            //Exit if no spawner
            if (m_spawner == null)
            {
                return;
            }

            //Exit if event does not have current value
            if (Event.current == null)
            {
                return;
            }

            RaycastHit hitInfo;
            bool raycastHit = ShowSpawnRange(out hitInfo);

			// Check if mouseUp was missed by Unity
			if (GUIUtility.hotControl == m_editor_control_id && Event.current.button == 0)
            {
				if (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseMove)
				{
					if (m_mouseDownForSpawn)
					{
						m_mouseDownForSpawn = false;
						//Undo either gets recorded at mouse up, or when the spawn is complete, depending on if mouse up happen before or after all the spawning complete in SpawnAfterEditorUpdate()
						if (m_spawnLocations.Count < 1)
						{
							if (m_paintSpawn)
							{
								m_spawner.RecordUndo("Painted Spawn");
								m_paintSpawn = false;
							}
							else
							{
								m_spawner.RecordUndo("Single Spawn");
							}
						}
					}

					GUIUtility.hotControl = 0;

					// Ensure the spawner cache is up to date after some action
					SpawnerToCache();
				}
            }

            //If SHIFT is not held down, visualization will be off
            m_spawner.VisualizationActive = false;

            //Keyboard handling
            if (m_spawner.m_lastSpawnedObject != null)
            {
                //Ctrl Left
                if (Event.current.Equals(m_spawner.m_defaults.KeyLeftEvent(false, true)))
                {
                    GUIUtility.hotControl = m_editor_control_id;
                    Vector3 movement = Quaternion.Euler(0F, SceneView.lastActiveSceneView.rotation.eulerAngles.y, 0f) * Vector3.left;
                    m_spawner.m_lastSpawnedObject.transform.position += (movement * 0.05f);
                    Event.current.Use();
                    GUIUtility.hotControl = 0;
                    return;
                }

                //Shift Ctrl Left
                if (Event.current.Equals(m_spawner.m_defaults.KeyLeftEvent(true, true)))
                {
                    GUIUtility.hotControl = m_editor_control_id;
                    m_spawner.m_lastSpawnedObject.transform.Rotate(Vector3.up, -1f);
                    Event.current.Use();
                    GUIUtility.hotControl = 0;
                    return;
                }

                //Ctrl right
                if (Event.current.Equals(m_spawner.m_defaults.KeyRightEvent(false, true)))
                {
                    GUIUtility.hotControl = m_editor_control_id;
                    Vector3 movement = Quaternion.Euler(0F, SceneView.lastActiveSceneView.rotation.eulerAngles.y, 0f) * Vector3.right;
                    m_spawner.m_lastSpawnedObject.transform.position += (movement * 0.05f);
                    Event.current.Use();
                    GUIUtility.hotControl = 0;
                    return;
                }

                //Shift Ctrl Right
                if (Event.current.Equals(m_spawner.m_defaults.KeyRightEvent(true, true)))
                {
                    GUIUtility.hotControl = m_editor_control_id;
                    m_spawner.m_lastSpawnedObject.transform.Rotate(Vector3.up, 1f);
                    Event.current.Use();
                    GUIUtility.hotControl = 0;
                    return;
                }

                //Ctrl Forward
                if (Event.current.Equals(m_spawner.m_defaults.KeyForwardEvent(false, true)))
                {
                    GUIUtility.hotControl = m_editor_control_id;
                    Vector3 movement = Quaternion.Euler(0F, SceneView.lastActiveSceneView.rotation.eulerAngles.y, 0f) * Vector3.forward;
                    m_spawner.m_lastSpawnedObject.transform.position += (movement * 0.05f);
                    Event.current.Use();
                    GUIUtility.hotControl = 0;
                    return;
                }

                //Shift Ctrl Forward
                if (Event.current.Equals(m_spawner.m_defaults.KeyForwardEvent(true, true)))
                {
                    GUIUtility.hotControl = m_editor_control_id;
                    m_spawner.m_lastSpawnedObject.transform.Translate(Vector3.up * 0.1f);
                    Event.current.Use();
                    GUIUtility.hotControl = 0;
                    return;
                }

                //Ctrl Backward
                if (Event.current.Equals(m_spawner.m_defaults.KeyBackwardEvent(false, true)))
                {
                    GUIUtility.hotControl = m_editor_control_id;
                    Vector3 movement = Quaternion.Euler(0F, SceneView.lastActiveSceneView.rotation.eulerAngles.y, 0f) * Vector3.back;
                    m_spawner.m_lastSpawnedObject.transform.position += (movement * 0.05f);
                    Event.current.Use();
                    GUIUtility.hotControl = 0;
                    return;
                }

                //Shift Ctrl Backward
                if (Event.current.Equals(m_spawner.m_defaults.KeyBackwardEvent(true, true)))
                {
                    GUIUtility.hotControl = m_editor_control_id;
                    m_spawner.m_lastSpawnedObject.transform.Translate(Vector3.down * 0.1f);
                    Event.current.Use();
                    GUIUtility.hotControl = 0;
                    return;
                }
            }

            // Ctrl Delete Backspace
            if (Event.current.Equals(m_spawner.m_defaults.KeyDeleteEvent(false, true)))
            {
                GUIUtility.hotControl = m_editor_control_id;
                if (EditorUtility.DisplayDialog("WARNING!",
                    "Are you sure you want to delete all instances of prefabs referred to by this spawner from your scene?\n\n" +
                    "NOTE: This will also clear the Undo History.",
                    "OK", "Cancel"))
                {
                    m_spawner.UnspawnAllPrefabs();
                    SpawnerToCache();
                }
                Event.current.Use();
                GUIUtility.hotControl = 0;
                return;
            }

            // CTRL ALT Z: Undo
            if (Event.current.control && Event.current.alt && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Z)
            {
                Undo();
                Event.current.Use();
                return;
            }

            //Scroll wheel
            if (Event.current.type == EventType.ScrollWheel)
            {
                if (Event.current.control)
                {
                    m_spawner.m_minInstances -= (int)Event.current.delta.y;
                    if (m_spawner.m_minInstances < 1)
                    {
                        m_spawner.m_minInstances = 1;
                    }
                    Event.current.Use();
                    //Settings changed, let's update ranges - probably no need to update child spawners, since their settings did not change.
                    m_spawner.UpdateTargetSpawnerRanges(false);
                    SpawnerToCache();
                    SceneView.RepaintAll();
                    return;
                }
                else if (Event.current.shift)
                {
                    m_spawner.m_maxSpawnRange -= Event.current.delta.y;
                    if (m_spawner.m_maxSpawnRange < 1f)
                    {
                        m_spawner.m_maxSpawnRange = 1f;
                    }
                    Event.current.Use();
                    //Settings changed, let's update ranges - probably no need to update child spawners, since their settings did not change.
                    m_spawner.UpdateTargetSpawnerRanges(false);
                    SpawnerToCache();
                    SceneView.RepaintAll();
                    return;
                }
            }

            //Check for the shift + ctrl + left mouse button event - spawn entire terrain
            if (Event.current.shift == true && Event.current.control == true && Event.current.isMouse == true)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    if (raycastHit && ResourcesReady(m_spawner))
                    {
                        GUIUtility.hotControl = m_editor_control_id;

                        GlobalSpawnDialog window = EditorWindow.GetWindow<GlobalSpawnDialog>(true, "GeNa Global Spawn", true);
                        window.Init(m_spawner, hitInfo);

                        m_lastSpawn.Clear();
                        m_lastSpawn.Add(new SpawnCall(hitInfo.transform, hitInfo.point, hitInfo.normal, true));
                    }
                    return;
                }
                return;
            }

            //Check for the ctrl + left mouse button event - spawn
            if (Event.current.control == true && Event.current.isMouse == true)
            {
                // Left button
                if (Event.current.button == 0)
                {
                    switch (Event.current.type)
                    {
                        case EventType.MouseDown:
                            GUIUtility.hotControl = m_editor_control_id;

                            if (raycastHit && ResourcesReady(m_spawner))
                            {
                                m_mouseDownForSpawn = true;
                                // Paint also starts with a mouse down, so we don't need to record before snapshot separately for it
                                m_spawner.Initialise(hitInfo.transform, "Spawn");

                                m_lastSpawn.Clear();
                                SpawnCall spawnCall = new SpawnCall(hitInfo.transform, hitInfo.point, hitInfo.normal);
                                m_spawnLocations.Add(spawnCall);
                                m_lastSpawn.Add(spawnCall);
                                EditorApplication.delayCall -= SpawnAfterEditorUpdate;
                                EditorApplication.delayCall += SpawnAfterEditorUpdate;
                            }
                            SceneView.RepaintAll();
                            break;
                        case EventType.MouseDrag:
                            // Paint Mode
                            if (GUIUtility.hotControl == m_editor_control_id && m_spawner.m_spawnMode == Constants.SpawnMode.Paint)
                            {
                                if (raycastHit)
                                {
                                    if (m_spawnLocations.Count > 0)
                                    {
                                        if ((hitInfo.point - m_spawnLocations[m_spawnLocations.Count - 1].m_location).sqrMagnitude > m_spawner.m_flowDistanceSqr)
                                        {
                                            m_paintSpawn = true;

                                            SpawnCall spawnCall = new SpawnCall(hitInfo.point);
                                            m_spawnLocations.Add(spawnCall);
                                            m_lastSpawn.Add(spawnCall);
                                            EditorApplication.delayCall -= SpawnAfterEditorUpdate;
                                            EditorApplication.delayCall += SpawnAfterEditorUpdate;
                                        }
                                    }
                                    else
                                    {
                                        if ((hitInfo.point - m_spawner.m_spawnOriginLocation).sqrMagnitude > m_spawner.m_flowDistanceSqr)
                                        {
                                            m_paintSpawn = true;

                                            SpawnCall spawnCall = new SpawnCall(hitInfo.point);
                                            m_spawnLocations.Add(spawnCall);
                                            m_lastSpawn.Add(spawnCall);
                                            EditorApplication.delayCall -= SpawnAfterEditorUpdate;
                                            EditorApplication.delayCall += SpawnAfterEditorUpdate;
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                return;
            }

            //Check for the CTRL + SHIFT + I - Iterate
            if (Event.current.control && Event.current.shift && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.I)
			{
				Event.current.Use();

				if (m_lastSpawn.Count < 1)
                {
                    // Nothing to iterate
                    return;
                }
                Undo();

				// If it was a global spawn
				if (m_lastSpawn[0].m_globalSpawn)
                {
                    if (m_lastSpawn.Count > 1)
                    {
                        Debug.LogWarningFormat("[GeNa] Last Spawn was Global but the iteration list contains more than 1 spawns: {0} > 1", m_lastSpawn.Count);
                    }

					// Hack: Need to do it in the delayed call, because Unity doesn't handle its own progress bars well lately.
					m_globalIteration = new SpawnCall(m_lastSpawn[0].m_transform, m_lastSpawn[0].m_location, m_lastSpawn[0].m_normal, true);
					EditorApplication.delayCall -= SpawnAfterEditorUpdate;
					EditorApplication.delayCall += SpawnAfterEditorUpdate;

					return;
				}

				// Single spawn, or painting
				m_spawner.Initialise(hitInfo.transform, "Spawn");
				m_spawnLocations = new List<SpawnCall>(m_lastSpawn);
				EditorApplication.delayCall -= SpawnAfterEditorUpdate;
				EditorApplication.delayCall += SpawnAfterEditorUpdate;
				return;
            }

            //Check for the SHIFT (show/move visualization) and SHIFT + left mouse events (update ranges + move visualisation - drag rotation)
            if (Event.current.shift == true)
            {
                //SHIFT is down -> visualization is active
                m_spawner.VisualizationActive = true;

                //If SHIFT and CONTROL both down, update the location of the visualization
                if (Event.current.control == true && m_spawner.m_showGizmos && raycastHit)
                {
                    m_spawner.SetSpawnOrigin(hitInfo.transform, hitInfo.point, hitInfo.normal);
                    SpawnerToCache();
                    m_spawner.m_needsVisualisationUpdate = true;
                }

                if (Event.current.isMouse == true)
                {
                    // Left button
                    if (Event.current.button == 0)
                    {
                        if (Event.current.type == EventType.MouseDown)
                        {
                            GUIUtility.hotControl = m_editor_control_id;
                            if (raycastHit)
                            {
                                m_spawner.m_showGizmos = m_tmpSpawner.m_showGizmos = true;
                                m_spawner.SetSpawnOrigin(hitInfo.transform, hitInfo.point, hitInfo.normal);
                                //Let's update ranges - including child spawners.
                                m_spawner.UpdateTargetSpawnerRanges(hitInfo, true);
                                SpawnerToCache();
                                SceneView.RepaintAll();
                                return;
                            }
                        }
                        else if (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl == m_editor_control_id &&
                                 m_spawner.m_rotationAlgorithm == Constants.RotationAlgorithm.Fixed &&
                                 m_spawner.m_enableRotationDragUpdate)
                        {
                            if (raycastHit)
                            {
                                Quaternion rot = Quaternion.LookRotation(hitInfo.point - m_spawner.m_spawnOriginLocation);
                                m_spawner.m_minRotationY = m_tmpSpawner.m_minRotationY = m_spawner.m_maxRotationY = rot.eulerAngles.y;
                                SceneView.RepaintAll();
                                return;
                            }
                        }
                    }
                }
            }

            //Visualise it
            if (m_spawner.m_rotationAlgorithm <= Constants.RotationAlgorithm.Fixed)
            {
                if (HasTrees(m_spawner.m_spawnPrototypes) || HasPrefabs(m_spawner.m_spawnPrototypes) || m_spawner.m_critMaskType == Constants.MaskType.Image)
                {
                    Handles.color = new Color(0f, 255f, 0f, 0.25f);
                    if (Spawner.ApproximatelyEqual(m_spawner.m_minRotationY, m_spawner.m_maxRotationY))
                    {
                        //Debug.LogFormat("drawing arrow at {0} with length {1}", m_spawner.m_spawnOriginLocation, Mathf.Clamp(m_spawner.m_maxSpawnRange / 6f, 0.25f, 1f));
                        Quaternion visRot = Quaternion.Euler(0f, m_spawner.m_minRotationY, 0f);
                        Handles.ArrowHandleCap(m_editor_control_id, m_spawner.m_spawnOriginLocation, visRot, Mathf.Clamp(m_spawner.m_maxSpawnRange * 0.2f, 0.25f, 40f), EventType.Repaint);
                    }
                    else
                    {
                        Handles.DrawSolidArc(m_spawner.m_spawnOriginLocation, Vector3.up, Quaternion.AngleAxis(m_spawner.m_minRotationY, Vector3.up) * Vector3.forward, m_spawner.m_maxRotationY - m_spawner.m_minRotationY, Mathf.Clamp(m_spawner.m_maxSpawnRange / 6f, 0.25f, 1f));
                    }
                }
            }
        }

        /// <summary>
        /// Shows the outline of the spawn range and does the raycasting.
        /// </summary>
        /// <returns>The Raycast hit info.</returns>
        private bool ShowSpawnRange(out RaycastHit hitInfo)
        {
            //Stop if not over the SceneView
            Vector2 mousePos;
            if (!MouseOverSceneView(out mousePos))
            {
                hitInfo = new RaycastHit();
                return false;
            }

            //bool inside = MouseOverSceneView(out mousePos);
            //Handles.Label(new Vector3(-395f, 60f, -316f), string.Format("{0} is {1}in {2}", mousePos, inside ? "" : "not ", SceneView.lastActiveSceneView.position.ToString()));

            //Let's do the raycast first
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            if (Physics.Raycast(ray, out hitInfo, 10000f, m_spawner.m_critSpawnCollisionLayers))
            {
                Vector3 origin = hitInfo.point;
                Vector3 aboveOrigin = origin;
                aboveOrigin.y += 5000f;
                RaycastHit hitInfo2;

                Vector3[] nodes;
                Vector3[] innerNodes;

                switch (m_spawner.m_spawnRangeShape)
                {
                    case Constants.SpawnRangeShape.Circle:
                        int res = Mathf.CeilToInt(24 + m_spawner.m_maxSpawnRange * 0.1f);
                        nodes = new Vector3[res];
                        innerNodes = new Vector3[res];

                        float step = 360f / (res - 1);
                        float radius = m_spawner.m_maxSpawnRange * 0.5f;
                        float innerRadius = radius * 0.99f;

                        for (int i = 0; i < nodes.Length; i++)
                        {
                            nodes[i] = new Vector3(Mathf.Sin(step * i * Mathf.Deg2Rad), 0f, Mathf.Cos(step * i * Mathf.Deg2Rad)) * radius + aboveOrigin;
                            innerNodes[i] = new Vector3(Mathf.Sin(step * i * Mathf.Deg2Rad), 0f, Mathf.Cos(step * i * Mathf.Deg2Rad)) * innerRadius + aboveOrigin;
                            nodes[i].y = innerNodes[i].y = GetEdgeHeight(ray, out hitInfo2, nodes[i], origin.y);
                        }
                        break;
                    case Constants.SpawnRangeShape.Square:
                        res = 6 + Mathf.CeilToInt(m_spawner.m_maxSpawnRange * 0.1f);
                        nodes = new Vector3[res * 4 + 1];
                        innerNodes = new Vector3[res * 4 + 1];

                        step = m_spawner.m_maxSpawnRange / res;
                        radius = m_spawner.m_maxSpawnRange * 0.5f;
                        innerRadius = radius * 0.99f;
                        int ix;

                        for (int i = 0; i < res; i++)
                        {
                            ix = i;
                            nodes[ix] = new Vector3(-radius, 0f, -radius + i * step) + aboveOrigin;
                            innerNodes[ix] = new Vector3(-innerRadius, 0f, -innerRadius + i * step) + aboveOrigin;
                            nodes[ix].y = innerNodes[ix].y = GetEdgeHeight(ray, out hitInfo2, nodes[ix], origin.y);

                            ix += res;
                            nodes[ix] = new Vector3(-radius + i * step, 0f, radius) + aboveOrigin;
                            innerNodes[ix] = new Vector3(-innerRadius + i * step, 0f, innerRadius) + aboveOrigin;
                            nodes[ix].y = innerNodes[ix].y = GetEdgeHeight(ray, out hitInfo2, nodes[ix], origin.y);

                            ix += res;
                            nodes[ix] = new Vector3(radius, 0f, radius - i * step) + aboveOrigin;
                            innerNodes[ix] = new Vector3(innerRadius, 0f, innerRadius - i * step) + aboveOrigin;
                            nodes[ix].y = innerNodes[ix].y = GetEdgeHeight(ray, out hitInfo2, nodes[ix], origin.y);

                            ix += res;
                            nodes[ix] = new Vector3(radius - i * step, 0f, -radius) + aboveOrigin;
                            innerNodes[ix] = new Vector3(innerRadius - i * step, 0f, -innerRadius) + aboveOrigin;
                            nodes[ix].y = innerNodes[ix].y = GetEdgeHeight(ray, out hitInfo2, nodes[ix], origin.y);
                        }
                        //And close the "circle"
                        nodes[nodes.Length - 1] = nodes[0];
                        innerNodes[innerNodes.Length - 1] = innerNodes[0];
                        break;
                    default:
                        throw new NotImplementedException("[GeNa] doesn't know this Spawn Shape: " + m_spawner.m_spawnRangeShape.ToString());
                }


                Handles.color = Color.blue;
                Handles.DrawAAPolyLine(6f, nodes);
                Handles.color = Color.white;
                Handles.DrawAAPolyLine(1f, innerNodes);
                //Handles.DrawLines(embankmentCorners);

                // We only got here if the mouse is over the sceneview - also only update if there was more than tiny movement of the mouse
                if ((m_lastMousePos - mousePos).sqrMagnitude > 4f)
                {
                    m_lastMousePos = mousePos;
                    SceneView.lastActiveSceneView.Repaint();
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the mouse is over the SceneView
        /// </summary>
        private bool MouseOverSceneView()
        {
            Vector2 mousePos;
            return MouseOverSceneView(out mousePos);
        }

        /// <summary>
        /// Checks if the mouse is over the SceneView
        /// </summary>
        private bool MouseOverSceneView(out Vector2 mousePos)
        {
            mousePos = Event.current.mousePosition;
            if (mousePos.x < 0f || mousePos.y < 0f)
            {
                return false;
            }

            Rect swPos = SceneView.lastActiveSceneView.position;
            if (mousePos.x > swPos.width || mousePos.y > swPos.height)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get's height for the spawn edge, or returns the default value.
        /// </summary>
        private float GetEdgeHeight(Ray ray, out RaycastHit hitInfo, Vector3 origin, float defVal)
        {
            ray = new Ray(origin, Vector3.down);
            if (Physics.Raycast(ray, out hitInfo, 10000f, m_spawner.m_critSpawnCollisionLayers))
            {
                return hitInfo.point.y;
            }
            else
            {
                return defVal;
            }
        }

        /// <summary>
        /// Checks that the prefabs of resources are still available and warns the user if they cannot be found.
        /// </summary>
        private bool ResourcesReady(Spawner spawner)
        {
            bool allOk = true;

            List<Prototype> protoList = spawner.m_spawnPrototypes;

            for (int protoIdx = 0; protoIdx < protoList.Count; protoIdx++)
            {
                for (int resIdx = 0; resIdx < protoList[protoIdx].m_resourceTree.Count; resIdx++)
                {
                    if (protoList[protoIdx].m_resourceTree[resIdx].m_resourceType == Constants.ResourceType.Prefab)
                    {
                        allOk = LookForMissingAsset(allOk, protoList[protoIdx].m_resourceTree[resIdx]);
                    }
                }
            }

            return allOk;
        }

        /// <summary>
        /// Looks for missing Assets down a tree, attempts to recover them and returns true if all have been accounted for.
        /// </summary>
        private bool LookForMissingAsset(bool allOk, Resource res)
        {
            // If the prefab is missing
            if (res.m_prefab == null && res.ContainerOnly == false)
            {
                // Attempt to find by GUID - This won't be needed, since if we have the Asset with the GUID, then we have the Asset. Leaving here for good measure.
                string path = AssetDatabase.GUIDToAssetPath(res.m_assetID);
                if (string.IsNullOrEmpty(path) == false)
                {
                    res.m_prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (res.m_prefab != null)
                    {
                        Debug.LogWarningFormat("[GeNa]: Identified missing asset for '{0}' by ID at path '{1}'.", res.m_name, path);
                        UpdateResourceIDs();
                    }
                }

                // If still missing
                if (res.m_prefab == null)
                {
                    // Attempt to find by name
                    if (string.IsNullOrEmpty(res.m_assetName) == false)
                    {
                        res.m_prefab = AssetUtils.GetAssetPrefab(res.m_assetName);
                        if (res.m_prefab != null)
                        {
                            Debug.LogWarningFormat("[GeNa]: Identified missing asset for '{0}' by filename at path '{1}'.", res.m_name, AssetDatabase.GetAssetPath(res.m_prefab));
                            UpdateResourceIDs();
                        }
                    }

                    if (res.m_prefab == null)
                    {
                        Debug.LogErrorFormat("Spawn aborted. Could not find the prefab for {0}", res.m_name);
                        allOk = false;
                    }
                }
            }

            for (int i = 0; i < res.Children.Count; i++)
            {
                allOk = LookForMissingAsset(allOk, res.Children[i]);
            }

            return allOk;
        }

        /// <summary>
        /// If spawning on a terrain, records terrain related undos, so the user can undo texture, tree and detail spawns.
        /// </summary>
        private void RecordTerrainUndo()
        {
            // HasTerrainProto is not perfect but will avoid most unneeded undo recording
            if (m_spawner.m_spawnOriginIsTerrain && (m_spawner.m_hasActiveTreeProtos || m_spawner.m_hasActiveGrassProtos || m_spawner.m_hasActiveTextureProtos))
            {
                Terrain terrain = Terrain.activeTerrain;

                if (terrain != null)
                {
                    string undoMsg = string.Format("[{0}] Terrain Spawn", PWApp.CONF.Name, m_spawner.m_parentName);
                    // Register the terrain
                    UnityEditor.Undo.RegisterCompleteObjectUndo(terrain.terrainData, undoMsg);
                    UnityEditor.Undo.RegisterCompleteObjectUndo(terrain.terrainData.alphamapTextures, undoMsg);
                }
            }
        }

        /// <summary>
        /// Undo all
        /// </summary>
        private void UndoAll()
        {
            GeNaUndo.UndoAll(m_spawner);
            SpawnerToCache();
        }

        /// <summary>
        /// Undo
        /// </summary>
        private void Undo(int count)
        {
            GeNaUndo.Undo(m_spawner, count);
            SpawnerToCache();
        }

        /// <summary>
        /// Undo
        /// </summary>
        private void Undo()
        {
            GeNaUndo.Undo(m_spawner);
            SpawnerToCache();
        }

        /// <summary>
        /// Hack to get around long running editor update issues
        /// </summary>
        void SpawnAfterEditorUpdate()
        {
            if (m_spawner != null)
            {
				// Hack to handle Global Spawn Iteration, because Unity doesn't handle its own progress bars well lately.
				if (m_globalIteration != null && m_globalIteration.m_globalSpawn)
				{
					m_spawner.GlobalSpawn(m_globalIteration.m_location, m_globalIteration.m_normal, m_globalIteration.m_transform);
					m_globalIteration = null;

					// We are done if this was only a Global Spawn Iteration
					if (m_spawnLocations.Count <= 0)
					{
						EditorApplication.delayCall -= SpawnAfterEditorUpdate;
						return;
					}
				}

                while (m_spawnLocations.Count > 0)
                {
                    SpawnCall sc = m_spawnLocations[0];
                    if (!sc.m_isSubspawn)
                    {
                        m_spawner.SetSpawnOrigin(sc.m_transform, sc.m_location, sc.m_normal);
                    }
                    m_spawner.Spawn(sc.m_location, sc.m_isSubspawn);
                    m_spawnLocations.RemoveAt(0);
                }
				EditorApplication.delayCall -= SpawnAfterEditorUpdate;

                // Undo either gets recorded at mouse up, or when the spawn is complete, 
                // depending on if mouse up happens before or after all the spawning completes
                // in SpawnAfterEditorUpdate()
                if (!m_mouseDownForSpawn)
                {
                    if (m_paintSpawn)
                    {
                        m_spawner.RecordUndo("Painted Spawn");
                        m_paintSpawn = false;
                    }
                    else
                    {
                        m_spawner.RecordUndo("Single Spawn");
                    }
                    SpawnerToCache();
                }
                SceneView.RepaintAll();
            }
        }

        /// <summary>
        /// Draws a separator between secitons
        /// </summary>
        private void Separator(Rect widthRect)
        {
            GUILayout.Space(5f);
            Rect r = GUILayoutUtility.GetLastRect();
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = m_separatorColor;
            Handles.DrawLine(new Vector3(widthRect.xMin, r.yMax), new Vector3(widthRect.xMax, r.yMax));
            Handles.color = oldColor;
            Handles.EndGUI();
        }

        private class Styles : EditorUtils.CommonStyles
        {
            public GUIStyle gpanel;
            public GUIStyle wrappedText;
            public GUIStyle resFlagsPanel;
            public GUIStyle resTreeFoldout;

            public GUIStyle staticResHeader;
            public GUIStyle dynamicResHeader;

            // Unity's bold label has placement issues
            public GUIStyle boldLabel;

            public GUIStyle advancedToggle;
            public GUIStyle advancedToggleDown;

            public GUIStyle helpNoWrap;

            public GUIStyle linkLabel;
            public GUIStyle linkPanelLabel;

            public GUIStyle boxLabelLeft;
            public GUIStyle boxWithLeftLabel;

            public GUIStyle addBtn;

            public GUIStyle inlineToggleBtn;
            public GUIStyle inlineToggleBtnDown;

            public GUIStyle areaDebug;

			public Texture2D undoIco;

            public Styles()
            {
                areaDebug = new GUIStyle("label");
                areaDebug.normal.background = GetBGTexture(GetColorFromHTML("#ff000055"));

                //Set up the box style
                gpanel = new GUIStyle(GUI.skin.box);
                gpanel.normal.textColor = GUI.skin.label.normal.textColor;
                gpanel.fontStyle = FontStyle.Bold;
                gpanel.alignment = TextAnchor.UpperLeft;

                boxLabelLeft = new GUIStyle(gpanel);
                boxLabelLeft.richText = true;
                boxLabelLeft.wordWrap = false;
                boxLabelLeft.margin.right = 0;
                boxLabelLeft.overflow.right = 1;

                boxWithLeftLabel = new GUIStyle(gpanel);
                boxWithLeftLabel.richText = true;
                boxWithLeftLabel.wordWrap = false;
                boxWithLeftLabel.margin.left = 0;

                // Add button
                addBtn = new GUIStyle("button");
                addBtn.margin = new RectOffset(4, 4, 0, 0);

                // Inline toggle button
                inlineToggleBtn = new GUIStyle(toggleButton);
                inlineToggleBtn.margin = deleteButton.margin;
                inlineToggleBtnDown = new GUIStyle(toggleButtonDown);
                inlineToggleBtnDown.margin = inlineToggleBtn.margin;

                //Resource tree
                resTreeFoldout = new GUIStyle(EditorStyles.foldout);
                resTreeFoldout.fontStyle = FontStyle.Bold;

                resFlagsPanel = new GUIStyle(GUI.skin.window);
                resFlagsPanel.normal.textColor = GUI.skin.label.normal.textColor;
                //resFlagsPanel.fontStyle = FontStyle.Bold;
                resFlagsPanel.alignment = TextAnchor.UpperCenter;
                resFlagsPanel.margin = new RectOffset(0, 0, 5, 7);
                resFlagsPanel.padding = new RectOffset(10, 10, 3, 3);
                resFlagsPanel.stretchWidth = true;
                resFlagsPanel.stretchHeight = false;

                //Setup the wrap style
                wrappedText = new GUIStyle(GUI.skin.label);
                wrappedText.fontStyle = FontStyle.Normal;
                wrappedText.wordWrap = true;

                staticResHeader = new GUIStyle();
                staticResHeader.overflow = new RectOffset(2, 2, 2, 2);

                dynamicResHeader = new GUIStyle(staticResHeader);

                //Bold label
                boldLabel = new GUIStyle("Label");
                boldLabel.fontStyle = FontStyle.Bold;

                //Advanced toggle
                advancedToggle = toggleButton;
                advancedToggle.padding = new RectOffset(5, 5, 0, 0);
                advancedToggle.margin = deleteButton.margin;
                advancedToggle.fixedHeight = deleteButton.fixedHeight;

                advancedToggleDown = toggleButtonDown;
                advancedToggleDown.padding = advancedToggle.padding;
                advancedToggleDown.margin = advancedToggle.margin;
                advancedToggleDown.fixedHeight = advancedToggle.fixedHeight;

                //Help
                helpNoWrap = new GUIStyle(help);
                helpNoWrap.wordWrap = false;

                //Linc ikon
                linkLabel = new GUIStyle(richLabel);
                linkLabel.contentOffset = new Vector2(0f, -2f);
                linkLabel.padding = new RectOffset(2, 2, 0, 0);

                linkPanelLabel = new GUIStyle(panelLabel);
                linkPanelLabel.contentOffset = new Vector2(0f, -2f);
                linkPanelLabel.padding = new RectOffset(2, 2, 0, 0);

				undoIco = Resources.Load("pwundo" + PWConst.VERSION_IN_FILENAMES) as Texture2D;

				// Setup colors for Unity Pro
				if (EditorGUIUtility.isProSkin)
                {
                    resFlagsPanel.normal.background = Resources.Load("pwdarkBoxp" + PWConst.VERSION_IN_FILENAMES) as Texture2D;

                    staticResHeader.normal.background = GetBGTexture(GetColorFromHTML("2d2d2dff"));
                    dynamicResHeader.normal.background = GetBGTexture(GetColorFromHTML("2d2d4cff"));
                }
                // or Unity Personal
                else
                {
                    resFlagsPanel.normal.background = Resources.Load("pwdarkBox" + PWConst.VERSION_IN_FILENAMES) as Texture2D;

                    staticResHeader.normal.background = GetBGTexture(GetColorFromHTML("a2a2a2ff"));
                    dynamicResHeader.normal.background = GetBGTexture(GetColorFromHTML("a2a2c1ff"));
                }
            }
        }
    }
}
