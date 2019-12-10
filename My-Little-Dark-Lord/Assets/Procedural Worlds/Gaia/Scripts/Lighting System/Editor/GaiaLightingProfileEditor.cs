using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using Gaia.Internal;
using PWCommon2;

namespace Gaia
{
    [CustomEditor(typeof(GaiaLightingProfile))]
    public class GaiaLightingProfileEditor : PWEditor
    {
        private EditorUtils m_editorUtils;
        private string m_version;
        private Color defaultBackground;
        private GaiaSettings m_gaiaSettings;
        private GaiaLightingProfile m_profile;
        private GaiaLightingProfileValues m_profileValues;
        private GaiaConstants.EnvironmentRenderer m_renderPipeline;

        private void OnEnable()
        {
            //Get Gaia Lighting Profile object
            m_profile = (GaiaLightingProfile)target;

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
        }

        public override void OnInspectorGUI()
        {
            //Initialization
            m_editorUtils.Initialize(); // Do not remove this!

            //Monitor for changes
            EditorGUI.BeginChangeCheck();

            defaultBackground = GUI.backgroundColor;

            if (m_gaiaSettings == null)
            {
                m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            }

            if (m_renderPipeline != m_gaiaSettings.m_pipelineProfile.m_activePipelineInstalled)
            {
                m_renderPipeline = m_gaiaSettings.m_pipelineProfile.m_activePipelineInstalled;
            }

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
                m_editorUtils.Panel("LightingProfileSettings", LightingProfileSettingsEnabled, false);

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
                    if (m_profile.m_lightingProfile != GaiaConstants.GaiaLightingProfileType.None)
                    {
                        GaiaLighting.GetProfile(m_profile.m_lightingProfile, m_profile, m_gaiaSettings.m_pipelineProfile, m_gaiaSettings.m_pipelineProfile.m_activePipelineInstalled);
                    }

                    GaiaLighting.UpdateAmbientEnvironment();
                }
            }
        }

        private void RealtimeUpdateEnabled(bool helpEnabled)
        {
            m_profile.m_updateInRealtime = m_editorUtils.ToggleLeft("UpdateChangesInRealtime", m_profile.m_updateInRealtime, helpEnabled);
            if (m_profile.m_updateInRealtime)
            {
                EditorGUILayout.HelpBox("Update In Realtime is enabled this will allow profiles to be endited inside the editor and automatically apply changes every frame. This feature can be expensive and should not be left enabled in testing and builds", MessageType.Warning);
            }
            else
            {
                if (m_editorUtils.Button("UpdateToScene"))
                {
                    if (m_profile.m_lightingProfile != GaiaConstants.GaiaLightingProfileType.None)
                    {
                        GaiaLighting.GetProfile(m_profile.m_lightingProfile, m_profile, m_gaiaSettings.m_pipelineProfile, m_gaiaSettings.m_pipelineProfile.m_activePipelineInstalled);
                    }
                }

                m_editorUtils.InlineHelp("UpdateToScene", helpEnabled);

                if (m_editorUtils.Button("UpdateDynamicGI"))
                {
                    GaiaLighting.UpdateAmbientEnvironment();
                }

                m_editorUtils.InlineHelp("UpdateDynamicGI", helpEnabled);
            }
        }

        private void GlobalSettingsEnabled(bool helpEnabled)
        {
            if (PlayerSettings.colorSpace != ColorSpace.Linear)
            {
                GUI.backgroundColor = Color.yellow;
                EditorGUILayout.HelpBox("Gaia lighting looks best in Linear Color Space. Go to Gaia standard tab and press Set Linear Deferred", MessageType.Warning);
            }

            GUI.backgroundColor = defaultBackground;

            m_profile.m_masterSkyboxMaterial = (Material)m_editorUtils.ObjectField("MasterSkyboxMaterial", m_profile.m_masterSkyboxMaterial, typeof(Material), false, GUILayout.Height(16f));
            GUILayout.Space(5);

            m_profile.m_enablePostProcessing = m_editorUtils.ToggleLeft("EnablePostProcessing", m_profile.m_enablePostProcessing);

            if (m_profile.m_enablePostProcessing)
            {
                m_profile.m_hideProcessVolume = m_editorUtils.ToggleLeft("HidePostProcessingVolumesInScene", m_profile.m_hideProcessVolume);
                m_profile.m_antiAliasingMode = (GaiaConstants.GaiaProAntiAliasingMode)EditorGUILayout.EnumPopup("Anti-Aliasing Mode", m_profile.m_antiAliasingMode);
            }

            m_profile.m_parentObjects = m_editorUtils.ToggleLeft("ParentObjectsToGaia", m_profile.m_parentObjects);

            m_profile.m_enableAmbientAudio = m_editorUtils.ToggleLeft("EnableAmbientAudio", m_profile.m_enableAmbientAudio);

            if (m_renderPipeline != GaiaConstants.EnvironmentRenderer.HighDefinition)
            {
                m_profile.m_enableFog = m_editorUtils.ToggleLeft("EnableFog", m_profile.m_enableFog);
            }
        }

        private void LightingProfileSettingsEnabled(bool helpEnabled)
        {
            m_profile.m_lightingProfile = (GaiaConstants.GaiaLightingProfileType)EditorGUILayout.EnumPopup("Time Of Day Profile", m_profile.m_lightingProfile);
            GUILayout.Space(10f);
            //Profile values
            if (m_profileValues == null)
            {
                m_profileValues = GetProfile();
            }
            else if (m_profile.m_lightingProfile != m_profileValues.m_profileType)
            {
                m_profileValues = GetProfile();
            }

            if (m_profile.m_lightingProfile == GaiaConstants.GaiaLightingProfileType.None)
            {
                EditorGUILayout.HelpBox("No Profile selected. Select another profile that is not 'None' to view settings to edit", MessageType.Info);
            }
            else
            {
                if (m_renderPipeline != GaiaConstants.EnvironmentRenderer.HighDefinition)
                {
                    if (m_profile.m_enablePostProcessing)
                    {
                        m_editorUtils.Heading("PostProcessingSettings");
                        if (m_renderPipeline == GaiaConstants.EnvironmentRenderer.HighDefinition)
                        {
                            m_profileValues.m_hDPostProcessingProfile = m_editorUtils.TextField("PostProcessingProfile", m_profileValues.m_hDPostProcessingProfile);
                        }
                        else
                        {
                            m_profileValues.m_postProcessingProfile = m_editorUtils.TextField("PostProcessingProfile", m_profileValues.m_postProcessingProfile);
                            m_profileValues.m_directToCamera = m_editorUtils.Toggle("DirectToCamera", m_profileValues.m_directToCamera);
                        }
                        GUILayout.Space(20f);
                    }

                    if (m_profile.m_enableAmbientAudio)
                    {
                        m_editorUtils.Heading("AmbientAudioSettings");
                        m_profileValues.m_ambientAudio = (AudioClip)m_editorUtils.ObjectField("AmbientAudio", m_profileValues.m_ambientAudio, typeof(AudioClip), false, GUILayout.Height(16f));
                        m_profileValues.m_ambientVolume = m_editorUtils.Slider("AmbientVolume", m_profileValues.m_ambientVolume, 0f, 1f);
                        GUILayout.Space(20f);
                    }

                    m_editorUtils.Heading("SunSettings");
                    m_profileValues.m_sunRotation = m_editorUtils.Slider("SunRotation", m_profileValues.m_sunRotation, 0f, 360f);
                    m_profileValues.m_sunPitch = m_editorUtils.Slider("SunPitch", m_profileValues.m_sunPitch, 0f, 360f);
                    if (m_renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
                    {
                        m_profileValues.m_sunColor = m_editorUtils.ColorField("SunColor", m_profileValues.m_sunColor);
                        m_profileValues.m_sunIntensity = m_editorUtils.FloatField("SunIntensity", m_profileValues.m_sunIntensity);
                    }
                    else
                    {
                        m_profileValues.m_lWSunColor = m_editorUtils.ColorField("SunColor", m_profileValues.m_lWSunColor);
                        m_profileValues.m_lWSunIntensity = m_editorUtils.FloatField("SunIntensity", m_profileValues.m_lWSunIntensity);
                    }
                    GUILayout.Space(20f);
                    m_editorUtils.Heading("ShadowSettings");
                    m_profileValues.m_shadowCastingMode = (LightShadows)EditorGUILayout.EnumPopup("Sun Shadow Casting Mode", m_profileValues.m_shadowCastingMode);
                    m_profileValues.m_shadowStrength = m_editorUtils.Slider("SunShadowStrength", m_profileValues.m_shadowStrength, 0f, 1f);
                    m_profileValues.m_sunShadowResolution = (LightShadowResolution)EditorGUILayout.EnumPopup("Sun Shadow Resolution", m_profileValues.m_sunShadowResolution);
                    GUILayout.Space(20f);
                    m_editorUtils.Heading("SkyboxSettings");
                    if (m_profile.m_lightingProfile == GaiaConstants.GaiaLightingProfileType.Default)
                    {
                        m_profileValues.m_sunSize = m_editorUtils.Slider("SunSize", m_profileValues.m_sunSize, 0f, 1f);
                        m_profileValues.m_sunConvergence = m_editorUtils.Slider("SunConvergence", m_profileValues.m_sunConvergence, 0f, 10f);
                        m_profileValues.m_atmosphereThickness = m_editorUtils.Slider("AtmosphereThickness", m_profileValues.m_atmosphereThickness, 0f, 5f);
                        m_profileValues.m_skyboxExposure = m_editorUtils.Slider("SkyboxExposure", m_profileValues.m_skyboxExposure, 0f, 8f);
                        m_profileValues.m_skyboxTint = m_editorUtils.ColorField("SkyboxTint", m_profileValues.m_skyboxTint);
                        m_profileValues.m_groundColor = m_editorUtils.ColorField("GroundColor", m_profileValues.m_groundColor);
                    }
                    else
                    {
                        m_profileValues.m_skyboxHDRI = (Cubemap)m_editorUtils.ObjectField("HDRISkybox", m_profileValues.m_skyboxHDRI, typeof(Cubemap), false, GUILayout.Height(16f));
                        m_profileValues.m_skyboxTint = m_editorUtils.ColorField("SkyboxTint", m_profileValues.m_skyboxTint);
                        m_profileValues.m_skyboxExposure = m_editorUtils.Slider("SkyboxExposure", m_profileValues.m_skyboxExposure, 0f, 8f);
                    }
                    GUILayout.Space(20f);
                    m_editorUtils.Heading("AmbientSettings");
                    m_profileValues.m_ambientMode = (AmbientMode)EditorGUILayout.EnumPopup("Ambient Mode", m_profileValues.m_ambientMode);
                    if (m_profileValues.m_ambientMode == AmbientMode.Skybox)
                    {
                        m_profileValues.m_ambientIntensity = m_editorUtils.Slider("AmbientIntensity", m_profileValues.m_ambientIntensity, 0f, 10f);
                    }
                    else if (m_profileValues.m_ambientMode == AmbientMode.Flat)
                    {
                        m_profileValues.m_skyAmbient = m_editorUtils.ColorField("SkyAmbient", m_profileValues.m_skyAmbient);
                    }
                    else if (m_profileValues.m_ambientMode == AmbientMode.Trilight)
                    {
                        m_profileValues.m_skyAmbient = m_editorUtils.ColorField("SkyAmbient", m_profileValues.m_skyAmbient);
                        m_profileValues.m_equatorAmbient = m_editorUtils.ColorField("EquatorAmbient", m_profileValues.m_equatorAmbient);
                        m_profileValues.m_groundAmbient = m_editorUtils.ColorField("GroundAmbient", m_profileValues.m_groundAmbient);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Custom Ambient Mode is not recommended.  Select a different mode", MessageType.Warning);
                    }

                    if (m_profile.m_enableFog)
                    {
                        GUILayout.Space(20f);
                        m_editorUtils.Heading("FogSettings");
                        m_profileValues.m_fogMode = (FogMode)EditorGUILayout.EnumPopup("Fog Mode", m_profileValues.m_fogMode);
                        if (m_profileValues.m_fogMode == FogMode.Linear)
                        {
                            m_profileValues.m_fogColor = m_editorUtils.ColorField("FogColor", m_profileValues.m_fogColor);
                            m_profileValues.m_fogStartDistance = m_editorUtils.FloatField("FogStartDistance", m_profileValues.m_fogStartDistance);
                            m_profileValues.m_fogEndDistance = m_editorUtils.FloatField("FogEndDistance", m_profileValues.m_fogEndDistance);
                        }
                        else
                        {
                            m_profileValues.m_fogColor = m_editorUtils.ColorField("FogColor", m_profileValues.m_fogColor);
                            m_profileValues.m_fogDensity = m_editorUtils.Slider("FogDensity", m_profileValues.m_fogDensity, 0f, 1f);
                        }
                    }

                    GUILayout.Space(20f);
                    m_editorUtils.Heading("LightmappingSettings");
                    m_profileValues.m_lightmappingMode = (LightmapEditorSettings.Lightmapper)EditorGUILayout.EnumPopup("Lightmapping Mode", m_profileValues.m_lightmappingMode);
                }
                else
                {
                    if (m_profile.m_enablePostProcessing)
                    {
                        m_editorUtils.Heading("PostProcessingSettings");
                        m_profileValues.m_hDPostProcessingProfile = m_editorUtils.TextField("PostProcessingProfile", m_profileValues.m_hDPostProcessingProfile);
                        GUILayout.Space(20f);
                    }

                    if (m_profile.m_enableAmbientAudio)
                    {
                        EditorGUILayout.LabelField("AmbientAudioSettings");
                        m_profileValues.m_ambientAudio = (AudioClip)m_editorUtils.ObjectField("AmbientAudio", m_profileValues.m_ambientAudio, typeof(AudioClip), false, GUILayout.Height(16f));
                        m_profileValues.m_ambientVolume = m_editorUtils.Slider("AmbientVolume", m_profileValues.m_ambientVolume, 0f, 1f);
                        GUILayout.Space(20f);
                    }

                    m_editorUtils.Heading("SunSettings");
                    m_profileValues.m_sunRotation = m_editorUtils.Slider("SunRotation", m_profileValues.m_sunRotation, 0f, 360f);
                    m_profileValues.m_sunPitch = m_editorUtils.Slider("SunPitch", m_profileValues.m_sunPitch, 0f, 360f);
                    m_profileValues.m_hDSunColor = m_editorUtils.ColorField("SunColor", m_profileValues.m_hDSunColor);
                    m_profileValues.m_hDSunIntensity = m_editorUtils.FloatField("SunIntensity", m_profileValues.m_hDSunIntensity);
                    GUILayout.Space(20f);
                    m_editorUtils.Heading("ShadowSettings");
                    m_profileValues.m_hDShadowDistance = m_editorUtils.Slider("ShadowDistance", m_profileValues.m_hDShadowDistance, 0f, 10000f);
                    GUILayout.Space(20f);
                    m_editorUtils.Heading("SkyboxSettings");
                    m_profileValues.m_hDSkyType = (GaiaConstants.HDSkyType)EditorGUILayout.EnumPopup("Sky Type", m_profileValues.m_hDSkyType);
                    if (m_profileValues.m_hDSkyType == GaiaConstants.HDSkyType.HDRI)
                    {
                        m_profileValues.m_hDHDRISkybox = (Cubemap)m_editorUtils.ObjectField("HDRISkybox", m_profileValues.m_hDHDRISkybox, typeof(Cubemap), false, GUILayout.Height(16f));
                        m_profileValues.m_hDHDRIExposure = m_editorUtils.FloatField("SkyboxExposure", m_profileValues.m_hDHDRIExposure);
                        m_profileValues.m_hDHDRIMultiplier = m_editorUtils.FloatField("SkyboxMultiplier", m_profileValues.m_hDHDRIMultiplier);
                    }
                    else if (m_profileValues.m_hDSkyType == GaiaConstants.HDSkyType.Procedural)
                    {
#if !UNITY_2019_3_OR_NEWER
                        m_profileValues.m_hDProceduralEnableSunDisk = m_editorUtils.Toggle("EnableSunDisk", m_profileValues.m_hDProceduralEnableSunDisk);
                        m_profileValues.m_hDProceduralIncludeSunInBaking = m_editorUtils.Toggle("IncludeSunInBaking", m_profileValues.m_hDProceduralIncludeSunInBaking);
                        m_profileValues.m_hDProceduralSunSize = m_editorUtils.FloatField("SunSize", m_profileValues.m_hDProceduralSunSize);
                        m_profileValues.m_hDProceduralSunSizeConvergence = m_editorUtils.FloatField("SunSizeConvergence", m_profileValues.m_hDProceduralSunSizeConvergence);
                        m_profileValues.m_hDProceduralAtmosphereThickness = m_editorUtils.FloatField("AtmosphereThickness", m_profileValues.m_hDProceduralAtmosphereThickness);
                        m_profileValues.m_hDProceduralSkyTint = m_editorUtils.ColorField("SkyboxTint", m_profileValues.m_hDProceduralSkyTint);
                        m_profileValues.m_hDProceduralGroundColor = m_editorUtils.ColorField("GroundColor", m_profileValues.m_hDProceduralGroundColor);
                        m_profileValues.m_hDProceduralExposure = m_editorUtils.FloatField("Exposure", m_profileValues.m_hDProceduralExposure);
                        m_profileValues.m_hDProceduralMultiplier = m_editorUtils.FloatField("Multiplier", m_profileValues.m_hDProceduralMultiplier);
#else
                        m_profileValues.m_hDPBSPlanetaryRadius = m_editorUtils.FloatField("PBSPlanetaryRadius", m_profileValues.m_hDPBSPlanetaryRadius);
                        m_profileValues.m_hDPBSPlantetCenterPosition = m_editorUtils.Vector3Field("PBSPlanetCenterPosition", m_profileValues.m_hDPBSPlantetCenterPosition, helpEnabled);
                        m_profileValues.m_hDPBSAirAttenuation = m_editorUtils.ColorField("PBSAirAttenuation", m_profileValues.m_hDPBSAirAttenuation);
                        m_profileValues.m_hDPBSAirAlbedo = m_editorUtils.ColorField("PBSAirAlbedo", m_profileValues.m_hDPBSAirAlbedo);
                        m_profileValues.m_hDPBSAirMaximumAltitude = m_editorUtils.FloatField("PBSAirMaximumAltitude", m_profileValues.m_hDPBSAirMaximumAltitude);
                        m_profileValues.m_hDPBSAerosolAttenuatonDistance = m_editorUtils.FloatField("PBSAerosolAttenuationDistance", m_profileValues.m_hDPBSAerosolAttenuatonDistance);
                        m_profileValues.m_hDPBSAerosolAlbedo = m_editorUtils.Slider("PBSAerosolAlbedo", m_profileValues.m_hDPBSAerosolAlbedo, 0f, 1f);
                        m_profileValues.m_hDPBSAerosolMaximumAltitude = m_editorUtils.FloatField("PBSAerosolMaximumAltitude", m_profileValues.m_hDPBSAerosolMaximumAltitude);
                        m_profileValues.m_hDPBSAerosolAnisotropy = m_editorUtils.Slider("PBSAerosolAnisotropy", m_profileValues.m_hDPBSAerosolAnisotropy, -1f, 1f);
                        m_profileValues.m_hDPBSPlanetRotation = m_editorUtils.Vector3Field("PBSPlanetRotation", m_profileValues.m_hDPBSPlanetRotation);
                        m_profileValues.m_hDPBSGroundColor = m_editorUtils.ColorField("PBSGroundColor", m_profileValues.m_hDPBSGroundColor);
                        m_profileValues.m_hDPBSGroundAlbedoTexture = (Cubemap)m_editorUtils.ObjectField("PBSGroundAlbedoTexture", m_profileValues.m_hDPBSGroundAlbedoTexture, typeof(Cubemap), false, GUILayout.Height(16f));
                        m_profileValues.m_hDPBSGroundEmissionTexture = (Cubemap)m_editorUtils.ObjectField("PBSGroundEmissionTexture", m_profileValues.m_hDPBSGroundEmissionTexture, typeof(Cubemap), false, GUILayout.Height(16f));
                        m_profileValues.m_hDPBSSpaceRotation = m_editorUtils.Vector3Field("PBSSpaceRotation", m_profileValues.m_hDPBSSpaceRotation);
                        m_profileValues.m_hDPBSSpaceEmissionTexture = (Cubemap)m_editorUtils.ObjectField("PBSSpaceEmissionTexture", m_profileValues.m_hDPBSSpaceEmissionTexture, typeof(Cubemap), false, GUILayout.Height(16f));
                        m_profileValues.m_hDPBSNumberOfBounces = m_editorUtils.IntSlider("PBSNumberOfBounces", m_profileValues.m_hDPBSNumberOfBounces, 1, 10);
                        m_profileValues.m_hDPBSExposure = m_editorUtils.FloatField("PBSExposure", m_profileValues.m_hDPBSExposure);
                        m_profileValues.m_hDPBSMultiplier = m_editorUtils.FloatField("PBSMultiplier", m_profileValues.m_hDPBSMultiplier);
                        m_profileValues.m_hDPBSIncludeSunInBaking = m_editorUtils.Toggle("PBSIncludeSunInBaking", m_profileValues.m_hDPBSIncludeSunInBaking);
#endif
                    }
                    else
                    {
                        m_profileValues.m_hDGradientTopColor = m_editorUtils.ColorField("TopColor", m_profileValues.m_hDGradientTopColor);
                        m_profileValues.m_hDGradientMiddleColor = m_editorUtils.ColorField("MiddleColor", m_profileValues.m_hDGradientMiddleColor);
                        m_profileValues.m_hDGradientBottomColor = m_editorUtils.ColorField("BottomColor", m_profileValues.m_hDGradientBottomColor);
                        m_profileValues.m_hDGradientDiffusion = m_editorUtils.FloatField("Diffusion", m_profileValues.m_hDGradientDiffusion);
                        m_profileValues.m_hDGradientExposure = m_editorUtils.FloatField("Exposure", m_profileValues.m_hDGradientExposure);
                        m_profileValues.m_hDGradientMultiplier = m_editorUtils.FloatField("Multiplier", m_profileValues.m_hDGradientMultiplier);
                    }
                    GUILayout.Space(20f);
                    m_editorUtils.Heading("AmbientSettings");
                    m_profileValues.m_hDAmbientMode = (GaiaConstants.HDAmbientMode)EditorGUILayout.EnumPopup("Ambient Mode", m_profileValues.m_hDAmbientMode);
#if !UNITY_2019_3_OR_NEWER
                    m_profileValues.m_hDAmbientDiffuseIntensity = m_editorUtils.FloatField("DiffuseIntensity", m_profileValues.m_hDAmbientDiffuseIntensity);
                    m_profileValues.m_hDAmbientSpecularIntensity = m_editorUtils.FloatField("SpecularIntensity", m_profileValues.m_hDAmbientSpecularIntensity);
#else
                    if (m_profileValues.m_hDAmbientMode == GaiaConstants.HDAmbientMode.Static)
                    {
                        EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("HDRP2019_3AmbientMode"), MessageType.Info);
                    }
#endif

                    GUILayout.Space(20f);
                    m_editorUtils.Heading("FogSettings");
#if !UNITY_2019_3_OR_NEWER
                    m_profileValues.m_hDFogType = (GaiaConstants.HDFogType)EditorGUILayout.EnumPopup("Fog Mode", m_profileValues.m_hDFogType);
                    if (m_profileValues.m_hDFogType == GaiaConstants.HDFogType.None)
                    {
                        EditorGUILayout.HelpBox("No fog mode selected. If you want to use fog please select a different mode", MessageType.Info);
                    }
                    else if (m_profileValues.m_hDFogType == GaiaConstants.HDFogType.Exponential)
                    {
                        m_profileValues.m_hDExponentialFogDensity = m_editorUtils.Slider("FogDensity", m_profileValues.m_hDExponentialFogDensity, 0f, 1f);
                        m_profileValues.m_hDExponentialFogDistance = m_editorUtils.FloatField("FogDistance", m_profileValues.m_hDExponentialFogDistance);
                        if (m_profileValues.m_hDExponentialFogDistance < 0f)
                        {
                            m_profileValues.m_hDExponentialFogDistance = 0f;
                        }
                        m_profileValues.m_hDExponentialFogBaseHeight = m_editorUtils.FloatField("FogBaseHeight", m_profileValues.m_hDExponentialFogBaseHeight);
                        if (m_profileValues.m_hDExponentialFogBaseHeight < 0f)
                        {
                            m_profileValues.m_hDExponentialFogBaseHeight = 0f;
                        }
                        m_profileValues.m_hDExponentialFogHeightAttenuation = m_editorUtils.Slider("HeightAttenuation", m_profileValues.m_hDExponentialFogHeightAttenuation, 0f, 1f);
                        m_profileValues.m_hDExponentialFogMaxDistance = m_editorUtils.FloatField("MaxFogDistance", m_profileValues.m_hDExponentialFogMaxDistance);
                        if (m_profileValues.m_hDExponentialFogMaxDistance < 0f)
                        {
                            m_profileValues.m_hDExponentialFogMaxDistance = 0f;
                        }
                    }
                    else if (m_profileValues.m_hDFogType == GaiaConstants.HDFogType.Linear)
                    {
                        m_profileValues.m_hDLinearFogDensity = m_editorUtils.Slider("FogDensity", m_profileValues.m_hDLinearFogDensity, 0f, 1f);
                        m_profileValues.m_hDLinearFogStart = m_editorUtils.FloatField("FogStart", m_profileValues.m_hDLinearFogStart);
                        if (m_profileValues.m_hDLinearFogStart < 0f)
                        {
                            m_profileValues.m_hDLinearFogStart = 0f;
                        }
                        m_profileValues.m_hDLinearFogEnd = m_editorUtils.FloatField("FogEnd", m_profileValues.m_hDLinearFogEnd);
                        if (m_profileValues.m_hDLinearFogEnd < 0f)
                        {
                            m_profileValues.m_hDLinearFogEnd = 0f;
                        }
                        m_profileValues.m_hDLinearFogHeightStart = m_editorUtils.FloatField("FogHeightStart", m_profileValues.m_hDLinearFogHeightStart);
                        if (m_profileValues.m_hDLinearFogHeightStart < 0f)
                        {
                            m_profileValues.m_hDLinearFogHeightStart = 0f;
                        }
                        m_profileValues.m_hDLinearFogHeightEnd = m_editorUtils.FloatField("FogHeightEnd", m_profileValues.m_hDLinearFogHeightEnd);
                        if (m_profileValues.m_hDLinearFogHeightEnd < 0f)
                        {
                            m_profileValues.m_hDLinearFogHeightEnd = 0f;
                        }
                        m_profileValues.m_hDLinearFogMaxDistance = m_editorUtils.FloatField("MaxFogDistance", m_profileValues.m_hDLinearFogMaxDistance);
                        if (m_profileValues.m_hDLinearFogMaxDistance < 0f)
                        {
                            m_profileValues.m_hDLinearFogMaxDistance = 0f;
                        }
                    }
                    else
                    {
                        m_profileValues.m_hDVolumetricFogScatterColor = m_editorUtils.ColorField("ScatterColor", m_profileValues.m_hDVolumetricFogScatterColor);
                        m_profileValues.m_hDVolumetricFogDistance = m_editorUtils.FloatField("FogDistance", m_profileValues.m_hDVolumetricFogDistance);
                        if (m_profileValues.m_hDVolumetricFogDistance < 0f)
                        {
                            m_profileValues.m_hDVolumetricFogDistance = 0f;
                        }
                        m_profileValues.m_hDVolumetricFogBaseHeight = m_editorUtils.FloatField("FogBaseHeight", m_profileValues.m_hDVolumetricFogBaseHeight);
                        if (m_profileValues.m_hDVolumetricFogBaseHeight < 0f)
                        {
                            m_profileValues.m_hDVolumetricFogBaseHeight = 0f;
                        }
                        m_profileValues.m_hDVolumetricFogMeanHeight = m_editorUtils.FloatField("FogMeanHeight", m_profileValues.m_hDVolumetricFogMeanHeight);
                        if (m_profileValues.m_hDVolumetricFogMeanHeight < 0f)
                        {
                            m_profileValues.m_hDVolumetricFogMeanHeight = 0f;
                        }
                        m_profileValues.m_hDVolumetricFogAnisotropy = m_editorUtils.Slider("FogAnisotropy", m_profileValues.m_hDVolumetricFogAnisotropy, 0f, 1f);
                        m_profileValues.m_hDVolumetricFogProbeDimmer = m_editorUtils.Slider("FogProbeDimmer", m_profileValues.m_hDVolumetricFogProbeDimmer, 0f, 1f);
                        m_profileValues.m_hDVolumetricFogMaxDistance = m_editorUtils.FloatField("MaxFogDistance", m_profileValues.m_hDVolumetricFogMaxDistance);
                        if (m_profileValues.m_hDVolumetricFogMaxDistance < 0f)
                        {
                            m_profileValues.m_hDVolumetricFogMaxDistance = 0f;
                        }
                        m_profileValues.m_hDVolumetricFogDepthExtent = m_editorUtils.FloatField("FogDepthExtent", m_profileValues.m_hDVolumetricFogDepthExtent);
                        if (m_profileValues.m_hDVolumetricFogDepthExtent < 0f)
                        {
                            m_profileValues.m_hDVolumetricFogDepthExtent = 0f;
                        }
                        m_profileValues.m_hDVolumetricFogSliceDistribution = m_editorUtils.Slider("FogSliceDistribution", m_profileValues.m_hDVolumetricFogSliceDistribution, 0f, 1f);
                    }
#else
                    m_profileValues.m_hDFogType2019_3 = (GaiaConstants.HDFogType2019_3)EditorGUILayout.EnumPopup("Fog Mode", m_profileValues.m_hDFogType2019_3);
                    if (m_profileValues.m_hDFogType2019_3 == GaiaConstants.HDFogType2019_3.None)
                    {
                        EditorGUILayout.HelpBox("No fog mode selected. If you want to use fog please select a different mode", MessageType.Info);
                    }
                    else
                    {
                        m_profileValues.m_hDVolumetricFogScatterColor = m_editorUtils.ColorField("ScatterColor", m_profileValues.m_hDVolumetricFogScatterColor);
                        m_profileValues.m_hDVolumetricFogDistance = m_editorUtils.FloatField("FogDistance", m_profileValues.m_hDVolumetricFogDistance);
                        if (m_profileValues.m_hDVolumetricFogDistance < 0f)
                        {
                            m_profileValues.m_hDVolumetricFogDistance = 0f;
                        }
                        m_profileValues.m_hDVolumetricFogBaseHeight = m_editorUtils.FloatField("FogBaseHeight", m_profileValues.m_hDVolumetricFogBaseHeight);
                        if (m_profileValues.m_hDVolumetricFogBaseHeight < 0f)
                        {
                            m_profileValues.m_hDVolumetricFogBaseHeight = 0f;
                        }
                        m_profileValues.m_hDVolumetricFogMeanHeight = m_editorUtils.FloatField("FogMeanHeight", m_profileValues.m_hDVolumetricFogMeanHeight);
                        if (m_profileValues.m_hDVolumetricFogMeanHeight < 0f)
                        {
                            m_profileValues.m_hDVolumetricFogMeanHeight = 0f;
                        }
                        m_profileValues.m_hDVolumetricFogAnisotropy = m_editorUtils.Slider("FogAnisotropy", m_profileValues.m_hDVolumetricFogAnisotropy, 0f, 1f);
                        m_profileValues.m_hDVolumetricFogProbeDimmer = m_editorUtils.Slider("FogProbeDimmer", m_profileValues.m_hDVolumetricFogProbeDimmer, 0f, 1f);
                        m_profileValues.m_hDVolumetricFogMaxDistance = m_editorUtils.FloatField("MaxFogDistance", m_profileValues.m_hDVolumetricFogMaxDistance);
                        if (m_profileValues.m_hDVolumetricFogMaxDistance < 0f)
                        {
                            m_profileValues.m_hDVolumetricFogMaxDistance = 0f;
                        }
                        m_profileValues.m_hDVolumetricFogDepthExtent = m_editorUtils.FloatField("FogDepthExtent", m_profileValues.m_hDVolumetricFogDepthExtent);
                        if (m_profileValues.m_hDVolumetricFogDepthExtent < 0f)
                        {
                            m_profileValues.m_hDVolumetricFogDepthExtent = 0f;
                        }
                        m_profileValues.m_hDVolumetricFogSliceDistribution = m_editorUtils.Slider("FogSliceDistribution", m_profileValues.m_hDVolumetricFogSliceDistribution, 0f, 1f);
                    }
#endif

                    GUILayout.Space(20f);
                    m_editorUtils.Heading("LightmappingSettings");
                    m_profileValues.m_lightmappingMode = (LightmapEditorSettings.Lightmapper)EditorGUILayout.EnumPopup("Lightmapping Mode", m_profileValues.m_lightmappingMode);
                }
            }
        }

        /// <summary>
        /// Gets profile
        /// </summary>
        /// <returns></returns>
        private GaiaLightingProfileValues GetProfile()
        {
            foreach (GaiaLightingProfileValues profile in m_profile.m_lightingProfiles)
            {
                if (profile.m_profileType == m_profile.m_lightingProfile)
                {
                    return profile;
                }
            }

            return null;
        }
    }
}