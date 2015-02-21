Shader "ABlumeUnlit" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
    Category {
       Lighting Off
       ZWrite On
       Cull Off
       SubShader {
            Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            Tags {Queue=Transparent}
               SetTexture [_MainTex] {
                    constantColor [_Color]
                    Combine texture * constant, texture * constant 
                }
            }
        } 
    }
}
