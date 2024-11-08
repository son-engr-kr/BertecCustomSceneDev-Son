Shader "Bertec/CustomOutline"
{
   Properties
   {
      _Color( "Main Color", Color ) = (1,1,1,1)
      _OutlineThickness( "Outline Thickness", Range( 0.0, 1 ) ) = 0.2
      _OutlineColor( "Outline Color", Color ) = (0,0,0,1)
   }

      CGINCLUDE
#include "UnityCG.cginc"

   half _OutlineThickness;
   half4 _OutlineColor;

   struct appdata
   {
      half4 vertex : POSITION;
      half4 uv : TEXCOORD0;
      half3 normal : NORMAL;
      fixed4 color : COLOR;
   };

   struct v2f
   {
      half4 pos : POSITION;
      //half2 uv : TEXCOORD0; // not used?
      fixed4 color : COLOR;
   };
   ENDCG

   SubShader
   {
      Tags
      {
         "RenderType" = "Opaque"
         "Queue" = "Transparent+1"
      }

// don't draw into the depth buffer; this makes the object transparent if want and more importantly, allows the cursor object to float through it cleanly.
ZWrite Off
// Do this so the body is never obscured by the ellipse lines (otherwise they bleed into the body)
ZTest Always

      Pass
      {
         Name "OUTLINE"

         Cull Front
         CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag

         v2f vert( appdata v )
         {
            v2f o;
            o.pos = UnityObjectToClipPos( v.vertex );
            half3 norm = mul( (half3x3)UNITY_MATRIX_IT_MV, v.normal );
            half2 offset = TransformViewToProjection( norm.xy );
            o.pos.xy += offset * o.pos.z * _OutlineThickness;
            o.color = _OutlineColor;
            return o;
         }

         // This is where the color is actually applied to the outline
         fixed4 frag( v2f i ) : COLOR
         {
            fixed4 o;
            o = i.color;
            return o;
         }
         ENDCG
      }

      Pass
      {
         Name "Body"

         Color[_Color]
      }
   }
}
