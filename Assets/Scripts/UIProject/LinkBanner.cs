using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkBanner : MonoBehaviour
{
    [SerializeField] private Button button;

    private string url = "https://google.com";

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            Application.OpenURL(url);
        });
    }

    public void SetUrl(string url)
    {
        this.url = url;
    }
}
