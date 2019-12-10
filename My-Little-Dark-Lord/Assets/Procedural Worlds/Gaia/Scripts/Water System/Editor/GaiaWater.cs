using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ProcedualWorlds.WaterSystem.MeshGeneration;
using ProcedualWorlds.WaterSystem.Reflections;
using System.IO;
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
        private static string m_unityVersion;
        public static string m_waterShader = "PWS/SP/Water/Ocean vP2.1 2019_01_14";
        private static List<string> m_profileList = new List<string>();
        private static List<Material> m_allMaterials = new List<Material>();

        private const string m_materialLocation = "/Procedural Worlds/Gaia/Gaia Lighting and Water/Gaia Water/Ocean Water/Resources/Material";
        private const string m_builtInKeyWord = "SP";
        private const string m_lightweightKeyWord = "LW";
        private const string m_highDefinitionKeyWord = "HD";

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
        /*
        public static void GetProfile(GaiaConstants.GaiaWaterProfileType waterType, GaiaWaterProfile waterProfile, GaiaConstants.EnvironmentRenderer renderPipeline)
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
                            UpdateGlobalWater(profile, renderPipeline, waterProfile);
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
                            UpdateGlobalWater(profile, renderPipeline, waterProfile);
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
        */

        public static void GetProfile(GaiaConstants.GaiaWaterProfileType waterType, GaiaWaterProfile waterProfile, GaiaConstants.EnvironmentRenderer renderPipeline, bool spawnWater)
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
                UpdateGlobalWater(waterProfile.m_waterProfiles[1], renderPipeline, waterProfile, spawnWater);
            }
        }

        #endregion

        #region Apply Settings

        /// <summary>
        /// Updates the global lighting settings in your scene
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="renderPipeline"></param>
        private static void UpdateGlobalWater(GaiaWaterProfileValues profile, GaiaConstants.EnvironmentRenderer renderPipeline, GaiaWaterProfile waterProfile, bool spawnWater)
        {
            //Spawns the water prefab in the scene
            if (spawnWater)
            {
                SpawnWater(m_waterProfile.m_waterPrefab);
            }

            //Sets global settings
            SetGlobalSettings(profile, m_waterProfile.m_activeWaterMaterial, renderPipeline);

            UpdateMaterial(m_waterProfile.m_waterPrefab, m_waterProfile.m_activeWaterMaterial);

            SetWaterReflectionsType(m_waterProfile.m_enableReflections, renderPipeline, waterProfile);

            /*
            //Sets water tint colors
            SetWaterColorTint(profile, m_waterProfile.m_masterWaterMaterial, renderPipeline);
            //Sets water opacity
            SetWaterOpacity(profile, m_waterProfile.m_masterWaterMaterial, renderPipeline);
            //Sets water main settings
            SetWaterMainSettings(profile, m_waterProfile.m_masterWaterMaterial, renderPipeline);
            //Sets water refractions
            SetWaterRefraction(profile, m_waterProfile.m_masterWaterMaterial, renderPipeline);
            //Sets water lighting
            SetWaterLighting(profile, m_waterProfile.m_masterWaterMaterial, renderPipeline);
            //Sets water shadows
            SetWaterShadow(profile, m_waterProfile.m_masterWaterMaterial, renderPipeline);
            //Sets water reflections
            SetWaterReflections(profile, m_waterProfile.m_masterWaterMaterial, renderPipeline);
            //Sets the water reflection component
            SetWaterReflectionsType(m_waterProfile.m_enableReflections, renderPipeline, waterProfile);
            //Sets water normals
            SetWaterNormals(profile, m_waterProfile.m_masterWaterMaterial, renderPipeline);
            //Sets water tessellation
            SetWaterTessellation(profile, m_waterProfile.m_masterWaterMaterial, renderPipeline);
            //Sets water waves and wind
            SetWaterWavesAndWind(profile, m_waterProfile.m_masterWaterMaterial, renderPipeline);
            //Sets water ocean foam
            SetWaterOceanFoam(profile, m_waterProfile.m_masterWaterMaterial, renderPipeline);
            //Sets water beach foam
            SetWaterBeachFoam(profile, m_waterProfile.m_masterWaterMaterial, renderPipeline);
            */
            //Sets the waters reflection settings
            SetupWaterReflectionSettings(m_waterProfile, false);
            //Sets the underwater effects in the scene
            SetUnderwaterEffects(m_waterProfile, profile);
            //Sets the auto wind setup on the water in the scene
            //SetAutoWind(m_waterProfile);
            //Water mesh generation
            UpdateWaterMeshQuality(m_waterProfile, m_waterProfile.m_waterPrefab);
            //Mark water as dirty
            MarkWaterMaterialDirty(m_waterProfile.m_activeWaterMaterial);

            //Check Water shader is good
            //if (CheckWaterMaterialAndShader(m_waterProfile.m_masterWaterMaterial))
            //{
            //
            //}
            //else
            //{
            //    Debug.LogError("[GaiaProWater.UpdateGlobalWater()] Shader of the material does not = " + m_waterShader + " Or master water material in the water profile is empty");
            //}

            //Destroys the parent object if it contains no partent childs
            DestroyParent("Ambient Skies Samples Environment");
        }

        /// <summary>
        /// Upgrades the water material on the prefab
        /// </summary>
        /// <param name="waterPrefab"></param>
        /// <param name="newMaterial"></param>
        private static void UpdateMaterial(GameObject waterPrefab, Material newMaterial)
        {
            GameObject waterObject = GameObject.Find(waterPrefab.name);
            if (waterObject != null)
            {
                MeshRenderer objectRenderer = waterObject.GetComponent<MeshRenderer>();
                if (objectRenderer != null)
                {
                    if (newMaterial != null)
                    {
                        objectRenderer.sharedMaterial = newMaterial;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the global settings of the water
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetGlobalSettings(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            waterMaterial.enableInstancing = m_waterProfile.m_enableGPUInstancing;
            waterMaterial.SetFloat("_EnableReflection", 1f);

            /*
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                if (m_waterProfile.m_enableOceanFoam)
                {
                    waterMaterial.SetFloat("_EnableOceanFoam1", 1f);
                }
                else
                {
                    waterMaterial.SetFloat("_EnableOceanFoam1", 0f);
                }

                if (m_waterProfile.m_enableBeachFoam)
                {
                    waterMaterial.SetFloat("_EnableBeachFoam1", 1f);
                }
                else
                {
                    waterMaterial.SetFloat("_EnableBeachFoam1", 0f);
                }
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
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
                    waterMaterial.SetFloat("_EnableBeachFoam1", 1f);
                }
                else
                {
                    waterMaterial.SetFloat("_EnableBeachFoam1", 0f);
                }
            }
            else
            {
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
                    waterMaterial.SetFloat("_EnableBeachFoam1", 1f);
                }
                else
                {
                    waterMaterial.SetFloat("_EnableBeachFoam1", 0f);
                }
            }
            */
        }

        /// <summary>
        /// Sets the water tint coloring
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterColorTint(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                waterMaterial.SetColor("_WaterTint", profile.m_waterTint);
                waterMaterial.SetColor("_ShallowTint", profile.m_shallowTint);
                waterMaterial.SetFloat("_ShallowOffset", profile.m_shallowOffset);
                waterMaterial.SetColor("_DepthTint", profile.m_depthTint);
                waterMaterial.SetFloat("_DepthOffset", profile.m_depthOffset);
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                waterMaterial.SetColor("_WaterTint2", profile.m_LWwaterTint);
                waterMaterial.SetColor("_ShallowTint1", profile.m_LWshallowTint);
                waterMaterial.SetFloat("_ShallowOffset1", profile.m_LWshallowOffset);
                waterMaterial.SetColor("_DepthTint2", profile.m_LWdepthTint);
                waterMaterial.SetFloat("_DepthOffset2", profile.m_LWdepthOffset);
            }
            else
            {
                waterMaterial.SetColor("_WaterTint", profile.m_HDwaterTint);
                waterMaterial.SetColor("_ShallowTint", profile.m_HDshallowTint);
                waterMaterial.SetFloat("_ShallowOffset", profile.m_HDshallowOffset);
                waterMaterial.SetColor("_DepthTint", profile.m_HDdepthTint);
                waterMaterial.SetFloat("_DepthOffset", profile.m_HDdepthOffset);
            }
        }

        /// <summary>
        /// Sets the waters opacity settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterOpacity(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
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
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                waterMaterial.SetFloat("_OpacityOcean", profile.m_LWopacityOcean);
                waterMaterial.SetFloat("_OpacityBeach2", profile.m_LWopacityBeach);
                if (profile.m_LWignoreVertexColor)
                {
                    waterMaterial.SetFloat("_IgnoreVertexColor", 1f);
                }
                else
                {
                    waterMaterial.SetFloat("_IgnoreVertexColor", 0f);
                }
            }
            else
            {
                waterMaterial.SetFloat("_OpacityOcean", profile.m_HDopacityOcean);
                waterMaterial.SetFloat("_OpacityBeach", profile.m_HDopacityBeach);
                if (profile.m_HDignoreVertexColor)
                {
                    waterMaterial.SetFloat("_IgnoreVertexColor", 1f);
                }
                else
                {
                    waterMaterial.SetFloat("_IgnoreVertexColor", 0f);
                }
            }
        }

        /// <summary>
        /// Sets the waters main settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterMainSettings(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                waterMaterial.SetFloat("_Occlusion1", profile.m_occlusion);
                waterMaterial.SetFloat("_Metallic", profile.m_metallic);
                waterMaterial.SetFloat("_Smoothness1", profile.m_smoothness);
                waterMaterial.SetFloat("_SmoothnessVariance1", profile.m_smoothnessVariance);
                waterMaterial.SetFloat("_SmoothnessThreshold1", profile.m_smoothnessThreshold);
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                waterMaterial.SetFloat("_Occlusion1", profile.m_LWocclusion);
                waterMaterial.SetFloat("_Metallic", profile.m_LWmetallic);
                waterMaterial.SetFloat("_Smoothness1", profile.m_LWsmoothness);
                waterMaterial.SetFloat("_SmoothnessVariance1", profile.m_LWsmoothnessVariance);
                waterMaterial.SetFloat("_SmoothnessThreshold1", profile.m_LWsmoothnessThreshold);
            }
            else
            {
                waterMaterial.SetFloat("_Occlusion1", profile.m_HDocclusion);
                waterMaterial.SetFloat("_Metallic", profile.m_HDmetallic);
                waterMaterial.SetFloat("_Smoothness1", profile.m_HDsmoothness);
                waterMaterial.SetFloat("_SmoothnessVariance1", profile.m_HDsmoothnessVariance);
                waterMaterial.SetFloat("_SmoothnessThreshold1", profile.m_HDsmoothnessThreshold);
            }
        }

        /// <summary>
        /// Sets the waters refraction settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterRefraction(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                waterMaterial.SetFloat("_RefractedDepth1", profile.m_refractedDepth);
                waterMaterial.SetFloat("_RefractionScale1", profile.m_refractionScale);
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                waterMaterial.SetFloat("_RefractedDepth1", profile.m_LWrefractedDepth);
                waterMaterial.SetFloat("_RefractionScale1", profile.m_LWrefractionScale);
            }
            else
            {
                waterMaterial.SetFloat("_RefractedDepth1", profile.m_HDrefractedDepth);
                waterMaterial.SetFloat("_RefractionScale1", profile.m_HDrefractionScale);
            }
        }

        /// <summary>
        /// Sets the waters lighting settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterLighting(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                waterMaterial.SetFloat("_LightIndirectStrengthSpecular1", profile.m_indirectLightSpecular);
                waterMaterial.SetFloat("_LightIndirectStrengthDiffuse1", profile.m_indirectLightDiffuse);
                waterMaterial.SetColor("_HighlightTint1", profile.m_highlightTint);
                waterMaterial.SetFloat("_HighlightOffset1", profile.m_highlightOffset);
                waterMaterial.SetFloat("_HighlightSharpness1", profile.m_highlightSharpness);
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                waterMaterial.SetFloat("_LightIndirectStrengthSpecular1", profile.m_LWindirectLightSpecular);
                waterMaterial.SetFloat("_LightIndirectStrengthDiffuse1", profile.m_LWindirectLightDiffuse);
                waterMaterial.SetColor("_HighlightTint1", profile.m_LWhighlightTint);
                waterMaterial.SetFloat("_HighlightOffset1", profile.m_LWhighlightOffset);
                waterMaterial.SetFloat("_HighlightSharpness1", profile.m_LWhighlightSharpness);
            }
            else
            {
                waterMaterial.SetFloat("_LightIndirectStrengthSpecular1", profile.m_HDindirectLightSpecular);
                waterMaterial.SetFloat("_LightIndirectStrengthDiffuse1", profile.m_HDindirectLightDiffuse);
                waterMaterial.SetColor("_HighlightTint1", profile.m_HDhighlightTint);
                waterMaterial.SetFloat("_HighlightOffset1", profile.m_HDhighlightOffset);
                waterMaterial.SetFloat("_HighlightSharpness1", profile.m_HDhighlightSharpness);
            }
        }

        /// <summary>
        /// Sets the waters shadow settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterShadow(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                waterMaterial.SetFloat("_ShadowStrength1", profile.m_shadowStrength);
                waterMaterial.SetFloat("_ShadowSharpness1", profile.m_shadowSharpness);
                waterMaterial.SetFloat("_ShadowOffset1", profile.m_shadowOffset);
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                waterMaterial.SetFloat("_ShadowStrength2", profile.m_LWshadowStrength);
                waterMaterial.SetFloat("_ShadowSharpness1", profile.m_LWshadowSharpness);
                waterMaterial.SetFloat("_ShadowOffset1", profile.m_LWshadowOffset);
            }
            else
            {
                waterMaterial.SetFloat("_ShadowStrength1", profile.m_HDshadowStrength);
                waterMaterial.SetFloat("_ShadowSharpness1", profile.m_HDshadowSharpness);
                waterMaterial.SetFloat("_ShadowOffset1", profile.m_HDshadowOffset);
            }
        }

        /// <summary>
        /// Sets the waters reflection settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterReflections(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                waterMaterial.SetFloat("_ReflectionIntensity", profile.m_reflectionIntenisty);
                waterMaterial.SetFloat("_ReflectionWobble", profile.m_reflectionWobble);
                waterMaterial.SetFloat("_ReflectionFresnelPower", profile.m_reflectionFresnelPower);
                waterMaterial.SetFloat("_ReflectionFrenelScale", profile.m_reflectionFresnelScale);
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                waterMaterial.SetFloat("_ReflectionIntensity", profile.m_LWreflectionIntenisty);
                waterMaterial.SetFloat("_ReflectionWobble", profile.m_LWreflectionWobble);
                waterMaterial.SetFloat("_ReflectionFresnelPower", profile.m_LWreflectionFresnelPower);
                waterMaterial.SetFloat("_ReflectionFrenelScale", profile.m_LWreflectionFresnelScale);
            }
            else
            {
                waterMaterial.SetFloat("_ReflectionIntensity", profile.m_HDreflectionIntenisty);
                waterMaterial.SetFloat("_ReflectionWobble", profile.m_HDreflectionWobble);
                waterMaterial.SetFloat("_ReflectionFresnelPower", profile.m_HDreflectionFresnelPower);
                waterMaterial.SetFloat("_ReflectionFrenelScale", profile.m_HDreflectionFresnelScale);
            }
        }

        /// <summary>
        /// Sets the waters normal settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterNormals(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                waterMaterial.SetTexture("_NormalMap", profile.m_normalMap);
                waterMaterial.SetFloat("_NormalMapStrength", profile.m_normalStrength);
                waterMaterial.SetFloat("_NormalMapTiling", profile.m_normalTiling);
                waterMaterial.SetFloat("_NormalMapSpeed", profile.m_normalSpeed);
                waterMaterial.SetFloat("_NormalMapTimescale", profile.m_normalTimescale);
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                waterMaterial.SetTexture("_NormalMap", profile.m_LWnormalMap);
                waterMaterial.SetFloat("_NormalMapStrength", profile.m_LWnormalStrength);
                waterMaterial.SetFloat("_NormalMapTiling", profile.m_LWnormalTiling);
                waterMaterial.SetFloat("_NormalMapSpeed", profile.m_LWnormalSpeed);
                waterMaterial.SetFloat("_NormalMapTimescale", profile.m_LWnormalTimescale);
            }
            else
            {
                waterMaterial.SetTexture("_NormalMap", profile.m_HDnormalMap);
                waterMaterial.SetFloat("_NormalMapStrength", profile.m_HDnormalStrength);
                waterMaterial.SetFloat("_NormalMapTiling", profile.m_HDnormalTiling);
                waterMaterial.SetFloat("_NormalMapSpeed", profile.m_HDnormalSpeed);
                waterMaterial.SetFloat("_NormalMapTimescale", profile.m_HDnormalTimescale);
            }
        }

        /// <summary>
        /// Sets the waters tessellation settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterTessellation(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                waterMaterial.SetFloat("_TessellationStrength", profile.m_tessellationAmount);
                waterMaterial.SetFloat("_TessellationMaxDistance", profile.m_tessellationDistance);
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                waterMaterial.SetFloat("_WaveTileUV1", profile.m_LWtessellationDistance);
                waterMaterial.SetFloat("_WaveHeight1", profile.m_LWtessellationAmount);
            }
            else
            {
                waterMaterial.SetFloat("_TessellationStrength", profile.m_HDtessellationDistance);
                waterMaterial.SetFloat("_TessellationMaxDistance", profile.m_HDtessellationAmount);
            }
        }

        /// <summary>
        /// Sets the waters wave and wind settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterWavesAndWind(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                waterMaterial.SetVector("_Wave01", profile.m_wave01);
                waterMaterial.SetVector("_Wave02", profile.m_wave02);
                waterMaterial.SetVector("_Wave03", profile.m_wave03);
                waterMaterial.SetFloat("_WaveSpeed", profile.m_waveSpeed);
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                waterMaterial.SetVector("_WaveNoiseShape1", profile.m_LWwave01);
                waterMaterial.SetVector("_WaveUp1", profile.m_LWwave02);
                waterMaterial.SetVector("_WaveDirection1", profile.m_LWwave03);
                waterMaterial.SetFloat("_WaveSpeed", profile.m_LWwaveSpeed);
            }
            else
            {
                waterMaterial.SetVector("_WaveNoiseShape1", profile.m_HDwave01);
                waterMaterial.SetVector("_WaveUp1", profile.m_HDwave02);
                waterMaterial.SetVector("_WaveDirection1", profile.m_HDwave03);
                waterMaterial.SetFloat("_WaveSpeed", profile.m_HDwaveSpeed);
            }
        }

        /// <summary>
        /// Sets the waters ocean foam settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterOceanFoam(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                waterMaterial.SetTexture("_OceanFoamMap", profile.m_oceanFoamTexture);
                waterMaterial.SetColor("_OceanFoamTint", profile.m_oceanFoamTint);
                waterMaterial.SetFloat("_OceanFoamTiling", profile.m_oceanFoamTiling);
                waterMaterial.SetFloat("_OceanFoamStrength", profile.m_oceanFoamAmount);
                waterMaterial.SetFloat("_OceanFoamDistance", profile.m_oceanFoamDistance);
                waterMaterial.SetFloat("_OceanFoamSpeed", profile.m_oceanFoamSpeed);
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                waterMaterial.SetTexture("_OceanFoamMap", profile.m_LWoceanFoamTexture);
                waterMaterial.SetColor("_OceanFoamTint", profile.m_LWoceanFoamTint);
                waterMaterial.SetFloat("_OceanFoamTiling", profile.m_LWoceanFoamTiling);
                waterMaterial.SetFloat("_OceanFoamStrength", profile.m_LWoceanFoamAmount);
                waterMaterial.SetFloat("_OceanFoamDistance", profile.m_LWoceanFoamDistance);
                waterMaterial.SetFloat("_OceanFoamSpeed", profile.m_LWoceanFoamSpeed);
            }
            else
            {
                waterMaterial.SetTexture("_OceanFoamMap", profile.m_HDoceanFoamTexture);
                waterMaterial.SetColor("_OceanFoamTint", profile.m_HDoceanFoamTint);
                waterMaterial.SetFloat("_OceanFoamTiling", profile.m_HDoceanFoamTiling);
                waterMaterial.SetFloat("_OceanFoamStrength", profile.m_HDoceanFoamAmount);
                waterMaterial.SetFloat("_OceanFoamDistance", profile.m_HDoceanFoamDistance);
                waterMaterial.SetFloat("_OceanFoamSpeed", profile.m_HDoceanFoamSpeed);
            }
        }

        /// <summary>
        /// Sets the waters beach foam settings
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="waterMaterial"></param>
        private static void SetWaterBeachFoam(GaiaWaterProfileValues profile, Material waterMaterial, GaiaConstants.EnvironmentRenderer renderPipeline)
        {
            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                waterMaterial.SetTexture("_BeachFoamMap", profile.m_beachFoamTexture);
                waterMaterial.SetColor("_BeachFoamTint", profile.m_beachFoamTint);
                waterMaterial.SetFloat("_BeachFoamTiling", profile.m_beachFoamTiling);
                waterMaterial.SetFloat("_BeachFoamStrength", profile.m_beachFoamAmount);
                waterMaterial.SetFloat("foamMax1", profile.m_beachFoamDistance);
                waterMaterial.SetFloat("_BeachFoamSpeed", profile.m_beachFoamSpeed);
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                waterMaterial.SetTexture("_BeachFoamMap", profile.m_LWbeachFoamTexture);
                waterMaterial.SetColor("_BeachFoamTint", profile.m_LWbeachFoamTint);
                waterMaterial.SetFloat("_BeachFoamTiling", profile.m_LWbeachFoamTiling);
                waterMaterial.SetFloat("_BeachFoamStrength", profile.m_LWbeachFoamAmount);
                waterMaterial.SetFloat("foamMax1", profile.m_LWbeachFoamDistance);
                waterMaterial.SetFloat("_BeachFoamSpeed", profile.m_LWbeachFoamSpeed);
            }
            else
            {
                waterMaterial.SetTexture("_BeachFoamMap", profile.m_HDbeachFoamTexture);
                waterMaterial.SetColor("_BeachFoamTint", profile.m_HDbeachFoamTint);
                waterMaterial.SetFloat("_BeachFoamTiling", profile.m_HDbeachFoamTiling);
                waterMaterial.SetFloat("_BeachFoamStrength", profile.m_HDbeachFoamAmount);
                waterMaterial.SetFloat("foamMax1", profile.m_HDbeachFoamDistance);
                waterMaterial.SetFloat("_BeachFoamSpeed", profile.m_HDbeachFoamSpeed);
            }
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
                    FollowPlayerSystem followPlayer = null;
                    if (profile.m_supportUnderwaterParticles)
                    {
                        if (underwaterParticles == null)
                        {
                            underwaterParticles = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(GaiaUtils.GetAssetPath("Underwater Particles Effects.prefab"))) as GameObject;
                            followPlayer = underwaterParticles.GetComponent<FollowPlayerSystem>();
                            if (followPlayer == null)
                            {
                                followPlayer = underwaterParticles.AddComponent<FollowPlayerSystem>();
                            }
                            followPlayer.m_player = underwaterEffects.m_playerCamera;
                            followPlayer.m_particleObjects.Add(underwaterParticles);
                            underwaterEffects.m_underwaterParticles = underwaterParticles;
                            underwaterParticles.transform.position = underwaterEffects.m_playerCamera.transform.position;
                        }
                        else
                        {
                            underwaterParticles.transform.position = underwaterEffects.m_playerCamera.transform.position;
                        }

                        followPlayer = underwaterParticles.GetComponent<FollowPlayerSystem>();
                        if (followPlayer != null)
                        {
                            followPlayer.m_player = underwaterEffects.m_playerCamera;
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
                    underwaterEffects.m_framesPerSecond = profile.m_causticFramePerSecond;
                    underwaterEffects.m_causticSize = profile.m_causticSize;
                    underwaterEffects.m_useCaustics = profile.m_useCastics;
                    underwaterEffects.m_mainLight = profile.m_mainCausticLight;
                    underwaterEffects.m_seaLevel = seaLevel;
                    //underwaterEffects.m_fogColor = profileValues.m_underwaterFogColor;
                    //underwaterEffects.m_fogDistance = profileValues.m_underwaterFogDistance;
                    //underwaterEffects.m_nearFogDistance = profileValues.m_underwaterNearFogDistance;
                    if (underwaterEffects.m_waterReflections == null)
                    {
                        underwaterEffects.m_waterReflections = Object.FindObjectOfType<PWS_WaterReflections>();
                    }

                    FollowPlayerSystem followPlayer = null;
                    GameObject underwaterParticles = GameObject.Find("Underwater Particles Effects");
                    if (profile.m_supportUnderwaterParticles)
                    {
                        if (underwaterParticles == null)
                        {
                            underwaterParticles = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(GaiaUtils.GetAssetPath("Underwater Particles Effects.prefab"))) as GameObject;
                            followPlayer = underwaterParticles.GetComponent<FollowPlayerSystem>();
                            if (followPlayer == null)
                            {
                                followPlayer = underwaterParticles.AddComponent<FollowPlayerSystem>();
                            }
                            followPlayer.m_player = underwaterEffects.m_playerCamera;
                            underwaterParticles.transform.position = underwaterEffects.m_playerCamera.transform.position;
                        }
                        else
                        {
                            followPlayer = underwaterParticles.GetComponent<FollowPlayerSystem>();
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
                        if (underwaterHorizon != null)
                        {
                            FollowPlayerSystem followPlayer = underwaterHorizon.GetComponent<FollowPlayerSystem>();
                            if (followPlayer)
                            {
                                followPlayer = underwaterHorizon.AddComponent<FollowPlayerSystem>();
                            }

                            followPlayer.m_followPlayer = true;
                            followPlayer.m_particleObjects.Add(underwaterHorizon);
                            followPlayer.m_player = underwaterEffects.m_playerCamera;
                            followPlayer.m_useOffset = true;
                            followPlayer.m_xoffset = 1000f;
                            followPlayer.m_zoffset = 250f;
                            followPlayer.m_yOffset = 250f;
                            underwaterHorizon.transform.SetParent(underwaterEffectsObject.transform);
                            underwaterHorizon.transform.position = new Vector3(0f, -4000f, 0f);
                        }
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
                            followPlayer.m_useOffset = true;
                            followPlayer.m_xoffset = 1000f;
                            followPlayer.m_zoffset = 250f;
                            followPlayer.m_yOffset = 250f;
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
                /*
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
                */
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
        public static void SetWaterReflectionsType(bool reflectionOn, GaiaConstants.EnvironmentRenderer renderPipeline, GaiaWaterProfile profile)
        {
            if (m_waterProfile == null)
            {
                m_waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));
            }

            GameObject waterObject = GameObject.Find("Ambient Water Sample");

            if (renderPipeline != GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                if (waterObject != null)
                {
                    PWS_WaterReflections reflection = waterObject.GetComponent<PWS_WaterReflections>();
                    if (reflection != null)
                    {
                        Object.DestroyImmediate(reflection);
                    }
                }

                if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
                {
                    LWRPWaterProbe(true);
                }
                else
                {
                    HDRPPlanarReflections(profile);
                }

                return;
            }

            if (CheckWaterMaterialAndShader(m_waterProfile.m_activeWaterMaterial))
            {
                m_waterProfile.m_enableReflections = reflectionOn;

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
        /// Reflections for LWRP
        /// </summary>
        private static void LWRPWaterProbe(bool enabled)
        {
            GameObject reflectionProbe = GameObject.Find("LWRP Water Reflection Probe");
            if (enabled)
            {
                if (m_parentObject == null)
                {
                    m_parentObject = GetOrCreateParentObject("Gaia Water Environment", true);
                }

                GaiaSceneInfo sceneInfo = GaiaSceneInfo.GetSceneInfo();
                if (reflectionProbe == null)
                {
                    reflectionProbe = new GameObject("LWRP Water Reflection Probe");
                }

                reflectionProbe.transform.SetParent(m_parentObject.transform);

                if (sceneInfo != null)
                {
                    reflectionProbe.transform.position = new Vector3(0f, sceneInfo.m_seaLevel + 0.5f, 0f);
                }
                else
                {
                    reflectionProbe.transform.position = new Vector3(0f, 0.5f, 0f);
                }

                ReflectionProbe probe = reflectionProbe.GetComponent<ReflectionProbe>();
                if (probe == null)
                {
                    probe = reflectionProbe.AddComponent<ReflectionProbe>();
                }

                probe.cullingMask = 0;
                probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
                probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
                probe.resolution = 512;
            }
            else
            {
                if (reflectionProbe != null)
                {
                    Object.DestroyImmediate(reflectionProbe);
                }
            }
        }

        /// <summary>
        /// Reflections for HDRP
        /// </summary>
        private static void HDRPPlanarReflections(GaiaWaterProfile profile)
        {
#if HDPipeline
            GameObject planarObject = GameObject.Find("HD Planar Water Reflections");
            if (profile.m_enableReflections)
            {
                if (m_parentObject == null)
                {
                    m_parentObject = GetOrCreateParentObject("Gaia Water Environment", true);
                }

                GaiaSceneInfo sceneInfo = GaiaSceneInfo.GetSceneInfo();
                if (planarObject == null)
                {
                    GameObject planarPrefab = profile.m_hdPlanarReflections;
                    if (planarPrefab != null)
                    {
                        planarObject = PrefabUtility.InstantiatePrefab(planarPrefab) as GameObject;
                    }
                    else
                    {
                        Debug.LogError("Missing 'HD Planar Water Reflections' Prefab in your project please make sure it exists");
                    }
                }

                Vector3 objectLocation = planarObject.transform.position;
                if (sceneInfo != null)
                {
                    objectLocation.y = sceneInfo.m_seaLevel + 1f;
                }

                planarObject.transform.position = objectLocation;
                planarObject.transform.SetParent(m_parentObject.transform);

                PlanarReflections reflections = planarObject.GetComponent<PlanarReflections>();
                if (reflections == null)
                {
                    reflections = planarObject.AddComponent<PlanarReflections>();
                }

                reflections.PlanarReflectionsConfiguration();
            }
            else
            {
                if (planarObject != null)
                {
                    Object.DestroyImmediate(planarObject);
                }
            }
#endif
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

            FocusWaterProfile focusWater = theParentGo.GetComponent<FocusWaterProfile>();
            if (focusWater == null)
            {
                focusWater = theParentGo.AddComponent<FocusWaterProfile>();
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

            shaderGood = true;

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
            GameObject sampleContent = GameObject.Find("Ambient Water Sample");
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
                    reflection.m_customReflectionDistance = profile.m_useCustomRenderDistance;
                    reflection.m_renderDistance = profile.m_customRenderDistance;
                    reflection.m_singleDistanceLayer = profile.m_useCustomRenderDistance;

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

        /// <summary>
        /// Setup the material name list
        /// </summary>
        public static bool SetupMaterials(GaiaConstants.EnvironmentRenderer renderPipeline, GaiaSettings gaiaSettings, int profileIndex)
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
        private static List<Material> GetMaterials(string path)
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
    }
}