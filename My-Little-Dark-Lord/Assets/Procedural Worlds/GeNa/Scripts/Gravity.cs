using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace GeNa
{
    /// <summary>
    /// A scriptable object to handle gravity interactions
    /// </summary>
    public class Gravity : ScriptableObject
    {
        [System.Serializable]
        public class GravityInstance
        {
            public Resource     m_resource;
            public GameObject   m_instance;
            public Vector3      m_startPosition;
            public Vector3      m_endPosition;
            public Vector3      m_startRotation;
            public Vector3      m_endRotation;
        }

        public bool m_haveGravity = false;
        public List<GravityInstance> m_instances = new List<GravityInstance>();

        /// <summary>
        /// Update the instance position and rotations
        /// </summary>
        public void UpdateInstances()
        {
            foreach (var instance in m_instances)
            {
                if (instance.m_instance != null)
                {
                    instance.m_endPosition = instance.m_instance.transform.position;
                    instance.m_endRotation = instance.m_instance.transform.rotation.eulerAngles;
                }
                m_haveGravity = true;
            }
        }

        /// <summary>
        /// Add the gravity instances to gravity object
        /// </summary>
        /// <param name="instanceList"></param>
        public void AddInstances(List<Gravity.GravityInstance> instanceList)
        {
            m_haveGravity = false;
            m_instances.AddRange(instanceList);
        }

        /// <summary>
        /// Update the originals to the starting point
        /// </summary>
        public void UpdateOriginalsToStart()
        {
            foreach (var instance in m_instances)
            {
                if (instance.m_instance != null)
                {
                    instance.m_instance.transform.position = instance.m_startPosition;
                    instance.m_instance.transform.rotation = Quaternion.Euler(instance.m_startRotation.x, instance.m_startRotation.y, instance.m_startRotation.z);
                }
            }
        }

        /// <summary>
        /// Update originals to ending point
        /// </summary>
        public void UpdateOriginalsToEnd()
        {
            foreach (var instance in m_instances)
            {
                if (instance.m_instance != null)
                {
                    instance.m_instance.transform.position = instance.m_endPosition;
                    instance.m_instance.transform.rotation = Quaternion.Euler(instance.m_endRotation.x, instance.m_endRotation.y, instance.m_endRotation.z);
                }
            }
        }

		/// <summary>
		/// Respawn the instance resources to their final state
		/// </summary>
		public void FinaliseGravity(Spawner spawner)
		{
			//Grab the existing lightprobes
			spawner.LoadLightProbes();
#if SECTR_CORE_PRESENT
			IDictionary<Transform, List<GameObject>> gameobjectsPerParents = new Dictionary<Transform, List<GameObject>>();
#endif

			//Now process the instances
			foreach (var instance in m_instances)
			{
				if (instance.m_instance != null)
				{
					//Create the new instance
					GameObject go;
#if UNITY_EDITOR
					go = PrefabUtility.InstantiatePrefab(instance.m_resource.m_prefab) as GameObject;
#else
                    go = Instantiate(instance.m_resource.m_prefab);
#endif

					go.name = "_Sp_" + instance.m_resource.m_name;
					if (instance.m_resource.m_conformToSlope == true)
					{
						go.name = "_Sp_" + instance.m_resource.m_name + " C";
					}
					go.transform.position = instance.m_endPosition;
					go.transform.localScale = instance.m_instance.transform.localScale;
					go.transform.rotation = Quaternion.Euler(instance.m_endRotation.x, instance.m_endRotation.y, instance.m_endRotation.z);
					go.transform.parent = instance.m_instance.transform.parent;
#if SECTR_CORE_PRESENT
					if (spawner.m_doSectorise)
					{
						// Create a collection of Game Objects for each parents for the sectorisation
						if (gameobjectsPerParents.ContainsKey(go.transform.parent))
						{
							gameobjectsPerParents[go.transform.parent].Add(go);
						}
						else
						{
							gameobjectsPerParents[go.transform.parent] = new List<GameObject> { go };
						}
					}
#endif

					//Handle optimisation and flags
					spawner.AutoOptimiseGameObject(instance.m_resource, go);

					//Handle light probes
					spawner.AutoProbeGameObject(instance.m_resource, go);

					//And delete the old instance
					DestroyImmediate(instance.m_instance);
				}
			}

#if SECTR_CORE_PRESENT
			if (spawner.m_doSectorise)
			{
				// Loop through the parents and send their child GOs to sectors
				// GOs not in sectors will be left in their parents. Nothing will happem if there are no sectors
				foreach (var parentAndGOs in gameobjectsPerParents)
				{
					SECTR_SectorUtils.SendObjectsIntoSectors(
						parentsUndoList: ref spawner.m_parentsUndoList,
						gameObjects: parentAndGOs.Value,
						parentLocation: parentAndGOs.Key.position,
						hierarchy: new string[] { parentAndGOs.Key.name },
						localizeBy: spawner.m_sectorReparentingMode,
						mergeSpawns: spawner.m_mergeSpawns,
						doGlobalParenting: false
						);

					// Check if parent is now empty and clean up
					spawner.UnspawnParentIfEmpty(parentAndGOs.Key.gameObject);
				}
			}
#endif

			//Delete instances
			m_haveGravity = false;
            m_instances.Clear();
        }
    }
}