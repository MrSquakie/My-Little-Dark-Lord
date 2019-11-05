using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
#if LWPipeline
using UnityEngine.Rendering.LWRP;
#endif
using System.Collections;
using UnityEditor.SceneManagement;
using ProcedualWorlds.WaterSystem.Reflections;

namespace Gaia.Pipeline.LWRP
{
    /// <summary>
    /// Static class that handles all the LWRP setup in Gaia
    /// </summary>
    public static class GaiaLWRPPipelineUtils
    {
        public static float m_waitTimer1 = 1f;
        public static float m_waitTimer2 = 3f;

        /// <summary>
        /// Configures project for LWRP
        /// </summary>
        /// <param name="profile"></param>
        private static void ConfigureSceneToLWRP(UnityPipelineProfile profile)
        {
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (gaiaSettings.m_currentRenderer != GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                Debug.LogError("Unable to configure your scene/project to LWRP as the current render inside of gaia does not equal Lightweight as it's active render pipeline. This process [GaiaLWRPPipelineUtils.ConfigureSceneToLWRP()] will now exit.");
                return;
            }

            if (profile.m_setLWPipelineProfile)
            {
                SetPipelineAsset(profile);
            }

            if (profile.m_LWAutoConfigureCamera)
            {
                ConfigureCamera();
            }

            if (profile.m_LWAutoConfigureLighting)
            {
                ConfigureLighting();
            }

            if (profile.m_LWAutoConfigureWater)
            {
                ConfigureWater(profile, gaiaSettings);
            }

            if (profile.m_LWAutoConfigureProbes)
            {
                ConfigureReflectionProbes();
            }

            if (profile.m_LWAutoConfigureTerrain)
            {
                ConfigureTerrain(profile);
            }

            FinalizeLWRP(profile);
        }

        /// <summary>
        /// Configures scripting defines in the project
        /// </summary>
        public static void SetScriptingDefines(UnityPipelineProfile profile)
        {
            bool isChanged = false;
            string currBuildSettings = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!currBuildSettings.Contains("LWPipeline"))
            {
                if (string.IsNullOrEmpty(currBuildSettings))
                {
                    currBuildSettings = "LWPipeline";
                }
                else
                {
                    currBuildSettings += ";LWPipeline";
                }
                isChanged = true;
            }

            if (currBuildSettings.Contains("HDPipeline"))
            {
                currBuildSettings = currBuildSettings.Replace("HDPipeline;", "");
                currBuildSettings = currBuildSettings.Replace("HDPipeline", "");
                isChanged = true;
            }

