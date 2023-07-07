using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
/*
[System.Serializable]
public class Question{
    public string[] questions;
}
*/

[System.Serializable]
public class QuestionMessage{
    public string message;
    //public Question question;
    public string question_json;
}

public class ChatControlYoutube : ChatControl
{

    public override void Start()
    {
        base.Start();
        //StartCoroutine(WebRequestTest());
    }

    private IEnumerator WebRequestTest(){
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
        foreach(var question in questions.question){
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
        foreach(var question in questions.question){
            Debug.Log(question);
        }
        OnQuestionRequest?.Invoke(questions);
        PanelTalkList.OnEndChatGPT.Invoke();
    }


}

