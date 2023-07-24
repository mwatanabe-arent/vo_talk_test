using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class QuestionMessage
{
    public string message;
    //public Question question;
    public string question_json;
}

public class ChatControlYoutube : ChatControl
{
    private HatebuRequest hatebuRequest;

    public override void Start()
    {
        hatebuRequest = GetComponent<HatebuRequest>();
        base.Start();
        //StartCoroutine(WebRequestTest());
    }

    private IEnumerator WebRequestTest()
    {
        //UnityWebRequest request = UnityWebRequest.Get(Define.ENDPOINT + "api/youtube");
        UnityWebRequest request = UnityWebRequest.Get(Define.ENDPOINT + "api/chat/?message=ここに質問を入れてください");
        yield return request.SendWebRequest();
        Debug.Log(request.downloadHandler.text);

        var questionMessage = JsonUtility.FromJson<QuestionMessage>(request.downloadHandler.text);
        Debug.Log(questionMessage.message);
        OnResponse?.Invoke(new TalkModel()
        {
            message = questionMessage.message,
            isRight = false,
            role = "system",
        });
        //Debug.Log(questionMessage.question);
        var questions = JsonUtility.FromJson<Questions>(questionMessage.question_json);
        foreach (var question in questions.question)
        {
            Debug.Log(question);
        }
        OnQuestionRequest?.Invoke(questions);
    }

    protected override void SimpleTalk(string message)
    {
        Debug.Log("ChatControlYoutube.SimpleTalk");
        StartCoroutine(QuestionTalk(message));
    }

    private IEnumerator QuestionTalk(string message)
    {
        OnTalkStart?.Invoke();
        PanelTalkList.OnStartChatGPT.Invoke();

        OnResponse?.Invoke(new TalkModel()
        {
            message = message,
            isRight = true,
            role = "user",
        });

        UnityWebRequest request = UnityWebRequest.Get(Define.ENDPOINT + $"api/chat/?message={message}");
        yield return request.SendWebRequest();
        Debug.Log(request.downloadHandler.text);

        var questionMessage = JsonUtility.FromJson<QuestionMessage>(request.downloadHandler.text);
        Debug.Log(questionMessage.message);
        OnResponse?.Invoke(new TalkModel()
        {
            message = questionMessage.message,
            isRight = false,
            role = "system",
        });

        var questions = JsonUtility.FromJson<Questions>(questionMessage.question_json);
        foreach (var question in questions.question)
        {
            Debug.Log(question);
        }
        OnQuestionRequest?.Invoke(questions);
        PanelTalkList.OnEndChatGPT.Invoke();
    }

    protected override void SelectAction()
    {
        OnTalkStart?.Invoke();
        PanelTalkList.OnStartChatGPT.Invoke();
        Debug.Log("ChatControlYoutube.SelectAction");
        StartCoroutine(hatebuRequest.GetHatebuTalk((response) =>
        {
            var questionMessage = JsonUtility.FromJson<QuestionMessage>(response);
            Debug.Log(questionMessage.message);
            OnResponse?.Invoke(new TalkModel()
            {
                message = questionMessage.message,
                isRight = false,
                role = "system",
            });

            var questions = JsonUtility.FromJson<Questions>(questionMessage.question_json);
            foreach (var question in questions.question)
            {
                Debug.Log(question);
            }
            OnQuestionRequest?.Invoke(questions);
            PanelTalkList.OnEndChatGPT.Invoke();
        }));
    }

    protected override void SelectTalk(SelectButtonItem item)
    {
        Debug.Log("ChatControlYoutube.SelectTalk");
        Debug.Log(item.title);
        StartCoroutine(SelectTalkCoroutine(item));
    }

    private IEnumerator SelectTalkCoroutine(SelectButtonItem item)
    {
        PanelTalkList.OnStartChatGPT.Invoke();

        OnResponse?.Invoke(new TalkModel()
        {
            message = item.title,
            isRight = true,
            role = "user",
        });

        //http://localhost:8000/api/hatebu/url/?url=ht

        string build_url = Define.ENDPOINT + $"api/hatebu/url?url={UnityWebRequest.EscapeURL(item.link)}";
        Debug.Log(build_url);

        UnityWebRequest request = UnityWebRequest.Get(build_url);
        yield return request.SendWebRequest();
        Debug.Log(request.downloadHandler.text);

        var questionMessage = JsonUtility.FromJson<QuestionMessage>(request.downloadHandler.text);
        Debug.Log(questionMessage.message);
        OnResponse?.Invoke(new TalkModel()
        {
            message = questionMessage.message,
            isRight = false,
            role = "system",
        });

        var questions = JsonUtility.FromJson<Questions>(questionMessage.question_json);
        foreach (var question in questions.question)
        {
            Debug.Log(question);
        }
        OnQuestionRequest?.Invoke(questions);
        PanelTalkList.OnEndChatGPT.Invoke();
    }


}

