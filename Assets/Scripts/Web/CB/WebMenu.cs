using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebMenu : MonoBehaviour
{
    public static WebMenu Instance;
    
    public bool mintsChecked = false;
    [SerializeField] public bool requestInProgress { get; set; }
    
    private int requestCount;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        requestInProgress = false;
    }

    public void SetConnection()
    {
        LevelLoader.Instance._ConnectionWithDB = true;
    }
    public void TryToReg()
    {
        if (!requestInProgress)
        {
            Debug.Log("Trying to registration");
            requestInProgress = true;
            WebManager.WM.Registration(CryptoReceiver.CR.walletAddress);
        }
        else StartCoroutine(WaitUntilRequest(TryToLog));
    }

    public void StatusReg()
    {
        Debug.Log("Reg complete");
    }
    public void TryToLog()
    {
        if (!requestInProgress)
        {
            Debug.Log("Trying to login");
            requestInProgress = true;
            WebManager.WM.Login(CryptoReceiver.CR.walletAddress);
        }
        else StartCoroutine(WaitUntilRequest(TryToLog));
    }
    
    public void StatusLog()
    {
        Debug.Log("Log complete");
    }
    
    public void MintUpdate()
    {
        if (!requestInProgress)
        {
            Debug.Log("Mint updating");
            requestInProgress = true;
            WebManager.WM.MintUpdate(LevelLoader.Instance.PlayerID);
        }
        else StartCoroutine(WaitUntilRequest(MintUpdate));
    }
    
    public void StatusMintUpdate()
    {
        Debug.Log("MintUpdate complete");
    }
    
    public void MintCheck()
    {
        if (!requestInProgress)
        {
            Debug.Log("Mint checking");
            requestInProgress = true;
            if (CryptoReceiver.CR.mints.Count > 0)
            {
                requestCount = 0;
                WebManager.WM.MintCheck(LevelLoader.Instance.PlayerID, CryptoReceiver.CR.mints);
                StartCoroutine(mintCheck());
            }
            else
            {
                Debug.Log("You don't have any CB's");
                requestInProgress = false;
                mintsChecked = true;
            }
        } 
        else StartCoroutine(WaitUntilRequest(MintCheck));
    }
    
    public void StatusMintCheck()
    {
        Debug.Log("MintCheck complete");
    }

    public void RequestPlus()
    {
        requestCount++;
    }

    public void TryToParticipate()
    {
        if (!requestInProgress)
        {
            requestInProgress = true;
            WebManager.WM.Participate(LevelLoader.Instance.PlayerID);
            StartCoroutine(Participating());
        }
        else StartCoroutine(WaitUntilRequest(TryToParticipate));
    }
    
    public void StatusParticipate()
    {
        Debug.Log("Participate complete");
    }

    public void OnScoreboardGet()
    {
        LevelLoader.Instance.scoreboard = WebManager.userData.playerData.score;
        LevelLoader.Instance.scoreboardLoaded = true;
        Debug.Log("Global Scoreboard update: " + LevelLoader.Instance.scoreboard);
    }
    
    public void OnScoreboardGetByIndex()
    {
        LevelLoader.Instance.scoreboard = WebManager.userData.playerData.score;
        LevelLoader.Instance.local_scoreboardLoaded = true;
        Debug.Log("Level Scoreboard update: " + LevelLoader.Instance.scoreboard);
    }
    
    public void OnError()
    {
        Debug.Log(WebManager.userData.error.errorText);
        
        if (WebManager.userData.error.errorText == "Error: Wallet is not recognized")
        {
            return;
        }
        
        if (WebManager.userData.error.errorText == "Error: User Exists")
        {
            Debug.Log("Trying to login");
            TryToLog();
            return;
        }

        if (WebManager.userData.error.errorText == "Error: Player doesnt have any NFT")
        {
            MintCheck();
            return;
        }
        
        if (WebManager.userData.error.errorText == "Error: Player doesnt have unused nft")
        {
            return;
        }
        
        if (WebManager.userData.error.errorText == "Error: Update Mint Error")
        {
            return;
        }
        
        if (WebManager.userData.error.errorText == "Error: Check Mint Error")
        {
            return;
        }
        
        if (WebManager.userData.error.errorText == "Error: Save Data Error")
        {
            return;
        }
        
        if (WebManager.userData.error.errorText == "Error: Unknown data")
        {
            Debug.Log("Unknown data");
            return;
        }

        if (WebManager.userData.error.errorText == "Error: Wrong data")
        {
            Debug.Log("Error: Wrong data");
            return;
        }
    }
    
    IEnumerator WaitUntilRequest(Action method)
    {
        yield return new WaitUntil(() => !requestInProgress);
        method();
        yield return null;
    }
    
    IEnumerator Participating()
    {
        if (LevelLoader.Instance.TournamentMode)
        {
            yield return new WaitUntil(() => WebManager.userData.playerData.participate);
            requestInProgress = false;
            LevelLoader.Instance.LoadMenu();
        }
        else
        {
            requestInProgress = false;
            LevelLoader.Instance.LoadMenu();
        }
        
        yield return null;
        
    }

    IEnumerator mintCheck()
    {
        yield return new WaitUntil(() => WebManager.userData.error.errorText == "Error: Check Mint Error" || requestCount == CryptoReceiver.CR.mints.Count);
        mintsChecked = true;
        requestInProgress = false;
            
    }

}
