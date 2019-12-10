#define GAIA_PRESENT

using UnityEngine;
using System.Collections;

namespace Gaia
{
    public static class GaiaConstants
    {
        /// <summary>
        /// Where scanned assets will be loaded from and saved to
        /// </summary>
        //public static readonly string AssetDir = "Gaia/Stamps";
        //public static readonly string AssetDirFromAssetDB = "Assets/Gaia/Stamps";

        /// <summary>
        /// Defines the type of lighting is suppose to be
        /// </summary>
        public enum GaiaLightingProfileType { Morning, Day, Evening, Night, Default, None }

        /// <summary>
        /// The supported Anti aliasing modes
        /// </summary>
        public enum GaiaProAntiAliasingMode { None, FXAA, MSAA, SMAA, TAA }

        /// <summary>
        /// The water mesh quality
        /// </summary>
        public enum WaterMeshQuality { VeryLow, Low, Medium, High, VeryHigh, Ultra, Cinematic, Custom }

        public enum HDSkyType { Gradient, HDRI, Procedural }

        public enum HDAmbientMode { Static, Dynamic }

        public enum HDFogType { None, Exponential, Linear, Volumetric }

        public enum HDFogType2019_3 { None, Volumetric }

        public enum HDSkyUpdateMode { OnChanged, OnDemand, Realtime }

        /// <summary>
        /// Sets the supported water resolution
        /// </summary>
        public enum GaiaProWaterReflectionsQuality { Resolution8, Resolution16, Resolution32, Resolution64, Resolution128, Resolution256, Resolution512, Resolution1024, Resolution2048, Resolution4096, Resolution8192 }

        /// <summary>
        /// Defines what type of water is suppose to be
        /// </summary>
        public enum GaiaWaterProfileType { DeepBlueOcean, ClearBlueOcean, StandardLake, StandardClearLake, None }

        /// <summary>
        /// Sets the wind type on how strong it is rendered in the scene
        /// </summary>
        public enum GaiaWindType { Calm, Moderate, Strong, None }

        public enum GaiaGlobalWindType { Calm, Moderate, Strong, None, Custom }

        /// <summary>
        /// Byte orders the scanner uses to load raw files.
        /// </summary>
        public enum RawByteOrder { IBM, Macintosh }

        /// <summary>
        /// Bit depths the scanner uses to load raw files.
        /// </summary>
        public enum RawBitDepth { Sixteen, Eight }

        /// <summary>
        /// The type of thing we are targeting
        /// </summary>
        public enum EnvironmentTarget { UltraLight, MobileAndVR, Desktop, PowerfulDesktop, Custom }

        /// <summary>
        /// The type of thing we are targeting
        /// </summary>
        public enum EnvironmentRenderer { BuiltIn, Lightweight, HighDefinition }


        /// <summary>
        /// Preset Environment Sizes
        /// </summary>
        public enum EnvironmentSizePreset { Tiny, Small, Medium, Large, XLarge, Custom }


        /// <summary>
        /// The size of the environment we are targeting
        /// </summary>
        public enum EnvironmentSize { Is256MetersSq, Is512MetersSq, Is1024MetersSq, Is2048MetersSq, Is4096MetersSq, Is8192MetersSq, Is16384MetersSq }

        /// <summary>
        /// The heightmap resolution per terrain chunk 
        /// </summary>
        public enum HeightmapResolution { _33 = 33, _65 = 65, _129 = 129, _257 = 257, _513 = 513, _1025 = 1025, _2049 = 2049, _4097 = 4097 }

        /// <summary>
        /// The texture resolution per terrain chunk (This enum is used both for control and base texture since they allow the same values)
        /// </summary>
        public enum TerrainTextureResolution { _16 = 16, _32 = 32, _64 = 64, _128 = 128, _256 = 256, _512 = 512, _1024 = 1024, _2048 = 2048 }

        /// <summary>
        /// The different Ambient Skies samples
        /// </summary>
        public enum Skies { Day, Morning, Evening, Night, None }

        /// <summary>
        /// The different Ambient Water samples
        /// </summary>
        public enum Water { DeepBlue, ClearBlue, ToxicGreen, Cyan, None }


        /// <summary>
        /// The different modes for Creating Post Processing Volumes when spawning - does the user want to create a new one for each spawn
        // or do they want the existing one replaced
        /// </summary>
        public enum BiomePostProcessingVolumeSpawnMode { Add, Replace }

        /// <summary>
        /// The different Post FX settings
        /// </summary>
        public enum PostFX { Day, Morning, Evening, Night, None }

