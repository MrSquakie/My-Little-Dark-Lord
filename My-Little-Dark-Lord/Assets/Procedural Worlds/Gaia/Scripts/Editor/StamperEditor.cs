using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine.Experimental.TerrainAPI;
using PWCommon2;
using Gaia.Internal;
using UnityEditorInternal;
//using PWCommon;

namespace Gaia
{
    [CustomEditor(typeof(Stamper))]
    public class StamperEditor : PWEditor, IPWEditor
    {
        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        Stamper m_stamper;
        float m_minX = -2000f;
        float m_maxX = 2000f;
        float m_minY = -2000f;
        float m_maxY = 2000f;
        float m_minZ = -2000f;
        float m_maxZ = 2000f;
        DateTime m_timeSinceLastUpdate = DateTime.Now;
        bool m_startedUpdates = false;
        private bool m_showTooltips = true;
        //private EditorUtilsOLD m_editorUtils = new EditorUtilsOLD();
        private EditorUtils m_editorUtils;

        private GaiaSettings m_gaiaSettings;

        private bool m_heightUpdateRequested;
        private GUIStyle m_imageMaskHeader;
        private List<Texture2D> m_tempTextureList = new List<Texture2D>();

        #region Stamper Settings



        private GaiaConstants.FeatureOperation operation = GaiaConstants.FeatureOperation.RaiseHeight;
        private GaiaResource resources;
        private float x;
        private float y;
        private float z;
        private float rotation;
        private float width;
        private float height;
        private bool baseLevelEnabled;
        private float baseLevel = 0f;
        private bool stampBase = false;
        private bool showBase = true;
        private bool adaptiveBase = false;
        private bool showBoundingBox = true;
        private AnimationCurve heightModifier;
        private int smoothIterations = 0;
        //private bool normaliseStamp = false;
        //private bool invertStamp = false;
        private float blendStrength = 0.5f;
        //private Texture2D feature = null;
        //private GaiaConstants.ImageFitnessFilterMode areaMaskMode;
        //private GaiaConstants.MaskInfluence areaMaskInfluence;
        //private int imageMaskSmoothIterations;
        //private Texture2D imageMask;
        //private bool imageMaskInvert;
        //private bool imageMaskNormalise;
        //private bool imageMaskFlip;
        //private float noiseMaskSeed;
        //private int noiseMaskOctaves;
        //private float noiseMaskPersistence;
        //private float noiseMaskFrequency;
        //private float noiseMaskLacunarity;
        //private float noiseZoom;
        //private AnimationCurve distanceMask;
        //private GaiaConstants.MaskInfluence distanceMaskInfluence;
        private bool showSeaLevelPlane = true;
        private bool showSeaLevelinStampPreview = true;
        private bool alwaysShow = false;
        private bool showRulers = false;
        private bool m_ShowErosionControls;
        private bool m_ShowAdvancedUI;
        private bool m_ShowThermalUI;
        private bool m_ShowWaterUI;
        private bool m_ShowSedimentUI;
        private bool m_ShowRiverBankUI;
        private UnityEditorInternal.ReorderableList m_masksReorderable;
        private UnityEditorInternal.ReorderableList m_autoSpawnerReorderable;
        private int m_maskBakingResolution = 2048;
        private string m_maskBakingPath;
        private string m_SaveAndLoadMessage;
        private MessageType m_SaveAndLoadMessageType;
        private bool m_autoSpawnRequested;
        private bool m_startedTerrainChanges;
        private long m_lastHeightmapUpdateTimeStamp;
        private bool m_activatePreviewRequested;
        private long m_activatePreviewTimeStamp;

        private CollisionMask[] m_collisionMaskListBeingDrawn;
        private bool m_masksExpanded = true;

        #endregion;

        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }

            //check if we opened a stamp selection window from this stamper, and if yes, close it down
            var allWindows = Resources.FindObjectsOfTypeAll<GaiaStampSelectorEditorWindow>();
            for (int i = allWindows.Length-1; i>=0;i--)
            {
                foreach (ImageMask imageMask in m_stamper.m_settings.m_imageMasks)
                {
                    if (allWindows[i].m_editedImageMask == imageMask)
                    {
                        allWindows[i].Close();
                    }
                }
            }


