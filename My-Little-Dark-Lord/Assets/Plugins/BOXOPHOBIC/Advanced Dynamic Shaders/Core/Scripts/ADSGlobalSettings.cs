// Advanced Dynamic Shaders
// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[HelpURL("https://docs.google.com/document/d/1PG_9bb0iiFGoi_yQd8IX0K3sMohuWqyq6_qa4AyVpFw/edit#heading=h.r3n3o9e92m65")]
[DisallowMultipleComponent]
[ExecuteInEditMode]
#endif
public class ADSGlobalSettings : MonoBehaviour
{

    public enum UpdateMode
    {
        Off = 0,
        On = 1,
    };

    [BCategory("Update")]
    public int category_Update;

    public UpdateMode realtimeUpdate = UpdateMode.Off;

    [BCategory("Globals")]
    public int category_Globals;

    public Texture2D globalTexture;
    public Vector4 globalSizeCenter = new Vector4(1, 1, 0, 0);

    public enum GlobalSettingEnum
    {
        Simple = 0,
        Seasons = 1,
    };
    [Space(10)]
    public GlobalSettingEnum globalSettings = GlobalSettingEnum.Simple;

    [BCategory("Globals Simple")]
    public int category_Simple;

    [BInteractive(1)]
    public int InteractiveGlobalsSimple;

    [Range(0, 8)]
    public float globalTintIntensity = 1.0f;
    public Color globalTintColorOne = Color.white;
    public Color globalTintColorTwo = Color.white;
    [Range(0, 1)]
    public float globalLeavesAmount = 1.0f;
    [Range(0, 1)]
    public float globalLeavesVar = 0.0f;
    [Range(0, 1)]
    public float globalSizeMin = 1.0f;
    [Range(0, 1)]
    public float globalSizeMax = 1.0f;

    [BCategory("Globals Seasons (Experimental)")]
    public int category_Seasons;

    [BInteractive(1)]
    public int InteractiveGlobalsSeasons;

    [BRangeOptions(0, 4, new string[] { "Winter", "Spring", "Summer", "Autumn", "Winter" })]
    public float season = 2.0f;

    [Space(10)]
    [Range(0, 8)]
    public float winterTintIntensity = 1.0f;
    public Color winterTintColorOne = Color.white;
    public Color winterTintColorTwo = Color.white;
    [Range(0, 1)]
    public float winterLeavesAmount = 1.0f;
    [Range(0, 1)]
    public float winterLeavesVar = 0.0f;
    [Range(0, 1)]
    public float winterSizeMin = 1.0f;
    [Range(0, 1)]
    public float winterSizeMax = 1.0f;

    [Space(10)]
    [Range(0, 8)]
    public float springTintIntensity = 1.0f;
    public Color springTintColorOne = Color.white;
    public Color springTintColorTwo = Color.white;
    [Range(0, 1)]
    public float springLeavesAmount = 1.0f;
    [Range(0, 1)]
    public float springLeavesVar = 0.0f;
    [Range(0, 1)]
    public float springSizeMin = 1.0f;
    [Range(0, 1)]
    public float springSizeMax = 1.0f;

    [Space(10)]
    [Range(0, 8)]
    public float summerTintIntensity = 1.0f;
    public Color summerTintColorOne = Color.white;
    public Color summerTintColorTwo = Color.white;
    [Range(0, 1)]
    public float summerLeavesAmount = 1.0f;
    [Range(0, 1)]
    public float summerLeavesVar = 0.0f;
    [Range(0, 1)]
    public float summerSizeMin = 1.0f;
    [Range(0, 1)]
    public float summerSizeMax = 1.0f;

    [Space(10)]
    [Range(0, 8)]
    public float autumnTintIntensity = 1.0f;
    public Color autumnTintColorOne = Color.white;
    public Color autumnTintColorTwo = Color.white;
    [Range(0, 1)]
    public float autumnLeavesAmount = 1.0f;
    [Range(0, 1)]
    public float autumnLeavesVar = 0.0f;
    [Range(0, 1)]
    public float autumnSizeMin = 1.0f;
    [Range(0, 1)]
    public float autumnSizeMax = 1.0f;

