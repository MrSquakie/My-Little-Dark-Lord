using UnityEngine;
using System.Collections.Generic;
using System;

namespace GeNa
{
    /// <summary>
    /// Spawn prototypes are collections of one or more resources that can be collectively spawned.
    /// </summary>
    [System.Serializable]
    public class Prototype : ISerializationCallbackReceiver
    {
        /// <summary>
        /// The Spawner the prototype belongs to
        /// </summary>
        public Spawner Spawner
        {
            get
            {
                return m_spawner;
            }
        }
        [SerializeField] Spawner m_spawner;

        /// <summary>
        /// An id received from the Spawner whern the prototype is created.
        /// Used to identify the prototype during serialization and possibly deep copy.
        /// </summary>
        public int Id { get { return m_id; } }
        [SerializeField] private int m_id = -1;

        /// <summary>
        /// ID code string used in for example container names.
        /// </summary>
        public string IdCode { get { return string.Format("[P{0}]", Id); } }

        public string   m_name;
        public bool     m_active = true;
        public Vector3  m_size = Vector3.one;
        public Vector3  m_extents = Vector3.one;
        [Obsolete("Prototype.m_boundsBorder is now obsolete. Use the Spawn Criteria Overrides system instead.")]
        public float    m_boundsBorder = 0f;
        public bool     m_constrainWithinMaskedBounds = false;
        public bool     m_invertMaskedAlpha = false;
        public bool     m_scaleOnMaskedAlpha = false;
        public float    m_scaleOnMaskedAlphaMin = 0.5f;
        public float    m_scaleOnMaskedAlphaMax = 1f;
        public bool     m_successOnMaskedAlpha = false;
        public float    m_forwardRotation = 0f;
        /// <summary>
        /// This is legacy and only used so we can get the resources from spawners that were created with legacy versions of GeNa
        /// </summary>
        [Obsolete("m_resources is now Obsolete. Use m_resourceTree instead.")]
        public List<Resource> m_resources = new List<Resource>();
        // This is only a list to support the legacy POI system, otherwise there will be a single top level resource per proto
        [NonSerialized] public List<Resource> m_resourceTree = new List<Resource>();
        public Constants.ResourceType m_resourceType = Constants.ResourceType.Prefab;
        public bool     m_hasColliders = false;
        public bool     m_hasMeshes = false;
        public bool     m_hasRigidBody = false;
        public long     m_instancesSpawned = 0;
        public Color    m_imageFilterColour = Color.white;
        public float    m_imageFilterFuzzyMatch = 0.8f;

        //Editor related
        public bool m_displayedInEditor = false;
        public bool m_showAdvancedOptions = false;

        //Linked spawner related

        /// <summary>
        /// Does this Proto have a Resource which is linked to a child Spawner?
        /// </summary>
        public bool HasLinkedSpawner
        {
            get
            {
                return m_hasLinkedSpawner;
            }
        }
        [SerializeField] private bool m_hasLinkedSpawner = false;
        
        //Spawn Criteria Overrides
        public bool ShowSpawnCriteriaOverrides = false;

        //Spawn Criteria Overrides - Toggles
        public bool m_anyCritOverrideApplies = false;
        public bool m_overrideCritVirginCheckType = false;
        public bool m_overrideCritSpawnCollisionLayers = false;
        public bool m_overrideCritBoundsBorder = false;
        public bool m_overrideCritCheckHeight = false;
        public bool m_overrideCritMinSpawnHeight = false;
        public bool m_overrideCritMaxSpawnHeight = false;
        public bool m_overrideCritHeightVariance = false;
        public bool m_overrideCritMinSpawnSlope = false;
        public bool m_overrideCritMaxSpawnSlope = false;
        public bool m_overrideCritCheckSlope = false;
        public bool m_overrideCritSlopeVariance = false;
        public bool m_overrideCritCheckTextures = false;
        public bool m_overrideCritTextureStrength = false;
        public bool m_overrideCritTextureVariance = false;
        public bool m_overrideCritMaskInvert = false;

