using UnityEditor;
using UnityEngine;

namespace grreuze {

	public class DIOAssetProcessor : AssetPostprocessor {

		void OnPreprocessTexture() {
			if (assetImporter.userData != "")
				return;

			int fileNameIndex = assetPath.LastIndexOf('/');
			string fileName = assetPath.Substring(fileNameIndex + 1);
			
			var importer = (TextureImporter)assetImporter;
			
            importer.userData = "DIO_applied";
			
			DIOData.LoadData();

			for (int i = DIOData.tex.Length-1; i >= 0; i--) {
				DIOTextureAsset tex = DIOData.tex[i];

                if (fileName.Contains(tex.prefix)) {

					importer.textureType = (TextureImporterType)tex.textureType;

					importer.sRGBTexture = tex.sRGB;
					importer.alphaSource = (TextureImporterAlphaSource)tex.alphaSource;
					importer.alphaIsTransparency = tex.alphaIsTransparency;

					importer.convertToNormalmap = tex.createFromGrayscale;
					importer.heightmapScale = tex.bumpiness;
					importer.normalmapFilter = (TextureImporterNormalFilter)tex.normalMapFilter;

					importer.spriteImportMode = (SpriteImportMode)tex.spriteMode;
					importer.spritePixelsPerUnit = tex.pixelsPerUnit;
					
                    TextureImporterSettings texSet = new TextureImporterSettings();
					importer.ReadTextureSettings(texSet);
					texSet.spriteMeshType = (SpriteMeshType)tex.meshType;
					texSet.spriteExtrude = (uint)tex.extrudeEdges;
					texSet.spriteAlignment = tex.spriteAlignment;
					texSet.spritePivot = new Vector2(tex.pivotX, tex.pivotY);
					importer.SetTextureSettings(texSet);
					
					importer.npotScale = (TextureImporterNPOTScale)tex.nonPowerOf2;
					importer.isReadable = tex.readWriteEnabled;

					importer.mipmapEnabled = tex.generateMipMaps;
					importer.borderMipmap = tex.borderMipMaps;
					importer.mipmapFilter = (TextureImporterMipFilter)tex.mipMapFiltering;
					importer.mipMapsPreserveCoverage = tex.mipMapPreserveCoverage;
					importer.alphaTestReferenceValue = tex.alphaCutoff;
					importer.fadeout = tex.fadeoutMipMaps;
					importer.mipmapFadeDistanceStart = tex.mipMapFadeStart;
					importer.mipmapFadeDistanceEnd = tex.mipMapFadeEnd;

					importer.wrapMode = (TextureWrapMode)tex.wrapMode;
					importer.wrapModeU = (TextureWrapMode)tex.wrapModeU;
					importer.wrapModeV = (TextureWrapMode)tex.wrapModeV;
					importer.wrapModeW = (TextureWrapMode)tex.wrapModeW;

					importer.filterMode = (FilterMode)tex.filterMode;
					importer.anisoLevel = tex.anisoLevel;

                    return;
                    // Let's see about platform specific settings afterward
                }
			}
		}

		void OnPreprocessModel() {
			if (assetImporter.userData != "")
				return;

			int fileNameIndex = assetPath.LastIndexOf('/');
			string fileName = assetPath.Substring(fileNameIndex + 1);

			var importer = (ModelImporter)assetImporter;

			importer.userData = "DIO_applied";

			DIOData.LoadData();

			for (int i = 0; i < DIOData.model.Length; i++) {
				DIOModelAsset model = DIOData.model[i];

				if (fileName.Contains(model.prefix)) {

					importer.globalScale = model.scaleFactor;
					importer.useFileScale = model.useFileScale;
					importer.meshCompression = (ModelImporterMeshCompression)model.meshCompression;
					importer.isReadable = model.readWriteEnabled;
					importer.optimizeMesh = model.optimizeMesh;
					importer.importBlendShapes = model.importBlendShapes;
					importer.addCollider = model.generateColliders;
					importer.keepQuads = model.keepQuads;
					importer.weldVertices = model.weldVertices;
					importer.importVisibility = model.importVisibility;
					importer.importCameras = model.importCameras;
					importer.importLights = model.importLights;
					importer.swapUVChannels = model.swapUVs;
					importer.generateSecondaryUV = model.generateLightmapUVs;

					importer.importNormals = (ModelImporterNormals)model.normals;
					importer.normalCalculationMode = (ModelImporterNormalCalculationMode)model.normalsMode;
					importer.normalSmoothingAngle = model.smoothingAngle;
					importer.importTangents = (ModelImporterTangents)model.tangents;

					importer.importMaterials = model.importMaterials;
					importer.materialName = (ModelImporterMaterialName)model.materialNaming;
					importer.materialSearch = (ModelImporterMaterialSearch)model.materialSearch;

					importer.animationType = (ModelImporterAnimationType)model.animationType;
					importer.importAnimation = model.importAnimation;

					return;
				}
			}
		}

		void OnPostprocessModel(GameObject import) {
			int fileNameIndex = assetPath.LastIndexOf('/');
			string fileName = assetPath.Substring(fileNameIndex + 1);
			
			DIOData.LoadData();

			for (int i = 0; i < DIOData.model.Length; i++) {
				DIOModelAsset model = DIOData.model[i];
				
				if (fileName.Contains(model.prefix)) {

					if (model.resetPosition)
						import.transform.position = Vector3.zero;

					if (model.resetRotation) 
						import.transform.rotation = Quaternion.identity;
				}
			}
		}

	}
}