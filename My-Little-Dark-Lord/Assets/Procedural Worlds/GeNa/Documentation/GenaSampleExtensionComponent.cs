using UnityEngine;
using GeNa;

public class GenaSampleExtensionComponent : MonoBehaviour, IGeNaExtension
{
	[SerializeField] private int m_numOne = 1;
	[SerializeField] private int m_numTwo = 2;
	[SerializeField] private int m_numThree = 3;

	/// <summary>
	/// The name to identify the Extension; Recommended naming: Stateless: "GetType().Name";
	/// Stateful: "string.Format("{0}.{1}", name, GetType().Name)".
	/// </summary>
	public string Name { get { return string.Format("{0}.{1}", name, GetType().Name); } }

	/// <summary>
	/// Whether or not this extension impacts Terrain heights.
	/// GeNa will handle undoing these actions to avoid undo conflicts.
	/// </summary>
	public bool AffectsHeights { get { return false; } }

	/// <summary>
	/// Whether or not this extension impacts Terrain textures.
	/// GeNa will handle undoing these actions to avoid undo conflicts.
	/// </summary>
	public bool AffectsTextures { get { return false; } }

	/// <summary>
	/// Whether or not this extension impacts Terrain details.
	/// GeNa will handle undoing these actions to avoid undo conflicts.
	/// </summary>
	public bool AffectsDetails { get { return false; } }

	/// <summary>
	/// Whether or not this extension impacts Terrain trees.
	/// GeNa will handle undoing these actions to avoid undo conflicts.
	/// </summary>
	public bool AffectsTrees { get { return false; } }

	/// <summary>
	/// Initialise the extension - To avoid some actions to happen at every instance of this 
	/// Extension spawning.
	/// </summary>
	public void Init(Spawner spawner)
	{
		Debug.LogFormat("{0} is initialised.", Name);
	}

	/// <summary>
	/// This will be called with details when spawning.
	/// </summary>
	/// <param name="spawner">The spawner that is spawning this.</param>
	/// <param name="resource">The resource that is spawning this.</param>
	/// <param name="spawnDetails">Spawn details of the spawn.</param>
	public void Spawn(Spawner spawner, Resource resource, SpawnDetails spawnDetails)
	{
		Debug.LogFormat("{0} Spawned. ->\n" +
			"Spawner: {1}\n" +
			"Resource: {2}\n" +
			"Details: {3}\n" +
			"and we got numbers: {4} {5} {6}", Name, spawner.name, resource.m_name, spawnDetails
			, m_numOne, m_numTwo, m_numThree);
    }

	/// <summary>
	/// This will be called when the Spawner is doing undo.
	/// </summary>
	public void Undo()
	{
		Debug.LogFormat("{0} processed its own Undo.", Name);
	}
}
