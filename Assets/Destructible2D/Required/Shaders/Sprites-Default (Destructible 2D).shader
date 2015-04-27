Shader "Sprites/Default (Destructible 2D)"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_AlphaTex ("Alpha Tex", 2D) = "white" {}
		_AlphaScale ("Alpha Scale", Vector) = (1,1,0,0)
		_AlphaOffset ("Alpha Offset", Vector) = (0,0,0,0)
		_Sharpness ("Sharpness", Float) = 1.0
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma multi_compile DUMMY PIXELSNAP_ON
				
				#include "UnityCG.cginc"
				
				sampler2D _MainTex;
				
				sampler2D _AlphaTex;
				
				float _Sharpness;
				
				float4 _Color;
				
				float2 _AlphaScale;
				
				float2 _AlphaOffset;
				
				struct a2v
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD;
				};
				
				struct v2f
				{
					float4 vertex   : SV_POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};
				
				void Vert(a2v i, out v2f o)
				{
					o.vertex   = mul(UNITY_MATRIX_MVP, i.vertex);
					o.color    = i.color * _Color;
					o.texcoord = i.texcoord;
#if PIXELSNAP_ON
					o.vertex = UnityPixelSnap(o.vertex);
#endif
				}
				
				void Frag(v2f i, out float4 o:COLOR0)
				{
					float4 mainTex  = tex2D(_MainTex, i.texcoord);
					float4 alphaTex = tex2D(_AlphaTex, (i.texcoord - _AlphaOffset) * _AlphaScale);
					
					o.rgba = mainTex * i.color;
					
					o.a *= saturate(0.5f + (alphaTex.a - 0.5f) * _Sharpness);
					
					o.rgb *= o.a;
				}
			ENDCG
		}
	}
}
