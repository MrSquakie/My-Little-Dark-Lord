using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Boxophobic;

public class ADSShaderSwitcher : EditorWindow
{

    public string[] SelectionTypeEnum = new string[]
    {
        "Folder",
        "Selected",
    };

    public int selectionType = 0;
    public int selectionTypeOld = -1;

    public string[] ShaderLightingEnumStandard = new string[]
    {
        "-",
        "Simple Lit",
        "Standard Lit",
        "Get From Tag"
    };

    public int materialLighting = 0;
    public int materialLightingOld = -1;

    List<Material> allADSMaterials;
    List<Material> allProjectMaterials;

    string folderPath;
    string folderPathOld = "";
    int selectedObjects = -1;
    int getFromTag;

    Color guiColor;
    string bannerText;
    string helpURL;
    static ADSShaderSwitcher Window;

    [MenuItem("Window/BOXOPHOBIC/Advanced Dynamic Shaders/Shader Switcher")]
    public static void ShowWindow()
    {
        Window = GetWindow<ADSShaderSwitcher>(false, "ADS Shader Switcher", true);
        Window.minSize = new Vector2(480, 320);
        //Window.maxSize = new Vector2(600f, 300f);
    }

    void OnEnable()
    {
        bannerText = "ADS Shader Switcher";
        helpURL = "https://docs.google.com/document/d/1PG_9bb0iiFGoi_yQd8IX0K3sMohuWqyq6_qa4AyVpFw/edit#heading=h.vqkx8theudsl";

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

        folderPath = Application.dataPath;

        getFromTag = ShaderLightingEnumStandard.Length - 1;
    }

    void OnGUI()
    {
        BEditorGUI.DrawWindowBanner(guiColor, bannerText, helpURL);

        GUILayout.BeginHorizontal();
        GUILayout.Space(25);

        GUILayout.BeginVertical();

        DrawMessage();

        DrawInspector();

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
        EditorGUILayout.HelpBox("The ADS Shader Switch tool will change all the selected materials in the project that are using ADS Shaders. When using the Get from Tag option, " +
            "the tool will check the (CustomLit) tag in order to assign the appropriate shader.", MessageType.None, true);
    }

    void DrawInspector()
    {
        GUIStyle stylePopup = new GUIStyle(EditorStyles.popup);
        stylePopup.alignment = TextAnchor.MiddleCenter;

        GUIStyle styleButton = new GUIStyle(EditorStyles.miniButton);
        //stylePopup.alignment = TextAnchor.MiddleCenter;

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent("Materials Selection", ""));
        selectionType = EditorGUILayout.Popup(selectionType, SelectionTypeEnum, stylePopup, GUILayout.MaxHeight(16), GUILayout.Width(160));

        GUILayout.EndHorizontal();

        //////////////////////////////

        GUILayout.BeginHorizontal();

        if (selectionType == 0)
            GUI.enabled = true;
        else
            GUI.enabled = false;

        EditorGUILayout.LabelField(new GUIContent("Materials Folder", ""));
        if (GUILayout.Button(new GUIContent("Assets" + folderPath.Replace(Application.dataPath, ""), "Assets" + folderPath.Replace(Application.dataPath, "")), styleButton, GUILayout.Width(160)))
        {
            folderPath = EditorUtility.OpenFolderPanel("Folder", "", "");
        }

        GUI.enabled = true;

        GUILayout.EndHorizontal();

        //////////////////////////////

        GUILayout.BeginHorizontal();
        if (Selection.objects.Length == 0 && selectionType == 1)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        EditorGUILayout.LabelField(new GUIContent("Materials Lighting", "When choosing a Shader Lighting option, all ADS Material will use the curent option Shader Lighting. " +
                                                  "To go back to the original material, select the Custom option."));
        materialLighting = EditorGUILayout.Popup(materialLighting, ShaderLightingEnumStandard, stylePopup, GUILayout.MaxHeight(16), GUILayout.Width(160));

        GUI.enabled = true;
        GUILayout.EndHorizontal();

        //////////////////////////////

        GUILayout.BeginHorizontal();
        if (materialLighting == 0 || materialLighting == getFromTag || (Selection.objects.Length == 0 && selectionType == 1))
            GUI.enabled = false;
        else
            GUI.enabled = true;

        EditorGUILayout.LabelField(new GUIContent("Materials Lighting Tag to Materials", "If enabled, the curent Shader Lighting option Tag will be applied to all Materials."));

        if (GUILayout.Button("Apply", styleButton, GUILayout.Width(160)))
        {
            ApplyShaderLightingTag();
        }

        GUI.enabled = true;

        GUILayout.EndHorizontal();

        //////////////////////////////

        if (selectionType != selectionTypeOld)
        {
            materialLighting = 0;
            materialLightingOld = -1;
            selectionTypeOld = selectionType;
        }

