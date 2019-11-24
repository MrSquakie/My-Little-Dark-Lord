using UnityEngine;

namespace GeNa
{
    /// <summary>
    /// Derive from this class to implement a GeNa extension that can be attached to Resources
    /// </summary>
    public interface INamedType
    {
		
		
		/// <summary>
		/// The name of the Prefab/GameObject the script is attached to, or the name of the class if it's not attached to anything.
		/// </summary>
		string Name { get; }
    }
}
