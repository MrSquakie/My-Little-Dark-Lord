using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ProcedualWorlds.WaterSystem.MeshGeneration;
using ProcedualWorlds.WaterSystem.Reflections;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Gaia
{
    public static class GaiaWater
    {
        #region Variables

        //Water profiles
        public static List<GaiaWaterProfileValues> m_waterProfiles;
        public static GaiaWaterProfile m_waterProfile;

        //Water shader that is found
        public static string m_waterShader = "PWS/SP/Water/Ocean vP2.1 2019_01_14";

        //Parent Object Values
        public static GameObject m_parentObject;

        //Stores the gaia settings
        private static GaiaSettings m_gaiaSettings;

        #endregion

        #region Setup

        /// <summary>
        /// Starts the setup process for selected lighting
        /// </summary>
        /// <param name="waterType"></param>
        /// <param name="renderPipeline"></param>
        public static void GetProfile(GaiaConstants.GaiaWaterProfileType waterType, GaiaWaterProfile waterProfile)
        {
            m_waterProfile = waterProfile;
            if (m_waterProfile == null)
            {
                m_waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));
            }

            if (m_waterProfile == null)
            {
                Debug.LogError("[AmbientSkiesSamples.GetProfile()] Asset 'Gaia Water System Profile' could not be found please make sure it exists within your project or that the name has not been changed. Due to this error the method will now exit.");
                return;
            }
            else
            {
                if (m_parentObject == null)
                {
                    m_parentObject = GetOrCreateParentObject("Gaia Water Environment", true);
                }

                bool wasSuccessfull = false;
                if (m_waterProfiles == null)
                {
                    m_waterProfiles = m_waterProfile.m_waterProfiles;
                    foreach (GaiaWaterProfileValues profile in m_waterProfiles)
                    {
                        if (profile.m_profileType == waterType)
                        {
                            UpdateGlobalWater(profile);
                            wasSuccessfull = true;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (GaiaWaterProfileValues profile in m_waterProfiles)
                    {
                        if (profile.m_profileType == waterType)
                        {
                            UpdateGlobalWater(profile);
                            wasSuccessfull = true;
                            break;
                        }
                    }

                    if (!wasSuccessfull)
                    {
                        Debug.LogError("[AmbientSkiesSamples.GetProfile()] No profile type matches one you have selected. Have you modified GaiaConstants.GaiaLightingProfileType?");
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
        private static void UpdateGlobalWater(GaiaWaterProfileValues profile)
        {
            //Spawns the water prefab in the scene
            SpawnWater(m_waterProfile.m_waterPrefab);

            //Check Water shader is good
            if (CheckWaterMaterialAndShader(m_waterProfile.m_masterWaterMaterial))
            {
                //Sets global settings
                SetGlobalSettings(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets water tint colors
                SetWaterColorTint(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets water opacity
                SetWaterOpacity(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets water main settings
                SetWaterMainSettings(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets water refractions
                SetWaterRefraction(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets water lighting
                SetWaterLighting(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets water shadows
                SetWaterShadow(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets water reflections
                SetWaterReflections(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets the water reflection component
                SetWaterReflections(m_waterProfile.m_enableReflections);
                //Sets water normals
                SetWaterNormals(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets water tessellation
                SetWaterTessellation(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets water waves and wind
                SetWaterWavesAndWind(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets water ocean foam
                SetWaterOceanFoam(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets water beach foam
                SetWaterBeachFoam(profile, m_waterProfile.m_masterWaterMaterial);
                //Sets the waters reflection settings
                SetupWaterReflectionSettings(m_waterProfile, false);
                //Sets the underwater effects in the scene
                SetUnderwaterEffects(m_waterProfile, profile);
                //Sets the auto wind setup on the water in the scene
                SetAutoWind(m_waterProfile);
                //Water mesh generation
                UpdateWaterMeshQuality(m_waterProfile, m_waterProfile.m_waterPrefab);
                //Mark water as dirty
                MarkWaterMaterialDirty(m_waterProfile.m_masterWaterMaterial);
            }
            else
            {
                Debug.LogError("[GaiaProWater.UpdateGlobalWater()] Shader of the material does not = " + m_waterShader + " Or master water material in the water profile is empty");
            }

            //Destroys the parent object if it contains no partent childs
            DestroyParent("Ambient Skies Samples Environment");
        }

        /// <summary>
        /// Sets the global settings of the water
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetGlobalSettings(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.enableInstancing = m_waterProfile.m_enableGPUInstancing;

            waterMaterial.SetFloat("_EnableReflection", 1f);

            if (m_waterProfile.m_enableOceanFoam)
            {
                waterMaterial.SetFloat("_EnableOceanFoam", 1f);
            }
            else
            {
                waterMaterial.SetFloat("_EnableOceanFoam", 0f);
            }

            if (m_waterProfile.m_enableBeachFoam)
            {
                waterMaterial.SetFloat("_EnableBeachFoam", 1f);
            }
            else
            {
                waterMaterial.SetFloat("_EnableBeachFoam", 0f);
            }
        }

        /// <summary>
        /// Sets the water tint coloring
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterColorTint(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.SetColor("_WaterTint", profile.m_waterTint);
            waterMaterial.SetColor("_ShallowTint", profile.m_shallowTint);
            waterMaterial.SetFloat("_ShallowOffset", profile.m_shallowOffset);
            waterMaterial.SetColor("_DepthTint", profile.m_depthTint);
            waterMaterial.SetFloat("_DepthOffset", profile.m_depthOffset);
        }

        /// <summary>
        /// Sets the waters opacity settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterOpacity(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.SetFloat("_OpacityOcean", profile.m_opacityOcean);
            waterMaterial.SetFloat("_OpacityBeach", profile.m_opacityBeach);
            if (profile.m_ignoreVertexColor)
            {
                waterMaterial.SetFloat("_IgnoreVertexColor", 1f);
            }
            else
            {
                waterMaterial.SetFloat("_IgnoreVertexColor", 0f);
            }
        }

        /// <summary>
        /// Sets the waters main settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterMainSettings(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.SetFloat("_Occlusion", profile.m_occlusion);
            waterMaterial.SetFloat("_Metallic", profile.m_metallic);
            waterMaterial.SetFloat("_Smoothness", profile.m_smoothness);
            waterMaterial.SetFloat("_SmoothnessVariance", profile.m_smoothnessVariance);
            waterMaterial.SetFloat("_SmoothnessThreshold", profile.m_smoothnessThreshold);
        }

        /// <summary>
        /// Sets the waters refraction settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterRefraction(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.SetFloat("_RefractedDepth", profile.m_refractedDepth);
            waterMaterial.SetFloat("_RefractionScale", profile.m_refractionScale);
        }

        /// <summary>
        /// Sets the waters lighting settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterLighting(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.SetFloat("_LightIndirectStrengthSpecular", profile.m_indirectLightSpecular);
            waterMaterial.SetFloat("_LightIndirectStrengthDiffuse", profile.m_indirectLightDiffuse);
            waterMaterial.SetColor("_HighlightTint", profile.m_highlightTint);
            waterMaterial.SetFloat("_HighlightOffset", profile.m_highlightOffset);
            waterMaterial.SetFloat("_HighlightSharpness", profile.m_highlightSharpness);
        }

        /// <summary>
        /// Sets the waters shadow settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterShadow(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.SetFloat("_ShadowStrength", profile.m_shadowStrength);
            waterMaterial.SetFloat("_ShadowSharpness", profile.m_shadowSharpness);
            waterMaterial.SetFloat("_ShadowOffset", profile.m_shadowOffset);
        }

        /// <summary>
        /// Sets the waters reflection settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterReflections(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.SetFloat("_ReflectionIntensity", profile.m_reflectionIntenisty);
            waterMaterial.SetFloat("_ReflectionWobble", profile.m_reflectionWobble);
            waterMaterial.SetFloat("_ReflectionFresnelPower", profile.m_reflectionFresnelPower);
            waterMaterial.SetFloat("_ReflectionFrenelScale", profile.m_reflectionFresnelScale);
        }

        /// <summary>
        /// Sets the waters normal settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterNormals(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.SetTexture("_NormalMap", profile.m_normalMap);
            waterMaterial.SetFloat("_NormalMapStrength", profile.m_normalStrength);
            waterMaterial.SetFloat("_NormalMapTiling", profile.m_normalTiling);
            waterMaterial.SetFloat("_NormalMapSpeed", profile.m_normalSpeed);
            waterMaterial.SetFloat("_NormalMapTimescale", profile.m_normalTimescale);
        }

        /// <summary>
        /// Sets the waters tessellation settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterTessellation(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.SetFloat("_TessellationStrenght", profile.m_tessellationAmount);
            waterMaterial.SetFloat("_TessellationMaxDistance", profile.m_tessellationDistance);
        }

        /// <summary>
        /// Sets the waters wave and wind settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterWavesAndWind(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.SetVector("_Wave001", profile.m_wave01);
            waterMaterial.SetFloat("_WaveSpeed", profile.m_waveSpeed);
            waterMaterial.SetVector("_Wave002", profile.m_wave02);
            waterMaterial.SetVector("_Wave003", profile.m_wave03);
        }

        /// <summary>
        /// Sets the waters ocean foam settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterOceanFoam(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.SetTexture("_OceanFoamMap", profile.m_oceanFoamTexture);
            waterMaterial.SetColor("_OceanFoamTint", profile.m_oceanFoamTint);
            waterMaterial.SetFloat("_OceanFoamTiling", profile.m_oceanFoamTiling);
            waterMaterial.SetFloat("_OceanFoamStrength", profile.m_oceanFoamAmount);
            waterMaterial.SetFloat("_OceanFoamDistance", profile.m_oceanFoamDistance);
            waterMaterial.SetFloat("_OceanFoamSpeed", profile.m_oceanFoamSpeed);
        }

        /// <summary>
        /// Sets the waters beach foam settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterBeachFoam(GaiaWaterProfileValues profile, Material waterMaterial)
        {
            waterMaterial.SetTexture("_BeachFoamMap", profile.m_beachFoamTexture);
            waterMaterial.SetColor("_BeachFoamTint", profile.m_beachFoamTint);
            waterMaterial.SetFloat("_BeachFoamTiling", profile.m_beachFoamTiling);
            waterMaterial.SetFloat("_BeachFoamStrength", profile.m_beachFoamAmount);
            waterMaterial.SetFloat("_BeachFoamDistance", profile.m_beachFoamDistance);
            waterMaterial.SetFloat("_BeachFoamSpeed", profile.m_beachFoamSpeed);
        }

        /// <summary>
        /// Sets the underwater effects
        /// </summary>
        /// <param name="profile"></param>
        private static void SetUnderwaterEffects(GaiaWaterProfile profile, GaiaWaterProfileValues profileValues)
        {
            if (profile.m_supportUnderwaterEffects)
            {
                float seaLevel = 0f;
                GaiaSceneInfo sceneInfo = GaiaSceneInfo.GetSceneInfo();
                if (sceneInfo != null)
                {
                    seaLevel = sceneInfo.m_seaLevel;
                }
                if (m_parentObject == null)
                {
                    m_parentObject = GetOrCreateParentObject("Gaia Water Environment", true);
                }
                GameObject underwaterEffectsObject = GameObject.Find("Underwater Effects");
                if (underwaterEffectsObject == null)
                {
                    underwaterEffectsObject = new GameObject("Underwater Effects");
                }

                GaiaUnderwaterEffects underwaterEffects = underwaterEffectsObject.GetComponent<GaiaUnderwaterEffects>();
                if (underwaterEffects == null)
                {
                    underwaterEffects = underwaterEffectsObject.AddComponent<GaiaUnderwaterEffects>();
                    underwaterEffects.LoadUnderwaterSystemAssets();

                    GameObject underwaterParticles = GameObject.Find("Underwater Particles Effects");
                    if (profile.m_supportUnderwaterParticles)
                    {
                        if (underwaterParticles == null)
                        {
                            underwaterParticles = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(GaiaUtils.GetAssetPath("Underwater Particles Effects.prefab"))) as GameObject;
                            FollowPlayerSystem followPlayer = underwaterParticles.AddComponent<FollowPlayerSystem>();
                            followPlayer.m_player = underwaterEffects.m_playerCamera;
                            followPlayer.m_particleObjects.Add(underwaterParticles);
                            underwaterEffects.m_underwaterParticles = underwaterParticles;
                            underwaterEffects.m_fogColor = profileValues.m_underwaterFogColor;
                            underwaterEffects.m_fogDistance = profileValues.m_underwaterFogDistance;
                            underwaterEffects.m_seaLevel = seaLevel;
                            underwaterEffects.m_waterReflections = Object.FindObjectOfType<PWS_WaterReflections>();
                            underwaterParticles.transform.position = underwaterEffects.m_playerCamera.transform.position;
                        }
                        else
                        {
                            FollowPlayerSystem followPlayer = underwaterParticles.GetComponent<FollowPlayerSystem>();
                            if (followPlayer != null)
                            {
                                followPlayer.m_player = underwaterEffects.m_playerCamera;
                            }

                            underwaterEffects.m_seaLevel = seaLevel;
                            underwaterEffects.m_fogColor = profileValues.m_underwaterFogColor;
                            underwaterEffects.m_fogDistance = profileValues.m_underwaterFogDistance;
                            underwaterEffects.m_waterReflections = Object.FindObjectOfType<PWS_WaterReflections>();
                            underwaterParticles.transform.position = underwaterEffects.m_playerCamera.transform.position;
                        }

                        underwaterParticles.transform.SetParent(underwaterEffectsObject.transform);
                    }
                    else
                    {
                        if (underwaterParticles != null)
                        {
                            Object.DestroyImmediate(underwaterParticles);
                        }
                    }

                    underwaterEffectsObject.transform.SetParent(m_parentObject.transform);
                }
                else
                {
                    underwaterEffects.LoadUnderwaterSystemAssets();
                    underwaterEffects.m_seaLevel = seaLevel;
                    underwaterEffects.m_supportFog = profile.m_supportUnderwaterFog;
                    underwaterEffects.m_fogColor = profileValues.m_underwaterFogColor;
                    underwaterEffects.m_fogDistance = profileValues.m_underwaterFogDistance;

                    GameObject underwaterParticles = GameObject.Find("Underwater Particles Effects");
                    if (profile.m_supportUnderwaterParticles)
                    {
                        if (underwaterParticles == null)
                        {
                            underwaterParticles = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(GaiaUtils.GetAssetPath("Underwater Particles Effects.prefab"))) as GameObject;
                            FollowPlayerSystem followPlayer = underwaterParticles.AddComponent<FollowPlayerSystem>();
                            followPlayer.m_player = underwaterEffects.m_playerCamera;
                            underwaterParticles.transform.position = underwaterEffects.m_playerCamera.transform.position;
                        }
                        else
                        {
                            FollowPlayerSystem followPlayer = underwaterParticles.GetComponent<FollowPlayerSystem>();
                            if (followPlayer != null)
                            {
                                followPlayer.m_player = underwaterEffects.m_playerCamera;
                            }

                            underwaterParticles.transform.position = underwaterEffects.m_playerCamera.transform.position;
                        }

                        underwaterParticles.transform.SetParent(underwaterEffectsObject.transform);
                    }
                    else
                    {
                        if (underwaterParticles != null)
                        {
                            Object.DestroyImmediate(underwaterParticles);
                        }
                    }

                    underwaterEffectsObject.transform.SetParent(m_parentObject.transform);
                }

                if (profile.m_supportUnderwaterFog)
                {
                    GameObject underwaterHorizon = GameObject.Find("Ambient Underwater Horizon");
                    if (underwaterHorizon == null)
                    {
                        underwaterHorizon = PrefabUtility.InstantiatePrefab(profile.m_underwaterHorizonPrefab) as GameObject;
                        underwaterEffects.m_horizonObject = underwaterHorizon;
                        FollowPlayerSystem followPlayer = underwaterHorizon.AddComponent<FollowPlayerSystem>();
                        followPlayer.m_followPlayer = true;
                        followPlayer.m_particleObjects.Add(underwaterHorizon);
                        followPlayer.m_player = underwaterEffects.m_playerCamera;
                        followPlayer.m_useOffset = true;
                        followPlayer.m_offset = 1000f;
                        underwaterHorizon.transform.SetParent(underwaterEffectsObject.transform);
                        underwaterHorizon.transform.position = new Vector3(0f, -4000f, 0f);
                    }
                    else
                    {
                        FollowPlayerSystem followPlayer = underwaterHorizon.GetComponent<FollowPlayerSystem>();
                        if (followPlayer != null)
                        {
                            underwaterEffects.m_horizonObject = underwaterHorizon;
                            followPlayer.m_followPlayer = true;
                            followPlayer.m_player = underwaterEffects.m_playerCamera;
                            followPlayer.m_useOffset = true;
                            followPlayer.m_offset = 1000f;
                            underwaterHorizon.transform.SetParent(underwaterEffectsObject.transform);
                        }
                    }
                }
                else
                {
                    GameObject underwaterHorizon = GameObject.Find("Ambient Underwater Horizon");
                    if (underwaterHorizon != null)
                    {
                        Object.DestroyImmediate(underwaterHorizon);
                    }
                }

                #if UNITY_POST_PROCESSING_STACK_V2
                if (profile.m_supportUnderwaterPostProcessing)
                {
                    GameObject postProcessObject = GameObject.Find("Underwater Post Processing");
                    if (postProcessObject == null)
                    {
                        postProcessObject = new GameObject("Underwater Post Processing");
                        postProcessObject.transform.SetParent(underwaterEffectsObject.transform);
                        postProcessObject.transform.position = new Vector3(0f, -3500f + seaLevel, 0f);
                        postProcessObject.layer = LayerMask.NameToLayer("TransparentFX");

                        PostProcessVolume postProcessVolume = postProcessObject.AddComponent<PostProcessVolume>();
                        postProcessVolume.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GetAssetPath(profileValues.m_postProcessingProfile));
                        postProcessVolume.priority = 3f;

                        BoxCollider boxCollider = postProcessObject.AddComponent<BoxCollider>();
                        boxCollider.isTrigger = true;
                        boxCollider.size = new Vector3(10000f, 7000f, 10000f);
                    }
                    else
                    {
                        postProcessObject.transform.SetParent(underwaterEffectsObject.transform);
                        postProcessObject.transform.position = new Vector3(0f, -3500f + seaLevel, 0f);
                        postProcessObject.layer = LayerMask.NameToLayer("TransparentFX");

                        PostProcessVolume postProcessVolume = postProcessObject.GetComponent<PostProcessVolume>();
                        if (postProcessVolume != null)
                        {
                            postProcessVolume.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GetAssetPath(profileValues.m_postProcessingProfile));
                            postProcessVolume.priority = 3f;
                        }

                        BoxCollider boxCollider = postProcessObject.GetComponent<BoxCollider>();
                        if (boxCollider != null)
                        {
                            boxCollider.isTrigger = true;
                            boxCollider.size = new Vector3(10000f, 7000f, 10000f);
                        }
                    }
                }
                else
                {
                    GameObject postProcessObject = GameObject.Find("Underwater Post Processing");
                    if (postProcessObject != null)
                    {
                        Object.DestroyImmediate(postProcessObject);
                    }
                }
                #endif
            }
            else
            {
                GameObject underwaterEffects = GameObject.Find("Underwater Effects");
                if (underwaterEffects != null)
                {
                    Object.DestroyImmediate(underwaterEffects);
                }
            }
        }

        /// <summary>
        /// Sets up the auto wind setup on the water
        /// </summary>
        /// <param name="profile"></param>
        private static void SetAutoWind(GaiaWaterProfile profile)
        {
            GameObject waterObject = GameObject.Find("Ambient Water Sample");
            if (waterObject != null)
            {
                GaiaWaterWind waterWind = waterObject.GetComponent<GaiaWaterWind>();
                if (profile.m_autoWindControlOnWater)
                {
                    if (waterWind == null)
                    {
                        waterWind = waterObject.AddComponent<GaiaWaterWind>();
                        if (waterWind.m_waterMaterial == null)
                        {
                            waterWind.m_waterMaterial = waterObject.GetComponent<Renderer>().sharedMaterial;
                        }
                        if (waterWind.m_windZone == null)
                        {
                            waterWind.m_windZone = Object.FindObjectOfType<WindZone>();
                        }
                    }
                }
                else
                {
                    if (waterWind != null)
                    {
                        Object.DestroyImmediate(waterWind);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the water mesh quality
        /// </summary>
        /// <param name="profile"></param>
        public static void UpdateWaterMeshQuality(GaiaWaterProfile profile, GameObject waterObject)
        {
            if (profile.m_enableWaterMeshQuality)
            {
                GameObject waterGameObject = GameObject.Find(waterObject.name);
                if (waterGameObject == null)
                {
                    Debug.LogWarning("Water has not been added to the scene. Please add it to the scene then try configure the water mesh quality.");
                }
                else
                {
                    PWS_MeshGenerationPro waterGeneration = waterGameObject.GetComponent<PWS_MeshGenerationPro>();
                    if (waterGeneration == null)
                    {
                        waterGeneration = waterGameObject.AddComponent<PWS_MeshGenerationPro>();
                        waterGeneration.m_MeshType = profile.m_meshType;
                        waterGeneration.m_Size.x = profile.m_xSize;
                        waterGeneration.m_Size.z = profile.m_zSize;

                        switch (profile.m_waterMeshQuality)
                        {
                            case GaiaConstants.WaterMeshQuality.VeryLow:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 2;
                                    waterGeneration.m_meshPoints.y = 2;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 4;
                                    waterGeneration.m_meshPoints.y = 4;
                                }

                                break;
                            case GaiaConstants.WaterMeshQuality.Low:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 4;
                                    waterGeneration.m_meshPoints.y = 4;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 8;
                                    waterGeneration.m_meshPoints.y = 8;
                                }

                                break;
                            case GaiaConstants.WaterMeshQuality.Medium:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 8;
                                    waterGeneration.m_meshPoints.y = 8;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 16;
                                    waterGeneration.m_meshPoints.y = 16;
                                }
                                break;
                            case GaiaConstants.WaterMeshQuality.High:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 16;
                                    waterGeneration.m_meshPoints.y = 16;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 32;
                                    waterGeneration.m_meshPoints.y = 32;
                                }

                                break;
                            case GaiaConstants.WaterMeshQuality.VeryHigh:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 32;
                                    waterGeneration.m_meshPoints.y = 32;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 64;
                                    waterGeneration.m_meshPoints.y = 64;
                                }

                                break;
                            case GaiaConstants.WaterMeshQuality.Ultra:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 64;
                                    waterGeneration.m_meshPoints.y = 64;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 128;
                                    waterGeneration.m_meshPoints.y = 128;
                                }

                                break;
                            case GaiaConstants.WaterMeshQuality.Cinematic:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 128;
                                    waterGeneration.m_meshPoints.y = 128;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 256;
                                    waterGeneration.m_meshPoints.y = 256;
                                }

                                break;
                            case GaiaConstants.WaterMeshQuality.Custom:
                                waterGeneration.m_meshPoints.x = profile.m_customMeshQuality;
                                waterGeneration.m_meshPoints.y = profile.m_customMeshQuality;
                                break;
                        }
                    }
                    else
                    {
                        waterGeneration.m_MeshType = profile.m_meshType;
                        waterGeneration.m_Size.x = profile.m_xSize;
                        waterGeneration.m_Size.z = profile.m_zSize;

                        switch (profile.m_waterMeshQuality)
                        {
                            case GaiaConstants.WaterMeshQuality.VeryLow:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 2;
                                    waterGeneration.m_meshPoints.y = 2;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 4;
                                    waterGeneration.m_meshPoints.y = 4;
                                }

                                break;
                            case GaiaConstants.WaterMeshQuality.Low:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 4;
                                    waterGeneration.m_meshPoints.y = 4;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 8;
                                    waterGeneration.m_meshPoints.y = 8;
                                }

                                break;
                            case GaiaConstants.WaterMeshQuality.Medium:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 8;
                                    waterGeneration.m_meshPoints.y = 8;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 16;
                                    waterGeneration.m_meshPoints.y = 16;
                                }
                                break;
                            case GaiaConstants.WaterMeshQuality.High:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 16;
                                    waterGeneration.m_meshPoints.y = 16;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 32;
                                    waterGeneration.m_meshPoints.y = 32;
                                }

                                break;
                            case GaiaConstants.WaterMeshQuality.VeryHigh:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 32;
                                    waterGeneration.m_meshPoints.y = 32;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 64;
                                    waterGeneration.m_meshPoints.y = 64;
                                }

                                break;
                            case GaiaConstants.WaterMeshQuality.Ultra:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 64;
                                    waterGeneration.m_meshPoints.y = 64;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 128;
                                    waterGeneration.m_meshPoints.y = 128;
                                }

                                break;
                            case GaiaConstants.WaterMeshQuality.Cinematic:
                                if (waterGeneration.m_MeshType == PWS_MeshGenerationPro.MeshType.Circle)
                                {
                                    waterGeneration.m_meshPoints.x = 128;
                                    waterGeneration.m_meshPoints.y = 128;
                                }
                                else
                                {
                                    waterGeneration.m_meshPoints.x = 256;
                                    waterGeneration.m_meshPoints.y = 256;
                                }

                                break;
                            case GaiaConstants.WaterMeshQuality.Custom:
                                waterGeneration.m_meshPoints.x = profile.m_customMeshQuality;
                                waterGeneration.m_meshPoints.y = profile.m_customMeshQuality;
                                break;
                        }
                    }

                    waterGeneration.ProceduralMeshGeneration();
                }
            }
            else
            {
                GameObject waterGameObject = GameObject.Find(waterObject.name);
                if (waterGameObject == null)
                {
                    Debug.LogWarning("Water has not been added to the scene. Please add it to the scene then try configure the water mesh quality.");
                }
                else
                {
                    PWS_MeshGenerationPro waterGeneration = waterGameObject.GetComponent<PWS_MeshGenerationPro>();
                    if (waterGeneration != null)
                    {
                        Object.DestroyImmediate(waterGeneration);
                    }

                    MeshFilter meshFilter = waterGameObject.GetComponent<MeshFilter>();
                    if (meshFilter != null)
                    {
                        if (meshFilter.sharedMesh.name != "Ambient Water Sample")
                        {
                            meshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(GetAssetPath("Ambient Water Sample"));
                        }
                    }
                }
            }
        }

        #endregion

        #region Utils

        /// <summary>
        /// Sets water reflections up in the scene
        /// </summary>
        /// <param name="reflectionOn"></param>
        public static void SetWaterReflections(bool reflectionOn)
        {
            if (m_waterProfile == null)
            {
                m_waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));
            }

            GameObject waterObject = GameObject.Find("Ambient Water Sample");

            if (CheckWaterMaterialAndShader(m_waterProfile.m_masterWaterMaterial))
            {
                m_waterProfile.m_enableReflections = reflectionOn;

                Material waterMaterial = m_waterProfile.m_masterWaterMaterial;
                if (reflectionOn)
                {
                    if (waterObject != null)
                    {
                        PWS_WaterReflections reflection = waterObject.GetComponent<PWS_WaterReflections>();
                        if (reflection == null)
                        {
                            reflection = waterObject.AddComponent<PWS_WaterReflections>();
                            reflection.m_skyboxOnly = false;
                            SetupWaterReflectionSettings(m_waterProfile, false);
                        }
                        else
                        {
                            reflection.m_skyboxOnly = false;
                            SetupWaterReflectionSettings(m_waterProfile, false);
                        }
                    }
                }
                else
                {
                    if (waterObject != null)
                    {
                        PWS_WaterReflections reflection = waterObject.GetComponent<PWS_WaterReflections>();
                        if (reflection == null)
                        {
                            reflection = waterObject.AddComponent<PWS_WaterReflections>();
                            reflection.m_skyboxOnly = true;
                            SetupWaterReflectionSettings(m_waterProfile, false);
                        }
                        else
                        {
                            reflection.m_skyboxOnly = true;
                            SetupWaterReflectionSettings(m_waterProfile, false);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("[GaiaProWater.SetWaterReflections()] Shader of the material does not = " + m_waterShader + " Or master water material in the water profile is empty");
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
                theParentGo = GameObject.Find("Gaia Water Environment");

                if (theParentGo == null)
                {
                    theParentGo = new GameObject("Gaia Water Environment");
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

        public static bool DoesWaterExist()
        {
            return GameObject.Find("Ambient Water Sample") != null;
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
        /// Mark the water material as dirty to be saved
        /// </summary>
        /// <param name="waterMaterial"></param>
        private static void MarkWaterMaterialDirty(Material waterMaterial)
        {
            if (waterMaterial != null)
            {
                EditorUtility.SetDirty(waterMaterial);
            }
        }

        #endregion

        #region Utils Pro

        /// <summary>
        /// Checks to see if the shader is good to begin applying settings
        /// </summary>
        /// <param name="waterMaterial"></param>
        /// <returns></returns>
        private static bool CheckWaterMaterialAndShader(Material waterMaterial)
        {
            bool shaderGood = false;
            if (waterMaterial == null)
            {
                shaderGood = false;
                return shaderGood;
            }
            if (waterMaterial.shader == Shader.Find(m_waterShader))
            {
                shaderGood = true;
            }

            return shaderGood;
        }

        /// <summary>
        /// Spawns the water prefab
        /// </summary>
        /// <param name="waterPrefab"></param>
        private static void SpawnWater(GameObject waterPrefab)
        {
            float seaLevel = 0f;
            GaiaSceneInfo sceneInfo = GaiaSceneInfo.GetSceneInfo();
            if (sceneInfo != null)
            {
                seaLevel = sceneInfo.m_seaLevel;
            }

            if (m_parentObject == null)
            {
                m_parentObject = GetOrCreateParentObject("Gaia Water Environment", true);
            }

            if (waterPrefab == null)
            {
                Debug.LogError("[GaiaProWater.SpawnWater()] Water prefab is empty please make sure a prefab is present that you want to spawn");
            }
            else
            {
                RemoveOldWater();

                float waterLocationXZ = 0f;

                GameObject waterObject = GameObject.Find(waterPrefab.name);
                if (waterObject == null)
                {
                    waterObject = PrefabUtility.InstantiatePrefab(waterPrefab) as GameObject;
                    waterObject.transform.SetParent(m_parentObject.transform);
                    waterObject.transform.position = new Vector3(waterLocationXZ, seaLevel, waterLocationXZ);
                }
                else
                {
                    waterObject.transform.SetParent(m_parentObject.transform);
                    waterObject.transform.position = new Vector3(waterLocationXZ, seaLevel, waterLocationXZ);
                }
            }

            foreach (Stamper stamper in Resources.FindObjectsOfTypeAll<Stamper>())
            {
                stamper.m_showSeaLevelPlane = false;
            }

            foreach (Spawner spawner in Resources.FindObjectsOfTypeAll<Spawner>())
            {
                spawner.m_showSeaLevelPlane = false;
            }
        }

        /// <summary>
        /// Removes old water prefab from the scene
        /// </summary>
        private static void RemoveOldWater()
        {
            GameObject oldWater = GameObject.Find("Ambient Water Sample");
            if (oldWater != null)
            {
                Object.DestroyImmediate(oldWater);
            }

            GameObject oldUnderwaterFX = GameObject.Find("Ambient Water Samples");
            if (oldUnderwaterFX != null)
            {
                Object.DestroyImmediate(oldUnderwaterFX);
            }
        }

        /// <summary>
        /// Removes systems from scene
        /// </summary>
        public static void RemoveSystems()
        {
            GameObject sampleContent = GameObject.Find("Ambient Water Samples Environment");
            if (sampleContent != null)
            {
                Object.DestroyImmediate(sampleContent);
            }
        }

        /// <summary>
        /// Configures the water reflection settings
        /// </summary>
        /// <param name="profile"></param>
        public static void SetupWaterReflectionSettings(GaiaWaterProfile profile, bool forceUpdate)
        {
            PWS_WaterReflections[] reflections = Object.FindObjectsOfType<PWS_WaterReflections>();
            if (reflections != null)
            {
                foreach(PWS_WaterReflections reflection in reflections)
                {
                    reflection.m_disablePixelLights = profile.m_disablePixelLights;
                    reflection.m_clipPlaneOffset = profile.m_clipPlaneOffset;
                    reflection.m_reflectionLayers = profile.m_reflectedLayers;
                    reflection.m_HDR = profile.m_useHDR;
                    reflection.m_MSAA = profile.m_allowMSAA;
                    reflection.m_RenderUpdate = profile.m_waterRenderUpdateMode;
                    reflection.m_updateThreshold = profile.m_interval;

                    switch (profile.m_reflectionResolution)
                    {
                        case GaiaConstants.GaiaProWaterReflectionsQuality.Resolution8:
                            reflection.m_renderTextureSize = 8;
                            break;
                        case GaiaConstants.GaiaProWaterReflectionsQuality.Resolution16:
                            reflection.m_renderTextureSize = 16;
                            break;
                        case GaiaConstants.GaiaProWaterReflectionsQuality.Resolution32:
                            reflection.m_renderTextureSize = 32;
                            break;
                        case GaiaConstants.GaiaProWaterReflectionsQuality.Resolution64:
                            reflection.m_renderTextureSize = 64;
                            break;
                        case GaiaConstants.GaiaProWaterReflectionsQuality.Resolution128:
                            reflection.m_renderTextureSize = 128;
                            break;
                        case GaiaConstants.GaiaProWaterReflectionsQuality.Resolution256:
                            reflection.m_renderTextureSize = 256;
                            break;
                        case GaiaConstants.GaiaProWaterReflectionsQuality.Resolution512:
                            reflection.m_renderTextureSize = 512;
                            break;
                        case GaiaConstants.GaiaProWaterReflectionsQuality.Resolution1024:
                            reflection.m_renderTextureSize = 1024;
                            break;
                        case GaiaConstants.GaiaProWaterReflectionsQuality.Resolution2048:
                            reflection.m_renderTextureSize = 2048;
                            break;
                        case GaiaConstants.GaiaProWaterReflectionsQuality.Resolution4096:
                            reflection.m_renderTextureSize = 4096;
                            break;
                        case GaiaConstants.GaiaProWaterReflectionsQuality.Resolution8192:
                            reflection.m_renderTextureSize = 8192;
                            break;
                    }

                    if (forceUpdate)
                    {
                        reflection.Generate();
                    }
                }
            }
        }

        /// <summary>
        /// Sets up underwater features
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="underwaterFog"></param>
        /// <param name="underwaterPostFX"></param>
        /// <param name="underwaterParticles"></param>
        private static void SetupUnderWaterFX(GaiaWaterProfileValues profile, bool underwaterFog, bool underwaterPostFX, bool underwaterParticles)
        {
            if (m_parentObject == null)
            {
                m_parentObject = GetOrCreateParentObject("Gaia Water Environment", true);
            }

            float seaLevel = 0f;
            GaiaSceneInfo sceneInfo = GaiaSceneInfo.GetSceneInfo();
            if (sceneInfo != null)
            {
                seaLevel -= sceneInfo.m_seaLevel + 4.9f;
            }

            #if UNITY_POST_PROCESSING_STACK_V2
            PostProcessProfile postProcess = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(GetAssetPath(profile.m_postProcessingProfile));
            if (underwaterPostFX)
            {
                if (postProcess == null)
                {
                    Debug.LogError("[GaiaProWater.SetupUnderWaterFX()] Post Processing profile could not be found. Please make sure the string defined is correct");
                }
                else
                {
                    GameObject postProcessVolumeObject = GameObject.Find("Underwater Post Processing FX");
                    if (postProcessVolumeObject == null)
                    {
                        postProcessVolumeObject = new GameObject("Underwater Post Processing FX");
                        postProcessVolumeObject.transform.SetParent(m_parentObject.transform);
                        postProcessVolumeObject.transform.position = new Vector3(0f, seaLevel, 0f);
                    }
                    else
                    {
                        postProcessVolumeObject.transform.SetParent(m_parentObject.transform);
                        postProcessVolumeObject.transform.position = new Vector3(0f, seaLevel, 0f);
                    }

                    BoxCollider collider = postProcessVolumeObject.GetComponent<BoxCollider>();
                    if (collider == null)
                    {
                        collider = postProcessVolumeObject.AddComponent<BoxCollider>();
                        collider.size = new Vector3(10000f, 5000f, 10000f);
                        collider.isTrigger = true;
                    }
                    else
                    {
                        collider.size = new Vector3(10000f, 5000f, 10000f);
                        collider.isTrigger = true;
                    }

                    PostProcessVolume processVolume = postProcessVolumeObject.GetComponent<PostProcessVolume>();
                    if (processVolume == null)
                    {
                        processVolume = postProcessVolumeObject.AddComponent<PostProcessVolume>();
                        processVolume.sharedProfile = postProcess;
                        processVolume.isGlobal = false;
                        processVolume.blendDistance = 5f;
                        processVolume.priority = 2f;
                    }
                    else
                    {
                        processVolume.sharedProfile = postProcess;
                        processVolume.isGlobal = false;
                        processVolume.blendDistance = 5f;
                        processVolume.priority = 2f;
                    }
                }
            }
            else
            {
                GameObject postProcessVolumeObject = GameObject.Find("Underwater Post Processing FX");
                if (postProcessVolumeObject != null)
                {
                    Object.DestroyImmediate(postProcessVolumeObject);
                }
            }
            #endif
        }

        #endregion
    }
}