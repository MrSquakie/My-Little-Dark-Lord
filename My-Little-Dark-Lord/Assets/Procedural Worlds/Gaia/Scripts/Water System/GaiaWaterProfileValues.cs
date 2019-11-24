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

        [Header("Color Tint Settings")]
        public Color m_waterTint = new Color(0f, 0.73f, 1f, 1f);
        public Color m_shallowTint = new Color(0.08f, 0.29f, 0.35f, 1f);
        [Range(-1f, 1f)]
        public float m_shallowOffset = -0.398f;
        public Color m_depthTint = new Color(0.14f, 0.26f, 0.3f, 1f);
        [Range(0f, 2f)]
        public float m_depthOffset = 0.843f;

        [Header("Opacity Settings")]
        [Range(0f, 1f)]
        public float m_opacityOcean = 0f;
        [Range(0f, 1f)]
        public float m_opacityBeach = 1f;
        public bool m_ignoreVertexColor = true;

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

        [Header("Refraction Settings")]
        [Range(0f, 50f)]
        public float m_refractedDepth = 50f;
        [Range(0f, 1f)]
        public float m_refractionScale = 1f;

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

        [Header("Shadow Settings")]
        [Range(0f, 1f)]
        public float m_shadowStrength = 0f;
        [Range(0f, 1f)]
        public float m_shadowSharpness = 0.01f;
        [Range(-1f, 0f)]
        public float m_shadowOffset = -1f;

        [Header("Reflection Settings")]
        [Range(0f, 1f)]
        public float m_reflectionIntenisty = 0.41f;
        [Range(0f, 1f)]
        public float m_reflectionWobble = 0.062f;
        [Range(0f, 10f)]
        public float m_reflectionFresnelPower = 1.7f;
        [Range(0f, 1f)]
        public float m_reflectionFresnelScale = 0.45f;

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

        [Header("Wave Tessellation")]
        [Range(0f, 1f)]
        public float m_tessellationAmount = 0.374f;
        public float m_tessellationDistance = 5000f;

        [Header("Wave Direction Settings")]
        public Vector4 m_wave01 = new Vector4(1f, 0f, 0.15f, 50f);
        public Vector4 m_wave02 = new Vector4(-1f, -0.5f, 0.15f, 45f);
        public Vector4 m_wave03 = new Vector4(1f, -1f, 0.15f, 45f);
        [Range(0f, 5f)]
        public float m_waveSpeed = 0.402f;

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
    }
}