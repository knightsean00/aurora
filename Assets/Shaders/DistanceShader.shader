Shader "Shaders/DistanceShader"
{
    Properties
    {
        _NearColor ("NearColor", Color) = (1,1,1,1)
        _FarColor ("FarColor", Color) = (1,1,1,1)
        _NearDist ("NearDist", Float) = 2
        _FarDist ("FarDist", Float) = 10
        _MaxIllum ("Max Illumination", Float) = 10
        _Position ("Position", Vector) = (0,0,0,0)
        _Opacity ("Global Opacity", Float) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off
        //ZWrite Off
        ZTest Always
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Position;
            float _NearDist;
            float _FarDist;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex /* - _Position */);
                o.uv.z = (distance(v.vertex, _Position) - _NearDist) / (_FarDist - _NearDist);
                o.uv.xy = v.uv.xy;
                return o;
            }

            fixed4 _NearColor;
            fixed4 _FarColor;
            float _Opacity;
            float _MaxIllum;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = lerp(_NearColor, _FarColor, i.uv.z);
                col.a = min(col.a * i.uv.y * i.uv.x * _Opacity, _MaxIllum);
                return col;
            }
            ENDCG
        }
    }
}
