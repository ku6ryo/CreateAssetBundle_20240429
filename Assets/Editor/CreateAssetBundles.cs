using UnityEditor;
using UnityEngine;
using System;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build asset bundle for VISIONIAR", validate = true)]
    static bool ValidateDoSomethingWithSelectedObjects()
    {
        // This method determines whether the menu item should be enabled or not
        return Selection.objects != null && Selection.objects.Length > 0;
    }

    [MenuItem("Assets/Build asset bundle for VISIONIAR")]
    static void BuildAssetBundles()
    {
        var selectedObj = Selection.activeObject;
        if (selectedObj == null) {
            Debug.LogError("No object selected");
        }
        // Check if it is a prefab
        var isPrefab = PrefabUtility.GetPrefabAssetType(selectedObj) == PrefabAssetType.Regular;
        if (isPrefab) {
            // Get current build target
            var initialBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            string path = AssetDatabase.GetAssetPath(selectedObj);
            Debug.Log($"Building an asset bundle for {path}");
            var uuid = Guid.NewGuid().ToString();
            var goNameOfPrefab = selectedObj.name;
            selectedObj.name = uuid;
            var filenameAndroid = $"{uuid}.android";
            var filenameIOS = $"{uuid}.ios";
            var filenameWin = $"{uuid}.windows";
            var filenameMac = $"{uuid}.mac";
            try {
                BuildAssetBundleForTarget(path, filenameAndroid, BuildTarget.Android);
                BuildAssetBundleForTarget(path, filenameIOS, BuildTarget.iOS);
                BuildAssetBundleForTarget(path, filenameWin, BuildTarget.StandaloneWindows);
                BuildAssetBundleForTarget(path, filenameMac, BuildTarget.StandaloneOSX);
            } finally {
                selectedObj.name = goNameOfPrefab;
            }
            EditorUserBuildSettings.SwitchActiveBuildTarget(
                BuildPipeline.GetBuildTargetGroup(initialBuildTarget),
                initialBuildTarget
            );
            Debug.Log($"Asset bundle built for {path}. Asset ID: {uuid}");
        } else {
            Debug.Log("Selected object is not a regular prefab");
        }
    }

    static void BuildAssetBundleForTarget(
        string assetPath,
        string filename,
        BuildTarget target
    )
    {
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
        buildMap[0].assetBundleName = filename;
        buildMap[0].assetNames = new string[] { assetPath };
        var outputDir = $"AssetBundles";
        // Check if the output directory exists
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }
        var targetGroup = BuildPipeline.GetBuildTargetGroup(target);
        EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
        BuildPipeline.BuildAssetBundles(outputDir, buildMap, BuildAssetBundleOptions.None, target);
    }
}