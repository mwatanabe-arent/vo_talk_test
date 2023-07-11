using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Questions
{
    public string[] question;
}

public class QuestionList : MonoBehaviour
{
    public GameObject questionButtonPrefab;
    public void Setup(Questions questions)
    {
        foreach (var q in questions.question)
        {
            QuestionButton button = Instantiate(questionButtonPrefab, transform).GetComponent<QuestionButton>();
            button.SetQuestion(q);
            button.gameObject.SetActive(true);
        }
    }

    public void Setup(List<SelectButtonItem> list){
        foreach (var q in list)
        {
            QuestionButton button = Instantiate(questionButtonPrefab, transform).GetComponent<QuestionButton>();
            button.SetSelectButtonItem(q);
            button.gameObject.SetActive(true);
        }

    }


}
