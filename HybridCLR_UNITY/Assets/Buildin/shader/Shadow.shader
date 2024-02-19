// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/NewShader" {
    Properties{
        _MainTex("Base (RGB)", 2D) = "white" {}
    //*********************绘制阴影的参数****************************
    _ShadowPlane("Shadow Plane", Vector) = (0,1,0,0.02)
    _ShadowColor("Shadow Color",Color) = (0.2, 0.2, 0.2, 1)
    _ShadowPara("Shadow Param",Vector) = (-0.006,60,3,0.02)
    _lightDirection("LightDir",Vector) = (0,-1,0)
        //*************************************************
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Lambert

            sampler2D _MainTex;

            struct Input {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutput o) {
                half4 c = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
            ENDCG
                //*******************绘制阴影的参数pass 只需要在你原来的shader里面加入这个pass就可以绘制出实时阴影了******************************
                pass {
                    Tags { "QUEUE" = "Transparent" "RenderType" = "Transparent" }
                    ZWrite Off
                    Fog { Mode Off }
                    Blend SrcAlpha OneMinusSrcAlpha
                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #include "UnityCG.cginc"
                    struct v2f {
                        float4 pos : SV_POSITION;
                        half3 texcord0 : TEXCOORD0;
                        half3 texcord1 : TEXCOORD1;
                    };
                    uniform fixed4 _ShadowPlane;
                    uniform fixed4 _ShadowPara;
                    uniform fixed3 _lightDirection;
                    uniform fixed4 _ShadowColor;
                    v2f vert(appdata_tan v)
                    {
                          v2f o;

                          fixed3 vt;
                          vt = mul(unity_ObjectToWorld , v.vertex).xyz;
                          fixed3 tmpvar_3;
                          tmpvar_3 = (vt - (((dot(_ShadowPlane.xyz, vt) - _ShadowPlane.w) / dot(_ShadowPlane.xyz, _lightDirection)) * _lightDirection));

                          fixed4 tmpvar_4;
                          tmpvar_4.w = 1.0;
                          tmpvar_4.xyz = tmpvar_3;
                          o.pos = mul(UNITY_MATRIX_VP , tmpvar_4);
                          o.texcord0 = mul(unity_ObjectToWorld,float4(0, 0, 0, 1));
                          o.texcord1 = tmpvar_3;
                          return o;
                    }

                    float4 frag(v2f inData) : COLOR
                    {
                        fixed3 posToPlane = inData.texcord0 - inData.texcord1;

                        fixed4 f = _ShadowColor;
                        fixed v = pow(1.0 - clamp(((sqrt(dot(posToPlane,posToPlane)) * _ShadowPara.w) - _ShadowPara.x),0,1),_ShadowPara.y) * _ShadowPara.z;
                        f.w = v * _ShadowColor.a;
                        return f;
                    }
                    ENDCG
                }
            //*************************************************

    }
        FallBack "Diffuse"
}
