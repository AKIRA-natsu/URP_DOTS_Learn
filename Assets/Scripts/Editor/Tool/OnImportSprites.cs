using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OnImportSprites : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        // Debug.Log(assetPath.ToLower());
        if (!assetPath.ToLower().Contains("sprite"))
            return;
        var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (textureImporter != null && textureImporter.textureType != TextureImporterType.Sprite)
        {
            textureImporter.textureType = TextureImporterType.Sprite;
            var setting = new TextureImporterPlatformSettings()
            {
                overridden = true,
                name = "iPhone",
                maxTextureSize = 1024,
                format = TextureImporterFormat.ASTC_4x4
            };
            textureImporter.SetPlatformTextureSettings(setting);
            setting.name = "Android";
            setting.format = TextureImporterFormat.ETC2_RGBA8;
            textureImporter.SetPlatformTextureSettings(setting);
            
        }
    }
}