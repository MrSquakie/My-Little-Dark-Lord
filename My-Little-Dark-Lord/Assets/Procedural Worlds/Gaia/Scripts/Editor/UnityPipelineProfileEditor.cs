using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PWCommon2;
using Gaia.Internal;

namespace Gaia.Pipeline
{
    [CustomEditor(typeof(UnityPipelineProfile))]
    public class UnityPipelineProfileEditor : PWEditor
    {
        private GUIStyle m_boxStyle;
        private bool m_showOptions;
        private Color defaultBackground;
        private EditorUtils m_editorUtils;
        private UnityPipelineProfile m_profile;
        private string m_version;

        private void OnEnable()
        {
            //Initialization
            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }

            //Get Gaia Lighting Profile object
            m_profile = (UnityPipelineProfile)target;

            m_version = PWApp.CONF.Version;
        }

        public override void OnInspectorGUI()
        {           
            //Initialization
            m_editorUtils.Initialize(); // Do not remove this!

            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            //Monitor for changes
            EditorGUI.BeginChangeCheck();

            defaultBackground = GUI.backgroundColor;

            EditorGUILayout.LabelField("Profile Version: " + m_version);

            bool enableEditMode = System.IO.Directory.Exists(GaiaUtils.GetAssetPath("Dev Utilities"));
            if (enableEditMode)
            {
                m_profile.m_editorUpdates = EditorGUILayout.ToggleLeft("Use Procedural Worlds Editor Settings", m_profile.m_editorUpdates);
            }
            else
            {
                m_profile.m_editorUpdates = false;
            }

            m_editorUtils.Panel("PipelineVersionSupport", PipelineVersionSettingsEnabled, false);
            if (m_profile.m_editorUpdates)
            {
                m_editorUtils.Panel("ProfileSettings", ProfileSettingsEnabled, false);
            }

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_profile, "Made changes");
                EditorUtility.SetDirty(m_profile);
            }
        }

        private void PipelineVersionSettingsEnabled(bool helpEnabled)
        {
            if (Application.unityVersion.Contains("2019.1"))
            {
                m_editorUtils.Heading("LWRPSupport");
                GUILayout.BeginHorizontal();
                m_editorUtils.Label("MinimumLWRPVersion");
                EditorGUILayout.LabelField(m_profile.m_min2019_1LWVersion);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                m_editorUtils.Label("MaximumLWRPVersion");
                EditorGUILayout.LabelField(m_profile.m_max2019_1LWVersion);
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                m_editorUtils.Heading("HDRPSupport");
                GUILayout.BeginHorizontal();
                m_editorUtils.Label("MinimumHDRPVersion");
                EditorGUILayout.LabelField(m_profile.m_min2019_1HDVersion);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                m_editorUtils.Label("MaximumHDRPVersion");
                EditorGUILayout.LabelField(m_profile.m_max2019_1HDVersion);
                GUILayout.EndHorizontal();
            }

            if (Application.unityVersion.Contains("2019.2"))
            {
                GUILayout.BeginHorizontal();
                m_editorUtils.Label("MinimumLWRPVersion " + m_profile.m_min2019_2LWVersion);
                m_editorUtils.Label("MaximumLWRPVersion " + m_profile.m_max2019_2LWVersion);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                m_editorUtils.Label("MinimumHDRPVersion " + m_profile.m_min2019_2HDVersion);
                m_editorUtils.Label("MaximumHDRPVersion " + m_profile.m_max2019_2HDVersion);
                GUILayout.EndHorizontal();
            }
        }

        private void ProfileSettingsEnabled(bool helpEnabled)
        {
            GUILayout.BeginVertical("Procedural Worlds Editor Settings", m_boxStyle);
            GUILayout.Space(20);

            EditorGUILayout.LabelField("2019.1");
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Minimum LWRP Version ");
            m_profile.m_min2019_1LWVersion = EditorGUILayout.TextArea(m_profile.m_min2019_1LWVersion);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Maximum LWRP Version ");
            m_profile.m_max2019_1LWVersion = EditorGUILayout.TextArea(m_profile.m_max2019_1LWVersion);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Minimum HDRP Version ");
            m_profile.m_min2019_1HDVersion = EditorGUILayout.TextArea(m_profile.m_min2019_1HDVersion);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Maximum HDRP Version ");
            m_profile.m_max2019_1HDVersion = EditorGUILayout.TextArea(m_profile.m_max2019_1HDVersion);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("2019.2");
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Minimum LWRP Version ");
            m_profile.m_min2019_2LWVersion = EditorGUILayout.TextArea(m_profile.m_min2019_2LWVersion);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Maximum LWRP Version ");
            m_profile.m_max2019_2LWVersion = EditorGUILayout.TextArea(m_profile.m_max2019_2LWVersion);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Minimum HDRP Version ");
            m_profile.m_min2019_2HDVersion = EditorGUILayout.TextArea(m_profile.m_min2019_2HDVersion);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Maximum HDRP Version ");
            m_profile.m_max2019_2HDVersion = EditorGUILayout.TextArea(m_profile.m_max2019_2HDVersion);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.BeginVertical("Pipeline Profile Settings", m_boxStyle);
            GUILayout.Space(20);
            DrawDefaultInspector();
            GUILayout.EndVertical();
        }
    }
}