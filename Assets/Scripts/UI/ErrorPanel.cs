using TMPro;
using UnityEngine;

public class ErrorPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void SetText(string newtext)
    {
        text.text = newtext;
    }
}