using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class PlayerSelectionMenu : MonoBehaviour
{
    public Vector3 firstPos;
    public float spacingBetweenModel;

    public float selectedModelSize;
    public float unselectedModelSize;

    public float modelSwitchDuration;

    public List<GameObject> models;
    public List<GameObject> duplicatedmodels;
    public List<GameObject> tempModels;

    public GameObject previousModel;
    public GameObject currentModel;
    public GameObject nextModel;
    public GameObject hiddenModel;
    [SerializeField] private bool playOnStart;
    [SerializeField] private int index;
    private bool modelOnlyOne;

    public Sequence moveSequence;

    public void Awake()
    {
        if (playOnStart) Initialise(); 
    }

    private void Start()
    {
        SelectCurrentModel();
    }

    public void Update()
    {
        if (!modelOnlyOne)
        {
            if (Input.GetAxisRaw("Horizontal") > 0.99)
            {
                Debug.Log("NextModel");
                //LevelLoader.Instance.SelectionAudio.Play();
                NextModel();
            }
            else if (Input.GetAxisRaw("Horizontal") < -0.99)
            {
                //LevelLoader.Instance.SelectionAudio.Play();
                Debug.Log("PreviousModel");
                PreviousModel();
            }
        }
        if (Input.GetKeyUp(KeyCode.Space)) //TODO
        {
            //SelectCurrentModel();
            //LevelLoader.Instance.SelectedModel = selectedModel;
        }
            
    }

    public void Initialise()
    {
        
        index = 0;
        currentModel = models[index];
        models[index].transform.position = firstPos;
        models[index].transform.localScale = Vector3.one * selectedModelSize;
        previousModel = null;
        nextModel = null;
        
        if (models.Count == 1) 
            modelOnlyOne = true;
        else 
            modelOnlyOne = false;
        
        if (models.Count == 2 || models.Count == 3)
        {
            duplicatedmodels = new List<GameObject>();
            var duplicatedModel = Instantiate(models[0], new Vector3(0f, -50f, 0f),
                Quaternion.Euler(new Vector3(349.5f,315f,10.5f)));
            duplicatedModel.transform.SetParent(gameObject.transform, false);
            
            duplicatedmodels.Add(duplicatedModel);
            duplicatedModel = Instantiate(models[1], new Vector3(0f, -50f, 0f),
                Quaternion.Euler(new Vector3(349.5f,315f,10.5f)));
            
            duplicatedModel.transform.SetParent(gameObject.transform, false);
            duplicatedmodels.Add(duplicatedModel);
            if (models.Count == 3)
            {
                duplicatedModel = Instantiate(models[2], new Vector3(0f, -50f, 0f),
                    Quaternion.Euler(new Vector3(349.5f,315f,10.5f)));
                duplicatedModel.transform.SetParent(gameObject.transform, false);
                duplicatedmodels.Add(duplicatedModel);
                
                nextModel = models[1];
                previousModel = models[models.Count - 1];
            }
            else
            {
                nextModel = models[1];
                previousModel = duplicatedmodels[1];
            }
        }
        else if (models.Count > 2)
        {
            nextModel = models[1];
            previousModel = models[models.Count - 1];
        }

        if (!modelOnlyOne)
        {
            nextModel.transform.position = firstPos + new Vector3(spacingBetweenModel, 0, 0);
            nextModel.transform.localScale = Vector3.one * unselectedModelSize;
            previousModel.transform.position = firstPos - new Vector3(spacingBetweenModel, 0, 0);
            previousModel.transform.localScale = Vector3.one * unselectedModelSize;
        }

        

        //for (var i = 0; i < models.Count; i++) models[i].GetComponent<Turntable>().Activate();
        //for (var i = 0; i < duplicatedmodels.Count; i++) duplicatedmodels[i].GetComponent<Turntable>().Activate();
        //NextModel();
    }

    public void UpdateModelList(List<GameObject> model)
    {
        models = new List<GameObject>();
        foreach (var obj in model)
        {
            obj.transform.rotation = Quaternion.Euler(new Vector3(349.5f,315f,10.5f));
            models.Add(obj);
        }
        Debug.Log("Models(PlayerSelectionMenu.Count: ): " + models.Count);
    }

    public void SelectCurrentModel()
    {
        GameObject selectedModel = Instantiate(currentModel, Vector3.zero, Quaternion.identity);
        selectedModel.transform.rotation = quaternion.Euler(0,-180,0);
        if (LevelLoader.Instance.modelHolder.transform.childCount > 0)
        {
            for (int i = 0; i < LevelLoader.Instance.modelHolder.transform.childCount; i++)
            {
                Destroy(LevelLoader.Instance.modelHolder.transform.GetChild(i).gameObject);
            }
        }

        selectedModel.transform.SetParent(LevelLoader.Instance.modelHolder.transform, false);
        LevelLoader.Instance.NFTmodel = selectedModel;

    }
    
    public void AddModelToList(GameObject model)
    {
        models.Add(model);
        Debug.Log("Added " + model.name + " model to list.\n" + "List current count: " + models.Count);
    }

    [ContextMenu("Next Model")]
    public void NextModel()
    {
        if (moveSequence != null) return;

        // Debug.Log("Next");

        index++;
        if (index >= models.Count) index = 0;

        StartMoveModels(true);
    }


    [ContextMenu("Previous Model")]
    public void PreviousModel()
    {
        if (moveSequence != null) return;

        //Debug.Log("Previous");

        index--;
        if (index < 0) index = models.Count - 1;

        StartMoveModels(false);
    }

    public void StartMoveModels(bool right)
    {
        if (moveSequence != null) return;

        var mySequence = DOTween.Sequence();
        moveSequence = mySequence;
        mySequence.Insert(0, currentModel.transform.DOScale(unselectedModelSize, modelSwitchDuration));
        if (right) //                                                                                                                                   ----------------------->
        {
            mySequence.Insert(0, previousModel.transform.DOScale(unselectedModelSize, modelSwitchDuration));
            mySequence.Insert(0, nextModel.transform.DOScale(selectedModelSize, modelSwitchDuration));
        }
        else //                                                                                                                                   <-----------------------
        {
            mySequence.Insert(0, previousModel.transform.DOScale(selectedModelSize, modelSwitchDuration));
            mySequence.Insert(0, nextModel.transform.DOScale(unselectedModelSize, modelSwitchDuration));
        }

        GameObject hiddenModel = null;

        if (models.Count == 2)
        {
            if (index == 1)
            {
                hiddenModel = duplicatedmodels[0];
                if (hiddenModel == previousModel || hiddenModel == nextModel || hiddenModel == currentModel)
                {
                    //  Debug.Log("Switch1");
                    hiddenModel = models[0];
                    Debug.Log(hiddenModel.name);
                }
                    
            }
            else
            {
                hiddenModel = duplicatedmodels[1];
                if (hiddenModel == previousModel || hiddenModel == nextModel || hiddenModel == currentModel)
                {
                    //  Debug.Log("Switch2");
                    hiddenModel = models[1];
                    Debug.Log(hiddenModel.name);
                }
                    
            }
        }
        else if (models.Count >= 3)
        {
            var iteration = 1;
            if (models.Count == 3) iteration = 1;
            var tempIndex = index;

            if (!right) //                                                                                                                                   <-----------------------
                for (var i = 0; i < iteration; i++)
                {
                    tempIndex--;
                    if (tempIndex < 0) tempIndex = models.Count - 1;
                }
            else //                                                                                                                                   ----------------------->
                for (var i = 0; i < iteration; i++)
                {
                    tempIndex++;
                    if (tempIndex >= models.Count) tempIndex = 0;
                }

            if (models.Count == 3)
            {
                hiddenModel = duplicatedmodels[tempIndex];
                if (hiddenModel == previousModel || hiddenModel == nextModel || hiddenModel == currentModel)
                {
                    //  Debug.Log("Switch1");
                    hiddenModel = models[tempIndex];
                    Debug.Log(hiddenModel.name);
                }
                    
            }
            else
            {
                hiddenModel = models[tempIndex];
            }
        }

        hiddenModel.transform.localScale = Vector3.one * unselectedModelSize;

        if (right) //                                                                                                                                   ----------------------->
        {
            hiddenModel.transform.position = firstPos + new Vector3(spacingBetweenModel, 0, 0) * 2;
            //hiddenModel.name = "HiddenOne";
            mySequence.Insert(0, hiddenModel.transform.DOMove(nextModel.transform.position, modelSwitchDuration));

            
            mySequence.Insert(0, currentModel.transform.DOMove(previousModel.transform.position, modelSwitchDuration));
            mySequence.Insert(0, nextModel.transform.DOMove(currentModel.transform.position, modelSwitchDuration));
            previousModel.transform.position -= new Vector3(spacingBetweenModel + 50, 0, 0);
            //previousModel.transform.
            previousModel = currentModel;
            currentModel = nextModel;
            nextModel = hiddenModel;
        }
        else //                                                                                                                                   <-----------------------
        {
            hiddenModel.transform.position = firstPos - new Vector3(spacingBetweenModel, 0, 0) * 2;
            
            mySequence.Insert(0, hiddenModel.transform.DOMove(previousModel.transform.position, modelSwitchDuration));

            mySequence.Insert(0, currentModel.transform.DOMove(nextModel.transform.position, modelSwitchDuration));
            mySequence.Insert(0, previousModel.transform.DOMove(currentModel.transform.position, modelSwitchDuration));
            nextModel.transform.position += new Vector3(spacingBetweenModel + 50, 0, 0);

            nextModel = currentModel;
            currentModel = previousModel;
            previousModel = hiddenModel;
        }

        mySequence.OnComplete(ResetSequence);
        LevelLoader.Instance.SelectionAudio.Play();
        SelectCurrentModel();
    }

    public void SetNewModelsTarget(bool right)
    {
    }

    public void ResetSequence()
    {
        moveSequence = null;
    }
}