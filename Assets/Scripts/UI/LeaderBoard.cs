using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] private GameObject LoadingWheel;
    
    [SerializeField] private Transform content;
    [SerializeField] private GameObject Leaderboard;
    [SerializeField] private GameObject Prefab;
    private bool unpackedScoreboard = false;
    public List<GlobalTableElement> Elements;
    
    public void LoadLocalScoreboard()
    {
        HidePanel();
        GetComponent<LocalLeaderBoard>().ShowPanel();
    }
    public void ShowPanel()
    {
        Leaderboard.SetActive(true);
    }
    
    public void HidePanel()
    {
        Leaderboard.SetActive(false);
    }

    public void ScoreboardTab()
    {
        if (Leaderboard.activeSelf)
        {
            Leaderboard.SetActive(false);
        }
        else
        {
            UpdateLeaderboard();
        }
    }
    
    public void UpdateLeaderboard ()
    {
        HidePanel();
        LevelLoader.Instance.scoreboardLoaded = false;
        unpackedScoreboard = false;
        WebManager.WM.GetScoreboard();
        LoadingWheel.SetActive(true);
        StartCoroutine(LoadLeaderboard());
    }

    private void UnpackGlobalScoreboard(string unpacked_data)
    {
        Elements = new List<GlobalTableElement>();
        
        foreach (var data in unpacked_data.Split('|'))
        {
            string[] dataParms = data.Split('&');
            if (dataParms.Length > 1)
            {
                GlobalTableElement newTableElement = new GlobalTableElement(dataParms[0], dataParms[1]);
                Elements.Add(newTableElement);
                newTableElement.DebugGlobalTableElement();
            }
        }
        unpackedScoreboard = true;
    }

    private IEnumerator LoadLeaderboard()
    {
        yield return new WaitUntil(() => LevelLoader.Instance.scoreboardLoaded);
        UnpackGlobalScoreboard(LevelLoader.Instance.scoreboard);
        yield return new WaitUntil(() => unpackedScoreboard);
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        int placements = 0;
        foreach (var element in Elements)
        { 
            GameObject row = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
            
            placements++;
            row.GetComponent<GlobalScore_Row_Holder>().rank.text = placements.ToString();
            row.GetComponent<GlobalScore_Row_Holder>().wallet.text = element.wallet_address;
            //row.GetComponent<GlobalScore_Row_Holder>().userId.text = element.userID;
            if (LevelLoader.Instance.PlayerID.ToString() == element.userID)
            {
                row.GetComponent<GlobalScore_Row_Holder>().SetImageColor(new Color(141, 0, 255, 96));
                LevelLoader.Instance.placement = placements.ToString();
            }
            row.transform.SetParent(content, false);
            row.name = element.userID;
        }
        
        LoadingWheel.SetActive(false);
        ShowPanel();
    }
    
    //private IEnumerator

}
