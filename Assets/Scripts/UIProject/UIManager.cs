using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogame;
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject talkListPrefab;
    private void Start()
    {
        UIController.Instance.AddPanel(talkListPrefab);
    }
}
