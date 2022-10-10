using UnityEngine;

public class MultiBlock : Block
{
    [SerializeField] private TypeBlockDictionary levelBlockPrefabs;

    public BlockType currentType;
    public Block currentBlock;

    public override void Awake()
    {
        base.Awake();
        currentBlock = levelBlockPrefabs[BlockType.Empty];
        currentBlock.gameObject.SetActive(true);
        currentType = BlockType.Empty;
    }

    [ContextMenu("Next")]
    public void Next()
    {
        var next = false;
        //levelBlockPrefabs[BlockType.Empty].gameObject.SetActive(true);
        foreach (var key in levelBlockPrefabs.Keys)
        {
            if (next)
            {
                levelBlockPrefabs[currentType].gameObject.SetActive(false);
                currentBlock = levelBlockPrefabs[key];
                currentBlock.gameObject.SetActive(true);
                currentType = key;
                return;
            }

            if (key == currentType) next = true;
        }

        foreach (var key in levelBlockPrefabs.Keys)
        {
            levelBlockPrefabs[currentType].gameObject.SetActive(false);
            currentBlock = levelBlockPrefabs[key];
            currentBlock.gameObject.SetActive(true);
            currentType = key;
            return;
        }
    }
}