﻿Shader "Custom/ParticleOnTop"
{
	Properties
	{
		_MainTex ("Particle Texture", 2D) = "white" {}
	}

	Category
	{
		Tags
		{
			"Queue"="Overlay+1000"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		
		BindChannels
		{
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		SubShader
		{
			Pass
			{
				SetTexture [_MainTex]
				{
					combine texture * primary
				}
			}
		}
	}
}
