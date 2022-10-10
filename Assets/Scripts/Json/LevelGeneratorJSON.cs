using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorJSON : MonoBehaviour
{
    public string jsonLvl = "";

    public static string TileToJson(Block block)
    {
        var jsonString = "{\"TileIndex\":" + block.GetIndex();

        jsonString += ",\"Type\":" + "\"" + block.type + "\"";
        jsonString += ",\"PosX\":" + block.transform.position.x;
        jsonString += ",\"PosZ\":" + block.transform.position.z;
        jsonString += ",\"PosY\":" + block.transform.position.y;
        switch (block.type)
        {
            case BlockType.Teleporter:
                jsonString += ",\"linkedTeleporterId\":" + ((TeleporterBlock) block).linkedTeleporterId;
                break;

            case BlockType.Bridge:
                jsonString += ",\"IsFirstTileOfBridge\":" +
                              ((BridgeBlock) block).isFirstTileOfBridge.ToString().ToLower();
                jsonString += ",\"BridgeAnchorTileId\":" + ((BridgeBlock) block).bridgeAnchorTileId;
                jsonString += ",\"PreviousTileIndex\":" + ((BridgeBlock) block).previousTileIndex;
                jsonString += ",\"NextTileIndex\":" + ((BridgeBlock) block).nextTileIndex;
                break;

            case BlockType.HardButton:
            case BlockType.SoftButton:
                var tiles = ((PresurePlateBlock) block).BridgeTileIndex;
                jsonString += ",\"BridgeTileIndex\":[";
                for (var i = 0; i < tiles.Count; i++)
                {
                    jsonString += tiles[i];
                    if (i != tiles.Count - 1) jsonString += ",";
                }

                jsonString += "]";
                break;
        }

        jsonString += "}";
        return jsonString;
    }

    public static string LevelToJson(Level level, List<Block> tiles)
    {
        var jsonString = "{\"LevelId\":" + level.levelIndex;
        jsonString += "{\"levelHeight\":" + level.levelIndex;
        jsonString += "{\"levelWidth\":" + level.levelIndex;
        jsonString += "{\"levelDepth\":" + level.levelIndex;
        jsonString += "\"Map\": { \"tile\": [";
        for (var i = 0; i < tiles.Count; i++)
        {
            jsonString += TileToJson(tiles[i]);
            if (i != tiles.Count - 1) jsonString += ",";
        }

        jsonString += "]}}";

        return jsonString;
    }


    public static string LevelToJson(List<Block> tiles, Vector2 playerPos, int levelId)
    {
        var jsonString = "{\"LevelId\":" + levelId;
        jsonString += ",\"levelHeight\":" + -1;
        jsonString += ",\"levelWidth\":" + -1;
        jsonString += ",\"levelDepth\":" + -1;

        jsonString += ",\"PlayerPosX\":" + playerPos.x;
        jsonString += ",\"PlayerPosZ\":" + playerPos.y;

        jsonString += ",\"Map\": { \"tiles\": [";
        for (var i = 0; i < tiles.Count; i++)
        {
            jsonString += TileToJson(tiles[i]);
            if (i != tiles.Count - 1) jsonString += ",";
        }

        jsonString += "]}}";

        return jsonString;
    }
}