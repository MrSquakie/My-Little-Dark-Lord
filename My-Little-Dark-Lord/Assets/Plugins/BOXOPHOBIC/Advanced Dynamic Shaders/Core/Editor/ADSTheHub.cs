// Advanced Dynamic Shaders
// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic;
using System.IO;
using System.Collections.Generic;

public class ADSTheHub : EditorWindow
{
    Color guiColor;
    string bannerText;
    string helpURL;

    string ADSVersionString = "2.3.3";
    static ADSTheHub Window;

    ADSShaderFeatures shaderFeaturesObject;
    List<Material> allProjectMaterials;
    List<Material> allADSMaterials;

    [MenuItem("Window/BOXOPHOBIC/Advanced Dynamic Shaders/The Hub")]
    public static void ShowWindow()
    {
        Window = GetWindow<ADSTheHub>(false, "ADS Hub", true);
        Window.minSize = new Vector2(480, 260);
        //Window.maxSize = new Vector2(600f, 300f);
    }

    void OnEnable()
    {
        bannerText = "ADS Hub " + ADSVersionString;
        helpURL = "https://docs.google.com/document/d/1PG_9bb0iiFGoi_yQd8IX0K3sMohuWqyq6_qa4AyVpFw/edit#heading=h.496xnfjj0nc3";

        // Check if Light or Dark Unity Skin
        // Set the Banner and Logo Textures
        if (EditorGUIUtility.isProSkin)
        {
            guiColor = new Color(1f, 0.754f, 0.186f);
        }
        else
        {
            guiColor = BConst.ColorDarkGray;
        }
    }

    void OnGUI()
    {
        BEditorGUI.DrawWindowBanner(guiColor, bannerText, helpURL);

        GUILayout.BeginHorizontal();
        GUILayout.Space(25);

        GUILayout.BeginVertical();

        DrawMessage();

        DrawButton();

        GUILayout.EndVertical();

        GUILayout.Space(20);
        GUILayout.EndHorizontal();


        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        BEditorGUI.DrawLogo();

        GUILayout.FlexibleSpace();
        GUILayout.Space(20);
        GUILayout.EndVertical();
    }

    void DrawMessage()
    {
        EditorGUILayout.HelpBox("The ADS Hub tool will upgrade your project to version " + ADSVersionString + " and it will create all the necessary assets for Advanced Dynamic Shaders to run smoothly!", MessageType.Info, true);
    }

    void DrawButton()
    {
        //GUIStyle styleButton = new GUIStyle(EditorStyles.miniButton);

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("");

        if (GUILayout.Button("Setup ADS", /*styleButton,*/ GUILayout.Width(140)))
        {
            GetAllADSMaterials();

            Update211To220();
            Update22XTo230();
            Update231To232();

            GenerateShaderFeatureAssets();

            allProjectMaterials = new List<Material>();
            allADSMaterials = new List<Material>();

            DestroyAutorun();

            AssetDatabase.Refresh();

            Debug.Log("[ADS] Advanced Dynamic Shaders " + ADSVersionString + " installed!");
        }

        GUILayout.Label("");
        GUILayout.EndHorizontal();
    }

    void GetAllADSMaterials()
    {
        allProjectMaterials = new List<Material>();

        // FindObjectsOfTypeAll not working properly for unloaded assets
        string[] allMatFiles = Directory.GetFiles(Application.dataPath, "*.mat", SearchOption.AllDirectories);

        for (int i = 0; i < allMatFiles.Length; i++)
        {
            string assetPath = "Assets" + allMatFiles[i].Replace(Application.dataPath, "").Replace('\\', '/');
            Material assetMat = (Material)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
            allProjectMaterials.Add(assetMat);
        }

        allADSMaterials = new List<Material>();

        for (int i = 0; i < allProjectMaterials.Count; i++)
        {
            if (allProjectMaterials[i] != null)
            {
                if (allProjectMaterials[i].shader.name.Contains("Advanced Dynamic Shaders"))
                {
                    allADSMaterials.Add(allProjectMaterials[i]);
                }
            }
        }

        //Debug.Log(allADSMaterials.Count + " ADS Materials Found");
    }

