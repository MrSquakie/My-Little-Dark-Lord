using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif
using UnityEditor.SceneManagement;
using Gaia.Pipeline;
using Gaia.Pipeline.HDRP;
using Gaia.Pipeline.LWRP;

namespace Gaia
{
    public static class GaiaLighting
    {
        #region Variables

        //Lighting profiles
        private static List<GaiaLightingProfileValues> m_lightingProfiles;
        private static GaiaLightingProfile m_lightingProfile;

        //Sun Values
        private static GameObject m_sunObject;
        private static Light m_sunLight;

        //Camera Values
        private static GameObject m_mainCamera;

        //Parent Object Values
        private static GameObject m_parentObject;

        //Post Processing Values
#if UNITY_POST_PROCESSING_STACK_V2
        private static PostProcessLayer m_processLayer;
        private static PostProcessVolume m_processVolume;
#endif

        //Ambient Audio Values
        private static AudioSource m_audioSource;

        //Stores gaia settings
        private static GaiaSettings m_gaiaSettings;

        private static int m_markSceneDirty;

        #endregion

        #region Setup

        /// <summary>
        /// Starts the setup process for selected lighting
        /// </summary>
        /// <param name="typeOfDay"></param>
        /// <param name="renderPipeline"></param>
        public static void GetProfile(GaiaConstants.GaiaLightingProfileType typeOfDay, GaiaLightingProfile lightProfile, UnityPipelineProfile pipelineProfile, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            m_lightingProfile = lightProfile;
            if (m_lightingProfile == null)
            {
                m_lightingProfile = AssetDatabase.LoadAssetAtPath<GaiaLightingProfile>(GetAssetPath("Gaia Lighting System Profile"));
            }

            if (m_lightingProfile == null)
            {
                Debug.LogError("[AmbientSkiesSamples.GetProfile()] Asset 'Gaia Lighting System Profile' could not be found please make sure it exists withiny our project or that th ename has not been changed. Due to this error the method will now exit.");
                return;
            }
            else
            {
                if (m_parentObject == null)
                {
                    m_parentObject = GetOrCreateParentObject("Gaia Lighting Environment", true);
                }

                bool wasSuccessfull = false;
                if (m_lightingProfiles == null)
                {
                    m_lightingProfiles = m_lightingProfile.m_lightingProfiles;
                    foreach (GaiaLightingProfileValues profile in m_lightingProfiles)
                    {
                        if (profile.m_profileType == typeOfDay)
                        {
                            if (renderPipeline == GaiaConstants.EnvironmentRenderer.HighDefinition)
                            {
                                GaiaHDRPPipelineUtils.UpdateSceneLighting(profile, pipelineProfile, lightProfile);
                            }
                            else
                            {
                                UpdateGlobalLighting(profile, renderPipeline);
                            }

                            wasSuccessfull = true;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (GaiaLightingProfileValues profile in m_lightingProfiles)
                    {
                        if (profile.m_profileType == typeOfDay)
                        {
                            if (renderPipeline == GaiaConstants.EnvironmentRenderer.HighDefinition)
                            {
                                GaiaHDRPPipelineUtils.UpdateSceneLighting(profile, pipelineProfile, lightProfile);
                            }
                            else
                            {
                                UpdateGlobalLighting(profile, renderPipeline);
                            }

                            wasSuccessfull = true;
                            break;
                        }
                    }

                    if (!wasSuccessfull)
                    {
                        Debug.LogError("[AmbientSkiesSamples.GetProfile()] No profile type matches one you haev selected. Have you modified GaiaConstants.GaiaLightingProfileType?");
                    }
                }
            }
        }

        #endregion

        #region Apply Settings

        /// <summary>
        /// Updates the global lighting settings in your scene
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="renderPipeline"></param>
        private static void UpdateGlobalLighting(GaiaLightingProfileValues profile, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            //Removes the old content from the scene
            RemoveOldLighting();

            //Applies sun settings
            ApplySunSettings(profile, renderPipeline);
            //Applies the Skybox
            SetSkyboxSettings(profile);
            //Applies the ambient light
            ApplyAmbientLighting(profile);
            //Applies the fog settings
            ApplyFogSettings(profile);
            //Sets the lightmapping settings
            SetLightmappingSettings(profile);
            //Applies the scene post processing
            SetupPostProcessing(profile);
            //Sets the ambient audio 
            SetupAmbientAudio(profile);
            //Sets up antialiasing
            ConfigureAntiAliasing(m_lightingProfile.m_antiAliasingMode, renderPipeline);
            //Sets up the global reflection probe in the scene
            //ProGlobalReflectionProbe();

            //Destroys the parent object if it contains no partent childs
            DestroyParent("Ambient Skies Samples Environment");

            //Marks the scene as dirty
            m_markSceneDirty++;
            if (m_markSceneDirty > 25)
            {
                m_markSceneDirty = 0;
                MarkSceneDirty(false);
            }
        }

        /// <summary>
        /// Apply sun settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="renderPipeline"></param>
        private static void ApplySunSettings(GaiaLightingProfileValues profile, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (m_sunObject == null || m_sunLight == null)
            {
                //Get the sun object and sun light
                GetMainDirectionalLight();
            }

            //Rotates the sun to specified values
            RotateSun(profile.m_sunRotation, profile.m_sunPitch);

            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                m_sunLight.color = profile.m_sunColor;
                m_sunLight.intensity = profile.m_sunIntensity;
                m_sunLight.shadows = profile.m_shadowCastingMode;
                m_sunLight.shadowStrength = profile.m_shadowStrength;
                m_sunLight.shadowResolution = profile.m_sunShadowResolution;
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                GaiaLWRPPipelineUtils.SetSunSettings(profile);
            }
        }

        /// <summary>
        /// Set the suns rotation and pitch
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="pitch"></param>
        private static void RotateSun(float rotation, float pitch)
        {
            //Correct the angle
            float angleDegrees = rotation % 360f;

            float sunAngle = pitch;

            //Set new directional light rotation
            if (m_sunObject != null)
            {
                Vector3 newRotation = m_sunObject.transform.rotation.eulerAngles;
                newRotation.y = angleDegrees;
                m_sunObject.transform.rotation = Quaternion.Euler(sunAngle, -newRotation.y, 0f);
            }
        }

        /// <summary>
        /// Sets the cubemap for the skybox
        /// </summary>
        /// <param name="cubemapTexture"></param>
        private static void SetSkyboxSettings(GaiaLightingProfileValues profile)
        {
            if (profile.m_skyboxHDRI == null)
            {
                Debug.LogError("[AmbientSkiesSamples.SetSkyboxHDRI()] HDRI map is missing. Go to Gaia Lighting System Profile and add one");
            }
            else
            {
                if (m_lightingProfile.m_masterSkyboxMaterial == null)
                {
                    m_lightingProfile.m_masterSkyboxMaterial = AssetDatabase.LoadAssetAtPath<Material>(GetAssetPath("Ambient Skies Sample Sky"));
                }

                if (m_lightingProfile.m_masterSkyboxMaterial != null)
                {
                    if (RenderSettings.skybox != m_lightingProfile.m_masterSkyboxMaterial)
                    {
                        RenderSettings.skybox = m_lightingProfile.m_masterSkyboxMaterial;
                    }

                    if (profile.m_profileType != GaiaConstants.GaiaLightingProfileType.Default)
                    {
                        if (m_lightingProfile.m_masterSkyboxMaterial.shader != Shader.Find("Skybox/Cubemap"))
                        {
                            m_lightingProfile.m_masterSkyboxMaterial.shader = Shader.Find("Skybox/Cubemap");
                        }

                        m_lightingProfile.m_masterSkyboxMaterial.SetTexture("_Tex", profile.m_skyboxHDRI);
                        m_lightingProfile.m_masterSkyboxMaterial.SetFloat("_Rotation", profile.m_sunRotation);
                        m_lightingProfile.m_masterSkyboxMaterial.SetColor("_Tint", profile.m_skyboxTint);
                        m_lightingProfile.m_masterSkyboxMaterial.SetFloat("_Exposure", profile.m_skyboxExposure);
                    }
                    else
                    {
                        if (m_lightingProfile.m_masterSkyboxMaterial.shader != Shader.Find("Skybox/Procedural"))
                        {
                            m_lightingProfile.m_masterSkyboxMaterial.shader = Shader.Find("Skybox/Procedural");
                        }

                        m_lightingProfile.m_masterSkyboxMaterial.SetColor("_SkyTint", profile.m_skyboxTint);
                        m_lightingProfile.m_masterSkyboxMaterial.SetFloat("_Exposure", profile.m_skyboxExposure);
                        m_lightingProfile.m_masterSkyboxMaterial.shader = Shader.Find("Skybox/Procedural");
                        m_lightingProfile.m_masterSkyboxMaterial.EnableKeyword("_SUNDISK_HIGH_QUALITY");
                        m_lightingProfile.m_masterSkyboxMaterial.SetFloat("_SunSize", profile.m_sunSize);
                        m_lightingProfile.m_masterSkyboxMaterial.SetFloat("_SunSizeConvergence", profile.m_sunConvergence);
                        m_lightingProfile.m_masterSkyboxMaterial.SetFloat("_AtmosphereThickness", profile.m_atmosphereThickness);
                        m_lightingProfile.m_masterSkyboxMaterial.SetColor("_GroundColor", profile.m_groundColor);
                    }
                }
            }
        }

        /// <summary>
        /// sets the ambient lighting
        /// </summary>
        /// <param name="profile"></param>
        private static void ApplyAmbientLighting(GaiaLightingProfileValues profile)
        {
            RenderSettings.ambientMode = profile.m_ambientMode;
            RenderSettings.ambientIntensity = profile.m_ambientIntensity;
            RenderSettings.ambientSkyColor = profile.m_skyAmbient;
            RenderSettings.ambientEquatorColor = profile.m_equatorAmbient;
            RenderSettings.ambientGroundColor = profile.m_groundAmbient;
            if (profile.m_ambientMode == UnityEngine.Rendering.AmbientMode.Skybox)
            {
                if (Lightmapping.lightingDataAsset == null)
                {
                    Debug.LogWarning("Lightmapping has not been baked. Since the 'Ambient Mode' is set to 'Skybox' you will not see any ambient changes with the 'Ambient Intensity' till the scene lighting has been baked");
                }
            }
        }

        /// <summary>
        /// Sets up and applies the fog in your scene
        /// </summary>
        /// <param name="profile"></param>
        private static void ApplyFogSettings(GaiaLightingProfileValues profile)
        {
            RenderSettings.fog = m_lightingProfile.m_enableFog;
            RenderSettings.fogMode = profile.m_fogMode;
            RenderSettings.fogColor = profile.m_fogColor;
            RenderSettings.fogDensity = profile.m_fogDensity;
            RenderSettings.fogStartDistance = profile.m_fogStartDistance;
            RenderSettings.fogEndDistance = profile.m_fogEndDistance;
        }

        /// <summary>
        /// Sets the lightmap settings
        /// </summary>
        /// <param name="profile"></param>
        private static void SetLightmappingSettings(GaiaLightingProfileValues profile)
        {
            LightmapEditorSettings.lightmapper = profile.m_lightmappingMode;
        }

        /// <summary>
        /// Sets up the post processing in the scene
        /// </summary>
        /// <param name="profile"></param>
        private static void SetupPostProcessing(GaiaLightingProfileValues profile)
        {
#if !UNITY_POST_PROCESSING_STACK_V2
            Debug.Log("[AmbientSkiesSamples.SetupPostProcessing()] Post Processing was not found. Please install post processing from the package manager to allow post processing to be setup");
#else
            if (m_lightingProfile.m_enablePostProcessing)
            {
                if (!string.IsNullOrEmpty(profile.m_postProcessingProfile))
                {
                    if (m_mainCamera == null)
                    {
                        m_mainCamera = GetOrCreateMainCamera();
                    }

                    m_processLayer = m_mainCamera.GetComponent<PostProcessLayer>();
                    if (m_processLayer == null)
                    {
                        m_processLayer = m_mainCamera.AddComponent<PostProcessLayer>();
                        m_processLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                        m_processLayer.volumeLayer = 2;
                        m_processLayer.finalBlitToCameraTarget = profile.m_directToCamera;
                    }
                    else
                    {
                        m_processLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                        m_processLayer.volumeLayer = 2;
                        m_processLayer.finalBlitToCameraTarget = profile.m_directToCamera;
                    }

                    if (m_processVolume == null)
                    {
                        m_processVolume = GetOrCreatePostProcessVolume(profile);
                        m_processVolume.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GetAssetPath(profile.m_postProcessingProfile));
                        m_processVolume.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
                    }
                    else
                    {
                        m_processVolume.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GetAssetPath(profile.m_postProcessingProfile));
                        m_processVolume.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
                    }

                    if (m_processVolume != null)
                    {
                        if (m_lightingProfile.m_parentObjects)
                        {
                            m_processVolume.gameObject.transform.SetParent(m_parentObject.transform);
                        }
                        if (m_lightingProfile.m_hideProcessVolume)
                        {
                            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(m_processVolume, false);
                        }
                        else
                        {
                            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(m_processVolume, true);
                        }
                    }
                }
                else
                {
                    Debug.LogError("[AmbientSkiesSamples.SetupPostProcessing()] Profile name is empty please insure it contains a valid string");
                }
            }
            else
            {
                RemovePostProcessing();
            }
#endif
        }

