using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Gaia.Pipeline
{
    public enum GaiaPackageVersion { Unity2019_1, Unity2019_2, Unity2019_3 }

    [System.Serializable]
    public class GaiaPackageSettings
    {
        [Header("Global Settings")]
        public string m_version = "Add Unity Version... Example: 2019.1, 2019.2";
        public bool m_isSupported = true;
        public GaiaPackageVersion m_unityVersion = GaiaPackageVersion.Unity2019_1;
    }

    public class UnityPipelineProfile : ScriptableObject
    {
        [Header("Global Settings")]
        public GaiaConstants.EnvironmentRenderer m_activePipelineInstalled = GaiaConstants.EnvironmentRenderer.BuiltIn;
        public GaiaSettings m_gaiaSettings;
        [HideInInspector]
        public bool m_editorUpdates = false;
        [HideInInspector]
        public bool m_pipelineSwitchUpdates = false;

        [Header("Material Settings")]
        public Material m_underwaterHorizonMaterial;

        [Header("Pipeline Supported Versions 2019.1")]
        [HideInInspector]
        public string m_min2019_1LWVersion = "5.7.2";
        [HideInInspector]
        public string m_max2019_1LWVersion = "5.13.0";
        [HideInInspector]
        public string m_min2019_1HDVersion = "5.7.2";
        [HideInInspector]
        public string m_max2019_1HDVersion = "5.13.0";

        [Header("Pipeline Supported Versions 2019.2")]
        [HideInInspector]
        public string m_min2019_2LWVersion = "5.7.2";
        [HideInInspector]
        public string m_max2019_2LWVersion = "5.13.0";
        [HideInInspector]
        public string m_min2019_2HDVersion = "5.7.2";
        [HideInInspector]
        public string m_max2019_2HDVersion = "5.13.0";

        [Header("Built-In Render Pipeline")]
        //public string m_builtInWaterShader = "Add Built In Shader";
        public string m_builtInHorizonObjectShader = "Standard";
        public Material m_builtInTerrainMaterial;
        public bool m_BuiltInAutoConfigureWater = true;

        [Header("Lightweight Render Pipeline")]
        public string m_lightweightPipelineProfile = "Procedural Worlds Lightweight Pipeline Profile";
        public string m_lightweightScriptDefine = "LWPipeline";
        public string m_lightweightHorizonObjectShader = "Lightweight Render Pipeline/Lit";
        public Material m_lightweightTerrainMaterial;
        public bool m_setLWPipelineProfile = true;
        public bool m_LWAutoConfigureTerrain = true;
        public bool m_LWAutoConfigureWater = true;
        public bool m_LWAutoConfigureCamera = true;
        public bool m_LWAutoConfigureProbes = true;
        public bool m_LWAutoConfigureLighting = true;

        [Header("High Definition Render Pipeline")]
        public string m_highDefinitionPipelineProfile = "Procedural Worlds HDRenderPipelineAsset";
        public string m_highDefinitionScriptDefine = "HDPipeline";
        public string m_highDefinitionHorizonObjectShader = "HDRP/Lit";
        public Material m_highDefinitionTerrainMaterial;
        public string m_HDVolumeObjectName = "HD Environment Volume";
        public string m_HDPostVolumeObjectName = "HD Post Processing Environment Volume";
        public string m_HDDefaultPostProcessing = "Default HDRP Post Processing Profile";
        public string m_HDDefaultSceneLighting = "Default Volume Profile";
        public string m_2019_3HDDefaultSceneLighting = "Default Volume Profile";
        public string m_HDSceneLighting = "HD Volume Profile";
        public string m_2019_3HDSceneLighting = "HD Volume Profile";
        public bool m_setHDPipelineProfile = true;
        public bool m_HDAutoConfigureTerrain = true;
        public bool m_HDAutoConfigureWater = true;
        public bool m_HDAutoConfigureCamera = true;
        public bool m_HDAutoConfigureProbes = true;
        public bool m_HDAutoConfigureLighting = true;

        [Header("Package Settings")]
        public Material[] m_vegetationMaterialLibrary;
        public Material[] m_waterMaterialLibrary;
        public GaiaPackageSettings[] m_packageSetupSettings;

        /// <summary>
        /// Create Gaia Lighting Profile asset
        /// </summary>
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Procedural Worlds/Gaia/Unity Pipeline Profile")]
        public static void CreateSkyProfiles()
        {
            UnityPipelineProfile asset = ScriptableObject.CreateInstance<UnityPipelineProfile>();
            AssetDatabase.CreateAsset(asset, "Assets/Unity Pipeline Profile.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
#endif
    }
}