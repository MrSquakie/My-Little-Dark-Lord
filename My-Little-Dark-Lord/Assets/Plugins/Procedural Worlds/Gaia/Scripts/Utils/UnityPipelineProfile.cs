using UnityEngine;
using UnityEditor;

namespace Gaia.Pipeline
{
    public enum GaiaPackageVersion { Unity2019_1, Unity2019_2, Unity2019_3 }

    [System.Serializable]
    public class GaiaPackageSettings
    {
        [Header("Global Settings")]
        public string m_version = "Add Unity Version... Example: 2019.1, 2019.2";
        public GaiaPackageVersion m_unityVersion = GaiaPackageVersion.Unity2019_1;
        [Header("Built-In")]
        public string m_builtInWaterPackage = "Add 2019.1 Package Name";
        public string m_builtInVegetationPackage = "Add 2019.1 Package Name";
        [Header("Lightweight")]
        public string m_lightweightWaterPackage = "Add 2019.1 Package Name";
        public string m_lightweightVegetationPackage = "Add 2019.1 Package Name";
        [Header("High Definition")]
        public string m_highDefinitionWaterPackage = "Add 2019.1 Package Name";
        public string m_highDefinitionVegetationPackage = "Add 2019.1 Package Name";
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

        [Header("Pipeline Supported Versions")]
        [HideInInspector]
        public string m_minLWVersion = "5.7.2";
        [HideInInspector]
        public string m_maxLWVersion = "5.13.0";
        [HideInInspector]
        public string m_minHDVersion = "5.7.2";
        [HideInInspector]
        public string m_maxHDVersion = "5.13.0";

        [Header("Built-In Render Pipeline")]
        public string m_builtInWaterShader = "Add Built In Shader";
        public string m_builtInHorizonObjectShader = "Standard";
        public bool m_BuiltInAutoConfigureWater = true;
        public Material m_builtInTerrainMaterial;

        [Header("Lightweight Render Pipeline")]
        public string m_lightweightPipelineProfile = "Procedural Worlds Lightweight Pipeline Profile";
        public bool m_setLWPipelineProfile = true;
        public string m_lightweightScriptDefine = "LWPipeline";
        public string m_lightweightWaterShader = "Add LWRP Shader";
        public string m_lightweightWaterShader2019v2 = "Add LWRP Shader";
        public string m_lightweightHorizonObjectShader = "Lightweight Render Pipeline/Lit";
        public Material m_lightweightTerrainMaterial;
        public bool m_LWAutoConfigureTerrain = true;
        public bool m_LWAutoConfigureWater = true;
        public bool m_LWAutoConfigureCamera = true;
        public bool m_LWAutoConfigureProbes = true;
        public bool m_LWAutoConfigureLighting = true;

        [Header("High Definition Render Pipeline")]
        public string m_highDefinitionPipelineProfile = "Procedural Worlds HDRenderPipelineAsset";
        public bool m_setHDPipelineProfile = true;
        public string m_highDefinitionScriptDefine = "HDPipeline";
        public string m_highDefinitionWaterShader = "Add HDRP Shader";
        public string m_highDefinitionHorizonObjectShader = "HDRP/Lit";
        public Material m_highDefinitionTerrainMaterial;
        public bool m_HDAutoConfigureTerrain = true;
        public bool m_HDAutoConfigureWater = true;
        public bool m_HDAutoConfigureCamera = true;
        public bool m_HDAutoConfigureProbes = true;
        public bool m_HDAutoConfigureLighting = true;
        public string m_HDVolumeObjectName = "HD Environment Volume";
        public string m_HDPostVolumeObjectName = "HD Post Processing Environment Volume";
        public string m_HDDefaultPostProcessing = "Default HDRP Post Processing Profile";
        public string m_HDDefaultSceneLighting = "Default Volume Profile";
        public string m_HDSceneLighting = "HD Volume Profile";

        [Header("Package Settings")]
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