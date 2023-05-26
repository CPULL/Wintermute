Shader "Wintermute/HeartBeat"
{
  Properties
  { 
    _MainTex("Unused", 2D) = "white" {}
    _LineColor("Line Color", Color) = (1, 1, 1, 1)
    _DotColor("Dot Color", Color) = (.2, 1, .3, 1)
    _BackgroundColor("_Background Color", Color) = (.10, 11, .12, 1)
    _Rate("Rate", Range(0, 5)) = 1
    _Size("Dot Size", Range(0, .1)) = .025
    _DotRatio("Dot Ratio", Float) = 5
    _Len("Line Length", Range(0, 1)) = .1
    _Thick("Line Thickness", Range(0, .5)) = .025

    _Peak("Peak", Range(0, 1)) = .9
    _Base("Base", Range(0, 1)) = .25
    _Width("Width", Range(.01, .2)) = .05
    [MaterialToggle] _Under("Draw after peak", Float) = 1
  }

  SubShader
  {
    Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalRenderPipeline" "Queue" = "Transparent" }

    Blend SrcAlpha OneMinusSrcAlpha
    ZWrite Off
    Cull Back

    Pass
    {
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

      struct Attributes
      {
        float4 positionOS   : POSITION;
        float2 uv           : TEXCOORD0;
      };

      struct Varyings
      {
        float4 positionHCS  : SV_POSITION;
        float2 uv           : TEXCOORD0;
      };

      CBUFFER_START(UnityPerMaterial)
      half4 _LineColor, _DotColor, _BackgroundColor;
      float _Rate, _Size, _DotRatio, _Len, _Thick;
      float _Peak, _Base, _Width, _Under;
      CBUFFER_END

      Varyings vert(Attributes IN)
      {
        Varyings OUT;
        OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
        OUT.uv = IN.uv;
        return OUT;
      }

      half4 frag(Varyings IN) : SV_Target
      {
        float2 uv = IN.uv;
        half4 col = _BackgroundColor;
        
        // Position according to time and rate
        float px = frac(_Rate * _Time.y) * (1+_Len);
        float py = _Base;

        if (uv.x > .5 - _Width && uv.x <= .5 + _Width) {
          // If we are close to the center draw the two lines
          py = -((_Peak - _Base) / _Width) * abs(uv.x - .5) + _Peak;
        }
        else if (_Under != 0 && uv.x > .5 && uv.x < .5 + 2 * _Width) {
          // If we want also the downbeat and we are close to the right size after the main beat
          if (uv.x < (2 * _Peak - 1) / ((_Peak - _Base) / _Width) + .5) {
            // Continue the normal descending line as before
            py = -((_Peak - _Base) / _Width) * abs(uv.x - .5) + _Peak;
          }
          else {
            // Draw a line going to the minimum point (1 - _Peak) to the _Base line
            float a = (1 - _Peak - _Base) / (((2 * _Peak - 1) / ((_Peak - _Base) / _Width) + .5) - (.5 + 2 * _Width));
            float b = _Base - a * (.5 + 2 * _Width);
            py = a * uv.x + b;
          }
        }

        if (uv.x > px - _Len && uv.x < px && abs(uv.y - py) < _Thick) {
          // If we are close to the point we were calculating use the line color
          col = _LineColor;
          col.a = (1 - px + uv.x);
          col.a *= col.a;
        }

        // Calculate the distance from the end of the line
        float dist = (px - uv.x) * (px - uv.x) * _DotRatio * _DotRatio + (py - uv.y) * (py - uv.y);
        if (dist < _Size * _Size) {
          // If we are close enough draw the dot with its color
          col = _DotColor;
        }
        return col;
      }
    ENDHLSL
    }
  }
}
