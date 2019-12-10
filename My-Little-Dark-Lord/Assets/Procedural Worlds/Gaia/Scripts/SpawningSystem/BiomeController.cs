using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
using static Gaia.GaiaConstants;
#endif
namespace Gaia
{



    /// <summary>
    /// A generic spawning system.
    /// </summary>
    [System.Serializable]
    public class BiomeController : MonoBehaviour
    {

        public float m_range = 1024;
        public ImageMask[] m_biomeMasks = new ImageMask[0];
        public List<AutoSpawner> m_autoSpawners = new List<AutoSpawner>();
#if UNITY_POST_PROCESSING_STACK_V2
        public PostProcessProfile m_postProcessProfile;
        public BiomePostProcessingVolumeSpawnMode m_ppVSpawnMode = BiomePostProcessingVolumeSpawnMode.Add;

#endif
        public float postProcessBlenddDistance;
        public bool m_drawPreview;
        public Color m_visualisationColor = GaiaConstants.spawnerInitColor;
        public bool m_biomePreviewDirty;
        private Terrain m_lastActiveTerrain;
        private float m_minWorldHeight;
        private float m_maxWorldHeight;
        private RenderTexture m_cachedPreviewRT;
        private GaiaSettings m_gaiaSettings;
        private bool m_showSeaLevelinPreview = true;
        public bool m_removeForeignGameObjects;
        public float m_removeForeignGameObjectStrength = 0.2f;
        public bool m_removeForeignTrees;
        public float m_removeForeignTreesStrength = 0.2f;
        public bool m_removeForeignTerrainDetails;
        public int m_removeForeignTerrainDetailsDensity = 15;



        //void OnEnable()
        //{
        //    if (m_gaiaSettings == null)
        //    {
        //        m_gaiaSettings = GaiaUtils.GetGaiaSettings();
        //    }
        //}


        public Terrain GetCurrentTerrain()
        {
            Terrain currentTerrain = Gaia.TerrainHelper.GetTerrain(transform.position);
            //Check if the stamper is over a terrain currently
            //if not, we will draw a preview based on the last active terrain we were over
            //if that is null either we can't draw a stamp preview
            if (currentTerrain)
            {
                //Update last active terrain with current
                if (m_lastActiveTerrain != currentTerrain)
                {
                    //if the current terrain is a new terrain, we should refresh the min max values in case this terrain has never been calculated before
                    GaiaSessionManager.GetSessionManager().GetWorldMinMax(ref m_minWorldHeight, ref m_maxWorldHeight);
                }
                m_lastActiveTerrain = currentTerrain;
            }
            else
            {
                if (m_lastActiveTerrain)
                    currentTerrain = m_lastActiveTerrain;
                else
                    return null;
            }
            return currentTerrain;
        }

        public void DrawBiomePreview()
        {
            if (m_drawPreview)
            {

                //Set up a multi-terrain operation once, all rules can then draw from the data collected here
                Terrain currentTerrain = GetCurrentTerrain();
                GaiaMultiTerrainOperation operation = new GaiaMultiTerrainOperation(currentTerrain, transform, m_range * 2f);
                operation.GetHeightmap();


                //only re-generate all textures etc. if settings have changed and the preview is dirty, otherwise we can just use the cached textures
                if (m_biomePreviewDirty == true)
                {
                    //Get additional op data (required for certain image masks)
                    operation.GetNormalmap();
                    operation.CollectTerrainCollisions();

                    //Clear texture cache first
                    if (m_cachedPreviewRT != null)
                    {
                        m_cachedPreviewRT.Release();
                        DestroyImmediate(m_cachedPreviewRT);
                    }

                    m_cachedPreviewRT = new RenderTexture(operation.RTheightmap);
                    RenderTexture currentRT = RenderTexture.active;
                    RenderTexture.active = m_cachedPreviewRT;
                    GL.Clear(true, true, Color.black);
                    RenderTexture.active = currentRT;

                    Graphics.Blit(ApplyBrush(operation), m_cachedPreviewRT);
                    RenderTexture.active = currentRT;
                    //ImageProcessing.WriteRenderTexture("D:\\previewRT.png", m_cachedPreviewRT);
                    //Everything processed, preview not dirty anymore
                    m_biomePreviewDirty = false;
                }

                //Now draw the preview according to the cached textures
                Material material = GaiaMultiTerrainOperation.GetDefaultGaiaSpawnerPreviewMaterial();
                material.SetInt("_zTestMode", (int)UnityEngine.Rendering.CompareFunction.Always);

                //assign the first color texture in the material
                material.SetTexture("_colorTexture0", m_cachedPreviewRT);

                //remove all other potential color textures, there can be caching issues if other visualisers were used in the meantime

                for (int colorIndex = 1; colorIndex < GaiaConstants.maxPreviewedTextures; colorIndex++)
                {
                    material.SetTexture("_colorTexture" + colorIndex, null);
                }



                //set the color
                material.SetColor("_previewColor0", m_visualisationColor);
                if (m_gaiaSettings == null)
                {
                    m_gaiaSettings = GaiaUtils.GetGaiaSettings();
                }

                Color seaLevelColor = m_gaiaSettings.m_stamperSeaLevelTintColor;
                if (!m_showSeaLevelinPreview)
                {
                    seaLevelColor.a = 0f;
                }
                material.SetColor("_seaLevelTintColor", seaLevelColor);
                material.SetFloat("_seaLevel", GaiaSessionManager.GetSessionManager(false).m_session.m_seaLevel);
                operation.Visualize(MultiTerrainOperationType.Heightmap, operation.RTheightmap, material, 1);

                //Clean up
                operation.CloseOperation();
                //Clean up temp textures
                GaiaUtils.ReleaseAllTempRenderTextures();
            }
        }



