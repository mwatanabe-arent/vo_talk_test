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
    public string role;
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
    [SerializeField] private GameObject linkBannerPrefab;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private Button clearButton;    // デバッグ用
    [SerializeField] private GameObject questionListPrefab;
    [SerializeField] private GameObject responseWait;
    [SerializeField] private Button youtubeButton;
    [SerializeField] private Button hatebuButton;

    private TalkHistory talkHistory;// = new TalkHistory();

    private string inputMessage;

    public static UnityEvent<string> OnSendTalkMessage = new UnityEvent<string>();
    public static UnityEvent OnSendStampMessage = new UnityEvent();
    public static UnityEvent OnSendSelectButton = new UnityEvent();

    public static UnityEvent OnClearButton = new UnityEvent();

    public static UnityEvent OnStartChatGPT = new UnityEvent();
    public static UnityEvent OnEndChatGPT = new UnityEvent();

    private void OnDestroy()
    {
        if (talkHistory != null)
        {
            string saveString = JsonUtility.ToJson(talkHistory);
            Debug.Log(saveString);
            if (0 < talkHistory.talkList.Count)
            {
                PlayerPrefs.SetString(Define.KEY_SAVED_MESSAGE, saveString);
                PlayerPrefs.Save();
            }
        }
    }

    protected override void initialize()
    {
        if (PlayerPrefs.HasKey(Define.KEY_SAVED_MESSAGE))
        {
            var json = PlayerPrefs.GetString(Define.KEY_SAVED_MESSAGE);
            talkHistory = JsonUtility.FromJson<TalkHistory>(json);
            Debug.Log(talkHistory.talkList.Count);
            foreach (TalkModel model in talkHistory.talkList)
            {
                AddMessage(model);
            }
        }
        else
        {
            talkHistory = new TalkHistory();
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
                isRight = true,
                role = "user",
            };
            talkBanner.Setup(model);
            talkHistory.talkList.Add(model);
            inputField.text = "";
            OnSendTalkMessage?.Invoke(inputMessage);
        });

        // 一旦閉鎖
        stampButton.interactable = false;
        stampButton.onClick.AddListener(() =>
        {
            OnSendStampMessage?.Invoke();
        });
        youtubeButton.onClick.AddListener(() =>
        {
            OnSendStampMessage?.Invoke();
        });
        hatebuButton.onClick.AddListener(() =>
        {
            OnSendSelectButton?.Invoke();
        });

        ChatControl.OnResponse.RemoveAllListeners();
        ChatControl.OnResponse.AddListener((val) =>
        {
            TalkBanner talkBanner = Instantiate(messageItemPrefab, contentRoot).GetComponent<TalkBanner>();
            talkBanner.Setup(val);
            talkHistory.talkList.Add(val);
        });
        ChatControl.OnQuestionRequest.AddListener(AddQuestionButtons);
        ChatControl.OnSelectRequest.AddListener(AddSelectButtons);


        LoginTest.OnResponse.RemoveAllListeners();
        LoginTest.OnResponse.AddListener((val) =>
        {
            TalkBanner talkBanner = Instantiate(messageItemPrefab, contentRoot).GetComponent<TalkBanner>();
            TalkModel model = new TalkModel()
            {
                message = val.message,
                isRight = false,
                role = "system",
            };
            talkBanner.Setup(model);
            talkHistory.talkList.Add(model);
        });
        clearButton.onClick.AddListener(() =>
        {
            ClearMessages();
            OnClearButton?.Invoke();
        });

        // デバッグ的な処理。とりあえず毎回クリア
        ClearMessages();

        ChatTest.OnResponse.AddListener(ChatTestAddMessage);
        ChatTest.OnResponseNews.AddListener(ChatTestAddNews);

        ChatTest.OnQuestionRequest.AddListener(AddQuestionButtons);

        responseWait.SetActive(false);

        OnStartChatGPT?.AddListener(() =>
        {
            responseWait.SetActive(true);
        });
        OnEndChatGPT?.AddListener(() =>
        {
            responseWait.SetActive(false);
        });
    }

    public void AddMessage(TalkModel model)
    {
        TalkBanner talkBanner = Instantiate(messageItemPrefab, contentRoot).GetComponent<TalkBanner>();
        talkBanner.Setup(model);
    }
    public void ChatTestAddMessage(TalkModel model)
    {
        AddMessage(model);
        talkHistory.talkList.Add(model);
    }
    public void ChatTestAddNews(TalkModel model, BingSearchControl.NewsValue news)
    {
        ChatTestAddMessage(model);
        LinkBanner linkBanner = Instantiate(linkBannerPrefab, contentRoot).GetComponent<LinkBanner>();
        linkBanner.SetUrl(news.url);
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

    private void AddQuestionButtons(Questions arg0)
    {
        QuestionList questionList = Instantiate(questionListPrefab, contentRoot).GetComponent<QuestionList>();

        questionList.Setup(arg0);
    }

    private void AddSelectButtons(List<SelectButtonItem> list){
        QuestionList questionList = Instantiate(questionListPrefab, contentRoot).GetComponent<QuestionList>();
        questionList.Setup(list);
    }

}
