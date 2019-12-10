    Shader "Hidden/Gaia/FilterImageMask" {

    Properties {
				//The input texture
				_InputTex ("Input Texture", any) = "" {}				
				//The image mask texture
				_ImageMaskTex ("Image Mask Texture", any) = "" {}
				//Flag to determine whether the Mask is inverted or not
				_Invert("Invert Distance Mask", Int) = 0
				//Strength from 0 to 1 to determine how "strong" the image mask effect is applied
				_Strength ("Strength", Float) = 0


				 }

    SubShader {

        ZTest Always Cull Off ZWrite Off

        CGINCLUDE

            #include "UnityCG.cginc"
            #include "TerrainTool.cginc"

            sampler2D _InputTex;
			sampler2D _ImageMaskTex;
			int _Invert;
			float _Strength;
			
            float4 _MainTex_TexelSize;      // 1/width, 1/height, width, height

           

            struct appdata_t {
                float4 vertex : POSITION;
                float2 pcUV : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 pcUV : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pcUV = v.pcUV;
                return o;
            }
		ENDCG
            

        Pass    // 0 Filter Image Multiply
        {
            Name "Filter Image Multiply"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment FilterImageMultiply

            float4 FilterImageMultiply(v2f i) : SV_Target
            {
				float col = UnpackHeightmap(tex2D(_InputTex, i.pcUV));
				float col2 = UnpackHeightmap(tex2D(_ImageMaskTex, i.pcUV));
				if (_Invert > 0)
				{
					col2 = (1.0f - col2);
				}
				return PackHeightmap(lerp(col,(col * col2),_Strength));
            }
            ENDCG
        }

		Pass    // 1 Filter Image Greater Than
        {
            Name "Filter Image Greater Than"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment FilterImageGreaterThan

            float4 FilterImageGreaterThan(v2f i) : SV_Target
            {
				float col = UnpackHeightmap(tex2D(_InputTex, i.pcUV));
				float col2 = UnpackHeightmap(tex2D(_ImageMaskTex, i.pcUV));

				if (_Invert > 0)
				{
					col2 = (1.0f - col2);
				}

				float result = col;
				if(col2>col)
				{
					result = col2;
				}


				return PackHeightmap(lerp(col,result,_Strength));
            }
            ENDCG
        }

		Pass    // 2 Filter Image Smaller Than
        {
            Name "Filter Image Smaller Than"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment FilterImageSmallerThan

            float4 FilterImageSmallerThan(v2f i) : SV_Target
            {
				float col = UnpackHeightmap(tex2D(_InputTex, i.pcUV));
				float col2 = UnpackHeightmap(tex2D(_ImageMaskTex, i.pcUV));

				if (_Invert > 0)
				{
					col2 = (1.0f - col2);
				}
				
				float result = col;
				if(col2<col)
				{
					result = col2;
				}
				return PackHeightmap(lerp(col,result,_Strength));
            }
            ENDCG
        }

		Pass    // 3 Filter Image Add
        {
            Name "Filter Image Add"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment FilterImageAdd

            float4 FilterImageAdd(v2f i) : SV_Target
            {
				float col = UnpackHeightmap(tex2D(_InputTex, i.pcUV));
				float col2 = UnpackHeightmap(tex2D(_ImageMaskTex, i.pcUV));

				if (_Invert > 0)
				{
					col2 = (1.0f - col2);
				}

				return PackHeightmap(lerp(col,(col + col2),_Strength));
            }
            ENDCG
        }

		Pass    // 4 Filter Image Subtract
        {
            Name "Filter Image Subtract"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment FilterImageSubtract

            float4 FilterImageSubtract(v2f i) : SV_Target
            {
				float col = UnpackHeightmap(tex2D(_InputTex, i.pcUV));
				float col2 = UnpackHeightmap(tex2D(_ImageMaskTex, i.pcUV));

				if (_Invert > 0)
				{
					col2 = (1.0f - col2);
				}
				return PackHeightmap(lerp(col,(col - col2),_Strength));
            }
            ENDCG
        }

    }
    Fallback Off
}
