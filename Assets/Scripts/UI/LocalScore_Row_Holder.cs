using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class LocalScore_Row_Holder : MonoBehaviour
{
    public TMP_Text rank;
    public TMP_Text wallet;
    public TMP_Text best_score;
    public TMP_Text best_time;
    [SerializeField] private GameObject userField;

    public void SetImageColor(Color color)
    {
        userField.GetComponent<Image>().color = color;
    }
}