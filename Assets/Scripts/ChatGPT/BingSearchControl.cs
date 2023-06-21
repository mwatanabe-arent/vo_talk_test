using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class BingSearchControl
{
    [System.Serializable]
    public class NewsValue
    {
        public string name;
        public string url;
        public string description;
        public string datePublished; // 時間
        public string category;
        public bool headline;
    }

    [System.Serializable]
    public class NewsRoot
    {
        public List<NewsValue> value;
    }

    public void GetNews(Action<List<NewsValue>> result)
    {
        var headers = new Dictionary<string, string>
            {
                {"Ocp-Apim-Subscription-Key", "c76e37c678bb4c05b92a324a26103df4"},
                {"Content-type", "application/json"},
                {"X-Slack-No-Retry", "1"}
            };
        var apiUrl = "https://api.bing.microsoft.com/v7.0/news/search";
        var request = new UnityWebRequest(apiUrl, "Get")
        {
            //uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        foreach (var header in headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        var operation = request.SendWebRequest();

        operation.completed += _ =>
        {
            if (operation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                       operation.webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                result.Invoke(null);
                Debug.LogError(operation.webRequest.error);
                throw new Exception();
            }
            else
            {
                var responseString = operation.webRequest.downloadHandler.text;
                var temp = JsonUtility.FromJson<NewsRoot>(request.downloadHandler.text);
                result.Invoke(temp.value);
            }
            request.Dispose();
        };

        /*
        Debug.Log(request.downloadHandler.text);
        var temp = JsonUtility.FromJson<NewsRoot>(request.downloadHandler.text);
        foreach (NewsValue v in temp.value)
        {
            Debug.Log(v.name);
        }
        */
    }

}
