using System;
using UnityEngine;
using System.Collections.Generic;
using PWCommon2;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GeNa
{
    /// <summary>
    /// Resources are the things that can be spawned
    /// </summary>
    [System.Serializable]
    public partial class Resource : ISerializationCallbackReceiver
    {
        /// <summary>
        /// The Spawner the Resource belongs to.
        /// </summary>
        public Spawner Spawner
        {
            get
            {
                return m_spawner;
            }
        }
        [SerializeField] Spawner m_spawner;

        public string m_name;

        public Constants.ResourceType m_resourceType = Constants.ResourceType.Prefab;
        public bool ContainerOnly = false;

        public GameObject m_prefab;
        public string   m_assetID;
        // Asset name is used to attempt to find the asset if it was imported separately and got a new GUID
        public string   m_assetName;

        public Resource Parent { get; private set; }

        [SerializeField]
        private Constants.ResourceStatic m_static = Constants.ResourceStatic.Static;
        public Constants.ResourceStatic Static
        {
            get
            {
                return m_static;
            }

            set
            {
                if (value != m_static)
                {
                    m_static = value;
                    switch (value)
                    {
                        case Constants.ResourceStatic.Dynamic:
                            // Set anchestors Dynamic as well
                            if (Parent != null)
                            {
                                Parent.Static = value;
                            }
                            break;
                        case Constants.ResourceStatic.Static:
                            // Set descendants static as well
                            for (int i = 0; i < Children.Count; i++)
                            {
                                Children[i].Static = value;
                            }
                            break;
                        default:
                            throw new NotImplementedException("Not sure what this static type means: " + value);
                    }
                }
            }
        }

        public float    m_successRate = 1f;
        public bool     m_conformToSlope = true;

        public Vector3  m_minOffset = new Vector3(0f, -0.15f, 0f);
        public Vector3  m_maxOffset = new Vector3(0f, -0.15f, 0f);
        public Vector3  m_minRotation = Vector3.zero;
        public Vector3  m_maxRotation = new Vector3(0f, 360f, 0f);
        public bool     m_sameScale = true;
        public Vector3  m_minScale = Vector3.one;
        public Vector3  m_maxScale = Vector3.one;

        /// <summary>
        /// The array that contains the Texture selection set that can be used as a brush.
        /// </summary>
        public Texture2D[] BrushTextures
        {
            get
            {
                return m_brushTextures.ToArray();
            }
        }
        [SerializeField] private List<Texture2D> m_brushTextures;

        /// <summary>
        /// Index of the selected Brush Texture in the Texture selection set: BrushTextures.
        /// </summary>
        public int BrushIndex
        {
            get
            {
                return m_brushTXIndex;
            }
            set
            {
                if (m_brushTXIndex != value)
                {
                    m_brushTXIndex = value;
                    UpdateBrushTexture();
                }
            }
        }
        [SerializeField] private int m_brushTXIndex = 0;

        /// <summary>
        /// Base brush that is used to generate brushes.
        /// </summary>
        public UBrush   m_baseBrush;

        /// <summary>
        /// A cache to avoid generating the same brush twice.
        /// </summary>
        public IDictionary<int, UBrush> m_brushCache = new Dictionary<int, UBrush>();
        public float    m_opacity = 1f;
        public float    m_targetStrength = 1f;

        public int      m_terrainProtoIdx = 0;

        public bool     m_hasColliders = false;
        public bool     m_hasRootCollider = false;
        public bool     m_hasMeshes = true;
        public bool     m_hasRigidBody = false;
        public bool     m_flagLightmapStatic = true;
        public bool     m_flagBatchingStatic = true;
        public bool     m_flagOccludeeStatic = true;
        public bool     m_flagOccluderStatic = true;
        public bool     m_flagNavigationStatic = true;
        public bool     m_flagOffMeshLinkGeneration = true;
        public bool     m_flagReflectionProbeStatic = true;
        public bool     m_flagMovingObject = false;
        public bool     m_flagCanBeOptimised = true;
        public bool     m_flagForceOptimise = false;
        public bool     m_flagIsOutdoorObject = true;

        public bool     m_useColliderBounds = false;

        public Vector3  m_basePosition = Vector3.zero;
        public Vector3  m_baseRotation = Vector3.zero;
        public Vector3  m_baseScale = Vector3.one;
        public Vector3  m_baseSize = Vector3.zero;
        public Vector3  m_boundsCenter = Vector3.zero;
        public Vector3  m_baseColliderCenter = Vector3.zero;
        public bool     m_baseColliderUseConstScale = true;
        public float    m_baseColliderConstScaleAmount = 0.75f;
        public Vector3  m_baseColliderScale = Vector3.one;
        public long     m_instancesSpawned = 0;

        //Integration settings for AVS
        public string   m_avsID;
        public Constants.AVSVegetationType m_avsVegetationType = Constants.AVSVegetationType.Objects;

        //Editor variables
        public bool     m_displayedInEditor = false;

        //Tree prototypes variables
        //The tree itself is not serialised
        public List<Resource> Children
        {
            get
            {
                if (m_children == null)
                {
                    m_children = new List<Resource>();
                }

                return m_children;
            }
        }
        [NonSerialized] private List<Resource> m_children = new List<Resource>();

        public bool     HasChildren { get { return Children.Count > 0; } }
        public bool     OpenedInGuiHierarchy = false;
        /// <summary>
        /// Parent index is used to serialize the tree information in the flattened serialized format.
        /// </summary>
        public int      _parentIx = -1;

        //Precalculated values for spawning
        /// <summary>
        /// Will this be spawned by <see cref="Spawner.SpawnResourceTree"/>.
        /// </summary>
        public bool NextSuccess { get; private set; }

        /// <summary>
        /// The next position offset that will be applied to this Resource.
        /// </summary>
        public Vector3 NextPosition { get; private set; }

        /// <summary>
        /// The next rotation offset that will be applied to this Resource.
        /// </summary>
        public Vector3 NextRotation { get; private set; }

        /// <summary>
        /// The next scale offset that will be applied to this Resource.
        /// </summary>
        public Vector3 NextScale { get; private set; }

        /// <summary>
        /// Child prototype of this Resource.  Note: Child Prototype hierarchies cannot have their own child prototypes.
        /// </summary>
        public List<int> LinkedSpawners
        {
            get
            {
                return m_linkedSpawners;
            }
        }
        [SerializeField] private List<int> m_linkedSpawners = new List<int>();

        /// <summary>
        /// Does this Resource have Linked Spawners? The resource triggers Linked Spawners to spawn at its location.
        /// </summary>
        public bool HasLinkedSpawners
        {
            get
            {
                return m_linkedSpawners.Count > 0;
            }
        }

        /// <summary>
        /// Does this Resource have any Extensions it triggers when spawning?
        /// </summary>
        public bool HasExtensions
        {
            get
            {
				bool stateless = (m_statelessExtensions != null && m_statelessExtensions.Length > 0);
				return stateless || _hasStatefulExtensions;
            }
        }

        /// <summary>
        /// Does this Resource tree have any Linked Spawners? (checking children as well) - The Resource triggers Linked Spawners to spawn at its location.
        /// </summary>
        public bool TreeHasLinkedSpawners
        {
            get
            {
                if (HasLinkedSpawners)
                {
                    return true;
                }

                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i].TreeHasLinkedSpawners)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Does this Resource tree have any Extensions attached? (checking children as well) - The Resource triggers its extensions.
        /// </summary>
        public bool TreeHasExtensions
        {
            get
            {
                if (HasExtensions)
                {
                    return true;
                }

                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i].TreeHasExtensions)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

		// GeNa extensions

		/// <summary>
		/// Instances of all the extensions, only used during spawning and is set during <seealso cref="Initialise"/>.
		/// </summary>
		internal ExtensionInstance[] ExtensionInstances {
			get
			{
				return m_extensionInstances;
			}
			set
			{
				m_extensionInstances = value;
			}
		}
		[NonSerialized] private ExtensionInstance[] m_extensionInstances;

		/// <summary>
		/// The array that contains the extensions that are triggered by this Resrource.
		/// </summary>
		public IGeNaExtension[] Extensions
		{
			get
			{
				CompleteStatefulExtensionsDeserialisation();
				return m_extensions;
			}
		}
		[NonSerialized] private IGeNaExtension[] m_extensions;
		// Have to store the parent Prefabs/GOs in this, so we can work around serialisation
		[SerializeField] private GameObject[] _extensions;
		[SerializeField] private bool _hasStatefulExtensions = false;

		/// <summary>
		/// This is used because we can't get them during deserialisation.
		/// </summary>
		private void CompleteStatefulExtensionsDeserialisation()
		{
			if (m_extensions != null || _hasStatefulExtensions == false)
			{
				return;
			}

			// Get the extensions from their pregabs/gos.
			if (_extensions != null)
			{
				// Using a set to avoid duplications
				HashSet<IGeNaExtension> set = new HashSet<IGeNaExtension>();

				foreach (GameObject go in _extensions)
				{
					// Check if it has the component
					bool gxInterfaced = false;

					foreach (Component comp in go.GetComponents<Component>())
					{
						Type[] interfaces = comp.GetType().GetInterfaces();

						// Check if it implements the interface
						foreach (Type iface in interfaces)
						{
							if (iface == typeof(IGeNaExtension))
							{
								gxInterfaced = true;
								break;
							}
						}

						if (gxInterfaced)
						{
							set.Add(comp as IGeNaExtension);
						}
					}

					if (!gxInterfaced)
					{
						//We dont want to spawn spawners
						Debug.LogErrorFormat("[GeNa] Prefab/GameObject '<b>{0}</b>' was serialised as the sate holder of a GeNa Extension," +
							" but does not have a valid GeNa Extension Component.", go.name);
					}
				}

				m_extensions = new List<IGeNaExtension>(set).ToArray();
			}
		}

		/// <summary>
		/// The array that contains stateless extensions that are triggered by this Resrource.
		/// </summary>
		public Type[] StatelessExtensions
        {
            get
            {
                return m_statelessExtensions;
            }
        }
        [NonSerialized] private Type[] m_statelessExtensions;
        [SerializeField] private string[] _statelessExtensions;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Resource(Spawner spawner)
        {
            m_spawner = spawner;
        }

        /// <summary>
        /// Deep copy constructor
        /// </summary>
        /// <param name="src"></param>
        public Resource(Spawner parentSpawner, Resource parent, Resource src) : this(parentSpawner)
        {
            m_name = src.m_name;
            m_assetID = src.m_assetID;
            m_assetName = src.m_assetName;

            Parent = parent;

            m_static = src.m_static;

            m_resourceType = src.m_resourceType;
            ContainerOnly = src.ContainerOnly;

            m_prefab = src.m_prefab;
            m_successRate = src.m_successRate;
            m_conformToSlope = src.m_conformToSlope;

            m_minOffset = src.m_minOffset;
            m_maxOffset = src.m_maxOffset;
            m_minRotation = src.m_minRotation;
            m_maxRotation = src.m_maxRotation;
            m_sameScale = src.m_sameScale;
            m_minScale = src.m_minScale;
            m_maxScale = src.m_maxScale;
            
            // Brushes - no need to clone the textures themselves, so we can use shallow cloning of the list.
            m_brushTextures = (src.m_brushTextures != null) ? new List<Texture2D>(src.m_brushTextures) : new List<Texture2D>();
            m_brushTXIndex = src.m_brushTXIndex;
            m_baseBrush = src.m_baseBrush;
            // m_brushCache is used on for spawn caching and does not need to be serialised or copied
            m_opacity = src.m_opacity;
            m_targetStrength = src.m_targetStrength;

            m_terrainProtoIdx = src.m_terrainProtoIdx;

            m_hasColliders = src.m_hasColliders;
            m_hasRootCollider = src.m_hasRootCollider;
            m_hasMeshes = src.m_hasMeshes;
            m_hasRigidBody = src.m_hasRigidBody;
            m_flagLightmapStatic = src.m_flagLightmapStatic;
            m_flagBatchingStatic = src.m_flagBatchingStatic;
            m_flagOccludeeStatic = src.m_flagOccludeeStatic;
            m_flagOccluderStatic = src.m_flagOccluderStatic;
            m_flagNavigationStatic = src.m_flagNavigationStatic;
            m_flagOffMeshLinkGeneration = src.m_flagOffMeshLinkGeneration;
            m_flagReflectionProbeStatic = src.m_flagReflectionProbeStatic;
            m_flagMovingObject = src.m_flagMovingObject;
            m_flagForceOptimise = src.m_flagForceOptimise;
            m_flagCanBeOptimised = src.m_flagCanBeOptimised;
            m_flagIsOutdoorObject = src.m_flagIsOutdoorObject;

            m_useColliderBounds = src.m_useColliderBounds;

            m_basePosition = src.m_basePosition;
            m_baseRotation = src.m_baseRotation;
            m_baseScale = src.m_baseScale;
            m_baseSize = src.m_baseSize;
            m_boundsCenter = src.m_boundsCenter;
            m_baseColliderCenter = src.m_baseColliderCenter;
            m_baseColliderUseConstScale = src.m_baseColliderUseConstScale;
            m_baseColliderConstScaleAmount = src.m_baseColliderConstScaleAmount;
            m_baseColliderScale = src.m_baseColliderScale;
            m_displayedInEditor = src.m_displayedInEditor;
            m_instancesSpawned = src.m_instancesSpawned;

            m_children = new List<Resource>();
            for (int i = 0; i < src.Children.Count; i++)
            {
                m_children.Add(new Resource(parentSpawner, this, src.Children[i]));
            }

            OpenedInGuiHierarchy = src.OpenedInGuiHierarchy;
            //_parentIx is only used to flatten when Serializing and should not be copied.

            //Linked Spawners
            m_linkedSpawners = (src.m_linkedSpawners != null) ? new List<int>(src.m_linkedSpawners) : new List<int>();

            // GeNa extensions
            m_extensions = (src.m_extensions != null) ? new List<IGeNaExtension>(src.m_extensions).ToArray() : null;
            _extensions = (src._extensions != null) ? new List<GameObject>(src._extensions).ToArray() : new GameObject[0];
            _hasStatefulExtensions = src._hasStatefulExtensions;
            m_statelessExtensions = (src.m_statelessExtensions != null) ? new List<Type>(src.m_statelessExtensions).ToArray() : new Type[0];
        }

        /// <summary>
        /// This should only ever be used by Spawner version upgrade methods.
        /// </summary>
        public void SetSpawner(Spawner spawner)
        {
            m_spawner = spawner;

            if (Children == null)
            {
                m_children = new List<Resource>();
                return;
            }

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].SetSpawner(spawner);
            }
        }

        /// <summary>
        /// Set the parent for this Resource
        /// </summary>
        public void SetParent(Resource parent)
        {
            if (parent != null)
            {
                Parent = parent;
            }
        }

        /// <summary>
        /// The Prototype is being deleted. Let's clean up. Remove any Child Proto relation involvind this or it's children.
        /// </summary>
        public void Delete()
        {
            // Remove Spawner Linking
            for (int i = 0; i < LinkedSpawners.Count; i++)
            {
                DetachLinkedSpawner(m_linkedSpawners[i]);
            }            

            // Do the same down the Resource tree
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Delete();
            }
        }

        /// <summary>
        /// Link a Child Spawner to this Resource.
        /// </summary>
        public void LinkChildSpawner(int childSpawnerIndex)
        {
            HashSet<int> set = new HashSet<int>(m_linkedSpawners);
            set.Add(childSpawnerIndex);
            m_linkedSpawners = new List<int>(set);
            Spawner.LinkChildSpawner(childSpawnerIndex);
        }

        /// <summary>
        /// Detach a Child Spawner to this Resource.
        /// </summary>
        public void DetachLinkedSpawner(int childSpawnerIndex)
        {
            m_linkedSpawners.Remove(childSpawnerIndex);
            Spawner.DetachChildSpawner(childSpawnerIndex);
        }

        /// <summary>
        /// Looks through the <seealso cref="Resource"/> tree and removes any stale links to Child Spawners (Checks if the indexes are in the Spawner's <seealso cref="m_linkedSpawners"/>.
        /// </summary>
        public bool RemoveStaleLinks()
        {
            bool removedAny = false;

            // Check for stale Spawner Links
            for (int i = 0; i < LinkedSpawners.Count; i++)
            {
                if (Spawner.IsChildLinked(m_linkedSpawners[i]) == false)
                {
                    m_linkedSpawners.Remove(m_linkedSpawners[i]);
                    removedAny = true;
                }
            }

            // Do the same down the Resource tree
            for (int i = 0; i < Children.Count; i++)
            {
                removedAny |= Children[i].RemoveStaleLinks();
            }

            return removedAny;
        }

        /// <summary>
        /// Add a brush texture to the Brush texture set.
        /// </summary>
        /// <param name="texture">Texture to be added.</param>
        public void AddBrushTexture(Texture2D texture)
        {
            bool emptySet = m_brushTextures == null || m_brushTextures.Count < 1;

            // Using a set to avoid duplications
            HashSet<Texture2D> set = emptySet ? new HashSet<Texture2D>() : new HashSet<Texture2D>(m_brushTextures);
            set.Add(texture);
            m_brushTextures = new List<Texture2D>(set);

            // Select it and update the texture if the set was empty
            if (emptySet || m_brushTXIndex < 0)
            {
                m_brushTXIndex = 0;
                UpdateBrushTexture();
            }
        }

        /// <summary>
        /// Remove a brush texture from the Brush texture set.
        /// </summary>
        /// <param name="index">Index of the texture to be removed.</param>
        public void RemoveBrushTexture(int index)
        {
            if (index < 0 || index > m_brushTextures.Count - 1)
            {
                Debug.LogWarningFormat("[GeNa] Can't remove Brush texture from the set. Index {0} in invalid for brushTextures[{1}]", index, m_brushTextures.Count);
                return;
            }

            m_brushTextures.RemoveAt(index);

            // Update the texture if removed the selected one.
            if (index == m_brushTXIndex)
            {
                UpdateBrushTexture();
            }
        }

        /// <summary>
        /// Clear the Brush texture set.
        /// </summary>
        public void ClearBrushTextures()
        {
            m_brushTextures.Clear();
            m_brushTXIndex = -1;
            UpdateBrushTexture();
        }

        /// <summary>
        /// Use when the brush texture needs updating.
        /// </summary>
        public void UpdateBrushTexture()
        {
            // Update the base brush
            if (m_brushTXIndex < 0 || m_brushTextures.Count <= m_brushTXIndex)
            {
                m_brushTXIndex = -1;
                m_baseBrush = new UBrush();
            }
            else
            {
                m_baseBrush = UBrush.GetBrush(m_brushTextures[m_brushTXIndex], (int)m_minScale.x);
            }

            // Clear the cache
            if (m_brushCache != null)
            {
                m_brushCache.Clear();
            }
            else
            {
                m_brushCache = new Dictionary<int, UBrush>();
            }
        }

        /// <summary>
        /// Add an extension that will be triggered by this Resource.
        /// </summary>
        /// <param name="extensionClass">The Extension class to be added.</param>
        public bool AddExtension(Type extensionClass)
        {
            Type[] interfaces = extensionClass.GetInterfaces();
            bool gxInterfaced = false;

            // Check if it implements the interface
            foreach (Type iface in interfaces)
            {
                if (iface == typeof(IGeNaExtension))
                {
                    gxInterfaced = true;
                    break;
                }
            }

            if (!gxInterfaced)
            {
                //We dont want to spawn spawners
                Debug.LogWarningFormat("<b>{0}</b> is not a valid GeNa Extension.", extensionClass.Name);
                return false;
            }

			// Make sure it's stateless
			if (extensionClass.IsSubclassOf(typeof(MonoBehaviour)))
			{
				//We dont want to spawn spawners
				Debug.LogWarningFormat("<b>{0}</b> is not a stateless GeNa Extension. Add it to a Prefab and try adding the prefab instead of the script itself.", extensionClass.Name);
				return false;
			}

            // Using a set to avoid duplications
            HashSet<Type> set = m_statelessExtensions == null ? new HashSet<Type>() : new HashSet<Type>(m_statelessExtensions);
            set.Add(extensionClass);
            m_statelessExtensions = new List<Type>(set).ToArray();

            // Let's make sure that the Protos are up to date
            Spawner.UpdateWhichProtosHaveExtensions();
            return true;
        }

        /// <summary>
        /// Add an extension that will be triggered by this Resource.
        /// </summary>
        /// <param name="extensionGO">Prefab/GameObject that has the Extension component to be added.</param>
        public bool AddExtension(GameObject extensionGO)
        {
            // Check if it has the component
            bool gxInterfaced = false;

			// Using a set to avoid duplications
			HashSet<IGeNaExtension> set = m_extensions == null ? new HashSet<IGeNaExtension>() : new HashSet<IGeNaExtension>(m_extensions);
			HashSet<GameObject> goSet = _extensions == null ? new HashSet<GameObject>() : new HashSet<GameObject>(_extensions);

			foreach (Component comp in extensionGO.GetComponents<Component>())
            {
                Type[] interfaces = comp.GetType().GetInterfaces();

                // Check if it implements the interface
                foreach (Type iface in interfaces)
                {
                    if (iface == typeof(IGeNaExtension))
                    {
                        gxInterfaced = true;
                        break;
                    }
                }

                if (gxInterfaced)
                {
					set.Add(comp as IGeNaExtension);
				}
			}

            if (!gxInterfaced)
            {
                //We dont want to spawn spawners
                Debug.LogWarningFormat("<b>{0}</b> does not have a valid GeNa Extension Component.", extensionGO.name);
                return false;
            }

			m_extensions = new List<IGeNaExtension>(set).ToArray();
			goSet.Add(extensionGO);
			_extensions = new List<GameObject>(goSet).ToArray();
			_hasStatefulExtensions = m_extensions.Length > 0;

			// Let's make sure that the Protos are up to date
			Spawner.UpdateWhichProtosHaveExtensions();
            return true;
        }

        /// <summary>
        /// Remove an extension so it's no longer triggered by this Resource.
        /// </summary>
        /// <param name="extension">Extension to be removed.</param>
        public void RemoveExtension(IGeNaExtension extension)
        {
			GameObject extensionGO = extension.gameObject;

			// Using a set to avoid duplications
			HashSet<GameObject> goSet = _extensions == null ? new HashSet<GameObject>() : new HashSet<GameObject>(_extensions);
			HashSet<IGeNaExtension> set = m_extensions == null ? new HashSet<IGeNaExtension>() : new HashSet<IGeNaExtension>(m_extensions);

			if (goSet.Contains(extensionGO) == false)
			{
				if (set.Contains(extension) == false)
				{
					Debug.LogWarningFormat("[GeNa] <i>Extension</i> '{0}' is not in the set of Resource '{1}' and can't be removed.", extension.Name, m_name);
					return;
				}

				Debug.LogWarningFormat("[GeNa] Prefab/GO of <i>Extension</i> '{0}' is not in the set of Resource '{1}' when removing.", extension.Name, m_name);
			}

			bool goHasGx = false;
			foreach (Component comp in extensionGO.GetComponents<Component>())
			{
				bool gxInterfaced = false;

				Type[] interfaces = comp.GetType().GetInterfaces();

				// Check if it implements the interface
				foreach (Type iface in interfaces)
				{
					if (iface == typeof(IGeNaExtension))
					{
						gxInterfaced = true;
						break;
					}
				}

				if (gxInterfaced)
				{
					goHasGx = true;

					if (set.Contains(extension) == false)
					{
						Debug.LogWarningFormat("[GeNa] <i>Extension</i> '{0}' is not in the set of Resource '{1}' and can't be removed.", extension.Name, m_name);
						continue;
					}
					set.Remove(comp as IGeNaExtension);
				}
			}

			if (!goHasGx)
			{
				//We dont want to spawn spawners
				Debug.LogErrorFormat("[GeNa] Prefab/GameObject '<b>{0}</b>' is disconnected from GeNa Extension '{1}'.", extensionGO.name, extension.Name);
			}

            m_extensions = new List<IGeNaExtension>(set).ToArray();

			goSet.Remove(extensionGO);
			_extensions = new List<GameObject>(goSet).ToArray();

			_hasStatefulExtensions = m_extensions.Length > 0;

			// Let's make sure that the Protos are up to date
			Spawner.UpdateWhichProtosHaveExtensions();
        }

        /// <summary>
        /// Remove an extension so it's no longer triggered by this Resource.
        /// </summary>
        /// <param name="index">Index of the extension to be removed.</param>
        public void RemoveExtension(int index)
        {
            if (index < 0 || m_extensions.Length - 1 < index)
            {
                Debug.LogWarningFormat("[GeNa] Can't remove <i>Extension</i> from the set of {0}. Index {1} is invalid for extensions[{2}]", m_name, index, m_brushTextures.Count);
                return;
            }

			RemoveExtension(m_extensions[index]);
        }

        /// <summary>
        /// Remove an extension so it's no longer triggered by this Resource.
        /// </summary>
        /// <param name="extension">Extension to be removed.</param>
        public void RemoveExtension(Type extension)
        {
            // Using a set to avoid duplications
            HashSet<Type> set = m_statelessExtensions == null ? new HashSet<Type>() : new HashSet<Type>(m_statelessExtensions);

            if (set.Contains(extension) == false)
            {
                Debug.LogWarningFormat("[GeNa] <i>Extension</i> {0} is not in the set of {1} and can't be removed.", extension.Name, m_name);
                return;
            }

            set.Remove(extension);
            m_statelessExtensions = new List<Type>(set).ToArray();

            // Let's make sure that the Protos are up to date
            Spawner.UpdateWhichProtosHaveExtensions();
        }

        /// <summary>
        /// Remove an extension so it's no longer triggered by this Resource.
        /// </summary>
        /// <param name="index">Index of the extension to be removed.</param>
        public void RemoveStatelessExtension(int index)
        {
            if (index < 0 || m_statelessExtensions.Length - 1 < index)
            {
                Debug.LogWarningFormat("[GeNa] Can't remove <i>Extension</i> from the set of {0}. Index {1} is invalid for extensions[{2}]", m_name, index, m_brushTextures.Count);
                return;
            }

            List<Type> list = new List<Type>(m_statelessExtensions);
            list.RemoveAt(index);
            m_statelessExtensions = list.ToArray();

            // Let's make sure that the Protos are up to date
            Spawner.UpdateWhichProtosHaveExtensions();
        }

        /// <summary>
        /// Remove all extensions so none is triggered by this Resource.
        /// </summary>
        public void ClearExtensions()
        {
            m_extensions = null;
			_extensions = null;
			_hasStatefulExtensions = false;

            m_statelessExtensions = null;

            // Let's make sure that the Protos are up to date
            Spawner.UpdateWhichProtosHaveExtensions();
        }

        /// <summary>
        /// Replace the prefab and handle recalculations for the Resource and its ancestors
        /// </summary>
        /// <param name="prefab"></param>
        public void ReplacePrefab(GameObject go)
        {
            ReplaceThePrefab(go);
        }

        /// <summary>
        /// Reset the Instance counters for this resource and its descendants
        /// </summary>
        public void ResetInstancesSpawned()
        {
            m_instancesSpawned = 0;

            if (HasChildren == false)
            {
                return;
            }

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].ResetInstancesSpawned();
            }
		}

		/// <summary>
		/// Takes care of any needed initialisations, including GeNa extensions.
		/// </summary>
		internal void Initialise(Spawner rootSpawner)
		{
			// Populate ExtensionInstances and Initialise any GeNa Extensions that may need it.
			List<ExtensionInstance> list = new List<ExtensionInstance>();

			if (Extensions != null)
			{
				for (int i = 0; i < Extensions.Length; i++)
				{
					if (Extensions[i] == null)
					{
						Debug.LogWarningFormat("[GeNa-{0}] GeNa Extension component at index " +
							"['<b>{1}</b>'] does not extist in the project (Resource '<b>{2}</b>'.",
							Spawner.name, i, m_name);
						continue;
					}

					// Make sure the Root Spawner is up to date on if the terrain will be affected, 
					// so it needs to record undo for it.
					rootSpawner.m_affectsHeight |= Extensions[i].AffectsHeights;
					rootSpawner.m_affectsTrees |= Extensions[i].AffectsTrees;
					rootSpawner.m_affectsGrass |= Extensions[i].AffectsDetails;
					rootSpawner.m_affectsTexture |= Extensions[i].AffectsTextures;

					// Initialise the extension and add it.
					Extensions[i].Init(Spawner);
					list.Add(new ExtensionInstance(this, false, i, Extensions[i]));
				}
			}

			if (StatelessExtensions != null)
			{
				for (int i = 0; i < StatelessExtensions.Length; i++)
				{
					IGeNaExtension extension = (IGeNaExtension)Activator.CreateInstance(StatelessExtensions[i]);

					if (extension == null)
					{
						Debug.LogWarningFormat("[GeNa-{0}] Unable to activate GeNa Extension script " +
							"'<b>{1}</b>' of Resource '<b>{2}</b>'. Does it exist in the project?",
							Spawner.name, StatelessExtensions[i].Name, m_name);
						continue;
					}

					// Make sure the Root Spawner is up to date on if the terrain will be affected, 
					// so it needs to record undo for it.
					rootSpawner.m_affectsHeight |= extension.AffectsHeights;
					rootSpawner.m_affectsTrees |= extension.AffectsTrees;
					rootSpawner.m_affectsGrass |= extension.AffectsDetails;
					rootSpawner.m_affectsTexture |= extension.AffectsTextures;

					// Initialise the extension and add it.
					extension.Init(Spawner);
					list.Add(new ExtensionInstance(this, true, i, extension));
				}
			}

			ExtensionInstances = list.ToArray();

			// Do the save for the children.
			for (int i = 0; i < Children.Count; i++)
			{
				Children[i].Initialise(rootSpawner);
			}
		}

		/// <summary>
		/// Recalculates bounds for the Resource and its ancestors
		/// </summary>
		public void RecalculateBounds()
        {
            //Need to instantiate this with all of its children with base settings to get the bounds.
            Bounds localBounds = GetInstantiatedBounds();
            m_baseSize = localBounds.size;
            m_boundsCenter = localBounds.center;

            if (Parent != null)
            {
                Parent.RecalculateBounds();
            }
        }

        /// <summary>
        /// Trigger resources in the tree to precalculate their offsets in preparation for getting extents for bounds checking and spawning.
        /// This is generally only called for children except for Legacy POI Prototypes.
        /// </summary>
        public void PrecalculateOffsets(XorshiftPlus randomGen, Vector3 spawnerScale, bool scaleToNearestInt)
        {
            //If static, sets the static values for this and children and returns
            if (PrecalculateStatic(randomGen, spawnerScale, scaleToNearestInt))
            {
                return;
            }

            // No need to calculate anything else if we are not spawning this branch any further
            if (!PrecalculateSuccess(randomGen))
            {
                return;
            }

            // If spawnerScale is one this is either not tope level, or being one we don't need to worry about it.
            if (spawnerScale == Vector3.one)
            {
                NextPosition = new Vector3(
                    randomGen.Next(m_minOffset.x, m_maxOffset.x),
                    randomGen.Next(m_minOffset.y, m_maxOffset.y),
                    randomGen.Next(m_minOffset.z, m_maxOffset.z));
            }
            else
            {
                NextPosition = new Vector3(
                    randomGen.Next(m_minOffset.x, m_maxOffset.x) * spawnerScale.x,
                    randomGen.Next(m_minOffset.y, m_maxOffset.y) * spawnerScale.y,
                    randomGen.Next(m_minOffset.z, m_maxOffset.z) * spawnerScale.z);
            }
            CalculateOffsets(randomGen, scaleToNearestInt);
        }

        /// <summary>
        /// Trigger resources in the tree to precalculate their offsets in preparation for getting extents for bounds checking and spawning.
        /// This is the overload for the top-level as it's called from the Prototype with the precalculated location offset.
        /// </summary>
        /// <param name="locationOffset">Override that sets the location offset to this value.</param>
        public void PrecalculateOffsets(XorshiftPlus randomGen, Vector3 spawnerScale, bool scaleToNearestInt, Vector3 locationOffset)
        {
            //If static, sets the static values for this and children and returns
            if (PrecalculateStatic(randomGen, spawnerScale, scaleToNearestInt))
            {
                return;
            }

            // No need to calculate anything else, if we are not spawning this branch any further
            if (!PrecalculateSuccess(randomGen))
            {
                return;
            }

            NextPosition = locationOffset;

            CalculateOffsets(randomGen, scaleToNearestInt);
        }

        /// <summary>
        /// Precalculate for static resources
        /// </summary>
        private bool PrecalculateStatic(XorshiftPlus randomGen, Vector3 spawnerScale, bool scaleToNearestInt)
        {
            //If static, set the static values
            if (Static == Constants.ResourceStatic.Static)
            {
                NextSuccess = true;
                NextPosition = new Vector3(m_basePosition.x * spawnerScale.x, m_basePosition.y * spawnerScale.y, m_basePosition.z * spawnerScale.z);
                NextRotation = m_baseRotation;

                if (scaleToNearestInt == true)
                {
                    NextScale = Spawner.ScaleToNearestInt(m_baseScale);
                }
                else
                {
                    NextScale = m_baseScale;
                }

                // Do the kids as well - it cost the same to do this here as doing during spawning and this is probably a better place for code cognitivity
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].PrecalculateOffsets(randomGen, Vector3.one, scaleToNearestInt);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Precalculate the success of spawning this Resource.
        /// </summary>
        private bool PrecalculateSuccess(XorshiftPlus randomGen)
        {
            NextSuccess = randomGen.Next() <= m_successRate;
            return NextSuccess;
        }

        /// <summary>
        /// Trigger resources in the tree to precalculate their offsets in preparation for getting extents for bounds checking and spawning.
        /// This is the overload for top level Resources where the spawnerScale applies.
        /// </summary>
        /// <param name="locationOffset">Override that sets the location offset to this value.</param>
        private void CalculateOffsets(XorshiftPlus randomGen, bool scaleToNearestInt)
        {
            // Scale offset
            if (m_sameScale)
            {
                float scale = randomGen.Next(m_minScale.x, m_maxScale.x);
                NextScale = new Vector3(scale, scale, scale);
            }
            else
            {
                NextScale = new Vector3(
                    randomGen.Next(m_minScale.x, m_maxScale.x),
                    randomGen.Next(m_minScale.y, m_maxScale.y),
                    randomGen.Next(m_minScale.z, m_maxScale.z));
            }

            if (scaleToNearestInt == true)
            {
                NextScale = Spawner.ScaleToNearestInt(NextScale);
            }

            // Rotation offset
            NextRotation = new Vector3(
                randomGen.Next(m_minRotation.x, m_maxRotation.x),
                randomGen.Next(m_minRotation.y, m_maxRotation.y),
                randomGen.Next(m_minRotation.z, m_maxRotation.z));

            // Do the kids as well
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].PrecalculateOffsets(randomGen, Vector3.one, scaleToNearestInt);
            }
        }

        /// <summary>
        /// Calculate bounds for this resource using the precalculated offsets and include it in the bounds provided.
        /// </summary>
        public void IncludeNextBounds(ref Bounds nextBounds, GTransform accumulativeTransform, bool topLevel)
        {
            if (!NextSuccess)
            {
                // This is where we stop
                return;
            }

            // Apply its transforms to the bounds coming up from children
            // For the top level y rotation and placement offset will be applied when casting the bounds and need to apply the spawner scale as well.
            Vector3 position = NextPosition;
            Vector3 scale = NextScale;
            Vector3 rotation = NextRotation;
            if (topLevel)
            {
                position = Vector3.zero;
                scale = Vector3.Scale(NextScale, accumulativeTransform.Scale);
                rotation.y = 0f;
            }

            // Get bounds of clidren - Keep traversing if Dynamic
            if (Static >= Constants.ResourceStatic.Dynamic)
            {
                Bounds childsBounds = new Bounds();

                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].IncludeNextBounds(ref childsBounds, accumulativeTransform, false);
                }

                if (childsBounds.size != Vector3.zero)
                {
                    // Scale
                    childsBounds.center = Vector3.Scale(childsBounds.center, scale);
                    childsBounds.size = Vector3.Scale(childsBounds.size, scale);

                    // Position and Rotation
                    childsBounds = GetRotatedBounds(childsBounds, Vector3.zero, rotation);
                    childsBounds.center += position;

                    Encapsulate(ref nextBounds, childsBounds);
                }

                // We are done if this is only a container - Only if dynamic. If static, this contains the bounds for the rest of the tree.
                if (ContainerOnly)
                {
                    //Nothing to see here
                    return;
                }
            }

            // Also done if this resource has no bounds on its own.
            if (m_baseSize == Vector3.zero)
            {
                //Nothing to see here
                return;
            }

            // Otherwise get the bounds for this
            Bounds bounds = GetBounds(position, scale, rotation);
            //The Spawner Placement Criteria scale was already applied when NextScale was calculated

            // and add it to the bounds
            Encapsulate(ref nextBounds, bounds);
        }

        /// <summary>
        /// Encapsulate one bounds into another.
        /// </summary>
        private static void Encapsulate(ref Bounds target, Bounds boundsToEncapsulate)
        {
            //If this is the first, set up the bounds
            if (target.size == Vector3.zero)
            {
                target = new Bounds(boundsToEncapsulate.center, boundsToEncapsulate.size);
            }
            else
            {
                target.Encapsulate(boundsToEncapsulate);
            }
        }

        /// <summary>
        /// Get bounds for this resource
        /// </summary>
        private Bounds GetBounds(Vector3 position, Vector3 scale, Vector3 rotation)
        {
            // Position
            Vector3 pos = position;

            // Size
            Vector3 size = m_baseSize;

            // Scale: by Collider or not
            if (m_hasColliders && m_useColliderBounds)
            {
                size = m_baseColliderScale;

                Vector3 offset = Vector3.zero;
                if (m_sameScale)
                {
                    offset = m_baseColliderCenter * scale.x;
                    size *= scale.x;
                }
                else
                {
                    offset = Vector3.Scale(m_baseColliderCenter, scale);
                    size = Vector3.Scale(size, scale);
                }

                if (m_baseColliderUseConstScale)
                {
                    size *= m_baseColliderConstScaleAmount;
                    offset *= m_baseColliderConstScaleAmount;
                }

                pos += offset;
            }
            else
            {
                if (m_sameScale)
                {
                    pos += m_boundsCenter * scale.x;
                    size *= scale.x;
                }
                else
                {
                    pos += Vector3.Scale(m_boundsCenter, scale);
                    size = Vector3.Scale(size, scale);
                }
            }

            Bounds bounds = new Bounds(pos, size);

            // Apoply its own rotation
            if (rotation != Vector3.zero)
            {
                bounds = GetRotatedBounds(bounds, position, rotation);
            }

            return bounds;
        }

        /// <summary>
        /// Calculate bounds for this resource for minimum bounds and include it in the bounds provided.
        /// </summary>
        public void IncludeInMinBounds(ref Bounds minBounds, GTransform accumulativeTransform, bool topLevel)
        {
            if (m_successRate <= 0f)
            {
                // This branch is off
                return;
            }

            // Apply its transforms to the bounds coming up from children
            // For the top level y rotation and placement offset will be applied when casting the bounds and need to apply the spawner scale as well.
            Vector3 position = GetClosestPos(Vector3.zero);
            Vector3 scale = new Vector3(
                Mathf.Clamp(0f, m_minScale.x, m_maxScale.x),
                Mathf.Clamp(0f, m_minScale.y, m_maxScale.y),
                Mathf.Clamp(0f, m_minScale.z, m_maxScale.z)
                );
            Vector3 rotation = new Vector3(
                Mathf.Clamp(0f, m_minRotation.x, m_maxRotation.x),
                Mathf.Clamp(0f, m_minRotation.y, m_maxRotation.y),
                Mathf.Clamp(0f, m_minRotation.z, m_maxRotation.z)
                );

            if (topLevel)
            {
                position = Vector3.zero;
                scale = Vector3.Scale(scale, accumulativeTransform.Scale);
                rotation.y = 0f;
            }

            // Get bounds of clidren - Keep traversing if Dynamic
            if (Static >= Constants.ResourceStatic.Dynamic)
            {
                Bounds childsBounds = new Bounds();

                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].IncludeInMinBounds(ref childsBounds, accumulativeTransform, false);
                }

                if (childsBounds.size != Vector3.zero)
                {
                    // Scale
                    childsBounds.center = Vector3.Scale(childsBounds.center, scale);
                    childsBounds.size = Vector3.Scale(childsBounds.size, scale);

                    // Position and Rotation
                    childsBounds = GetRotatedBounds(childsBounds, Vector3.zero, rotation);
                    childsBounds.center += position;

                    Encapsulate(ref minBounds, childsBounds);
                }

                // We are done if this is only a container - Only if dynamic. If static, this contains the bounds for the rest of the tree.
                if (ContainerOnly)
                {
                    //Nothing to see here
                    return;
                }
            }

            // Also done if this resource has no bounds on its own.
            if (m_baseSize == Vector3.zero)
            {
                //Nothing to see here
                return;
            }

            // Otherwise get the bounds for this
            Bounds bounds = GetBounds(position, scale, rotation);
            //The Spawner Placement Criteria scale was already applied when NextScale was calculated

            // and add it to the bounds
            Encapsulate(ref minBounds, bounds);
        }

        /// <summary>
        /// Get the closes position to a target within the offset range
        /// </summary>
        private Vector3 GetClosestPos(Vector3 target)
        {
            return new Vector3(
                Mathf.Clamp(target.x, m_minOffset.x, m_maxOffset.x),
                Mathf.Clamp(target.y, m_minOffset.y, m_maxOffset.y),
                Mathf.Clamp(target.z, m_minOffset.z, m_maxOffset.z)
                );
        }

        /// <summary>
        /// Get bounds for the bounds rotated.
        /// </summary>
        private Bounds GetRotatedBounds(Bounds bounds, Vector3 pivot, Vector3 rotation)
        {
            if (rotation == Vector3.zero)
            {
                return bounds;
            }

            Bounds newBounds = new Bounds(Spawner.RotatePointAroundPivot(bounds.min, pivot, rotation), Vector3.zero);
            newBounds.Encapsulate(Spawner.RotatePointAroundPivot(
                new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z), pivot, rotation));
            newBounds.Encapsulate(Spawner.RotatePointAroundPivot(
                new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z), pivot, rotation));
            newBounds.Encapsulate(Spawner.RotatePointAroundPivot(
                new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z), pivot, rotation));

            newBounds.Encapsulate(Spawner.RotatePointAroundPivot(
                new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z), pivot, rotation));
            newBounds.Encapsulate(Spawner.RotatePointAroundPivot(
                new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z), pivot, rotation));
            newBounds.Encapsulate(Spawner.RotatePointAroundPivot(
                new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z), pivot, rotation));
            newBounds.Encapsulate(Spawner.RotatePointAroundPivot(bounds.max, pivot, rotation));

            return newBounds;
        }


        #region Helper methods

        /// <summary>
        /// Return the bounds of an instantiated instance of the this resource, including its children, spawned with its base(ingested) object
        /// </summary>
        private Bounds GetInstantiatedBounds()
        {
            GameObject go = GetBaseInstantiated(null);

            Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(r.bounds);
            }
            foreach (Collider c in go.GetComponentsInChildren<Collider>())
            {
                bounds.Encapsulate(c.bounds);
            }

            UnityEngine.Object.DestroyImmediate(go);
            return bounds;
        }

        private GameObject GetBaseInstantiated(GameObject parent)
        {
            // Nothing to do here if not prefab
            if (m_resourceType != Constants.ResourceType.Prefab)
            {
                return null;
            }

            GameObject go = null;

            //If not an empty GO
            if (m_prefab != null)
            {
#if UNITY_EDITOR
                go = PrefabUtility.InstantiatePrefab(m_prefab) as GameObject;
#else
                go = UnityEngine.Object.Instantiate(m_prefab);
#endif
            }
            //Empty GO
            else
            {
                go = new GameObject(m_name);
            }

            //Add the node to its parent
            if (parent != null)
            {
                go.transform.parent = parent.transform;
            }

            // Set its transform
            go.transform.localPosition = m_basePosition;
            go.transform.localScale = m_baseScale;
            go.transform.localRotation = Quaternion.Euler(m_baseRotation);

            // If it has children traverse down the tree
            if (HasChildren)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].GetBaseInstantiated(go);
                }
            }

            return go;
        }
        #endregion

        #region Editor Only helper methods
