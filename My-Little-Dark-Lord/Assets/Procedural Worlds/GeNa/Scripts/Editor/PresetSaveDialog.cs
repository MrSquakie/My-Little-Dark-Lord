// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using UnityEditor;
using GeNa.Internal;
using PWCommon2;
using System.IO;
using System.Collections.Generic;

namespace GeNa
{
    /// <summary>
    /// Editor Window
    /// </summary>
    public class PresetSaveDialog : EditorWindow, IPWEditor
    {
        private EditorUtils m_editorUtils;
		
        public bool PositionChecked { get; set; }

        [SerializeField] private SpawnerEditor m_spawnerEditor;
        [SerializeField] private bool m_saveFocused = false;

        // Older Unity messes with brackets, so need to name built-in without
        public const string BUILTIN_SIGNATURE = "Built-in";

        private const float BTN_WIDTH = 70f;

        [SerializeField] private string m_targetPresetName;

        [SerializeField] private List<IMsg> m_msgs = new List<IMsg>();

        private Vector2 m_scrollPos = Vector2.zero;

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

            minSize = maxSize = new Vector2(420f, 335f);
        }

        /// <summary>
        /// This custom dialogue needs to be initialized with the Editor that called.
        /// </summary>
        public void Init(SpawnerEditor spawnerEditor)
        {
            m_spawnerEditor = spawnerEditor;
        }

        #endregion

        #region GUI main

        void OnGUI()
        {
            m_editorUtils.Initialize(); // Do not remove this!

            List<SpawnerPreset> presetList = new List<SpawnerPreset>(SpawnerPreset.AvailablePresets);
            List<string> presetNameList = new List<string>();
            List<string> userPresetNameList = new List<string>();
            for (int i = 0; i < presetList.Count; i++)
            {
                presetNameList.Add(presetList[i].ToString());
                if (presetList[i].ToString().Contains(BUILTIN_SIGNATURE) == false)
                {
                    userPresetNameList.Add(presetList[i].ToString());
                }
            }

            m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);
            {
                GUILayout.Space(5f);
                m_editorUtils.Label("Select preset", m_editorUtils.Styles.body);

                GUILayout.Space(5f);

                int indexOfTarget = userPresetNameList.IndexOf(m_targetPresetName);
                int selected = EditorGUILayout.Popup(indexOfTarget, userPresetNameList.ToArray());
                if (selected != indexOfTarget)
                {
                    m_targetPresetName = userPresetNameList[selected];
                }

                m_editorUtils.Label("or type name", m_editorUtils.Styles.body);

                m_targetPresetName = EditorGUILayout.TextField(m_targetPresetName);

                GUILayout.FlexibleSpace();
                DisplayMessages();
            }
            EditorGUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                // Save
                GUI.SetNextControlName("Save button");
                if (m_editorUtils.Button("Save Btn", GUILayout.Width(BTN_WIDTH)))
                {
                    Save(presetList, presetNameList);
                }

