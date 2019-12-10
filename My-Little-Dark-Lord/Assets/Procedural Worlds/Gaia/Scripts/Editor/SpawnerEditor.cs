using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using PWCommon2;
using UnityEditorInternal;
using Gaia.Internal;
using static Gaia.GaiaConstants;
using System.Linq;

namespace Gaia
{
    [CustomEditor(typeof(Spawner))]
    public class SpawnerEditor : PWEditor, IPWEditor
    {
        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        Spawner m_spawner;
        private EditorUtils m_editorUtils;
        DateTime m_timeSinceLastUpdate = DateTime.Now;
        bool m_startedUpdates = false;
        private bool m_showTooltips = true;
        private bool m_spawnerMaskListExpanded = true;

        private bool[] m_spawnRuleMaskListExpanded;


        private ReorderableList[] m_reorderableRuleMasksLists;


        //The Spawner Editor is a complex editor drawing settings for resources, spawner settings, reorderable mask lists, etc.
        //For all this to work it is sometimes required to store the current thing that is "Being Drawn" in a temporary variable so it becomes accessible elsewhere.
        private ImageMask[] m_maskListBeingDrawn;
        private CollisionMask[] m_collisionMaskListBeingDrawn;
        private int m_spawnRuleIndexBeingDrawn;
        private int m_spawnRuleMaskIndexBeingDrawn;
        private ResourceProtoTexture m_textureResourcePrototypeBeingDrawn;
        private ResourceProtoTree m_treeResourcePrototypeBeingDrawn;
        private ResourceProtoDetail m_terrainDetailPrototypeBeingDrawn;
        private ResourceProtoGameObject m_gameObjectResourcePrototypeBeingDrawn;
        //private ResourceProtoGeNaSpawner m_geNaSpawnerPrototypeBeingDrawn;

        //private EditorUtilsOLD m_editorUtils = new EditorUtilsOLD();
        private UnityEditorInternal.ReorderableList m_reorderableSpawnerMaskList;

        private float m_lastXPos;
        private float m_lastZPos;

        private bool m_activatePreviewRequested;
        private long m_activatePreviewTimeStamp;
        private Color visColor;

        private GUIStyle m_spawnRulesHeader;
        private GUIStyle m_singleSpawnRuleHeader;

        private string m_SaveAndLoadMessage = "";
        private MessageType m_SaveAndLoadMessageType;
        private GaiaSettings m_gaiaSettings;
        private bool m_rulePanelHelpActive;
        private List<Texture2D> m_tempTextureList = new List<Texture2D>();
        private int m_drawResourcePreviewRuleId;
        private string m_ResourceManagementMessage = "";
        private MessageType m_ResourceManagementMessageType;


        /// <summary>
        /// This is a color used to initialize all spawning rules with a progressing palette.
        /// </summary>
        private Color m_rollingColor = Color.red;

        private bool m_changesMadeSinceLastSave;
        private Color m_dirtyColor;
        private Color m_normalBGColor;

        void OnEnable()
        {
            //Get the settings and update tooltips
            if (m_gaiaSettings == null)
            {
                m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            }
            if (m_gaiaSettings != null)
            {
                m_showTooltips = m_gaiaSettings.m_showTooltips;
            }

            //Get our spawner
            m_spawner = (Spawner)target;

            //make sure we got settings
            if (m_spawner.m_settings == null)
            {
                m_spawner.m_settings = ScriptableObject.CreateInstance<SpawnerSettings>();
                m_reorderableRuleMasksLists = null;
                serializedObject.ApplyModifiedProperties();
            }



            //Init editor utils
            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }

            CreateMaskLists();


            //Clean up any rules that relate to missing resources
            CleanUpRules();

            //Do some simple sanity checks
            if (m_spawner.m_rndGenerator == null)
            {
                m_spawner.m_rndGenerator = new Gaia.XorshiftPlus(m_spawner.m_seed);
            }

