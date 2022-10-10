using UnityEngine;

public delegate void ConnectionCallback(string pubkey);

public class PhantomBridge : MonoBehaviour
{
    private bool connected = false;

    private ConnectionCallback connectionCallback;

    private string pubkey;


    public void ConnectToWallet(ConnectionCallback connectionCallback)
    {
        this.connectionCallback = connectionCallback;
        WebGLPluginJS.ConnectToWallet();
    }

    public void WalletConnectionCallback(string key)
    {
        pubkey = key;
        if (connectionCallback != null) connectionCallback(pubkey);
  
    }

    public void Disconnect()
    {
        WebGLPluginJS.DisconnectWallet();
    }

    public void GetAllNfts()
    {
        WebGLPluginJS.GetAllNfts();
    }


    public void NFTFetchCallback(string result)
    {
        Debug.Log(result);
    }
}