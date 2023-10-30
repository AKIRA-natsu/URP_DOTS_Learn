using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OnImportAnimations : AssetPostprocessor
{
    private void OnPreprocessModel()
    {
        // Debug.Log(assetPath.ToLower());
        if (!assetPath.ToLower().Contains("animation"))
            return;
        var fbxImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        if (fbxImporter != null && fbxImporter.animationType == ModelImporterAnimationType.Generic)
        {
            fbxImporter.animationType = ModelImporterAnimationType.Human;
        }
    }
}