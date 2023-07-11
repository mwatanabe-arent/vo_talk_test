using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;



public class ChatControl : MonoBehaviour
{
    public static UnityEvent<TalkModel> OnResponse = new UnityEvent<TalkModel>();
    public static UnityEvent<Questions> OnQuestionRequest = new UnityEvent<Questions>();
    public static UnityEvent<List<SelectButtonItem>> OnSelectRequest = new UnityEvent<List<SelectButtonItem>>();
    public virtual void Start(){
        PanelTalkList.OnSendTalkMessage.AddListener(SimpleTalk);
        communication = new Communication("あなたはサポートロボットのロールプレイをしてください");

        // 質問ボタンが押された時も普通に質問をする
        QuestionButton.OnQuestionButton.AddListener(SimpleTalk);
        QuestionButton.OnSelectButton.AddListener(SelectTalk);

        PanelTalkList.OnSendStampMessage.AddListener(StampAction);

        PanelTalkList.OnSendSelectButton.AddListener(SelectAction);
    }


    protected Communication communication;

    protected virtual void SimpleTalk(string message)
    {
        PanelTalkList.OnStartChatGPT.Invoke();
        communication.Submit($"{message}", (val) =>
        {
            Debug.Log(val.content);
            OnResponse?.Invoke(new TalkModel()
            {
                message = val.content,
                isRight = false,
                role = "system",
            });
            PanelTalkList.OnEndChatGPT.Invoke();

        });
        //StartCoroutine(SimpleTalkCoroutine(message));
    }
    protected virtual void SelectTalk(SelectButtonItem item)
    {
        SimpleTalk(item.title);
    }

    protected void StampAction()
    {
        Debug.Log("StampAction");
        StartCoroutine(StampCoroutine());
    }
    protected virtual void SelectAction()
    {
        Debug.Log("SelectAction");
    }
    protected virtual IEnumerator StampCoroutine()
    {
        PanelTalkList.OnStartChatGPT.Invoke();
        UnityWebRequest request = UnityWebRequest.Get(Define.ENDPOINT + "api/youtube");
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
        PanelTalkList.OnEndChatGPT.Invoke();
    }

    protected IEnumerator GetJson(string api, Action<string> callback){
        UnityWebRequest request = UnityWebRequest.Get(Define.ENDPOINT + api);
        yield return request.SendWebRequest();
        Debug.Log(request.downloadHandler.text);
        callback(request.downloadHandler.text);
    }

}