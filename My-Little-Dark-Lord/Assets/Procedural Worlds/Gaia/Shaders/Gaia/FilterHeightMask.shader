    Shader "Hidden/Gaia/FilterHeightMask" {

    Properties {
				//The main texture (existing heightmap)
				_HeightMapTex ("Heightmap Texture", any) = "" {}				
				//The input texture
				_InputTex ("Input Texture", any) = "" {}				
				//1-pixel high distance mask texture representing an animation curve
				_HeightMaskTex ("Height Mask Texture", any) = "" {}
				//Flag to determine whether the Mask is inverted or not
			    _Invert("Invert Mask", Float) = 1
				//Lower height bound where the height mask will be applied
				_MinHeight ("Minimum Height", Float) = 0
				//Upper height bound where the height mask will be applied
				_MaxHeight ("Maximum Height", Float) = 0
				 //Strength from 0 to 1 to determine how "strong" the distance mask effect is applied
				 _Strength ("Strength", Float) = 0

				 }

    SubShader {

        ZTest Always Cull Off ZWrite Off

        CGINCLUDE

            #include "UnityCG.cginc"
            #include "TerrainTool.cginc"

			sampler2D _HeightMapTex;
            sampler2D _InputTex;
			sampler2D _HeightMaskTex;
			int _Invert;
			float _MinHeight; 
			float _MaxHeight;
			float _Strength;
			
            float4 _MainTex_TexelSize;      // 1/width, 1/height, width, height

           sampler2D _BrushTex;

            float4 _BrushParams;
            #define BRUSH_STRENGTH      (_BrushParams[0])
            #define BRUSH_TARGETHEIGHT  (_BrushParams[1])

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
		
		
			float GetFilteredHeight(v2f i)
			{
				float height = UnpackHeightmap(tex2D(_HeightMapTex, i.pcUV));
				float filteredHeight = UnpackHeightmap(tex2D(_HeightMaskTex, smoothstep(_MinHeight, _MaxHeight, height)));
				if (_Invert > 0)
				{
					filteredHeight = (1.0f - filteredHeight);
				}
				return filteredHeight;
			}
		ENDCG
            

         Pass    // 0 Height Mask Multiply
        {
            Name "Height Mask Multiply"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment HeightMaskMultiply

            float4 HeightMaskMultiply(v2f i) : SV_Target
            {
				float inputTexHeight = UnpackHeightmap(tex2D(_InputTex, i.pcUV));
				float filteredHeight = GetFilteredHeight(i);
				return PackHeightmap(lerp(inputTexHeight,filteredHeight*inputTexHeight,_Strength));
            }
            ENDCG
        }

		Pass    // 1 Height Mask Greater Than
		{
			Name "Height Mask Greater Than"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment HeightMaskGreaterThan

			float4 HeightMaskGreaterThan(v2f i) : SV_Target
			{
				float inputTexHeight = UnpackHeightmap(tex2D(_InputTex, i.pcUV));
				float filteredHeight = GetFilteredHeight(i);

				float result = inputTexHeight;
				if (filteredHeight > inputTexHeight)
				{
					result = filteredHeight;
				}

				return PackHeightmap(lerp(inputTexHeight, result,_Strength));
			}
			ENDCG
		}

		Pass    // 2 Height Mask Smaller Than
		{
			Name "Height Mask Smaller Than"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment HeightMaskSmallerThan

			float4 HeightMaskSmallerThan(v2f i) : SV_Target
			{
				float inputTexHeight = UnpackHeightmap(tex2D(_InputTex, i.pcUV));
				float filteredHeight = GetFilteredHeight(i);

				float result = inputTexHeight;
				if (filteredHeight < inputTexHeight)
				{
					result = filteredHeight;
				}

				return PackHeightmap(lerp(inputTexHeight, result,_Strength));
			}
			ENDCG
		}

		Pass    // 3 Height Mask Add
		{
			Name "Height Mask Add"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment HeightMaskAdd

			float4 HeightMaskAdd(v2f i) : SV_Target
			{
				float inputTexHeight = UnpackHeightmap(tex2D(_InputTex, i.pcUV));
				float filteredHeight = GetFilteredHeight(i);
				return PackHeightmap(lerp(inputTexHeight, inputTexHeight + filteredHeight,_Strength));
			}
			ENDCG
		}

		Pass    // 4 Height Mask Add
		{
			Name "Height Mask Subtract"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment HeightMaskSubtract

			float4 HeightMaskSubtract(v2f i) : SV_Target
			{
				float inputTexHeight = UnpackHeightmap(tex2D(_InputTex, i.pcUV));
				float filteredHeight = GetFilteredHeight(i);
				return PackHeightmap(lerp(inputTexHeight, inputTexHeight - filteredHeight,_Strength));
			}
			ENDCG
		}
	

    }
    Fallback Off
}
