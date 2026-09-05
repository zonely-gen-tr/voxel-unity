using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZonelyVoxelEngine
{
    public class CatalogUI : MonoBehaviour
    {
        public ThirdPersonController controller;
        public ThirdPersonCamera cameraController;
        public AvatarEquipment equipment;

        public Transform previewRoot;
        public Camera previewCamera;
        public RenderTexture previewTexture;

        bool open;
        string search = "";
        AssetCategory? activeCategory = null;
        Vector2 listScroll;
        GameObject previewObject;
        RemoteAssetDefinition selected;
        bool previewLoading;

        GUIStyle header, button, selectedButton, small, categoryButton;

        void Start()
        {
            selected = RemoteAssetCatalog.All.FirstOrDefault();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
                SetOpen(!open);

            if (open && Input.GetKeyDown(KeyCode.Escape))
                SetOpen(false);

            if (previewObject != null)
                previewObject.transform.Rotate(0f, 35f * Time.unscaledDeltaTime, 0f, Space.World);
        }

        public void SetOpen(bool value)
        {
            open = value;
            controller.InputEnabled = !open;
            cameraController.InputEnabled = !open;

            if (open)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void EnsureStyles()
        {
            if (header != null) return;

            header = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
            small = new GUIStyle(GUI.skin.label) { fontSize = 11 };
            small.normal.textColor = new Color(0.72f, 0.78f, 0.84f);

            button = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                fixedHeight = 36
            };
            selectedButton = new GUIStyle(button);
            selectedButton.normal.textColor = new Color(0.45f, 0.9f, 1f);

            categoryButton = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 34,
                fontStyle = FontStyle.Bold
            };
        }

        void OnGUI()
        {
            EnsureStyles();

            if (!open)
            {
                GUI.Box(new Rect(16, 16, 330, 74), "Zonely Voxel Game Engine • Unity Web");
                GUI.Label(new Rect(30, 43, 300, 20), "WASD • Space • Shift • E = Catalog");
                return;
            }

            float w = Mathf.Min(Screen.width - 60, 1180);
            float h = Mathf.Min(Screen.height - 60, 720);
            Rect panel = new Rect((Screen.width - w) * .5f, (Screen.height - h) * .5f, w, h);
            GUI.Box(panel, "");

            GUILayout.BeginArea(new Rect(panel.x + 14, panel.y + 14, 190, panel.height - 28));
            GUILayout.Label("ASSET CATALOG", header);
            GUILayout.Label("Remote CDN Registry", small);
            GUILayout.Space(10);

            if (GUILayout.Button("Tümü", categoryButton)) activeCategory = null;
            foreach (AssetCategory cat in System.Enum.GetValues(typeof(AssetCategory)))
            {
                int count = RemoteAssetCatalog.All.Count(x => x.category == cat);
                if (GUILayout.Button($"{cat}  ({count})", categoryButton))
                    activeCategory = cat;
            }
            GUILayout.EndArea();

            float midX = panel.x + 214;
            float rightW = 350;
            float midW = panel.width - 214 - rightW - 14;

            GUILayout.BeginArea(new Rect(midX, panel.y + 14, midW, panel.height - 28));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Ara:", GUILayout.Width(34));
            search = GUILayout.TextField(search);
            GUILayout.EndHorizontal();
            GUILayout.Space(8);

            IEnumerable<RemoteAssetDefinition> q = RemoteAssetCatalog.All;
            if (activeCategory.HasValue)
                q = q.Where(x => x.category == activeCategory.Value);
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(x => x.displayName.ToLowerInvariant().Contains(search.ToLowerInvariant()) ||
                                 x.id.ToLowerInvariant().Contains(search.ToLowerInvariant()));

            var list = q.ToList();
            GUILayout.Label($"{list.Count} asset", small);

            listScroll = GUILayout.BeginScrollView(listScroll);
            foreach (var item in list)
            {
                GUIStyle style = selected == item ? selectedButton : button;
                if (GUILayout.Button($"{item.displayName}   [{item.category}]", style))
                {
                    selected = item;
                    _ = LoadPreview(item);
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(panel.xMax - rightW, panel.y + 14, rightW - 14, panel.height - 28));
            GUILayout.Label(selected != null ? selected.displayName : "Seç", header);
            GUILayout.Label(selected != null ? selected.category.ToString() : "-", small);

            Rect texRect = GUILayoutUtility.GetRect(320, 320, GUILayout.ExpandWidth(true));
            if (previewTexture != null)
                GUI.DrawTexture(texRect, previewTexture, ScaleMode.ScaleToFit, false);

            GUILayout.Space(10);

            if (selected != null)
            {
                GUILayout.Label("ID: " + selected.id, small);
                GUILayout.Label("CDN: jsDelivr / GitHub", small);

                GUI.enabled = !previewLoading;
                if (GUILayout.Button(previewLoading ? "Yükleniyor..." : "3D Preview Yükle", GUILayout.Height(34)))
                    _ = LoadPreview(selected);

                if ((selected.category == AssetCategory.Character || selected.category == AssetCategory.Armor)
                    && GUILayout.Button("Karaktere Uygula", GUILayout.Height(38)))
                {
                    _ = equipment.EquipAvatar(selected.id);
                }

                if (selected.category == AssetCategory.Weapon
                    && GUILayout.Button("Eline Al", GUILayout.Height(38)))
                {
                    _ = equipment.EquipWeapon(selected.id);
                }
                GUI.enabled = true;
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("E veya ESC ile kapat", small);
            GUILayout.EndArea();
        }

        async System.Threading.Tasks.Task LoadPreview(RemoteAssetDefinition def)
        {
            if (previewLoading || previewRoot == null)
                return;

            previewLoading = true;
            if (previewObject != null)
                Destroy(previewObject);

            var host = new GameObject("PreviewHost");
            host.layer = previewRoot.gameObject.layer;
            host.transform.SetParent(previewRoot, false);
            previewObject = host;

            var obj = await RemoteGltfUtility.Load(
                def.url,
                host.transform,
                Mathf.Clamp(def.previewHeight, .6f, 2.4f),
                def.rotationOffset,
                true
            );

            if (obj == null && host != null)
                Destroy(host);

            previewLoading = false;
        }
    }
}
