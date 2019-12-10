using PWCommon2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Gaia
{
    //This class is not a full editor class by itself, but used to collect reusable methods
    //for editing Spawner Settings in a reorderable list.
    public class SpawnerPresetListEditor : PWEditor, IPWEditor
    {

        public static float OnElementHeight()
        {
          return EditorGUIUtility.singleLineHeight;
        }

        public static List<BiomeSpawnerListEntry> OnRemoveListEntry(List<BiomeSpawnerListEntry> oldList, int index)
        {
            //if (index < 0 || index >= oldList.Length)
            //    return null;
            //SpawnerPresetListEntry toRemove = oldList[index];
            //SpawnerPresetListEntry[] newList = new SpawnerPresetListEntry[oldList.Length - 1];
            //for (int i = 0; i < newList.Length; ++i)
            //{
            //    if (i < index)
            //    {
            //        newList[i] = oldList[i];
            //    }
            //    else if (i >= index)
            //    {
            //        newList[i] = oldList[i + 1];
            //    }
            //}
            oldList.RemoveAt(index);
            return oldList;
        }

        public static List<BiomeSpawnerListEntry> OnAddListEntry(List<BiomeSpawnerListEntry> oldList)
        {
            oldList.Add(new BiomeSpawnerListEntry() { m_autoAssignPrototypes = true });
            return oldList;
        }

        public static void DrawListHeader(Rect rect, bool currentFoldOutState, List<BiomeSpawnerListEntry> spawnerList, EditorUtils editorUtils, string headerKey)
        {
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            //rect.xMin += 0f;
            EditorGUI.LabelField(rect,editorUtils.GetContent(headerKey));
            //bool newFoldOutState = EditorGUI.Foldout(rect, currentFoldOutState, PropertyCount("SpawnerAdded", spawnerList, editorUtils), true);
            EditorGUI.indentLevel = oldIndent;
            //return newFoldOutState;
        }

        public static void DrawList(ReorderableList list, EditorUtils editorUtils)
        {
            Rect maskRect;
            //if (listExpanded)
            //{
                maskRect = EditorGUILayout.GetControlRect(true, list.GetHeight());
                list.DoList(maskRect);
            //}
            //else
            //{
            //    int oldIndent = EditorGUI.indentLevel;
            //    EditorGUI.indentLevel = 1;
            //    listExpanded = EditorGUILayout.Foldout(listExpanded, PropertyCount("SpawnerAdded", (List<SpawnerPresetListEntry>)list.list, editorUtils), true);
            //    maskRect = GUILayoutUtility.GetLastRect();
            //    EditorGUI.indentLevel = oldIndent;
            //}

            //editorUtils.Panel("MaskBaking", DrawMaskBaking, false);
        }

        public static void DrawListElement_GaiaManager(Rect rect, BiomeSpawnerListEntry listEntry, EditorUtils m_editorUtils)
        {
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 110f, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("SpawnerListAutoAssignPrototypes"));
            listEntry.m_autoAssignPrototypes = EditorGUI.Toggle(new Rect(rect.x + 110f, rect.y,20f, EditorGUIUtility.singleLineHeight), listEntry.m_autoAssignPrototypes);
            listEntry.m_spawnerSettings = (SpawnerSettings)EditorGUI.ObjectField(new Rect(rect.x + 150f, rect.y, rect.width -150f, EditorGUIUtility.singleLineHeight), listEntry.m_spawnerSettings, typeof(SpawnerSettings), false);
            EditorGUI.indentLevel = oldIndent;
        }

        public static void DrawListElement_AdvancedTab(Rect rect, BiomeSpawnerListEntry listEntry, EditorUtils m_editorUtils)
        {
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect labelRect = rect;
            //260 is the total width of the controls at the end of each row
            labelRect.width = rect.width - 280;
            labelRect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(labelRect, listEntry.m_spawnerSettings.name);

            EditorGUIUtility.AddCursorRect(labelRect, MouseCursor.Zoom);
            if (labelRect.Contains(Event.current.mousePosition) && Event.current.clickCount > 0)
            {
                Selection.activeObject = listEntry.m_spawnerSettings;
                EditorGUIUtility.PingObject(Selection.activeObject);
            }

            labelRect.x = rect.width - 230;
            labelRect.width = 110f;

            EditorGUI.LabelField(labelRect, m_editorUtils.GetContent("SpawnerListAutoAssignPrototypes"));

            labelRect.x = rect.width - 130f;
            labelRect.width = 20f;

            listEntry.m_autoAssignPrototypes = EditorGUI.Toggle(labelRect, listEntry.m_autoAssignPrototypes);

            labelRect.x = rect.width - 80f;
            labelRect.width = 100f;
            if (GUI.Button(labelRect, m_editorUtils.GetContent("AdvancedTabAddSpawner")))
            {
                Spawner newSpawner = listEntry.m_spawnerSettings.CreateSpawner(listEntry.m_autoAssignPrototypes);
                Selection.activeGameObject = newSpawner.gameObject;
            }


            EditorGUI.indentLevel = oldIndent;
        }

        public static void DrawListElement(Rect rect, BiomeSpawnerListEntry listEntry, EditorUtils m_editorUtils)
        {
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            listEntry.m_spawnerSettings = (SpawnerSettings)EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), listEntry.m_spawnerSettings, typeof(SpawnerSettings), false);
            EditorGUI.indentLevel = oldIndent;
        }

        public static GUIContent PropertyCount(string key, List<BiomeSpawnerListEntry> list, EditorUtils editorUtils)
        {
            GUIContent content = editorUtils.GetContent(key);
            content.text += " [" + list.Count + "]";
            return content;
        }


    }
}