    void Update211To220()
    {
        for (int i = 0; i < allADSMaterials.Count; i++)
        {
            if (allADSMaterials[i] != null)
            {
                Material material = allADSMaterials[i];

                if (material.shader == Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Cloth") ||
                    material.shader == Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Generic") ||
                    material.shader == Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Cloth") ||
                    material.shader == Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Generic") ||
                    material.shader == Shader.Find("Hidden/BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Grass (Legacy)") ||
                    material.shader == Shader.Find("Hidden/BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Grass (Legacy)"))
                {

                    if (material.GetFloat("_Internal_UnityToBoxophobic") != 1)
                    {
                        if (material.HasProperty("_Mode"))
                        {
                            material.SetFloat("_RenderType", material.GetFloat("_Mode"));
                        }

                        if (material.HasProperty("_CullMode"))
                        {
                            material.SetFloat("_RenderFaces", material.GetFloat("_CullMode"));
                        }

                        //Main set on inputs
                        if (material.HasProperty("_MainUVs"))
                        {
                            material.SetVector("_UVZero", material.GetVector("_MainUVs"));
                        }

                        if (material.HasProperty("_MainTex"))
                        {
                            material.SetTexture("_AlbedoTex", material.GetTexture("_MainTex"));
                        }

                        if (material.HasProperty("_BumpMap"))
                        {
                            material.SetTexture("_NormalTex", material.GetTexture("_BumpMap"));
                        }

                        if (material.HasProperty("_MetallicGlossMap"))
                        {
                            material.SetTexture("_SurfaceTex", material.GetTexture("_MetallicGlossMap"));
                        }

                        if (material.HasProperty("_BumpScale"))
                        {
                            material.SetFloat("_NormalScale", material.GetFloat("_BumpScale"));
                        }

                        if (material.HasProperty("_Glossiness"))
                        {
                            material.SetFloat("_Smoothness", material.GetFloat("_Glossiness"));
                        }

                        if (material.HasProperty("_MotionNoise"))
                        {
                            material.SetFloat("_GlobalTurbulence", material.GetFloat("_MotionNoise"));
                        }

                        // Upgrade RenderMode without selecting the material
                        if (material.HasProperty("_RenderType"))
                        {
                            var renderType = material.GetFloat("_RenderType");

                            if (renderType == 0)
                            {
                                material.SetOverrideTag("RenderType", "");
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                                material.SetInt("_ZWrite", 1);
                                material.renderQueue = -1;

                                material.EnableKeyword("_RENDERTYPEKEY_OPAQUE");
                                material.DisableKeyword("_RENDERTYPEKEY_CUT");
                                material.DisableKeyword("_RENDERTYPEKEY_FADE");
                                material.DisableKeyword("_RENDERTYPEKEY_TRANSPARENT");
                            }
                            else if (renderType == 1)
                            {
                                material.SetOverrideTag("RenderType", "TransparentCutout");
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                                material.SetInt("_ZWrite", 1);
                                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;

                                material.DisableKeyword("_RENDERTYPEKEY_OPAQUE");
                                material.EnableKeyword("_RENDERTYPEKEY_CUT");
                                material.DisableKeyword("_RENDERTYPEKEY_FADE");
                                material.DisableKeyword("_RENDERTYPEKEY_TRANSPARENT");
                            }
                            else if (renderType == 2)
                            {
                                material.SetOverrideTag("RenderType", "Transparent");
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                material.SetInt("_ZWrite", 0);
                                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                                material.DisableKeyword("_RENDERTYPEKEY_OPAQUE");
                                material.DisableKeyword("_RENDERTYPEKEY_CUT");
                                material.EnableKeyword("_RENDERTYPEKEY_FADE");
                                material.DisableKeyword("_RENDERTYPEKEY_TRANSPARENT");
                            }
                            else if (renderType == 3)
                            {
                                material.SetOverrideTag("RenderType", "Transparent");
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                material.SetInt("_ZWrite", 0);
                                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                                material.DisableKeyword("_RENDERTYPEKEY_OPAQUE");
                                material.DisableKeyword("_RENDERTYPEKEY_CUT");
                                material.DisableKeyword("_RENDERTYPEKEY_FADE");
                                material.EnableKeyword("_RENDERTYPEKEY_TRANSPARENT");
                            }
                        }

                        // Since ADS Legacy properties have the same name as Unity propeties this can be set to 1
                        material.SetFloat("_Internal_UnityToBoxophobic", 1);

                        // Debug chnaged Materials
                        Debug.Log("[ADS] " + material.name + " - Updated");
                    }
                }
            }
        }
    }

    void Update22XTo230()
    {
        for (int i = 0; i < allADSMaterials.Count; i++)
        {
            if (allADSMaterials[i] != null)
            {
                Material material = allADSMaterials[i];

                // Change Advanced Lit tag to Standard Lit tag
                if (material.shader == Shader.Find("Hidden/BOXOPHOBIC/Advanced Dynamic Shaders/Advanced Lit/Grass"))
                {
                    material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Grass");
                    Debug.Log("[ADS] " + material.name + " - Changed Advanced Lit to Standard Lit");
                }
                else if (material.shader == Shader.Find("Hidden/BOXOPHOBIC/Advanced Dynamic Shaders/Advanced Lit/Plant"))
                {
                    material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Plant");
                    Debug.Log("[ADS] " + material.name + " - Changed Advanced Lit to Standard Lit");
                }
                else if (material.shader == Shader.Find("Hidden/BOXOPHOBIC/Advanced Dynamic Shaders/Advanced Lit/Tree Leaf"))
                {
                    material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Tree Leaf");
                    Debug.Log("[ADS] " + material.name + " - Changed Advanced Lit to Standard Lit");
                }


                // Change Advanced Lit tag to Standard Lit tag
                var materialPath = AssetDatabase.GetAssetPath(material);
                var materialName = material.name;
                var materialRenamed = "";

                var assetName = Path.GetFileName(materialPath);
                var assetPath = materialPath.Replace(assetName, "");

                if (material.name.Contains("Advanced Lit"))
                {
                    materialRenamed = materialName.Replace("Advanced Lit", "Standard Lit");
                    AssetDatabase.MoveAsset(materialPath, assetPath + materialRenamed + ".mat");
                }
            }
        }

        FileUtil.DeleteFileOrDirectory(BEditorUtils.GetBoxophobicFolder() + "/Advanced Dynamic Shaders/Core/Legacy");
        FileUtil.DeleteFileOrDirectory(BEditorUtils.GetBoxophobicFolder() + "/Advanced Dynamic Shaders/Core/Legacy.meta");
    }

