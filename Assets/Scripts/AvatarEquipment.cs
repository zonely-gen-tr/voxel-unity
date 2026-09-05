using System.Threading.Tasks;
using UnityEngine;

namespace ZonelyVoxelEngine
{
    public class AvatarEquipment : MonoBehaviour
    {
        public Transform avatarRoot;
        public Transform handSocket;
        public Renderer fallbackRenderer;

        GameObject currentAvatar;
        GameObject currentWeapon;

        public string CurrentAvatarId { get; private set; } = "char_knight";
        public string CurrentWeaponId { get; private set; } = "";

        public async void LoadDefault()
        {
            await EquipAvatar("char_knight");
        }

        public async Task EquipAvatar(string id)
        {
            var def = RemoteAssetCatalog.Find(id);
            if (def == null) return;

            if (currentAvatar != null)
                Destroy(currentAvatar);

            if (fallbackRenderer != null)
                fallbackRenderer.enabled = true;

            var obj = await RemoteGltfUtility.Load(
                def.url,
                avatarRoot,
                1.85f,
                def.rotationOffset,
                true
            );

            if (obj != null)
            {
                currentAvatar = obj;
                CurrentAvatarId = id;
                if (fallbackRenderer != null)
                    fallbackRenderer.enabled = false;
            }
        }

        public async Task EquipWeapon(string id)
        {
            var def = RemoteAssetCatalog.Find(id);
            if (def == null || def.category != AssetCategory.Weapon)
                return;

            if (currentWeapon != null)
                Destroy(currentWeapon);

            var host = new GameObject("HeldRemoteAsset");
            host.transform.SetParent(handSocket, false);
            currentWeapon = host;

            var obj = await RemoteGltfUtility.Load(
                def.url,
                host.transform,
                def.previewHeight,
                Vector3.zero,
                true
            );

            if (obj == null)
                return;

            host.transform.localPosition = new Vector3(0.55f, 1.05f, 0.15f);
            host.transform.localEulerAngles = new Vector3(20f, 0f, 55f);
            host.transform.localScale = Vector3.one * 0.7f;
            CurrentWeaponId = id;
        }
    }
}
