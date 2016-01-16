Shader "Mobile/Diffuse Vertex Colored" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 150

		CGPROGRAM
#pragma surface surf Lambert noforwardadd

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
		float3 color : COLOR;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb * IN.color.rgb;
	}
	ENDCG
	}

		Fallback "Mobile/VertexLit"
}