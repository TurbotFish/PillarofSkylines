using UnityEditor;

namespace grreuze {

    [System.Serializable]
    public static class DIOData {

        public static DIOTextureAsset[] tex;
        public static DIOModelAsset[] model;

		#region Initialize

		public static DIOTextureAsset[] InitializeTextureOptions() {
            tex = new DIOTextureAsset[3];
            
            for (int i = 0; i < tex.Length; i++) {
                DIOTextureAsset tex = DIOData.tex[i];

                TextureImporterSettings settings = new TextureImporterSettings();

                switch (i) {
                    default:
                    case 0:
                        settings.ApplyTextureType(TextureImporterType.Default);
                        tex.name = "Texture";
                        tex.prefix = "T_";
                        break;
                    case 1:
                        settings.ApplyTextureType(TextureImporterType.Sprite);
                        tex.name = "Sprite";
                        tex.prefix = "SP_";
                        break;
                    case 2:
                        settings.ApplyTextureType(TextureImporterType.NormalMap);
                        tex.name = "Normal Map";
                        tex.prefix = "_N";
                        break;
                }

                tex.textureType = (int)settings.textureType;

                tex.createFromGrayscale = settings.convertToNormalMap;
                tex.bumpiness = 0.25f;
                tex.normalMapFilter = (int)settings.normalMapFilter;
                
                tex.spriteMode = settings.spriteMode;
                tex.pixelsPerUnit = 100;
                tex.meshType = (int)settings.spriteMeshType;
                tex.extrudeEdges = 1;
                tex.spriteAlignment = settings.spriteAlignment;
                tex.pivotX = 0.5f;
                tex.pivotY = 0.5f;

                tex.sRGB = true;
                tex.alphaSource = 1;
                tex.alphaIsTransparency = true;
                tex.nonPowerOf2 = (int)settings.npotScale;
                tex.readWriteEnabled = settings.readable;

                tex.generateMipMaps = settings.mipmapEnabled;
                tex.borderMipMaps = settings.borderMipmap;
                tex.mipMapFiltering = (int)settings.mipmapFilter;
                tex.mipMapPreserveCoverage = settings.mipMapsPreserveCoverage;
				tex.alphaCutoff = 0.5f;
                tex.fadeoutMipMaps = settings.fadeOut;
                tex.mipMapFadeStart = 1;
                tex.mipMapFadeEnd = 3;

                tex.wrapMode = (int)settings.wrapMode;
                tex.wrapModeU = (int)settings.wrapModeU;
                tex.wrapModeV = (int)settings.wrapModeV;
                tex.wrapModeW = (int)settings.wrapModeW;
                tex.filterMode = 1; //Bilinear
                tex.anisoLevel = settings.aniso;

                // Platform specific stuff aaaa

                DIOData.tex[i] = tex;
            }
            return tex;
        }

		public static DIOModelAsset[] InitializeModelOptions() {
			model = new DIOModelAsset[3];

			for (int i = 0; i < model.Length; i++) {
				DIOModelAsset model = DIOData.model[i];
				
				switch (i) {
					default:
					case 0:
						model.name = "Static Mesh";
						model.prefix = "SM_";
						break;
					case 1:
						model.name = "Animation";
						model.prefix = "A_";
						break;
					case 2:
						model.name = "Blockout Mesh";
						model.prefix = "SMB_";
						break;
				}

				model.scaleFactor = 1;
				model.useFileScale = true;
				model.meshCompression = 0;
				model.readWriteEnabled = true;
				model.optimizeMesh = true;
				model.importBlendShapes = true;
				model.generateColliders = i == 2;
				model.keepQuads = false;
				model.weldVertices = true;
				model.importVisibility = true;
				model.importCameras = false;
				model.importLights = false;
				model.swapUVs = false;
				model.generateLightmapUVs = false;

				model.normals = 0;
				model.normalsMode = 4;
				model.smoothingAngle = 60;
				model.tangents = 1;

				model.importMaterials = false;
				model.materialNaming = 0;
				model.materialSearch = 1;

				model.importAnimation = i == 1;
				model.animationType = i == 1 ? 2 : 0;

				model.resetPosition = true;
				model.resetRotation = true;

				DIOData.model[i] = model;
			}
			return model;
		}

        public static void Initialize() {
            InitializeTextureOptions();
			InitializeModelOptions();
        }
		#endregion

		#region Load

