Shader "Retro3D/Unlit"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
        _Color("Color", Color) = (0.5, 0.5, 0.5, 1)
        _GeoRes("Geometric Resolution", Float) = 40
        _Distort("Distortion", Float) = 40
    }
    SubShader
    {
        Pass
        {
            Fog {Mode Global}
            CGPROGRAM


            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 texcoord : TEXCOORD;
                
                UNITY_FOG_COORDS(2)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _GeoRes;
            float _Distort;

            v2f vert(appdata_base v)
            {
                v2f o;

                float4 wp = mul(UNITY_MATRIX_MV, v.vertex);
                wp.xyz = floor(wp.xyz * _GeoRes) / _GeoRes;

                float4 sp = mul(UNITY_MATRIX_P, wp);
                o.position = sp;

                float2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.texcoord = float3(uv * sp.z, sp.z);
                UNITY_TRANSFER_FOG(o,o.position);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.texcoord.xy / i.texcoord.z;
                _Color = tex2D(_MainTex, uv) * _Color;
                UNITY_APPLY_FOG(i.fogCoord, _Color);
                return _Color;
            }

            ENDCG
        }
    }
}
