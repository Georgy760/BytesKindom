using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    public int PlayerID;
    public int LevelIndex;
    private float _target;
    private float _currentAmount;
    [SerializeField] private int levelToLoad;
    public AudioSource MusicSource;

    public AudioSource SelectionAudio;
    public GameObject modelHolder;
    public GameObject NFTmodel;
    
    public string pubkey; 
    public string walletAddress;
    public bool _connected = false;
    public bool _ConnectionWithDB = false;
    public string scoreboard;
    public bool scoreboardLoaded = false;
    public bool local_scoreboardLoaded = false;
    public bool GuestMode;
    public string placement;
    public string best_score;
    public string best_time;
    public int currentIndex;
    public bool TournamentMode = false;
    


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
        
    }

    public void ClearLevelIndex()
    {
        LevelIndex = -1;
    }

    public void LoadNextScene()
    {
        if (!GuestMode)
        {
            Debug.Log("LoadNextScene");
            _target = 0;
            _currentAmount = 0;
            int sceneIndex;
            levelToLoad = SceneManager.GetActiveScene().buildIndex;
            if (levelToLoad < SceneManager.sceneCountInBuildSettings - 1)
            {
                levelToLoad += 1;
                sceneIndex = levelToLoad;
            }
            else
            {
                levelToLoad = 1;
                sceneIndex = levelToLoad;
            }

            //_loaderCanvas.SetActive(true);


            SceneManager.LoadScene(sceneIndex);
            //_loaderCanvas.SetActive(false);
        }
        else
        {
            if(LevelIndex == 3) LoadScene("lvl2");
            if(LevelIndex == 2) LoadScene("lvl6");
            if(LevelIndex == 6) LoadScene("Menu_Guest");
        }
    }
    
    public void LoadScene(int sceneIndex)
    {
        Debug.Log("LoadScene: " + sceneIndex);
        _target = 0;
        _currentAmount = 0;
        //_current_level.text = "LVL: " + sceneIndex;
        
        //_loaderCanvas.SetActive(true);
        
        
        SceneManager.LoadScene(sceneIndex);
        //_loaderCanvas.SetActive(false);
        
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log("LoadScene: " + sceneName);
        _target = 0;
        _currentAmount = 0;
        //_current_level.text = "LVL: " + SceneManager.GetSceneByName(sceneName).buildIndex;
        
        //_loaderCanvas.SetActive(true);

        SceneManager.LoadScene(sceneName);
        //_loaderCanvas.SetActive(false);
        
    }
    
    public void LoadMenu()
    {
        if (WebManager.userData.playerData.participate)
        {
            Debug.Log("Player ID: " + PlayerID);
            Debug.Log("Player have unused nft & can participate");
            CardNFTPanelHandler NFT_Panel = GameObject.Find("UI Handler").GetComponent<CardNFTPanelHandler>();

            if (NFT_Panel.loaded)
            {
                Debug.Log("LoadScene: Menu");

                SceneManager.LoadScene("Menu");

                
                return;
            }

            if (!NFT_Panel.loadingInProgress)
            {
                NFT_Panel.RefreshNFTListAndOpenMenu();
            }
        } else Debug.Log("Player doesnt have unused nft");

    }

    public void LoadMenuFromGame()
    {
        if (!Instance.GuestMode)
        {
            LoadScene("Menu");
        } else LoadScene("Menu_Guest");
        
    }

    
}