        /// <summary>
        /// Sets up the ambient audio
        /// </summary>
        /// <param name="profile"></param>
        private static void SetupAmbientAudio(GaiaLightingProfileValues profile)
        {
            if (m_lightingProfile.m_enableAmbientAudio)
            {
                if (m_audioSource == null)
                {
                    GetOrCreateAmbientAudio();
                    m_audioSource.clip = profile.m_ambientAudio;
                    m_audioSource.volume = profile.m_ambientVolume;
                }
                else
                {
                    m_audioSource.clip = profile.m_ambientAudio;
                    m_audioSource.volume = profile.m_ambientVolume;
                }

                if (m_lightingProfile.m_parentObjects)
                {
                    if (m_audioSource != null)
                    {
                        m_audioSource.gameObject.transform.SetParent(m_parentObject.transform);
                    }
                }
            }
            else
            {
                RemoveAmbientAudio();
            }
        }

        /// <summary>
        /// Sets up AntiAliasing on the camera
        /// </summary>
        /// <param name="antiAliasingMode"></param>
        /// <param name="profile"></param>
        private static void ConfigureAntiAliasing(GaiaConstants.GaiaProAntiAliasingMode antiAliasingMode, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
#if UNITY_POST_PROCESSING_STACK_V2
            if (m_lightingProfile.m_enablePostProcessing)
            {
                if (m_mainCamera == null)
                {
                    m_mainCamera = GetOrCreateMainCamera();
                }

                m_processLayer = m_mainCamera.GetComponent<PostProcessLayer>();
                if (m_processLayer == null)
                {
                    Debug.LogError("[GaiaProLighting.ConfigureAntiAliasing() Post Processing Layer could not be found on the main camera");
                }
                else
                {
                    Camera camera = m_mainCamera.GetComponent<Camera>();
                    switch (antiAliasingMode)
                    {
                        case GaiaConstants.GaiaProAntiAliasingMode.None:
                            m_processLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
                            camera.allowMSAA = false;
                            break;
                        case GaiaConstants.GaiaProAntiAliasingMode.FXAA:
                            m_processLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                            camera.allowMSAA = false;
                            break;
                        case GaiaConstants.GaiaProAntiAliasingMode.MSAA:
                            m_processLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
                            camera.allowMSAA = true;
                            break;
                        case GaiaConstants.GaiaProAntiAliasingMode.SMAA:
                            m_processLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                            camera.allowMSAA = false;
                            break;
                        case GaiaConstants.GaiaProAntiAliasingMode.TAA:
                            if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
                            {
                                Debug.Log("Temporal Anti Aliasing is not recommended to use in LWRP. We recommend using FXAA or SMAA");
                            }
                            else
                            {
                                m_processLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                            }
                            camera.allowMSAA = false;
                            break;
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Sets the post processing component status
        /// </summary>
        /// <param name="enabled"></param>
        public static void SetPostProcessingStatus(bool enabled)
        {
#if UNITY_POST_PROCESSING_STACK_V2
            if (m_gaiaSettings == null)
            {
                m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            }

            if (m_gaiaSettings != null)
            {
                if (!m_gaiaSettings.m_gaiaLightingProfile.m_enablePostProcessing)
                {
                    return;
                }
            }

            if (enabled)
            {
                foreach (SceneView sceneView in SceneView.sceneViews)
                {
                    sceneView.sceneViewState.showImageEffects = true;
                }
            }
            else
            {
                foreach (SceneView sceneView in SceneView.sceneViews)
                {
                    sceneView.sceneViewState.showImageEffects = false;
                }
            }
#endif
        }

        #endregion

        #region GX Only Functions

        /// <summary>
        /// Sets up and bakes occlusion culling
        /// </summary>
        /// <param name="occlusionCullingEnabled"></param>
        /// <param name="bakeOcclusionCulling"></param>
        /// <param name="clearBakedData"></param>
        public static void AddOcclusionCulling(bool occlusionCullingEnabled, bool bakeOcclusionCulling, bool clearBakedData)
        {
            GameObject parentObject = GetOrCreateParentObject("Gaia Lighting Environment", true);
            Terrain terrain = GetActiveTerrain();

            GameObject occlusionCullObject = GameObject.Find("Occlusion Culling Volume");
            if (occlusionCullObject == null)
            {
                occlusionCullObject = new GameObject("Occlusion Culling Volume");
                occlusionCullObject.transform.SetParent(parentObject.transform);
                OcclusionArea occlusionArea = occlusionCullObject.AddComponent<OcclusionArea>();
                if (terrain != null)
                {
                    occlusionArea.size = new Vector3(terrain.terrainData.size.x, terrain.terrainData.size.y, terrain.terrainData.size.z);
                }
                else
                {
                    occlusionArea.size = new Vector3(2000f, 1000f, 2000f);
                }

                StaticOcclusionCulling.smallestOccluder = 4f;
                StaticOcclusionCulling.smallestHole = 0.2f;
                StaticOcclusionCulling.backfaceThreshold = 15f;
            }
            else
            {
                OcclusionArea occlusionArea = occlusionCullObject.GetComponent<OcclusionArea>();
                if (occlusionArea != null)
                {
                    if (terrain != null)
                    {
                        occlusionArea.size = new Vector3(terrain.terrainData.size.x, terrain.terrainData.size.y, terrain.terrainData.size.z);
                    }
                    else
                    {
                        occlusionArea.size = new Vector3(2000f, 1000f, 2000f);
                    }

                    StaticOcclusionCulling.smallestOccluder = 4f;
                    StaticOcclusionCulling.smallestHole = 0.2f;
                    StaticOcclusionCulling.backfaceThreshold = 15f;
                }
            }

            Selection.activeGameObject = occlusionCullObject;
            EditorGUIUtility.PingObject(occlusionCullObject);
        }

        /// <summary>
        /// Sets up and bakes occlusion culling
        /// </summary>
        /// <param name="occlusionCullingEnabled"></param>
        /// <param name="bakeOcclusionCulling"></param>
        /// <param name="clearBakedData"></param>
        public static void RemoveOcclusionCulling(bool occlusionCullingEnabled, bool bakeOcclusionCulling, bool clearBakedData)
        {
            GameObject occlusionObject = GameObject.Find("Occlusion Culling Volume");
            if (occlusionObject != null)
            {
                Object.DestroyImmediate(occlusionObject);
            }
        }

        /// <summary>
        /// Sets up and bakes occlusion culling
        /// </summary>
        /// <param name="occlusionCullingEnabled"></param>
        /// <param name="bakeOcclusionCulling"></param>
        /// <param name="clearBakedData"></param>
        public static void BakeOcclusionCulling(bool occlusionCullingEnabled, bool bakeOcclusionCulling, bool clearBakedData)
        {
            StaticOcclusionCulling.GenerateInBackground();
        }

        /// <summary>
        /// Sets up and bakes occlusion culling
        /// </summary>
        /// <param name="occlusionCullingEnabled"></param>
        /// <param name="bakeOcclusionCulling"></param>
        /// <param name="clearBakedData"></param>
        public static void CancelOcclusionCulling(bool occlusionCullingEnabled, bool bakeOcclusionCulling, bool clearBakedData)
        {
            StaticOcclusionCulling.Cancel();
        }

        /// <summary>
        /// Sets up and bakes occlusion culling
        /// </summary>
        /// <param name="occlusionCullingEnabled"></param>
        /// <param name="bakeOcclusionCulling"></param>
        /// <param name="clearBakedData"></param>
        public static void ClearOcclusionCulling(bool occlusionCullingEnabled, bool bakeOcclusionCulling, bool clearBakedData)
        {
            StaticOcclusionCulling.Clear();
        }

        /// <summary>
        /// Bakes lightmapping in ASync
        /// </summary>
        public static void BakeLighting()
        {
            Lightmapping.BakeAsync();
        }

        /// <summary>
        /// Cancels the lightmap baking process
        /// </summary>
        public static void CancelLightmapBaking()
        {
            Lightmapping.Cancel();
        }

        /// <summary>
        /// Clears the baked lightmap data
        /// </summary>
        public static void ClearBakedLightmaps()
        {
            Lightmapping.Clear();
        }

        /// <summary>
        /// Clears the baked light data of your disk
        /// </summary>
        public static void ClearLightmapDataOnDisk()
        {
            if (EditorUtility.DisplayDialog("Warning!", "Would you like to clear all the baked lightmap GI data on your hard drive?", "Yes", "No"))
            {
                Lightmapping.ClearDiskCache();
            }
        }

        /// <summary>
        /// Removes gaia lighting from the scene
        /// </summary>
        public static void RemoveGaiaLighting()
        {
            GameObject gaiaLighting = GameObject.Find("Ambient Skies Samples Environment");
            if (gaiaLighting != null)
            {
                Object.DestroyImmediate(gaiaLighting);
            }
        }

        /// <summary>
        /// Adds global wind shader script to the scene
        /// </summary>
        public static void AddGlobalWindShader(GaiaConstants.GaiaGlobalWindType windType)
        {
            GameObject windShderObject = GameObject.Find("PW Global Wind Settings");
            if (windShderObject == null)
            {
                windShderObject = new GameObject("PW Global Wind Settings");
            }

            ProceduralWorldsGlobalWind vegetationWind = windShderObject.GetComponent<ProceduralWorldsGlobalWind>();
            if (vegetationWind == null)
            {
                vegetationWind = windShderObject.AddComponent<ProceduralWorldsGlobalWind>();
            }

            vegetationWind.m_windType = windType;

            GameObject parentObject = GetOrCreateParentObject("Gaia Lighting Environment", true);
            windShderObject.transform.SetParent(parentObject.transform);
        }

        /// <summary>
        /// Removes the global wind shader from the scene
        /// </summary>
        public static void RemoveGlobalWindShader()
        {
            GameObject windShderObject = GameObject.Find("Global Wind Settings");
            if (windShderObject != null)
            {
                Object.DestroyImmediate(windShderObject);
            }
        }

        #endregion

        #region Utils

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
        /// Get the main directional light in the scene
        /// </summary>
        /// <returns>Main light or null</returns>
        private static void GetMainDirectionalLight()
        {
            GameObject lightObject = GameObject.Find("Directional Light");
            Light mainSunLight = null;

            if (lightObject != null)
            {
                mainSunLight = lightObject.GetComponent<Light>();
            }
            if (lightObject == null)
            {
                //Grab the first directional light we can find
                Light[] lights = Object.FindObjectsOfType<Light>();
                foreach (var light in lights)
                {
                    if (light.type == LightType.Directional)
                    {
                        lightObject = light.gameObject;
                        mainSunLight = light;
                    }
                }

                if (lightObject == null)
                {
                    lightObject = new GameObject("Directional Light");
                    lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                    Light lightSettings = lightObject.AddComponent<Light>();
                    lightSettings.type = LightType.Directional;
                    mainSunLight = lightSettings;
                }
            }

            m_sunObject = lightObject;
            m_sunLight = mainSunLight;
            RenderSettings.sun = m_sunLight;
        }

        /// <summary>
        /// Get or create the main camera in the scene
        /// </summary>
        /// <returns>Existing or new main camera</returns>
        private static GameObject GetOrCreateMainCamera()
        {
            //Get or create the main camera
            GameObject mainCameraObj = null;

            if (Camera.main != null)
            {
                mainCameraObj = Camera.main.gameObject;
            }

            if (mainCameraObj == null)
            {
                mainCameraObj = GameObject.Find("Main Camera");
            }

            if (mainCameraObj == null)
            {
                mainCameraObj = GameObject.Find("Camera");
            }

            if (mainCameraObj == null)
            {
                Camera[] cameras = Object.FindObjectsOfType<Camera>();
                foreach (var camera in cameras)
                {
                    mainCameraObj = camera.gameObject;
                    break;
                }
            }

            if (mainCameraObj == null)
            {
                mainCameraObj = new GameObject("Main Camera");
                mainCameraObj.tag = "MainCamera";
                SetupMainCamera(mainCameraObj);
            }

            return mainCameraObj;
        }

        /// <summary>
        /// Setup the main camera for use with HDR skyboxes
        /// </summary>
        /// <param name="mainCameraObj"></param>
        private static void SetupMainCamera(GameObject mainCameraObj)
        {
            if (mainCameraObj.GetComponent<FlareLayer>() == null)
            {
                mainCameraObj.AddComponent<FlareLayer>();
            }

            if (mainCameraObj.GetComponent<AudioListener>() == null)
            {
                mainCameraObj.AddComponent<AudioListener>();
            }

            Camera mainCamera = mainCameraObj.GetComponent<Camera>();
            if (mainCamera == null)
            {
                mainCamera = mainCameraObj.AddComponent<Camera>();
            }

#if UNITY_5_6_OR_NEWER
                mainCamera.allowHDR = true;
#else
                mainCamera.hdr = true;
#endif

#if UNITY_2017_0_OR_NEWER
                mainCamera.allowMSAA = false;
#endif
        }

        /// <summary>
        /// Gets or creates the post process volume
        /// </summary>
        /// <returns></returns>
#if UNITY_POST_PROCESSING_STACK_V2
        private static PostProcessVolume GetOrCreatePostProcessVolume(GaiaLightingProfileValues profile)
        {
            PostProcessVolume processVolume = null;
            GameObject processVolumeObject = GameObject.Find("Global Post Processing");
            if (processVolumeObject == null)
            {
                processVolume = new GameObject("Global Post Processing").AddComponent<PostProcessVolume>();
                processVolume.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
                processVolume.isGlobal = true;
            }   
            else
            {
                processVolume = processVolumeObject.GetComponent<PostProcessVolume>();
            }

            return processVolume;
        }
#endif

        /// <summary>
        /// Removes the post processing from the scene
        /// </summary>
        public static void RemovePostProcessing()
        {
#if UNITY_POST_PROCESSING_STACK_V2
            PostProcessLayer processLayer = Object.FindObjectOfType<PostProcessLayer>();
            if (processLayer != null)
            {
                Object.DestroyImmediate(processLayer);
                m_processLayer = null;
            }

            GameObject processVolume = GameObject.Find("Global Post Processing");
            if (processVolume != null)
            {
                Object.DestroyImmediate(processVolume);
                m_processVolume = null;
            }
#endif
        }

        /// <summary>
        /// Removes all post processing v2 from the scene
        /// </summary>
        public static void RemoveAllPostProcessV2()
        {
#if UNITY_POST_PROCESSING_STACK_V2
            m_processLayer = Object.FindObjectOfType<PostProcessLayer>();
            if (m_processLayer != null)
            {
                Object.DestroyImmediate(m_processLayer);
                m_processLayer = null;
            }

            PostProcessVolume[] postProcessVolumes = Object.FindObjectsOfType<PostProcessVolume>();
            if (postProcessVolumes != null)
            {
                foreach(PostProcessVolume volume in postProcessVolumes)
                {
                    Object.DestroyImmediate(volume.gameObject);
                }
            }
#endif
        }

        /// <summary>
        /// Gets or creates the ambient audio volume
        /// </summary>
        /// <returns></returns>
        private static void GetOrCreateAmbientAudio()
        {
            GameObject ambientAudio = GameObject.Find("Ambient Audio");
            if (ambientAudio == null)
            {
                ambientAudio = new GameObject("Ambient Audio");
                AudioSource audioSource = ambientAudio.AddComponent<AudioSource>();
                audioSource.loop = true;
                audioSource.minDistance = 100f;
                audioSource.maxDistance = 10000f;

                m_audioSource = audioSource;
            }
            else
            {
                m_audioSource = ambientAudio.GetComponent<AudioSource>();
            }
        }

        /// <summary>
        /// Removes ambient audio from the scene
        /// </summary>
        private static void RemoveAmbientAudio()
        {
            GameObject ambientAudio = GameObject.Find("Ambient Audio");
            if (ambientAudio != null)
            {
                Object.DestroyImmediate(ambientAudio);
                m_audioSource = null;
            }
        }

        /// <summary>
        /// Get or create a parent object
        /// </summary>
        /// <param name="parentGameObject"></param>
        /// <param name="parentToGaia"></param>
        /// <returns>Parent Object</returns>
        private static GameObject GetOrCreateParentObject(string parentGameObject, bool parentToGaia)
        {
            //Get the parent object
            GameObject theParentGo = GameObject.Find(parentGameObject);

            if (theParentGo == null)
            {
                theParentGo = GameObject.Find("Gaia Lighting Environment");

                if (theParentGo == null)
                {
                    theParentGo = new GameObject("Gaia Lighting Environment");
                }
            }

            if (parentToGaia)
            {
                GameObject gaiaParent = GameObject.Find("Gaia Environment");
                if (gaiaParent == null)
                {
                    gaiaParent = new GameObject("Gaia Environment");
                    theParentGo.transform.SetParent(gaiaParent.transform);
                }
                if (gaiaParent != null)
                {
                    theParentGo.transform.SetParent(gaiaParent.transform);
                }
            }

            FocusLightingProfile focusLighting = theParentGo.GetComponent<FocusLightingProfile>();
            if (focusLighting == null)
            {
                focusLighting = theParentGo.AddComponent<FocusLightingProfile>();
            }

            return theParentGo;
        }

        /// <summary>
        /// Focuses the profile and selects it in your project
        /// </summary>
        public static void FocusGaiaLightingProfile()
        {
            GaiaLightingProfile lightingProfile = AssetDatabase.LoadAssetAtPath<GaiaLightingProfile>(GetAssetPath("Gaia Lighting System Profile"));
            if (lightingProfile != null)
            {
                EditorGUIUtility.PingObject(lightingProfile);
                Selection.activeObject = lightingProfile;
            }
            else
            {
                Debug.LogError("[AmbientSkiesSamples.FocusGaiaLightingProfile()] Unable to focus profile as it does not exists. Please make sure the Gaia Lighting System Profile is set to Gaia Lighting System Profile within your project");
            }
        }

        /// <summary>
        /// Get a color from a html string
        /// </summary>
        /// <param name="htmlString">Color in RRGGBB or RRGGBBBAA or #RRGGBB or #RRGGBBAA format.</param>
        /// <returns>Color or white if unable to parse it.</returns>
        private static Color GetColorFromHTML(string htmlString)
        {
            Color color = Color.white;
            if (!htmlString.StartsWith("#"))
            {
                htmlString = "#" + htmlString;
            }
            if (!ColorUtility.TryParseHtmlString(htmlString, out color))
            {
                color = Color.white;
            }
            return color;
        }

        /// <summary>
        /// Find parent object and destroys it if it's empty
        /// </summary>
        /// <param name="parentGameObject"></param>
        private static void DestroyParent(string parentGameObject)
        {
            //If string isn't empty
            if (!string.IsNullOrEmpty(parentGameObject))
            {
                //If string doesn't = Ambient Skies Environment
                if (parentGameObject != "Ambient Skies Samples Environment")
                {
                    //Sets the paramater to Ambient Skies Environment
                    parentGameObject = "Ambient Skies Samples Environment";
                }

                //Find parent object
                GameObject parentObject = GameObject.Find(parentGameObject);
                if (parentObject != null)
                {
                    //Find parents in parent object
                    Transform[] parentChilds = parentObject.GetComponentsInChildren<Transform>();
                    if (parentChilds.Length == 1)
                    {
                        //Destroy object if object is empty
                        Object.DestroyImmediate(parentObject);
                    }
                }
            }
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
                AssetDatabase.SaveAssets();
            }
        }

        #endregion

        #region Pro Utils

        /// <summary>
        /// Sets a default ambient lighting bright and high contrast color to remove dark shaodws when developing your terrain
        /// </summary>
        public static void SetDefaultAmbientLight()
        {
            if (RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Skybox && Lightmapping.lightingDataAsset == null)
            {
                if (EditorUtility.DisplayDialog("Set Default Ambient Light", "It looks like you are starting in a new scene. Would you like Gaia to set up default ambient lighting suitable for editing your terrain?", "Yes", "No"))
                {
                    RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
                    RenderSettings.ambientSkyColor = new Color(0.635f, 0.696f, 0.735f, 0f);
                    RenderSettings.ambientEquatorColor = new Color(0.509f, 0.509f, 0.509f, 0f);
                    RenderSettings.ambientGroundColor = new Color(0.389f, 0.449f, 0.518f, 0f);
                }
            }
        }

#if UNITY_POST_PROCESSING_STACK_V2
        /// <summary>
        /// Creates a biome post processing profile
        /// </summary>
        /// <param name="biomeName"></param>
        /// <param name="postProcessProfile"></param>
        /// <param name="size"></param>
        /// <param name="blendDistance"></param>
        public static void PostProcessingBiomeSpawning(string biomeName, PostProcessProfile postProcessProfile, float size, float blendDistance, GaiaConstants.BiomePostProcessingVolumeSpawnMode spawnMode)
        {

            //Debug.LogError("[AmbientSkiesSamples.SetupPostProcessing()] Post Processing was not found. Please install post processing from the package manager to allow post processing to be setup");
            if (m_parentObject == null)
            {
                m_parentObject = GetOrCreateParentObject("Gaia Lighting Environment", true);
            }

            if (m_lightingProfile == null)
            {
                m_lightingProfile = AssetDatabase.LoadAssetAtPath<GaiaLightingProfile>(GetAssetPath("Gaia Lighting System Profile"));
            }

            GameObject biomeObject = GameObject.Find(biomeName);
            if (biomeObject == null)
            {
                Debug.LogError("[AmbientSkiesSamples.PostProcessingBiomeSpawning()] biomeName could not be found. Does " + biomeName + " object exist in the scene?");
            }
            else
            {
                string objectName = biomeObject.name;
                Transform objectTransform = biomeObject.transform;

                GameObject ppVolumeObject = null;
                PostProcessVolume processVolume = null;
                BoxCollider collider = null;
                GaiaPostProcessBiome postProcessBiome = null;


                if (spawnMode == GaiaConstants.BiomePostProcessingVolumeSpawnMode.Replace)
                {
                    ppVolumeObject = GameObject.Find(objectName + " Post Processing");
                    if (ppVolumeObject != null)
                    {
                        processVolume = ppVolumeObject.GetComponent <PostProcessVolume>();
                        collider = processVolume.GetComponent<BoxCollider>();
                        postProcessBiome = processVolume.GetComponent<GaiaPostProcessBiome>();
                    }
                }


                if (ppVolumeObject==null || spawnMode == GaiaConstants.BiomePostProcessingVolumeSpawnMode.Add)
                {
                    ppVolumeObject = new GameObject(objectName + " Post Processing");
                    ppVolumeObject.layer = LayerMask.NameToLayer("TransparentFX");
                }

                if (processVolume == null)
                {
                    processVolume = ppVolumeObject.AddComponent<PostProcessVolume>();
                    processVolume.priority = 1;
                    if (m_lightingProfile.m_hideProcessVolume)
                    {
                        UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(processVolume, false);
                    }
                    else
                    {
                        UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(processVolume, true);
                    }
                }

                if (collider==null)
                {
                    collider = processVolume.gameObject.AddComponent<BoxCollider>();
                    collider.isTrigger = true;
                }

                if (postProcessBiome==null)
                {
                    postProcessBiome = processVolume.gameObject.AddComponent<GaiaPostProcessBiome>();
                }

                //PostProcessProfile processProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GetAssetPath(postProcessProfile));
                if (postProcessProfile == null)
                {
                    Debug.LogError("[AmbientSkiesSamples.PostProcessingBiomeSpawning()] Missing post processing profile. Please check that a valid profile is present.");
                }
                else
                {
                    processVolume.sharedProfile = postProcessProfile;
                }

                ppVolumeObject.transform.position = objectTransform.position;
                processVolume.gameObject.transform.SetParent(m_parentObject.transform);
                processVolume.blendDistance = blendDistance;
                collider.size = new Vector3(size, size, size);

                postProcessBiome.m_postProcessProfile = postProcessProfile;
                postProcessBiome.m_postProcessVolume = processVolume;
                postProcessBiome.m_blendDistance = blendDistance;
                postProcessBiome.m_priority = 1;

                postProcessBiome.m_triggerCollider = collider;
                postProcessBiome.m_triggerSize = collider.size;
            }
        }
#endif
        
        /// <summary>
        /// Removes old objects from the scene
        /// </summary>
        private static void RemoveOldLighting()
        {
            GameObject oldObjects = GameObject.Find("Ambient Skies Samples");
            if (oldObjects != null)
            {
                Object.DestroyImmediate(oldObjects);
            }
        }

        /// <summary>
        /// Sets up a new global reflection probe in the scene
        /// </summary>
        private static void ProGlobalReflectionProbe()
        {
            GaiaSceneInfo gaiaInfo = GaiaSceneInfo.GetSceneInfo();
            GameObject reflectionProbeObject = GameObject.Find("Pro Global Reflection Probe");
            if (reflectionProbeObject == null)
            {
                reflectionProbeObject = new GameObject("Pro Global Reflection Probe");
            }

            GameObject parentObject = GetOrCreateParentObject("Gaia Lighting Environment", true);
            reflectionProbeObject.transform.SetParent(parentObject.transform);

            ReflectionProbe probe = reflectionProbeObject.GetComponent<ReflectionProbe>();
            if (probe == null)
            {
                probe = reflectionProbeObject.AddComponent<ReflectionProbe>();
                probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
                probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
                probe.resolution = 128;
            }
            else
            {
                probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
                probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
                probe.resolution = 128;
            }

            Terrain terrain = Terrain.activeTerrain;
            if (terrain == null)
            {
                probe.size = new Vector3(5000f, 2000f, 5000f);
                if (gaiaInfo != null)
                {
                    Vector3 location = reflectionProbeObject.transform.position;
                    location.y = gaiaInfo.m_seaLevel + 1.5f;
                    reflectionProbeObject.transform.position = location;
                }
            }
            else
            {
                probe.size = new Vector3(terrain.terrainData.size.x, 2000f, terrain.terrainData.size.z);
                Vector3 location = reflectionProbeObject.transform.position;
                location.y = terrain.SampleHeight(location) + 1.5f;
                if (location.y < gaiaInfo.m_seaLevel)
                {
                    location.y = gaiaInfo.m_seaLevel + 1.5f;
                }
                reflectionProbeObject.transform.position = location;
            }

            probe.RenderProbe();
        }

        /// <summary>
        /// Removes systems from scene
        /// </summary>
        public static void RemoveSystems()
        {
            GameObject sampleContent = GameObject.Find("Ambient Skies Samples Environment");
            if (sampleContent != null)
            {
                Object.DestroyImmediate(sampleContent);
            }
        }

        /// <summary>
        /// Sets a quick optimization for targeted environment quality
        /// </summary>
        /// <param name="gaiaSettings"></param>
        public static void QuickOptimize(GaiaSettings gaiaSettings)
        {
            if (EditorUtility.DisplayDialog("Warning!", "Proceeding with this optimization will modify your 'Terrain Settings, LOD Bias Settings, Shadow Distance and Water Reflections. Would you like to proceed?", "Yes", "No"))
            {
                Terrain[] terrains = Terrain.activeTerrains;
                float lodBias = QualitySettings.lodBias;
                float shadowDistance = QualitySettings.shadowDistance;
                bool waterReflections = gaiaSettings.m_gaiaWaterProfile.m_enableReflections;

                if (gaiaSettings.m_currentEnvironment != GaiaConstants.EnvironmentTarget.Custom)
                {
                    Debug.Log("Configuring your scene to " + gaiaSettings.m_currentEnvironment.ToString());
                }

                switch(gaiaSettings.m_currentEnvironment)
                {
                    case GaiaConstants.EnvironmentTarget.UltraLight:
                        {
                            if (terrains != null)
                            {
                                foreach (Terrain terrain in terrains)
                                {
                                    terrain.detailObjectDensity = 0.1f;
                                    terrain.detailObjectDistance = 40f;
                                    terrain.heightmapPixelError = 45f;
                                }
                            }

                            if (lodBias > 1)
                            {
                                QualitySettings.lodBias = 0.7f;
                            }

                            if (shadowDistance > 100)
                            {
                                QualitySettings.shadowDistance = 50f;
                            }

                            QualitySettings.shadowResolution = ShadowResolution.Low;
                            QualitySettings.shadowCascade4Split = new Vector3(0.1f, 0.3f, 0.5f);

                            if (waterReflections)
                            {
                                waterReflections = false;
                                GaiaWater.SetWaterReflectionsType(waterReflections, gaiaSettings.m_pipelineProfile.m_activePipelineInstalled, gaiaSettings.m_gaiaWaterProfile);
                            }
                            break;
                        }
                    case GaiaConstants.EnvironmentTarget.MobileAndVR:
                        {
                            if (terrains != null)
                            {
                                foreach (Terrain terrain in terrains)
                                {
                                    terrain.detailObjectDensity = 0.2f;
                                    terrain.detailObjectDistance = 60f;
                                    terrain.heightmapPixelError = 22f;
                                }
                            }

                            if (lodBias > 1)
                            {
                                QualitySettings.lodBias = 0.9f;
                            }

                            if (shadowDistance > 100)
                            {
                                QualitySettings.shadowDistance = 65f;
                            }

                            QualitySettings.shadowResolution = ShadowResolution.Low;
                            QualitySettings.shadowCascade4Split = new Vector3(0.1f, 0.4f, 0.6f);

                            if (waterReflections)
                            {
                                waterReflections = false;
                                GaiaWater.SetWaterReflectionsType(waterReflections, gaiaSettings.m_pipelineProfile.m_activePipelineInstalled, gaiaSettings.m_gaiaWaterProfile);
                            }
                            break;
                        }
                    case GaiaConstants.EnvironmentTarget.Desktop:
                        {
                            if (terrains != null)
                            {
                                foreach (Terrain terrain in terrains)
                                {
                                    terrain.detailObjectDensity = 0.3f;
                                    terrain.detailObjectDistance = 120f;
                                    terrain.heightmapPixelError = 13f;
                                }
                            }

                            if (lodBias > 1)
                            {
                                QualitySettings.lodBias = 1.1f;
                            }

                            if (shadowDistance > 150)
                            {
                                QualitySettings.shadowDistance = 100f;
                            }

                            QualitySettings.shadowResolution = ShadowResolution.Medium;
                            QualitySettings.shadowCascade4Split = new Vector3(0.3f, 0.45f, 0.7f);

                            if (waterReflections)
                            {
                                waterReflections = false;
                                GaiaWater.SetWaterReflectionsType(waterReflections, gaiaSettings.m_pipelineProfile.m_activePipelineInstalled, gaiaSettings.m_gaiaWaterProfile);
                            }
                            break;
                        }
                    case GaiaConstants.EnvironmentTarget.PowerfulDesktop:
                        {
                            if (terrains != null)
                            {
                                foreach (Terrain terrain in terrains)
                                {
                                    terrain.detailObjectDensity = 0.5f;
                                    terrain.detailObjectDistance = 175f;
                                    terrain.heightmapPixelError = 7f;
                                }
                            }

                            if (lodBias >= 2)
                            {
                                QualitySettings.lodBias = 1.5f;
                            }

                            if (shadowDistance > 200)
                            {
                                QualitySettings.shadowDistance = 125f;
                            }

                            QualitySettings.shadowResolution = ShadowResolution.High;
                            QualitySettings.shadowCascade4Split = new Vector3(0.25f, 0.5f, 0.7f);

                            if (waterReflections)
                            {
                                waterReflections = false;
                                GaiaWater.SetWaterReflectionsType(waterReflections, gaiaSettings.m_pipelineProfile.m_activePipelineInstalled, gaiaSettings.m_gaiaWaterProfile);
                            }
                            break;
                        }
                    case GaiaConstants.EnvironmentTarget.Custom:
                        {
                            Debug.Log("The target environment is set to 'Custom'. Unable to modify quick optimization settings in custom setting");
                            break;
                        }
                }

                if (EditorUtility.DisplayDialog("Info!", "Would you like to show the current settings that have been set in the console?", "Yes", "No"))
                {
                    if (gaiaSettings.m_currentEnvironment != GaiaConstants.EnvironmentTarget.Custom)
                    {
                        if (terrains != null)
                        {
                            foreach (Terrain terrain in terrains)
                            {
                                Debug.Log(terrain.name + " Detail Density is " + terrain.detailObjectDensity);
                                Debug.Log(terrain.name + " Detail Distance is " + terrain.detailObjectDistance);
                                Debug.Log(terrain.name + " Pixel Error is " + terrain.heightmapPixelError);
                            }
                        }

                        Debug.Log("LOD Bias in quality settings is set to " + QualitySettings.lodBias);
                        Debug.Log("Shadow Distance in quality settings is set to " + QualitySettings.shadowDistance);
                        Debug.Log("Shadow Resolution in quality settings is set to " + QualitySettings.shadowResolution.ToString());
                        Debug.Log("Water reflections is set to " + gaiaSettings.m_gaiaWaterProfile.m_enableReflections);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the dynamic GI
        /// </summary>
        public static void UpdateAmbientEnvironment()
        {
            DynamicGI.UpdateEnvironment();
        }

        #endregion
    }
}