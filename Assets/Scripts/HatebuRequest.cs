using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class SelectButtonItem
{
    public string title;
    public string link;
    public string category;
}

[System.Serializable]
public class HatebuItem : SelectButtonItem
{
}

public class HatebuRequest : MonoBehaviour
{
    /*
    IEnumerator Start()
    {
        yield return StartCoroutine(GetHatebuItems((list) => {
            foreach (var item in list)
            {
                Debug.Log(item.title);
            }
        }));
    }
    */

    public IEnumerator GetHatebuTalk(Action<string> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(Define.ENDPOINT + "api/hatebu/character");
        yield return request.SendWebRequest();
        Debug.Log(request.downloadHandler.text);
        callback.Invoke(request.downloadHandler.text);
    }

    public IEnumerator GetHatebuItems(Action<List<SelectButtonItem>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(Define.ENDPOINT + "api/hatebu");
        yield return request.SendWebRequest();
        Debug.Log(request.downloadHandler.text);
        string cleanedJsonString = request.downloadHandler.text.Replace("\\n", "").Replace("\\", "");
        Debug.Log(cleanedJsonString);
        List<SelectButtonItem> list = JsonHelper.FromJson<SelectButtonItem>(cleanedJsonString).ToList();
        callback.Invoke(list);
    }

}
