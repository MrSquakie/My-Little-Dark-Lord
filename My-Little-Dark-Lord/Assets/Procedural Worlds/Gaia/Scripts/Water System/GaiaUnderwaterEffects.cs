using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using ProcedualWorlds.WaterSystem.Reflections;

namespace Gaia
{
    public class GaiaUnderwaterEffects : MonoBehaviour
    {
        [Header("Global Settings")]
        public bool m_startingUnderwater = false;
        public float m_seaLevel = 50f;
        public Transform m_playerCamera;
        public GameObject m_underwaterParticles;
        public GameObject m_horizonObject;
        public PWS_WaterReflections m_waterReflections;


        [Header("Caustic Settings")]
        public bool m_useCaustics = true;
        public Light m_mainLight;
        public int m_framesPerSecond = 24;
        [Range(1f, 100f)]
        public float m_causticSize = 15f;
        public Texture2D[] m_causticTextures;

        [Header("Fog Settings")]
        public bool m_supportFog = true;
        public Color m_fogColor = new Color(0.533f, 0.764f, 1f, 1f);
        public float m_fogDistance = 45f;
        public float m_nearFogDistance = -4f;

        [Header("Audio Settings")]
        [Range(0f, 1f)]
        public float m_playbackVolume = 0.5f;
        public AudioClip m_submergeSoundFXDown;
        public AudioClip m_submergeSoundFXUp;
        public AudioClip m_underwaterSoundFX;

        private int m_indexNumber = 0;
        private AudioSource m_audioSource;
        private AudioSource m_audioSourceUnderwater;
        private ParticleSystem m_underwaterParticleSystem;

        private Color m_surfaceFogColor;
        private float m_surfaceFogDensity;
        private float m_surfaceFogStartDistance;
        private float m_surfaceFogEndDistance;

        private bool m_surfaceSetup = false;
        private bool m_underwaterSetup = false;
        private bool m_startingSystem = false;
        private List<MeshRenderer> m_horizonMeshRenders = new List<MeshRenderer>();
        private GaiaSceneInfo m_sceneInfo;

        private void Start()
        {
            if (m_sceneInfo == null)
            {
                m_sceneInfo = GaiaSceneInfo.GetSceneInfo();
            }

            if (m_sceneInfo != null)
            {
                m_seaLevel = m_sceneInfo.m_seaLevel;
            }

            if (m_playerCamera == null)
            {
                m_playerCamera = Camera.main.transform;
            }

            if (m_waterReflections == null)
            {
                m_waterReflections = FindObjectOfType<PWS_WaterReflections>();
            }

            m_mainLight = GetMainLight();
            m_audioSource = GetAudioSource();
            m_audioSourceUnderwater = GetAudioSource();
            m_audioSourceUnderwater.clip = m_underwaterSoundFX;
            m_audioSourceUnderwater.loop = true;
            m_audioSourceUnderwater.volume = m_playbackVolume;
            m_audioSourceUnderwater.Stop();

            if (m_startingUnderwater)
            {
                m_underwaterSetup = false;
                m_surfaceSetup = true;
            }
            else
            {
                m_underwaterSetup = true;
                m_surfaceSetup = false;
            }

            if (m_underwaterParticles != null)
            {
                m_underwaterParticleSystem = m_underwaterParticles.GetComponent<ParticleSystem>();
                if (m_underwaterParticleSystem != null)
                {
                    m_underwaterParticleSystem.Stop();
                }
            }

            if (m_horizonObject != null)
            {
                MeshRenderer[] meshRenders = m_horizonObject.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer render in meshRenders)
                {
                    m_horizonMeshRenders.Add(render);
                    render.enabled = false;
                }
            }

            m_surfaceFogColor = RenderSettings.fogColor;
            m_surfaceFogDensity = RenderSettings.fogDensity;
            m_surfaceFogStartDistance = RenderSettings.fogStartDistance;
            m_surfaceFogEndDistance = RenderSettings.fogEndDistance;
        }

        private void Update()
        {
            if (m_playerCamera == null)
            {
                Debug.LogError("Player Camera is missing from the setup. Exiting!");
                return;
            }
            else
            {
                if (m_playerCamera.position.y > m_seaLevel)
                {
                    if (!m_surfaceSetup)
                    {
                        SetupWaterSystems(false);
                        m_underwaterSetup = false;
                        m_surfaceSetup = true;
                    }
                }
                else
                {
                    if (!m_underwaterSetup)
                    {
                        SetupWaterSystems(true);
                        m_underwaterSetup = true;
                        m_surfaceSetup = false;
                    }
                }
            }
        }

        /// <summary>
        /// Plays the underwater caustic animations
        /// </summary>
        /// <param name="systemOn"></param>
        /// <returns></returns>
        private void CausticsAnimation()
        {
            if (m_mainLight != null)
            {
                m_mainLight.cookieSize = m_causticSize;
                m_mainLight.cookie = m_causticTextures[m_indexNumber];
                m_indexNumber++;
            }

            if (m_indexNumber == m_causticTextures.Length)
            {
                m_indexNumber = 0;
            }
        }

