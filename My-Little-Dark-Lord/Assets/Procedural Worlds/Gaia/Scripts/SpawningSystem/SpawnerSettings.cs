// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using System.Collections.Generic;
using static Gaia.GaiaConstants;
using UnityEditor;

/*
 * Scriptable Object containing settings for a Spawner
 */

namespace Gaia
{
    /// <summary> Contains information about a Sequence of clips to play and how </summary>
    [CreateAssetMenu(menuName = "Procedural Worlds/Gaia/Spawner Settings")]
    [System.Serializable]
    public class SpawnerSettings : ScriptableObject, ISerializationCallbackReceiver
    {

        #region Public Variables

        /// <summary>
        /// The resources associated with the spawner
        /// </summary>
        public GaiaResource m_resources = new GaiaResource();

        ///// <summary>
        ///// Spanwer x location - done this way to expose in the editor as a simple slider
        ///// </summary>
        public float m_x = 0f;

        ///// <summary>
        ///// Spawner y location - done this way to expose in the editor as a simple slider
        ///// </summary>
        public float m_y = 50f;

        ///// <summary>
        ///// Spawner z location - done this way to expose in the editor as a simple slider
        ///// </summary>
        public float m_z = 0f;

        ///// <summary>
        ///// Spawner width - this is the horizontal scaling factor - applied to both x & z
        ///// </summary>
        public float m_width = 10f;

        ///// <summary>
        ///// Spawner height - this is the vertical scaling factor
        ///// </summary>
        public float m_height = 10f;

        ///// <summary>
        ///// Spawner rotation
        ///// </summary>
        public float m_rotation = 0f;


        /// <summary>
        /// Range of the spawn area
        /// </summary>
        public float m_spawnRange = 500f;



        


        /// <summary>
        /// The path this resources file came from
        /// </summary>
        public string m_resourcesPath;


        /// <summary>
        /// The GUID of the last used resources file; Used while saving and loading to save / load the resource file reference
        /// </summary>
        //public string m_resourcesGUID;


        /// <summary>
        /// The prefabs that can be spawned and their settings
        /// </summary>
        public List<SpawnRule> m_spawnerRules = new List<SpawnRule>();

        /// <summary>
        /// Whether or not to show gizmos
        /// </summary>
        public bool m_showGizmos = true;

        /// <summary>
        /// Whether or not to show debug messages
        /// </summary>
        public bool m_showDebug = false;

        /// <summary>
        /// Whether or not to show the terrain helper
        /// </summary>
        public bool m_showTerrainHelper = true;


        public SpawnMode spawnMode = SpawnMode.Replace;

        [SerializeField]
        private ImageMask[] imageMasks = new ImageMask[0];
        //Using a property to make sure the image mask list is always initialized
        //<summary>All image filters that are being applied in this spawning process</summary>
        
        public ImageMask[] m_imageMasks {
                                            get
                                            {
                                                if (imageMasks == null)
                                                {
                                                    imageMasks = new ImageMask[0];
                                                }
                                                return imageMasks;
                                            }
                                            set
                                            {
                                                imageMasks = value;  
                                            }
        }
        
        //public  float m_powerOf;



        #endregion
        #region Serialization

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }

        #endregion


        public Spawner CreateSpawner(bool autoAddResources = false, Transform targetTransform = null)
        {
            //Find or create gaia
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }
            GameObject spawnerObj = new GameObject(this.name);
            spawnerObj.AddComponent<Spawner>();
            if (targetTransform != null)
            {
                spawnerObj.transform.parent = targetTransform;
            }
            else
            {
                spawnerObj.transform.parent = gaiaObj.transform;
            }

            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.LoadSettings(this);
            //spawner.m_settings.m_resources = (GaiaResource)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(this.m_resourcesGUID), typeof(GaiaResource));
            if (autoAddResources)
            {
                TerrainLayer[] terrainLayers = new TerrainLayer[0];
                DetailPrototype[] terrainDetails = new DetailPrototype[0];
                TreePrototype[] terrainTrees = new TreePrototype[0];
                GaiaDefaults.GetPrototypes(new List<BiomeSpawnerListEntry>() { new BiomeSpawnerListEntry() {m_spawnerSettings = this, m_autoAssignPrototypes=true } }, ref terrainLayers, ref terrainDetails, ref terrainTrees, Terrain.activeTerrain);

                foreach (Terrain t in Terrain.activeTerrains)
                {
                    GaiaDefaults.ApplyPrototypesToTerrain(t, terrainLayers, terrainDetails, terrainTrees);
                }
            }

            foreach (SpawnRule rule in spawner.m_settings.m_spawnerRules)
            {
                rule.m_spawnedInstances = 0;
            }

            if (Terrain.activeTerrains.Length > 0)
            {
                spawner.FitToAllTerrains();
            }
            else
            {
                spawner.FitToTerrain();
            }
            return spawner;
        }

    }
}
