﻿using PWCommon2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Gaia
{
    //This class is not a full editor class by itself, but used to collect reusable methods
    //for editing collision masks in a reorderable list.
    public class CollisionMaskListEditor : PWEditor, IPWEditor
    {

        

        public static float OnElementHeight(int index, CollisionMask[] collisionMasks)
        {
            return EditorGUIUtility.singleLineHeight ;
        }

        public static CollisionMask[] OnRemoveMaskListEntry(CollisionMask[] oldList, int index)
        {
            if (index < 0 || index >= oldList.Length)
                return null;
            CollisionMask toRemove = oldList[index];
            CollisionMask[] newList = new CollisionMask[oldList.Length - 1];
            for (int i = 0; i < newList.Length; ++i)
            {
                if (i < index)
                {
                    newList[i] = oldList[i];
                }
                else if (i >= index)
                {
                    newList[i] = oldList[i + 1];
                }
            }
            return newList;
        }

        public static CollisionMask[] OnAddMaskListEntry(CollisionMask[] oldList)
        {
            CollisionMask[] newList = new CollisionMask[oldList.Length + 1];
            for (int i = 0; i < oldList.Length; ++i)
            {
                newList[i] = oldList[i];
            }
            newList[newList.Length - 1] = new CollisionMask();
            return newList;
        }

        public static bool DrawFilterListHeader(Rect rect, bool currentFoldOutState, CollisionMask[] maskList, EditorUtils editorUtils)
        {
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            rect.xMin += 8f;
            bool newFoldOutState = EditorGUI.Foldout(rect, currentFoldOutState, PropertyCount("MaskRespectedCollisions", maskList, editorUtils), true);
            EditorGUI.indentLevel = oldIndent;
            return newFoldOutState;
        }

        public static void DrawMaskList(ref bool listExpanded, ReorderableList list, EditorUtils editorUtils, Rect rect)
        {
            Rect maskRect;
            if (listExpanded)
            {
                maskRect = rect;
                list.DoList(maskRect);
            }
            else
            {
                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 1;
                listExpanded = EditorGUI.Foldout(rect,listExpanded, PropertyCount("MaskRespectedCollisions", (CollisionMask[])list.list, editorUtils), true);
                maskRect = rect;
                EditorGUI.indentLevel = oldIndent;
            }

            //editorUtils.Panel("MaskBaking", DrawMaskBaking, false);
        }

        public static void DrawMaskListElement(Rect rect, int index, CollisionMask collisionMask, EditorUtils m_editorUtils, Terrain currentTerrain, GaiaConstants.FeatureOperation operation)
        {
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            //Active Label
            Rect fieldRect = new Rect(rect.x, rect.y, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(fieldRect, m_editorUtils.GetContent("MaskActive"));
            //Active Checkbox
            fieldRect.x += rect.width * 0.1f;
            fieldRect.width = rect.width * 0.05f;
            collisionMask.m_active = EditorGUI.Toggle(fieldRect, collisionMask.m_active);
            //Invert Label
            fieldRect.x += rect.width * 0.05f;
            fieldRect.width = rect.width * 0.1f;
            EditorGUI.LabelField(fieldRect, m_editorUtils.GetContent("MaskInvert"));
            //Invert Checkbox
            fieldRect.x += rect.width * 0.1f;
            fieldRect.width = rect.width * 0.05f;
            collisionMask.m_invert = EditorGUI.Toggle(fieldRect, collisionMask.m_invert);
            //Type dropdown
            fieldRect.x += rect.width * 0.05f;
            fieldRect.width = rect.width * 0.2f;
            CollisionMaskType oldType = collisionMask.m_type;
            collisionMask.m_type = (CollisionMaskType)EditorGUI.EnumPopup(fieldRect, collisionMask.m_type);
            switch (collisionMask.m_type)
            {
                case CollisionMaskType.TerrainTree:
                    //Tree dropdown
                    int oldTreeId = collisionMask.m_treePrototypeId;
                    fieldRect.x += rect.width * 0.2f;
                    //Building up a value array of incrementing ints of the size of the tree prototype array, this array will then match the displayed string selection in the popup
                    int[] prototypeValueArray = Enumerable
                                        .Repeat(0, (int)((currentTerrain.terrainData.treePrototypes.Length - 0) / 1) + 1)
                                        .Select((tr, ti) => tr + (1 * ti))
                                        .ToArray();
                    collisionMask.m_treePrototypeId = EditorGUI.IntPopup(fieldRect, collisionMask.m_treePrototypeId, currentTerrain.terrainData.treePrototypes.Select(x=>x.prefab.name).ToArray(), prototypeValueArray);

                    if (oldType != collisionMask.m_type || oldTreeId != collisionMask.m_treePrototypeId)
                    {
                        collisionMask.m_Radius = GaiaUtils.GetTreeRadius(currentTerrain, collisionMask.m_treePrototypeId);
                    }

                    fieldRect.x += rect.width * 0.2f;
                    fieldRect.width = rect.width * 0.1f;
                    collisionMask.m_Radius = EditorGUI.FloatField(fieldRect, collisionMask.m_Radius);
                    fieldRect.x += rect.width * 0.1f;
                    fieldRect.width = rect.width * 0.2f;
                    break;
                case CollisionMaskType.Tag:
                    //Tree dropdown
                    fieldRect.x += rect.width * 0.2f;
                    ////Building up a value array of incrementing ints of the size of the available tags this array will then match the displayed string selection in the popup
                    //int[] tagValueArray = Enumerable
                    //                    .Repeat(0, (int)((currentTerrain.terrainData.treePrototypes.Length - 0) / 1) + 1)
                    //                    .Select((tr, ti) => tr + (1 * ti))
                    //                    .ToArray();
                    string oldTag = collisionMask.m_tag;
                    collisionMask.m_tag = EditorGUI.TagField(fieldRect, collisionMask.m_tag);

                    if (oldType != collisionMask.m_type || oldTag != collisionMask.m_tag)
                    {
                        collisionMask.m_Radius = GaiaUtils.GetBoundsForTaggedObject(collisionMask.m_tag);
                    }


                    fieldRect.x += rect.width * 0.2f;
                    fieldRect.width = rect.width * 0.1f;
                    collisionMask.m_Radius = EditorGUI.FloatField(fieldRect, collisionMask.m_Radius);
                    fieldRect.x += rect.width * 0.1f;
                    fieldRect.width = rect.width * 0.2f;
                    break;
                //case CollisionMaskType.Layer:
                //    //Tree dropdown
                //    fieldRect.x += rect.width * 0.2f;
                //    ////Building up a value array of incrementing ints of the size of the available tags this array will then match the displayed string selection in the popup
                //    //int[] tagValueArray = Enumerable
                //    //                    .Repeat(0, (int)((currentTerrain.terrainData.treePrototypes.Length - 0) / 1) + 1)
                //    //                    .Select((tr, ti) => tr + (1 * ti))
                //    //                    .ToArray();
                //    string oldLayer = collisionMask.m_layer;
                //    collisionMask.m_layer = EditorGUI.TextField(fieldRect, collisionMask.m_layer);

                //    if (oldType != collisionMask.m_type || oldLayer != collisionMask.m_layer)
                //    {
                //        collisionMask.m_Radius = 10f;
                //    }


                //    fieldRect.x += rect.width * 0.2f;
                //    fieldRect.width = rect.width * 0.1f;
                //    collisionMask.m_Radius = EditorGUI.FloatField(fieldRect, collisionMask.m_Radius);
                //    fieldRect.x += rect.width * 0.1f;
                //    fieldRect.width = rect.width * 0.2f;
                //    break;

            }

            if (GUI.Button(fieldRect, m_editorUtils.GetContent("MaskCollisionBake")))
            {
                switch (collisionMask.m_type)
                {
                    case CollisionMaskType.TerrainTree:
                        GaiaSessionManager.GetSessionManager().m_collisionMaskCache.BakeAllTreeCollisions(collisionMask.m_treePrototypeId, collisionMask.m_Radius);
                        break;
                    case CollisionMaskType.Tag:
                        GaiaSessionManager.GetSessionManager().m_collisionMaskCache.BakeAllTagCollisions(collisionMask.m_tag, collisionMask.m_Radius);
                        break;
                }
                

            }


            EditorGUI.indentLevel = oldIndent;
        }

        public static GUIContent PropertyCount(string key, Array array, EditorUtils editorUtils)
        {
            GUIContent content = editorUtils.GetContent(key);
            content.text += " [" + array.Length + "]";
            return content;
        }



    }
}