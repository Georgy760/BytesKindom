using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public Level level;

    public MultiBlock prefab;
    public List<MultiBlock> tiles;

    public int levelId;
    public Vector2 playerStartPos;
    public Vector2 mapSize;

    public string json;
    public List<List<MultiBlock>> map;

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.transform.gameObject.name);
                var multiBlock = hit.transform.gameObject.GetComponent<MultiBlock>();
                if (multiBlock != null) multiBlock.Next();
            }
        }
    }

    [ContextMenu("GetJsonString")]
    public void GetThisJson()
    {
        json = GetJsonString();
    }

    public string GetJsonString()
    {
        var tiles = new List<Block>();
        var jsonLvl = "";
        var index = 0;
        for (var z = 0; z < map.Count; z++)
            if (map[z] != null)
                for (var x = 0; x < map[z].Count; x++)
                    if (map[z][x] != null && map[z][x].currentType != BlockType.Empty)
                    {
                        map[z][x].currentBlock.SetIndex(index);
                        index++;
                        tiles.Add(map[z][x].currentBlock);
                    }

        jsonLvl = LevelGeneratorJSON.LevelToJson(tiles, playerStartPos, levelId);
        return jsonLvl;
    }

    [ContextMenu("SetTileIndex")]
    public void SetTileIndex()
    {
        var index = 0;
        for (var z = 0; z < map.Count; z++)
            if (map[z] != null)
                for (var x = 0; x < map[z].Count; x++)
                    if (map[z][x] != null && map[z][x].currentType != BlockType.Empty)
                    {
                        map[z][x].currentBlock.SetIndex(index);
                        index++;
                    }
    }

    [ContextMenu("UpdateMapWithNewSize")]
    public void UpdateMapWithNewSize()
    {
        if (map == null) map = new List<List<MultiBlock>>();

        for (var z = 0; z < Mathf.Max(mapSize.y, map.Count); z++)
        {
            if (map.Count <= z)
            {
                var newList = new List<MultiBlock>();
                map.Add(newList);
            }
            else if (map[z] == null)
            {
                if (mapSize.y - 1 >= z)
                {
                    var newList = new List<MultiBlock>();
                    map[z] = newList;
                }
                else
                {
                    continue;
                }
            }
            else if (z > mapSize.y - 1)
            {
                for (var i = 0; i < map[z].Count; i++)
                    if (map[z][i] != null)
                        Destroy(map[z][i].gameObject);
                map[z] = null;
                continue;
            }

            for (var x = 0; x < Mathf.Max(mapSize.x, map[z].Count); x++)
                if (map[z].Count <= x)
                {
                    var multiBlock = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.identity);
                    map[z].Add(multiBlock);
                }
                else if (map[z][x] == null)
                {
                    if (mapSize.x - 1 >= x)
                    {
                        var multiBlock = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.identity);
                        map[z][x] = multiBlock;
                    }
                    else
                    {
                    }
                }
                else if (x > mapSize.x - 1)
                {
                    if (map[z][x] != null) Destroy(map[z][x].gameObject);
                    map[z][x] = null;
                }
        }
    }
}