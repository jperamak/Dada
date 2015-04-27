Shader "Destructible 2D/Vertex Colours"
{
	SubShader
	{
		Tags
		{
			"IgnoreProjector" = "True"
		}
		Pass
		{
			Lighting Off
			Cull Off
			
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				
				struct a2v
				{
					float4 vertex : POSITION;
					float4 color  : COLOR;
				};
				
				struct v2f
				{
					float4 pos  : SV_POSITION;
					float4 rgba : COLOR;
				};
				
				struct f2g
				{
					half4 rgba : COLOR0;
				};
				
				void Vert(a2v i, out v2f o)
				{
					o.pos  = mul(UNITY_MATRIX_MVP, i.vertex);
					o.rgba = i.color;
				}
				
				void Frag(v2f i, out f2g o)
				{
					o.rgba = i.rgba;
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader