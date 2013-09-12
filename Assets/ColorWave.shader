Shader "ColorWave" {
	Properties {
		_GradientTex ("Masked Gradient (R Gradient, G Mask)", 2D) = "white" {}
		_Color("Color", COLOR) = (1,1,1,1)
		_WaveColor("Wave Color", COLOR) = (0,0,0,0)
		_LowerBound("Lower Bound", float) = 0
		_UpperBound("Upper Bound", float) = 0.1
		_Strength("Strength", float) = 10
	}
	Subshader{
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			float4 _GradientTex_ST;
			sampler2D _GradientTex;
			half4 _Color;
			half4 _WaveColor;
			float _LowerBound;
			float _UpperBound;
			float _Strength;
			
			struct data{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD;
			};
			
			struct v2f{
				float4 position : POSITION;
				float2 uv : TEXCOORD;
			};
			
			v2f vert(data v){
				v2f o;
				o.position = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _GradientTex);
				return o;
			}
			
			half4 frag(v2f i) : COLOR{
				half4 gradientMask = tex2D(_GradientTex, i.uv);
				//float gradient = step(_LowerBound, gradientMask.r);
				//gradient *= step(gradientMask.r, _UpperBound);	// this has no smooth falloff
				float gradient = min(_UpperBound - gradientMask.r, gradientMask.r - _LowerBound);
				gradient = saturate(gradient * _Strength);
				gradient *= gradientMask.g;	// mask
				half4 col = lerp(_Color, _WaveColor, gradient);
				return col;
			}
			
			ENDCG
		}
	}
}
