#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Gaia
{
    /// <summary>
    /// Focuses the lighting profile
    /// </summary>
    public class FocusWaterProfile : MonoBehaviour
    {
        /// <summary>
        /// Focus the profile specified with m_profileName string
        /// </summary>
        public void FocusProfile()
        {
            GaiaSettings settings = GaiaUtils.GetGaiaSettings();
            if (settings != null)
            {
                if (settings.m_gaiaLightingProfile != null)
                {
                    Selection.activeObject = settings.m_gaiaWaterProfile;
                }
                else
                {
                    Debug.LogError("Water Profile in Gaia Settings has not been assigned");
                }
            }
            else
            {
                Debug.LogError("Unable to find Gaia Settings");
            }
        }
    }
}
#endif