using System.Collections.Generic;

namespace ZonelyVoxelEngine
{
    public static class RemoteAssetCatalog
    {
        const string ADV_CHAR =
            "https://cdn.jsdelivr.net/gh/KayKit-Game-Assets/KayKit-Character-Pack-Adventures-1.0@main/" +
            "addons/kaykit_character_pack_adventures/Characters/gltf/";

        const string ADV_ASSET =
            "https://cdn.jsdelivr.net/gh/KayKit-Game-Assets/KayKit-Character-Pack-Adventures-1.0@main/" +
            "addons/kaykit_character_pack_adventures/Assets/gltf/";

        const string DUNGEON =
            "https://cdn.jsdelivr.net/gh/KayKit-Game-Assets/KayKit-Dungeon-Remastered-1.0@main/" +
            "addons/kaykit_dungeon_remastered/Assets/gltf/";

        public static readonly List<RemoteAssetDefinition> All = new List<RemoteAssetDefinition>
        {
            // Character / armor appearances
            new RemoteAssetDefinition("char_knight", "Knight", AssetCategory.Character, ADV_CHAR + "Knight.glb", 1.9f),
            new RemoteAssetDefinition("char_barbarian", "Barbarian", AssetCategory.Character, ADV_CHAR + "Barbarian.glb", 1.9f),
            new RemoteAssetDefinition("char_mage", "Mage", AssetCategory.Character, ADV_CHAR + "Mage.glb", 1.9f),
            new RemoteAssetDefinition("char_rogue", "Rogue", AssetCategory.Character, ADV_CHAR + "Rogue.glb", 1.9f),
            new RemoteAssetDefinition("char_rogue_hooded", "Rogue Hooded", AssetCategory.Character, ADV_CHAR + "Rogue_Hooded.glb", 1.9f),

            // Same real characters exposed as armor/look presets for first prototype
            new RemoteAssetDefinition("armor_knight", "Knight Armor", AssetCategory.Armor, ADV_CHAR + "Knight.glb", 1.9f),
            new RemoteAssetDefinition("armor_barbarian", "Barbarian Armor", AssetCategory.Armor, ADV_CHAR + "Barbarian.glb", 1.9f),
            new RemoteAssetDefinition("armor_mage", "Mage Robe", AssetCategory.Armor, ADV_CHAR + "Mage.glb", 1.9f),
            new RemoteAssetDefinition("armor_rogue", "Rogue Armor", AssetCategory.Armor, ADV_CHAR + "Rogue.glb", 1.9f),
            new RemoteAssetDefinition("armor_hooded", "Hooded Rogue Armor", AssetCategory.Armor, ADV_CHAR + "Rogue_Hooded.glb", 1.9f),

            // Weapons and equipment
            Weapon("sword_1h", "1H Sword", ADV_ASSET + "sword_1handed.gltf", 1.25f),
            Weapon("sword_2h", "2H Sword", ADV_ASSET + "sword_2handed.gltf", 1.55f),
            Weapon("axe_1h", "1H Axe", ADV_ASSET + "axe_1handed.gltf", 1.25f),
            Weapon("axe_2h", "2H Axe", ADV_ASSET + "axe_2handed.gltf", 1.55f),
            Weapon("dagger", "Dagger", ADV_ASSET + "dagger.gltf", 0.9f),
            Weapon("staff", "Staff", ADV_ASSET + "staff.gltf", 1.65f),
            Weapon("wand", "Wand", ADV_ASSET + "wand.gltf", 0.9f),
            Weapon("crossbow_1h", "1H Crossbow", ADV_ASSET + "crossbow_1handed.gltf", 1.1f),
            Weapon("crossbow_2h", "2H Crossbow", ADV_ASSET + "crossbow_2handed.gltf", 1.35f),
            Weapon("shield_round", "Round Shield", ADV_ASSET + "shield_round.gltf", 1.0f),
            Weapon("shield_spikes", "Spiked Shield", ADV_ASSET + "shield_spikes.gltf", 1.0f),
            Weapon("spellbook", "Spellbook", ADV_ASSET + "spellbook_open.gltf", 0.8f),

            // Block / modular world pieces
            new RemoteAssetDefinition("block_dirt_large", "Dirt Floor Large", AssetCategory.Block, DUNGEON + "floor_dirt_large.gltf.glb", 1.1f),
            new RemoteAssetDefinition("block_dirt_rocky", "Dirt Floor Rocky", AssetCategory.Block, DUNGEON + "floor_dirt_large_rocky.gltf.glb", 1.1f),
            new RemoteAssetDefinition("block_foundation", "Foundation Block", AssetCategory.Block, DUNGEON + "floor_foundation_allsides.gltf.glb", 1.2f),
            new RemoteAssetDefinition("block_foundation_corner", "Foundation Corner", AssetCategory.Block, DUNGEON + "floor_foundation_corner.gltf.glb", 1.2f),
            new RemoteAssetDefinition("block_grate", "Grate Block", AssetCategory.Block, DUNGEON + "floor_tile_big_grate.gltf.glb", 1.2f),
            new RemoteAssetDefinition("block_spikes", "Spike Floor", AssetCategory.Block, DUNGEON + "floor_tile_big_spikes.glb", 1.2f),
            new RemoteAssetDefinition("block_barrier", "Barrier", AssetCategory.Block, DUNGEON + "barrier.gltf.glb", 1.4f),
            new RemoteAssetDefinition("block_column", "Column", AssetCategory.Block, DUNGEON + "column.gltf.glb", 2.0f),

            // Decor / loot
            new RemoteAssetDefinition("decor_chest", "Chest", AssetCategory.Decor, DUNGEON + "chest.glb", 1.25f),
            new RemoteAssetDefinition("decor_gold_chest", "Gold Chest", AssetCategory.Decor, DUNGEON + "chest_gold.glb", 1.25f),
            new RemoteAssetDefinition("decor_barrel", "Large Barrel", AssetCategory.Decor, DUNGEON + "barrel_large.gltf.glb", 1.35f),
            new RemoteAssetDefinition("decor_crates", "Stacked Crates", AssetCategory.Decor, DUNGEON + "crates_stacked.gltf.glb", 1.35f),
            new RemoteAssetDefinition("decor_chair", "Chair", AssetCategory.Decor, DUNGEON + "chair.gltf.glb", 1.4f),
            new RemoteAssetDefinition("decor_coin", "Coin", AssetCategory.Decor, DUNGEON + "coin.gltf.glb", 0.55f),
            new RemoteAssetDefinition("decor_coin_stack", "Coin Stack", AssetCategory.Decor, DUNGEON + "coin_stack_large.gltf.glb", 0.8f),
            new RemoteAssetDefinition("decor_candle", "Lit Candle", AssetCategory.Decor, DUNGEON + "candle_lit.gltf.glb", 0.8f),
            new RemoteAssetDefinition("decor_banner", "Red Banner", AssetCategory.Decor, DUNGEON + "banner_red.gltf.glb", 1.8f),
            new RemoteAssetDefinition("decor_bottle", "Potion Bottle", AssetCategory.Decor, DUNGEON + "bottle_A_green.gltf.glb", 0.7f)
        };

        static RemoteAssetDefinition Weapon(string id, string name, string url, float h)
        {
            var d = new RemoteAssetDefinition(id, name, AssetCategory.Weapon, url, h);
            d.heldScale = 0.7f;
            return d;
        }

        public static RemoteAssetDefinition Find(string id)
        {
            return All.Find(x => x.id == id);
        }
    }
}
