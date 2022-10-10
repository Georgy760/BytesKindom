using System.Runtime.InteropServices;


public class WebGLPluginJS
{

    [DllImport("__Internal")]
    public static extern void ConnectToWallet();
   
    [DllImport("__Internal")]
    public static extern void DisconnectWallet();
   
    [DllImport("__Internal")]
    public static extern string GetAllNfts();


  
}

