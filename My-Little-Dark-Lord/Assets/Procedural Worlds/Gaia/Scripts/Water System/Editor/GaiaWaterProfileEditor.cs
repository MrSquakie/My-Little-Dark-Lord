using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ProcedualWorlds.WaterSystem.MeshGeneration;
using ProcedualWorlds.WaterSystem.Reflections;
using Gaia.Internal;
using PWCommon2;
using System.IO;

namespace Gaia
{
    [CustomEditor(typeof(GaiaWaterProfile))]
    public class GaiaWaterProfileEditor : PWEditor
    {
        private EditorUtils m_editorUtils;
        private string m_version;
        private string m_unityVersion;
        private GaiaSettings m_gaiaSettings;
        private GaiaWaterProfile m_profile;
        private GaiaWaterProfileValues m_profileValues;
        private PWS_MeshGenerationPro m_waterSystemPro;
        private GaiaConstants.EnvironmentRenderer m_renderPipeline;
        private List<string> m_profileList = new List<string>();
        private List<Material> m_allMaterials = new List<Material>();

        private const string m_materialLocation = "/Procedural Worlds/Gaia/Gaia Lighting and Water/Gaia Water/Ocean Water/Resources/Material";
        private const string m_builtInKeyWord = "SP";
        private const string m_lightweightKeyWord = "LW";
        private const string m_highDefinitionKeyWord = "HD";

        private int newProfileListIndex = 1;

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

            newProfileListIndex = m_gaiaSettings.m_selectedWaterProfile;
            if (newProfileListIndex < 0)
            {
                newProfileListIndex = 1;
            }

            SetupMaterials(m_renderPipeline, m_gaiaSettings, newProfileListIndex);

            m_profile.m_activeWaterMaterial = m_allMaterials[newProfileListIndex];
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
                bool enableEditMode = System.IO.Directory.Exists(GaiaUtils.GetAssetPath("Dev Utilities"));
                if (enableEditMode)
                {
                    m_profile.m_editSettings = EditorGUILayout.ToggleLeft("Use Procedural Worlds Editor Settings", m_profile.m_editSettings);
                }
                else
                {
                    m_profile.m_editSettings = false;
                }

                m_editorUtils.Panel("UpdateSettings", RealtimeUpdateEnabled, false);
                m_editorUtils.Panel("GlobalSettings", GlobalSettingsEnabled, false);
                m_editorUtils.Panel("ReflectionSettings", ReflectionSettingsEnabled, false);
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

                if (m_profile.m_updateInRealtime)
                {
                    if (m_profile.m_waterProfile != GaiaConstants.GaiaWaterProfileType.None)
                    {
                        GaiaWater.GetProfile(m_profile.m_waterProfile, m_profile, m_renderPipeline, true);
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
                    if (m_profile.m_waterProfile != GaiaConstants.GaiaWaterProfileType.None)
                    {
                        GaiaWater.GetProfile(m_profile.m_waterProfile, m_profile, m_renderPipeline, true);
                    }
                }
            }
        }

        private void GlobalSettingsEnabled(bool helpEnabled)
        {
            /*
            m_profile.m_masterWaterMaterial = (Material)m_editorUtils.ObjectField("MasterWaterMaterial", m_profile.m_masterWaterMaterial, typeof(Material), false, GUILayout.Height(16f));
            if (m_profile.m_masterWaterMaterial == null)
            {
                EditorGUILayout.HelpBox("Missing a master water material, please add one.", MessageType.Error);
            }
            */
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

            /*
            m_profile.m_enableOceanFoam = m_editorUtils.ToggleLeft("EnableOceanFoam", m_profile.m_enableOceanFoam);
            m_profile.m_enableBeachFoam = m_editorUtils.ToggleLeft("EnableBeachFoam", m_profile.m_enableBeachFoam);
            m_profile.m_autoWindControlOnWater = m_editorUtils.ToggleLeft("EnableAutoWindControl", m_profile.m_autoWindControlOnWater);
            */
        }