        if (selectionType == 0)
        {
            if (folderPath == "")
            {
                folderPath = Application.dataPath;
                materialLighting = 0;
                materialLightingOld = -1;
            }

            if (folderPath != folderPathOld)
            {
                materialLighting = 0;
                materialLightingOld = -1;
                folderPathOld = folderPath;
            }
        }
        else if (selectionType == 1)
        {
            if (Selection.objects.Length != selectedObjects)
            {
                materialLighting = 0;
                materialLightingOld = -1;
                selectedObjects = Selection.objects.Length;
            }
        }

        if (materialLighting != 0 && materialLighting != materialLightingOld)
        {
            GetAllADSMaterials();
            ChangeShaderLighting();

            materialLightingOld = materialLighting;
        }
    }

    void GetAllADSMaterials()
    {
        allProjectMaterials = new List<Material>();

        if (selectionType == 0)
        {
            // FindObjectsOfTypeAll not working properly for unloaded assets
            string[] allMatFiles = Directory.GetFiles(folderPath, "*.mat", SearchOption.AllDirectories);

            for (int i = 0; i < allMatFiles.Length; i++)
            {
                string assetPath = "Assets" + allMatFiles[i].Replace(Application.dataPath, "").Replace('\\', '/');
                Material assetMat = (Material)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
                allProjectMaterials.Add(assetMat);
            }
        }
        else if (selectionType == 1)
        {
            for (int i = 0; i < Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets).Length; i++)
            {
                allProjectMaterials.Add((Material)Selection.objects[i]);
            }
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

    void ApplyShaderLightingTag()
    {
        Material material;

        for (int i = 0; i < allADSMaterials.Count; i++)
        {
            material = allADSMaterials[i];

            var materialPath = AssetDatabase.GetAssetPath(material);
            var materialName = material.name;
            var materialRenamed = "";

            var assetName = Path.GetFileName(materialPath);
            var assetPath = materialPath.Replace(assetName, "");

            if (material.name.Contains("Simple Lit"))
            {
                materialRenamed = materialName.Replace("Simple Lit", ShaderLightingEnumStandard[materialLighting]);
            }
            else if (material.name.Contains("Standard Lit"))
            {
                materialRenamed = materialName.Replace("Standard Lit", ShaderLightingEnumStandard[materialLighting]);
            }
            else
            {
                materialRenamed = materialName + " (" + ShaderLightingEnumStandard[materialLighting] + ")";
            }

            AssetDatabase.MoveAsset(materialPath, assetPath + materialRenamed + ".mat");

        }

        AssetDatabase.Refresh();
    }

    void ChangeShaderLighting()
    {
        Material material;

        for (int i = 0; i < allADSMaterials.Count; i++)
        {
            material = allADSMaterials[i];

            if (materialLighting == getFromTag)
            {
                if (material.name.Contains("Simple Lit"))
                {
                    if (material.shader.name.Contains("Cloth"))
                        material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Cloth");
                    else if (material.shader.name.Contains("Generic"))
                        material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Generic");
                    else if (material.shader.name.Contains("Grass"))
                        material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Grass");                    
                    else if (material.shader.name.Contains("Plant"))
                        material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Plant");
                    else if (material.shader.name.Contains("Tree Bark"))
                        material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Tree Bark");
                    else if (material.shader.name.Contains("Tree Leaf"))
                        material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Tree Leaf");
                }
                else if (material.name.Contains("Standard Lit"))
                {
                    if (material.shader.name.Contains("Cloth"))
                        material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Cloth");
                    else if (material.shader.name.Contains("Generic"))
                        material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Generic");
                    else if (material.shader.name.Contains("Grass"))
                        material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Grass");
                    else if (material.shader.name.Contains("Plant"))
                        material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Plant");
                    else if (material.shader.name.Contains("Tree Bark"))
                        material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Tree Bark");
                    else if (material.shader.name.Contains("Tree Leaf"))
                        material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Tree Leaf");
                }
            }
            else
            {
                if (material.shader.name.Contains("Cloth"))
                {
                    material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/" + ShaderLightingEnumStandard[materialLighting] + "/Cloth");
                }
                else if (material.shader.name.Contains("Generic"))
                {
                    material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/" + ShaderLightingEnumStandard[materialLighting] + "/Generic");
                }
                else if (material.shader.name.Contains("Grass"))
                {
                    material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/" + ShaderLightingEnumStandard[materialLighting] + "/Grass");
                }
                else if (material.shader.name.Contains("Plant"))
                {
                    material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/" + ShaderLightingEnumStandard[materialLighting] + "/Plant");
                }
                else if (material.shader.name.Contains("Tree Bark"))
                {
                    material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/" + ShaderLightingEnumStandard[materialLighting] + "/Tree Bark");
                }
                else if (material.shader.name.Contains("Tree Leaf"))
                {
                    material.shader = Shader.Find("BOXOPHOBIC/Advanced Dynamic Shaders/" + ShaderLightingEnumStandard[materialLighting] + "/Tree Leaf");
                }
            }

            SetRenderType(material);
        }
    }

    void SetRenderType(Material material)
    {        
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
    }
}
