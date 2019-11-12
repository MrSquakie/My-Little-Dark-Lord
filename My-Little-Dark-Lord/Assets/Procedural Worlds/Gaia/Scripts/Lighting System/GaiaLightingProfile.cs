using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gaia
{
    public class GaiaLightingProfile : ScriptableObject
    {
        [HideInInspector]
        public bool m_updateInRealtime = false;
        [HideInInspector]
        public GaiaConstants.GaiaLightingProfileType m_lightingProfile = GaiaConstants.GaiaLightingProfileType.Day;
        [HideInInspector]
        public bool m_editSettings = false;

        [HideInInspector]
        public Material m_masterSkyboxMaterial;
        public List<GaiaLightingProfileValues> m_lightingProfiles = new List<GaiaLightingProfileValues>();

        [HideInInspector]
        public bool m_parentObjects = true;
        [HideInInspector]
        public bool m_hideProcessVolume = true;
        [HideInInspector]
        public bool m_enablePostProcessing = true;
        [HideInInspector]
        public bool m_enableAmbientAudio = true;
        [HideInInspector]
        public bool m_enableFog = true;
        [HideInInspector]
        public GaiaConstants.GaiaProAntiAliasingMode m_antiAliasingMode = GaiaConstants.GaiaProAntiAliasingMode.TAA;

        /// <summary>
        /// Create Gaia Lighting System Profile asset
        /// </summary>
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Procedural Worlds/Gaia/Lighting Profile")]
        public static void CreateSkyProfiles()
        {
            GaiaLightingProfile asset = ScriptableObject.CreateInstance<GaiaLightingProfile>();
            AssetDatabase.CreateAsset(asset, "Assets/Gaia Lighting System Profile.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
#endif
    }
}