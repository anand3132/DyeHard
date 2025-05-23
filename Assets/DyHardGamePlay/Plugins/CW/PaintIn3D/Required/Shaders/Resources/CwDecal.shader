﻿Shader "Hidden/PaintCore/CwDecal"
{
	Properties
	{
		_ReplaceTexture("Replace Texture", 2D) = "white" {}
		_Texture("Texture", 2D) = "white" {}
		_Shape("Shape", 2D) = "white" {}
		_TileTexture("Tile Texture", 2D) = "white" {}
		_MaskTexture("Mask Texture", 2D) = "white" {}
		_LocalMaskTexture("Local Mask Texture", 2D) = "white" {}
	}

	SubShader
	{
		Blend Off
		Cull Off
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 0 // ALPHA_BLEND
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 1 // ALPHA_BLEND_INVERSE
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 2 // PREMULTIPLIED
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 3 // ADDITIVE
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 4 // ADDITIVE_SOFT
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 5 // SUBTRACTIVE
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 6 // SUBTRACTIVE_SOFT
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 7 // REPLACE
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 8 // REPLACE_ORIGINAL
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 9 // REPLACE_CUSTOM
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 10 // MULTIPLY_INVERSE_RGB
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 11 // BLUR
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 12 // NORMAL_BLEND
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 13 // NORMAL_REPLACE
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD CW_LINE_CLIP CW_QUAD_CLIP
			#define BLEND_MODE_INDEX 14 // FLOW
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD
			#define BLEND_MODE_INDEX 15 // NORMAL_REPLACE_ORIGINAL
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD
			#define BLEND_MODE_INDEX 16 // NORMAL_REPLACE_CUSTOM
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD
			#define BLEND_MODE_INDEX 17 // MIN
			#include "CwDecal.cginc"
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_local __ CW_LINE CW_QUAD
			#define BLEND_MODE_INDEX 18 // MAX
			#include "CwDecal.cginc"
			ENDCG
		}
	} // SubShader
} // Shader