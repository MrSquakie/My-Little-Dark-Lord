// Advanced Dynamic Shaders
// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(ADSShaderFeatures))]
public class ADSShaderFeaturesInspector : Editor
{
    //string[] featureVegetationStudioGeneric = new string[]
    //{
    //    "        //Vegetation Studio Support",
    //    "        #include \"../../../../Utils/CGIncludes/VS_indirect.cginc;\"",
    //    "        #pragma instancing_options procedural:setup",
    //    "        #pragma multi_compile GPU_FRUSTUM_ON __"
    //};

    string[] featureVegetationStudioFoliage = new string[]
    {
        "        //Vegetation Studio Support with Grass Scale",
        "        #include \"../../../../Utils/CGIncludes/VS_indirect.cginc\"",
        "        #pragma instancing_options procedural:setupScale",
        "        #pragma multi_compile GPU_FRUSTUM_ON __"
    };

    string[] featureVegetationStudioTrees = new string[]
    {
        "        //Vegetation Studio Support for Trees",
        "        #include \"../../../../Utils/CGIncludes/VS_indirect.cginc\"",
        "        #pragma instancing_options assumeuniformscaling maxcount:50 procedural:setup",
        "        #pragma multi_compile GPU_FRUSTUM_ON __"
    };

    string[] featureGPUInstancer = new string[]
    {
        "        //GPU Instancer Support",
        "        #include \"UnityCG.cginc\"",
        "        #include \"GPUInstancer/Shaders/Include/GPUInstancerInclude.cginc\"",
        "        #pragma instancing_options procedural:setupGPUI"
    };

    string[] featureADSLODFadeDither = new string[]
    {
        "        //ADS LOD Dither Fade Support",
        "        #define ADS_LODFADE_DITHER",
        "        #pragma multi_compile _ LOD_FADE_CROSSFADE"
    };

    string[] featureADSLODFadeScale = new string[]
    {
        "        //ADS LOD Scale Fade Support",
        "        #define ADS_LODFADE_SCALE",
        "        #pragma multi_compile __ LOD_FADE_CROSSFADE",
    };

    //////////////////////////////////////////////////////////////////

    string[] ADSCompatibilityFull = new string[]
    {
        "None",
        "GPU Instancer",
        "Vegetation Studio"
    };

    string[] ADSCompatibilityGPUIOnly = new string[]
    {
        "None",
        "GPU Instancer"
    };

    string[] ADSLODFadeFull = new string[]
    {
        "None",
        "Cross Fade",
        "Scale Fade"
    };

    string[] ADSLODFadeCrossFadeOnly = new string[]
    {
        "None",
        "Cross Fade",
    };

    //string[] excludeProps = new string[] { "m_Script" };

    Color guiColor;
    string bannerText;
    string helpURL;
    ADSShaderFeatures t;

    void OnEnable()
    {
        t = (ADSShaderFeatures)target;

        if (t.ADSShader == null)
        {
            bannerText = "ADS Shader Features";
        }
        else
        {
            bannerText = "ADS " + t.ADSShader.name.Replace("BOXOPHOBIC/Advanced Dynamic Shaders/", "").Replace("/", " ");
        }

        
        helpURL = "https://docs.google.com/document/d/1PG_9bb0iiFGoi_yQd8IX0K3sMohuWqyq6_qa4AyVpFw/edit#heading=h.dnai0aq3nu7d";

        // Check if Light or Dark Unity Skin
        // Set the Banner and Logo Textures
        if (EditorGUIUtility.isProSkin)
        {
            guiColor = BConst.ColorLightGray;
        }
        else
        {
            guiColor = BConst.ColorDarkGray;
        }
    }

    public override void OnInspectorGUI()
    {
        BEditorGUI.DrawBanner(guiColor, bannerText, helpURL);

        DrawCustomInspector();
        DrawButton();

        BEditorGUI.DrawLogo();
    }

