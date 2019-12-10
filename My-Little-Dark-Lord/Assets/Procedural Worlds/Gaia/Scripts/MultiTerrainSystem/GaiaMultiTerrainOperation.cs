using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using static Gaia.GaiaConstants;

namespace Gaia
{
    /// <summary>
    /// Enum to signal the type of operation that is supposed to be performed across multiple terrrains.
    /// </summary>
    public enum MultiTerrainOperationType { Heightmap, Texture, Tree, TerrainDetail, GameObject, Collision }

    /// <summary>
    /// Dictionary Key to bind a terrain that is affected by a specific operation type. 
    /// </summary>
    public struct TerrainOperation
    {
        public Terrain terrain;
        public MultiTerrainOperationType operationType;
    }

    /// <summary>
    /// The affected pixels on the respective map (heightmap, splatmap, etc.) when a multiterrain operation is performed.
    /// </summary>
    public class AffectedPixels
    {
        /// <summary>
        /// The coordinates in the total operation for these affected pixels
        /// </summary>
        public Vector2Int pixelCoordinate;

        /// <summary>
        /// The affected pixels on the local map on the terrain.
        /// </summary>
        /// 
        public RectInt affectedLocalPixels;

        /// <summary>
        /// The affected pixels in the total operation.
        /// </summary>
        public RectInt affectedOperationPixels;


        /// <summary>
        /// Splatmap ID / Layer ID for texture operations.
        /// </summary>
        public int splatMapID;

        /// <summary>
        /// Channel ID (color channel of the splatmap image) for texture oprations.
        /// </summary>
        public int channelID;


        /// <summary>
        /// Returns true if this area actually contains any affected pixels
        /// </summary>
        /// <returns></returns>
        public bool IsAffected()
        {
            return !(affectedLocalPixels.width == 0 || affectedLocalPixels.height == 0);
        }
    }

    /// <summary>
    /// Handles getting and setting height, textures, trees, etc. in a multi-terrain context
    /// </summary>
    public class GaiaMultiTerrainOperation
    {
        /// <summary>
        /// The terrain from which the multi-terrain operation originates (where the stamper / spawner is placed over atm)
        /// </summary>
        public Terrain m_originTerrain;

        /// <summary>
        /// The transform from which the multi-terrain operation originates (the stamper / spawner transform usually)
        /// </summary>
        public Transform m_originTransform;

        /// <summary>
        /// The range of the operation.
        /// </summary>
        public float m_range;


        /// <summary>
        /// List of all terrain data objects affected by a splatmap change. This list can then be processed for splatmap syncing on closing the operation.
        /// </summary>
        private List<TerrainData> affectedSplatmapData = new List<TerrainData>();

        /// <summary>
        /// List of all terrain objects affected by a heightmpa change. This list can then be processed for heightmap syncing on closing the operation.
        /// </summary>
        public List<Terrain> affectedHeightmapData = new List<Terrain>();


        //Render Textures to hold queried existing data from this multi-terrain operation

        public RenderTexture RTheightmap;
        public RenderTexture RTnormalmap;
        public RenderTexture RTterrainTree;
        public RenderTexture RTgameObject;
        public RenderTexture RTdetailmap;
        public RenderTexture RTtextureSplatmap;
        public RenderTexture RTcollision; 


        public RectInt m_heightmapPixels;
        private RectInt m_texturePixels;
        private RectInt m_terrainTreePixels;
        private RectInt m_collisionPixels;
        private RectInt m_gameObjectPixels;
        private RectInt m_terrainDetailPixels;

        private Vector2 m_heightmapPixelSize;
        
        private Vector2 m_texturePixelSize;
        private Vector2 m_terrainDetailPixelSize;
        private Vector2 m_terrainTreePixelSize;
        private Vector2 m_gameObjectPixelSize;
        private Vector2 m_collisionPixelSize;

        private BrushTransform m_heightmapBrushTransform;
        private BrushTransform m_textureBrushTransform;
        private BrushTransform m_terrainTreeBrushTransform;
        private BrushTransform m_collisionBrushTransform;
        private BrushTransform m_gameObjectBrushTransform;
        public BrushTransform m_terrainDetailBrushTransform;

        /// <summary>
        /// Holds the affected pixels per terrain per operation.
        /// </summary>
        public Dictionary<TerrainOperation, AffectedPixels> affectedTerrainPixels = new Dictionary<TerrainOperation, AffectedPixels>();

        private static Material m_GaiaStamperPreviewMaterial;
        private static Material m_GaiaSpawnerPreviewMaterial;

        private int m_originalMasterTextureLimit = 0;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="originTerrain">The origin terrain that serves as a reference point for the whole operation.</param>
        /// <param name="toolTransform">The transform of the tool (spawner / stamper) that calls the whole operation.</param>
        /// <param name="range">The range of the operation</param>
        public GaiaMultiTerrainOperation(Terrain originTerrain, Transform toolTransform, float range, bool fullTextureQuality = false)
        {
            //All operations need to be executed with masterTextureLimit = 0, else there are quality / functional issues in the render texture processing!
            m_originalMasterTextureLimit = QualitySettings.masterTextureLimit;
            if (fullTextureQuality)
            {
                QualitySettings.masterTextureLimit = 0;
            }

            m_originTerrain = originTerrain;
            m_originTransform = toolTransform;
            m_range = range;
        }

        #region OPERATIONS

        #region HEIGHTMAP

        /// <summary>
        /// Gets the heightmap data for this operation and stores it in the RTheightmap render texture.
        /// </summary>
        public void GetHeightmap()
        {
            RenderTexture previousRT = RenderTexture.active;
            int heightmapResolution = m_originTerrain.terrainData.heightmapResolution;

            m_heightmapPixelSize = new Vector2(
            m_originTerrain.terrainData.size.x / (heightmapResolution - 1.0f),
            m_originTerrain.terrainData.size.z / (heightmapResolution - 1.0f));

            m_heightmapBrushTransform = TerrainPaintUtility.CalculateBrushTransform(m_originTerrain, GaiaUtils.ConvertPositonToTerrainUV(m_originTerrain, new Vector2(m_originTransform.position.x, m_originTransform.position.z)), m_range, m_originTransform.rotation.eulerAngles.y);

            m_heightmapPixels = GetPixelsForResolution(m_originTerrain.terrainData.size, m_heightmapBrushTransform.GetBrushXYBounds(), heightmapResolution, heightmapResolution, 0);


            CreateDefaultRenderTexture(ref RTheightmap, m_heightmapPixels.width, m_heightmapPixels.height, Terrain.heightmapRenderTextureFormat);

          

            AddAffectedTerrainPixels(m_heightmapPixels, MultiTerrainOperationType.Heightmap, heightmapResolution, heightmapResolution);
            Material blitMaterial = TerrainPaintUtility.GetBlitMaterial();
            RenderTexture.active = RTheightmap;
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, m_heightmapPixels.width, 0, m_heightmapPixels.height);

            var relevantEntries = affectedTerrainPixels.Where(x => x.Key.operationType == MultiTerrainOperationType.Heightmap);

            foreach (var entry in relevantEntries)
            {
                if (!entry.Value.IsAffected())
                    continue;

                Texture originTexture = entry.Key.terrain.terrainData.heightmapTexture;
                if ((originTexture.width != heightmapResolution) || (originTexture.height != heightmapResolution))
                {
                    Debug.LogWarning(String.Format("Mismatched heightmap resolutions between terrains: The terrain {0} does use a different heightmap resolution than the origin terrain, this terrain will be ignored.", entry.Key.terrain.name));
                    continue;
                }

                FilterMode oldFilterMode = originTexture.filterMode;

                originTexture.filterMode = FilterMode.Point;

                blitMaterial.SetTexture("_MainTex", originTexture);
                blitMaterial.SetPass(0);
                CopyIntoPixels(entry.Value.affectedOperationPixels, entry.Value.affectedLocalPixels, originTexture);

                originTexture.filterMode = oldFilterMode;
            }

            GL.PopMatrix();

            RenderTexture.active = previousRT;

        }

        private void CreateDefaultRenderTexture(ref RenderTexture renderTex, int width, int height, RenderTextureFormat renderTextureFormat)
        {
            if (renderTex != null)
            {
                RenderTexture.ReleaseTemporary(renderTex);
            }
            renderTex = RenderTexture.GetTemporary(width, height, 0, renderTextureFormat, RenderTextureReadWrite.Linear);
            renderTex.wrapMode = TextureWrapMode.Clamp;
            renderTex.filterMode = FilterMode.Point;
        }

        /// <summary>
        /// Sets the heightmap across the operation area according to the information in the supplied Render Texture.
        /// </summary>
        /// <param name="newHeightmapRT">The render texture containing the new target heightmap.</param>
        public void SetHeightmap(RenderTexture newHeightmapRT)
        {
            var previousRT = RenderTexture.active;
            RenderTexture.active = newHeightmapRT;
            int heightmapResolution = m_originTerrain.terrainData.heightmapResolution;
            var relevantEntries = affectedTerrainPixels.Where(x => x.Key.operationType == MultiTerrainOperationType.Heightmap);

            foreach (var entry in relevantEntries)
            {
                if (!entry.Value.IsAffected())
                    continue;

                var terrainData = entry.Key.terrain.terrainData;
                if ((terrainData.heightmapResolution != heightmapResolution) || (terrainData.heightmapResolution != heightmapResolution))
                {
                    Debug.LogWarning(String.Format("Mismatched heightmap resolutions between terrains: The terrain {0} does use a different heightmap resolution than the origin terrain, this terrain will be ignored.", entry.Key.terrain.name));
                    continue;
                }

                terrainData.CopyActiveRenderTextureToHeightmap(entry.Value.affectedOperationPixels, entry.Value.affectedLocalPixels.min, entry.Key.terrain.drawInstanced ? TerrainHeightmapSyncControl.None : TerrainHeightmapSyncControl.HeightOnly);
                if (!affectedHeightmapData.Contains(entry.Key.terrain))
                {
                    affectedHeightmapData.Add(entry.Key.terrain);
                }
            }

            RenderTexture.active = previousRT;

            RenderTexture.ReleaseTemporary(RTheightmap);
            RTheightmap = null;
        }

        #endregion

        #region NORMALMAP

