Shader "Custom/PassFadeBlack"
{
	Properties
	{
		_Color("Color", COLOR) = (0,0,0,0)
	}
	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+500" }
		LOD 100

		ZTest Off
		cull off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return _Color;
			}
			ENDCG
		}
	}
}