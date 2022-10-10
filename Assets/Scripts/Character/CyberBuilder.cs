using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberBuilder : MonoBehaviour
{
    [SerializeField] private List<GameObject> _bodyList;
    [SerializeField] private List<GameObject> _maskList;
    [SerializeField] private List<GameObject> _hatList;
    [SerializeField] private List<GameObject> _eyesList;
    [SerializeField] private List<GameObject> _mouthList;
    [SerializeField] private List<GameObject> _frontList;
    [SerializeField] private List<GameObject> _sidesList;

    //[SerializeField] private GameObject _prefab;

    public List<GameObject> GenerateNFT(CardNFTPrefab nft)
    {
        Debug.Log("GeneratingNFT_Model\n");
        List<GameObject> models = new List<GameObject>();
        //GameObject model = _prefab;
        //model.name = nft.nftName.text;
        foreach (var obj in _bodyList)
        {
            if (obj.name == nft.bodyName)
            {
                models.Add(obj);
                //GameObject body = Instantiate(obj, model.transform);
                Debug.Log(obj.name);
            }
        }
        foreach (var obj in _maskList)
        {
            if (obj.name == nft.maskName)
            {
                models.Add(obj);
                Debug.Log(obj.name);
            }
        }
        foreach (var obj in _hatList)
        {
            if (obj.name == nft.hatName)
            {
                models.Add(obj);
                Debug.Log(obj.name);
            }
        }
        foreach (var obj in _eyesList)
        {
            if (obj.name == nft.eyesName)
            {
                models.Add(obj);
                Debug.Log(obj.name);
            }
        }
        foreach (var obj in _mouthList)
        {
            if (obj.name == nft.mouthName)
            {
                models.Add(obj);
                Debug.Log(obj.name);
            }
        }
        foreach (var obj in _frontList)
        {
            if (obj.name == nft.frontName)
            {
                models.Add(obj);
                Debug.Log(obj.name);
            }
        }
        foreach (var obj in _sidesList)
        {  
            if (obj.name == nft.sidesName)
            {
                models.Add(obj);
                Debug.Log(obj.name);
            }
        }
        
        //PlayerSelectionMenu.models.Add(model);
        //SettingsMenu.Instance.NFTmodel.Add(model);
        return models;
    }
}
