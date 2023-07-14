using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueID : Singleton<UniqueID>
{
    // GUIDを利用してIDを生成する.
    // 生成されたIDはPlayerPrefsに保存される.
    public string Get()
    {
        string key = "unique_id";
        string id = "";
        if (PlayerPrefs.HasKey(key) == false)
        {
            id = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(key, id);
            PlayerPrefs.Save();
        }
        else
        {
            id = PlayerPrefs.GetString(key);
        }

        return id;
    }
}
