using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;
#if HDPipeline
using UnityEngine.Experimental.Rendering.HDPipeline;
#endif
using System.Collections;
using ProcedualWorlds.WaterSystem.Reflections;

namespace Gaia.Pipeline.HDRP
{
    /// <summary>
    /// Static class that handles all the HDRP setup in Gaia
    /// </summary>
    public static class GaiaHDRPPipelineUtils
    {
        public static float m_waitTimer1 = 1f;
        public static float m_waitTimer2 = 3f;

        private static int m_markSceneDirty;

        /// <summary>
        /// Configures project for HDRP
        /// </summary>
        /// <param name="profile"></param>
        private static void ConfigureSceneToHDRP(UnityPipelineProfile profile)
        {
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            if (gaiaSettings.m_currentRenderer != GaiaConstants.EnvironmentRenderer.HighDefinition)
            {
                Debug.LogError("Unable to configure your scene/project to LWRP as the current render inside of gaia does not equal Lightweight as it's active render pipeline. This process [GaiaLWRPPipelineUtils.ConfigureSceneToLWRP()] will now exit.");
                return;
            }

            SetScriptingDefines(profile);

            if (profile.m_setHDPipelineProfile)
            {
                SetPipelineAsset(profile);
            }

            if (profile.m_HDAutoConfigureCamera)
            {
                ConfigureCamera();
            }

            if (profile.m_HDAutoConfigureLighting)
            {
                ConfigureLighting();
            }

            if (profile.m_HDAutoConfigureWater)
            {
                ConfigureWater(profile, gaiaSettings);
            }

            if (profile.m_HDAutoConfigureProbes)
            {
                ConfigureReflectionProbes();
            }

            if (profile.m_HDAutoConfigureTerrain)
            {
                ConfigureTerrain(profile);
            }

            FinalizeHDRP(profile);
        }

