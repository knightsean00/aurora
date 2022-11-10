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

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4x4 _NearColors;
                _NearColors[0] = fixed4(0, 0.9696946, 1, 1);
                _NearColors[1] = fixed4(1, 0.7, 0, 1);
                _NearColors[2] = fixed4(1, 0, 1, 1);
                _NearColors[3] = fixed4(1, 0, 1, 1);
                fixed4x4 _FarColors;
                _FarColors[0] = fixed4(0.46720982, 1, 0, 1);
                _FarColors[1] = fixed4(1, 0, 0, 1);
                _FarColors[2] = fixed4(1, 0, 1, 1);
                _FarColors[3] = fixed4(1, 0, 1, 1);
                int ix = int(i.uv.z);
                fixed4 col = lerp(_NearColors[ix], _FarColors[ix], i.uv.w);
                float alpha = min(col.a * i.uv.y * i.uv.x * _Opacity, _MaxIllum);
                col.rgb *= alpha;
                col.a = min(1, alpha);
                return col;
            }
            ENDCG
        }
    }
}