#if UNITY_EDITOR
        /// <summary>
        /// Process prefab replacement
        /// </summary>
        private void ReplaceThePrefab(GameObject go)
        {
            m_name = go.name;

            //Get bounds
            Bounds localColliderBounds = Spawner.GetLocalColliderBounds(go);

            //Get colliders
            m_hasRootCollider = Spawner.HasRootCollider(go);
            m_hasColliders = Spawner.HasColliders(go);

            //Get meshes
            m_hasMeshes = Spawner.HasMeshes(go);

            //Get rigid body
            m_hasRigidBody = Spawner.HasRigidBody(go);

            //If top level resource
            if (Parent == null)
            {
                m_basePosition = Vector3.zero;
            }
            else
            {
                m_basePosition = go.transform.localPosition;
            }

            m_baseRotation = go.transform.localEulerAngles;
            m_baseScale = go.transform.localScale;
            m_baseColliderCenter = localColliderBounds.center;
            m_baseColliderScale = localColliderBounds.size;

            if (Spawner.ApproximatelyEqual(go.transform.localScale.x, go.transform.localScale.y, 0.000001f) && Spawner.ApproximatelyEqual(go.transform.localScale.x, go.transform.localScale.z, 0.000001f))
            {
                m_sameScale = true;
            }
            else
            {
                m_sameScale = false;
            }

            //We can only determine if it is a prefab in the editor
            if (Spawner.IsPrefab(go))
            {
#if UNITY_2018_3_OR_NEWER
                m_prefab = Spawner.GetPrefabAsset(go);
                if (m_prefab == null)
                {
                    m_prefab = go;
                }
#else
                if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
                {
                    res.m_prefab = GetPrefabAsset(go);
                }
                else
                {
                    res.m_prefab = go;
                }
#endif

                if (m_prefab != null)
                {
                    //Get its asset ID
                    string path = AssetDatabase.GetAssetPath(m_prefab);
                    if (!string.IsNullOrEmpty(path))
                    {
                        m_assetID = AssetDatabase.AssetPathToGUID(path);
                        m_assetName = Spawner.GetAssetName(path);
                    }

                    //Get flags
                    StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(m_prefab);
                    m_flagBatchingStatic = (flags & StaticEditorFlags.BatchingStatic) == StaticEditorFlags.BatchingStatic;
#if UNITY_5 || UNITY_2017 || UNITY_2018 || UNITY_2019_1
                    m_flagLightmapStatic = (flags & StaticEditorFlags.LightmapStatic) == StaticEditorFlags.LightmapStatic;
#else
                    m_flagLightmapStatic = (flags & StaticEditorFlags.ContributeGI) == StaticEditorFlags.ContributeGI;
#endif
                    m_flagNavigationStatic = (flags & StaticEditorFlags.NavigationStatic) == StaticEditorFlags.NavigationStatic;
                    m_flagOccludeeStatic = (flags & StaticEditorFlags.OccludeeStatic) == StaticEditorFlags.OccludeeStatic;
                    m_flagOccluderStatic = (flags & StaticEditorFlags.OccluderStatic) == StaticEditorFlags.OccluderStatic;
                    m_flagOffMeshLinkGeneration = (flags & StaticEditorFlags.OffMeshLinkGeneration) == StaticEditorFlags.OffMeshLinkGeneration;
                    m_flagReflectionProbeStatic = (flags & StaticEditorFlags.ReflectionProbeStatic) == StaticEditorFlags.ReflectionProbeStatic;
                }
                else
                {
                    Debug.LogErrorFormat("Unable to get prefab for '{0}'", m_name);
                }
            }
            //Else this is just a GO (container in the tree) not a prefab.
            else
            {
                ContainerOnly = true;

                // Warn the user if it has more components than just the Transform since it's not a prefab.
                Component[] components = go.GetComponents<Component>();
                if (components != null && components.Length > 1)
                {
                    Debug.LogWarningFormat("[GeNa]: Warning! Gameobject '{0}' has Components but it's not a Prefab Instance. Make it into a Prefab if you wish to keep its Components information for spawning.",
                        go.name);
                }
            }
            RecalculateBounds();
        }
