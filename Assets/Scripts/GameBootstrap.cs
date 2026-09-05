using System.Collections.Generic;
using UnityEngine;

namespace ZonelyVoxelEngine
{
    public class GameBootstrap : MonoBehaviour
    {
        const int PREVIEW_LAYER = 30;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoStart()
        {
            if (FindFirstObjectByType<GameBootstrap>() != null)
                return;

            var go = new GameObject("ZonelyVoxelEngineBootstrap");
            go.AddComponent<GameBootstrap>();
        }

        void Start()
        {
            Application.targetFrameRate = 120;
            BuildLighting();
            BuildWorld();
            var player = BuildPlayer();
            var cam = BuildCamera(player.transform);
            BuildPreviewAndUI(player, cam);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void BuildLighting()
        {
            RenderSettings.ambientLight = new Color(0.55f, 0.62f, 0.68f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.55f, 0.78f, 0.95f);
            RenderSettings.fogDensity = 0.008f;

            var sun = new GameObject("Sun").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.35f;
            sun.transform.rotation = Quaternion.Euler(48, -28, 0);
            sun.shadows = LightShadows.Soft;
        }

        Material Mat(Color c)
        {
            var shader = Shader.Find("Standard");
            var m = new Material(shader);
            m.color = c;
            return m;
        }

        GameObject Cube(string name, Vector3 pos, Vector3 scale, Color color, Transform parent = null)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = name;
            g.transform.position = pos;
            g.transform.localScale = scale;
            if (parent) g.transform.SetParent(parent, true);
            g.GetComponent<Renderer>().sharedMaterial = Mat(color);
            return g;
        }

        void BuildWorld()
        {
            var world = new GameObject("SpawnIsland").transform;

            for (int x = -12; x <= 12; x++)
            for (int z = -12; z <= 12; z++)
            {
                float d = Mathf.Sqrt(x * x + z * z);
                if (d > 12.2f) continue;

                float y = d > 10.5f ? -0.5f : 0f;
                Cube("Grass", new Vector3(x, y, z), Vector3.one,
                    new Color(0.34f, 0.62f, 0.28f), world);

                if (d > 8.5f)
                    Cube("Dirt", new Vector3(x, y - 1f, z), Vector3.one,
                        new Color(0.42f, 0.28f, 0.18f), world);
            }

            var platform = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            platform.name = "SpawnPlatform";
            platform.transform.position = new Vector3(0, .62f, 0);
            platform.transform.localScale = new Vector3(4.2f, .12f, 4.2f);
            platform.GetComponent<Renderer>().sharedMaterial = Mat(new Color(.16f, .2f, .24f));

            // Remote world gallery
            string[] gallery =
            {
                "decor_chest", "decor_gold_chest", "decor_barrel", "decor_crates",
                "block_foundation", "block_column", "block_barrier", "block_dirt_rocky"
            };

            for (int i = 0; i < gallery.Length; i++)
            {
                float a = i / (float)gallery.Length * Mathf.PI * 2f;
                Vector3 p = new Vector3(Mathf.Cos(a) * 8.5f, .72f, Mathf.Sin(a) * 8.5f);
                Cube("Pedestal", p, new Vector3(1.8f, .35f, 1.8f),
                    new Color(.22f, .26f, .3f), world);

                var host = new GameObject("RemoteGallery_" + gallery[i]);
                host.transform.position = p + Vector3.up * .2f;
                var loader = host.AddComponent<GalleryRemoteLoader>();
                loader.assetId = gallery[i];
            }
        }

        GameObject BuildPlayer()
        {
            var p = new GameObject("Player");
            p.transform.position = new Vector3(0, 1.15f, 0);

            var cc = p.AddComponent<CharacterController>();
            cc.height = 1.85f;
            cc.radius = .38f;
            cc.center = new Vector3(0, .93f, 0);
            cc.stepOffset = .35f;

            var visualRoot = new GameObject("VisualRoot").transform;
            visualRoot.SetParent(p.transform, false);

            var fallback = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            fallback.name = "FallbackCharacter";
            fallback.transform.SetParent(visualRoot, false);
            fallback.transform.localPosition = new Vector3(0, .93f, 0);
            fallback.transform.localScale = new Vector3(.55f, .9f, .55f);
            Destroy(fallback.GetComponent<Collider>());
            fallback.GetComponent<Renderer>().sharedMaterial = Mat(new Color(.17f, .3f, .42f));

            var avatarRoot = new GameObject("AvatarRoot").transform;
            avatarRoot.SetParent(visualRoot, false);

            var handSocket = new GameObject("HandSocket").transform;
            handSocket.SetParent(visualRoot, false);

            var equipment = p.AddComponent<AvatarEquipment>();
            equipment.avatarRoot = avatarRoot;
            equipment.handSocket = handSocket;
            equipment.fallbackRenderer = fallback.GetComponent<Renderer>();

            var ctrl = p.AddComponent<ThirdPersonController>();
            ctrl.visualRoot = visualRoot;

            equipment.LoadDefault();
            return p;
        }

        Camera BuildCamera(Transform player)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(.52f, .76f, .94f);
            cam.fieldOfView = 68f;

            var follow = go.AddComponent<ThirdPersonCamera>();
            follow.target = player;

            var ctrl = player.GetComponent<ThirdPersonController>();
            ctrl.cameraTransform = go.transform;
            return cam;
        }

        void BuildPreviewAndUI(GameObject player, Camera mainCam)
        {
            var previewRoot = new GameObject("PreviewRoot").transform;
            previewRoot.position = new Vector3(0, -1000, 0);
            previewRoot.gameObject.layer = PREVIEW_LAYER;

            var light = new GameObject("PreviewLight").AddComponent<Light>();
            light.transform.SetParent(previewRoot, false);
            light.type = LightType.Directional;
            light.intensity = 1.5f;
            light.transform.rotation = Quaternion.Euler(40, -25, 0);
            light.gameObject.layer = PREVIEW_LAYER;

            var camGo = new GameObject("PreviewCamera");
            camGo.transform.SetParent(previewRoot, false);
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(.04f, .06f, .08f);
            cam.cullingMask = 1 << PREVIEW_LAYER;
            cam.transform.localPosition = new Vector3(3.5f, 1.6f, 4.4f);
            cam.transform.LookAt(previewRoot.position + Vector3.up * .9f);

            var rt = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
            rt.name = "CatalogPreviewRT";
            cam.targetTexture = rt;

            var ui = gameObject.AddComponent<CatalogUI>();
            ui.controller = player.GetComponent<ThirdPersonController>();
            ui.cameraController = mainCam.GetComponent<ThirdPersonCamera>();
            ui.equipment = player.GetComponent<AvatarEquipment>();
            ui.previewRoot = previewRoot;
            ui.previewCamera = cam;
            ui.previewTexture = rt;
        }
    }

    public class GalleryRemoteLoader : MonoBehaviour
    {
        public string assetId;

        async void Start()
        {
            var d = RemoteAssetCatalog.Find(assetId);
            if (d == null) return;

            var obj = await RemoteGltfUtility.Load(d.url, transform, d.previewHeight, d.rotationOffset, true);
            if (obj != null)
            {
                transform.localScale = Vector3.one * .85f;
            }
        }
    }
}
