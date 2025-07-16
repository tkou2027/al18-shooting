Shader "Custom/TimeStopShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EffectColor ("Effect Color", Color) = (0.0, 0.0, 0.3, 1.0)
        _EffectCenterWorld ("Effect Center (World)", Vector) = (0, 0, 0, 0)
        _EffectRadius ("Effect Radius", Float) = 0.0
        _RadiusRatio ("Radius Ratio", Float) = 1.0
        // wave
        _WaveAmplitude ("Wave Amplitude", Float) = 0.2
        _WaveFrequency ("Wave Frequency", Float) = 10.0
        _TimeScale ("Wave Move Time Speed", Float) = 0.0
        // border
        _BorderColor ("Border Color", Color) = (0.0, 0.0, 0.3, 1.0)
        _BorderRadius ("Border Radius", Float) = 0.0

        // simple
        _UseSimple ("Use Simple Color", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Sprite"
            "CanUseSpriteAtlas"="True"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            sampler2D _MainTex;

            fixed4 _EffectColor;
            float4 _EffectCenterWorld;
            float _EffectRadius;
            float _RadiusRatio;
            // Wave
            float _WaveAmplitude;
            float _WaveFrequency;
            float _TimeScale;

            // Border
            fixed4 _BorderColor;
            float _BorderRadius;

            float _UseSimple;

            float getWaveRadius(float2 pos, float2 center)
            {
                 float2 delta = pos - center;
                 float dist = length(delta);
                 float angle = atan2(delta.y, delta.x);

                 float time = _Time.y * _TimeScale;
                 float wave = sin(_WaveFrequency * angle + time) - 1; // max 0
                 return _WaveAmplitude * wave;
            }

            fixed4 getFilteredColor(fixed4 col, bool insideBorder)
            {
                fixed4 targetCol = insideBorder ? _EffectColor : _BorderColor;
                
                // calc saturation
                float maxRGB = max(col.r, max(col.g, col.b));
                float minRGB = min(col.r, min(col.g, col.b));
                float saturation = (maxRGB - minRGB) * 2; // 2 is a factor
                
                // lerp based on saturation
                fixed4 colFiltered = col;
                colFiltered.rgb = (1 - saturation) * col.rgb + saturation * targetCol.rgb;
 
                // if use simple, just return effect color
                return _UseSimple > 0.5 ? _EffectColor : colFiltered;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col = lerp(col, _BorderColor, _UseSimple);
                
                float dist = distance(i.worldPos.xy, _EffectCenterWorld.xy);
                float waveRadius = getWaveRadius(i.worldPos.xy, _EffectCenterWorld.xy);
                float effectRadius = _EffectRadius * _RadiusRatio;

                return dist < waveRadius + effectRadius
                    ? getFilteredColor(col, dist < waveRadius + effectRadius -_BorderRadius)
                    : col;
            }
            ENDCG
        }
    }
}
