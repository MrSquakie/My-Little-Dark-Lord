// Advanced Dynamic Shaders
// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[HelpURL("https://docs.google.com/document/d/1PG_9bb0iiFGoi_yQd8IX0K3sMohuWqyq6_qa4AyVpFw/edit#heading=h.b6n5ylhvvzzk")]
[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
#endif
public class ADSGlobalMotion : MonoBehaviour
{

    public enum UpdateMode
    {
        Off = 0,
        On = 1,
    };

    [BCategory("Update")]
    public int category_Update;

    public UpdateMode realtimeUpdate = UpdateMode.Off;

    [BMessage("Warning", "Please note that Motion Speed, Turbulence Speed and Turbulence Scale won't work properly with contiunous realtime update!", 5, 0)]
    public bool message_Update;

    [BCategory("Motion")]
    public int category_Motion;

    public float motionAmplitude = 1.0f;
    public float motionSpeed = 1.0f;
    public float motionScale = 1.0f;

    [BCategory("Turbulence")]
    public int category_Turbulence;

    public Texture2D turbulenceTexture;
    public float turbulenceContrast = 1.0f;
    public float turbulenceSpeed = 1.0f;
    public float turbulenceScale = 1.0f;

#if UNITY_EDITOR
    [HideInInspector]
    public Mesh arrowMesh;
    [HideInInspector]
    public Material arrowMaterial;

    private Shader debugShader;
#endif

    void Start()
    {

#if UNITY_EDITOR
        gameObject.GetComponent<MeshFilter>().mesh = arrowMesh;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = arrowMaterial;

        // Get legacy ADS Globals Settings
        //if (GameObject.Find("ADS Globals") != null)
        //{
        //    var legacyADSGlobals = GameObject.Find("ADS Globals").GetComponent<ADSGlobals>();

        //    motionAmplitude = legacyADSGlobals.motionAmplitude;
        //    motionSpeed = legacyADSGlobals.motionSpeed;
        //    motionScale = legacyADSGlobals.motionScale;

        //    turbulenceTexture = legacyADSGlobals.turbulenceTexture;
        //    turbulenceContrast = legacyADSGlobals.turbulenceContrast;
        //    turbulenceSpeed = legacyADSGlobals.turbulenceSpeed;
        //    turbulenceScale = legacyADSGlobals.turbulenceScale;

        //    GameObject.Find("ADS Globals").SetActive(false);
        //}
#endif

        // Set gameobject name to be searchable
        gameObject.name = "ADS Global Motion";

        // Disable Arrow in play mode
        if (Application.isPlaying == true)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

        // Send global information to shaders
        SetGlobalShaderProperties();

    }


    void Update()
    {

        #if UNITY_EDITOR
        if (Selection.Contains(gameObject) == true)
        {
            if (Application.isPlaying == true)
            {
                if (realtimeUpdate == UpdateMode.On)
                {
                    SetGlobalShaderProperties();
                }
            }
            else
            {
                SetGlobalShaderProperties();
            }
            
        }

        if (realtimeUpdate == UpdateMode.Off)
        {
            message_Update = false;
        }
        else
        {
            message_Update = true;
        }

        if (Application.isEditor)        
            return;               
        #endif

        if (realtimeUpdate == UpdateMode.Off)        
            return;        
        else
            SetGlobalShaderProperties();

    }

    // Send global information to shaders
    void SetGlobalShaderProperties()
    {

        // Send Motion parameters to shaders
        Shader.SetGlobalVector("ADS_GlobalDirection", gameObject.transform.forward);
        Shader.SetGlobalFloat("ADS_GlobalAmplitude", motionAmplitude);
        Shader.SetGlobalFloat("ADS_GlobalSpeed", motionSpeed);
        Shader.SetGlobalFloat("ADS_GlobalScale", motionScale);

        // Send Turbulence parameters to shaders
        if (turbulenceTexture == null || turbulenceContrast <= 0)
        {
            Shader.SetGlobalFloat("ADS_TurbulenceTex_ON", 0.0f);
        }
        else
        {
            Shader.SetGlobalFloat("ADS_TurbulenceTex_ON", 1.0f);

            // Set white texture until a texture is added
            if (turbulenceTexture == null)            
                Shader.SetGlobalTexture("ADS_TurbulenceTex", Texture2D.whiteTexture);
            else
                Shader.SetGlobalTexture("ADS_TurbulenceTex", turbulenceTexture);

            Shader.SetGlobalFloat("ADS_TurbulenceContrast", turbulenceContrast);
            Shader.SetGlobalFloat("ADS_TurbulenceSpeed", turbulenceSpeed * 0.1f);
            Shader.SetGlobalFloat("ADS_TurbulenceScale", turbulenceScale * 0.1f);
        }

    }
}
