using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Gaia
{

    public static class GaiaDirectories
    {
        public const string SESSION_DIRECTORY = "/Sessions";
        public const string DATA_DIRECTORY = "/Data";
        public const string STAMP_DIRECTORY = "/Stamps";
        public const string USER_STAMP_DIRECTORY = "/Stamps/My Saved Stamps";
        public const string MASK_DIRECTORY = "/Gaia Masks";
        public const string EXPORT_DIRECTORY = "/Gaia Exports";
        public const string COLLISION_DATA_DIRECTORY = "/TerrainCollisionData";
        public const string GAIA_PRO = "/Procedural Worlds/Gaia/Gaia Pro";
        public const string GAIA_SHADER_DIRECTORY = "/Procedural Worlds/PW Shader Library";
        public const string TERRAIN_MESH_EXPORT_DIRECTORY = "/Mesh Terrains";
        
        /// <summary>
        /// Returns the Gaia Pro folder exists in the project
        /// </summary>
        /// <returns></returns>
        public static bool GetGaiaProDirectory()
        {
            bool isPro = false;
            string dataPath = Application.dataPath;

            if (Directory.Exists(dataPath + GAIA_PRO))
            {
                isPro = true;
            }
            else
            {
                isPro = false;
            }

            return isPro;
        }

        /// <summary>
        /// Return the Gaia directory in the project
        /// </summary>
        /// <returns>String containing the Gaia directory</returns>
        public static string GetGaiaDirectory()
        {
            //Default Directory, will be returned if not in Editor
            string gaiaDirectory = "Assets/Procedural Worlds/Gaia/";
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets("Gaia_ReadMe", null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (Path.GetFileName(path) == "Gaia_ReadMe.txt")
                {
                    gaiaDirectory = path.Replace("/Gaia_ReadMe.txt","");
                }
            }
#endif
            return gaiaDirectory;
        }

        /// <summary>
        /// Returns the Gaia Session Directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Session directory</returns>
        public static string GetSessionDirectory()
        {
            return GetGaiaSubDirectory(SESSION_DIRECTORY);
        }

        /// <summary>
        /// Returns the Gaia Data Directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Data directory</returns>
        public static string GetDataDirectory()
        {
            return GetGaiaSubDirectory(DATA_DIRECTORY);
        }

        /// <summary>
        /// Returns the Gaia Stamps Directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetStampDirectory()
        {
            return GetGaiaSubDirectory(STAMP_DIRECTORY);
        }

        /// <summary>
        /// Returns the Gaia Mask Export Directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetMaskDirectory()
        {
            return GetGaiaSubDirectory(MASK_DIRECTORY);
        }

        /// <summary>
        /// Returns the Gaia Mask Export Directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetExportDirectory()
        {
            return GetGaiaSubDirectory(EXPORT_DIRECTORY);
        }

        /// <summary>
        /// Returns the Gaia User Stamp directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetUserStampDirectory()
        {
            return GetGaiaSubDirectory(USER_STAMP_DIRECTORY);
        }

        /// <summary>
        /// Returns the Gaia Collision Data Directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetCollisionDataDirectory()
        {
            return GetGaiaSubDirectory(COLLISION_DATA_DIRECTORY);
        }

        public static string GetTerrainMeshExportDirectory()
        {
            return CreatePathIfDoesNotExist(GetExportDirectory() + TERRAIN_MESH_EXPORT_DIRECTORY);
        }

        /// <summary>
        /// Creates and returns the path to a certain stamp feature type.
        /// </summary>
        /// <param name="featureType">The feature type which we want to create / get the folder for.</param>
        /// <returns>The path to the specific stamp feature insisde the stamps folder.</returns>
        public static string GetStampFeatureDirectory(GaiaConstants.FeatureType featureType)
        {
            return CreatePathIfDoesNotExist(GetStampDirectory() + "/" + featureType.ToString());
        }

        /// <summary>
        /// Creates and returns the path to a certain terrain's baked collision data.
        /// </summary>
        /// <param name="terrain">The terrain for which we want to create / get the folder for.</param>
        /// <returns>The path to the collision data folder for this terrain.</returns>
        public static string GetTerrainCollisionDirectory(Terrain terrain)
        {
            return CreatePathIfDoesNotExist(GetCollisionDataDirectory() + "/" + terrain.name);
        }



        /// <summary>
        /// Gets the path for a specific stamp instance within the stamp / feature structure.
        /// </summary>
        /// <param name="m_featureType">The stamp feature type</param>
        /// <param name="m_featureName">The stamp name</param>
        /// <returns>The path to the specific stamp instance directory.</returns>
        public static string GetStampInstanceDirectory(GaiaConstants.FeatureType m_featureType, string m_featureName)
        {
            return CreatePathIfDoesNotExist(GetStampFeatureDirectory(m_featureType)) + "/" + m_featureName;
        }


        /// <summary>
        /// Gets the path to a specific stamp data directory. (The Data subfolder within the stamp instance folder)
        /// </summary>
        /// <param name="m_featureType">The stamp feature type</param>
        /// <param name="m_featureName">The stamp name</param>
        /// <returns>The path to the specific stamp instance data directory.</returns>
        public static string GetStampInstanceDataDirectory(GaiaConstants.FeatureType m_featureType, string m_featureName)
        {
            return CreatePathIfDoesNotExist(GetStampInstanceDirectory( m_featureType, m_featureName) + DATA_DIRECTORY);
        }

        public static string GetShaderDirectory()
        {
            return Application.dataPath + GAIA_SHADER_DIRECTORY;
        }

        /// <summary>
        /// Returns a path to a Gaia subdirectory. The subdirectory will be created if it does not exist already.
        /// </summary>
        /// <param name="subDir">The Subdir to create.</param>
        /// <returns>The complete path to the subdir.</returns>
        private static string GetGaiaSubDirectory(string subDir)
        {
            string path = GetGaiaDirectory() + subDir;
            return CreatePathIfDoesNotExist(path);
        }

        /// <summary>
        /// Checks if a path exists, if not it will be created.
        /// </summary>
        /// <param name="path"></param>
        private static string CreatePathIfDoesNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        
    }
}