    void Start()
    {

        // Set gameobject name to be searchable
        gameObject.name = "ADS Global Settings";

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

        //Set Global Texture attributes
        if (globalTexture == null)
        {
            Shader.SetGlobalFloat("ADS_GlobalTex_ON", 0.0f);
        }
        else
        {
            Shader.SetGlobalFloat("ADS_GlobalTex_ON", 1.0f);

            // Set white texture until a texture is added
            if (globalTexture == null)
                Shader.SetGlobalTexture("ADS_GlobalTex", Texture2D.whiteTexture);
            else
                Shader.SetGlobalTexture("ADS_GlobalTex", globalTexture);

            var globalUVsX = 1 / globalSizeCenter.x;
            var globalUVsY = 1 / globalSizeCenter.y;
            var globalUVsZ = (1 / globalSizeCenter.x) * globalSizeCenter.z - 0.5f;
            var globalUVsW = (1 / globalSizeCenter.y) * globalSizeCenter.w - 0.5f;
            Shader.SetGlobalVector("ADS_GlobalUVs", new Vector4(globalUVsX, globalUVsY, globalUVsZ, globalUVsW));
        }

        // Send Global Settings to shaders
        if (globalSettings == GlobalSettingEnum.Simple)
        {
            InteractiveGlobalsSimple = 1;
            InteractiveGlobalsSeasons = 0;

            Shader.SetGlobalFloat("ADS_GlobalTintIntensity", globalTintIntensity);
            Shader.SetGlobalColor("ADS_GlobalTintColorOne", globalTintColorOne);
            Shader.SetGlobalColor("ADS_GlobalTintColorTwo", globalTintColorTwo);
            Shader.SetGlobalFloat("ADS_GlobalLeavesAmount", globalLeavesAmount);
            Shader.SetGlobalFloat("ADS_GlobalLeavesVar", globalLeavesVar);
            Shader.SetGlobalFloat("ADS_GlobalSizeMin", globalSizeMin - 1.0f);
            Shader.SetGlobalFloat("ADS_GlobalSizeMax", globalSizeMax - 1.0f);
        }
        else
        {
            InteractiveGlobalsSimple = 0;
            InteractiveGlobalsSeasons = 1;

            if (season < 0.15f)
                season = 0.0f;
            else if (season > 0.85f && season < 1.15f)
                season = 1.0f;
            else if (season > 1.85f && season < 2.15f)
                season = 2.0f;
            else if (season > 2.85f && season < 3.15f)
                season = 3.0f;
            else if (season > 3.85f)
                season = 4.0f;


            var seasonLerp = 0.0f;
            if (season >= 0 && season < 1)
            {
                seasonLerp = season;
                Shader.SetGlobalFloat("ADS_GlobalTintIntensity", Mathf.Lerp(winterTintIntensity, springTintIntensity, seasonLerp));
                Shader.SetGlobalColor("ADS_GlobalTintColorOne", Color.Lerp(winterTintColorOne, springTintColorOne, seasonLerp));
                Shader.SetGlobalColor("ADS_GlobalTintColorTwo", Color.Lerp(winterTintColorTwo, springTintColorTwo, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalLeavesAmount", Mathf.Lerp(winterLeavesAmount, springLeavesAmount, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalLeavesVar", Mathf.Lerp(winterLeavesVar, springLeavesVar, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalSizeMin", Mathf.Lerp(winterSizeMin - 1.0f, springSizeMin - 1.0f, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalSizeMax", Mathf.Lerp(winterSizeMax - 1.0f, springSizeMax - 1.0f, seasonLerp));
            }
            else if (season >= 1 && season < 2)
            {
                seasonLerp = season - 1.0f;
                Shader.SetGlobalFloat("ADS_GlobalTintIntensity", Mathf.Lerp(springTintIntensity, summerTintIntensity, seasonLerp));
                Shader.SetGlobalColor("ADS_GlobalTintColorOne", Color.Lerp(springTintColorOne, summerTintColorOne, seasonLerp));
                Shader.SetGlobalColor("ADS_GlobalTintColorTwo", Color.Lerp(springTintColorTwo, summerTintColorTwo, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalLeavesAmount", Mathf.Lerp(springLeavesAmount, summerLeavesAmount, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalLeavesVar", Mathf.Lerp(springLeavesVar, summerLeavesVar, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalSizeMin", Mathf.Lerp(springSizeMin - 1.0f, summerSizeMin - 1.0f, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalSizeMax", Mathf.Lerp(springSizeMax - 1.0f, summerSizeMax - 1.0f, seasonLerp));
            }
            else if (season >= 2 && season < 3)
            {
                seasonLerp = season - 2.0f;
                Shader.SetGlobalFloat("ADS_GlobalTintIntensity", Mathf.Lerp(summerTintIntensity, autumnTintIntensity, seasonLerp));
                Shader.SetGlobalColor("ADS_GlobalTintColorOne", Color.Lerp(summerTintColorOne, autumnTintColorOne, seasonLerp));
                Shader.SetGlobalColor("ADS_GlobalTintColorTwo", Color.Lerp(summerTintColorTwo, autumnTintColorTwo, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalLeavesAmount", Mathf.Lerp(summerLeavesAmount, autumnLeavesAmount, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalLeavesVar", Mathf.Lerp(summerLeavesVar, autumnLeavesVar, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalSizeMin", Mathf.Lerp(summerSizeMin - 1.0f, autumnSizeMin - 1.0f, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalSizeMax", Mathf.Lerp(summerSizeMax - 1.0f, autumnSizeMax - 1.0f, seasonLerp));
            }
            else if (season >= 3 && season <= 4)
            {
                seasonLerp = season - 3.0f;
                Shader.SetGlobalFloat("ADS_GlobalTintIntensity", Mathf.Lerp(autumnTintIntensity, winterTintIntensity, seasonLerp));
                Shader.SetGlobalColor("ADS_GlobalTintColorOne", Color.Lerp(autumnTintColorOne, winterTintColorOne, seasonLerp));
                Shader.SetGlobalColor("ADS_GlobalTintColorTwo", Color.Lerp(autumnTintColorTwo, winterTintColorTwo, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalLeavesAmount", Mathf.Lerp(autumnLeavesAmount, winterLeavesAmount, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalLeavesVar", Mathf.Lerp(autumnLeavesVar, winterLeavesVar, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalSizeMin", Mathf.Lerp(autumnSizeMin - 1.0f, winterSizeMin - 1.0f, seasonLerp));
                Shader.SetGlobalFloat("ADS_GlobalSizeMax", Mathf.Lerp(autumnSizeMax - 1.0f, winterSizeMax - 1.0f, seasonLerp));
            }
        }

    }
}
