using System;
using UnityEngine;

namespace ZonelyVoxelEngine
{
    public enum AssetCategory
    {
        Character,
        Armor,
        Weapon,
        Block,
        Decor
    }

    [Serializable]
    public class RemoteAssetDefinition
    {
        public string id;
        public string displayName;
        public AssetCategory category;
        public string url;
        public float previewHeight = 2f;
        public Vector3 rotationOffset = Vector3.zero;
        public Vector3 heldPosition = new Vector3(0.45f, 1.15f, 0.15f);
        public Vector3 heldRotation = new Vector3(0f, 0f, 35f);
        public float heldScale = 1f;

        public RemoteAssetDefinition(
            string id,
            string displayName,
            AssetCategory category,
            string url,
            float previewHeight = 2f)
        {
            this.id = id;
            this.displayName = displayName;
            this.category = category;
            this.url = url;
            this.previewHeight = previewHeight;
        }
    }
}
