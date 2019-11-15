// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using UnityEditor;
using GeNa.Internal;
using PWCommon2;
using System;

namespace GeNa
{
    /// <summary>
    /// Main Workflow Editor Window
    /// </summary>
    public class PreferencesWindow : EditorWindow, IPWEditor
    {
        private Vector2 m_scrollPosition = Vector2.zero;
        private EditorUtils m_editorUtils;
		
        private TabSet m_mainTabs;
		
        public bool PositionChecked { get; set; }
        
        #region Custom Menu Items
        /// <summary>
        /// 
        /// </summary>
        [MenuItem("Window/" + PWConst.COMMON_MENU + "/GeNa/Preferences...", false, 40)]
        public static void MenuGeNaMainWindow()
        {
            var window = EditorWindow.GetWindow<PreferencesWindow>(false, "GeNa Preferences");
            window.Show();
        }

        #endregion

        #region Constructors destructors and related delegates

        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }
        }

        private void OnEnable()
        {
			minSize = new Vector2(400f, 500f);

			if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }

            Tab[] tabs = new Tab[]
                {
                    new Tab("General Tab", GeneralTab),
                    new Tab("Advanced Tab", AdvancedTab),
                };
            m_mainTabs = new TabSet(m_editorUtils, tabs);
        }

        #endregion

        #region GUI main

        /// <summary>
        /// OnGUI
        /// </summary>
        void OnGUI()
        {
            m_editorUtils.Initialize(); // Do not remove this!
            m_editorUtils.GUIHeader();
            //Scroll
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, false);

			// Add content here
			m_editorUtils.Tabs(m_mainTabs);

			//End scroll
			GUILayout.EndScrollView();
            m_editorUtils.GUIFooter();
        }

        #endregion

        #region General Settings

        /// <summary>
        /// General Settings Tab
        /// </summary>
        private void GeneralTab()
        {
            m_editorUtils.Panel("Undo Panel", UndoPanel, true);
        }

        /// <summary>
        /// Undo Panel
        /// </summary>
        private void UndoPanel(bool helpActive)
        {
            float lw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 220f;

            Preferences.UndoSteps = m_editorUtils.
                IntField("Undo Steps", Preferences.UndoSteps, helpActive);

            TimeSpan span = TimeSpan.FromMinutes(Preferences.UndoPurgeTime);
            string purgeTime = string.Format("{0:00}:{1:00}", (int)span.TotalHours, span.Minutes);
            string newTime = m_editorUtils.TextField("Undo Purge Timing", purgeTime, helpActive);
            if (newTime != purgeTime)
            {
                Preferences.SetUndoPurgeTime(newTime);
            }

            Preferences.UndoGroupingTime = m_editorUtils.
                IntField("Undo Grouping Time", Preferences.UndoGroupingTime, helpActive);

            Preferences.UndoExpiredMessages = m_editorUtils.Toggle("Show Expired Undo Messages", Preferences.UndoExpiredMessages, helpActive);

            EditorGUIUtility.labelWidth = lw;
        }
        #endregion

        #region Advanced Settings

        /// <summary>
        /// Advanced Settings Tab
        /// </summary>
        private void AdvancedTab()
        {
            m_editorUtils.Panel("Adv Spawn Panel", SpawnPanel, true);
        }

        /// <summary>
        /// Spawning Panel
        /// </summary>
        private void SpawnPanel(bool helpActive)
        {
            float lw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 220f;

            Preferences.DefaultSpawnToTarget = m_editorUtils.Toggle("Default Spawn To Target", Preferences.DefaultSpawnToTarget, helpActive);

            EditorGUIUtility.labelWidth = lw;
        } 
        #endregion
    }
}