        ///// <summary>
        ///// The Material pass selected for the stamping operation
        ///// </summary>
        //public enum MaterialPass
        //{
        //    RaiseHeight = 0,
        //    LowerHeight = 1,
        //    BlendHeight = 2,
        //    StencilHeight = 3,
        //    DifferenceHeight = 4
        //}
        /// <summary>
        /// Control type - from std assets
        /// </summary>
        public enum EnvironmentControllerType { FirstPerson, FlyingCamera, ThirdPerson, None }

        /// <summary>
        /// Operational mode of the manager
        /// </summary>
        public enum ManagerEditorMode { Standard, Advanced, Extensions, ShowMore }

        /// <summary>
        /// Operational mode of the manager
        /// </summary>
        public enum ManagerEditorNewsMode { MoreOnGaia, MoreOnProceduralWorlds }

        /// <summary>
        /// Operational mode of the spawner component
        /// </summary>
        public enum OperationMode { DesignTime, RuntimeInterval, RuntimeTriggeredInterval }

        /// <summary>
        /// Type of terrain operation
        /// </summary>
        public enum TerrainOperationType { AddToTerrain, ApplyMaskToSplatmap, ContrastFilter, GrowFeaturesFilter, DeNoiseFilter, HydraulicFilter, MultiplyTerrain, PowerOfFilter, QuantizeFilter, QuantizeCurvesFilter, SetTerrainToHeight, ShrinkFeaturesFilter, SubtractFromTerrain, ThermalFilter, ExportAspectMap, ExportBaseMap, ExportCurvatureMap, ExportFlowMap, ExportHeightMap, ExportNoiseMap, ExportNormalMap, ExportMasks, ExportSlopeMap }

        /// <summary>
        /// Type of mask merge operation
        /// </summary>
        public enum MaskMergeType { AssignMask2IfGreaterThan, AssignMask2IfLessThan, AddMask2, MultiplyByMask2, SubtractMask2 }

        /// <summary>
        /// Type of rain map
        /// </summary>
        public enum ErosionRainType { Constant, ErodePeaks, ErodeValleys, ErodeSlopes }

        /// <summary>
        /// Type of curvature to calculate
        /// </summary>
        public enum CurvatureType { Average, Horizontal, Vertical }

        /// <summary>
        /// Type of aspect to calculate
        /// </summary>
        public enum AspectType { Aspect, Northerness, Easterness }

        /// <summary>
        /// Type of noise being generated
        /// </summary>
        public enum NoiseType { None, Perlin, Billow, Ridged }

        /// <summary>
        /// The fitness filter mode to apply when a texture is used to filter on fitness
        /// </summary>
        public enum ImageFitnessFilterMode { None, ImageGreyScale, ImageRedChannel, ImageGreenChannel, ImageBlueChannel, ImageAlphaChannel, TerrainTexture0, TerrainTexture1, TerrainTexture2, TerrainTexture3, TerrainTexture4, TerrainTexture5, TerrainTexture6, TerrainTexture7, PerlinNoise, BillowNoise, RidgedNoise }

        /// <summary>
        /// Classification of feature types - also used to load and save features
        /// </summary>
        public enum FeatureType { Hills, Islands, Lakes, Mesas, Mountains, Plains, Rivers, Rocks, Valleys, Waterfalls };

        /// <summary>
        /// Types of borders to put on generated terrain
        /// </summary>
        public enum GeneratorBorderStyle { None, Mountains, Water }

        /// <summary>
        /// Different operation modes for the spawner - depending on what the spawner is doing, it will create different paint contexts for textures, heightmaps, etc.
        /// </summary>
        public enum SpawnerApplyBrushMode { Preview, Texture, TerrainDetail,
            TerrainTree,
            JustCacheContext
        }

        /// <summary>
        /// Obscure preset color, used as a flag to detect that the user has never set a custom color for themselves
        /// </summary>
        public static readonly Color spawnerInitColor = new Color(1f, 0.9849656f, 0.7688679f, 0f);

        /// <summary>
        /// Determines whether the spawns should replace the exisiting instances, added to existing, or remove existing instances.
        /// </summary>
        public enum SpawnMode { Replace, Add, Remove };

        /// <summary>
        /// The type of feature operation that will be used on the terrain by the stamper
        /// AddHeight - Will add height is stamped object exceeds height at that location
        /// RemoveHeight - Will remove height if the terrain is higher than stamp at that location
        /// Blend Height - Will blend the height difference from the existing terrain towards the stamp
        /// Set Height - Will set the terrain height at the given position 1:1 as it is in the stamp, no ifs and buts
        /// Add Height - Will add the stamp height on top of the existing terrain features
        /// Subtract Height - Will remove the stamp height from the existing terrain features
        /// Contrast - Will bring scale up height differences in the terrain to bring out details
        /// Hydraulic Erosion - Will simulate removing sediment from the terrain caused by rainfall over time
        /// </summary>
        public enum FeatureOperation { RaiseHeight, LowerHeight, BlendHeight, SetHeight, AddHeight, SubtractHeight, Contrast, SharpenRidges, Terrace, HydraulicErosion, HeightTransform, PowerOf, Smooth };

