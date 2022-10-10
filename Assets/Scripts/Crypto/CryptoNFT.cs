using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;

[CreateAssetMenu(menuName = "Puppy Gaming/NFT")]
public class CryptoNFT : ScriptableObject
{
    //[SerializeField] private Dictionary<string, GameObject> attributesModels = new Dictionary<string, GameObject>();
    public Dictionary<string, string> attributesName = new Dictionary<string, string>();
    
    public string name;
    public string sprite;
    public string description;
    //public string attributes;
}
