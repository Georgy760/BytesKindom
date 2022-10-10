using System.Collections;
using System.Text;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

public delegate void NFTFetchCallbackDelegate(JSONNode parameter);

public class ServerTalker : MonoBehaviour
{
    public string testAddress = "F9NrBRut6Z1EaB9YBaCnghv8Eouj8jM1swAvY9onwdmf";


    public void FetchCyberBloks(string pubkey, NFTFetchCallbackDelegate callback)
    {
        StartCoroutine(FetchCyberBloksCoroutine(pubkey, callback));
    }

    private IEnumerator FetchCyberBloksCoroutine(string pubkey, NFTFetchCallbackDelegate callback)
    {
        var form = new WWWForm();
        form.AddField("pubkey", pubkey);

        var www = UnityWebRequest.Post("http://localhost:8000/API/fetchNFTS", form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            Debug.Log("POST successful!");
            var sb = new StringBuilder();
            foreach (var dict in www.GetResponseHeaders())
                sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");

            var metadataJSONArray = JSON.Parse(www.downloadHandler.text);
            callback(metadataJSONArray);
        }

        www.Dispose();
    }

    public void testCallback(JSONNode metadataJSONArray)
    {
        Debug.Log(metadataJSONArray[0]["attributes"]["attributes"]);
    }

    public void testFetch()
    {
        FetchCyberBloks(testAddress, testCallback);
    }
}