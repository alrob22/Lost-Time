// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/CurvedWorld" {
    Properties {
        // Diffuse texture
        _MainTex ("Base (RGB)", 2D) = "white" {}
        // Degree of curvature
        _Curvature ("Curvature", Float) = 0.001
        // Render Mode selector: 0 = Opaque, 1 = Cutout, 2 = Transparent
        _RenderMode ("Render Mode (0: Opaque, 1: Cutout)", Float) = 0
        // Alpha cutoff value for Cutout mode
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        CGPROGRAM
        // Surface shader function is called surf, and vertex preprocessor function is called vert
        // addshadow used to add shadow collector and caster passes following vertex modification
        #pragma surface surf Lambert vertex:vert addshadow
        // These allow the shader to enable specific blocks of code for different render modes
        // #pragma shader_feature _RENDERMODE_CUTOUT
        // #pragma shader_feature _RENDERMODE_TRANSPARENT
 
        // Access the shaderlab properties
        uniform sampler2D _MainTex;
        uniform float _Curvature;
        uniform float _RenderMode;
        uniform float _Cutoff;
 
        // Basic input structure to the shader function
        // requires only a single set of UV texture mapping coordinates
        struct Input {
            float2 uv_MainTex;
        };
 
        // This is where the curvature is applied
        void vert( inout appdata_full v)
        {
            // Transform the vertex coordinates from model space into world space
            float4 vv = mul( unity_ObjectToWorld, v.vertex );
 
            // Now adjust the coordinates to be relative to the camera position
            vv.xyz -= _WorldSpaceCameraPos.xyz;
 
            // Reduce the y coordinate (i.e. lower the "height") of each vertex based
            // on the square of the distance from the camera in the z axis, multiplied
            // by the chosen curvature factor
            vv = float4( 0.0f, (vv.z * vv.z) * - _Curvature, 0.0f, 0.0f );
 
            // Now apply the offset back to the vertices in model space
            v.vertex += mul(unity_WorldToObject, vv);
        }
 
        // This is just a default surface shader
        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;

            // If _RenderMode is set to 1 (approximately), use clip() to discard fragments 
            // whose alpha is below the _Cutoff value.
            if (_RenderMode > 0.5 && _RenderMode < 1.5) {
                clip(o.Alpha - _Cutoff);
            }
        }
        ENDCG
    }
    // FallBack "Diffuse"
}