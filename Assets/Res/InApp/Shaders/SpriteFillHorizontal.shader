﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/SpriteFillHorizontal"
{Properties
    {
       [Toggle] _LeftStart("Left start",Float) = 1
        _MainColor("Color",Color) = (1,0,0,1)
        [PerRendererData] _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _Rate("rate",Range(0,1.0)) = 0

    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZTest Always
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #pragma shader_feature  _CLOCKWISE_ON

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
             float4 _MainTex_ST;
            fixed4 _MainColor;
            float _Rate;
            float _LeftStart;
            float cli;
            v2f vert(appdata_t v)
            {
                v2f o;
                //UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);

            if(_LeftStart == 0)
            {
            if(i.texcoord.x < _Rate)
                clip(-1);
            }
            else
            {
             if(i.texcoord.x > _Rate)
                clip(-1);
            }

                return col * _MainColor;
            }
            ENDCG
        }
    }

}
