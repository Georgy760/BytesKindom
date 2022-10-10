using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[Serializable]
public class UserData
{
    public Player playerData;
    public Error error;
}

[Serializable]
public class Error
{
    public string errorText;
    public bool isError;
}

[Serializable]
public class Player
{
    public int id;
    public string wallet;
    public string mints;
    public bool participate;
    public string score;

    /*
    public int levels_score;
    public int levels_completed;
    */

    public Player()
    {
    }

    public Player(string testWallet, string mintkeys)
    {
        wallet = testWallet;
        mints = mintkeys;
    }

    public void SetMintsKeys(string mintkeys)
    {
        mints = mintkeys;
    }
    

    public void SetWallet(string address)
    {
        wallet = address;
    }
}

public class WebManager : MonoBehaviour
{
    public enum RequestType
    {
        logging,
        register,
        save,
        mintUpdate,
        mintCheck,
        participate,
        saveLevel,
        getScoreboard,
        getScoreboardByIndex
    }

    public static WebManager WM;
    public static UserData userData = new();
    [SerializeField] private string targetURL;

    [SerializeField] private UnityEvent OnLogged, OnRegistered, OnParticipated, OnMintUpdate, OnCheckMint, OnError, OnGetScoreboard, OnLevelSaved, OnGetScoreboardByIndex;

    private void Awake()
    {
        if (WM == null)
        {
            WM = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        userData.error = new Error {errorText = "text", isError = true};
        userData.playerData = new Player("TestWallet", "Mints");
    }


    public string GetUserData(UserData data)
    {
        return JsonUtility.ToJson(data);
    }

    public UserData SetUserData(string data)
    {
        print(data);
        return JsonUtility.FromJson<UserData>(data);
    }

    /// <summary>
    /// Method <c>Login</c> checking string at DB.
    /// </summary>
    public void Login(string wallet)
    {
        StopAllCoroutines();
        Logging(wallet);
    }
    /// <summary>
    /// Method <c>Registration</c> adding string to DB.
    /// </summary>
    public void Registration(string wallet)
    {
        StopAllCoroutines();
        Registering(wallet);
    }

    private bool CheckString(string toCheck)
    {
        toCheck = toCheck.Trim();
        if (toCheck.Length > 10 && toCheck.Length < 35) return true;
        Debug.Log("Error");
        return false;
    }

    public void SaveData(string id, string wallet, int lvl, int score)
    {
        StopAllCoroutines();
        SaveProgress(id, wallet, lvl, score);
    }

    /// <summary>
    /// Method <c>MintUpdate</c> removing all mints that UID is own.
    /// </summary>
    public void MintUpdate(int id)
    {
        UpdateMint(id);
    }

    /// <summary>
    /// Method <c>MintCheck</c> adding all mints that UID is own.
    /// </summary>
    public void MintCheck(int id, List<string> mints)
    {
        foreach (var mint in mints)
        {
            CheckMint(id, mint);
        }
    }

    public void GetScoreboard()
    {
        userData.playerData.score = "";
        getScoreboard();
    }

    public void GetScoreboardByIndex(int index)
    {
        getScoreboardByIndex(index);
    }

    private void Logging(string wallet)
    {
        var form = new WWWForm();
        form.AddField("type", RequestType.logging.ToString());
        form.AddField("wallet_address", wallet);
        StartCoroutine(SendData(form, RequestType.logging));
    }

    private void Registering(string wallet)
    {
        var form = new WWWForm();
        form.AddField("type", RequestType.register.ToString());
        form.AddField("wallet_address", wallet);
        StartCoroutine(SendData(form, RequestType.register));
    }

    public void SaveProgress(string id, string wallet, int lvl, int score)
    {
        var form = new WWWForm();
        form.AddField("type", RequestType.save.ToString());
        form.AddField("id", id);
        form.AddField("wallet_address", wallet);
        form.AddField("level", lvl);
        form.AddField("levelScore", score);
        StartCoroutine(SendData(form, RequestType.save));
    }
    
    private void UpdateMint(int id)
    {
        var form = new WWWForm();
        form.AddField("type", RequestType.mintUpdate.ToString());
        form.AddField("id", id);
        StartCoroutine(SendData(form, RequestType.mintUpdate));
    }

    private void CheckMint(int id, string mint)
    {
        var form = new WWWForm();
        form.AddField("type", RequestType.mintCheck.ToString());
        form.AddField("id", id);
        form.AddField("mint", mint);
        StartCoroutine(SendData(form, RequestType.mintCheck));
    }

    public void Participate(int id)
    {
        var form = new WWWForm();
        form.AddField("type", RequestType.participate.ToString());
        form.AddField("id", id);
        StartCoroutine(SendData(form, RequestType.participate));
    }

    public void SaveLevel(int id, int index, int score, int win, int loose, string time)
    {
        var form = new WWWForm();
        form.AddField("type", RequestType.saveLevel.ToString());
        form.AddField("id", id);
        form.AddField("index", index);
        form.AddField("score", score);
        form.AddField("win", win);
        form.AddField("loose", loose);
        form.AddField("time", time); 
        StartCoroutine(SendData(form, RequestType.saveLevel));
    }
    private void getScoreboard()
    {
        var form = new WWWForm();
        form.AddField("type", RequestType.getScoreboard.ToString());
        StartCoroutine(SendData(form, RequestType.getScoreboard));
    }

    private void getScoreboardByIndex(int index)
    {
        var form = new WWWForm();
        form.AddField("type", RequestType.getScoreboardByIndex.ToString());
        form.AddField("index", index);
        StartCoroutine(SendData(form, RequestType.getScoreboardByIndex));
    }
    private IEnumerator SendData(WWWForm form, RequestType type)
    {
        using (var www = UnityWebRequest.Post(targetURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var data = SetUserData(www.downloadHandler.text);
                if (!data.error.isError)
                {
                    if (type != RequestType.save)
                    {
                        userData = data;
                        if (type == RequestType.logging) OnLogged.Invoke();
                        if (type == RequestType.register) OnRegistered.Invoke();
                        if (type == RequestType.mintCheck) OnCheckMint.Invoke();
                        if (type == RequestType.mintUpdate) OnMintUpdate.Invoke();
                        if (type == RequestType.participate) OnParticipated.Invoke();
                        if (type == RequestType.saveLevel) OnLevelSaved.Invoke();
                        if (type == RequestType.getScoreboard) OnGetScoreboard.Invoke();
                        if (type == RequestType.getScoreboardByIndex) OnGetScoreboardByIndex.Invoke();
                    }
                }
                else
                {
                    userData = data;
                    OnError.Invoke();
                }
            }
        }
    }
}