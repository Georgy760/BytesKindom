using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

public class CardNFTPanelHandler : MonoBehaviour
{
    public CardNFTPrefab nftPrefab;
    public Transform content;
    public GameObject loadingText;
    public bool loaded = false;
    public List<GameObject> CyberBloks_prefabs;
    
    public CyberBuilder CyberBuilder;
    public bool loadingInProgress = false;
    private bool LoadingNFT_status = false;

    [SerializeField] private PlayerSelectionMenu PlayerSelectionMenu;
    [SerializeField] private GameObject NFTPrefab;
    
    [Header("DEBUG")]
    [Header("-----------------")]
    [SerializeField] private bool testModelLoading = false;
    [SerializeField] private bool debugLoadingSystem;
    [SerializeField] private int numberOfNFTs;
    

    private void Start()
    {
        if (debugLoadingSystem)
        {
            for (int i = 0; i < numberOfNFTs; i++)
            {
                CryptoNFT NFT_test = new CryptoNFT();
                NFT_test.name = "test";
                NFT_test.attributesName.Add("BODY", "SolanaSkin");
                NFT_test.attributesName.Add("EYES", "DBZPurpleGlasses");
                NFT_test.attributesName.Add("FRONT", "EthereumBox");
                CryptoReceiver.CR.myNFTs.Add(NFT_test);
            }

            LoadingNFT_status = false;
            CyberBloks_prefabs = new List<GameObject>();
            loaded = false;
            content.gameObject.SetActive(loaded);
            // Display loading panel while istantiating NFTs
            loadingText.SetActive(true);
            // Clear items to refresh to stop duplicates:
            foreach (Transform t in content)
            {
                Destroy(t.gameObject);
            }
            Debug.Log("RefreshNFT_START");
            StartCoroutine(RefreshNFT_Debug());
        }
    }

    public void RefreshNFTList()
    {
        Debug.Log("RefreshNFT_List");
        loadingInProgress = true;
        if (testModelLoading)
        {
            Debug.LogError("TEST-MODE");
            loaded = false;
            content.gameObject.SetActive(loaded);
            loadingText.SetActive(true);
            foreach (Transform t in content)
            {
                Destroy(t.gameObject);
            }

            List<GameObject> CyberBlocksTest = new List<GameObject>();
            foreach (var obj in CyberBloks_prefabs)
            {
                GameObject CB_test = Instantiate(obj, Vector3.zero, Quaternion.identity);
                CB_test.transform.SetParent(content.transform, false);
                CyberBlocksTest.Add(CB_test);
            }
            PlayerSelectionMenu.UpdateModelList(CyberBlocksTest);
            PlayerSelectionMenu.Initialise();
            loaded = true;
            loadingText.SetActive(false);
            content.gameObject.SetActive(loaded);
        }
        else
        {
            LoadingNFT_status = false;
            CyberBloks_prefabs = new List<GameObject>();
            loaded = false;
            content.gameObject.SetActive(loaded);
            // Display loading panel while istantiating NFTs
            loadingText.SetActive(true);
            // Clear items to refresh to stop duplicates:
            foreach (Transform t in content)
            {
                Destroy(t.gameObject);
            }
            Debug.Log("RefreshNFT_START");
            StartCoroutine(RefreshNFT());
            //PlayerSelectionMenu.UpdateModelList(CyberBloks_prefabs);
            //PlayerSelectionMenu.Initialise();
            //loaded = true;
        }
    }
    public void RefreshNFTListAndOpenMenu()
    {
        Debug.Log("RefreshNFT_List");
        loadingInProgress = true;
        LoadingNFT_status = false;
        CyberBloks_prefabs = new List<GameObject>();
        loaded = false;
        content.gameObject.SetActive(loaded);
            // Display loading panel while istantiating NFTs
        loadingText.SetActive(true);
            // Clear items to refresh to stop duplicates:
        foreach (Transform t in content)
        {
            Destroy(t.gameObject);
        }
        Debug.Log("RefreshNFT_START");
        StartCoroutine(RefreshNFT_And_OpenMenu());
            //PlayerSelectionMenu.UpdateModelList(CyberBloks_prefabs);
            //PlayerSelectionMenu.Initialise();
            //loaded = true;
        
    }
    IEnumerator RefreshNFT()
    {
        // For each NFT in the myNFTs list we create a new card to display our NFT
        var ownedNFT = CryptoReceiver.CR.myNFTs;
        for (int i = 0; i < ownedNFT.Count; i++){
            yield return new WaitForSeconds(0.3f);
            CardNFTPrefab item = Instantiate(nftPrefab);
            //item.transform.SetParent(content.transform, false);
            string value = "";
            if (ownedNFT[i].attributesName.TryGetValue("name", out value))
            {
                item.nftName.text = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("image", out value))
            {
                item.nftImageString = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("description", out value))
            {
                item.nftDescription.text = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("BODY", out value))
            {
                item.bodyName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("MASK", out value))
            {
                item.maskName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("HAT", out value))
            {
                item.hatName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("EYES", out value))
            {
                item.eyesName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("MOUTH", out value))
            {
                item.mouthName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("FRONT", out value))
            {
                item.frontName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("SIDES", out value))
            {
                item.sidesName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }

            List<GameObject> Attributes = new List<GameObject>();
            Attributes = CyberBuilder.GenerateNFT(item);
            GameObject nft = Instantiate(NFTPrefab, new Vector3(0, -50, 0), Quaternion.identity);
            nft.transform.SetParent(content.transform, false);
            nft.name = item.nftName.text;

            Debug.Log("NFT_name: " + nft.name + "\n" + "NFT pos: " + nft.transform.position + "\n" + "NFT rot: " + nft.transform.rotation.eulerAngles + "\n" + "NFT parent: " + nft.transform.parent.name + "\n");
            foreach (var obj in Attributes)
            {
                GameObject attribute = Instantiate(obj);
                attribute.name = obj.name;
                attribute.transform.SetParent(nft.transform, false);
                
                attribute.layer = 5;
                Debug.Log("Attribute_name: " + attribute.name + "\n" + "Attribute pos: " + attribute.transform.position + "\n");
            }
            
            CyberBloks_prefabs.Add(nft);
            //PlayerSelectionMenu.AddModelToList(nft);
            //item.nftName.text = ownedNFT[i].name;
            //item.nftImageString = ownedNFT[i].sprite;
            //item.nftDescription.text = ownedNFT[i].description;
            Debug.Log("content: " + content.childCount + "\n" + "ownedNFT's: " + ownedNFT.Count);
                // This will remove the laoding panel once the last NFT has been instantiated
                if (content.childCount == ownedNFT.Count)
                {
                    PlayerSelectionMenu.UpdateModelList(CyberBloks_prefabs);
                    PlayerSelectionMenu.Initialise();
                    PlayerSelectionMenu.SelectCurrentModel();
                    loaded = true;
                    Debug.Log("Loaded: " + loaded);
                    loadingText.SetActive(false);
                    LoadingNFT_status = true;
                    Debug.Log(CyberBloks_prefabs.Count);
                    Debug.Log("RefreshNFT_END");
                    Debug.Log("Loaded: " + loaded);
                    loadingText.SetActive(false);
                    content.gameObject.SetActive(loaded);
                    loadingInProgress = false;
                }
        }
        
    }
    
