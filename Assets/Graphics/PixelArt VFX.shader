Shader "Universal Render Pipeline/PixelArt VFX"
{
  // The _BaseMap variable is visible in the Material's Inspector, as a field
  // called Base Map.
  Properties
  {
    [MainTexture] _MainTex("Base Map", 2D) = "white"
    _Bits("Bits", Integer) = 64
    _ColW("Color Weight", float) = 1
    _ColR("Color Range", float) = 40
    _PixW("Pixel Weight", float) = 2
    _PixR("Pixel Range", float) = 40
    _Sat("Saturation Weight", float) = 1
  }

  SubShader
  {
    Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

    Pass
    {
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

      struct Attributes
      {
        float4 positionOS   : POSITION;
        // The uv variable contains the UV coordinate on the texture for the given vertex.
        float2 uv           : TEXCOORD0;
      };

      struct Varyings
      {
        float4 positionHCS  : SV_POSITION;
        // The uv variable contains the UV coordinate on the texture for the  given vertex.
        float2 uv           : TEXCOORD0;
      };

      // This macro declares _BaseMap as a Texture2D object.
      TEXTURE2D(_MainTex);
      // This macro declares the sampler for the _BaseMap texture.
      SAMPLER(sampler_MainTex);

      CBUFFER_START(UnityPerMaterial)
      // The following line declares the _BaseMap_ST variable, so that you
      // can use the _BaseMap variable in the fragment shader. The _ST
      // suffix is necessary for the tiling and offset function to work.
      float4 _MainTex_ST;
      float4 _MainTex_TexelSize;
      float _Bits;
      float _ColW;
      float _ColR;
      float _PixW;
      float _PixR;
      float _Sat;
      CBUFFER_END

      Varyings vert(Attributes IN)
      {
        Varyings OUT;
        OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
        // The TRANSFORM_TEX macro performs the tiling and offset transformation.
        OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
        return OUT;
      }

      half4 frag(Varyings IN) : SV_Target
      {

        float2 uv = floor(IN.uv * _Bits) / _Bits;
        half4 colpx = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
        float mean = (colpx.r + colpx.g + colpx.b) * .3333334;


        half4 resC, resP;
        resC.r = (int)(color.r * _ColR) / _ColR;
        resC.g = (int)(color.g * _ColR) / _ColR;
        resC.b = (int)(color.b * _ColR) / _ColR;
        resC.a = 1;

        resP.r = (int)(colpx.r * _PixR) / _PixR;
        resP.g = (int)(colpx.g * _PixR) / _PixR;
        resP.b = (int)(colpx.b * _PixR) / _PixR;
        resP.a = 1;

        half4 res = (_ColW * resC + _PixW * resP + _Sat * mean) / (_ColW + _PixW + _Sat);
        return res;
      }
      ENDHLSL
    }
  }
}