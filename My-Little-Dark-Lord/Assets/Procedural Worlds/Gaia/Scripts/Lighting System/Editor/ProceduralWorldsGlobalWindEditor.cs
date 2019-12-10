using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PWCommon2;
using Gaia.Internal;

namespace Gaia
{
    [CustomEditor(typeof(ProceduralWorldsGlobalWind))]
    public class ProceduralWorldsGlobalWindEditor : PWEditor
    {
        private EditorUtils m_editorUtils;
        private string m_version;
        private ProceduralWorldsGlobalWind m_globalWind;
        private float m_globalWindValue;

        private void OnEnable()
        {
            //Get Gaia Lighting Profile object
            m_globalWind = (ProceduralWorldsGlobalWind)target;
            //Get version of Gaia application
            m_version = PWApp.CONF.Version;

            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }
        }

        public override void OnInspectorGUI()
        {
            //Initialization
            m_editorUtils.Initialize(); // Do not remove this!

            //Monitor for changes
            EditorGUI.BeginChangeCheck();

            m_editorUtils.Panel("GlobalSettings", GlobalSettingsEnabled, false);
            m_editorUtils.Panel("WindSettings", WindSettingsEnabled, false);

            UpdateWindSettings(m_globalWind.m_windType);

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_globalWind, "Made changes");
                EditorUtility.SetDirty(m_globalWind);

                m_globalWind.UpdateWindSettings(m_globalWind.m_windZone);
            }
        }

        private void GlobalSettingsEnabled(bool helpEnabled)
        {
            m_globalWind.m_windZone = (WindZone)m_editorUtils.ObjectField("WindZoneObject", m_globalWind.m_windZone, typeof(WindZone), false, helpEnabled, GUILayout.Height(16f));
            m_globalWind.m_windType = (GaiaConstants.GaiaGlobalWindType)EditorGUILayout.EnumPopup("Wind Mode", m_globalWind.m_windType);
            m_editorUtils.InlineHelp("WindMode", helpEnabled);

            //m_globalWind.m_overwriteWaveSpeed = m_editorUtils.ToggleLeft("WaterWaveSpeed", m_globalWind.m_overwriteWaveSpeed, helpEnabled);
        }

        private void WindSettingsEnabled(bool helpEnabled)
        {
            EditorGUILayout.HelpBox("Go to global settings to change your Wind Mode to update your wind settings", MessageType.Info);
            if (m_globalWind.m_windType == GaiaConstants.GaiaGlobalWindType.Custom)
            {
                m_globalWind.m_windSpeed = m_editorUtils.Slider("WindSpeed", m_globalWind.m_windSpeed, 0f, 5f, helpEnabled);
                m_globalWind.m_windTurbulence = m_editorUtils.Slider("WindTurbulence", m_globalWind.m_windTurbulence, 0f, 5f, helpEnabled);
                m_globalWind.m_windFrequency = m_editorUtils.Slider("WindFrequency", m_globalWind.m_windFrequency, 0f, 5f, helpEnabled);
                m_globalWindValue = Mathf.Clamp(m_globalWind.m_windSpeed + m_globalWind.m_windTurbulence * m_globalWind.m_windFrequency, 0f, 5f);
                if (m_globalWindValue >= 5f)
                {
                    EditorGUILayout.HelpBox("Maximum global wind speed reached.", MessageType.Warning);
                }
            }
            else
            {
                EditorGUILayout.LabelField("Wind Speed: " + m_globalWind.m_windSpeed.ToString());
                EditorGUILayout.LabelField("Wind Turbulence: " + m_globalWind.m_windTurbulence.ToString());
                EditorGUILayout.LabelField("Wind Frequency: " + m_globalWind.m_windTurbulence.ToString());
            }

            if (m_globalWind.m_windType == GaiaConstants.GaiaGlobalWindType.Custom)
            {
                EditorGUILayout.LabelField("Global Wind Value Is: " + m_globalWindValue.ToString());
                m_editorUtils.InlineHelp("GlobalWindValue", helpEnabled);
            }

            EditorGUILayout.Space();
            m_editorUtils.InlineHelp("WindSetupHelp", helpEnabled);
        }

        private void UpdateWindSettings(GaiaConstants.GaiaGlobalWindType windType)
        {
            switch (windType)
            {
                case GaiaConstants.GaiaGlobalWindType.Calm:
                    m_globalWind.m_windSpeed = 0.2f;
                    m_globalWind.m_windTurbulence = 0.2f;
                    m_globalWind.m_windFrequency = 0.05f;
                    break;
                case GaiaConstants.GaiaGlobalWindType.Moderate:
                    m_globalWind.m_windSpeed = 0.45f;
                    m_globalWind.m_windTurbulence = 0.4f;
                    m_globalWind.m_windFrequency = 0.2f;
                    break;
                case GaiaConstants.GaiaGlobalWindType.Strong:
                    m_globalWind.m_windSpeed = 1f;
                    m_globalWind.m_windTurbulence = 0.8f;
                    m_globalWind.m_windFrequency = 0.5f;
                    break;
                case GaiaConstants.GaiaGlobalWindType.None:
                    m_globalWind.m_windSpeed = 0f;
                    m_globalWind.m_windTurbulence = 0f;
                    m_globalWind.m_windFrequency = 0f;
                    break;
            }
        }
    }
}