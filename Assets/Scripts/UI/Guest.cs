using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (LevelLoader.Instance.GuestMode)
        {
            gameObject.SetActive(false);
        }
    }

}
