Shader "UI/TilingShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}  // UI에 사용할 텍스처
        _Offset ("Offset", Vector) = (0, 0, 0, 0)   // 타일링 오프셋
        _Color ("Tint", Color) = (1, 1, 1, 1)       // 색상 틴트
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }
        Blend SrcAlpha OneMinusSrcAlpha  // 투명도 처리
        Cull Off                         // 양면 렌더링
        ZWrite Off                       // 깊이 쓰기 비활성화

        // **Stencil 설정**: Mask와의 상호작용
        Stencil
        {
            Ref 1          // Mask가 기록하는 스텐실 값
            Comp Equal      // 스텐실 값이 1인 곳에서만 렌더링
            Pass Keep       // 패스 시 스텐실 값 유지
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;  // 메인 텍스처
            float4 _Offset;      // 타일링 오프셋
            float4 _Color;       // 색상 틴트

            // 정점 셰이더
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);  // 클립 공간으로 변환
                o.uv = v.uv + _Offset.xy;                  // 타일링 오프셋 적용
                return o;
            }

            // 프래그먼트 셰이더
            half4 frag (v2f i) : SV_Target
            {
                // 텍스처와 색상을 곱하여 최종 색상 생성
                half4 texColor = tex2D(_MainTex, i.uv) * _Color;

                // 알파가 0인 경우 픽셀 버림
                if (texColor.a <= 0.01)
                    discard;

                return texColor;
            }
            ENDCG
        }
    }
}
