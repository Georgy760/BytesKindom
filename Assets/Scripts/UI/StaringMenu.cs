using System;
using System.Collections;
using SimpleJSON;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Web.Example;

public class StaringMenu : MonoBehaviour
{
    private void Update()
    {
        /*
        if (!LevelLoader.Instance._connected)
        {
            LevelLoader.Instance._connected = CryptoReceiver.CR.isConnected;
        }
        if(LevelLoader.Instance._connected && !openMenu) 
        {
            LevelLoader.Instance.walletAddress = CryptoReceiver.CR.walletAddress;
            walletAddressText.text = "";
            walletAddressText.text = "Wallet address: " + LevelLoader.Instance.walletAddress;
            Debug.Log("WalletAddress: " + LevelLoader.Instance.walletAddress);
            LevelLoader.Instance.pubkey = CryptoReceiver.CR.pubKey;
            StartCoroutine(WaitForMint());
            if (!CryptoReceiver.CR.loaded)
            {
                return;
            }
            WebMenu.Instance.TryToReg();
            OpenMainMenu();
            openMenu = true;
        }
        */
    }
    
    

    public void Connect()
    {
        CryptoReceiver.CR.OnConnect();
        StartCoroutine(WalletConnection());
    }

    public void GusetMode()
    {
        LevelLoader.Instance.GuestMode = true;
        LevelLoader.Instance.LoadScene("Menu_Guest");
    }


    IEnumerator WaitForMint()
    { 
        Debug.Log("Started Coroutine at timestamp : " + Time.time);
        yield return new WaitForSeconds(10);
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
    IEnumerator openMainMenu()
    {
        yield return new WaitUntil(() => LevelLoader.Instance._ConnectionWithDB);
        Debug.Log("Player ID: " + WebManager.userData.playerData.id);
        LevelLoader.Instance.PlayerID = WebManager.userData.playerData.id;
        yield return new WaitUntil(() => WebMenu.Instance.mintsChecked);
        LevelLoader.Instance.LoadScene("Hub");
    }

    IEnumerator WalletConnection()
    {
        yield return new WaitUntil(() => LevelLoader.Instance._connected);
        Debug.Log("WalletAddress: " + LevelLoader.Instance.walletAddress);
        StartCoroutine(WaitForMint());
        yield return new WaitUntil(() => CryptoReceiver.CR.loaded);
        WebMenu.Instance.TryToReg();
        StartCoroutine(openMainMenu());
    }


}