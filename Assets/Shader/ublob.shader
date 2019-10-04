Shader "Custom/ublob"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _AltTex ("altTex", 2D) = "white" {}
        _Speed ("Speed", Range(0,1)) = 0.5
        _Pale ("Pale", Range(0,1)) = 0.0
        _Yellow ("Yellow", Range(0,1)) = 0.0
        _Size("Size", Range(0,1)) = 0.0
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

        #define MAX_DIST  50.0
        #define EPSILON  0.0001
        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
            float4 Vertex:POSITION;
            float2 Position:TEXCOORD0;
        };

        half _Yellow;
        half _Size;
        half _Pale;
        half _Speed;
        fixed4 _Color;
        fixed4 picCol;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

 
float scene(float3 position){
    float3 c = float3(10.0,50.0,20.0);
    position = (position % c) + c*float3(0.5,0.5,0.5);
    position.y -= 4.0;
    float sphere = length(
        float3(
            position.x , 
            position.y, 
            position.z)
        )-1.8;
    position /=2;
    sphere += (sin(10*position.x)*sin(10*position.y * ((_CosTime.r* (_Speed*3.))-1.5)*0.6*sin(position.z +(((_SinTime.g* (_Speed*2.))-1.5)*0.2) )))*0.1;

    return sphere;
}
float scene_loose(float3 position){
    float3 c = float3(10.0,50.0,10.0);
    position = (position % c) + c*float3(0.5,0.5,0.5);
    position.y -= 4.0;
    float sphere = length(
        float3(
            position.x , 
            position.y, 
            position.z)
        )-0.8;
    
    position.x /=2;
    position.y /=3;
    position.z/=(_CosTime.g * (_Speed*2.))*30 + position.z/position.x;
    sphere += (sin(20*position.x)*sin(200*position.y * ((_CosTime.r* (_Speed*3.)) -1.5)*0.60*sin(2*position.z +(((_SinTime.g* (_Speed*2.))-1.5)*0.2) )))*abs(tan(position.z /position.x));

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
    const int steps = 8;
    const float smallNumber = 0.01;
    const float maxDist = 20.;
    float dist = 0.;
    float totalDistance = 0.;
    float3 positionOnRay = origin;
    
    for(int i = 0 ; i < steps; i++){
        
        if(_Size < 0.5){
            dist = scene_loose(positionOnRay);
        }
        else{

            dist = scene(positionOnRay);
        }
        
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
            fixed3 K_a = fixed3((dist * 10000) *(_Yellow),(dist * 10000)*_Yellow, (dist * 10055) +0.5); //  _LightColor0.rgb;// //ambient
		    fixed3 K_d = fixed3(11.54, 0.45, 0.55); // directiona
		    fixed3 K_s = fixed3(1., 0.4, 1.);
		    float shininess = 1.0;
		    fixed3 color = phongIllumination(K_a, K_d, K_s, shininess, positionOnRay, origin, (_Time.z * (6* _Speed) )) ;
            fixed4 colFromPic = float4(tex2D (_MainTex, positionOnRay.xy).rgb,a);
            fixed4 retCol = lerp(colFromPic, float4(color.r,color.g,color.b,a),  clamp(color.r,0,1) );
            retCol = pow(retCol, (1 - (max(_Pale * 0.7, 0.2))));
            
            return retCol;
 
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
    picCol = tex2D (_MainTex, IN.uv_MainTex) * _Color;
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
    o.Alpha =t.a;
    }
    ENDCG
    }
    FallBack "BlinnPhong"
}
