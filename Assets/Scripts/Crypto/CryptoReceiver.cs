using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Events;

public class CryptoReceiver : MonoBehaviour
{
    public static CryptoReceiver CR;
    public List<string> myTokens;
    public List<CryptoNFT> myNFTs;
    public string pubKey;
    [DllImport("__Internal")]
    private static extern string SolanaLogin();
    public bool connected = false;
    public string walletAddress;
    public string shortAddress;
    public bool isConnected = false;
    public List<string> mints;
    public string mintsString = null;
    public bool loaded = false;
    private float timer;

    [SerializeField] private UnityEvent OnConnected, OnRecivedMint;
    // When the script first loads, it makes sure this is the only running one and destroys any other.
    // It also persists across scenes
    void Awake()
    {
        if (CR == null)
        {
            CR = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        
    }

    // This function receives the string sent from the JavaScript function on the WebGL's
    // page and add each string (NFT Mint ID) into a list
    // This list can be accessed anywhere with CryptoReceiver.CR.myTokens
    public void ReceiveToken(string mint)
    {
        if (!myTokens.Contains(mint))
        {
            int addLocation = 0;
            while (addLocation < myTokens.Count)
            {
                addLocation++;
            }
            myTokens.Add(mint);
            Debug.Log("Added: " + mint);
        }
    }

    // This function receives the string sent from the JavaScript function on the WebGL's
    // page and and parses the string into sections divided by the "|" character as the function only allows 1 string
    // It then populates a list with CryptoNFT objects which contain common NFT data
    public void ReceiveMetadata(string metaData)
    {
        
        // Instantiate a new CryptoNFT object
        CryptoNFT newNFT = CryptoNFT.CreateInstance<CryptoNFT>();
        foreach (var data in metaData.Split('|'))
        {
            loaded = false;
            string[] dataParms = data.Split('$');
            if (dataParms[0] == "name")
            {
                mints.Add(dataParms[1]);
            }
            newNFT.attributesName.Add(dataParms[0], dataParms[1]);
            loaded = true;
        }

        foreach (var NFT_attributes in newNFT.attributesName)
        {
            Debug.Log("NFT_attributes: "+ NFT_attributes.Key + "." + NFT_attributes.Value + "\n");
        }
        // Add the instantiated CryptoNFT to the myNFTs List
        int addLocation = 0;
        while (addLocation < myNFTs.Count)
        {
            addLocation++;
        }
        myNFTs.Add(newNFT);
    }

    public void ReceiveMint(string mintKey)
    {
        if (!mints.Contains(mintKey))
        {
            //mints.Add(mintKey);
            if (mintsString == null)
            {
                mintsString = String.Empty;
                mintsString += mintKey + "|";
            }
            else mintsString += mintKey + "|";
            Debug.Log("Added: " + mintKey);
            OnRecivedMint.Invoke();
        }
        else Debug.Log("Mint already in the list");
    }
    public void OnConnect(){
            SolanaLogin();
    }

    public void ClearOnLogout()
    {
        myNFTs.Clear();
        myTokens.Clear();
    }

    // This function receives the users Public Addres on Phantom login and saves it along with a shortened version e.g AaAa....BbBb
    // and sets the 'isConnected' variable
    public void ReceiveAddress(string address) 
    {
        walletAddress = address;
        string namestart = address.Substring(0, 4);
        string nameend = address.Substring((address.Length - 4), 4);
        shortAddress = namestart + "...." + nameend;
        isConnected = true;
        LevelLoader.Instance._connected = true;
        LevelLoader.Instance.walletAddress = walletAddress;
        LevelLoader.Instance.pubkey = pubKey;
        OnConnected.Invoke();
    }


}