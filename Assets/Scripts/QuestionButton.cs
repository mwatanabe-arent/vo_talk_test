﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class QuestionButton : MonoBehaviour
{
    public static UnityEvent<string> OnQuestionButton = new UnityEvent<string>();
    public static UnityEvent<SelectButtonItem> OnSelectButton = new UnityEvent<SelectButtonItem>();

    public SelectButtonItem selectButtonItem;

    private string questionMessage;

    public void SetQuestion(string message)
    {
        questionMessage = message;
        transform.GetComponentInChildren<TextMeshProUGUI>().text = message;
    }

    public void SetSelectButtonItem(SelectButtonItem item){
        selectButtonItem = item;
        transform.GetComponentInChildren<TextMeshProUGUI>().text = selectButtonItem.title;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if(selectButtonItem != null){
                OnSelectButton?.Invoke(selectButtonItem);
            }
            else{
                OnQuestionButton?.Invoke(questionMessage);
            }
        });
    }


}
