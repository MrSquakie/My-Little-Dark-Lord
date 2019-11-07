namespace GeNa
{
    /// <summary>
    /// GeNa Constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The type of the spawner. The new "Structure" mode is used for biomes, building structures, etc.
        /// </summary>
        public enum SpawnerType { Regular, Structured }

        /// <summary>
        /// Determines the way Child Spawners get spawned
        /// </summary>
        public enum ChildSpawnMode { All, Random }

        /// <summary>
        /// The spawn mode that is used when using CTRL + Left Mouse Button
        /// </summary>
        public enum SpawnMode { Single, Paint/*, Spline*/ }

        /// <summary>
        /// The type of shape that is used to check the spawn range
        /// </summary>
        public enum SpawnRangeShape { Circle, Square }

        /// <summary>
        /// The type of spawner location algorithm
        /// </summary>
        public enum LocationAlgorithm { Centered, Every, LastSpawn, Organic }

        /// <summary>
        /// The type of spawner rotation algorithm
        /// </summary>
        public enum RotationAlgorithm { Ranged, Fixed, LastSpawnCenter, LastSpawnClosest }

        /// <summary>
        /// The check to use for virgin space
        /// </summary>
        public enum VirginCheckType { None, Point, Bounds }

        /// <summary>
        /// Check that can use a range, min + max, and both (the more strict prevails)
        /// </summary>
        public enum CriteriaRangeType { None, Range, MinMax, Mixed }

        /// <summary>
        /// The type of spawn component
        /// </summary>
        public enum ResourceType {  Prefab, TerrainTree, TerrainGrass, TerrainTexture }

        /// <summary>
        /// The type of mask to assign to the spawn
        /// </summary>
        public enum MaskType { Perlin, Billow, Ridged, Image }

        /// <summary>
        /// Distance to search before instantiating new probegroup
        /// </summary>
        public const float MinimimProbeGroupDistance = 100f;

        /// <summary>
        /// Minimum distance before instantiating new probes
        /// </summary>
        public const float MinimimProbeDistance = 15f;

        /// <summary>
        /// Wont opmimise stuff if larger than this
        /// </summary>
        public const float MaximumOptimisationSize = 10f;

        /// <summary>
        /// The minimum size of texture brushes in pixels
        /// </summary>
        public const float MIN_TX_BRUSH_SIZE_IN_PIX = 1f;

        /// <summary>
        /// Vegetation type for AVS
        /// </summary>
        public enum AVSVegetationType
        {
            Grass = 0,
            Plant = 1,
            Tree = 2,
            Objects = 3,
            LargeObjects = 4
        }

        /// <summary>
        /// Static setting of Resources. Using enum so we can later add other options e.g. static except success
        /// </summary>
        public enum ResourceStatic {
            Static = 0,
            Dynamic = 1
        }
	}
}