    void Update231To232()
    {
        for (int i = 0; i < allADSMaterials.Count; i++)
        {
            if (allADSMaterials[i] != null)
            {
                Material material = allADSMaterials[i];

                if (material.shader == Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Generic") ||
                    material.shader == Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Generic"))
                {
                    if (material.HasProperty("_Internal_Version") && material.GetInt("_Internal_Version") < 232)
                    {
                        if (material.HasProperty("_LocalDirection"))
                        {
                            material.SetVector("_MotionLocalDirection", material.GetVector("_LocalDirection"));
                            material.SetInt("_Internal_Version", 232);

                            // Debug chnaged Materials
                            Debug.Log("[ADS] " + material.name + " - Updated");
                        }
                    }
                }
            }
        }
    }

    void GenerateShaderFeatureAssets()
    {
        string boxophobicFolder = BEditorUtils.GetBoxophobicFolder();
        string simpleLitPath = "/Advanced Dynamic Shaders/Core/Shaders/Simple Lit/Simple Lit ";
        string standardLitPath = "/Advanced Dynamic Shaders/Core/Shaders/Standard Lit/Standard Lit ";
        string[] shaderTypes = new string[] { "Cloth", "Generic", "Grass", "Plant", "Tree Bark", "Tree Leaf" };

        for (int i = 0; i < 6; i++)
        {
            if (AssetDatabase.LoadAssetAtPath<Shader>(boxophobicFolder + simpleLitPath + shaderTypes[i] + ".shader") != null)
            {
                if (AssetDatabase.LoadAssetAtPath<ADSShaderFeatures>(boxophobicFolder + simpleLitPath + shaderTypes[i] + ".asset") == null)
                {
                    var shader = AssetDatabase.LoadAssetAtPath<Shader>(boxophobicFolder + simpleLitPath + shaderTypes[i] + ".shader");

                    var instanceAsset = CreateInstance(typeof(ADSShaderFeatures));
                    AssetDatabase.CreateAsset(instanceAsset, boxophobicFolder + simpleLitPath + shaderTypes[i] + ".asset");

                    AssetDatabase.SaveAssets();

                    var featuresAsset = AssetDatabase.LoadAssetAtPath<ADSShaderFeatures>(boxophobicFolder + simpleLitPath + shaderTypes[i] + ".asset");
                    featuresAsset.ADSShader = shader;

                    EditorUtility.SetDirty(featuresAsset);
                }
            }
        }

        for (int i = 0; i < 6; i++)
        {
            if (AssetDatabase.LoadAssetAtPath<Shader>(boxophobicFolder + standardLitPath + shaderTypes[i] + ".shader") != null)
            {
                if (AssetDatabase.LoadAssetAtPath<ADSShaderFeatures>(boxophobicFolder + standardLitPath + shaderTypes[i] + ".asset") == null)
                {
                    var shader = AssetDatabase.LoadAssetAtPath<Shader>(boxophobicFolder + standardLitPath + shaderTypes[i] + ".shader");

                    var instanceAsset = CreateInstance(typeof(ADSShaderFeatures));
                    AssetDatabase.CreateAsset(instanceAsset, boxophobicFolder + standardLitPath + shaderTypes[i] + ".asset");
                    AssetDatabase.SaveAssets();

                    var featuresAsset = AssetDatabase.LoadAssetAtPath<ADSShaderFeatures>(boxophobicFolder + standardLitPath + shaderTypes[i] + ".asset");
                    featuresAsset.ADSShader = shader;

                    EditorUtility.SetDirty(featuresAsset);
                }
            }
        }
    }

    void DestroyAutorun()
    {
        FileUtil.DeleteFileOrDirectory(BEditorUtils.GetBoxophobicFolder() + "/Advanced Dynamic Shaders/Core/Editor/ADSTheHubAutorun.cs");
        FileUtil.DeleteFileOrDirectory(BEditorUtils.GetBoxophobicFolder() + "/Advanced Dynamic Shaders/Core/Editor/ADSTheHubAutorun.cs.meta");
    }
}
