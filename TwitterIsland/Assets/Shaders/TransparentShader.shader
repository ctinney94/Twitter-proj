
Shader "Mobile/TransparentShader"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		SubShader
	{

		Blend SrcAlpha OneMinusSrcAlpha

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite Off
		Lighting Off

		Pass
	{
		GLSLPROGRAM

#ifdef VERTEX  
		varying lowp vec2 uv;

	void main()
	{
		gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
		uv = gl_MultiTexCoord0.xy;
	}
#endif

#ifdef FRAGMENT
	uniform lowp sampler2D _MainTex;
	varying lowp vec2 uv;

	void main()
	{
		gl_FragColor = texture2D(_MainTex, uv);
	}
#endif
	ENDGLSL
	}
	}
}