            for (int i = 0; i < m_tempTextureList.Count; i++)
            {
                UnityEngine.Object.DestroyImmediate(m_tempTextureList[i]);
            }

        }

        /// <summary>
        /// Called when object selected
        /// </summary>
        void OnEnable()
        {
            //if (m_editorUtils == null)
            //{
            //    m_editorUtils = new EditorUtils(this);
            //}
            //Get the settings and update tooltips
            if (m_gaiaSettings == null)
            {
                m_gaiaSettings = Gaia.GaiaUtils.GetGaiaSettings();
            }

            m_imageMaskHeader = new GUIStyle();
            // Setup colors for Unity Pro
            if (EditorGUIUtility.isProSkin)
            {
                m_imageMaskHeader.normal.background = GaiaUtils.GetBGTexture(GaiaUtils.GetColorFromHTML("2d2d2dff"), m_tempTextureList);
            }
            else
            {
                m_imageMaskHeader.normal.background = GaiaUtils.GetBGTexture(GaiaUtils.GetColorFromHTML("a2a2a2ff"), m_tempTextureList);
            }

            m_stamper = (Stamper)target;
            
            if (m_stamper.m_settings == null)
            {
                m_stamper.m_settings = ScriptableObject.CreateInstance<StamperSettings>();
                serializedObject.ApplyModifiedProperties();
            }

           

            //SerializedObject propObj = new SerializedObject(serializedObject.FindProperty("m_currentSettings").objectReferenceValue);
            //m_clipData = propObj.FindProperty("m_clipData");

            //StamperSettings tmp = serializedObject.FindProperty("m_currentSettings").objectReferenceValue as StamperSettings;

            //foreach (var sp in serializedObject.GetIterator())
            //{
            //    Debug.Log(sp.ToString());
            //}

            //m_clipData = serializedObject.FindProperty("m_currentSettings").FindPropertyRelative("m_currentSettings.m_clipData");
            CreateMaskList();
            CreateAutoSpawnerList();


            
            if (m_gaiaSettings != null)
            {
                m_showTooltips = m_gaiaSettings.m_showTooltips;
            }

            //Init editor utils
            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }


            if (GaiaWater.DoesWaterExist())
            {
                m_stamper.m_showSeaLevelPlane = false;
            }

            GaiaLighting.SetPostProcessingStatus(false);

            StartEditorUpdates();
        }

        void CreateAutoSpawnerList()
        {
            m_autoSpawnerReorderable = new UnityEditorInternal.ReorderableList(m_stamper.m_autoSpawners, typeof(BiomeSpawnerListEntry), true, true, true, true);
            m_autoSpawnerReorderable.elementHeightCallback = OnElementHeightAutoSpawnerListEntry;
            m_autoSpawnerReorderable.drawElementCallback = DrawAutoSpawnerListElement; ;
            m_autoSpawnerReorderable.drawHeaderCallback = DrawAutoSpawnerListHeader;
            m_autoSpawnerReorderable.onAddCallback = OnAddAutoSpawnerListEntry;
            m_autoSpawnerReorderable.onRemoveCallback = OnRemoveAutosSpawnerListEntry;
            m_autoSpawnerReorderable.onReorderCallback = OnReorderAutoSpawnerList;
        }

        private void OnReorderAutoSpawnerList(ReorderableList list)
        {
            //Do nothing, changing the order does not immediately affect anything in the stamper
        }

        private void OnRemoveAutosSpawnerListEntry(ReorderableList list)
        {
            m_stamper.m_autoSpawners = StamperAutoSpawnerListEditor.OnRemoveListEntry(m_stamper.m_autoSpawners, m_autoSpawnerReorderable.index);
            list.list = m_stamper.m_autoSpawners;
        }

        private void OnAddAutoSpawnerListEntry(ReorderableList list)
        {
            m_stamper.m_autoSpawners = StamperAutoSpawnerListEditor.OnAddListEntry(m_stamper.m_autoSpawners);
            list.list = m_stamper.m_autoSpawners;
        }

        private void DrawAutoSpawnerListHeader(Rect rect)
        {
            StamperAutoSpawnerListEditor.DrawListHeader(rect, true, m_stamper.m_autoSpawners, m_editorUtils);
        }

        private void DrawAutoSpawnerListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            StamperAutoSpawnerListEditor.DrawListElement(rect, m_stamper.m_autoSpawners[index], m_editorUtils);
        }

        private float OnElementHeightAutoSpawnerListEntry(int index)
        {
            return StamperAutoSpawnerListEditor.OnElementHeight();
        }

        private void CreateMaskList()
        {
            m_masksReorderable = new UnityEditorInternal.ReorderableList(m_stamper.m_settings.m_imageMasks, typeof(ImageMask), true, true, true, true);
            m_masksReorderable.elementHeightCallback = OnElementHeightStamperMaskListEntry; 
            m_masksReorderable.drawElementCallback = DrawStamperMaskListElement; ;
            m_masksReorderable.drawHeaderCallback = DrawStamperMaskListHeader;
            m_masksReorderable.onAddCallback = OnAddStamperMaskListEntry;
            m_masksReorderable.onRemoveCallback = OnRemoveStamperMaskListEntry;
            m_masksReorderable.onReorderCallback = OnReorderStamperMaskList;

            foreach (ImageMask mask in m_stamper.m_settings.m_imageMasks)
            {
                mask.m_reorderableCollisionMaskList = CreateStamperCollisionMaskList(mask.m_reorderableCollisionMaskList, mask.m_collisionMasks);
            }
        }

        private float OnElementHeightStamperMaskListEntry(int index)
        {
            return ImageMaskListEditor.OnElementHeight(index, m_stamper.m_settings.m_imageMasks[index]);
        }

        private void DrawStamperMaskListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            //bool isCopiedMask = m_stamper.m_settings.m_imageMasks[index] != null && m_stamper.m_settings.m_imageMasks[index] == m_copiedImageMask;

            ImageMask copiedImageMask = GaiaSessionManager.GetSessionManager(false).m_copiedImageMask;

            MaskListButtonCommand mlbc = ImageMaskListEditor.DrawMaskListElement(rect, index, m_stamper.m_settings.m_imageMasks, ref m_collisionMaskListBeingDrawn, m_editorUtils, Terrain.activeTerrain, GaiaUtils.IsStampOperation(m_stamper.m_settings.m_operation), copiedImageMask, m_imageMaskHeader.normal.background, m_gaiaSettings);
            switch (mlbc)
            {
                case MaskListButtonCommand.Delete:
                        m_masksReorderable.index = index;
                        OnRemoveStamperMaskListEntry(m_masksReorderable);
                    break;
                case MaskListButtonCommand.Duplicate:
                        ImageMask newImageMask = ImageMask.Clone(m_stamper.m_settings.m_imageMasks[index]);
                        m_stamper.m_settings.m_imageMasks = GaiaUtils.InsertElementInArray(m_stamper.m_settings.m_imageMasks, newImageMask, index + 1);
                        m_masksReorderable.list = m_stamper.m_settings.m_imageMasks;
                        m_stamper.m_settings.m_imageMasks[index + 1].m_reorderableCollisionMaskList = CreateStamperCollisionMaskList(m_stamper.m_settings.m_imageMasks[index + 1].m_reorderableCollisionMaskList, m_stamper.m_settings.m_imageMasks[index + 1].m_collisionMasks);
                        serializedObject.ApplyModifiedProperties();
 
                    break;
                case MaskListButtonCommand.Copy:
                    GaiaSessionManager.GetSessionManager(false).m_copiedImageMask = m_stamper.m_settings.m_imageMasks[index];
                    break;
                case MaskListButtonCommand.Paste:
                    m_stamper.m_settings.m_imageMasks[index] = ImageMask.Clone(copiedImageMask);
                    //Rebuild collsion mask list with new content from the cloning
                    m_stamper.m_settings.m_imageMasks[index].m_reorderableCollisionMaskList = CreateStamperCollisionMaskList(m_stamper.m_settings.m_imageMasks[index].m_reorderableCollisionMaskList, m_stamper.m_settings.m_imageMasks[index].m_collisionMasks);
                    GaiaSessionManager.GetSessionManager(false).m_copiedImageMask = null;
                    break;

            }
        }

        private void DrawStamperMaskListHeader(Rect rect)
        {
            m_masksExpanded = ImageMaskListEditor.DrawFilterListHeader(rect, m_masksExpanded, m_stamper.m_settings.m_imageMasks, m_editorUtils);
        }

        private void OnAddStamperMaskListEntry(ReorderableList list)
        {
            m_stamper.m_settings.m_imageMasks = ImageMaskListEditor.OnAddMaskListEntry(m_stamper.m_settings.m_imageMasks, m_stamper.m_maxCurrentTerrainHeight, m_stamper.m_minCurrentTerrainHeight, m_stamper.m_seaLevel);
            ImageMask lastElement = m_stamper.m_settings.m_imageMasks[m_stamper.m_settings.m_imageMasks.Length - 1];
            lastElement.m_reorderableCollisionMaskList = CreateStamperCollisionMaskList(lastElement.m_reorderableCollisionMaskList, lastElement.m_collisionMasks);
            list.list = m_stamper.m_settings.m_imageMasks;
        }

        private void OnRemoveStamperMaskListEntry(ReorderableList list)
        {
            m_stamper.m_settings.m_imageMasks = ImageMaskListEditor.OnRemoveMaskListEntry(m_stamper.m_settings.m_imageMasks, list.index);
            list.list = m_stamper.m_settings.m_imageMasks;
        }

        private void OnReorderStamperMaskList(ReorderableList list)
        {
                m_stamper.m_stampDirty = true;
                m_stamper.DrawStampPreview();
        }


        /// <summary>
        /// Creates the reorderable collision mask list for collision masks in the spawner itself.
        /// </summary>
        public ReorderableList CreateStamperCollisionMaskList(ReorderableList list, CollisionMask[] collisionMasks)
        {
            list = new ReorderableList(collisionMasks, typeof(CollisionMask), true, true, true, true);
            list.elementHeightCallback = OnElementHeightCollisionMaskList;
            list.drawElementCallback = DrawStamperCollisionMaskElement;
            list.drawHeaderCallback = DrawStamperCollisionMaskListHeader;
            list.onAddCallback = OnAddStamperCollisionMaskListEntry;
            list.onRemoveCallback = OnRemoveStamperCollisionMaskMaskListEntry;
            return list;
        }

        private void OnRemoveStamperCollisionMaskMaskListEntry(ReorderableList list)
        {
            //look up the collision mask in the spawner's mask list
            foreach (ImageMask imagemask in m_stamper.m_settings.m_imageMasks)
            {
                if (imagemask.m_reorderableCollisionMaskList == list)
                {
                    imagemask.m_collisionMasks = CollisionMaskListEditor.OnRemoveMaskListEntry(imagemask.m_collisionMasks, list.index);
                    list.list = imagemask.m_collisionMasks;
                    return;
                }
            }
        }

        private void OnAddStamperCollisionMaskListEntry(ReorderableList list)
        {
            //look up the collision mask in the spawner's mask list
            foreach (ImageMask imagemask in m_stamper.m_settings.m_imageMasks)
            {
                if (imagemask.m_reorderableCollisionMaskList == list)
                {
                    imagemask.m_collisionMasks = CollisionMaskListEditor.OnAddMaskListEntry(imagemask.m_collisionMasks);
                    list.list = imagemask.m_collisionMasks;
                    return;
                }
            }
        }

        private void DrawStamperCollisionMaskListHeader(Rect rect)
        {
            foreach (ImageMask imagemask in m_stamper.m_settings.m_imageMasks)
            {
                if (imagemask.m_collisionMasks == m_collisionMaskListBeingDrawn)
                {
                    imagemask.m_collisionMaskExpanded = CollisionMaskListEditor.DrawFilterListHeader(rect, imagemask.m_collisionMaskExpanded, imagemask.m_collisionMasks, m_editorUtils);
                }
            }
        }

        private void DrawStamperCollisionMaskElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (m_collisionMaskListBeingDrawn != null && m_collisionMaskListBeingDrawn.Length > index && m_collisionMaskListBeingDrawn[index] != null)
            {
                CollisionMaskListEditor.DrawMaskListElement(rect, index, m_collisionMaskListBeingDrawn[index], m_editorUtils, Terrain.activeTerrain, GaiaConstants.FeatureOperation.Contrast);
            }
        }

        private float OnElementHeightCollisionMaskList(int index)
        {
            return CollisionMaskListEditor.OnElementHeight(index, m_collisionMaskListBeingDrawn);
        }


        //private void OnReorderMaskList(ReorderableList list)
        //{
        //    //refresh the stamp preview when the order of filter operations has changed
        //    m_stamper.m_stampDirty = true;
        //    m_stamper.DrawStampPreview();
        //}

        //private void OnRemoveMaskListEntry(ReorderableList list)
        //{
        //    int idx = list.index;
        //    if (idx < 0 || idx >= m_stamper.m_settings.m_imageMasks.Length)
        //        return;
        //    ImageMask toRemove = m_stamper.m_settings.m_imageMasks[idx];
        //    ImageMask[] newList = new ImageMask[m_stamper.m_settings.m_imageMasks.Length - 1];
        //    for (int i = 0; i < newList.Length; ++i)
        //    {
        //        if (i < idx)
        //        {
        //            newList[i] = m_stamper.m_settings.m_imageMasks[i];
        //        }
        //        else if (i >= idx)
        //        {
        //            newList[i] = m_stamper.m_settings.m_imageMasks[i + 1];
        //        }
        //    }
        //    m_stamper.m_settings.m_imageMasks = newList;
        //    m_masksReorderable.list = m_stamper.m_settings.m_imageMasks;
        //}

        //private float OnElementHeight(int index)
        //{
        //    //switch (index)
        //    //{
        //    //    case 0: 
        //    //        return 30f;
        //    //        break;
        //    //    default:
        //    //        return 10f;
        //    //        break;
        //    //}
        //    return EditorGUIUtility.singleLineHeight * 7f;
        //}

        //void OnAddMaskListEntry(UnityEditorInternal.ReorderableList list)
        //{
        //    ImageMask[] newList = new ImageMask[m_stamper.m_settings.m_imageMasks.Length + 1];
        //    for (int i = 0; i < m_stamper.m_settings.m_imageMasks.Length; ++i)
        //    {
        //        newList[i] = m_stamper.m_settings.m_imageMasks[i];
        //    }
        //    newList[newList.Length - 1] = new ImageMask();
        //    m_stamper.m_settings.m_imageMasks = newList;
        //    list.list = m_stamper.m_settings.m_imageMasks;
        //}
        //void DrawMaskListElement(Rect rect, int index, bool isActive, bool isFocused)
        //{
        //    ImageMask imageFilter = m_stamper.m_settings.m_imageMasks[index];
        //    int oldIndent = EditorGUI.indentLevel;
        //    EditorGUI.indentLevel = 0;
        //    imageFilter.m_active = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskActive"), imageFilter.m_active);
        //    imageFilter.m_invert = EditorGUI.Toggle(new Rect(rect.x + rect.width * 0.5f, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskInvert"), imageFilter.m_invert);

        //    rect.y += EditorGUIUtility.singleLineHeight;
        //    imageFilter.m_operation = (ImageMaskOperation)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskOperation"), imageFilter.m_operation);
        //    if (GaiaUtils.IsStampOperation(operation))
        //    {
        //        rect.y += EditorGUIUtility.singleLineHeight;
        //        imageFilter.m_influence = (ImageMaskInfluence)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskInfluence"), imageFilter.m_influence);
        //    }
        //    else
        //    {
        //        imageFilter.m_influence = ImageMaskInfluence.Local;
        //    }
        //    rect.y += EditorGUIUtility.singleLineHeight;
        //    imageFilter.m_strength = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskStrength"), imageFilter.m_strength, 0, 1);
        //    rect.y += EditorGUIUtility.singleLineHeight;

        //    //First mask in the stack always needs to be in "Multiply" mode
        //    if (index == 0)
        //    {
        //        imageFilter.m_blendMode = ImageMaskBlendMode.Multiply;
        //    }

        //    switch (imageFilter.m_operation)
        //    {
        //        case ImageMaskOperation.ImageMask:
        //            //First mask in the stack always needs to be in "Multiply" mode, others can be selected by the user
        //            if (index > 0)
        //            {
        //                imageFilter.m_blendMode = (ImageMaskBlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskBlendMode"), imageFilter.m_blendMode);
        //            }
        //            rect.y += EditorGUIUtility.singleLineHeight;
        //            imageFilter.m_imageMaskTexture = (Texture2D)EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskImageMask"), imageFilter.m_imageMaskTexture, typeof(Texture2D), false);
        //            break;
        //        case ImageMaskOperation.DistanceMask:
        //            //distance masks are always in multiply mode
        //            if (index > 0)
        //            {
        //                imageFilter.m_blendMode = ImageMaskBlendMode.Multiply;
        //            }
        //            imageFilter.m_distanceMaskCurve = EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskDistanceMask"), imageFilter.m_distanceMaskCurve);
        //            rect.y += EditorGUIUtility.singleLineHeight;
        //            imageFilter.m_distanceMaskAxes = (ImageMaskDistanceMaskAxes)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskDistanceMaskAxes"), imageFilter.m_distanceMaskAxes);
        //            if (imageFilter.m_distanceMaskAxes == ImageMaskDistanceMaskAxes.XZ)
        //            {
        //                rect.y += EditorGUIUtility.singleLineHeight;
        //                imageFilter.m_xOffSet = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskXOffset"), imageFilter.m_xOffSet);
        //                imageFilter.m_zOffSet = EditorGUI.FloatField(new Rect(rect.x + rect.width * 0.5f, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskZOffset"), imageFilter.m_zOffSet);
        //                imageFilter.m_xOffSetScalar = Mathf.Lerp(-0.5f, 0.5f, Mathf.InverseLerp(m_stamper.m_cachedTerrain.terrainData.size.x * 0.5f, m_stamper.m_cachedTerrain.terrainData.size.x * -0.5f, imageFilter.m_xOffSet));
        //                imageFilter.m_zOffSetScalar = Mathf.Lerp(-0.5f, 0.5f, Mathf.InverseLerp(m_stamper.m_cachedTerrain.terrainData.size.z * 0.5f, m_stamper.m_cachedTerrain.terrainData.size.z * -0.5f, imageFilter.m_zOffSet));
        //            }

        //            break;
        //        case ImageMaskOperation.HeightMask:
        //            //height masks are always in multiply mode
        //            if (index > 0)
        //            {
        //                imageFilter.m_blendMode = ImageMaskBlendMode.Multiply;
        //            }
        //            imageFilter.m_heightMaskCurve = EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskHeightMaskCurve"), imageFilter.m_heightMaskCurve);
        //            rect.y += EditorGUIUtility.singleLineHeight;
        //            EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskHeightMinMax"), ref imageFilter.m_heightMin, ref imageFilter.m_heightMax, 0, m_stamper.m_cachedTerrain.terrainData.size.y);
        //            imageFilter.m_scalarMaxHeight = Mathf.Lerp(-0.5f, 0.5f, Mathf.InverseLerp(m_stamper.m_cachedTerrain.transform.position.y - m_stamper.m_cachedTerrain.terrainData.size.y, m_stamper.m_cachedTerrain.transform.position.y + m_stamper.m_cachedTerrain.terrainData.size.y, imageFilter.m_heightMax));
        //            imageFilter.m_scalarMinHeight = Mathf.Lerp(-0.5f, 0.5f, Mathf.InverseLerp(m_stamper.m_cachedTerrain.transform.position.y - m_stamper.m_cachedTerrain.terrainData.size.y, m_stamper.m_cachedTerrain.transform.position.y + m_stamper.m_cachedTerrain.terrainData.size.y, imageFilter.m_heightMin));
        //            break;
        //        //case ImageMaskOperation.HeightTransform:
        //        //    //distance masks are always in multiply mode
        //        //    if (index > 0)
        //        //    {
        //        //        imageFilter.m_blendMode = ImageMaskBlendMode.Multiply;
        //        //    }
        //        //    imageFilter.m_heightTransformCurve = EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskHeightTransformCurve"), imageFilter.m_heightTransformCurve);
        //        //    break;
        //        default:
        //            break;
        //    }
        //    EditorGUI.indentLevel = oldIndent;
        //}
        //void DrawFilterListHeader(Rect rect)
        //{
        //    int oldIndent = EditorGUI.indentLevel;
        //    EditorGUI.indentLevel = 0;
        //    rect.xMin += 8f;
        //    m_masksExpanded = EditorGUI.Foldout(rect, m_masksExpanded, PropertyCount("MaskSettings", m_stamper.m_settings.m_imageMasks), true);
        //    EditorGUI.indentLevel = oldIndent;
        //}

        /// <summary>
        /// Called when object deselected
        /// </summary>
        void OnDisable()
        {
            m_stamper = (Stamper)target;
            if (m_stamper != null)
            {
                if (!m_stamper.m_alwaysShow)
                {
                    m_stamper.HidePreview();
                }
            }
            GaiaLighting.SetPostProcessingStatus(true);
        }


        /// <summary>
        /// Start editor updates
        /// </summary>
        public void StartEditorUpdates()
        {
            if (!m_startedUpdates)
            {
                m_startedUpdates = true;
                EditorApplication.update += EditorUpdate;
            }

            if (!m_startedTerrainChanges)
            {
                m_startedTerrainChanges = true;
                TerrainCallbacks.heightmapChanged += OnHeightmapChanged;
            }
        }

        private void OnHeightmapChanged(Terrain terrain, RectInt heightRegion, bool synched)
        {
            if (m_autoSpawnRequested)
            {
                //delay preview re-activation as well
                if (m_activatePreviewRequested)
                {
                    m_activatePreviewTimeStamp = GaiaUtils.GetUnixTimestamp();
                }
                m_lastHeightmapUpdateTimeStamp = GaiaUtils.GetUnixTimestamp();
            }
        }

        /// <summary>
        /// Stop editor updates
        /// </summary>
        public void StopEditorUpdates()
        {
            if (m_startedUpdates)
            {
                m_startedUpdates = false;
                EditorApplication.update -= EditorUpdate;
            }

            if (m_startedTerrainChanges)
            {
                m_startedTerrainChanges = false;
                TerrainCallbacks.heightmapChanged -= OnHeightmapChanged;
            }
        }

        /// <summary>
        /// This is used just to force the editor to repaint itself
        /// </summary>
        void EditorUpdate()
        {
            if (m_stamper != null)
            {
                if (m_stamper.m_updateCoroutine != null)
                {
                    if ((DateTime.Now - m_timeSinceLastUpdate).TotalMilliseconds > 500)
                    {
                        m_timeSinceLastUpdate = DateTime.Now;
                        Repaint();
                    }
                }
                else
                {
                    if ((DateTime.Now - m_timeSinceLastUpdate).TotalSeconds > 5)
                    {
                        m_timeSinceLastUpdate = DateTime.Now;
                        Repaint();
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            m_editorUtils.Initialize();
            serializedObject.Update();

            //Get our stamper
            m_stamper = (Stamper)target;

            long currentTimeStamp = GaiaUtils.GetUnixTimestamp();

            //Do we still have outstanding spawn ot height update requests?
            if (m_autoSpawnRequested || m_heightUpdateRequested)
            {
                //push re-activation of the preview forward
                m_activatePreviewTimeStamp = currentTimeStamp;

                //we do have, lock GUI
                GUI.enabled = false;
                

                //do we want to process those requests yet?
                if ((m_lastHeightmapUpdateTimeStamp + m_gaiaSettings.m_autoTextureTreshold) < currentTimeStamp)
                {
                    if (m_heightUpdateRequested)
                    {
                        //force recalculate for the terrain we just stamped, then update our min max values
                        var gsm = GaiaSessionManager.GetSessionManager(false);
                        foreach (Terrain t in m_stamper.m_lastAffectedTerrains)
                        {
                            gsm.ForceTerrainMinMaxCalculation(t);
                        }
                        m_stamper.UpdateMinMaxHeight();
                        m_heightUpdateRequested = false;
                    }

                    if (m_autoSpawnRequested && Spawner.HandleAutoSpawnerStack(m_stamper.m_autoSpawners, true))
                    {
                        m_autoSpawnRequested = false;
                        GaiaSessionManager.GetSessionManager().m_collisionMaskCache.EndAutoSpawn();
                        //unlock GUI
                        GUI.enabled = true;
                        EditorUtility.ClearProgressBar();
                    }
                }
                else
                {
                    
                    EditorUtility.DisplayProgressBar("Stamping", "Waiting for Unity Terrain Updates to complete...", 0);
                }
            }
            //Do not reactivate the preview while the autospawn is still running, can influence spawn results
            if (!m_autoSpawnRequested && m_activatePreviewRequested && (m_activatePreviewTimeStamp + m_gaiaSettings.m_stamperAutoHidePreviewMilliseconds < currentTimeStamp))
            {
                m_activatePreviewRequested = false;
                m_stamper.m_drawPreview = true;
            }

            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            //Setup the wrap style
            if (m_wrapStyle == null)
            {
                m_wrapStyle = new GUIStyle(GUI.skin.label);
                m_wrapStyle.wordWrap = true;
            }

            //Draw the intro
            //m_editorUtils.Panel("StamperInfo", DrawStamperInfo, true);
            //m_editorUtils.GUIHeader(null,"The stamper allows you to stamp features into your terrain. Click here to see a tutorial.",false,"", "http://www.procedural-worlds.com/gaia/tutorials/stamper-introduction/");

            //Disable if spawning
            if (m_stamper.m_stampComplete != true && !m_stamper.m_cancelStamp)
            {
                GUI.enabled = false;
            }

    
            EditorGUI.BeginChangeCheck();
            m_editorUtils.Panel("Operation", DrawOperation, true);

            //m_editorUtils.Panel("Filter", DrawFilterSettings, true);


            //Always update the editor location values regardless of whether the input fileds are drawn or not
            x = m_stamper.m_settings.m_x;
            y = m_stamper.m_settings.m_y;
            z = m_stamper.m_settings.m_z;
            rotation = m_stamper.m_settings.m_rotation;
            width = m_stamper.m_settings.m_width;
            height = m_stamper.m_settings.m_height;


            m_editorUtils.Panel("Location", DrawLocation, false);

            m_editorUtils.Panel("Appearance", DrawAppearance, false);

            m_editorUtils.Panel("SaveLoad", DrawSaveAndLoad, false);

            //m_editorUtils.Panel("TerrainHelper", DrawTerrainHelper, false);

            //m_editorUtils.Panel("StamperControls", DrawStamperControls, true);
            DrawStamperControls(false);

            //bool changesToHM = false;

            //Check if sea level changed
            if (m_stamper.m_seaLevel != GaiaSessionManager.GetSessionManager(false).GetSeaLevel())
            {
                m_stamper.m_seaLevel = GaiaSessionManager.GetSessionManager(false).GetSeaLevel();
                m_stamper.m_stampDirty = true;
            }

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                m_stamper.UpdateStamp();
                Undo.RecordObject(m_stamper, "Made changes");
                EditorUtility.SetDirty(m_stamper.m_settings);

                //MArk stamp / shader for re-calculation
                m_stamper.m_stampDirty = true;

                //Do we have a stamp operation selected?
                if (GaiaUtils.IsStampOperation(operation))
                {
                //    //Check to see if we need to load a new stamp
                //    if (feature != null)
                //    {
                //        if (m_stamper.m_stampImage == null)
                //        {
                //            m_stamper.m_stampImage = feature;
                //            m_stamper.m_invertStamp = invertStamp;
                //            m_stamper.m_normaliseStamp = normaliseStamp;
                //            m_stamper.m_smoothIterations = smoothIterations;
                //            m_stamper.LoadStamp();
                //            m_stamper.FitToTerrain();
                //            baseLevel = m_stamper.m_settings.m_baseLevel;
                //            width = m_stamper.m_settings.m_width;
                //            height = m_stamper.m_settings.m_height;
                //            rotation = m_stamper.m_settings.m_rotation;
                //            x = m_stamper.m_settings.m_x;
                //            y = m_stamper.m_settings.m_y;
                //            z = m_stamper.m_settings.m_z;
                //            changesToHM = true;
                //        }
                //        else if (m_stamper.m_stampImage.GetInstanceID() != feature.GetInstanceID())
                //        {
                //            m_stamper.m_stampImage = feature;
                //            m_stamper.m_invertStamp = invertStamp;
                //            m_stamper.m_normaliseStamp = normaliseStamp;
                //            m_stamper.m_smoothIterations = smoothIterations;
                //            m_stamper.LoadStamp();
                //            baseLevel = m_stamper.m_settings.m_baseLevel;
                //            changesToHM = true;
                //        }
                //    }

                //    //Determine if a stamp image reload from scratch is required - this is the case when going back from normalization, or
                //    //when the smoothing parameter changed, otherwise the smoothing result will not be correct!
                //    if (
                //        //Reload required: user wishes to de-normalize the stamp
                //        (m_stamper.m_normaliseStamp != normaliseStamp && normaliseStamp == false) ||
                //        //Reload required: user wishes a different smoothing parameter
                //        m_stamper.m_smoothIterations != smoothIterations
                //       )
                //    {
                //        m_stamper.m_normaliseStamp = normaliseStamp;
                //        m_stamper.m_smoothIterations = smoothIterations;
                //        m_stamper.LoadStampByGUID(true);
                //    }



                //    //And normalise it
                //    if (m_stamper.m_normaliseStamp != normaliseStamp)
                //    {
                //        m_stamper.m_normaliseStamp = normaliseStamp;
                //        if (normaliseStamp)
                //        {
                //            m_stamper.NormaliseStamp();
                //            changesToHM = true;
                //        }
                //    }

                //    //And smooth it
                //    if (m_stamper.m_smoothIterations != smoothIterations)
                //    {
                //        m_stamper.m_smoothIterations = smoothIterations;
                //        m_stamper.SmoothStamp();
                //        changesToHM = true;
                //    }

                //    //Were changes to the heightmap data made? If yes, update the stamp image
                //    if (changesToHM)
                //    {
                //        m_stamper.UpdateStampImageFromHeightmap();
                //    }


                //    //Note that stamp inversion is handled by inverting the brush strength before passing it into the 
                //    //preview / stamp shader - faster this way than converting the heightmap
                //    m_stamper.m_invertStamp = invertStamp;

                //    //Rebuild height modiifier in case it was nullified due to not being shown on the GUI
                //    if (heightModifier == null || heightModifier.keys.Length==0)
                //    {
                //        heightModifier = new AnimationCurve(new Keyframe[2] { new Keyframe() { time = 0, value = 0 }, new Keyframe() { time = 1, value = 1 } });
                //    }

                //    m_stamper.m_heightModifier = heightModifier;
                    
                    m_stamper.m_settings.m_drawStampBase = stampBase;
                    m_stamper.m_blendStrength = blendStrength;


                    //User switched to a different operation type? Set some default settings
                    if (m_stamper.m_settings.m_operation != operation)
                    {
                        if (operation == GaiaConstants.FeatureOperation.RaiseHeight || operation == GaiaConstants.FeatureOperation.AddHeight)
                        {
                            //Move the base level to 0 to make sure the stamp is not hidden right from the start
                            baseLevel = 0f;
                            showBase = true;

                            //Move the stamp to the center of the terrain hight to make it likely it is visible right away
                            y = Terrain.activeTerrain.terrainData.size.y / 2f;
                        }
                        if (operation == GaiaConstants.FeatureOperation.LowerHeight || operation == GaiaConstants.FeatureOperation.SubtractHeight)
                        {
                            //Move the base level to 1 to make sure the stamp is not hidden right from the start
                            baseLevel = 1f;
                            showBase = true;

                            //Move the stamp close to the bottom 10% of the terrain to make it likely it is visible right away
                            y = Terrain.activeTerrain.terrainData.size.y / 10f;
                        }

                    }
                }
                else
                {
                    //no stamp operation - empty the stamp image to a white texture
                    m_stamper.EmptyStampImage();
                }


                m_stamper.m_settings.m_operation = operation;

                m_stamper.m_MaskTexturesDirty = true;
                //if (m_stamper.m_heightModifier != heightModifier)
                //{
                //    m_stamper.m_MaskTexturesDirty = true;
                //}

               
               

                //m_stamper.m_distanceMask = distanceMask;
                //m_stamper.m_distanceMaskInfluence = distanceMaskInfluence;
                m_stamper.m_smoothIterations = smoothIterations;
                m_stamper.m_settings.m_x = x;
                m_stamper.m_settings.m_y = y;
                m_stamper.m_settings.m_z = z;
                m_stamper.m_settings.m_width = width;
                m_stamper.m_settings.m_height = height;
                m_stamper.m_settings.m_rotation = rotation;
                m_stamper.m_alwaysShow = alwaysShow;
                m_stamper.m_showSeaLevelPlane = showSeaLevelPlane;
                m_stamper.m_showSeaLevelinStampPreview = showSeaLevelinStampPreview;
                m_stamper.m_settings.m_baseLevel = baseLevel;
                m_stamper.m_settings.m_adaptiveBase = adaptiveBase;
                m_stamper.m_showBoundingBox = showBoundingBox;
                m_stamper.m_showBase = showBase;
                m_stamper.m_showRulers = showRulers;


                EditorUtility.SetDirty(m_stamper);
            }

            //m_stamper.m_currentSettings.m_clipData = new StamperSettings.ClipData[m_clipData.arraySize];
            //for (int x = 0; x < m_clipData.arraySize; x++) {
            //    m_stamper.m_currentSettings.m_clipData[x] = new StamperSettings.ClipData()
            //    {
            //        m_clip = (AudioClip)m_clipData.GetArrayElementAtIndex(x).FindPropertyRelative("m_clip").objectReferenceValue,
            //        m_volume = m_clipData.GetArrayElementAtIndex(x).FindPropertyRelative("m_volume").floatValue
            //    };
            //}
                 
            serializedObject.ApplyModifiedProperties();
            //SerializedObject propObj = new SerializedObject(serializedObject.FindProperty("m_currentSettings").objectReferenceValue);
            //propObj.ApplyModifiedProperties();


        }

        private void DrawSaveAndLoad(bool obj)
        {
            if(m_SaveAndLoadMessage!="")
                EditorGUILayout.HelpBox(m_SaveAndLoadMessage, m_SaveAndLoadMessageType, true);

            EditorGUILayout.BeginHorizontal();
            if (m_editorUtils.Button("LoadButton"))
            {
                string openFilePath = EditorUtility.OpenFilePanel("Load Stamper settings..", GaiaDirectories.GetMaskDirectory(), "asset");
                

                bool loadConditionsMet = true;

                //Do we have a path to begin with?
                if (openFilePath==null || openFilePath =="")
                {
                    //Silently abort in this case, the user has pressed "Abort" in the File Open Dialog
                    loadConditionsMet = false;
                }


                //Look for the Assets Directory
                if (!openFilePath.Contains("Assets") && loadConditionsMet)
                {
                    m_SaveAndLoadMessage = m_editorUtils.GetContent("LoadNoAssetDirectory").text;
                    m_SaveAndLoadMessageType = MessageType.Error;
                    loadConditionsMet = false;
                }
                if (loadConditionsMet)
                {

                    openFilePath = openFilePath.Substring(openFilePath.IndexOf("Assets"));
                    StamperSettings settingsToLoad = (StamperSettings)AssetDatabase.LoadAssetAtPath(openFilePath, typeof(StamperSettings));

                    if (settingsToLoad != null)
                    {
                        m_stamper.LoadSettings(settingsToLoad);
                        CreateMaskList();
                        //Update the internal editor position / scale values after loading
                        x = m_stamper.m_settings.m_x;
                        y = m_stamper.m_settings.m_y;
                        z = m_stamper.m_settings.m_z;
                        rotation = m_stamper.m_settings.m_rotation;
                        width = m_stamper.m_settings.m_width;
                        height = m_stamper.m_settings.m_height;
                        //mark stamper as dirty so it will be redrawn
                        m_stamper.m_stampDirty = true;
                        m_SaveAndLoadMessage = m_editorUtils.GetContent("LoadSuccessful").text;
                        m_SaveAndLoadMessageType = MessageType.Info;
                    }
                    else
                    {
                        m_SaveAndLoadMessage = m_editorUtils.GetContent("LoadFailed").text;
                        m_SaveAndLoadMessageType = MessageType.Error;
                    }
                }

            }
            if (m_editorUtils.Button("SaveButton"))
            {
                string saveFilePath = EditorUtility.SaveFilePanel("Save Stamper settings as..", GaiaDirectories.GetMaskDirectory(), "StamperSettings", "asset");

                bool saveConditionsMet = true;

                //Do we have a path to begin with?
                if (saveFilePath == null || saveFilePath == "")
                {
                    //Silently abort in this case, the user has pressed "Abort" in the File Open Dialog
                    saveConditionsMet = false;
                }

                //Look for the Assets Directory
                if (!saveFilePath.Contains("Assets") && saveConditionsMet)
                {
                    m_SaveAndLoadMessage = m_editorUtils.GetContent("SaveNoAssetDirectory").text;
                    m_SaveAndLoadMessageType = MessageType.Error;
                    saveConditionsMet = false;
                }

                if (saveConditionsMet)
                {
                    saveFilePath = saveFilePath.Substring(saveFilePath.IndexOf("Assets"));

                    // Check if there is already an asset in this path
                    StamperSettings settingsToLoad = (StamperSettings)AssetDatabase.LoadAssetAtPath(saveFilePath, typeof(StamperSettings));

                    if (settingsToLoad != null)
                    {
                        AssetDatabase.DeleteAsset(saveFilePath);
                    }
                    
                    AssetDatabase.CreateAsset(m_stamper.m_settings, saveFilePath);
                    EditorUtility.SetDirty(m_stamper.m_settings);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    //Check if save was successful
                    settingsToLoad = (StamperSettings)AssetDatabase.LoadAssetAtPath(saveFilePath, typeof(StamperSettings));
                    if (settingsToLoad != null)
                    {
                        m_SaveAndLoadMessage = m_editorUtils.GetContent("SaveSuccessful").text;
                        m_SaveAndLoadMessageType = MessageType.Info;
                        //dissociate the current stamper settings from the file we just saved, otherwise the user will continue editing the file afterwards

                        m_stamper.m_settings = ScriptableObject.CreateInstance<StamperSettings>();
                        m_stamper.LoadSettings(settingsToLoad);
                        CreateMaskList();
                        m_stamper.m_stampDirty = true;

                    }
                    else
                    {
                        m_SaveAndLoadMessage = m_editorUtils.GetContent("SaveFailed").text;
                        m_SaveAndLoadMessageType = MessageType.Error;
                    }
                }

            }
            EditorGUILayout.EndHorizontal();

            
        }


        private void DrawUndo(bool showHelp)
        {
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();

            m_stamper.m_recordUndo = m_editorUtils.Toggle(m_stamper.m_recordUndo, "UndoRecord");

            GUIContent content = m_editorUtils.GetContent("UndoSteps");
            content.text = string.Format(content.text, m_stamper.m_currentStamperUndoOperation, m_stamper.m_stamperUndoOperations.Count > 0 ? m_stamper.m_stamperUndoOperations.Count - 1 : 0);

            bool previousGUIState = GUI.enabled;

            m_editorUtils.Label(content);
            if (m_stamper.m_stamperUndoOperations.Count > 0)
            {
                EditorGUI.indentLevel++;
                if (m_stamper.m_currentStamperUndoOperation == 0)
                {
                    GUI.enabled = false;
                    if (m_editorUtils.Button("UndoBackButton"))
                    {
                    }

                    GUI.enabled = previousGUIState;
                }
                else
                {
                    if (m_stamper.m_currentStamperUndoOperation == 1)
                    {
                        if (m_editorUtils.Button("UndoBackButton"))
                        {
                            m_stamper.m_currentStamperUndoOperation--;
                            m_stamper.m_stamperUndoOperations[m_stamper.m_currentStamperUndoOperation].SaveToWorld(true);
                        }
                    }
                    else
                    {
                        if (m_editorUtils.Button("UndoBackButton"))
                        {
                            m_stamper.m_currentStamperUndoOperation--;
                            m_stamper.m_stamperUndoOperations[m_stamper.m_currentStamperUndoOperation].SaveToWorld(true);
                        }
                    }
                }

                if (m_stamper.m_currentStamperUndoOperation == m_stamper.m_stamperUndoOperations.Count - 1)
                {
                    GUI.enabled = false;
                    if (m_editorUtils.Button("UndoForwardButton"))
                    {
                    }

                    GUI.enabled = previousGUIState;
                }
                else
                {
                    if (m_editorUtils.Button("UndoForwardButton"))
                    {
                        m_stamper.m_currentStamperUndoOperation++;
                        m_stamper.m_stamperUndoOperations[m_stamper.m_currentStamperUndoOperation].SaveToWorld(true);
                    }
                }

                EditorGUI.indentLevel--;
            }

            GUILayout.EndHorizontal();

        }



        private void DrawStamperControls(bool showHelp)
        {


            DrawUndo(showHelp);

            ////Display progress
            //if (m_stamper.m_stampComplete != true && !m_stamper.m_cancelStamp)
            //{
            //    GUILayout.BeginVertical();
            //    GUILayout.Space(20);
            //    GUILayout.BeginHorizontal();
            //    if (GUILayout.Button(GetLabel("Cancel")))
            //    {
            //        m_stamper.CancelStamp();
            //    }
            //    GUILayout.EndHorizontal();
            //    GUILayout.Space(5);
            //    GUILayout.EndVertical();
            //    ProgressBar(string.Format("Progress ({0:0.0}%)", m_stamper.m_stampProgress * 100f), m_stamper.m_stampProgress);
            //}
            //else
            //{
            //Stamp control
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(GetLabel("Flatten")))
            {
                if (EditorUtility.DisplayDialog("Flatten Terrain tiles ?", "Are you sure you want to flatten all terrain tiles - this can not be undone ?", "Yes", "No"))
                {
                    m_stamper.FlattenTerrain();
                    m_stamper.m_stamperUndoOperations.Clear();
                    m_stamper.m_currentStamperUndoOperation=0;
                }
            }
            if (GUILayout.Button(GetLabel("Ground")))
            {
                m_stamper.AlignToGround();
                m_stamper.UpdateStamp();
            }
            if (GUILayout.Button(GetLabel("Fit To Terrain")))
            {
                m_stamper.FitToTerrain();
                m_stamper.UpdateStamp();
                x = m_stamper.m_settings.m_x;
                y = m_stamper.m_settings.m_y;
                z = m_stamper.m_settings.m_z;
                rotation = m_stamper.m_settings.m_rotation;
                width = m_stamper.m_settings.m_width;
            }
            GUILayout.EndHorizontal();
            if (Terrain.activeTerrains.Length > 1)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Fit To World")))
                {
                    m_stamper.FitToAllTerrains();
                    m_stamper.UpdateStamp();
                    x = m_stamper.m_settings.m_x;
                    y = m_stamper.m_settings.m_y;
                    z = m_stamper.m_settings.m_z;
                    rotation = m_stamper.m_settings.m_rotation;
                    width = m_stamper.m_settings.m_width;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(8);

            Rect listRect = EditorGUILayout.GetControlRect(true, m_autoSpawnerReorderable.GetHeight());
            m_autoSpawnerReorderable.DoList(listRect);

            //foreach (StamperAutoSpawner autoSpawner in m_stamper.m_autoSpawners)
            //{
            //    GUILayout.BeginHorizontal();
            //    autoSpawner.isActive = m_editorUtils.Toggle(autoSpawner.isActive, "ToggleAutoSpawnTextures");
            //    m_editorUtils.Label(new GUIContent(autoSpawner.status.ToString()));
            //    autoSpawner.spawner = (Spawner)m_editorUtils.ObjectField("AutoSpawnTexturesSpawner", autoSpawner.spawner, typeof(Spawner), true);
            //    GUILayout.EndHorizontal();
            //}
            GUILayout.BeginHorizontal();
            Color currentBGColor = GUI.backgroundColor;
            //highlight the currently active preview.
            if (m_stamper.m_drawPreview)
            {
                GUI.backgroundColor = Color.red;
            }
            if (GUILayout.Button(GetLabel("Preview")))
            {
                //as soon as the user interacts with the button, the user is in control, no need for auto activate anymore
                m_activatePreviewRequested = false;
                m_stamper.TogglePreview();
            }
            GUI.backgroundColor = currentBGColor;
            //Regardless, re-enable the spawner controls to be able to cancel
            GUI.enabled = true;

            if (m_autoSpawnRequested || m_heightUpdateRequested)
            {
                if (GUILayout.Button(GetLabel("Cancel")))
                {
                    foreach (AutoSpawner autoSpawner in m_stamper.m_autoSpawners)
                    {
                        autoSpawner.status = AutoSpawnerStatus.Done;
                        autoSpawner.spawner.CancelSpawn();
                    }
                }
            }
            else
            {

                if (GUILayout.Button(GetLabel("Stamp")))
                {
                    //Check for potential missing resources in autospawners first
                    foreach (AutoSpawner autoSpawner in m_stamper.m_autoSpawners)
                    {
                        if (autoSpawner!=null && autoSpawner.spawner!=null && autoSpawner.spawner.enabled && autoSpawner.spawner.CheckForMissingResources())
                        {
                            Debug.Log("Spawner " + autoSpawner.spawner.name + " is missing resources on the terrain, stamping was cancelled. Please deactivate this Spawner in the Autospawner List, or let the Spawner add the missing resources to the terrain.");
                            return;
                        }
                    }



                    if (m_stamper.m_recordUndo)
                    {
                        GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
                        if (mgr.TileCount > 0)
                        {
                            //Store initial state of terrain
                            if (m_stamper.m_stamperUndoOperations.Count == 0)
                            {
                                GaiaWorldManager undo = new GaiaWorldManager(Terrain.activeTerrains);
                                undo.LoadFromWorld();
                                m_stamper.m_stamperUndoOperations.Add(undo);
                            }
                        }
                    }


                    m_stamper.m_syncHeightmaps = true;
                    EditorUtility.DisplayProgressBar("Stamping", "Stamping operation...", 0);
                    m_stamper.Stamp();


                    if (m_stamper.m_recordUndo)
                    {
                        GaiaWorldManager stmpMgr = new GaiaWorldManager(Terrain.activeTerrains);
                        stmpMgr.LoadFromWorld();

                        //Are we on the last operation in the list -> add a new one at the end
                        if (m_stamper.m_currentStamperUndoOperation >= m_stamper.m_stamperUndoOperations.Count - 1)
                        {
                            m_stamper.m_stamperUndoOperations.Add(stmpMgr);
                            m_stamper.m_currentStamperUndoOperation++;
                        }
                        else //overwrite the following op and delete the rest to start a new "branch"
                        {
                            m_stamper.m_currentStamperUndoOperation++;
                            m_stamper.m_stamperUndoOperations[m_stamper.m_currentStamperUndoOperation] = stmpMgr;
                            for (int i = m_stamper.m_stamperUndoOperations.Count - 1; i > m_stamper.m_currentStamperUndoOperation; i--)
                            {
                                m_stamper.m_stamperUndoOperations.RemoveAt(i);
                            }
                        }
                    }
                    //always deactivate the preview during autospawn, even if the delay is set to 0 ms
                    //preview being drawn during autospawning can influence the spawn ruesults
                    m_stamper.m_drawPreview = false;
                    m_activatePreviewRequested = true;
                    m_activatePreviewTimeStamp = GaiaUtils.GetUnixTimestamp();

                    //always request a height update
                    m_heightUpdateRequested = true;

                    if (m_stamper.m_autoSpawners.Exists(x => x.isActive == true && x.spawner != null))
                    {
                        //Do not execute spawns right away as this can lead to errors in texture spawning, set a flag to do it after all "OnTerrainChanged" callbacks
                        m_autoSpawnRequested = true;
                        //Notify the collision mask cache that we are about to do an autospawn - this will prevent excessive cache clearing by the spawners
                        GaiaSessionManager.GetSessionManager().m_collisionMaskCache.BeginAutoSpawn();
                        m_lastHeightmapUpdateTimeStamp = GaiaUtils.GetUnixTimestamp();
                        foreach (AutoSpawner autoSpawner in m_stamper.m_autoSpawners.FindAll(x => x.isActive == true && x.spawner != null))
                        {
                            autoSpawner.status = AutoSpawnerStatus.Queued;
                        }

                    }
                    else
                    {
                        EditorUtility.ClearProgressBar();
                    }

                    ////Check that they have a single selected terrain
                    //if (Gaia.TerrainHelper.GetActiveTerrainCount() != 1)
                    //{
                    //    EditorUtility.DisplayDialog("OOPS!", "You must have only one active terrain in order to use a Spawner. Please either add a terrain, or deactivate all but one terrain.", "OK");
                    //}
                    //else
                    //{
                    //     //Check that the centre of the terrain is at 0,0,0, and offer to move
                    //     bool isCentred = true;
                    //     Bounds b = new Bounds();
                    //     Terrain terrain = Gaia.TerrainHelper.GetActiveTerrain();
                    //     Gaia.TerrainHelper.GetTerrainBounds(terrain, ref b);
                    //     if ((b.center.x != 0f) || (b.min.y != 0f) || (b.center.z != 0f))
                    //     {
                    //         isCentred = false;
                    //         if (EditorUtility.DisplayDialog("OOPS!", "The terrain must be centered at 0,0,0 for stamping to work properly. Would you like GAIA to move it for you? You will need to reposition your stamp after this to adjust for the movement. You can move the terrain back to its original position after you have finished with the stamper.", "OK", "Cancel"))
                    //         {
                    //             terrain.transform.position = new Vector3(b.extents.x * -1f, 0f, b.extents.z * -1f);
                    //         }
                    //     }

                    //     if (isCentred)
                    //     {
                    //         //Check that they are not using terrain based mask - this can give unexpected results
                    //         if (areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture0 ||
                    //             areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture1 ||
                    //             areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture2 ||
                    //             areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture3 ||
                    //             areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture4 ||
                    //             areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture5 ||
                    //             areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture6 ||
                    //             areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture7
                    //             )
                    //         {
                    //             //Do an alert and fix if necessary
                    //             if (!m_stamper.IsFitToTerrain())
                    //             {
                    //                 if (EditorUtility.DisplayDialog("WARNING!", "This feature requires your stamp to be Fit To Terrain in order to guarantee correct placement.", "Stamp Anyway", "Cancel"))
                    //                 {
                    //                     m_stamper.Stamp();
                    //                 }
                    //             }
                    //             else
                    //             {
                    //                 m_stamper.Stamp();
                    //             }
                    //         }
                    //         else
                    //         {
                    //             m_stamper.Stamp();
                    //         }
                    //     }
                    //// }
                }


            }






                //if (m_stamper.CanRedo())
                //{
                //    if (GUILayout.Button(GetLabel("Redo")))
                //    {
                //        m_stamper.Redo();
                //    }
                //}
                //else
                //{
                //    if (!m_stamper.CanUndo())
                //    {
                //        GUI.enabled = false;
                //    }

                //    if (GUILayout.Button(GetLabel("Undo")))
                //    {
                //        m_stamper.Undo();
                //    }
                //}

                GUI.enabled = true;

                GUILayout.EndHorizontal();
                // }
            
        }

        private void DrawTerrainHelper(bool showHelp)
        {
            if (GUILayout.Button(GetLabel("Show Terrain Utilities")))
            {
                var export = EditorWindow.GetWindow<GaiaTerrainExplorerEditor>(false, "Terrain Utilities");
    export.Show();
            }

                GUILayout.BeginHorizontal();
           
            if (GUILayout.Button(GetLabel("Smooth")))
            {
                if (EditorUtility.DisplayDialog("Smooth Terrain tiles ?", "Are you sure you want to smooth all terrain tiles - this can not be undone ?", "Yes", "No"))
                {
                    m_stamper.SmoothTerrain();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(GetLabel("Clear Trees")))
            {
                if (EditorUtility.DisplayDialog("Clear Terrain trees ?", "Are you sure you want to clear all terrain trees - this can not be undone ?", "Yes", "No"))
                {
                    m_stamper.ClearTrees();
                }
            }
            if (GUILayout.Button(GetLabel("Clear Details")))
            {
                if (EditorUtility.DisplayDialog("Clear Terrain details ?", "Are you sure you want to clear all terrain details - this can not be undone ?", "Yes", "No"))
                {
                    m_stamper.ClearDetails();
                }
            }
            GUILayout.EndHorizontal();
        }


        private void DrawAppearance(bool showHelp)
        {
            
            m_editorUtils.LabelField("SeaLevel", new GUIContent(GaiaSessionManager.GetSessionManager(false).GetSeaLevel().ToString() + " m"),showHelp);
            float newSeaLEvel = m_editorUtils.Slider("SeaLevel", GaiaSessionManager.GetSessionManager(false).GetSeaLevel(), 0, m_stamper.m_cachedTerrain.terrainData.size.y,showHelp);
            GaiaSessionManager.GetSessionManager(false).SetSeaLevel(newSeaLEvel);
            showSeaLevelPlane = m_editorUtils.Toggle("ShowSeaLevelPlane", m_stamper.m_showSeaLevelPlane, showHelp);
            showSeaLevelinStampPreview = m_editorUtils.Toggle("ShowSeaLevelStampPreview", m_stamper.m_showSeaLevelinStampPreview, showHelp);
            //Color gizmoColour = EditorGUILayout.ColorField(GetLabel("Gizmo Colour"), m_stamper.m_gizmoColour);
            //alwaysShow = m_editorUtils.Toggle("AlwaysShowStamper", m_stamper.m_alwaysShow, showHelp);
            showBoundingBox = m_editorUtils.Toggle("ShowBoundingBox", m_stamper.m_showBoundingBox, showHelp);
            //showRulers = m_stamper.m_showRulers = m_editorUtils.Toggle("ShowRulers", m_stamper.m_showRulers , showHelp);
            //bool showTerrainHelper = m_stamper.m_showTerrainHelper = EditorGUILayout.Toggle(GetLabel("Show Terrain Helper"), m_stamper.m_showTerrainHelper);
        }

        private void DrawMaskSettings(bool showHelp)
        {
            Rect maskRect;
            if (m_masksExpanded)
            {
                maskRect = EditorGUILayout.GetControlRect(true, m_masksReorderable.GetHeight());
                m_masksReorderable.DoList(maskRect);
            }
            else
            {
                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 1;
                m_masksExpanded = EditorGUILayout.Foldout(m_masksExpanded, ImageMaskListEditor.PropertyCount("MaskSettings", m_stamper.m_settings.m_imageMasks, m_editorUtils), true);
                maskRect = GUILayoutUtility.GetLastRect();
                EditorGUI.indentLevel = oldIndent;
            }

            m_editorUtils.Panel("MaskBaking", DrawMaskBaking, false);





                // distanceMask = m_editorUtils.CurveField("DistanceMask", m_stamper.m_distanceMask, showHelp);
                // distanceMaskInfluence = (GaiaConstants.MaskInfluence)m_editorUtils.EnumPopup("DistanceMaskInfluence", m_stamper.m_distanceMaskInfluence, showHelp);

                ////distanceMaskInfluence = (Gaia.GaiaConstants.MaskInfluence)EditorGUILayout.EnumPopup(GetLabel("Distance Mask Influence"), m_stamper.m_distanceMaskInfluence);
                // areaMaskMode = (Gaia.GaiaConstants.ImageFitnessFilterMode)m_editorUtils.EnumPopup("AreaMask", m_stamper.m_areaMaskMode, showHelp);
                // areaMaskInfluence = (Gaia.GaiaConstants.MaskInfluence)m_editorUtils.EnumPopup("AreaMaskInfluence", m_stamper.m_areaMaskInfluence, showHelp);

                // if (areaMaskMode != GaiaConstants.ImageFitnessFilterMode.None)
                // {
                //     imageMask = (Texture2D)m_editorUtils.ObjectField("ImageMask", m_stamper.m_imageMask, typeof(Texture2D), false, showHelp);
                // }
                // if (areaMaskMode == GaiaConstants.ImageFitnessFilterMode.PerlinNoise || areaMaskMode == GaiaConstants.ImageFitnessFilterMode.RidgedNoise ||
                //      areaMaskMode == GaiaConstants.ImageFitnessFilterMode.BillowNoise)
                // {
                //     noiseMaskSeed = m_editorUtils.Slider("NoiseSeed", noiseMaskSeed, 0f, 65000f, showHelp);
                //     noiseMaskOctaves = m_editorUtils.IntSlider("Octaves", noiseMaskOctaves, 1, 12, showHelp);
                //     noiseMaskPersistence = m_editorUtils.Slider("Persistence", noiseMaskPersistence, 0f, 1f, showHelp);
                //     noiseMaskFrequency = m_editorUtils.Slider("Frequency", noiseMaskFrequency, 0f, 1f, showHelp);
                //     noiseMaskLacunarity = m_editorUtils.Slider("Lacunarity", noiseMaskLacunarity, 1.5f, 3.5f, showHelp);
                //     noiseZoom = m_editorUtils.Slider("Zoom", noiseZoom, 1f, 1000f, showHelp);
                // }

                // if (areaMaskMode != GaiaConstants.ImageFitnessFilterMode.None)
                // {
                //     imageMaskSmoothIterations = m_editorUtils.IntSlider("SmoothMask", m_stamper.m_imageMaskSmoothIterations, 0, 20, showHelp);
                //     imageMaskNormalise = m_editorUtils.Toggle("NormaliseMask", m_stamper.m_imageMaskNormalise, showHelp);
                //     imageMaskInvert = m_editorUtils.Toggle("InvertMask", m_stamper.m_imageMaskInvert, showHelp);
                //     imageMaskFlip = m_editorUtils.Toggle("FlipMask", m_stamper.m_imageMaskFlip, showHelp);
                // }
            }

        private void DrawMaskBaking(bool showHelp)
        {
            if (m_stamper.m_cachedMaskTexture != null)
            {
                Rect previewRect = EditorGUILayout.GetControlRect();
                float size = previewRect.width - EditorGUIUtility.labelWidth;
                previewRect.x = previewRect.x + EditorGUIUtility.labelWidth;
                previewRect.width = size;
                previewRect.height = size;
                EditorGUILayout.BeginVertical();
                EditorGUI.DrawPreviewTexture(previewRect, m_stamper.m_cachedMaskTexture);
                EditorGUILayout.EndVertical();
                GUILayout.Space(Mathf.Abs(previewRect.height) *1.3f);
            }
            m_maskBakingResolution = m_editorUtils.IntField("MaskBakingResolution", m_maskBakingResolution, showHelp);
            if (m_maskBakingPath == "")
            {
                m_maskBakingPath = GaiaDirectories.GetMaskDirectory().Replace("Assets", Application.dataPath) + "BakedStamperMask.exr";
            }
            //EditorGUILayout.BeginHorizontal();
            //m_editorUtils.TextNonLocalized("Filename:"  + m_maskBakingPath);
            //if (GUILayout.Button("Select..."))
            //{
            //    m_maskBakingPath = EditorUtility.SaveFilePanel("Save baked mask as..", GaiaDirectories.GetMaskDirectory(), "BakedStamperMask", "exr");
            //}
            //EditorGUILayout.EndHorizontal();

            


            if (GUILayout.Button(GetLabel("Export Mask")))
            {
                string saveFilePath = EditorUtility.SaveFilePanel("Save Mask as..", GaiaDirectories.GetUserStampDirectory(), "MyMask", "exr");
                float widthfactor = m_stamper.m_cachedTerrain.terrainData.size.x / 100f;
                ImageProcessing.BakeMaskStack(m_stamper.m_settings.m_imageMasks, m_stamper.m_cachedTerrain, m_stamper.transform, m_stamper.m_settings.m_width * widthfactor, m_maskBakingResolution, saveFilePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }


        private void DrawStampSettings(bool showHelp)
        {
            //if (m_stamper.m_stampOperation != GaiaConstants.FeatureOperation.HydraulicErosion)
            //{
            //    feature = (Texture2D)EditorGUILayout.ObjectField(GetLabel("Stamp Preview"), m_stamper.m_stampImage, typeof(Texture2D), false);
            //}

            if (operation == GaiaConstants.FeatureOperation.BlendHeight)
            {
                blendStrength = EditorGUILayout.Slider(GetLabel("Blend Strength"), m_stamper.m_blendStrength, 0f, 1f);
            }

            //GUILayout.Label(stamper.m_feature, GUILayout.Width(200f), GUILayout.Height(200f) );
            //heightModifier = EditorGUILayout.CurveField(GetLabel("Transform Height"), m_stamper.m_heightModifier);
            //smoothIterations = EditorGUILayout.IntSlider(GetLabel("Smooth Stamp"), m_stamper.m_smoothIterations, 0, 10);
            //normaliseStamp = EditorGUILayout.Toggle(GetLabel("Normalise Stamp"), m_stamper.m_normaliseStamp);
            ////Note that stamp inversion is handled by inverting the brush strength before passing it into the 
            ////preview / stamp shader - faster this way than converting the heightmap
            //invertStamp = EditorGUILayout.Toggle(GetLabel("Invert Stamp"), m_stamper.m_invertStamp);
            //Only enable the base level settings for these operation types. The base level does not add value for the other op types.
            baseLevelEnabled = m_stamper.m_settings.m_operation == GaiaConstants.FeatureOperation.AddHeight ||
                                    m_stamper.m_settings.m_operation == GaiaConstants.FeatureOperation.SubtractHeight ||
                                    m_stamper.m_settings.m_operation == GaiaConstants.FeatureOperation.RaiseHeight ||
                                    m_stamper.m_settings.m_operation == GaiaConstants.FeatureOperation.LowerHeight;

            if (baseLevelEnabled)
            {
                baseLevel = EditorGUILayout.Slider(GetLabel("Base Level"), m_stamper.m_settings.m_baseLevel, 0f, 1f);
                stampBase = EditorGUILayout.Toggle(GetLabel("Stamp Base"), m_stamper.m_settings.m_drawStampBase);
                adaptiveBase = EditorGUILayout.Toggle(GetLabel("Adaptive Base"), m_stamper.m_settings.m_adaptiveBase);
                showBase = EditorGUILayout.Toggle(GetLabel("Show Base"), m_stamper.m_showBase);
            }
        }

        private void DrawLocation(bool showHelp)
        {
            //GUILayout.Label("Operation:", EditorStyles.boldLabel);
            bool GUIwasEnabled = GUI.enabled;
            m_stamper.m_activacteLocationSliders = m_editorUtils.Toggle("ActivateLocationSliders", m_stamper.m_activacteLocationSliders);
            if(!m_stamper.m_activacteLocationSliders)
            {
                m_stamper.m_settings.m_x = m_stamper.transform.position.x;
                m_stamper.m_settings.m_y = m_stamper.transform.position.y;
                m_stamper.m_settings.m_z = m_stamper.transform.position.z;
                m_stamper.m_settings.m_rotation = m_stamper.transform.rotation.y;
                m_stamper.m_settings.m_width = Mathf.Max(m_stamper.transform.localScale.x, m_stamper.transform.localScale.z);
                m_stamper.m_settings.m_height = m_stamper.transform.localScale.y;
                GUI.enabled = false;
            }
            

                x = EditorGUILayout.Slider(GetLabel("Position X"), m_stamper.m_settings.m_x, m_minX, m_maxX);
                y = m_stamper.m_settings.m_y;
                y = EditorGUILayout.Slider(GetLabel("Position Y"), m_stamper.m_settings.m_y, m_minY, m_maxY);
                z = EditorGUILayout.Slider(GetLabel("Position Z"), m_stamper.m_settings.m_z, m_minZ, m_maxZ);
                rotation = EditorGUILayout.Slider(GetLabel("Rotation"), m_stamper.m_settings.m_rotation, -180f, 180f);
                Bounds bounds = new Bounds();
                Gaia.TerrainHelper.GetTerrainBounds(ref bounds);
                width = EditorGUILayout.Slider(GetLabel("Width"), m_stamper.m_settings.m_width, 0.1f, Mathf.Max(bounds.size.x, bounds.size.z));
                height = EditorGUILayout.Slider(GetLabel("Height"), m_stamper.m_settings.m_height, 0.1f, 100f);

            GUI.enabled = GUIwasEnabled;
          
        }

        private void DrawOperation(bool showHelp)
        {
            operation = (GaiaConstants.FeatureOperation)EditorGUILayout.Popup(GetLabel("Operation Type"), (int)m_stamper.m_settings.m_operation, GaiaConstants.FeatureOperationNames);

            //Drawing the "special" controls for the respective operation type.

            switch (m_stamper.m_settings.m_operation)
            {
                case GaiaConstants.FeatureOperation.RaiseHeight:
                    m_editorUtils.InlineHelp("RaiseHeightIntro", showHelp);
                    DrawStampSettings(showHelp);
                    break;
                case GaiaConstants.FeatureOperation.LowerHeight:
                    m_editorUtils.InlineHelp("LowerHeightIntro", showHelp);

                    DrawStampSettings(showHelp);
                    break;
                case GaiaConstants.FeatureOperation.BlendHeight:
                    m_editorUtils.InlineHelp("BlendHeightIntro", showHelp);
                    DrawStampSettings(showHelp);
                    break;
                case GaiaConstants.FeatureOperation.SetHeight:
                    m_editorUtils.InlineHelp("SetHeightIntro", showHelp);
                    DrawStampSettings(showHelp);
                    break;
                case GaiaConstants.FeatureOperation.AddHeight:
                    m_editorUtils.InlineHelp("AddHeightIntro", showHelp);
                    DrawStampSettings(showHelp);
                    break;
                case GaiaConstants.FeatureOperation.SubtractHeight:
                    m_editorUtils.InlineHelp("SubtractHeightIntro", showHelp);
                    DrawStampSettings(showHelp);
                    break;

                case GaiaConstants.FeatureOperation.HydraulicErosion:
                    m_editorUtils.InlineHelp("HydraulicErosionIntro", showHelp);
                    DrawHydraulicErosionControls(showHelp);
                    break;
                case GaiaConstants.FeatureOperation.Contrast:
                    m_editorUtils.InlineHelp("ContrastIntro", showHelp);
                    DrawContrastControls(showHelp);
                    break;
                case GaiaConstants.FeatureOperation.Terrace:
                    m_editorUtils.InlineHelp("TerraceIntro", showHelp);
                    DrawTerraceControls(showHelp);
                    break;
                case GaiaConstants.FeatureOperation.SharpenRidges:
                    m_editorUtils.InlineHelp("SharpenRidgesIntro", showHelp);
                    DrawSharpenRidgesControls(showHelp);
                    break;
                case GaiaConstants.FeatureOperation.HeightTransform:
                    m_editorUtils.InlineHelp("HeightTransformIntro", showHelp);
                    DrawHeightTransformControls(showHelp);
                    break;
                case GaiaConstants.FeatureOperation.PowerOf:
                    m_editorUtils.InlineHelp("PowerOfIntro", showHelp);
                    DrawPowerOfControls(showHelp);
                    break;
                case GaiaConstants.FeatureOperation.Smooth:
                    m_editorUtils.InlineHelp("SmoothIntro", showHelp);
                    DrawSmoothControls(showHelp);
                    break;
                default:
                    DrawStampSettings(showHelp);
                    //m_editorUtils.Panel("StampSettings", DrawStampSettings, true);
                    break;
            }

            DrawMaskSettings(showHelp);
        }

        private void DrawSmoothControls(bool showHelp)
        {
            m_stamper.m_settings.m_smoothVerticality = EditorGUILayout.Slider(m_editorUtils.GetContent("MaskSmoothVerticality"), m_stamper.m_settings.m_smoothVerticality, -1f, 1f);
            m_stamper.m_settings.m_smoothBlurRadius = EditorGUILayout.Slider(m_editorUtils.GetContent("MaskSmoothBlurRadius"), m_stamper.m_settings.m_smoothBlurRadius, 0f, 30f);
        }

        private void DrawStamperInfo(bool showHelp)
        {
            string message = m_editorUtils.GetTextValue("StamperIntro"); ;

            EditorGUILayout.HelpBox(message, MessageType.Info, true);
        }

        private void DrawPowerOfControls(bool showHelp)
        {
            m_stamper.m_settings.m_powerOf = EditorGUILayout.Slider("PowerOf", m_stamper.m_settings.m_powerOf, 0.01f, 5.0f);
        }

        private void DrawContrastControls(bool showHelp)
        {
            m_stamper.m_settings.m_contrastFeatureSize = EditorGUILayout.Slider("Feature Size",m_stamper.m_settings.m_contrastFeatureSize, 1.0f, 100.0f);
            m_stamper.m_settings.m_contrastStrength = EditorGUILayout.Slider("Strength", m_stamper.m_settings.m_contrastStrength, 0.01f, 10.0f);
        }

        private void DrawHeightTransformControls(bool showHelp)
        {
            m_stamper.m_settings.m_heightTransformCurve = m_editorUtils.CurveField("MaskHeightTransformCurve", m_stamper.m_settings.m_heightTransformCurve);
        }

        private void DrawTerraceControls(bool helpEnabled)
        {
            m_stamper.m_settings.m_terraceCount = EditorGUILayout.Slider("Terrace Count", m_stamper.m_settings.m_terraceCount, 2.0f, 1000.0f);
            //m_stamper.m_terraceJitterCount = EditorGUILayout.Slider("Jitter Amount", m_stamper.m_terraceJitterCount, 0.0f, 1.0f);
            m_stamper.m_settings.m_terraceBevelAmountInterior = EditorGUILayout.Slider("Bevel Amount", m_stamper.m_settings.m_terraceBevelAmountInterior, 0.0f, 1.0f);
        }

        private void DrawSharpenRidgesControls(bool helpEnabled)
        {
            m_stamper.m_settings.m_sharpenRidgesMixStrength = EditorGUILayout.Slider("Sharpness", m_stamper.m_settings.m_sharpenRidgesMixStrength, 0, 1);
            m_stamper.m_settings.m_sharpenRidgesIterations = EditorGUILayout.Slider("Iterations", m_stamper.m_settings.m_sharpenRidgesIterations, 0, 20);
        }

        private void DrawHydraulicErosionControls(bool helpEnabled)
        {
            m_stamper.m_settings.m_erosionSimScale = m_editorUtils.Slider("ErosionSimScale", m_stamper.m_settings.m_erosionSimScale, 0.0f, 100f);
            m_stamper.m_settings.m_erosionHydroTimeDelta = m_editorUtils.Slider("ErosionHydroTimeDelta", m_stamper.m_settings.m_erosionHydroTimeDelta, 0.0f, 0.1f);
            m_stamper.m_settings.m_erosionHydroIterations = m_editorUtils.IntSlider("ErosionHydroIterations", m_stamper.m_settings.m_erosionHydroIterations, 1, 500);


            EditorGUI.indentLevel++;
            m_ShowAdvancedUI = EditorGUILayout.Foldout(m_ShowAdvancedUI, "Advanced");

            if (m_ShowAdvancedUI)
            {
                //m_ErosionSettings.m_IterationBlendScalar.DrawInspectorGUI();
               //m_ErosionSettings.m_GravitationalConstant = EditorGUILayout.Slider(ErosionStyles.m_GravitationConstant, m_ErosionSettings.m_GravitationalConstant, 0.0f, -100.0f);

                EditorGUI.indentLevel++;
                m_ShowThermalUI = EditorGUILayout.Foldout(m_ShowThermalUI, "Thermal Smoothing");
                if (m_ShowThermalUI)
                {
                    //m_ErosionSettings.m_DoThermal = EditorGUILayout.Toggle(ErosionStyles.m_DoThermal, m_ErosionSettings.m_DoThermal);
                    m_stamper.m_settings.m_erosionThermalTimeDelta = m_editorUtils.Slider("ErosionThermalTimeDelta", m_stamper.m_settings.m_erosionThermalTimeDelta, 0, 0.01f);
                    m_stamper.m_settings.m_erosionThermalIterations = m_editorUtils.IntSlider("ErosionThermalIterations", m_stamper.m_settings.m_erosionThermalIterations, 0, 100);
                    m_stamper.m_settings.m_erosionThermalReposeAngle = m_editorUtils.IntSlider("ErosionThermalReposeAngle", m_stamper.m_settings.m_erosionThermalReposeAngle, 0, 90);
                }

                m_ShowWaterUI = EditorGUILayout.Foldout(m_ShowWaterUI, "Water Transport");
                if (m_ShowWaterUI)
                {
                    //m_ErosionSettings.m_WaterLevelScale = EditorGUILayout.Slider(ErosionStyles.m_WaterLevelScale, m_ErosionSettings.m_WaterLevelScale, 0.0f, 100.0f);
                    m_stamper.m_settings.m_erosionPrecipRate = m_editorUtils.Slider("ErosionPrecipRate", m_stamper.m_settings.m_erosionPrecipRate, 0f, 1f);
                    m_stamper.m_settings.m_erosionEvaporationRate = m_editorUtils.Slider("ErosionEvaporationRate", m_stamper.m_settings.m_erosionEvaporationRate, 0f, 1f);
                    m_stamper.m_settings.m_erosionFlowRate = m_editorUtils.Slider("ErosionFlowRate", m_stamper.m_settings.m_erosionFlowRate, 0f, 1f);
                }

                m_ShowSedimentUI = EditorGUILayout.Foldout(m_ShowSedimentUI, "Sediment Transport");
                if (m_ShowSedimentUI)
                {
                    //m_ErosionSettings.m_SedimentScale = EditorGUILayout.Slider(ErosionStyles.m_SedimentScale, m_ErosionSettings.m_SedimentScale, 0.0f, 10.0f);
                    m_stamper.m_settings.m_erosionSedimentCapacity = m_editorUtils.Slider("ErosionSedimentCapacity", m_stamper.m_settings.m_erosionSedimentCapacity, 0f, 1f);
                    m_stamper.m_settings.m_erosionSedimentDepositRate = m_editorUtils.Slider("ErosionSedimentDepositRate", m_stamper.m_settings.m_erosionSedimentDepositRate, 0f, 1f);
                    m_stamper.m_settings.m_erosionSedimentDissolveRate = m_editorUtils.Slider("ErosionSedimentDissolveRate", m_stamper.m_settings.m_erosionSedimentDissolveRate, 0f, 1f);
                }

                m_ShowRiverBankUI = EditorGUILayout.Foldout(m_ShowRiverBankUI, "Riverbank");
                if (m_ShowRiverBankUI)
                {
                    m_stamper.m_settings.m_erosionRiverBankDepositRate = m_editorUtils.Slider("ErosionRiverBankDepositRate", m_stamper.m_settings.m_erosionRiverBankDepositRate, 0f, 10f);
                    m_stamper.m_settings.m_erosionRiverBankDissolveRate = m_editorUtils.Slider("ErosionRiverBankDissolveRate", m_stamper.m_settings.m_erosionRiverBankDissolveRate, 0f, 10f);
                    m_stamper.m_settings.m_erosionRiverBedDepositRate = m_editorUtils.Slider("ErosionRiverBedDepositRate", m_stamper.m_settings.m_erosionRiverBedDepositRate, 0f, 10f);
                    m_stamper.m_settings.m_erosionRiverBedDissolveRate = m_editorUtils.Slider("ErosionRiverBedDissolveRate", m_stamper.m_settings.m_erosionRiverBedDissolveRate, 0f, 10f);
                }
            }

        }


        private Vector2 ConvertPositonToTerrainUV(Terrain terrain, Vector2 worldSpacePosition)
        {
            float u = (worldSpacePosition.x - terrain.transform.position.x) / terrain.terrainData.size.x; 
            float v = (worldSpacePosition.y - terrain.transform.position.z) / terrain.terrainData.size.z;
            return new Vector2(u,v);
        }

        private void OnSceneGUI()
        {
            // dont render preview if this isnt a repaint. losing performance if we do
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            m_stamper.DrawStampPreview();
        }


        /// <summary>
        /// Display a progress bar
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        void ProgressBar(string label, float value)
        {
            // Get a rect for the progress bar using the same margins as a textfield:
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Get a content label - look the tooltip up if possible
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        GUIContent GetLabel(string name)
        {
            string tooltip = "";
            if (m_showTooltips && m_tooltips.TryGetValue(name, out tooltip))
            {
                return new GUIContent(name, tooltip);
            }
            else
            {
                return new GUIContent(name);
            }
        }


        /// <summary>
        /// The tooltips
        /// </summary>
        static Dictionary<string, string> m_tooltips = new Dictionary<string,string>
        {
            { "Stamp Preview", "Preview texture for the feature being stamped. Drag and drop preview textures here." },
            { "Transform Height", "Pre-process and modify the stamp height. Can be used to further refine stamp shapes."},
            { "Invert Stamp", "Invert the stamp. Good for carving lakes and valleys."},
            { "Base Level", "Base level of the stamp."},
            { "Ground Base", "Sticks the stamp base level to the terrain base."},
            { "Stamp Base", "Applies stamp below base level, or not. Good for excluding low lying areas from the stamp."},
            { "Show Base", "Shows the base level as a yellow plane."},
            { "Show Bounding Box", "Show a box around the stamp, useful to better visualize the position of the stamp."},
            { "Normalise Stamp", "Modify stamp heights to use full height range. Essential for correct height settings when using Stencil Heights operation."},
            { "Operation Type", "The way this stamp will be applied to the terrain.\nRaise - Adds stamp to terrain if stamp height greater than terrain height.\nLower - Cuts stamp from terrain if stamp height lower than terrain height. \nBlend - Blend between terrain and stamp.\nDifference - Calculate height difference.\nStencil - Adjust by stencil height - normalise first."},
            { "Blend Strength", "Blend between terrain and stamp. 0 - all terrain - 1 - all stamp."},
            { "Stencil Height", "Adjusted height in meters that a normalised stamp will be applied to the terrain."},
            { "Distance Mask", "Masks the effect of the stamp over distance from center. Left hand side of curve is centre of stamp, right hand side of curve is outer edge of stamp. Set right hand side to zero to blend edges of stamp into existing terrain."},
            { "Area Mask", "Masks the effect of the stamp using the strength of the texture or noise function provided. A value of 1 means apply full effect, a value of 0 means apply no effect. Visually this is much the same ways as a greyscale image mask. If using a terrain texture, then paint on the terrain with the selected texture, and the painted area will be used as the mask."},
            
            { "Noise Seed", "The seed value for the noise function - the same seed will always generate the same noise for a given set of parameters."},
            { "Octaves", "The amount of detail in the noise - more octaves mean more detail and longer calculation time."},
            { "Persistence", "The roughness of the noise. Controls how quickly amplitudes diminish for successive octaves. 0..1."},
            { "Frequency", "The frequency of the first octave."},
            { "Lacunarity", "The frequency multiplier between successive octaves. Experiment between 1.5 - 3.5."},
            { "Zoom", "The zoom level of the noise. Larger zooms display the noise over larger areas."},

            { "Image Mask", "The image to use as the area mask."},
            { "Invert Mask", "Invert the image used as the area mask before using it."},
            { "Smooth Mask", "Smooth the mask before applying it. This is a nice way to clean noise up in the mask, or to soften the edges of the mask."},
            { "Normalise Mask", "Normalise the mask before applying it. Ensures that the full dynamic range of the mask is used."},
            { "Flip Mask", "Flip the mask on its x and y axis mask before applying it. Useful sometimes to match the unity terrain as this is flipped internally."},
            { "Seed", "The unique seed for this spawner. This will cause all subseqent spawns to exactly match this spawn" },
            { "Smooth Stamp", "Smooth the stamp before applying it to the terrain. Good for cleaning up noisy stamps."},
            { "Preview Material", "The material used to display the Preview mesh. Has no effect other than to make the preview viewable."},
            { "Resources", "The terrains rsources file. Changing sea level will update that resource files sea level, and this will impact where the spawners spawn."},
            { "Texture Spawner", "The texure spawner. An optional feature that enables the spawn button. Enables you to do a texure spawn and saves you from having to select the texture spawner manually."},
            { "Position X", "X location of stamp centre." },
            { "Position Y", "Y location of stamp centre." },
            { "Position Z", "Z location of stamp centre." },
            { "Width", "Modify the width of the stamp." },
            { "Height", "Modify the height of this stamp." },
            { "Rotation", "Modify the rotation of this stamp." },
            { "Stick To Groud", "Stick the stamp to the base of the terrain." },
            { "Always Show Stamper", "Always show the stamper, even when something else is selected, otherwise hide it when something else is selected." },
            { "Gizmo Colour", "The colour of the gizmo that is drawn to show the size of the stamp, used to make positioning easier." },
            { "Sea Level", "The sea level in meters. Changes to this are applied back to the resources file, and then impact the spawners, so treat with care, and only change before spawning." },
            { "Show Sea Level", "Show sea level." },
            { "Show Rulers", "Show rulers." },
            { "Show Terrain Helper", "Show the terrain helper buttons - treat these with care!" },
            { "Flatten", "Flatten all terrains." },
            { "Smooth", "Smooth all terrains." },
            { "Clear Trees", "Clear trees on all terrains." },
            { "Clear Details", "Clear details on all terrains." },
            { "Ground", "Align the stamp to the base of the terrain." },
            { "Fit To Terrain", "Fit the stamp to the terrain." },
            { "Stamp", "Apply this stamp to the terrain." },
            { "Preview", "Show or hide the stamp preview mesh." },
            { "Undo", "Undo the last stamp." },
            { "Redo", "Redo the last stamp." },
        };

    }
}