		public static DIOTextureAsset[] LoadTextureOptions() {
            tex = new DIOTextureAsset[EditorPrefs.GetInt("DIO.tex.Length")];

            for (int i = 0; i < tex.Length; i++) {
                DIOTextureAsset tex = DIOData.tex[i];

                tex.name = EditorPrefs.GetString("DIO.tex[" + i + "].name");
                tex.prefix = EditorPrefs.GetString("DIO.tex[" + i + "].prefix");

                tex.textureType = EditorPrefs.GetInt("DIO.tex[" + i + "].textureType");

                tex.createFromGrayscale = EditorPrefs.GetBool("DIO.tex[" + i + "].createFromGrayscale");
                tex.bumpiness = EditorPrefs.GetFloat("DIO.tex[" + i + "].bumpiness");
                tex.normalMapFilter = EditorPrefs.GetInt("DIO.tex[" + i + "].normalMapFilter");
                tex.spriteMode = EditorPrefs.GetInt("DIO.tex[" + i + "].spriteMode");
                tex.pixelsPerUnit = EditorPrefs.GetFloat("DIO.tex[" + i + "].pixelsPerUnit");
                tex.meshType = EditorPrefs.GetInt("DIO.tex[" + i + "].meshType");
                tex.extrudeEdges = EditorPrefs.GetInt("DIO.tex[" + i + "].extrudeEdges");
                tex.spriteAlignment = EditorPrefs.GetInt("DIO.tex[" + i + "].spriteAlignment");
                tex.pivotX = EditorPrefs.GetFloat("DIO.tex[" + i + "].pivotX");
                tex.pivotY = EditorPrefs.GetFloat("DIO.tex[" + i + "].pivotY");

                tex.sRGB = EditorPrefs.GetBool("DIO.tex[" + i + "].sRGB");
                tex.alphaSource = EditorPrefs.GetInt("DIO.tex[" + i + "].alphaSource");
                tex.alphaIsTransparency = EditorPrefs.GetBool("DIO.tex[" + i + "].alphaIsTransparency");
                tex.nonPowerOf2 = EditorPrefs.GetInt("DIO.tex[" + i + "].nonPowerOf2");
                tex.readWriteEnabled = EditorPrefs.GetBool("DIO.tex[" + i + "].readWriteEnabled");

                tex.generateMipMaps = EditorPrefs.GetBool("DIO.tex[" + i + "].generateMipMaps");
                tex.borderMipMaps = EditorPrefs.GetBool("DIO.tex[" + i + "].borderMipMaps");
                tex.mipMapFiltering = EditorPrefs.GetInt("DIO.tex[" + i + "].mipMapFiltering");
                tex.mipMapPreserveCoverage = EditorPrefs.GetBool("DIO.tex[" + i + "].mipMapPreserveCoverage");
				tex.alphaCutoff = EditorPrefs.GetFloat("DIO.tex[" + i + "].alphaCutoff");
				tex.fadeoutMipMaps = EditorPrefs.GetBool("DIO.tex[" + i + "].fadeoutMipMaps");
                tex.mipMapFadeStart = EditorPrefs.GetInt("DIO.tex[" + i + "].mipMapFadeStart");
                tex.mipMapFadeEnd = EditorPrefs.GetInt("DIO.tex[" + i + "].mipMapFadeEnd");

                tex.wrapMode = EditorPrefs.GetInt("DIO.tex[" + i + "].wrapMode");
                tex.wrapModeU = EditorPrefs.GetInt("DIO.tex[" + i + "].wrapModeU");
                tex.wrapModeV = EditorPrefs.GetInt("DIO.tex[" + i + "].wrapModeV");
                tex.wrapModeW = EditorPrefs.GetInt("DIO.tex[" + i + "].wrapModeW");
                tex.filterMode = EditorPrefs.GetInt("DIO.tex[" + i + "].filterMode");
                tex.anisoLevel = EditorPrefs.GetInt("DIO.tex[" + i + "].anisoLevel");

                // Platform specific stuff here as well

                DIOData.tex[i] = tex;
			}
			return tex;
        }

