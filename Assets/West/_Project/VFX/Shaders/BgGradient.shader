Shader "Custom/BgGradient"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (1, 1, 1, 1)    // 기본 상단 색상
        _BottomColor ("Bottom Color", Color) = (0, 0, 0, 1) // 아래쪽에서 위로 올라올 색상
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // 버텍스 데이터 구조체
            struct appdata
            {
                float4 vertex : POSITION;
            };

            // 버텍스 셰이더와 프래그먼트 셰이더 간의 데이터
            struct v2f
            {
                float4 pos : SV_POSITION; // 클립 공간 좌표
                float2 uv : TEXCOORD0;    // 정규화된 UV 좌표
            };

            // 인스펙터에서 설정할 색상
            float4 _TopColor;   
            float4 _BottomColor;

            // 버텍스 셰이더: 클립 좌표로 변환하고 UV 계산
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); // 월드 -> 클립 공간 변환

                // 정규화된 UV 계산
                o.uv = o.pos.xy / o.pos.w; // NDC 좌표
                o.uv = o.uv * 0.5 + 0.5;   // [0, 1] 범위로 변환

                return o;
            }

            // 프래그먼트 셰이더: TopColor 위에 BottomColor가 올라오는 그라디언트
            fixed4 frag (v2f i) : SV_Target
            {
                // UV의 Y 좌표를 이용해 그라디언트 계산 (0: 하단, 1: 상단)
                float gradient = 1.0 - saturate(i.uv.y);

                // BottomColor가 점차 위로 올라오도록 색상 혼합
                return lerp(_BottomColor,_TopColor , gradient);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