            if (m_spawner.m_spawnFitnessAttenuator == null)
            {
                m_spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1.0f), new Keyframe(1f, 0.0f));
            }

            if (m_spawner.m_spawnCollisionLayers.value == 0)
            {
                m_spawner.m_spawnCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();
            }

            m_lastXPos = m_spawner.transform.position.x;
            m_lastZPos = m_spawner.transform.position.z;

            if (m_spawnRulesHeader == null || m_singleSpawnRuleHeader == null)
            {
                m_spawnRulesHeader = new GUIStyle();
                m_spawnRulesHeader.overflow = new RectOffset(2, 2, 2, 2);
                m_singleSpawnRuleHeader = new GUIStyle(m_spawnRulesHeader);

                // Setup colors for Unity Pro
                if (EditorGUIUtility.isProSkin)
                {
                    m_spawnRulesHeader.normal.background = GaiaUtils.GetBGTexture(GaiaUtils.GetColorFromHTML("2d2d2dff"), m_tempTextureList);
                    m_singleSpawnRuleHeader.normal.background = GaiaUtils.GetBGTexture(GaiaUtils.GetColorFromHTML("2d2d4cff"), m_tempTextureList);
                }
                // or Unity Personal
                else
                {
                    m_spawnRulesHeader.normal.background = GaiaUtils.GetBGTexture(GaiaUtils.GetColorFromHTML("a2a2a2ff"), m_tempTextureList);
                    m_singleSpawnRuleHeader.normal.background = GaiaUtils.GetBGTexture(GaiaUtils.GetColorFromHTML("a2a2c1ff"), m_tempTextureList);
                }
            }

            if (GaiaWater.DoesWaterExist())
            {
                m_spawner.m_showSeaLevelPlane = false;
            }

            GaiaLighting.SetPostProcessingStatus(false);

            m_dirtyColor = GaiaUtils.GetColorFromHTML("#FF666666");
            m_normalBGColor = GUI.backgroundColor;
            StartEditorUpdates();
        }

        
        private void CreateMaskLists()
        {
            //Create the spawner mask list
            CreateSpawnerMaskList();

            //Create the individual spawn rule mask lists
            m_spawnRuleMaskListExpanded = new bool[m_spawner.m_settings.m_spawnerRules.Count];
            m_reorderableRuleMasksLists = new ReorderableList[m_spawner.m_settings.m_spawnerRules.Count];

            for (int i = 0; i < m_spawner.m_settings.m_spawnerRules.Count; i++)
            {
                m_reorderableRuleMasksLists[i] = CreateSpawnRuleMaskList(m_reorderableRuleMasksLists[i], m_spawner.m_settings.m_spawnerRules[i].m_imageMasks);
                m_spawnRuleMaskListExpanded[i] = true;
            }
        }

        void OnDisable()
        {
            GaiaLighting.SetPostProcessingStatus(true);
        }

        void OnDestroy()
        {
            //check if we opened a stamp selection window from this spawner, and if yes, close it down
            var allWindows = Resources.FindObjectsOfTypeAll<GaiaStampSelectorEditorWindow>();
            for (int i = allWindows.Length - 1; i >= 0; i--)
            {
                //Check General Masks first

                foreach (ImageMask imageMask in m_spawner.m_settings.m_imageMasks)
                {
                    if (allWindows[i].m_editedImageMask == imageMask)
                    {
                        allWindows[i].Close();
                    }
                }

                //Then the rule specific masks

                foreach (SpawnRule spawnRule in m_spawner.m_settings.m_spawnerRules)
                {
                    foreach (ImageMask imageMask in spawnRule.m_imageMasks)
                    {
                        if (allWindows[i].m_editedImageMask == imageMask)
                        {
                            allWindows[i].Close();
                        }
                    }
                }
            }

            for (int i = 0; i < m_tempTextureList.Count; i++)
            {
                UnityEngine.Object.DestroyImmediate(m_tempTextureList[i]);
            }

        }

        #region SPAWN RULE MASK LIST MANAGEMENT

        /// <summary>
        /// Creates the reorderable collision mask list for collision masks in the spawn rules.
        /// </summary>
        public ReorderableList CreateSpawnRuleCollisionMaskList(ReorderableList list, CollisionMask[] collisionMasks)
        {
            list = new ReorderableList(collisionMasks, typeof(CollisionMask), true, true, true, true);
            list.elementHeightCallback = OnElementHeightCollisionMaskList;
            list.drawElementCallback = DrawSpawnRuleCollisionMaskElement;
            list.drawHeaderCallback = DrawSpawnRuleCollisionMaskListHeader;
            list.onAddCallback = OnAddSpawnRuleCollisionMaskListEntry;
            list.onRemoveCallback = OnRemoveSpawnRuleCollisionMaskMaskListEntry;
            return list;
        }

        private void OnRemoveSpawnRuleCollisionMaskMaskListEntry(ReorderableList list)
        {
            //find spawn rule index & mask index which are being edited, so we know who this list of collision masks belongs to
            int maskIndex = -99;
            int spawnRuleIndex = FindSpawnRuleIndexByReorderableCollisionMaskList(list,ref maskIndex);
            m_spawner.m_settings.m_spawnerRules[spawnRuleIndex].m_imageMasks[maskIndex].m_collisionMasks = CollisionMaskListEditor.OnRemoveMaskListEntry(m_spawner.m_settings.m_spawnerRules[spawnRuleIndex].m_imageMasks[maskIndex].m_collisionMasks, list.index);
            list.list = m_spawner.m_settings.m_spawnerRules[spawnRuleIndex].m_imageMasks[maskIndex].m_collisionMasks;
        }

        private void OnAddSpawnRuleCollisionMaskListEntry(ReorderableList list)
        {
            //find spawn rule index & mask index which are being edited, so we know who this list of collision masks belongs to
            int maskIndex = -99;
            int spawnRuleIndex = FindSpawnRuleIndexByReorderableCollisionMaskList(list, ref maskIndex);
            m_spawner.m_settings.m_spawnerRules[spawnRuleIndex].m_imageMasks[maskIndex].m_collisionMasks = CollisionMaskListEditor.OnAddMaskListEntry(m_spawner.m_settings.m_spawnerRules[spawnRuleIndex].m_imageMasks[maskIndex].m_collisionMasks);
            list.list = m_spawner.m_settings.m_spawnerRules[spawnRuleIndex].m_imageMasks[maskIndex].m_collisionMasks;
        }

        private void DrawSpawnRuleCollisionMaskListHeader(Rect rect)
        {
            m_spawner.m_settings.m_spawnerRules[m_spawnRuleIndexBeingDrawn].m_imageMasks[m_spawnRuleMaskIndexBeingDrawn].m_collisionMaskExpanded = CollisionMaskListEditor.DrawFilterListHeader(rect, m_spawner.m_settings.m_spawnerRules[m_spawnRuleIndexBeingDrawn].m_imageMasks[m_spawnRuleMaskIndexBeingDrawn].m_collisionMaskExpanded, m_spawner.m_settings.m_spawnerRules[m_spawnRuleIndexBeingDrawn].m_imageMasks[m_spawnRuleMaskIndexBeingDrawn].m_collisionMasks, m_editorUtils);
        }

        private void DrawSpawnRuleCollisionMaskElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (m_collisionMaskListBeingDrawn != null && m_collisionMaskListBeingDrawn.Length > index && m_collisionMaskListBeingDrawn[index] !=null)
            {
                CollisionMaskListEditor.DrawMaskListElement(rect, index, m_collisionMaskListBeingDrawn[index], m_editorUtils, Terrain.activeTerrain, GaiaConstants.FeatureOperation.Contrast);
            }
        }

        private float OnElementHeightCollisionMaskList(int index)
        {
            return CollisionMaskListEditor.OnElementHeight(index, m_collisionMaskListBeingDrawn);
        }



        /// <summary>
        /// Creates the reorderable mask list for the individual spawn rules.
        /// </summary>
        private ReorderableList CreateSpawnRuleMaskList(ReorderableList list, ImageMask[] imageMasks)
        {
            list = new UnityEditorInternal.ReorderableList(imageMasks, typeof(ImageMask), true, true, true, true);
            list.elementHeightCallback = OnElementHeightSpawnRuleMaskList;
            list.drawElementCallback = DrawSpawnRuleMaskListElement;
            list.drawHeaderCallback = DrawSpawnRuleMaskListHeader;
            list.onAddCallback = OnAddSpawnRuleMaskListEntry;
            list.onRemoveCallback = OnRemoveSpawnRuleMaskListEntry;
            list.onReorderCallback = OnReorderSpawnRuleMaskList;

            foreach (ImageMask mask in imageMasks)
            {
                mask.m_reorderableCollisionMaskList = CreateSpawnRuleCollisionMaskList(mask.m_reorderableCollisionMaskList, mask.m_collisionMasks);
            }

            return list;
        }

        private float OnElementHeightSpawnRuleMaskList(int index)
        {
            if (m_maskListBeingDrawn.Length > 0)
            {
                return ImageMaskListEditor.OnElementHeight(index, m_maskListBeingDrawn[index]);
            }
            else
            {
                return 0f;
            }
        }

        private void OnReorderSpawnRuleMaskList(ReorderableList list)
        {
            m_spawner.m_spawnPreviewDirty = true;
            m_spawner.DrawSpawnerPreview();
        }

        private void OnRemoveSpawnRuleMaskListEntry(ReorderableList list)
        {
            //find spawn rule index that is being edited
            int spawnRuleIndex = FindSpawnRuleIndexByReorderableMaskList(list);
            m_spawner.m_settings.m_spawnerRules[spawnRuleIndex].m_imageMasks = ImageMaskListEditor.OnRemoveMaskListEntry(m_spawner.m_settings.m_spawnerRules[spawnRuleIndex].m_imageMasks, list.index);
            list.list = m_spawner.m_settings.m_spawnerRules[spawnRuleIndex].m_imageMasks;
        }

        private void OnAddSpawnRuleMaskListEntry(ReorderableList list)
        {
            //find spawn rule index that is being edited
            int spawnRuleIndex = FindSpawnRuleIndexByReorderableMaskList(list);
            m_spawner.m_settings.m_spawnerRules[spawnRuleIndex].m_imageMasks =  ImageMaskListEditor.OnAddMaskListEntry(m_spawner.m_settings.m_spawnerRules[spawnRuleIndex].m_imageMasks, m_spawner.m_maxWorldHeight, m_spawner.m_minWorldHeight, GaiaSessionManager.GetSessionManager(false).m_session.m_seaLevel);
            //set up the new collision mask inside the newly added mask
            ImageMask[] maskList = m_spawner.m_settings.m_spawnerRules[spawnRuleIndex].m_imageMasks;
            ImageMask lastElement = maskList[maskList.Length - 1];
            lastElement.m_reorderableCollisionMaskList = CreateSpawnRuleCollisionMaskList(lastElement.m_reorderableCollisionMaskList, lastElement.m_collisionMasks);
            list.list = maskList;
        }


        private void DrawSpawnRuleMaskListHeader(Rect rect)
        {
             m_spawnRuleMaskListExpanded[m_spawnRuleIndexBeingDrawn] = ImageMaskListEditor.DrawFilterListHeader(rect, m_spawnRuleMaskListExpanded[m_spawnRuleIndexBeingDrawn], m_spawner.m_settings.m_spawnerRules[m_spawnRuleIndexBeingDrawn].m_imageMasks, m_editorUtils);
        }

        int FindSpawnRuleIndexByReorderableMaskList(ReorderableList maskList)
        {
            //find texture index that is being edited
            int spawnRuleIndex = -99;

            for (int i = 0; i < m_reorderableRuleMasksLists.Length; i++)
            {
                if (m_reorderableRuleMasksLists[i] == maskList)
                {
                    spawnRuleIndex = i;
                }
            }
            return spawnRuleIndex;
        }

        int FindSpawnRuleIndexByReorderableCollisionMaskList(ReorderableList collisionMaskList, ref int maskIndex)
        {
            //find texture index that is being edited
            int spawnRuleIndex = -99;

            for (int i = 0; i < m_reorderableRuleMasksLists.Length; i++)
            {
                for (int j = 0; j < m_reorderableRuleMasksLists[i].list.Count; j++)
                {
                    if (((ImageMask)m_reorderableRuleMasksLists[i].list[j]).m_reorderableCollisionMaskList == collisionMaskList)
                    {
                        maskIndex = j;
                        spawnRuleIndex = i;
                    }
                }
            }
            return spawnRuleIndex;
        }

        private void DrawSpawnRuleMaskListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (m_maskListBeingDrawn != null && m_maskListBeingDrawn.Length > index)
            {

                ImageMask copiedImageMask = GaiaSessionManager.GetSessionManager(false).m_copiedImageMask;

                m_spawnRuleMaskIndexBeingDrawn = index;
                //bool isCopiedMask = m_maskListBeingDrawn[index] != null && m_maskListBeingDrawn[index] == m_copiedImageMask;
                MaskListButtonCommand mlbc = ImageMaskListEditor.DrawMaskListElement(rect, index, m_maskListBeingDrawn, ref m_collisionMaskListBeingDrawn, m_editorUtils, Terrain.activeTerrain, false, copiedImageMask, m_spawnRulesHeader.normal.background, m_gaiaSettings);

                switch (mlbc)
                {
                    case MaskListButtonCommand.Delete:
                        foreach (ReorderableList rl in m_reorderableRuleMasksLists)
                        {
                            if (rl.list == m_maskListBeingDrawn)
                            {
                                rl.index = index;
                                OnRemoveSpawnRuleMaskListEntry(rl);
                            }
                        }

                        break;
                    case MaskListButtonCommand.Duplicate:
                        foreach (ReorderableList rl in m_reorderableRuleMasksLists)
                        {
                            if (rl.list == m_maskListBeingDrawn)
                            {
                                
                                ImageMask newImageMask = ImageMask.Clone(m_maskListBeingDrawn[index]);
                                m_spawner.m_settings.m_spawnerRules[m_spawnRuleIndexBeingDrawn].m_imageMasks = GaiaUtils.InsertElementInArray(m_spawner.m_settings.m_spawnerRules[m_spawnRuleIndexBeingDrawn].m_imageMasks, newImageMask, index + 1);
                                rl.list = m_spawner.m_settings.m_spawnerRules[m_spawnRuleIndexBeingDrawn].m_imageMasks;
                                m_spawner.m_settings.m_spawnerRules[m_spawnRuleIndexBeingDrawn].m_imageMasks[index+1].m_reorderableCollisionMaskList = CreateSpawnRuleCollisionMaskList(m_spawner.m_settings.m_spawnerRules[m_spawnRuleIndexBeingDrawn].m_imageMasks[index + 1].m_reorderableCollisionMaskList, m_spawner.m_settings.m_spawnerRules[m_spawnRuleIndexBeingDrawn].m_imageMasks[index + 1].m_collisionMasks);
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                       
                        break;
                    case MaskListButtonCommand.Copy:
                        GaiaSessionManager.GetSessionManager(false).m_copiedImageMask = m_maskListBeingDrawn[index];
                        break;
                    case MaskListButtonCommand.Paste:
                        m_maskListBeingDrawn[index] = ImageMask.Clone(copiedImageMask);
                        //Rebuild collsion mask list with new content from the cloning
                        m_maskListBeingDrawn[index].m_reorderableCollisionMaskList = CreateSpawnRuleCollisionMaskList(m_maskListBeingDrawn[index].m_reorderableCollisionMaskList, m_maskListBeingDrawn[index].m_collisionMasks);
                        GaiaSessionManager.GetSessionManager(false).m_copiedImageMask = null;
                        break;

                }
                
                
            }
        }

        #endregion

        #region SPAWNER MASK LIST MANAGEMENT

        /// <summary>
        /// Creates the reorderable mask list for the spawner itself.
        /// </summary>
        private void CreateSpawnerMaskList()
        {
            m_reorderableSpawnerMaskList = new UnityEditorInternal.ReorderableList(m_spawner.m_settings.m_imageMasks, typeof(ImageMask), true, true, true, true);
            m_reorderableSpawnerMaskList.elementHeightCallback = OnElementHeightSpawnerMaskList;
            m_reorderableSpawnerMaskList.drawElementCallback = DrawSpawnerMaskListElement;
            m_reorderableSpawnerMaskList.drawHeaderCallback = DrawSpawnerMaskListHeader;
            m_reorderableSpawnerMaskList.onAddCallback = OnAddSpawnerMaskListEntry;
            m_reorderableSpawnerMaskList.onRemoveCallback = OnRemoveSpawnerMaskListEntry;
            m_reorderableSpawnerMaskList.onReorderCallback = OnReorderSpawnerMaskList;

            foreach (ImageMask mask in m_spawner.m_settings.m_imageMasks)
            {
                mask.m_reorderableCollisionMaskList = CreateSpawnerCollisionMaskList(mask.m_reorderableCollisionMaskList, mask.m_collisionMasks);
            }
        }

        private float OnElementHeightSpawnerMaskList(int index)
        {
            return ImageMaskListEditor.OnElementHeight(index, m_spawner.m_settings.m_imageMasks[index]);
        }

        private void OnReorderSpawnerMaskList(ReorderableList list)
        {
            m_spawner.m_spawnPreviewDirty = true;
            m_spawner.DrawSpawnerPreview();
        }

        private void OnRemoveSpawnerMaskListEntry(ReorderableList list)
        {
            m_spawner.m_settings.m_imageMasks = ImageMaskListEditor.OnRemoveMaskListEntry(m_spawner.m_settings.m_imageMasks, list.index);
            list.list = m_spawner.m_settings.m_imageMasks;
        }

        private void OnAddSpawnerMaskListEntry(ReorderableList list)
        {
            m_spawner.m_settings.m_imageMasks = ImageMaskListEditor.OnAddMaskListEntry(m_spawner.m_settings.m_imageMasks, m_spawner.m_maxWorldHeight, m_spawner.m_minWorldHeight, GaiaSessionManager.GetSessionManager(false).m_session.m_seaLevel);
            ImageMask lastElement = m_spawner.m_settings.m_imageMasks[m_spawner.m_settings.m_imageMasks.Length - 1];
            lastElement.m_reorderableCollisionMaskList = CreateSpawnRuleCollisionMaskList(lastElement.m_reorderableCollisionMaskList, lastElement.m_collisionMasks);
            list.list = m_spawner.m_settings.m_imageMasks;
        }

        private void DrawSpawnerMaskListHeader(Rect rect)
        {
            m_spawnerMaskListExpanded = ImageMaskListEditor.DrawFilterListHeader(rect, m_spawnerMaskListExpanded, m_spawner.m_settings.m_imageMasks, m_editorUtils);
        }

        private void DrawSpawnerMaskListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            ImageMask copiedImageMask = GaiaSessionManager.GetSessionManager(false).m_copiedImageMask;
            //bool isCopiedMask = m_spawner.m_settings.m_imageMasks[index] != null && m_spawner.m_settings.m_imageMasks[index] == m_copiedImageMask;
            MaskListButtonCommand mlbc = ImageMaskListEditor.DrawMaskListElement(rect, index, m_spawner.m_settings.m_imageMasks, ref m_collisionMaskListBeingDrawn, m_editorUtils, Terrain.activeTerrain, false, copiedImageMask, m_spawnRulesHeader.normal.background, m_gaiaSettings);
            switch (mlbc)
            {
                case MaskListButtonCommand.Delete:
                        m_reorderableSpawnerMaskList.index = index;
                        OnRemoveSpawnerMaskListEntry(m_reorderableSpawnerMaskList);
                    break;
                case MaskListButtonCommand.Duplicate:
                        ImageMask newImageMask = ImageMask.Clone(m_spawner.m_settings.m_imageMasks[index]);
                        m_spawner.m_settings.m_imageMasks = GaiaUtils.InsertElementInArray(m_spawner.m_settings.m_imageMasks, newImageMask, index + 1);
                        m_reorderableSpawnerMaskList.list = m_spawner.m_settings.m_imageMasks;
                         m_spawner.m_settings.m_imageMasks[index + 1].m_reorderableCollisionMaskList = CreateSpawnRuleCollisionMaskList(m_spawner.m_settings.m_imageMasks[index + 1].m_reorderableCollisionMaskList, m_spawner.m_settings.m_imageMasks[index+1].m_collisionMasks);
                        serializedObject.ApplyModifiedProperties();

                    break;
                case MaskListButtonCommand.Copy:
                    GaiaSessionManager.GetSessionManager(false).m_copiedImageMask = m_spawner.m_settings.m_imageMasks[index];
                    break;
                case MaskListButtonCommand.Paste:
                    m_spawner.m_settings.m_imageMasks[index] = ImageMask.Clone(copiedImageMask);
                    //Rebuild collsion mask list with new content from the cloning
                    m_spawner.m_settings.m_imageMasks[index].m_reorderableCollisionMaskList = CreateSpawnRuleCollisionMaskList(m_spawner.m_settings.m_imageMasks[index].m_reorderableCollisionMaskList, m_spawner.m_settings.m_imageMasks[index].m_collisionMasks);
                    GaiaSessionManager.GetSessionManager(false).m_copiedImageMask = null;
                    break;

            }
        }



        /// <summary>
        /// Creates the reorderable collision mask list for collision masks in the spawner itself.
        /// </summary>
        public ReorderableList CreateSpawnerCollisionMaskList(ReorderableList list, CollisionMask[] collisionMasks)
        {
            list = new ReorderableList(collisionMasks, typeof(CollisionMask), true, true, true, true);
            list.elementHeightCallback = OnElementHeightCollisionMaskList;
            list.drawElementCallback = DrawSpawnerCollisionMaskElement;
            list.drawHeaderCallback = DrawSpawnerCollisionMaskListHeader;
            list.onAddCallback = OnAddSpawnerCollisionMaskListEntry;
            list.onRemoveCallback = OnRemoveSpawnerCollisionMaskMaskListEntry;
            return list;
        }

        private void OnRemoveSpawnerCollisionMaskMaskListEntry(ReorderableList list)
        {
            //look up the collision mask in the spawner's mask list
            foreach (ImageMask imagemask in m_spawner.m_settings.m_imageMasks)
            {
                if (imagemask.m_reorderableCollisionMaskList == list)
                {
                    imagemask.m_collisionMasks = CollisionMaskListEditor.OnRemoveMaskListEntry(imagemask.m_collisionMasks, list.index);
                    list.list = imagemask.m_collisionMasks;
                    return;
                }
            }
        }

        private void OnAddSpawnerCollisionMaskListEntry(ReorderableList list)
        {
            //look up the collision mask in the spawner's mask list
            foreach (ImageMask imagemask in m_spawner.m_settings.m_imageMasks)
            {
                if (imagemask.m_reorderableCollisionMaskList == list)
                {
                    imagemask.m_collisionMasks = CollisionMaskListEditor.OnAddMaskListEntry(imagemask.m_collisionMasks);
                    list.list = imagemask.m_collisionMasks;
                    return;
                }
            }
        }

        private void DrawSpawnerCollisionMaskListHeader(Rect rect)
        {
            foreach (ImageMask imagemask in m_spawner.m_settings.m_imageMasks)
            {
                if (imagemask.m_collisionMasks == m_collisionMaskListBeingDrawn)
                {
                    imagemask.m_collisionMaskExpanded = CollisionMaskListEditor.DrawFilterListHeader(rect, imagemask.m_collisionMaskExpanded, imagemask.m_collisionMasks, m_editorUtils);
                }
            }
        }

        private void DrawSpawnerCollisionMaskElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (m_collisionMaskListBeingDrawn != null && m_collisionMaskListBeingDrawn.Length > index && m_collisionMaskListBeingDrawn[index] != null)
            {
                CollisionMaskListEditor.DrawMaskListElement(rect, index, m_collisionMaskListBeingDrawn[index], m_editorUtils, Terrain.activeTerrain, GaiaConstants.FeatureOperation.Contrast);
            }
        }

        #endregion

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
        }

        /// <summary>
        /// This is used just to force the editor to repaint itself
        /// </summary>
        void EditorUpdate()
        {
            if (m_spawner != null)
            {
                if (m_spawner.m_updateCoroutine != null)
                {
                    if ((DateTime.Now - m_timeSinceLastUpdate).TotalMilliseconds > 500)
                    {
                        //Debug.Log("Active repainting spawner " + m_spawner.gameObject.name);
                        m_timeSinceLastUpdate = DateTime.Now;
                        Repaint();
                    }
                }
                else
                {
                    if ((DateTime.Now - m_timeSinceLastUpdate).TotalSeconds > 5)
                    {
                        //Debug.Log("Inactive repainting spawner " + m_spawner.gameObject.name);
                        m_timeSinceLastUpdate = DateTime.Now;
                        Repaint();
                    }
                }
            }
        }

        private void OnSceneGUI()
        {
            // dont render preview if this isnt a repaint. losing performance if we do
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            //reset rotation, rotation for the spawner is currently not supported because it causes too many issues
            m_spawner.transform.rotation = new Quaternion();
            
            //set the preview dirty if the transform changed so it will be redrawn correctly in the new location
            //the lastXPos & lastZPos variables are a workaround, because transform.hasChanged was triggering too often
            if (m_lastXPos!=m_spawner.transform.position.x || m_lastZPos != m_spawner.transform.position.z)
            {
                m_lastXPos = m_spawner.transform.position.x;
                m_lastZPos = m_spawner.transform.position.z;
                m_spawner.m_spawnPreviewDirty = true;
            }

            m_spawner.DrawSpawnerPreview();

        }

        /// <summary>
        /// Draw the UI
        /// </summary>
        public override void OnInspectorGUI()
        {
            //Reset Resource Preview
            m_drawResourcePreviewRuleId = -99;
            //Reset reorderable masks lists in case the spawner was reset etc.
            if (m_spawner.m_settings.m_spawnerRules.Count == 0 && m_reorderableRuleMasksLists.Count() > 0)
            {
                m_reorderableRuleMasksLists = new ReorderableList[0];
            }

            //Get our spawner
            m_spawner = (Spawner)target;

            //Init editor utils
            m_editorUtils.Initialize();
            serializedObject.Update();

            if (m_activatePreviewRequested && (m_activatePreviewTimeStamp + m_gaiaSettings.m_stamperAutoHidePreviewMilliseconds < GaiaUtils.GetUnixTimestamp()))
            {
                m_activatePreviewRequested = false;
                m_spawner.m_drawPreview = true;
                //force repaint
                EditorWindow view = EditorWindow.GetWindow<SceneView>();
                view.Repaint();
            }

            //Check if sea level changed
            if (m_spawner.m_seaLevel != GaiaSessionManager.GetSessionManager(false).GetSeaLevel())
            {
                //Dirty the preview to force it to be refreshed according to the new sea level
                m_spawner.m_spawnPreviewDirty= true;
                m_spawner.m_seaLevel = GaiaSessionManager.GetSessionManager(false).GetSeaLevel();
            }



            //Disable if spawning
            if (m_spawner.m_spawnProgress > 0f)
            {
                GUI.enabled = false;
            }


            EditorGUI.BeginChangeCheck();

            m_editorUtils.Panel("SpawnSettings",DrawSpawnSettings,true);
            GUILayout.Space(10);
            DrawSpawnRulesPanel("SpawnRules",DrawSpawnRules);
            GUILayout.Space(10);
            //m_editorUtils.Panel("Statistics", DrawStatistics, false);
            //GUILayout.Space(10);


            //GUILayout.BeginVertical();
            //GUILayout.Space(20);
            //bool showGizmos = EditorGUILayout.Toggle(GetLabel("Show Gizmos"), m_spawner.m_showGizmos);
            //bool showStatistics = m_spawner.m_showStatistics = EditorGUILayout.Toggle(GetLabel("Show Statistics"), m_spawner.m_showStatistics);
            //bool showTerrainHelper = m_spawner.m_showTerrainHelper = EditorGUILayout.Toggle(GetLabel("Show Terrain Helper"), m_spawner.m_showTerrainHelper);
            //GUILayout.EndVertical();

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                m_changesMadeSinceLastSave = true;
                Undo.RecordObject(m_spawner, "Made changes");
                m_spawner.m_spawnPreviewDirty = true;
                //m_spawner.m_showGizmos = showGizmos;
                //m_spawner.m_showStatistics = showStatistics;
                //m_spawner.m_showTerrainHelper = showTerrainHelper;


                if (m_spawner.m_imageMask != null)
                {
                    GaiaUtils.MakeTextureReadable(m_spawner.m_imageMask);
                    GaiaUtils.MakeTextureUncompressed(m_spawner.m_imageMask);
                }
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(m_spawner);
                EditorUtility.SetDirty(m_spawner.m_settings);


            }

            //m_editorUtils.Panel("ResourceManagement", DrawResourceManagement, false);
            //GUILayout.Space(10);
            m_editorUtils.Panel("Appearance", DrawAppearance, false);
            m_editorUtils.Panel("SpawnControl", DrawSpawnControls, true);
        }

        private void DrawAppearance(bool showHelp)
        {

            m_editorUtils.LabelField("SeaLevel", new GUIContent(GaiaSessionManager.GetSessionManager(false).GetSeaLevel().ToString() + " m"), showHelp);
            float newSeaLEvel = m_editorUtils.Slider("SeaLevel", GaiaSessionManager.GetSessionManager(false).GetSeaLevel(), 0, m_spawner.GetCurrentTerrain().terrainData.size.y, showHelp);
            GaiaSessionManager.GetSessionManager(false).SetSeaLevel(newSeaLEvel);
            m_spawner.m_showSeaLevelPlane = m_editorUtils.Toggle("ShowSeaLevelPlane", m_spawner.m_showSeaLevelPlane, showHelp);
            m_spawner.m_showSeaLevelinStampPreview = m_editorUtils.Toggle("ShowSeaLevelSpawnerPreview", m_spawner.m_showSeaLevelinStampPreview, showHelp);
            //Color gizmoColour = EditorGUILayout.ColorField(GetLabel("Gizmo Colour"), m_stamper.m_gizmoColour);
            //alwaysShow = m_editorUtils.Toggle("AlwaysShowStamper", m_stamper.m_alwaysShow, showHelp);
            m_spawner.m_showBoundingBox = m_editorUtils.Toggle("ShowBoundingBox", m_spawner.m_showBoundingBox, showHelp);
            //showRulers = m_stamper.m_showRulers = m_editorUtils.Toggle("ShowRulers", m_stamper.m_showRulers, showHelp);
            //bool showTerrainHelper = m_stamper.m_showTerrainHelper = EditorGUILayout.Toggle(GetLabel("Show Terrain Helper"), m_stamper.m_showTerrainHelper);
        }

        private void DrawGameObjectDropBox()
        {
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            if (m_ResourceManagementMessage != "")
            {
                EditorGUILayout.HelpBox(m_ResourceManagementMessage, m_ResourceManagementMessageType, true);
            }

            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(drop_area, m_editorUtils.GetTextValue("DropAreaText"), m_boxStyle);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    //Work out if we have prefab instances or prefab objects
                    bool havePrefabInstances = false;
                    foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                    {
                        PrefabAssetType pt = PrefabUtility.GetPrefabAssetType(dragged_object);

                        if (pt == PrefabAssetType.Regular || pt == PrefabAssetType.Model)
                        {
                            havePrefabInstances = true;
                            break;
                        }
                    }

                    if (havePrefabInstances)
                    {
                        List<GameObject> prototypes = new List<GameObject>();

                        foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                        {
                            PrefabAssetType pt = PrefabUtility.GetPrefabAssetType(dragged_object);

                            if (pt == PrefabAssetType.Regular || pt == PrefabAssetType.Model)
                            {
                                prototypes.Add(dragged_object as GameObject);
                            }
                            else
                            {
                                m_ResourceManagementMessageType = MessageType.Error;
                                m_ResourceManagementMessage = m_editorUtils.GetTextValue("DropAreaOnlyPrefabInstances");
                            }
                        }

                            //Same them as a single entity
                            if (prototypes.Count > 0)
                            {
                                m_spawner.m_settings.m_resources.AddGameObject(prototypes);

                                //Create a new rule from the newly added gameobject
                                AddNewRule(SpawnerResourceType.GameObject, m_spawner.m_settings.m_resources.m_gameObjectPrototypes.Length-1);

                            }
                    }
                    else
                    {
                        foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                        {
                            if (PrefabUtility.GetPrefabAssetType(dragged_object) == PrefabAssetType.Regular)
                            {
                                m_spawner.m_settings.m_resources.AddGameObject(dragged_object as GameObject);
                                AddNewRule(SpawnerResourceType.GameObject, m_spawner.m_settings.m_resources.m_gameObjectPrototypes.Length - 1);
                            }
                            else
                            {
                                //Debug.LogWarning("You may only add prefabs or game objects attached to prefabs!");
                                m_ResourceManagementMessageType = MessageType.Error;
                                m_ResourceManagementMessage = m_editorUtils.GetTextValue("DropAreaOnlyPrefabsOrGameObjects");
                            }
                        }
                    }

                }
                break;
                
            }
        }

        private void DrawSpawnControls(bool helpEnabled)
        {
            m_editorUtils.Panel("ClearSpawns", DrawTerrainControls, false);

            if (m_changesMadeSinceLastSave)
            {
                GUI.backgroundColor = m_dirtyColor;
                m_editorUtils.Panel("SaveLoadChangesPending", DrawSaveAndLoad, (m_spawner.m_createdfromBiomePreset || m_spawner.m_createdFromGaiaManager) ? true : false);
            }
            else
            {
                m_editorUtils.Panel("SaveLoad", DrawSaveAndLoad, (m_spawner.m_createdfromBiomePreset || m_spawner.m_createdFromGaiaManager) ? true : false);
            }
            GUI.backgroundColor = m_normalBGColor;
            GUILayout.Space(5);
            //Ragardless, re-enable the spawner controls
            GUI.enabled = true;

            //Display progress
            if (m_spawner.m_spawnProgress > 0f && m_spawner.m_spawnProgress < 1f)
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Cancel")))
                {
                    m_spawner.CancelSpawn();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();
                ProgressBar(string.Format("Progress ({0:0.0}%)", m_spawner.m_spawnProgress * 100f), m_spawner.m_spawnProgress);
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                //if (GUILayout.Button(GetLabel("Ground")))
                //{
                //    m_spawner.GroundToTerrain();
                //}
                if (GUILayout.Button(GetLabel("Fit To Terrain")))
                {
                    m_spawner.FitToTerrain();
                }
                if (Terrain.activeTerrains.Length > 1)
                {
                    if (GUILayout.Button(GetLabel("Fit To World")))
                    {
                        m_spawner.FitToAllTerrains();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                //highlight the currently active preview.
                if (m_spawner.m_drawPreview)
                {
                    GUI.backgroundColor = Color.red;
                }
                if (GUILayout.Button(GetLabel("Preview")))
                {
                    //as soon as the user interacts with the button, the user is in control, no need for auto activate anymore
                    m_activatePreviewRequested = false;
                    m_spawner.TogglePreview();
                    //force repaint
                    EditorWindow view = EditorWindow.GetWindow<SceneView>();
                    view.Repaint();
                }
                //if (GUILayout.Button(GetLabel("Preview")))
                //{
                //    foreach (ResourceProtoGameObject proto in m_spawner.m_settings.m_resources.m_gameObjectPrototypes)
                //    {
                //        foreach (ResourceProtoGameObjectInstance instance in proto.m_instances)
                //        {
                //            instance.m_minSpawnOffsetY = 0;
                //            instance.m_maxSpawnOffsetY = 0;
                //        }
                //    }
                    
                //}


                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUI.backgroundColor = m_normalBGColor;
                GUI.enabled = true;
                if (GUILayout.Button(GetLabel("Spawn Local")))
                {
                    Spawn(false);
                    //}
                }
                if (GUILayout.Button(GetLabel("Spawn World")))
                {
                    Spawn(true);
                    //}
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }

            //if (GUILayout.Button("Deactivate"))
            //{
            //    spawner.DeactivateAllInstances();
            //}

            GUI.enabled = true;
        }

        private void DrawTerrainControls(bool obj)
        {
            GUILayout.BeginVertical();

            //GUILayout.BeginHorizontal();

            //if (GUILayout.Button(GetLabel("Flatten")))
            //{
            //    if (EditorUtility.DisplayDialog("Flatten Terrain tiles ?", "Are you sure you want to flatten all terrain tiles - this can not be undone ?", "Yes", "No"))
            //    {
            //        m_spawner.FlattenTerrain();
            //    }
            //}
            //if (GUILayout.Button(GetLabel("Smooth")))
            //{
            //    if (EditorUtility.DisplayDialog("Smooth Terrain tiles ?", "Are you sure you want to smooth all terrain tiles - this can not be undone ?", "Yes", "No"))
            //    {
            //        m_spawner.SmoothTerrain();
            //    }
            //}
            //GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button(m_editorUtils.GetTextValue("ClearAllTrees")))
            {
                //if (EditorUtility.DisplayDialog("Clear Terrain trees ?", "Are you sure you want to clear all terrain trees - this can not be undone ?", "Yes", "No"))
                //{
                //    m_spawner.ClearTrees();
                //}
                if (EditorUtility.DisplayDialog(m_editorUtils.GetTextValue("ClearAllTreesDialogueTitle"), m_editorUtils.GetTextValue("ClearAllTreesDialogueText"), m_editorUtils.GetTextValue("Yes"), m_editorUtils.GetTextValue("No")))
                {
                    m_spawner.ClearTrees();
                }

            }
            if (GUILayout.Button(m_editorUtils.GetTextValue("ClearAllDetails")))
            {
                if (EditorUtility.DisplayDialog(m_editorUtils.GetTextValue("ClearAllDetailsDialogueTitle"), m_editorUtils.GetTextValue("ClearAllDetailsDialogueText"), m_editorUtils.GetTextValue("Yes"), m_editorUtils.GetTextValue("No")))
                {
                    m_spawner.ClearDetails();
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(m_editorUtils.GetTextValue("ClearSpawnerGameObjects")))
            {
                if (EditorUtility.DisplayDialog(m_editorUtils.GetTextValue("ClearSpawnerGameObjectsDialogueTitle"), m_editorUtils.GetTextValue("ClearSpawnerGameObjectsDialogueText"), m_editorUtils.GetTextValue("Yes"), m_editorUtils.GetTextValue("No")))
                {
                    m_spawner.ClearAllGameObjects();
                }
            }
            if (GUILayout.Button(m_editorUtils.GetTextValue("ClearAll")))
            {
                if (EditorUtility.DisplayDialog(m_editorUtils.GetTextValue("ClearAllDialogueTitle"), m_editorUtils.GetTextValue("ClearAllDialogueText"), m_editorUtils.GetTextValue("Yes"), m_editorUtils.GetTextValue("No")))
                {
                    m_spawner.ClearTrees();
                    m_spawner.ClearDetails();
                    m_spawner.ClearAllGameObjects();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        //private void DrawStatistics(bool obj)
        //{
           
        //    m_editorUtils.LabelField("ActiveRules", GetLabel(m_spawner.m_settings.m_spawnerRules.Where(x => x.m_isActive == true).Count().ToString()));
        //    m_editorUtils.LabelField("InactiveRules", GetLabel(m_spawner.m_settings.m_spawnerRules.Where(x=>x.m_isActive!=true).Count().ToString()));
        //    //EditorGUILayout.LabelField(GetLabel("TOTAL Rules"), GetLabel(m_spawner.m_totalRuleCnt.ToString()));
        //    GUILayout.Space(8);
        //    m_editorUtils.LabelField("InstanceCount");
        //    foreach (SpawnRule rule in m_spawner.m_settings.m_spawnerRules)
        //    {
        //        EditorGUILayout.LabelField(new GUIContent(rule.m_resourceIdx.ToString() + " "+ rule.m_name), GetLabel(rule.m_spawnedInstances.ToString()));
        //    }
        //    //EditorGUILayout.LabelField(GetLabel("Active Instances"), GetLabel(m_spawner.m_activeInstanceCnt.ToString()));
        //    //EditorGUILayout.LabelField(GetLabel("Inactive Instances"), GetLabel(m_spawner.m_inactiveInstanceCnt.ToString()));
        //    //EditorGUILayout.LabelField(GetLabel("TOTAL Instances"), GetLabel(m_spawner.m_totalInstanceCnt.ToString()));
        //    //EditorGUILayout.LabelField(GetLabel("MAX INSTANCES"), GetLabel(m_spawner.m_maxInstanceCnt.ToString()));

        //}

        private bool DrawSpawnRulesPanel(string nameKey, Action<bool> contentMethod)
        {
            GUIContent panelLabel = m_editorUtils.GetContent(nameKey);

            //Panel Label
            GUIStyle panelLabelStyle = new GUIStyle(GUI.skin.label);
            panelLabelStyle.normal.textColor = GUI.skin.label.normal.textColor;
            panelLabelStyle.fontStyle = FontStyle.Bold;
            panelLabelStyle.normal.background = GUI.skin.label.normal.background;

            // Panel Frame
            GUIStyle panelFrameStyle = new GUIStyle(GUI.skin.box);
            panelFrameStyle.normal.textColor = GUI.skin.label.normal.textColor;
            panelFrameStyle.fontStyle = FontStyle.Bold;
            panelFrameStyle.alignment = TextAnchor.UpperLeft;

            // Panel
            GUIStyle panelStyle = new GUIStyle(GUI.skin.label);
            panelStyle.normal.textColor = GUI.skin.label.normal.textColor;
            panelStyle.alignment = TextAnchor.UpperLeft;

            //Always unfold if we have no rules yet, to show the prompt to add new stuff
            bool unfolded = m_spawner.m_settings.m_spawnerRules.Count>0 ? m_spawner.m_rulePanelUnfolded : true;
            bool helpActive = m_rulePanelHelpActive;



            GUILayout.BeginVertical(panelFrameStyle);
            {
                GUILayout.BeginHorizontal(m_spawnRulesHeader);
                {
                    unfolded = GUILayout.Toggle(unfolded, unfolded ? "-" : "+", panelLabelStyle, GUILayout.MinWidth(14));
                    GUILayout.Space(-5f);
                    unfolded = GUILayout.Toggle(unfolded, panelLabel, panelLabelStyle);
                    GUILayout.FlexibleSpace();
                    if (m_editorUtils.Button(GetLabel("+")))
                    {
                        //Resource Type will be ignored when passing without ID
                        AddNewRule(SpawnerResourceType.GameObject);
                        //Always unfold when adding a new rule, we want to see & edit it
                        unfolded = true;
                    }

                    if (m_editorUtils.Button(GetLabel("A")))
                    {
                        for (int idx = 0; idx < m_spawner.m_settings.m_spawnerRules.Count; idx++)
                        {
                            m_spawner.m_settings.m_spawnerRules[idx].m_isActive = true;
                        }
                    }

      
                    if (m_editorUtils.Button(GetLabel("I")))
                    {
                        for (int idx = 0; idx < m_spawner.m_settings.m_spawnerRules.Count; idx++)
                        {
                            m_spawner.m_settings.m_spawnerRules[idx].m_isActive = false;
                        }
                    }
                    if (m_editorUtils.Button("X All"))
                    {
                        if (EditorUtility.DisplayDialog("Delete All Rules ?", "Are you sure you want to delete all rules - this can not be undone ?", "Yes", "No"))
                        {
                            m_spawner.m_settings.m_spawnerRules.Clear();
                            m_reorderableRuleMasksLists = new ReorderableList[0];
                            PruneResources();
                        }
                    }

                    //EditorGUILayout.EndVertical();
                    //m_editorUtils.HelpToggle(ref helpActive);
                }
                GUILayout.EndHorizontal();

                if (helpActive)
                {
                    GUILayout.Space(2f);
                    m_editorUtils.InlineHelp(nameKey, helpActive);
                }

                if (unfolded)
                {
                    GUILayout.BeginVertical(panelStyle);
                    {
                        
                        if (m_spawner.m_settings.m_spawnerRules.Count == 0)
                        {
                            GUILayout.Space(20);
                            //No rules yet, display a message to prompt the user to add a first resource
                            m_editorUtils.LabelField("NoRulesYet",new GUIStyle() { wordWrap=true});
                            GUILayout.Space(10);
                            if (m_editorUtils.Button("CreateFirstRule"))
                            {
                                //Resource Type will be ignored when passing without ID
                                AddNewRule(SpawnerResourceType.GameObject);
                                //Always unfold when adding a new rule, we want to see & edit it
                                unfolded = true;
                            }
                            GUILayout.Space(20);
                        }
                        else
                        {
                            GUILayout.Space(10);
                            EditorGUI.indentLevel++;
                            contentMethod.Invoke(helpActive);
                            EditorGUI.indentLevel--;
                            GUILayout.Space(10);
                        }
                        
                        DrawGameObjectDropBox();
                        GUILayout.Space(10);
                    }
                    GUILayout.EndVertical();
                }
            }
            if (unfolded)
            {
                //Footer - repeat the buttons so they are easily accessible anywhere
                GUILayout.BeginHorizontal(m_spawnRulesHeader);
                {
                    //unfolded = GUILayout.Toggle(unfolded, unfolded ? "-" : "+", panelLabelStyle);
                    //GUILayout.Space(-5f);
                    //unfolded = GUILayout.Toggle(unfolded, panelLabel, panelLabelStyle);
                    GUILayout.FlexibleSpace();
                    if (m_editorUtils.Button(GetLabel("+")))
                    {
                        //Resource Type will be ignored when passing without ID
                        AddNewRule(SpawnerResourceType.GameObject);
                        //Always unfold when adding a new rule, we want to see & edit it
                        unfolded = true;
                    }

                    if (m_editorUtils.Button(GetLabel("A")))
                    {
                        for (int idx = 0; idx < m_spawner.m_settings.m_spawnerRules.Count; idx++)
                        {
                            m_spawner.m_settings.m_spawnerRules[idx].m_isActive = true;
                        }
                    }


                    if (m_editorUtils.Button(GetLabel("I")))
                    {
                        for (int idx = 0; idx < m_spawner.m_settings.m_spawnerRules.Count; idx++)
                        {
                            m_spawner.m_settings.m_spawnerRules[idx].m_isActive = false;
                        }
                    }
                    if (m_editorUtils.Button(GetLabel("X All")))
                    {
                        if (EditorUtility.DisplayDialog("Delete All Rules ?", "Are you sure you want to delete all rules - this can not be undone ?", "Yes", "No"))
                        {
                            m_spawner.m_settings.m_spawnerRules.Clear();
                            m_reorderableRuleMasksLists = new ReorderableList[0];
                            PruneResources();
                        }
                    }

                    //EditorGUILayout.EndVertical();
                    //m_editorUtils.HelpToggle(ref helpActive);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            m_spawner.m_rulePanelUnfolded = unfolded;
            m_rulePanelHelpActive = helpActive;
            return unfolded;
        }

        private void AddNewRule(SpawnerResourceType resourceType, int resourceID = -99)
        {
            SpawnRule newRule = new SpawnRule();

            //no resource ID given? Create a new one
            if (resourceID == -99)
            {
                newRule.m_name = "New Spawn Rule Entry";


                //check the previous rule, if any. We assume the user wants to create another rule of the same resource type.
                if (m_spawner.m_settings.m_spawnerRules.Count >= 1)
                {
                    newRule.m_resourceType = m_spawner.m_settings.m_spawnerRules[m_spawner.m_settings.m_spawnerRules.Count - 1].m_resourceType;
                }
                //add a new resource prototype as well, we assume the user wants to add a new resource rather than re-use one
                switch (newRule.m_resourceType)
                {
                    case SpawnerResourceType.TerrainTexture:
                        newRule.m_resourceIdx = AddNewTextureResource();
                        break;
                    case SpawnerResourceType.TerrainTree:
                        newRule.m_resourceIdx = AddNewTreeResource();
                        break;
                    case SpawnerResourceType.TerrainDetail:
                        newRule.m_resourceIdx = AddNewTerrainDetailResource();
                        break;
                    case SpawnerResourceType.GameObject:
                        newRule.m_resourceIdx = AddNewGameObjectResource();
                        break;
                }
            }
            else
            {
                //use existing resource type / id
                newRule.m_resourceType = resourceType;
                switch (resourceType)
                {
                    case SpawnerResourceType.TerrainTexture:
                        newRule.m_name = m_spawner.m_settings.m_resources.m_texturePrototypes[resourceID].m_name;
                        break;
                    case SpawnerResourceType.TerrainTree:
                        newRule.m_name = m_spawner.m_settings.m_resources.m_treePrototypes[resourceID].m_name;
                        break;
                    case SpawnerResourceType.TerrainDetail:
                        newRule.m_name = m_spawner.m_settings.m_resources.m_detailPrototypes[resourceID].m_name;
                        break;
                    case SpawnerResourceType.GameObject:
                        newRule.m_name = m_spawner.m_settings.m_resources.m_gameObjectPrototypes[resourceID].m_name;
                        break;
                }
                newRule.m_resourceIdx = resourceID;
            }

            newRule.m_imageMasks = new ImageMask[0];
            //always fold out the resource settings for new rules so user can edit directly
            newRule.m_isFoldedOut = true;
            newRule.m_resourceSettingsFoldedOut = true;
            m_spawner.m_rulePanelUnfolded = true;
            m_spawner.m_settings.m_spawnerRules.Add(newRule);
            AddNewMaskList();

        }

        private void DrawSpawnSettings(bool helpEnabled)
        {
            m_spawner.m_settings.m_spawnRange = m_editorUtils.Slider("Range", m_spawner.m_settings.m_spawnRange, 0, m_spawner.GetMaxSpawnerRange(), helpEnabled);
            m_spawner.m_settings.spawnMode = (SpawnMode)m_editorUtils.EnumPopup("SpawnMode", m_spawner.m_settings.spawnMode, helpEnabled);
            ImageMaskListEditor.DrawMaskList(ref m_spawnerMaskListExpanded, m_reorderableSpawnerMaskList, m_editorUtils);
            m_editorUtils.InlineHelp("SpawnerMask",helpEnabled);
        }

        private void DrawSpawnRules(bool helpEnabled)
        {
            for (int ruleIdx = 0; ruleIdx < m_spawner.m_settings.m_spawnerRules.Count; ruleIdx++)
            {

                m_spawnRuleIndexBeingDrawn = ruleIdx;
                SpawnRule rule = m_spawner.m_settings.m_spawnerRules[ruleIdx];

                DrawSingleRulePanel(rule, ruleIdx);

                //if (rule.m_isActive)
                //{
                //    rule.m_isFoldedOut = EditorGUILayout.Foldout(rule.m_isFoldedOut, rule.m_name, true);
                //}
                //else
                //{
                //    rule.m_isFoldedOut = EditorGUILayout.Foldout(rule.m_isFoldedOut, rule.m_name + " [inactive]", true);
                //}
                //if (rule.m_isFoldedOut)
                //{
                    

                //    GUILayout.Space(20);
                //}
            } //for
        }

        private bool DrawSingleRulePanel(SpawnRule rule, int ruleIdx)
        {
            string instanceCount = "";

            if (rule.m_resourceType != SpawnerResourceType.TerrainTexture)
            {
                instanceCount = "(" + rule.m_spawnedInstances + ")";
            }


            GUIContent panelLabel = new GUIContent(instanceCount + " " + rule.m_name);

            //Panel Label
            GUIStyle panelLabelStyle = new GUIStyle(GUI.skin.label);
            panelLabelStyle.normal.textColor = GUI.skin.label.normal.textColor;
            panelLabelStyle.fontStyle = FontStyle.Bold;
            panelLabelStyle.normal.background = GUI.skin.label.normal.background;

            // Panel Frame
            GUIStyle panelFrameStyle = new GUIStyle(GUI.skin.box);
            panelFrameStyle.normal.textColor = GUI.skin.label.normal.textColor;
            panelFrameStyle.fontStyle = FontStyle.Bold;
            panelFrameStyle.alignment = TextAnchor.UpperLeft;

            // Panel
            GUIStyle panelStyle = new GUIStyle(GUI.skin.label);
            panelStyle.normal.textColor = GUI.skin.label.normal.textColor;
            panelStyle.alignment = TextAnchor.UpperLeft;

            bool unfolded = rule.m_isFoldedOut;
            bool helpEnabled = rule.m_isHelpActive;



            GUILayout.BeginVertical(panelFrameStyle);
            {
                GUILayout.BeginHorizontal(m_singleSpawnRuleHeader);
                {
                    //Rect rect = EditorGUILayout.GetControlRect();
                    unfolded = GUILayout.Toggle(unfolded, unfolded ? "-" : "+", panelLabelStyle,GUILayout.MinWidth(14));

                    

                    rule.m_isActive = GUILayout.Toggle(rule.m_isActive,"");


                    GUI.enabled = rule.m_isActive && GUI.enabled;
                    bool originalGUIState = GUI.enabled;
                    Texture2D resourceTexture = GetSpawnRulePreviewTexture(rule, m_spawner.m_settings.m_resources);
                    if (resourceTexture != null)
                    {
                        m_editorUtils.Image(resourceTexture,20,20);
                    }
                    Rect imageRect = GUILayoutUtility.GetLastRect();
                    imageRect.width = 20;
                    imageRect.height = 20;
                    //EditorGUIUtility.AddCursorRect(imageRect, MouseCursor.Zoom);
                    //if (imageRect.Contains(Event.current.mousePosition))
                    //{
                    //    m_drawResourcePreviewRuleId = ruleIdx;
                    //    //unfold so we can see the larger preview actually
                    //    unfolded = true;
                    //}
                    unfolded = GUILayout.Toggle(unfolded, panelLabel, panelLabelStyle,GUILayout.MinWidth(0));
                    GUILayout.FlexibleSpace();

                    //Deactivate upwards button for first position in the rule list
                    if (ruleIdx == 0)
                    {
                        GUI.enabled = false;
                    }

                    float smallButtonSize = 20;
                    GUIStyle smallButtonStyle = new GUIStyle(GUI.skin.button);
                    smallButtonStyle.padding = new RectOffset(2, 2, 2, 2);
                    smallButtonStyle.margin = new RectOffset(5, 5, 0, 2);

                    GUIContent GCupIcon = GaiaEditorUtils.GetIconGUIContent("IconUp", m_gaiaSettings.m_IconUp, m_gaiaSettings.m_IconProUp, m_editorUtils);
                    if (m_editorUtils.Button(GCupIcon, smallButtonStyle, GUILayout.Height(smallButtonSize), GUILayout.Width(smallButtonSize)))
                    {
                       SwapRules(ruleIdx - 1, ruleIdx);
                    }

                    GUI.enabled = originalGUIState;

                    //Deactivate downwards button for last position in the rule list
                    if (ruleIdx == m_spawner.m_settings.m_spawnerRules.Count()-1)
                    {
                        GUI.enabled = false;
                    }


                    GUIContent GCdownIcon = GaiaEditorUtils.GetIconGUIContent("IconDown", m_gaiaSettings.m_IconDown, m_gaiaSettings.m_IconProDown, m_editorUtils);
                    if (m_editorUtils.Button(GCdownIcon, smallButtonStyle, GUILayout.Height(smallButtonSize), GUILayout.Width(smallButtonSize)))
                    {
                        SwapRules(ruleIdx, ruleIdx + 1);
                    }
                    GUI.enabled = originalGUIState;

                    GUIContent GCduplicateIcon = GaiaEditorUtils.GetIconGUIContent("IconDuplicate", m_gaiaSettings.m_IconDuplicate, m_gaiaSettings.m_IconProDuplicate, m_editorUtils);
                    if (m_editorUtils.Button(GCduplicateIcon, smallButtonStyle, GUILayout.Height(smallButtonSize), GUILayout.Width(smallButtonSize)))
                    {
                        DuplicateRule(ruleIdx);
                    }

                    DrawVisualiseButton(ruleIdx, smallButtonStyle, smallButtonSize);

                    //Unless the spawner is spawning, still offer to delete even if the rule is inactive
                    if (!m_spawner.IsSpawning())
                    {
                        GUI.enabled = true;
                    }
                    GUIContent GCremoveIcon = GaiaEditorUtils.GetIconGUIContent("IconRemove", m_gaiaSettings.m_IconRemove, m_gaiaSettings.m_IconProRemove, m_editorUtils);
                    if (m_editorUtils.Button(GCremoveIcon, smallButtonStyle, GUILayout.Height(smallButtonSize), GUILayout.Width(smallButtonSize)))
                    {
                        m_spawner.m_settings.m_spawnerRules.Remove(rule);
                        RemoveMaskList(ruleIdx);
                        PruneResources();
                        CleanPreviewRuleIDs();
                    }

                    GUI.enabled = !m_spawner.IsSpawning() && rule.m_isActive;

                    //EditorGUILayout.EndVertical();
                    m_editorUtils.HelpToggle(ref helpEnabled);
                }
                GUILayout.EndHorizontal();

                //if (helpActive)
                //{
                //    GUILayout.Space(2f);
                //    m_editorUtils.InlineHelp(nameKey, helpActive);
                //}

                if (unfolded)
                {
                    GUILayout.BeginVertical(panelStyle);
                    {
                        if (rule != null && m_spawner.m_settings.m_spawnerRules.Count > ruleIdx)
                        {
                            DrawSingleRule(rule, ruleIdx, helpEnabled);
                        }
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();

            //Leave a little space between the rules, but no extra space behind the last rule
            if (ruleIdx < m_spawner.m_settings.m_spawnerRules.Count - 1)
            {
                GUILayout.Space(8);
            }

            rule.m_isFoldedOut = unfolded;
            rule.m_isHelpActive = helpEnabled;
            GUI.enabled = !m_spawner.IsSpawning();
            return unfolded;
        }


        /// <summary>
        /// Swaps out the position / index of two spawn rules in the stack
        /// </summary>
        /// <param name="firstRuleID"></param>
        /// <param name="secondRuleID"></param>
        private void SwapRules(int firstRuleID, int secondRuleID)
        {
            //Check if both ids are actually in index first
            int maxIndex = m_spawner.m_settings.m_spawnerRules.Count() - 1;
            if (firstRuleID > maxIndex || secondRuleID > maxIndex || firstRuleID < 0 || secondRuleID < 0)
            {
                Debug.LogError("Could not swap rules: First Index " + firstRuleID.ToString() + " or Second Index " + secondRuleID.ToString() + " are out of bounds.");
                return;
            }

            SpawnRule tempRule = m_spawner.m_settings.m_spawnerRules[firstRuleID];
            m_spawner.m_settings.m_spawnerRules[firstRuleID] = m_spawner.m_settings.m_spawnerRules[secondRuleID];
            m_spawner.m_settings.m_spawnerRules[secondRuleID] = tempRule;

            m_reorderableRuleMasksLists = GaiaUtils.SwapElementsInArray(m_reorderableRuleMasksLists, firstRuleID, secondRuleID);

            m_spawnRuleMaskListExpanded = GaiaUtils.SwapElementsInArray(m_spawnRuleMaskListExpanded, firstRuleID, secondRuleID);

            //Swap preview rule indexes as well, if affected
            bool visChange = false;
            for (int i = 0; i < m_spawner.m_previewRuleIds.Count(); i++)
            {
                if (m_spawner.m_previewRuleIds[i] == secondRuleID)
                {
                    m_spawner.m_previewRuleIds[i] = firstRuleID;
                    visChange = true;
                }
                //the else here is important!
                else if (m_spawner.m_previewRuleIds[i] == firstRuleID)
                {
                    m_spawner.m_previewRuleIds[i] = secondRuleID;
                    visChange = true;
                }

                //If the order of visualized rules has changed, we need to update the visualization as well
                if (visChange)
                {
                    m_spawner.m_previewRuleIds.Sort();
                    m_spawner.m_spawnPreviewDirty = true;
                }
            }

        }


        /// <summary>
        /// Duplicates a spawn rule in the stack
        /// </summary>
        /// <param name="spawnRuleId"></param>
        private void DuplicateRule(int spawnRuleId)
        {
            SpawnRule sourceRule = m_spawner.m_settings.m_spawnerRules[spawnRuleId];
            SpawnRule newRule = new SpawnRule();

            //GaiaUtils.CopyFields(m_spawner.m_settings.m_spawnerRules[spawnRuleId], newRule);
            newRule.m_resourceType = sourceRule.m_resourceType;
            newRule.m_resourceIdx = sourceRule.m_resourceIdx;
            newRule.m_name = sourceRule.m_name;
            newRule.m_locationIncrementMin = sourceRule.m_locationIncrementMin;
            newRule.m_locationIncrementMax = sourceRule.m_locationIncrementMax;
            newRule.m_jitterPercent = sourceRule.m_jitterPercent;
            newRule.m_minRequiredFitness = sourceRule.m_minRequiredFitness;
            newRule.m_minDirection = sourceRule.m_minDirection;
            newRule.m_maxDirection = sourceRule.m_maxDirection;
            newRule.m_boundsCheckQuality= sourceRule.m_boundsCheckQuality;
            newRule.m_goSpawnTarget = sourceRule.m_goSpawnTarget;

            newRule.m_isFoldedOut = sourceRule.m_isFoldedOut;
            newRule.m_resourceSettingsFoldedOut = sourceRule.m_resourceSettingsFoldedOut;
            m_spawner.m_settings.m_spawnerRules.Insert(spawnRuleId + 1, newRule);

            newRule.m_imageMasks = new ImageMask[sourceRule.m_imageMasks.Count()];

            //Create a deep copy for each mask in the list > we want source & target to be independent from each other
            for (int i = 0; i < newRule.m_imageMasks.Count(); i++)
            {
                newRule.m_imageMasks[i] = ImageMask.Clone(m_spawner.m_settings.m_spawnerRules[spawnRuleId].m_imageMasks[i]);
                newRule.m_imageMasks[i].m_reorderableCollisionMaskList = CreateSpawnRuleCollisionMaskList(newRule.m_imageMasks[i].m_reorderableCollisionMaskList, newRule.m_imageMasks[i].m_collisionMasks);

            }

            //Insert the new mask list
            ReorderableList newMaskList = new ReorderableList(newRule.m_imageMasks,typeof(ImageMask));
            newMaskList = CreateSpawnRuleMaskList(newMaskList, newRule.m_imageMasks);
            m_reorderableRuleMasksLists = GaiaUtils.InsertElementInArray(m_reorderableRuleMasksLists, newMaskList, spawnRuleId + 1);

            //Insert the expanded flag
            m_spawnRuleMaskListExpanded = GaiaUtils.InsertElementInArray(m_spawnRuleMaskListExpanded, false, spawnRuleId + 1);
        }

        /// <summary>
        /// Removes all Resource entries that are not in use by any rule anymore, this prevents build up of unused resource entries
        /// </summary>
        private void PruneResources()
        {
            GaiaResource resource = m_spawner.m_settings.m_resources;
            for (int i = resource.m_texturePrototypes.Length - 1; i >= 0; i--)
            {
                if (m_spawner.m_settings.m_spawnerRules.Find(x => x.m_resourceType == SpawnerResourceType.TerrainTexture && x.m_resourceIdx == i) == null)
                {
                    resource.m_texturePrototypes = GaiaUtils.RemoveArrayIndexAt(resource.m_texturePrototypes, i);
                    m_spawner.CorrectIndicesAfteResourceDeletion(SpawnerResourceType.TerrainTexture, i);
                }
            }
            for (int i = resource.m_treePrototypes.Length - 1; i >= 0; i--)
            {
                if (m_spawner.m_settings.m_spawnerRules.Find(x => x.m_resourceType == SpawnerResourceType.TerrainTree && x.m_resourceIdx == i) == null)
                {
                    resource.m_treePrototypes = GaiaUtils.RemoveArrayIndexAt(resource.m_treePrototypes, i);
                    m_spawner.CorrectIndicesAfteResourceDeletion(SpawnerResourceType.TerrainTree, i);
                }
            }
            for (int i = resource.m_detailPrototypes.Length - 1; i >= 0; i--)
            {
                if (m_spawner.m_settings.m_spawnerRules.Find(x => x.m_resourceType == SpawnerResourceType.TerrainDetail && x.m_resourceIdx == i) == null)
                {
                    resource.m_detailPrototypes = GaiaUtils.RemoveArrayIndexAt(resource.m_detailPrototypes, i);
                    m_spawner.CorrectIndicesAfteResourceDeletion(SpawnerResourceType.TerrainDetail, i);
                }
            }
            for (int i = resource.m_gameObjectPrototypes.Length - 1; i >= 0; i--)
            {
                if (m_spawner.m_settings.m_spawnerRules.Find(x => x.m_resourceType == SpawnerResourceType.GameObject && x.m_resourceIdx == i) == null)
                {
                    resource.m_gameObjectPrototypes = GaiaUtils.RemoveArrayIndexAt(resource.m_gameObjectPrototypes, i);
                    m_spawner.CorrectIndicesAfteResourceDeletion(SpawnerResourceType.GameObject, i);
                }
            }
        }

        private void DrawVisualiseButton( int spawnRuleID, GUIStyle buttonStyle, float smallButtonSize)
        {
            Color currentBGColor = GUI.backgroundColor;
            if (m_spawner.m_previewRuleIds.Contains(spawnRuleID) && m_spawner.m_drawPreview)
            {
                GUI.backgroundColor = m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_visualisationColor;
            }

            GUIContent GCvisualizeIcon = GaiaEditorUtils.GetIconGUIContent("IconVisible", m_gaiaSettings.m_IconVisible, m_gaiaSettings.m_IconProVisible, m_editorUtils);
            GUILayout.Space(5);
            if (m_editorUtils.Button(GCvisualizeIcon, buttonStyle, GUILayout.Height(smallButtonSize), GUILayout.Width(smallButtonSize)))

            {
                //is this rule being shown already? then only remove this rule
                if (m_spawner.m_previewRuleIds.Contains(spawnRuleID) && m_spawner.m_drawPreview)
                {
                    m_spawner.m_previewRuleIds.Remove(spawnRuleID);
                    if (m_spawner.m_previewRuleIds.Count <= 0)
                        m_spawner.m_drawPreview = false;
                }
                else
                {
                    if (m_spawner.m_drawPreview)
                    {
                        //this rule needs to be added for visualisation, would we exceed the maximum allowed number?
                        if (m_spawner.m_previewRuleIds.Count() >= GaiaConstants.maxPreviewedTextures)
                        {
                            //Yes, kick lowest rule in stack out first
                            m_spawner.m_previewRuleIds.RemoveAt(0);
                        }

                        //mark this rule for visualisation
                        m_spawner.m_previewRuleIds.Add(spawnRuleID);
                        //Sort the rules ascending, important because the lower rules should overwrite the earlier ones.
                        m_spawner.m_previewRuleIds.Sort();
                    }
                    else
                    {
                        //the spawner was currently not displaying the preview. Throw out any old rules first, and start fresh
                        m_spawner.m_previewRuleIds.Clear();
                        m_spawner.m_previewRuleIds.Add(spawnRuleID);
                        m_spawner.m_drawPreview = true;
                    }
                }

            }

            GUI.backgroundColor = currentBGColor;

            Color currentVisColor = m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_visualisationColor;
            if (GaiaUtils.ColorsEqual(currentVisColor, GaiaConstants.spawnerInitColor))
            {
                //The magic number of 0.618... equals the golden ratio to get an even distributon of colors from the available palette
                currentVisColor = m_gaiaSettings.m_spawnerColorGradient.Evaluate((0.618033988749895f * spawnRuleID) % 1);
            }
            GUILayout.Space(-15);
            m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_visualisationColor = EditorGUILayout.ColorField(currentVisColor, GUILayout.MaxWidth(60));
            GUILayout.Space(5);
        }

        private void DrawSingleRule(SpawnRule rule, int spawnRuleID, bool helpEnabled)
        {
            if (spawnRuleID == m_drawResourcePreviewRuleId)
            {
                Texture2D resourceTexture = GetSpawnRulePreviewTexture(rule, m_spawner.m_settings.m_resources);

                if (resourceTexture != null)
                {
                    //Rect previewRect = EditorGUILayout.GetControlRect();
                    //EditorGUI.LabelField(new Rect(previewRect.x, previewRect.y, EditorGUIUtility.labelWidth, previewRect.height), label);
                    m_editorUtils.Image(resourceTexture);
                    //GUILayout.Space(100);
                }
            }


            rule.m_resourceSettingsFoldedOut = m_editorUtils.Foldout(rule.m_resourceSettingsFoldedOut, "ResourceSettings");
            if (rule.m_resourceSettingsFoldedOut)
            {
                DrawRuleResourceSettings(rule, spawnRuleID, helpEnabled);
            }

            float maxLocationIncrement = 100f;
            if (m_spawner.m_lastActiveTerrain != null)
                maxLocationIncrement = m_spawner.m_lastActiveTerrain.terrainData.size.x * 0.5f;

            if (rule.m_resourceType == GaiaConstants.SpawnerResourceType.TerrainTree)
            {
                m_editorUtils.MinMaxSliderWithFields("TreeProtoLocationIncrement", ref m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_locationIncrementMin, ref m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_locationIncrementMax, 0f, maxLocationIncrement, helpEnabled);
                m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_jitterPercent = m_editorUtils.Slider("TreeProtoJitterPercent", m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_jitterPercent, 0f, 1f, helpEnabled);
            }
            if (rule.m_resourceType == GaiaConstants.SpawnerResourceType.GameObject) // || rule.m_resourceType == GaiaConstants.SpawnerResourceType.GeNaSpawner)
            {
                m_editorUtils.MinMaxSliderWithFields("TreeProtoLocationIncrement", ref m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_locationIncrementMin, ref m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_locationIncrementMax, 0f, maxLocationIncrement, helpEnabled);
                m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_jitterPercent = m_editorUtils.Slider("TreeProtoJitterPercent", m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_jitterPercent, 0f, 1f, helpEnabled);
            }
            if (rule.m_resourceType == GaiaConstants.SpawnerResourceType.TerrainDetail)
            {
                m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_terrainDetailDensity = m_editorUtils.IntSlider("DetailProtoDensity", m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_terrainDetailDensity, 1, 50, helpEnabled);
            }


            //if (m_spawner.m_showStatistics && rule.m_resourceType != GaiaConstants.SpawnerResourceType.TerrainTexture)
            //{
            //    EditorGUILayout.LabelField(GetLabel("Instances Spawned"), new GUIContent(rule.m_activeInstanceCnt.ToString()));
            //}

            //Direction control for spawned POI
            if (rule.m_resourceType == GaiaConstants.SpawnerResourceType.GameObject)
            {
                m_editorUtils.MinMaxSliderWithFields("GameObjectProtoMinMaxDirection", ref rule.m_minDirection, ref rule.m_maxDirection, 0f, 360f, helpEnabled);
                rule.m_boundsCheckQuality = m_editorUtils.Slider("GameObjectProtoBoundsCheckQuality", rule.m_boundsCheckQuality, 1f, 100f, helpEnabled);
                rule.m_minRequiredFitness = m_editorUtils.Slider("GameObjectMinFitness", rule.m_minRequiredFitness, 0, 1, helpEnabled);
                rule.m_goSpawnTarget = (Transform)m_editorUtils.ObjectField("GameObjectSpawnTarget", rule.m_goSpawnTarget, typeof(Transform), true);
            }
            m_maskListBeingDrawn = m_spawner.m_settings.m_spawnerRules[spawnRuleID].m_imageMasks;
            EditorGUI.indentLevel++;
            ImageMaskListEditor.DrawMaskList(ref m_spawnRuleMaskListExpanded[spawnRuleID], m_reorderableRuleMasksLists[spawnRuleID], m_editorUtils);
            EditorGUI.indentLevel--;


        }

        private void DrawRuleResourceSettings(SpawnRule rule, int spawnRuleID, bool helpEnabled)
        {
            rule.m_resourceType = (Gaia.GaiaConstants.SpawnerResourceType)EditorGUILayout.EnumPopup(GetLabel("Resource Type"), rule.m_resourceType);

            GUIContent[] assetChoices = null;
            switch (rule.m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        assetChoices = new GUIContent[m_spawner.m_settings.m_resources.m_texturePrototypes.Length + 1];
                        for (int assetIdx = 0; assetIdx < m_spawner.m_settings.m_resources.m_texturePrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_spawner.m_settings.m_resources.m_texturePrototypes[assetIdx].m_name);
                        }

                        assetChoices[assetChoices.Length - 1] = m_editorUtils.GetContent("AddNewTexture");

                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        assetChoices = new GUIContent[m_spawner.m_settings.m_resources.m_detailPrototypes.Length + 1];
                        for (int assetIdx = 0; assetIdx < m_spawner.m_settings.m_resources.m_detailPrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_spawner.m_settings.m_resources.m_detailPrototypes[assetIdx].m_name);
                        }
                        assetChoices[assetChoices.Length - 1] = m_editorUtils.GetContent("AddNewTerrainDetail");
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        assetChoices = new GUIContent[m_spawner.m_settings.m_resources.m_treePrototypes.Length + 1];
                        for (int assetIdx = 0; assetIdx < m_spawner.m_settings.m_resources.m_treePrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_spawner.m_settings.m_resources.m_treePrototypes[assetIdx].m_name);
                        }
                        assetChoices[assetChoices.Length - 1] = m_editorUtils.GetContent("AddNewTree");
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        assetChoices = new GUIContent[m_spawner.m_settings.m_resources.m_gameObjectPrototypes.Length + 1];
                        for (int assetIdx = 0; assetIdx < m_spawner.m_settings.m_resources.m_gameObjectPrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_spawner.m_settings.m_resources.m_gameObjectPrototypes[assetIdx].m_name);
                        }
                        assetChoices[assetChoices.Length - 1] = m_editorUtils.GetContent("AddNewGameObject");
                        break;
                    }
                //case GaiaConstants.SpawnerResourceType.GeNaSpawner:
                //    {
                //        assetChoices = new GUIContent[m_spawner.m_settings.m_resources.m_geNaSpawnerPrototypes.Length + 1];
                //        for (int assetIdx = 0; assetIdx < m_spawner.m_settings.m_resources.m_geNaSpawnerPrototypes.Length; assetIdx++)
                //        {
                //            assetChoices[assetIdx] = new GUIContent(m_spawner.m_settings.m_resources.m_geNaSpawnerPrototypes[assetIdx].m_name);
                //        }
                //        assetChoices[assetChoices.Length - 1] = m_editorUtils.GetContent("AddNewGeNaSpawner");
                //        break;
                //    }
                    /*
                default:
                    {
                        assetChoices = new GUIContent[m_spawner.m_settings.m_resources.m_stampPrototypes.Length];
                        for (int assetIdx = 0; assetIdx < m_spawner.m_settings.m_resources.m_stampPrototypes.Length; assetIdx++)
                        {
                            assetChoices[assetIdx] = new GUIContent(m_spawner.m_settings.m_resources.m_stampPrototypes[assetIdx].m_name);
                        }
                        break;
                    } */
            }

            rule.m_resourceIdx = EditorGUILayout.Popup(GetLabel("Selected Resource"), rule.m_resourceIdx, assetChoices);
            rule.m_resourceIdx = Mathf.Clamp(rule.m_resourceIdx,0, assetChoices.Length - 1);


            switch (rule.m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        //user wants a new resource? Create one on the fly
                        if (rule.m_resourceIdx == assetChoices.Length - 1)
                        {
                            AddNewTextureResource();
                        }

                        rule.m_name = m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_name;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        //user wants a new resource? Create one on the fly
                        if (rule.m_resourceIdx == assetChoices.Length - 1)
                        {
                            AddNewTerrainDetailResource();
                        }
                        rule.m_name = m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_name;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        //user wants a new resource? Create one on the fly
                        if (rule.m_resourceIdx == assetChoices.Length - 1)
                        {
                            AddNewTreeResource();
                        }
                        rule.m_name = m_spawner.m_settings.m_resources.m_treePrototypes[rule.m_resourceIdx].m_name;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        //user wants a new resource? Create one on the fly
                        if (rule.m_resourceIdx == assetChoices.Length - 1)
                        {
                            AddNewGameObjectResource();
                        }

                        rule.m_name = m_spawner.m_settings.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_name;

                        //See if we can find a custom fitness
                        if (m_spawner.m_settings.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_instances.Length > 0)
                        {
                            GameObject go = m_spawner.m_settings.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_instances[0].m_desktopPrefab;
                            bool gotExtension = false;
                            //if (go.GetComponent<ISpawnRuleExtension>() != null)
                            //{
                            //    gotExtension = true;
                            //}
                            //else
                            //{
                            //    if (go.GetComponentInChildren<ISpawnRuleExtension>() != null)
                            //    {
                            //        gotExtension = true;
                            //    }
                            //}
                            if (gotExtension)
                            {
                                Debug.Log("Got a spawn rule extension on " + go.name);
                            }
                        }
                        break;
                    }
                //case GaiaConstants.SpawnerResourceType.GeNaSpawner:
                //    {
                //        //user wants a new resource? Create one on the fly
                //        if (rule.m_resourceIdx == assetChoices.Length - 1)
                //        {
                //            AddNewGeNaSpawnerResource();
                //        }

                //        rule.m_name = m_spawner.m_settings.m_resources.m_geNaSpawnerPrototypes[rule.m_resourceIdx].m_name;
                //        break;
                //    }



                    /*
                default:
                    {
                        rule.m_name = m_spawner.m_settings.m_resources.m_stampPrototypes[rule.m_resourceIdx].m_name;
                        break;
                    } */
            }

            //Check to see if we can use extended fitness and spawner

            //rule.m_minViableFitness = EditorGUILayout.Slider(GetLabel("Min Viable Fitness"), rule.m_minViableFitness, 0f, 1f);
            //rule.m_failureRate = EditorGUILayout.Slider(GetLabel("Failure Rate"), rule.m_failureRate, 0f, 1f);
            //rule.m_maxInstances = (ulong)EditorGUILayout.LongField(GetLabel("Max Instances"), (long)rule.m_maxInstances);
            //rule.m_ignoreMaxInstances = EditorGUILayout.Toggle(GetLabel("Ignore Max Instances"), rule.m_ignoreMaxInstances);



            if (rule.m_resourceType == GaiaConstants.SpawnerResourceType.TerrainTexture)
            {
                m_textureResourcePrototypeBeingDrawn = m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx];
                //cache the textures to notice if the user changes those, if yes we need to immediately trigger a refresh to switch out the texture on terrain as well
                Texture2D oldDiffuse = m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_texture;
                Texture2D oldNormal = m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_normal;
                Texture2D oldMaskMap = m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_maskmap;
                //m_editorUtils.Panel("Texture Prototype Settings", DrawTexturePrototype, false);
                DrawTexturePrototype(helpEnabled);

                if (oldDiffuse != m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_texture || oldNormal != m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_normal || oldMaskMap != m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_maskmap)
                {
                    //prototype has changed, refresh on terrain & update resource & rule name
                    RefreshTerrainPrototype(spawnRuleID, oldDiffuse);
                    m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_name = m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_texture.name;
                    rule.m_name = m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_name;
                }
                oldDiffuse = null;
                oldNormal = null;
                oldMaskMap = null;

            }

            if (rule.m_resourceType == GaiaConstants.SpawnerResourceType.TerrainDetail)
            {
                Texture2D oldDetailTexture = m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_detailTexture;
                GameObject oldPrefab = m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_detailProtoype;
                m_terrainDetailPrototypeBeingDrawn = m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx];
                

                DrawTerrainDetailPrototype(helpEnabled);

                if (oldDetailTexture != m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_detailTexture || oldPrefab != m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_detailProtoype)
                {
                    //prototype has changed, refresh on terrain & update resource & rule name
                    RefreshTerrainPrototype(spawnRuleID, oldDetailTexture, oldPrefab);
                    if (m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_renderMode == DetailRenderMode.VertexLit)
                    {
                        m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_name = m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_detailProtoype.name;
                    }
                    else
                    {
                        m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_name = m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_detailTexture.name;
                    }
                    rule.m_name = m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_name;
                }
                oldDetailTexture = null;
                oldPrefab = null;


            }

            if (rule.m_resourceType == GaiaConstants.SpawnerResourceType.TerrainTree)
            {
                GameObject oldDesktopPrefab = m_spawner.m_settings.m_resources.m_treePrototypes[rule.m_resourceIdx].m_desktopPrefab;
                m_treeResourcePrototypeBeingDrawn = m_spawner.m_settings.m_resources.m_treePrototypes[rule.m_resourceIdx];

                DrawTreePrototype(helpEnabled);

                if (oldDesktopPrefab != m_spawner.m_settings.m_resources.m_treePrototypes[rule.m_resourceIdx].m_desktopPrefab)
                {
                    RefreshTerrainPrototype(spawnRuleID, null, oldDesktopPrefab);
                    m_spawner.m_settings.m_resources.m_treePrototypes[rule.m_resourceIdx].m_name = m_spawner.m_settings.m_resources.m_treePrototypes[rule.m_resourceIdx].m_desktopPrefab.name;
                    rule.m_name = m_spawner.m_settings.m_resources.m_treePrototypes[rule.m_resourceIdx].m_name;
                }

            }

            if (rule.m_resourceType == GaiaConstants.SpawnerResourceType.GameObject)
            {
                m_gameObjectResourcePrototypeBeingDrawn = m_spawner.m_settings.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx];

                DrawGameObjectPrototype(helpEnabled);

            }

            //if (rule.m_resourceType == GaiaConstants.SpawnerResourceType.GeNaSpawner)
            //{
            //    m_geNaSpawnerPrototypeBeingDrawn = m_spawner.m_settings.m_resources.m_geNaSpawnerPrototypes[rule.m_resourceIdx];

            //    DrawGeNaSpawnerPrototype(helpEnabled);

            //}
        }

        private int AddNewGameObjectResource()
        {
            string newGameObjectName = m_editorUtils.GetTextValue("NewGameObject");
            int nextNewNumber = m_spawner.m_settings.m_resources.m_gameObjectPrototypes.Where(x => x.m_name.StartsWith(newGameObjectName)).Count() + 1;
            m_spawner.m_settings.m_resources.m_gameObjectPrototypes = GaiaUtils.AddElementToArray(m_spawner.m_settings.m_resources.m_gameObjectPrototypes, new ResourceProtoGameObject() { m_name = newGameObjectName + " " + nextNewNumber.ToString() });
            return m_spawner.m_settings.m_resources.m_gameObjectPrototypes.Count() - 1;
        }

        private int AddNewGeNaSpawnerResource()
        {
            string newGeNaSpawner = m_editorUtils.GetTextValue("NewGeNaSpawner");
            int nextNewNumber = m_spawner.m_settings.m_resources.m_geNaSpawnerPrototypes.Where(x => x.m_name.StartsWith(newGeNaSpawner)).Count() + 1;
            m_spawner.m_settings.m_resources.m_geNaSpawnerPrototypes = GaiaUtils.AddElementToArray(m_spawner.m_settings.m_resources.m_geNaSpawnerPrototypes, new ResourceProtoGeNaSpawner() { m_name = newGeNaSpawner + " " + nextNewNumber.ToString() });
            return m_spawner.m_settings.m_resources.m_geNaSpawnerPrototypes.Count() - 1;
        }

        private int AddNewTreeResource()
        {
            string newTreeName = m_editorUtils.GetTextValue("NewTree");
            int nextNewNumber = m_spawner.m_settings.m_resources.m_treePrototypes.Where(x => x.m_name.StartsWith(newTreeName)).Count() + 1;
            m_spawner.m_settings.m_resources.m_treePrototypes = GaiaUtils.AddElementToArray(m_spawner.m_settings.m_resources.m_treePrototypes, new ResourceProtoTree() { m_name = newTreeName + " " + nextNewNumber.ToString() });
            return m_spawner.m_settings.m_resources.m_treePrototypes.Count() - 1;
        }

        private int AddNewTerrainDetailResource()
        {
            string newDetailName = m_editorUtils.GetTextValue("NewTerrainDetail");
            int nextNewNumber = m_spawner.m_settings.m_resources.m_detailPrototypes.Where(x => x.m_name.StartsWith(newDetailName)).Count() + 1;
            m_spawner.m_settings.m_resources.m_detailPrototypes = GaiaUtils.AddElementToArray(m_spawner.m_settings.m_resources.m_detailPrototypes, new ResourceProtoDetail() { m_name = newDetailName + " " + nextNewNumber.ToString() });
            return m_spawner.m_settings.m_resources.m_detailPrototypes.Count() - 1;
        }

        private int AddNewTextureResource()
        {
            string newTextureName = m_editorUtils.GetTextValue("NewTexture");
            int nextNewNumber = m_spawner.m_settings.m_resources.m_texturePrototypes.Where(x => x.m_name.StartsWith(newTextureName)).Count() + 1;
            m_spawner.m_settings.m_resources.m_texturePrototypes = GaiaUtils.AddElementToArray(m_spawner.m_settings.m_resources.m_texturePrototypes, new ResourceProtoTexture() { m_name = newTextureName + " " + nextNewNumber.ToString() });
            return m_spawner.m_settings.m_resources.m_texturePrototypes.Count() - 1;
        }



        public static Texture2D GetSpawnRulePreviewTexture(SpawnRule rule, GaiaResource resource)
        {
            

            Texture2D resourceTexture = null;

            //draw preview
            switch (rule.m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    if (rule.m_resourceIdx < resource.m_texturePrototypes.Length && resource.m_texturePrototypes[rule.m_resourceIdx] != null)
                    {
                         resourceTexture = AssetPreview.GetAssetPreview(resource.m_texturePrototypes[rule.m_resourceIdx].m_texture);
                    }
                    break;
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    if (rule.m_resourceIdx < resource.m_detailPrototypes.Length && resource.m_detailPrototypes[rule.m_resourceIdx] != null)
                    {
                        ResourceProtoDetail protoDetail = resource.m_detailPrototypes[rule.m_resourceIdx];
                        switch (protoDetail.m_renderMode)
                        {
                            case DetailRenderMode.Grass:
                                resourceTexture = protoDetail.m_detailTexture;
                                break;
                            case DetailRenderMode.GrassBillboard:
                                resourceTexture = protoDetail.m_detailTexture;
                                break;
                            case DetailRenderMode.VertexLit:
                                resourceTexture = AssetPreview.GetAssetPreview(protoDetail.m_detailProtoype);
                                break;
                        }
                    }
                    break;

                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    if (rule.m_resourceIdx < resource.m_treePrototypes.Length && resource.m_treePrototypes[rule.m_resourceIdx] != null)
                    {
                        GameObject protoTree = resource.m_treePrototypes[rule.m_resourceIdx].m_desktopPrefab;
                        resourceTexture = AssetPreview.GetAssetPreview(protoTree);
                    }
                    break;
                case GaiaConstants.SpawnerResourceType.GameObject:
                    if (rule.m_resourceIdx < resource.m_gameObjectPrototypes.Length && resource.m_gameObjectPrototypes[rule.m_resourceIdx] != null)
                    {
                        //Get the first instance as preview object for now
                        if (resource.m_gameObjectPrototypes[rule.m_resourceIdx].m_instances.Length > 0)
                        {
                            GameObject protoGameObject = resource.m_gameObjectPrototypes[rule.m_resourceIdx].m_instances[0].m_desktopPrefab;
                            resourceTexture = AssetPreview.GetAssetPreview(protoGameObject);
                        }
                    }
                    break;
            }

            return resourceTexture;
        }

        private void CleanPreviewRuleIDs()
        {
            for (int i = m_spawner.m_previewRuleIds.Count-1; i >= 0; i--)
            {
                if (m_spawner.m_previewRuleIds[i] > m_spawner.m_settings.m_spawnerRules.Count-1)
                {
                    m_spawner.m_previewRuleIds.RemoveAt(i);
                }
            }
        }

        private void RemoveMaskList(int ruleIdx)
        {
            if (ruleIdx < 0 || ruleIdx >= m_reorderableRuleMasksLists.Length)
                return;
            ReorderableList[] newList = new ReorderableList[m_reorderableRuleMasksLists.Length - 1];
            for (int i = 0; i < newList.Length; ++i)
            {
                if (i < ruleIdx)
                {
                    newList[i] = m_reorderableRuleMasksLists[i];
                }
                else if (i >= ruleIdx)
                {
                    newList[i] = m_reorderableRuleMasksLists[i + 1];
                }
            }
            m_reorderableRuleMasksLists = newList;

            if (ruleIdx >= m_spawnRuleMaskListExpanded.Length)
                return;
 
            bool[] newExpandedList = new bool[m_spawnRuleMaskListExpanded.Length - 1];
            for (int i = 0; i < newExpandedList.Length; ++i)
            {
                if (i < ruleIdx)
                {
                    newExpandedList[i] = m_spawnRuleMaskListExpanded[i];
                }
                else if (i >= ruleIdx)
                {
                    newExpandedList[i] = m_spawnRuleMaskListExpanded[i + 1];
                }
            }
            m_spawnRuleMaskListExpanded = newExpandedList;
        }

        private void AddNewMaskList()
        {
            ReorderableList[] newList = new ReorderableList[m_reorderableRuleMasksLists.Length + 1];
            for (int i = 0; i < m_reorderableRuleMasksLists.Length; ++i)
            {
                newList[i] = m_reorderableRuleMasksLists[i];
            }
            newList[newList.Length - 1] = CreateSpawnRuleMaskList(newList[newList.Length - 1], m_spawner.m_settings.m_spawnerRules[newList.Length - 1].m_imageMasks);
            m_reorderableRuleMasksLists = newList;

            bool[] newExpandedList = new bool[m_spawnRuleMaskListExpanded.Length + 1];
            for (int i = 0; i < m_spawnRuleMaskListExpanded.Length; ++i)
            {
                newExpandedList[i] = m_spawnRuleMaskListExpanded[i];
            }

            newExpandedList[newExpandedList.Length - 1] = true;
            m_spawnRuleMaskListExpanded = newExpandedList;
        }

        private void RefreshTerrainPrototype(int spawnRuleID, Texture2D oldTexture=null, GameObject oldGameObject = null)
        {
            Terrain activeTerrain = Gaia.TerrainHelper.GetActiveTerrain();

            if (activeTerrain == null)
            {
                return;
            }
            if (activeTerrain.terrainData == null)
            {
                return;
            }
         

            SpawnRule sr = m_spawner.m_settings.m_spawnerRules[spawnRuleID];
            switch (sr.m_resourceType)
            {
                case SpawnerResourceType.TerrainTexture:
                    int texturePrototypeID = -1;

                    //look up prototype ID based on the old texture, if available it means the user wants to switch out textures
                    if (oldTexture != null)
                    {
                        
                        int localTerrainIdx = 0;
                       
                            foreach (TerrainLayer proto in activeTerrain.terrainData.terrainLayers)
                            {
                                if (PWCommon2.Utils.IsSameTexture(oldTexture, proto.diffuseTexture, false) == true)
                                {
                                    texturePrototypeID = localTerrainIdx;
                                    break;
                                }
                                localTerrainIdx++;
                            }
                        
                    }
                    else
                    {
                        texturePrototypeID = m_spawner.m_settings.m_resources.PrototypeIdxInTerrain(SpawnerResourceType.TerrainTexture, sr.m_resourceIdx);
                    }


                    if (texturePrototypeID != -1)
                    {
                        foreach (Terrain t in Terrain.activeTerrains)
                        {
                            ResourceProtoTexture resourceProtoTexture = m_spawner.m_settings.m_resources.m_texturePrototypes[sr.m_resourceIdx];
                            //reference the exisiting prototypes, then assign them - otherwise the terrain details won't update properly
                            TerrainLayer[] exisitingLayers = t.terrainData.terrainLayers;
                            exisitingLayers[texturePrototypeID].diffuseTexture = resourceProtoTexture.m_texture;
                            exisitingLayers[texturePrototypeID].normalMapTexture = resourceProtoTexture.m_normal;
                            exisitingLayers[texturePrototypeID].maskMapTexture = resourceProtoTexture.m_maskmap;
                            exisitingLayers[texturePrototypeID].tileSize = new Vector2(resourceProtoTexture.m_sizeX, resourceProtoTexture.m_sizeY);
                            exisitingLayers[texturePrototypeID].tileOffset = new Vector2(resourceProtoTexture.m_offsetX, resourceProtoTexture.m_offsetY);
                            exisitingLayers[texturePrototypeID].normalScale = resourceProtoTexture.m_normalScale;
                            exisitingLayers[texturePrototypeID].metallic = resourceProtoTexture.m_metallic;
                            exisitingLayers[texturePrototypeID].smoothness = resourceProtoTexture.m_smoothness;
                            t.terrainData.terrainLayers = exisitingLayers;
                        }
                    }
                    break;
                case SpawnerResourceType.TerrainDetail:
                
                    int detailPrototypeID = -1;

                    //look up prototype ID based on the old texture, if available it means the user wants to switch out textures
                    if (oldTexture != null || oldGameObject != null)
                    {
                        int localTerrainIdx = 0;
                        foreach (DetailPrototype proto in activeTerrain.terrainData.detailPrototypes)
                        {
                            if (m_spawner.m_settings.m_resources.m_detailPrototypes[sr.m_resourceIdx].m_renderMode == DetailRenderMode.VertexLit)
                            {
                                if (oldGameObject == proto.prototype)
                                {
                                    detailPrototypeID = localTerrainIdx;
                                    break;
                                }
                            }
                            else
                            {
                                if (PWCommon2.Utils.IsSameTexture(oldTexture, proto.prototypeTexture, false) == true)
                                {
                                    detailPrototypeID = localTerrainIdx;
                                    break;
                                }
                            }
                            localTerrainIdx++;
                        }
                    }
                    else
                    {
                        detailPrototypeID = m_spawner.m_settings.m_resources.PrototypeIdxInTerrain(SpawnerResourceType.TerrainDetail, sr.m_resourceIdx);
                    }
                    if (detailPrototypeID != -1)
                    {
                        foreach (Terrain t in Terrain.activeTerrains)
                        {
                            ResourceProtoDetail resourceProtoDetail = m_spawner.m_settings.m_resources.m_detailPrototypes[sr.m_resourceIdx];
                            //reference the exisiting prototypes, then assign them - otherwise the terrain details won't update properly
                            DetailPrototype[] exisitingPrototypes = t.terrainData.detailPrototypes;
                            exisitingPrototypes[detailPrototypeID].bendFactor = resourceProtoDetail.m_bendFactor;
                            exisitingPrototypes[detailPrototypeID].dryColor = resourceProtoDetail.m_dryColour;
                            exisitingPrototypes[detailPrototypeID].healthyColor = resourceProtoDetail.m_healthyColour;
                            exisitingPrototypes[detailPrototypeID].maxHeight = resourceProtoDetail.m_maxHeight;
                            exisitingPrototypes[detailPrototypeID].minHeight = resourceProtoDetail.m_minHeight;
                            exisitingPrototypes[detailPrototypeID].minWidth = resourceProtoDetail.m_maxWidth;
                            exisitingPrototypes[detailPrototypeID].maxWidth = resourceProtoDetail.m_minWidth;
                            exisitingPrototypes[detailPrototypeID].noiseSpread = resourceProtoDetail.m_noiseSpread;
                            exisitingPrototypes[detailPrototypeID].prototype = resourceProtoDetail.m_detailProtoype;
                            exisitingPrototypes[detailPrototypeID].prototypeTexture = resourceProtoDetail.m_detailTexture;
                            exisitingPrototypes[detailPrototypeID].renderMode = resourceProtoDetail.m_renderMode;
                            t.terrainData.detailPrototypes = exisitingPrototypes;
                            //t.terrainData.detailPrototypes[prototypeID].usePrototypeMesh = resourceProtoDetail.pr;
                        }
                    }
                    break;

                case SpawnerResourceType.TerrainTree:

                    int treePrototypeID = -1;

                    //look up prototype ID based on the old texture, if available it means the user wants to switch out textures
                    if (oldGameObject != null)
                    {
                        int localTerrainIdx = 0;
                        foreach (TreePrototype proto in activeTerrain.terrainData.treePrototypes)
                        {
                            if (oldGameObject == proto.prefab)
                            {
                                treePrototypeID = localTerrainIdx;
                                break;
                            }
                            localTerrainIdx++;
                        }
                    }
                    else
                    {
                        treePrototypeID = m_spawner.m_settings.m_resources.PrototypeIdxInTerrain(SpawnerResourceType.TerrainDetail, sr.m_resourceIdx);
                    }
                    if (treePrototypeID != -1)
                    {
                        foreach (Terrain t in Terrain.activeTerrains)
                        {
                            ResourceProtoTree resourceProtoTree = m_spawner.m_settings.m_resources.m_treePrototypes[sr.m_resourceIdx];
                            //reference the exisiting prototypes, then assign them - otherwise the terrain details won't update properly
                            TreePrototype[] exisitingPrototypes = t.terrainData.treePrototypes;
                            exisitingPrototypes[treePrototypeID].bendFactor = resourceProtoTree.m_bendFactor;
                            exisitingPrototypes[treePrototypeID].prefab = resourceProtoTree.m_desktopPrefab;
                            t.terrainData.treePrototypes = exisitingPrototypes;
                        }
                    }
                    break;
            }
            
        }

        private void DrawTexturePrototype(bool showHelp)
        {
            GaiaResourceEditor.DrawTexturePrototype(m_textureResourcePrototypeBeingDrawn, m_editorUtils, showHelp);
            if (m_editorUtils.Button("ResourceProtoRefreshButton"))
            {
                RefreshTerrainPrototype(m_spawnRuleIndexBeingDrawn);
            }
        }

        private void DrawTreePrototype(bool showHelp)
        {
            GaiaResourceEditor.DrawTreePrototype(m_treeResourcePrototypeBeingDrawn, m_editorUtils, showHelp);
            if (m_editorUtils.Button("ResourceProtoRefreshButton"))
            {
                RefreshTerrainPrototype(m_spawnRuleIndexBeingDrawn);
            }
        }

        private void DrawTerrainDetailPrototype(bool showHelp)
        {
            GaiaResourceEditor.DrawTerrainDetailPrototype(m_terrainDetailPrototypeBeingDrawn, m_editorUtils, showHelp);
            if (m_editorUtils.Button("ResourceProtoRefreshButton"))
            {
                RefreshTerrainPrototype(m_spawnRuleIndexBeingDrawn);
            }
        }


        private void DrawGameObjectPrototype(bool showHelp)
        {
            GaiaResourceEditor.DrawGameObjectPrototype(m_gameObjectResourcePrototypeBeingDrawn, m_editorUtils, showHelp);
        }

        //private void DrawGeNaSpawnerPrototype(bool showHelp)
        //{
        //    GaiaResourceEditor.DrawGeNaSpawnerPrototype(m_geNaSpawnerPrototypeBeingDrawn, m_editorUtils, showHelp);
        //}


        private void DrawSaveAndLoad(bool obj)
        {
            GUI.backgroundColor = m_normalBGColor;
            if (m_spawner.m_createdfromBiomePreset)
            {
                m_SaveAndLoadMessage = m_editorUtils.GetTextValue("CreatedFromBiomePresetMessage");
                m_SaveAndLoadMessageType = MessageType.Warning;
            }

            if (m_spawner.m_createdFromGaiaManager)
            {
                m_SaveAndLoadMessage = m_editorUtils.GetTextValue("CreatedFromGaiaManagerMessage");
                m_SaveAndLoadMessageType = MessageType.Warning;
            }




            if (m_SaveAndLoadMessage != "")
                EditorGUILayout.HelpBox(m_SaveAndLoadMessage, m_SaveAndLoadMessageType, true);

            EditorGUILayout.BeginHorizontal();
            if (m_editorUtils.Button("LoadButton"))
            {
                //Dismiss Tutorial messages at this point
                m_spawner.m_createdfromBiomePreset = false;
                m_spawner.m_createdFromGaiaManager = false;

                string openFilePath = EditorUtility.OpenFilePanel("Load Spawner settings..", GaiaDirectories.GetDataDirectory(), "asset");


                bool loadConditionsMet = true;

                //Do we have a path to begin with?
                if (openFilePath == null || openFilePath == "")
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
                    SpawnerSettings settingsToLoad = (SpawnerSettings)AssetDatabase.LoadAssetAtPath(openFilePath, typeof(SpawnerSettings));

                    if (settingsToLoad != null)
                    {
                        //Load in the resource file that was last used first
                        
                        //settingsToLoad.m_resourcesPath = AssetDatabase.GUIDToAssetPath(settingsToLoad.m_resourcesGUID);

                        m_spawner.LoadSettings(settingsToLoad);

                        //m_spawner.m_settings.m_resources = (GaiaResource)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(settingsToLoad.m_resourcesGUID), typeof(GaiaResource));

                        CreateMaskLists();
                        //Update the internal editor position / scale values after loading
                        //x = m_stamper.m_settings.m_x;
                        //y = m_stamper.m_settings.m_y;
                        //z = m_stamper.m_settings.m_z;
                        //rotation = m_stamper.m_settings.m_rotation;
                        //width = m_stamper.m_settings.m_width;
                        //height = m_stamper.m_settings.m_height;
                        //mark stamper as dirty so it will be redrawn
                        m_spawner.m_spawnPreviewDirty= true;
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
                //Dismiss Tutorial messages at this point
                m_spawner.m_createdfromBiomePreset = false;
                m_spawner.m_createdFromGaiaManager = false;

                string saveFilePath = EditorUtility.SaveFilePanel("Save Stamper settings as..", GaiaDirectories.GetDataDirectory(), "SpawnerSettings", "asset");

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

                    //Update with newest GUID of resource file
                    //m_spawner.m_settings.m_resourcesGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(m_spawner.m_settings.m_resources));


                    // Check if there is already an asset in this path
                    SpawnerSettings settingsToLoad = (SpawnerSettings)AssetDatabase.LoadAssetAtPath(saveFilePath, typeof(SpawnerSettings));

                    if (settingsToLoad != null)
                    {
                        AssetDatabase.DeleteAsset(saveFilePath);
                    }

                    AssetDatabase.CreateAsset(m_spawner.m_settings, saveFilePath);
                    EditorUtility.SetDirty(m_spawner.m_settings);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    //Check if save was successful
                    settingsToLoad = (SpawnerSettings)AssetDatabase.LoadAssetAtPath(saveFilePath, typeof(SpawnerSettings));
                    if (settingsToLoad != null)
                    {
                        m_SaveAndLoadMessage = m_editorUtils.GetContent("SaveSuccessful").text;
                        m_changesMadeSinceLastSave = false;
                        m_SaveAndLoadMessageType = MessageType.Info;
                        //dissociate the current stamper settings from the file we just saved, otherwise the user will continue editing the file afterwards
                        //We do this by just loading the file in again we just created
                        m_spawner.LoadSettings(settingsToLoad);
                        CreateMaskLists();
                        m_spawner.m_spawnPreviewDirty= true;
                        //update the gaia manager window (if exists)

                        foreach (GaiaManagerEditor gme in Resources.FindObjectsOfTypeAll<GaiaManagerEditor>())
                        {
                            gme.UpdateAllSpawnersList();
                        }


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




        public void Spawn(bool allTerrains)
        {
            bool cancel = true;

            //Check that they have at least one single selected terrain
            if (Gaia.TerrainHelper.GetActiveTerrainCount() < 1)
            {
                EditorUtility.DisplayDialog("OOPS!", "You must have at least one active terrain in order to use a Spawner. Please add a terrain from the Gaia Manager Window or the scene hierarchy create menu.", "OK");
                return;
            }
            else
            {
                cancel = false;
            }

            //Check that the terrain layer is selected
            if (!PWCommon2.Utils.IsInLayerMask(Gaia.TerrainHelper.GetActiveTerrain().gameObject, m_spawner.m_spawnCollisionLayers))
            {
                if (EditorUtility.DisplayDialog("WARNING!", "This feature requires your Spawner to have a layer mask which includes the Terrain layer.", "Add Terrain Layer To Spawner", "Cancel"))
                {
                    m_spawner.m_spawnCollisionLayers.value |= Gaia.TerrainHelper.GetActiveTerrainLayer().value;
                }
                else
                {
                    Debug.Log("Spawn cancelled");
                    cancel = true;
                }
            }

            //Check that the sea level matches the sea level of the session
            //GaiaSessionManager sessionMgr = GaiaSessionManager.GetSessionManager();
            //if (sessionMgr != null && sessionMgr.m_session != null)
            //{
            //    if (m_spawner.m_settings.m_resources.m_seaLevel != sessionMgr.m_session.m_seaLevel)
            //    {
            //        if (EditorUtility.DisplayDialog("WARNING!", "Your resources sea level does not match your session sea level. To ensure consistent treatment of heights while spawning your resources sea level should match your session sea level.", "Update Resources Sea Level ?", "Ignore"))
            //        {
            //            m_spawner.m_settings.m_resources.ChangeSeaLevel(m_spawner.m_settings.m_resources.m_seaLevel, sessionMgr.m_session.m_seaLevel);
            //        }
            //    }
            //}

            //Check that the resources are in the terrain
            if (!cancel)
            {
                cancel = m_spawner.CheckForMissingResources();
                if (cancel)
                {
                    Debug.Log("Spawner " + m_spawner.name + " is missing resources on the terrain, spawn was cancelled. Please deactivate the rule that contains the missing resources, or let the Spawner add the missing resources to the terrain.");
                }


            }

            if (!cancel)
            {
                //Check that they are not using terrain based mask - this can give unexpected results
                //if (areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture0 ||
                //    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture1 ||
                //    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture2 ||
                //    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture3 ||
                //    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture4 ||
                //    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture5 ||
                //    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture6 ||
                //    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture7
                //    )
                //{
                //    //Do an alert and fix if necessary
                //    if (!m_spawner.IsFitToTerrain())
                //    {
                //        if (EditorUtility.DisplayDialog("WARNING!", "This feature requires your Spawner to be Fit To Terrain in order to guarantee correct placement.", "Spawn Anyway", "Cancel"))
                //        {
                //            m_spawner.RunSpawnerIteration(); 
                //        }
                //        else
                //        {
                //            Debug.Log("Spawn cancelled");
                //        }
                //    }
                //    else
                //    {
                //        m_spawner.RunSpawnerIteration();
                //}
                //}
                //else
                //{
                m_spawner.Spawn(allTerrains);
                
                //}
            }

            //deactivate preview - so that we can see the result
          
            if (m_gaiaSettings.m_spawnerAutoHidePreviewMilliseconds > 0)
            {
                m_activatePreviewRequested = true;
                m_activatePreviewTimeStamp = GaiaUtils.GetUnixTimestamp();
            }
            m_spawner.m_drawPreview = false;
        }

      


        /// <summary>
        /// Delete any old rules left over from previous resources / changes to resources
        /// </summary>
        void CleanUpRules()
        {
            //Drop out if no spawner or resources
            if (m_spawner == null || m_spawner.m_settings.m_resources == null)
            {
                return;
            }

            //Drop out if spawner doesnt have resources
            int idx = 0;
            SpawnRule rule;
            bool dirty = false;
            while (idx < m_spawner.m_settings.m_spawnerRules.Count)
            {
                rule = m_spawner.m_settings.m_spawnerRules[idx];

                switch (rule.m_resourceType)
                {
                    case GaiaConstants.SpawnerResourceType.TerrainTexture:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_settings.m_resources.m_texturePrototypes.Length)
                            {
                                m_spawner.m_settings.m_spawnerRules.RemoveAt(idx);
                                dirty = true;
                            }
                            else
                            {
                                if (rule.m_name != m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_name)
                                {
                                    rule.m_name = m_spawner.m_settings.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_name;
                                    dirty = true;
                                }
                                idx++;
                            }
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.TerrainDetail:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_settings.m_resources.m_detailPrototypes.Length)
                            {
                                m_spawner.m_settings.m_spawnerRules.RemoveAt(idx);
                                dirty = true;
                            }
                            else
                            {
                                if (rule.m_name != m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_name)
                                {
                                    rule.m_name = m_spawner.m_settings.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_name;
                                    dirty = true;
                                }
                                idx++;
                            }
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.TerrainTree:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_settings.m_resources.m_treePrototypes.Length)
                            {
                                m_spawner.m_settings.m_spawnerRules.RemoveAt(idx);
                                dirty = true;
                            }
                            else
                            {
                                if (rule.m_name != m_spawner.m_settings.m_resources.m_treePrototypes[rule.m_resourceIdx].m_name)
                                {
                                    rule.m_name = m_spawner.m_settings.m_resources.m_treePrototypes[rule.m_resourceIdx].m_name;
                                    dirty = true;
                                }
                                idx++;
                            }
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.GameObject:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_settings.m_resources.m_gameObjectPrototypes.Length)
                            {
                                m_spawner.m_settings.m_spawnerRules.RemoveAt(idx);
                                dirty = true;
                            }
                            else
                            {
                                if (rule.m_name != m_spawner.m_settings.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_name)
                                {
                                    rule.m_name = m_spawner.m_settings.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_name;
                                    dirty = true;
                                }
                                idx++;
                            }
                            break;
                        }
                    //case GaiaConstants.SpawnerResourceType.GeNaSpawner:
                    //    {
                    //        if (rule.m_resourceIdx >= m_spawner.m_settings.m_resources.m_geNaSpawnerPrototypes.Length)
                    //        {
                    //            m_spawner.m_settings.m_spawnerRules.RemoveAt(idx);
                    //            dirty = true;
                    //        }
                    //        else
                    //        {
                    //            if (rule.m_name != m_spawner.m_settings.m_resources.m_geNaSpawnerPrototypes[rule.m_resourceIdx].m_name)
                    //            {
                    //                rule.m_name = m_spawner.m_settings.m_resources.m_geNaSpawnerPrototypes[rule.m_resourceIdx].m_name;
                    //                dirty = true;
                    //            }
                    //            idx++;
                    //        }
                    //        break;
                    //    }

                    default:
                            idx++;
                    break;
                        
                }
            }
            //Mark it as dirty if we deleted something
            if (dirty)
            {
                Debug.LogWarning(string.Format("{0} : There was a mismatch between your spawner settings and your resources file. Spawner settings have been updated to match resources.", m_spawner.name));
                EditorUtility.SetDirty(m_spawner);
            }
        }

        /// <summary>
        /// Draw a progress bar
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
        /// Handy layer mask interface
        /// </summary>
        /// <param name="label"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        static LayerMask LayerMaskField(GUIContent label, LayerMask layerMask)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                    maskWithoutEmpty |= (1 << i);
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;
            return layerMask;
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
        static Dictionary<string, string> m_tooltips = new Dictionary<string, string>
        {
            { "Resources", "The object that contains the resources that these rules will apply to." },
            { "Execution Mode", "The way this spawner runs. Design time : At design time only. Runtime Interval : At run time on a timed interval. Runtime Triggered Interval : At run time on a timed interval, and only when the tagged game object is closer than the trigger range from the center of the spawner." },
            { "Shape", "The shape of the spawn area. The spawner will only spawn within this area." },
            { "Range","Distance in meters from the centre of the spawner that the spawner can spawn in. Shown as a red box or sphere in the gizmos." },
            { "Spawn Interval", "The time in seconds between spawn iterations." },
            { "Trigger Range","Distance in meters from the centre of the spawner that the trigger will activate." },
            { "Trigger Tags","The tags of the game objects that will set the spawner off. Multiple tags can be separated by commas eg Player,Minion etc." },
            { "Rule Selector", "The way a rule is selected to be spawned. \nAll : All rules are selected. \nFittest : Only the rule with the fittest spawn criteria is selected. If multiple rules have the same fitness then one will be randomly selected.\nWeighted Fittest : The chance of a rule being selected is directly proportional to its fitness. Fitter rules have more chance of selection. Use this to create more natural blends between objects.\nRandom : Rule selection is random." },
            { "Spawn Mdoe", "This setting controls whether the spawned instances will replace, will be added to, or will be removed from the existing instances on the terrain." },
            { "Collision Layers", "Controls which layers are checked for collisions when spawning. Must at least include the layer the terrain is on. Add additional layers if other collisions need to be detected as well. Influences terrain detection, tree detection and game object detection." },
            { "Location Selector", "How the spawner selects locations to spawn in. \nEvery Location: The spawner will attempt to spawn at every location. \nEvery Location Jittered: The spawner will attempt to spawn at every location, but will offset the location by a random jitter factor. Use this to break up lines.\nRandom Location: The spawner will attempt to spawn at random locations.\nRandom Location Clustered: The spawner will attempt to spawn clusters at random locations." },
            { "Location Increment", "The distance from the last location that every new location will be incremented in meters." },
            { "Max Jitter Percent", "Every new location will be offset by a random distance up to a maximum of the jitter percentage multiplied by the location increment." },
            { "Locations Per Spawn", "The number of locations that will be checked every Spawn interval. This does not guarantee that something will be spawned at that location, because lack of fitness may preclude that location from being used." },
            { "Max Cluster Size", "The maximum individuals in a cluster before a new cluster is started." },

            { "X", "Delete all rules."},
            { "I", "Inavtivate all rules."},
            { "A", "Activate all rules."},
            { "+", "Add a rule."},
            { "-", "Delete the rule."},
            { "Visualise", "Visualise this rule in the visualiser."},

            { "Distance Mask", "Mask fitness over distance. Left hand side of curve represents the centre of the spawner. Use this to alter spawn success away from centre e.g. peter out towards edges."},
            { "Area Mask", "Mask fitness over area. None - Don't apply image filter. Grey Scale - apply image filter using greys scale. R - Apply filter from red channel. G - Apply filter from green channel. B - Apply filter from blue channel. A - Apply filter from alpha channel. Terrain Texture Slot - apply mask from texture painted on terrain."},
            { "Image Mask", "The texure to use as the source of the area mask."},
            { "Smooth Mask", "Smooth the mask before applying it. This is a nice way to clean noise up in the mask, or to soften the edges of the mask."},
            { "Normalise Mask", "Normalise the mask before applying it. Ensures that the full dynamic range of the mask is used."},
            { "Invert Mask", "Invert the mask before applying it."},
            { "Flip Mask", "Flip the mask on its x and y axis mask before applying it. Useful sometimes to match the unity terrain as this is flipped internally."},
            { "Seed", "The unique seed for this spawner. If the environment, resources or rules dont change, then hitting Reset and respawning will always regenerate the same result." },

            { "Noise Mask", "Mask fitness with a noise value."},
            { "Noise Seed", "The seed value for the noise function - the same seed will always generate the same noise for a given set of parameters."},
            { "Octaves", "The amount of detail in the noise - more octaves mean more detail and longer calculation time."},
            { "Persistence", "The roughness of the noise. Controls how quickly amplitudes diminish for successive octaves. 0..1."},
            { "Frequency", "The frequency of the first octave."},
            { "Lacunarity", "The frequency multiplier between successive octaves. Experiment between 1.5 - 3.5."},
            { "Zoom", "The zoom level of the noise. Larger zooms display the noise over larger areas."},
            { "Invert", "Invert the noise."},


            { "Name", "Rule name - purely for convenience" },
            { "Resource Type", "The type of resource this rule will apply to." },
            { "Selected Resource", "The resource this rule applies to. To modify how the resource interprets terrain fitness change its spawn criteria." },
            { "Min Viable Fitness", "The minimum fitness needed to be considered viable to spawn." },
            { "Failure Rate", "The amount of the time that the rule will fail even if fit enough. 0 means never fail, and 1 means always fail. Use this to thin things out." },
            { "Max Instances", "The maximum number of resource instances this rule can spawn. Use this to stop over population." },
            { "Ignore Max Instances", "Ignores the max instances criteria. Useful for texturing very large terrains." },
            { "Active", "Whether this rule is active or not. Use this to disable the rule."},
            { "Curr Inst Count", "The number of instances of this rule that have been spawned."},
            { "Instances Spawned", "The number of times this resource has been spawned." },
            { "Inactive Inst Count", "The number of inactive instances that have been spawned, but are now inactive and in the pool for re-use. Only relevant when game objects have been spawned" },
         
            { "Active Rules", "The number of active rules being managed by the spawner."},
            { "Inactive Rules", "The number of inactive rules being managed by the spawner."},
            { "TOTAL Rules", "The total number of rules being managed by the spawner."},
            { "MAX INSTANCES", "The maximum number of instances that can be managed by the spawner."},
            { "Active Instances", "The number of active instances being managed by the spawner."},
            { "Inactive Instances", "The number inactive instances being managed by the spawner."},
            { "TOTAL Instances", "The total number of active and inactive instances being managed by the spawner."},

            { "Min Direction", "Minimum rotation in degrees for this game object spawner." },
            { "Max Direction", "Maximum rotation in degrees fpr this game object spawner." },

            { "Ground Level", "Ground level for this feature, used to make positioning easier." },
            { "Show Ground Level", "Show ground level." },
            { "Stick To Ground", "Stick to ground level." },
            { "Show Gizmos", "Show the spawners gizmos." },
            { "Show Rulers", "Show rulers." },
            { "Show Statistics", "Show spawner statistics." },
            { "Flatten", "Flatten the entire terrain - use with care!" },
            { "Smooth", "Smooth the entire terrain - removes jaggies and increases frame rate - run multiple times to increase effect - use with care!" },
            { "Clear Trees", "Clear trees from entire terrain - use with care!" },
            { "Clear Details", "Clear details / grass from entire terrain - use with care!" },
            { "Ground", "Position the spawner at ground level on the terrain." },
            { "Fit To Terrain", "Fits and aligns the spawner to the terrain." },
            { "Reset", "Resets the spawner, deletes any spawned game objects, and resets the random number generator." },
            { "Spawn Local", "Run a single spawn iteration at the exact spawner position and range. You can run as many spawn iterations as you like." },
            { "Spawn World", "Run a single spawn iteration on every terrain in the scene. You can run as many spawn iterations as you like." },
        };

    }
}
