using SimpleJSON;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    [SerializeField] private string publicKey;

    public JSONNode bloksJSON;

    // Start is called before the first frame update
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //
            Destroy(this);
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SetPublicKey(string publicKey)
    {
        this.publicKey = publicKey;
    }

    public void StoreBloksInfo(JSONNode json)
    {
        bloksJSON = json;
    }
}