using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginTest : MonoBehaviour
{
    [SerializeField] private Button submitButton;
    private string jwtToken;

    public static UnityEvent<ChatData> OnResponse = new UnityEvent<ChatData>();

    IEnumerator Start()
    {
        var postData = new Dictionary<string, string>()
        {
            { "username","user1" },
            { "password","user1" },
        };
        UnityWebRequest request = UnityWebRequest.Post("http://localhost:8000/api/login", postData);

        yield return request.SendWebRequest();

        Debug.Log(request.downloadHandler.text);

        var auth = JsonUtility.FromJson<LoginData>(request.downloadHandler.text);

        //Debug.Log(auth);
        Debug.Log(auth.token);
        jwtToken = auth.token;

        //PanelTalkList.OnSendTalkMessage.AddListener(SendTalkButton);
        PanelTalkList.OnSendTalkMessage.AddListener(InitiatorTestButton);
    }

    public void SendTalkButton(string sendMessage)
    {
        StartCoroutine(SendTalkMessage(sendMessage));
    }

    private IEnumerator SendTalkMessage(string message)
    {
        var postDataChat = new Dictionary<string, string>()
        {
            { "message",$"{message}" },
        };
        UnityWebRequest requestChat = UnityWebRequest.Post("http://localhost:8000/api/chat/1", postDataChat);
        requestChat.SetRequestHeader("Authorization", $"jwt {jwtToken}");
        yield return requestChat.SendWebRequest();

        Debug.Log(requestChat.downloadHandler.text);

        var chatData = JsonUtility.FromJson<ChatData>(requestChat.downloadHandler.text);
        OnResponse?.Invoke(chatData);
    }

    public void InitiatorTestButton(string sendMessage)
    {
        StartCoroutine(InitiatorTest());
    }
    IEnumerator InitiatorTest()
    {
        UnityWebRequest requestChat = UnityWebRequest.Get("http://localhost:8000/api/chatinitiator/get");
        requestChat.SetRequestHeader("Authorization", $"jwt {jwtToken}");
        yield return requestChat.SendWebRequest();

        Debug.Log(requestChat.downloadHandler.text);
        var chatData = JsonUtility.FromJson<ChatData>(requestChat.downloadHandler.text);
        OnResponse?.Invoke(chatData);
    }



}
