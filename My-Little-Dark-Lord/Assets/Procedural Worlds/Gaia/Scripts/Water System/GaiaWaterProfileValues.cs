using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaia
{
    [System.Serializable]
    public class GaiaWaterProfileValues
    {
        [Header("Global Settings")]
        public string m_typeOfWater = "Deep Blue Ocean";
        public GaiaConstants.GaiaWaterProfileType m_profileType = GaiaConstants.GaiaWaterProfileType.DeepBlueOcean;

        [Header("Post Processing Settings")]
        public string m_postProcessingProfile = "Underwater Post Processing";

        [Header("Underwater Effects")]
        public Color m_underwaterFogColor = Color.cyan;
        public float m_underwaterFogDistance = 45f;
        public float m_underwaterNearFogDistance = -4f;

        [Header("Color Tint Settings")]
        public Color m_waterTint = new Color(0f, 0.73f, 1f, 1f);
        public Color m_shallowTint = new Color(0.08f, 0.29f, 0.35f, 1f);
        [Range(-1f, 1f)]
        public float m_shallowOffset = -0.398f;
        public Color m_depthTint = new Color(0.14f, 0.26f, 0.3f, 1f);
        [Range(0f, 2f)]
        public float m_depthOffset = 0.843f;
        //LWRP
        public Color m_LWwaterTint = new Color(0f, 0.73f, 1f, 1f);
        public Color m_LWshallowTint = new Color(0.08f, 0.29f, 0.35f, 1f);
        [Range(-1f, 1f)]
        public float m_LWshallowOffset = -0.398f;
        public Color m_LWdepthTint = new Color(0.14f, 0.26f, 0.3f, 1f);
        [Range(0f, 2f)]
        public float m_LWdepthOffset = 0.843f;
        //HDRP
        public Color m_HDwaterTint = new Color(0f, 0.73f, 1f, 1f);
        public Color m_HDshallowTint = new Color(0.08f, 0.29f, 0.35f, 1f);
        [Range(-1f, 1f)]
        public float m_HDshallowOffset = -0.398f;
        public Color m_HDdepthTint = new Color(0.14f, 0.26f, 0.3f, 1f);
        [Range(0f, 2f)]
        public float m_HDdepthOffset = 0.843f;

        [Header("Opacity Settings")]
        [Range(0f, 1f)]
        public float m_opacityOcean = 0f;
        [Range(0f, 1f)]
        public float m_opacityBeach = 1f;
        public bool m_ignoreVertexColor = true;
        //LWRP
        [Range(0f, 1f)]
        public float m_LWopacityOcean = 0f;
        [Range(0f, 1f)]
        public float m_LWopacityBeach = 1f;
        public bool m_LWignoreVertexColor = true;
        //HDRP
        [Range(0f, 1f)]
        public float m_HDopacityOcean = 0f;
        [Range(0f, 1f)]
        public float m_HDopacityBeach = 1f;
        public bool m_HDignoreVertexColor = true;

        [Header("Main Settings")]
        [Range(0f, 1f)]
        public float m_occlusion = 0.212f;
        [Range(0f, 1f)]
        public float m_metallic = 0f;
        [Range(0f, 1f)]
        public float m_smoothness = 0.308f;
        [Range(0f, 1f)]
        public float m_smoothnessVariance = 0.031f;
        [Range(0f, 1f)]
        public float m_smoothnessThreshold = 0.332f;
        //LWRP
        [Range(0f, 1f)]
        public float m_LWocclusion = 0.212f;
        [Range(0f, 1f)]
        public float m_LWmetallic = 0f;
        [Range(0f, 1f)]
        public float m_LWsmoothness = 0.308f;
        [Range(0f, 1f)]
        public float m_LWsmoothnessVariance = 0.031f;
        [Range(0f, 1f)]
        public float m_LWsmoothnessThreshold = 0.332f;
        //HDRP
        [Range(0f, 1f)]
        public float m_HDocclusion = 0.212f;
        [Range(0f, 1f)]
        public float m_HDmetallic = 0f;
        [Range(0f, 1f)]
        public float m_HDsmoothness = 0.308f;
        [Range(0f, 1f)]
        public float m_HDsmoothnessVariance = 0.031f;
        [Range(0f, 1f)]
        public float m_HDsmoothnessThreshold = 0.332f;

        [Header("Refraction Settings")]
        [Range(0f, 50f)]
        public float m_refractedDepth = 50f;
        [Range(0f, 1f)]
        public float m_refractionScale = 1f;
        //LWRP
        [Range(0f, 50f)]
        public float m_LWrefractedDepth = 50f;
        [Range(0f, 1f)]
        public float m_LWrefractionScale = 1f;
        //HDRP
        [Range(0f, 50f)]
        public float m_HDrefractedDepth = 50f;
        [Range(0f, 1f)]
        public float m_HDrefractionScale = 1f;

        [Header("Lighting Settings")]
        [Range(0f, 1f)]
        public float m_indirectLightSpecular = 0.566f;
        [Range(0f, 1f)]
        public float m_indirectLightDiffuse = 0.586f;
        public Color m_highlightTint = Color.white;
        [Range(-1f, -0.5f)]
        public float m_highlightOffset = -1f;
        [Range(0f, 1f)]
        public float m_highlightSharpness = 0.162f;
        //LWRP
        [Range(0f, 1f)]
        public float m_LWindirectLightSpecular = 0.566f;
        [Range(0f, 1f)]
        public float m_LWindirectLightDiffuse = 0.586f;
        public Color m_LWhighlightTint = Color.white;
        [Range(-1f, -0.5f)]
        public float m_LWhighlightOffset = -1f;
        [Range(0f, 1f)]
        public float m_LWhighlightSharpness = 0.162f;
        //HDRP
        [Range(0f, 1f)]
        public float m_HDindirectLightSpecular = 0.566f;
        [Range(0f, 1f)]
        public float m_HDindirectLightDiffuse = 0.586f;
        public Color m_HDhighlightTint = Color.white;
        [Range(-1f, -0.5f)]
        public float m_HDhighlightOffset = -1f;
        [Range(0f, 1f)]
        public float m_HDhighlightSharpness = 0.162f;

        [Header("Shadow Settings")]
        [Range(0f, 1f)]
        public float m_shadowStrength = 0f;
        [Range(0f, 1f)]
        public float m_shadowSharpness = 0.01f;
        [Range(-1f, 0f)]
        public float m_shadowOffset = -1f;
        //LWRP
        [Range(0f, 1f)]
        public float m_LWshadowStrength = 0f;
        [Range(0f, 1f)]
        public float m_LWshadowSharpness = 0.01f;
        [Range(-1f, 0f)]
        public float m_LWshadowOffset = -1f;
        //HDRP
        [Range(0f, 1f)]
        public float m_HDshadowStrength = 0f;
        [Range(0f, 1f)]
        public float m_HDshadowSharpness = 0.01f;
        [Range(-1f, 0f)]
        public float m_HDshadowOffset = -1f;

        [Header("Reflection Settings")]
        [Range(0f, 1f)]
        public float m_reflectionIntenisty = 0.41f;
        [Range(0f, 1f)]
        public float m_reflectionWobble = 0.062f;
        [Range(0f, 10f)]
        public float m_reflectionFresnelPower = 1.7f;
        [Range(0f, 1f)]
        public float m_reflectionFresnelScale = 0.45f;
        //LWRP
        [Range(0f, 1f)]
        public float m_LWreflectionIntenisty = 0.41f;
        [Range(0f, 1f)]
        public float m_LWreflectionWobble = 0.062f;
        [Range(0f, 10f)]
        public float m_LWreflectionFresnelPower = 1.7f;
        [Range(0f, 1f)]
        public float m_LWreflectionFresnelScale = 0.45f;
        //HDRP
        [Range(0f, 1f)]
        public float m_HDreflectionIntenisty = 0.41f;
        [Range(0f, 1f)]
        public float m_HDreflectionWobble = 0.062f;
        [Range(0f, 10f)]
        public float m_HDreflectionFresnelPower = 1.7f;
        [Range(0f, 1f)]
        public float m_HDreflectionFresnelScale = 0.45f;

        [Header("Normal Map Settings")]
        public Texture2D m_normalMap;
        [Range(0f, 1f)]
        public float m_normalStrength = 0.411f;
        [Range(0f, 10000f)]
        public float m_normalTiling = 84.4f;
        [Range(0f, 0.1f)]
        public float m_normalSpeed = 0.1f;
        [Range(0f, 1f)]
        public float m_normalTimescale = 0.213f;
        //LWRP
        public Texture2D m_LWnormalMap;
        [Range(0f, 1f)]
        public float m_LWnormalStrength = 0.411f;
        [Range(0f, 10000f)]
        public float m_LWnormalTiling = 84.4f;
        [Range(0f, 0.1f)]
        public float m_LWnormalSpeed = 0.1f;
        [Range(0f, 1f)]
        public float m_LWnormalTimescale = 0.213f;
        //HDRP
        public Texture2D m_HDnormalMap;
        [Range(0f, 1f)]
        public float m_HDnormalStrength = 0.411f;
        [Range(0f, 10000f)]
        public float m_HDnormalTiling = 84.4f;
        [Range(0f, 0.1f)]
        public float m_HDnormalSpeed = 0.1f;
        [Range(0f, 1f)]
        public float m_HDnormalTimescale = 0.213f;

        [Header("Wave Tessellation")]
        [Range(0f, 1f)]
        public float m_tessellationAmount = 0.374f;
        public float m_tessellationDistance = 5000f;
        //LWRP
        [Range(0f, 1f)]
        public float m_LWtessellationAmount = 0.374f;
        public float m_LWtessellationDistance = 5000f;
        //HDRP
        [Range(0f, 1f)]
        public float m_HDtessellationAmount = 0.374f;
        public float m_HDtessellationDistance = 5000f;

        [Header("Wave Direction Settings")]
        public Vector4 m_wave01 = new Vector4(1f, 0f, 0.15f, 50f);
        public Vector4 m_wave02 = new Vector4(-1f, -0.5f, 0.15f, 45f);
        public Vector4 m_wave03 = new Vector4(1f, -1f, 0.15f, 45f);
        [Range(0f, 5f)]
        public float m_waveSpeed = 0.402f;
        //LWRP
        public Vector4 m_LWwave01 = new Vector4(1f, 0f, 0.15f, 50f);
        public Vector4 m_LWwave02 = new Vector4(-1f, -0.5f, 0.15f, 45f);
        public Vector4 m_LWwave03 = new Vector4(1f, -1f, 0.15f, 45f);
        [Range(0f, 5f)]
        public float m_LWwaveSpeed = 0.402f;
        //HDRP
        public Vector4 m_HDwave01 = new Vector4(1f, 0f, 0.15f, 50f);
        public Vector4 m_HDwave02 = new Vector4(-1f, -0.5f, 0.15f, 45f);
        public Vector4 m_HDwave03 = new Vector4(1f, -1f, 0.15f, 45f);
        [Range(0f, 5f)]
        public float m_HDwaveSpeed = 0.402f;

        [Header("Ocean Foam Settings")]
        public Texture2D m_oceanFoamTexture;
        public Color m_oceanFoamTint = Color.white;
        [Range(0f, 100f)]
        public float m_oceanFoamTiling = 11.9f;
        [Range(0f, 1f)]
        public float m_oceanFoamAmount = 0.03f;
        [Range(0f, 1000f)]
        public float m_oceanFoamDistance = 66f;
        [Range(0f, 1f)]
        public float m_oceanFoamSpeed = 0.03f;
        //LWRP
        public Texture2D m_LWoceanFoamTexture;
        public Color m_LWoceanFoamTint = Color.white;
        [Range(0f, 100f)]
        public float m_LWoceanFoamTiling = 11.9f;
        [Range(0f, 1f)]
        public float m_LWoceanFoamAmount = 0.03f;
        [Range(0f, 1000f)]
        public float m_LWoceanFoamDistance = 66f;
        [Range(0f, 1f)]
        public float m_LWoceanFoamSpeed = 0.03f;
        //HDRP
        public Texture2D m_HDoceanFoamTexture;
        public Color m_HDoceanFoamTint = Color.white;
        [Range(0f, 100f)]
        public float m_HDoceanFoamTiling = 11.9f;
        [Range(0f, 1f)]
        public float m_HDoceanFoamAmount = 0.03f;
        [Range(0f, 1000f)]
        public float m_HDoceanFoamDistance = 66f;
        [Range(0f, 1f)]
        public float m_HDoceanFoamSpeed = 0.03f;

        [Header("Beach Foam Settings")]
        public Texture2D m_beachFoamTexture;
        public Color m_beachFoamTint = Color.white;
        [Range(0f, 1f)]
        public float m_beachFoamTiling = 0.593f;
        [Range(0f, 1f)]
        public float m_beachFoamAmount = 0.0358f;
        [Range(0f, 100f)]
        public float m_beachFoamDistance = 30.1f;
        [Range(0f, 1f)]
        public float m_beachFoamSpeed = 0.066f;
        //LWRP
        public Texture2D m_LWbeachFoamTexture;
        public Color m_LWbeachFoamTint = Color.white;
        [Range(0f, 1f)]
        public float m_LWbeachFoamTiling = 0.593f;
        [Range(0f, 1f)]
        public float m_LWbeachFoamAmount = 0.0358f;
        [Range(0f, 100f)]
        public float m_LWbeachFoamDistance = 30.1f;
        [Range(0f, 1f)]
        public float m_LWbeachFoamSpeed = 0.066f;
        //HDRP
        public Texture2D m_HDbeachFoamTexture;
        public Color m_HDbeachFoamTint = Color.white;
        [Range(0f, 1f)]
        public float m_HDbeachFoamTiling = 0.593f;
        [Range(0f, 1f)]
        public float m_HDbeachFoamAmount = 0.0358f;
        [Range(0f, 100f)]
        public float m_HDbeachFoamDistance = 30.1f;
        [Range(0f, 1f)]
        public float m_HDbeachFoamSpeed = 0.066f;
    }
}