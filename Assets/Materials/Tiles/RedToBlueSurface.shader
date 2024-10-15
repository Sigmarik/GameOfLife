Shader "Custom/TeamColored"
{
    Properties
    {
        _Balance ("Team Balance", Integer) = 0
        _Highlight ("Heighlight", Integer) = 0
        _BaseEmission ("Emission", Float) = 0.0
        _HighlightStrength ("Highlight Strength", Float) = 1.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Rimlight ("Rimlight", Float) = 1.0
        _PositiveTeamColor("Positive Team Color", Color) = (1.0, 0.0, 0.0, 1.0)
        _NegativeTeamColor("Negative Team Color", Color) = (0.0, 0.0, 1.0, 1.0)
        _NeutralTeamColor ("Neutral Team Color",  Color) = (0.5, 0.0, 0.5, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldNormal;
        };

        int _Balance;
        int _Highlight;

        fixed _BaseEmission;
        fixed _HighlightStrength;
        fixed _Rimlight;

        fixed3 _PositiveTeamColor;
        fixed3 _NegativeTeamColor;
        fixed3 _NeutralTeamColor;

        half _Glossiness;
        half _Metallic;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float borderHighlight(float3 view, float3 normal) {
            float projection = dot(view, normal);

            float pow4 = pow(abs(projection), 0.4);

            return 1.0 - pow4;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed3 neutral = _NeutralTeamColor.rgb;
            fixed3 color_of_choice = neutral;
            fixed saturation = 1.0 - 1.0 / (1 + abs(_Balance));

            if (_Balance > 0) {
                color_of_choice = _PositiveTeamColor.rgb;
            } else if (_Balance < 0) {
                color_of_choice = _NegativeTeamColor.rgb;
            }
            
            o.Albedo = neutral * (1.0 - saturation) + color_of_choice * saturation;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;

            float emission_light = _BaseEmission + _HighlightStrength * _Highlight;
            float rim_light = borderHighlight(IN.worldNormal, IN.viewDir)
                * _Rimlight;

            o.Emission = o.Albedo * (emission_light + rim_light);

            // o.Albedo = fixed3(rim_light, rim_light, rim_light);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
