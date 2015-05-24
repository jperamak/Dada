Shader "Destructible 2D/Gradient"
{
	Properties
	{
		_BottomColour("Bottom Colour", Color) = (0.0, 0.0, 1.0, 1.0)
		_BottomHeight("Bottom Height", Float) = 1.0
		_TopColour("Top Colour", Color) = (0.0, 0.0, 0.0, 1.0)
		_TopHeight("Top Height", Float) = 10.0
	}
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
				
				float4 _BottomColour;
				float  _BottomHeight;
				float4 _TopColour;
				float  _TopHeight;
				
				struct a2v
				{
					float4 vertex : POSITION;
				};
				
				struct v2f
				{
					float4 pos    : SV_POSITION;
					float  height : TEXCOORD0;
				};
				
				struct f2g
				{
					half4 rgba : COLOR0;
				};
				
				void Vert(a2v i, out v2f o)
				{
					o.pos    = mul(UNITY_MATRIX_MVP, i.vertex);
					o.height = mul(_Object2World, i.vertex).y;
				}
				
				void Frag(v2f i, out f2g o)
				{
					float height01 = saturate((i.height - _BottomHeight) / (_TopHeight - _BottomHeight));
					
					o.rgba = lerp(_BottomColour, _TopColour, height01);
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader