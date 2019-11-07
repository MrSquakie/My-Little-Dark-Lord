using System;
using UnityEngine;

namespace GeNa
{
    /// <summary>
    /// Handy little class to detect changes to the terrain object. Add this script onto the terrain.
    /// </summary>
    [ExecuteInEditMode]
    public class TerrainEvents : MonoBehaviour
    {

        [Flags]
        internal enum TerrainChangedFlags
        {
            NoChange = 0,
            Heightmap = 1,
            TreeInstances = 2,
            DelayedHeightmapUpdate = 4,
            FlushEverythingImmediately = 8,
            RemoveDirtyDetailsImmediately = 16,
            WillBeDestroyed = 256,
        }

        void OnTerrainChanged(int flags)
        {
            Debug.Log((TerrainChangedFlags)flags);
        }
    }
}
