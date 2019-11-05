﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ProcedualWorlds.WaterSystem.MeshGeneration;
using ProcedualWorlds.WaterSystem.Reflections;

namespace Gaia
{
    public class GaiaWaterProfile : ScriptableObject
    {
        [HideInInspector]
        public bool m_updateInRealtime = false;
        [HideInInspector]
        public bool m_allowMSAA = false;
        [HideInInspector]
        public bool m_useHDR = false;
        [HideInInspector]
        public GaiaConstants.GaiaWaterProfileType m_waterProfile = GaiaConstants.GaiaWaterProfileType.DeepBlueOcean;
        [HideInInspector]
        public PWS_WaterReflections.RenderUpdateMode m_waterRenderUpdateMode = PWS_WaterReflections.RenderUpdateMode.Update;
        [HideInInspector]
        public float m_interval = 0.25f;
        [HideInInspector]
        public bool m_editSettings = false;

        [HideInInspector]
        public Material m_masterWaterMaterial;
        [HideInInspector]
        public GameObject m_waterPrefab;
        [HideInInspector]
        public GameObject m_underwaterParticles;
        [HideInInspector]
        public GameObject m_underwaterHorizonPrefab;

        public List<GaiaWaterProfileValues> m_waterProfiles = new List<GaiaWaterProfileValues>();

        [HideInInspector]
        public bool m_enableWaterMeshQuality = false;
        [HideInInspector]
        public GaiaConstants.WaterMeshQuality m_waterMeshQuality = GaiaConstants.WaterMeshQuality.Medium;
        public PWS_MeshGenerationPro.MeshType m_meshType = PWS_MeshGenerationPro.MeshType.Plane;
        [HideInInspector]
        public int m_zSize = 1000;
        [HideInInspector]
        public int m_xSize = 1000;
        [HideInInspector]
        public int m_customMeshQuality = 100;

        [HideInInspector]
        public bool m_enableReflections = true;
        [HideInInspector]
        public bool m_disablePixelLights = true;
        [HideInInspector]
        public GaiaConstants.GaiaProWaterReflectionsQuality m_reflectionResolution = GaiaConstants.GaiaProWaterReflectionsQuality.Resolution512;
        [HideInInspector]
        public float m_clipPlaneOffset = 40f;
        [HideInInspector]
        public LayerMask m_reflectedLayers = 1;

        [HideInInspector]
        public bool m_enableOceanFoam = true;
        [HideInInspector]
        public bool m_enableBeachFoam = true;
        [HideInInspector]
        public bool m_enableGPUInstancing = true;
        [HideInInspector]
        public bool m_autoWindControlOnWater = true;

        [HideInInspector]
        public bool m_supportUnderwaterEffects = true;
        [HideInInspector]
        public bool m_supportUnderwaterPostProcessing = true;
        [HideInInspector]
        public bool m_supportUnderwaterFog = true;
        [HideInInspector]
        public bool m_supportUnderwaterParticles = true;

        /// <summary>
        /// Create Gaia Lighting System Profile asset
        /// </summary>
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Procedural Worlds/Gaia/Water Profile")]
        public static void CreateSkyProfiles()
        {
            GaiaWaterProfile asset = ScriptableObject.CreateInstance<GaiaWaterProfile>();
            AssetDatabase.CreateAsset(asset, "Assets/Gaia Water System Profile.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
#endif
    }
}