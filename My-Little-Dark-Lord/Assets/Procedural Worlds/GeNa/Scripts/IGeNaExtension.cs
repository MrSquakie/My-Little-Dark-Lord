using UnityEngine;

namespace GeNa
{
    /// <summary>
    /// Use this interface to implement a GeNa extension that can be attached to Resources.
    /// </summary>
    public interface IGeNaExtension
    {
		/// <summary>
		/// The name to identify the Extension; Recommended naming: Stateless: "GetType().Name";
		/// Stateful: "string.Format("{0}.{1}", name, GetType().Name)".
		/// </summary>
		string Name { get; }

		/// <summary>
		/// This is automatically used by stateful extensions. Stateless should just return null.
		/// </summary>
		GameObject gameObject { get; }

		/// <summary>
		/// Whether or not this extension impacts Terrain heights.
		/// GeNa will handle undoing these actions to avoid undo conflicts.
		/// </summary>
		bool AffectsHeights { get; }

		/// <summary>
		/// Whether or not this extension impacts Terrain textures.
		/// GeNa will handle undoing these actions to avoid undo conflicts.
		/// </summary>
		bool AffectsTextures { get; }

		/// <summary>
		/// Whether or not this extension impacts Terrain details.
		/// GeNa will handle undoing these actions to avoid undo conflicts.
		/// </summary>
		bool AffectsDetails { get; }

		/// <summary>
		/// Whether or not this extension impacts Terrain trees.
		/// GeNa will handle undoing these actions to avoid undo conflicts.
		/// </summary>
		bool AffectsTrees { get; }

		/// <summary>
		/// Initialise the extension - To avoid some actions to happen at every instance of this 
		/// Extension spawning.
		/// </summary>
		void Init(Spawner spawner);

		/// <summary>
		/// This will be called with details when spawning.
		/// </summary>
		/// <param name="spawner">The spawner that is spawning this.</param>
		/// <param name="resource">The resource that is spawning this.</param>
		/// <param name="spawnDetails">Spawn details of the spawn.</param>
		void Spawn(Spawner spawner, Resource resource, SpawnDetails spawnDetails);

        /// <summary>
        /// This will be called when the Spawner is doing undo.
        /// </summary>
        void Undo();
    }
}
