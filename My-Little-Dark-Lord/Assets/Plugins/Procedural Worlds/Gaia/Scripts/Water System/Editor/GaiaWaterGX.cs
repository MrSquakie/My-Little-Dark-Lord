using UnityEditor;

namespace Gaia.GX.ProceduralWorlds
{
    public class GaiaWaterGX
    {
        #region Generic informational methods

        /// <summary>
        /// Returns the publisher name if provided. 
        /// This will override the publisher name in the namespace ie Gaia.GX.PublisherName
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Procedural Worlds";
        }

        /// <summary>
        /// Returns the package name if provided
        /// This will override the package name in the class name ie public class PackageName.
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Water";
        }

        #endregion

        #region Methods exposed by Gaia as buttons must be prefixed with GX_

        /// <summary>
        /// Adds water system to the scene
        /// </summary>
        public static void GX_WaterSetup_AddWater()
        {
            GaiaWaterProfile waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));
            GaiaWater.GetProfile(GaiaConstants.GaiaWaterProfileType.DeepBlueOcean, waterProfile);
        }

        /// <summary>
        /// Removes water system from the scene
        /// </summary>
        public static void GX_WaterSetup_RemoveWater()
        {
            GaiaWater.RemoveSystems();
        }

        /// <summary>
        /// Sets water style to deep blue
        /// </summary>
        public static void GX_WaterStyles_DeepBlue()
        {
            GaiaWaterProfile waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));
            GaiaWater.GetProfile(GaiaConstants.GaiaWaterProfileType.DeepBlueOcean, waterProfile);
        }

        /// <summary>
        /// Sets water style to deep blue
        /// </summary>
        public static void GX_WaterStyles_ClearBlue()
        {
            GaiaWaterProfile waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));
            GaiaWater.GetProfile(GaiaConstants.GaiaWaterProfileType.ClearBlueOcean, waterProfile);
        }

        /// <summary>
        /// Sets water style to deep blue
        /// </summary>
        public static void GX_WaterStyles_StandardLake()
        {
            GaiaWaterProfile waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));
            GaiaWater.GetProfile(GaiaConstants.GaiaWaterProfileType.StandardLake, waterProfile);
        }

        /// <summary>
        /// Sets water style to deep blue
        /// </summary>
        public static void GX_WaterStyles_StandardClearLake()
        {
            GaiaWaterProfile waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));
            GaiaWater.GetProfile(GaiaConstants.GaiaWaterProfileType.StandardClearLake, waterProfile);
        }

        /// <summary>
        /// Enables water reflections
        /// </summary>
        public static void GX_WaterReflections_EnableReflections()
        {
            GaiaWater.SetWaterReflections(true);
        }

        /// <summary>
        /// Disables water reflections
        /// </summary>
        public static void GX_WaterReflections_DisableReflections()
        {
            GaiaWater.SetWaterReflections(false);
        }

        #endregion

        #region Utils

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns></returns>
        private static string GetAssetPath(string name)
        {
            string[] assets = AssetDatabase.FindAssets(name, null);
            if (assets.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(assets[0]);
            }
            return null;
        }

        #endregion
    }
}