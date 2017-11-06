using System;
using UnityEngine;

namespace grreuze {
    
    [Serializable]
    public struct DIOTextureAsset {

        public string name;
        public string prefix;
		
        public int textureType;

		public bool sRGB;
		// Manque light type
		public int alphaSource;
		public bool alphaIsTransparency;

		public bool createFromGrayscale;
		[Range(0, 0.3f)]
		public float bumpiness;
		public int normalMapFilter;

		public int spriteMode;
		public float pixelsPerUnit;
		public int meshType;
		[Range(0, 32)]
		public int extrudeEdges;
		public int spriteAlignment;
		public float pivotX;
		public float pivotY;

		public int nonPowerOf2;
		public bool readWriteEnabled;

		public bool generateMipMaps;
		public bool borderMipMaps;
		public int mipMapFiltering;
		public bool mipMapPreserveCoverage;
		public float alphaCutoff;
		public bool fadeoutMipMaps;
		public int mipMapFadeStart;
		public int mipMapFadeEnd;

		public int wrapMode;
		public int wrapModeU;
		public int wrapModeV;
		public int wrapModeW;
		public int filterMode;
		[Range(0, 16)]
		public int anisoLevel;

        //platform specific settings later
	}
    
    [Serializable]
	public struct DIOModelAsset {

        public string name;
        public string prefix;
		
		public float scaleFactor;
		public bool useFileScale;
		public int meshCompression;
		public bool readWriteEnabled;
		public bool optimizeMesh;
		public bool importBlendShapes;
		public bool generateColliders;
		public bool keepQuads;
		public bool weldVertices;
		public bool importVisibility;
		public bool importCameras;
		public bool importLights;
		public bool swapUVs;
		public bool generateLightmapUVs;

		public int normals;
		public int normalsMode;
		[Range(0, 180)]
		public int smoothingAngle;
		public int tangents;

		public bool importMaterials;
		public int materialNaming;
		public int materialSearch;

		public bool importAnimation;
		public int animationType;

		public bool resetPosition;
		public bool resetRotation;
	}
}