        //Spawn Criteria Overrides - Overrides
        public Constants.VirginCheckType m_critVirginCheckType = Constants.VirginCheckType.Point;
        public LayerMask m_critSpawnCollisionLayers;
        public float m_critBoundsBorder = 0f;
        //This replaces the CritCheckHeight switch.
        public Constants.CriteriaRangeType m_critCheckHeightType = Constants.CriteriaRangeType.Mixed;
        //Now legacy - replaced by CheckHeightType.
        public bool m_critCheckHeight = true;
        public float m_critMinSpawnHeight = -20f;
        public float m_critMaxSpawnHeight = 5000f;
        public float m_critHeightVariance = 30f;
        //This replaces the CritCheckSlope switch.
        public Constants.CriteriaRangeType m_critCheckSlopeType = Constants.CriteriaRangeType.Range;
        //Now legacy - replaced by CheckSlopeType.
        public bool m_critCheckSlope = true;
        //Private fields accessible by properties
        [SerializeField] private float m_critMinSpawnSlope = 0f;
        [SerializeField] private float m_critMaxSpawnSlope = 90f;
        public float m_critSlopeVariance = 30f;
        public bool m_critCheckTextures = false;
        public float m_critTextureStrength = 0f;
        public float m_critTextureVariance = 0.1f;
        public bool m_disableCritCheckMask = false;
        public bool m_critMaskInvert = false;

        //Per spawn - criteria
        public float m_critMinHeight = 0f;
        public float m_critMaxHeight = 10000f;
        public float m_critMinSlope = 0f;
        public float m_critMaxSlope = 90f;
        public float m_critMinTextureStrength = 0f;
        public float m_critMaxTextureStrength = 1f;
        public float m_critMaskFractalMin = 0f;
        public float m_critMaskFractalMax = 1f;

        //Extension variables

        /// <summary>
        /// Does this Proto have a Resource which triggers any extensions?
        /// </summary>
        public bool HasExtensions
        {
            get
            {
                return m_hasExtensions;
            }
        }
        [SerializeField] private bool m_hasExtensions = false;

        //Tree prototypes variables
        /// <summary>
        /// List that contains all the resources for a flattened serialization.
        /// </summary>
        [SerializeField] private List<Resource> _allResources = new List<Resource>();

        /// <summary>
        /// Minimum Slope Limit. Value has to be between 0f and 90f.
        /// </summary>
        public float MinSpawnSlope
        {
            get
            {
                return m_critMinSpawnSlope;
            }
            set
            {
                m_critMinSpawnSlope = Mathf.Clamp(value, 0f, 90f);
            }
        }

        /// <summary>
        /// Maximum Slope Limit. Value has to be between 0f and 90f.
        /// </summary>
        public float MaxSpawnSlope
        {
            get
            {
                return m_critMaxSpawnSlope;
            }
            set
            {
                m_critMaxSpawnSlope = Mathf.Clamp(value, 0f, 90f);
            }
        }

        /// <summary>
        /// Is the prototype a now legacy POI prototype
        /// </summary>
        public bool LegacyPOI { get { return m_resourceTree.Count > 1; } }

        /// <summary>
        /// Returns the top-level resource's rotation unless this is POI prototype
        /// </summary>
        public float TopRotation
        {
            get
            {
                if (LegacyPOI)
                {
                    return 0f;
                }

                return m_resourceTree[0].NextRotation.y;
            }
        }

