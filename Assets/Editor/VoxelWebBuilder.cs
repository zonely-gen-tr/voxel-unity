#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ZonelyVoxelEngine.Editor
{
    public static class VoxelWebBuilder
    {
        const string ScenePath = "Assets/Scenes/Main.unity";

        [MenuItem("Zonely Voxel Game Engine/Build Web")]
        public static void BuildWeb()
        {
            EnsureScene();
            Directory.CreateDirectory("Build/Web");

            PlayerSettings.productName = "Zonely Voxel Game Engine";
            PlayerSettings.companyName = "ZonelyVoxelEngine";
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;

            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = "Build/Web",
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("Unity Web build failed: " + report.summary.result);

            Debug.Log("Web build hazır: Build/Web");
        }

        [MenuItem("Zonely Voxel Game Engine/Build Web + Single HTML")]
        public static void BuildWebAndPack()
        {
            BuildWeb();

            string project = Directory.GetParent(Application.dataPath)!.FullName;
            string tool = Path.Combine(project, "tools", "pack-unity-single-html.mjs");
            string input = Path.Combine(project, "Build", "Web");
            string output = Path.Combine(project, "Build", "Single", "zonely-voxel-engine.html");

            Directory.CreateDirectory(Path.GetDirectoryName(output)!);

            var psi = new ProcessStartInfo
            {
                FileName = "node",
                Arguments = $"\"{tool}\" \"{input}\" \"{output}\"",
                WorkingDirectory = project,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                using var p = Process.Start(psi);
                string stdout = p!.StandardOutput.ReadToEnd();
                string stderr = p.StandardError.ReadToEnd();
                p.WaitForExit();

                Debug.Log(stdout);
                if (p.ExitCode != 0)
                    Debug.LogError(stderr);
                else
                    Debug.Log("Tek HTML hazır: Build/Single/zonely-voxel-engine.html");
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    "Web build oluştu ama Node packer çalıştırılamadı.\n" +
                    "Komut: node tools/pack-unity-single-html.mjs Build/Web Build/Single/zonely-voxel-engine.html\n" +
                    ex
                );
            }
        }

        static void EnsureScene()
        {
            Directory.CreateDirectory("Assets/Scenes");
            if (File.Exists(ScenePath))
                return;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
        }
    }
}
#endif
