Shader "Shaders/DistanceShader"
{
    Properties
    {
        //_NearColor ("NearColor", Color) = (1,1,1,1)
        //_FarColor ("FarColor", Color) = (1,1,1,1)
        _NearDist ("NearDist", Float) = 2
        _FarDist ("FarDist", Float) = 10
        _MaxIllum ("Max Illumination", Float) = 10
        _Position ("Position", Vector) = (0,0,0,0)
        _Opacity ("Global Opacity", Float) = 1
        _ScanTime ("Global Opacity", Float) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }
        // No culling or depth
        Cull Off
        ZWrite Off
        ZTest Always
        //BlendOp Sub
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Position;
            float _NearDist;
            float _FarDist;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex /* - _Position */);
                o.uv.w = (distance(v.vertex, _Position) - _NearDist) / (_FarDist - _NearDist);
                o.uv.xyz = v.uv.xyz;
                return o;
            }

            float _Opacity;
            float _MaxIllum;
            float _ScanTime;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4x4 _NearColors;
                _NearColors[0] = fixed4(0.0549019608, 0.9529411765, 0.7725490196, 1);
                _NearColors[1] = fixed4(0.9764705882, 0, 0.3137254902, 1);
                _NearColors[2] = fixed4(1, 0, 1, 1);
                _NearColors[3] = fixed4(1, 0, 1, 1);
                fixed4x4 _FarColors;
                _FarColors[0] = fixed4(0.3450980392, 0.4862745098, 0.8823529412, 1);
                _FarColors[1] = fixed4(0.9764705882, 0.4156862745, 0, 1);
                _FarColors[2] = fixed4(1, 0, 1, 1);
                _FarColors[3] = fixed4(1, 0, 1, 1);
                int ix = int(i.uv.z);
                fixed4 col = lerp(_NearColors[ix], _FarColors[ix], i.uv.w);
                if (_ScanTime + i.uv.y > _Time.y) {
                    col.a = 0;
                } else {
                    float alpha = min(col.a * i.uv.x * _Opacity, _MaxIllum);
                    col.a = min(1, alpha);
                }
                col.rgb *= col.a;
                return col;
            }
            ENDCG
        }
    }
}
