using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProtocolDataRoot
{

}

[System.Serializable]
public class LoginData
{
    public InnerData data;
    [System.Serializable]
    public struct InnerData
    {
        public string token;
    }
}
