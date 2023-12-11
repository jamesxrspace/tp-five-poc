Shader "Custom/Unlit/Rotation/DoubleSide"
{
    Properties
    {
        _MainTex("Image", 2D) = "white" {}
        _Rotation ("Rotation", Float) = 0
        // if true : Rotate by clockwise.
        [Toggle] _ToggleInverseRotation ("InverseRotation", int) = 1
        [Toggle] _ToggleFlipX ("FlipX", Float) = 0
        [Toggle] _ToggleFlipY ("FlipY", Float) = 0
        [Enum(DOUBLE_SIDE, 0, ONESIDE_BACK, 1, ONESIDE_FRONT, 2)] _DisplayMode ("Display Mode", int) = 0
        [Enum(LETTERBOX_DISABLE, 0, LETTERBOX_ENABLE, 1)] _Letterbox ("Letterbox", int) = 0
        _LetterboxColor("LetterboxColor", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Geometry"
            "IgnoreProjector"="True"
            "RenderType"="Opaque"
        }
        Lighting Off

        Pass
        {
            Cull [_DisplayMode]
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Rotation;
            int _ToggleInverseRotation;
            int _InverseRotation;
            float _ToggleFlipX;
            float _ToggleFlipY;
            float _FlipX;
            float _FlipY;
            int _Letterbox;
            float4 _LetterboxColor;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // v.texcoord : The model's each vertex's uv value
                o.uv = v.texcoord;

                // --- Rotation ---
                _InverseRotation = _ToggleInverseRotation ? 1 : -1;
                _Rotation = _InverseRotation * _Rotation * 0.01745329222f; //(3.1415926f/180.0f);
                float s = sin(_Rotation);
                float c = cos(_Rotation);

                _FlipX = _ToggleFlipX ? -1 : 1;
                _FlipY = _ToggleFlipY ? -1 : 1;

                // --- Fix backward UV ---
                const float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                const float val = dot(v.normal, viewDir);
                if (val < 0.0)
                    o.uv.x = -o.uv.x + 1;
                // --- Fix backward UV End ---

                // Rotation with flip X and Y
                float2x2 rotationMatrix = float2x2(c * _FlipX, -s * _FlipY, s * _FlipX, c * _FlipY);

                rotationMatrix *= 0.5;
                rotationMatrix += 0.5;
                rotationMatrix = rotationMatrix * 2 - 1;
                o.uv.xy -= 0.5;

                // Use mul to rotate uv with rotationMatrix
                o.uv.xy = mul(o.uv, rotationMatrix);
                // translate UV pos back to original position
                o.uv.xy += 0.5;
                // --- Rotation End ---

                // Finally, use TransformTex uv with (xy)scale and (zw)translation，according the Tiling and Offset of _MainTex
                o.uv = TRANSFORM_TEX(o.uv, _MainTex);

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Get color from _MainTex by uv mapping.
                half4 c = tex2D(_MainTex, i.uv);

                // Letterbox
                if (_Letterbox == 1
                    && (i.uv.x <= 0.0f || i.uv.y <= 0.0f || i.uv.x >= 1.0f || i.uv.y >= 1.0f))
                    c = _LetterboxColor;

                return c;
            }
            ENDCG
        }
    }
}