    IEnumerator RefreshNFT_And_OpenMenu()
    {
        // For each NFT in the myNFTs list we create a new card to display our NFT
        var ownedNFT = CryptoReceiver.CR.myNFTs;
        for (int i = 0; i < ownedNFT.Count; i++){
            yield return new WaitForSeconds(0.3f);
            CardNFTPrefab item = Instantiate(nftPrefab);
            //item.transform.SetParent(content.transform, false);
            string value = "";
            if (ownedNFT[i].attributesName.TryGetValue("name", out value))
            {
                item.nftName.text = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("image", out value))
            {
                item.nftImageString = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("description", out value))
            {
                item.nftDescription.text = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("BODY", out value))
            {
                item.bodyName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("MASK", out value))
            {
                item.maskName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("HAT", out value))
            {
                item.hatName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("EYES", out value))
            {
                item.eyesName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("MOUTH", out value))
            {
                item.mouthName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("FRONT", out value))
            {
                item.frontName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("SIDES", out value))
            {
                item.sidesName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }

            List<GameObject> Attributes = new List<GameObject>();
            Attributes = CyberBuilder.GenerateNFT(item);
            GameObject nft = Instantiate(NFTPrefab, new Vector3(0, -50, 0), Quaternion.identity);
            nft.transform.SetParent(content.transform, false);
            nft.name = item.nftName.text;

            Debug.Log("NFT_name: " + nft.name + "\n" + "NFT pos: " + nft.transform.position + "\n" + "NFT rot: " + nft.transform.rotation.eulerAngles + "\n" + "NFT parent: " + nft.transform.parent.name + "\n");
            foreach (var obj in Attributes)
            {
                GameObject attribute = Instantiate(obj);
                attribute.name = obj.name;
                attribute.transform.SetParent(nft.transform, false);
                
                attribute.layer = 5;
                Debug.Log("Attribute_name: " + attribute.name + "\n" + "Attribute pos: " + attribute.transform.position + "\n");
            }
            
            CyberBloks_prefabs.Add(nft);
            //PlayerSelectionMenu.AddModelToList(nft);
            //item.nftName.text = ownedNFT[i].name;
            //item.nftImageString = ownedNFT[i].sprite;
            //item.nftDescription.text = ownedNFT[i].description;
            Debug.Log("content: " + content.childCount + "\n" + "ownedNFT's: " + ownedNFT.Count);
                // This will remove the laoding panel once the last NFT has been instantiated
                if (content.childCount == ownedNFT.Count)
                {
                    PlayerSelectionMenu.UpdateModelList(CyberBloks_prefabs);
                    PlayerSelectionMenu.Initialise();
                    PlayerSelectionMenu.SelectCurrentModel();
                    loaded = true;
                    Debug.Log("Loaded: " + loaded);
                    loadingText.SetActive(false);
                    LoadingNFT_status = true;
                    Debug.Log(CyberBloks_prefabs.Count);
                    Debug.Log("RefreshNFT_END");
                    Debug.Log("Loaded: " + loaded);
                    loadingText.SetActive(false);
                    content.gameObject.SetActive(loaded);
                    loadingInProgress = false;
                    LevelLoader.Instance.LoadMenu();
                }
        }
        
    }
    IEnumerator RefreshNFT_Debug()
    {
        // For each NFT in the myNFTs list we create a new card to display our NFT
        var ownedNFT = CryptoReceiver.CR.myNFTs;
        for (int i = 0; i < ownedNFT.Count; i++){
            yield return new WaitForSeconds(0.3f);
            CardNFTPrefab item = Instantiate(nftPrefab);
            //item.transform.SetParent(content.transform, false);
            string value = "";
            if (ownedNFT[i].attributesName.TryGetValue("name", out value))
            {
                item.nftName.text = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("image", out value))
            {
                item.nftImageString = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("description", out value))
            {
                item.nftDescription.text = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("BODY", out value))
            {
                item.bodyName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("MASK", out value))
            {
                item.maskName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("HAT", out value))
            {
                item.hatName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("EYES", out value))
            {
                item.eyesName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("MOUTH", out value))
            {
                item.mouthName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("FRONT", out value))
            {
                item.frontName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }
            if (ownedNFT[i].attributesName.TryGetValue("SIDES", out value))
            {
                item.sidesName = value;
                Debug.Log("$$$$" + value + "$$$$" + "\n");
            }

            List<GameObject> Attributes = new List<GameObject>();
            Attributes = CyberBuilder.GenerateNFT(item);
            GameObject nft = Instantiate(NFTPrefab, new Vector3(0, -50, 0), Quaternion.identity);
            nft.transform.SetParent(content.transform, false);
            nft.name = item.nftName.text;

            Debug.Log("NFT_name: " + nft.name + "\n" + "NFT pos: " + nft.transform.position + "\n" + "NFT rot: " + nft.transform.rotation.eulerAngles + "\n" + "NFT parent: " + nft.transform.parent.name + "\n");
            foreach (var obj in Attributes)
            {
                GameObject attribute = Instantiate(obj);
                attribute.name = obj.name;
                attribute.transform.SetParent(nft.transform, false);
                
                attribute.layer = 5;
                Debug.Log("Attribute_name: " + attribute.name + "\n" + "Attribute pos: " + attribute.transform.position + "\n");
            }
            
            CyberBloks_prefabs.Add(nft);
            //PlayerSelectionMenu.AddModelToList(nft);
            //item.nftName.text = ownedNFT[i].name;
            //item.nftImageString = ownedNFT[i].sprite;
            //item.nftDescription.text = ownedNFT[i].description;
            Debug.Log("content: " + content.childCount + "\n" + "ownedNFT's: " + ownedNFT.Count);
                // This will remove the laoding panel once the last NFT has been instantiated
                if (content.childCount == ownedNFT.Count)
                {
                    PlayerSelectionMenu.UpdateModelList(CyberBloks_prefabs);
                    PlayerSelectionMenu.Initialise();
                    PlayerSelectionMenu.SelectCurrentModel();
                    loaded = true;
                    Debug.Log("Loaded: " + loaded);
                    loadingText.SetActive(false);
                    LoadingNFT_status = true;
                    Debug.Log(CyberBloks_prefabs.Count);
                    Debug.Log("RefreshNFT_END");
                    Debug.Log("Loaded: " + loaded);
                    loadingText.SetActive(false);
                    content.gameObject.SetActive(loaded);
                    loadingInProgress = false;
                }
        }
        
    }
}
