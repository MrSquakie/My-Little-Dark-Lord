using UnityEngine;

namespace Gaia
{

    public enum SeasonMode { Autumn, Winter, Spring, Summer }
    [ExecuteInEditMode]
    public class ProceduralWorldsGlobalWind : MonoBehaviour
    {
        //Stored Wind Zone
        [Header("Wind Settings")]
        [Tooltip("The wind zone component that sets the wind settings")]
        public WindZone m_windZone;
        [Range(0f, 15f)]
        public float m_windSpeed = 0.2f;
        [Range(0f, 15f)]
        public float m_windTurbulence = 0.2f;
        //[Range(0f, 360f)]
        //[Tooltip("Sets the rotation 'y' of the wind zone object")]
        //[HideInInspector]
        //public float m_windDirection = 25f;
        [Header("Rain Settings")]
        [Tooltip("Sets the rain intensity")]
        [Range(0f, 1f)]
        [HideInInspector]
        public float m_rainIntensity = 0f;
        [Header("Snow Settings")]
        [Range(0f, 10f)]
        [Tooltip("Sets the snow intensity")]
        [HideInInspector]
        public float m_snowIntensity = 0f;
        [Header("Season Settings")]
        //[Tooltip("Sets seasons to be active on the vegetation")]
        //public SeasonMode m_seasonMode = SeasonMode.Summer;
        [Range(0f, 1f)]
        [Tooltip("Sets the season tint intensity")]
        [HideInInspector]
        public float m_seasonTintIntensity = 0.5f;
        [Range(0f, 1f)]
        [Tooltip("Sets the season tint")]
        [HideInInspector]
        public float m_season = 0.5f;

        private const string m_windZoneObjectName = "PW Wind Zone";
        [Header("Shader Settings")]
        [Tooltip("Sets the global 'Wind Speed' in the shader")]
        public const string m_shaderWindSpeed = "PWSF_GlobalWindIntensity";
        //[Tooltip("Sets the global 'Wind Intensity' in the shader")]
        //public string m_shaderWindIntensity = "Dynamic Wind Intensity";

        //[Tooltip("Sets the global 'Rain Speed' in the shader")]
        //public string m_shaderRainSpeed = "PWS_RainSpeed";
        [Tooltip("Sets the global 'Rain Intensity' in the shader")]
        public const string m_shaderRainIntensity = "Dynamic Rain Intensity";

        //[Tooltip("Sets the global 'Snow Speed' in the shader")]
        //public string m_shaderSnowSpeed = "PWS_SnowSpeed";
        [Tooltip("Sets the global 'Snow Intensity' in the shader")]
        public const string m_shaderSnowIntensity = "PWSF_GlobalSnowIntensity";

        [Tooltip("Sets the global 'Global Tint Intensity' in the shader")]
        public const string m_shaderGlobalTintIntensity = "PWS_GlobalTintIntensity";
        [Tooltip("Sets the global 'Global Season Mode' in the shader")]
        private const string m_shaderSeasonMode = "PWS_GlobalSeasonMode";
        [Tooltip("Sets the global 'Global Season Color Shift' in the shader")]
        public const string m_shaderSeasonColorShift = "PWS_GlobalSeasonColorShift";

        private void OnEnable()
        {
            //Gets the wind zone if it's empty
            if (m_windZone == null)
            {
                m_windZone = GetOrCreateWindZone();
            }
        }

        /// <summary>
        /// Load on start
        /// </summary>
        private void Start()
        {
            //Gets the wind zone if it's empty
            if (m_windZone == null)
            {
                m_windZone = GetOrCreateWindZone();
            }

            if (gameObject.name != "PW Global Wind Settings")
            {
                gameObject.name = "PW Global Wind Settings";
            }
        }

        /// <summary>
        /// Update every frame
        /// </summary>
        private void Update()
        {
            //Gets the wind zone if it's empty
            if (m_windZone == null)
            {
                m_windZone = GetOrCreateWindZone();
                return;
            }
            else
            {
                //Set the global wind settings
                UpdateWindSettings(m_windZone);
                //Sets the global rain settings
                //UpdateRainSettings();
                //Sets the global snow settings
                //UpdateSnowSettings();
                //Sets the season settings
                //UpdateSeasonMode();
            }
        }

        /// <summary>
        /// Updates the wind settings
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="windZone"></param>
        private void UpdateWindSettings(WindZone windZone)
        {
            Shader.SetGlobalFloat(m_shaderWindSpeed, m_windZone.windMain * 6f + m_windZone.windTurbulence);

            m_windZone.windMain = m_windSpeed;
            m_windZone.windTurbulence = m_windTurbulence;

            //m_windZone.gameObject.transform.eulerAngles = new Vector3(25f, m_windDirection, 0f);
        }

        /// <summary>
        /// Updates the rain settings
        /// </summary>
        private void UpdateRainSettings()
        {
            Shader.SetGlobalFloat(m_shaderRainIntensity, m_rainIntensity);
        }

        /// <summary>
        /// Updates the snow settings
        /// </summary>
        private void UpdateSnowSettings()
        {
            Shader.SetGlobalFloat(m_shaderSnowIntensity, m_snowIntensity);
        }

        /// <summary>
        /// Updates the season settings
        /// </summary>
        private void UpdateSeasonMode()
        {
            Shader.SetGlobalFloat(m_shaderGlobalTintIntensity, m_seasonTintIntensity);
            Shader.SetGlobalFloat(m_shaderSeasonColorShift, m_season);
        }

        /// <summary>
        /// Create a wind zone fi it doesn't exist
        /// </summary>
        private WindZone GetOrCreateWindZone()
        {          
            GameObject windZone = GameObject.Find(m_windZoneObjectName);
            WindZone windSettings = FindObjectOfType<WindZone>();
            if (windSettings != null)
            {
                windSettings.transform.SetParent(transform);
                return windSettings;
            }
            if (windZone == null)
            {
                windZone = new GameObject(m_windZoneObjectName);
                Vector3 windRotation = new Vector3(25f, 0f, 0f);
                windZone.transform.Rotate(windRotation, Space.World);
                windZone.transform.SetParent(transform);
                windSettings = windZone.AddComponent<WindZone>();
                windSettings.mode = WindZoneMode.Directional;
                windSettings.windMain = 0.2f;
                windSettings.windTurbulence = 0.2f;
                windSettings.windPulseMagnitude = 0.2f;
                windSettings.windPulseFrequency = 0.05f;
                m_windSpeed = windSettings.windMain;
                m_windTurbulence = windSettings.windTurbulence;

                m_windZone = windSettings;
            }
            else
            {
                windSettings = windZone.GetComponent<WindZone>();
                m_windSpeed = windSettings.windMain;
                m_windTurbulence = windSettings.windTurbulence;
            }

            return windSettings;
        }
    }
}