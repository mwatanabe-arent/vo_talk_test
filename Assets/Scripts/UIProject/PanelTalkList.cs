using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using anogame;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TalkModel
{
    public string message;
    public bool isRight;
}

[System.Serializable]
public class TalkHistory
{
    public List<TalkModel> talkList = new List<TalkModel>();
}

public class PanelTalkList : UIPanel
{
    public TMP_InputField inputField;
    [SerializeField] private Button stampButton;
    [SerializeField] private Button submitButton;
    [SerializeField] private GameObject messageItemPrefab;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private Button clearButton;    // デバッグ用

    private TalkHistory talkHistory;// = new TalkHistory();

    private string inputMessage;

    public static UnityEvent<string> OnSendTalkMessage = new UnityEvent<string>();
    public static UnityEvent OnSendStampMessage = new UnityEvent();

    private void OnDestroy()
    {
        if (talkHistory != null)
        {
            string saveString = JsonUtility.ToJson(talkHistory);
            Debug.Log(saveString);
            PlayerPrefs.SetString(Define.KEY_SAVED_MESSAGE, saveString);
            PlayerPrefs.Save();
        }
    }

    protected override void initialize()
    {
        if (PlayerPrefs.HasKey(Define.KEY_SAVED_MESSAGE))
        {
            var json = PlayerPrefs.GetString(Define.KEY_SAVED_MESSAGE);
            talkHistory = JsonUtility.FromJson<TalkHistory>(json);
            Debug.Log(talkHistory.talkList.Count);
        }
        else
        {
            talkHistory = new TalkHistory();
            talkHistory.talkList.Add(new TalkModel() { isRight = true, message = "メッセージ１" });
            talkHistory.talkList.Add(new TalkModel() { isRight = false, message = "メッセージ2" });
        }

        foreach (TalkModel model in talkHistory.talkList)
        {
            AddMessage(model);
        }

        submitButton.interactable = false;
        inputField.onEndEdit.AddListener((input) =>
        {
            inputMessage = input;
            submitButton.interactable = 0 < inputMessage.Length && inputMessage.Length < 40;
        });
        submitButton.onClick.AddListener(() =>
        {
            TalkBanner talkBanner = Instantiate(messageItemPrefab, contentRoot).GetComponent<TalkBanner>();

            TalkModel model = new TalkModel()
            {
                message = inputMessage,
                isRight = true
            };
            talkBanner.Setup(model);
            talkHistory.talkList.Add(model);
            inputField.text = "";
            OnSendTalkMessage?.Invoke(inputMessage);
        });

        stampButton.onClick.AddListener(() =>
        {
            OnSendStampMessage?.Invoke();
        });

        LoginTest.OnResponse.RemoveAllListeners();
        LoginTest.OnResponse.AddListener((val) =>
        {
            TalkBanner talkBanner = Instantiate(messageItemPrefab, contentRoot).GetComponent<TalkBanner>();
            TalkModel model = new TalkModel()
            {
                message = val.message,
                isRight = false
            };
            talkBanner.Setup(model);
            talkHistory.talkList.Add(model);
        });

        clearButton.onClick.AddListener(ClearMessages);
    }

    public void AddMessage(TalkModel model)
    {
        TalkBanner talkBanner = Instantiate(messageItemPrefab, contentRoot).GetComponent<TalkBanner>();
        talkBanner.Setup(model);
    }

    private void ClearMessages()
    {
        var deleteList = new List<GameObject>();
        foreach (Transform child in contentRoot)
        {
            if (child != contentRoot)
            {
                deleteList.Add(child.gameObject);
            }
            else
            {
                Debug.Log("contentRoot!!");
            }
        }
        foreach (var obj in deleteList)
        {
            Destroy(obj);
        }
        PlayerPrefs.DeleteKey(Define.KEY_SAVED_MESSAGE);
        PlayerPrefs.Save();

        talkHistory = new TalkHistory();
    }


}
