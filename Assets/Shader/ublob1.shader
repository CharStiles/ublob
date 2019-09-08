Shader "Custom/ublob1"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
	SubShader {
		Tags
        {
             "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" 
		}
		
		Lighting Off
		ZTest LEqual
        ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull back
		
		//Fog { Mode Off }

		CGPROGRAM
		#pragma surface surf BlinnPhong  alphatest:_Cutoff alpha
		#pragma target 3.0
		#pragma glsl
        sampler2D _MainTex;

        #define MAX_DIST  40.0
        #define EPSILON  0.0001
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
    float3 c = float3(6.0,50.0,5.0);
    position = (position % c) + c*float3(0.5,0.5,0.5);
    position.y -= 4.0 - ((1-((position.z%MAX_DIST)/MAX_DIST)));
    float sphere = length(
        float3(
            position.x , 
            position.y, 
            position.z)
        )-0.8;
    
    // This is different from the ground equation because the UV is only 
    // between -1 and 1 we want more than 1/2pi of a wave per length of the 
    // screen so we multiply the position by a factor of 10 inside the trig 
    // functions. Since sin and cos oscillate between -1 and 1, that would be 
    // the entire height of the screen so we divide by a factor of 10
    float ground = position.y + sin(position.x * 10.) / 10. 
                              + cos(position.z * 10.) / 10. + 1.;
    position.x /=2;
    position.y /=3;
    position.z/=_CosTime.g*30 + position.z/position.x;
    sphere += (sin(20*position.x)*sin(200*position.y * (_CosTime.r-1.5)*0.60*sin(2*position.z +((_SinTime.g-1.5)*0.2) )))*abs(tan(position.z /position.x));
    // We want to return whichever one is closest to the ray, so we return the 
    // minimum distance
    return sphere;
}
float shadow( in float3 ro, in float3 rd, float mint, float maxt, float k )
{
    float res = 1.0;
    for( float t=mint; t < maxt; )
    {
        float h = scene(ro + rd*t);
        if( h<0.001 )
            return 0.0;
        res = min( res, k*h/t );
        t += h;
    }
    return res;
}
        fixed3 estimateNormal(fixed3 p) {
			return -normalize(fixed3(
				scene(fixed3(p.x + EPSILON, p.y, p.z)) - scene(fixed3(p.x, p.y, p.z)),
				scene(fixed3(p.x, p.y + EPSILON, p.z)) - scene(fixed3(p.x, p.y, p.z)),
				scene(fixed3(p.x, p.y, p.z + EPSILON)) - scene(fixed3(p.x, p.y, p.z))
			));
		}
// phong code by https://zhuanlan.zhihu.com/p/32911014

fixed3 phongContribForLight(fixed3 k_d, fixed3 k_s, float alpha, fixed3 p, fixed3 eye,
			fixed3 lightPos, fixed3 lightIntensity) {
			fixed3 N = estimateNormal(p);
			fixed3 L = normalize(lightPos - p);
			fixed3 V = normalize(eye - p);
			fixed3 R = normalize(reflect(-L, N));

			float dotLN = dot(L, N);
			float dotRV = dot(R, V);

			if (dotLN < 0.0) {
				// Light not visible from this point on the surface
				return fixed3(0.0, 0.0, 0.0);
			}

			if (dotRV < 0.0) {
				// Light reflection in opposite direction as viewer, apply only diffuse
				// component
				return lightIntensity * (k_d * dotLN);
			}
			return lightIntensity * (k_d * dotLN + k_s * pow(dotRV, alpha));
		}
		fixed3 phongIllumination(fixed3 k_a, fixed3 k_d, fixed3 k_s, float alpha, fixed3 p, fixed3 eye, fixed time) {
			const fixed3 ambientLight = 0.1 * fixed3(1.0, 1.0, 1.0);
			fixed3 color = ambientLight * k_a;
			fixed3 light1Pos = _WorldSpaceLightPos0.xyz;
			fixed3 light1Intensity = fixed3(0.4, 0.4, 0.4);
			color += phongContribForLight(k_d, k_s, alpha, p, eye,
				light1Pos,
				light1Intensity);
			fixed3 light2Pos = fixed3(2.0 * sin(0.37 * time),
				2.0 * cos(0.37 * time),
				2.0);
			fixed3 light2Intensity = fixed3(0.4, 0.4, 0.4);
			color += phongContribForLight(k_d, k_s, alpha, p, eye,
				light2Pos,
				light2Intensity);
			return color;
		}

float4 trace (float3 origin, float3 direction){
    const int steps = 64;
    const float smallNumber = 0.001;
    const float maxDist = MAX_DIST;
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
            float a = clamp(1. - (totalDistance / maxDist),0,0.75) * 1.4;
            fixed3 K_a = fixed3(0.9,0.7,0.8);// //ambient
		    fixed3 K_d = fixed3(a, a, a); // directiona
		    fixed3 K_s = fixed3(1,1-a,10);
		    float shininess = 0.010;
		    fixed3 color = phongIllumination(K_a, K_d, K_s, shininess, positionOnRay, origin, _Time.z ) ;
		
            return float4(color.r,color.g,color.b,a);
 
        }
        
        if (totalDistance > maxDist){
 
            return float4(0.,0.,0.,0.); // background color
        }
    }
    
    return float4(0.0,0.0,0.0,0.0);// background color
}

void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
    fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
    //float2 uv = -1. + (2. * IN.Position);
    float2 uv =  ((IN.screenPos.xy/_ScreenParams.xy));

    uv = (uv * float2(250.0,250.0));// + float2(.10,.10) ;
    uv.x = uv.x - 1.0;
    uv.y = uv.y - 0.5;
    uv.y *= (_ScreenParams.y/_ScreenParams.x); 
    float3 camPos = _WorldSpaceCameraPos;
	float3 camForward = mul((float3x3)unity_CameraToWorld , float3(0,0,1));
    float3 camRight = normalize(cross(float3(0.0, 1.0, 0.0), camForward));
    float3 camUp = normalize(cross(camForward, camRight));
    
    float fPersp = 2.0;
    float3 dir = normalize(uv.x * camRight + uv.y * camUp + camForward * fPersp);
   
    float4 t = trace(camPos.xyz, dir);
    o.Albedo = t.rgb;
    // Metallic and smoothness come from slider variables
   // o.Metallic = _Metallic;
   // o.Smoothness = _Glossiness;
    o.Alpha =t.a;
        }
        ENDCG
    }
    FallBack "BlinnPhong"
}
