using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GeNa
{
    /// <summary>
    /// Light probe manager
    /// </summary>
    public class ProbeManager
    {
        #pragma warning disable 414
        private Quadtree<LightProbeGroup> m_probeLocations = new Quadtree<LightProbeGroup>(new Rect(0, 0, 10f, 10f));
        #pragma warning restore 414

        /// <summary>
        /// Load the probes in from the scene
        /// </summary>
        public void LoadProbesFromScene()
        {
            //Start time
            //DateTime startTime = DateTime.Now;

            //Destroy previous contents
            m_probeLocations = null;

            //Work out the bounds of the environment
            float minY = float.NaN;
            float minX = float.NaN;
            float maxX = float.NaN;
            float minZ = float.NaN;
            float maxZ = float.NaN;
            Terrain sampleTerrain = null;
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                if (float.IsNaN(minY))
                {
                    sampleTerrain = terrain;
                    minY = terrain.transform.position.y;
                    minX = terrain.transform.position.x;
                    minZ = terrain.transform.position.z;
                    maxX = minX + terrain.terrainData.size.x;
                    maxZ = minZ + terrain.terrainData.size.z;
                }
                else
                {
                    if (terrain.transform.position.x < minX)
                    {
                        minX = terrain.transform.position.x;
                    }
                    if (terrain.transform.position.z < minZ)
                    {
                        minZ = terrain.transform.position.z;
                    }
                    if ((terrain.transform.position.x + terrain.terrainData.size.x) > maxX)
                    {
                        maxX = terrain.transform.position.x + terrain.terrainData.size.x;
                    }
                    if ((terrain.transform.position.z + terrain.terrainData.size.z) > maxZ)
                    {
                        maxZ = terrain.transform.position.z + terrain.terrainData.size.z;
                    }
                }
            }

            if (sampleTerrain != null)
            {
                Rect terrainBounds = new Rect(minX, minZ, maxX - minX, maxZ - minZ);
                m_probeLocations = new Quadtree<LightProbeGroup>(terrainBounds, 32);
            }
            else
            {
                Rect bigSpace = new Rect(-10000f, -10000f, 20000f, 20000f);
                m_probeLocations = new Quadtree<LightProbeGroup>(bigSpace, 32);
            }

            //Now grab all the light probes in the scene
            LightProbeGroup probeGroup;
            LightProbeGroup [] probeGroups = UnityEngine.Object.FindObjectsOfType<LightProbeGroup>();

            for (int probeGroupIdx = 0; probeGroupIdx < probeGroups.Length; probeGroupIdx++)
            {
                probeGroup = probeGroups[probeGroupIdx];
                for (int probePosition = 0; probePosition < probeGroup.probePositions.Length; probePosition++)
                {
                    m_probeLocations.Insert(probeGroup.transform.position.x + probeGroup.probePositions[probePosition].x, probeGroup.transform.position.z + probeGroup.probePositions[probePosition].z, probeGroup);
                }
            }
            //Debug.Log(string.Format("Loaded {0} probe positions in {1:0.000} ms", m_probeLocations.Count, (DateTime.Now - startTime).TotalMilliseconds));
        }

        /// <summary>
        /// Add a probe instance into storage - must be called after the initial load call
        /// </summary>
        /// <param name="position">Position it is being located at</param>
        /// <param name="probeGroup">Probe group being managed</param>
        public void AddProbe(Vector3 position, LightProbeGroup probeGroup)
        {
            if (m_probeLocations == null)
            {
                return;
            }
            m_probeLocations.Insert(position.x, position.z, probeGroup);
        }

        /// <summary>
        /// Remove a probe instance from storage
        /// </summary>
        /// <param name="position">Position it is being located at</param>
        /// <param name="probeGroup">Probe group being managed</param>
        public void RemoveProbe(Vector3 position, LightProbeGroup probeGroup)
        {
            if (m_probeLocations == null)
            {
                return;
            }
            m_probeLocations.Remove(position.x, position.z, probeGroup);
        }

        /// <summary>
        /// Return the lightprobes in the vicinity of the location and range
        /// </summary>
        /// <param name="position">Location to check</param>
        /// <param name="range">Distance from this location to check for</param>
        /// <returns>List of matching light probe groups</returns>
        public List<LightProbeGroup> GetProbeGroups(Vector3 position, float range)
        {
            if (m_probeLocations == null)
            {
                return new List<LightProbeGroup>();
            }
            Rect query = new Rect(position.x - range, position.z - range, range * 2f, range * 2f);
            return m_probeLocations.Find(query).ToList();
        }

        /// <summary>
        /// Return the number of trees within range of the location provided
        /// </summary>
        /// <param name="position">Location to check</param>
        /// <param name="range">Range around location to check</param>
        /// <returns>Number of trees within range</returns>
        public int Count(Vector3 position, float range)
        {
            if (m_probeLocations == null)
            {
                return 0;
            }
            Rect query = new Rect(position.x - range, position.z - range, range * 2f, range * 2f);
            return m_probeLocations.Find(query).Count();
        }

        /// <summary>
        /// Return the number of probes being managed
        /// </summary>
        /// <returns>Number of probes being managed</returns>
        public int Count()
        {
            if (m_probeLocations == null)
            {
                return 0;
            }
            return m_probeLocations.Count;
        }

        /// <summary>
        /// Clears the storage
        /// </summary>
        public void Clear()
        {
            if (m_probeLocations != null)
            {
                m_probeLocations = null;
            }
        }
    }
}
