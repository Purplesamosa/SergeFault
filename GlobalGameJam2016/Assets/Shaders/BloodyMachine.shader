Shader "Custom/BloodyMachine"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Mask ("Bloody Mask", 2D) = "white"{}
		_BloodStep ("Blood Step", range(0.1, 1)) = 1
		_BloodColor ("Blood Color", color) = (1, 0, 0, 1)
		_BloodAlpha ("Blood Opacity", range(0, 1)) = 1
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
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _Mask;
			
			half _BloodStep;
			fixed4 _BloodColor;
			half _BloodAlpha;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				
				fixed m = (tex2D(_Mask, IN.texcoord)).r;
				
				fixed s = step(_BloodStep, m) * _BloodAlpha * c.a;
				
				fixed4 res = fixed4(1, 1, 1, c.a);
				
				res.rgb = (c.rgb * (1-s)) + (s * _BloodColor.rgb);
				
				res.rgb *= c.a;
				return res;
			}
		ENDCG
		}
	}
}