        /// <summary>
        /// Gets the normal map data for this operation and stores it in the RTnormalmap render texture.
        /// </summary>
        public void GetNormalmap()
        {

            RenderTexture previousRT = RenderTexture.active;
            int heightmapResolution = m_originTerrain.terrainData.heightmapResolution;

            m_heightmapPixelSize = new Vector2(
            m_originTerrain.terrainData.size.x / (heightmapResolution - 1.0f),
            m_originTerrain.terrainData.size.z / (heightmapResolution - 1.0f));

            m_terrainDetailBrushTransform = TerrainPaintUtility.CalculateBrushTransform(m_originTerrain, GaiaUtils.ConvertPositonToTerrainUV(m_originTerrain, new Vector2(m_originTransform.position.x, m_originTransform.position.z)), m_range, m_originTransform.rotation.eulerAngles.y);

            m_heightmapPixels = GetPixelsForResolution(m_originTerrain.terrainData.size, m_terrainDetailBrushTransform.GetBrushXYBounds(), heightmapResolution, heightmapResolution, 0);

            if (RTnormalmap != null)
            {
                RenderTexture.ReleaseTemporary(RTnormalmap);
            }
            CreateDefaultRenderTexture(ref RTnormalmap, m_heightmapPixels.width, m_heightmapPixels.height, Terrain.normalmapRenderTextureFormat);

            Material mat = TerrainPaintUtility.GetBlitMaterial();

            RenderTexture.active = RTnormalmap;
            GL.Clear(false, true, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, m_heightmapPixels.width, 0, m_heightmapPixels.height);

            var relevantEntries = affectedTerrainPixels.Where(x => x.Key.operationType == MultiTerrainOperationType.Heightmap);

            foreach (var entry in relevantEntries)
            {
                Texture originTexture = entry.Key.terrain.normalmapTexture;
                if (originTexture == null)
                {
                    Debug.LogWarning("Normal maps missing on terrain " + entry.Key.terrain.name);
                    continue;
                }

                if (!entry.Value.IsAffected())
                    continue;

                if ((originTexture.width != heightmapResolution) || (originTexture.height != heightmapResolution))
                {
                    Debug.LogWarning(String.Format("Mismatched heightmap resolutions between terrains: The terrain {0} does use a different heightmap resolution than the origin terrain, this terrain will be ignored.", entry.Key.terrain.name));
                    continue;
                }

                FilterMode oldFilterMode = originTexture.filterMode;

                originTexture.filterMode = FilterMode.Point;

                mat.SetTexture("_MainTex", originTexture);
                mat.SetPass(0);

                CopyIntoPixels(entry.Value.affectedOperationPixels, entry.Value.affectedLocalPixels, originTexture);

                originTexture.filterMode = oldFilterMode;
            }

            GL.PopMatrix();

            RenderTexture.active = previousRT;

        }

        #endregion

        #region SPLATMAP

        /// <summary>
        /// Gets the splatmap data for this operation and the passed terrain layer and stores it in the RTtexturesplatmap render texture.
        /// </summary>
        public void GetSplatmap(TerrainLayer layer)
        {
            RenderTexture currentRT = RenderTexture.active;

            if (layer == null)
                return;

            int controlTextureResolution = m_originTerrain.terrainData.alphamapResolution;

            m_texturePixelSize = new Vector2(
            m_originTerrain.terrainData.size.x / (controlTextureResolution - 1.0f),
            m_originTerrain.terrainData.size.z / (controlTextureResolution - 1.0f));

            m_textureBrushTransform = TerrainPaintUtility.CalculateBrushTransform(m_originTerrain, GaiaUtils.ConvertPositonToTerrainUV(m_originTerrain, new Vector2(m_originTransform.position.x, m_originTransform.position.z)), m_range, m_originTransform.rotation.eulerAngles.y);

            m_texturePixels = GetPixelsForResolution(m_originTerrain.terrainData.size, m_textureBrushTransform.GetBrushXYBounds(), controlTextureResolution, controlTextureResolution,0);

            if (RTtextureSplatmap != null)
            {
                RenderTexture.ReleaseTemporary(RTtextureSplatmap);
            }

            CreateDefaultRenderTexture(ref RTtextureSplatmap, m_texturePixels.width, m_texturePixels.height, RenderTextureFormat.R8);

            AddAffectedTerrainPixels(m_texturePixels, MultiTerrainOperationType.Texture, controlTextureResolution, controlTextureResolution, layer);

            RenderTexture.active = RTtextureSplatmap;
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, m_texturePixels.width, 0, m_texturePixels.height);



            Material mat = TerrainPaintUtility.GetCopyTerrainLayerMaterial();

            Vector4[] lmArray = {   new Vector4(1, 0, 0, 0),
                                    new Vector4(0, 1, 0, 0),
                                    new Vector4(0, 0, 1, 0),
                                    new Vector4(0, 0, 0, 1) };

            var relevantEntries = affectedTerrainPixels.Where(x => x.Key.operationType == MultiTerrainOperationType.Texture);

            foreach (var entry in relevantEntries)
            {
                if (!entry.Value.IsAffected())
                    continue;


                Texture originTexture = TerrainPaintUtility.GetTerrainAlphaMapChecked(entry.Key.terrain, entry.Value.splatMapID);
                if ((originTexture.width != controlTextureResolution) || (originTexture.height != controlTextureResolution))
                {
                    Debug.LogWarning("Mismatched control texture resolution on one of the terrains. Expected:( " +
                        originTexture.width + " x " + originTexture.height + ") Found: (" + controlTextureResolution + " x " + controlTextureResolution + ")",
                        entry.Key.terrain);
                    continue;
                }

                FilterMode oldFilterMode = originTexture.filterMode;
                originTexture.filterMode = FilterMode.Point;


                mat.SetVector("_LayerMask", lmArray[entry.Value.channelID]);
                mat.SetTexture("_MainTex", originTexture);
                mat.SetPass(0);

                CopyIntoPixels(entry.Value.affectedOperationPixels, entry.Value.affectedLocalPixels, originTexture);

                originTexture.filterMode = oldFilterMode;
            }

            GL.PopMatrix();
            
