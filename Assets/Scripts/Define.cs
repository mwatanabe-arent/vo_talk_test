using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public static readonly string KEY_SAVED_MESSAGE = "saved_message";
#if UNITY_EDITOR
    public static readonly string ENDPOINT = "http://localhost:8000/";
#else
    public static readonly string ENDPOINT = "http://13.115.179.135:8000/";
#endif
}