            if (isChanged)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currBuildSettings);
            }
        }

        /// <summary>
        /// Sets the pipeline asset to the procedural worlds asset if the profile is set yo change it
        /// </summary>
        /// <param name="profile"></param>
        public static void SetPipelineAsset(UnityPipelineProfile profile)
        {
            if (GraphicsSettings.renderPipelineAsset == null)
            {
                GraphicsSettings.renderPipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(GetAssetPath(profile.m_lightweightPipelineProfile));
            }
            else if (GraphicsSettings.renderPipelineAsset.name != profile.m_lightweightPipelineProfile)
            {
                GraphicsSettings.renderPipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(GetAssetPath(profile.m_lightweightPipelineProfile));
            }

            profile.m_pipelineSwitchUpdates = true;
        }

        /// <summary>
        /// Configures camera to LWRP
        /// </summary>
        private static void ConfigureCamera()
        {
            Camera camera = GetCamera();
            if (camera == null)
            {
                Debug.LogWarning("[GaiaLWRPPipelineUtils.ConfigureCamera()] A camera could not be found to upgrade in your scene.");
            }
            else
            {
#if LWPipeline
                LWRPAdditionalCameraData cameraData = camera.gameObject.GetComponent<LWRPAdditionalCameraData>();
                if (cameraData == null)
                {
                    cameraData = camera.gameObject.AddComponent<LWRPAdditionalCameraData>();
                    cameraData.renderShadows = true;
                }
                else
                {
                    cameraData.renderShadows = true;
                }
#endif
            }
        }

        /// <summary>
        /// Configures lighting to LWRP
        /// </summary>
        private static void ConfigureLighting()
        {
#if LWPipeline
            LWRPAdditionalLightData[] lightsData = Object.FindObjectsOfType<LWRPAdditionalLightData>();
            if (lightsData != null)
            {
                foreach (LWRPAdditionalLightData data in lightsData)
                {
                    if (data.gameObject.GetComponent<LWRPAdditionalLightData>() == null)
                    {
                        data.gameObject.AddComponent<LWRPAdditionalLightData>();
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Sets the sun intensity
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="volumeProfile"></param>
        public static void SetSunSettings(GaiaLightingProfileValues profile)
        {
            Light light = GetSunLight();
            if (light != null)
            {
                light.color = profile.m_lWSunColor;
                light.intensity = profile.m_lWSunIntensity;
            }
        }

        /// <summary>
        /// Configures water to LWRP
        /// </summary>
        /// <param name="profile"></param>
        private static void ConfigureWater(UnityPipelineProfile profile, GaiaSettings gaiaSettings)
        {
            if (gaiaSettings == null)
            {
                Debug.LogError("Gaia settings could not be found. Please make sure gaia settings is import into your project");
            }
            else
            {
                Material waterMaterial = gaiaSettings.m_gaiaWaterProfile.m_masterWaterMaterial;
                if (waterMaterial != null)
                {
#if !UNITY_2019_2_OR_NEWER
                    waterMaterial.shader = Shader.Find(profile.m_lightweightWaterShader);
#else
                    waterMaterial.shader = Shader.Find(profile.m_lightweightWaterShader2019v2);
#endif
                }
            }

            if (profile.m_underwaterHorizonMaterial != null)
            {
                profile.m_underwaterHorizonMaterial.shader = Shader.Find(profile.m_lightweightHorizonObjectShader);
            }

            PWS_WaterReflections reflection = Object.FindObjectOfType<PWS_WaterReflections>();
            if (reflection != null)
            {
                Object.DestroyImmediate(reflection);
            }
        }

        /// <summary>
        /// Configures reflections to LWRP
        /// </summary>
        private static void ConfigureReflectionProbes()
        {
            ReflectionProbe[] reflectionProbes = Object.FindObjectsOfType<ReflectionProbe>();
            if (reflectionProbes != null)
            {
                foreach(ReflectionProbe probe in reflectionProbes)
                {
                    if (probe.resolution > 512)
                    {
                        Debug.Log(probe.name + " This probes resolution is quite high and could cause performance issues in Lightweight Pipeline. Recommend lowing the resolution if you're targeting mobile platform");
                    }
                }
            }
        }

        /// <summary>
        /// Configures and setup the terrain
        /// </summary>
        /// <param name="profile"></param>
        private static void ConfigureTerrain(UnityPipelineProfile profile)
        {
            Terrain[] terrains = Terrain.activeTerrains;
            if (terrains != null)
            {
                foreach (Terrain terrain in terrains)
                {
#if !UNITY_2019_2_OR_NEWER
                    terrain.materialType = Terrain.MaterialType.Custom;
#endif
                    terrain.materialTemplate = profile.m_lightweightTerrainMaterial;
                }
            }
        }

        /// <summary>
        /// Finalizes the LWRP setup for your project 
        /// </summary>
        /// <param name="profile"></param>
        private static void FinalizeLWRP(UnityPipelineProfile profile)
        {
            MarkSceneDirty(true);
            profile.m_activePipelineInstalled = GaiaConstants.EnvironmentRenderer.Lightweight;
        }

        /// <summary>
        /// Cleans up LWRP components in the scene
        /// </summary>
        public static void CleanUpLWRP(UnityPipelineProfile profile, GaiaSettings gaiaSettings)
        {
#if LWPipeline
            LWRPAdditionalCameraData[] camerasData = Object.FindObjectsOfType<LWRPAdditionalCameraData>();
            if (camerasData != null)
            {
                foreach (LWRPAdditionalCameraData data in camerasData)
                {
                    Object.DestroyImmediate(data);
                }
            }

            LWRPAdditionalLightData[] lightsData = Object.FindObjectsOfType<LWRPAdditionalLightData>();
            if (lightsData != null)
            {
                foreach (LWRPAdditionalLightData data in lightsData)
                {
                    Object.DestroyImmediate(data);
                }
            }
#endif

            if (gaiaSettings != null)
            {
                Material waterMat = gaiaSettings.m_gaiaWaterProfile.m_masterWaterMaterial;
                if (waterMat != null)
                {
                    waterMat.shader = Shader.Find(profile.m_builtInWaterShader);
                }
            }

            if (profile.m_underwaterHorizonMaterial != null)
            {
                profile.m_underwaterHorizonMaterial.shader = Shader.Find(profile.m_builtInHorizonObjectShader);
            }

            GameObject waterPrefab = GameObject.Find("Ambient Water Sample");
            if (waterPrefab != null)
            {
                PWS_WaterReflections reflection = waterPrefab.GetComponent<PWS_WaterReflections>();
                if (reflection == null)
                {
                    reflection = waterPrefab.AddComponent<PWS_WaterReflections>();
                }
            }

            Terrain[] terrains = Terrain.activeTerrains;
            if (terrains != null)
            {
                foreach (Terrain terrain in terrains)
                {
#if !UNITY_2019_2_OR_NEWER
                    terrain.materialType = Terrain.MaterialType.BuiltInStandard;
#else
                    terrain.materialTemplate = profile.m_builtInTerrainMaterial;
#endif
                }
            }

            GraphicsSettings.renderPipelineAsset = null;

            GaiaLighting.GetProfile(GaiaConstants.GaiaLightingProfileType.Day, gaiaSettings.m_gaiaLightingProfile, gaiaSettings.m_pipelineProfile, GaiaConstants.EnvironmentRenderer.BuiltIn);

            MarkSceneDirty(false);
            profile.m_activePipelineInstalled = GaiaConstants.EnvironmentRenderer.BuiltIn;

            bool isChanged = false;
            string currBuildSettings = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (currBuildSettings.Contains("LWPipeline"))
            {
                currBuildSettings = currBuildSettings.Replace("LWPipeline;", "");
                currBuildSettings = currBuildSettings.Replace("LWPipeline", "");
                isChanged = true;
            }

            if (isChanged)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currBuildSettings);
            }
        }

        /// <summary>
        /// Gets and returns the main camera in the scene
        /// </summary>
        /// <returns></returns>
        private static Camera GetCamera()
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                return camera;
            }

            camera = Object.FindObjectOfType<Camera>();
            if (camera != null)
            {
                return camera;
            }

            return null;
        }

        /// <summary>
        /// Gets and returns the sun light in the scene
        /// </summary>
        /// <returns></returns>
        private static Light GetSunLight()
        {
            Light light = null;
            GameObject lightObject = GameObject.Find("Directional Light");
            if (lightObject != null)
            {
                light = Object.FindObjectOfType<Light>();
                if (light != null)
                {
                    if (light.type == LightType.Directional)
                    {
                        return light;
                    }
                }
            }

            Light[] lights = Object.FindObjectsOfType<Light>();
            foreach(Light activeLight in lights)
            {
                if (activeLight.type == LightType.Directional)
                {
                    return activeLight;
                }
            }

            return null;
        }

        /// <summary>
        /// Starts the LWRP Setup
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static IEnumerator StartLWRPSetup(UnityPipelineProfile profile)
        {
            if (profile == null)
            {
                Debug.LogError("UnityPipelineProfile is empty");
                yield return null;
            }
            else
            {
                EditorUtility.DisplayProgressBar("Installing LightWeight2018x", "Updating scripting defines", 0.5f);
                m_waitTimer1 -= Time.deltaTime;
                if (m_waitTimer1 < 0)
                {
                    SetScriptingDefines(profile);
                }
                else
                {
                    yield return null;
                }

                while (EditorApplication.isCompiling == true)
                {
                    yield return null;
                }

                EditorUtility.DisplayProgressBar("Installing LightWeight2018x", "Updating scene to LightWeight2018x", 0.75f);
                m_waitTimer2 -= Time.deltaTime;
                if (m_waitTimer2 < 0)
                {
                    ConfigureSceneToLWRP(profile);
                    profile.m_pipelineSwitchUpdates = false;
                    EditorUtility.ClearProgressBar();
                }
                else
                {
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns>The path or null</returns>
        private static string GetAssetPath(string name)
        {
            string[] assets = AssetDatabase.FindAssets(name, null);
            if (assets.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(assets[0]);
            }
            return null;
        }

        /// <summary>
        /// Save the assets and marks scene as dirty
        /// </summary>
        /// <param name="saveAlso"></param>
        private static void MarkSceneDirty(bool saveAlso)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            if (saveAlso)
            {
                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
            }
        }
    }
}