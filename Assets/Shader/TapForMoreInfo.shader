Shader "UI/Unlit/TapForMoreInfo"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
             "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" 
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
                float4 c : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.c = v.c;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float pulse = sin( (length(i.uv) * 12.6) + (_Time.z) );

                pulse = (pulse + 1.0) / 2.0;
                pulse = (pulse * 0.6)+ 0.6;

                // sample the texture
                
                fixed4 col = tex2D(_MainTex, i.uv) * float4(1,1,1,pulse);
                // apply fog
              
                col.rgb = float3(1,1,1)-col.rgb;
                return float4(col.rgb, i.c.a * col.a);

            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
