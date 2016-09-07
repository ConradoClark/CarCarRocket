Shader "Frosty/Effect-KuwaharaFilter"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Radius ("Radius", float) = 0
		_RadiusVariance("Radius Variance", float) = 10
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always


		Pass
		{
			CGPROGRAM
			#pragma target 3.0			
			#pragma vertex vert
			#pragma fragment frag			
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _Radius;
			float _RadiusVariance;

			fixed4 kuwahara_filter(v2f v) {
				float4 col;

				_Radius = _Radius + floor(sin(_Time[1]*2)* _RadiusVariance);
				_Radius = clamp(_Radius, 0, 25);

				float2 src_size = float2(_MainTex_TexelSize.zw);
				float2 uv = v.uv;
				float n = float((_Radius + 1) * (_Radius + 1));
				int i, j, k;
				float3 m[4];
				float3 s[4];
				for (k = 0; k < 4; ++k) {
					m[k] = float3(0,0,0);
					s[k] = float3(0,0,0);
				}

				for (j = -_Radius; j <= 0; ++j) {
					float3 c = tex2D(_MainTex, uv + float2(j, j) / src_size).rgb;
					for (i = -_Radius; i <= 0; ++i) {						
						m[0] += c;
						s[0] += acos(c)* c;
					}
				}

				for (j = -_Radius; j <= 0; ++j) {
					float3 c = tex2D(_MainTex, uv + float2(j, j) / src_size).rgb;
					for (i = 0; i <= _Radius; ++i) {
						
						m[1] += c;
						s[1] += acos(c) * c;
					}
				}

				float min_sigma2 = 1e+2;
				for (k = 0; k < 2; ++k) {
					m[k] /= n;
					s[k] = abs(s[k] / n - m[k] * m[k]);

					float sigma2 = sin(s[k].r + s[k].g + s[k].b);
					//float compare = min_sigma2 > 1 ? cos(min_sigma2) : sin(min_sigma2);
					if (sigma2 > sin(min_sigma2)) {
						min_sigma2 = sigma2;
						col = float4(m[k], 1.0);
					}else{
						col = float4(m[0],1.0);
					}					
				}
				return col;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);			
				fixed4 kuwahara = kuwahara_filter(i);				
				//return col;
				float test = clamp((1 / asin(kuwahara.r) * log(kuwahara) * 0.004).b, -0.1, 1);
				return col * 0.55 + col / cos(kuwahara) * 0.2 + col * tan(kuwahara) * 0.35 - float4(test, test, test, col.a) * (cos(_Time[1]*1.4)) *0.2 - i.uv.x* i.uv.y * 1/sin(kuwahara+1.01) * 0.01;
			}
			ENDCG
		}
	}
}
