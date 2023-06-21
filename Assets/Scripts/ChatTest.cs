using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChatTest : MonoBehaviour
{
    private Communication communication;
    public static UnityEvent<TalkModel> OnResponse = new UnityEvent<TalkModel>();

    private void Start()
    {
        communication = new Communication();

        PanelTalkList.OnSendStampMessage.AddListener(() =>
        {

            BingSearchControl bingSearchControl = new BingSearchControl();

            bingSearchControl.GetNews((result) =>
            {
                int index = Random.Range(0, result.Count);

                string description = result[index].description;

                communication.Submit($"" +
                    $"あなたは私の良き友人です。" +
                    $"次のメッセージ内容は最近起こったニュースの内容です。" +
                    $"日常会話を開始するように話題を振ってください。" +
                    $"・{description}", (val) =>
                {
                    Debug.Log(val.content);

                    OnResponse?.Invoke(new TalkModel()
                    {
                        message = val.content,
                        isRight = false
                    });

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
