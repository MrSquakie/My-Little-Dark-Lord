// Advanced Dynamic Shaders
// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic;

public class ADSShaderGUI : ShaderGUI
{

    private Material material;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {

        material = materialEditor.target as Material;

        if (material == null)
            return;

        base.OnGUI(materialEditor, props);

        // Auto assign material props from Standard to ADS
        BEditorUtils.UnityToBoxophobicProperties(material);

        SetRenderType();

        // Unused GI flags
        //materialEditor.LightmapEmissionProperty(0);
        //foreach (Material target in materialEditor.targets)
        //{
        //    target.globalIlluminationFlags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        //}

    }

    void SetRenderType()
    {

        // Set Render Type
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

    void SetMotionType()
    {

        // Set Motion Type
        if (material.HasProperty("_MotionType"))
        {
            var motionType = material.GetFloat("_MotionType");

            if (motionType == 0)
            {
                material.EnableKeyword("_MOTIONTYPEKEY_MOTION");
                material.DisableKeyword("_MOTIONTYPEKEY_MOTION_MOTION2");
                material.DisableKeyword("_MOTIONTYPEKEY_MOTION_MOTION3");
                material.DisableKeyword("_MOTIONTYPEKEY_MOTION_MOTION2_MOTION3");
            }

            if (motionType == 1)
            {
                material.DisableKeyword("_MOTIONTYPEKEY_MOTION");
                material.EnableKeyword("_MOTIONTYPEKEY_MOTION_MOTION2");
                material.DisableKeyword("_MOTIONTYPEKEY_MOTION_MOTION3");
                material.DisableKeyword("_MOTIONTYPEKEY_MOTION_MOTION2_MOTION3");
            }

            if (motionType == 2)
            {
                material.DisableKeyword("_MOTIONTYPEKEY_MOTION");
                material.DisableKeyword("_MOTIONTYPEKEY_MOTION_MOTION2");
                material.EnableKeyword("_MOTIONTYPEKEY_MOTION_MOTION3");
                material.DisableKeyword("_MOTIONTYPEKEY_MOTION_MOTION2_MOTION3");
            }

            if (motionType == 3)
            {
                material.DisableKeyword("_MOTIONTYPEKEY_MOTION");
                material.DisableKeyword("_MOTIONTYPEKEY_MOTION_MOTION2");
                material.DisableKeyword("_MOTIONTYPEKEY_MOTION_MOTION3");
                material.EnableKeyword("_MOTIONTYPEKEY_MOTION_MOTION2_MOTION3");
            }
        }
    }

    void SetTreeBranchAndBase()
    {

        // Set Tree Branch and Base
        if (material.HasProperty("_EnableBranch") && material.HasProperty("_EnableBase"))
        {
            var enableBranch = material.GetFloat("_EnableBranch");
            var enableBase = material.GetFloat("_EnableBase");

            if (enableBranch == 0 && enableBase == 0)
            {
                material.EnableKeyword("_BLENDING_OFF");
                material.DisableKeyword("_BLENDING_BRANCH");
                material.DisableKeyword("_BLENDING_BASE");
                material.DisableKeyword("_BLENDING_BRANCHANDBASE");
            }
            else if (enableBranch == 1 && enableBase == 0)
            {
                material.DisableKeyword("_BLENDING_OFF");
                material.EnableKeyword("_BLENDING_BRANCH");
                material.DisableKeyword("_BLENDING_BASE");
                material.DisableKeyword("_BLENDING_BRANCHANDBASE");
            }
            else if (enableBranch == 0 && enableBase == 1)
            {
                material.DisableKeyword("_BLENDING_OFF");
                material.DisableKeyword("_BLENDING_BRANCH");
                material.EnableKeyword("_BLENDING_BASE");
                material.DisableKeyword("_BLENDING_BRANCHANDBASE");
            }
            else if (enableBranch == 1 && enableBase == 1)
            {
                material.DisableKeyword("_BLENDING_OFF");
                material.DisableKeyword("_BLENDING_BRANCH");
                material.DisableKeyword("_BLENDING_BASE");
                material.EnableKeyword("_BLENDING_BRANCHANDBASE");
            }
        }
    }

}