        /// <summary>
        /// The official UI names for the different Feature Operations, and their position in the dropdown menu. The order of this List must match with the order
        /// of the enum "FeatureOperation"
        /// </summary>
        public static string[] FeatureOperationNames = {    "Stamping/Raise Height",
                                                            "Stamping/Lower Height",
                                                            "Stamping/Blend Height",
                                                            "Stamping/Set Height",
                                                            "Stamping/Add Height",
                                                            "Stamping/Subtract Height",
                                                            "Effects/Contrast",
                                                            "Effects/Sharpen Ridges",
                                                            "Effects/Terraces",
                                                            "Erosion/HydraulicErosion",
                                                            "Effects/HeightTransform",
                                                            "Effects/PowerOf",
                                                            "Effects/Smooth"};
                    
                                                            

        /// <summary>
        /// Defines which water visualisation we want to display - the real, actual water (if available), the simple blue plane or none.
        /// </summary>
        public enum PreferredWaterVisualisation { ActualWater, Plane, None}


        /// <summary>
        /// Defines what the distance and area mask in the stamper can influence : Either the mask is applied to the stamp result as a whole,
        /// or the mask is applied only to the stamp itself. 
        /// </summary>
        public enum MaskInfluence { OnlyStampItself, TotalSpawnResult }

        /// <summary>
        /// The different output types for the erosion image mask
        /// </summary>
        public enum ErosionMaskOutput { Sediment, WaterVelocity, WaterFlux }

        /// <summary>
        /// The shape of the spawner
        /// </summary>
        public enum SpawnerShape { Box, Sphere }

        /// <summary>
        /// The algorithm used to choose spawn locations
        /// RandomLocation - A random location will be chosen
        /// RandomLocationSeeded - A random start location will be chose, the next location will be based in seed throw radius
        /// EveryLocation - Every location in the spawner area will be checked
        /// EveryLocationJittered - Every location in the spawner area will be checked, and jitter added to break up the lines
        /// </summary>
        public enum SpawnerLocation { RandomLocation, RandomLocationClustered, EveryLocation, EveryLocationJittered }

        /// <summary>
        /// The algorithm used at spawn locations to determine suitability
        /// PointCheck - Just that location is checked
        /// BoundedAreaCheck - The entire area is checked - slower but good for large object placement
        /// </summary>
        public enum SpawnerLocationCheckType { PointCheck, BoundedAreaCheck }

        /// <summary>
        /// The algorithm which defines which rule will be selected in spawn location. Regardless of whether
        ///   selected or not, the rule will still only run if it exceeds its minimum fitness settings, and 
        ///   didnt randomly fail (failure rate).
        /// All - All rules will be run at this location
        /// Fittest - Only the fittest rule will be run at this location
        /// WeightedFittest - The fittest rule will most likely run, then the next fittest, distributed by relative fitness
        /// Random - A random rule will be chosen and evaluated to run
        /// </summary>
        public enum SpawnerRuleSelector { All, Fittest, WeightedFittest, Random }

        /// <summary>
        /// The type of resource that the spawner will use in a spawn rule
        /// </summary>
        public enum SpawnerResourceType { TerrainTexture, TerrainDetail, TerrainTree, GameObject //,GeNaSpawner
        }

        /// <summary>
        /// Used by system determine what constitutes a valid virgin terrain height check threshold
        /// </summary>
        public static float VirginTerrainCheckThreshold = 0.01f;

        /// <summary>
        /// Determines how many textures can be previewed at the same time in the spawner
        /// </summary>
        public static int maxPreviewedTextures = 5;

        /// <summary>
        /// Name of the standard GameObject spawn transform under which new game objects will be created from the spawner
        /// </summary>
        public static string defaultGOSpawnTarget ="Gaia Game Object Spawns";

        /// <summary>
        /// File type to use when saving images
        /// </summary>
        public enum ImageFileType {  Jpg, Png, Exr }

        /// <summary>
        /// Image formatting defaults
        /// </summary>
        public const TextureFormat defaultTextureFormat = TextureFormat.RGBA32;
        public const TextureFormat fmtHmTextureFormat = TextureFormat.RGBA32;
        public const TextureFormat fmtRGBA32 = TextureFormat.RGBA32;

        /// <summary>
        /// Storage formats
        /// </summary>
        public enum StorageFormat { PNG, JPG }
        public const StorageFormat defaultImageStorageFormat = StorageFormat.PNG;

        /// <summary>
        /// Image channels
        /// </summary>
        public enum ImageChannel { R, G, B, A }
        public const ImageChannel defaultImageStorageChannel = ImageChannel.R;

    }
}