#endif

        #endregion

        #region Debug

#if GENA_DEBUG && UNITY_EDITOR
        /// <summary>
        /// Debug the bounds
        /// </summary>
        public static void DebugBounds(Vector3 center, Vector3 extents, float duration = 12f)
        {
            DebugBounds(center, extents, Quaternion.identity, Color.blue, duration);
        }

        /// <summary>
        /// Debug the bounds
        /// </summary>
        public static void DebugBounds(Vector3 center, Vector3 extents, Vector3 rotation, float duration = 12f)
        {
            DebugBounds(center, extents, Quaternion.Euler(rotation), Color.blue, duration);
        }

        /// <summary>
        /// Debug the bounds
        /// </summary>
        public static void DebugBounds(Vector3 center, Vector3 extents, Color color, float duration = 12f)
        {
            DebugBounds(center, extents, Quaternion.identity, color, duration);
        }

        /// <summary>
        /// Debug the bounds
        /// </summary>
        public static void DebugBounds(Vector3 center, Vector3 extents, Vector3 rotation, Color color, float duration = 12f)
        {
            DebugBounds(new Bounds(center, extents * 2f), Quaternion.Euler(rotation), color, duration);
        }

        /// <summary>
        /// Debug the bounds
        /// </summary>
        public static void DebugBounds(Bounds bounds, float duration = 12f)
        {
            DebugBounds(bounds, Color.blue, duration);
        }

        /// <summary>
        /// Debug the bounds
        /// </summary>
        public static void DebugBounds(Bounds bounds, Vector3 rotation, float duration = 12f)
        {
            DebugBounds(bounds, Quaternion.Euler(rotation), Color.blue, duration);
        }

        /// <summary>
        /// Debug the bounds
        /// </summary>
        public static void DebugBounds(Bounds bounds, Color color, float duration = 12f)
        {
            DebugBounds(bounds, Quaternion.identity, color, duration);
        }

        /// <summary>
        /// Debug the bounds
        /// </summary>
        public static void DebugBounds(Vector3 center, Vector3 extents, Quaternion rotation, float duration = 12f)
        {
            DebugBounds(center, extents, rotation, Color.blue, duration);
        }

        /// <summary>
        /// Debug the bounds
        /// </summary>
        public static void DebugBounds(Vector3 center, Vector3 extents, Quaternion rotation, Color color, float duration = 12f)
        {
            DebugBounds(new Bounds(center, extents * 2f), rotation, color, duration);
        }

        /// <summary>
        /// Debug the bounds
        /// </summary>
        public static void DebugBounds(Bounds bounds, Quaternion rotation, float duration = 12f)
        {
            DebugBounds(bounds, rotation, Color.blue, duration);
        }

        /// <summary>
        /// Debug the bounds
        /// </summary>
        public static void DebugBounds(Bounds bounds, Quaternion rotation, Color color, float duration = 12f)
        {
            Vector3[] corners = new Vector3[8];
            if (rotation == Quaternion.identity)
            {
                corners[0] = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z);
                corners[1] = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z);
                corners[2] = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z);
                corners[3] = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z);

                corners[4] = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z);
                corners[5] = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z);
                corners[6] = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z);
                corners[7] = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z);
            }
            else
            {
                corners[0] = Spawner.RotatePointAroundPivot(
                    new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z),
                    bounds.center, rotation);
                corners[1] = Spawner.RotatePointAroundPivot(
                    new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z),
                    bounds.center, rotation);
                corners[2] = Spawner.RotatePointAroundPivot(
                    new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z),
                    bounds.center, rotation);
                corners[3] = Spawner.RotatePointAroundPivot(
                    new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z),
                    bounds.center, rotation);

                corners[4] = Spawner.RotatePointAroundPivot(
                    new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z),
                    bounds.center, rotation);
                corners[5] = Spawner.RotatePointAroundPivot(
                    new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z),
                    bounds.center, rotation);
                corners[6] = Spawner.RotatePointAroundPivot(
                    new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z),
                    bounds.center, rotation);
                corners[7] = Spawner.RotatePointAroundPivot(
                    new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z),
                    bounds.center, rotation);
            }

            for (int i = 0; i < 4; i++)
            {
                Debug.DrawLine(corners[i % 4], corners[(i + 1) % 4], color, duration);
                Debug.DrawLine(corners[i], corners[4 + i], color, duration);
                Debug.DrawLine(corners[4 + i % 4], corners[4 + (i + 1) % 4], color, duration);
            }
        }

        /// <summary>
        /// Debug Pivot
        /// </summary>
        private static void DebugPivot(Vector3 pivot, float halfSize = 0.3f, float duration = 12f)
        {
            DebugPivot(pivot, Color.green, halfSize, duration);
        }

        /// <summary>
        /// Debug Pivot
        /// </summary>
        private static void DebugPivot(Vector3 pivot, Color color, float halfSize = 0.3f, float duration = 12f)
        {
            Debug.DrawLine(new Vector3(pivot.x - halfSize, pivot.y, pivot.z), new Vector3(pivot.x + halfSize, pivot.y, pivot.z), color, duration);
            Debug.DrawLine(new Vector3(pivot.x, pivot.y - halfSize, pivot.z), new Vector3(pivot.x, pivot.y + halfSize, pivot.z), color, duration);
            Debug.DrawLine(new Vector3(pivot.x, pivot.y, pivot.z - halfSize), new Vector3(pivot.x, pivot.y, pivot.z + halfSize), color, duration);
        }
#endif

        #endregion

        #region Serialization

        /// <summary>
        /// Custom serialisation
        /// </summary>
        public void OnBeforeSerialize()
		{
			// Store a string representation for the extension types.
			// Not accomodating generic and other types as it's probably unecessary.
			if (m_statelessExtensions != null)
			{
				_statelessExtensions = new string[m_statelessExtensions.Length];
				for (int i = 0; i < m_statelessExtensions.Length; i++)
				{
					_statelessExtensions[i] = m_statelessExtensions[i].AssemblyQualifiedName;
				}
			}
        }

        /// <summary>
        /// Custom serialisation
        /// </summary>
        public void OnAfterDeserialize()
        {
            // Get the extensin types by the stored strings.
            if (_statelessExtensions != null)
			{
				List<Type> list = new List<Type>();

				foreach (string typeName in _statelessExtensions)
				{
					Type type = Type.GetType(typeName);
					if (type == null)
					{
						Debug.LogWarningFormat("[GeNa] GeNa Extension type '<b>{0}</b>' cannot be found in " +
							"the project and will be dropped from Resource {1}.", typeName, m_name);
						continue;
					}

					list.Add(type);
				}

				m_statelessExtensions = list.ToArray();
			}
        }

        #endregion
    }
}