        /// <summary>
        /// Updates the scene lighting
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="pipelineProfile"></param>
        public static void UpdateSceneLighting(GaiaLightingProfileValues profile, UnityPipelineProfile pipelineProfile, GaiaLightingProfile lightingProfile)
        {
#if HDPipeline
            VolumeProfile volumeProfile = GetAndSetVolumeProfile(pipelineProfile);
            if (volumeProfile == null)
            {
                Debug.LogError("Volume profile could not be found. Check that 'Unity Pipeline Gaia Profile' HD Scene Lighting is set to 'HD Volume Profile'");
            }
            else
            {
                SetSkyboxAndFog(profile, volumeProfile);
                SetAmbientLight(profile, volumeProfile);
                SetVolumetricLighting(profile, volumeProfile);
                SetShadows(profile, volumeProfile);
                SetSunSettings(profile);
                SetAntiAliasing(lightingProfile.m_antiAliasingMode);

                GaiaLighting.RemovePostProcessing();
                if (lightingProfile.m_enablePostProcessing)
                {
                    SetPostProcessing(pipelineProfile, profile);
                }
                else
                {
                    RemovePostProcesing(pipelineProfile);
                }
            }
#endif

            //Marks the scene as dirty
            m_markSceneDirty++;
            if (m_markSceneDirty > 25)
            {
                m_markSceneDirty = 0;
                MarkSceneDirty(false);
            }
        }

#if HDPipeline
        /// <summary>
        /// Sets the skybox and fog settings 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="volumeProfile"></param>
        private static void SetSkyboxAndFog(GaiaLightingProfileValues profile, VolumeProfile volumeProfile)
        {
            VisualEnvironment visualEnvironment;
            HDRISky hDRISky;
            ProceduralSky proceduralSky;
            GradientSky gradientSky;
            ExponentialFog exponentialFog;
            LinearFog linearFog;
            VolumetricFog volumetricFog;
            if (volumeProfile.TryGet(out visualEnvironment))
            {
                if (volumeProfile.TryGet(out hDRISky))
                {
                    if (volumeProfile.TryGet(out proceduralSky))
                    {
                        if (volumeProfile.TryGet(out gradientSky))
                        {
                            switch (profile.m_hDSkyType)
                            {
                                case GaiaConstants.HDSkyType.HDRI:
                                    visualEnvironment.skyType.value = 1;
                                    hDRISky.active = true;
                                    hDRISky.hdriSky.value = profile.m_hDHDRISkybox;
                                    hDRISky.exposure.value = profile.m_hDHDRIExposure;
                                    hDRISky.multiplier.value = profile.m_hDHDRIMultiplier;
                                    hDRISky.rotation.value = profile.m_sunRotation;

                                    proceduralSky.active = false;
                                    gradientSky.active = false;
                                    break;
                                case GaiaConstants.HDSkyType.Procedural:
                                    visualEnvironment.skyType.value = 2;
                                    hDRISky.active = false;
                                    proceduralSky.active = true;
                                    proceduralSky.enableSunDisk.value = profile.m_hDProceduralEnableSunDisk;
                                    proceduralSky.includeSunInBaking.value = profile.m_hDProceduralIncludeSunInBaking;
                                    proceduralSky.sunSize.value = profile.m_hDProceduralSunSize;
                                    proceduralSky.sunSizeConvergence.value = profile.m_hDProceduralSunSizeConvergence;
                                    proceduralSky.atmosphereThickness.value = profile.m_hDProceduralAtmosphereThickness;
                                    proceduralSky.skyTint.value = profile.m_hDProceduralSkyTint;
                                    proceduralSky.groundColor.value = profile.m_hDProceduralGroundColor;
                                    proceduralSky.exposure.value = profile.m_hDProceduralExposure;
                                    proceduralSky.multiplier.value = profile.m_hDProceduralMultiplier;

                                    gradientSky.active = false;
                                    break;
                                case GaiaConstants.HDSkyType.Gradient:
                                    visualEnvironment.skyType.value = 3;
                                    hDRISky.active = false;
                                    proceduralSky.active = false;
                                    gradientSky.active = true;
                                    gradientSky.top.value = profile.m_hDGradientTopColor;
                                    gradientSky.middle.value = profile.m_hDGradientMiddleColor;
                                    gradientSky.bottom.value = profile.m_hDGradientBottomColor;
                                    gradientSky.gradientDiffusion.value = profile.m_hDGradientDiffusion;
                                    gradientSky.exposure.value = profile.m_hDGradientExposure;
                                    gradientSky.multiplier.value = profile.m_hDGradientMultiplier;
                                    break;
                            }

                            if (volumeProfile.TryGet(out exponentialFog))
                            {
                                if (volumeProfile.TryGet(out linearFog))
                                {
                                    if (volumeProfile.TryGet(out volumetricFog))
                                    {
                                        switch (profile.m_hDFogType)
                                        {
                                            case GaiaConstants.HDFogType.None:
                                                visualEnvironment.fogType.value = FogType.None;
                                                exponentialFog.active = false;
                                                linearFog.active = false;
                                                volumetricFog.active = false;
                                                break;
                                            case GaiaConstants.HDFogType.Exponential:
                                                visualEnvironment.fogType.value = FogType.Exponential;
                                                exponentialFog.active = true;
                                                exponentialFog.density.value = profile.m_hDExponentialFogDensity;
                                                exponentialFog.fogDistance.value = profile.m_hDExponentialFogDistance;
                                                exponentialFog.fogBaseHeight.value = profile.m_hDExponentialFogBaseHeight;
                                                exponentialFog.fogHeightAttenuation.value = profile.m_hDExponentialFogHeightAttenuation;
                                                exponentialFog.maxFogDistance.value = profile.m_hDExponentialFogMaxDistance;

                                                linearFog.active = false;
                                                volumetricFog.active = false;
                                                break;
                                            case GaiaConstants.HDFogType.Linear:
                                                visualEnvironment.fogType.value = FogType.Linear;
                                                exponentialFog.active = false;
                                                linearFog.active = true;
                                                linearFog.density.value = profile.m_hDLinearFogDensity;
                                                linearFog.fogStart.value = profile.m_hDLinearFogStart;
                                                linearFog.fogEnd.value = profile.m_hDLinearFogEnd;
                                                linearFog.fogHeightStart.value = profile.m_hDLinearFogHeightStart;
                                                linearFog.fogHeightEnd.value = profile.m_hDLinearFogHeightEnd;
                                                linearFog.maxFogDistance.value = profile.m_hDLinearFogMaxDistance;

                                                volumetricFog.active = false;
                                                break;
                                            case GaiaConstants.HDFogType.Volumetric:
                                                visualEnvironment.fogType.value = FogType.Volumetric;
                                                exponentialFog.active = false;
                                                linearFog.active = false;
                                                volumetricFog.active = true;
                                                volumetricFog.albedo.value = profile.m_hDVolumetricFogScatterColor;
                                                volumetricFog.meanFreePath.value = profile.m_hDVolumetricFogDistance;
                                                volumetricFog.baseHeight.value = profile.m_hDVolumetricFogBaseHeight;
                                                volumetricFog.meanHeight.value = profile.m_hDVolumetricFogMeanHeight;
                                                volumetricFog.anisotropy.value = profile.m_hDVolumetricFogAnisotropy;
                                                volumetricFog.globalLightProbeDimmer.value = profile.m_hDVolumetricFogProbeDimmer;
                                                volumetricFog.maxFogDistance.value = profile.m_hDVolumetricFogMaxDistance;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the ambient lighting settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="volumeProfile"></param>
        private static void SetAmbientLight(GaiaLightingProfileValues profile, VolumeProfile volumeProfile)
        {
            IndirectLightingController indirectLighting;
            VisualEnvironment visualEnvironment;

            if (volumeProfile.TryGet(out visualEnvironment))
            {
                switch (profile.m_hDAmbientMode)
                {
                    case GaiaConstants.HDAmbientMode.Static:
                        visualEnvironment.skyAmbientMode.value = SkyAmbientMode.Static;
                        break;
                    case GaiaConstants.HDAmbientMode.Dynamic:
                        visualEnvironment.skyAmbientMode.value = SkyAmbientMode.Dynamic;
                        break;
                }

                if (volumeProfile.TryGet(out indirectLighting))
                {
                    indirectLighting.indirectDiffuseIntensity.value = profile.m_hDAmbientDiffuseIntensity;
                    indirectLighting.indirectSpecularIntensity.value = profile.m_hDAmbientSpecularIntensity;
                }
            }
        }

        /// <summary>
        /// Sets the volumetric lighting quality settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="volumeProfile"></param>
        private static void SetVolumetricLighting(GaiaLightingProfileValues profile, VolumeProfile volumeProfile)
        {
            VolumetricLightingController volumetricLightingController;
            if (volumeProfile.TryGet(out volumetricLightingController))
            {
                volumetricLightingController.depthExtent.value = profile.m_hDVolumetricFogDepthExtent;
                volumetricLightingController.sliceDistributionUniformity.value = profile.m_hDVolumetricFogSliceDistribution;
            }
        }

        /// <summary>
        /// Sets the shadow settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="volumeProfile"></param>
        private static void SetShadows(GaiaLightingProfileValues profile, VolumeProfile volumeProfile)
        {
            HDShadowSettings hDShadow;
            if (volumeProfile.TryGet(out hDShadow))
            {
                hDShadow.maxShadowDistance.value = profile.m_hDShadowDistance;
            }
        }

        /// <summary>
        /// Sets the HDRP Anti Aliasing
        /// </summary>
        /// <param name="antiAliasingMode"></param>
        public static void SetAntiAliasing(GaiaConstants.GaiaProAntiAliasingMode antiAliasingMode)
        {
            Camera camera = GetCamera();
            if (camera != null)
            {
                HDAdditionalCameraData cameraData = camera.gameObject.GetComponent<HDAdditionalCameraData>();
                if (cameraData != null)
                {
                    switch(antiAliasingMode)
                    {
                        case GaiaConstants.GaiaProAntiAliasingMode.None:
                            cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
                            break;
                        case GaiaConstants.GaiaProAntiAliasingMode.FXAA:
                            cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;
                            break;
                        case GaiaConstants.GaiaProAntiAliasingMode.SMAA:
                            cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                            break;
                        case GaiaConstants.GaiaProAntiAliasingMode.TAA:
                            cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;
                            break;
                        case GaiaConstants.GaiaProAntiAliasingMode.MSAA:
                            Debug.Log("MSAA Anti Aliasing is not supported in HDRP. Please select a different Anti Aliasing method");
                            break;
                    }
                }
            }
        }
#endif  
        /// <summary>
        /// Sets the sun intensity
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="volumeProfile"></param>
        private static void SetSunSettings(GaiaLightingProfileValues profile)
        {
            Light light = GetSunLight();
            if (light != null)
            {
                light.color = profile.m_hDSunColor;
                light.intensity = profile.m_hDSunIntensity;
            }

            //Rotates the sun to specified values
            RotateSun(profile.m_sunRotation, profile.m_sunPitch, light);
        }


        /// <summary>
        /// Sets post processing
        /// </summary>
        /// <param name="pipelineProfile"></param>
        /// <param name="profile"></param>
        private static void SetPostProcessing(UnityPipelineProfile pipelineProfile, GaiaLightingProfileValues profile)
        {
#if HDPipeline
            if (string.IsNullOrEmpty(profile.m_hDPostProcessingProfile))
            {
                Debug.LogError("Post Processing Profile in selected profile for HDRP is empty please make sure you've defined a profile. Exiting function!");
                return;
            }
            Volume volume = GetAndSetPostProcessVolume(pipelineProfile, profile);
            bool loadProfile = false;
            if (volume.sharedProfile == null)
            {
                loadProfile = true;
            }
            else if (volume.sharedProfile.name != profile.m_hDPostProcessingProfile)
            {
                loadProfile = true;
            }

            if (loadProfile)
            {
                volume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(profile.m_hDPostProcessingProfile));
            }
#endif
        }

        /// <summary>
        /// Removes HDRP Post processing
        /// </summary>
        /// <param name="pipelineProfile"></param>
        private static void RemovePostProcesing(UnityPipelineProfile pipelineProfile)
        {
            GameObject postObject = GameObject.Find(pipelineProfile.m_HDPostVolumeObjectName);
            if (postObject != null)
            {
                Object.DestroyImmediate(postObject);
            }
        }

        /// <summary>
        /// Set the suns rotation and pitch
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="pitch"></param>
        private static void RotateSun(float rotation, float pitch, Light sunLight)
        {
            //Correct the angle
            float angleDegrees = rotation % 360f;

            float sunAngle = pitch;

            //Set new directional light rotation
            if (sunLight != null)
            {
                Vector3 newRotation = sunLight.gameObject.transform.rotation.eulerAngles;
                newRotation.y = angleDegrees;
                sunLight.gameObject.transform.rotation = Quaternion.Euler(sunAngle, -newRotation.y, 0f);
            }
        }

        /// <summary>
        /// Configures scripting defines in the project
        /// </summary>
        public static void SetScriptingDefines(UnityPipelineProfile profile)
        {
            bool isChanged = false;
            string currBuildSettings = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!currBuildSettings.Contains("HDPipeline"))
            {
                if (string.IsNullOrEmpty(currBuildSettings))
                {
                    currBuildSettings = "HDPipeline";
                }
                else
                {
                    currBuildSettings += ";HDPipeline";
                }
                isChanged = true;
            }
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
        /// Sets the pipeline asset to the procedural worlds asset if the profile is set yo change it
        /// </summary>
        /// <param name="profile"></param>
        public static void SetPipelineAsset(UnityPipelineProfile profile)
        {
            if (GraphicsSettings.renderPipelineAsset == null)
            {
                GraphicsSettings.renderPipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(GetAssetPath(profile.m_highDefinitionPipelineProfile));
            }
            else if (GraphicsSettings.renderPipelineAsset.name != profile.m_highDefinitionPipelineProfile)
            {
                GraphicsSettings.renderPipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(GetAssetPath(profile.m_highDefinitionPipelineProfile));
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
#if HDPipeline
                HDAdditionalCameraData cameraData = camera.gameObject.GetComponent<HDAdditionalCameraData>();
                if (cameraData == null)
                {
                    cameraData = camera.gameObject.AddComponent<HDAdditionalCameraData>();
                }
#endif
            }
        }

        /// <summary>
        /// Configures lighting to LWRP
        /// </summary>
        private static void ConfigureLighting()
        {
#if HDPipeline
            HDAdditionalLightData[] lightsData = Object.FindObjectsOfType<HDAdditionalLightData>();
            if (lightsData != null)
            {
                foreach(HDAdditionalLightData data in lightsData)
                {
                    if (data.gameObject.GetComponent<HDAdditionalLightData>() == null)
                    {
                        data.gameObject.AddComponent<HDAdditionalLightData>();
                    }
                }
            }
#endif
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
                    waterMaterial.shader = Shader.Find(profile.m_highDefinitionWaterShader);
                }
            }

            if (profile.m_underwaterHorizonMaterial != null)
            {
                profile.m_underwaterHorizonMaterial.shader = Shader.Find(profile.m_highDefinitionHorizonObjectShader);
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
                foreach (ReflectionProbe probe in reflectionProbes)
                {
#if HDPipeline
                    if (probe.GetComponent<HDAdditionalReflectionData>() == null)
                    {
                        probe.gameObject.AddComponent<HDAdditionalReflectionData>();
                    }
#endif
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
                foreach(Terrain terrain in terrains)
                {
#if !UNITY_2019_2_OR_NEWER
                    terrain.materialType = Terrain.MaterialType.Custom;
#endif
                    terrain.materialTemplate = profile.m_highDefinitionTerrainMaterial;
                }
            }
        }

        /// <summary>
        /// Finalizes the LWRP setup for your project 
        /// </summary>
        /// <param name="profile"></param>
        private static void FinalizeHDRP(UnityPipelineProfile profile)
        {
            MarkSceneDirty(true);
            profile.m_activePipelineInstalled = GaiaConstants.EnvironmentRenderer.HighDefinition;
        }

        /// <summary>
        /// Sets up default HDRP lighting in your scene
        /// </summary>
        public static void SetupDefaultSceneLighting(UnityPipelineProfile profile)
        {
#if !HDPipeline
            Debug.LogError("HDRP has  not been installed with Gaia. Please go to standard tab and extra settings to install HDRP into Gaia");
#else
            Volume sceneVolume = Object.FindObjectOfType<Volume>();
            if (sceneVolume != null)
            {
                if (sceneVolume.sharedProfile.Has<VisualEnvironment>())
                {
                    return;
                }
            }

            if (EditorUtility.DisplayDialog("Setup Default HDRP Lighting", "It looks like you are starting with a new terrain and HDRP lighting is not setup in your scene. Would you like to setup default HDRP lighting suitable for editing your terrain?", "Yes", "No"))
            {
                GameObject parentObject = GetOrCreateParentObject("Gaia Lighting Environment", true);
                if (string.IsNullOrEmpty(profile.m_HDDefaultSceneLighting))
                {
                    Debug.LogError("'HD Default Scene Lighting' is empty please check 'Unity Pipeline Gaia Profile' name input is not empty");
                    return;
                }
                else
                {
                    if (string.IsNullOrEmpty(profile.m_HDVolumeObjectName))
                    {
                        Debug.LogError("'HD Volume Object' is empty please check 'Unity Pipeline Gaia Profile' name input is not empty");
                        return;
                    }
                    GameObject volumeObject = GameObject.Find(profile.m_HDVolumeObjectName);
                    if (volumeObject == null)
                    {
                        volumeObject = new GameObject(profile.m_HDVolumeObjectName);
                        volumeObject.AddComponent<Volume>();
                    }

                    Volume volume = volumeObject.GetComponent<Volume>();
                    if (volume == null)
                    {
                        volume = volumeObject.AddComponent<Volume>();
                        volume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(profile.m_HDDefaultSceneLighting));
                        volume.isGlobal = true;
                    }
                    else
                    {
                        volume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(profile.m_HDDefaultSceneLighting));
                        volume.isGlobal = true;
                    }

                    StaticLightingSky staticLighting = volumeObject.GetComponent<StaticLightingSky>();
                    if (staticLighting == null)
                    {
                        staticLighting = volumeObject.AddComponent<StaticLightingSky>();
                        staticLighting.profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(profile.m_HDDefaultSceneLighting));
                        staticLighting.staticLightingSkyUniqueID = 2;
                    }
                    else
                    {
                        staticLighting.profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(profile.m_HDDefaultSceneLighting));
                        staticLighting.staticLightingSkyUniqueID = 2;
                    }

                    Light sunLight = GetSunLight();
                    if (sunLight != null)
                    {
                        sunLight.intensity = 3.3f;
                    }

                    if (string.IsNullOrEmpty(profile.m_HDDefaultPostProcessing))
                    {
                        Debug.LogError("'HD Volume Object' is empty please check 'Unity Pipeline Gaia Profile' name input is not empty");
                        return;
                    }
                    GameObject postVolumeObject = GameObject.Find(profile.m_HDPostVolumeObjectName);
                    if (postVolumeObject == null)
                    {
                        postVolumeObject = new GameObject(profile.m_HDPostVolumeObjectName);
                        postVolumeObject.AddComponent<Volume>();
                    }

                    Volume postVolume = postVolumeObject.GetComponent<Volume>();
                    if (volume == null)
                    {
                        postVolume = postVolumeObject.AddComponent<Volume>();
                        postVolume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(profile.m_HDDefaultPostProcessing));
                        postVolume.isGlobal = true;
                    }
                    else
                    {
                        postVolume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(profile.m_HDDefaultPostProcessing));
                        postVolume.isGlobal = true;
                    }

                    GaiaLighting.RemovePostProcessing();

                    volumeObject.transform.SetParent(parentObject.transform);
                    postVolumeObject.transform.SetParent(parentObject.transform);
                }
            }

            GaiaLighting.RemovePostProcessing();
#endif
        }

        /// <summary>
        /// Cleans up LWRP components in the scene
        /// </summary>
        public static void CleanUpHDRP(UnityPipelineProfile profile, GaiaSettings gaiaSettings)
        {
#if HDPipeline
            HDAdditionalCameraData[] camerasData = Object.FindObjectsOfType<HDAdditionalCameraData>();
            if (camerasData != null)
            {
                foreach (HDAdditionalCameraData data in camerasData)
                {
                    Object.DestroyImmediate(data);
                }
            }

            HDAdditionalLightData[] lightsData = Object.FindObjectsOfType<HDAdditionalLightData>();
            if (lightsData != null)
            {
                foreach (HDAdditionalLightData data in lightsData)
                {
                    Object.DestroyImmediate(data);
                }
            }

            HDAdditionalReflectionData[] reflectionsData = Object.FindObjectsOfType<HDAdditionalReflectionData>();
            if (reflectionsData != null)
            {
                foreach (HDAdditionalReflectionData data in reflectionsData)
                {
                    Object.DestroyImmediate(data);
                }
            }
#endif



            GameObject hdEnvironment = GameObject.Find("HD Environment Volume");
            if (hdEnvironment != null)
            {
                Object.DestroyImmediate(hdEnvironment);
            }

            GameObject hdPostProcessEnvironment = GameObject.Find("HD Post Processing Environment Volume");
            if (hdPostProcessEnvironment != null)
            {
                Object.DestroyImmediate(hdPostProcessEnvironment);
            }

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
        /// Starts the LWRP Setup
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static IEnumerator StartHDRPSetup(UnityPipelineProfile profile)
        {
            if (profile == null)
            {
                Debug.LogError("UnityPipelineProfile is empty");
                yield return null;
            }
            else
            {
                EditorUtility.DisplayProgressBar("Installing HighDefinition2018x", "Updating scripting defines", 0.5f);
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

                EditorUtility.DisplayProgressBar("Installing HighDefinition2018x", "Updating scene to HighDefinition2018x", 0.75f);
                m_waitTimer2 -= Time.deltaTime;
                if (m_waitTimer2 < 0)
                {
                    ConfigureSceneToHDRP(profile);
                    SetupDefaultSceneLighting(profile);
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

#if HDPipeline
        /// <summary>
        /// Sets the volume profile
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        private static VolumeProfile GetAndSetVolumeProfile(UnityPipelineProfile profile)
        {
            VolumeProfile volumeProfile = null;
            GameObject volumeObject = GetOrCreateVolumeObject(profile);

            volumeProfile = volumeObject.GetComponent<Volume>().sharedProfile;

            return volumeProfile;
        }

        /// <summary>
        /// Gets and returns the post processing volume component
        /// </summary>
        /// <param name="pipelineProfile"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        private static Volume GetAndSetPostProcessVolume(UnityPipelineProfile pipelineProfile, GaiaLightingProfileValues profile)
        {
            Volume volume = null;
            GameObject volumeObject = GameObject.Find(pipelineProfile.m_HDPostVolumeObjectName);
            if (volumeObject == null)
            {
                volumeObject = new GameObject(pipelineProfile.m_HDPostVolumeObjectName);
                volume = volumeObject.AddComponent<Volume>();
                volume.isGlobal = true;
            }
            else
            {
                volume = volumeObject.GetComponent<Volume>();
                if (volume == null)
                {
                    volume = volumeObject.AddComponent<Volume>();
                    volume.isGlobal = true;
                }
                else
                {
                    volume.isGlobal = true;
                }
            }

            GameObject parentObject = GetOrCreateParentObject("Gaia Lighting Environment", true);
            volumeObject.transform.SetParent(parentObject.transform);

            return volume;
        }

        /// <summary>
        /// Gets or creates volume object
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        private static GameObject GetOrCreateVolumeObject(UnityPipelineProfile profile)
        {
            GameObject volumeObject = GameObject.Find(profile.m_HDVolumeObjectName);
            Volume volume = null;
            if (volumeObject == null)
            {
                volumeObject = new GameObject(profile.m_HDVolumeObjectName);
                volume = volumeObject.AddComponent<Volume>();
                volume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(profile.m_HDSceneLighting));
                volume.isGlobal = true;
            }
            else
            {
                volume = volumeObject.GetComponent<Volume>();
                if (volume == null)
                {
                    volume = volumeObject.AddComponent<Volume>();
                    volume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(profile.m_HDSceneLighting));
                    volume.isGlobal = true;
                }
                else
                {
                    volume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GetAssetPath(profile.m_HDSceneLighting));
                    volume.isGlobal = true;
                }
            }

            GameObject parentObject = GetOrCreateParentObject("Gaia Lighting Environment", true);
            volumeObject.transform.SetParent(parentObject.transform);

            return volumeObject;
        }
#endif
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
            foreach (Light activeLight in lights)
            {
                if (activeLight.type == LightType.Directional)
                {
                    return activeLight;
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

            return theParentGo;
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
