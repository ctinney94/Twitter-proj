Shader "Custom/islandBlend" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Texture2("Blend Texture 1", 2D) = "white" {}

	_Blend1("Blend", Range(0, 1)) = 0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 400

		CGPROGRAM
#pragma surface surf Lambert
#pragma target 3.0
		//#pragma surface surf BlinnPhong

		sampler2D _MainTex;
	sampler2D _Texture2;
	float _Blend1;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		//half4 c = tex2D (_MainTex, IN.uv_MainTex);
		fixed4 mainCol = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 texTwoCol = tex2D(_Texture2, IN.uv_MainTex);
		fixed4 output = lerp(mainCol, texTwoCol, _Blend1);
		o.Albedo = output.rgb;
		o.Alpha = output.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}