// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEditor;
using UnityEngine;
using PWCommon2;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using GeNa.Internal;
using System;

namespace GeNa
{
    [CustomEditor(typeof(SpawnerPreset))]
    public class PresetEditor : PWEditor
    {
        private readonly HashSet<string> HIDDEN_PROPERTIES = new HashSet<string>(new string[] { "Name", "ActiveOptions", "name", "hideFlags", "NumberOfActiveOptions" });

        private SpawnerPreset m_preset;
        private EditorUtils m_editorUtils;

        private bool m_showActives = true;
        private bool m_showInactives = false;

        List<InactiveItem> m_inactives;

        /// <summary>
        /// OnEnable
        /// </summary>
        public void OnEnable()
        {
            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }

            m_preset = target as SpawnerPreset;
        }

        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }
        }

        /// <summary>
        /// OnInspectorGUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            m_editorUtils.Initialize(); // Do not remove this!

            m_inactives = new List<InactiveItem>();

            m_showActives = m_editorUtils.Panel("Actives Panel", ActivesPanel, m_showActives);

            m_showInactives = m_editorUtils.Panel("Inactives Panel", InactivesPanel, m_showInactives);
        }

        /// <summary>
        /// Panel that displays the active options (that will be applied to the Spawner).
        /// </summary>
        private void ActivesPanel(bool helpEnabled)
        {
            EditorGUI.BeginChangeCheck();
            {
                List<PropertyInfo> propertiesInCategory = new List<PropertyInfo>();
                string lastHeader = null;

                foreach (PropertyInfo propertyInfo in typeof(SpawnerPreset).
                    GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    // Skip non-relevant properties
                    if (HIDDEN_PROPERTIES.Contains(propertyInfo.Name))
                    {
                        continue;
                    }

                    // Handle special header properties: Non-public and starting with CTGRHDR_
                    if (propertyInfo.Name.StartsWith("CTGRHDR_"))
                    {
                        propertiesInCategory = ListCategory(propertiesInCategory, lastHeader);

                        lastHeader = PascalTorWords(propertyInfo.Name.Replace("CTGRHDR_", ""));

                        // Inactive section also needs headers
                        m_inactives.Add(new InactiveItem(lastHeader, true));
                        continue;
                    }

                    // If in active
                    if (m_preset.IsActiveProperty(propertyInfo.Name))
                    {
                        propertiesInCategory.Add(propertyInfo);
                        continue;
                    }

                    m_inactives.Add(new InactiveItem(propertyInfo.Name));
                }
                // List the last category if it has any items
                ListCategory(propertiesInCategory, lastHeader);

                // Display a text if there are no active options
                if (m_preset.NumberOfActiveOptions < 1)
                {
                    m_editorUtils.Label("No Active Options", m_editorUtils.Styles.richLabel);
                }

            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_preset);
            }
        }

        /// <summary>
        /// List an active category if it contains items.
        /// </summary>
        private List<PropertyInfo> ListCategory(List<PropertyInfo> propertiesInCategory, string lastHeader)
        {
            if (string.IsNullOrEmpty(lastHeader) == false && propertiesInCategory.Count > 0)
            {
                GUILayout.Label(lastHeader, EditorStyles.boldLabel);

                foreach (PropertyInfo prop in propertiesInCategory)
                {
                    DisplayActive(prop);
                }
                propertiesInCategory = new List<PropertyInfo>();

            }

            return propertiesInCategory;
        }

        /// <summary>
        /// Panel that displays the inactive options (that will not be applied to the Spawner).
        /// </summary>
        private void InactivesPanel(bool helpEnabled)
        {
            EditorGUI.BeginChangeCheck();
            {
                foreach (InactiveItem item in m_inactives)
                {
                    if (item.IsHeader)
                    {
                        GUILayout.Label(item.Name, EditorStyles.boldLabel);
                    }
                    else
                    {
                        if(GUILayout.Button(PascalTorWords(item.Name), GUI.skin.label))
                        {
                            m_preset.ActivateOption(item.Name);
                        }
                    }
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_preset);
            }
        }

        /// <summary>
        /// Displays an active property and handles updates
        /// </summary>
        /// <param name="propertyInfo"></param>
        protected void DisplayActive(PropertyInfo propertyInfo)
        {
            // Add a delete button to make inactive

            GUILayout.BeginHorizontal();
            {
                if (m_editorUtils.DeleteButtonMini())
                {
                    m_preset.DeactivateOption(propertyInfo.Name);
                }

                var val = propertyInfo.GetValue(m_preset, null);
                bool change = false;

                // Need to do this overcomplicated way so it's also compatible with old Unity that uses < .Net4.6
                if (val is bool)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (bool)val, ref change);
                }
                else if (val is string)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (string)val, ref change);
                }
                else if (val is int)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (int)val, ref change);
                }
                else if (val is float)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (float)val, ref change);
                }
                else if (val is long)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (long)val, ref change);
                }
                else if (val is Constants.SpawnerType)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (Constants.SpawnerType)val, ref change);
                }
                else if (val is Constants.SpawnMode)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (Constants.SpawnMode)val, ref change);
                }
                else if (val is Constants.SpawnRangeShape)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (Constants.SpawnRangeShape)val, ref change);
                }
                else if (val is Constants.LocationAlgorithm)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (Constants.LocationAlgorithm)val, ref change);
                }
                else if (val is Constants.RotationAlgorithm)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (Constants.RotationAlgorithm)val, ref change);
                }
                else if (val is Constants.VirginCheckType)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (Constants.VirginCheckType)val, ref change);
                }
                else if (val is Constants.CriteriaRangeType)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (Constants.CriteriaRangeType)val, ref change);
                }
                else if (val is Constants.MaskType)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (Constants.MaskType)val, ref change);
                }
                else if (val is Constants.ChildSpawnMode)
                {
                    val = FieldForActive(PascalTorWords(propertyInfo.Name), (Constants.ChildSpawnMode)val, ref change);
                }
                else
                {
                    Debug.LogErrorFormat("[GeNa.PresetEditor] No idea what to do with this property: ({0}){1}", 
                        propertyInfo.PropertyType, propertyInfo.Name);
                }

                if (change)
                {
                    propertyInfo.SetValue(m_preset, val, null);
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected bool FieldForActive(string name, bool value, ref bool change)
        {
            bool oldValue = value;
            value = EditorGUILayout.Toggle(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected string FieldForActive(string name, string value, ref bool change)
        {
            string oldValue = value;
            value = EditorGUILayout.TextField(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected int FieldForActive(string name, int value, ref bool change)
        {
            int oldValue = value;
            value = EditorGUILayout.IntField(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected float FieldForActive(string name, float value, ref bool change)
        {
            float oldValue = value;
            value = EditorGUILayout.FloatField(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected long FieldForActive(string name, long value, ref bool change)
        {
            long oldValue = value;
            value = EditorGUILayout.LongField(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected Constants.SpawnerType FieldForActive(string name, Constants.SpawnerType value, ref bool change)
        {
            Constants.SpawnerType oldValue = value;
            value = (Constants.SpawnerType)EditorGUILayout.EnumPopup(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected Constants.SpawnMode FieldForActive(string name, Constants.SpawnMode value, ref bool change)
        {
            Constants.SpawnMode oldValue = value;
            value = (Constants.SpawnMode)EditorGUILayout.EnumPopup(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected Constants.SpawnRangeShape FieldForActive(string name, Constants.SpawnRangeShape value, ref bool change)
        {
            Constants.SpawnRangeShape oldValue = value;
            value = (Constants.SpawnRangeShape)EditorGUILayout.EnumPopup(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected Constants.LocationAlgorithm FieldForActive(string name, Constants.LocationAlgorithm value, ref bool change)
        {
            Constants.LocationAlgorithm oldValue = value;
            value = (Constants.LocationAlgorithm)EditorGUILayout.EnumPopup(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected Constants.RotationAlgorithm FieldForActive(string name, Constants.RotationAlgorithm value, ref bool change)
        {
            Constants.RotationAlgorithm oldValue = value;
            value = (Constants.RotationAlgorithm)EditorGUILayout.EnumPopup(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected Constants.VirginCheckType FieldForActive(string name, Constants.VirginCheckType value, ref bool change)
        {
            Constants.VirginCheckType oldValue = value;
            value = (Constants.VirginCheckType)EditorGUILayout.EnumPopup(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected Constants.CriteriaRangeType FieldForActive(string name, Constants.CriteriaRangeType value, ref bool change)
        {
            Constants.CriteriaRangeType oldValue = value;
            value = (Constants.CriteriaRangeType)EditorGUILayout.EnumPopup(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected Constants.MaskType FieldForActive(string name, Constants.MaskType value, ref bool change)
        {
            Constants.MaskType oldValue = value;
            value = (Constants.MaskType)EditorGUILayout.EnumPopup(name, value);
            change = value != oldValue;
            return value;
        }

        /// <summary>
        /// Field for an active option
        /// </summary>
        protected Constants.ChildSpawnMode FieldForActive(string name, Constants.ChildSpawnMode value, ref bool change)
        {
            Constants.ChildSpawnMode oldValue = value;
            value = (Constants.ChildSpawnMode)EditorGUILayout.EnumPopup(name, value);
            change = value != oldValue;
            return value;
        }


        /// <summary>
        /// Converts Pascal case text to words
        /// </summary>
        protected static string PascalTorWords(string text)
        {
            return Regex.Replace(text, @"([^^])([A-Z])", "$1 $2");
        }

        #region Helper methods

        #endregion


        #region Helper Classes, Structs

        protected struct InactiveItem
        {
            public string Name;
            public bool IsHeader;

            public InactiveItem(string label, bool isHeader)
            {
                Name = label;
                IsHeader = isHeader;
            }

            public InactiveItem(string label)
            {
                Name = label;
                IsHeader = false;
            }
        }

        #endregion
    }
}
