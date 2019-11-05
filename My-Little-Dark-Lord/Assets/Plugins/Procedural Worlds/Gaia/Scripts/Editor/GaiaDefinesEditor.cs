using UnityEditor;

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
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!symbols.Contains("GAIA_PRESENT"))
            {
                symbols += ";" + "GAIA_PRESENT";
            }

            if (GaiaProUtils.IsGaiaPro())
            {
                if (!symbols.Contains("GAIA_PRO_PRESENT"))
                {
                    symbols += ";GAIA_PRO_PRESENT";
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
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

