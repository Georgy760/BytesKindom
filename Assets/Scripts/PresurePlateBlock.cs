using System.Collections.Generic;
using UnityEngine;

public class PresurePlateBlock : Block
{
    public List<int> BridgeTileIndex;
    private int finishedAnim;
    private bool startedAnim;

    public void TriggerBridge()
    {
        if (startedAnim) return;

        FindObjectOfType<PlayerMovement>().SetCanMove(false);
        var bridge = (BridgeBlock) LevelManager.Instance.GetTileFromIndex(BridgeTileIndex[0]);
        var shouldOpen = !bridge.GetIsOpen();

        for (var i = 1; i < BridgeTileIndex.Count; i++)
        {
            bridge = (BridgeBlock) LevelManager.Instance.GetTileFromIndex(BridgeTileIndex[i]);
            //  bridge.Toggle();
            if (shouldOpen && bridge.GetIsOpen()) shouldOpen = false;
        }

        for (var i = 0; i < BridgeTileIndex.Count; i++)
        {
            bridge = (BridgeBlock) LevelManager.Instance.GetTileFromIndex(BridgeTileIndex[i]);
            if (shouldOpen)
            {
                bridge.Open(index);
                Debug.Log("Open");
            }
            else
            {
                bridge.Close(index);
                Debug.Log("Close");
            }
        }

        startedAnim = true;
    }

    public void AddAnimFinished()
    {
        finishedAnim++;
        if (finishedAnim == BridgeTileIndex.Count)
        {
            finishedAnim = 0;
            startedAnim = false;
            FindObjectOfType<PlayerMovement>().SetCanMove(true);
        }
    }

    public void TriggerBridgeOld()
    {
        BridgeBlock bridge = null;
        for (var i = 0; i < BridgeTileIndex.Count; i++)
        {
            bridge = (BridgeBlock) LevelManager.Instance.GetTileFromIndex(BridgeTileIndex[i]);
            bridge.Toggle();
        }
    }
}