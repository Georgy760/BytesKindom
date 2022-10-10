using System;
using System.Collections;
using SimpleJSON;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Web.Example;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject _PanelCollection;
    [SerializeField] private CardNFTPanelHandler _cardNftPanelHandler;
    [SerializeField] private TMP_Text walletAddressText;


    private void Start()
    { 
        walletAddressText.text = ""; 
        walletAddressText.text = "Wallet address: " + LevelLoader.Instance.walletAddress;
    }


    public void ParticipateButton()
    {
        WebMenu.Instance.TryToParticipate();
    }

    public void HideCollectionTab()
    {
        _PanelCollection.SetActive(false);
    }
    public void CollectionTab()
    {
        if (_PanelCollection.activeSelf)
        {
            _PanelCollection.SetActive(false);
        }
        else
        {
            _cardNftPanelHandler.RefreshNFTList();
            _PanelCollection.SetActive(true);
        }
    }

}