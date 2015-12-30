Shader "Custom/wireframe" {

	Properties 
	{
		_SubBaseTex("Sub Base Texture", 2D) = "white" {}
		_SubAlpha("Sub Texture Alpha", range(0, 1)) = 1
		_SubTint("Sub Texture Tint Color", Color) = (1, 1, 1, 1)
		/*_SubClipping ("Sub Clipping", range(-10,10)) = 1
		_SubClipping2 ("Sub Clipping 2", range(1, 10)) = 1
		_SubClipping3 ("Sub Clipping 3", range(0, 1)) = 1*/
		
		_BaseTex("Base Texture", 2D) = "white" {}
		_Alpha("Base Texture Alpha", range(0, 1)) = 1
		_Tint("Base Texture Tint Color", Color) = (1, 1, 1, 1)
		
		_MainTex("WF Texture", 2D) = "white" {}
		_Fill ("WF Texture Alpha", range(0,1)) = 0 //Bools are not supported in Shader Lab. YES, REALLY.
		_Color ("WF Color", Color) = (1,1,1,1)
		_Thickness ("WF Thickness", range(0,10)) = 1
		_ClippingOnOff("Clipping on/off", range(0,1)) = 0
		_Clipping ("Clipping", range(0,10)) = 1		
		_Clipping2 ("Clipping 2", range(0,10)) = 1
		_Clipping3 ("Clipping 3", range(0,100)) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		
			CGPROGRAM
				#pragma surface surf Lambert alpha

				sampler2D _SubBaseTex;
				float4 _SubTint;
				float _SubAlpha;//, _SubClipping, _SubClipping2, _SubClipping3;

				struct Input {
					float2 uv_SubBaseTex;
					float3 worldPos;
				};

				void surf(Input IN, inout SurfaceOutput o) {
				
					//if (_SubClipping3 > 0.5f)
					//clip (frac((IN.worldPos.y+IN.worldPos.z*_SubClipping) * _SubClipping2) - 0.5);
					
					fixed4 c = tex2D(_SubBaseTex, IN.uv_SubBaseTex) * _SubTint;
					o.Albedo = c.rgb * _SubTint;
					o.Alpha = c.a;
					o.Alpha = _SubAlpha;
				}
		
			ENDCG
			
			CGPROGRAM
				#pragma surface surf Lambert alpha

				sampler2D _BaseTex;
				fixed4 _Color, _Tint;
				float _Alpha;//, _SubClipping, _SubClipping2, _SubClipping3;

				struct Input {
					float2 uv_BaseTex;
					float3 worldPos;
				};

				void surf(Input IN, inout SurfaceOutput o) {
				
					//clip (frac((IN.worldPos.y+IN.worldPos.z*_SubClipping) * _SubClipping2) - 0.5);
					
					fixed4 c = tex2D(_BaseTex, IN.uv_BaseTex) * _Tint;
					o.Albedo = c.rgb * _Tint;
					o.Alpha = _Alpha * c.a;
				}
		
			ENDCG

		Pass
		{
			Tags { "RenderType"="Transparent" "Queue"="Transparent" }
			
			Blend SrcAlpha OneMinusSrcAlpha //Alpha blending 
			LOD 200
			CGPROGRAM
				#pragma target 4.0
				#include "UnityCG.cginc"
				#include "wireframeFunctions.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma geometry geom

				// Vertex Shader
				wf_v2g vert(appdata_base v)
				{
					//v.color.a = 0.2f;
					return wf_vert(v);
				}
				
				// Geometry Shader
				[maxvertexcount(3)]
				void geom(triangle wf_v2g p[3], inout TriangleStream<wf_g2f> triStream)
				{
					wf_geom(p, triStream);
				}
				
				// Fragment Shader
				float4 frag(wf_g2f input) : COLOR
				{	
					//return wf_frag(input);
					float4 col = wf_frag(input);				
					if( col.a < 0.5f ) discard;
					else col.a = 1.0f;
					
					return col;
				}
			
			ENDCG
		}
	}
} 