            RenderTexture.active = currentRT;

        }

        /// <summary>
        /// Sets the splatmap across the operation area according to the information in the supplied Render Texture. Note that this will always affect the texture layer that was fetched last via GetSplatmap().
        /// </summary>
        /// <param name="paintRenderTexture">The render texture containing the new target splatmap.</param>
        /// <param name="centerTerrainOnly">Should the splatmap be applied to the center terrain of the operation only?</param>
        public void SetSplatmap(RenderTexture paintRenderTexture, bool centerTerrainOnly)
        {
            
            Vector4[] layerMasks = { new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1) };

            Material mat = TerrainPaintUtility.GetCopyTerrainLayerMaterial();

            int controlTextureResolution = m_originTerrain.terrainData.alphamapResolution;

            var rtd = new RenderTextureDescriptor(paintRenderTexture.width, paintRenderTexture.height, RenderTextureFormat.ARGB32);
            rtd.useMipMap = false;
            rtd.autoGenerateMips = false;
            rtd.sRGB = false;

            RenderTexture paintTarget = RenderTexture.GetTemporary(rtd);
            RenderTexture.active = paintTarget;

            var relevantEntries = affectedTerrainPixels.Where(x => x.Key.operationType == MultiTerrainOperationType.Texture && (x.Key.terrain == m_originTerrain || !centerTerrainOnly));

            foreach (var entry in relevantEntries)
            {
                if (!entry.Value.IsAffected())
                    continue;

                RectInt paintRect = entry.Value.affectedOperationPixels;

                Rect sourceRect = new Rect(
                    paintRect.x / (float)m_texturePixels.width,
                    paintRect.y / (float)m_texturePixels.height,
                    paintRect.width / (float)m_texturePixels.width,
                    paintRect.height / (float)m_texturePixels.height);

                paintRenderTexture.filterMode = FilterMode.Point;

                int splatMapID = entry.Value.splatMapID;
                int channelID = entry.Value.channelID;
                var terrainData = entry.Key.terrain.terrainData;
                var splats = terrainData.alphamapTextures;
                for (int j = 0; j < splats.Length; j++)
                {
                    Texture2D sourceTex = splats[j];
                    if ((sourceTex.width != controlTextureResolution) || (sourceTex.height != controlTextureResolution))
                    {
                        Debug.LogWarning("Mismatched control texture resolution on one of the terrains. Expected:( " +
                        sourceTex.width + " x " + sourceTex.height + ") Found: (" + controlTextureResolution + " x " + controlTextureResolution + ")",
                        entry.Key.terrain);
                        continue;
                    }

                    Rect completeRect = new Rect(
                         entry.Value.affectedLocalPixels.x / (float)sourceTex.width,
                         entry.Value.affectedLocalPixels.y / (float)sourceTex.height,
                         entry.Value.affectedLocalPixels.width / (float)sourceTex.width,
                         entry.Value.affectedLocalPixels.height / (float)sourceTex.height);

                    mat.SetTexture("_MainTex", paintRenderTexture);
                    mat.SetTexture("_OldAlphaMapTexture", RTtextureSplatmap);
                    mat.SetTexture("_OriginalTargetAlphaMap", splats[splatMapID]);

                    mat.SetTexture("_AlphaMapTexture", sourceTex);
                    mat.SetVector("_LayerMask", j == splatMapID ? layerMasks[channelID] : Vector4.zero);
                    mat.SetVector("_OriginalTargetAlphaMask", layerMasks[channelID]);
                    mat.SetPass(1);

                    GL.PushMatrix();
                    GL.LoadPixelMatrix(0, paintTarget.width, 0, paintTarget.height);

                    GL.Begin(GL.QUADS);
                    GL.Color(new Color(1.0f, 1.0f, 1.0f, 1.0f));

                    GL.MultiTexCoord2(0, sourceRect.x, sourceRect.y);
                    GL.MultiTexCoord2(1, completeRect.x, completeRect.y);
                    GL.Vertex3(paintRect.x, paintRect.y, 0.0f);
                    GL.MultiTexCoord2(0, sourceRect.x, sourceRect.yMax);
                    GL.MultiTexCoord2(1, completeRect.x, completeRect.yMax);
                    GL.Vertex3(paintRect.x, paintRect.yMax, 0.0f);
                    GL.MultiTexCoord2(0, sourceRect.xMax, sourceRect.yMax);
                    GL.MultiTexCoord2(1, completeRect.xMax, completeRect.yMax);
                    GL.Vertex3(paintRect.xMax, paintRect.yMax, 0.0f);
                    GL.MultiTexCoord2(0, sourceRect.xMax, sourceRect.y);
                    GL.MultiTexCoord2(1, completeRect.xMax, completeRect.y);
                    GL.Vertex3(paintRect.xMax, paintRect.y, 0.0f);

                    GL.End();
                    GL.PopMatrix();
                    
                    terrainData.CopyActiveRenderTextureToTexture(TerrainData.AlphamapTextureName, j, paintRect, entry.Value.affectedLocalPixels.min, true);
                    
                    if (!affectedSplatmapData.Contains(terrainData))
                    {
                        affectedSplatmapData.Add(terrainData);
                    }
                }
            }

            //remove the entries from dictionary, as it will negatively affect other texture spawns
            foreach (var entry in affectedTerrainPixels.Where(x => x.Key.operationType == MultiTerrainOperationType.Texture).ToList())
            {
                affectedTerrainPixels.Remove(entry.Key);
            }

 

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(paintRenderTexture);
            paintRenderTexture = null;

            RenderTexture.ReleaseTemporary(paintTarget);
            paintTarget = null;
        }

        #endregion

        #region TERRAIN DETAILS

        /// <summary>
        /// Collects the affected terrains for a terrain detail operation. 
        /// </summary>
        public void CollectTerrainDetails()
        {
            RenderTexture previousRT = RenderTexture.active;
            int terrainDetailResolution = m_originTerrain.terrainData.detailResolution;



            m_terrainDetailPixelSize = new Vector2(
            m_originTerrain.terrainData.size.x / (terrainDetailResolution - 1.0f),
            m_originTerrain.terrainData.size.z / (terrainDetailResolution - 1.0f));

            m_terrainDetailBrushTransform = TerrainPaintUtility.CalculateBrushTransform(m_originTerrain, GaiaUtils.ConvertPositonToTerrainUV(m_originTerrain, new Vector2(m_originTransform.position.x, m_originTransform.position.z)), m_range, m_originTransform.rotation.eulerAngles.y);

            m_terrainDetailPixels = GetPixelsForResolution(m_originTerrain.terrainData.size, m_terrainDetailBrushTransform.GetBrushXYBounds(), terrainDetailResolution, terrainDetailResolution, 0);

            CreateDefaultRenderTexture(ref RTdetailmap, m_terrainDetailPixels.width, m_terrainDetailPixels.height, RenderTextureFormat.R16);
            
            AddAffectedTerrainPixels(m_terrainDetailPixels, MultiTerrainOperationType.TerrainDetail, terrainDetailResolution, terrainDetailResolution);

        }

        /// <summary>
        /// Sets the terrain details across the operation area according to the information in the supplied Render Texture. 
        /// </summary>
        /// <param name="targetDetailTexture">A render texture containing the desired new detail distribution.</param>
        /// <param name="terrainLayerID">The terrain detail prototype ID.</param>
        /// <param name="spawnMode">The used spawn mode (replace, add, etc.)</param>
        /// <param name="centerTerrainOnly">Should the details be applied to the center terrain of the operation only?</param>
        public void SetTerrainDetails(RenderTexture targetDetailTexture, int terrainLayerID, SpawnMode spawnMode, int terrainDetailDensity, ref int instanceCounter, bool centerTerrainOnly = false)
        {
            Color[] colors = GetRTColorArray(targetDetailTexture);
            int terrainDetailResolution = m_originTerrain.terrainData.detailResolution;

            var relevantEntries = affectedTerrainPixels.Where(x => x.Key.operationType == MultiTerrainOperationType.TerrainDetail && (x.Key.terrain == m_originTerrain || !centerTerrainOnly));

            foreach (var entry in relevantEntries)
            {

                if (!entry.Value.IsAffected())
                    continue;

                var terrainData = entry.Key.terrain.terrainData;
                if ((terrainData.detailResolution != terrainDetailResolution) || (terrainData.detailResolution != terrainDetailResolution))
                {
                    Debug.LogWarning("Mismatched terrain detail resolution on terrain: " + entry.Key.terrain.name);
                    continue;
                }

                int colorIndex = 0;
                //build up a int map
                int[,] map = new int[terrainData.detailResolution, terrainData.detailResolution];

                // this will be set to -1 if the spawn mode is set to remove to invert the spawn.
                int invert = 1;

                //If we don't replace the terrain detail data we have to read in the existing data first
                if (spawnMode != SpawnMode.Replace)
                {
                    map = terrainData.GetDetailLayer(0, 0, terrainData.detailResolution, terrainData.detailResolution, terrainLayerID);
                    if (spawnMode == SpawnMode.Remove)
                    {
                        invert = -1;
                    }
                }
                else
                {
                    instanceCounter = 0;
                }

                int maxX = entry.Value.affectedLocalPixels.x + entry.Value.affectedLocalPixels.width;
                int maxY = entry.Value.affectedLocalPixels.y + entry.Value.affectedLocalPixels.height;

                for (int x = entry.Value.affectedLocalPixels.x; x < maxX; x++)
                {
                    for (int y = entry.Value.affectedLocalPixels.y; y < maxY; y++)
                    {
                        //We have to calculate the correct color index on the original Paint context
                        //this must work for all cases, partial intersection  of the Paint Context with Local Context, LC fully encapsulated by PC, etc.
                        colorIndex = Mathf.Clamp(((y - (entry.Value.affectedLocalPixels.y - entry.Value.affectedOperationPixels.y)) * targetDetailTexture.descriptor.width) + x - (entry.Value.affectedLocalPixels.x - entry.Value.affectedOperationPixels.x), 0, colors.Length - 1);

                        //At higher strength values we put a grass amount according to strength, at lower values, we start to "thin out" the grass
                        //by putting more and more 0s in our map array. Note that the map array assignment is inverted in the next line, else the result will be flipped!
                        int oldValue = map[y, x];
                        map[y, x] = (int)Mathf.Clamp(map[y, x] + invert * Mathf.InverseLerp(0.5f, 1f, colors[colorIndex].r) * terrainDetailDensity, 0, terrainDetailDensity);  //(UnityEngine.Random.value < (Mathf.Pow(colors[colorIndex].r, 5f)) ? (int)(colors[colorIndex].r * terrainDetailDensity) : 0), 0, terrainDetailDensity);
                        //map[y, x] = Mathf.Clamp(map[y, x] + invert * (UnityEngine.Random.value < (Mathf.Pow(colors[colorIndex].r, 5f)) ? (int)(colors[colorIndex].r * terrainDetailDensity) : 0), 0, terrainDetailDensity);

                        instanceCounter += map[y, x] - oldValue;
                    }
                }

                terrainData.SetDetailLayer(0, 0, terrainLayerID, map);
            }

            RenderTexture.ReleaseTemporary(targetDetailTexture);
            targetDetailTexture = null;
        }

        #endregion

        #region TERRAIN TREES

        /// <summary>
        /// Collects the affected terrains for a terrain tree operation. 
        /// </summary>
        public void CollectTerrainTrees()
        {
            RenderTexture previousRT = RenderTexture.active;
            int terrainTreeResolution = (int)m_originTerrain.terrainData.size.x;

            m_terrainTreePixelSize = new Vector2(
            m_originTerrain.terrainData.size.x / (terrainTreeResolution - 1.0f),
            m_originTerrain.terrainData.size.z / (terrainTreeResolution - 1.0f));

            m_terrainTreeBrushTransform = TerrainPaintUtility.CalculateBrushTransform(m_originTerrain, GaiaUtils.ConvertPositonToTerrainUV(m_originTerrain, new Vector2(m_originTransform.position.x, m_originTransform.position.z)), m_range, m_originTransform.rotation.eulerAngles.y);

            m_terrainTreePixels = GetPixelsForResolution(m_originTerrain.terrainData.size, m_terrainTreeBrushTransform.GetBrushXYBounds(), terrainTreeResolution, terrainTreeResolution, 0);

            CreateDefaultRenderTexture(ref RTterrainTree, m_terrainTreePixels.width, m_terrainTreePixels.height, RenderTextureFormat.R16);

            AddAffectedTerrainPixels(m_terrainTreePixels, MultiTerrainOperationType.Tree, terrainTreeResolution, terrainTreeResolution);
        }


        /// <summary>
        /// Sets the terrain trees across the operation area according to the distribution information in the supplied Render Texture. 
        /// </summary>
        /// <param name="targetTreeTexture">The render texture containing the desired tree distribution info.</param>
        /// <param name="protoTypeIndex">The tree prototype index on the terrain.</param>
        /// <param name="protoTree">The Gaia tree prototype data.</param>
        /// <param name="spawnMode">The used spawn mode (replace, add, etc.)</param>
        /// <param name="centerTerrainOnly">Should the trees be applied to the center terrain of the operation only?</param>
        public void SetTerrainTrees(RenderTexture targetTreeTexture, int protoTypeIndex, ResourceProtoTree protoTree, SpawnRule rule, SpawnMode spawnMode, ref int instanceCounter, float removalStrength = 0f, bool centerTerrainOnly = false)
        {
            Color[] colors = GetRTColorArray(targetTreeTexture);

            var relevantEntries = affectedTerrainPixels.Where(x => x.Key.operationType == MultiTerrainOperationType.Tree && (x.Key.terrain == m_originTerrain || !centerTerrainOnly));

            foreach (var entry in relevantEntries)
            {
                if (!entry.Value.IsAffected())
                    continue;

                var terrainData = entry.Key.terrain.terrainData;

                int colorIndex = 0;
                //build up a tree instance List
                List<TreeInstance> spawnedTreeInstances = new List<TreeInstance>();

               

                //Now handled in spawner before spawning
                //if (spawnMode == SpawnMode.Replace)
                //{
                //    //In replace mode, we do a selection of the existing tree instances but minus the one we are spawning atm,
                //    //then write that info right back to the terrain. This will remove only the tree type we are spawning right now.
                //    terrainData.SetTreeInstances(terrainData.treeInstances.Where(x => x.prototypeIndex != protoTypeIndex).ToArray(), true);
                //    instanceCounter = 0;
                //}


                //Add the existing trees to our list
                spawnedTreeInstances.AddRange(terrainData.treeInstances);

                //int localResolution = Mathf.Max(terrainData.alphamapResolution, terrainData.heightmapResolution);
                RenderTextureDescriptor rtDesc = targetTreeTexture.descriptor;
                rtDesc.width = Mathf.RoundToInt(terrainData.size.x);
                rtDesc.height = Mathf.RoundToInt(terrainData.size.z);

                if (spawnMode == SpawnMode.Add || spawnMode == SpawnMode.Replace)
                {
                    //In Add or Replace spawn mode, we have to iterate over the terrain and add instances
                    float strength = 0f;
                    float lastXstrength = 0f;
                    float lastYstrength = 0f;

                    
                    for (float x = 0; x < terrainData.size.x; x += rule.m_locationIncrementMin)
                    {
                        float bestStrength = 0f;
                        x += (1f - lastXstrength) * (rule.m_locationIncrementMax - rule.m_locationIncrementMin);
                        for (float y = 0; y < terrainData.size.z; y += rule.m_locationIncrementMin)
                        {
                            //Add up the difference to max location increment, depending on the last strength value

                            y += (1f - lastYstrength) * (rule.m_locationIncrementMax - rule.m_locationIncrementMin);

                            //Jitter the position
                            float xPos = x + rule.m_locationIncrementMin * UnityEngine.Random.Range(-rule.m_jitterPercent, rule.m_jitterPercent * 2f);
                            float yPos = y + rule.m_locationIncrementMin * UnityEngine.Random.Range(-rule.m_jitterPercent, rule.m_jitterPercent * 2f);


                            int localX = Mathf.RoundToInt(xPos * rtDesc.width / terrainData.size.x);
                            int localY = Mathf.RoundToInt(yPos * rtDesc.height / terrainData.size.z);


                            if (entry.Value.affectedLocalPixels.Contains(new Vector2Int(localX, localY)))
                            {

                                colorIndex = (localY - (entry.Value.affectedLocalPixels.y - entry.Value.affectedOperationPixels.y)) * targetTreeTexture.descriptor.width + localX - (entry.Value.affectedLocalPixels.x - entry.Value.affectedOperationPixels.x);
                                strength = (colorIndex >= 0 && colorIndex < colors.Length - 1) ? colors[colorIndex].r : 0;

                                if (UnityEngine.Random.value < Mathf.Pow(strength, 5f))
                                {
                                    TreeInstance treeInstance = new TreeInstance();
                                    treeInstance.prototypeIndex = protoTypeIndex;
                                    treeInstance.position = new Vector3(xPos / (float)terrainData.size.x, 0, yPos / (float)terrainData.size.z);

                                    //Determine width and height scale according to the prototype settings
                                    switch (protoTree.m_spawnScale)
                                    {
                                        case SpawnScale.Fixed:
                                            treeInstance.widthScale = protoTree.m_minWidth;
                                            treeInstance.heightScale = protoTree.m_minHeight;
                                            break;
                                        case SpawnScale.Random:
                                            float randomValue = UnityEngine.Random.value;
                                            treeInstance.widthScale = Mathf.Lerp(protoTree.m_minWidth, protoTree.m_maxWidth, randomValue);
                                            treeInstance.heightScale = Mathf.Lerp(protoTree.m_minHeight, protoTree.m_maxHeight, randomValue);
                                            break;
                                        case SpawnScale.Fitness:
                                            float fitnessValue = Mathf.InverseLerp(0, Mathf.Pow(1, 5f), Mathf.Pow(strength, 5f));
                                            treeInstance.widthScale = Mathf.Lerp(protoTree.m_minWidth, protoTree.m_maxWidth, fitnessValue);
                                            treeInstance.heightScale = Mathf.Lerp(protoTree.m_minHeight, protoTree.m_maxHeight, fitnessValue);
                                            //Debug.Log("Width:" + treeInstance.widthScale.ToString());
                                            //Debug.Log("Height:" + treeInstance.heightScale.ToString());
                                            break;
                                        case SpawnScale.FitnessRandomized:
                                            float FRFitnessValue = Mathf.InverseLerp(0, Mathf.Pow(1, 5f), Mathf.Pow(strength, 5f));
                                            treeInstance.widthScale = Mathf.Lerp(protoTree.m_minWidth, protoTree.m_maxWidth, FRFitnessValue);
                                            treeInstance.heightScale = Mathf.Lerp(protoTree.m_minHeight, protoTree.m_maxHeight, FRFitnessValue);
                                            treeInstance.widthScale *= UnityEngine.Random.Range(1f-protoTree.m_widthRandomPercentage, 1f+ protoTree.m_widthRandomPercentage);
                                            treeInstance.heightScale *= UnityEngine.Random.Range(1f - protoTree.m_heightRandomPercentage, 1f + protoTree.m_heightRandomPercentage);
                                            break;
                                    }



                                    //if (protoTree.m_dna.m_rndScaleInfluence)
                                    //{
                                    //    treeInstance.heightScale = UnityEngine.Random.Range(protoTree.m_dna.m_minScale, protoTree.m_dna.m_maxScale);
                                    //    treeInstance.widthScale = UnityEngine.Random.Range(protoTree.m_dna.m_minScale, protoTree.m_dna.m_maxScale);
                                    //}
                                    //else
                                    //{
                                    //    treeInstance.heightScale = protoTree.m_dna.m_height * strength;
                                    //    treeInstance.widthScale = protoTree.m_dna.m_width * strength;
                                    //}
                                    treeInstance.rotation = UnityEngine.Random.Range(0, 360f);
                                    treeInstance.color = Color.Lerp(protoTree.m_dryColour, protoTree.m_healthyColour, strength);
                                    treeInstance.lightmapColor = Color.white;
                                    spawnedTreeInstances.Add(treeInstance);

                                    if (strength > bestStrength)
                                    {
                                        bestStrength = strength;
                                    }

                                    instanceCounter++;
                                }

                            }
                            lastYstrength = bestStrength;

                        } // for y loop
                        lastXstrength = bestStrength;
                    } //for x loop
                }
                else
                {
                    //In Remove Spawn mode we iterate through the instances of our current prototype ID, and remove them from the array according to their fitness value on the terrain
                    for (int i = spawnedTreeInstances.Count - 1; i >= 0; i--)
                    {
                        TreeInstance treeInstance = spawnedTreeInstances[i];
                        if (treeInstance.prototypeIndex == protoTypeIndex)
                        {
                            float terrainXPos = treeInstance.position.x * terrainData.size.x;
                            float terrainYPos = treeInstance.position.z * terrainData.size.z;

                            int localX = Mathf.RoundToInt(terrainXPos * rtDesc.width / terrainData.size.x);
                            int localY = Mathf.RoundToInt(terrainYPos * rtDesc.height / terrainData.size.z);

                            if (entry.Value.affectedLocalPixels.Contains(new Vector2Int(localX, localY)))
                            {
                                colorIndex = (localY - (entry.Value.affectedLocalPixels.y - entry.Value.affectedOperationPixels.y)) * targetTreeTexture.descriptor.width + localX - (entry.Value.affectedLocalPixels.x - entry.Value.affectedOperationPixels.x);
                                float strength = (colorIndex >= 0 && colorIndex < colors.Length - 1) ? colors[colorIndex].r : 0;
                                if (strength>removalStrength)
                                {
                                    spawnedTreeInstances.Remove(treeInstance);
                                    instanceCounter--;
                                }
                            }
                        }
                    }
                }

                terrainData.SetTreeInstances(spawnedTreeInstances.ToArray(), true);
                //GaiaSessionManager.GetSessionManager().m_collisionMaskCache.BakeTerrainTreeCollisions(entry.Key.terrain, protoTypeIndex, 5f);
            }
            RenderTexture.ReleaseTemporary(targetTreeTexture);
            targetTreeTexture = null;
        }

        #endregion

        #region GAME OBJECTS
        /// <summary>
        /// Collects the affected terrains for a game object spawn. 
        /// </summary>
        public void CollectTerrainGameObjects()
        {
            RenderTexture previousRT = RenderTexture.active;
            int terrainGameObjectResolution = (int)m_originTerrain.terrainData.size.x;

            m_gameObjectPixelSize = new Vector2(
            m_originTerrain.terrainData.size.x / (terrainGameObjectResolution - 1.0f),
            m_originTerrain.terrainData.size.z / (terrainGameObjectResolution - 1.0f));

            m_gameObjectBrushTransform = TerrainPaintUtility.CalculateBrushTransform(m_originTerrain, GaiaUtils.ConvertPositonToTerrainUV(m_originTerrain, new Vector2(m_originTransform.position.x, m_originTransform.position.z)), m_range, m_originTransform.rotation.eulerAngles.y);

            m_gameObjectPixels = GetPixelsForResolution(m_originTerrain.terrainData.size, m_terrainDetailBrushTransform.GetBrushXYBounds(), terrainGameObjectResolution, terrainGameObjectResolution, 0);

            CreateDefaultRenderTexture(ref RTgameObject, m_gameObjectPixels.width, m_gameObjectPixels.height, RenderTextureFormat.R16);
            
            AddAffectedTerrainPixels(m_gameObjectPixels, MultiTerrainOperationType.GameObject, terrainGameObjectResolution, terrainGameObjectResolution);
        }

        /// <summary>
        /// Spawns game objects across the operation area according to the distribution information in the supplied Render Texture. 
        /// </summary>
        /// <param name="targetGameObjectTexture">The render texture containing the desired game object distribution info</param>
        /// <param name="protoGO">The Gaia tree game object prototype data</param>
        /// <param name="target">A target transform under which to create the new Game Object instances.</param>
        /// <param name="spawnMode">The used spawn mode (replace, add, etc.)</param>
        /// <param name="centerTerrainOnly">Should the Game Objects be applied to the center terrain of the operation only?</param>
        public void SetTerrainGameObjects(RenderTexture targetGameObjectTexture, ResourceProtoGameObject protoGO, SpawnRule rule, Transform target, SpawnMode spawnMode, ref int instanceCounter, float removalStrength = 0f, bool centerTerrainOnly = false)
        {
            Color[] colors = GetRTColorArray(targetGameObjectTexture);
           
           

            var relevantEntries = affectedTerrainPixels.Where(x => x.Key.operationType == MultiTerrainOperationType.GameObject && (x.Key.terrain == m_originTerrain || !centerTerrainOnly));

            foreach (var entry in relevantEntries)
            {
                if (!entry.Value.IsAffected())
                    continue;


                var terrainData = entry.Key.terrain.terrainData;
                int colorIndex = 0;


                RenderTextureDescriptor rtDesc = targetGameObjectTexture.descriptor;
                rtDesc.width = Mathf.RoundToInt(terrainData.size.x);
                rtDesc.height = Mathf.RoundToInt(terrainData.size.z);

                if (spawnMode == SpawnMode.Add || spawnMode == SpawnMode.Replace)
                {
                    //Adding or replacing: We iterate over the terrain and add instances according to fitness
                    //(The removal for replace mode already took place in the spawner earlier)

                    float strength = 0f;
                    float lastXstrength = 0f;
                    float lastYstrength = 0f;

                    //add a random starting offset between min max increment to avoid spawns becoming too similar
                    float startX = UnityEngine.Random.Range(rule.m_locationIncrementMin, rule.m_locationIncrementMax);
                    float startY = UnityEngine.Random.Range(rule.m_locationIncrementMin, rule.m_locationIncrementMax);


                    if (spawnMode == SpawnMode.Replace)
                    {
                        instanceCounter = 0;
                    }

                    for (float x = startX; x < terrainData.size.x; x += rule.m_locationIncrementMin)
                    {
                        float bestStrength = 0f;
                        x += (1f - lastXstrength) * (rule.m_locationIncrementMax - rule.m_locationIncrementMin);
                        for (float y = startY; y < terrainData.size.z; y += rule.m_locationIncrementMin)
                        {
                            //Add up the difference to max location increment, depending on the last strength value

                            y += (1f - lastYstrength) * (rule.m_locationIncrementMax - rule.m_locationIncrementMin);

                            //Jitter the position
                            float xPos = x + rule.m_locationIncrementMin * UnityEngine.Random.Range(-rule.m_jitterPercent, rule.m_jitterPercent * 2f);
                            float yPos = y + rule.m_locationIncrementMin * UnityEngine.Random.Range(-rule.m_jitterPercent, rule.m_jitterPercent * 2f);

                            //Debug.Log("X:" + xPos.ToString() + " Y:" + yPos.ToString());

                            int localX = Mathf.RoundToInt(xPos * rtDesc.width / terrainData.size.x);
                            int localY = Mathf.RoundToInt(yPos * rtDesc.height / terrainData.size.z);


                            if (entry.Value.affectedLocalPixels.Contains(new Vector2Int(localX, localY)))
                            {
                                colorIndex = (localY - (entry.Value.affectedLocalPixels.y - entry.Value.affectedOperationPixels.y)) * targetGameObjectTexture.descriptor.width + localX - (entry.Value.affectedLocalPixels.x - entry.Value.affectedOperationPixels.x);
                                strength = (colorIndex >= 0 && colorIndex < colors.Length - 1) ? colors[colorIndex].r : 0;
                                if (rule.m_minRequiredFitness < strength)
                                {

                                    float spawnRotationY = UnityEngine.Random.Range(rule.m_minDirection, rule.m_maxDirection);

                                    //For Gamebjects we don't want to take the strength directly as only spawn criteria - GOs spawn over a larger area, need to check the average strength of the area first, bx doing a sub-iteration.
                                    float bounds = protoGO.m_dna.m_boundsRadius;
                                    float subXMin = xPos - bounds;
                                    float subYMin = yPos - bounds;
                                    float subXMax = xPos + bounds;
                                    float subYMax = yPos + bounds;

                                    float accumulatedStrength = 0f;
                                    float numberOfChecks = 0;
                                    float increment = (subXMax - subXMin) / rule.m_boundsCheckQuality;


                                    for (float subX = subXMin; subX <= subXMax; subX += increment)
                                    {
                                        for (float subY = subYMin; subY <= subYMax; subY += increment)
                                        {
                                            int subLocalX = Mathf.RoundToInt(subX * rtDesc.width / terrainData.size.x);
                                            int subLocalY = Mathf.RoundToInt(subY * rtDesc.height / terrainData.size.z);
                                            if (entry.Value.affectedLocalPixels.Contains(new Vector2Int(localX, localY)))
                                            {
                                                colorIndex = (subLocalY - (entry.Value.affectedLocalPixels.y - entry.Value.affectedOperationPixels.y)) * targetGameObjectTexture.descriptor.width + subLocalX - (entry.Value.affectedLocalPixels.x - entry.Value.affectedOperationPixels.x);
                                                accumulatedStrength += (colorIndex >= 0 && colorIndex < colors.Length - 1) ? colors[colorIndex].r : 0;
                                            }
                                            else
                                            {
                                                //We hit outside the spawn area, don't want to spawn in this case anyways
                                                accumulatedStrength = int.MinValue;
                                            }
                                            numberOfChecks++;
                                        }
                                    }

                                    //only actually spawn if the minimum strength is still being hit on average across the bounds area

                                    float avg = accumulatedStrength / numberOfChecks;

                                    if (avg < rule.m_minRequiredFitness)
                                    {
                                        continue;
                                    }

                                    if (avg > bestStrength)
                                    {
                                        bestStrength = avg;
                                    }

                                    float scale = protoGO.m_dna.m_scaleMultiplier;
                                    
                                    //Determine the scale according to 

                                   
                                    int spawnedInstances = 0;
                                    float boundsRadius = protoGO.m_dna.m_boundsRadius * scale;
                                    Vector3 scaleVect = new Vector3(scale, scale, scale);
                                    float xWorldSpace = xPos + entry.Key.terrain.transform.position.x;
                                    float zWorldSpace = yPos + entry.Key.terrain.transform.position.z;
                                    float yWorldSpace = terrainData.GetInterpolatedHeight(xPos / (float)terrainData.size.x, yPos / (float)terrainData.size.z) + entry.Key.terrain.transform.position.y;
                                    Vector3 worldSpacelocation = new Vector3(xWorldSpace, yWorldSpace, zWorldSpace);

                                    ResourceProtoGameObjectInstance gpi;

                                    for (int idx = 0; idx < protoGO.m_instances.Length; idx++)
                                    {
                                        gpi = protoGO.m_instances[idx];

                                        if (gpi.m_desktopPrefab == null)
                                        {
                                            Debug.LogWarning("Spawn Rule " + rule.m_name + " is missing a prefab for a GameObject. Please check the resource settings in this rule and check if the instance at position " + (idx + 1).ToString() + " has a prefab maintained.");
                                            continue;
                                        }

                                        spawnedInstances = UnityEngine.Random.Range(gpi.m_minInstances, gpi.m_maxInstances + 1); //Randomly choose how many instances to spawn
                                        for (int inst = 0; inst < spawnedInstances; inst++) //For each instance
                                        {
                                            if (UnityEngine.Random.value >= gpi.m_failureRate) //Handle failure override
                                            {
                                                Vector3 instanceLocation = worldSpacelocation;
                                                instanceLocation.x += (UnityEngine.Random.Range(gpi.m_minSpawnOffsetX, gpi.m_maxSpawnOffsetX) * scale);
                                                instanceLocation.z += (UnityEngine.Random.Range(gpi.m_minSpawnOffsetZ, gpi.m_maxSpawnOffsetZ) * scale);
                                                instanceLocation = Gaia.GaiaUtils.RotatePointAroundPivot(instanceLocation, worldSpacelocation, new Vector3(0f, spawnRotationY, 0f));

                                                float scalarX = (instanceLocation.x - entry.Key.terrain.transform.position.x) / (float)terrainData.size.x;
                                                float scalarZ = (instanceLocation.z - entry.Key.terrain.transform.position.z) / (float)terrainData.size.z;

                                                instanceLocation.y = terrainData.GetInterpolatedHeight(scalarX, scalarZ) + entry.Key.terrain.transform.position.y;
                                                instanceLocation.y += (UnityEngine.Random.Range(gpi.m_minSpawnOffsetY, gpi.m_maxSpawnOffsetY) * scale);

#if UNITY_EDITOR
                                                GameObject go = PrefabUtility.InstantiatePrefab(gpi.m_desktopPrefab) as GameObject;
#else
                                                GameObject go = GameObject.Instantiate(gpi.m_desktopPrefab) as GameObject;
#endif

                                                go.name = "_Sp_" + go.name;

                                                //location = gpSpawnInfo.m_hitLocationWU;

                                                go.transform.position = instanceLocation;

                                                //Determine the local scale according to the instance resource settings


                                                int instanceLocalX = Mathf.RoundToInt((instanceLocation.x - entry.Key.terrain.transform.position.x) * rtDesc.width / terrainData.size.x);
                                                int instanceLocalY = Mathf.RoundToInt((instanceLocation.z - entry.Key.terrain.transform.position.z) * rtDesc.height / terrainData.size.z);

                                                colorIndex = (instanceLocalY - (entry.Value.affectedLocalPixels.y - entry.Value.affectedOperationPixels.y)) * targetGameObjectTexture.descriptor.width + instanceLocalX - (entry.Value.affectedLocalPixels.x - entry.Value.affectedOperationPixels.x);
                                                float instanceFitness = (colorIndex >= 0 && colorIndex < colors.Length - 1) ? colors[colorIndex].r : 0;
                                                


                                                Vector3 localScale = new Vector3();
                                                switch (gpi.m_spawnScale)
                                                {
                                                    case SpawnScale.Fixed:
                                                        if (gpi.m_commonScale)
                                                        {
                                                            localScale = new Vector3(gpi.m_minScale, gpi.m_minScale, gpi.m_minScale);
                                                        }
                                                        else
                                                        {
                                                            localScale = new Vector3(gpi.m_minXYZScale.x, gpi.m_minXYZScale.y, gpi.m_minXYZScale.z);
                                                        }
                                                        break;
                                                    case SpawnScale.Random:
                                                        if (gpi.m_commonScale)
                                                        {
                                                            float randomValue = UnityEngine.Random.Range(gpi.m_minScale, gpi.m_maxScale);
                                                            localScale = new Vector3(randomValue, randomValue, randomValue);
                                                        }
                                                        else
                                                        {
                                                            localScale = new Vector3(UnityEngine.Random.Range(gpi.m_minXYZScale.x, gpi.m_maxXYZScale.x),
                                                                                    UnityEngine.Random.Range(gpi.m_minXYZScale.y, gpi.m_maxXYZScale.y),
                                                                                    UnityEngine.Random.Range(gpi.m_minXYZScale.z, gpi.m_maxXYZScale.z));
                                                        }
                                                        break;
                                                    case SpawnScale.Fitness:
                                                        if (gpi.m_commonScale)
                                                        {
                                                            float fitnessValue = Mathf.Lerp(gpi.m_minScale, gpi.m_maxScale, instanceFitness);
                                                            localScale = new Vector3(fitnessValue, fitnessValue, fitnessValue);
                                                        }
                                                        else
                                                        {
                                                            localScale = new Vector3(Mathf.Lerp(gpi.m_minScale, gpi.m_maxScale, instanceFitness),
                                                                                    Mathf.Lerp(gpi.m_minScale, gpi.m_maxScale, instanceFitness),
                                                                                    Mathf.Lerp(gpi.m_minScale, gpi.m_maxScale, instanceFitness));
                                                        }
                                                        break;
                                                    case SpawnScale.FitnessRandomized:
                                                        if (gpi.m_commonScale)
                                                        {
                                                            float fitnessValue = Mathf.Lerp(gpi.m_minScale, gpi.m_maxScale, instanceFitness);
                                                            localScale = new Vector3(fitnessValue, fitnessValue, fitnessValue);
                                                            localScale *= UnityEngine.Random.Range(1f - gpi.m_scaleRandomPercentage, 1f + gpi.m_scaleRandomPercentage);
                                                        }
                                                        else
                                                        {
                                                            float xScale = Mathf.Lerp(gpi.m_minXYZScale.x, gpi.m_maxXYZScale.x, instanceFitness);
                                                            float yScale = Mathf.Lerp(gpi.m_minXYZScale.y, gpi.m_maxXYZScale.y, instanceFitness);
                                                            float zScale = Mathf.Lerp(gpi.m_minXYZScale.z, gpi.m_maxXYZScale.z, instanceFitness);
                                                            xScale *= UnityEngine.Random.Range(1f - gpi.m_XYZScaleRandomPercentage.x, 1f + gpi.m_XYZScaleRandomPercentage.x);
                                                            yScale *= UnityEngine.Random.Range(1f - gpi.m_XYZScaleRandomPercentage.y, 1f + gpi.m_XYZScaleRandomPercentage.y);
                                                            zScale *= UnityEngine.Random.Range(1f - gpi.m_XYZScaleRandomPercentage.z, 1f + gpi.m_XYZScaleRandomPercentage.z);

                                                            localScale = new Vector3(xScale,yScale,zScale);
                                                        }
                                                        break; 
                                                }

                                                float localDist = Vector3.Distance(worldSpacelocation, instanceLocation);
                                                float distanceScale = gpi.m_scaleByDistance.Evaluate(localDist / boundsRadius);

                                                go.transform.localScale = localScale * scale * distanceScale;


                                                //if (gpi.m_useParentScale)
                                                //{
                                                //    localScale = scale;
                                                //    go.transform.localScale = scaleVect;
                                                //}
                                                //else
                                                //{
                                                //    localDist = Vector3.Distance(worldSpacelocation, instanceLocation);
                                                //    localScale = gpi.m_minScale + (gpi.m_scaleByDistance.Evaluate(localDist / boundsRadius) * UnityEngine.Random.Range(0f, gpi.m_maxScale - gpi.m_minScale));
                                                //    go.transform.localScale = new Vector3(localScale, localScale, localScale);
                                                //}

                                                go.transform.rotation = Quaternion.Euler(
                                                    new Vector3(
                                                        UnityEngine.Random.Range(gpi.m_minRotationOffsetX, gpi.m_maxRotationOffsetX),
                                                        UnityEngine.Random.Range(gpi.m_minRotationOffsetY + spawnRotationY, gpi.m_maxRotationOffsetY + spawnRotationY),
                                                        UnityEngine.Random.Range(gpi.m_minRotationOffsetZ, gpi.m_maxRotationOffsetZ)));

                                                if (protoGO.m_instances[idx].m_rotateToSlope == true)
                                                {
                                                    go.transform.rotation = Quaternion.FromToRotation(go.transform.up, terrainData.GetInterpolatedNormal(scalarX, scalarZ)) * go.transform.rotation;
                                                }

                                                ////Set the parent
                                                if (target != null)
                                                {
                                                    go.transform.parent = target;
                                                }
                                                instanceCounter++;
                                            }
                                        }
                                    }

                                }

                            }
                            lastYstrength = strength;
                        } // for y loop
                        lastXstrength = bestStrength;
                    } //for x loop
                }

                else //Spawn Mode check
                {
                    //Remove Mode: We iterate through the spawned instances below the target transform and remove them according to the fitness
#if UNITY_EDITOR
                    for (int i = target.childCount - 1; i >= 0; i--)
                    {
                        Transform childTransform = target.GetChild(i);
                        GameObject go = childTransform.gameObject;
                        if (go != null)
                        {
                            float terrainXPos = go.transform.position.x - entry.Key.terrain.transform.position.x;
                            float terrainYPos = go.transform.position.z - entry.Key.terrain.transform.position.z;

                            int localX = Mathf.RoundToInt(terrainXPos * rtDesc.width / terrainData.size.x);
                            int localY = Mathf.RoundToInt(terrainYPos * rtDesc.height / terrainData.size.z);

                            if (entry.Value.affectedLocalPixels.Contains(new Vector2Int(localX, localY)))
                            {
                                colorIndex = (localY - (entry.Value.affectedLocalPixels.y - entry.Value.affectedOperationPixels.y)) * targetGameObjectTexture.descriptor.width + localX - (entry.Value.affectedLocalPixels.x - entry.Value.affectedOperationPixels.x);
                                float strength = (colorIndex >= 0 && colorIndex < colors.Length - 1) ? colors[colorIndex].r : 0;
                                if (strength > removalStrength)
                                {
                                    GameObject.DestroyImmediate(go);
                                    instanceCounter--;
                                }
                            }
                        }
                    }
#endif
                }
            }

            RenderTexture.ReleaseTemporary(targetGameObjectTexture);
            targetGameObjectTexture = null;
        }

        #endregion

        #region COLLISIONS


        /// <summary>
        /// Collects the affected terrains for evaluating collision masks. 
        /// </summary>
        public void CollectTerrainCollisions()
        {
            RenderTexture previousRT = RenderTexture.active;
            int terrainCollisionResolution = (int)m_originTerrain.terrainData.size.x;

            m_collisionPixelSize = new Vector2(
            m_originTerrain.terrainData.size.x / (terrainCollisionResolution - 1.0f),
            m_originTerrain.terrainData.size.z / (terrainCollisionResolution - 1.0f));

            m_collisionBrushTransform = TerrainPaintUtility.CalculateBrushTransform(m_originTerrain, GaiaUtils.ConvertPositonToTerrainUV(m_originTerrain, new Vector2(m_originTransform.position.x, m_originTransform.position.z)), m_range, m_originTransform.rotation.eulerAngles.y);

            m_collisionPixels = GetPixelsForResolution(m_originTerrain.terrainData.size, m_collisionBrushTransform.GetBrushXYBounds(), terrainCollisionResolution, terrainCollisionResolution, 0);

            CreateDefaultRenderTexture(ref RTcollision, terrainCollisionResolution, terrainCollisionResolution, RenderTextureFormat.R16);
            
            AddAffectedTerrainPixels(m_collisionPixels, MultiTerrainOperationType.Collision, terrainCollisionResolution, terrainCollisionResolution);
        }


        /// <summary>
        /// Stores the combined collison mask info in RTCollision for the passed in collision mask array.
        /// </summary>
        /// <param name="collisionMasks">An array of collision masks for evaluation.</param>
        public void GetCollisionMask(CollisionMask[] collisionMasks)
        {
            GaiaSessionManager gaiaSessionManager = GaiaSessionManager.GetSessionManager(false);
            Material blitMaterial = TerrainPaintUtility.GetBlitMaterial();
            RenderTexture currentRT = RenderTexture.active;
            RenderTextureDescriptor rtDesc = RTcollision.descriptor;
            //rtDesc.width = targetTextureWidth;
            //rtDesc.height = targetTextureHeight;

            //RenderTexture output = new RenderTexture(rtDesc);

            RenderTexture.active = RTcollision;
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));

            GL.PushMatrix();
            GL.LoadPixelMatrix(0, m_collisionPixels.width, 0, m_collisionPixels.height);

            //prepare some non-render textures required for processing
            RenderTexture tempsourceTexture = RenderTexture.GetTemporary(rtDesc);
            //Texture2D tempsourceTexture = Texture2D.whiteTexture;
            //Texture2D finalTexture = new Texture2D(rtDesc.width, rtDesc.height);

            var relevantEntries = affectedTerrainPixels.Where(x => x.Key.operationType == MultiTerrainOperationType.Collision);

            foreach (var entry in relevantEntries)
            {
                if (!entry.Value.IsAffected())
                    continue;

                RenderTexture workaround1 = RenderTexture.GetTemporary(RTcollision.descriptor);
          
                RenderTexture completeCollisionMask = RenderTexture.GetTemporary(RTcollision.descriptor);
                RenderTexture.active = completeCollisionMask;
                //we start with a full white texture, the areas that would create a collision are painted in black
                GL.Clear(true, true, Color.white);


                //We need a buffer texture to continously feed the output of the tree collision baking back into the shader.
                //while still iterating through the trees.
                RenderTexture collisionMaskBuffer = RenderTexture.GetTemporary(completeCollisionMask.descriptor);
                //Clear the buffer for a clean start
                Graphics.Blit(completeCollisionMask, collisionMaskBuffer);



                Material combineMaterial = new Material(Shader.Find("Hidden/Gaia/CombineCollisionMasks"));
                combineMaterial.SetFloat("_Strength", 1f);

                foreach (CollisionMask collMask in collisionMasks.Where(x => x.m_active == true))
                {
                    tempsourceTexture = gaiaSessionManager.m_collisionMaskCache.LoadCollisionMask(entry.Key.terrain, collMask.m_type, collMask.m_Radius, collMask.m_treePrototypeId, collMask.m_tag);
                    //ImageProcessing.WriteRenderTexture("D:\\TempSource.png", tempsourceTexture);
                    if (tempsourceTexture != null)
                    {
                        // ImageProcessing.DebugWriteTexture2D("D:\\Tree1.png", (Texture2D)tempsourceTexture);
                        combineMaterial.SetInt("_Invert", collMask.m_invert ? 1 : 0);
                        combineMaterial.SetTexture("_InputTex", collisionMaskBuffer);
                        combineMaterial.SetTexture("_ImageMaskTex", tempsourceTexture);
                        combineMaterial.SetVector("_Dimensions", new Vector4(collisionMaskBuffer.width, collisionMaskBuffer.height, tempsourceTexture.width, tempsourceTexture.height));
                        Graphics.Blit(tempsourceTexture, completeCollisionMask, combineMaterial, 0);
                        //store result in buffer for the next iteration
                        Graphics.Blit(completeCollisionMask, collisionMaskBuffer);
                        
                    }
                }
                if (tempsourceTexture != null)
                {
                    //ImageProcessing.WriteRenderTexture("D:\\Tree1.png", completeCollisionMask);
                    //RenderTexture.active = completeCollisionMask;
                    //finalTexture.ReadPixels(new Rect(0, 0, completeCollisionMask.width, completeCollisionMask.height), 0, 0);
                    //finalTexture.Apply();
                    RenderTexture.active = RTcollision;
                    FilterMode oldFilterMode = FilterMode.Point;
                    oldFilterMode = completeCollisionMask.filterMode;
                    completeCollisionMask.filterMode = FilterMode.Point;
                    blitMaterial.SetTexture("_MainTex", completeCollisionMask);
                    blitMaterial.SetPass(0);
                    CopyIntoPixels(entry.Value.affectedOperationPixels, entry.Value.affectedLocalPixels, completeCollisionMask);
                    completeCollisionMask.filterMode = oldFilterMode;
                }
                RenderTexture.ReleaseTemporary(collisionMaskBuffer);
                collisionMaskBuffer = null;
                RenderTexture.ReleaseTemporary(completeCollisionMask);
                completeCollisionMask = null;
                RenderTexture.ReleaseTemporary(workaround1);
                workaround1 = null;

            }

            GL.PopMatrix();


            RenderTexture.active = currentRT;
            if (tempsourceTexture != Texture2D.whiteTexture && tempsourceTexture !=null)
            {
            //    tempsourceTexture.Release();
                tempsourceTexture = null;
            }
            //GameObject.DestroyImmediate(finalTexture, true);
            //finalTexture = null;

            //GC.Collect();
        }

        #endregion

        #endregion

        #region HELPERS

        /// <summary>
        /// Clean up when this class is not longer needed
        /// </summary>
        public void CloseOperation()
        {
            if (RTcollision != null)
            {
                RenderTexture.ReleaseTemporary(RTcollision);
                RTcollision = null;
            }
            if (RTdetailmap != null)
            {
                RenderTexture.ReleaseTemporary(RTdetailmap);
                RTdetailmap = null;
            }
            if (RTgameObject != null)
            {
                RenderTexture.ReleaseTemporary(RTgameObject);
                RTgameObject = null;
            }
            if (RTheightmap != null)
            {
                RenderTexture.ReleaseTemporary(RTheightmap);
                RTheightmap = null;
            }
            if (RTnormalmap != null)
            {
                RenderTexture.ReleaseTemporary(RTnormalmap);
                RTnormalmap = null;
            }
            if (RTterrainTree != null)
            {
                RenderTexture.ReleaseTemporary(RTterrainTree);
                RTterrainTree = null;
            }
            if (RTtextureSplatmap != null)
            {
                RenderTexture.ReleaseTemporary(RTtextureSplatmap);
                RTtextureSplatmap = null;
            }

            ////Sync heightmaps after the changes
            foreach (var terrain in affectedHeightmapData)
            {
                terrain.terrainData.SyncHeightmap();
                terrain.editorRenderFlags = TerrainRenderFlags.All;
            }

            ////Sync splatmaps after the changes
            foreach (var terrainData in affectedSplatmapData)
            {
                terrainData.SetBaseMapDirty();
                terrainData.SyncTexture(TerrainData.AlphamapTextureName);
            }

            QualitySettings.masterTextureLimit = m_originalMasterTextureLimit;
        }


        /// <summary>
        /// Helper function to copy a pixel rect onto the render texture.
        /// </summary>
        /// <param name="targetPixels">The target pixels on the render texture.</param>
        /// <param name="originalPixels">The original pixels to be copied.</param>
        /// <param name="originalTexture">The original texture to copy from.</param>
        public void CopyIntoPixels(RectInt targetPixels, RectInt originalPixels, Texture originalTexture)
        {
            if ((targetPixels.width + targetPixels.height > 1))
            {
                GL.Begin(GL.QUADS);
                GL.Color(new Color(1.0f, 1.0f, 1.0f, 1.0f));

                float width = originalPixels.width / (float)originalTexture.width;
                float height = originalPixels.height / (float)originalTexture.height;
                float x = originalPixels.x / (float)originalTexture.width;
                float y = originalPixels.y / (float)originalTexture.height;

                Rect originalUVs = new Rect(x, y, width, height);

                //Construct the vertices
                //Vertex 1
                GL.TexCoord2(originalUVs.x, originalUVs.y);
                GL.Vertex3(targetPixels.x, targetPixels.y, 0.0f);
                //Vertex2
                GL.TexCoord2(originalUVs.x, originalUVs.yMax);
                GL.Vertex3(targetPixels.x, targetPixels.yMax, 0.0f);
                //Vertex3
                GL.TexCoord2(originalUVs.xMax, originalUVs.yMax);
                GL.Vertex3(targetPixels.xMax, targetPixels.yMax, 0.0f);
                //Vertex4
                GL.TexCoord2(originalUVs.xMax, originalUVs.y);
                GL.Vertex3(targetPixels.xMax, targetPixels.y, 0.0f);
                GL.End();
            }
        }

        /// <summary>
        /// Helper function to set up shared material properties in shaders that perform heightmap changes
        /// </summary>
        /// <param name="material">The material in question</param>
        /// <param name="opType">The operation that is being performed.</param>
        public void SetupMaterialProperties(Material material, MultiTerrainOperationType opType)
        {
            RectInt pixelRect = new RectInt();
            Vector2 pixelSize = new Vector2();
            BrushTransform brushTransform = new BrushTransform();

            GetOperationInfo(ref pixelRect, ref pixelSize, ref brushTransform, opType);

            float pcSizeX = (pixelRect.width) * pixelSize.x;
            float pcSizeZ = (pixelRect.height) * pixelSize.y;
            Vector2 sizeU = pcSizeX * brushTransform.targetX;
            Vector2 sizeV = pcSizeZ * brushTransform.targetY;
            float operationOriginX = (pixelRect.xMin - 0.5f) * pixelSize.x;
            float operationOriginZ = (pixelRect.yMin - 0.5f) * pixelSize.y;

            Vector2 brushOffset = brushTransform.targetOrigin + operationOriginX * brushTransform.targetX + operationOriginZ * brushTransform.targetY;

            material.SetVector("_PCUVToBrushUVScales", new Vector4(sizeU.x, sizeU.y, sizeV.x, sizeV.y));
            material.SetVector("_PCUVToBrushUVOffset", new Vector4(brushOffset.x, brushOffset.y, 0.0f, 0.0f));
        }

        private void GetOperationInfo(ref RectInt pixelRect, ref Vector2 pixelSize, ref BrushTransform brushTransform, MultiTerrainOperationType opType)
        {
            switch (opType)
            {
                case MultiTerrainOperationType.Heightmap:
                    pixelRect = m_heightmapPixels;
                    pixelSize = m_heightmapPixelSize;
                    brushTransform = m_heightmapBrushTransform;
                    break;
            }
        }

        /// <summary>
        /// Helper function to add the terrains hit by the operation range to the dictionary of affected operation pixels.
        /// </summary>
        /// <param name="opRect">Rect representing the operation</param>
        /// <param name="opType">Operation Type</param>
        /// <param name="opWidth">Width of the operation</param>
        /// <param name="opHeight">Height of the operation</param>
        /// <param name="layer">Terrain layer (if textures / splatmap operation)</param>
        private void AddAffectedTerrainPixels(RectInt opRect, MultiTerrainOperationType opType, int opWidth, int opHeight, TerrainLayer layer = null)
        {
            //clear out any old data associated with this operation type
            foreach (Terrain t in Terrain.activeTerrains)
            {
                affectedTerrainPixels.Remove(new TerrainOperation() { terrain = t, operationType = opType });
            }
            //Transfer the 2d PixelRect in a 3D Bounds object to see where it intersects with terrain bounds
            //Bounds brushBounds = new Bounds(new Vector3(opRect.center.x, m_originTerrain.transform.position.y + m_originTerrain.terrainData.size.y / 2f, opRect.center.y), new Vector3(opRect.width, m_originTerrain.terrainData.size.y, opRect.height));
            //needs to be in world space
            //brushBounds.center = new Vector3(brushBounds.center.x + m_originTerrain.transform.position.x, m_originTerrain.transform.position.y, brushBounds.center.z + m_originTerrain.transform.position.z);

            //Use max range for y to make sure we always catch the terrains regardless of the y-distance to the terrains
            Bounds brushBounds = new Bounds(m_originTransform.position, new Vector3(m_range, float.MaxValue, m_range));


            //remove 1 from width and height - if the brushBounds ends exactly at the border to another terrain, and the border of that terrain touches brush bounds on the edge
            //it would be included in the operation. Not good for world spawning as this drags in a lot of unneccesary terrains into the operation!

            brushBounds.size = new Vector3(brushBounds.size.x - 1, brushBounds.size.y - 1, brushBounds.size.z - 1);

            // add center tile
            if (layer == null)
            {
                affectedTerrainPixels.Add(new TerrainOperation() { terrain = m_originTerrain, operationType = opType }, GetAffectedPixels(opRect, 0, 0, opWidth, opHeight, 0, 0));
            }
            else
            {
                int tileLayerIndex = TerrainPaintUtility.FindTerrainLayerIndex(m_originTerrain, layer);
                if (tileLayerIndex == -1)
                {
                    Debug.LogWarning("Could not find layer index on terrain for layer " + layer.name);
                }
                else
                {
                    affectedTerrainPixels.Add(new TerrainOperation() { terrain = m_originTerrain, operationType = opType }, GetAffectedPixels(opRect, 0, 0, opWidth, opHeight, tileLayerIndex / 4, tileLayerIndex % 4));
                }
            }
            //Debug.Log("Brush Bounds Center: " + brushBounds.center.ToString() + " Extents: " + brushBounds.extents.ToString()); 

            //Go through all active terrains, process the ones that intersect with the brush & are affected by the change
            foreach (Terrain t in Terrain.activeTerrains)
            {
                if (t != m_originTerrain)
                {
                    //Check needs to performed in world space, terrain bounds are in local space of the terrain
                    Bounds worldSpaceBounds = t.terrainData.bounds;
                    worldSpaceBounds.center = new Vector3(worldSpaceBounds.center.x + t.transform.position.x, worldSpaceBounds.center.y + t.transform.position.y, worldSpaceBounds.center.z + t.transform.position.z);
                    if (brushBounds.Intersects(worldSpaceBounds))
                    {
                        int horizDelta = GetHorizontalDelta(m_originTerrain, t);
                        int vertDelta = GetVerticalDelta(m_originTerrain, t);
                        if (layer == null)
                        {
                            affectedTerrainPixels.Add(new TerrainOperation() { terrain = t, operationType = opType }, GetAffectedPixels(opRect, horizDelta * (opWidth - 1), vertDelta * (opHeight - 1), opWidth, opHeight, 0, 0));
                        }
                        else
                        {
                            int tileLayerIndex = TerrainPaintUtility.FindTerrainLayerIndex(t, layer);
                            if (tileLayerIndex == -1)
                            {
                                Debug.LogWarning("Could not find layer index on terrain for layer " + layer.name);
                                continue;
                            }
                            affectedTerrainPixels.Add(new TerrainOperation() { terrain = t, operationType = opType }, GetAffectedPixels(opRect, horizDelta * (opWidth - 1), vertDelta * (opHeight - 1), opWidth, opHeight, tileLayerIndex / 4, tileLayerIndex % 4));
                        }
                    }
                    //else
                    //{
                    //    Debug.Log("Terrain Bounds Center: " + worldSpaceBounds.center.ToString() + " Extents: " + worldSpaceBounds.extents.ToString()); 
                    //}
                }
            }

        }

        /// <summary>
        /// Gets the horizontal difference in "terrain pieces" between a terrain and the origin terrain of the operation
        /// </summary>
        /// <param name="originTerrain">The origin terrain for this operation.</param>
        /// <param name="t">The terrain to determine the difference for.</param>
        /// <returns></returns>
        private int GetHorizontalDelta(Terrain originTerrain, Terrain t)
        {
            return Mathf.RoundToInt((t.transform.position.x - originTerrain.transform.position.x) / originTerrain.terrainData.size.x);
        }


        /// <summary>
        /// Gets the vertical difference in "terrain pieces" between a terrain and the origin terrain of the operation
        /// </summary>
        /// <param name="originTerrain">The origin terrain for this operation.</param>
        /// <param name="t">The terrain to determine the difference for.</param>
        /// <returns></returns>
        private int GetVerticalDelta(Terrain originTerrain, Terrain t)
        {
            return Mathf.RoundToInt((t.transform.position.z - originTerrain.transform.position.z) / originTerrain.terrainData.size.z);
        }
        /// <summary>
        /// Calculates a Rect in the correct texture (heightmap, splatmap, etc.) resolution when the "localBounds" parameter would be laid over a terrain..
        /// </summary>
        /// <param name="terrainSize">Size of the terrain</param>
        /// <param name="localBounds">The local bounds to evaluate</param>
        /// <param name="inputTextureWidth">Width of the texture in question.</param>
        /// <param name="inputTextureHeight">HEight of the texture in question.</param>
        /// <param name="additionalSeam">Additional Pixels to add to the rect</param>
        /// <returns></returns>
        private RectInt GetPixelsForResolution(Vector3 terrainSize, Rect localBounds, int inputTextureWidth, int inputTextureHeight, int additionalSeam)
        {
            float Xsize = (inputTextureWidth - 1.0f) / terrainSize.x;
            float Ysize = (inputTextureHeight - 1.0f) / terrainSize.z;
            int xMin = Mathf.FloorToInt(localBounds.xMin * Xsize) - additionalSeam;
            int yMin = Mathf.FloorToInt(localBounds.yMin * Ysize) - additionalSeam;
            int xMax = Mathf.CeilToInt(localBounds.xMax * Xsize) + additionalSeam;
            int yMax = Mathf.CeilToInt(localBounds.yMax * Ysize) + additionalSeam;
            int width = xMax - xMin + 1;
            int height = yMax - yMin + 1;
            return new RectInt(xMin, yMin, width, height);
        }
        /// <summary>
        /// Gets the default preview material for the Gaia Stamper
        /// </summary>
        /// <returns></returns>
        public static Material GetDefaultGaiaStamperPreviewMaterial()
        {
            if (m_GaiaStamperPreviewMaterial == null)
                m_GaiaStamperPreviewMaterial = new Material(Shader.Find("Hidden/Gaia/StampPreview"));
            return m_GaiaStamperPreviewMaterial;
        }
        /// <summary>
        /// Gets the default preview material for the Gaia Spawner
        /// </summary>
        /// <returns></returns>
        public static Material GetDefaultGaiaSpawnerPreviewMaterial()
        {
            if (m_GaiaSpawnerPreviewMaterial == null)
                m_GaiaSpawnerPreviewMaterial = new Material(Shader.Find("Hidden/Gaia/SpawnerPreview"));
            return m_GaiaSpawnerPreviewMaterial;
        }

        /// <summary>
        /// Visualises an operation in the scene view
        /// </summary>
        /// <param name="opType">Operation Type</param>
        /// <param name="previewTexture">The contents to visualise.</param>
        /// <param name="mat">The material used for visualisaton.</param>
        /// <param name="pass">The pass used in the material.</param>
        public void Visualize(MultiTerrainOperationType opType, RenderTexture previewTexture, Material mat, int pass)
        {
            Texture meshTexture = previewTexture;
            meshTexture.filterMode = FilterMode.Point;
            FilterMode currentFilterMode = meshTexture.filterMode;
            Vector3 terrainPos = m_originTerrain.GetPosition();

            RectInt pixels = new RectInt();
            Vector2 pixelSize = new Vector2();
            BrushTransform brushTransform = new BrushTransform();

            //Pull correct resolution sizes according to operation type
            GetOperationInfo(ref pixels, ref pixelSize, ref brushTransform, opType);

            //Prepare all relevant material properties
            float heightmapPixelsWidth = 1.0f / meshTexture.width;
            float heigthmapPixelsHeight = 1.0f / meshTexture.height;
            int qPixelX = pixels.width - 1;
            int qPixelY = pixels.height - 1;
            int numVerts = qPixelX * qPixelY * (2 * 3);
            float sizeX = pixelSize.x;
            float sizeY = 2.0f * m_originTerrain.terrainData.heightmapScale.y;
            float sizeZ = pixelSize.y;
            float operationOriginX = pixels.xMin * pixelSize.x;
            float operationOriginZ = pixels.yMin * pixelSize.y;
            float operationSizeX = pixelSize.x;
            float operationSizeZ = pixelSize.y;

            Vector2 sizeU = operationSizeX * brushTransform.targetX;
            Vector2 sizeV = operationSizeZ * brushTransform.targetY;
            Vector2 brushUVoffset = brushTransform.targetOrigin + operationOriginX * brushTransform.targetX + operationOriginZ * brushTransform.targetY;

            //Set material properties
            mat.SetVector("_QuadRez", new Vector4(qPixelX, qPixelY, numVerts, 0.0f));
            mat.SetVector("_HeightmapUV_PCPixelsX", new Vector4(heightmapPixelsWidth, 0.0f, 0.0f, 0.0f));
            mat.SetVector("_HeightmapUV_PCPixelsY", new Vector4(0.0f, heigthmapPixelsHeight, 0.0f, 0.0f));
            mat.SetVector("_HeightmapUV_Offset", new Vector4(0.5f * heightmapPixelsWidth, 0.5f * heigthmapPixelsHeight, 0.0f, 0.0f));
            mat.SetTexture("_Heightmap", meshTexture);
            mat.SetVector("_ObjectPos_PCPixelsX", new Vector4(sizeX, 0.0f, 0.0f, 0.0f));
            mat.SetVector("_ObjectPos_HeightMapSample", new Vector4(0.0f, sizeY, 0.0f, 0.0f));
            mat.SetVector("_ObjectPos_PCPixelsY", new Vector4(0.0f, 0.0f, sizeZ, 0.0f));
            mat.SetVector("_ObjectPos_Offset", new Vector4(pixels.xMin * sizeX, 0.0f, pixels.yMin * sizeZ, 1.0f));
            mat.SetVector("_BrushUV_PCPixelsX", new Vector4(sizeU.x, sizeU.y, 0.0f, 0.0f));
            mat.SetVector("_BrushUV_PCPixelsY", new Vector4(sizeV.x, sizeV.y, 0.0f, 0.0f));
            mat.SetVector("_BrushUV_Offset", new Vector4(brushUVoffset.x, brushUVoffset.y, 0.0f, 1.0f));
            mat.SetTexture("_BrushTex", Texture2D.whiteTexture);
            mat.SetVector("_TerrainObjectToWorldOffset", terrainPos);
            mat.SetPass(pass);
            Graphics.DrawProceduralNow(MeshTopology.Triangles, numVerts);

            meshTexture.filterMode = currentFilterMode;
        }



        private AffectedPixels GetAffectedPixels(RectInt opRect, int opCoordinateX, int opCoordinateY, int opWidth, int opHeight, int splatmapID, int channelID)
        {
            AffectedPixels returnPixels = new AffectedPixels();
            returnPixels.pixelCoordinate = new Vector2Int(opCoordinateX, opCoordinateY);

            returnPixels.affectedLocalPixels = new RectInt()
            {
                x = Mathf.Max(0, opRect.x - opCoordinateX),
                y = Mathf.Max(0, opRect.y - opCoordinateY),
                xMax = Mathf.Min(opWidth, opRect.xMax - opCoordinateX),
                yMax = Mathf.Min(opHeight, opRect.yMax - opCoordinateY)
            };
            returnPixels.affectedOperationPixels = new RectInt(
            returnPixels.affectedLocalPixels.x + returnPixels.pixelCoordinate.x - opRect.x,
            returnPixels.affectedLocalPixels.y + returnPixels.pixelCoordinate.y - opRect.y,
            returnPixels.affectedLocalPixels.width,
            returnPixels.affectedLocalPixels.height
            );

            returnPixels.channelID = channelID;
            returnPixels.splatMapID = splatmapID;

            return returnPixels;
        }

        /// <summary>
        /// Fetches an 1-dimensional Color array from the current destination render texture from this paint context.
        /// </summary>
        /// <returns></returns>
        private Color[] GetRTColorArray(RenderTexture targetTexture)
        {
            RenderTextureDescriptor rtDesc = targetTexture.descriptor;
            Texture2D copyTexture = new Texture2D(rtDesc.width, rtDesc.height, TextureFormat.RGBAFloat, false);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = targetTexture;
            copyTexture.ReadPixels(new Rect(0f, 0f, rtDesc.width, rtDesc.height), 0, 0);
            copyTexture.Apply();
            var colors = copyTexture.GetPixels(0, 0, copyTexture.width, copyTexture.height);
            RenderTexture.active = currentRT;
            return colors;
        }

        #endregion

    }

}