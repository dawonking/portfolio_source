Shader "Unlit/texture_left"
{
	Properties
	{
		_Color("Tint", Color) = (0, 0, 0, 1)
		_MainTex("Texture", 2D) = "white" {}

		tiling_x("tiling_x",Range(0,1)) = 1
		tiling_y("tiling_y",Range(0,1)) = 1
		alpha_right("alpha_right",Range(0,1)) = 1

		start_pos("start_pos", Vector) = (0,1,0,0)
		end_pos("end_pos", Vector) = (1,0,0,0)

		set1("set1", Vector) = (0,0,0,0)
		set2("set2", Vector) = (0,0,0,0)

		set1_x_r("set1_x_r",Range(0,1)) = 1
		set2_x_r("set2_x_r",Range(0,1)) = 1

	}
		SubShader
		{
			Tags{ "RenderType" = "Transparent" "Queue" = "Transparent"}
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				float2 start_pos;
				float2 end_pos;
				float2 set1;
				float2 set2;

				float tran_t;

				fixed alpha_left;
				fixed alpha_right;
				fixed tiling_x;
				fixed tiling_y;
				fixed set1_x_r;
				fixed set2_x_r;
				fixed4 _Color;

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
					fixed4 col : COLOR;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.position = UnityObjectToClipPos(v.vertex);
					/*
					추가로 작성한 'float4 _Texture2D_ST' 변수  (반드시 texture name + _ST 로 작성해야함)

					x, y 에 tiling 정보를

					z,w 에 offset정보를 보내줍니다.
					*/
					_MainTex_ST = float4(tiling_x, tiling_y, _MainTex_ST.z, _MainTex_ST.w);
					//_MainTex_ST = float4((1 - tiling_x), (1 - tiling_y), tiling_x, tiling_y);
					float2 a = float2(_MainTex_ST.x, _MainTex_ST.y);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				float set_x_pos(float num) {
					float a = 0;
					return a;
				}

				//0.4~0.6

				float shade_lerp(float x1_num, float x2_num, float t)
				{
					return (1 - t) * x1_num + t * x2_num;
				}

				// a = start , b = set1 , c = set2 , d = end

				float2 lerp_triple(float2 a, float2 b , float2 c , float t) {
					float2 p0 = float2(shade_lerp(a.x , b.x , t), shade_lerp(a.y , b.y , t));
					float2 p1 = float2(shade_lerp(b.x, c.x, t), shade_lerp(b.y, c.y, t));
					float2 result = float2(shade_lerp(p0.x, p1.x, t), shade_lerp(p0.y, p1.y, t));
					return result;
				}

				float2 lerp_quad(float2 a, float b, float2 c, float d, float t) {

					float2 p0 = float2(lerp_triple(a,b,c,t));
					float2 p1 = float2(lerp_triple(b, c, d, t));
					float2 result = float2(shade_lerp(p0.x, p1.x, t), shade_lerp(p0.y, p1.y, t));
					return result;
				}

				float time(float start, float end , float x) {
					float a = end - start;
					float set = x - start;
					float result = set / a;
					return result;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					alpha_left = tiling_x;
					
					set1.x = set1_x_r;
					set2.x = set2_x_r;
					if ( i.uv.x >= alpha_right && i.uv.x <= 1)
					{

						//alpha_left = start , end = alpha_right
						col.a = lerp_quad(float2(start_pos.x, start_pos.y), float2(set1.x, set2.y), float2(set2.x, set2.y), float2(end_pos.x, end_pos.y), time(alpha_left, alpha_right, i.uv.x));
						//col.a = 0.5;
						
						col.a = clamp(col.a, 0, 1);

					}

					return col;
				}
				ENDCG

			}
		}
}
