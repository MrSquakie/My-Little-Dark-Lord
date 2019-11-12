using System;
using UnityEngine;

namespace GeNa
{
    /// <summary>
    /// Struct holding information and the instance of an Extension
    /// </summary>
	[Serializable]
    internal struct ExtensionInstance
	{
		/// <summary>
		/// The Resource the Extension is attached to.
		/// </summary>
		public Resource Parent;

		/// <summary>
		/// Is this a stateless Extension (this determines where it is stored in the parent Resource)?
		/// </summary>
		public bool IsStateless;

		/// <summary>
		/// The index of this Extension in its parent Resource's appropriate list.
		/// </summary>
		public int Index;

		/// <summary>
		/// The actual instance of the Extension. (doesn't get serialised - can be acquired from its parent resource)
		/// </summary>
		[NonSerialized] public IGeNaExtension Instance;

		public ExtensionInstance(Resource parentRes, bool isStateless, int index, IGeNaExtension instance)
		{
			Parent = parentRes;
			IsStateless = isStateless;
			Index = index;
			Instance = instance;
		}
	}
}