        private void ReflectionSettingsEnabled(bool helpEnabled)
        {
            m_profile.m_enableReflections = m_editorUtils.ToggleLeft("EnableReflections", m_profile.m_enableReflections);
            if (m_profile.m_enableReflections)
            {
                if (m_renderPipeline != GaiaConstants.EnvironmentRenderer.BuiltIn)
                {
                    if (m_renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
                    {
                        EditorGUILayout.HelpBox("Realtime reflections is not yet supported in SRP", MessageType.Info);
                    }
                    else
                    {
                        m_profile.m_hdPlanarReflections = (GameObject)m_editorUtils.ObjectField("HDPlanarReflections", m_profile.m_hdPlanarReflections, typeof(GameObject), false, GUILayout.Height(16f));
                        if (m_profile.m_hdPlanarReflections == null)
                        {
                            EditorGUILayout.HelpBox("Missing HD Planar Reflections Prefab. Please add it to use this feature.", MessageType.Error);
                        }
                        else
                        {
                            if (m_editorUtils.Button("FocusPlanarReflections"))
                            {
                                GameObject reflectionObject = GameObject.Find(m_profile.m_hdPlanarReflections.name);
                                if (reflectionObject != null)
                                {
                                    Selection.activeObject = reflectionObject;
                                }
                                else
                                {
                                    Debug.Log("Object doesn't exist in the scene");
                                }
                            }

                            EditorGUILayout.HelpBox("In HDRP reflections are handled on the Planar Reflections. You can edit the settings from there. To changes the reflection resolution you can change this in the HD Render Pipeline asset. If you're reflections don't cover the field of view focus the planar reflections and update the field of view on the Planar Reflections object.", MessageType.Info);
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Reflections render the object reflections on the water surface. This can be very expensive feature and could cause performance issue on low end machines.", MessageType.Info);

                    GUILayout.Space(15f);
                    m_editorUtils.Heading("ReflectionSupport");
                    m_profile.m_allowMSAA = m_editorUtils.ToggleLeft("AllowMSAA", m_profile.m_allowMSAA);
                    m_profile.m_useHDR = m_editorUtils.ToggleLeft("UseHDR", m_profile.m_useHDR);
                    m_profile.m_disablePixelLights = m_editorUtils.ToggleLeft("DisablePixelLights", m_profile.m_disablePixelLights);
                    m_profile.m_useCustomRenderDistance = m_editorUtils.ToggleLeft("UseCustomRenderDistance", m_profile.m_useCustomRenderDistance);
                    GUILayout.Space(15f);
                    m_editorUtils.Heading("ReflectionSetup");
                    m_profile.m_clipPlaneOffset = m_editorUtils.Slider("ClipPlaneOffset", m_profile.m_clipPlaneOffset, -5f, 100f);
                    m_profile.m_waterRenderUpdateMode = (PWS_WaterReflections.RenderUpdateMode)EditorGUILayout.EnumPopup("Render Update Mode", m_profile.m_waterRenderUpdateMode);
                    if (m_profile.m_waterRenderUpdateMode == PWS_WaterReflections.RenderUpdateMode.Interval)
                    {
                        m_profile.m_interval = m_editorUtils.Slider("IntervalTime", m_profile.m_interval, 0f, 50f);
                    }
                    GUILayout.Space(15f);
                    m_editorUtils.Heading("ReflectionRendering");
                    m_profile.m_reflectionResolution = (GaiaConstants.GaiaProWaterReflectionsQuality)EditorGUILayout.EnumPopup("Reflection Resolution", m_profile.m_reflectionResolution);
                    m_profile.m_reflectedLayers = LayerMaskField("ReflectedLayers", m_editorUtils, m_profile.m_reflectedLayers);
                    if (m_profile.m_useCustomRenderDistance)
                    {
                        m_profile.m_customRenderDistance = m_editorUtils.FloatField("CustomRenderDistance", m_profile.m_customRenderDistance);
                    }

                    if (!m_profile.m_updateInRealtime)
                    {
                        if (m_editorUtils.Button("UpdateReflectionSettings"))
                        {
                            GaiaWater.SetupWaterReflectionSettings(m_profile, true);
                        }
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
                m_profile.m_waterMeshQuality = (GaiaConstants.WaterMeshQuality)EditorGUILayout.EnumPopup("Water Mesh Quality", m_profile.m_waterMeshQuality);
                m_profile.m_meshType = (PWS_MeshGenerationPro.MeshType)EditorGUILayout.EnumPopup("Mesh Type", m_profile.m_meshType);
                if (m_profile.m_waterMeshQuality == GaiaConstants.WaterMeshQuality.Custom)
                {
                    if (m_profile.m_meshType == PWS_MeshGenerationPro.MeshType.Plane)
                    {
                        m_profile.m_customMeshQuality = m_editorUtils.IntSlider("CustomMeshQuality", m_profile.m_customMeshQuality, 16, 256);
                    }
                    else
                    {
                        m_profile.m_customMeshQuality = m_editorUtils.IntSlider("CustomMeshQuality", (int)m_profile.m_customMeshQuality * 2, 2, 256) / 2;
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
            if (m_renderPipeline == GaiaConstants.EnvironmentRenderer.HighDefinition)
            {
                EditorGUILayout.HelpBox("Underwater effects are not yet supported in HDRP.", MessageType.Info);
            }
            else
            {
                if (m_editorUtils.Button("EditUnderwaterSettings"))
                {
                    FocusUnderwaterSettings();
                }

                m_editorUtils.Panel("CausticSettings", CausticSettingsEnabled, false);

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

                    if (m_renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
                    {
                        EditorGUILayout.HelpBox("Underwater effects are in 'Experimental' These will change and be updated over time. LWRP does work but you may experience some artifacts.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Underwater effects are in 'Experimental' These will change and be updated over time.", MessageType.Info);
                    }
                }
            }
        }

        private void WaterProfileSettingsEnabled(bool helpEnabled)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Water Profile");
            if (newProfileListIndex > m_profileList.Count)
            {
                newProfileListIndex = 0;
            }
            newProfileListIndex = EditorGUILayout.Popup(newProfileListIndex, m_profileList.ToArray(), GUILayout.MaxWidth(2000f), GUILayout.Height(16f));
            EditorGUILayout.EndHorizontal();

            if (m_gaiaSettings.m_selectedWaterProfile != newProfileListIndex)
            {
                m_gaiaSettings.m_selectedWaterProfile = newProfileListIndex;
                m_profile.m_activeWaterMaterial = m_allMaterials[newProfileListIndex];
                GaiaWater.GetProfile(m_profile.m_waterProfile, m_profile, m_renderPipeline, true);
            }

            if (m_editorUtils.Button("ModifyMaterialSettings"))
            {
                FocusWaterMaterial(m_allMaterials[newProfileListIndex]);
            }

            if (m_renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                /*
                m_profile.m_waterProfile = (GaiaConstants.GaiaWaterProfileType)EditorGUILayout.EnumPopup("Water Profile", m_profile.m_waterProfile);

                GUILayout.Space(15f);
                //Profile values
                if (m_profileValues == null)
                {
                    m_profileValues = GetProfile();
                }
                else if (m_profile.m_waterProfile != m_profileValues.m_profileType)
                {
                    m_profileValues = GetProfile();
                }

                m_editorUtils.Heading("PostProcessingSettings");
                m_profileValues.m_postProcessingProfile = m_editorUtils.TextField("PostProcessingProfile", m_profileValues.m_postProcessingProfile);
                GUILayout.Space(15f);
                m_editorUtils.Heading("UnderwaterEffectsSettings");
                m_profileValues.m_underwaterFogColor = m_editorUtils.ColorField("UnderwaterFogColor", m_profileValues.m_underwaterFogColor);
                m_profileValues.m_underwaterFogDistance = m_editorUtils.FloatField("UnderwaterFogDistance", m_profileValues.m_underwaterFogDistance);
                m_profileValues.m_underwaterNearFogDistance = m_editorUtils.FloatField("UnderwaterNearFogDistance", m_profileValues.m_underwaterNearFogDistance);
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
                m_profileValues.m_reflectionFresnelPower = m_editorUtils.Slider("ReflectionFresnelPower", m_profileValues.m_reflectionFresnelPower, 0f, 10f);
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
                m_profileValues.m_tessellationAmount = m_editorUtils.Slider("TessellationStrength", m_profileValues.m_tessellationAmount, 0.01f, 1f);
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
                */
            }
            else if (m_renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                EditorGUILayout.HelpBox("Lightweight water profile editing will be coming soon", MessageType.Info);

                /*
                m_editorUtils.Heading("PostProcessingSettings");
                m_profileValues.m_postProcessingProfile = m_editorUtils.TextField("PostProcessingProfile", m_profileValues.m_postProcessingProfile);
                GUILayout.Space(15f);
                m_editorUtils.Heading("UnderwaterEffectsSettings");
                m_profileValues.m_underwaterFogColor = m_editorUtils.ColorField("UnderwaterFogColor", m_profileValues.m_underwaterFogColor);
                m_profileValues.m_underwaterFogDistance = m_editorUtils.FloatField("UnderwaterFogDistance", m_profileValues.m_underwaterFogDistance);
                GUILayout.Space(15f);
                m_editorUtils.Heading("ColorTintSettings");
                m_profileValues.m_LWwaterTint = m_editorUtils.ColorField("WaterTint", m_profileValues.m_LWwaterTint);
                m_profileValues.m_LWshallowTint = m_editorUtils.ColorField("ShallowTint", m_profileValues.m_LWshallowTint);
                m_profileValues.m_LWshallowOffset = m_editorUtils.Slider("ShallowOffset", m_profileValues.m_LWshallowOffset, -1f, 1f);
                m_profileValues.m_LWdepthTint = m_editorUtils.ColorField("DepthTint", m_profileValues.m_LWdepthTint);
                m_profileValues.m_LWdepthOffset = m_editorUtils.Slider("DepthOffset", m_profileValues.m_LWdepthOffset, 0f, 2f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("OpacitySettings");
                m_profileValues.m_LWopacityOcean = m_editorUtils.Slider("OceanOpacity", m_profileValues.m_LWopacityOcean, 0f, 1f);
                m_profileValues.m_LWopacityBeach = m_editorUtils.Slider("BeachOpacity", m_profileValues.m_LWopacityBeach, 0f, 1f);
                m_profileValues.m_LWignoreVertexColor = m_editorUtils.Toggle("IgnoreVertexColor", m_profileValues.m_LWignoreVertexColor);
                GUILayout.Space(15f);
                m_editorUtils.Heading("PhysicallyBasedRenderingSettings");
                m_profileValues.m_LWocclusion = m_editorUtils.Slider("Occlusion", m_profileValues.m_LWocclusion, 0f, 1f);
                m_profileValues.m_LWmetallic = m_editorUtils.Slider("Metallic", m_profileValues.m_LWmetallic, 0f, 1f);
                m_profileValues.m_LWsmoothness = m_editorUtils.Slider("Smoothness", m_profileValues.m_LWsmoothness, 0f, 1f);
                m_profileValues.m_LWsmoothnessVariance = m_editorUtils.Slider("SmoothnessVariance", m_profileValues.m_LWsmoothnessVariance, 0f, 1f);
                m_profileValues.m_LWsmoothnessThreshold = m_editorUtils.Slider("SmoothnessThreshold", m_profileValues.m_LWsmoothnessThreshold, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("RefractionSettings");
                m_profileValues.m_LWrefractedDepth = EditorGUILayout.Slider("RefractedDepth", m_profileValues.m_LWrefractedDepth, 0f, 50f);
                m_profileValues.m_LWrefractionScale = EditorGUILayout.Slider("RefractedScale", m_profileValues.m_LWrefractionScale, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("LightingSettings");
                m_profileValues.m_LWindirectLightDiffuse = m_editorUtils.Slider("IndirectLightDiffuse", m_profileValues.m_LWindirectLightDiffuse, 0f, 1f);
                m_profileValues.m_LWindirectLightSpecular = m_editorUtils.Slider("IndirectLightSpecular", m_profileValues.m_LWindirectLightSpecular, 0f, 1f);
                m_profileValues.m_LWhighlightTint = m_editorUtils.ColorField("HighlightTint", m_profileValues.m_LWhighlightTint);
                m_profileValues.m_LWhighlightOffset = m_editorUtils.Slider("HighlightOffset", m_profileValues.m_LWhighlightOffset, -1f, -0.5f);
                m_profileValues.m_LWhighlightSharpness = m_editorUtils.Slider("HighlightSharpness", m_profileValues.m_LWhighlightSharpness, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("ShadowSettings");
                m_profileValues.m_LWshadowStrength = m_editorUtils.Slider("ShadowStrength", m_profileValues.m_LWshadowStrength, 0f, 1f);
                m_profileValues.m_LWshadowSharpness = m_editorUtils.Slider("ShadowSharpness", m_profileValues.m_LWshadowSharpness, 0f, 1f);
                m_profileValues.m_LWshadowOffset = m_editorUtils.Slider("ShadowOffset", m_profileValues.m_LWshadowOffset, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("ReflectionSettings");
                m_profileValues.m_LWreflectionIntenisty = m_editorUtils.Slider("ReflectionIntensity", m_profileValues.m_LWreflectionIntenisty, 0f, 1f);
                m_profileValues.m_LWreflectionWobble = m_editorUtils.Slider("ReflectionWobble", m_profileValues.m_LWreflectionWobble, 0f, 1f);
                m_profileValues.m_LWreflectionFresnelPower = m_editorUtils.Slider("ReflectionFresnelPower", m_profileValues.m_LWreflectionFresnelPower, 0f, 10f);
                m_profileValues.m_LWreflectionFresnelScale = m_editorUtils.Slider("ReflectionFresnelScale", m_profileValues.m_LWreflectionFresnelScale, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("NormalMapSettings");
                m_profileValues.m_LWnormalMap = (Texture2D)m_editorUtils.ObjectField("NormalMap", m_profileValues.m_LWnormalMap, typeof(Texture2D), false, GUILayout.Height(16f));
                m_profileValues.m_LWnormalStrength = m_editorUtils.Slider("NormalStrength", m_profileValues.m_LWnormalStrength, 0f, 1f);
                m_profileValues.m_LWnormalTiling = m_editorUtils.Slider("NormalTiling", m_profileValues.m_LWnormalTiling, 0f, 10000f);
                m_profileValues.m_LWnormalSpeed = m_editorUtils.Slider("NormalSpeed", m_profileValues.m_LWnormalSpeed, 0f, 0.1f);
                m_profileValues.m_LWnormalTimescale = m_editorUtils.Slider("NormalTimescale", m_profileValues.m_LWnormalTimescale, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("WaveTessellation");
                m_profileValues.m_LWtessellationAmount = m_editorUtils.Slider("TessellationStrength", m_profileValues.m_LWtessellationAmount, 0.01f, 1f);
                m_profileValues.m_LWtessellationDistance = m_editorUtils.FloatField("TessellationDistance", m_profileValues.m_LWtessellationDistance);
                GUILayout.Space(15f);
                m_editorUtils.Heading("WaveDirectionSettings");
                m_profileValues.m_LWwave01 = m_editorUtils.Vector4Field("Wave01", m_profileValues.m_LWwave01);
                m_profileValues.m_LWwave02 = m_editorUtils.Vector4Field("Wave02", m_profileValues.m_LWwave02);
                m_profileValues.m_LWwave03 = m_editorUtils.Vector4Field("Wave03", m_profileValues.m_LWwave03);
                m_profileValues.m_LWwaveSpeed = m_editorUtils.Slider("WaveSpeed", m_profileValues.m_LWwaveSpeed, 0f, 5f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("OceanFoamSettings");
                m_profileValues.m_LWoceanFoamTexture = (Texture2D)m_editorUtils.ObjectField("OceanFoamTexture", m_profileValues.m_LWoceanFoamTexture, typeof(Texture2D), false, GUILayout.Height(16f));
                m_profileValues.m_LWoceanFoamTint = m_editorUtils.ColorField("OceanFoamTint", m_profileValues.m_LWoceanFoamTint);
                m_profileValues.m_LWoceanFoamTiling = m_editorUtils.Slider("OceanFoamTiling", m_profileValues.m_LWoceanFoamTiling, 0f, 100f);
                m_profileValues.m_LWoceanFoamAmount = m_editorUtils.Slider("OceanFoamAmount", m_profileValues.m_LWoceanFoamAmount, 0f, 1f);
                m_profileValues.m_LWoceanFoamDistance = m_editorUtils.Slider("OceanFoamDistance", m_profileValues.m_LWoceanFoamDistance, 0f, 1000f);
                m_profileValues.m_LWoceanFoamSpeed = m_editorUtils.Slider("OceanFoamSpeed", m_profileValues.m_LWoceanFoamSpeed, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("BeachFoamSettings");
                m_profileValues.m_LWbeachFoamTexture = (Texture2D)m_editorUtils.ObjectField("BeachFoamTexture", m_profileValues.m_LWbeachFoamTexture, typeof(Texture2D), false, GUILayout.Height(16f));
                m_profileValues.m_LWbeachFoamTint = m_editorUtils.ColorField("BeachFoamTint", m_profileValues.m_LWbeachFoamTint);
                m_profileValues.m_LWbeachFoamTiling = m_editorUtils.Slider("BeachFoamTiling", m_profileValues.m_LWbeachFoamTiling, 0f, 1f);
                m_profileValues.m_LWbeachFoamAmount = m_editorUtils.Slider("BeachFoamAmount", m_profileValues.m_LWbeachFoamAmount, 0f, 1f);
                m_profileValues.m_LWbeachFoamDistance = m_editorUtils.Slider("BeachFoamDistance", m_profileValues.m_LWbeachFoamDistance, 0f, 100f);
                m_profileValues.m_LWbeachFoamSpeed = m_editorUtils.Slider("BeachFoamSpeed", m_profileValues.m_LWbeachFoamSpeed, 0f, 1f);
                */
            }
            else
            {
                EditorGUILayout.HelpBox("High Definition water profile editing will be coming soon", MessageType.Info);

                /*
                m_editorUtils.Heading("PostProcessingSettings");
                m_profileValues.m_postProcessingProfile = m_editorUtils.TextField("PostProcessingProfile", m_profileValues.m_postProcessingProfile);
                GUILayout.Space(15f);
                m_editorUtils.Heading("UnderwaterEffectsSettings");
                m_profileValues.m_underwaterFogColor = m_editorUtils.ColorField("UnderwaterFogColor", m_profileValues.m_underwaterFogColor);
                m_profileValues.m_underwaterFogDistance = m_editorUtils.FloatField("UnderwaterFogDistance", m_profileValues.m_underwaterFogDistance);
                GUILayout.Space(15f);
                m_editorUtils.Heading("ColorTintSettings");
                m_profileValues.m_HDwaterTint = m_editorUtils.ColorField("WaterTint", m_profileValues.m_HDwaterTint);
                m_profileValues.m_HDshallowTint = m_editorUtils.ColorField("ShallowTint", m_profileValues.m_HDshallowTint);
                m_profileValues.m_HDshallowOffset = m_editorUtils.Slider("ShallowOffset", m_profileValues.m_HDshallowOffset, -1f, 1f);
                m_profileValues.m_HDdepthTint = m_editorUtils.ColorField("DepthTint", m_profileValues.m_HDdepthTint);
                m_profileValues.m_HDdepthOffset = m_editorUtils.Slider("DepthOffset", m_profileValues.m_HDdepthOffset, 0f, 2f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("OpacitySettings");
                m_profileValues.m_HDopacityOcean = m_editorUtils.Slider("OceanOpacity", m_profileValues.m_HDopacityOcean, 0f, 1f);
                m_profileValues.m_HDopacityBeach = m_editorUtils.Slider("BeachOpacity", m_profileValues.m_HDopacityBeach, 0f, 1f);
                m_profileValues.m_HDignoreVertexColor = m_editorUtils.Toggle("IgnoreVertexColor", m_profileValues.m_HDignoreVertexColor);
                GUILayout.Space(15f);
                m_editorUtils.Heading("PhysicallyBasedRenderingSettings");
                m_profileValues.m_HDocclusion = m_editorUtils.Slider("Occlusion", m_profileValues.m_HDocclusion, 0f, 1f);
                m_profileValues.m_HDmetallic = m_editorUtils.Slider("Metallic", m_profileValues.m_HDmetallic, 0f, 1f);
                m_profileValues.m_HDsmoothness = m_editorUtils.Slider("Smoothness", m_profileValues.m_HDsmoothness, 0f, 1f);
                m_profileValues.m_HDsmoothnessVariance = m_editorUtils.Slider("SmoothnessVariance", m_profileValues.m_HDsmoothnessVariance, 0f, 1f);
                m_profileValues.m_HDsmoothnessThreshold = m_editorUtils.Slider("SmoothnessThreshold", m_profileValues.m_HDsmoothnessThreshold, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("RefractionSettings");
                m_profileValues.m_HDrefractedDepth = EditorGUILayout.Slider("RefractedDepth", m_profileValues.m_HDrefractedDepth, 0f, 50f);
                m_profileValues.m_HDrefractionScale = EditorGUILayout.Slider("RefractedScale", m_profileValues.m_HDrefractionScale, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("LightingSettings");
                m_profileValues.m_HDindirectLightDiffuse = m_editorUtils.Slider("IndirectLightDiffuse", m_profileValues.m_HDindirectLightDiffuse, 0f, 1f);
                m_profileValues.m_HDindirectLightSpecular = m_editorUtils.Slider("IndirectLightSpecular", m_profileValues.m_HDindirectLightSpecular, 0f, 1f);
                m_profileValues.m_HDhighlightTint = m_editorUtils.ColorField("HighlightTint", m_profileValues.m_HDhighlightTint);
                m_profileValues.m_HDhighlightOffset = m_editorUtils.Slider("HighlightOffset", m_profileValues.m_HDhighlightOffset, -1f, -0.5f);
                m_profileValues.m_HDhighlightSharpness = m_editorUtils.Slider("HighlightSharpness", m_profileValues.m_HDhighlightSharpness, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("ShadowSettings");
                m_profileValues.m_HDshadowStrength = m_editorUtils.Slider("ShadowStrength", m_profileValues.m_HDshadowStrength, 0f, 1f);
                m_profileValues.m_HDshadowSharpness = m_editorUtils.Slider("ShadowSharpness", m_profileValues.m_HDshadowSharpness, 0f, 1f);
                m_profileValues.m_HDshadowOffset = m_editorUtils.Slider("ShadowOffset", m_profileValues.m_HDshadowOffset, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("ReflectionSettings");
                m_profileValues.m_HDreflectionIntenisty = m_editorUtils.Slider("ReflectionIntensity", m_profileValues.m_HDreflectionIntenisty, 0f, 1f);
                m_profileValues.m_HDreflectionWobble = m_editorUtils.Slider("ReflectionWobble", m_profileValues.m_HDreflectionWobble, 0f, 1f);
                m_profileValues.m_HDreflectionFresnelPower = m_editorUtils.Slider("ReflectionFresnelPower", m_profileValues.m_HDreflectionFresnelPower, 0f, 10f);
                m_profileValues.m_HDreflectionFresnelScale = m_editorUtils.Slider("ReflectionFresnelScale", m_profileValues.m_HDreflectionFresnelScale, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("NormalMapSettings");
                m_profileValues.m_HDnormalMap = (Texture2D)m_editorUtils.ObjectField("NormalMap", m_profileValues.m_HDnormalMap, typeof(Texture2D), false, GUILayout.Height(16f));
                m_profileValues.m_HDnormalStrength = m_editorUtils.Slider("NormalStrength", m_profileValues.m_HDnormalStrength, 0f, 1f);
                m_profileValues.m_HDnormalTiling = m_editorUtils.Slider("NormalTiling", m_profileValues.m_HDnormalTiling, 0f, 10000f);
                m_profileValues.m_HDnormalSpeed = m_editorUtils.Slider("NormalSpeed", m_profileValues.m_HDnormalSpeed, 0f, 0.1f);
                m_profileValues.m_HDnormalTimescale = m_editorUtils.Slider("NormalTimescale", m_profileValues.m_HDnormalTimescale, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("WaveTessellation");
                m_profileValues.m_HDtessellationAmount = m_editorUtils.Slider("TessellationStrength", m_profileValues.m_HDtessellationAmount, 0.01f, 1f);
                m_profileValues.m_HDtessellationDistance = m_editorUtils.FloatField("TessellationDistance", m_profileValues.m_HDtessellationDistance);
                GUILayout.Space(15f);
                m_editorUtils.Heading("WaveDirectionSettings");
                m_profileValues.m_HDwave01 = m_editorUtils.Vector4Field("Wave01", m_profileValues.m_HDwave01);
                m_profileValues.m_HDwave02 = m_editorUtils.Vector4Field("Wave02", m_profileValues.m_HDwave02);
                m_profileValues.m_HDwave03 = m_editorUtils.Vector4Field("Wave03", m_profileValues.m_HDwave03);
                m_profileValues.m_HDwaveSpeed = m_editorUtils.Slider("WaveSpeed", m_profileValues.m_HDwaveSpeed, 0f, 5f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("OceanFoamSettings");
                m_profileValues.m_HDoceanFoamTexture = (Texture2D)m_editorUtils.ObjectField("OceanFoamTexture", m_profileValues.m_oceanFoamTexture, typeof(Texture2D), false, GUILayout.Height(16f));
                m_profileValues.m_HDoceanFoamTint = m_editorUtils.ColorField("OceanFoamTint", m_profileValues.m_HDoceanFoamTint);
                m_profileValues.m_HDoceanFoamTiling = m_editorUtils.Slider("OceanFoamTiling", m_profileValues.m_HDoceanFoamTiling, 0f, 100f);
                m_profileValues.m_HDoceanFoamAmount = m_editorUtils.Slider("OceanFoamAmount", m_profileValues.m_HDoceanFoamAmount, 0f, 1f);
                m_profileValues.m_HDoceanFoamDistance = m_editorUtils.Slider("OceanFoamDistance", m_profileValues.m_HDoceanFoamDistance, 0f, 1000f);
                m_profileValues.m_HDoceanFoamSpeed = m_editorUtils.Slider("OceanFoamSpeed", m_profileValues.m_HDoceanFoamSpeed, 0f, 1f);
                GUILayout.Space(15f);
                m_editorUtils.Heading("BeachFoamSettings");
                m_profileValues.m_HDbeachFoamTexture = (Texture2D)m_editorUtils.ObjectField("BeachFoamTexture", m_profileValues.m_HDbeachFoamTexture, typeof(Texture2D), false, GUILayout.Height(16f));
                m_profileValues.m_HDbeachFoamTint = m_editorUtils.ColorField("BeachFoamTint", m_profileValues.m_HDbeachFoamTint);
                m_profileValues.m_HDbeachFoamTiling = m_editorUtils.Slider("BeachFoamTiling", m_profileValues.m_HDbeachFoamTiling, 0f, 1f);
                m_profileValues.m_HDbeachFoamAmount = m_editorUtils.Slider("BeachFoamAmount", m_profileValues.m_HDbeachFoamAmount, 0f, 1f);
                m_profileValues.m_HDbeachFoamDistance = m_editorUtils.Slider("BeachFoamDistance", m_profileValues.m_HDbeachFoamDistance, 0f, 100f);
                m_profileValues.m_HDbeachFoamSpeed = m_editorUtils.Slider("BeachFoamSpeed", m_profileValues.m_HDbeachFoamSpeed, 0f, 1f);
                */
            }
        }

        private void CausticSettingsEnabled(bool helpEnabled)
        {
            m_profile.m_useCastics = m_editorUtils.ToggleLeft("UseCaustics", m_profile.m_useCastics, helpEnabled);
            if (m_profile.m_useCastics)
            {
                m_profile.m_mainCausticLight = (Light)m_editorUtils.ObjectField("MainCausticLight", m_profile.m_mainCausticLight, typeof(Light), false, helpEnabled, GUILayout.Height(16f));
                if (m_profile.m_mainCausticLight == null)
                {
                    m_profile.m_mainCausticLight = GetMainLight();
                }
                m_profile.m_causticFramePerSecond = m_editorUtils.IntSlider("CausticFPS", m_profile.m_causticFramePerSecond, 15, 120, helpEnabled);
                m_profile.m_causticSize = m_editorUtils.Slider("CausticSize", m_profile.m_causticSize, 0.1f, 100f, helpEnabled);

                EditorGUILayout.HelpBox("Caustics setup is applied directly to the Directional Light using the cookie setup.", MessageType.Info);
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
        /// Gets and returns a directional light
        /// </summary>
        /// <returns></returns>
        private Light GetMainLight()
        {
            Light[] mainLight = FindObjectsOfType<Light>();
            if (mainLight != null)
            {
                foreach (Light light in mainLight)
                {
                    if (light.type == LightType.Directional)
                    {
                        return light;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets profile
        /// </summary>
        /// <returns></returns>
        private GaiaWaterProfileValues GetProfile()
        {
            foreach (GaiaWaterProfileValues profile in m_profile.m_waterProfiles)
            {
                if (profile.m_profileType == m_profile.m_waterProfile)
                {
                    return profile;
                }
            }

            return null;
        }

        /// <summary>
        /// Copies all the profile values from built-in to all LWRP/HDRP profiles
        /// </summary>
        private void CopyAllProfiles()
        {
            foreach (GaiaWaterProfileValues profile in m_profile.m_waterProfiles)
            {
                //LWRP
                profile.m_LWwaterTint = profile.m_waterTint;
                profile.m_LWshallowTint = profile.m_shallowTint;
                profile.m_LWshallowOffset = profile.m_shallowOffset;
                profile.m_LWdepthTint = profile.m_depthTint;
                profile.m_LWdepthOffset = profile.m_depthOffset;

                profile.m_LWopacityOcean = profile.m_opacityOcean;
                profile.m_LWopacityBeach = profile.m_opacityBeach;
                profile.m_LWignoreVertexColor = profile.m_ignoreVertexColor;

                profile.m_LWocclusion = profile.m_occlusion;
                profile.m_LWmetallic = profile.m_metallic;
                profile.m_LWsmoothness = profile.m_smoothness;
                profile.m_LWsmoothnessVariance = profile.m_smoothnessVariance;
                profile.m_LWsmoothnessThreshold = profile.m_smoothnessThreshold;

                profile.m_LWrefractedDepth = profile.m_refractedDepth;
                profile.m_LWrefractionScale = profile.m_refractionScale;

                profile.m_LWindirectLightDiffuse = profile.m_indirectLightDiffuse;
                profile.m_LWindirectLightSpecular = profile.m_indirectLightSpecular;
                profile.m_LWhighlightTint = profile.m_highlightTint;
                profile.m_LWhighlightOffset = profile.m_highlightOffset;
                profile.m_LWhighlightSharpness = profile.m_highlightSharpness;

                profile.m_LWshadowStrength = profile.m_shadowStrength;
                profile.m_LWshadowSharpness = profile.m_shadowSharpness;
                profile.m_LWshadowOffset = profile.m_shadowOffset;

                profile.m_LWreflectionIntenisty = profile.m_reflectionIntenisty;
                profile.m_LWreflectionWobble = profile.m_reflectionWobble;
                profile.m_LWreflectionFresnelPower = profile.m_reflectionFresnelPower;
                profile.m_LWreflectionFresnelScale = profile.m_reflectionFresnelScale;

                profile.m_LWnormalMap = profile.m_normalMap;
                profile.m_LWnormalStrength = profile.m_normalStrength;
                profile.m_LWnormalTiling = profile.m_normalTiling;
                profile.m_LWnormalSpeed = profile.m_normalSpeed;
                profile.m_LWnormalTimescale = profile.m_normalTimescale;

                profile.m_LWtessellationAmount = profile.m_tessellationAmount;
                profile.m_LWtessellationDistance = profile.m_tessellationDistance;

                profile.m_LWwave01 = profile.m_wave01;
                profile.m_LWwave02 = profile.m_wave02;
                profile.m_LWwave03 = profile.m_wave03;
                profile.m_LWwaveSpeed = profile.m_waveSpeed;

                profile.m_LWoceanFoamTexture = profile.m_oceanFoamTexture;
                profile.m_LWoceanFoamTint = profile.m_oceanFoamTint;
                profile.m_LWoceanFoamTiling = profile.m_oceanFoamTiling;
                profile.m_LWoceanFoamAmount = profile.m_oceanFoamAmount;
                profile.m_LWoceanFoamDistance = profile.m_oceanFoamDistance;
                profile.m_LWoceanFoamSpeed = profile.m_oceanFoamSpeed;

                profile.m_LWbeachFoamTexture = profile.m_beachFoamTexture;
                profile.m_LWbeachFoamTint = profile.m_beachFoamTint;
                profile.m_LWbeachFoamTiling = profile.m_beachFoamTiling;
                profile.m_LWbeachFoamAmount = profile.m_beachFoamAmount;
                profile.m_LWbeachFoamDistance = profile.m_beachFoamDistance;
                profile.m_LWbeachFoamSpeed = profile.m_beachFoamSpeed;

                Debug.Log("LWRP values copied successfully");

                //HDRP
                profile.m_HDwaterTint = profile.m_waterTint;
                profile.m_HDshallowTint = profile.m_shallowTint;
                profile.m_HDshallowOffset = profile.m_shallowOffset;
                profile.m_HDdepthTint = profile.m_depthTint;
                profile.m_HDdepthOffset = profile.m_depthOffset;

                profile.m_HDopacityOcean = profile.m_opacityOcean;
                profile.m_HDopacityBeach = profile.m_opacityBeach;
                profile.m_HDignoreVertexColor = profile.m_ignoreVertexColor;

                profile.m_HDocclusion = profile.m_occlusion;
                profile.m_HDmetallic = profile.m_metallic;
                profile.m_HDsmoothness = profile.m_smoothness;
                profile.m_HDsmoothnessVariance = profile.m_smoothnessVariance;
                profile.m_HDsmoothnessThreshold = profile.m_smoothnessThreshold;

                profile.m_HDrefractedDepth = profile.m_refractedDepth;
                profile.m_HDrefractionScale = profile.m_refractionScale;

                profile.m_HDindirectLightDiffuse = profile.m_indirectLightDiffuse;
                profile.m_HDindirectLightSpecular = profile.m_indirectLightSpecular;
                profile.m_HDhighlightTint = profile.m_highlightTint;
                profile.m_HDhighlightOffset = profile.m_highlightOffset;
                profile.m_HDhighlightSharpness = profile.m_highlightSharpness;

                profile.m_HDshadowStrength = profile.m_shadowStrength;
                profile.m_HDshadowSharpness = profile.m_shadowSharpness;
                profile.m_HDshadowOffset = profile.m_shadowOffset;

                profile.m_HDreflectionIntenisty = profile.m_reflectionIntenisty;
                profile.m_HDreflectionWobble = profile.m_reflectionWobble;
                profile.m_HDreflectionFresnelPower = profile.m_reflectionFresnelPower;
                profile.m_HDreflectionFresnelScale = profile.m_reflectionFresnelScale;

                profile.m_HDnormalMap = profile.m_normalMap;
                profile.m_HDnormalStrength = profile.m_normalStrength;
                profile.m_HDnormalTiling = profile.m_normalTiling;
                profile.m_HDnormalSpeed = profile.m_normalSpeed;
                profile.m_HDnormalTimescale = profile.m_normalTimescale;

                profile.m_HDtessellationAmount = profile.m_tessellationAmount;
                profile.m_HDtessellationDistance = profile.m_tessellationDistance;

                profile.m_HDwave01 = profile.m_wave01;
                profile.m_HDwave02 = profile.m_wave02;
                profile.m_HDwave03 = profile.m_wave03;
                profile.m_HDwaveSpeed = profile.m_waveSpeed;

                profile.m_HDoceanFoamTexture = profile.m_oceanFoamTexture;
                profile.m_HDoceanFoamTint = profile.m_oceanFoamTint;
                profile.m_HDoceanFoamTiling = profile.m_oceanFoamTiling;
                profile.m_HDoceanFoamAmount = profile.m_oceanFoamAmount;
                profile.m_HDoceanFoamDistance = profile.m_oceanFoamDistance;
                profile.m_HDoceanFoamSpeed = profile.m_oceanFoamSpeed;

                profile.m_HDbeachFoamTexture = profile.m_beachFoamTexture;
                profile.m_HDbeachFoamTint = profile.m_beachFoamTint;
                profile.m_HDbeachFoamTiling = profile.m_beachFoamTiling;
                profile.m_HDbeachFoamAmount = profile.m_beachFoamAmount;
                profile.m_HDbeachFoamDistance = profile.m_beachFoamDistance;
                profile.m_HDbeachFoamSpeed = profile.m_beachFoamSpeed;

                Debug.Log("HDRP values copied successfully");
            }
        }

        /// <summary>
        /// Focuses the water material selected
        /// </summary>
        /// <param name="waterName"></param>
        private void FocusWaterMaterial(Object unityObject)
        {
            if (unityObject != null)
            {
                Selection.activeObject = unityObject;
            }
        }

        /// <summary>
        /// Focuses the underwater settings gameobject
        /// </summary>
        private void FocusUnderwaterSettings()
        {
            GameObject underwaterSettings = GameObject.Find("Underwater Effects");
            if (underwaterSettings != null)
            {
                Selection.activeObject = underwaterSettings;
            }
        }

        /// <summary>
        /// Setup the material name list
        /// </summary>
        private bool SetupMaterials(GaiaConstants.EnvironmentRenderer renderPipeline, GaiaSettings gaiaSettings, int profileIndex)
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
        private List<Material> GetMaterials(string path)
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
    }
}