        private RenderTexture ApplyBrush(GaiaMultiTerrainOperation operation)
        {
            Terrain currentTerrain = GetCurrentTerrain();

            RenderTextureDescriptor rtDescriptor = operation.RTheightmap.descriptor;
            //Random write needs to be enabled for certain mask types to function!
            rtDescriptor.enableRandomWrite = true;

            RenderTexture inputTexture = RenderTexture.GetTemporary(rtDescriptor);

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = inputTexture;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = currentRT;

            //Iterate through all image masks and set up the current paint context in case the shader uses heightmap data
            foreach (ImageMask mask in m_biomeMasks)
            {
                mask.m_multiTerrainOperation = operation;
                mask.m_seaLevel = GaiaSessionManager.GetSessionManager(false).GetSeaLevel();
                mask.m_maxWorldHeight = m_maxWorldHeight;
                mask.m_minWorldHeight = m_minWorldHeight;
            }


            //Get the combined masks for the biome 
            RenderTexture biomeOutputTexture = RenderTexture.GetTemporary(rtDescriptor);
            Graphics.Blit(ImageProcessing.ApplyMaskStack(inputTexture, m_biomeMasks, ImageMaskInfluence.Local), biomeOutputTexture);
            //ImageProcessing.WriteRenderTexture("D:\\previewRT.png", m_cachedPreviewRT);


            //if (opType == MultiTerrainOperationType.Tree)
            //{
            //    ImageProcessing.WriteRenderTexture("D:\\spawnerOutputTexture.png", spawnerOutputTexture);
            //    ImageProcessing.WriteRenderTexture("D:\\ruleOutputTexture.png", ruleOutputTexture);
            //}




            //clean up temporary textures
            ReleaseRenderTexture(inputTexture);
            inputTexture = null;
            return biomeOutputTexture;
        }


        private void ReleaseRenderTexture(RenderTexture texture)
        {
            if (texture != null)
            {
                RenderTexture.ReleaseTemporary(texture);
                texture = null;
            }
        }

        public void RemoveForeignTrees()
        {
            GaiaUtils.DisplayProgressBarNoEditor("Removing Foreign Trees", "Preparing...", 0);
            List<GameObject> knownTreePrefabs = new List<GameObject>();
            List<TreePrototype> treeProtosToRemove = new List<TreePrototype>();
            Terrain currentTerrain = GetCurrentTerrain();
            foreach (AutoSpawner autoSpawner in m_autoSpawners)
            {
                foreach (SpawnRule sr in autoSpawner.spawner.m_settings.m_spawnerRules)
                {
                    if (sr.m_resourceType == GaiaConstants.SpawnerResourceType.TerrainTree)
                    {
                        knownTreePrefabs.Add(autoSpawner.spawner.m_settings.m_resources.m_treePrototypes[sr.m_resourceIdx].m_desktopPrefab);
                    }
                }
            }

            GaiaMultiTerrainOperation operation = new GaiaMultiTerrainOperation(currentTerrain, transform, m_range * 2f);
            operation.GetHeightmap();
            operation.GetNormalmap();
            operation.CollectTerrainDetails();
            operation.CollectTerrainTrees();
            operation.CollectTerrainGameObjects();
            operation.CollectTerrainCollisions();

            int protoIndex = 0;
            foreach (TreePrototype t in currentTerrain.terrainData.treePrototypes)
            {
                GaiaUtils.DisplayProgressBarNoEditor("Removing Foreign Trees", "Removing Trees...", (float)(protoIndex + 1) / (float)(currentTerrain.terrainData.treePrototypes.Count()));
                if (!knownTreePrefabs.Contains(t.prefab))
                {
                    int counter = 0;
                    operation.SetTerrainTrees(ApplyBrush(operation), protoIndex, null, null, GaiaConstants.SpawnMode.Remove, ref counter, m_removeForeignTreesStrength);
                }
                protoIndex++;
            }
            GaiaUtils.ClearProgressBarNoEditor();
            operation.CloseOperation();
        }

