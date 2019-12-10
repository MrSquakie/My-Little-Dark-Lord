using UnityEditor;
using UnityEngine.Rendering;

namespace Gaia
{
    /// <summary>
    /// Injects GAIA_PRESENT define into project
    /// </summary>
    [InitializeOnLoad]
    public class GaiaDefinesEditor : Editor
    {
        static GaiaDefinesEditor()
        {
            //Make sure we inject GAIA_PRESENT
            bool updateScripting = false;
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!symbols.Contains("GAIA_PRESENT"))
            {
                updateScripting = true;
                symbols += ";" + "GAIA_PRESENT";
            }

            if (GaiaProUtils.IsGaiaPro())
            {
                if (!symbols.Contains("GAIA_PRO_PRESENT"))
                {
                    updateScripting = true;
                    symbols += ";GAIA_PRO_PRESENT";
                }
            }

            if (updateScripting)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            }

            //CheckLinearSetup();
        }

        /// <summary>
        /// Check if linear light intensity setup is needed
        /// </summary>
        private static void CheckLinearSetup()
        {
            if (PlayerSettings.colorSpace == UnityEngine.ColorSpace.Linear)
            {
                GraphicsSettings.lightsUseLinearIntensity = true;
            }
            else
            {
                GraphicsSettings.lightsUseLinearIntensity = false;
            }
        }
    }

    /// <summary>
    /// Pro utils to check for gaia pro
    /// </summary>
    public static class GaiaProUtils
    {
        #region Gaia Pro

        /// <summary>
        /// Checks if gaia pro exists
        /// </summary>
        /// <returns></returns>
        public static bool IsGaiaPro()
        {
            bool isPro = false;

            //string gaiaConfig = Gaia.Internal.PWApp.CONF.Name;
            //if (gaiaConfig.Contains("Gaia Pro"))
            //{
            //    isPro = true;
            //    return isPro;
            //}

            isPro = GaiaDirectories.GetGaiaProDirectory();

            return isPro;
        }

        #endregion
    }
}