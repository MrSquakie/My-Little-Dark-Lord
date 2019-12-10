namespace ProcedualWorlds.WaterSystem.Reflections
{
    using System.Collections;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    /// <summary>
    /// Generates a water reflection.
    /// </summary>
    [ExecuteInEditMode]
    public class PWS_WaterReflections : MonoBehaviour
    {
        #region PublicVariables
        /// <summary>
        /// Allow MSAA.
        /// </summary>
        public bool m_MSAA;
        /// <summary>
        /// Allow HDR.
        /// </summary>
        public bool m_HDR;
        /// <summary>
        /// To decided if shadows are to be rendered
        /// </summary>
        public bool m_shadowRender;
        /// <summary>
        /// Use a custom render path
        /// </summary>
        public bool m_customRenderPath;
        /// <summary>
        /// To decided if shadows are to be rendered
        /// </summary>
        public bool m_skyboxOnly;
        /// <summary>
        /// Enables the use of m_refreshRate (in seconds)
        /// </summary>
        public bool m_useRefreshTime;
        /// <summary>
        /// Custom reflection distance
        /// </summary>
        public bool m_customReflectionDistance;
        /// <summary>
        /// single distance layer
        /// </summary>
        public bool m_singleDistanceLayer;
        /// <summary>
        /// To decided if pixel lights are to be rendered.
        /// </summary>
        public bool m_disablePixelLights;
        /// <summary>
        /// To decide if the reflections camera should be hidden
        /// </summary>
        public bool m_hideReflectionCamera = true;
        /// <summary>
        /// The texture size to use for rendering.
        /// </summary>
        public int m_renderTextureSize = 256;
        /// <summary>
        /// Refresh rate that can be used with FixedUpdate, Update, OnRender
        /// </summary>
        public float m_refreshRate = 0.25f;
        /// <summary>
        /// Render Distance of the reflections camera
        /// </summary>
        public float m_renderDistance = 150f;
        /// <summary>
        /// Clip plane offset to use.
        /// </summary>
        public float m_clipPlaneOffset = 0.07f;
        /// <summary>
        /// Update theshold for the InvokeRepeating.
        /// </summary>
        public float m_updateThreshold = 0.5f;
        /// <summary>
        /// Camera used to find orentation of reflection.
        /// </summary>
        public Camera m_RenderCamera;
        /// <summary>
        /// LayerMask used for what to actual render.
        /// </summary>
        public LayerMask m_reflectionLayers = -1;
        /// <summary>
        /// Update method used for deciding what update method to use.
        /// </summary>
        public RenderUpdateMode m_RenderUpdate = RenderUpdateMode.OnRender;
        /// <summary>
        /// Render path to use for rendering of the camera.
        /// </summary>
        public RenderingPath m_RenderPath;
        /// <summary>
        /// Distances check
        /// </summary>
        public float[] m_distances = new float[32];
        #endregion
        #region PrivateVariables
        private bool m_ableToRender = true;
        private bool m_rebuild;
        private int m_oldPixelLightCount;
        private int m_oldRenderTextureSize;
        private float m_oldShadowDistance;
        private float m_tempTime;
        private Vector3 m_worldPosition;
        private Vector3 m_normal;
        private Vector4 m_reflectionPlane;
        private Vector3 m_oldPosition;
        private Vector3 m_newPosition;
        private Vector3 m_euler;
        private Vector4 m_clipPlane;
        private Vector3 m_currentPosition;
        private Vector3 m_currentRotation;
        private Matrix4x4 m_projection;
        private Matrix4x4 m_reflection;
        private RenderTexture m_reflectionTexture;
        private Camera m_reflectionCamera;
        #endregion
        #region UnityMethods
        /// <summary>
        /// On awake sets up anything that could have changed since enable.
        /// </summary>
        public void Awake()
        {
            Generate();
        }
        /// <summary>
        /// Update is called every frame
        /// </summary>
        public void Update()
        {
            if (Application.isPlaying && m_RenderUpdate == RenderUpdateMode.Update)
            {
                if (m_RenderCamera != null)
                {
                    if (m_RenderCamera.transform.position != m_currentPosition || m_RenderCamera.transform.rotation.eulerAngles != m_currentRotation)
                    {
                        if (m_useRefreshTime)
                        {
                            if (Time.time >= m_tempTime)
                            {
                                m_tempTime += m_refreshRate;
                                BuildReflection();
                                m_currentPosition = m_RenderCamera.transform.position;
                                m_currentRotation = m_RenderCamera.transform.rotation.eulerAngles;
                            }
                        }
                        else
                        {
                            BuildReflection();
                            m_currentPosition = m_RenderCamera.transform.position;
                            m_currentRotation = m_RenderCamera.transform.rotation.eulerAngles;
                        }
                    }
                }
                else
                {
                    CameraSetup();
                }
            }
        }
        /// <summary>
        /// OnWillRenderObject allows us to get a editor reflection working,
        /// Aswell as provides another way to refresh that is more stable.
        /// </summary>
        public void OnWillRenderObject()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                if (SceneView.lastActiveSceneView == null)
                {
                    return;
                }

                m_RenderCamera = SceneView.lastActiveSceneView.camera;
                if (m_RenderCamera != null)
                {
                    if (m_RenderCamera.transform.position != m_currentPosition || m_RenderCamera.transform.rotation.eulerAngles != m_currentRotation)
                    {
                        CameraSetup();
                        GenerateCamera();
                        ResyncCameraSettings();
                        UpdateCameraModes();
                        BuildReflection();
                        m_currentPosition = m_RenderCamera.transform.position;
                        m_currentRotation = m_RenderCamera.transform.rotation.eulerAngles;
                    }
                }
            }
#endif
            if (m_RenderUpdate == RenderUpdateMode.OnRender && Application.isPlaying)
            {
                if (m_RenderCamera != null)
                {
                    if (m_RenderCamera.transform.position != m_currentPosition || m_RenderCamera.transform.rotation.eulerAngles != m_currentRotation)
                    {
                        if (m_useRefreshTime)
                        {
                            if (Time.time >= m_tempTime)
                            {
                                m_tempTime += m_refreshRate;
                                BuildReflection();
                                m_currentPosition = m_RenderCamera.transform.position;
                                m_currentRotation = m_RenderCamera.transform.rotation.eulerAngles;
                            }
                        }
                        else
                        {
                            BuildReflection();
                            m_currentPosition = m_RenderCamera.transform.position;
                            m_currentRotation = m_RenderCamera.transform.rotation.eulerAngles;
                        }
                    }
                }
                else
                {
                    CameraSetup();
                }
            }
        }
        /// <summary>
        /// Fixed Update
        /// </summary>
        public void FixedUpdate()
        {
            if (m_RenderUpdate == RenderUpdateMode.FixedUpdate)
            {
                if (m_RenderCamera != null)
                {
                    if (m_RenderCamera.transform.position != m_currentPosition || m_RenderCamera.transform.rotation.eulerAngles != m_currentRotation)
                    {
                        if (m_useRefreshTime)
                        {
                            if (Time.time >= m_tempTime)
                            {
                                m_tempTime += m_refreshRate;
                                BuildReflection();
                                m_currentPosition = m_RenderCamera.transform.position;
                                m_currentRotation = m_RenderCamera.transform.rotation.eulerAngles;
                            }
                        }
                        else
                        {
                            BuildReflection();
                            m_currentPosition = m_RenderCamera.transform.position;
                            m_currentRotation = m_RenderCamera.transform.rotation.eulerAngles;
                        }
                    }
                }
                else
                {
                    CameraSetup();
                }
            }
        }
        /// <summary>
        /// OnDisable ClearData
        /// </summary>
        public void OnDisable()
        {
            m_rebuild = true;
            ClearData();
        }
        /// <summary>
        /// On enable rebuild the required data
        /// </summary>
        public void OnEnable()
        {
            Generate();
            m_rebuild = false;
        }
        /// <summary>
        /// OnDestroy ClearData
        /// </summary>
        public void OnDestroy()
        {
            ClearData();
        }
        #endregion
        #region Generate
        public void Generate()
        {
            if (GetComponent<Renderer>())
            {
                m_oldPixelLightCount = QualitySettings.pixelLightCount;
                CameraSetup();
                GenerateCamera();
                ResyncCameraSettings();
                UpdateCameraModes();
                CreateMirrorObjects();
                CancelInvoke();
                if (m_RenderUpdate == RenderUpdateMode.Interval)
                {
                    InvokeRepeating("RefreshReflection", 0, m_updateThreshold);
                }
            }
            else
            {
                Debug.Log("unable to create reflections, render missing");
                return;
            }
        }
        #endregion
        #region Camerasetup
        /// <summary>
        /// Sets the camera to use for mirroring
        /// </summary>
        public void CameraSetup()
        {
            if (m_RenderCamera == null)
            {
                m_RenderCamera = Camera.current;
                if (m_RenderCamera == null)
                {
                    m_RenderCamera = Camera.main;
                }
                if (m_RenderCamera == null)
                {
                    return;
                }
            }

        }
        /// <summary>
        /// Used to clear Camera and Render texture
        /// </summary>
        public void ClearData()
        {
            if (m_reflectionTexture)
            {
                m_reflectionTexture.Release();
                DestroyImmediate(m_reflectionTexture);
                m_reflectionTexture = null;
            }
            DestroyImmediate(m_reflectionCamera);
        }
        #endregion
        #region RefreshReflections
        /// <summary>
        /// Refresh reflection
        /// </summary>
        public void RefreshReflection()
        {
            if (m_RenderCamera != null)
            {
                if (m_RenderCamera.transform.position != m_currentPosition || m_RenderCamera.transform.rotation.eulerAngles != m_currentRotation)
                {
                    BuildReflection();
                    m_currentPosition = m_RenderCamera.transform.position;
                    m_currentRotation = m_RenderCamera.transform.rotation.eulerAngles;
                }
            }
            else
            {
                CameraSetup();
            }
        }
        #endregion
        #region BuildReflections
        /// <summary>
        /// Builds the necessary steps to produce a reflection
        /// </summary>
        public void BuildReflection()
        {
            if (m_RenderCamera)
            {
                if (m_ableToRender == true)
                {
                    m_ableToRender = false;
                    PlanePosition();
                    PixelLightCount(m_disablePixelLights);
                    GenerateReflection();
                    CreateMirrorObjects();
                    PixelLightCount(false);
                    m_ableToRender = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                Debug.Log("no rendering camera found");
                return;
            }
        }
        /// <summary>
        /// Position Data
        /// </summary>
        public void PlanePosition()
        {
            m_worldPosition = transform.position;
            m_normal = transform.up;
        }
        /// <summary>
        /// Pixel Light settings
        /// </summary>
        /// <param name="Value">Pixel Light value</param>
        public void PixelLightCount(bool Value)
        {
            if (Value)
            {
                m_oldPixelLightCount = QualitySettings.pixelLightCount;
                QualitySettings.pixelLightCount = 0;
            }
            else
            {
                if (QualitySettings.pixelLightCount != m_oldPixelLightCount)
                {
                    QualitySettings.pixelLightCount = m_oldPixelLightCount;
                }
            }
        }
        /// <summary>
        /// Can be used to keep the cameras settings in check
        /// </summary>
        public void ResyncCameraSettings()
        {
            m_reflectionCamera.orthographic = m_RenderCamera.orthographic;
            m_reflectionCamera.fieldOfView = m_RenderCamera.fieldOfView;
            m_reflectionCamera.aspect = m_RenderCamera.aspect;
            m_reflectionCamera.orthographicSize = m_RenderCamera.orthographicSize;
            if (m_customRenderPath)
            {
                m_reflectionCamera.renderingPath = m_RenderPath;
            }
            else
            {
                m_reflectionCamera.renderingPath = m_RenderCamera.actualRenderingPath;
            }
            m_reflectionCamera.allowHDR = m_HDR;
            m_reflectionCamera.allowMSAA = m_MSAA;
        }
        /// <summary>
        /// Updates Cameras flags, background color & when present the sky
        /// </summary>
        public void UpdateCameraModes()
        {
            m_reflectionCamera.clearFlags = m_RenderCamera.clearFlags;
            m_reflectionCamera.backgroundColor = m_RenderCamera.backgroundColor;
            if (m_RenderCamera.clearFlags == CameraClearFlags.Skybox)
            {
                Skybox Skybox1 = m_RenderCamera.GetComponent(typeof(Skybox)) as Skybox;
                Skybox skybox2 = m_reflectionCamera.GetComponent(typeof(Skybox)) as Skybox;
                if (!Skybox1 || !Skybox1.material)
                {
                    skybox2.enabled = false;
                }
                else
                {
                    skybox2.enabled = true;
                    skybox2.material = Skybox1.material;
                }
            }
        }
        /// <summary>
        /// Create a mirror texture
        /// </summary>
        public void CreateMirrorObjects()
        {
            if (m_rebuild)
            {
                CreateTexture();
                if (m_hideReflectionCamera)
                {
                    m_reflectionTexture.hideFlags = HideFlags.HideAndDontSave;
                }
                else
                {
                    m_reflectionTexture.hideFlags = HideFlags.DontSave;
                }
            }
            if (m_oldRenderTextureSize != m_renderTextureSize)
            {
                CreateTexture();
                if (m_hideReflectionCamera)
                {
                    m_reflectionTexture.hideFlags = HideFlags.HideAndDontSave;
                }
                else
                {
                    m_reflectionTexture.hideFlags = HideFlags.DontSave;
                }
                m_oldRenderTextureSize = m_renderTextureSize;
            }
        }
        public void CreateTexture()
        {
            m_reflectionTexture = new RenderTexture(m_renderTextureSize, m_renderTextureSize, 16)
            {
                name = "__MirrorReflection" + GetInstanceID(),
                isPowerOfTwo = true
            };
        }
        /// <summary>
        /// Generates the camera used for the mirror
        /// </summary>
        public void GenerateCamera()
        {
            if (!m_reflectionCamera)
            {
                GameObject MirrorGameObject = new GameObject("Mirror Refl Camera id" + GetInstanceID() + " for " + m_RenderCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
                m_reflectionCamera = MirrorGameObject.GetComponent<Camera>();
                m_reflectionCamera.enabled = false;
                m_reflectionCamera.transform.position = transform.position;
                m_reflectionCamera.transform.rotation = transform.rotation;
                if (m_hideReflectionCamera)
                {
                    MirrorGameObject.hideFlags = HideFlags.HideAndDontSave;
                }
                else
                {
                    // MirrorGameObject.hideFlags = HideFlags.DontSave;
                }
            }
        }
        /// <summary>
        /// Generates the reflection based on the main camera
        /// </summary>
        public void GenerateReflection()
        {
            // Reflect camera around reflection plane
            float DotProduct = -Vector3.Dot(m_normal, m_worldPosition) - m_clipPlaneOffset;
            Vector4 ReflectionPlane = new Vector4(m_normal.x, m_normal.y, m_normal.z, DotProduct);

            m_reflection = Matrix4x4.zero;
            CalculateReflectionMatrix(ref m_reflection, ReflectionPlane);
            m_oldPosition = m_RenderCamera.transform.position;
            m_newPosition = m_reflection.MultiplyPoint(m_oldPosition);
            m_reflectionCamera.worldToCameraMatrix = m_RenderCamera.worldToCameraMatrix * m_reflection;

            // Setup oblique projection matrix so that near plane is our reflection plane.
            // This way we clip everything below/above it for free.
            m_clipPlane = CameraSpacePlane(m_reflectionCamera, m_worldPosition, m_normal, 1.0f);
            m_projection = m_RenderCamera.CalculateObliqueMatrix(m_clipPlane);
            m_reflectionCamera.projectionMatrix = m_projection;
            // Distances for each layer or general layer
            if (m_customReflectionDistance)
            {
                if (m_singleDistanceLayer)
                {
                    for (int LayerID = 0; LayerID < m_distances.Length; LayerID++)
                    {
                        // For each distance set to render layer
                        m_distances[LayerID] = m_renderDistance;
                    }
                }
            }
            if (m_customReflectionDistance)
            {
                m_reflectionCamera.layerCullDistances = m_distances;
                m_reflectionCamera.layerCullSpherical = true;
            }
            else
            {
                m_reflectionCamera.layerCullDistances = m_RenderCamera.layerCullDistances;
                m_reflectionCamera.layerCullSpherical = true;
            }
            if (m_skyboxOnly)
            {
                m_reflectionCamera.cullingMask = 0;
            }
            else
            {
                // Never render water layer
                m_reflectionCamera.cullingMask = ~(1 << 4) & m_reflectionLayers.value;
            }

            m_reflectionCamera.targetTexture = m_reflectionTexture;
            GL.invertCulling = true;
            m_reflectionCamera.transform.position = m_newPosition;
            m_euler = m_RenderCamera.transform.eulerAngles;
            m_reflectionCamera.transform.eulerAngles = new Vector3(0, m_euler.y, m_euler.z);
            m_reflectionCamera.Render();
            m_reflectionCamera.transform.position = m_oldPosition;
            GL.invertCulling = false;

            Shader.SetGlobalTexture("_ReflectionTex", m_reflectionTexture);
        }
        /// <summary>
        /// Extended sign: returns -1, 0 or 1 based on sign of a
        /// </summary>
        /// <param name="signValue"></param>
        /// <returns></returns>
        private static float Sign(float signValue)
        {
            if (signValue > 0.0f) return 1.0f;
            if (signValue < 0.0f) return -1.0f;
            return 0.0f;
        }
        /// <summary>
        /// Given position/normal of the plane, calculates plane in camera space.
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="pos"></param>
        /// <param name="normal"></param>
        /// <param name="sideSign"></param>
        /// <returns></returns>
        private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            Vector3 offsetPos = pos + normal * m_clipPlaneOffset;
            Matrix4x4 m = cam.worldToCameraMatrix;
            Vector3 cpos = m.MultiplyPoint(offsetPos);
            Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }
        /// <summary>
        /// Calculates reflection matrix around the given plane
        /// </summary>
        /// <param name="reflectionMat"></param>
        /// <param name="plane"></param>
        private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
            reflectionMat.m01 = (-2F * plane[0] * plane[1]);
            reflectionMat.m02 = (-2F * plane[0] * plane[2]);
            reflectionMat.m03 = (-2F * plane[3] * plane[0]);

            reflectionMat.m10 = (-2F * plane[1] * plane[0]);
            reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
            reflectionMat.m12 = (-2F * plane[1] * plane[2]);
            reflectionMat.m13 = (-2F * plane[3] * plane[1]);

            reflectionMat.m20 = (-2F * plane[2] * plane[0]);
            reflectionMat.m21 = (-2F * plane[2] * plane[1]);
            reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
            reflectionMat.m23 = (-2F * plane[3] * plane[2]);

            reflectionMat.m30 = 0F;
            reflectionMat.m31 = 0F;
            reflectionMat.m32 = 0F;
            reflectionMat.m33 = 1F;
        }

        #endregion
        #region RenderUpdateModeEnum
        /// <summary>
        /// Enum for render update
        /// </summary>
        public enum RenderUpdateMode
        {
            OnEnable, Update, FixedUpdate, Interval, OnRender
        };
        #endregion
    }
}