using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ProcedualWorlds.WaterSystem.MeshGeneration;
using ProcedualWorlds.WaterSystem.Reflections;
using Gaia.Internal;
using PWCommon2;

namespace Gaia
{
    [CustomEditor(typeof(GaiaWaterProfile))]
    public class GaiaWaterProfileEditor : PWEditor
    {
        private EditorUtils m_editorUtils;
        private string m_version;
        private GaiaSettings m_gaiaSettings;
        private GaiaWaterProfile m_profile;
        private GaiaWaterProfileValues m_profileValues;
        private PWS_MeshGenerationPro m_waterSystemPro;
        private GaiaConstants.GaiaWaterProfileType m_waterProfile;
        private GaiaConstants.EnvironmentRenderer m_renderPipeline;

        private void OnEnable()
        {
            //Get Gaia Lighting Profile object
            m_profile = (GaiaWaterProfile)target;
            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }

            if (m_gaiaSettings == null)
            {
                m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            }

            m_renderPipeline = m_gaiaSettings.m_pipelineProfile.m_activePipelineInstalled;
            m_version = PWApp.CONF.Version;

            m_waterSystemPro = Object.FindObjectOfType<PWS_MeshGenerationPro>();
        }

        /// <summary>
        /// Setup on destroy
        /// </summary>
        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }
        }

        public override void OnInspectorGUI()
        {
            //Initialization
            m_editorUtils.Initialize(); // Do not remove this!

            if (m_gaiaSettings == null)
            {
                m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            }

            if (m_renderPipeline != m_gaiaSettings.m_pipelineProfile.m_activePipelineInstalled)
            {
                m_renderPipeline = m_gaiaSettings.m_pipelineProfile.m_activePipelineInstalled;
            }

            //Monitor for changes
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Profile Version: " + m_version);

            if (GaiaProUtils.IsGaiaPro())
            {
                //m_profile.m_editSettings = EditorGUILayout.Toggle("Enable Edit Settings Mode", m_profile.m_editSettings);

                m_editorUtils.Panel("UpdateSettings", RealtimeUpdateEnabled, false);
                m_editorUtils.Panel("GlobalSettings", GlobalSettingsEnabled, false);
                m_editorUtils.Panel("MeshQualitySettings", WaterMeshQualityEnabled, false);
                m_editorUtils.Panel("UnderwaterSettings", UnderwaterSettingsEnabled, false);
                m_editorUtils.Panel("WaterProfileSettings", WaterProfileSettingsEnabled, false);

                if (m_profile.m_editSettings)
                {
                    DrawDefaultInspector();
                }
            }
            else
            {
                if (GUILayout.Button("Get Gaia Pro To Edit Profile"))
                {
                    Application.OpenURL(m_gaiaSettings.m_gaiaProURL);
                }
            }

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_profile, "Made changes");
                EditorUtility.SetDirty(m_profile);

                m_profile.m_waterProfile = m_waterProfile;

                if (m_profile.m_updateInRealtime)
                {
                    if (m_waterProfile != GaiaConstants.GaiaWaterProfileType.None)
                    {
                        GaiaWater.GetProfile(m_waterProfile, m_profile);
                    }
                }
            }
        }

        private void RealtimeUpdateEnabled(bool helpEnabled)
        {
            m_profile.m_updateInRealtime = m_editorUtils.ToggleLeft("UpdateInRealtime", m_profile.m_updateInRealtime);
            if (m_profile.m_updateInRealtime)
            {
                EditorGUILayout.HelpBox("Update In Realtime is enabled this will allow profiles to be endited inside the editor and automatically apply changes every frame. This feature can be expensive and should not be left enabled in testing and builds", MessageType.Warning);
            }
            else
            {
                if (m_editorUtils.Button("UpdateToScene"))
                {
                    if (m_waterProfile != GaiaConstants.GaiaWaterProfileType.None)
                    {
                        GaiaWater.GetProfile(m_waterProfile, m_profile);
                    }
                }
            }
        }

        private void GlobalSettingsEnabled(bool helpEnabled)
        {
            m_profile.m_masterWaterMaterial = (Material)m_editorUtils.ObjectField("MasterWaterMaterial", m_profile.m_masterWaterMaterial, typeof(Material), false, GUILayout.Height(16f));
            if (m_profile.m_masterWaterMaterial == null)
            {
                EditorGUILayout.HelpBox("Missing a master water material, please add one.", MessageType.Error);
            }
            m_profile.m_waterPrefab = (GameObject)m_editorUtils.ObjectField("WaterPrefab", m_profile.m_waterPrefab, typeof(GameObject), false, GUILayout.Height(16f));
            if (m_profile.m_waterPrefab == null)
            {
                EditorGUILayout.HelpBox("Missing a water prefab, please add one.", MessageType.Error);
            }
            m_profile.m_enableGPUInstancing = m_editorUtils.ToggleLeft("EnableGPUInstancing", m_profile.m_enableGPUInstancing);
            if (!m_profile.m_enableGPUInstancing)
            {
                EditorGUILayout.HelpBox("Enabling GPU instancing will help improve performance. We recommend you enable this setting.", MessageType.Info);
            }
            m_profile.m_enableOceanFoam = m_editorUtils.ToggleLeft("EnableOceanFoam", m_profile.m_enableOceanFoam);
            m_profile.m_enableBeachFoam = m_editorUtils.ToggleLeft("EnableBeachFoam", m_profile.m_enableBeachFoam);
            m_profile.m_autoWindControlOnWater = m_editorUtils.ToggleLeft("EnableAutoWindControl", m_profile.m_autoWindControlOnWater);
            m_profile.m_enableReflections = m_editorUtils.ToggleLeft("EnableReflections", m_profile.m_enableReflections);
            if (m_profile.m_enableReflections)
            {
                EditorGUILayout.HelpBox("Reflections render the object reflections on the water surface. This can be very expensive feature and could cause performance issue on low end machines.", MessageType.Info);

                m_profile.m_allowMSAA = m_editorUtils.ToggleLeft("AllowMSAA", m_profile.m_allowMSAA);
                m_profile.m_useHDR = m_editorUtils.ToggleLeft("UseHDR", m_profile.m_useHDR);
                m_profile.m_disablePixelLights = m_editorUtils.ToggleLeft("DisablePixelLights", m_profile.m_disablePixelLights);
                m_profile.m_clipPlaneOffset = m_editorUtils.Slider("ClipPlaneOffset", m_profile.m_clipPlaneOffset, -5f, 100f);
                m_profile.m_waterRenderUpdateMode = (PWS_WaterReflections.RenderUpdateMode)EditorGUILayout.EnumPopup("Render Update Mode", m_profile.m_waterRenderUpdateMode);
                if (m_profile.m_waterRenderUpdateMode == PWS_WaterReflections.RenderUpdateMode.Interval)
                {
                    m_profile.m_interval = m_editorUtils.Slider("IntervalTime", m_profile.m_interval, 0f, 50f);
                }
                m_profile.m_reflectionResolution = (GaiaConstants.GaiaProWaterReflectionsQuality)EditorGUILayout.EnumPopup("Reflection Resolution", m_profile.m_reflectionResolution);
                m_profile.m_reflectedLayers = LayerMaskField("ReflectedLayers", m_editorUtils, m_profile.m_reflectedLayers);

                if (!m_profile.m_updateInRealtime)
                {
                    if (m_editorUtils.Button("UpdateReflectionSettings"))
                    {
                        GaiaWater.SetupWaterReflectionSettings(m_profile, true);
                    }
                }
            }
        }

        private void WaterMeshQualityEnabled(bool helpEnabled)
        {
            //m_profile.m_enableWaterMeshQuality = EditorGUILayout.ToggleLeft("Enable Water Mesh Quality", m_profile.m_enableWaterMeshQuality);
            m_profile.m_enableWaterMeshQuality = true;
            if (m_profile.m_enableWaterMeshQuality)
            {
                m_profile.m_waterMeshQuality = (GaiaConstants.WaterMeshQuality)EditorGUILayout.EnumPopup("Water Mesh Qulaity", m_profile.m_waterMeshQuality);
                m_profile.m_meshType = (PWS_MeshGenerationPro.MeshType)EditorGUILayout.EnumPopup("Mesh Type", m_profile.m_meshType);
                if (m_profile.m_waterMeshQuality == GaiaConstants.WaterMeshQuality.Custom)
                {
                    if (m_profile.m_meshType == PWS_MeshGenerationPro.MeshType.Plane)
                    {
                        m_profile.m_customMeshQuality = m_editorUtils.IntSlider("CustomMeshQualiy", m_profile.m_customMeshQuality, 16, 256);
                    }
                    else
                    {
                        m_profile.m_customMeshQuality = m_editorUtils.IntSlider("CustomMeshQualiy", (int)m_profile.m_customMeshQuality * 2, 2, 256) / 2;
                    }

                }

                if (m_profile.m_meshType == PWS_MeshGenerationPro.MeshType.Plane)
                {
                    m_profile.m_xSize = m_editorUtils.IntField("WaterSize", m_profile.m_xSize);
                    m_profile.m_zSize = m_profile.m_xSize;
                }
                else
                {
                    m_profile.m_xSize = m_editorUtils.IntField("WaterSize", m_profile.m_xSize);
                    m_profile.m_zSize = m_profile.m_xSize;
                }

                EditorGUILayout.BeginHorizontal();
                m_editorUtils.Label("PolyCountGenerated");
                if (m_waterSystemPro == null)
                {
                    m_waterSystemPro = FindObjectOfType<PWS_MeshGenerationPro>();
                }
                else
                {
                    EditorGUILayout.LabelField(m_waterSystemPro.CalculateVerticesRequired().ToString());
                }
                EditorGUILayout.EndHorizontal();

                if (!m_profile.m_updateInRealtime)
                {
                    if (m_editorUtils.Button("UpdateWaterMeshQuality"))
                    {
                        GaiaWater.UpdateWaterMeshQuality(m_profile, m_profile.m_waterPrefab);
                    }
                }
            }
        }

        private void UnderwaterSettingsEnabled(bool helpEnabled)
        {
            m_profile.m_underwaterParticles = (GameObject)m_editorUtils.ObjectField("UnderwaterParticlesPrefab", m_profile.m_underwaterParticles, typeof(GameObject), false, GUILayout.Height(16f));
            if (m_profile.m_underwaterParticles == null)
            {
                EditorGUILayout.HelpBox("Missing underwater particles prefab, please add one.", MessageType.Error);
            }
            m_profile.m_underwaterHorizonPrefab = (GameObject)m_editorUtils.ObjectField("UnderwaterHorizonPrefab", m_profile.m_underwaterHorizonPrefab, typeof(GameObject), false, GUILayout.Height(16f));
            if (m_profile.m_underwaterHorizonPrefab == null)
            {
                EditorGUILayout.HelpBox("Missing a underwater horizon prefab, please add one.", MessageType.Error);
            }
            m_profile.m_supportUnderwaterEffects = m_editorUtils.ToggleLeft("SupportUnderwaterEffects", m_profile.m_supportUnderwaterEffects);
            if (m_profile.m_supportUnderwaterEffects)
            {
                m_profile.m_supportUnderwaterPostProcessing = m_editorUtils.ToggleLeft("SupportUnderwaterPostProcessing", m_profile.m_supportUnderwaterPostProcessing);

                m_profile.m_supportUnderwaterFog = m_editorUtils.ToggleLeft("SupportUnderwaterFog", m_profile.m_supportUnderwaterFog);

                m_profile.m_supportUnderwaterParticles = m_editorUtils.ToggleLeft("SupportUnderwaterParticles", m_profile.m_supportUnderwaterParticles);

                EditorGUILayout.HelpBox("Underwater effects are in 'Experimental' These will change and be updated over time.", MessageType.Info);
            }
        }

        private void WaterProfileSettingsEnabled(bool helpEnabled)
        {
            if (m_renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                m_waterProfile = m_profile.m_waterProfile;
                m_waterProfile = (GaiaConstants.GaiaWaterProfileType)EditorGUILayout.EnumPopup("Water Profile", m_waterProfile);
                GUILayout.Space(15f);
                //Profile values
                if (m_profileValues == null)
                {
                    m_profileValues = GetProfile();
                }
                else if (m_waterProfile != m_profileValues.m_profileType)
                {
                    m_profileValues = GetProfile();
                }
                m_editorUtils.Heading("PostProcessingSettings");
                m_profileValues.m_postProcessingProfile = m_editorUtils.TextField("PostProcessingProfile", m_profileValues.m_postProcessingProfile);
                GUILayout.Space(15f);
                m_editorUtils.Heading("UnderwaterEffectsSettings");
                m_profileValues.m_underwaterFogColor = m_editorUtils.ColorField("UnderwaterFogColor", m_profileValues.m_underwaterFogColor);
                m_profileValues.m_underwaterFogDistance = m_editorUtils.FloatField("UnderwaterFogDistance", m_profileValues.m_underwaterFogDistance);
                GUILayout.Space(15f);
                m_editorUtils.Heading("ColorTintSettings");
                m_profileValues.m_waterTint = m_editorUtils.ColorField("WaterTint", m_profileValues.m_waterTint);
                m_profileValues.m_shallowTint = m_editorUtils.ColorField("ShallowTint", m_profileValues.m_shallowTint);
                m_profileValues.m_shallowOffset = m_editorUtils.Slider("ShallowOffset", m_profileValues.m_shallowOffset, -1f, 1f);
                m_profileValues.m_depthTint = m_editorUtils.ColorField("DepthTint", m_profileValues.m_depthTint);
                m_profileValues.m_depthOffset = m_editorUtils.Slider("DepthOffset", m_profileValues.m_depthOffset, 0f, 2f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("OpacitySettings");
                m_profileValues.m_opacityOcean = m_editorUtils.Slider("OceanOpacity", m_profileValues.m_opacityOcean, 0f, 1f);
                m_profileValues.m_opacityBeach = m_editorUtils.Slider("BeachOpacity", m_profileValues.m_opacityBeach, 0f, 1f);
                m_profileValues.m_ignoreVertexColor = m_editorUtils.Toggle("IgnoreVertexColor", m_profileValues.m_ignoreVertexColor);
                GUILayout.Space(15f);
                m_editorUtils.Heading("PhysicallyBasedRenderingSettings");
                m_profileValues.m_occlusion = m_editorUtils.Slider("Occlusion", m_profileValues.m_occlusion, 0f, 1f);
                m_profileValues.m_metallic = m_editorUtils.Slider("Metallic", m_profileValues.m_metallic, 0f, 1f);
                m_profileValues.m_smoothness = m_editorUtils.Slider("Smoothness", m_profileValues.m_smoothness, 0f, 1f);
                m_profileValues.m_smoothnessVariance = m_editorUtils.Slider("SmoothnessVariance", m_profileValues.m_smoothnessVariance, 0f, 1f);
                m_profileValues.m_smoothnessThreshold = m_editorUtils.Slider("SmoothnessThreshold", m_profileValues.m_smoothnessThreshold, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("RefractionSettings");
                m_profileValues.m_refractedDepth = EditorGUILayout.Slider("RefractedDepth", m_profileValues.m_refractedDepth, 0f, 50f);
                m_profileValues.m_refractionScale = EditorGUILayout.Slider("RefractedScale", m_profileValues.m_refractionScale, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("LightingSettings");
                m_profileValues.m_indirectLightDiffuse = m_editorUtils.Slider("IndirectLightDiffuse", m_profileValues.m_indirectLightDiffuse, 0f, 1f);
                m_profileValues.m_indirectLightSpecular = m_editorUtils.Slider("IndirectLightSpecular", m_profileValues.m_indirectLightSpecular, 0f, 1f);
                m_profileValues.m_highlightTint = m_editorUtils.ColorField("HighlightTint", m_profileValues.m_highlightTint);
                m_profileValues.m_highlightOffset = m_editorUtils.Slider("HighlightOffset", m_profileValues.m_highlightOffset, -1f, -0.5f);
                m_profileValues.m_highlightSharpness = m_editorUtils.Slider("HighlightSharpness", m_profileValues.m_highlightSharpness, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("ShadowSettings");
                m_profileValues.m_shadowStrength = m_editorUtils.Slider("ShadowStrength", m_profileValues.m_shadowStrength, 0f, 1f);
                m_profileValues.m_shadowSharpness = m_editorUtils.Slider("ShadowSharpness", m_profileValues.m_shadowSharpness, 0f, 1f);
                m_profileValues.m_shadowOffset = m_editorUtils.Slider("ShadowOffset", m_profileValues.m_shadowOffset, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("ReflectionSettings");
                m_profileValues.m_reflectionIntenisty = m_editorUtils.Slider("ReflectionIntensity", m_profileValues.m_reflectionIntenisty, 0f, 1f);
                m_profileValues.m_reflectionWobble = m_editorUtils.Slider("ReflectionWobble", m_profileValues.m_reflectionWobble, 0f, 1f);
                m_profileValues.m_reflectionFresnelPower = m_editorUtils.Slider("ReflectionFresnelPower", m_profileValues.m_reflectionFresnelPower, 0f, 1f);
                m_profileValues.m_reflectionFresnelScale = m_editorUtils.Slider("ReflectionFresnelScale", m_profileValues.m_reflectionFresnelScale, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("NormalMapSettings");
                m_profileValues.m_normalMap = (Texture2D)m_editorUtils.ObjectField("NormalMap", m_profileValues.m_normalMap, typeof(Texture2D), false, GUILayout.Height(16f));
                m_profileValues.m_normalStrength = m_editorUtils.Slider("NormalStrength", m_profileValues.m_normalStrength, 0f, 1f);
                m_profileValues.m_normalTiling = m_editorUtils.Slider("NormalTiling", m_profileValues.m_normalTiling, 0f, 10000f);
                m_profileValues.m_normalSpeed = m_editorUtils.Slider("NormalSpeed", m_profileValues.m_normalSpeed, 0f, 0.1f);
                m_profileValues.m_normalTimescale = m_editorUtils.Slider("NormalTimescale", m_profileValues.m_normalTimescale, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("WaveTessellation");
                m_profileValues.m_tessellationAmount = m_editorUtils.Slider("TessellationStrength", m_profileValues.m_tessellationAmount, 0f, 1f);
                m_profileValues.m_tessellationDistance = m_editorUtils.FloatField("TessellationDistance", m_profileValues.m_tessellationDistance);
                GUILayout.Space(15f);
                m_editorUtils.Heading("WaveDirectionSettings");
                m_profileValues.m_wave01 = m_editorUtils.Vector4Field("Wave01", m_profileValues.m_wave01);
                m_profileValues.m_wave02 = m_editorUtils.Vector4Field("Wave02", m_profileValues.m_wave02);
                m_profileValues.m_wave03 = m_editorUtils.Vector4Field("Wave03", m_profileValues.m_wave03);
                m_profileValues.m_waveSpeed = m_editorUtils.Slider("WaveSpeed", m_profileValues.m_waveSpeed, 0f, 5f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("OceanFoamSettings");
                m_profileValues.m_oceanFoamTexture = (Texture2D)m_editorUtils.ObjectField("OceanFoamTexture", m_profileValues.m_oceanFoamTexture, typeof(Texture2D), false, GUILayout.Height(16f));
                m_profileValues.m_oceanFoamTint = m_editorUtils.ColorField("OceanFoamTint", m_profileValues.m_oceanFoamTint);
                m_profileValues.m_oceanFoamTiling = m_editorUtils.Slider("OceanFoamTiling", m_profileValues.m_oceanFoamTiling, 0f, 100f);
                m_profileValues.m_oceanFoamAmount = m_editorUtils.Slider("OceanFoamAmount", m_profileValues.m_oceanFoamAmount, 0f, 1f);
                m_profileValues.m_oceanFoamDistance = m_editorUtils.Slider("OceanFoamDistance", m_profileValues.m_oceanFoamDistance, 0f, 1000f);
                m_profileValues.m_oceanFoamSpeed = m_editorUtils.Slider("OceanFoamSpeed", m_profileValues.m_oceanFoamSpeed, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("BeachFoamSettings");
                m_profileValues.m_beachFoamTexture = (Texture2D)m_editorUtils.ObjectField("BeachFoamTexture", m_profileValues.m_beachFoamTexture, typeof(Texture2D), false, GUILayout.Height(16f));
                m_profileValues.m_beachFoamTint = m_editorUtils.ColorField("BeachFoamTint", m_profileValues.m_beachFoamTint);
                m_profileValues.m_beachFoamTiling = m_editorUtils.Slider("BeachFoamTiling", m_profileValues.m_beachFoamTiling, 0f, 1f);
                m_profileValues.m_beachFoamAmount = m_editorUtils.Slider("BeachFoamAmount", m_profileValues.m_beachFoamAmount, 0f, 1f);
                m_profileValues.m_beachFoamDistance = m_editorUtils.Slider("BeachFoamDistance", m_profileValues.m_beachFoamDistance, 0f, 100f);
                m_profileValues.m_beachFoamSpeed = m_editorUtils.Slider("BeachFoamSpeed", m_profileValues.m_beachFoamSpeed, 0f, 1f);            
            }
            else if (m_renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                EditorGUILayout.HelpBox("Water Profile Editing is not yet supported in LWRP. Coming Soon!", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Water Profile Editing is not yet supported in HDRP. Coming Soon!", MessageType.Info);
            }
        }

        /// <summary>
        /// Handy layer mask interface
        /// </summary>
        /// <param name="label"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        static LayerMask LayerMaskField(string label, EditorUtils editorUtils, LayerMask layerMask)
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

            label = editorUtils.GetContent(label).text;
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
        /// Gets profile
        /// </summary>
        /// <returns></returns>
        private GaiaWaterProfileValues GetProfile()
        {
            foreach (GaiaWaterProfileValues profile in m_profile.m_waterProfiles)
            {
                if (profile.m_profileType == m_waterProfile)
                {
                    return profile;
                }
            }

            return null;
        }
    }
}