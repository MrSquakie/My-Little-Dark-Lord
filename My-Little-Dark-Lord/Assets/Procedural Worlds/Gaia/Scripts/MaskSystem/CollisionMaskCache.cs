using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gaia
{
    [System.Serializable]
    public class CollisionMaskCacheEntry
    {
        public string fileName;
        public string assetPath;
        public string fullPath;
        public RenderTexture texture;
    }

    [System.Serializable]
    public class CollisionMaskCache : UnityEngine.Object
    {
        public bool m_autoSpawnRunning = false;
        bool m_cacheClearedForAutoSpawn = false;
        public GaiaSession m_gaiaSession;
        public CollisionMaskCacheEntry[] m_cacheEntries {
            get
            {
                if (m_gaiaSession == null)
                {
                    m_gaiaSession = GaiaSessionManager.GetSessionManager(false).m_session;
                }
                return m_gaiaSession.m_collisionMaskCacheEntries;
            }
            set
            {
                if (m_gaiaSession == null)
                {
                    m_gaiaSession = GaiaSessionManager.GetSessionManager(false).m_session;
                }
                m_gaiaSession.m_collisionMaskCacheEntries = value;
            }
        }
       

        public RenderTexture BakeTerrainTagCollisions(Terrain terrain, string tag, float radius)
        {
            GameObject[] allGOsWithTag = new GameObject[0];

            bool tagFound = false;
            try
            {
                allGOsWithTag = GameObject.FindGameObjectsWithTag(tag);
                tagFound = true;
            }
            catch
            {
                tagFound = false;
            }

            if (!tagFound)
            {
                Debug.LogWarning("Could not find Game Objects for tag: " + tag + " when trying to bake a collision mask. Does this tag exist?" );
                return null;
            }


            //Array to store all positions and radi of the objects
            Vector4[] posAndRad = new Vector4[allGOsWithTag.Length];
            for (int i = 0; i < allGOsWithTag.Length; i++)
            {
                //need scalar position on the terrain from 0..1
                //get relative positon on the terrain first
                float scaleFactor = (allGOsWithTag[i].transform.localScale.x + allGOsWithTag[i].transform.localScale.z) / 2f;
                Vector4 v4 = new Vector4(allGOsWithTag[i].transform.position.x, allGOsWithTag[i].transform.position.y, allGOsWithTag[i].transform.position.z, WorldDistance2UVDistance(terrain, radius * scaleFactor));
                v4.x -= terrain.transform.position.x;
                v4.y -= terrain.transform.position.y;
                v4.z -= terrain.transform.position.z;
                //now make that relative position scalar (0..1)
                v4.x /= terrain.terrainData.size.x;
                v4.y /= terrain.terrainData.size.y;
                v4.z /= terrain.terrainData.size.z;
                //flip on z-axis
                v4.z = 1 - v4.z;

                posAndRad[i] = v4;
            }

            string fileName = GetTagCollisionMaskFileName(terrain, tag, radius);

            return BakeVectorArrayForTerrain(posAndRad, terrain, fileName);
        }

        public void WriteCacheToDisk()
        {
#if UNITY_EDITOR

            int length = m_cacheEntries.Length;
            for (int i = 0; i < length; i++)
            {
                ImageProcessing.WriteRenderTexture(m_cacheEntries[i].fullPath, m_cacheEntries[i].texture);

                //var importer = AssetImporter.GetAtPath(collisionMaskCache[i].assetPath) as TextureImporter;
                ////Set texture up as readable, otherwise it can create issues when overwriting later
                //if (importer != null)
                //{
                //    importer.textureType = TextureImporterType.Default;
                //    importer.isReadable = true;
                //}

                //Refresh mask immediately by reimporting it in the same step
                AssetDatabase.ImportAsset(m_cacheEntries[i].assetPath, ImportAssetOptions.ForceUpdate);

                m_cacheEntries[i].texture = null;

            }

            //clear the cache for a clean start, this will free the cache from outdated entries, etc.
            //the cache will be rebuilt quickly only with the relevant files which are saved to disk now.
            m_cacheEntries = new CollisionMaskCacheEntry[0];
#endif
        }

        public RenderTexture BakeTerrainTreeCollisions(Terrain terrain, int treePrototypeId, float boundsRadius)
        {
            //Iterate through the trees and build a v4 array with the positions

            TreeInstance[] allRelevantTrees = terrain.terrainData.treeInstances.Where(x => x.prototypeIndex == treePrototypeId).ToArray();
            //Array to store all positions and radi of the tree
            Vector4[] posAndRad = new Vector4[allRelevantTrees.Length];

            for (int i = 0; i < allRelevantTrees.Length; i++)
            {
                posAndRad[i] = new Vector4(allRelevantTrees[i].position.x, allRelevantTrees[i].position.y, allRelevantTrees[i].position.z, WorldDistance2UVDistance(terrain,boundsRadius * allRelevantTrees[i].widthScale));
            }

            string fileName = GetTreeCollisionMaskFileName(terrain, treePrototypeId, boundsRadius);

            return BakeVectorArrayForTerrain(posAndRad, terrain, fileName);

        }

        /// <summary>
        /// Converts a regular world space unity unit distance to a distance in scalar (0-1) UV-space on a terrain.
        /// </summary>
        /// <param name="Terrain">The terrain on which the conversion takes place</param>
        /// <param name="distance">The distance to convert.</param>
        /// <returns></returns>
        private float WorldDistance2UVDistance(Terrain terrain, float distance)
        {
            float longerSideLength = Mathf.Max(terrain.terrainData.size.x, terrain.terrainData.size.z);
            return Mathf.InverseLerp(0, longerSideLength, distance);
        }

        private RenderTexture BakeVectorArrayForTerrain(Vector4[] posAndRad, Terrain terrain, string filename)
        {
            //setting up with default settings from the paint context source render texture
            RenderTextureDescriptor rtDesc = new RenderTextureDescriptor();
            rtDesc.autoGenerateMips = true;
            rtDesc.bindMS = false;
            rtDesc.colorFormat = RenderTextureFormat.R16;
            rtDesc.depthBufferBits = 0;
            rtDesc.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            rtDesc.enableRandomWrite = true;
            rtDesc.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R16_UNorm;
            rtDesc.height = Mathf.RoundToInt(terrain.terrainData.size.z);
            rtDesc.memoryless = RenderTextureMemoryless.None;
            rtDesc.msaaSamples = 1;
            rtDesc.sRGB = false;
            rtDesc.shadowSamplingMode = UnityEngine.Rendering.ShadowSamplingMode.None;
            rtDesc.useDynamicScale = false;
            rtDesc.useMipMap = false;
            rtDesc.volumeDepth = 1;
            rtDesc.vrUsage = VRTextureUsage.None;
            rtDesc.width = Mathf.RoundToInt(terrain.terrainData.size.x);

            RenderTexture collisionMask = new RenderTexture(rtDesc);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = collisionMask;
            //we start with a full white texture, the areas that would create a collision are painted in black
            GL.Clear(true, true, Color.white);
            RenderTexture.active = currentRT;

            //We need a buffer texture to continously feed the output of the tree collision baking back into the shader.
            //while still iterating through the trees.
            RenderTexture collisionMaskBuffer = RenderTexture.GetTemporary(collisionMask.descriptor);
            //Clear the buffer for a clean start
            Graphics.Blit(collisionMask, collisionMaskBuffer);
            //Prepare a material that will bake the trees into a mask as we spawn them
            Material treeBakeMat = new Material(Shader.Find("Hidden/Gaia/TreeCollisionMaskBaking"));
            treeBakeMat.SetFloat("_Strength", 1f);
            AnimationCurve tempCurve = new AnimationCurve(new Keyframe[] { new Keyframe { time = 0, value = 1 }, new Keyframe { time = 1, value = 0 } });
            Texture2D tempTexture = new Texture2D(256, 1);
            ImageProcessing.CreateMaskCurveTexture(ref tempTexture);
            ImageProcessing.BakeCurveTexture(tempCurve, tempTexture);
            treeBakeMat.SetTexture("_DistanceMaskTex", tempTexture);


            treeBakeMat.SetInt("_Invert", 0);

            //Iterate through the trees and bake
            foreach (Vector4 v4 in posAndRad)
            {
                treeBakeMat.SetFloat("_BoundsRadius", v4.w);
                treeBakeMat.SetVector("_TargetPosition", new Vector4(v4.x, v4.z, 0, 0));
                treeBakeMat.SetTexture("_InputTex", collisionMaskBuffer);
                Graphics.Blit(collisionMaskBuffer, collisionMask, treeBakeMat, 2);
                Graphics.Blit(collisionMask, collisionMaskBuffer);
            }

            //Store the final result in the appropiate file

            SaveCollisionMaskInCache(collisionMask, terrain, filename);
            //Clean Up
            DestroyImmediate(tempTexture);
            tempTexture = null;
            //RenderTexture.ReleaseTemporary(collisionMask);
            RenderTexture.ReleaseTemporary(collisionMaskBuffer);
            return collisionMask;
        }


        private string GetTreeCollisionMaskFileName(Terrain terrain, int treePrototypeId, float radius)
        {
            return terrain.name + "_" + CollisionMaskType.TerrainTree.ToString() + "_" + treePrototypeId.ToString() + "_" + radius.ToString()+ ".png";
        }

        private string GetTreeCollisionMaskSearchString(int treePrototypeIndex)
        {
            return CollisionMaskType.TerrainTree.ToString() + "_" + treePrototypeIndex.ToString();
        }


        private string GetTagCollisionMaskFileName(Terrain terrain, string m_tag, float radius)
        {
            return terrain.name + "_" + CollisionMaskType.Tag.ToString() + "_" + m_tag + "_" + radius.ToString() + ".png";
        }


        private string GetTagCollisionMaskSearchString(string tag)
        {
            return CollisionMaskType.Tag.ToString() + "_" + tag;
        }

        private void SaveCollisionMaskInCache(RenderTexture renderTexture, Terrain terrain, string fileName)
        {
            if (m_cacheEntries == null)
            {
                m_cacheEntries = new CollisionMaskCacheEntry[0];
            }

            //Convert the RT
            //Texture2D texture = GaiaUtils.ConvertRenderTextureToTexture2D(renderTexture);

            //texture.name = fileName;

            //Check if there is an entry already
            int length = m_cacheEntries.Length;
            bool found = false;
            for (int i = 0; i < length; i++)
            {
                if (m_cacheEntries[i].fileName == fileName)
                {
                    //found an entry, update with new render texture
                    found = true;
                    m_cacheEntries[i].texture.Release();
                    DestroyImmediate(m_cacheEntries[i].texture);
                    WriteCollisionCacheEntry(m_cacheEntries[i], renderTexture, terrain, fileName);
                    break;
                }
            }

            //Not found? Need to append cache array and write the new entry at the end
            if (!found)
            {
                AddNewCollisionMaskCacheEntry(renderTexture, terrain, fileName);
            }


        }

        private void AddNewCollisionMaskCacheEntry(RenderTexture texture, Terrain terrain, string fileName)
        {
            CollisionMaskCacheEntry[] newArray = new CollisionMaskCacheEntry[m_cacheEntries.Length + 1];
            int length2 = m_cacheEntries.Length;
            for (int i = 0; i < length2; i++)
            {
                newArray[i] = m_cacheEntries[i];
            }
            newArray[newArray.Length - 1] = new CollisionMaskCacheEntry();
            WriteCollisionCacheEntry(newArray[newArray.Length - 1], texture, terrain, fileName);
            m_cacheEntries = newArray;
            //EditorUtility.SetDirty(this);
#if UNITY_EDITOR
            EditorUtility.SetDirty(GaiaSessionManager.GetSessionManager(false));
#endif
        }

        private void WriteCollisionCacheEntry(CollisionMaskCacheEntry entry, RenderTexture texture, Terrain terrain, string fileName)
        {

            //build the paths
            string assetPath = GaiaDirectories.GetTerrainCollisionDirectory(terrain) + "/" + fileName;
            string fullPath = assetPath.Replace("Assets", Application.dataPath);

            //clear old texture
            if (entry.texture != null && entry.texture != Texture2D.whiteTexture)
            {
                UnityEngine.Object.DestroyImmediate(entry.texture);
                entry.texture = null;
            }
            //overwrite texture contents
            entry.texture = texture;

            //assign Paths & name
            entry.fileName = fileName;
            entry.assetPath = assetPath;
            entry.fullPath = fullPath;
            //EditorUtility.SetDirty(entry);
            //ImageProcessing.WriteTexture2D(entry.fullPath, entry.texture);
            //AssetDatabase.ImportAsset(entry.assetPath, ImportAssetOptions.ForceUpdate);
        }




        public RenderTexture LoadCollisionMask(Terrain terrain, CollisionMaskType type, float radius, int id = 0, string tag = "")
        {

            if (m_cacheEntries == null)
            {
                m_cacheEntries = new CollisionMaskCacheEntry[0];
            }

            string fileName = "";
            switch (type)
            {
                case CollisionMaskType.TerrainTree:
                    fileName = GetTreeCollisionMaskFileName(terrain, id, radius);
                    break;

                case CollisionMaskType.Tag:
                    fileName = GetTagCollisionMaskFileName(terrain, tag, radius);
                    break;
                default:
                    return null;
            }

            if (fileName != "")
            {
                //file in cache? If yes, return from there
                int length = m_cacheEntries.Length;
                for (int i = 0; i < length; i++)
                {
                    if (m_cacheEntries[i].fileName == fileName)
                        //found it, we can return it & are done
                        return m_cacheEntries[i].texture;
                }

                //not in cache? We need to bake it then!
                switch (type)
                {
                    case CollisionMaskType.TerrainTree:
                        return BakeTerrainTreeCollisions(terrain, id, radius);
                    case CollisionMaskType.Tag:
                        return BakeTerrainTagCollisions(terrain, tag, radius);
                    default:
                        return null;
                }


                //not in cache? try reading from disk...
#if UNITY_EDITOR
                //Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(GaiaDirectories.GetTerrainCollisionDirectory(terrain) + "/" + GetTreeCollisionMaskFileName(terrain, id, radius), typeof(Texture2D));


                //if (texture != null)
                //{
                //    //found it on disk, write a copy of it in cache for next access attempt, then return the copy.
                //    //We need to work with a copy here because otherwise we can run into issue when trying to overwrite the image file
                //    Texture2D copy = new Texture2D(texture.width, texture.height, texture.format, true);
                //    Graphics.CopyTexture(texture, copy);
                //    AddNewCollisionMaskCacheEntry(copy, terrain, fileName);
                //    return copy;
                //}
#endif

            }
            else return null;


        }

        public void BakeAllTreeCollisions(int treePrototypeId, float radius)
        {
            foreach (Terrain t in Terrain.activeTerrains)
            {
                BakeTerrainTreeCollisions(t, treePrototypeId, radius);
            }
        }

        public void BakeAllTagCollisions(string m_tag, float m_tagRadius)
        {
            foreach (Terrain t in Terrain.activeTerrains)
            {
                BakeTerrainTagCollisions(t, m_tag, m_tagRadius);
            }
        }

        public void ClearCache()
        {
#if UNITY_EDITOR

            int length = m_cacheEntries.Length;
            for (int i = 0; i < length; i++)
            {
                DestroyImmediate(m_cacheEntries[i].texture);
                m_cacheEntries[i].texture = null;

            }

            m_cacheEntries = new CollisionMaskCacheEntry[0];
#endif
        }

        public void ClearCacheForSpawn()
        {
            if (!m_autoSpawnRunning)
            {
                ClearCache();
            }
            else
            {
                if (!m_cacheClearedForAutoSpawn)
                {
                    ClearCache();
                    m_cacheClearedForAutoSpawn = true;
                }
            }
        }

        public void SetTreeDirty(int treePrototypeIndex)
        {
            string searchString = GetTreeCollisionMaskSearchString(treePrototypeIndex);
            SetDirty(searchString);

        }

        public void SetTagDirty(string tag)
        {
            string searchString = GetTagCollisionMaskSearchString(tag);
            SetDirty(searchString);
        }


        private void SetDirty(string searchString)
        {
            //Release the affected render textures
            for (int i = 0; i < m_cacheEntries.Length; i++)
            {
                if (m_cacheEntries[i].fileName.Contains(searchString))
                {
                    m_cacheEntries[i].texture.Release();
                    DestroyImmediate(m_cacheEntries[i].texture);
                    m_cacheEntries[i].texture = null;
                }
            }
            //Remove the affected array entries
            m_cacheEntries = m_cacheEntries.Where(x => x.fileName.Contains(searchString) != true).ToArray();
        }

        public void BeginAutoSpawn()
        {
            m_autoSpawnRunning = true;
            m_cacheClearedForAutoSpawn = false;
        }

        public void EndAutoSpawn()
        {
            m_autoSpawnRunning = false;
            m_cacheClearedForAutoSpawn = false;
        }

    }
}