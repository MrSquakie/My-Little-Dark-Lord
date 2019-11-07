using UnityEngine;
using PWCommon2;

namespace GeNa
{
    public class SplatPainter
    {
        public int Size { get; set; }
        public float Opacity { get; set; }
        public float TargetStrength { get; set; }
        public UBrush Brush { get; set; }
        public TerrainData TerrainData { get; private set; }

        public SplatPainter(TerrainData terrainData)
        {
            TerrainData = terrainData;
        }

        public void Paint(float xCenterNormalized, float yCenterNormalized, int splatIndex)
        {
            if (splatIndex >= TerrainData.alphamapLayers)
                return;

            float xCenter = xCenterNormalized * (float)TerrainData.alphamapWidth;
            float yCenter = yCenterNormalized * (float)TerrainData.alphamapHeight;

            float xDistance = xCenter - Mathf.FloorToInt(xCenter);
            float yDistance = yCenter - Mathf.FloorToInt(yCenter);
            int xOverlap = Mathf.CeilToInt(xDistance);
            int yOverlap = Mathf.CeilToInt(yDistance);

            float radius = Size * 0.5f;
            //int remainder = Size % 2;

            int x1 = Mathf.Clamp(Mathf.FloorToInt(xCenter - radius), 0, TerrainData.alphamapWidth - 1);
            int y1 = Mathf.Clamp(Mathf.FloorToInt(yCenter - radius), 0, TerrainData.alphamapHeight - 1);
            int rightMost = Mathf.Clamp(x1 + Size + xOverlap, 0, TerrainData.alphamapWidth);
            int topMost = Mathf.Clamp(y1 + Size + yOverlap, 0, TerrainData.alphamapHeight);

            int width = rightMost - x1;
            int height = topMost - y1;
            float[,,] alphamaps = TerrainData.GetAlphamaps(x1, y1, width, height);

            float BL_Ratio = xDistance * yDistance;
            float TL_Ratio = xDistance * (1f - yDistance);
            float BR_Ratio = (1f - xDistance) * yDistance;
            float TR_Ratio = (1f - xDistance) * (1f - yDistance);

            for (int y2 = 0; y2 < height; ++y2)
            {
                for (int x2 = 0; x2 < width; ++x2)
                {
                    float BL_Val = Brush.GetStrengthAtCoords(x2 - 1, y2 - 1);
                    float TL_Val = Brush.GetStrengthAtCoords(x2 - 1, y2);
                    float BR_Val = Brush.GetStrengthAtCoords(x2, y2 - 1);
                    float TR_Val = Brush.GetStrengthAtCoords(x2, y2);
                    float brushStrengthAtCoords = BL_Ratio * BL_Val + TL_Ratio * TL_Val + BR_Ratio * BR_Val + TR_Ratio * TR_Val;

                    float strength = ApplyBrush(alphamaps[y2, x2, splatIndex], brushStrengthAtCoords * Opacity);
                    alphamaps[y2, x2, splatIndex] = strength;
                    Normalize(x2, y2, splatIndex, alphamaps);
                }
            }
            TerrainData.SetAlphamaps(x1, y1, alphamaps);
        }

        private float ApplyBrush(float alpha, float brushStrength)
        {
            if ((double)TargetStrength > (double)alpha)
            {
                alpha += brushStrength;
                alpha = Mathf.Min(alpha, TargetStrength);
                return alpha;
            }
            alpha -= brushStrength;
            alpha = Mathf.Max(alpha, TargetStrength);
            return alpha;
        }

        private void Normalize(int x, int y, int splatIndex, float[,,] alphamap)
        {
            float num1 = alphamap[y, x, splatIndex];
            float num2 = 0.0f;
            int length = alphamap.GetLength(2);
            for (int index = 0; index < length; ++index)
            {
                if (index != splatIndex)
                    num2 += alphamap[y, x, index];
            }
            if ((double)num2 > 0.01)
            {
                float num3 = (1f - num1) / num2;
                for (int index = 0; index < length; ++index)
                {
                    if (index != splatIndex)
                        alphamap[y, x, index] *= num3;
                }
            }
            else
            {
                for (int index = 0; index < length; ++index)
                    alphamap[y, x, index] = index != splatIndex ? 0.0f : 1f;
            }
        }
    }
}
