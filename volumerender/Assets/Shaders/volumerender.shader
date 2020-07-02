Shader "Custom/volumerender"
{
    Properties
    {
        _Volume("Volume", 3D) = "" {}
        _Iteration ("iter",Int)=10
        _Color("Color", Color) = (1, 1, 1, 1)
        _Intensity("Intensity", Range(0.0, 1.0)) = 0.1
        [Header(Ranges)]
        _MinX("MinX", Range(0, 1)) = 0.0
        _MaxX("MaxX", Range(0, 1)) = 1.0
        _MinY("MinY", Range(0, 1)) = 0.0
        _MaxY("MaxY", Range(0, 1)) = 1.0
        _MinZ("MinZ", Range(0, 1)) = 0.0
        _MaxZ("MaxZ", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent"
            "RenderType" = "Transparent" 
        }
        LOD 100

        Pass
        {
            Cull Back
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha 
            Lighting Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct Ray
            {
                float3 from;
                float3 dir;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldpos : TEXCOORD1;
                float3 objpos:TEXCOORD2;
            };
            float _Iteration;
            sampler3D _Volume;
            float4 _MainTex_ST;
            fixed _MinX, _MaxX, _MinY, _MaxY, _MinZ, _MaxZ;
            fixed _Intensity;
            fixed4 _Color;
            void intersect(Ray ray,out float near,out float far)
            {
                float3 inversed=1.0/ray.dir;
                float3 t1=inversed*(0.5-ray.from);
                float3 t2=inversed*(-0.5-ray.from);
                float3 tnear=min(t1,t2);
                float3 tfar=max(t1,t2);
                float2 tmp=min(tfar.xx,tfar.yz);
                far=min(tmp.x,tmp.y);
                tmp=max(tnear.xx,tnear.yz);
                near=max(tmp.x,tmp.y);
            }
            fixed4 sample(float3 pos)
            {
                fixed x = step(pos.x, _MaxX) * step(_MinX, pos.x);
                fixed y = step(pos.y, _MaxY) * step(_MinY, pos.y);
                fixed z = step(pos.z, _MaxZ) * step(_MinZ, pos.z);
                float4 tmpc=float4(tex3D(_Volume, pos).xyz,tex3D(_Volume, pos).a * x * y * z);
                return tex3D(_Volume, pos).a * x * y * z;
            }
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldpos=mul(unity_ObjectToWorld, v.vertex);
                o.objpos=v.vertex;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                Ray ray;
                ray.from=i.objpos;
                ray.dir=normalize(mul(unity_WorldToObject,i.worldpos-_WorldSpaceCameraPos));
                float near;
                float far;
                intersect(ray,near,far);
                float stepsize=far/float(_Iteration);
                float3 pos=ray.from;
                fixed3 finalcol=fixed3(0.0,0.0,0.0);
                fixed alpha=1.0;
                
                
                float3 lstep = ray.dir / _Iteration;
                float3 lpos = i.objpos;
                fixed output = 0.0;
                [loop]
                for(int i=0;i<_Iteration;++i)
                {
                    /*fixed4 col=sample(pos+0.5);
                    finalcol=finalcol+alpha*col.rgb;
                    alpha=alpha*(1-col.alpha);*/
                    fixed a = sample(lpos + 0.5);
                    output += (1 - output) * a * _Intensity;
                    lpos += lstep;
                    if (!all(max(0.5 - abs(lpos), 0.0)) || output > 0.99) break;
                }

                // sample the texture
                
                return _Color * output;
            }
            ENDCG
        }
    }
}
