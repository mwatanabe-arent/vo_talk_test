using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class QuestionButton : MonoBehaviour
{
    public static UnityEvent<string> OnQuestionButton = new UnityEvent<string>();

    private string questionMessage;

    public void SetQuestion(string message)
    {
        questionMessage = message;
        transform.GetComponentInChildren<TextMeshProUGUI>().text = message;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnQuestionButton?.Invoke(questionMessage);
        });
    }


}
