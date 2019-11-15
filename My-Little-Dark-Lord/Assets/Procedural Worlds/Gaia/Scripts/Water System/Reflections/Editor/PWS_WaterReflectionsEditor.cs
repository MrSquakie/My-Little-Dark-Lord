using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace ProcedualWorlds.WaterSystem.Reflections
{
    /// <summary>
    /// Editor for the PWS_WaterReflections
    /// </summary>
    [System.Serializable]
    [CustomEditor(typeof(PWS_WaterReflections))]
    public class PWS_WaterReflectionsEditor : Editor
    {
        #region InspectorRegion
        /// <summary>
        /// Custom editor for PWS_WaterReflections
        /// </summary
        public override void OnInspectorGUI()
        {
            PWS_WaterReflections WaterReflections = (PWS_WaterReflections)target;
            WaterReflections.m_MSAA = EditorGUILayout.Toggle("Allow MSAA", WaterReflections.m_MSAA);
            WaterReflections.m_HDR = EditorGUILayout.Toggle("Use HDR", WaterReflections.m_HDR);
            //WaterReflections.m_shadowRender = EditorGUILayout.Toggle("Use shadows", WaterReflections.m_shadowRender);
            WaterReflections.m_skyboxOnly = EditorGUILayout.Toggle("Render only skybox", WaterReflections.m_skyboxOnly);
            WaterReflections.m_customReflectionDistance = EditorGUILayout.Toggle("Custom reflection distance", WaterReflections.m_customReflectionDistance);
            if (WaterReflections.m_customReflectionDistance)
            {
                WaterReflections.m_renderDistance = EditorGUILayout.FloatField("Reflection distance", WaterReflections.m_renderDistance);
            }
            WaterReflections.m_clipPlaneOffset= EditorGUILayout.FloatField("Clip plane offset", WaterReflections.m_clipPlaneOffset);
            WaterReflections.m_renderTextureSize = EditorGUILayout.IntField("Render texture size", WaterReflections.m_renderTextureSize);
            // WaterReflections.m_RenderPath = (RenderingPath)EditorGUILayout.EnumPopup(WaterReflections.m_RenderPath);
            EditorGUILayout.LabelField("Layers to Render");
            WaterReflections.m_reflectionLayers = EditorGUILayout.MaskField(InternalEditorUtility.LayerMaskToConcatenatedLayersMask(WaterReflections.m_reflectionLayers), InternalEditorUtility.layers);
            EditorGUILayout.LabelField("Rendering Method");
            WaterReflections.m_RenderUpdate = (PWS_WaterReflections.RenderUpdateMode)EditorGUILayout.EnumPopup(WaterReflections.m_RenderUpdate);
            if (WaterReflections.m_RenderUpdate == PWS_WaterReflections.RenderUpdateMode.Interval)
            {
                WaterReflections.m_updateThreshold = EditorGUILayout.Slider(WaterReflections.m_updateThreshold, 0.01f, 50);
            }
            else
            {
                if (WaterReflections.m_RenderUpdate != PWS_WaterReflections.RenderUpdateMode.OnEnable)
                {
                    WaterReflections.m_useRefreshTime = EditorGUILayout.Toggle("Modify refresh rate", WaterReflections.m_useRefreshTime);
                    if (WaterReflections.m_useRefreshTime)
                    {
                        WaterReflections.m_refreshRate = EditorGUILayout.Slider(WaterReflections.m_refreshRate, 0.01f, 1);
                    }
                }
            }
            if (GUILayout.Button("Update Settings"))
            {
                WaterReflections.Generate();
            }
        }
        #endregion
    }
}
