using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatTest : MonoBehaviour
{
    private Communication communication;
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
                });
            });


        });
    }

}