		public static DIOModelAsset[] LoadModelOptions() {
			model = new DIOModelAsset[EditorPrefs.GetInt("DIO.model.Length")];

			for (int i = 0; i < model.Length; i++) {
				DIOModelAsset model = DIOData.model[i];

				model.name = EditorPrefs.GetString("DIO.model[" + i + "].name");
				model.prefix = EditorPrefs.GetString("DIO.model[" + i + "].prefix");

				model.scaleFactor = EditorPrefs.GetFloat("DIO.model[" + i + "].scaleFactor");
				model.useFileScale = EditorPrefs.GetBool("DIO.model[" + i + "].useFileScale");
				model.meshCompression = EditorPrefs.GetInt("DIO.model[" + i + "].meshCompression");
				model.readWriteEnabled = EditorPrefs.GetBool("DIO.model[" + i + "].readWriteEnabled");
				model.optimizeMesh = EditorPrefs.GetBool("DIO.model[" + i + "].optimizeMesh");
				model.importBlendShapes = EditorPrefs.GetBool("DIO.model[" + i + "].importBlendShapes");
				model.generateColliders = EditorPrefs.GetBool("DIO.model[" + i + "].generateColliders");
				model.keepQuads = EditorPrefs.GetBool("DIO.model[" + i + "].keepQuads");
				model.weldVertices = EditorPrefs.GetBool("DIO.model[" + i + "].weldVertices");
				model.importVisibility = EditorPrefs.GetBool("DIO.model[" + i + "].importVisibility");
				model.importCameras = EditorPrefs.GetBool("DIO.model[" + i + "].importCameras");
				model.importLights = EditorPrefs.GetBool("DIO.model[" + i + "].importLights");
				model.swapUVs = EditorPrefs.GetBool("DIO.model[" + i + "].swapUVs");
				model.generateLightmapUVs = EditorPrefs.GetBool("DIO.model[" + i + "].generateLightmapUVs");

				model.normals = EditorPrefs.GetInt("DIO.model[" + i + "].normals");
				model.normalsMode = EditorPrefs.GetInt("DIO.model[" + i + "].normalsMode");
				model.smoothingAngle = EditorPrefs.GetInt("DIO.model[" + i + "].smoothingAngle");
				model.tangents = EditorPrefs.GetInt("DIO.model[" + i + "].tangents");

				model.importMaterials = EditorPrefs.GetBool("DIO.model[" + i + "].importMaterials");
				model.materialNaming = EditorPrefs.GetInt("DIO.model[" + i + "].materialNaming");
				model.materialSearch = EditorPrefs.GetInt("DIO.model[" + i + "].materialSearch");

				model.animationType = EditorPrefs.GetInt("DIO.model[" + i + "].animationType");
				model.importAnimation = EditorPrefs.GetBool("DIO.model[" + i + "].importAnimation");

				model.resetPosition = EditorPrefs.GetBool("DIO.model[" + i + "].resetPosition");
				model.resetRotation = EditorPrefs.GetBool("DIO.model[" + i + "].resetRotation");

				DIOData.model[i] = model;
			}

			return model;
		}

        public static void LoadData() {
			if (EditorPrefs.HasKey("DIO.Initialized")) {
				tex = LoadTextureOptions();
				model = LoadModelOptions();
			} else
				Initialize();
		}
		#endregion

		#region Save

