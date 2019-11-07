// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using UnityEditor;
using GeNa.Internal;
using PWCommon2;

namespace GeNa
{
    /// <summary>
    /// Editor Window
    /// </summary>
    public class GlobalSpawnDialog : EditorWindow, IPWEditor
    {
        private EditorUtils m_editorUtils;
		
        public bool PositionChecked { get; set; }

        [SerializeField] private Spawner m_spawner;
        [SerializeField] private Vector3 m_hitLocation;
        [SerializeField] private Vector3 m_hitNormal;
        [SerializeField] private Transform m_hitTransform;

        // This is used to ensure that the window is used correctly.
        [SerializeField] private bool m_initialized = false;
        [SerializeField] private bool m_okFocused = false;

        private const float BTN_WIDTH = 70f;

        #region Constructors destructors and related delegates

        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }
        }

        void OnEnable()
        {
            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }

            minSize = maxSize = new Vector2(330f, 100f);
        }

        /// <summary>
        /// This custom dialogue needs to be initialized with the Editor that called and the hit info this call belongs to (for the callback).
        /// </summary>
        public void Init(Spawner actingSpawner, RaycastHit hitInfo)
        {
            Init(actingSpawner, hitInfo.point, hitInfo.normal, hitInfo.transform);
        }

        /// <summary>
        /// This custom dialogue needs to be initialized with the Editor that called and the hit info this call belongs to (for the callback).
        /// </summary>
        public void Init(Spawner actingSpawner, Vector3 location, Vector3 normal, Transform transform)
        {
            m_spawner = actingSpawner;
            m_hitLocation = location;
            m_hitNormal = normal;
            m_hitTransform = transform;
            m_initialized = true;
        }

        #endregion

        #region GUI main

        void OnGUI()
        {
            m_editorUtils.Initialize(); // Do not remove this!

            GUILayout.Space(5f);
            m_editorUtils.Label("Are you sure?");

            GUILayout.Space(15f);
            if (m_spawner != null)
            {
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 80f;
                m_spawner.m_globalSpawnJitterPct = m_editorUtils.Slider("Jitter", m_spawner.m_globalSpawnJitterPct * 100f, 0f, 100f) * 0.01f;
                EditorGUIUtility.labelWidth = labelWidth;
            }

            // Need to use this, otherwise the progress bar can rarely cause unidentifiable GUI errors.
            bool doSpawn = false;

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                // Spawn
                GUI.SetNextControlName("Ok button");
                if (m_editorUtils.Button("Ok Btn", GUILayout.Width(BTN_WIDTH)))
                {
                    if (m_initialized && m_spawner != null)
                    {
                        doSpawn = true;
                    }
                    else
                    {
                        Debug.LogErrorFormat("[GeNa]: Global Spawn dialogue unable to proceed ({0}). This is likely due to a coding error. Aborting Global Spawn...",
                            m_initialized == false ? "Not initialized" : "Spawner is " + m_spawner);
                        GUIUtility.hotControl = 0;
                        this.Close();
                    }
                }

                // or close
                if (m_editorUtils.Button("Cancel Btn", GUILayout.Width(BTN_WIDTH)))
                {
                    GUIUtility.hotControl = 0;
                    this.Close();
                }
            }
            GUILayout.EndHorizontal();

            if (!m_okFocused)
            {
                GUI.FocusControl("Ok button");
                m_okFocused = true;
            }

            // Need to use this, otherwise the progress bar can rarely cause unidentifiable GUI errors.
            if (doSpawn)
            {
                m_spawner.GlobalSpawn(m_hitLocation, m_hitNormal, m_hitTransform);
                this.Close();
            }
        }

        #endregion
    }
}