using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BridgeBlock : Block
{
    public Vector3 anchor;
    public Vector3 rotationAxis = Vector3.right;
    public float rotationSpeed;

    [SerializeField] private bool isOpen;

    public bool isFirstTileOfBridge;
    public int bridgeAnchorTileId;
    public int previousTileIndex;
    public int nextTileIndex;
    public int myPressurePlateIndex;
    [SerializeField] protected AudioSource _audioSource;
    private float angleLeft;

    public Coroutine rotationCoroutine;

    public void SetIsOpen(bool value)
    {
        isOpen = value;
    }

    public bool GetIsOpen()
    {
        return isOpen;
    }

    public void SetUp()
    {
        Block tile = null;
        if (isFirstTileOfBridge)
            tile = LevelManager.Instance.GetTileFromIndex(bridgeAnchorTileId);
        else
            tile = LevelManager.Instance.GetTileFromIndex(previousTileIndex);

        var dir = transform.position - tile.transform.position;
        Debug.Log("LA DIR : " + dir + " t1 : " + tile.transform.position + " t2 : " + transform.position);
        if (dir.x > 0)
        {
            anchor = new Vector3(tile.transform.position.x + 0.5f, -0.25f, tile.transform.position.z);
            rotationAxis = Vector3.forward;
            Debug.Log("Case1");
        }
        else if (dir.x < 0)
        {
            anchor = new Vector3(tile.transform.position.x - 0.5f, -0.25f, tile.transform.position.z);
            rotationAxis = Vector3.back;
            Debug.Log("Case2");
        }
        else if (dir.z < 0)
        {
            anchor = new Vector3(tile.transform.position.x, -0.25f, tile.transform.position.z - 0.5f);
            rotationAxis = Vector3.right;
            Debug.Log("Case3");
        }
        else if (dir.z > 0)
        {
            anchor = new Vector3(tile.transform.position.x, -0.25f, tile.transform.position.z + 0.5f);
            rotationAxis = Vector3.left;
            Debug.Log("Case4");
        }

        if (nextTileIndex != -1)
        {
            Debug.Log("tileIndex : " + nextTileIndex + " block " + gameObject.name + "index : " + index);
            var bridge = (BridgeBlock) LevelManager.Instance.GetTileFromIndex(nextTileIndex);
            bridge.SetUp();
        }

        Debug.Log("Done");
    }

    [ContextMenu("Toggle")]
    public void Toggle()
    {
        Debug.Log(" CA TOGGLE A MAX PAR ICI ");
        if (GetIsOpen())
        {
            //Close();
        }
    }

    public void Open(int pressurePlateId)
    {
        myPressurePlateIndex = pressurePlateId;
        if (isOpen)
        {
            if (nextTileIndex != -1)
            {
                var bridge = (BridgeBlock) LevelManager.Instance.GetTileFromIndex(nextTileIndex);
                bridge.Open(pressurePlateId);
            }

            return;
        }

        SetIsOpen(true);
        if (angleLeft == 0) angleLeft = 180;
        StartRotationCoroutine(angleLeft, rotationAxis);
    }

    public void Close(int pressurePlateId)
    {
        myPressurePlateIndex = pressurePlateId;
        if (nextTileIndex != -1)
        {
            var bridge = (BridgeBlock) LevelManager.Instance.GetTileFromIndex(nextTileIndex);
            if (bridge.isOpen)
            {
                bridge.Close(pressurePlateId);
                return;
            }
        }

        SetIsOpen(false);
        if (angleLeft == 0) angleLeft = 180;
        StartRotationCoroutine(angleLeft, -rotationAxis, false);
    }

    public void StartRotationCoroutine(float angle, Vector3 axis, bool open = true)
    {
        if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);

        rotationCoroutine = StartCoroutine(Rotate(angle, axis, open));
    }

    public IEnumerator Rotate(float angle, Vector3 axis, bool open)
    {
        var rotationLeft = angle;
        float rotAmount = 0;

        _audioSource.Play();

        while (rotationLeft > 0)
        {
            //Debug.Log("rotate moi caaaaa");
            rotAmount = Mathf.Min(rotationLeft, rotationSpeed * Time.deltaTime);
            transform.RotateAround(anchor, axis, rotAmount);
            rotationLeft -= rotAmount;
            yield return null;
        }

        if (open)
        {
            if (nextTileIndex != -1)
            {
                var bridge = (BridgeBlock) LevelManager.Instance.GetTileFromIndex(nextTileIndex);
                if (isOpen) bridge.transform.position = new Vector3(transform.position.x, -0.25f, transform.position.z);
                bridge.Show(true).OnComplete(RotateNextTile);
            }
            else
            {
                var pp = (PresurePlateBlock) LevelManager.Instance.GetTileFromIndex(myPressurePlateIndex);
                pp.AddAnimFinished();
            }
        }
        else
        {
            if (previousTileIndex != -1)
            {
                //BridgeBlock bridge = (BridgeBlock)LevelManager.Instance.GetTileFromIndex(previousTileIndex);
                Hide(false).OnComplete(ClosePreviousTile);
            }
            else
            {
                var pp = (PresurePlateBlock) LevelManager.Instance.GetTileFromIndex(myPressurePlateIndex);
                pp.AddAnimFinished();
            }
        }
    }

    public void RotateNextTile()
    {
        if (nextTileIndex != -1)
        {
            var bridge = (BridgeBlock) LevelManager.Instance.GetTileFromIndex(nextTileIndex);
            bridge.Open(myPressurePlateIndex);
        }
    }

    public void ClosePreviousTile()
    {
        if (previousTileIndex != -1)
        {
            var bridge = (BridgeBlock) LevelManager.Instance.GetTileFromIndex(previousTileIndex);
            bridge.Close(myPressurePlateIndex);
        }
    }
}