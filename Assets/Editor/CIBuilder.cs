#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ZonelyVoxelEngine.Editor
{
    public static class CIBuilder
    {
        const string ScenePath = "Assets/Scenes/Main.unity";
        const string OutputPath = "build/WebGL";

        public static void Build()
        {
            Directory.CreateDirectory("Assets/Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, ScenePath);

            PlayerSettings.productName = "Zonely Voxel Game Engine";
            PlayerSettings.companyName = "ZonelyVoxelEngine";
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.decompressionFallback = false;

            Directory.CreateDirectory(OutputPath);

            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = OutputPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception($"WebGL build failed: {report.summary.result}; errors={report.summary.totalErrors}");

            Debug.Log($"WebGL build succeeded: {OutputPath}");
        }
    }
}
#endif