                GUILayout.Space(5f);
                // or close
                if (m_editorUtils.Button("Close Btn", GUILayout.Width(BTN_WIDTH)))
                {
                    GUIUtility.hotControl = 0;
                    this.Close();
                }
            }
            GUILayout.EndHorizontal();

            if (!m_saveFocused)
            {
                GUI.FocusControl("Save button");
                m_saveFocused = true;
            }
        }

        /// <summary>
        /// Display messages if any by type
        /// </summary>
        private void DisplayMessages()
        {
            foreach (IMsg msg in m_msgs)
            {
                EditorGUILayout.HelpBox(msg.Text, msg.Type);
            }
        }

        /// <summary>
        /// Save preset
        /// </summary>
        private void Save(List<SpawnerPreset> presetList, List<string> presetNameList)
        {
            // Clear messages first
            m_msgs.Clear();

            if (m_spawnerEditor == null)
            {
                NotInited();
                m_msgs.Add(new Msg(m_editorUtils.GetTextValue("Not saved Message")));
                return;
            }

            if (presetNameList.Contains(m_targetPresetName))
            {
                // Existing preset - display the overwrite dialog and hook back to Spawner Editor if the user select "don't show again for the file in this session"                
                if (m_spawnerEditor.ShouldOverwritePreset(m_targetPresetName))
                {
                    m_spawnerEditor.SaveToPreset(presetList[presetNameList.IndexOf(m_targetPresetName)]);
                    m_msgs.Add(new Msg(m_editorUtils.GetTextValue("Overwritten Message")));
                }
                else
                {
                    // Saving cancelled
                    return;
                }
            }
            else
            {
                string presetsFolderPath = GetPresetFolder();

                if (string.IsNullOrEmpty(presetsFolderPath))
                {
                    m_msgs.Add(new Err(m_editorUtils.GetTextValue("Unable to locate/create Presets folder")));
                    return;
                }

                SpawnerPreset preset = ScriptableObject.CreateInstance<SpawnerPreset>();
                m_spawnerEditor.SaveToPreset(preset);
                AssetDatabase.CreateAsset(preset, presetsFolderPath + "/" + m_targetPresetName + ".asset");

                m_msgs.Add(new Msg(string.Format(m_editorUtils.GetTextValue("Saved Message"), presetsFolderPath + "/" + m_targetPresetName)));
                m_msgs.Add(new Warn(string.Format(m_editorUtils.GetTextValue("Note new preset does nothing"))));

                EditorGUIUtility.PingObject(preset);
                Selection.activeObject = preset;
            }

            m_spawnerEditor.SetTargetPreset(m_targetPresetName);
        }

        /// <summary>
        /// Throw an error because the dialog was not inited.
        /// </summary>
        private void NotInited()
        {
            m_msgs.Add(new Err(m_editorUtils.GetTextValue("Not inited Error")));
        }

        private string GetPresetFolder()
        {
            string folderPath = null;

            // Get the preset folder (that is not the built-in GeNa folder)
            foreach (string path in AssetDatabase.GetAllAssetPaths())
            {
                // If ends as a preset folder should and is not the GeNa builtin one
                if (path.EndsWith("Resources/" + SpawnerPreset.PRESETS_FOLDER) && !path.Contains(string.
                    Format("/Procedural Worlds/{0}/{1}/Resources/{2}", PWApp.CONF.Folder, PWApp.CONF.ScriptsFolder, SpawnerPreset.PRESETS_FOLDER)))
                {
                    folderPath = path;
                    break;
                }
            }

            // None found. Let's create one in the project root and inform the user
            if (string.IsNullOrEmpty(folderPath))
            {
                string parent = "Assets/Resources";
                if (!Directory.Exists(parent))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                AssetDatabase.CreateFolder(parent, SpawnerPreset.PRESETS_FOLDER);
                folderPath = parent + "/" + SpawnerPreset.PRESETS_FOLDER;

                if (!Directory.Exists(folderPath))
                {
                    return null;
                }

                m_msgs.Add(new Msg(string.Format(m_editorUtils.GetTextValue("Created presets folder Message"), folderPath)));
                m_msgs.Add(new Warn(string.Format(m_editorUtils.GetTextValue("New folder note"))));
            }

            return folderPath;
        }

        #endregion
        
        /// <summary>
        /// Messages to the user
        /// </summary>
        private interface IMsg
        {
            string Text { get; }
            MessageType Type { get; }

        }

        private struct Msg : IMsg
        {
            public string Text { get; private set; }
            public MessageType Type { get; private set; }

            public Msg(string text)
            {
                Text = text;
                Type = MessageType.Info;
            }
        }

        private struct Warn : IMsg
        {
            public string Text { get; private set; }
            public MessageType Type { get; private set; }

            public Warn(string text)
            {
                Text = text;
                Type = MessageType.Warning;
            }
        }

        private struct Err : IMsg
        {
            public string Text { get; private set; }
            public MessageType Type { get; private set; }

            public Err(string text)
            {
                Text = text;
                Type = MessageType.Error;
            }
        }
    }
}