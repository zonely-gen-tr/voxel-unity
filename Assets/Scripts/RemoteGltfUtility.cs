using System;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;

namespace ZonelyVoxelEngine
{
    public sealed class RemoteGltfHandle : MonoBehaviour
    {
        public GltfImport Importer;

        void OnDestroy()
        {
            Importer?.Dispose();
            Importer = null;
        }
    }

    public static class RemoteGltfUtility
    {
        public static async Task<GameObject> Load(
            string url,
            Transform parent,
            float targetHeight,
            Vector3 rotationOffset,
            bool normalize = true)
        {
            var root = new GameObject("RemoteGLTF");
            root.transform.SetParent(parent, false);

            var importer = new GltfImport();
            var handle = root.AddComponent<RemoteGltfHandle>();
            handle.Importer = importer;

            bool ok;
            try
            {
                ok = await importer.Load(url);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Remote GLTF load exception: {url}\n{ex}");
                UnityEngine.Object.Destroy(root);
                return null;
            }

            if (!ok)
            {
                Debug.LogError("Remote GLTF load failed: " + url);
                UnityEngine.Object.Destroy(root);
                return null;
            }

            ok = await importer.InstantiateMainSceneAsync(root.transform);
            if (!ok)
            {
                Debug.LogError("Remote GLTF instantiate failed: " + url);
                UnityEngine.Object.Destroy(root);
                return null;
            }

            root.transform.localEulerAngles = rotationOffset;

            if (normalize)
                Normalize(root, targetHeight);

            SetLayerRecursive(root, parent.gameObject.layer);
            return root;
        }

        public static void Normalize(GameObject root, float targetHeight)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
                return;

            Bounds b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                b.Encapsulate(renderers[i].bounds);

            float h = Mathf.Max(0.0001f, b.size.y);
            float scale = targetHeight / h;
            root.transform.localScale *= scale;

            // Recalculate after scale
            renderers = root.GetComponentsInChildren<Renderer>(true);
            b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                b.Encapsulate(renderers[i].bounds);

            Vector3 parentWorld = root.transform.parent ? root.transform.parent.position : Vector3.zero;
            Vector3 correction = new Vector3(
                parentWorld.x - b.center.x,
                parentWorld.y - b.min.y,
                parentWorld.z - b.center.z
            );
            root.transform.position += correction;
        }

        static void SetLayerRecursive(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
                SetLayerRecursive(child.gameObject, layer);
        }
    }
}