        /// <summary>
        /// Sets the water settings
        /// </summary>
        /// <param name="isUnderwater"></param>
        private void SetupWaterSystems(bool isUnderwater)
        {
            if (m_startingSystem)
            {
                if (isUnderwater)
                {
                    if (m_waterReflections != null)
                    {
                        m_waterReflections.enabled = false;
                    }

                    if (m_submergeSoundFXDown != null)
                    {
                        m_audioSource.PlayOneShot(m_submergeSoundFXDown, m_playbackVolume);
                    }

                    if (m_causticTextures != null)
                    {
                        if (m_useCaustics)
                        {
                            InvokeRepeating("CausticsAnimation", 0f, 1f / m_framesPerSecond);
                        }
                        else
                        {
                            CancelInvoke();
                        }
                    }

                    if (m_supportFog)
                    {
                        RenderSettings.fogColor = m_fogColor;
                        RenderSettings.fogDensity = 0.5f;
                        RenderSettings.fogStartDistance = m_nearFogDistance;
                        RenderSettings.fogEndDistance = m_fogDistance;
                    }

                    if (m_horizonMeshRenders != null)
                    {
                        foreach (MeshRenderer render in m_horizonMeshRenders)
                        {
                            render.enabled = true;
                        }
                    }

                    m_underwaterParticles.SetActive(true);
                    m_underwaterParticleSystem.Play();
                    m_audioSourceUnderwater.Play();
                }
                else
                {
                    if (m_waterReflections != null)
                    {
                        m_waterReflections.enabled = true;
                    }

                    if (m_submergeSoundFXUp != null)
                    {
                        m_audioSource.PlayOneShot(m_submergeSoundFXUp, m_playbackVolume);
                    }

                    CancelInvoke();

                    if (m_mainLight != null)
                    {
                        m_mainLight.cookie = null;
                    }

                    if (m_supportFog)
                    {
                        RenderSettings.fogColor = m_surfaceFogColor;
                        RenderSettings.fogDensity = m_surfaceFogDensity;
                        RenderSettings.fogStartDistance = m_surfaceFogStartDistance;
                        RenderSettings.fogEndDistance = m_surfaceFogEndDistance;
                    }

                    if (m_horizonMeshRenders != null)
                    {
                        foreach (MeshRenderer render in m_horizonMeshRenders)
                        {
                            render.enabled = false;
                        }
                    }

                    m_underwaterParticleSystem.Stop();
                    m_underwaterParticles.SetActive(false);
                    m_audioSourceUnderwater.Stop();
                }
            }
            else
            {
                m_startingSystem = true;
            }
        }

        /// <summary>
        /// Gets and returns the audio source
        /// </summary>
        /// <returns></returns>
        private AudioSource GetAudioSource()
        {
            AudioSource audioSource = null;
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                return audioSource;
            }
            {
                return audioSource;
            }
        }

        /// <summary>
        /// Gets and returns a directional light
        /// </summary>
        /// <returns></returns>
        private Light GetMainLight()
        {
            Light[] mainLight = FindObjectsOfType<Light>();
            if (mainLight != null)
            {
                foreach (Light light in mainLight)
                {
                    if (light.type == LightType.Directional)
                    {
                        return light;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Updates the fog surface settings such as color density distance etc.
        /// </summary>
        public void UpdateSurfaceFogSettings()
        {
            m_surfaceFogColor = RenderSettings.fogColor;
            m_surfaceFogDensity = RenderSettings.fogDensity;
            m_surfaceFogStartDistance = RenderSettings.fogStartDistance;
            m_surfaceFogEndDistance = RenderSettings.fogEndDistance;
        }

        public void LoadUnderwaterSystemAssets()
        {
#if UNITY_EDITOR
            m_playerCamera = Camera.main.transform;

            m_causticTextures = new Texture2D[16];
            m_causticTextures[0] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_001.tif"));
            m_causticTextures[1] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_002.tif"));
            m_causticTextures[2] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_003.tif"));
            m_causticTextures[3] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_004.tif"));
            m_causticTextures[4] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_005.tif"));
            m_causticTextures[5] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_006.tif"));
            m_causticTextures[6] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_007.tif"));
            m_causticTextures[7] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_008.tif"));
            m_causticTextures[8] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_009.tif"));
            m_causticTextures[9] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_010.tif"));
            m_causticTextures[10] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_011.tif"));
            m_causticTextures[11] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_012.tif"));
            m_causticTextures[12] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_013.tif"));
            m_causticTextures[13] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_014.tif"));
            m_causticTextures[14] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_015.tif"));
            m_causticTextures[15] = AssetDatabase.LoadAssetAtPath<Texture2D>(GaiaUtils.GetAssetPath("CausticsRender_016.tif"));

            m_submergeSoundFXDown = AssetDatabase.LoadAssetAtPath<AudioClip>(GaiaUtils.GetAssetPath("Gaia Ambient Submerge Down.mp3"));
            m_submergeSoundFXUp = AssetDatabase.LoadAssetAtPath<AudioClip>(GaiaUtils.GetAssetPath("Gaia Ambient Submerge Up.mp3"));
            m_underwaterSoundFX = AssetDatabase.LoadAssetAtPath<AudioClip>(GaiaUtils.GetAssetPath("Gaia Ambient Underwater Sound Effect.mp3"));
            //m_horizonObject = AssetDatabase.LoadAssetAtPath<GameObject>(GaiaUtils.GetAssetPath("Ambient Underwater Horizon.prefab"));
#endif
        }
    }
}