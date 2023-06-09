using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoginTest : MonoBehaviour
{
    IEnumerator Start()
    {
        var postData = new Dictionary<string, string>()
        {
            { "username","test" },
            { "password","test" },
        };
        UnityWebRequest request = UnityWebRequest.Post("http://localhost:8000/api/login/", postData);

        yield return request.SendWebRequest();

        Debug.Log(request.downloadHandler.text);

        var auth = JsonUtility.FromJson<LoginData>(request.downloadHandler.text);

        Debug.Log(auth.token);

        var postDataChat = new Dictionary<string, string>()
        {
            { "message","Ç≤ã@åôÇ¢Ç©Ç™Ç≈Ç∑Ç©ÅH" },
        };
        UnityWebRequest requestChat = UnityWebRequest.Post("http://localhost:8000/api/chat/", postDataChat);
        requestChat.SetRequestHeader("Authorization", $"jwt {auth.token}");
        yield return requestChat.SendWebRequest();

        Debug.Log(requestChat.downloadHandler.text);


    }
    private struct LoginData
    {
        public string token;
    }

}
