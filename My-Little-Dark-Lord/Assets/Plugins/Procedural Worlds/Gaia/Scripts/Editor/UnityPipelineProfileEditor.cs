using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gaia.Pipeline
{
    [CustomEditor(typeof(UnityPipelineProfile))]
    public class UnityPipelineProfileEditor : Editor
    {
        private GUIStyle m_boxStyle;
        private bool m_showOptions;
        private Color defaultBackground;

        public override void OnInspectorGUI()
        {
            //Initialization

            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            //Get Gaia Lighting Profile object
            UnityPipelineProfile profile = (UnityPipelineProfile)target;

            //Monitor for changes
            EditorGUI.BeginChangeCheck();

            defaultBackground = GUI.backgroundColor;

            GUILayout.BeginVertical("Information Settings", m_boxStyle);
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Minimum LWRP Version " + profile.m_minLWVersion);
            EditorGUILayout.LabelField("Maximum LWRP Version " + profile.m_maxLWVersion);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Minimum HDRP Version " + profile.m_minHDVersion);
            EditorGUILayout.LabelField("Maximum HDRP Version " + profile.m_maxHDVersion);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            bool editorUpdates = profile.m_editorUpdates;
            string minLWVersion = profile.m_minLWVersion;
            string maxLWVersion = profile.m_maxLWVersion;
            string minHDVersion = profile.m_minHDVersion;
            string maxHDVersion = profile.m_maxHDVersion;
            if (editorUpdates)
            {
                GUILayout.BeginVertical("Procedural Worlds Editor Settings", m_boxStyle);
                GUILayout.Space(20);

                editorUpdates = EditorGUILayout.ToggleLeft("Use Procedural Worlds Editor Settings", editorUpdates);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Minimum LWRP Version ");
                minLWVersion = EditorGUILayout.TextArea(minLWVersion);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Maximum LWRP Version ");
                maxLWVersion = EditorGUILayout.TextArea(maxLWVersion);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Minimum HDRP Version ");
                minHDVersion = EditorGUILayout.TextArea(minHDVersion);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Maximum HDRP Version ");
                maxHDVersion = EditorGUILayout.TextArea(maxHDVersion);
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            GUILayout.BeginVertical("Pipeline Profile Settings", m_boxStyle);
            GUILayout.Space(20);
            DrawDefaultInspector();
            GUILayout.EndVertical();

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(profile, "Made changes");
                EditorUtility.SetDirty(profile);

                profile.m_editorUpdates = editorUpdates;
                profile.m_minLWVersion = minLWVersion;
                profile.m_maxLWVersion = maxLWVersion;
                profile.m_minHDVersion = minHDVersion;
                profile.m_maxHDVersion = maxHDVersion;
            }
        }
    }
}