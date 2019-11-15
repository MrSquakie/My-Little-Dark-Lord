using UnityEngine;
using System.Collections;

namespace GeNa
{
    /// <summary>
    /// Some sample code to demonstrate runtime spawning. For example you could attach this to your player to spawn things around them.
    /// </summary>
    public class RuntimeSpawner : MonoBehaviour
    {
        [Tooltip("The amount of time in seconds that the spawner will run a spawn iteration.")]
        public float m_spawnInterval = 10f;

        [Tooltip("The spawner that will run the spawn iteration.")]
        public Spawner m_spawner = null;

        [Tooltip("Update the spawner settings on every spawn iteration, otherwise just use the original criteria / settings ant apply them at the current location.")]
        public bool m_updateSpawnerSettings = true;

        [Tooltip("Show debug messages when it runs.")]
        public bool m_showDebug = false;

        /// <summary>
        /// Use this for initialization - start the timed co-routine up
        /// </summary>
        void Start()
        {
            StartCoroutine(RunSpawnerIteration(m_spawnInterval));
        }

        /// <summary>
        /// Co-routine that runs the spawner iteration
        /// </summary>
        /// <param name="waitTime">The amout of time to wait between spawner iterations</param>
        /// <returns></returns>
        private IEnumerator RunSpawnerIteration(float waitTime)
        {
            while (true)
            {
                yield return new WaitForSeconds(waitTime);
                if (m_spawner != null)
                {
                    if (m_showDebug)
                    {
                        Debug.Log("Running spawner iteration");
                    }
                    if (m_updateSpawnerSettings)
                    {
                        //Get the thing that is underneath us if possible - we will use this to update the criteria for the spawner
                        Ray ray = new Ray(transform.position, Vector3.down);
                        RaycastHit hitInfo;
                        if (Physics.Raycast(ray, out hitInfo, 10000f, m_spawner.m_critSpawnCollisionLayers))
                        {
                            m_spawner.SetSpawnOrigin(hitInfo.transform, hitInfo.point, hitInfo.normal);
                            m_spawner.Spawn(hitInfo.point, false);
                        }
                        //Otherwise just do the best we can - we never got a hit
                        else
                        {
                            m_spawner.SetSpawnOrigin(null, transform.position, Vector3.up);
                            m_spawner.Spawn(transform.position, false);
                        }
                    }
                    else
                    {
                        m_spawner.Spawn(transform.position, false);
                    }
                }
                else
                {
                    if (m_showDebug)
                    {
                        Debug.Log("Need a spawner in order to do the spawn!");
                    }
                }
            }
        }
    }
}
