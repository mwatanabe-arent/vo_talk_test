using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogame;
using UnityEngine.UI;
using TMPro;

public class PanelTalkList : UIPanel
{
    public TMP_InputField inputField;
    [SerializeField] private Button stampButton;
    [SerializeField] private Button submitButton;
    [SerializeField] private GameObject messageItemPrefab;
    [SerializeField] private Transform contentRoot;

    private string inputMessage;

    protected override void initialize()
    {
        submitButton.interactable = false;
        inputField.onEndEdit.AddListener((input) =>
        {
            inputMessage = input;
            submitButton.interactable = 0 < inputMessage.Length && inputMessage.Length < 40;
        });
        submitButton.onClick.AddListener(() =>
        {
            TalkBanner talkBanner = Instantiate(messageItemPrefab, contentRoot).GetComponent<TalkBanner>();
            talkBanner.Setup(inputMessage, true);
            inputField.text = "";
        });
    }




}
