Shader "Custom/blob"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Background" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma vertex vert 
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
            float4 Vertex:POSITION;
            float2 Position:TEXCOORD0;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


 
float scene(float3 position){
    float3 c = float3(2.0,2.0,2.0);
    position = (position % c) + c*float3(0.5,0.5,0.5);
    float sphere = length(
        float3(
            position.x , 
            position.y, 
            position.z)
        )-0.5;
    
    // This is different from the ground equation because the UV is only 
    // between -1 and 1 we want more than 1/2pi of a wave per length of the 
    // screen so we multiply the position by a factor of 10 inside the trig 
    // functions. Since sin and cos oscillate between -1 and 1, that would be 
    // the entire height of the screen so we divide by a factor of 10
    float ground = position.y + sin(position.x * 10.) / 10. 
                              + cos(position.z * 10.) / 10. + 1.;
    
    // We want to return whichever one is closest to the ray, so we return the 
    // minimum distance
    return sphere;
}

float4 trace (float3 origin, float3 direction){
    const int steps = 128;
    const float smallNumber = 0.001;
    const float maxDist = 10.;
    float dist = 0.;
    float totalDistance = 0.;
    float3 positionOnRay = origin;
    
    for(int i = 0 ; i < steps; i++){
        
        dist = scene(positionOnRay);
        
        // advance along the ray trajectory the amount that we know the ray
        // can travel without going through an object
        positionOnRay += dist * direction;
        
        // total distance is keeping track of how much the ray has traveled
        // thus far
        totalDistance += dist;
        
        // if we hit an object or are close enough to an object
        if (dist < smallNumber){
            // return the distance the ray had to travel normalized so be white
            // at the front and black in the back 
            return float4(1,1,1,1);//1. - (float4(totalDistance,totalDistance,totalDistance,totalDistance) / maxDist);
 
        }
        
        if (totalDistance > maxDist){
 
            return float4(0.,0.,0.,0.); // background color
        }
    }
    
    return float4(1.,0.,0.,0.);// background color
}
void vert (inout appdata_full v)
    {
        v.vertex += tex2Dlod(_MainTex,v.texcoord * float4(sin(_Time.x),cos(_Time.y/30.) ,1,1) );
    }
void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
    fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
    //float2 uv = -1. + (2. * IN.Position);
    float2 uv =  ((IN.screenPos.xy/_ScreenParams.xy));
    
    uv.x = uv.x - 5; /// WHY
    uv.y = uv.y - 3; ///WHYYY
    uv = (uv * float2(2.0,2.0));// - float2(2.0,2.0) ;

    uv.x *= (_ScreenParams.x/_ScreenParams.y); 
  	float3 Point = mul(unity_ObjectToWorld, IN.Vertex + float4(0,0,-1,0)) ;
    float3 camPos = _WorldSpaceCameraPos;
    float direction = normalize(Point - _WorldSpaceCameraPos.xyz);
	float3 lookat = float3(camPos.x,camPos.y,camPos.z + 1.0);	
	float3 camForward = mul((float3x3)unity_CameraToWorld , float3(0,0,1));
    float3 camRight = normalize(cross(float3(0.0, 1.0, 0.0), camForward));
    float3 camUp = normalize(cross(camForward, camRight));
    
    float fPersp = 10.0;
    float3 dir = normalize(uv.x * camRight + uv.y * camUp + camForward * fPersp);
    float3 rayOrigin = _WorldSpaceCameraPos.xyz;
    float4 t = trace(rayOrigin, dir);
    o.Albedo = t.rgb;
    // Metallic and smoothness come from slider variables
    o.Metallic = _Metallic;
    o.Smoothness = _Glossiness;
    o.Alpha = t.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
