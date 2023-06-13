using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginTest : MonoBehaviour
{
    [SerializeField] private Button submitButton;

    private string jwtToken;

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
        Debug.Log(auth.data.token);
        jwtToken = auth.data.token;

        PanelTalkList.OnSendTalkMessage.AddListener(SendTalkButton);

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
    }


}
