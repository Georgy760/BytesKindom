using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOld : MonoBehaviour
{
    [SerializeField] public int levelIndex;

    public float timeBetweenBlockSpawn;

    /* Blocks values : 
     * empty = 0
     * blocks = 1
     * goal = 2 
     * Hard Plate = 3
     * Bridge = 4 
     * Teleporter = 5
     * Soft Plate = 6
     * Unstable Tile = 7
     */

    [SerializeField] public int spawnX, spawnY;

    [SerializeField] private Block levelBlockPrefab;

    [SerializeField] private IntBlockDictionary levelBlockPrefabs;

    public bool spawnOnStart;
    private Block[,,] levelBlocks;
    public int[,,] levelLayout;

    /*int[,,] testLayout= new int[1,6,10]{
        {
            {0,1,1,0,0,1,1,0,1,0},
            {1,1,1,1,0,1,1,1,1,1},
            {0,1,1,1,0,1,1,0,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {0,0,0,0,0,1,1,1,1,0},
            {0,0,0,1,1,1,0,0,1,0}
        }
    };*/

    public int[,,] testLayout =
    {
        {
            /*{0,0,0,0,0,0,0,0,0,0,0,0,1,1},
            {1,1,1,0,0,0,0,1,5,1,0,1,2,1},
            {1,1,1,1,0,5,1,0,1,1,0,1,1,0},
            {1,1,0,1,1,1,1,0,1,1,0,1,1,0},
            {0,0,0,1,1,1,1,0,1,1,1,1,0,0},
            {0,0,0,1,1,1,1,0,0,0,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,1,0,0}*/

            /* {0,0,0,0,0,0,0,0,0,1,0,0,0},
             {1,1,1,0,1,1,0,0,1,2,1,1,1},
             {1,1,1,1,1,1,0,0,0,1,0,1,1},
             {1,1,1,0,1,3,1,1,0,0,1,1,1},
             {0,1,1,0,0,1,1,1,4,4,1,1,0},
             {0,0,0,0,0,1,1,1,0,0,1,1,0},*/

            {1, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 7, 1, 1, 1, 1, 1, 0, 0, 0, 0},
            {0, 1, 5, 1, 1, 1, 1, 1, 1, 1, 1},
            {0, 0, 0, 1, 1, 1, 1, 1, 1, 2, 1},
            {0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1},
            {0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0}

            /* {1,1,1,3,0,0,0,0,0,0,0},
             {1,1,1,1,0,0,0,0,0,0,0},
             {1,1,1,1,4,4,1,1,1,1,1},
             {0,0,0,0,0,0,1,1,1,2,1},
             {0,0,0,0,0,0,1,1,1,1,1},
             {0,0,0,0,0,0,0,0,0,0,0}*/
        }
    };

    private void Start()
    {
        if (spawnOnStart) StartCoroutine(StartLevel());
    }

    // Start is called before the first frame update
    [ContextMenu("LoadThisLevel")]
    public List<Block> LoadThisLevel()
    {
        return LoadLayout(testLayout);
    }

    public Block GetTileAt(Vector2 pos)
    {
        if (IsPlayerPosOutOfBound(pos)) return null;

        return levelBlocks[0, (int) pos.y, (int) pos.x];
    }

    public int GetTileValueAt(Vector2 pos)
    {
        if (IsPlayerPosOutOfBound(pos)) return 0;

        return levelLayout[0, (int) pos.y, (int) pos.x];
    }


    public List<Block> LoadLayout(int[,,] layout)
    {
        var blocks = new List<Block>();
        var height = layout.GetLength(0);

        var depth = layout.GetLength(1);
        var width = layout.GetLength(2);

        //Debug.Log("h:"+height+"w:"+width+"d:"+depth);

        levelLayout = layout;
        levelBlocks = new Block[height, depth, width];

        for (var layer = 0; layer < height; layer++)
        for (var z = 0; z < depth; z++)
        for (var x = 0; x < width; x++)
        {
            var val = levelLayout[layer, z, x];
            if (val != 0)
            {
                var pos = (x, layer, z);
                var newBlock = AddBlock(pos, val);
                newBlock.Hide();
                blocks.Add(newBlock);
                levelBlocks[layer, z, x] = newBlock;
            }
            else
            {
                levelBlocks[layer, z, x] = null;
            }
        }

        return blocks;
    }

    private IEnumerator RevealLevel()
    {
        foreach (var b in levelBlocks)
            if (b != null)
            {
                b.Show();
                yield return new WaitForSeconds(timeBetweenBlockSpawn);
            }
    }

    private Block GetBlock(int val)
    {
        if (levelBlockPrefabs.Contains(val))
            return levelBlockPrefabs[val];
        return null;
    }

    public Block AddBlock((int x, int y, int z) pos, int value, bool originalSize = false)
    {
        var axis = Vector3.zero;
        var anchor = Vector3.zero;
        if (value == 0) return null;

        var vecPos = new Vector3(pos.x, pos.y, pos.z);
        if (value == 4)
        {
            /*Block bridge = GetTileAt(new Vector2(pos.x + 1, pos.z));
                Debug.Log("le bridge : " + bridge);
                if (bridge != null && bridge.type == BlockType.Bridge)
                {
                    vecPos = new Vector3(pos.x - 1, pos.y - 0.25f, pos.z);
                    Debug.Log("droite");
                }

                bridge = GetTileAt(new Vector2(pos.x - 1, pos.z));
                if (bridge != null && bridge.type == BlockType.Bridge)
                {
                    vecPos = new Vector3(pos.x + 1, pos.y - 0.25f, pos.z);
                    Debug.Log("gauche");
                }

                bridge = GetTileAt(new Vector2(pos.x, pos.z - 1));
                if (bridge != null && bridge.type == BlockType.Bridge)
                {
                    vecPos = new Vector3(pos.x, pos.y - 0.25f, pos.z + 1);
                    Debug.Log("bas");
                }

                bridge = GetTileAt(new Vector2(pos.x, pos.z + 1));
                if (bridge != null && bridge.type == BlockType.Bridge)
                {
                    vecPos = new Vector3(pos.x, pos.y - 0.25f, pos.z - 1);
                    Debug.Log("Haut");
                }*/

            if (GetTileValueAt(new Vector2(pos.x + 1, pos.z)) == 4)
            {
                vecPos = new Vector3(pos.x - 1, pos.y - 0.25f, pos.z);
                anchor = new Vector3(pos.x - 0.5f, pos.y - 0.25f, pos.z);
                axis = Vector3.forward;
                Debug.Log("droite");
            }

            if (GetTileValueAt(new Vector2(pos.x - 1, pos.z)) == 4)
            {
                vecPos = new Vector3(pos.x + 1, pos.y - 0.25f, pos.z);
                anchor = new Vector3(pos.x + 0.5f, pos.y - 0.25f, pos.z);
                axis = Vector3.back;
                Debug.Log("gauche");
            }

            if (GetTileValueAt(new Vector2(pos.x, pos.z - 1)) == 4)
            {
                vecPos = new Vector3(pos.x, pos.y - 0.25f, pos.z + 1);
                anchor = new Vector3(pos.x, pos.y - 0.25f, pos.z + 0.5f);
                axis = Vector3.left;
                Debug.Log("bas");
            }

            if (GetTileValueAt(new Vector2(pos.x, pos.z + 1)) == 4)
            {
                vecPos = new Vector3(pos.x, pos.y - 0.25f, pos.z - 1);
                anchor = new Vector3(pos.x, pos.y - 0.25f, pos.z - 0.5f);
                axis = Vector3.right;
                Debug.Log("Haut");
            }

            Debug.Log("new vect : " + vecPos);
        }

        var block = Instantiate(GetBlock(value), vecPos, Quaternion.identity);
        if (value == 4)
        {
            ((BridgeBlock) block).anchor = anchor;
            ((BridgeBlock) block).rotationAxis = axis;
        }

        if (originalSize)
        {
            // block.transform.localScale = Vector3.one;
        }

        return block;
    }

    [ContextMenu("Toggle Bridge")]
    public void TriggerBridge()
    {
        var height = levelLayout.GetLength(0);
        var depth = levelLayout.GetLength(1);
        var width = levelLayout.GetLength(2);

        for (var layer = 0; layer < height; layer++)
        for (var z = 0; z < depth; z++)
        for (var x = 0; x < width; x++)
        {
            var val = levelLayout[layer, z, x];
            var newPos = new Vector2(x, z);
            if (val == 4) ((BridgeBlock) GetTileAt(new Vector2(x, z))).Toggle();
        }
    }

    public Coroutine StartLevelCoroutine()
    {
        return StartCoroutine(StartLevel());
    }

    public IEnumerator StartLevel()
    {
        LoadThisLevel();
        yield return StartCoroutine(RevealLevel());
        yield return LevelManager.Instance.StartSpawnPlayer((spawnX, spawnY));
    }

    public bool IsPlayerPosOutOfBound(Vector2 pos1)
    {
        var valide = true;
        var x = (int) pos1.x;
        var y = (int) pos1.y;

        if (y < 0 || x < 0 || y >= levelLayout.GetLength(1) || x >= levelLayout.GetLength(2)) return true;

        if (levelLayout[0, y, x] == 0)
            // Debug.Log("pos1 not valide : " + x + " , " + y);
            valide = false;

        return !valide;
    }

    public bool HasPlayerWin(Vector2 pos1, Vector2 pos2)
    {
        if (pos1 != pos2 || IsPlayerPosOutOfBound(pos1)) return false;

        var x = (int) pos1.x;
        var y = (int) pos1.y;

        return levelLayout[0, y, x] == 2;
    }

    public bool IsPlayerOnTeleporter(Vector2 pos1, Vector2 pos2)
    {
        if (pos1 != pos2 || IsPlayerPosOutOfBound(pos1)) return false;

        var x = (int) pos1.x;
        var y = (int) pos1.y;

        return levelLayout[0, y, x] == 5;
    }

    public bool IsPlayerOnSoftSwitch(Vector2 pos1, Vector2 pos2)
    {
        if (IsPlayerPosOutOfBound(pos2) || IsPlayerPosOutOfBound(pos1)) return false;

        var x = (int) pos1.x;
        var y = (int) pos1.y;
        var xBis = (int) pos2.x;
        var yBis = (int) pos2.y;

        return levelLayout[0, y, x] == 6 || levelLayout[0, yBis, xBis] == 6;
    }

    public bool IsPlayerOnHardSwitch(Vector2 pos1, Vector2 pos2)
    {
        if (pos1 != pos2 || IsPlayerPosOutOfBound(pos1)) return false;

        var x = (int) pos1.x;
        var y = (int) pos1.y;

        return levelLayout[0, y, x] == 3;
    }

    public bool IsOnUnstableTile(Vector2 pos1, Vector2 pos2)
    {
        if (pos1 != pos2 || IsPlayerPosOutOfBound(pos1)) return false;

        var x = (int) pos1.x;
        var y = (int) pos1.y;

        return levelLayout[0, y, x] == 7;
    }

    public Vector2 GetOtherTeleporter(Vector2 myTeleporterPos)
    {
        var myX = (int) myTeleporterPos.x;
        var myY = (int) myTeleporterPos.y;

        if (levelLayout[0, myY, myX] != 5) return Vector2.one * -1;

        var height = levelLayout.GetLength(0);
        var depth = levelLayout.GetLength(1);
        var width = levelLayout.GetLength(2);

        for (var layer = 0; layer < height; layer++)
        for (var z = 0; z < depth; z++)
        for (var x = 0; x < width; x++)
        {
            var val = levelLayout[layer, z, x];
            var newPos = new Vector2(x, z);
            if (val == 5 && myTeleporterPos != newPos) return newPos;
        }

        return Vector2.one * -1;
    }
}