    void DrawCustomInspector()
    {
        GUILayout.Space(10);

        t.ADSShader = (Shader)EditorGUILayout.ObjectField("ADS Shader", t.ADSShader, typeof(Shader), false);

        GUILayout.Space(10);

        if (t.ADSShader == null)
        {
            EditorGUILayout.HelpBox("Shader is missing. Please assign the associated shader to continue!", MessageType.Warning);
        }
        else if (t.ADSShader.name.Contains("Advanced Dynamic Shaders") == false)
        {
            EditorGUILayout.HelpBox("Please assign a valid Advanced Dynamic Shaders shader to continue!", MessageType.Warning);
        }
        else
        {
            if (t.ADSShader.name.Contains("Cloth") || t.ADSShader.name.Contains("Generic"))
            {
                t.compatibility = (int)EditorGUILayout.Popup("Compatibility", t.compatibility, ADSCompatibilityGPUIOnly);
            }
            else
            {
                t.compatibility = (int)EditorGUILayout.Popup("Compatibility", t.compatibility, ADSCompatibilityFull);
            }

            if (t.ADSShader.name.Contains("Grass") || t.ADSShader.name.Contains("Plant"))
            {
                t.LODFade = EditorGUILayout.Popup("LOD Fade", t.LODFade, ADSLODFadeFull);
            }
            else
            {
                t.LODFade = EditorGUILayout.Popup("LOD Fade", t.LODFade, ADSLODFadeCrossFadeOnly);
            }
        }

        GUILayout.Space(20);
    }

    void DrawButton()
    {
        if (t.ADSShader == null || t.ADSShader.name.Contains("Advanced Dynamic Shaders") == false)
        {
            GUI.enabled = false;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("");

        if (GUILayout.Button("Compile"))
        {
            InjectShaderFeature();
        }

        GUILayout.Label("");
        GUILayout.EndHorizontal();

        GUI.enabled = true;

        GUILayout.Space(20);
    }

    void InjectShaderFeature()
    {
        string shaderAssetPath = AssetDatabase.GetAssetPath(t.ADSShader);

        int fBegin = 0;
        int fEnd = 0;
        int fLines = 0;
        int fOffset = 0;

        StreamReader reader = new StreamReader(shaderAssetPath);

        List<string> lines = new List<string>();

        int l = 0;

        while (!reader.EndOfStream)
        {
            lines.Add(reader.ReadLine());

            if (lines[l].Contains("ADS Features") && fBegin == 0)
            {
                fBegin = l;
            }

            if (lines[l].Contains("ADS End") && fEnd == 0)
            {
                fEnd = l;
            }

            l++;
        }

        reader.Close();

        fLines = fEnd - fBegin;

        if (fLines > 1)
        {
            lines.RemoveRange(fBegin + 1, fLines - 1);
        }

        if (t.compatibility == 1)
        {
            lines.InsertRange(fBegin + 1 + fOffset, featureGPUInstancer);
            fOffset += featureGPUInstancer.Length;
        }
        else if (t.compatibility == 2)
        {
            if (t.ADSShader.name.Contains("Grass") || t.ADSShader.name.Contains("Plant"))
            {
                lines.InsertRange(fBegin + 1 + fOffset, featureVegetationStudioFoliage);
                fOffset += featureVegetationStudioFoliage.Length;
            }
            else if (t.ADSShader.name.Contains("Tree"))
            {
                lines.InsertRange(fBegin + 1 + fOffset, featureVegetationStudioTrees);
                fOffset += featureVegetationStudioTrees.Length;
            }
        }

        if (t.LODFade == 1)
        {
            lines.InsertRange(fBegin + 1 + fOffset, featureADSLODFadeDither);
            fOffset += featureADSLODFadeDither.Length;
        }
        else if (t.LODFade == 2)
        {
            lines.InsertRange(fBegin + 1 + fOffset, featureADSLODFadeScale);
            fOffset += featureADSLODFadeScale.Length;
        }

        StreamWriter writer = new StreamWriter(shaderAssetPath);

        for (int i = 0; i < lines.Count; i++)
        {
            writer.WriteLine(lines[i]);
        }

        writer.Close();

        lines = new List<string>();

        AssetDatabase.Refresh();
        AssetDatabase.ImportAsset(shaderAssetPath);
        EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<ADSShaderFeatures>(shaderAssetPath.Replace(".shader", ".asset")));
    }
}
