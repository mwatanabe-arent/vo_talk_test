using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LoginData
{
    /*
    public InnerData data;
    [System.Serializable]
    public struct InnerData
    {
        public string token;
    }
    */
    public string token;

}

[System.Serializable]
public class ChatData
{
    public string message;
    /*
    public InnerData data;
    [System.Serializable]
    public struct InnerData
    {
        public int id;
        public string message;
        public int user_info;
        public string user_name;
        public string group_name;
        public string created_at;
        public string ai_response;
        public bool is_secret;
    }
    */
}