        /// <summary>
        /// Bounds center of the top-level <see cref="Resource"/> relative to its pivot.
        /// </summary>
        public Vector3 NextBoundsCenter { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Prototype(Spawner spawner)
        {
            m_spawner = spawner;
            m_id = spawner.NextProtoId;
        }

        /// <summary>
        /// Deep copy constructor
        /// </summary>
        /// <param name="src"></param>
        public Prototype(Spawner parentSpawner, Prototype src)
        {
            m_spawner = parentSpawner;
            // Making sure the source has a valid ID in case it was created in an older version of GeNa
            m_id = src._GetValidId();

            m_name = src.m_name;
            m_active = src.m_active;
            m_size = src.m_size;
            m_extents = src.m_extents;
#pragma warning disable 0618
            m_boundsBorder = src.m_boundsBorder;
#pragma warning restore 0618
            m_constrainWithinMaskedBounds = src.m_constrainWithinMaskedBounds;
            m_invertMaskedAlpha = src.m_invertMaskedAlpha;
            m_scaleOnMaskedAlpha = src.m_scaleOnMaskedAlpha;
            m_scaleOnMaskedAlphaMin = src.m_scaleOnMaskedAlphaMin;
            m_scaleOnMaskedAlphaMax = src.m_scaleOnMaskedAlphaMax;
            m_successOnMaskedAlpha = src.m_successOnMaskedAlpha;
            m_forwardRotation = src.m_forwardRotation;
            m_resourceTree = new List<Resource>();
            for (int i = 0; i < src.m_resourceTree.Count; i++)
            {
                m_resourceTree.Add(new Resource(parentSpawner, null, src.m_resourceTree[i]));
            }
            m_resourceType = src.m_resourceType;
            m_hasColliders = src.m_hasColliders;
            m_hasMeshes = src.m_hasMeshes;
            m_hasRigidBody = src.m_hasRigidBody;

            //Editor related
            m_displayedInEditor = src.m_displayedInEditor;
            m_showAdvancedOptions = src.m_showAdvancedOptions;

            //Child proto Editor related
            m_hasLinkedSpawner = src.m_hasLinkedSpawner;

            m_instancesSpawned = src.m_instancesSpawned;
            m_imageFilterColour = src.m_imageFilterColour;
            m_imageFilterFuzzyMatch = src.m_imageFilterFuzzyMatch;

            //Spawn Criteria Overrides
            ShowSpawnCriteriaOverrides = src.ShowSpawnCriteriaOverrides;

            //Spawn Criteria Overrides - Toggles
            m_overrideCritVirginCheckType = src.m_overrideCritVirginCheckType;
            m_overrideCritSpawnCollisionLayers = src.m_overrideCritSpawnCollisionLayers;
            m_overrideCritBoundsBorder = src.m_overrideCritBoundsBorder;
            m_overrideCritCheckHeight = src.m_overrideCritCheckHeight;
            m_overrideCritMinSpawnHeight = src.m_overrideCritMinSpawnHeight;
            m_overrideCritMaxSpawnHeight = src.m_overrideCritMaxSpawnHeight;
            m_overrideCritHeightVariance = src.m_overrideCritHeightVariance;
            m_overrideCritCheckSlope = src.m_overrideCritCheckSlope;
            m_overrideCritMinSpawnSlope = src.m_overrideCritMinSpawnSlope;
            m_overrideCritMaxSpawnSlope = src.m_overrideCritMaxSpawnSlope;
            m_overrideCritSlopeVariance = src.m_overrideCritSlopeVariance;
            m_overrideCritCheckTextures = src.m_overrideCritCheckTextures;
            m_overrideCritTextureStrength = src.m_overrideCritTextureStrength;
            m_overrideCritTextureVariance = src.m_overrideCritTextureVariance;
            m_overrideCritMaskInvert = src.m_overrideCritMaskInvert;

            //Spawn Criteria Overrides - Overrides
            m_critVirginCheckType = src.m_critVirginCheckType;
            m_critSpawnCollisionLayers = src.m_critSpawnCollisionLayers;
            m_critBoundsBorder = src.m_critBoundsBorder;
            m_critCheckHeightType = src.m_critCheckHeightType;
            //Now legacy - replaced by CheckHeightType.
            m_critCheckHeight = src.m_critCheckHeight;
            m_critMinSpawnHeight = src.m_critMinSpawnHeight;
            m_critMaxSpawnHeight = src.m_critMaxSpawnHeight;
            m_critHeightVariance = src.m_critHeightVariance;
            
            m_critCheckSlopeType = src.m_critCheckSlopeType;
            //Now legacy - replaced by CheckSlopeType.
            m_critCheckSlope = src.m_critCheckSlope;
            MinSpawnSlope = src.MinSpawnSlope;
            MaxSpawnSlope = src.MaxSpawnSlope;
            m_critSlopeVariance = src.m_critSlopeVariance;
            m_critCheckTextures = src.m_critCheckTextures;
            m_critTextureStrength = src.m_critTextureStrength;
            m_critTextureVariance = src.m_critTextureVariance;
            m_disableCritCheckMask = src.m_disableCritCheckMask;
            m_critMaskInvert = src.m_critMaskInvert;

            m_anyCritOverrideApplies = src.m_anyCritOverrideApplies =
                m_overrideCritVirginCheckType ||
                m_overrideCritSpawnCollisionLayers ||
                m_overrideCritBoundsBorder ||
                m_overrideCritCheckHeight ||
                m_overrideCritMinSpawnHeight ||
                m_overrideCritMaxSpawnHeight ||
                m_overrideCritHeightVariance ||
                m_overrideCritCheckSlope ||
                m_overrideCritSlopeVariance ||
                m_overrideCritCheckTextures ||
                m_overrideCritTextureStrength ||
                m_overrideCritTextureVariance ||
                m_overrideCritMaskInvert ||
                m_disableCritCheckMask;

            //Extension variables
            m_hasExtensions = src.m_hasExtensions;

            //_allResources is only used to flatten when Serializing and should not be copied.
        }

        /// <summary>
        /// This should only ever be used by Spawner version upgrade methods.
        /// </summary>
        public void SetSpawner(Spawner spawner)
        {
            m_spawner = spawner;

            if (m_resourceTree == null)
            {
                Debug.LogErrorFormat("Prototype {0} resources list is null.", m_name);
                return;
            }

            for (int i = 0; i < m_resourceTree.Count; i++)
            {
                m_resourceTree[i].SetSpawner(spawner);
            }
        }

        /// <summary>
        /// Deletes the Prototype and cleans up. Removes it from parent Resources or Child Prototypes if there are any.
        /// </summary>
        public void Delete()
        {
            for (int i = 0; i < m_resourceTree.Count; i++)
            {
                m_resourceTree[i].Delete();
            } 

            if (m_spawner.m_spawnPrototypes.Contains(this))
            {
                m_spawner.m_spawnPrototypes.Remove(this);
            }

            // Update HasTerrainProto if this was a terrain proto.
            if (m_resourceType > Constants.ResourceType.Prefab)
            {
                m_spawner.UpdateHasActiveTerrainProto();
            }

            // Make sure the visualization resolution is optimized
            m_spawner.UpdateVisualisationResolution();
        }

        /// <summary>
        /// Looks through the child <seealso cref="Resource"/>s and Updates <see cref="=m_hasLinkedSpawner"/> for this Proto.
        /// </summary>
        public void UpdateHasLinkedSpawner()
        {
            m_hasLinkedSpawner = false;

            // Check if any of the resources have a Child Proto
            for (int i = 0; i < m_resourceTree.Count; i++)
            {
                if (m_resourceTree[i].TreeHasLinkedSpawners)
                {
                    m_hasLinkedSpawner = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Looks through the child <seealso cref="Resource"/>s and Updates <see cref="=m_hasExtensions"/> for this Proto.
        /// </summary>
        public void UpdateHasExtensions()
        {
            m_hasExtensions = false;

            // Check if any of the resources have a Child Proto
            for (int i = 0; i < m_resourceTree.Count; i++)
            {
                if (m_resourceTree[i].TreeHasExtensions)
                {
                    m_hasExtensions = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Sets conform slope for the root level Resources
        /// (Only root level, because lower levels generally don't use the setting).
        /// </summary>
        public void SetRootConformSlope(bool value)
        {
            for (int i = 0; i < m_resourceTree.Count; i++)
            {
                m_resourceTree[i].m_conformToSlope = value;
            }
        }

        /// <summary>
        /// Looks through the child <seealso cref="Resources"/> and removes any stale links to Child Spawners (Checks if the indexes are in the Spawner's <seealso cref="m_linkedSpawners"/>.
        /// </summary>
        public bool RemoveStaleLinks()
        {
            bool removedAny = false;
            for (int i = 0; i < m_resourceTree.Count; i++)
            {
                removedAny |= m_resourceTree[i].RemoveStaleLinks();
            }

            return removedAny;
		}

		/// <summary>
		/// Takes care of any needed initialisations, including GeNa extensions.
		/// </summary>
		internal void Initialise(Spawner rootSpawner)
		{
			// Initialise the resources - they will init any GeNa extensions that may need it.
			for (int i = 0; i < m_resourceTree.Count; i++)
			{
				m_resourceTree[i].Initialise(rootSpawner);
			}
		}

		/// <summary>
		/// Get the minimum extents for this prototype according to all its own and its resources settings.
		/// </summary>
		public Vector3 GetNextExtents(Vector3 placementCritScale)
        {
            Bounds bounds = new Bounds();

            GTransform wsTransform = new GTransform
            {
                Postition = Vector3.zero,
                Rotation = Vector3.zero,
                Scale = placementCritScale
            };

            for (int i = 0; i < m_resourceTree.Count; i++)
            {
                //For legacy POI we don't handle "top level"
                m_resourceTree[i].IncludeNextBounds(ref bounds, wsTransform, !LegacyPOI);
            }

            NextBoundsCenter = bounds.center;
            Vector3 extents = bounds.extents;
            extents += new Vector3(m_critBoundsBorder * extents.x, m_critBoundsBorder * extents.y, m_critBoundsBorder * extents.z);

            return extents;
        }

        /// <summary>
        /// Get the minimum extents for this prototype according to all its own and its resources settings.
        /// </summary>
        public Vector3 GetMinExtents(Vector3 placementCritScale)
        {
            Bounds bounds = new Bounds();

            GTransform wsTransform = new GTransform
            {
                Postition = Vector3.zero,
                Rotation = Vector3.zero,
                Scale = placementCritScale
            };

            for (int i = 0; i < m_resourceTree.Count; i++)
            {
                //For legacy POI we don't handle "top level"
                m_resourceTree[i].IncludeInMinBounds(ref bounds, wsTransform, !LegacyPOI);
            }

            Vector3 extents = bounds.extents;
            extents += new Vector3(m_critBoundsBorder * extents.x, m_critBoundsBorder * extents.y, m_critBoundsBorder * extents.z);
            return extents;
        }

        /// <summary>
        /// Trigger all resources to precalculate their offsets in preparation for getting extents for bounds checking and spawning.
        /// </summary>
        /// <param name="topResourceLocationOffset">The already calculated location offset for the top level resource in the tree.
        /// This is ignored for POI prototypes, since the origin is the center of their bounds.</param>
        public void PrecalculateOffsets(XorshiftPlus randomGen, Vector3 spawnerScale, bool scaleToNearestInt, Vector3 topResourceLocationOffset)
        {
            //Set the top level resource's position offset if not POI
            if (m_resourceTree.Count == 1)
            {
                m_resourceTree[0].PrecalculateOffsets(randomGen, spawnerScale, scaleToNearestInt, topResourceLocationOffset);
                return;
            }

            //This is a legacy POI prototype. Loop through the resources and use the overload which also precalculates their location offsets
            for (int i = 0; i < m_resourceTree.Count; i++)
            {
                m_resourceTree[i].PrecalculateOffsets(randomGen, spawnerScale, scaleToNearestInt);
            }
        }

        /// <summary>
        /// Iterate through the resources and get chances of success
        /// </summary>
        /// <returns></returns>
        public float GetSuccessChance()
        {
            if (!m_active)
            {
                return 0f;
            }

            float success = 0f;
            foreach (var srcRes in m_resourceTree)
            {
                if (srcRes.m_successRate > success)
                {
                    success = srcRes.m_successRate;
                }
            }
            return success;
        }

        /// <summary>
        /// Flattening the resource tree
        /// </summary>
        public void OnBeforeSerialize()
        {
            _allResources.Clear();

            AddTreeToFlatList(m_resourceTree);
        }

        /// <summary>
        /// Used only before Serialization. It makes sure that the Proto has a valid ID (in case this's an older GeNa Spawner)
        /// </summary>
        /// <returns></returns>
        public int _GetValidId()
        {
            //First let's ensure that the Proto has a valid ID even if it was created in an older version of GeNa
            if (m_id < 0)
            {
                m_id = m_spawner.NextProtoId;
            }
            return m_id;
        }

        /// <summary>
        /// Traverses through a list of resources and adds them to the flat list recursively with their children included.
        /// Using the flat list for setialization. It also sets their parent index member for deserialization.
        /// </summary>
        private void AddTreeToFlatList(List<Resource> resources, int parentIndex = -1)
        {
            _GetValidId();

            if (resources == null)
            {
                return;
            }

            for (int i = 0; i < resources.Count; i++)
            {
                resources[i]._parentIx = parentIndex;
                _allResources.Add(resources[i]);
                // The index of the Resource in the list is _allResources.Count - 1
                AddTreeToFlatList(resources[i].Children, _allResources.Count - 1);
            }
        }

        /// <summary>
        /// Rebuilds the resource tree
        /// </summary>
        public void OnAfterDeserialize()
        {
#pragma warning disable 0618
            // Upgrade legacy spawners
            if (m_resources != null)
            {
                m_resourceTree = m_resources;
                m_resources = null;
            }
#pragma warning restore 0618
            else
            {
                m_resourceTree = new List<Resource>();
            }

            for (int i = 0; i < _allResources.Count; i++)
            {
                // If parent index is less than zero, it's a top level resource and will be added to m_resources
                if (_allResources[i]._parentIx < 0)
                {
                    m_resourceTree.Add(_allResources[i]);
                }
                // otherwise add it to its parent
                else
                {
                    _allResources[_allResources[i]._parentIx].Children.Add(_allResources[i]);
                    _allResources[i].SetParent(_allResources[_allResources[i]._parentIx]);
                }
            }
        }
    }
}