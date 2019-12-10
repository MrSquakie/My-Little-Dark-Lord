using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gaia
{
    public class ImageProcessing
    {

 
        //static RenderTexture outputTexture;

        /// <summary>
        /// Applies a stack of masks in an array on a render texture to get a combined final mask.
        /// </summary>
        /// <param name="inputTexture">The input render texture</param>
        /// <param name="maskStack">An array of ImageMasks to apply on the render texture.</param>
        /// <returns>A modified render texture containing the results of the filter application.</returns>
        public static RenderTexture ApplyMaskStack(RenderTexture inputTexture, ImageMask[] maskStack, ImageMaskInfluence influence)
        {
            //early out if no valid masks to begin with
            if (maskStack.Where(x=>x.m_active==true && x.m_influence == influence).Count() <=0)
            {
                return inputTexture;
            }

            RenderTexture outputTexture = RenderTexture.GetTemporary(inputTexture.descriptor);

            for (int i = 0; i < maskStack.Length; i++)
            {
                if (maskStack[i].m_active && maskStack[i].m_influence == influence)
                {
                    //release old texture first before assigning new
                    if (outputTexture != null)
                    {
                        RenderTexture.ReleaseTemporary(outputTexture);
                        outputTexture = null;
                    }

                    outputTexture = maskStack[i].Apply(inputTexture);

                    //Write the result texture back to the input so we can use the result
                    //of this iteration as input for the next, and so on.
                    Graphics.Blit(outputTexture, inputTexture);
                }
            }

            if (inputTexture != null)
            {
                RenderTexture.ReleaseTemporary(inputTexture);
                inputTexture = null;
            }


            return outputTexture;
        }


        /// <summary>
        /// Outputs a Render texture as a .png file
        /// </summary>
        /// <param name="path">The path to write to, including filename ending in .exr</param>
        /// <param name="sourceRenderTexture">The render texture to export</param>
        public static void WriteRenderTexturePNG(string path, RenderTexture sourceRenderTexture)
        {
            RenderTexture origTex = RenderTexture.active;
            RenderTexture.active = sourceRenderTexture;
            Texture2D exportTexture = new Texture2D(RenderTexture.active.width, RenderTexture.active.height, TextureFormat.RGB24, false);
            exportTexture.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0);
            exportTexture.Apply();
            byte[] exrBytes = ImageConversion.EncodeToPNG(exportTexture);
            PWCommon2.Utils.WriteAllBytes(path, exrBytes);
            RenderTexture.active = origTex;
        }


        /// <summary>
        /// Outputs a Render texture as a .exr file
        /// </summary>
        /// <param name="path">The path to write to, including filename ending in .exr</param>
        /// <param name="sourceRenderTexture">The render texture to export</param>
        public static void WriteRenderTexture(string path, RenderTexture sourceRenderTexture)
        {
            RenderTexture origTex = RenderTexture.active;
            RenderTexture.active = sourceRenderTexture;
            Texture2D exportTexture = new Texture2D(RenderTexture.active.width, RenderTexture.active.height, TextureFormat.RGBAFloat, false);
            exportTexture.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0);
            exportTexture.Apply();
            byte[] exrBytes = ImageConversion.EncodeToEXR(exportTexture, Texture2D.EXRFlags.CompressZIP);
            PWCommon2.Utils.WriteAllBytes(path, exrBytes);
            RenderTexture.active = origTex;
        }

        /// <summary>
        /// Outputs a Texture2D as a .png file for debug purposes
        /// </summary>
        /// <param name="path">The path to write to, including filename ending in .png</param>
        /// <param name="sourceTexture">The texture to export</param>
        public static void WriteTexture2D(string path, Texture2D sourceTexture)
        {
            byte[] exrBytes = ImageConversion.EncodeToPNG(sourceTexture);
            if (exrBytes != null)
            {
                PWCommon2.Utils.WriteAllBytes(path, exrBytes);
            }
        }

        public static void BakeMaskStack(ImageMask[] maskStack, Terrain terrain, Transform transform, float range, int resolution, string path)
        {
            //Simulate an operation to allow the image masks to acces the current terrain data
            GaiaMultiTerrainOperation operation = new GaiaMultiTerrainOperation(terrain, transform, range);
            operation.GetHeightmap();
            operation.GetNormalmap();
            operation.CollectTerrainCollisions();

            float maxCurrentTerrainHeight = 0f;
            float minCurrentTerrainHeight = 0f;
            float seaLevel;

            GaiaSessionManager gsm = GaiaSessionManager.GetSessionManager();
            gsm.GetWorldMinMax(ref minCurrentTerrainHeight, ref maxCurrentTerrainHeight);
            seaLevel = gsm.GetSeaLevel();

            if (maskStack.Length > 0)
            {
                //We start from a white texture, so we need the first mask action in the stack to always be "Multiply", otherwise there will be no result.
                maskStack[0].m_blendMode = ImageMaskBlendMode.Multiply;

                //Iterate through all image masks and set up the required data that masks might need to function properly
                foreach (ImageMask mask in maskStack)
                {
                    //mask.m_heightmapContext = heightmapContext;
                    //mask.m_normalmapContext = normalmapContext;
                    //mask.m_collisionContext = collisionContext;
                    mask.m_multiTerrainOperation = operation;
                    mask.m_seaLevel = seaLevel;
                    mask.m_maxWorldHeight = maxCurrentTerrainHeight;
                    mask.m_minWorldHeight = minCurrentTerrainHeight;
                }

            }

            RenderTexture inputTexture = RenderTexture.GetTemporary(resolution, resolution, 0,RenderTextureFormat.ARGBFloat);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = inputTexture;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = currentRT;
            RenderTexture localOutputTexture = RenderTexture.GetTemporary(inputTexture.descriptor);
            RenderTexture globalOutputTexture = RenderTexture.GetTemporary(inputTexture.descriptor);
            localOutputTexture = ImageProcessing.ApplyMaskStack(inputTexture, maskStack, ImageMaskInfluence.Local);
            globalOutputTexture = ImageProcessing.ApplyMaskStack(localOutputTexture, maskStack, ImageMaskInfluence.Global);

            WriteRenderTexture(path, globalOutputTexture);

            RenderTexture.ReleaseTemporary(inputTexture);
            RenderTexture.ReleaseTemporary(localOutputTexture);
            RenderTexture.ReleaseTemporary(globalOutputTexture);
            inputTexture = null;
            localOutputTexture = null;
            globalOutputTexture = null;
            operation.CloseOperation();

        }

        public static Texture2D CreateMaskCurveTexture(ref Texture2D inputCurveTexture)
        {
            if (inputCurveTexture == null)
            {
                TextureFormat format = TextureFormat.RGB24;
                if (SystemInfo.SupportsTextureFormat(TextureFormat.RFloat))
                    format = TextureFormat.RFloat;
                else if (SystemInfo.SupportsTextureFormat(TextureFormat.RHalf))
                    format = TextureFormat.RHalf;

                inputCurveTexture = new Texture2D(256, 1, format, false, true)
                {
                    name = "Height Mask curve texture",
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear,
                    anisoLevel = 0,
                    hideFlags = HideFlags.DontSave
                };
            }

            return inputCurveTexture;
        }

        /// <summary>
        /// Bakes an animation curve into a 1 pixel high black and white texture that represents the curve contents. This texture is intended for use as shader input
        /// </summary>
        /// <param name="animationCurve">The curve to bake.</param>
        /// <param name="bakedTexture">The Texture2D to bake into, needs to be 1 pixel high, 256 pixels wide.</param>
        public static void BakeCurveTexture(AnimationCurve animationCurve, Texture2D bakedTexture)
        {
            if (animationCurve != null && animationCurve.length > 0)
            {
                float range = animationCurve[animationCurve.length - 1].time;
                for (float i = 0f; i <= 1f; i += 1f / 255f)
                {
                    float c = animationCurve.Evaluate(i * range);
                    bakedTexture.SetPixel(Mathf.FloorToInt(i * 255f), 0, new Color(c, c, c));
                }
                bakedTexture.Apply();
            }
        }
    }
}