        public void RemoveForeignGameObjects()
        {
            GaiaUtils.DisplayProgressBarNoEditor("Removing Foreign GameObjects", "Preparing...", 0);
            List<ResourceProtoGameObjectInstance> knownProtoInstances = new List<ResourceProtoGameObjectInstance>();
            List<ResourceProtoGameObjectInstance> GoProtoInstancesToRemove = new List<ResourceProtoGameObjectInstance>();
            Terrain currentTerrain = GetCurrentTerrain();
            foreach (AutoSpawner autoSpawner in m_autoSpawners)
            {
                foreach (SpawnRule sr in autoSpawner.spawner.m_settings.m_spawnerRules)
                {
                    if (sr.m_resourceType == GaiaConstants.SpawnerResourceType.GameObject)
                    {
                        foreach (ResourceProtoGameObjectInstance instance in autoSpawner.spawner.m_settings.m_resources.m_gameObjectPrototypes[sr.m_resourceIdx].m_instances)
                        {
                            knownProtoInstances.Add(instance);
                        }
                    }
                }
            }

            GaiaMultiTerrainOperation operation = new GaiaMultiTerrainOperation(currentTerrain, transform, m_range * 2f);
            operation.GetHeightmap();
            operation.GetNormalmap();
            operation.CollectTerrainDetails();
            operation.CollectTerrainTrees();
            operation.CollectTerrainGameObjects();
            operation.CollectTerrainCollisions();

            int protoIndex = 0;
            var allSpawners = Resources.FindObjectsOfTypeAll<Spawner>();
            foreach (Spawner spawner in allSpawners)
            {
                GaiaUtils.DisplayProgressBarNoEditor("Removing Foreign GameObjects", "Removing Game Objects...", (float)(protoIndex + 1) / (float)(allSpawners.Length));
                foreach (SpawnRule sr in spawner.m_settings.m_spawnerRules)
                {
                    if (sr.m_resourceType == GaiaConstants.SpawnerResourceType.GameObject)
                    {
                        ResourceProtoGameObject protoGO = spawner.m_settings.m_resources.m_gameObjectPrototypes[sr.m_resourceIdx];
                        foreach (ResourceProtoGameObjectInstance instance in protoGO.m_instances)
                        {
                            if (!knownProtoInstances.Contains(instance))
                            {

                                Transform target = spawner.GetGOSpawnTarget(sr, protoGO);
                                operation.SetTerrainGameObjects(ApplyBrush(operation), protoGO, sr, target, GaiaConstants.SpawnMode.Remove, ref sr.m_spawnedInstances, m_removeForeignGameObjectStrength);
                                //no need to look at other instances if this one triggered the removal already
                                break;
                            }
                        }
                    }
                }
                protoIndex++;
            }
            operation.CloseOperation();

#if UNITY_EDITOR
            //need to dirty the scene when we remove game objects
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif

        }

        public void RemoveForeignTerrainDetails()
        {
            GaiaUtils.DisplayProgressBarNoEditor("Removing Foreign Terrain Details", "Preparing...", 0);
            List<ResourceProtoDetail> knownTerrainDetails = new List<ResourceProtoDetail>();
            List<DetailPrototype> detailProtosToRemove = new List<DetailPrototype>();
            Terrain currentTerrain = GetCurrentTerrain();
            foreach (AutoSpawner autoSpawner in m_autoSpawners)
            {
                foreach (SpawnRule sr in autoSpawner.spawner.m_settings.m_spawnerRules)
                {
                    if (sr.m_resourceType == GaiaConstants.SpawnerResourceType.TerrainDetail)
                    {
                        knownTerrainDetails.Add(autoSpawner.spawner.m_settings.m_resources.m_detailPrototypes[sr.m_resourceIdx]);
                    }
                }
            }

            GaiaMultiTerrainOperation operation = new GaiaMultiTerrainOperation(currentTerrain, transform, m_range * 2f);
            operation.GetHeightmap();
            operation.GetNormalmap();
            operation.CollectTerrainDetails();
            operation.CollectTerrainTrees();
            operation.CollectTerrainGameObjects();
            operation.CollectTerrainCollisions();

            int protoIndex = 0;
            foreach (DetailPrototype t in currentTerrain.terrainData.detailPrototypes)
            {
                GaiaUtils.DisplayProgressBarNoEditor("Removing Foreign GameObjects", "Removing Game Objects...", (float)(protoIndex + 1) / (float)(currentTerrain.terrainData.detailPrototypes.Count()));
                if ((knownTerrainDetails.Find(x => x.m_detailProtoype == t.prototype) == null || t.prototype == null) && (knownTerrainDetails.Find(x => x.m_detailTexture == t.prototypeTexture) == null || t.prototypeTexture == null))
                {
                    int counter = 0;
                    operation.SetTerrainDetails(ApplyBrush(operation), protoIndex, GaiaConstants.SpawnMode.Remove, m_removeForeignTerrainDetailsDensity, ref counter);
                }
                protoIndex++;
            }
            operation.CloseOperation();
        }
    }


}
