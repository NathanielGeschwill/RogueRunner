// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/WorldBender"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1)
        _VertexOffset("Vertex Offset", Float) = (0,0,0)
        _zFix("Z Fix", float) = 0.0
        _Amount("Amount", float) = .00009
        _Bending("Bool Bend", float) = 0.0

    }

    SubShader
    {
        
        Pass
        {
            CGPROGRAM
 
            #include "UnityCG.cginc"  

            #pragma vertex vertexFunc
            #pragma fragment FragmentFunc

            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f{
                float4 position : SV_POSITION;  
                float2 uv : TEXCOORD0;
            };


            fixed4 _VertexOffset;
            fixed4 _Color;
            uniform float4 _worldpos;
            uniform float1 _Amount;
            uniform float4 camera_location;
            uniform float4 _ver;
            uniform float1 _zFix;
            uniform float1 _Bending;
            
            
            sampler2D _MainTex;

            v2f vertexFunc(appdata IN)
            {
                v2f OUT;
                camera_location = float4(_WorldSpaceCameraPos.x, _WorldSpaceCameraPos.y, _WorldSpaceCameraPos.z, 1);
                _worldpos = (mul(unity_ObjectToWorld, IN.vertex));
            
                float dist = distance(camera_location, _worldpos);
                _ver = (1 * _Amount)*(pow(dist, 2));

                if(_zFix==1 && _Bending == 1)
                {
                    IN.vertex = float4(IN.vertex.x - _ver.x, IN.vertex.y, IN.vertex.z, 1);
                }
                else if (_zFix == -1 && _Bending == 1)
                {
                    IN.vertex = float4(IN.vertex.x, IN.vertex.y, IN.vertex.z - _ver.z, 1);
                }
                else if (_Bending == 1)
                {
                    IN.vertex = float4(IN.vertex.x, IN.vertex.y + _ver.y, IN.vertex.z, 1);
                }
                
                OUT.position = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;

                return OUT;
            }
            
            fixed4 FragmentFunc(v2f IN) : SV_Target
            {
                //float dist = distance(IN.position_in_world_space, float4())

                fixed4 pixelColor = tex2D(_MainTex, IN.uv);

                return pixelColor * _Color;
            }

            ENDCG
        }
    }
        
}