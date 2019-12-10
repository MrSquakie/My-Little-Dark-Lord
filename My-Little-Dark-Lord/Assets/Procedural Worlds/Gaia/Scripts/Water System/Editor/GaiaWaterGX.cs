using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gaia.GX.ProceduralWorlds
{
    public class GaiaWaterGX
    {
        #region Private values

        private static string m_unityVersion;
        private static List<string> m_profileList = new List<string>();
        private static List<Material> m_allMaterials = new List<Material>();

        private const string m_materialLocation = "/Procedural Worlds/Gaia/Gaia Lighting and Water/Gaia Water/Ocean Water/Resources/Material";
        private const string m_builtInKeyWord = "SP";
        private const string m_lightweightKeyWord = "LW";
        private const string m_highDefinitionKeyWord = "HD";

        #endregion

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
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            GaiaWaterProfile waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));
            if (SetupMaterials(gaiaSettings.m_currentRenderer, gaiaSettings, 0))
            {
                GaiaWater.GetProfile(GaiaConstants.GaiaWaterProfileType.DeepBlueOcean, waterProfile, gaiaSettings.m_pipelineProfile.m_activePipelineInstalled, true);
            }
            else
            {
                Debug.Log("Materials could not be found");
            }
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
        public static void GX_WaterStyles_ClearBlueOceanFlat()
        {
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            GaiaWaterProfile waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));

            if (SetupMaterials(gaiaSettings.m_currentRenderer, gaiaSettings, 0))
            {
                GaiaWater.GetProfile(GaiaConstants.GaiaWaterProfileType.DeepBlueOcean, waterProfile, gaiaSettings.m_pipelineProfile.m_activePipelineInstalled, true);
            }
            else
            {
                Debug.Log("Materials could not be found");
            }
        }

        /// <summary>
        /// Sets water style to deep blue
        /// </summary>
        public static void GX_WaterStyles_ClearBlueOcean()
        {
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            GaiaWaterProfile waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));

            if (SetupMaterials(gaiaSettings.m_currentRenderer, gaiaSettings, 1))
            {
                GaiaWater.GetProfile(GaiaConstants.GaiaWaterProfileType.ClearBlueOcean, waterProfile, gaiaSettings.m_pipelineProfile.m_activePipelineInstalled, true);
            }
            else
            {
                Debug.Log("Materials could not be found");
            }
        }

        /// <summary>
        /// Sets water style to deep blue
        /// </summary>
        public static void GX_WaterStyles_DeepBlueOcean()
        {
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            GaiaWaterProfile waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));
            if (SetupMaterials(gaiaSettings.m_currentRenderer, gaiaSettings, 2))
            {
                GaiaWater.GetProfile(GaiaConstants.GaiaWaterProfileType.StandardLake, waterProfile, gaiaSettings.m_pipelineProfile.m_activePipelineInstalled, true);
            }
            else
            {
                Debug.Log("Materials could not be found");
            }
        }

        /// <summary>
        /// Sets water style to deep blue
        /// </summary>
        public static void GX_WaterStyles_DeepBlueGreenOcean()
        {
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            GaiaWaterProfile waterProfile = AssetDatabase.LoadAssetAtPath<GaiaWaterProfile>(GetAssetPath("Gaia Water System Profile"));
            if(SetupMaterials(gaiaSettings.m_currentRenderer, gaiaSettings, 3))
            {
                GaiaWater.GetProfile(GaiaConstants.GaiaWaterProfileType.StandardClearLake, waterProfile, gaiaSettings.m_pipelineProfile.m_activePipelineInstalled, true);
            }
            else
            {
                Debug.Log("Materials could not be found");
            }
        }
   
        /// <summary>
        /// Enables water reflections
        /// </summary>
        public static void GX_WaterReflections_EnableReflections()
        {
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            GaiaWater.SetWaterReflectionsType(true, gaiaSettings.m_pipelineProfile.m_activePipelineInstalled, gaiaSettings.m_gaiaWaterProfile);
        }

        /// <summary>
        /// Disables water reflections
        /// </summary>
        public static void GX_WaterReflections_DisableReflections()
        {
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            GaiaWater.SetWaterReflectionsType(false, gaiaSettings.m_pipelineProfile.m_activePipelineInstalled, gaiaSettings.m_gaiaWaterProfile);
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

        /// <summary>
        /// Setup the material name list
        /// </summary>
        private static bool SetupMaterials(GaiaConstants.EnvironmentRenderer renderPipeline, GaiaSettings gaiaSettings, int profileIndex)
        {
            bool successful = false;

            string[] folders = Directory.GetDirectories(Application.dataPath + m_materialLocation, ".", SearchOption.AllDirectories);
            m_unityVersion = Application.unityVersion;
            m_unityVersion = m_unityVersion.Remove(m_unityVersion.LastIndexOf(".")).Replace(".", "_0");
            string keyWordToSearch = "";

            if (renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
            {
                keyWordToSearch = m_builtInKeyWord;
            }
            else if (renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
            {
                keyWordToSearch = m_lightweightKeyWord;
            }
            else
            {
                keyWordToSearch = m_highDefinitionKeyWord;
            }

            string mainFolder = "";
            foreach (string folderName in folders)
            {
                if (folderName.Contains(keyWordToSearch + " " + m_unityVersion))
                {
                    mainFolder = folderName;
                    break;
                }
            }

            m_profileList.Clear();
            List<Material> allMaterials = GetMaterials(mainFolder);
            if (allMaterials != null)
            {
                foreach (Material mat in allMaterials)
                {
                    m_profileList.Add(mat.name);
                }
            }

            if (allMaterials.Count > 0)
            {
                successful = true;
            }

            gaiaSettings.m_gaiaWaterProfile.m_activeWaterMaterial = m_allMaterials[profileIndex];
            return successful;
        }

        /// <summary>
        /// Removes Suffix in file formats required
        /// </summary>
        /// <param name="path"></param>
        private static List<Material> GetMaterials(string path)
        {
            List<Material> materials = new List<Material>();

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var files = dirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension.EndsWith("mat"))
                {
                    materials.Add(AssetDatabase.LoadAssetAtPath<Material>(GaiaUtils.GetAssetPath(file.Name)));
                }
            }

            m_allMaterials = materials;

            return materials;
        }

        #endregion
    }
}