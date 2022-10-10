using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalLeaderBoard : MonoBehaviour
{
    [SerializeField] private GameObject LoadingWheel;
    [SerializeField] private Transform Content;
    [SerializeField] private GameObject Leaderboard;
    [SerializeField] private GameObject Prefab;
    [SerializeField] private TMP_Text LevelLable;
    private bool _unpackedScoreboard = false;
    public List<LocalTableElement> Elements;
    private int _currentIndex;

    public void LoadGlobalScoreboard()
    {
        HidePanel();
        GetComponent<LeaderBoard>().UpdateLeaderboard();
    }
    
    public void PreviousIndex()
    {
        _currentIndex = (_currentIndex - 1)  % 15;
        if (_currentIndex == -1) _currentIndex = 14;
        UpdateLeaderboardByIndex(_currentIndex);
    }

    public void NextIndex()
    {
        _currentIndex = (_currentIndex + 1)  % 15;
        UpdateLeaderboardByIndex(_currentIndex);
    }
    
    public void ShowPanel()
    {
        UpdateLeaderboardByIndex(0);
        _currentIndex = 0;
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
            UpdateLeaderboardByIndex(0);
        }
    }

    public void UpdateLeaderboardByIndex(int index)
    {
        HidePanel();
        LevelLoader.Instance.local_scoreboardLoaded = false;
        _unpackedScoreboard = false;
        WebManager.WM.GetScoreboardByIndex(index);
        LevelLable.text = "Level-" + (index + 1);
        Debug.Log("Level-" + index);
        LoadingWheel.SetActive(true);
        StartCoroutine(LoadLocalLeaderboard());
    }

    private void UnpackLocalScoreboard(string unpacked_data)
    {
        Debug.Log("UnpackLocal");
        Elements = new List<LocalTableElement>();
        
        foreach (var data in unpacked_data.Split('|'))
        {
            string[] dataParms = data.Split('&');
            if (dataParms.Length > 1)
            {
                LocalTableElement newTableElement = new LocalTableElement(dataParms[0], dataParms[1], dataParms[2], dataParms[3]);
                Elements.Add(newTableElement);
                newTableElement.DebugLocalTableElement();
            }
        }
        _unpackedScoreboard = true;
    }

    private IEnumerator LoadLocalLeaderboard()
    {
        Debug.Log("Coroutine");
        yield return new WaitUntil(() => LevelLoader.Instance.local_scoreboardLoaded);
        UnpackLocalScoreboard(LevelLoader.Instance.scoreboard);
        yield return new WaitUntil(() => _unpackedScoreboard);
        Debug.Log("UnpackComplete");
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }
        int placements = 0;
        bool GotScore = false;
        string placement = ""; 
        foreach (var element in Elements)
        {
            GameObject row = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
            placements++;
            row.GetComponent<LocalScore_Row_Holder>().rank.text = placements.ToString();
            row.GetComponent<LocalScore_Row_Holder>().wallet.text = element.wallet_address;
            row.GetComponent<LocalScore_Row_Holder>().best_score.text = element.best_score;
            row.GetComponent<LocalScore_Row_Holder>().best_time.text = element.best_time;
            //row.GetComponent<GlobalScore_Row_Holder>().userId.text = element.userID;
            Debug.Log("Wallet_address: " + element.wallet_address + "\nBestScore: " + element.best_score + "\nBestTime: ");
            
            LevelLoader.Instance.currentIndex = _currentIndex + 1;
            if (LevelLoader.Instance.PlayerID.ToString() == element.userID)
            {
                GotScore = true;
                row.GetComponent<LocalScore_Row_Holder>().SetImageColor(new Color(141, 0, 255, 96));
                placement = placements.ToString();
            }
            row.transform.SetParent(Content, false);
            row.name = element.userID;
        }

        if (GotScore)
        {
            LevelLoader.Instance.placement = placement;
        } else LevelLoader.Instance.placement = "!";
        LoadingWheel.SetActive(false);
        Leaderboard.SetActive(true);
    }
}