		public static void SaveData() {
            EditorPrefs.SetBool("DIO.Initialized", true);

            EditorPrefs.SetInt("DIO.tex.Length", tex.Length);

            for (int i = 0; i < tex.Length; i++) {
                DIOTextureAsset tex = DIOData.tex[i];
                
                EditorPrefs.SetString("DIO.tex[" + i + "].name", tex.name);
                EditorPrefs.SetString("DIO.tex[" + i + "].prefix", tex.prefix);
                
                EditorPrefs.SetInt("DIO.tex[" + i + "].textureType", tex.textureType);

                EditorPrefs.SetBool("DIO.tex[" + i + "].createFromGrayscale", tex.createFromGrayscale);
                EditorPrefs.SetFloat("DIO.tex[" + i + "].bumpiness", tex.bumpiness);
                EditorPrefs.SetInt("DIO.tex[" + i + "].normalMapFilter", tex.normalMapFilter);
                EditorPrefs.SetInt("DIO.tex[" + i + "].spriteMode", tex.spriteMode);
                EditorPrefs.SetFloat("DIO.tex[" + i + "].pixelsPerUnit", tex.pixelsPerUnit);
                EditorPrefs.SetInt("DIO.tex[" + i + "].meshType", tex.meshType);
                EditorPrefs.SetInt("DIO.tex[" + i + "].extrudeEdges", tex.extrudeEdges);
                EditorPrefs.SetInt("DIO.tex[" + i + "].spriteAlignment", tex.spriteAlignment);
                EditorPrefs.SetFloat("DIO.tex[" + i + "].pivotX", tex.pivotX);
                EditorPrefs.SetFloat("DIO.tex[" + i + "].pivotY", tex.pivotY);

                EditorPrefs.SetBool("DIO.tex[" + i + "].sRGB", tex.sRGB);
                EditorPrefs.SetInt("DIO.tex[" + i + "].alphaSource", tex.alphaSource);
                EditorPrefs.SetBool("DIO.tex[" + i + "].alphaIsTransparency", tex.alphaIsTransparency);
                EditorPrefs.SetInt("DIO.tex[" + i + "].nonPowerOf2", tex.nonPowerOf2);
                EditorPrefs.SetBool("DIO.tex[" + i + "].readWriteEnabled", tex.readWriteEnabled);

                EditorPrefs.SetBool("DIO.tex[" + i + "].generateMipMaps", tex.generateMipMaps);
                EditorPrefs.SetBool("DIO.tex[" + i + "].borderMipMaps", tex.borderMipMaps);
                EditorPrefs.SetInt("DIO.tex[" + i + "].mipMapFiltering", tex.mipMapFiltering);
                EditorPrefs.SetBool("DIO.tex[" + i + "].mipMapPreserveCoverage", tex.mipMapPreserveCoverage);
				EditorPrefs.SetFloat("DIO.tex[" + i + "].alphaCutoff", tex.alphaCutoff);
				EditorPrefs.SetBool("DIO.tex[" + i + "].fadeoutMipMaps", tex.fadeoutMipMaps);
                EditorPrefs.SetInt("DIO.tex[" + i + "].mipMapFadeStart", tex.mipMapFadeStart);
                EditorPrefs.SetInt("DIO.tex[" + i + "].mipMapFadeEnd", tex.mipMapFadeEnd);

                EditorPrefs.SetInt("DIO.tex[" + i + "].wrapMode", tex.wrapMode);
                EditorPrefs.SetInt("DIO.tex[" + i + "].wrapModeU", tex.wrapModeU);
                EditorPrefs.SetInt("DIO.tex[" + i + "].wrapModeV", tex.wrapModeV);
                EditorPrefs.SetInt("DIO.tex[" + i + "].wrapModeW", tex.wrapModeW);
                EditorPrefs.SetInt("DIO.tex[" + i + "].filterMode", tex.filterMode);
                EditorPrefs.SetInt("DIO.tex[" + i + "].anisoLevel", tex.anisoLevel);
                
                //Platform spec haha
            }

			EditorPrefs.SetInt("DIO.model.Length", model.Length);

			for (int i = 0; i < model.Length; i++) {
				DIOModelAsset model = DIOData.model[i];

				EditorPrefs.SetString("DIO.model[" + i + "].name", model.name);
				EditorPrefs.SetString("DIO.model[" + i + "].prefix", model.prefix);

				EditorPrefs.SetFloat("DIO.model[" + i + "].scaleFactor", model.scaleFactor);
				EditorPrefs.SetBool("DIO.model[" + i + "].useFileScale", model.useFileScale);
				EditorPrefs.SetInt("DIO.model[" + i + "].meshCompression", model.meshCompression);
				EditorPrefs.SetBool("DIO.model[" + i + "].readWriteEnabled", model.readWriteEnabled);
				EditorPrefs.SetBool("DIO.model[" + i + "].optimizeMesh", model.optimizeMesh);
				EditorPrefs.SetBool("DIO.model[" + i + "].importBlendShapes", model.importBlendShapes);
				EditorPrefs.SetBool("DIO.model[" + i + "].generateColliders", model.generateColliders);
				EditorPrefs.SetBool("DIO.model[" + i + "].keepQuads", model.keepQuads);
				EditorPrefs.SetBool("DIO.model[" + i + "].weldVertices", model.weldVertices);
				EditorPrefs.SetBool("DIO.model[" + i + "].importVisibility", model.importVisibility);
				EditorPrefs.SetBool("DIO.model[" + i + "].importCameras", model.importCameras);
				EditorPrefs.SetBool("DIO.model[" + i + "].importLights", model.importLights);
				EditorPrefs.SetBool("DIO.model[" + i + "].swapUVs", model.swapUVs);
				EditorPrefs.SetBool("DIO.model[" + i + "].generateLightmapUVs", model.generateLightmapUVs);

				EditorPrefs.SetInt("DIO.model[" + i + "].normals", model.normals);
				EditorPrefs.SetInt("DIO.model[" + i + "].normalsMode", model.normalsMode);
				EditorPrefs.SetInt("DIO.model[" + i + "].smoothingAngle", model.smoothingAngle);
				EditorPrefs.SetInt("DIO.model[" + i + "].tangents", model.tangents);

				EditorPrefs.SetBool("DIO.model[" + i + "].importMaterials", model.importMaterials);
				EditorPrefs.SetInt("DIO.model[" + i + "].materialNaming", model.materialNaming);
				EditorPrefs.SetInt("DIO.model[" + i + "].materialSearch", model.materialSearch);

				EditorPrefs.SetInt("DIO.model[" + i + "].animationType", model.animationType);
				EditorPrefs.SetBool("DIO.model[" + i + "].importAnimation", model.importAnimation);

				EditorPrefs.SetBool("DIO.model[" + i + "].resetPosition", model.resetPosition);
				EditorPrefs.SetBool("DIO.model[" + i + "].resetRotation", model.resetRotation);

			}

		}

		#endregion
	}

}
