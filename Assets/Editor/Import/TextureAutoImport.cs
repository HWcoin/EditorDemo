using UnityEngine;
using UnityEditor;

public class TextureAutoImport : AssetPostprocessor {

    void OnPreprocessTexture() {
        
        //导入资源自动设置参数
        TextureImporter targetImporter = (TextureImporter) assetImporter;

        targetImporter.textureType = TextureImporterType.Sprite;
        targetImporter.spriteImportMode = SpriteImportMode.Single;
        
        targetImporter.textureType = TextureImporterType.Advanced;
        targetImporter.alphaIsTransparency = true;
        targetImporter.wrapMode = TextureWrapMode.Clamp;
        targetImporter.textureFormat = TextureImporterFormat.ARGB16;
        targetImporter.npotScale = TextureImporterNPOTScale.None;
        targetImporter.isReadable = true;
        targetImporter.wrapMode = TextureWrapMode.Clamp;
        targetImporter.mipmapEnabled = false;
        targetImporter.maxTextureSize = 1024;
    }
	
}
