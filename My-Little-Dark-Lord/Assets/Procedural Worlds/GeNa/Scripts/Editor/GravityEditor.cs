using UnityEngine;
using UnityEditor;
using GeNa.Internal;
using PWCommon2;

namespace GeNa
{
    /// <summary>
    /// Gravity editor - not doing much just yet
    /// </summary>

    [CustomEditor(typeof(Gravity))]
    public class GravityEditor : PWEditor
    {
        private Vector2 m_scrollPosition = Vector2.zero;
        private Gravity m_gravity;
        private EditorUtils m_editorUtils;
        
        #region Constructors destructors and related delegates

        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }
        }

        /// <summary>
        /// Called when we select this in the scene
        /// </summary>
        void OnEnable()
        {
            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }

            //Check for target
            if (target == null)
            {
                return;
            }
            //Setup target
            m_gravity = (Gravity) target;
        }

        #endregion

        /// <summary>
        /// Editor UX
        /// </summary>
        public override void OnInspectorGUI()
        {
            //Set the target
            m_gravity = (Gravity) target;
            
            m_editorUtils.Initialize(); // Do not remove this!
            m_editorUtils.GUIHeader(description: "Gravity");

            //Scroll
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, false);

            //Monitor for changes
            EditorGUI.BeginChangeCheck();
            {

                GUILayout.Space(5);

                //Generic high level control

                DrawDefaultInspector();                
            }
            if (EditorGUI.EndChangeCheck())
            {
                //Check for changes, make undo record, make changes and let editor know we are dirty
                Undo.RecordObject(m_gravity, "Made changes");
                EditorUtility.SetDirty(m_gravity);
            }

            //End scroll
            GUILayout.EndScrollView();

            m_editorUtils.GUIFooter();
        }
    }
}