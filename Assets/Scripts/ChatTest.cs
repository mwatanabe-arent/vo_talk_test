using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChatTest : MonoBehaviour
{
    private Communication communication;
    public static UnityEvent<TalkModel> OnResponse = new UnityEvent<TalkModel>();
    public static UnityEvent<TalkModel, BingSearchControl.NewsValue> OnResponseNews = new UnityEvent<TalkModel, BingSearchControl.NewsValue>();

    private void Start()
    {
        communication = new Communication();

        PanelTalkList.OnSendStampMessage.AddListener(() =>
        {

            BingSearchControl bingSearchControl = new BingSearchControl();

            bingSearchControl.GetNews((result) =>
            {
                int index = Random.Range(0, result.Count);

                var news = result[index];
                Debug.Log(news.description);
                communication.Submit($"" +
                    $"あなたは私の良き友人です。" +
                    $"次のメッセージ内容は最近起こったニュースの内容です。" +
                    $"日常会話を開始するように話題を振ってください。" +
                    $"・{news.description}", (val) =>
                {
                    Debug.Log(val.content);

                    OnResponseNews?.Invoke(new TalkModel()
                    {
                        message = val.content,
                        isRight = false
                    },
                    news);

                });
            });
        });
        PanelTalkList.OnSendTalkMessage.AddListener((send_message) =>
        {
            communication.Submit($"{send_message}", (val) =>
                {
                    Debug.Log(val.content);
                    OnResponse?.Invoke(new TalkModel()
                    {
                        message = val.content,
                        isRight = false
                    });
                });
        });
    }

}
