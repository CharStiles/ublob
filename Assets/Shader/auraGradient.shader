Shader "UI/Unlit/AuraGradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
             "Queue"="Transparent" "RenderType"="Fade" "IgnoreProjector"="True" 
		}
		
		Lighting Off
		ZTest LEqual
        ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull back
        LOD 100

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
                float4 c : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 col : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.col = v.c;
                return o;
    
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float pulse = sin( (length(i.uv*2.0 + float2(cos(_Time.y),sin(_Time.y)))) + (_Time.y) );

                pulse = (pulse + 1.0) / 2.0;
                //pulse = (pulse * 0.6)+ 0.6;
                float4 col1 = float4(64./255.,74./255.,154./255.,1.);
                float4 col2 = float4(99./255.,50./255.,140./255.,1.);

                float4 grad = lerp(col1,col2,pulse);
                // sample the texture
                
                fixed4 col = grad;
                // apply fog
              
               // col.rgb = float3(1,1col.rgb;
                return float4(col.r,col.g,col.b,i.col.a);

            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
