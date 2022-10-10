using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public delegate void CompleteCallback(GameObject go);

public class CyberBlok : MonoBehaviour
{
    //[SerializeField] private string publicKey;

    //private JSONNode _metadatas;

    private Dictionary<string, GameObject> attributesModels;

    private CompleteCallback callback;

    private int stepsNeeded;

    public void Setup(string pubkey, JSONNode metadatas, int stepsToValidate, CompleteCallback callback)
    {
        stepsNeeded = stepsToValidate;
    //    publicKey = pubkey;
    //    _metadatas = metadatas;
        attributesModels = new Dictionary<string, GameObject>();
        this.callback = callback;
    }

    /*
    public CyberBlok(string pubKey,JSONNode metadatas){
        this.publicKey= pubKey;
        this._metadatas=metadatas;
        Debug.Log(metadatas.ToString());
    }*/

    public void AddPart(string part, string partName, GameObject model)
    {
        //attributesName.Add(part, partName);
        attributesModels.Add(part, model);
        model.transform.parent = transform;
        model.transform.localPosition = transform.localPosition;
        model.transform.localScale = Vector3.one;
        model.transform.localRotation = Quaternion.identity;
        stepsNeeded -= 1;
        if (stepsNeeded == 0)
            if (callback != null)
                callback(gameObject);
    }
}