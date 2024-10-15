Shader "Custom/TileShader"
{
    Properties
    {
        _Balance ("Team", Integer) = 0
        _Highlight ("Heighlight", Integer) = 0
        _BaseEmission ("Emission", Float) = 0.0
        _Opacity ("Opacity", Range(0,1)) = 0.3
        _HighlightOpacity ("Highlight Opacity", Range(0,1)) = 0.4
        _HighlightStrength ("Highlight Strength", Float) = 1.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _PositiveTeamColor("Positive Team Color", Color) = (1.0, 0.0, 0.0, 1.0)
        _NegativeTeamColor("Negative Team Color", Color) = (0.0, 0.0, 1.0, 1.0)
        _NeutralTeamColor ("Neutral Team Color",  Color) = (0.5, 0.0, 0.5, 1.0)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:blend

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        int _Balance;
        int _Highlight;

        fixed _BaseEmission;
        fixed _HighlightStrength;

        fixed _Opacity;
        fixed _HighlightOpacity;

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

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            
            fixed3 neutral = _NeutralTeamColor.rgb;
            fixed3 color_of_choice = neutral;

            if (_Balance > 0) {
                color_of_choice = _PositiveTeamColor.rgb;
            } else if (_Balance < 0) {
                color_of_choice = _NegativeTeamColor.rgb;
            }
            
            o.Albedo = _Highlight ? color_of_choice : (color_of_choice + neutral) / 2.0;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = _Highlight ? _HighlightOpacity : _Opacity;

            o.Emission = o.Albedo * (_BaseEmission + _HighlightStrength * _Highlight);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
