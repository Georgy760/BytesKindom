using UnityEngine;

public class GlobalTableElement 
{
    public string userID;
    public string wallet_address;

    public GlobalTableElement (string id, string address)
    {
        userID = id;
        wallet_address = address;
    }

    public void DebugGlobalTableElement()
    {
        Debug.Log("UserID: " + userID + "\n" + "Wallet address: " + wallet_address);
    }
}

public class LocalTableElement
{
    public string userID;
    public string wallet_address;
    public string best_score;
    public string best_time;
    public LocalTableElement (string id, string address, string bestScore, string bestTime)
    {
        userID = id;
        wallet_address = address;
        best_score = bestScore;
        best_time = bestTime;
    }

    public void DebugLocalTableElement()
    {
        Debug.Log("UserID: " + userID + "\nWallet address: " + wallet_address
                  +  "\nBest score: " + best_score + "\nBest time:" + best_time);
    } 
}
