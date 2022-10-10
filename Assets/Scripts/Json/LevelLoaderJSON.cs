using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using Object = UnityEngine.Object;

public class LevelLoaderJSON : MonoBehaviour
{
    public TextAsset jsonLvl;
    public List<Block> blockList;
    public Dictionary<int, Block> blocksIndex;
    public Dictionary<Vector2, Block> blocksPos;
    private List<BridgeBlock> bridges;
    private readonly string path = "Prefab/";

    public void LoadLevel(TextAsset jsonLvl)
    {
        var level = JSON.Parse(jsonLvl.ToString());
        var tiles = level["Map"]["tiles"].AsArray;
        var index = level["LevelId"];
        LevelLoader.Instance.LevelIndex = Convert.ToInt32(index);
        
        bridges = new List<BridgeBlock>();
        foreach (var value in tiles.Children) LoadTile(value.ToString());
        for (var i = 0; i < bridges.Count; i++) bridges[i].SetUp();
    }

    [ContextMenu("Load lvl1")]
    public List<Block> LoadThisLvL()
    {
        // TextAsset jsonLvl = (TextAsset)Resources.Load("lvl1.json");
        LoadLevel(jsonLvl);
        return blockList;
    }

    public void AddBlocks(int index, Vector2 pos, Block block)
    {
        if (blocksIndex == null) blocksIndex = new Dictionary<int, Block>();

        if (blocksPos == null) blocksPos = new Dictionary<Vector2, Block>();

        blockList.Add(block);

        if (!blocksIndex.ContainsKey(index))
            blocksIndex.Add(index, block);
        else
            blocksIndex[index] = block;

        if (!blocksPos.ContainsKey(pos))
            blocksPos.Add(pos, block);
        else
            blocksPos[pos] = block;
    }

    public Block LoadTile(string jsonTile)
    {
        var tile = JSON.Parse(jsonTile);
        var type = BlockType.Empty;
        Object go = null;
        Debug.Log(jsonTile);
        switch (tile["Type"])
        {
            case "Normal":
                type = BlockType.Normal;
                go = Resources.Load(path + "BasicTileGround");
                break;
            case "Goal":
                type = BlockType.Normal;
                go = Resources.Load(path + "LightCone");
                break;
            case "Unstable":
                type = BlockType.Unstable;
                go = Resources.Load(path + "UnstableTile");
                break;
            case "Teleporter":
                type = BlockType.Teleporter;
                go = Resources.Load(path + "TeleporterTile");
                break;
            case "Bridge":
                type = BlockType.Bridge;
                go = Resources.Load(path + "BridgeTile");
                break;
            case "SoftButton":
                type = BlockType.SoftButton;
                go = Resources.Load(path + "SoftButtonTile");
                break;
            case "HardButton":
                type = BlockType.HardButton;
                go = Resources.Load(path + "HardButtonTile");
                break;
        }

        var pos = new Vector3(tile["PosX"].AsInt, tile["PosY"].AsInt, tile["PosZ"].AsInt);
        Vector2 posV2 = new Vector3(tile["PosX"].AsInt, tile["PosZ"].AsInt);
        go = Instantiate(go, pos, Quaternion.identity);
        var block = ((GameObject) go).GetComponent<Block>();
        AddBlocks(tile["TileIndex"].AsInt, posV2, block);
        block.SetIndex(tile["TileIndex"].AsInt);
        block.gameObject.transform.localScale = Vector3.zero;

        switch (type)
        {
            case BlockType.Teleporter:
                var tpBlock = (TeleporterBlock) block;
                tpBlock.linkedTeleporterId = tile["linkedTeleporterId"].AsInt;
                break;
            case BlockType.Bridge:
                var bridgeBlock = (BridgeBlock) block;
                bridgeBlock.isFirstTileOfBridge = tile["IsFirstTileOfBridge"].AsBool;
                bridgeBlock.bridgeAnchorTileId = tile["BridgeAnchorTileId"].AsInt;
                bridgeBlock.previousTileIndex = tile["PreviousTileIndex"].AsInt;
                bridgeBlock.nextTileIndex = tile["NextTileIndex"].AsInt;
                if (bridgeBlock.isFirstTileOfBridge) bridges.Add(bridgeBlock);
                break;
            case BlockType.SoftButton:
            case BlockType.HardButton:
                var ppBlock = (PresurePlateBlock) block;
                var linkedTiles = tile["BridgeTileIndex"].AsArray;
                foreach (var value in linkedTiles.Children) ppBlock.BridgeTileIndex.Add(value.AsInt);
                break;
            case BlockType.Normal:
            case BlockType.Unstable:

                break;
        }

        return block;
    }
}