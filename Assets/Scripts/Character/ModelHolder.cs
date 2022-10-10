using System.Collections.Generic;
using UnityEngine;

public class ModelHolder : MonoBehaviour
{
    public List<GameObject> allModels;

    [SerializeField] private int activeModelIdx;

    public void Start()
    {
        SetModelFromSettings();
    }

    private void SetModel(int idx)
    {
        for (var i = 0; i < allModels.Count; i++)
            if (i == idx)
                allModels[i].SetActive(true);
            else
                allModels[i].SetActive(false);
    }

    private void SetModelFromSettings()
    {
        if (LevelLoader.Instance.modelHolder != null)
        {
            GameObject CyberBlockNew =
                Instantiate(LevelLoader.Instance.NFTmodel, Vector3.zero, Quaternion.Euler(0, 180, 0));
            CyberBlockNew.transform.localScale = Vector3.one;
            CyberBlockNew.transform.SetParent(transform, false);
            CyberBlockNew.name = "SelectedCB";
            allModels.Add(CyberBlockNew);
            for (var i = 0; i < allModels.Count; i++)
                if (i == allModels.Count - 1)
                {
                    allModels[i].SetActive(true);
                }
                else
                    allModels[i].SetActive(false);
        } else SetModel(activeModelIdx);
    }

    public void NextModel()
    {
        activeModelIdx++;
        activeModelIdx %= allModels.Count;
        SetModel(activeModelIdx);
    }
}